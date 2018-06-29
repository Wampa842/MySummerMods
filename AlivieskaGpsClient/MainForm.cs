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
using System.Net.Http;

namespace AlivieskaGpsClient
{
	public partial class MainForm : Form
	{
		private string _mapImagePath = "resources\\map.png";
		private Bitmap _baseImage = null;
		private Point _imageCenter;
		private int _prevX, _prevY;
		private Size _imageSize;

		private HttpClient _httpClient = null;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			_baseImage = new Bitmap(_mapImagePath);
			_imageCenter = new Point(mapImage.Width / 2, mapImage.Height / 2);
			_imageSize = new Size(mapImage.Width, mapImage.Height);

			mapImage.Invalidate();
		}

		private void zoomSlider_Scroll(object sender, EventArgs e)
		{
			_imageSize.Width = _imageSize.Height = zoomSlider.Value * 4;
			zoomMultLabel.Text = ((TrackBar)sender).Value.ToString() + "%";
			mapImage.Invalidate();
		}

		private void mapImage_Paint(object sender, PaintEventArgs e)
		{
			MapDrawing.DrawImageCenter(e.Graphics, _baseImage, _imageCenter, _imageSize);
			MapDrawing.DrawPointsOfInterest(e.Graphics, _imageCenter, _imageSize);

		}

		private void mapImage_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_prevX = e.X;
				_prevY = e.Y;
			}
			if (e.Button == MouseButtons.Right)
			{
				float x = (e.X - _imageCenter.X) / (float)(zoomSlider.Value * 4);
				float y = (e.Y - _imageCenter.Y) / (float)(zoomSlider.Value * 4);
				gpsDataX.Text = x.ToString();
				gpsDataZ.Text = y.ToString();
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter("locations.csv", true))
					writer.WriteLineAsync($"\"{outputName.Text}\",\"{x}\",\"{y}\"");
			}
		}

		private void mapImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_imageCenter.X += e.X - _prevX;
				_imageCenter.Y += e.Y - _prevY;

				((PictureBox)sender).Invalidate();

				_prevX = e.X;
				_prevY = e.Y;
			}
		}

		private void gpsConnectButton_Click(object sender, EventArgs e)
		{
			if(_httpClient == null)
			{
				((Button)sender).Text = "Disconnect";
			}
			else
			{
				((Button)sender).Text = "Connect";
			}
		}

		private void gpsUpdateTimer_Tick(object sender, EventArgs e)
		{
			if(_httpClient != null)
			{

			}
		}
	}

	public static class MapDrawing
	{
		public static void DrawImageCenter(Graphics g, Image image, Point center, Size size)
		{
			g.DrawImage(image, center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
		}

		public class CircleStyle
		{
			public bool Stroke { get; set; }
			public Color StrokeColor { get; set; }
			public float StrokeWidth { get; set; }
			public bool Fill { get; set; }
			public Color FillColor { get; set; }
			public float Radius { get; set; }

			public CircleStyle(bool stroke, Color strokeColor, float strokeWidth, bool fill, Color fillColor, float radius)
			{
				this.Stroke = stroke;
				this.StrokeColor = strokeColor;
				this.StrokeWidth = strokeWidth;
				this.Fill = fill;
				this.FillColor = fillColor;
				this.Radius = radius;
			}

			public CircleStyle() : this(true, Color.FromArgb(255, 0, 0, 0), 1.0f, true, Color.FromArgb(127, 0, 0, 0), 10) { }
		}

		public static void DrawCircle(Graphics g, int x, int y, CircleStyle style)
		{
			Rectangle rect = new Rectangle((int)(x - style.Radius), (int)(y - style.Radius), (int)(2 * style.Radius), (int)(2 * style.Radius));
			if (style.Fill)
			{
				g.DrawEllipse(new Pen(new SolidBrush(style.FillColor)), rect);
			}
			if (style.Stroke)
			{
				g.DrawEllipse(new Pen(style.StrokeColor, style.StrokeWidth), rect);
			}
		}

		public static void DrawCircle(Graphics g, Point center, CircleStyle style)
		{
			DrawCircle(g, center.X, center.Y, style);
		}

		public class PointOfInterest : IComparable<PointOfInterest>, IEquatable<PointOfInterest>
		{
			public enum PointOfInterestType { Other = 1, Town = 2, Service = 4, Work = 8 }
			public CircleStyle Style { get; }
			public PointF Location { get; }
			public float X { get { return Location.X; } }
			public float Y { get { return Location.Y; } }
			public string Name { get; }
			public string ID { get; }
			public PointOfInterestType Type { get; }

			public PointOfInterest(string id, string name, PointF location, CircleStyle style, PointOfInterestType type)
			{
				this.Style = style;
				this.Location = location;
				this.Name = name;
				this.ID = id;
				this.Type = type;
			}

			public void Draw(Graphics g, Point center, Size size)
			{
				DrawCircle(g, new Point((int)(), (int)(0)), this.Style);
			}

			public PointOfInterest FromString(string line)
			{
				string[] tok = line.Split(',');

				return new PointOfInterest(tok[0].Trim('"'), tok[2].Trim('"'), )
			}

			public int CompareTo(PointOfInterest other)
			{
				//return this.GetHashCode().CompareTo(other.GetHashCode());
				return ((int)this.Type).CompareTo((int)other.Type);
			}

			public bool Equals(PointOfInterest other)
			{
				return this.ID == other.ID;
			}
		}

		public static SortedSet<PointOfInterest> PointsOfInterest = new SortedSet<PointOfInterest>
		{
			new PointOfInterest("home", "Kesselinperä", new PointF(0.4414f, 0.4581f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town)
			//new PointOfInterest("perajarvi", "Peräjärvi", new PointF(0.909f, 0.8175f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town),
			//new PointOfInterest("loppe", "Loppe", new PointF(0.817f, 0.6659f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town)
		};

		public static void DrawPointsOfInterest(Graphics g, Point center, Size size)
		{
			Point p = new Point(center.X, center.Y);
			DrawCircle(g, p, new CircleStyle());
			p = new Point((int)(0.3 * size.Width + center.X), (int)(0.3 * size.Height + center.Y));
			DrawCircle(g, p, new CircleStyle());
		}
	}
}
