#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Serialization;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// A regular polygon with a user definable number of vertices. It is oriented in a way that the lower boundary is always an edge.
	/// </summary>
	[Serializable]
	public class RegularPolygon : ClosedPathShapeBase
	{
		private int _vertices = 7;
		private double _cornerRadius = 0;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RegularPolygon), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				RegularPolygon s = (RegularPolygon)obj;
				info.AddBaseValueEmbedded(s, typeof(RegularPolygon).BaseType);
				info.AddValue("NumberOfVertices", s._vertices);
				info.AddValue("CornerRadius", s._cornerRadius);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				RegularPolygon s = null != o ? (RegularPolygon)o : new RegularPolygon();
				info.GetBaseValueEmbedded(s, typeof(RegularPolygon).BaseType, parent);

				s._vertices = info.GetInt32("NumberOfVertices");
				s._cornerRadius = info.GetDouble("CornerRadius");
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public RegularPolygon()
		{
		}

		public RegularPolygon(PointD2D graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}

		public RegularPolygon(double posX, double posY)
			:
			this(new PointD2D(posX, posY))
		{
		}

		public RegularPolygon(PointD2D graphicPosition, PointD2D graphicSize)
			:
			this(graphicPosition)
		{
			this.SetSize(graphicSize.X, graphicSize.Y, true);
		}

		public RegularPolygon(double posX, double posY, PointD2D graphicSize)
			:
			this(new PointD2D(posX, posY), graphicSize)
		{
		}

		public RegularPolygon(double posX, double posY, double width, double height)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height))
		{
		}

		public RegularPolygon(PointD2D graphicPosition, double Rotation)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public RegularPolygon(double posX, double posY, double Rotation)
			:
			this(new PointD2D(posX, posY), Rotation)
		{
		}

		public RegularPolygon(PointD2D graphicPosition, PointD2D graphicSize, double Rotation)
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize.X, graphicSize.Y, true);
		}

		public RegularPolygon(double posX, double posY, PointD2D graphicSize, double Rotation)
			:
			this(new PointD2D(posX, posY), graphicSize, Rotation)
		{
		}

		public RegularPolygon(double posX, double posY, double width, double height, double Rotation)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation)
		{
		}

		private static void Exchange(ref double x, ref double y)
		{
			double h = x;
			x = y;
			y = h;
		}

		public static RegularPolygon FromLTRB(double left, double top, double right, double bottom)
		{
			if (left > right)
				Exchange(ref left, ref right);
			if (top > bottom)
				Exchange(ref top, ref bottom);

			return new RegularPolygon(left, top, right - left, bottom - top);
		}

		public RegularPolygon(RegularPolygon from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as RegularPolygon;
				if (null != from)
				{
					this._vertices = from._vertices;
					this._cornerRadius = from._cornerRadius;
				}
			}
			return isCopied;
		}

		#endregion Constructors

		public override object Clone()
		{
			return new RegularPolygon(this);
		}

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetPath();
		}

		public int NumberOfVertices
		{
			get
			{
				return _vertices;
			}
			set
			{
				var oldValue = _vertices;
				value = Math.Max(3, value);
				_vertices = value;
				if (value != oldValue)
				{
					OnChanged();
				}
			}
		}

		public double CornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				var oldValue = _cornerRadius;
				value = Math.Max(0, value);
				value = Math.Min(float.MaxValue, value);
				_cornerRadius = value;
				if (value != oldValue)
				{
					OnChanged();
				}
			}
		}

		public bool HasEvenNumberOfVertices
		{
			get
			{
				return 0 == (_vertices % 2);
			}
		}

		/// <summary>
		/// Gets the path of the object in object world coordinates.
		/// </summary>
		/// <returns></returns>
		protected GraphicsPath GetPath()
		{
			// we adjust the lower edge to be parallel to the x-axis
			GraphicsPath gp = new GraphicsPath();

			double angleStep = 2 * Math.PI / _vertices;
			double startAngle = -Math.PI / 2 - angleStep / 2;

			double relHeigth = 2;
			double relWidth = 2;
			double relYMid = 0.5;
			if (HasEvenNumberOfVertices)
			{
				// with an even number of vertices, the upper line is also parallel
				relHeigth = 2 * Math.Abs(Math.Sin(startAngle));
				int ridx = (int)Math.Round((2 + _vertices) / 4.0);
				relWidth = 2 * Math.Cos(Math.PI * ((2 * ridx - 1) / (double)_vertices - 0.5));
			}
			else // odd number of vertices
			{
				relHeigth = 1 + Math.Abs(Math.Sin(startAngle)); // one vertex is always on top
				relYMid = 1 / relHeigth;
				int ridx = (int)Math.Round((2 + _vertices) / 4.0);
				relWidth = 2 * Math.Cos(Math.PI * ((2 * ridx - 1) / (double)_vertices - 0.5));
			}

			var pts = new PointD2D[_vertices];
			for (int i = 0; i < _vertices; i++)
			{
				double w = i * angleStep + startAngle;
				double x = Width / 2 + Math.Cos(w) * Width / relWidth;
				double y = Height * relYMid - Math.Sin(w) * Height / relHeigth;
				pts[i] = new PointD2D(x, y);
			}

			if (!(_cornerRadius > 0))
			{
				for (int i = 1; i < pts.Length; i++)
					gp.AddLine((float)pts[i - 1].X, (float)pts[i - 1].Y, (float)pts[i].X, (float)pts[i].Y);
			}
			else // _cornerRadius>0
			{
				// calculate the round corners
				for (int i = 0; i < _vertices; i++)
				{
					int prevIdx = (i - 1 + _vertices) % _vertices;
					int nextIdx = (i + 1) % _vertices;

					var prevVec = pts[prevIdx] - pts[i];
					var nextVec = pts[nextIdx] - pts[i];
					var prevLen = prevVec.VectorLength;
					var nextLen = nextVec.VectorLength;
					prevVec /= prevLen;
					nextVec /= nextLen;
					var alpha = Math.Acos(nextVec.DotProduct(prevVec));
					var distanceFromCorner = _cornerRadius / Math.Tan(alpha / 2);
					var bisectionVec = (prevVec + nextVec).GetNormalized();

					if (distanceFromCorner <= prevLen / 2 && distanceFromCorner <= nextLen / 2) // regular case -> corner radius is small
					{
						var distanceFromCornerToMidpoint = _cornerRadius / Math.Sin(alpha / 2);
						var midPoint = pts[i] + distanceFromCornerToMidpoint * bisectionVec;
						var cornerStartAngle = -Math.Atan2(prevVec.X, prevVec.Y);
						var cornerSweepAngle = -Math.PI + alpha;
						gp.AddArc((float)(midPoint.X - _cornerRadius), (float)(midPoint.Y - _cornerRadius), (float)(2 * _cornerRadius), (float)(2 * _cornerRadius), (float)(cornerStartAngle * 180 / Math.PI), (float)(cornerSweepAngle * 180 / Math.PI));
					}
					else // nonregular case -> corner radius is too big
					{
						var m = Math.Tan(alpha / 2);
						var d = Math.Min(prevLen / 2, nextLen / 2);
						var dd = d / Math.Sqrt(1 + m * m);
						var y = m * dd; // heigth of the intersection point above the bisector
						var distanceFromCornerToMidpoint = dd + Math.Sqrt(_cornerRadius * _cornerRadius - y * y);
						var midPoint = pts[i] + distanceFromCornerToMidpoint * bisectionVec;
						var midPointToStart = pts[i] + d * prevVec - midPoint; // Vector from midPoint to SweepStart
						var cornerStartAngle = Math.Atan2(midPointToStart.Y, midPointToStart.X);
						var cornerSweepAngle = -2 * Math.Asin(y / _cornerRadius);
						gp.AddArc((float)(midPoint.X - _cornerRadius), (float)(midPoint.Y - _cornerRadius), (float)(2 * _cornerRadius), (float)(2 * _cornerRadius), (float)(cornerStartAngle * 180 / Math.PI), (float)(cornerSweepAngle * 180 / Math.PI));
					}
				}
			}

			gp.CloseFigure();

			return gp;
		}

		public static GraphicsPath GetPath(double x0, double y0, double radius, int vertices)
		{
			// we adjust the lower edge to be parallel to the x-axis
			GraphicsPath gp = new GraphicsPath();

			double angleStep = 2 * Math.PI / vertices;
			double startAngle = -Math.PI / 2 - angleStep / 2;

			PointF[] pts = new PointF[vertices];
			for (int i = 0; i < vertices; i++)
			{
				double w = i * angleStep + startAngle;
				double x = x0 + radius * Math.Cos(w); ;
				double y = y0 - radius * Math.Sin(w);
				pts[i] = new PointF((float)x, (float)y);
			}
			gp.AddPolygon(pts);

			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();

			if (this._fillBrush.IsVisible && gp.IsVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation)))
			{
				result = new GraphicBaseHitTestObject(this);
			}
			else if (this._linePen.IsVisible && gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
			{
				result = new GraphicBaseHitTestObject(this);
			}
			else
			{
				gp.Transform(htd.GetTransformation(_transformation)); // Transform to page coord
				if (gp.IsOutlineVisible((PointF)htd.HittedPointInPageCoord, new Pen(Color.Black, 6)))
				{
					result = new GraphicBaseHitTestObject(this);
				}
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			var path = GetPath();

			var bounds = Bounds;
			var boundsF = (RectangleF)bounds;
			if (Brush.IsVisible)
			{
				Brush.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
				g.FillPath(Brush, path);
			}

			Pen.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
			g.DrawPath(Pen, path);
			g.Restore(gs);
		}
	} // End Class
} // end Namespace