using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Trạng thái toàn cục của restricted mode và xử lý thoát bằng tổ hợp phím tùy chỉnh.
    /// Mặc định: Ctrl+Shift+F12. Sau khi thoát, tất cả chức năng chặn phím sẽ tắt.
    /// </summary>
    public static class RestrictedState
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int KEY_PRESSED = 0x8000;

        #endregion

        /// <summary>
        /// Biến toàn cục: true = đang ở Restricted Mode (chặn phím, v.v.), false = đã thoát, mọi chức năng tắt.
        /// Mặc định false khi khởi động app để form cấu hình hiển thị; chỉ thành true khi gọi EnterRestrictedMode().
        /// </summary>
        public static bool IsRestrictedMode { get; private set; } = false;

        /// <summary>
        /// Sự kiện khi người dùng nhấn tổ hợp thoát và restricted mode được tắt.
        /// Form/hook nên đăng ký để Uninstall hook và tắt các chức năng khác.
        /// </summary>
        public static event Action RestrictedModeExited;

        /// <summary>
        /// Sự kiện khi user nhấn tổ hợp thoát — Form cần hiện ô nhập mật khẩu, nếu đúng mới gọi ConfirmExitRestricted() và tắt restricted.
        /// </summary>
        public static event Action ExitRestrictedRequested;

        // Tổ hợp phím thoát (mặc định Ctrl+Shift+F12)
        private static Keys _exitKey = Keys.F12;
        private static bool _exitCtrl = true;
        private static bool _exitShift = true;
        private static bool _exitAlt = false;

        /// <summary>
        /// Cấu hình tổ hợp phím thoát restricted mode.
        /// </summary>
        /// <param name="key">Phím chính (mặc định F12)</param>
        /// <param name="ctrl">Bắt buộc giữ Ctrl</param>
        /// <param name="shift">Bắt buộc giữ Shift</param>
        /// <param name="alt">Bắt buộc giữ Alt</param>
        public static void SetExitHotkey(Keys key, bool ctrl = true, bool shift = true, bool alt = false)
        {
            _exitKey = key;
            _exitCtrl = ctrl;
            _exitShift = shift;
            _exitAlt = alt;
        }

        /// <summary>
        /// Đặt lại tổ hợp mặc định: Ctrl+Shift+F12.
        /// </summary>
        public static void SetDefaultExitHotkey()
        {
            SetExitHotkey(Keys.F12, ctrl: true, shift: true, alt: false);
        }

        /// <summary>
        /// Áp dụng tổ hợp thoát từ config (dùng khi load config.json).
        /// </summary>
        public static void ApplyExitHotkeyConfig(ExitHotkeyConfig c)
        {
            if (c == null) return;
            var key = Keys.F12;
            if (!string.IsNullOrEmpty(c.Key) && Enum.TryParse(c.Key, true, out Keys parsed))
                key = parsed;
            SetExitHotkey(key, c.Ctrl, c.Shift, c.Alt);
        }

        /// <summary>
        /// Gọi từ keyboard hook khi có phím key down. Trả về true nếu đây là tổ hợp thoát
        /// và đã kích hoạt thoát (đã set IsRestrictedMode = false và raise RestrictedModeExited).
        /// Khi đó hook không nên chặn phím này.
        /// </summary>
        public static bool CheckAndTriggerExit(Keys key)
        {
            if (!IsRestrictedMode) return false;
            if (key != _exitKey) return false;

            bool ctrlDown = (GetAsyncKeyState((int)Keys.LControlKey) & KEY_PRESSED) != 0 ||
                            (GetAsyncKeyState((int)Keys.RControlKey) & KEY_PRESSED) != 0;
            bool shiftDown = (GetAsyncKeyState((int)Keys.LShiftKey) & KEY_PRESSED) != 0 ||
                             (GetAsyncKeyState((int)Keys.RShiftKey) & KEY_PRESSED) != 0;
            bool altDown = (GetAsyncKeyState((int)Keys.LMenu) & KEY_PRESSED) != 0 ||
                           (GetAsyncKeyState((int)Keys.RMenu) & KEY_PRESSED) != 0;

            if (ctrlDown != _exitCtrl || shiftDown != _exitShift || altDown != _exitAlt)
                return false;

            // Yêu cầu Form hiện dialog mật khẩu; chỉ khi đúng mới gọi ConfirmExitRestricted()
            ExitRestrictedRequested?.Invoke();
            return true;
        }

        /// <summary>
        /// Gọi sau khi nhập đúng mật khẩu thoát — thực sự tắt restricted và raise RestrictedModeExited.
        /// </summary>
        public static void ConfirmExitRestricted()
        {
            IsRestrictedMode = false;
            RestrictedModeExited?.Invoke();
        }

        /// <summary>
        /// Bật lại restricted mode (dùng nếu sau này có nút "Vào restricted").
        /// </summary>
        public static void EnterRestrictedMode()
        {
            IsRestrictedMode = true;
        }
    }
}
