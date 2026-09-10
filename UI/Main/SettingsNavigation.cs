using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace RestrictedMode
{
    internal sealed class SettingsNavigation : UserControl
    {
        private readonly TableLayoutPanel navigation = new TableLayoutPanel
        { Dock = DockStyle.Top, Height = 52, ColumnCount = 3, RowCount = 1, Padding = new Padding(16, 6, 16, 6) };
        private readonly Panel content = new Panel { Dock = DockStyle.Fill };
        private readonly List<Button> buttons = new List<Button>();
        private readonly List<Panel> pages = new List<Panel>();
        public SettingsNavigation()
        {
            for (int i = 0; i < 3; i++) navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            Controls.Add(content); Controls.Add(navigation);
            content.BringToFront();
        }
        public Panel AddPage(string title)
        {
            var page = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), Visible = false };
            var button = new Button { Text = title, Dock = DockStyle.Fill, Margin = new Padding(4, 0, 4, 0),
                UseMnemonic = false, FlatStyle = FlatStyle.Flat, TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false, Font = new Font("Segoe UI", 10F) };
            button.FlatAppearance.BorderSize = 0;
            int index = pages.Count;
            button.Click += (sender, args) => SelectPage(index);
            pages.Add(page); buttons.Add(button); content.Controls.Add(page);
            navigation.Controls.Add(button, index, 0);
            SelectPage(0);
            return page;
        }
        public void SelectPage(int index)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].Visible = i == index;
                buttons[i].BackColor = i == index ? Color.FromArgb(37, 99, 235) : Color.White;
                buttons[i].ForeColor = i == index ? Color.White : Color.FromArgb(30, 41, 59);
            }
            pages[index].BringToFront();
        }
    }
}
