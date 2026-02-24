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
        private bool _blockAllSysKeys;
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
        /// Thêm danh sách phím tắt mặc định thường dùng để thoát restricted.
        /// </summary>
        public void BlockDefaultRestrictedKeys()
        {
            BlockKey(Keys.LMenu);       // Alt trái
            BlockKey(Keys.RMenu);       // Alt phải
            BlockKey(Keys.LWin);        // Windows trái
            BlockKey(Keys.RWin);        // Windows phải
            BlockKey(Keys.Escape);      // Esc (có thể bỏ nếu cần)
            BlockKey(Keys.Tab);         // Tab (kết hợp Alt+Tab)
            BlockKey(Keys.F4);          // Alt+F4
            BlockKey(Keys.Apps);        // Menu key (phím Application)
            _blockAllSysKeys = true;    // Chặn tổ hợp có Alt (SysKey)
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
        /// Bật/tắt chặn mọi SysKey (tổ hợp có Alt).
        /// </summary>
        public void SetBlockAllSysKeys(bool block)
        {
            _blockAllSysKeys = block;
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
                // Nếu không ở restricted mode thì không chặn phím nào
                if (!RestrictedState.IsRestrictedMode)
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                var hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                var key = (Keys)hookStruct.vkCode;

                // Tổ hợp thoát restricted → không chặn, đã tắt IsRestrictedMode trong RestrictedState
                if (RestrictedState.CheckAndTriggerExit(key))
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);

                bool shouldBlock = false;

                // 1. Kiểm tra event tùy chỉnh (return true = chặn phím)
                if (KeyIntercept != null && KeyIntercept(key))
                    shouldBlock = true;

                // 2. Chặn SysKey (Alt+...) nếu bật
                if (!shouldBlock && _blockAllSysKeys && (wParam == (IntPtr)WM_SYSKEYDOWN))
                    shouldBlock = true;

                // 3. Chặn phím nằm trong danh sách
                if (!shouldBlock && (_blockedKeys.Contains(key) || _blockedVirtualKeys.Contains((int)hookStruct.vkCode)))
                    shouldBlock = true;

                // 4. Chặn Ctrl+Esc (mở Start menu)
                if (!shouldBlock && key == Keys.Escape && (GetAsyncKeyState((int)Keys.LControlKey) != 0 || GetAsyncKeyState((int)Keys.RControlKey) != 0))
                    shouldBlock = true;

                // 5. Chặn Ctrl+Shift+Delete (thường mở Task Manager / Security)
                if (!shouldBlock && key == Keys.Delete)
                {
                    bool ctrl = (GetAsyncKeyState((int)Keys.LControlKey) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.RControlKey) & 0x8000) != 0;
                    bool shift = (GetAsyncKeyState((int)Keys.LShiftKey) & 0x8000) != 0 || (GetAsyncKeyState((int)Keys.RShiftKey) & 0x8000) != 0;
                    if (ctrl && shift)
                        shouldBlock = true;
                }

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
