using System;
using Microsoft.Win32;

namespace RestrictedMode
{
    /// <summary>
    /// Enable/disable Task Manager for current user via registry (HKCU\...\Policies\System, DisableTaskMgr).
    /// </summary>
    public static class TaskManagerPolicy
    {
        private const string PolicyPath = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
        private const string ValueName = "DisableTaskMgr";

        /// <summary>
        /// Disables Task Manager.
        /// </summary>
        /// <returns>True if successful.</returns>
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
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Re-enables Task Manager.
        /// </summary>
        /// <returns>True if successful.</returns>
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
        /// Returns true if Task Manager is currently disabled.
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
