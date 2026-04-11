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
using Altaxo.Data;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.LabelFormatting
{
  /// <summary>
  /// Formats a numeric item in scientific notation, i.e. in the form mantissa*10^exponent.
  /// </summary>
  public class NumericLabelFormattingScientific : LabelFormattingBase
  {
    private bool _showExponentAlways;

    #region Serialization


    /// <summary>
    /// Serializes <see cref="NumericLabelFormattingScientific"/> instances.
    /// </summary>
    /// <remarks>
    /// Initial version added on 2016-03-02.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingScientific), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingScientific)obj;
        info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingScientific).BaseType!);
        info.AddValue("ShowExponentAlways", s._showExponentAlways);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingScientific?)o ?? new NumericLabelFormattingScientific();
        info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingScientific).BaseType!, parent);
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
    /// Initializes a new instance of the <see cref="NumericLabelFormattingScientific"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public NumericLabelFormattingScientific(NumericLabelFormattingScientific from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override object Clone()
    {
      return new NumericLabelFormattingScientific(this);
    }

    /// <inheritdoc/>
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
        var oldValue = _showExponentAlways;
        _showExponentAlways = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <inheritdoc/>
    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    /// <summary>
    /// Formats the specified numeric tick value.
    /// </summary>
    /// <param name="tick">The tick value to format.</param>
    /// <returns>The formatted tick label.</returns>
    public string FormatItem(double tick)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    /// <summary>
    /// Splits a numeric value into mantissa text and exponent text.
    /// </summary>
    /// <param name="ditem">The numeric value.</param>
    /// <param name="firstpart">The text before the exponent part.</param>
    /// <param name="mant">The mantissa value.</param>
    /// <param name="middelpart">The separator text between mantissa and exponent.</param>
    /// <param name="exponent">The exponent text.</param>
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
      if (!(posOfE > 0))
        throw new InvalidProgramException();

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

    /// <summary>
    /// Measures the specified item.
    /// </summary>
    /// <param name="g">The graphics context used for measurement.</param>
    /// <param name="font">The font used for measurement.</param>
    /// <param name="mtick">The value to measure.</param>
    /// <param name="morg">The measurement origin.</param>
    /// <returns>The measured size of the formatted label.</returns>
    public override VectorD3D MeasureItem(IGraphicsContext3D g, FontX3D font, Altaxo.Data.AltaxoVariant mtick, PointD3D morg)
    {
      SplitInFirstPartAndExponent(mtick, out var firstpart, out var mant, out var middelpart, out var exponent);

      var size1 = g.MeasureString(_prefix + firstpart + middelpart, font, PointD3D.Empty);
      var size2 = g.MeasureString(exponent, font, PointD3D.Empty);
      var size3 = g.MeasureString(_suffix, font, PointD3D.Empty);

      return new VectorD3D(size1.X + size2.X + size3.X, size1.Y, font.Depth);
    }

    /// <summary>
    /// Draws the specified item.
    /// </summary>
    /// <param name="g">The graphics context used for drawing.</param>
    /// <param name="brush">The material used to draw the label.</param>
    /// <param name="font">The font used to draw the label.</param>
    /// <param name="item">The value to draw.</param>
    /// <param name="morg">The drawing origin.</param>
    public override void DrawItem(IGraphicsContext3D g, IMaterial brush, FontX3D font, Altaxo.Data.AltaxoVariant item, PointD3D morg)
    {
      SplitInFirstPartAndExponent(item, out var firstpart, out var mant, out var middelpart, out var exponent);

      var size1 = g.MeasureString(_prefix + firstpart + middelpart, font, morg);
      g.DrawString(_prefix + firstpart + middelpart, font, brush, morg);
      var orginalY = morg.Y;
      morg = morg + new VectorD3D(size1.X, size1.Y, 0);
      var font2 = font.WithSize(font.Size * 2 / 3.0);
      g.DrawString(exponent, font2, brush, morg);
      if (!string.IsNullOrEmpty(_suffix))
      {
        var shiftX = g.MeasureString(exponent, font2, morg).X;
        morg = new PointD3D(morg.X + shiftX, orginalY, morg.Z);
      }

      if (!string.IsNullOrEmpty(_suffix))
      {
        g.DrawString(_suffix, font, brush, morg);
      }
    }

    /// <summary>
    /// Gets measured label items for the provided values.
    /// </summary>
    /// <param name="g">The graphics context used for measurement.</param>
    /// <param name="font">The font used for measurement.</param>
    /// <param name="items">The values to convert into measured label items.</param>
    /// <returns>The measured label items.</returns>
    public override IMeasuredLabelItem[] GetMeasuredItems(IGraphicsContext3D g, FontX3D font, AltaxoVariant[] items)
    {
      var litems = new MeasuredLabelItem[items.Length];

      var localfont1 = font;
      var localfont2 = font.WithSize(font.Size * 2 / 3);

      string[] firstp = new string[items.Length];
      string[] middel = new string[items.Length];
      string[] expos = new string[items.Length];
      double[] mants = new double[items.Length];

      double maxexposize = 0;
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
        maxexposize = Math.Max(maxexposize, g.MeasureString(exponent, localfont2, PointD3D.Empty).X);
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
        litems[i] = new MeasuredLabelItem(g, localfont1, localfont2, _prefix + firstp[i] + mid, expos[i], _suffix, maxexposize);
      }

      return litems;
    }

    /// <summary>
    /// Represents a measured scientific label item.
    /// </summary>
    protected new class MeasuredLabelItem : IMeasuredLabelItem
    {
      /// <summary>
      /// Stores the mantissa text.
      /// </summary>
      protected string _firstpart;
      /// <summary>
      /// Stores the exponent text.
      /// </summary>
      protected string _exponent;
      /// <summary>
      /// Stores the suffix text.
      /// </summary>
      protected string _lastpart;
      /// <summary>
      /// Stores the main font.
      /// </summary>
      protected FontX3D _font1;
      /// <summary>
      /// Stores the exponent font.
      /// </summary>
      protected FontX3D _font2;
      /// <summary>
      /// Stores the measured size of the mantissa part.
      /// </summary>
      protected VectorD3D _size1;
      /// <summary>
      /// Stores the measured size of the exponent part.
      /// </summary>
      protected VectorD3D _size2;
      /// <summary>
      /// Stores the measured size of the suffix part.
      /// </summary>
      protected VectorD3D _size3;
      /// <summary>
      /// Stores the right padding used to align exponents.
      /// </summary>
      protected double _rightPadding;

      #region IMeasuredLabelItem Members

      /// <summary>
      /// Initializes a new measured scientific label item.
      /// </summary>
      /// <param name="g">The graphics context used for measurement.</param>
      /// <param name="font1">The primary font.</param>
      /// <param name="font2">The font used for the exponent.</param>
      /// <param name="firstpart">The mantissa text.</param>
      /// <param name="exponent">The exponent text.</param>
      /// <param name="lastpart">The suffix text.</param>
      /// <param name="maxexposize">The maximum exponent width used for alignment.</param>
      public MeasuredLabelItem(IGraphicsContext3D g, FontX3D font1, FontX3D font2, string firstpart, string exponent, string lastpart, double maxexposize)
      {
        _firstpart = firstpart;
        _exponent = exponent;
        _lastpart = lastpart;
        _font1 = font1;
        _font2 = font2;
        _size1 = g.MeasureString(_firstpart, _font1, PointD3D.Empty);
        _size2 = g.MeasureString(_exponent, _font2, new PointD3D(_size1.X, 0, 0));
        _size3 = g.MeasureString(_lastpart, _font1, PointD3D.Empty);
        _rightPadding = maxexposize - _size2.X;
      }

      /// <summary>
      /// Gets the measured size of the label.
      /// </summary>
      public virtual VectorD3D Size
      {
        get
        {
          return new VectorD3D(_size1.X + _size2.X + _rightPadding + _size3.X, _size1.Y, _font1.Depth);
        }
      }

      /// <summary>
      /// Draws the measured label.
      /// </summary>
      /// <param name="g">The graphics context used for drawing.</param>
      /// <param name="brush">The material used to draw the label.</param>
      /// <param name="point">The drawing origin.</param>
      public virtual void Draw(IGraphicsContext3D g, IMaterial brush, PointD3D point)
      {
        g.DrawString(_firstpart, _font1, brush, point);

        point = point.WithXPlus(_size1.X);

        g.DrawString(_exponent, _font2, brush, point.WithYPlus(_size1.Y - _size2.Y));

        point = point.WithXPlus(_size2.X);

        g.DrawString(_lastpart, _font1, brush, point);
      }

      #endregion IMeasuredLabelItem Members
    }
  }
}
