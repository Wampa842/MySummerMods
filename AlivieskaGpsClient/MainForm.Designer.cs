namespace AlivieskaGpsClient
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.zoomSlider = new System.Windows.Forms.TrackBar();
			this.connectionUrlText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gpsConnectionBox = new System.Windows.Forms.GroupBox();
			this.default8080Button = new System.Windows.Forms.Button();
			this.connectionStatusLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.gpsDataHeading = new System.Windows.Forms.TextBox();
			this.gpsDataSpeed = new System.Windows.Forms.TextBox();
			this.gpsDataY = new System.Windows.Forms.TextBox();
			this.gpsDataZ = new System.Windows.Forms.TextBox();
			this.gpsDataX = new System.Windows.Forms.TextBox();
			this.gpsConnectButton = new System.Windows.Forms.Button();
			this.mapImage = new System.Windows.Forms.PictureBox();
			this.zoomMultLabel = new System.Windows.Forms.Label();
			this.selectedPoiBox = new System.Windows.Forms.GroupBox();
			this.selectedPoiNameLabel = new System.Windows.Forms.Label();
			this.gpsUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.resetMapButton = new System.Windows.Forms.Button();
			this.followCheck = new System.Windows.Forms.CheckBox();
			this.default80Button = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.zoomSlider)).BeginInit();
			this.gpsConnectionBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImage)).BeginInit();
			this.selectedPoiBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// zoomSlider
			// 
			this.zoomSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.zoomSlider.LargeChange = 50;
			this.zoomSlider.Location = new System.Drawing.Point(418, 64);
			this.zoomSlider.Maximum = 400;
			this.zoomSlider.Minimum = 100;
			this.zoomSlider.Name = "zoomSlider";
			this.zoomSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.zoomSlider.Size = new System.Drawing.Size(45, 318);
			this.zoomSlider.SmallChange = 25;
			this.zoomSlider.TabIndex = 1;
			this.zoomSlider.TickFrequency = 10;
			this.zoomSlider.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.zoomSlider.Value = 100;
			this.zoomSlider.Scroll += new System.EventHandler(this.zoomSlider_Scroll);
			// 
			// connectionUrlText
			// 
			this.connectionUrlText.Location = new System.Drawing.Point(6, 41);
			this.connectionUrlText.Name = "connectionUrlText";
			this.connectionUrlText.Size = new System.Drawing.Size(158, 20);
			this.connectionUrlText.TabIndex = 2;
			this.connectionUrlText.Text = "http://localhost/gps/";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Server address:";
			// 
			// gpsConnectionBox
			// 
			this.gpsConnectionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gpsConnectionBox.Controls.Add(this.default80Button);
			this.gpsConnectionBox.Controls.Add(this.default8080Button);
			this.gpsConnectionBox.Controls.Add(this.connectionStatusLabel);
			this.gpsConnectionBox.Controls.Add(this.label6);
			this.gpsConnectionBox.Controls.Add(this.label5);
			this.gpsConnectionBox.Controls.Add(this.label4);
			this.gpsConnectionBox.Controls.Add(this.label3);
			this.gpsConnectionBox.Controls.Add(this.label2);
			this.gpsConnectionBox.Controls.Add(this.gpsDataHeading);
			this.gpsConnectionBox.Controls.Add(this.gpsDataSpeed);
			this.gpsConnectionBox.Controls.Add(this.gpsDataY);
			this.gpsConnectionBox.Controls.Add(this.gpsDataZ);
			this.gpsConnectionBox.Controls.Add(this.gpsDataX);
			this.gpsConnectionBox.Controls.Add(this.gpsConnectButton);
			this.gpsConnectionBox.Controls.Add(this.label1);
			this.gpsConnectionBox.Controls.Add(this.connectionUrlText);
			this.gpsConnectionBox.Location = new System.Drawing.Point(469, 12);
			this.gpsConnectionBox.Name = "gpsConnectionBox";
			this.gpsConnectionBox.Size = new System.Drawing.Size(170, 230);
			this.gpsConnectionBox.TabIndex = 4;
			this.gpsConnectionBox.TabStop = false;
			this.gpsConnectionBox.Text = "Server connection";
			// 
			// default8080Button
			// 
			this.default8080Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.default8080Button.Location = new System.Drawing.Point(90, 17);
			this.default8080Button.Name = "default8080Button";
			this.default8080Button.Size = new System.Drawing.Size(36, 21);
			this.default8080Button.TabIndex = 15;
			this.default8080Button.Text = ":8080";
			this.default8080Button.UseVisualStyleBackColor = true;
			this.default8080Button.Click += new System.EventHandler(this.default8080Button_Click);
			// 
			// connectionStatusLabel
			// 
			this.connectionStatusLabel.AutoSize = true;
			this.connectionStatusLabel.Font = new System.Drawing.Font("Webdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.connectionStatusLabel.ForeColor = System.Drawing.Color.DarkGray;
			this.connectionStatusLabel.Location = new System.Drawing.Point(145, 70);
			this.connectionStatusLabel.Name = "connectionStatusLabel";
			this.connectionStatusLabel.Size = new System.Drawing.Size(19, 17);
			this.connectionStatusLabel.TabIndex = 1;
			this.connectionStatusLabel.Text = "n";
			this.connectionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 177);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Speed";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 203);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(47, 13);
			this.label5.TabIndex = 13;
			this.label5.Text = "Heading";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 151);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "Height";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(14, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Y";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 99);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(14, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "X";
			// 
			// gpsDataHeading
			// 
			this.gpsDataHeading.Location = new System.Drawing.Point(72, 200);
			this.gpsDataHeading.Name = "gpsDataHeading";
			this.gpsDataHeading.ReadOnly = true;
			this.gpsDataHeading.Size = new System.Drawing.Size(92, 20);
			this.gpsDataHeading.TabIndex = 10;
			this.gpsDataHeading.Text = "012";
			this.gpsDataHeading.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gpsDataSpeed
			// 
			this.gpsDataSpeed.Location = new System.Drawing.Point(72, 174);
			this.gpsDataSpeed.Name = "gpsDataSpeed";
			this.gpsDataSpeed.ReadOnly = true;
			this.gpsDataSpeed.Size = new System.Drawing.Size(92, 20);
			this.gpsDataSpeed.TabIndex = 9;
			this.gpsDataSpeed.Text = "012";
			this.gpsDataSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gpsDataY
			// 
			this.gpsDataY.Location = new System.Drawing.Point(72, 148);
			this.gpsDataY.Name = "gpsDataY";
			this.gpsDataY.ReadOnly = true;
			this.gpsDataY.Size = new System.Drawing.Size(92, 20);
			this.gpsDataY.TabIndex = 8;
			this.gpsDataY.Text = "012";
			this.gpsDataY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gpsDataZ
			// 
			this.gpsDataZ.Location = new System.Drawing.Point(72, 122);
			this.gpsDataZ.Name = "gpsDataZ";
			this.gpsDataZ.ReadOnly = true;
			this.gpsDataZ.Size = new System.Drawing.Size(92, 20);
			this.gpsDataZ.TabIndex = 7;
			this.gpsDataZ.Text = "012";
			this.gpsDataZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gpsDataX
			// 
			this.gpsDataX.Location = new System.Drawing.Point(72, 96);
			this.gpsDataX.Name = "gpsDataX";
			this.gpsDataX.ReadOnly = true;
			this.gpsDataX.Size = new System.Drawing.Size(92, 20);
			this.gpsDataX.TabIndex = 6;
			this.gpsDataX.Text = "012";
			this.gpsDataX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gpsConnectButton
			// 
			this.gpsConnectButton.Location = new System.Drawing.Point(6, 67);
			this.gpsConnectButton.Name = "gpsConnectButton";
			this.gpsConnectButton.Size = new System.Drawing.Size(133, 23);
			this.gpsConnectButton.TabIndex = 5;
			this.gpsConnectButton.Text = "Connect";
			this.gpsConnectButton.UseVisualStyleBackColor = true;
			this.gpsConnectButton.Click += new System.EventHandler(this.gpsConnectButton_Click);
			// 
			// mapImage
			// 
			this.mapImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mapImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mapImage.InitialImage = global::AlivieskaGpsClient.Properties.Resources.map;
			this.mapImage.Location = new System.Drawing.Point(12, 12);
			this.mapImage.Name = "mapImage";
			this.mapImage.Size = new System.Drawing.Size(400, 400);
			this.mapImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.mapImage.TabIndex = 5;
			this.mapImage.TabStop = false;
			this.mapImage.Paint += new System.Windows.Forms.PaintEventHandler(this.mapImage_Paint);
			this.mapImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapImage_MouseDown);
			this.mapImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapImage_MouseMove);
			// 
			// zoomMultLabel
			// 
			this.zoomMultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.zoomMultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.zoomMultLabel.Location = new System.Drawing.Point(418, 37);
			this.zoomMultLabel.Name = "zoomMultLabel";
			this.zoomMultLabel.Size = new System.Drawing.Size(45, 24);
			this.zoomMultLabel.TabIndex = 6;
			this.zoomMultLabel.Text = "100%";
			this.zoomMultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// selectedPoiBox
			// 
			this.selectedPoiBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectedPoiBox.Controls.Add(this.selectedPoiNameLabel);
			this.selectedPoiBox.Location = new System.Drawing.Point(469, 248);
			this.selectedPoiBox.Name = "selectedPoiBox";
			this.selectedPoiBox.Size = new System.Drawing.Size(170, 164);
			this.selectedPoiBox.TabIndex = 7;
			this.selectedPoiBox.TabStop = false;
			this.selectedPoiBox.Text = "Selected";
			// 
			// selectedPoiNameLabel
			// 
			this.selectedPoiNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.selectedPoiNameLabel.Location = new System.Drawing.Point(6, 16);
			this.selectedPoiNameLabel.Name = "selectedPoiNameLabel";
			this.selectedPoiNameLabel.Size = new System.Drawing.Size(158, 145);
			this.selectedPoiNameLabel.TabIndex = 0;
			this.selectedPoiNameLabel.Text = "label7";
			// 
			// gpsUpdateTimer
			// 
			this.gpsUpdateTimer.Interval = 1000;
			this.gpsUpdateTimer.Tick += new System.EventHandler(this.gpsUpdateTimer_Tick);
			// 
			// resetMapButton
			// 
			this.resetMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.resetMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.resetMapButton.Location = new System.Drawing.Point(418, 388);
			this.resetMapButton.Name = "resetMapButton";
			this.resetMapButton.Size = new System.Drawing.Size(45, 24);
			this.resetMapButton.TabIndex = 8;
			this.resetMapButton.Text = "reset";
			this.resetMapButton.Click += new System.EventHandler(this.resetMapButton_Click);
			// 
			// followCheck
			// 
			this.followCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.followCheck.Appearance = System.Windows.Forms.Appearance.Button;
			this.followCheck.Location = new System.Drawing.Point(418, 12);
			this.followCheck.Name = "followCheck";
			this.followCheck.Size = new System.Drawing.Size(45, 22);
			this.followCheck.TabIndex = 9;
			this.followCheck.Text = "Follow";
			this.followCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.followCheck.UseVisualStyleBackColor = true;
			// 
			// default80Button
			// 
			this.default80Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.default80Button.Location = new System.Drawing.Point(128, 17);
			this.default80Button.Name = "default80Button";
			this.default80Button.Size = new System.Drawing.Size(36, 21);
			this.default80Button.TabIndex = 16;
			this.default80Button.Text = "/gps";
			this.default80Button.UseVisualStyleBackColor = true;
			this.default80Button.Click += new System.EventHandler(this.default80Button_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(651, 424);
			this.Controls.Add(this.followCheck);
			this.Controls.Add(this.resetMapButton);
			this.Controls.Add(this.selectedPoiBox);
			this.Controls.Add(this.zoomMultLabel);
			this.Controls.Add(this.mapImage);
			this.Controls.Add(this.gpsConnectionBox);
			this.Controls.Add(this.zoomSlider);
			this.DoubleBuffered = true;
			this.MinimumSize = new System.Drawing.Size(667, 462);
			this.Name = "MainForm";
			this.Text = "Alivieska GPS client";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.zoomSlider)).EndInit();
			this.gpsConnectionBox.ResumeLayout(false);
			this.gpsConnectionBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImage)).EndInit();
			this.selectedPoiBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TrackBar zoomSlider;
		private System.Windows.Forms.TextBox connectionUrlText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox gpsConnectionBox;
		private System.Windows.Forms.Button gpsConnectButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox mapImage;
		private System.Windows.Forms.Label zoomMultLabel;
		private System.Windows.Forms.GroupBox selectedPoiBox;
		private System.Windows.Forms.Label selectedPoiNameLabel;
		private System.Windows.Forms.Label connectionStatusLabel;
		private System.Windows.Forms.Timer gpsUpdateTimer;
		private System.Windows.Forms.TextBox gpsDataHeading;
		private System.Windows.Forms.TextBox gpsDataSpeed;
		private System.Windows.Forms.TextBox gpsDataY;
		private System.Windows.Forms.TextBox gpsDataZ;
		private System.Windows.Forms.TextBox gpsDataX;
		private System.Windows.Forms.Button resetMapButton;
		private System.Windows.Forms.Button default8080Button;
		private System.Windows.Forms.CheckBox followCheck;
		private System.Windows.Forms.Button default80Button;
	}
}

