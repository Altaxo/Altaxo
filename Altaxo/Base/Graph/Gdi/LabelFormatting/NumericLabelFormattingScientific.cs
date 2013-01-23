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

namespace Altaxo.Graph.Gdi.LabelFormatting
{
	/// <summary>
	/// Formats a numeric item in scientific notation, i.e. in the form mantissa*10^exponent.
	/// </summary>
	public class NumericLabelFormattingScientific : NumericLabelFormattingBase
	{
		bool _showExponentAlways;


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.NumericLabelFormattingScientific", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.LabelFormatting.NumericLabelFormattingScientific", 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				NumericLabelFormattingScientific s = (NumericLabelFormattingScientific)obj;
				info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingBase));
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				NumericLabelFormattingScientific s = null != o ? (NumericLabelFormattingScientific)o : new NumericLabelFormattingScientific();
				info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingBase), parent);
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingScientific), 2)]
		class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				NumericLabelFormattingScientific s = (NumericLabelFormattingScientific)obj;
				info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingBase));
				info.AddValue("ShowExponentAlways", s._showExponentAlways);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				NumericLabelFormattingScientific s = null != o ? (NumericLabelFormattingScientific)o : new NumericLabelFormattingScientific();
				info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingBase), parent);
				s._showExponentAlways = info.GetBoolean("ShowExponentAlways");
				return s;
			}
		}

		#endregion



		public NumericLabelFormattingScientific()
		{

		}

		public NumericLabelFormattingScientific(NumericLabelFormattingScientific from)
			: base(from) // everything is done here, since CopyFrom is virtual
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as NumericLabelFormattingScientific;
				if (null != from)
				{
					this._showExponentAlways = from._showExponentAlways;
				}
			}
			return isCopied;
		}

		public override object Clone()
		{
			return new NumericLabelFormattingScientific(this);
		}


		/// <summary>Gets or sets a value indicating whether to show the exponent for all numeric values.</summary>
		/// <value>
		/// 	If <see langword="true"/>, the exponent is shown even for numbers inbetween 1 and 10. If false, for numbers between 1 and 10 only the mantissa is displayed.
		/// </value>
		public bool ShowExponentAlways
		{
			get
			{
				return _showExponentAlways;
			}
			set
			{
				_showExponentAlways = value;
			}
		}

		protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
		{
			throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
		}


		public string FormatItem(double tick)
		{
			throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
		}

		protected void SplitInFirstPartAndExponent(double ditem, out string firstpart, out double mant, out string middelpart, out string exponent)
		{
			string sitem1 = ditem.ToString("E");

			if (ditem == 0)
			{
				mant = 0;
				firstpart = 0.ToString();
				middelpart = string.Empty;
				exponent = string.Empty;
				return;
			}

			int posOfE = sitem1.IndexOf('E');
			System.Diagnostics.Debug.Assert(posOfE > 0);

			int expo = int.Parse(sitem1.Substring(posOfE + 1));
			mant = ditem * Calc.RMath.Pow(10, -expo);

			if (expo != 0 || _showExponentAlways)
			{

				firstpart = mant.ToString();
				exponent = expo.ToString();

				if (firstpart == 1.ToString())
				{
					firstpart = string.Empty;
					middelpart = "10";
				}
				else
				{
					middelpart = "\u00D710";
				}
			}
			else
			{
				firstpart = mant.ToString();
				middelpart = string.Empty;
				exponent = string.Empty;
			}

		}

		public override System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, FontX font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
		{
			string firstpart, middelpart, exponent;
			double mant;
			SplitInFirstPartAndExponent((double)mtick, out firstpart, out mant, out middelpart, out exponent);

			var gdiFont = font.ToGdi();
			SizeF size1 = g.MeasureString(_prefix + firstpart + middelpart, gdiFont, new PointF(0, 0), strfmt);
			SizeF size2 = g.MeasureString(exponent, gdiFont, new PointF(size1.Width, 0), strfmt);
			SizeF size3 = g.MeasureString(_suffix, gdiFont, new PointF(0, 0), strfmt);

			return new SizeF(size1.Width + size2.Width + size3.Width, size1.Height);
		}

		public override void DrawItem(Graphics g, BrushX brush, FontX font, StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
		{
			string firstpart, middelpart, exponent;
			double mant;
			SplitInFirstPartAndExponent((double)item, out firstpart, out mant, out middelpart, out exponent);

			var gdiFont = font.ToGdi();
			SizeF size1 = g.MeasureString(_prefix + firstpart + middelpart, gdiFont, morg, strfmt);
			g.DrawString(_prefix + firstpart + middelpart, gdiFont, brush, morg, strfmt);
			var orginalY = morg.Y;
			morg.X += size1.Width;
			morg.Y += size1.Height / 3;
			FontX font2 = font.GetFontWithNewSize(font.Size * 2 / 3.0);
			var gdiFont2 = font2.ToGdi();
			g.DrawString(exponent, gdiFont2, brush, morg);
			if (!string.IsNullOrEmpty(_suffix))
			{
				morg.X += g.MeasureString(exponent, gdiFont2, morg, strfmt).Width;
				morg.Y = orginalY;
			}

			if (!string.IsNullOrEmpty(_suffix))
			{
				g.DrawString(_suffix, gdiFont, brush, morg, strfmt);
			}
		}

		public override IMeasuredLabelItem[] GetMeasuredItems(Graphics g, FontX font, StringFormat strfmt, Altaxo.Data.AltaxoVariant[] items)
		{
			MeasuredLabelItem[] litems = new MeasuredLabelItem[items.Length];

			FontX localfont1 = font;
			FontX localfont2 = font.GetFontWithNewSize(font.Size * 2 / 3);

			StringFormat localstrfmt = (StringFormat)strfmt.Clone();

			string[] firstp = new string[items.Length];
			string[] middel = new string[items.Length];
			string[] expos = new string[items.Length];
			double[] mants = new double[items.Length];

			float maxexposize = 0;
			int firstpartmin = int.MaxValue;
			int firstpartmax = int.MinValue;
			for (int i = 0; i < items.Length; ++i)
			{
				string firstpart, exponent;
				if (items[i].IsType(Altaxo.Data.AltaxoVariant.Content.VDouble))
				{
					SplitInFirstPartAndExponent((double)items[i], out firstpart, out mants[i], out middel[i], out exponent);
					if (exponent.Length > 0)
					{
						firstpartmin = Math.Min(firstpartmin, firstpart.Length);
						firstpartmax = Math.Max(firstpartmax, firstpart.Length);
					}
				}
				else
				{
					firstpart = items[i].ToString(); middel[i] = string.Empty; exponent = string.Empty;
				}
				firstp[i] = firstpart;
				expos[i] = exponent;
				maxexposize = Math.Max(maxexposize, g.MeasureString(exponent, localfont2.ToGdi(), new PointF(0, 0), strfmt).Width);
			}

			if (firstpartmax > 0 && firstpartmax > firstpartmin) // then we must use special measures to equilibrate the mantissa
			{
				firstp = NumericLabelFormattingAuto.FormatItems(mants);
			}


			for (int i = 0; i < items.Length; ++i)
			{
				string mid = string.Empty;
				if (!string.IsNullOrEmpty(expos[i]))
				{
					if (string.IsNullOrEmpty(firstp[i]))
						mid = "10";
					else
						mid = "\u00D710";
				}
				litems[i] = new MeasuredLabelItem(g, localfont1, localfont2, localstrfmt, _prefix + firstp[i] + mid, expos[i], _suffix, maxexposize);
			}

			return litems;

		}

		protected new class MeasuredLabelItem : IMeasuredLabelItem
		{
			protected string _firstpart;
			protected string _exponent;
			protected string _lastpart;
			protected FontX _font1;
			protected FontX _font2;
			protected System.Drawing.StringFormat _strfmt;
			protected SizeF _size1;
			protected SizeF _size2;
			protected SizeF _size3;
			protected float _rightPadding;

			#region IMeasuredLabelItem Members

			public MeasuredLabelItem(Graphics g, FontX font1, FontX font2, StringFormat strfmt, string firstpart, string exponent, string lastpart, float maxexposize)
			{
				_firstpart = firstpart;
				_exponent = exponent;
				_lastpart = lastpart;
				_font1 = font1;
				_font2 = font2;
				_strfmt = strfmt;
				_size1 = g.MeasureString(_firstpart, _font1.ToGdi(), new PointF(0, 0), strfmt);
				_size2 = g.MeasureString(_exponent, _font2.ToGdi(), new PointF(_size1.Width, 0), strfmt);
				_size3 = g.MeasureString(_lastpart, _font1.ToGdi(), new PointF(0, 0), strfmt);
				_rightPadding = maxexposize - _size2.Width;

			}

			public virtual SizeF Size
			{
				get
				{
					return new SizeF(_size1.Width + _size2.Width + _rightPadding + _size3.Width, _size1.Height);
				}
			}

			public virtual void Draw(Graphics g, BrushX brush, PointF point)
			{
				g.DrawString(_firstpart, _font1.ToGdi(), brush, point, _strfmt);

				point.X += _size1.Width;
				point.Y += 0;

				g.DrawString(_exponent, _font2.ToGdi(), brush, point, _strfmt);

				point.X += _size2.Width;
				g.DrawString(_lastpart, _font1.ToGdi(), brush, point, _strfmt);
			}

			#endregion

		}
	}
}
