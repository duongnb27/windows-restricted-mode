using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Low-level keyboard hook; blocks global shortcuts (e.g. Alt+Tab, Win, Ctrl+Esc).
    /// </summary>
    public class KeyboardHook : IDisposable
    {
        #region Win32 API

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;
        private readonly HashSet<Keys> _blockedKeys = new HashSet<Keys>();
        private readonly HashSet<int> _blockedVirtualKeys = new HashSet<int>();
        private bool _disposed;

        /// <summary>
        /// Raised on key press before block decision; return true to block the key.
        /// </summary>
        public event Func<Keys, bool> KeyIntercept;

        public KeyboardHook()
        {
            _proc = HookCallback;
            BlockDefaultRestrictedKeys();
        }

        /// <summary>
        /// Blocks default restricted keys (L/R Win).
        /// </summary>
        public void BlockDefaultRestrictedKeys()
        {
            BlockKey(Keys.LWin);
            BlockKey(Keys.RWin);
        }

        /// <summary>
        /// Blocks a key.
        /// </summary>
        public void BlockKey(Keys key)
        {
            _blockedKeys.Add(key);
            _blockedVirtualKeys.Add((int)key);
        }

        /// <summary>
        /// Unblocks a key.
        /// </summary>
        public void UnblockKey(Keys key)
        {
            _blockedKeys.Remove(key);
            _blockedVirtualKeys.Remove((int)key);
        }

        /// <summary>
        /// Installs hook and starts blocking keys.
        /// </summary>
        public void Install()
        {
            if (_hookId != IntPtr.Zero) return;
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }
            if (_hookId == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Failed to install keyboard hook. Error: {err}");
            }
        }

        /// <summary>
        /// Uninstalls hook and stops blocking.
        /// </summary>
        public void Uninstall()
        {
            if (_hookId == IntPtr.Zero) return;
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                if (!RestrictedState.IsRestrictedMode)
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                var hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                var key = (Keys)hookStruct.vkCode;

                if (RestrictedState.CheckAndTriggerExit(key))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                const int KeyPressed = 0x8000;
                bool altDown = (GetAsyncKeyState((int)Keys.LMenu) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RMenu) & KeyPressed) != 0;
                bool ctrlDown = (GetAsyncKeyState((int)Keys.LControlKey) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RControlKey) & KeyPressed) != 0;
                bool shiftDown = (GetAsyncKeyState((int)Keys.LShiftKey) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RShiftKey) & KeyPressed) != 0;

                bool keyInterceptWantsBlock = KeyIntercept != null && KeyIntercept(key);
                bool isBlockedKey = _blockedKeys.Contains(key) || _blockedVirtualKeys.Contains((int)hookStruct.vkCode);
                bool isAltTab = key == Keys.Tab && altDown;
                bool isAltF4 = key == Keys.F4 && altDown;
                bool isCtrlEsc = key == Keys.Escape && ctrlDown;
                bool isCtrlShiftDelete = key == Keys.Delete && ctrlDown && shiftDown;
                bool isCtrlAltDelete = key == Keys.Delete && ctrlDown && altDown;

                bool shouldBlock = keyInterceptWantsBlock
                    || isBlockedKey
                    || isAltTab
                    || isAltF4
                    || isCtrlEsc
                    || isCtrlShiftDelete
                    || isCtrlAltDelete;

                if (shouldBlock)
                    return (IntPtr)1;
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            if (_disposed) return;
            Uninstall();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
