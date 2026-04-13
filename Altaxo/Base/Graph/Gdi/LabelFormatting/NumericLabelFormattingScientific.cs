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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Calc;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Formats a numeric item in scientific notation, i.e. in the form mantissa*10^exponent.
  /// </summary>
  public class NumericLabelFormattingScientific : NumericLabelFormattingBase
  {
    private bool _showExponentAlways;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.NumericLabelFormattingScientific", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.LabelFormatting.NumericLabelFormattingScientific", 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingScientific)o;
        info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingBase));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingScientific?)o ?? new NumericLabelFormattingScientific();
        info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingBase), parent);
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingScientific), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingScientific)o;
        info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingBase));
        info.AddValue("ShowExponentAlways", s._showExponentAlways);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingScientific?)o ?? new NumericLabelFormattingScientific();
        info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingBase), parent);
        s._showExponentAlways = info.GetBoolean("ShowExponentAlways");
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericLabelFormattingScientific"/> class.
    /// </summary>
    public NumericLabelFormattingScientific()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericLabelFormattingScientific"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public NumericLabelFormattingScientific(NumericLabelFormattingScientific from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (base.CopyFrom(obj))
      {
        if (obj is NumericLabelFormattingScientific from)
        {
          _showExponentAlways = from._showExponentAlways;
        }
        return true;
      }
      return false;
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new NumericLabelFormattingScientific(this);
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
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

    /// <inheritdoc/>
    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    /// <summary>
    /// Formats a numeric value in scientific notation.
    /// </summary>
    /// <param name="tick">The numeric value.</param>
    /// <returns>The formatted string.</returns>
    public string FormatItem(double tick)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    /// <summary>
    /// Splits a numeric value into a mantissa and exponent representation.
    /// </summary>
    /// <param name="ditem">The numeric value to split.</param>
    /// <param name="firstpart">Receives the formatted first part of the label.</param>
    /// <param name="mant">Receives the mantissa.</param>
    /// <param name="middelpart">Receives the separator between mantissa and exponent.</param>
    /// <param name="exponent">Receives the formatted exponent.</param>
    protected void SplitInFirstPartAndExponent(double ditem, out string firstpart, out double mant, out string middelpart, out string exponent)
    {
      if (0 == ditem || double.IsNaN(ditem) || double.IsInfinity(ditem))
      {
        mant = ditem;
        firstpart = ditem.ToString();
        middelpart = string.Empty;
        exponent = string.Empty;
        return;
      }

      (mant, var expo) = Rounding.SplitIntoDecimalMantissaAndExponent(ditem);

      if (expo != 0 || _showExponentAlways)
      {
        firstpart = mant.ToString("G15");
        exponent = expo.ToString();

        if (firstpart == 1.ToString("G15"))
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
        firstpart = mant.ToString("G15");
        middelpart = string.Empty;
        exponent = string.Empty;
      }
    }

    /// <inheritdoc />
    public override System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, FontX font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
    {
      SplitInFirstPartAndExponent(mtick, out var firstpart, out var mant, out var middelpart, out var exponent);

      var gdiFont = GdiFontManager.ToGdi(font);
      SizeF size1 = g.MeasureString(_prefix + firstpart + middelpart, gdiFont, new PointF(0, 0), strfmt);
      SizeF size2 = g.MeasureString(exponent, gdiFont, new PointF(size1.Width, 0), strfmt);
      SizeF size3 = g.MeasureString(_suffix, gdiFont, new PointF(0, 0), strfmt);

      return new SizeF(size1.Width + size2.Width + size3.Width, size1.Height);
    }

    /// <inheritdoc />
    public override void DrawItem(Graphics g, BrushX brush, FontX font, StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
    {
      SplitInFirstPartAndExponent(item, out var firstpart, out var mant, out var middelpart, out var exponent);

      var gdiFont = GdiFontManager.ToGdi(font);
      SizeF size1 = g.MeasureString(_prefix + firstpart + middelpart, gdiFont, morg, strfmt);
      using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, new Geometry.RectangleD2D(0, 0, size1.Width, size1.Height), g, 1))
      {
        g.DrawString(_prefix + firstpart + middelpart, gdiFont, brushGdi, morg, strfmt);
        var orginalY = morg.Y;
        morg.X += size1.Width;
        morg.Y += size1.Height / 3;
        FontX font2 = font.WithSize(font.Size * 2 / 3.0);
        var gdiFont2 = GdiFontManager.ToGdi(font2);
        g.DrawString(exponent, gdiFont2, brushGdi, morg);
        if (!string.IsNullOrEmpty(_suffix))
        {
          morg.X += g.MeasureString(exponent, gdiFont2, morg, strfmt).Width;
          morg.Y = orginalY;
        }

        if (!string.IsNullOrEmpty(_suffix))
        {
          g.DrawString(_suffix, gdiFont, brushGdi, morg, strfmt);
        }
      }
    }

    /// <inheritdoc />
    public override IMeasuredLabelItem[] GetMeasuredItems(Graphics g, FontX font, StringFormat strfmt, Altaxo.Data.AltaxoVariant[] items)
    {
      var litems = new MeasuredLabelItem[items.Length];

      FontX localfont1 = font;
      FontX localfont2 = font.WithSize(font.Size * 2 / 3);

      var localstrfmt = (StringFormat)strfmt.Clone();

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
          SplitInFirstPartAndExponent(items[i], out firstpart, out mants[i], out middel[i], out exponent);
          if (exponent.Length > 0)
          {
            firstpartmin = Math.Min(firstpartmin, firstpart.Length);
            firstpartmax = Math.Max(firstpartmax, firstpart.Length);
          }
        }
        else
        {
          firstpart = items[i].ToString();
          middel[i] = string.Empty;
          exponent = string.Empty;
        }
        firstp[i] = firstpart;
        expos[i] = exponent;
        maxexposize = Math.Max(maxexposize, g.MeasureString(exponent, GdiFontManager.ToGdi(localfont2), new PointF(0, 0), strfmt).Width);
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

    /// <summary>
    /// Represents a measured scientific-notation label item.
    /// </summary>
    protected new class MeasuredLabelItem : IMeasuredLabelItem
    {
      /// <summary>
      /// The leading portion of the label.
      /// </summary>
      protected string _firstpart;
      /// <summary>
      /// The exponent portion of the label.
      /// </summary>
      protected string _exponent;
      /// <summary>
      /// The trailing portion of the label.
      /// </summary>
      protected string _lastpart;
      /// <summary>
      /// The font used for the leading and trailing text.
      /// </summary>
      protected FontX _font1;
      /// <summary>
      /// The font used for the exponent text.
      /// </summary>
      protected FontX _font2;
      /// <summary>
      /// The string formatting used for drawing.
      /// </summary>
      protected System.Drawing.StringFormat _strfmt;
      /// <summary>
      /// The measured size of the leading portion.
      /// </summary>
      protected SizeF _size1;
      /// <summary>
      /// The measured size of the exponent portion.
      /// </summary>
      protected SizeF _size2;
      /// <summary>
      /// The measured size of the trailing portion.
      /// </summary>
      protected SizeF _size3;
      /// <summary>
      /// The padding added to keep exponents aligned.
      /// </summary>
      protected float _rightPadding;

      #region IMeasuredLabelItem Members

      /// <summary>
      /// Initializes a new instance of the <see cref="MeasuredLabelItem"/> class.
      /// </summary>
      /// <param name="g">The graphics context used for measurement.</param>
      /// <param name="font1">The base font.</param>
      /// <param name="font2">The exponent font.</param>
      /// <param name="strfmt">The string format.</param>
      /// <param name="firstpart">The leading portion of the label.</param>
      /// <param name="exponent">The exponent portion of the label.</param>
      /// <param name="lastpart">The trailing portion of the label.</param>
      /// <param name="maxexposize">The maximum measured exponent width.</param>
      public MeasuredLabelItem(Graphics g, FontX font1, FontX font2, StringFormat strfmt, string firstpart, string exponent, string lastpart, float maxexposize)
      {
        _firstpart = firstpart;
        _exponent = exponent;
        _lastpart = lastpart;
        _font1 = font1;
        _font2 = font2;
        _strfmt = strfmt;
        _size1 = g.MeasureString(_firstpart, GdiFontManager.ToGdi(_font1), new PointF(0, 0), strfmt);
        _size2 = g.MeasureString(_exponent, GdiFontManager.ToGdi(_font2), new PointF(_size1.Width, 0), strfmt);
        _size3 = g.MeasureString(_lastpart, GdiFontManager.ToGdi(_font1), new PointF(0, 0), strfmt);
        _rightPadding = maxexposize - _size2.Width;
      }

      /// <inheritdoc />
      public virtual SizeF Size
      {
        get
        {
          return new SizeF(_size1.Width + _size2.Width + _rightPadding + _size3.Width, _size1.Height);
        }
      }

      /// <inheritdoc />
      public virtual void Draw(Graphics g, BrushXEnv brush, PointF point)
      {
        using (var gdibrush = BrushCacheGdi.Instance.BorrowBrush(brush))
        {
          g.DrawString(_firstpart, GdiFontManager.ToGdi(_font1), gdibrush, point, _strfmt);

          point.X += _size1.Width;
          point.Y += 0;

          g.DrawString(_exponent, GdiFontManager.ToGdi(_font2), gdibrush, point, _strfmt);

          point.X += _size2.Width;
          g.DrawString(_lastpart, GdiFontManager.ToGdi(_font1), gdibrush, point, _strfmt);
        }
      }

      #endregion IMeasuredLabelItem Members
    }
  }
}
