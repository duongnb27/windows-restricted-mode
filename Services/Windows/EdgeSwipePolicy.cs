using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace RestrictedMode
{
    /// <summary>
    /// Applies the machine edge-swipe policy without disabling Widgets or Notification Center.
    /// Verify gesture blocking on the target Windows build. Administrator rights are required.
    /// Reboot (or at least sign out) after
    /// first apply — AllowEdgeSwipe often does not take effect until then.
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

        private const string EdgePath = @"Software\Policies\Microsoft\Windows\EdgeUI";
        private const string EdgeValue = "AllowEdgeSwipe";

        public static bool IsBlocked()
        {
            using (var root = OpenMachineRegistry())
            using (var key = root.OpenSubKey(EdgePath))
                return Equals(key?.GetValue(EdgeValue), 0);
        }

        // This is a persistent machine setting, independent of restricted sessions.
        public static void SetBlocked(bool blocked)
        {
            using (var root = OpenMachineRegistry())
            using (var key = root.CreateSubKey(EdgePath, true))
            {
                if (key == null) throw new IOException(UIText.EdgeSettingsOpenFailed);
                int desired = blocked ? 0 : 1;
                key.SetValue(EdgeValue, desired, RegistryValueKind.DWord);
                if (!Equals(key.GetValue(EdgeValue), desired) ||
                    key.GetValueKind(EdgeValue) != RegistryValueKind.DWord)
                    throw new IOException(UIText.EdgeSettingsVerificationFailed);
            }
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
