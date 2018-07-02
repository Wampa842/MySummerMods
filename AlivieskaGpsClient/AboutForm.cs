using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlivieskaGpsClient
{
	partial class AboutForm : Form
	{
		private Form _notice;

		public AboutForm()
		{
			InitializeComponent();
			_notice = new Form
			{
				Size = new Size(400, 300),
				StartPosition = FormStartPosition.Manual,
				FormBorderStyle = FormBorderStyle.FixedToolWindow,
				ShowInTaskbar = false,
				ShowIcon = false,
				Text = "GNU General Public License 3.0-or-later - Copyright notice"
			};
			_notice.Controls.Add(new Label()
			{
				Name = "noticeLabel",
				Dock = DockStyle.Top,
				AutoSize = false,
				Text = "Copyright (C) 2018 Wampa842\n\nThis program is free software: you can redistribute it and / or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version." +
					"\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details." +
					"\n\nYou should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.",
				Height = 185
			});
			LinkLabel link = new LinkLabel()
			{
				Name = "gnuLink",
				Text = "http://www.gnu.org/licenses/",
				Dock = DockStyle.Bottom,
				TextAlign = ContentAlignment.MiddleCenter
			};
			link.LinkClicked += (o, e) => { System.Diagnostics.Process.Start("http://www.gnu.org/licenses/"); };
			_notice.Controls.Add(link);
		}

		private void showCopyrightNoticeButton_Click(object sender, EventArgs e)
		{
			_notice.Location = new Point(this.Location.X + this.Width, this.Location.Y);
			_notice.ShowDialog();
		}

		private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private void showLicenseButton_Click(object sender, EventArgs e)
		{
			if(System.IO.File.Exists("COPYING.TXT"))
			{
				System.Diagnostics.Process.Start("COPYING.TXT");
			}
			else
			{
				((Button)sender).Text = "File not found";
				((Button)sender).Enabled = false;
			}
		}
	}
}
