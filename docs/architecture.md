# Project structure

The application remains one .NET Framework 4.8 executable. Modules are grouped by
responsibility; the RestrictedMode namespace is retained to preserve WinForms
resource and designer compatibility.

| Location | Responsibility |
| --- | --- |
| Program.cs | Application entry point |
| Core/ | Restricted-mode state and exit events |
| Configuration/AppConfig.cs | Configuration data models |
| Configuration/ConfigManager.cs | Encrypted configuration storage |
| Services/Windows/ | Registry policies, taskbar/start menu and Scheduled Task integration |
| Services/Input/ | Keyboard hook and touch exit corners |
| Services/Processes/ | Managed application watchdog |
| UI/Main/RestrictedModeApplication.cs | Form lifecycle, entering/exiting restricted mode |
| UI/Main/ConfigurationBindings.cs | Form/config mapping and Start action |
| UI/Main/AutoSave.cs | Change events, automatic persistence and save status |
| UI/Main/SettingsNavigation.cs | Equal-width navigation buttons and page switching |
| UI/Main/StartupSettings.cs | Startup checkbox and status/error presentation |
| UI/Main/ManagedApplications.cs | Application list editing actions |
| UI/Main/ModernLayout.cs | Tabs, sections, colors and layout |
| UI/Main/*.Designer.cs | WinForms control creation and event bindings |
| UI/Dialogs/ | Password and application editing dialogs |
| UI/Theme/ | Shared typography |
| Assets/ | Embedded lock icon and PNG source |

## Adding functionality

Put OS operations in Services and expose explicit methods/results. Keep dialogs
and user-facing errors in UI. Add configuration fields to AppConfig and map them
in ConfigurationBindings. Scheduled Task status is read from Windows, not persisted
as a duplicate setting. Add sections/tabs in ModernLayout. Register new source
files in RestrictedMode.csproj, which uses explicit compile includes.

The main form is split into partial files so existing control events and private
state remain connected without duplicating logic. Services do not call into form
controls (the existing ExitHotCorners service accepts its UI invocation target).

## Validation and known limits

Build Release with MSBuild RestrictedMode.sln /p:Configuration=Release
/p:Platform="Any CPU". The icon and administrator manifest are embedded in the EXE.
Preview rendering uses an isolated copy with runtime policy/startup hooks disabled;
never launch the production form solely to render a screenshot.

Registry, Scheduled Task and touchscreen behavior must be checked on a kiosk.
Configuration writes use atomic replacement and a .bak copy. Load failure fallback
and in-memory policy snapshot behavior remain unchanged. UI edits save immediately
after initialization; managed-app add/edit/remove actions also save. Start stays in
settings if saving fails.
