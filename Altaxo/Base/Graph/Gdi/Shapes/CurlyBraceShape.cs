#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
	public class CurlyBraceShape : ShapeGraphic
	{
		#region Serialization

		#region Clipboard serialization

		protected CurlyBraceShape(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
			CurlyBraceShape s = this;
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
			CurlyBraceShape s = (CurlyBraceShape)base.SetObjectData(obj, info, context, selector);
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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CurlyBraceShape), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				CurlyBraceShape s = (CurlyBraceShape)obj;
				info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				CurlyBraceShape s = null != o ? (CurlyBraceShape)o : new CurlyBraceShape();
				info.GetBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType, parent);

				return s;
			}
		}


		#endregion


		#region Constructors
		public CurlyBraceShape()
		{
		}

		public CurlyBraceShape(PointF graphicPosition)
			:
			this()
		{
			this.SetPosition(graphicPosition);
		}

		public CurlyBraceShape(float posX, float posY)
			:
			this(new PointF(posX, posY))
		{
		}


		public CurlyBraceShape(PointF graphicPosition, SizeF graphicSize)
			:
			this(graphicPosition)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public CurlyBraceShape(float posX, float posY, SizeF graphicSize)
			:
			this(new PointF(posX, posY), graphicSize)
		{
		}

		public CurlyBraceShape(float posX, float posY, float width, float height)
			:
			this(new PointF(posX, posY), new SizeF(width, height))
		{
		}

		public CurlyBraceShape(PointF graphicPosition, float Rotation)
			:
			this()
		{

			this.SetPosition(graphicPosition);
			this.Rotation = Rotation;
		}

		public CurlyBraceShape(float posX, float posY, float Rotation)
			:
			this(new PointF(posX, posY), Rotation)
		{
		}

		public CurlyBraceShape(PointF graphicPosition, SizeF graphicSize, float Rotation)
			:
			this(graphicPosition, Rotation)
		{
			this.SetSize(graphicSize);
			this.AutoSize = false;
		}

		public CurlyBraceShape(float posX, float posY, SizeF graphicSize, float Rotation)
			:
			this(new PointF(posX, posY), graphicSize, Rotation)
		{
		}

		public CurlyBraceShape(float posX, float posY, float width, float height, float Rotation)
			:
			this(new PointF(posX, posY), new SizeF(width, height), Rotation)
		{
		}

		static void Exchange(ref float x, ref float y)
		{
			float h = x;
			x = y;
			y = h;
		}
		public static CurlyBraceShape FromLTRB(float left, float top, float right, float bottom)
		{
			if (left > right)
				Exchange(ref left, ref right);
			if (top > bottom)
				Exchange(ref top, ref bottom);

			return new CurlyBraceShape(left, top, right - left, bottom - top);
		}

		public CurlyBraceShape(CurlyBraceShape from)
			:
			base(from)
		{
		}

		#endregion

		public override object Clone()
		{
			return new CurlyBraceShape(this);
		}


		public override void Paint(Graphics g, object obj)
		{
			GraphicsState gs = g.Save();
			g.TranslateTransform(X, Y);
			if (_rotation != -0)
				g.RotateTransform(-_rotation);

			if (Brush.IsVisible)
			{
				Brush.Rectangle = _bounds;
				g.FillRectangle(Brush, _bounds);
			}

			Pen.BrushRectangle = _bounds;

			var path = GetPath();

			g.DrawPath(Pen, path);

			g.Restore(gs);
		}

		public override IHitTestObject HitTest(HitTestData htd)
		{

			var localHitTestData = htd.NewFromAdditionalTransformation(_transfoToLayerCoord);
			GraphicsPath gp = GetPath();
			gp.Transform(localHitTestData.Transformation); // Transform to page coord
			gp.Widen(new Pen(Color.Black,6 ));

			if (gp.IsVisible(localHitTestData.HittedPointInPageCoord))
			{
				return new HitTestObject(gp, this);
			}
			else
				return null;
		}

    public override GraphicsPath GetObjectPath()
    {
      return GetPath();
    }

    public override GraphicsPath GetSelectionPath()
    {
      return GetPath();
    }

		/// <summary>
		/// Gets the untranslated and unrotated path of this shape.
		/// </summary>
		/// <returns>Untranslated and unrotated path of this shape</returns>
		private GraphicsPath GetPath()
		{
			var path = new GraphicsPath();

			float angle = 90;
			if (_bounds.Height > 0.5 * _bounds.Width)
			{
				double dy = 2 - _bounds.Width / _bounds.Height;
				angle = (float)(180 * Math.Asin(8 / (4 + dy * dy) - 1) / Math.PI);
			}

			path.AddArc(
				_bounds.X + 0.5f * _bounds.Width - _bounds.Height, _bounds.Y + 0.5f * _bounds.Height,
				_bounds.Height, _bounds.Height,
				0, -angle);
			path.AddArc(
				_bounds.X, _bounds.Y - 0.5f * _bounds.Height,
			 _bounds.Height, _bounds.Height,
				180 - angle, angle);

			path.StartFigure();
			path.AddArc(
		_bounds.X + 0.5f * _bounds.Width, _bounds.Y + 0.5f * _bounds.Height,
		_bounds.Height, _bounds.Height,
		180, angle);

			path.AddArc(
				_bounds.Right - _bounds.Height, _bounds.Y - 0.5f * _bounds.Height,
			 _bounds.Height, _bounds.Height,
			 angle, -angle);
			return path;
		}
	} // End Class

} // end Namespace

