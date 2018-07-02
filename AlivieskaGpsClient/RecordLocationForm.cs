using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AlivieskaGpsClient
{
	public partial class RecordLocationForm : Form
	{
		private readonly string _poiOutPath = "resources\\locations_custom.csv";
		private readonly string _hazardOutPath = "resources\\hazards_custom.csv";
		public GpsData Data { get; set; }
		public RecordLocationForm(GpsData data)
		{
			InitializeComponent();
			Data = data;
			poiTypeSelect.SelectedIndex = 0;
			hazardTypeSelect.SelectedIndex = 0;
		}

		private void writePoiButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(poiIDText.Text))
				return;
			string id = poiIDText.Text.Trim().Replace(' ', '_').Replace(',', ';');
			if(MapDrawing.PointsOfInterest.Any(poi => poi.ID == id))
			{
				MessageBox.Show($"The identifier '{poiIDText.Text}' already exists.");
				return;
			}
			using (StreamWriter writer = new StreamWriter(_poiOutPath, true))
			{
				writer.WriteLine($"\"{id}\",{poiTypeSelect.SelectedIndex},\"{poiNameText.Text.Trim()}\",{Data.MapPosition.X},{Data.MapPosition.Y}");
			}
		}

		private void writeHazardButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(hazardIDText.Text))
				return;
			string id = hazardIDText.Text.Trim().Replace(' ', '_').Replace(',', ';');
			if (MapDrawing.Hazards.Any(poi => poi.ID == id))
			{
				MessageBox.Show($"The identifier '{hazardIDText.Text}' already exists.");
				return;
			}
			using (StreamWriter writer = new StreamWriter(_hazardOutPath))
			{
				writer.WriteLine($"\"{id}\",{hazardTypeSelect.SelectedIndex},\"{hazardNameText.Text.Trim()}\",\"{hazardDescriptionText.Text.Trim()}\",{Data.MapPosition.X},{Data.MapPosition.Y}");
			}
		}
	}
}
