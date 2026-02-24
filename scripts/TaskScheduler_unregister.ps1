# Require admin: re-launch with elevation if not running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

# 1. Application info (must match script_register.ps1)
$AppName = "RestrictedMode"
$TaskName = "Launch_$AppName"

# 2. Remove scheduled task
$task = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
if ($task) {
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
    Write-Host "Removed task: $TaskName" -ForegroundColor Green
} else {
    Write-Host "Task not found: $TaskName" -ForegroundColor Yellow
}

# 3. Remove Desktop shortcut (if present)
$ShortcutPath = [System.IO.Path]::Combine([Environment]::GetFolderPath("Desktop"), "$AppName.lnk")
if (Test-Path $ShortcutPath) {
    Remove-Item $ShortcutPath -Force
    Write-Host "Removed shortcut: $ShortcutPath" -ForegroundColor Green
} else {
    Write-Host "Desktop shortcut not found." -ForegroundColor Yellow
}

Write-Host "--- RestrictedMode auto-run with Windows has been unregistered ---" -ForegroundColor Green

Read-Host "Press Enter to exit"
