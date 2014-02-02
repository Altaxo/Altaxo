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
	public class CurlyBraceShape : OpenPathShapeBase
	{
		#region Serialization

		private class DeprecatedCurlyBraceShape : ClosedPathShapeBase
		{
			public DeprecatedCurlyBraceShape()
				: base(new ItemLocationDirect()) { }

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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.CurlyBraceShape", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
				/*
				CurlyBraceShape s = (CurlyBraceShape)obj;
				info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (DeprecatedCurlyBraceShape)o : new DeprecatedCurlyBraceShape();
				info.GetBaseValueEmbedded(s, typeof(DeprecatedCurlyBraceShape).BaseType, parent);

				var l = new CurlyBraceShape();
				l.CopyFrom(s);
				l.Pen = s.Pen; // we don't need to clone, since it is abandoned anyway

				return l;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CurlyBraceShape), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CurlyBraceShape)obj;
				info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (CurlyBraceShape)o : new CurlyBraceShape();
				info.GetBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public CurlyBraceShape()
			: base(new ItemLocationDirect())
		{
		}

		public CurlyBraceShape(CurlyBraceShape from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public static CurlyBraceShape FromLTRB(double left, double top, double right, double bottom)
		{
			if (left > right)
				Exchange(ref left, ref right);
			if (top > bottom)
				Exchange(ref top, ref bottom);

			var result = new CurlyBraceShape();
			result._location.SizeX = RADouble.NewAbs(right - left);
			result._location.SizeY = RADouble.NewAbs(bottom - top);
			result._location.PositionX = RADouble.NewAbs(left);
			result._location.PositionY = RADouble.NewAbs(top);
			return result;
		}

		#endregion Constructors

		public override object Clone()
		{
			return new CurlyBraceShape(this);
		}

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetRectangularObjectOutline();
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			var bounds = Bounds;
			var boundsF = (RectangleF)bounds;

			Pen.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
			var path = GetPath();
			g.DrawPath(Pen, path);

			if (_outlinePen != null && _outlinePen.IsVisible)
			{
				path.Widen(Pen);
				_outlinePen.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
				g.DrawPath(_outlinePen, path);
			}

			g.Restore(gs);
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			HitTestObjectBase result = null;
			GraphicsPath gp = GetPath();
			if (gp.IsOutlineVisible((PointF)htd.GetHittedPointInWorldCoord(_transformation), _linePen))
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

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Graphics properties", true);
			((CurlyBraceShape)hitted).OnChanged();
			return true;
		}

		/// <summary>
		/// Gets the untranslated and unrotated path of this shape.
		/// </summary>
		/// <returns>Untranslated and unrotated path of this shape</returns>
		private GraphicsPath GetPath()
		{
			var path = new GraphicsPath();

			float angle = 90;
			var bounds = Bounds;
			if (bounds.Height > 0.5 * bounds.Width)
			{
				double dy = 2 - bounds.Width / bounds.Height;
				angle = (float)(180 * Math.Asin(8 / (4 + dy * dy) - 1) / Math.PI);
			}

			path.AddArc(
				(float)(bounds.X + 0.5 * bounds.Width - bounds.Height),
				(float)(bounds.Y + 0.5 * bounds.Height),
				(float)bounds.Height,
				(float)bounds.Height,
				0,
				-angle);

			path.AddArc(
				(float)bounds.X,
				(float)(bounds.Y - 0.5f * bounds.Height),
			 (float)bounds.Height,
			 (float)bounds.Height,
				180 - angle, angle);

			path.StartFigure();

			path.AddArc(
		(float)(bounds.X + 0.5 * bounds.Width),
		(float)(bounds.Y + 0.5f * bounds.Height),
		(float)bounds.Height,
		(float)bounds.Height,
		180, angle);

			path.AddArc(
				(float)(bounds.Right - bounds.Height),
				(float)(bounds.Y - 0.5f * bounds.Height),
			 (float)bounds.Height,
			 (float)bounds.Height,
			 angle, -angle);
			return path;
		}

		private static void Exchange(ref double x, ref double y)
		{
			var h = x;
			x = y;
			y = h;
		}
	} // End Class
} // end Namespace