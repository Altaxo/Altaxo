#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;

namespace Altaxo.Graph.Graph3D.LabelFormatting
{
	/// <summary>
	/// Base class that can be used to derive a numeric abel formatting class
	/// </summary>
	public abstract class MultiLineLabelFormattingBase : LabelFormattingBase
	{
		private double _relativeLineSpacing = 1;
		private Alignment _textBlockAlignment;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultiLineLabelFormattingBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (MultiLineLabelFormattingBase)obj;
				info.AddBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase).BaseType);
				info.AddValue("LineSpacing", s._relativeLineSpacing);
				info.AddEnum("BlockAlignment", s._textBlockAlignment);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (MultiLineLabelFormattingBase)o;
				info.GetBaseValueEmbedded(s, typeof(MultiLineLabelFormattingBase).BaseType, parent);
				s._relativeLineSpacing = info.GetDouble("LineSpacing");
				s._textBlockAlignment = (Alignment)info.GetEnum("BlockAlignment", typeof(Alignment));

				return s;
			}
		}

		#endregion Serialization

		protected MultiLineLabelFormattingBase()
		{
		}

		protected MultiLineLabelFormattingBase(MultiLineLabelFormattingBase from)
			: base(from) // everything is done here, since CopyFrom is virtual
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as MultiLineLabelFormattingBase;
				if (null != from)
				{
					this._relativeLineSpacing = from._relativeLineSpacing;
					this._textBlockAlignment = from._textBlockAlignment;
				}
			}
			return isCopied;
		}

		public double LineSpacing
		{
			get
			{
				return _relativeLineSpacing;
			}
			set
			{
				_relativeLineSpacing = value;
			}
		}

		public Alignment TextBlockAlignment
		{
			get
			{
				return _textBlockAlignment;
			}
			set
			{
				_textBlockAlignment = value;
			}
		}

		/// <summary>
		/// Measures a couple of items and prepares them for being drawn.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="font">Font used.</param>
		/// <param name="strfmt">String format used.</param>
		/// <param name="items">Array of items to be drawn.</param>
		/// <returns>An array of <see cref="IMeasuredLabelItem" /> that can be used to determine the size of each item and to draw it.</returns>
		public override IMeasuredLabelItem[] GetMeasuredItems(IGraphicsContext3D g, FontX3D font, AltaxoVariant[] items)
		{
			string[] titems = FormatItems(items);
			if (!string.IsNullOrEmpty(_prefix) || !string.IsNullOrEmpty(_suffix))
			{
				for (int i = 0; i < titems.Length; ++i)
					titems[i] = _prefix + titems[i] + _suffix;
			}

			MeasuredLabelItem[] litems = new MeasuredLabelItem[titems.Length];

			FontX3D localfont = font;

			for (int i = 0; i < titems.Length; ++i)
			{
				litems[i] = new MeasuredLabelItem(g, localfont, titems[i], _relativeLineSpacing, Alignment.Near, Alignment.Near, _textBlockAlignment);
			}

			return litems;
		}

		protected new class MeasuredLabelItem : IMeasuredLabelItem
		{
			protected string[] _text;
			protected VectorD3D[] _stringSize;
			protected FontX3D _font;
			protected VectorD3D _size;

			protected Alignment _horizontalAlignment;
			protected Alignment _verticalAlignment;
			protected Alignment _textBlockAligment;
			protected double _lineSpacing;

			#region IMeasuredLabelItem Members

			public MeasuredLabelItem(IGraphicsContext3D g, FontX3D font, string itemtext, double lineSpacing, Alignment horizontalAlignment, Alignment verticalAlignment, Alignment textBlockAligment)
			{
				_text = itemtext.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				_stringSize = new VectorD3D[_text.Length];
				_font = font;
				_horizontalAlignment = horizontalAlignment;
				_verticalAlignment = verticalAlignment;
				_textBlockAligment = textBlockAligment;
				_lineSpacing = lineSpacing;
				_size = VectorD3D.Empty;
				var bounds = RectangleD3D.Empty;
				var position = PointD3D.Empty;
				for (int i = 0; i < _text.Length; ++i)
				{
					_stringSize[i] = g.MeasureString(_text[i], _font, PointD3D.Empty);
					bounds = bounds.WithRectangleIncluded(new RectangleD3D(position, _stringSize[i]));
					position = position.WithYPlus(-_stringSize[i].Y * _lineSpacing);
				}
				_size = bounds.Size;
			}

			public virtual VectorD3D Size
			{
				get
				{
					return _size;
				}
			}

			public virtual void Draw(IGraphicsContext3D g, IMaterial brush, PointD3D point)
			{
				var positionX = point.X + GetHorizontalOffset();
				var positionY = point.Y + GetVerticalOffset();

				for (int i = 0; i < _text.Length; ++i)
				{
					var posX = positionX;
					switch (_textBlockAligment)
					{
						case Alignment.Center:
							posX += (_size.X - _stringSize[i].X) * 0.5;
							break;

						case Alignment.Far:
							posX += (_size.X - _stringSize[i].X);
							break;
					}

					g.DrawString(_text[i], _font, brush, new PointD3D(posX, positionY, 0));
					positionY -= _stringSize[i].Y * _lineSpacing;
				}
			}

			private double GetHorizontalOffset()
			{
				switch (_horizontalAlignment)
				{
					default:
					case Alignment.Near:
						return 0;

					case Alignment.Center:
						return _size.X / 2;

					case Alignment.Far:
						return _size.X;
				}
			}

			private double GetVerticalOffset()
			{
				switch (_verticalAlignment)
				{
					default:
					case Alignment.Near:
						return 0;

					case Alignment.Center:
						return _size.Y / 2;

					case Alignment.Far:
						return _size.Y;
				}
			}

			#endregion IMeasuredLabelItem Members
		}
	}
}