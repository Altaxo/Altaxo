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
	public class EllipseShape : ClosedPathShapeBase
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EllipseGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EllipseShape), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				EllipseShape s = (EllipseShape)obj;
				info.AddBaseValueEmbedded(s, typeof(EllipseShape).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				EllipseShape s = null != o ? (EllipseShape)o : new EllipseShape();
				info.GetBaseValueEmbedded(s, typeof(EllipseShape).BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public EllipseShape()
		{
		}

		public EllipseShape(PointD2D graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}

		public EllipseShape(double posX, double posY)
			:
			this(new PointD2D(posX, posY))
		{
		}

		public EllipseShape(PointD2D graphicPosition, PointD2D graphicSize)
			:
			this(graphicPosition)
		{
			this.SetSize(graphicSize.X, graphicSize.Y, true);
		}

		public EllipseShape(double posX, double posY, PointD2D graphicSize)
			:
			this(new PointD2D(posX, posY), graphicSize)
		{
		}

		public EllipseShape(double posX, double posY, double width, double height)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height))
		{
		}

		public EllipseShape(PointD2D graphicPosition, double Rotation)
			:
			this()
		{
			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public EllipseShape(double posX, double posY, double Rotation)
			:
			this(new PointD2D(posX, posY), Rotation)
		{
		}

		public EllipseShape(PointD2D graphicPosition, PointD2D graphicSize, double Rotation)
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize.X, graphicSize.Y, true);
		}

		public EllipseShape(double posX, double posY, PointD2D graphicSize, double Rotation)
			:
			this(new PointD2D(posX, posY), graphicSize, Rotation)
		{
		}

		public EllipseShape(double posX, double posY, double width, double height, double Rotation)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation)
		{
		}

		public EllipseShape(EllipseShape from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		#endregion Constructors

		public override object Clone()
		{
			return new EllipseShape(this);
		}

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			GraphicsPath gp = new GraphicsPath();
			var bounds = this.Bounds;
			gp.AddEllipse(new RectangleF((float)(bounds.X), (float)(bounds.Y), (float)bounds.Width, (float)bounds.Height));
			return gp;
		}

		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			var bounds = Bounds;
			var boundsF = (RectangleF)bounds;
			if (Brush.IsVisible)
			{
				Brush.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
				g.FillEllipse(Brush, boundsF);
			}

			Pen.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(ScaleX, ScaleY)));
			g.DrawEllipse(Pen, boundsF);
			g.Restore(gs);
		}
	} // end class
} // end Namespace