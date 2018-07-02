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
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace AlivieskaGpsClient
{
	// Drawing classes and methods
	public static class MapDrawing
	{
		#region DRAWING_GENERIC
		// Draw an image around its center
		public static void DrawImageCenter(Graphics g, Image image, Point center, Size size)
		{
			g.DrawImage(image, center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
		}

		// Draw a cross spanning the entire area, intersecting at given coordinates
		public static void DrawCross(Graphics g, Pen pen, float x, float y, Size size)
		{
			g.DrawLine(pen, 0, y, size.Width, y);
			g.DrawLine(pen, x, 0, x, size.Height);
		}

		// Draw a cross spanning the entire area, intersecting at a given point
		public static void DrawCross(Graphics g, Pen pen, Point intersect, Size size)
		{
			DrawCross(g, pen, intersect.X, intersect.Y, size);
		}

		// Vertices of the arrow polygon
		private static PointF[] _arrowPoints =
		{
			new Point(0, -10),
			new Point(7, 10),
			new Point(0, 5),
			new Point(-7, 10)
		};

		private static readonly PointF[] _arrowPointsTransformed = new PointF[4];

		// Draw an arrow, rotated
		public static void DrawArrow(Graphics g, PointF position, double angle, Point center, Size size)
		{
			double rad = angle * Math.PI / 180.0;
			float cos = (float)Math.Cos(rad);
			float sin = (float)Math.Sin(rad);

			for (int i = 0; i < _arrowPoints.Length; ++i)
			{
				float vx = _arrowPoints[i].X * cos - _arrowPoints[i].Y * sin;
				float vy = _arrowPoints[i].X * sin + _arrowPoints[i].Y * cos;
				_arrowPointsTransformed[i].X = vx + (position.X * size.Width + center.X);
				_arrowPointsTransformed[i].Y = vy + (position.Y * size.Height + center.Y);
			}

			g.FillPolygon(new SolidBrush(Color.Orange), _arrowPointsTransformed);
			g.DrawPolygon(new Pen(Color.Red, 1.75f), _arrowPointsTransformed);
		}
		#endregion

		#region POI
		// Properties of the circle to be drawn
		public class CircleStyle
		{
			public bool Stroke { get; set; }        // Should this circle be stroked?
			public Color StrokeColor { get; set; }  // Stroke line color
			public float StrokeWidth { get; set; }  // Stroke line thickness
			public bool Fill { get; set; }          // Should the circle be filled?
			public Color FillColor { get; set; }    // Fill color
			public float Radius { get; set; }       // Radius

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

			public static CircleStyle[] Presets { get; } =
			{
				new CircleStyle(true, Color.Blue, 1.0f, false, Color.Black, 7.5f),
				new CircleStyle(true, Color.Blue, 2.0f, true, Color.FromArgb(80, Color.MediumBlue), 20.0f),
				new CircleStyle(true, Color.DarkGreen, 1.0f, true, Color.Green, 7.5f),
				new CircleStyle(true, Color.Orange, 1.0f, true, Color.Yellow, 7.5f)
			};
			public static CircleStyle Default => Presets[0];
			public static CircleStyle Town => Presets[1];
			public static CircleStyle Work => Presets[2];
			public static CircleStyle Shop => Presets[3];
		}

		// Draw a circle around its center coordinates using the specified style
		public static void DrawCircle(Graphics g, int x, int y, CircleStyle style)
		{
			Rectangle rect = new Rectangle((int)(x - style.Radius), (int)(y - style.Radius), (int)(2 * style.Radius), (int)(2 * style.Radius));
			if (style.Fill)
			{
				g.FillEllipse(new SolidBrush(style.FillColor), rect);
			}
			if (style.Stroke)
			{
				g.DrawEllipse(new Pen(style.StrokeColor, style.StrokeWidth), rect);
			}
		}

		// Draw a circle around its center point using the specified style
		public static void DrawCircle(Graphics g, Point center, CircleStyle style)
		{
			DrawCircle(g, center.X, center.Y, style);
		}

		// A point on the map
		public abstract class MapPoint
		{
			public PointF Location { get; } // X, Y location within the -0.5..0.5 boundaries
			public float X => Location.X;   // X coordinate between the -0.5..0.5 boundaries
			public float Y => Location.Y;   // Y coordinate between the -0.5..0.5 boundaries
			public bool Enabled { get; set; } = true;   // Indicates whether the point should be visible and selectable

			// Point on the map at origin
			public MapPoint()
			{

			}

			// Point on the map defined by a point
			protected MapPoint(PointF location)
			{
				this.Location = location;
			}

			// Point on the map defined by a pair of coordinates
			protected MapPoint(float x, float y)
			{
				this.Location = new PointF(x, y);
			}

			// X coordinate mapped to center and size
			public int MapX(Point center, Size size)
			{
				return (int)(this.X * size.Width + center.X);
			}

			// Y coordinate mapped to center and size
			public int MapY(Point center, Size size)
			{
				return (int)(this.Y * size.Height + center.Y);
			}

			// Point mapped to center and size
			public Point MapLocation(Point center, Size size)
			{
				return new Point(MapX(center, size), MapY(center, size));
			}
		}

		// A point of interest on the map
		public class PointOfInterest : MapPoint, IComparable<PointOfInterest>, IEquatable<PointOfInterest>
		{
			public enum PointOfInterestType { Other = 0, Town = 1, Service = 2, Work = 3 }
			public CircleStyle Style { get; }
			public string Name { get; }
			public string ID { get; }
			public PointOfInterestType Type { get; }

			public PointOfInterest(string id, string name, PointF location, CircleStyle style, PointOfInterestType type) : base(location)
			{
				this.Style = style;
				//this.Location = location;
				this.Name = name;
				this.ID = id;
				this.Type = type;
			}

			// Draw a circle on an area defined by its center point and size
			public void Draw(Graphics g, Point center, Size size)
			{
				//DrawCircle(g, (int)(this.X * size.Width + center.X), (int)(this.Y * size.Height + center.Y), this.Style);
				Rectangle rect = new Rectangle((int)(MapX(center, size) - this.Style.Radius), (int)(MapY(center, size) - this.Style.Radius), (int)(2 * this.Style.Radius), (int)(2 * this.Style.Radius));
				if (this.Style.Fill)
				{
					g.FillEllipse(new SolidBrush(this.Style.FillColor), rect);
				}
				if (this.Style.Stroke)
				{
					g.DrawEllipse(new Pen(this.Style.StrokeColor, this.Style.StrokeWidth), rect);
				}
			}

			// Check if a point is within the circle's radius
			public bool InRange(PointF point, Point center, Size size)
			{
				double x = MapX(center, size) - point.X;
				double y = MapY(center, size) - point.Y;
				double distSq = Math.Pow(x, 2) + Math.Pow(y, 2);

				return distSq <= Math.Pow(this.Style.Radius, 2);
			}

			public static SortedSet<PointOfInterest> ReadFromCsv(string path)
			{
				SortedSet<PointOfInterest> set = new SortedSet<PointOfInterest>();
				TextFieldParser parser = new TextFieldParser(path);
				parser.SetDelimiters(";");
				while(!parser.EndOfData)
				{
					string[] tok = parser.ReadFields();
					set.Add(new PointOfInterest(tok[0].Trim(), tok[2].Trim(), new PointF(float.Parse(tok[3]), float.Parse(tok[4])), CircleStyle.Presets[int.Parse(tok[1])], (PointOfInterestType)int.Parse(tok[1])));
				}
				return set;
			}

			// Sort by type, then by ID
			public int CompareTo(PointOfInterest other)
			{
				if (this.Type != other.Type)
					return ((int)this.Type).CompareTo((int)other.Type);
				return this.ID.CompareTo(other.ID);
			}

			// Only the ID has to be unique
			public bool Equals(PointOfInterest other)
			{
				if (this.ID == other.ID)
					return true;
				return object.Equals(this, other);
			}
		}

		// Set of points of interest, sorted by their type.
		public static SortedSet<PointOfInterest> PointsOfInterest;

		// Draw all points of interest
		public static void DrawPointsOfInterest(Graphics g, Point center, Size size, bool drawTowns, bool drawJobs, bool drawShops, bool drawOthers)
		{
			if (PointsOfInterest != null)
				foreach (var poi in PointsOfInterest)
				{
					if ((poi.Type == PointOfInterest.PointOfInterestType.Town && drawTowns) || (poi.Type == PointOfInterest.PointOfInterestType.Work && drawJobs) || (poi.Type == PointOfInterest.PointOfInterestType.Service && drawShops) || (poi.Type == PointOfInterest.PointOfInterestType.Other && drawOthers))
						poi.Draw(g, center, size);
				}
		}
#endregion

		#region HAZARD
		// A class representing a road hazard. TBD: create an abstract class to be inherited by this and PointOfInterest?
		public class RoadHazard : MapPoint, IComparable<RoadHazard>, IEquatable<RoadHazard>
		{
			private static readonly Bitmap[] _icons = new Bitmap[]
			{
				new Bitmap("resources\\hazard_other.png"),
				new Bitmap("resources\\hazard_road.png"),
				new Bitmap("resources\\hazard_traffic.png"),
				new Bitmap("resources\\hazard_train_b.png"),
				new Bitmap("resources\\hazard_other.png")
			};
			public static Size IconSize = new Size(26, 26);
			public static Size HalfSize = new Size(IconSize.Width / 2, IconSize.Height / 2);

			public enum RoadHazardType { Other = 0, Topography = 1, Traffic = 2, Railway = 3, Police = 4 }
			public string ID { get; }
			public string Name { get; }
			public string Description { get; }
			public RoadHazardType Type { get; }
			public Bitmap Image { get { return _icons[(int)Type]; } }

			public RoadHazard(string id, string name, string description, PointF location, RoadHazardType type) : base(location)
			{
				this.ID = id;
				this.Name = name;
				this.Description = description;
				this.Type = type;
			}
			
			// Draw the hazard's icon at its center location
			public void Draw(Graphics g, Point center, Size size)
			{
				g.DrawImage(Image, new Rectangle(Point.Subtract(MapLocation(center, size), HalfSize), IconSize));
			}
			
			// Read all road hazards from a CSV file
			public static SortedSet<RoadHazard> ReadFromCsv(string path)
			{
				SortedSet<RoadHazard> set = new SortedSet<RoadHazard>();
				TextFieldParser parser = new TextFieldParser(path);
				parser.SetDelimiters(";");
				while(!parser.EndOfData)
				{
					string[] tok = parser.ReadFields();
					set.Add(new RoadHazard(tok[0].Trim(), tok[2].Trim(), tok[3].Trim(), new PointF(float.Parse(tok[4]), float.Parse(tok[5])), (RoadHazardType)int.Parse(tok[1])));
				}
				return set;
			}

			// Compare by type (reverse), then by ID
			public int CompareTo(RoadHazard other)
			{
				if (this.Type != other.Type)
					return ((int)other.Type).CompareTo((int)this.Type);
				return this.ID.CompareTo(other.ID);
			}

			// Make sure IDs are unique
			public bool Equals(RoadHazard other)
			{
				if (this.ID == other.ID)
					return true;
				return object.Equals(this, other);
			}
		}

		public static SortedSet<RoadHazard> Hazards;

		public static void DrawRoadHazards(Graphics g, Point center, Size size, bool drawTopography, bool drawTraffic, bool drawRailway, bool drawPolice, bool drawOthers)
		{
			if (Hazards != null)
			{
				foreach (var h in Hazards)
				{
					//if ((h.Type == RoadHazard.RoadHazardType.Topography && drawTopography) || (h.Type == RoadHazard.RoadHazardType.Traffic && drawTraffic) || (h.Type == RoadHazard.RoadHazardType.Railway && drawRailway) || (h.Type == RoadHazard.RoadHazardType.Police && drawPolice) || (h.Type == RoadHazard.RoadHazardType.Other && drawOthers))
					if (h.Enabled)
					{
						h.Draw(g, center, size);
					}
				}
			}
		}

		#endregion
	}
}
