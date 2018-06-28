/*
AlivieskaGpsClient is NOT part of the mod pack and is licensed separately.
Connection between this and AlivieskaGpsServer happens exclusively through HTTP and it is only in the same repository for convenience.

Copyright (c) 2018 Wampa842

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
using Svg;
using Svg.Transforms;

namespace AlivieskaGpsClient
{
	public partial class MainForm : Form
	{
		private float _prevX, _prevY;
		private Point _circlePos;
		private SvgDocument _svgDoc = null;
		private SvgScale _svgScale;
		private SvgTranslate _svgPan;
		private string _svgPath = "resources\\map.svg";

		private void _initSvg()
		{
			return;
			if(_svgDoc == null)
				_svgDoc = SvgDocument.Open(_svgPath);
			_svgScale = new SvgScale(zoomSlider.Value / 100);
			_svgPan = new SvgTranslate(0.0f);
			_svgDoc.Transforms = new SvgTransformCollection { _svgPan, _svgScale };
			_svgDoc.Width = new SvgUnit(SvgUnitType.Pixel, mapImage.Width);
			_svgDoc.Height = new SvgUnit(SvgUnitType.Pixel, mapImage.Height);
			_svgDoc.X = 0;
			_svgDoc.Y = 0;
			//mapImage.Invalidate();
		}

		public MainForm()
		{
			InitializeComponent();
		}

		private void mapImage_Click(object sender, EventArgs e)
		{
			//_circlePos = ((MouseEventArgs)e).Location;
			//gpsDataX.Text = _circlePos.X.ToString();
			//gpsDataZ.Text = _circlePos.Y.ToString();
			//((PictureBox)sender).Invalidate();
		}

		private void mapImage_Paint(object sender, PaintEventArgs e)
		{
			if (_svgDoc != null)
			{
				((PictureBox)sender).Image = _svgDoc.Draw();
			}
			MapDrawing.DrawCircle(e.Graphics, _circlePos, new MapDrawing.CircleStyle());
		}

		private void MainForm_Validated(object sender, EventArgs e)
		{
			if (_svgDoc != null)
			{
				gpsDataX.Text = _svgPan.X.ToString();
				gpsDataZ.Text = _svgPan.Y.ToString();
				gpsDataY.Text = _svgScale.X.ToString();
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += (obj, args) => { _initSvg(); };
			worker.RunWorkerAsync();
			worker.RunWorkerCompleted += (obj, args) => { ((BackgroundWorker)obj).Dispose(); };
		}

		private void zoomSlider_Scroll(object sender, EventArgs e)
		{
			_svgScale.X = _svgScale.Y = ((TrackBar)sender).Value / 100.0f;
		}

		private void mapImage_MouseDown(object sender, MouseEventArgs e)
		{
			_prevX = e.X;
			_prevY = e.Y;
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			_initSvg();
		}

		private void mapImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//_svgPan.X += e.X - _prevX;
				//_prevX = e.X;

				//_svgPan.Y += e.Y - _prevY;
				//_prevY = e.Y;
				_circlePos = e.Location;
			}
			mapImage.Invalidate();
		}
	}

	public static class MapDrawing
	{
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
			new PointOfInterest("home", "Kesselinperä", new PointF(0.4414f, 0.4581f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town),
			new PointOfInterest("perajarvi", "Peräjärvi", new PointF(0.909f, 0.8175f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town),
			new PointOfInterest("loppe", "Loppe", new PointF(0.817f, 0.6659f), new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(127, Color.Blue), 20.0f), PointOfInterest.PointOfInterestType.Town)
		};

		public static void DrawPointsOfInterest(Graphics g)
		{
			foreach (var poi in PointsOfInterest)
			{
				Point p = new Point();
				DrawCircle(g, p, poi.Style);
			}
		}
	}
}
