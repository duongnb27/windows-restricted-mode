using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace RestrictedMode
{
    /// <summary>
    /// Hide / disable the Start Menu on Windows 10 and 11.
    /// Four layered strategies (most → least aggressive):
    ///   1. Suspend StartMenuExperienceHost.exe so the flyout cannot open at all
    ///   2. Win32 – find and hide/disable the "Start" button window on every taskbar
    ///   3. Watcher – periodically re-suspend the host if Windows restarts it, and dismiss any visible flyout
    ///   4. Registry – HKCU NoStartMenu policy (legacy / pre-Win11)
    /// No Explorer restart is needed; all approaches take effect immediately.
    /// </summary>
    public static class StartMenuPolicy
    {
        #region Win32

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const uint PROCESS_SUSPEND_RESUME = 0x0800;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("ntdll.dll")]
        private static extern uint NtSuspendProcess(IntPtr processHandle);

        [DllImport("ntdll.dll")]
        private static extern uint NtResumeProcess(IntPtr processHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        #endregion

        private const string RegistryPolicyPath =
            @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        private const string RegistryValueName = "NoStartMenu";

        private static Timer _watchTimer;
        private static volatile bool _isHidden;
        private static readonly object _lock = new object();
        private static readonly HashSet<int> _suspendedPids = new HashSet<int>();

        public static bool Hide()
        {
            _isHidden = true;
            SetRegistryPolicy(true);
            SetStartButtons(false);
            SuspendStartMenuHost();
            DismissVisibleStartMenu();
            StartWatcher();
            return true;
        }

        public static bool Show()
        {
            _isHidden = false;
            StopWatcher();
            ResumeStartMenuHost();
            SetStartButtons(true);
            SetRegistryPolicy(false);
            return true;
        }

        public static bool IsHidden() => _isHidden;

        #region Strategy 1 – Suspend StartMenuExperienceHost process

        private static void SuspendStartMenuHost()
        {
            lock (_lock)
            {
                try
                {
                    foreach (var p in Process.GetProcessesByName("StartMenuExperienceHost"))
                    {
                        int pid = p.Id;
                        p.Dispose();

                        if (_suspendedPids.Contains(pid))
                            continue;

                        IntPtr handle = OpenProcess(PROCESS_SUSPEND_RESUME, false, pid);
                        if (handle != IntPtr.Zero)
                        {
                            if (NtSuspendProcess(handle) == 0)
                                _suspendedPids.Add(pid);
                            CloseHandle(handle);
                        }
                    }
                }
                catch { }
            }
        }

        private static void ResumeStartMenuHost()
        {
            lock (_lock)
            {
                try
                {
                    foreach (var p in Process.GetProcessesByName("StartMenuExperienceHost"))
                    {
                        int pid = p.Id;
                        p.Dispose();

                        IntPtr handle = OpenProcess(PROCESS_SUSPEND_RESUME, false, pid);
                        if (handle != IntPtr.Zero)
                        {
                            NtResumeProcess(handle);
                            CloseHandle(handle);
                        }
                    }
                }
                catch { }
                _suspendedPids.Clear();
            }
        }

        #endregion

        #region Strategy 2 – Win32: hide / disable the Start button window

        private static void SetStartButtons(bool show)
        {
            foreach (IntPtr btn in GetStartButtonHandles())
            {
                if (show)
                {
                    ShowWindow(btn, SW_SHOW);
                    EnableWindow(btn, true);
                }
                else
                {
                    EnableWindow(btn, false);
                    ShowWindow(btn, SW_HIDE);
                }
            }
        }

        private static List<IntPtr> GetStartButtonHandles()
        {
            var list = new List<IntPtr>();

            IntPtr taskbar = FindWindow("Shell_TrayWnd", null);
            if (taskbar != IntPtr.Zero)
            {
                IntPtr start = FindWindowEx(taskbar, IntPtr.Zero, "Start", null);
                if (start != IntPtr.Zero)
                    list.Add(start);
            }

            EnumWindows((hWnd, lParam) =>
            {
                var sb = new StringBuilder(256);
                if (GetClassName(hWnd, sb, sb.Capacity) > 0 &&
                    string.Equals(sb.ToString(), "Shell_SecondaryTrayWnd", StringComparison.Ordinal))
                {
                    IntPtr start = FindWindowEx(hWnd, IntPtr.Zero, "Start", null);
                    if (start != IntPtr.Zero)
                        list.Add(start);
                }
                return true;
            }, IntPtr.Zero);

            return list;
        }

        #endregion

        #region Strategy 3 – Watcher: re-suspend host + dismiss visible flyout

        private static void StartWatcher()
        {
            StopWatcher();
            _watchTimer = new Timer(WatcherTick, null, 0, 500);
        }

        private static void StopWatcher()
        {
            var t = _watchTimer;
            _watchTimer = null;
            t?.Dispose();
        }

        private static void WatcherTick(object state)
        {
            if (!_isHidden) return;
            try
            {
                SuspendStartMenuHost();
                DismissVisibleStartMenu();
            }
            catch { }
        }

        private static void DismissVisibleStartMenu()
        {
            try
            {
                foreach (var p in Process.GetProcessesByName("StartMenuExperienceHost"))
                {
                    uint targetPid = (uint)p.Id;
                    p.Dispose();

                    EnumWindows((hWnd, lParam) =>
                    {
                        if (!IsWindowVisible(hWnd)) return true;

                        GetWindowThreadProcessId(hWnd, out uint windowPid);
                        if (windowPid != targetPid) return true;

                        ShowWindow(hWnd, SW_HIDE);
                        return true;
                    }, IntPtr.Zero);
                    break;
                }
            }
            catch { }
        }

        #endregion

        #region Strategy 4 – Registry policy (legacy / pre-Win11)

        private static void SetRegistryPolicy(bool hide)
        {
            try
            {
                if (hide)
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(RegistryPolicyPath, writable: true))
                    {
                        key?.SetValue(RegistryValueName, 1, RegistryValueKind.DWord);
                    }
                }
                else
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(RegistryPolicyPath, writable: true))
                    {
                        key?.DeleteValue(RegistryValueName, throwOnMissingValue: false);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
