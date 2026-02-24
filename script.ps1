# 1. Khai báo thông tin ứng dụng
$AppName = "RestrictedMode" 
$ExePath = "C:\Path\To\Your\RestrictedMode.exe" # <-- HÃY THAY ĐƯỜNG DẪN THỰC TẾ TẠI ĐÂY
$TaskName = "Launch_$AppName"

# 2. Thiết lập Trigger: Tự động chạy khi người dùng đăng nhập (Logon)
$Trigger = New-ScheduledTaskTrigger -AtLogon

# 3. Tạo Action và các thiết lập quyền cao nhất
$Action = New-ScheduledTaskAction -Execute $ExePath
$Principal = New-ScheduledTaskPrincipal -LogonType Interactive -RunLevel Highest
$Settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -Priority 0

# 4. Đăng ký Task vào hệ thống (Nếu đã có thì sẽ ghi đè - Force)
Register-ScheduledTask -TaskName $TaskName -Action $Action -Trigger $Trigger -Principal $Principal -Settings $Settings -Force

Write-Host "--- Da thiet lap RestrictedMode tu dong chay cung Windows (Admin) ---" -ForegroundColor Green

# 5. Tạo Shortcut ngoai Desktop (Tùy chọn, để bạn mở thủ công khi cần)
$WshShell = New-Object -ComObject WScript.Shell
$ShortcutPath = [System.IO.Path]::Combine([Environment]::GetFolderPath("Desktop"), "$AppName.lnk")
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = "C:\Windows\System32\schtasks.exe"
$Shortcut.Arguments = "/run /tn `"$TaskName`""
$Shortcut.IconLocation = $ExePath
$Shortcut.WindowStyle = 7
$Shortcut.Save()
