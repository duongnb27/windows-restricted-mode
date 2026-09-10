using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestrictedMode
{
    /// <summary>
    /// Global restricted mode state and exit hotkey (default Ctrl+Shift+F12).
    /// </summary>
    public static class RestrictedState
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int KEY_PRESSED = 0x8000;

        #endregion

        public static bool IsRestrictedMode { get; private set; } = false;
        /// <summary>
        /// Raised when restricted mode exits; form/hook should uninstall and clean up.
        /// </summary>
        public static event Action RestrictedModeExited;
        /// <summary>
        /// Raised when exit is requested (hotkey or hot corner); form shows password dialog then calls ConfirmExitRestricted if correct.
        /// </summary>
        public static event Action ExitRestrictedRequested;

        private static readonly Keys DefaultExitKey = Keys.F12;
        private static readonly bool DefaultExitCtrl = true;
        private static readonly bool DefaultExitShift = true;
        private static readonly bool DefaultExitAlt = false;

        private static Keys _exitKey = DefaultExitKey;
        private static bool _exitCtrl = DefaultExitCtrl;
        private static bool _exitShift = DefaultExitShift;
        private static bool _exitAlt = DefaultExitAlt;
        private static bool _alsoAllowDefaultHotkey = true;

        public static void SetExitHotkey(Keys key, bool ctrl = true, bool shift = true, bool alt = false, bool alsoAllowDefault = true)
        {
            _exitKey = key;
            _exitCtrl = ctrl;
            _exitShift = shift;
            _exitAlt = alt;
            _alsoAllowDefaultHotkey = alsoAllowDefault;
        }

        public static void SetDefaultExitHotkey()
        {
            SetExitHotkey(DefaultExitKey, DefaultExitCtrl, DefaultExitShift, DefaultExitAlt, alsoAllowDefault: true);
        }

        public static void ApplyExitHotkeyConfig(ExitHotkeyConfig c)
        {
            if (c == null) return;
            var key = DefaultExitKey;
            if (!string.IsNullOrEmpty(c.Key) && Enum.TryParse(c.Key, true, out Keys parsed))
                key = parsed;
            SetExitHotkey(key, c.Ctrl, c.Shift, c.Alt, c.AlsoAllowDefaultHotkey);
        }

        /// <summary>
        /// Called from keyboard hook on key down. Returns true if exit hotkey matched (hook should not block).
        /// </summary>
        public static bool CheckAndTriggerExit(Keys key)
        {
            if (!IsRestrictedMode) return false;

            bool ctrlDown = (GetAsyncKeyState((int)Keys.LControlKey) & KEY_PRESSED) != 0 ||
                            (GetAsyncKeyState((int)Keys.RControlKey) & KEY_PRESSED) != 0;
            bool shiftDown = (GetAsyncKeyState((int)Keys.LShiftKey) & KEY_PRESSED) != 0 ||
                             (GetAsyncKeyState((int)Keys.RShiftKey) & KEY_PRESSED) != 0;
            bool altDown = (GetAsyncKeyState((int)Keys.LMenu) & KEY_PRESSED) != 0 ||
                           (GetAsyncKeyState((int)Keys.RMenu) & KEY_PRESSED) != 0;

            bool matchedCustom = MatchesHotkey(key, ctrlDown, shiftDown, altDown, _exitKey, _exitCtrl, _exitShift, _exitAlt);
            bool matchedDefault = _alsoAllowDefaultHotkey &&
                MatchesHotkey(key, ctrlDown, shiftDown, altDown, DefaultExitKey, DefaultExitCtrl, DefaultExitShift, DefaultExitAlt);

            if (!matchedCustom && !matchedDefault)
                return false;

            ExitRestrictedRequested?.Invoke();
            return true;
        }

        private static bool MatchesHotkey(Keys key, bool ctrlDown, bool shiftDown, bool altDown,
            Keys expectedKey, bool expectedCtrl, bool expectedShift, bool expectedAlt)
        {
            if (key != expectedKey) return false;
            return ctrlDown == expectedCtrl && shiftDown == expectedShift && altDown == expectedAlt;
        }

        /// <summary>
        /// Call from outside RestrictedState to request exit; events can only be raised from the declaring type.
        /// </summary>
        public static void RequestExitRestricted()
        {
            ExitRestrictedRequested?.Invoke();
        }

        /// <summary>
        /// Call after correct password; turns off restricted mode and raises RestrictedModeExited.
        /// </summary>
        public static void ConfirmExitRestricted()
        {
            IsRestrictedMode = false;
            RestrictedModeExited?.Invoke();
        }

        public static void EnterRestrictedMode()
        {
            IsRestrictedMode = true;
        }
    }
}
