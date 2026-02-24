# Require admin: re-launch with elevation if not running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

# 1. Application info
$AppName = "RestrictedMode" 
$ExePath = Join-Path $PSScriptRoot "RestrictedMode.exe"
$TaskName = "Launch_$AppName"

# 2. Trigger: run when user logs on
$Trigger = New-ScheduledTaskTrigger -AtLogon

# 3. Action and highest-privilege settings
$Action = New-ScheduledTaskAction -Execute $ExePath
$Principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -LogonType Interactive -RunLevel Highest
$Settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -Priority 0

# 4. Register task (overwrites if exists - Force)
Register-ScheduledTask -TaskName $TaskName -Action $Action -Trigger $Trigger -Principal $Principal -Settings $Settings -Force

Write-Host "--- RestrictedMode is set to run with Windows (Admin) ---" -ForegroundColor Green

# 5. Create Desktop shortcut (optional, for manual launch)
$WshShell = New-Object -ComObject WScript.Shell
$ShortcutPath = [System.IO.Path]::Combine([Environment]::GetFolderPath("Desktop"), "$AppName.lnk")
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = "C:\Windows\System32\schtasks.exe"
$Shortcut.Arguments = "/run /tn `"$TaskName`""
$Shortcut.IconLocation = $ExePath
$Shortcut.WindowStyle = 7
$Shortcut.Save()

Read-Host "Press Enter to exit"
