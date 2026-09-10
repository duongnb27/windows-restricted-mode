# Deployment

Build Release and copy RestrictedMode.exe and RestrictedMode.exe.config, plus
config.json if configured. The embedded manifest requests Administrator rights.
The Settings tab includes "Block screen-edge swipes (restart required)".
It reads the current machine registry setting, not config.json. Checking writes
AllowEdgeSwipe=0; unchecking writes AllowEdgeSwipe=1. Each successful change shows
a restart reminder. Restart Windows manually to apply the change.
Starting/stopping Restricted Mode and closing the app never change this setting.
An existing AllowEdgeSwipe=0 is shown as checked, including manually configured values.
The app does not write AllowNewsAndInterests or DisableNotificationCenter.
Run under the kiosk account; HKCU belongs to the account used for elevation.
Explorer is never restarted. Existing folder windows are asked to close on Start;
desktop/taskbar processes are not killed. Windows may require a reboot for policy
changes to become effective; registry verification alone does not verify activation.

Use the startup checkbox in the Utility group to enable/disable launch at Windows
sign-in. Changes apply immediately, without Save. The checkbox reads Task Scheduler,
not config.json. The app registers Launch_RestrictedMode for the elevated account,
with interactive logon and highest privileges, using the current EXE directory.
Keep the EXE at that location; after moving it, enable startup again from the new copy.
A task for another EXE/account is not displayed as enabled; checking replaces it.
The legacy registration scripts are no longer needed.

Test on the target touchscreen: toggle edge-swipe blocking in Settings, restart,
verify right/left swipes; stop, close and reopen to verify the setting persists.
Logs: %LOCALAPPDATA%\RestrictedMode\policy.log.
