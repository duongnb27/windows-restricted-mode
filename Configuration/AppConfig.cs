namespace RestrictedMode
{
    public class ExitHotkeyConfig
    {
        public string Key { get; set; } = "F12";
        public bool Ctrl { get; set; } = true;
        public bool Shift { get; set; } = true;
        public bool Alt { get; set; } = false;
        public bool AlsoAllowDefaultHotkey { get; set; } = true;
    }

    public class WatchDogProcessConfig
    {
        public string ExePath { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
    }

    public class WatchDogConfig
    {
        public int CheckIntervalMs { get; set; } = 5000;
        public WatchDogProcessConfig[] Processes { get; set; } = new WatchDogProcessConfig[0];
    }

    public enum ExitHotCornerPosition
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public class AppConfig
    {
        public ExitHotkeyConfig ExitHotkey { get; set; } = new ExitHotkeyConfig();
        public WatchDogConfig WatchDog { get; set; } = new WatchDogConfig();
        /// <summary>
        /// Password to exit restricted mode; empty = not required.
        /// </summary>
        public string RestrictedPassword { get; set; }
        public bool ExitHotCornerEnabled { get; set; } = false;
        /// <summary>
        /// 0=TopLeft, 1=TopRight, 2=BottomLeft, 3=BottomRight.
        /// </summary>
        public int ExitHotCornerCorner { get; set; } = (int)ExitHotCornerPosition.TopLeft;
        /// <summary>
        /// Size of corner zone in pixels.
        /// </summary>
        public int ExitHotCornerSizePx { get; set; } = 50;
        /// <summary>
        /// Utility: hide taskbar on all monitors when in restricted mode.
        /// </summary>
        public bool UtilityHideTaskbar { get; set; } = false;
        /// <summary>
        /// Utility: hide Start Menu when in restricted mode.
        /// </summary>
        public bool UtilityHideStartMenu { get; set; } = false;
    }

}
