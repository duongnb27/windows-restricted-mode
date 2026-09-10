using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace RestrictedMode
{
    internal sealed class StartupTask : IDisposable
    {
        private const string TaskName = "Launch_RestrictedMode";
        private readonly List<object> objects = new List<object>();
        private readonly dynamic service;
        private readonly dynamic folder;
        private T Keep<T>(T value) { objects.Add(value); return value; }
        public StartupTask()
        {
            try
            {
                service = Keep(Activator.CreateInstance(Type.GetTypeFromProgID("Schedule.Service", true)));
                service.Connect();
                folder = Keep((object)service.GetFolder(@"\"));
            }
            catch { Dispose(); throw; }
        }
        private static bool IsNotFound(Exception ex)
        {
            const int notFound = unchecked((int)0x80070002);
            for (Exception e = ex; e != null; e = e.InnerException)
            {
                if (e is COMException com && com.ErrorCode == notFound) return true;
                if (e.HResult == notFound) return true;
            }
            return false;
        }

        private dynamic Find()
        {
            try { return Keep((object)folder.GetTask(TaskName)); }
            catch (Exception ex) when (IsNotFound(ex)) { return null; }
        }
        private static bool PrincipalMatchesCurrentUser(string principalUser)
        {
            if (string.IsNullOrEmpty(principalUser)) return false;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (string.Equals(principalUser, identity.User.Value, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(principalUser, identity.Name, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(principalUser, Environment.UserName, StringComparison.OrdinalIgnoreCase)) return true;
            int slash = identity.Name.LastIndexOf('\\');
            return slash >= 0 && string.Equals(principalUser, identity.Name.Substring(slash + 1),
                StringComparison.OrdinalIgnoreCase);
        }

        public bool IsEnabled()
        {
            dynamic task = Find();
            if (task == null || !(bool)task.Enabled) return false;
            dynamic definition = Keep((object)task.Definition);
            dynamic principal = Keep((object)definition.Principal);
            dynamic actions = Keep((object)definition.Actions);
            dynamic triggers = Keep((object)definition.Triggers);
            if ((int)actions.Count != 1) return false;
            dynamic action = Keep((object)actions.Item(1));
            if ((int)action.Type != 0 || !string.Equals(Path.GetFullPath((string)action.Path),
                Application.ExecutablePath, StringComparison.OrdinalIgnoreCase)) return false;
            if ((int)principal.LogonType != 3 || (int)principal.RunLevel != 1) return false;
            if (!PrincipalMatchesCurrentUser((string)principal.UserId)) return false;
            for (int i = 1; i <= (int)triggers.Count; i++)
            {
                dynamic trigger = Keep((object)triggers.Item(i));
                if ((int)trigger.Type == 9 && (bool)trigger.Enabled) return true;
            }
            return false;
        }
        public void SetEnabled(bool enabled)
        {
            if (!enabled)
            {
                try
                {
                    if (Find() != null) folder.DeleteTask(TaskName, 0);
                }
                catch (Exception ex) when (IsNotFound(ex)) { /* already removed */ }
                if (Find() != null) throw new IOException(UIText.StartupRemovalFailed);
                return;
            }
            dynamic definition = Keep((object)service.NewTask(0));
            dynamic principal = Keep((object)definition.Principal);
            string user = WindowsIdentity.GetCurrent().User.Value;
            principal.UserId = user;
            principal.LogonType = 3; // InteractiveToken: visible UI in this user's session.
            principal.RunLevel = 1; // HighestAvailable: required by the app manifest.
            dynamic settings = Keep((object)definition.Settings);
            settings.Enabled = true;
            settings.DisallowStartIfOnBatteries = false;
            settings.StopIfGoingOnBatteries = false;
            settings.ExecutionTimeLimit = "PT0S";
            settings.MultipleInstances = 2; // IgnoreNew
            dynamic triggers = Keep((object)definition.Triggers);
            dynamic trigger = Keep((object)triggers.Create(9));
            trigger.UserId = user;
            trigger.Enabled = true;
            dynamic actions = Keep((object)definition.Actions);
            dynamic action = Keep((object)actions.Create(0));
            action.Path = Application.ExecutablePath;
            action.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Keep((object)folder.RegisterTaskDefinition(TaskName, definition, 6, user, null, 3, null));
            if (!IsEnabled()) throw new IOException(UIText.StartupVerificationFailed);
        }
        public void Dispose()
        {
            for (int i = objects.Count - 1; i >= 0; i--)
                if (Marshal.IsComObject(objects[i])) Marshal.ReleaseComObject(objects[i]);
            objects.Clear();
        }
    }
}
