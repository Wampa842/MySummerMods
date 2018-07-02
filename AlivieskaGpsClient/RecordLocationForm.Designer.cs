namespace AlivieskaGpsClient
{
	partial class RecordLocationForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			this.mainTabs = new System.Windows.Forms.TabControl();
			this.recordPoiTab = new System.Windows.Forms.TabPage();
			this.lockPoiPositionCheck = new System.Windows.Forms.CheckBox();
			this.poiYLabel = new System.Windows.Forms.Label();
			this.poiXLabel = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.writePoiButton = new System.Windows.Forms.Button();
			this.poiTypeSelect = new System.Windows.Forms.ComboBox();
			this.poiNameText = new System.Windows.Forms.TextBox();
			this.poiIDText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.recordHazardTab = new System.Windows.Forms.TabPage();
			this.lockHazardPositionCheck = new System.Windows.Forms.CheckBox();
			this.hazardYLabel = new System.Windows.Forms.Label();
			this.hazardXLabel = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.writeHazardButton = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.hazardDescriptionText = new System.Windows.Forms.TextBox();
			this.hazardTypeSelect = new System.Windows.Forms.ComboBox();
			this.hazardNameText = new System.Windows.Forms.TextBox();
			this.hazardIDText = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.mainTabs.SuspendLayout();
			this.recordPoiTab.SuspendLayout();
			this.recordHazardTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTabs
			// 
			this.mainTabs.Controls.Add(this.recordPoiTab);
			this.mainTabs.Controls.Add(this.recordHazardTab);
			this.mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTabs.Location = new System.Drawing.Point(0, 0);
			this.mainTabs.Name = "mainTabs";
			this.mainTabs.SelectedIndex = 0;
			this.mainTabs.Size = new System.Drawing.Size(651, 150);
			this.mainTabs.TabIndex = 0;
			// 
			// recordPoiTab
			// 
			this.recordPoiTab.Controls.Add(this.lockPoiPositionCheck);
			this.recordPoiTab.Controls.Add(this.poiYLabel);
			this.recordPoiTab.Controls.Add(this.poiXLabel);
			this.recordPoiTab.Controls.Add(this.label9);
			this.recordPoiTab.Controls.Add(this.label8);
			this.recordPoiTab.Controls.Add(this.writePoiButton);
			this.recordPoiTab.Controls.Add(this.poiTypeSelect);
			this.recordPoiTab.Controls.Add(this.poiNameText);
			this.recordPoiTab.Controls.Add(this.poiIDText);
			this.recordPoiTab.Controls.Add(this.label3);
			this.recordPoiTab.Controls.Add(this.label2);
			this.recordPoiTab.Controls.Add(this.label1);
			this.recordPoiTab.Location = new System.Drawing.Point(4, 22);
			this.recordPoiTab.Name = "recordPoiTab";
			this.recordPoiTab.Padding = new System.Windows.Forms.Padding(3);
			this.recordPoiTab.Size = new System.Drawing.Size(643, 124);
			this.recordPoiTab.TabIndex = 0;
			this.recordPoiTab.Text = "Point of interest";
			this.recordPoiTab.UseVisualStyleBackColor = true;
			// 
			// lockPoiPositionCheck
			// 
			this.lockPoiPositionCheck.Appearance = System.Windows.Forms.Appearance.Button;
			this.lockPoiPositionCheck.Location = new System.Drawing.Point(499, 65);
			this.lockPoiPositionCheck.Name = "lockPoiPositionCheck";
			this.lockPoiPositionCheck.Size = new System.Drawing.Size(136, 24);
			this.lockPoiPositionCheck.TabIndex = 3;
			this.lockPoiPositionCheck.Text = "Lock position";
			this.lockPoiPositionCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lockPoiPositionCheck.UseVisualStyleBackColor = true;
			// 
			// poiYLabel
			// 
			this.poiYLabel.AutoSize = true;
			this.poiYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.poiYLabel.Location = new System.Drawing.Point(329, 68);
			this.poiYLabel.Name = "poiYLabel";
			this.poiYLabel.Size = new System.Drawing.Size(14, 13);
			this.poiYLabel.TabIndex = 6;
			this.poiYLabel.Text = "0";
			// 
			// poiXLabel
			// 
			this.poiXLabel.AutoSize = true;
			this.poiXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.poiXLabel.Location = new System.Drawing.Point(329, 42);
			this.poiXLabel.Name = "poiXLabel";
			this.poiXLabel.Size = new System.Drawing.Size(14, 13);
			this.poiXLabel.TabIndex = 5;
			this.poiXLabel.Text = "0";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label9.Location = new System.Drawing.Point(280, 68);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(15, 13);
			this.label9.TabIndex = 6;
			this.label9.Text = "Y";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label8.Location = new System.Drawing.Point(280, 42);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(15, 13);
			this.label8.TabIndex = 5;
			this.label8.Text = "X";
			// 
			// writePoiButton
			// 
			this.writePoiButton.Location = new System.Drawing.Point(499, 95);
			this.writePoiButton.Name = "writePoiButton";
			this.writePoiButton.Size = new System.Drawing.Size(138, 23);
			this.writePoiButton.TabIndex = 4;
			this.writePoiButton.Text = "Record";
			this.writePoiButton.UseVisualStyleBackColor = true;
			this.writePoiButton.Click += new System.EventHandler(this.writePoiButton_Click);
			// 
			// poiTypeSelect
			// 
			this.poiTypeSelect.FormattingEnabled = true;
			this.poiTypeSelect.Items.AddRange(new object[] {
            "Other",
            "Town",
            "Shop or service",
            "Job"});
			this.poiTypeSelect.Location = new System.Drawing.Point(92, 65);
			this.poiTypeSelect.Name = "poiTypeSelect";
			this.poiTypeSelect.Size = new System.Drawing.Size(130, 21);
			this.poiTypeSelect.TabIndex = 2;
			// 
			// poiNameText
			// 
			this.poiNameText.Location = new System.Drawing.Point(92, 39);
			this.poiNameText.Name = "poiNameText";
			this.poiNameText.Size = new System.Drawing.Size(130, 20);
			this.poiNameText.TabIndex = 1;
			// 
			// poiIDText
			// 
			this.poiIDText.Location = new System.Drawing.Point(92, 13);
			this.poiIDText.Name = "poiIDText";
			this.poiIDText.Size = new System.Drawing.Size(130, 20);
			this.poiIDText.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Type:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Unique ID:";
			// 
			// recordHazardTab
			// 
			this.recordHazardTab.Controls.Add(this.lockHazardPositionCheck);
			this.recordHazardTab.Controls.Add(this.hazardYLabel);
			this.recordHazardTab.Controls.Add(this.hazardXLabel);
			this.recordHazardTab.Controls.Add(this.label12);
			this.recordHazardTab.Controls.Add(this.label13);
			this.recordHazardTab.Controls.Add(this.writeHazardButton);
			this.recordHazardTab.Controls.Add(this.label7);
			this.recordHazardTab.Controls.Add(this.hazardDescriptionText);
			this.recordHazardTab.Controls.Add(this.hazardTypeSelect);
			this.recordHazardTab.Controls.Add(this.hazardNameText);
			this.recordHazardTab.Controls.Add(this.hazardIDText);
			this.recordHazardTab.Controls.Add(this.label4);
			this.recordHazardTab.Controls.Add(this.label5);
			this.recordHazardTab.Controls.Add(this.label6);
			this.recordHazardTab.Location = new System.Drawing.Point(4, 22);
			this.recordHazardTab.Name = "recordHazardTab";
			this.recordHazardTab.Padding = new System.Windows.Forms.Padding(3);
			this.recordHazardTab.Size = new System.Drawing.Size(643, 124);
			this.recordHazardTab.TabIndex = 1;
			this.recordHazardTab.Text = "Hazard";
			this.recordHazardTab.UseVisualStyleBackColor = true;
			// 
			// lockHazardPositionCheck
			// 
			this.lockHazardPositionCheck.Appearance = System.Windows.Forms.Appearance.Button;
			this.lockHazardPositionCheck.Location = new System.Drawing.Point(499, 65);
			this.lockHazardPositionCheck.Name = "lockHazardPositionCheck";
			this.lockHazardPositionCheck.Size = new System.Drawing.Size(136, 24);
			this.lockHazardPositionCheck.TabIndex = 4;
			this.lockHazardPositionCheck.Text = "Lock position";
			this.lockHazardPositionCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lockHazardPositionCheck.UseVisualStyleBackColor = true;
			// 
			// hazardYLabel
			// 
			this.hazardYLabel.AutoSize = true;
			this.hazardYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.hazardYLabel.Location = new System.Drawing.Point(329, 68);
			this.hazardYLabel.Name = "hazardYLabel";
			this.hazardYLabel.Size = new System.Drawing.Size(14, 13);
			this.hazardYLabel.TabIndex = 15;
			this.hazardYLabel.Text = "0";
			// 
			// hazardXLabel
			// 
			this.hazardXLabel.AutoSize = true;
			this.hazardXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.hazardXLabel.Location = new System.Drawing.Point(329, 42);
			this.hazardXLabel.Name = "hazardXLabel";
			this.hazardXLabel.Size = new System.Drawing.Size(14, 13);
			this.hazardXLabel.TabIndex = 13;
			this.hazardXLabel.Text = "0";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label12.Location = new System.Drawing.Point(280, 68);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(15, 13);
			this.label12.TabIndex = 16;
			this.label12.Text = "Y";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label13.Location = new System.Drawing.Point(280, 42);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(15, 13);
			this.label13.TabIndex = 14;
			this.label13.Text = "X";
			// 
			// writeHazardButton
			// 
			this.writeHazardButton.Location = new System.Drawing.Point(499, 95);
			this.writeHazardButton.Name = "writeHazardButton";
			this.writeHazardButton.Size = new System.Drawing.Size(138, 23);
			this.writeHazardButton.TabIndex = 5;
			this.writeHazardButton.Text = "Record";
			this.writeHazardButton.UseVisualStyleBackColor = true;
			this.writeHazardButton.Click += new System.EventHandler(this.writeHazardButton_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(234, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(63, 13);
			this.label7.TabIndex = 11;
			this.label7.Text = "Description:";
			// 
			// hazardDescriptionText
			// 
			this.hazardDescriptionText.Location = new System.Drawing.Point(306, 13);
			this.hazardDescriptionText.Name = "hazardDescriptionText";
			this.hazardDescriptionText.Size = new System.Drawing.Size(331, 20);
			this.hazardDescriptionText.TabIndex = 3;
			// 
			// hazardTypeSelect
			// 
			this.hazardTypeSelect.FormattingEnabled = true;
			this.hazardTypeSelect.Items.AddRange(new object[] {
            "Other",
            "Road and topography",
            "Traffic",
            "Railway",
            "Police"});
			this.hazardTypeSelect.Location = new System.Drawing.Point(92, 65);
			this.hazardTypeSelect.Name = "hazardTypeSelect";
			this.hazardTypeSelect.Size = new System.Drawing.Size(130, 21);
			this.hazardTypeSelect.TabIndex = 2;
			// 
			// hazardNameText
			// 
			this.hazardNameText.Location = new System.Drawing.Point(92, 39);
			this.hazardNameText.Name = "hazardNameText";
			this.hazardNameText.Size = new System.Drawing.Size(130, 20);
			this.hazardNameText.TabIndex = 1;
			// 
			// hazardIDText
			// 
			this.hazardIDText.Location = new System.Drawing.Point(92, 13);
			this.hazardIDText.Name = "hazardIDText";
			this.hazardIDText.Size = new System.Drawing.Size(130, 20);
			this.hazardIDText.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Type:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 42);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Name:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 13);
			this.label6.TabIndex = 6;
			this.label6.Text = "Unique ID:";
			// 
			// RecordLocationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 150);
			this.Controls.Add(this.mainTabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RecordLocationForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Record current position";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecordLocationForm_FormClosing);
			this.mainTabs.ResumeLayout(false);
			this.recordPoiTab.ResumeLayout(false);
			this.recordPoiTab.PerformLayout();
			this.recordHazardTab.ResumeLayout(false);
			this.recordHazardTab.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl mainTabs;
		private System.Windows.Forms.TabPage recordPoiTab;
		private System.Windows.Forms.TabPage recordHazardTab;
		private System.Windows.Forms.TextBox poiNameText;
		private System.Windows.Forms.TextBox poiIDText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox poiTypeSelect;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button writePoiButton;
		private System.Windows.Forms.ComboBox hazardTypeSelect;
		private System.Windows.Forms.TextBox hazardNameText;
		private System.Windows.Forms.TextBox hazardIDText;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox hazardDescriptionText;
		private System.Windows.Forms.Button writeHazardButton;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label poiYLabel;
		private System.Windows.Forms.Label poiXLabel;
		private System.Windows.Forms.CheckBox lockPoiPositionCheck;
		private System.Windows.Forms.CheckBox lockHazardPositionCheck;
		private System.Windows.Forms.Label hazardYLabel;
		private System.Windows.Forms.Label hazardXLabel;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
	}
}