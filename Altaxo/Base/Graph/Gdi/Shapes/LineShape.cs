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
	[Serializable]
	public class LineShape : OpenPathShapeBase
	{
		#region Serialization

		private class DeprecatedLineShape : ClosedPathShapeBase
		{
			public DeprecatedLineShape()
				: base(new ItemLocationDirect(), (Altaxo.Main.Properties.IReadOnlyPropertyBag)null)
			{
			}

			/// <summary>
			/// Get the object outline for arrangements in object world coordinates.
			/// </summary>
			/// <returns>Object outline for arrangements in object world coordinates</returns>
			public override GraphicsPath GetObjectOutlineForArrangements()
			{
				throw new NotImplementedException();
			}

			public override void Paint(Graphics g, object obj)
			{
				throw new NotImplementedException();
			}

			public override object Clone()
			{
				throw new NotImplementedException();
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LineGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.LineShape", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
				/*
				LineShape s = (LineShape)obj;
				info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (DeprecatedLineShape)o : new DeprecatedLineShape();

				info.GetBaseValueEmbedded(s, "AltaxoBase,Altaxo.Graph.GraphicsObject,0", parent);

				if (info.CurrentElementName == "LinePen")// 2012-06-18 bugfix: the next three lines are in some cases deserialized in ClosedPathShapeBase
				{
					s.Pen = (PenX)info.GetValue("LinePen", s);
					info.GetBoolean("Fill");
					info.GetValue("FillBrush", s);
				}

				var l = new LineShape(info);
				l.CopyFrom(s);
				l.Pen = s.Pen; // we don't need to clone, since it is abandoned anyway

				return l;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineShape), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (LineShape)obj;
				info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (LineShape)o : new LineShape(info);
				info.GetBaseValueEmbedded(s, typeof(LineShape).BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		protected LineShape(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(new ItemLocationDirect(), info)
		{
		}

		public LineShape(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: base(new ItemLocationDirect(), context)
		{
		}

		public LineShape(LineShape from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public LineShape(PointD2D startPosition, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: base(new ItemLocationDirect(), context)
		{
			this.Position = startPosition;
		}

		public LineShape(double posX, double posY, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: this(new PointD2D(posX, posY), context)
		{
		}

		public LineShape(PointD2D startPosition, PointD2D endPosition, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			:
			this(startPosition, context)
		{
			this._location.SizeX = RADouble.NewAbs(endPosition.X - startPosition.X);
			this._location.SizeY = RADouble.NewAbs(endPosition.Y - startPosition.Y);
		}

		public LineShape(double startX, double startY, PointD2D endPosition, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			:
			this(new PointD2D(startX, startY), endPosition, context)
		{
		}

		public LineShape(double startX, double startY, double endX, double endY, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			:
			this(new PointD2D(startX, startY), new PointD2D(endX, endY), context)
		{
		}

		public LineShape(PointD2D startPosition, PointD2D endPosition, double lineWidth, NamedColor lineColor)
			:
			this(startPosition, null)
		{
			this._location.SizeX = RADouble.NewAbs(endPosition.X - startPosition.X);
			this._location.SizeY = RADouble.NewAbs(endPosition.Y - startPosition.Y);
			this.Pen.Width = (float)lineWidth;
			this.Pen.Color = lineColor;
		}

		public LineShape(double startX, double startY, double endX, double endY, double lineWidth, NamedColor lineColor)
			:
			this(new PointD2D(startX, startY), new PointD2D(endX, endY), null)
		{
			this.Pen.Width = (float)lineWidth;
			this.Pen.Color = lineColor;
		}

		#endregion Constructors

		public override bool AllowNegativeSize
		{
			get
			{
				return true;
			}
		}

		public override object Clone()
		{
			return new LineShape(this);
		}

		public GraphicsPath GetSelectionPath()
		{
			return GetPath();
		}

		/// <summary>
		/// Gets the path of the object in object world coordinates.
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetPath();
		}

		/// <summary>
		/// Gets the path of the object in object world coordinates.
		/// </summary>
		/// <returns></returns>
		protected GraphicsPath GetPath()
		{
			GraphicsPath gp = new GraphicsPath();
			var bounds = Bounds;
			gp.AddLine((float)(bounds.X), (float)(bounds.Y), (float)(bounds.X + Width), (float)(bounds.Y + Height));
			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();
			if (gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
			{
				result = new LineShapeHitTestObject(this);
			}
			else
			{
				gp.Transform(htd.GetTransformation(_transformation)); // Transform to page coord
				if (gp.IsOutlineVisible((PointF)htd.HittedPointInPageCoord, new Pen(Color.Black, 6)))
				{
					result = new LineShapeHitTestObject(this);
				}
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Line properties", true);
			((LineShape)hitted).OnChanged();
			return true;
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			var bounds = Bounds;
			Pen.SetEnvironment((RectangleF)bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
			g.DrawLine(Pen, (float)bounds.X, (float)bounds.Y, (float)bounds.Right, (float)bounds.Bottom);

			if (_outlinePen != null && _outlinePen.IsVisible)
			{
				GraphicsPath p = new GraphicsPath();
				p.AddLine((float)bounds.X, (float)bounds.Y, (float)bounds.Right, (float)bounds.Bottom);
				p.Widen(Pen);
				OutlinePen.SetEnvironment((RectangleF)bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
				g.DrawPath(OutlinePen, p);
			}

			g.Restore(gs);
		}

		protected class LineShapeHitTestObject : GraphicBaseHitTestObject
		{
			public LineShapeHitTestObject(LineShape parent)
				: base(parent)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				if (gripLevel <= 1)
				{
					LineShape ls = (LineShape)_hitobject;
					PointF[] pts = new PointF[] { new PointF(0, 0), new PointF((float)ls.Width, (float)ls.Height) };
					for (int i = 0; i < pts.Length; i++)
					{
						var pt = ls._transformation.TransformPoint(pts[i]);
						pt = this.Transformation.TransformPoint(pt);
						pts[i] = pt;
					}

					IGripManipulationHandle[] grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 3];

					// Translation grips
					GraphicsPath path = new GraphicsPath();
					path.AddLine(pts[0], pts[1]);
					path.Widen(new Pen(Color.Black, (float)(6 / pageScale)));
					grips[0] = new MovementGripHandle(this, path, null);

					// PathNode grips
					if (gripLevel == 1)
					{
						grips[2] = grips[0]; // put the movement grip to the background, the two NodeGrips need more priority
						float gripRadius = (float)(3 / pageScale);
						grips[0] = new PathNodeGripHandle(this, new PointF(0, 0), pts[0], gripRadius);
						grips[1] = new PathNodeGripHandle(this, new PointF(1, 1), pts[1], gripRadius);
					}
					return grips;
				}
				else
				{
					return base.GetGrips(pageScale, gripLevel);
				}
			}
		}
	} // End Class
} // end Namespace