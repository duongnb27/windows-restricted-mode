using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Global restricted mode state and custom exit hotkey (default Ctrl+Shift+F12).
    /// </summary>
    public static class RestrictedState
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int KEY_PRESSED = 0x8000;

        #endregion

        /// <summary>
        /// True while in restricted mode (key blocking etc.); false after exit.
        /// </summary>
        public static bool IsRestrictedMode { get; private set; } = false;

        /// <summary>
        /// Raised when restricted mode exits; form/hook should uninstall and clean up.
        /// </summary>
        public static event Action RestrictedModeExited;

        /// <summary>
        /// Raised when exit hotkey is pressed; form shows password dialog, then calls ConfirmExitRestricted() if correct.
        /// </summary>
        public static event Action ExitRestrictedRequested;

        private static Keys _exitKey = Keys.F12;
        private static bool _exitCtrl = true;
        private static bool _exitShift = true;
        private static bool _exitAlt = false;

        /// <summary>
        /// Sets exit hotkey.
        /// </summary>
        /// <param name="key">Main key (default F12)</param>
        /// <param name="ctrl">Require Ctrl</param>
        /// <param name="shift">Require Shift</param>
        /// <param name="alt">Require Alt</param>
        public static void SetExitHotkey(Keys key, bool ctrl = true, bool shift = true, bool alt = false)
        {
            _exitKey = key;
            _exitCtrl = ctrl;
            _exitShift = shift;
            _exitAlt = alt;
        }

        /// <summary>
        /// Resets to default: Ctrl+Shift+F12.
        /// </summary>
        public static void SetDefaultExitHotkey()
        {
            SetExitHotkey(Keys.F12, ctrl: true, shift: true, alt: false);
        }

        /// <summary>
        /// Applies exit hotkey from config (e.g. after loading config.json).
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
        /// Called from keyboard hook on key down. Returns true if exit hotkey matched and exit was triggered (hook should not block).
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

            ExitRestrictedRequested?.Invoke();
            return true;
        }

        /// <summary>
        /// Call after correct password; turns off restricted mode and raises RestrictedModeExited.
        /// </summary>
        public static void ConfirmExitRestricted()
        {
            IsRestrictedMode = false;
            RestrictedModeExited?.Invoke();
        }

        /// <summary>
        /// Enters restricted mode.
        /// </summary>
        public static void EnterRestrictedMode()
        {
            IsRestrictedMode = true;
        }
    }
}
