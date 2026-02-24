using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RestrictedMode
{
    /// <summary>
    /// Hide or show the taskbar on all monitors using Win32
    /// (Shell_TrayWnd = primary, Shell_SecondaryTrayWnd = secondary monitors).
    /// </summary>
    public static class TaskbarPolicy
    {
        #region Win32

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private const string ShellTrayWnd = "Shell_TrayWnd";
        private const string ShellSecondaryTrayWnd = "Shell_SecondaryTrayWnd";

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        #endregion

        /// <summary>
        /// Hides the taskbar on all monitors.
        /// </summary>
        /// <returns>True if at least one taskbar was found and hidden.</returns>
        public static bool Hide()
        {
            bool any = false;
            foreach (IntPtr hwnd in GetAllTaskbarHandles())
            {
                if (hwnd != IntPtr.Zero)
                {
                    ShowWindow(hwnd, SW_HIDE);
                    any = true;
                }
            }
            return any;
        }

        /// <summary>
        /// Shows the taskbar on all monitors again.
        /// </summary>
        /// <returns>True if at least one taskbar was found and shown.</returns>
        public static bool Show()
        {
            bool any = false;
            foreach (IntPtr hwnd in GetAllTaskbarHandles())
            {
                if (hwnd != IntPtr.Zero)
                {
                    ShowWindow(hwnd, SW_SHOW);
                    any = true;
                }
            }
            return any;
        }

        private static IEnumerable<IntPtr> GetAllTaskbarHandles()
        {
            var list = new List<IntPtr>();

            // Primary taskbar
            IntPtr primary = FindWindow(ShellTrayWnd, null);
            if (primary != IntPtr.Zero)
                list.Add(primary);

            // Secondary taskbars (one per additional monitor)
            EnumWindows((hWnd, lParam) =>
            {
                var sb = new StringBuilder(256);
                if (GetClassName(hWnd, sb, sb.Capacity) > 0 &&
                    string.Equals(sb.ToString(), ShellSecondaryTrayWnd, StringComparison.Ordinal))
                {
                    list.Add(hWnd);
                }
                return true;
            }, IntPtr.Zero);

            return list;
        }
    }
}
