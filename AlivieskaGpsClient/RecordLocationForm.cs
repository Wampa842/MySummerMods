/*
Copyright (C) 2018 Wampa842

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
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
				writer.WriteLine($"\"{id}\";{poiTypeSelect.SelectedIndex};\"{poiNameText.Text.Trim()}\";{Data.MapPosition.X};{Data.MapPosition.Y}");
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

		private void RecordLocationForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
	}
}
