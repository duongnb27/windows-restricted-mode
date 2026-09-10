using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace RestrictedMode
{
    /// <summary>
    /// Blocks Windows edge-swipe system UI (Notification Center / Action Center, Task View, tablet edge UI)
    /// Uses a machine policy for edge swipe and a user policy for Notification Center.
    /// Administrator rights are required. Reboot after deployment to activate all policies.
    /// </summary>
    public static class EdgeSwipePolicy
    {
        public static string LastError { get; private set; }
        public static string LogPath => Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "RestrictedMode", "policy.log");

        private static void LogError(string operation, Exception error)
        {
            string message = operation + ": " + error.Message;
            LastError = string.IsNullOrEmpty(LastError) ? message : LastError + Environment.NewLine + message;
            Trace.TraceError(message);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                File.AppendAllText(LogPath, DateTimeOffset.Now.ToString("o") + " " + operation +
                    Environment.NewLine + error + Environment.NewLine);
            }
            catch (Exception logError) { Trace.TraceError("Cannot write policy log: " + logError); }
        }

        private static RegistryKey OpenMachineRegistry()
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
        }

        // Snapshots belong to this restricted session, not to each Start click.
        private sealed class PolicyValue
        {
            public readonly RegistryHive Hive;
            public readonly string Path;
            public readonly string Name;
            public readonly int Desired;
            private bool captured, changed, existed;
            private object original;
            private RegistryValueKind kind;

            public PolicyValue(RegistryHive hive, string path, string name, int desired)
            { Hive = hive; Path = path; Name = name; Desired = desired; }

            private RegistryKey OpenRoot()
            {
                return RegistryKey.OpenBaseKey(Hive, Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64 : RegistryView.Registry32);
            }

            public bool Apply()
            {
                try
                {
                    using (var root = OpenRoot())
                    {
                        using (var key = root.OpenSubKey(Path))
                        {
                            object current = key?.GetValue(Name, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                            if (!captured)
                            {
                                existed = current != null;
                                original = current;
                                if (existed) kind = key.GetValueKind(Name);
                                captured = true;
                            }
                            if (current is int && (int)current == Desired &&
                                key.GetValueKind(Name) == RegistryValueKind.DWord) return true;
                        }
                        using (var key = root.CreateSubKey(Path, true))
                        {
                            if (key == null) throw new IOException("Cannot open policy key.");
                            // Retain snapshot even if a write or its verification fails.
                            changed = true;
                            key.SetValue(Name, Desired, RegistryValueKind.DWord);
                            if (!Equals(key.GetValue(Name), Desired) || key.GetValueKind(Name) != RegistryValueKind.DWord)
                                throw new IOException("Policy verification failed.");
                        }
                    }
                    return true;
                }
                catch (Exception ex) { LogError("Apply " + Hive + "\\" + Path + "\\" + Name, ex); return false; }
            }

            public bool Restore()
            {
                try
                {
                    if (changed)
                    {
                        using (var root = OpenRoot())
                        using (var key = root.CreateSubKey(Path, true))
                        {
                            if (key == null) throw new IOException("Cannot restore policy key.");
                            if (existed) key.SetValue(Name, original, kind);
                            else key.DeleteValue(Name, false);
                            object actual = key.GetValue(Name, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                            if (existed ? (key.GetValueKind(Name) != kind ||
                                !System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(actual, original)) : actual != null)
                                throw new IOException("Policy restore verification failed.");
                        }
                    }
                    captured = changed = false;
                    original = null;
                    return true;
                }
                catch (Exception ex) { LogError("Restore " + Hive + "\\" + Path + "\\" + Name, ex); return false; }
            }
        }

        private static readonly PolicyValue Edge = new PolicyValue(RegistryHive.LocalMachine,
            @"Software\Policies\Microsoft\Windows\EdgeUI", "AllowEdgeSwipe", 0);
        private static readonly PolicyValue Notifications = new PolicyValue(RegistryHive.CurrentUser,
            @"Software\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", 1);

        public static bool Disable()
        {
            LastError = null;
            bool edge = Edge.Apply();
            bool notifications = Notifications.Apply();
            return edge && notifications;
        }

        public static bool Enable()
        {
            LastError = null;
            bool edge = Edge.Restore();
            bool notifications = Notifications.Restore();
            return edge && notifications;
        }

        private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr parameter);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hwnd, StringBuilder name, int length);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        /// <summary>Closes folder windows in this session without terminating the desktop shell.</summary>
        public static void CloseFolderWindows()
        {
            using (var current = Process.GetCurrentProcess())
            {
                EnumWindows((hwnd, parameter) =>
                {
                    var name = new StringBuilder(256);
                    GetClassName(hwnd, name, name.Capacity);
                    if (name.ToString() != "CabinetWClass" && name.ToString() != "ExploreWClass")
                        return true;
                    try
                    {
                        GetWindowThreadProcessId(hwnd, out uint processId);
                        using (var owner = Process.GetProcessById((int)processId))
                        {
                            if (owner.SessionId == current.SessionId &&
                                string.Equals(owner.ProcessName, "explorer", StringComparison.OrdinalIgnoreCase))
                            {
                                // WM_CLOSE targets only the folder window, never explorer.exe itself.
                                if (!PostMessage(hwnd, 0x0010, IntPtr.Zero, IntPtr.Zero))
                                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                            }
                        }
                    }
                    catch (ArgumentException) { /* Window process already exited. */ }
                    catch (Exception ex) { LogError("Close File Explorer window", ex); }
                    return true;
                }, IntPtr.Zero);
            }
        }
    }
}
