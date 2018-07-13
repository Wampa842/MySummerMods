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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AlivieskaGpsClient
{
	// Form behaviour
	public partial class MainForm : Form
	{
		private readonly string _locationsPath = "resources\\locations.csv";    // A CSV file containing points of interest
		private readonly string _hazardsPath = "resources\\hazards.csv";        // A CSV file containing information about road hazards
		private readonly string _mapImagePath = "resources\\map.png";           // The map background image file
		private Bitmap _baseImage = null;                           // The image to display on the form
		private Point _imageCenter;                                 // The center coordinates of the image
		private int _prevX, _prevY;                                 // Used in calculating the delta position while panning the image
		private Size _imageSize;                                    // The magnification of the image
		private MapDrawing.PointOfInterest _hoverPoi = null;        // The point of interest the mouse cursor is over
		private MapDrawing.PointOfInterest _selectedPoi = null;     // A point of interest selected by clicking

		private MouseButtons _panButton = MouseButtons.Left;        // The mouse button that grabs and pans the map
		private MouseButtons _selectButton = MouseButtons.Right;    // The mouse button which selects points of interest on the map
		private MouseButtons _selectArbitraryButton = MouseButtons.Middle;  // The mouse button which selects an arbitrary point on the map

		private GpsData _gpsData;                                   // Object that handles the connection to the server
		private System.Timers.Timer _colorResetTimer = new System.Timers.Timer { AutoReset = false, Enabled = false, Interval = 250 };  // Timer that makes the light do the blinky

		private DetailsForm _detailsForm = new DetailsForm();		// Shows info about a selected location
		private RecordLocationForm _recordForm;                     // Allows the user to record their own locations
		private AboutForm _aboutForm = new AboutForm();				// Shows license and version information

		public MainForm()
		{
			InitializeComponent();
			_gpsData = new GpsData(this);
			//_recordForm.Data = _gpsData
			_recordForm = new RecordLocationForm(_gpsData);
			_colorResetTimer.Elapsed += (o, args) => connectionStatusLabel.ForeColor = Color.ForestGreen;
		}

		// Load resources; initialize pan and zoom
		private void MainForm_Load(object sender, EventArgs e)
		{
			_baseImage = new Bitmap(_mapImagePath);
			_imageCenter = new Point(mapImage.Width / 2, mapImage.Height / 2);
			_imageSize = new Size(mapImage.Width, mapImage.Height);

			MapDrawing.PointsOfInterest = MapDrawing.PointOfInterest.ReadFromCsv(_locationsPath);
			MapDrawing.Hazards = MapDrawing.RoadHazard.ReadFromCsv(_hazardsPath);

			connectionUrlText.Text = Settings.Default.Url;
			followCheck.Checked = Settings.Default.Follow;
			mapImage.Invalidate();
		}

		// Change the image magnification
		private void zoomSlider_Scroll(object sender, EventArgs e)
		{
			_imageSize.Width = _imageSize.Height = zoomSlider.Value * 4;
			zoomMultLabel.Text = ((TrackBar)sender).Value.ToString() + "%";
			mapImage.Invalidate();
		}

		// Display the image and points of interest
		private void mapImage_Paint(object sender, PaintEventArgs e)
		{
			MapDrawing.DrawImageCenter(e.Graphics, _baseImage, _imageCenter, _imageSize);
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			MapDrawing.DrawPointsOfInterest(e.Graphics, _imageCenter, _imageSize, displayTownsCheck.Checked, displayJobsCheck.Checked, displayShopsCheck.Checked, displayLocationsCheck.Checked);
			MapDrawing.DrawRoadHazards(e.Graphics, _imageCenter, _imageSize, displayRoadHazardsCheck.Checked, displayTrafficHazardsCheck.Checked, displayRailwayHazardsCheck.Checked, displayHazardsCheck.Checked, displayHazardsCheck.Checked);
			if (_selectedPoi != null)
				MapDrawing.DrawCross(e.Graphics, new Pen(Color.Black, 1.0f), _selectedPoi.MapLocation(_imageCenter, _imageSize), mapImage.Size);
			if (_gpsData.Success)
				MapDrawing.DrawArrow(e.Graphics, _gpsData.MapPosition, _gpsData.Heading, _imageCenter, _imageSize);
		}

		// Set up previous coordinates for panning and handle point selection
		private void mapImage_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == _panButton)
			{
				_prevX = e.X;
				_prevY = e.Y;
			}
			if (e.Button == _selectButton)
			{
				bool inRange = false;
				if (MapDrawing.PointsOfInterest != null)
				{
					foreach (var poi in MapDrawing.PointsOfInterest.Reverse())
					{
						if (poi.Enabled && poi.InRange(e.Location, _imageCenter, _imageSize))
						{
							inRange = true;
							_selectedPoi = poi;
							_detailsForm.UpdateDetails(poi);
							break;
						}
					}
				}
				if (!inRange)
				{
					_selectedPoi = null;
				}
				else
				{

				}
			}
			if (e.Button == _selectArbitraryButton)
			{
				//float x = (e.X - _imageCenter.X) / (float)(zoomSlider.Value * 4);
				//float y = (e.Y - _imageCenter.Y) / (float)(zoomSlider.Value * 4);
				//gpsDataX.Text = x.ToString();
				//gpsDataZ.Text = y.ToString();
				//using (System.IO.StreamWriter writer = new System.IO.StreamWriter("locations.csv", true))
				//	writer.WriteLineAsync($"\"{outputName.Text}\",\"{x}\",\"{y}\"");
			}
			((PictureBox)sender).Invalidate();
		}

		// Pan the image and handle hovering
		private void mapImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == _panButton)
			{
				_imageCenter.X += e.X - _prevX;
				_imageCenter.Y += e.Y - _prevY;

				((PictureBox)sender).Invalidate();

				_prevX = e.X;
				_prevY = e.Y;
			}
			bool inRange = false;
			if (MapDrawing.PointsOfInterest != null)
			{
				foreach (var poi in MapDrawing.PointsOfInterest.Reverse())
				{
					if (poi.Enabled && poi.InRange(e.Location, _imageCenter, _imageSize))
					{
						_hoverPoi = poi;
						inRange = true;
						break;
					}
				}
			}
			((PictureBox)sender).Cursor = inRange ? Cursors.Hand : Cursors.SizeAll;
			if (!inRange)
			{
				_hoverPoi = null;
			}
		}

		// Connect to or disconnect from the server
		private void gpsConnectButton_Click(object sender, EventArgs e)
		{
			// If the timer is running, stop it and enable editing the URL text box.
			if (gpsUpdateTimer.Enabled)
			{
				gpsUpdateTimer.Stop();
				connectionUrlText.Enabled = true;
				gpsConnectButton.Text = "Connect";
				connectionStatusLabel.ForeColor = Color.DarkGray;
			}
			// If not, lock the text box and start the timer.
			else
			{
				gpsUpdateTimer.Start();
				connectionUrlText.Enabled = false;
				gpsConnectButton.Text = "Disconnect";
				connectionStatusLabel.ForeColor = Color.Blue;
			}
		}

		// Periodically request data from the server
		private void gpsUpdateTimer_Tick(object sender, EventArgs e)
		{
			_gpsData.Get(connectionUrlText.Text);
		}

		// Reset pan and zoom
		private void resetMapButton_Click(object sender, EventArgs e)
		{
			_imageCenter = new Point(mapImage.Width / 2, mapImage.Height / 2);
			_imageSize = new Size(mapImage.Width, mapImage.Height);
			zoomSlider.Value = zoomSlider.Minimum;
			zoomMultLabel.Text = zoomSlider.Value + "%";
			mapImage.Invalidate();
		}

		// Set URL to http://localhost:8080/
		private void default8080Button_Click(object sender, EventArgs e)
		{
			if (connectionUrlText.Enabled)
				connectionUrlText.Text = "http://localhost:8080/";
		}

		// Save config and close other forms when the window is closed
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Default.Follow = followCheck.Checked;
			Settings.Default.Url = connectionUrlText.Text;
			Settings.Default.Save();
			_detailsForm.Close();
			_recordForm.Close();
		}

		// Set URL to http://localhost/gps/
		private void default80Button_Click(object sender, EventArgs e)
		{
			if (connectionUrlText.Enabled)
				connectionUrlText.Text = "http://localhost/gps/";
		}

		// Open/close Details form
		private void showDetailsButton_Click(object sender, EventArgs e)
		{
			if (_detailsForm.Visible)
			{
				_detailsForm.Hide();
				((Button)sender).Text = "Details >>";
			}
			else
			{
				_detailsForm.Show();
				((Button)sender).Text = "Hide <<";
			}
		}

		// Update location visibility
		private void poiDisplayCheck_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				foreach (var poi in MapDrawing.PointsOfInterest)
				{
					if (poi.Type == MapDrawing.PointOfInterest.PointOfInterestType.Town)
						poi.Enabled = displayTownsCheck.Checked;
					if (poi.Type == MapDrawing.PointOfInterest.PointOfInterestType.Service)
						poi.Enabled = displayShopsCheck.Checked;
					if (poi.Type == MapDrawing.PointOfInterest.PointOfInterestType.Work)
						poi.Enabled = displayJobsCheck.Checked;
				}
				foreach (var poi in MapDrawing.Hazards)
				{
					if (poi.Type == MapDrawing.RoadHazard.RoadHazardType.Topography)
						poi.Enabled = displayRoadHazardsCheck.Checked;
					if (poi.Type == MapDrawing.RoadHazard.RoadHazardType.Traffic)
						poi.Enabled = displayTrafficHazardsCheck.Checked;
					if (poi.Type == MapDrawing.RoadHazard.RoadHazardType.Railway)
						poi.Enabled = displayRailwayHazardsCheck.Checked;
				}
				mapImage.Invalidate();
			}
			catch { } // Suppress a NullReferenceException that might occur if the config is loaded before the locations data. Stopgap - should fix soon.
		}

		// Uncheck/check all points of interest
		private void displayLocationsCheck_CheckedChanged(object sender, EventArgs e)
		{
			displayTownsCheck.Checked = displayTownsCheck.Enabled = ((CheckBox)sender).Checked;
			displayShopsCheck.Checked = displayShopsCheck.Enabled = ((CheckBox)sender).Checked;
			displayJobsCheck.Checked = displayJobsCheck.Enabled = ((CheckBox)sender).Checked;
			poiDisplayCheck_CheckedChanged(sender, e);
		}

		// Uncheck/check all hazards
		private void displayHazardsCheck_CheckedChanged(object sender, EventArgs e)
		{
			displayRoadHazardsCheck.Checked = displayRoadHazardsCheck.Enabled = ((CheckBox)sender).Checked;
			displayTrafficHazardsCheck.Checked = displayTrafficHazardsCheck.Enabled = ((CheckBox)sender).Checked;
			displayRailwayHazardsCheck.Checked = displayRailwayHazardsCheck.Enabled = ((CheckBox)sender).Checked;
			poiDisplayCheck_CheckedChanged(sender, e);
		}

		// Open/close the location recorder window
		private void showRecordButton_Click(object sender, EventArgs e)
		{
			if(_recordForm.Visible)
			{
				_recordForm.Hide();
			}
			else
			{
				_recordForm.Location = Point.Add(this.Location, new Size(0, this.Height));
				_recordForm.Show();
			}
		}

		// Keep sub-forms aligned
		private void MainForm_Move(object sender, EventArgs e)
		{
			_recordForm.Location = Point.Add(this.Location, new Size(0, this.Height));
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			_aboutForm.ShowDialog();
		}

		// Update the form to display whatever data is currently present
		public void UpdateGpsData()
		{
			if (gpsUpdateTimer.Enabled)
			{
				if (_gpsData.Success)
				{
					gpsDataX.Text = _gpsData.X.ToString();
					gpsDataY.Text = _gpsData.Y.ToString();
					gpsDataZ.Text = _gpsData.Z.ToString();
					gpsDataHeading.Text = _gpsData.Heading.ToString();
					gpsDataSpeed.Text = _gpsData.Speed.ToString();

					connectionStatusLabel.ForeColor = Color.LimeGreen;
					_colorResetTimer.Start();
				}
				else
				{
					connectionStatusLabel.ForeColor = Color.Red;
				}
			}
			if (followCheck.Checked)
			{
				PointF p = _gpsData.MapPosition;
				_imageCenter.X = (int)(-p.X * _imageSize.Width + mapImage.Width / 2);
				_imageCenter.Y = (int)(-p.Y * _imageSize.Height + mapImage.Height / 2);
			}
			mapImage.Invalidate();
			_recordForm.UpdateDisplay();
		}
	}
}
