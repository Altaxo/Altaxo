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
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
  /// <summary>
  /// Interpolates linearly between two colors by linearly interpolate the A, the R, the G and the B value of the two colors.
  /// </summary>
  public class ColorProviderARGBGradient : ColorProviderBase
  {
    private const double maxColorComponent = 255.999;

    private double _alpha0 = 1;
    private double _alpha1 = 1;
    private double _red0 = 0;
    private double _red1 = 1;
    private double _green0 = 0;
    private double _green1 = 1;
    private double _blue0 = 0;
    private double _blue1 = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderARGBGradient), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColorProviderARGBGradient)obj;
        info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));
        info.AddValue("Alpha0", s._alpha0);
        info.AddValue("Alpha1", s._alpha1);
        info.AddValue("Red0", s._red0);
        info.AddValue("Red1", s._red1);
        info.AddValue("Green0", s._green0);
        info.AddValue("Green1", s._green1);
        info.AddValue("Blue0", s._blue0);
        info.AddValue("Blue1", s._blue1);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ColorProviderARGBGradient?)o ?? new ColorProviderARGBGradient();
        info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);
        s._alpha0 = info.GetDouble("Alpha0");
        s._alpha1 = info.GetDouble("Alpha1");
        s._red0 = info.GetDouble("Red0");
        s._red1 = info.GetDouble("Red1");
        s._green0 = info.GetDouble("Green0");
        s._green1 = info.GetDouble("Green1");
        s._blue0 = info.GetDouble("Blue0");
        s._blue1 = info.GetDouble("Blue1");

        return s;
      }
    }

    #endregion Serialization

    public override bool Equals(IColorProvider? other)
    {
      if (!base.Equals(other))
        return false;

      var from = (ColorProviderARGBGradient)other;

      return
        _alpha0 == from._alpha0 &&
        _alpha1 == from._alpha1 &&
        _red0 == from._red0 &&
        _red1 == from._red1 &&
        _green0 == from._green0 &&
        _green1 == from._green1 &&
        _blue0 == from._blue0 &&
        _blue1 == from._blue1;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + (_alpha0 + _alpha1 + _red0 + _red1 + _green0 + _green1 + _blue0 + _blue1).GetHashCode() * 13;
    }

    public double Red0
    {
      get
      {
        return _red0;
      }
    }

    public ColorProviderARGBGradient WithRed0(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _red0)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._red0 = value;
        return result;
      }
    }

    public double Red1
    {
      get
      {
        return _red1;
      }
    }

    public ColorProviderARGBGradient WithRed1(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _red1)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._red1 = value;
        return result;
      }
    }

    public double Green0
    {
      get
      {
        return _green0;
      }
    }

    public ColorProviderARGBGradient WithGreen0(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _green0)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._green0 = value;
        return result;
      }
    }

    public double Green1
    {
      get
      {
        return _green1;
      }
    }

    public ColorProviderARGBGradient WithGreen1(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _green1)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._green1 = value;
        return result;
      }
    }

    public double Blue0
    {
      get
      {
        return _blue0;
      }
    }

    public ColorProviderARGBGradient WithBlue0(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _blue0)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._blue0 = value;
        return result;
      }
    }

    public double Blue1
    {
      get
      {
        return _blue1;
      }
    }

    public ColorProviderARGBGradient WithBlue1(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _blue1)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._blue1 = value;
        return result;
      }
    }

    public double Opaqueness0
    {
      get
      {
        return _alpha0;
      }
    }

    public ColorProviderARGBGradient WithOpaqueness0(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _alpha0)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._alpha0 = value;
        return result;
      }
    }

    public double Opaqueness1
    {
      get
      {
        return _alpha1;
      }
    }

    public ColorProviderARGBGradient WithOpaqueness1(double value)
    {
      if (!(value >= 0 && value <= 1))
        throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

      if (value == _alpha1)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._alpha1 = value;
        return result;
      }
    }

    public Color ColorAtR0
    {
      get
      {
        return GetColorFrom0To1Continuously(0);
      }
    }

    public ColorProviderARGBGradient WithColorAtR0(Color value)
    {
      double a, r, g, b;
      a = value.A / 255.0;
      r = value.R / 255.0;
      g = value.G / 255.0;
      b = value.B / 255.0;

      if (a == _alpha0 || r == _red0 || g == _green0 || b == _blue0)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._alpha0 = a;
        result._red0 = r;
        result._green0 = g;
        result._blue0 = b;

        return result;
      }
    }

    public Color ColorAtR1
    {
      get
      {
        return GetColorFrom0To1Continuously(1);
      }
    }

    public ColorProviderARGBGradient WithColorAtR1(Color value)
    {
      double a, r, g, b;
      a = value.A / 255.0;
      r = value.R / 255.0;
      g = value.G / 255.0;
      b = value.B / 255.0;

      if (a == _alpha1 || r == _red1 || g == _green1 || b == _blue1)
      {
        return this;
      }
      else
      {
        var result = (ColorProviderARGBGradient)MemberwiseClone();
        result._alpha1 = a;
        result._red1 = r;
        result._green1 = g;
        result._blue1 = b;

        return result;
      }
    }

    /// <summary>
    /// Calculates a color from the provided relative value, that is guaranteed to be between 0 and 1
    /// </summary>
    /// <param name="relVal">Value used for color calculation. Guaranteed to be between 0 and 1.</param>
    /// <returns>A color associated with the relative value.</returns>
    protected override Color GetColorFrom0To1Continuously(double relVal)
    {
      double r0 = 1 - relVal;
      double r1 = relVal;

      return Color.FromArgb(
        (int)Math.Floor((_alpha0 * r0 + _alpha1 * r1) * (1 - Transparency) * maxColorComponent),
        (int)Math.Floor((_red0 * r0 + _red1 * r1) * maxColorComponent),
        (int)Math.Floor((_green0 * r0 + _green1 * r1) * maxColorComponent),
        (int)Math.Floor((_blue0 * r0 + _blue1 * r1) * maxColorComponent)
        );
    }
  }
}
