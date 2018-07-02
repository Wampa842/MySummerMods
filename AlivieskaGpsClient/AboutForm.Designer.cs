namespace AlivieskaGpsClient
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.showLicenseButton = new System.Windows.Forms.Button();
			this.showCopyrightNoticeButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 20);
			this.label1.TabIndex = 25;
			this.label1.Text = "Alivieska GPS client";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(322, 26);
			this.label2.TabIndex = 25;
			this.label2.Text = "Developed by Wampa842\r\nto be used with My Summer Car and the Alivieska GPS Server" +
    " mod";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.showLicenseButton);
			this.groupBox1.Controls.Add(this.showCopyrightNoticeButton);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.pictureBox1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 65);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(386, 93);
			this.groupBox1.TabIndex = 26;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "License";
			// 
			// showLicenseButton
			// 
			this.showLicenseButton.Location = new System.Drawing.Point(155, 64);
			this.showLicenseButton.Name = "showLicenseButton";
			this.showLicenseButton.Size = new System.Drawing.Size(101, 23);
			this.showLicenseButton.TabIndex = 1;
			this.showLicenseButton.Text = "Open license";
			this.showLicenseButton.UseVisualStyleBackColor = true;
			this.showLicenseButton.Click += new System.EventHandler(this.showLicenseButton_Click);
			// 
			// showCopyrightNoticeButton
			// 
			this.showCopyrightNoticeButton.Location = new System.Drawing.Point(13, 64);
			this.showCopyrightNoticeButton.Name = "showCopyrightNoticeButton";
			this.showCopyrightNoticeButton.Size = new System.Drawing.Size(136, 23);
			this.showCopyrightNoticeButton.TabIndex = 0;
			this.showCopyrightNoticeButton.Text = "Show copyright notice";
			this.showCopyrightNoticeButton.UseVisualStyleBackColor = true;
			this.showCopyrightNoticeButton.Click += new System.EventHandler(this.showCopyrightNoticeButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 45);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(151, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "License: GNU GPL-3.0-or-later";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::AlivieskaGpsClient.Properties.Resources.gplv3_127x51;
			this.pictureBox1.Location = new System.Drawing.Point(262, 11);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(118, 69);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Copyright (C) 2018 Wampa842";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(330, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "version 1.0.0";
			// 
			// AboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(410, 170);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutForm_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button showCopyrightNoticeButton;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button showLicenseButton;
	}
}
