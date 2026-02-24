using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Low-level Keyboard Hook - chặn phím tắt toàn cục (ví dụ: Alt+Tab, Win, Ctrl+Esc).
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
        /// Sự kiện khi có phím được nhấn (trước khi quyết định chặn). Return true để chặn phím.
        /// </summary>
        public event Func<Keys, bool> KeyIntercept;

        public KeyboardHook()
        {
            _proc = HookCallback;
            BlockDefaultRestrictedKeys();
        }

        /// <summary>
        /// Các phím default bị chặn.
        /// </summary>
        public void BlockDefaultRestrictedKeys()
        {
            BlockKey(Keys.LWin);        // Windows trái (mở Start / tổ hợp Win+...)
            BlockKey(Keys.RWin);        // Windows phải
        }

        /// <summary>
        /// Chặn một phím theo Keys.
        /// </summary>
        public void BlockKey(Keys key)
        {
            _blockedKeys.Add(key);
            _blockedVirtualKeys.Add((int)key);
        }

        /// <summary>
        /// Bỏ chặn một phím.
        /// </summary>
        public void UnblockKey(Keys key)
        {
            _blockedKeys.Remove(key);
            _blockedVirtualKeys.Remove((int)key);
        }

        /// <summary>
        /// Cài đặt hook và bắt đầu chặn phím.
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
                throw new InvalidOperationException($"Không thể cài đặt keyboard hook. Error: {err}");
            }
        }

        /// <summary>
        /// Gỡ hook và dừng chặn phím.
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

                // Trạng thái phím modifier (chỉ cần đọc một lần)
                const int KeyPressed = 0x8000;
                bool altDown = (GetAsyncKeyState((int)Keys.LMenu) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RMenu) & KeyPressed) != 0;
                bool ctrlDown = (GetAsyncKeyState((int)Keys.LControlKey) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RControlKey) & KeyPressed) != 0;
                bool shiftDown = (GetAsyncKeyState((int)Keys.LShiftKey) & KeyPressed) != 0 || (GetAsyncKeyState((int)Keys.RShiftKey) & KeyPressed) != 0;

                // Các tổ hợp phím bị chặn (liệt kê rõ ràng)
                bool keyInterceptWantsBlock = KeyIntercept != null && KeyIntercept(key);
                bool isBlockedKey = _blockedKeys.Contains(key) || _blockedVirtualKeys.Contains((int)hookStruct.vkCode);  // Win trái/phải
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
