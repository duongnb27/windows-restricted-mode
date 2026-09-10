# Deployment

Build Release and copy RestrictedMode.exe and RestrictedMode.exe.config, plus
config.json if configured. The embedded manifest requests Administrator rights.
No policy script is required: the app reads actual registry values on every Start.
Run under the kiosk account; HKCU belongs to the account used for elevation.

Missing or different policies are applied and verified. Their original value and
registry type (or absence) are retained for the restricted session. Repeated Start
calls do not replace that snapshot. Exiting restricted mode or closing normally
restores only values changed by this app. Existing matching policies are left alone,
including values installed previously by an administrator.
Restore failures are logged and shown; a subsequent close can retry failed restores.
Snapshots are in memory: forced termination, crash or power loss cannot restore them.
The next launch reads the registry as it then exists.

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

Test on the target touchscreen: absent, already matching and differing policies;
repeated Start; normal exit and reopen; right/left swipes after activation.
Logs: %LOCALAPPDATA%\RestrictedMode\policy.log.
