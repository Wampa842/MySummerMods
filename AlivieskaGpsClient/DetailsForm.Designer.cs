namespace AlivieskaGpsClient
{
	partial class DetailsForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.titleLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.Location = new System.Drawing.Point(12, 387);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(246, 54);
			this.label1.TabIndex = 0;
			this.label1.Text = "TODO";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// titleLabel
			// 
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.titleLabel.Location = new System.Drawing.Point(12, 9);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(246, 21);
			this.titleLabel.TabIndex = 1;
			this.titleLabel.Text = "Title";
			this.titleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// DetailsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(270, 450);
			this.Controls.Add(this.titleLabel);
			this.Controls.Add(this.label1);
			this.Name = "DetailsForm";
			this.ShowIcon = false;
			this.Text = "Details";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label titleLabel;
	}
}