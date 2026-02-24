using System;
using Microsoft.Win32;

namespace RestrictedMode
{
    /// <summary>
    /// Vô hiệu hóa / bật lại Task Manager cho user hiện tại qua registry.
    /// Đường dẫn: HKEY_CURRENT_USER\...\Policies\System, value DisableTaskMgr.
    /// </summary>
    public static class TaskManagerPolicy
    {
        private const string PolicyPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
        private const string ValueName = "DisableTaskMgr";

        /// <summary>
        /// Vô hiệu hóa Task Manager (Ctrl+Shift+Esc / Ctrl+Alt+Del → Task Manager sẽ bị disable).
        /// </summary>
        /// <returns>True nếu thành công.</returns>
        public static bool Disable()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(PolicyPath, writable: true))
                {
                    if (key != null)
                    {
                        key.SetValue(ValueName, 1, RegistryValueKind.DWord);
                        return true;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Cần quyền ghi registry (thường vẫn được với HKEY_CURRENT_USER)
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Bật lại Task Manager.
        /// </summary>
        /// <returns>True nếu thành công.</returns>
        public static bool Enable()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(PolicyPath, writable: true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(ValueName, throwOnMissingValue: false);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra Task Manager có đang bị disable không.
        /// </summary>
        public static bool IsDisabled()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(PolicyPath, writable: false))
                {
                    if (key == null) return false;
                    var val = key.GetValue(ValueName);
                    if (val == null) return false;
                    return Convert.ToInt32(val) != 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
