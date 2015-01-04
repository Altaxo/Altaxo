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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace Altaxo.Graph.Gdi.Background
{
	/// <summary>
	/// Backs the item with a color filled rectangle.
	/// </summary>
	[Serializable]
	public class DarkMarbel
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IBackgroundStyle
	{
		protected BrushX _brush;
		protected double _shadowLength = 5;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.DarkMarbel", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("Programming error - this should not be called");
				/*
				DarkMarbel s = (DarkMarbel)obj;
				info.AddValue("Color", s._color);
				info.AddValue("ShadowLength", s._shadowLength);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DarkMarbel s = null != o ? (DarkMarbel)o : new DarkMarbel();
				s.Brush = new BrushX((NamedColor)info.GetValue("Color", s));
				s._shadowLength = info.GetDouble();

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.DarkMarbel", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DarkMarbel), 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DarkMarbel s = (DarkMarbel)obj;
				info.AddValue("Brush", s._brush);
				info.AddValue("ShadowLength", s._shadowLength);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DarkMarbel s = null != o ? (DarkMarbel)o : new DarkMarbel();
				s.Brush = (BrushX)info.GetValue("Brush", s);
				s._shadowLength = info.GetDouble();

				return s;
			}
		}

		#endregion Serialization

		public DarkMarbel()
		{
			_brush = new BrushX(NamedColors.LightGray) { ParentObject = this };
		}

		public DarkMarbel(NamedColor c)
		{
			this.Brush = new BrushX(c);
		}

		public DarkMarbel(DarkMarbel from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(DarkMarbel from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this._shadowLength = from._shadowLength;
			this.Brush = from._brush;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _brush)
				yield return new Main.DocumentNodeAndName(_brush, "Brush");
		}

		public object Clone()
		{
			return new DarkMarbel(this);
		}

		#region IBackgroundStyle Members

		public RectangleD MeasureItem(System.Drawing.Graphics g, RectangleD innerArea)
		{
			innerArea.Inflate(3.0 * _shadowLength / 2, 3.0 * _shadowLength / 2);
			return innerArea;
		}

		public void Draw(System.Drawing.Graphics g, RectangleD innerArea)
		{
			Draw(g, _brush, innerArea);
		}

		public void Draw(Graphics g, BrushX brush, RectangleD innerArea)
		{
			innerArea.Inflate(_shadowLength / 2, _shadowLength / 2); // Padding
			var outerArea = innerArea;
			outerArea.Inflate(_shadowLength, _shadowLength);

			brush.SetEnvironment(outerArea, BrushX.GetEffectiveMaximumResolution(g, 1));
			g.FillRectangle(brush, (RectangleF)outerArea);

			SolidBrush twhite = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
			RectangleF oA = (RectangleF)outerArea;
			RectangleF iA = (RectangleF)innerArea;
			g.FillPolygon(twhite, new PointF[] {
                                                      new PointF(oA.Left,oA.Top), // upper left point
                                                      new PointF(oA.Right,oA.Top), // go to the right
                                                      new PointF(iA.Right,iA.Top), // go 45 deg left down in the upper right corner
                                                      new PointF(iA.Left,iA.Top), // upper left corner of the inner rectangle
                                                      new PointF(iA.Left,iA.Bottom), // lower left corner of the inner rectangle
                                                      new PointF(oA.Left,oA.Bottom) // lower left corner
      });

			SolidBrush tblack = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
			g.FillPolygon(tblack, new PointF[] {
                                                      new PointF(oA.Right,oA.Bottom),
                                                      new PointF(oA.Right,oA.Top),
                                                      new PointF(iA.Right,iA.Top),
                                                      new PointF(iA.Right,iA.Bottom), // upper left corner of the inner rectangle
                                                      new PointF(iA.Left,iA.Bottom), // lower left corner of the inner rectangle
                                                      new PointF(oA.Left,oA.Bottom) // lower left corner
      });
		}

		public bool SupportsBrush
		{
			get
			{
				return true;
			}
		}

		public BrushX Brush
		{
			get
			{
				return _brush;
			}
			set
			{
				_brush = value == null ? null : value.Clone();
				_brush.ParentObject = this;
			}
		}

		#endregion IBackgroundStyle Members
	}
}