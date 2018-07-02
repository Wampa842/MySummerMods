using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlivieskaGpsClient
{
	public partial class DetailsForm : Form
	{
		public DetailsForm()
		{
			InitializeComponent();
		}

		public void UpdateDetails(MapDrawing.PointOfInterest poi)
		{
			titleLabel.Text = poi.Name;
		}

		private void DetailsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
	}
}
