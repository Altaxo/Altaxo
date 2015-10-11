#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Poly2Tri;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Graph3D.Primitives
{
	public class Text3D
	{
		private string _text = "Käppi";
		private double _fontSize = 12;
		private double _depth = 3;
		private FontX3D _font;
		private const double distanceCutThreshold = 0.0001;

		private List<Poly2Tri.Polygon> _polygons = new List<Poly2Tri.Polygon>();

		public Text3D()
		{
		}

		public Text3D(string text, FontX3D font)
		{
			_text = text;
			_fontSize = font.Size;
			_depth = font.Depth;
			_font = font;
		}

		/* This method determines whether the points an ArrayList of SDEPoint objects are clockwise or anti-clockwise.
  * It does it by calculating the area using this alogrithm:
  * Area = Area + (X2 - X1) * (Y2 + Y1) / 2
 * If the area is positive then it is anti-clockwise, otherwise it is clockwise.
 * @param ptArrayList an ArrayList of SDEPoint objects likely to be used to generate a polygon
	*/

		private bool isClockwise(List<PolygonPoint> ptArrayList)
		{
			return GetPolygonArea(ptArrayList) < 0;
		}

		private double GetPolygonArea(List<PolygonPoint> ptArrayList)
		{
			PolygonPoint pt1 = ptArrayList[0];
			PolygonPoint firstPt = pt1;
			PolygonPoint lastPt = default(PolygonPoint);
			double area = 0.0;
			for (int i = 1; i < ptArrayList.Count; ++i)
			{
				var pt2 = ptArrayList[i];
				area += (((pt2.X - pt1.X) * (pt2.Y + pt1.Y)) / 2);
				pt1 = pt2;
				lastPt = pt1;
			}
			area += (((firstPt.X - lastPt.X) * (firstPt.Y + lastPt.Y)) / 2);
			return area;
		}

		private double GetPolygonArea(PointF[] ptArrayList, int start, int count)
		{
			double area = 0.0;
			var pt1 = ptArrayList[start];
			for (int i = start + 1; i < count; ++i)
			{
				var pt2 = ptArrayList[i + start];
				area += ((pt2.X - pt1.X) * (pt2.Y + pt1.Y));
				pt1 = pt2;
			}
			pt1 = ptArrayList[start];
			var ptl = ptArrayList[start + count - 1];
			area += ((pt1.X - ptl.X) * (pt1.Y + ptl.Y));
			return 0.5 * area;
		}

		private double GetDistance(PolygonPoint a, PolygonPoint b)
		{
			double dx = b.X - a.X;
			double dy = b.Y - a.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		public static VectorD3D MeasureString(string text, FontX3D font, System.Drawing.StringFormat strgfmt)
		{
			var path = new System.Drawing.Drawing2D.GraphicsPath();
			path.AddString(text, Altaxo.Graph.Gdi.GdiFontManager.ToGdi(font.Font).FontFamily, 0, 1000, new System.Drawing.PointF(0, 0), strgfmt);
			var size = path.GetBounds();
			path.Dispose();
			return new VectorD3D(size.Width * font.Size / 1000.0, size.Height * font.Size / 1000.0, font.Depth);
		}

		private void CreatePolygons()
		{
			_polygons.Clear();

			var path = new System.Drawing.Drawing2D.GraphicsPath();
			path.AddString(_text, new System.Drawing.FontFamily("Calibri"), 0, 1000, new System.Drawing.PointF(0, 0), System.Drawing.StringFormat.GenericTypographic);
			path.Flatten();
			double minDist = _fontSize * distanceCutThreshold;

			var types = path.PathTypes;
			var pts = path.PathPoints;
			var len = path.PointCount;
			int i;
			var l = new List<PolygonPoint>();

			for (i = 0; i < len; ++i)
			{
				l.Add(new PolygonPoint(pts[i].X / 1000, (1000 - pts[i].Y) / 1000));
				if ((types[i] & 0x80) != 0)
				{
					++i;
					break;
				}
			}

			if (GetDistance(l[0], l[l.Count - 1]) < minDist)
				l.RemoveAt(l.Count - 1);

			var p = new Poly2Tri.Polygon(l);
			l.Clear();

			while (i < len)
			{
				for (; i < len; ++i)
				{
					l.Add(new PolygonPoint(pts[i].X / 1000, (1000 - pts[i].Y) / 1000));
					if ((types[i] & 0x80) != 0)
					{
						++i;
						break;
					}
				}

				if (GetDistance(l[0], l[l.Count - 1]) < minDist)
					l.RemoveAt(l.Count - 1);

				var b = isClockwise(l);

				if (!b)
				{
					Poly2Tri.P2T.Triangulate(p);
					_polygons.Add(p); // Trianguliere und addiere das alte Polygon

					p = new Poly2Tri.Polygon(l);
				}
				else
				{
					p.AddHole(new Poly2Tri.Polygon(l));
				}

				l.Clear();
			}

			Poly2Tri.P2T.Triangulate(p);
			_polygons.Add(p);
		}

		public void AddWithNormals(Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int startIndex)
		{
			if (_polygons.Count == 0)
				CreatePolygons();

			var pdict = new Dictionary<PointF, int>();
			int[] threeIndices = new int[3];

			foreach (var polygon in _polygons)
			{
				pdict.Clear();
				foreach (var tri in polygon.Triangles)
				{
					for (int i = 0; i <= 2; ++i)
					{
						var p = new PointF(tri.Points[i].Xf, tri.Points[i].Yf);
						int pos;
						if (pdict.TryGetValue(p, out pos))
						{
							threeIndices[i] = (pos + startIndex);
						}
						else
						{
							int idx = pdict.Count;
							pdict.Add(p, idx);
							threeIndices[i] = (startIndex + idx);
							AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, 0), new VectorD3D(0, 0, -1));
						}
					}
					AddIndices(threeIndices[0], threeIndices[1], threeIndices[2]);
				}
				startIndex += pdict.Count;

				// now extrude the depth

				int len = polygon.Points.Count;
				int len2 = 2 * len;
				for (int i = 0; i < len; ++i)
				{
					int i2 = 2 * i;
					var p = polygon.Points[i];
					AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, 0), new VectorD3D(0, 1, 0));
					AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, -_depth), new VectorD3D(0, 1, 0));

					// now the triangles
					AddIndices(
					startIndex + i2,
					startIndex + (i2 + 2) % len2,
					startIndex + i2 + 1);

					AddIndices(
					startIndex + i2 + 1,
					startIndex + (i2 + 2) % len2,
					startIndex + (i2 + 3) % len2);
				}
				startIndex += len2;

				if (null != polygon.Holes)
				{
					foreach (var ph in polygon.Holes)
					{
						len = ph.Points.Count;
						len2 = 2 * len;
						for (int i = 0; i < len; ++i)
						{
							int i2 = 2 * i;
							var p = ph.Points[i];
							AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, 0), new VectorD3D(0, 1, 0));
							AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, -_depth), new VectorD3D(0, 1, 0));

							// now the triangles
							AddIndices(
							startIndex + i2,
							startIndex + (i2 + 2) % len2,
							startIndex + i2 + 1);

							AddIndices(
							startIndex + i2 + 1,
							startIndex + (i2 + 2) % len2,
							startIndex + (i2 + 3) % len2);
						}
						startIndex += len2;
					}
				}

				// now the back side

				pdict.Clear();
				foreach (var tri in polygon.Triangles)
				{
					for (int i = 2; i >= 0; --i)
					{
						var p = new PointF(tri.Points[i].Xf, tri.Points[i].Yf);
						int pos;
						if (pdict.TryGetValue(p, out pos))
						{
							threeIndices[i] = (pos + startIndex);
						}
						else
						{
							int idx = pdict.Count;
							pdict.Add(p, idx);
							threeIndices[i] = (startIndex + idx);
							AddPositionAndNormal(new PointD3D(p.X * _fontSize, p.Y * _fontSize, -_depth), new VectorD3D(0, 0, 1));
						}
					}
					AddIndices(threeIndices[0], threeIndices[2], threeIndices[1]);
				}
				startIndex += pdict.Count;
			}
		}
	}
}