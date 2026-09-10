using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RestrictedMode
{
    public partial class RestrictedModeApplication
    {
        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            using (var f = new ProcessEditForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK && f.Result != null)
                {
                    var li = new ListViewItem(f.Result.ExePath);
                    li.SubItems.Add(f.Result.Arguments ?? "");
                    li.SubItems.Add(f.Result.WorkingDirectory ?? "");
                    li.Tag = f.Result;
                    lstProcesses.Items.Add(li);
                    SaveChanges();
                }
            }
        }

        private void btnRemoveProcess_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItems.Count == 0) return;
            foreach (ListViewItem li in lstProcesses.SelectedItems.Cast<ListViewItem>().ToArray())
                lstProcesses.Items.Remove(li);
            SaveChanges();
        }

        private void lstProcesses_DoubleClick(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedItems.Count == 0) return;
            var li = lstProcesses.SelectedItems[0];
            var p = li.Tag as WatchDogProcessConfig ?? new WatchDogProcessConfig { ExePath = li.Text, Arguments = li.SubItems.Count > 1 ? li.SubItems[1].Text : "", WorkingDirectory = li.SubItems.Count > 2 ? li.SubItems[2].Text : "" };
            using (var f = new ProcessEditForm(p))
            {
                if (f.ShowDialog(this) == DialogResult.OK && f.Result != null)
                {
                    li.Text = f.Result.ExePath;
                    while (li.SubItems.Count < 3) li.SubItems.Add("");
                    li.SubItems[1].Text = f.Result.Arguments ?? "";
                    li.SubItems[2].Text = f.Result.WorkingDirectory ?? "";
                    li.Tag = f.Result;
                    SaveChanges();
                }
            }
        }

    }
}
