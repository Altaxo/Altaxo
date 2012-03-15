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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Shapes
{
	[Serializable]
	public class RectangleShape : ClosedPathShapeBase
	{
		#region Serialization

		#region Clipboard serialization

		protected RectangleShape(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		/// <summary>
		/// Serializes RectangleGraphic Version 0.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			RectangleShape s = this;
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Deserializes the RectangleGraphic Version 0.
		/// </summary>
		/// <param name="obj">The empty RectangleGraphic object to deserialize into.</param>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <param name="selector">The deserialization surrogate selector.</param>
		/// <returns>The deserialized RectangleGraphic.</returns>
		public new object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			RectangleShape s = (RectangleShape)base.SetObjectData(obj, info, context, selector);
			return s;
		}


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}
		#endregion

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.RectangleGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleShape), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				RectangleShape s = (RectangleShape)obj;
				info.AddBaseValueEmbedded(s, typeof(RectangleShape).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				RectangleShape s = null != o ? (RectangleShape)o : new RectangleShape();
				info.GetBaseValueEmbedded(s, typeof(RectangleShape).BaseType, parent);

				return s;
			}
		}


		#endregion


		#region Constructors
		public RectangleShape()
		{
		}

		public RectangleShape(PointD2D graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}

		public RectangleShape(double posX, double posY)
			:
			this(new PointD2D(posX, posY))
		{
		}


		public RectangleShape(PointD2D graphicPosition, PointD2D graphicSize)
			:
			this(graphicPosition)
		{
			this.SetSize(graphicSize.X, graphicSize.Y);
		}

		public RectangleShape(double posX, double posY, PointD2D graphicSize)
			:
			this(new PointD2D(posX, posY), graphicSize)
		{
		}

		public RectangleShape(double posX, double posY, double width, double height)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height))
		{
		}

		public RectangleShape(PointD2D graphicPosition, double Rotation)
			:
			this()
		{

			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public RectangleShape(double posX, double posY, double Rotation)
			:
			this(new PointD2D(posX, posY), Rotation)
		{
		}

		public RectangleShape(PointD2D graphicPosition, PointD2D graphicSize, double Rotation)
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize.X, graphicSize.Y);
		}

		public RectangleShape(double posX, double posY, PointD2D graphicSize, double Rotation)
			:
			this(new PointD2D(posX, posY), graphicSize, Rotation)
		{
		}

		public RectangleShape(double posX, double posY, double width, double height, double Rotation)
			:
			this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation)
		{
		}

		static void Exchange(ref double x, ref double y)
		{
			double h = x;
			x = y;
			y = h;
		}
		public static RectangleShape FromLTRB(double left, double top, double right, double bottom)
		{
			if (left > right)
				Exchange(ref left, ref right);
			if (top > bottom)
				Exchange(ref top, ref bottom);

			return new RectangleShape(left, top, right - left, bottom - top);
		}

		public RectangleShape(RectangleShape from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		#endregion

		public override object Clone()
		{
			return new RectangleShape(this);
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

			var boundsF = (RectangleF)_bounds;
			if (Brush.IsVisible)
			{
				Brush.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
				g.FillRectangle(Brush, boundsF);
			}

			Pen.SetEnvironment(boundsF, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
			g.DrawRectangle(Pen, (float)_bounds.X, (float)_bounds.Y, (float)_bounds.Width, (float)_bounds.Height);
			g.Restore(gs);
		}
	} // End Class

} // end Namespace

