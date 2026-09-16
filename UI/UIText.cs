namespace RestrictedMode
{
    // Single source for UI wording. Keep runtime labels and Designer labels in sync
    // by referencing these constants; dynamic values are supplied by the caller.
    internal static class UIText
    {
        public const string EdgeSettingsOpenFailed = "Cannot open edge-swipe settings.";
        public const string EdgeSettingsVerificationFailed = "Edge-swipe setting verification failed.";
        public const string StartupRemovalFailed = "Startup task was not removed.";
        public const string StartupVerificationFailed = "Startup task verification failed.";
        public const string AppTitle = "Restricted Mode | Settings";
        public const string AppName = "Restricted Mode";
        public const string AppSubtitle = "Configure access, managed apps and Windows startup.";
        public const string AccessTab = "Access & Security";
        public const string AppsTab = "Managed Apps";
        public const string SettingsTab = "Settings";
        public const string PasswordGroup = "Administrator access";
        public const string HotkeyGroup = "Exit shortcut";
        public const string HotCornerGroup = "Exit corner";
        public const string ApplicationsGroup = "Keep applications running";
        public const string DesktopGroup = "Desktop && startup";
        public const string PasswordHint = "Custom exit password:";
        public const string DefaultPassword = "Default password";
        public const string SavePassword = "Save";
        public const string UnsavedPasswordChanges = "Unsaved password changes";
        public const string HotCornerEnabled = "Enable hot corner to request exit";
        public const string AddApplication = "Add application";
        public const string Remove = "Remove";
        public const string Start = "Start";
        public const string DefaultHotkeys = "Default hotkeys: Ctrl + Shift + F12";
        public const string Alt = "Alt";
        public const string Shift = "Shift";
        public const string Ctrl = "Ctrl";
        public const string CustomHotkeys = "Custom hotkeys to exit Restricted Mode:";
        public const string CheckInterval = "Check every (seconds):";
        public const string ExeFile = "Exe file";
        public const string ArgumentsColumn = "Arguments";
        public const string WorkingDirectoryColumn = "Working directory";
        public const string Corner = "Corner:";
        public const string CornerSize = "Size (pixels):";
        public const string HideTaskbar = "Hide Taskbar";
        public const string HideStartMenu = "Hide Start Menu";
        public const string TopLeft = "Top left";
        public const string TopRight = "Top right";
        public const string BottomLeft = "Bottom left";
        public const string BottomRight = "Bottom right";
        public const string ExitTitle = "Exit Restricted Mode";
        public const string ExitPrompt = "Enter password to exit Restricted Mode:";
        public const string SaveFailed = "Save failed. Check folder permissions and try again.";
        public const string BlockEdgeSwipes = "Block screen-edge swipes (restart required)";
        public const string RestartMessage = "Setting saved. Restart Windows for the change to take effect.\n\n";
        public const string PersistentSettingMessage = "Stopping Restricted Mode or closing this app will keep this setting.";
        public const string RestartTitle = "Restart required";
        public const string EdgeSwipesSaveFailed = "Could not save the edge-swipe setting: ";
        public const string EdgeSwipesTitle = "Edge swipes";
        public const string EdgeSwipesUnavailable = "Edge-swipe status unavailable";
        public const string StartWithWindows = "Start with Windows";
        public const string StartupSaveFailed = "Could not update Windows startup: ";
        public const string StartupTitle = "Windows startup";
        public const string StartupUnavailable = "Startup status unavailable";
        public const string ExePath = "Exe path:";
        public const string Browse = "...";
        public const string ArgumentsLabel = "Arguments:";
        public const string WorkingDirectoryLabel = "Working directory (optional):";
        public const string OK = "OK";
        public const string Cancel = "Cancel";
        public const string ProcessTitle = "Add / Edit process";
        public const string ExeFilter = "Executable (*.exe)|*.exe|All files (*.*)|*.*";
        public const string SelectExe = "Select .exe file";
        public const string SelectWorkingDirectory = "Select working directory";
        public const string MissingExe = "Please select the .exe file path.";
        public const string MissingInformation = "Missing information";
        public const string ExeNotFound = "The .exe file does not exist.";
        public const string ErrorTitle = "Error";
        public const string DirectoryNotFound = "Working directory does not exist.";
        public const string AdministratorVerification = "Administrator verification";
        public const string Unlock = "Unlock";
        public const string PasswordPrompt = "Enter password:";
        public const string PasswordTitle = "Password";
        public const string IncorrectPassword = "Incorrect password. Please try again.";
    }
}
