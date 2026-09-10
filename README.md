# Restricted Mode

Windows kiosk controls with an English settings interface: Access & security,
Managed apps, and Windows settings. Startup can be enabled at sign-in directly
from the Windows settings tab. The EXE requests administrator privileges.

See [architecture](docs/architecture.md) for module responsibilities and extension
points, and [deployment](Scripts/README.md) for packaging and policy lifecycle.

Build `RestrictedMode.sln` in Release / Any CPU. Deploy `bin/Release/RestrictedMode.exe`
and `RestrictedMode.exe.config`, plus `config.json` when preconfigured.
The lock icon is embedded; no extra icon file is required at deployment.
