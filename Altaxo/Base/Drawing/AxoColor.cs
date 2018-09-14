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
using System.Text;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Type of colors that is shown e.g. in comboboxes.
  /// </summary>
  [Serializable]
  public enum ColorType
  {
    /// <summary>
    /// Known colors and system colors are shown.
    /// </summary>
    KnownAndSystemColor,

    /// <summary>
    /// Known colors are shown.
    /// </summary>
    KnownColor,

    /// <summary>
    /// Only plot colors are shown.
    /// </summary>
    PlotColor
  }

  public static class GdiColorHelper
  {
    public static NamedColor ToNamedColor(System.Drawing.Color c)
    {
      return new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B));
    }

    public static NamedColor ToNamedColor(System.Drawing.Color c, string name)
    {
      return new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), name);
    }

    public static System.Drawing.Color ToGdi(NamedColor color)
    {
      var c = color.Color;
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }
  }

  [Serializable]
  public struct AxoColor : IEquatable<AxoColor>
  {
    private bool _isFromArgb;
    private byte _a, _r, _g, _b;
    private float _scA, _scR, _scG, _scB;

    public float ScA { get { return _scA; } set { _scA = value; _a = A2I(value); _isFromArgb = false; } }

    public float ScR { get { return _scR; } set { _scR = value; _r = GammaCorrectedFromLinear(value); _isFromArgb = false; } }

    public float ScG { get { return _scG; } set { _scG = value; _g = GammaCorrectedFromLinear(value); _isFromArgb = false; } }

    public float ScB { get { return _scB; } set { _scB = value; _b = GammaCorrectedFromLinear(value); _isFromArgb = false; } }

    public byte A { get { return _a; } set { _a = value; _scA = I2A(value); } }

    public byte R { get { return _r; } set { _r = value; _scR = LinearFromGammaCorrected(value); } }

    public byte G { get { return _g; } set { _g = value; _scG = LinearFromGammaCorrected(value); } }

    public byte B { get { return _b; } set { _b = value; _scB = LinearFromGammaCorrected(value); } }

    public bool IsFromArgb
    {
      get
      {
        return _isFromArgb;
      }
      set
      {
        _isFromArgb = value;
      }
    }

    /// <summary>
    /// Converts a color in given as ARGB integer into the linear SC ARGB space.
    /// </summary>
    /// <param name="a">Alpha component.</param>
    /// <param name="r">Red component-</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <returns></returns>
    public static Tuple<float, float, float, float> ToScARGBFromIARGB(byte a, byte r, byte g, byte b)
    {
      return new Tuple<float, float, float, float>(I2A(a), LinearFromGammaCorrected(r), LinearFromGammaCorrected(g), LinearFromGammaCorrected(b));
    }

    /// <summary>
    /// Convert from linear SRGB to gamma corrected values, see wikipedia (SRGB).
    /// </summary>
    /// <param name="x"></param>
    /// <returns>Gamma corrected values (range 0.255).</returns>
    public static byte GammaCorrectedFromLinear(float x)
    {
      double r;
      if (x <= 0.0031308)
        r = x * 12.92;
      else
        r = 1.055 * Math.Pow(x, (1 / 2.4)) - 0.055;

      r = Math.Round(r * 255);
      if (!(r > 0))
        r = 0;
      if (!(r <= 255))
        r = 255;
      return (byte)r;
    }

    /// <summary>
    /// Conversion to linear SRGB, see wikipedia (SRGB).
    /// </summary>
    /// <param name="x"></param>
    /// <returns>Linear SRGB value (normal range 0..1).</returns>
    public static float LinearFromGammaCorrected(byte x)
    {
      const double fac = 1 / 255.0;
      var n = (x * fac);

      if (n < 0.04045)
        return (float)(n / 12.92);
      else
        return (float)Math.Pow((n + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Convert the alpha channel value in the range 0..1 to a byte value in the range 0..255.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static byte A2I(float x)
    {
      double r;
      r = Math.Round(x * 255);
      if (!(r > 0))
        r = 0;
      if (!(r <= 255))
        r = 255;
      return (byte)r;
    }

    /// <summary>
    /// Convert the alpha channel value (0..255) into a float value (0..1).
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float I2A(byte x)
    {
      const double fac = 1 / 255.0;
      return (float)(x * fac);
    }

    public static AxoColor FromScRgb(float a, float r, float g, float b)
    {
      return new AxoColor() { ScA = a, ScR = r, ScG = g, ScB = b, _isFromArgb = false };
    }

    public static AxoColor FromArgb(byte a, byte r, byte g, byte b)
    {
      return new AxoColor() { A = a, R = r, G = g, B = b, _isFromArgb = true };
    }

    public int ToArgb()
    {
      return _a << 24 | _r << 16 | _g << 8 | _b;
    }

    public AxoColor ToFullyOpaque()
    {
      return ToFullyOpaque(this);
    }

    public static AxoColor ToFullyOpaque(AxoColor c)
    {
      AxoColor result = c;
      result._a = 255;
      result._scA = 1;
      return result;
    }

    public AxoColor ToAlphaValue(byte alpha)
    {
      return ToAlphaValue(this, alpha);
    }

    public static AxoColor ToAlphaValue(AxoColor c, byte alpha)
    {
      AxoColor result = c;
      result._a = alpha;
      result._scA = I2A(alpha);
      return result;
    }

    public override int GetHashCode()
    {
      if (_isFromArgb)
        return _a << 24 + _b << 16 + _g << 8 + _r;
      else
        return (_scA * 4294967296.0 + _scB * 16777216.0 + _scG * 65536.0 + _scR * 256.0).GetHashCode();
    }

    public bool Equals(AxoColor from)
    {
      if (_isFromArgb && from._isFromArgb)
        return
          _a == from._a &&
          _b == from._b &&
          _g == from._g &&
          _r == from._r;
      else if (!_isFromArgb && !from._isFromArgb)
        return
          _scA == from._scA &&
          _scB == from._scB &&
          _scG == from._scG &&
          _scR == from._scR;
      else
        return
          _a == from._a &&
          _b == from._b &&
          _g == from._g &&
          _r == from._r &&
          _scA == from._scA &&
          _scB == from._scB &&
          _scG == from._scG &&
          _scR == from._scR;
    }

    public override bool Equals(object obj)
    {
      if (obj is AxoColor)
        return Equals((AxoColor)obj);
      else
        return false;
    }

    public static bool operator ==(AxoColor x, AxoColor y)
    {
      return x.Equals(y);
    }

    public static bool operator !=(AxoColor x, AxoColor y)
    {
      return !(x.Equals(y));
    }

    public override string ToString()
    {
      return ToInvariantString();
    }

    public string ToInvariantString()
    {
      if (_isFromArgb)
      {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{#{0:X2}{1:X2}{2:X2}{3:X2}{4}", _a, _r, _g, _b, '}');
      }
      else
      {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{sc#{0:R}, {1:R}, {2:R}, {3:R}{4}", _scA, _scR, _scG, _scB, '}');
      }
    }

    public static AxoColor FromInvariantString(string val)
    {
      if (val.StartsWith("{sc#"))
      {
        int first = 4;
        int next = val.IndexOf(',', first);
        if (next < 0)
          throw new ArgumentException("Wrong color format: body unrecognized", "val");
        var a = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

        first = next + 1;
        next = val.IndexOf(',', first);
        if (next < 0)
          throw new ArgumentException("Wrong color format: body unrecognized", "val");
        var r = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

        first = next + 1;
        next = val.IndexOf(',', first);
        if (next < 0)
          throw new ArgumentException("Wrong color format: body unrecognized", "val");
        var g = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

        first = next + 1;
        next = val.Length - 1;
        var b = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

        return AxoColor.FromScRgb(a, r, g, b);
      }
      else if (val.StartsWith("{#"))
      {
        var u = uint.Parse(val.Substring(2, 8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
        return AxoColor.FromArgb((byte)(u >> 24), (byte)(u >> 16), (byte)(u >> 8), (byte)u);
      }
      else
      {
        throw new ArgumentException("Wrong color format: header unrecognized", "val");
      }
    }

    #region Conversion operators

    public static implicit operator System.Drawing.Color(AxoColor c)
    {
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    #endregion Conversion operators

    #region Conversion to/from other color models

    #region HSL model

    /// <summary>
    /// Converts Hsl (Hue, Saturation, Lightness) values (all values in the range [0, 1] into
    /// the linear Rgb space.
    /// </summary>
    /// <param name="hue">The hue value (0..1).</param>
    /// <param name="saturation">The saturation value (0..1).</param>
    /// <param name="lightness">The lightness value (0..1).</param>
    /// <returns>The red, green, blue components in linear color space.</returns>
    public static (float red, float green, float blue) LinearRgbFromHsl(float hue, float saturation, float lightness)
    {
      if (!(hue >= 0 && hue <= 1))
        throw new ArgumentOutOfRangeException(nameof(hue), hue, "Value must be within a range of 0 - 1.");

      if (!(saturation >= 0 && saturation <= 1))
        throw new ArgumentOutOfRangeException(nameof(saturation), saturation, "Value must be within a range of 0 - 1.");

      if (!(lightness >= 0 && lightness <= 1))
        throw new ArgumentOutOfRangeException(nameof(lightness), lightness, "Value must be within a range of 0 - 1.");

      if (0 == saturation)
      {
        return (lightness, lightness, lightness);
      }

      float fMax, fMid, fMin;

      if (0.5 < lightness)
      {
        fMax = lightness - (lightness * saturation) + saturation;
        fMin = lightness + (lightness * saturation) - saturation;
      }
      else
      {
        fMax = lightness + (lightness * saturation);
        fMin = lightness - (lightness * saturation);
      }

      hue *= 6;
      var iSextant = (int)Math.Floor(hue);
      if (hue >= 5)
      {
        hue -= 6;
      }
      hue -= 2 * (float)Math.Floor((double)((iSextant + 1) % 6) / 2);
      if (0 == iSextant % 2)
      {
        fMid = (hue * (fMax - fMin)) + fMin;
      }
      else
      {
        fMid = fMin - (hue * (fMax - fMin));
      }

      switch (iSextant)
      {
        case 1:
          return (fMid, fMax, fMin);

        case 2:
          return (fMin, fMax, fMid);

        case 3:
          return (fMin, fMid, fMax);

        case 4:
          return (fMid, fMin, fMax);

        case 5:
          return (fMax, fMin, fMid);

        default:
          return (fMax, fMid, fMin);
      }
    }

    /// <summary>
    /// Creates a Color from alpha, hue, saturation and lightness (AHSL model).
    /// </summary>
    /// <param name="alpha">The alpha channel value.</param>
    /// <param name="hue">The hue value (0..1).</param>
    /// <param name="saturation">The saturation value (0..1).</param>
    /// <param name="lightness">The lightness value (0..1).</param>
    /// <returns>A Color with the given values.</returns>
    public static AxoColor FromAhsl(float alpha, float hue, float saturation, float lightness)
    {
      var (red, green, blue) = LinearRgbFromHsl(hue, saturation, lightness);
      return AxoColor.FromScRgb(alpha, red, green, blue);
    }

    #endregion HSL model

    #region Hsb model

    /// <summary>
    /// Creates a color from alpha, hue, saturation and brightness (AHSB model).
    /// </summary>
    /// <param name="alpha">The alpha value (0..1).</param>
    /// <param name="hue">The hue value (0..1).</param>
    /// <param name="saturation">The saturation value (0..1).</param>
    /// <param name="brightness">The brightness value (0..1).</param>
    /// <returns>The created color.</returns>
    public static AxoColor FromAhsb(float alpha, float hue, float saturation, float brightness)
    {
      var (red, green, blue) = LinearRgbFromHsb(hue, saturation, brightness);
      return AxoColor.FromScRgb(alpha, red, green, blue);
    }

    /// <summary>
    /// Converts the color to the linear AHSB model, with alpha [0, 1], Hue [0, 1], Saturation [0, 1] and Brightness [0, 1].
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidProgramException"></exception>
    public (float alpha, float hue, float saturation, float brightness) ToAhsb()
    {
      var (h, s, b) = HsbFromLinearRgb(ScR, ScG, ScB);
      return (ScA, h, s, b);
    }

    /// <summary>
    /// Creates a Color from alpha, hue, saturation and brightness (AHSB model).
    /// </summary>
    /// <param name="hue">The hue value (0..1).</param>
    /// <param name="saturation">The saturation value (0..1).</param>
    /// <param name="brightness">The brightness value (0..1).</param>
    /// <returns>The red, green, blue components in linear Rgb color space.</returns>
    public static (float red, float green, float blue) LinearRgbFromHsb(float hue, float saturation, float brightness)
    {
      if (!(hue >= 0 && hue <= 1))
        throw new ArgumentOutOfRangeException(nameof(hue), hue, "Value must be within a range of 0 - 1.");

      if (!(saturation >= 0 && saturation <= 1))
        throw new ArgumentOutOfRangeException(nameof(saturation), saturation, "Value must be within a range of 0 - 1.");

      if (!(brightness >= 0 && brightness <= 1))
        throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Value must be within a range of 0 - 1.");

      float red = 0, green = 0, blue = 0;
      if (saturation == 0)
      {
        red = green = blue = brightness;
      }
      else
      {
        float h = (hue - (float)Math.Floor(hue)) * 6;
        float f = h - (float)Math.Floor(h);
        float p = brightness * (1 - saturation);
        float q = brightness * (1 - saturation * f);
        float t = brightness * (1 - (saturation * (1 - f)));
        switch ((int)h)
        {
          case 0:
            red = brightness;
            green = t;
            blue = p;
            break;

          case 1:
            red = q;
            green = brightness;
            blue = p;
            break;

          case 2:
            red = p;
            green = brightness;
            blue = t;
            break;

          case 3:
            red = p;
            green = q;
            blue = brightness;
            break;

          case 4:
            red = t;
            green = p;
            blue = brightness;
            break;

          case 5:
            red = brightness;
            green = p;
            blue = q;
            break;
        }
      }
      return (red, green, blue);
    }

    /// <summary>
    /// Converts the linear Rgb components to the AHSB model, with Hue [0, 1], Saturation [0, 1] and Brightness [0, 1].
    /// </summary>
    /// <returns>The hue, saturation and brightness components (all in the range [0, 1].</returns>
    /// <exception cref="System.InvalidProgramException"></exception>
    public static (float hue, float saturation, float brightness) HsbFromLinearRgb(float r, float g, float b)
    {
      float hue = 0, saturation = 0, brightness = 0;

      var max = Math.Max(Math.Max(r, g), b);
      var min = Math.Min(Math.Min(r, g), b);

      if (max == min)
      {
        hue = 0;
      }
      else if (max == r)
      {
        hue = (g - b) / (max - min);
      }
      else if (max == g)
      {
        hue = 2 + (b - r) / (max - min);
      }
      else if (max == b)
      {
        hue = 4 + (r - g) / (max - min);
      }
      else
      {
        throw new InvalidProgramException();
      }

      if (hue < 0)
        hue += 6;
      hue /= 6;

      saturation = 0 == max ? 0 : (max - min) / max;

      brightness = max;

      return (hue, saturation, brightness);
    }

    #endregion Hsb model

    #region Hue alone

    /// <summary>
    /// Gets the hue value of the color.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidProgramException"></exception>
    public float GetHue()
    {
      float hue = 0;
      var r = ScR;
      var g = ScG;
      var b = ScB;

      var max = Math.Max(Math.Max(r, g), b);
      var min = Math.Min(Math.Min(r, g), b);

      if (max == min)
      {
        hue = 0;
      }
      else if (max == r)
      {
        hue = (g - b) / (max - min);
      }
      else if (max == g)
      {
        hue = 2 + (b - r) / (max - min);
      }
      else if (max == b)
      {
        hue = 4 + (r - g) / (max - min);
      }
      else
      {
        throw new InvalidProgramException();
      }

      if (hue < 0)
        hue += 6;

      return hue / 6;
    }

    /// <summary>
    /// Creates a fully saturated bright color from hue, returning gamma corrected components R, G, and B.
    /// </summary>
    /// <param name="hue">The hue value (0..1).</param>
    /// <param name="r">Calculated red component.</param>
    /// <param name="g">Calculated green component.</param>
    /// <param name="b">Calculated blue component.</param>
    public static void FromHue(float hue, out byte r, out byte g, out byte b)
    {
      float h = (hue - (float)Math.Floor(hue)) * 6;
      float f = h - (float)Math.Floor(h);

      float p = 0;
      float q = (1 - f);
      float t = f;

      switch ((int)h)
      {
        case 0:
          r = GammaCorrectedFromLinear(1);
          g = GammaCorrectedFromLinear(t);
          b = GammaCorrectedFromLinear(p);
          break;

        case 1:
          r = GammaCorrectedFromLinear(q);
          g = GammaCorrectedFromLinear(1);
          b = GammaCorrectedFromLinear(p);
          break;

        case 2:
          r = GammaCorrectedFromLinear(p);
          g = GammaCorrectedFromLinear(1);
          b = GammaCorrectedFromLinear(t);
          break;

        case 3:
          r = GammaCorrectedFromLinear(p);
          g = GammaCorrectedFromLinear(q);
          b = GammaCorrectedFromLinear(1);
          break;

        case 4:
          r = GammaCorrectedFromLinear(t);
          g = GammaCorrectedFromLinear(p);
          b = GammaCorrectedFromLinear(1);
          break;

        case 5:
          r = GammaCorrectedFromLinear(1);
          g = GammaCorrectedFromLinear(p);
          b = GammaCorrectedFromLinear(q);
          break;

        default:
          throw new NotImplementedException();
      }
    }

    #endregion Hue alone

    #region CMYK

    /// <summary>
    /// Provides a quick and dirty conversion from the CMYK color model to an <see cref="AxoColor"/>. No color conversion profile is used here!
    /// </summary>
    /// <param name="alpha">The alpha value in the range [0, 1].</param>
    /// <param name="c">The calpha value in the range [0, 1].</param>
    /// <param name="y">The yalpha value in the range [0, 1].</param>
    /// <param name="m">The malpha value in the range [0, 1].</param>
    /// <param name="k">The kalpha value in the range [0, 1].</param>
    /// <returns></returns>
    public static AxoColor FromAcmyk(float alpha, float c, float y, float m, float k)
    {
      var r = 1 - Math.Min(1, c * (1 - k) + k);
      var g = 1 - Math.Min(1, m * (1 - k) + k);
      var b = 1 - Math.Min(1, y * (1 - k) + k);

      return AxoColor.FromScRgb(alpha, r, g, b);
    }

    /// <summary>
    /// Provides a quick and dirty conversion from an <see cref="AxoColor"/> to the CMYK model. No color conversion profile is used here!
    /// </summary>
    /// <returns>The components for alpha, C, M, Y and K. All components are in the range [0, 1].</returns>
    public (float alpha, float c, float m, float y, float k) ToAcmyk()
    {
      var c = 1 - ScR;
      var m = 1 - ScG;
      var y = 1 - ScB;
      var min = Math.Min(c, Math.Min(m, y));

      float cc, mm, yy, kk;

      if ((1f - min) < 1E-5)
      {
        cc = mm = yy = 0;
        kk = 1;
      }
      else
      {
        cc = ((c - min) / (1f - min));
        mm = ((m - min) / (1f - min));
        yy = ((y - min) / (1f - min));
        kk = min;
      }
      return (ScA, cc, mm, yy, kk);
    }

    #endregion CMYK

    public static byte NormFloatToByte(float f)
    {
      return (byte)(f * 255 + 0.5f);
    }

    public static float ByteToNormFloat(byte val)
    {
      return val / 255.0f;
    }

    #endregion Conversion to/from other color models
  }

  public static class AxoColors
  {
    #region Generated code

    private static List<AxoColor> _colors = new List<AxoColor> {
AxoColor.FromArgb(255, 240, 248, 255),
AxoColor.FromArgb(255, 250, 235, 215),
AxoColor.FromArgb(255, 0, 255, 255),
AxoColor.FromArgb(255, 127, 255, 212),
AxoColor.FromArgb(255, 240, 255, 255),
AxoColor.FromArgb(255, 245, 245, 220),
AxoColor.FromArgb(255, 255, 228, 196),
AxoColor.FromArgb(255, 0, 0, 0),
AxoColor.FromArgb(255, 255, 235, 205),
AxoColor.FromArgb(255, 0, 0, 255),
AxoColor.FromArgb(255, 138, 43, 226),
AxoColor.FromArgb(255, 165, 42, 42),
AxoColor.FromArgb(255, 222, 184, 135),
AxoColor.FromArgb(255, 95, 158, 160),
AxoColor.FromArgb(255, 127, 255, 0),
AxoColor.FromArgb(255, 210, 105, 30),
AxoColor.FromArgb(255, 255, 127, 80),
AxoColor.FromArgb(255, 100, 149, 237),
AxoColor.FromArgb(255, 255, 248, 220),
AxoColor.FromArgb(255, 220, 20, 60),
AxoColor.FromArgb(255, 0, 255, 255),
AxoColor.FromArgb(255, 0, 0, 139),
AxoColor.FromArgb(255, 0, 139, 139),
AxoColor.FromArgb(255, 184, 134, 11),
AxoColor.FromArgb(255, 169, 169, 169),
AxoColor.FromArgb(255, 0, 100, 0),
AxoColor.FromArgb(255, 189, 183, 107),
AxoColor.FromArgb(255, 139, 0, 139),
AxoColor.FromArgb(255, 85, 107, 47),
AxoColor.FromArgb(255, 255, 140, 0),
AxoColor.FromArgb(255, 153, 50, 204),
AxoColor.FromArgb(255, 139, 0, 0),
AxoColor.FromArgb(255, 233, 150, 122),
AxoColor.FromArgb(255, 143, 188, 143),
AxoColor.FromArgb(255, 72, 61, 139),
AxoColor.FromArgb(255, 47, 79, 79),
AxoColor.FromArgb(255, 0, 206, 209),
AxoColor.FromArgb(255, 148, 0, 211),
AxoColor.FromArgb(255, 255, 20, 147),
AxoColor.FromArgb(255, 0, 191, 255),
AxoColor.FromArgb(255, 105, 105, 105),
AxoColor.FromArgb(255, 30, 144, 255),
AxoColor.FromArgb(255, 178, 34, 34),
AxoColor.FromArgb(255, 255, 250, 240),
AxoColor.FromArgb(255, 34, 139, 34),
AxoColor.FromArgb(255, 255, 0, 255),
AxoColor.FromArgb(255, 220, 220, 220),
AxoColor.FromArgb(255, 248, 248, 255),
AxoColor.FromArgb(255, 255, 215, 0),
AxoColor.FromArgb(255, 218, 165, 32),
AxoColor.FromArgb(255, 128, 128, 128),
AxoColor.FromArgb(255, 0, 128, 0),
AxoColor.FromArgb(255, 173, 255, 47),
AxoColor.FromArgb(255, 240, 255, 240),
AxoColor.FromArgb(255, 255, 105, 180),
AxoColor.FromArgb(255, 205, 92, 92),
AxoColor.FromArgb(255, 75, 0, 130),
AxoColor.FromArgb(255, 255, 255, 240),
AxoColor.FromArgb(255, 240, 230, 140),
AxoColor.FromArgb(255, 230, 230, 250),
AxoColor.FromArgb(255, 255, 240, 245),
AxoColor.FromArgb(255, 124, 252, 0),
AxoColor.FromArgb(255, 255, 250, 205),
AxoColor.FromArgb(255, 173, 216, 230),
AxoColor.FromArgb(255, 240, 128, 128),
AxoColor.FromArgb(255, 224, 255, 255),
AxoColor.FromArgb(255, 250, 250, 210),
AxoColor.FromArgb(255, 211, 211, 211),
AxoColor.FromArgb(255, 144, 238, 144),
AxoColor.FromArgb(255, 255, 182, 193),
AxoColor.FromArgb(255, 255, 160, 122),
AxoColor.FromArgb(255, 32, 178, 170),
AxoColor.FromArgb(255, 135, 206, 250),
AxoColor.FromArgb(255, 119, 136, 153),
AxoColor.FromArgb(255, 176, 196, 222),
AxoColor.FromArgb(255, 255, 255, 224),
AxoColor.FromArgb(255, 0, 255, 0),
AxoColor.FromArgb(255, 50, 205, 50),
AxoColor.FromArgb(255, 250, 240, 230),
AxoColor.FromArgb(255, 255, 0, 255),
AxoColor.FromArgb(255, 128, 0, 0),
AxoColor.FromArgb(255, 102, 205, 170),
AxoColor.FromArgb(255, 0, 0, 205),
AxoColor.FromArgb(255, 186, 85, 211),
AxoColor.FromArgb(255, 147, 112, 219),
AxoColor.FromArgb(255, 60, 179, 113),
AxoColor.FromArgb(255, 123, 104, 238),
AxoColor.FromArgb(255, 0, 250, 154),
AxoColor.FromArgb(255, 72, 209, 204),
AxoColor.FromArgb(255, 199, 21, 133),
AxoColor.FromArgb(255, 25, 25, 112),
AxoColor.FromArgb(255, 245, 255, 250),
AxoColor.FromArgb(255, 255, 228, 225),
AxoColor.FromArgb(255, 255, 228, 181),
AxoColor.FromArgb(255, 255, 222, 173),
AxoColor.FromArgb(255, 0, 0, 128),
AxoColor.FromArgb(255, 253, 245, 230),
AxoColor.FromArgb(255, 128, 128, 0),
AxoColor.FromArgb(255, 107, 142, 35),
AxoColor.FromArgb(255, 255, 165, 0),
AxoColor.FromArgb(255, 255, 69, 0),
AxoColor.FromArgb(255, 218, 112, 214),
AxoColor.FromArgb(255, 238, 232, 170),
AxoColor.FromArgb(255, 152, 251, 152),
AxoColor.FromArgb(255, 175, 238, 238),
AxoColor.FromArgb(255, 219, 112, 147),
AxoColor.FromArgb(255, 255, 239, 213),
AxoColor.FromArgb(255, 255, 218, 185),
AxoColor.FromArgb(255, 205, 133, 63),
AxoColor.FromArgb(255, 255, 192, 203),
AxoColor.FromArgb(255, 221, 160, 221),
AxoColor.FromArgb(255, 176, 224, 230),
AxoColor.FromArgb(255, 128, 0, 128),
AxoColor.FromArgb(255, 255, 0, 0),
AxoColor.FromArgb(255, 188, 143, 143),
AxoColor.FromArgb(255, 65, 105, 225),
AxoColor.FromArgb(255, 139, 69, 19),
AxoColor.FromArgb(255, 250, 128, 114),
AxoColor.FromArgb(255, 244, 164, 96),
AxoColor.FromArgb(255, 46, 139, 87),
AxoColor.FromArgb(255, 255, 245, 238),
AxoColor.FromArgb(255, 160, 82, 45),
AxoColor.FromArgb(255, 192, 192, 192),
AxoColor.FromArgb(255, 135, 206, 235),
AxoColor.FromArgb(255, 106, 90, 205),
AxoColor.FromArgb(255, 112, 128, 144),
AxoColor.FromArgb(255, 255, 250, 250),
AxoColor.FromArgb(255, 0, 255, 127),
AxoColor.FromArgb(255, 70, 130, 180),
AxoColor.FromArgb(255, 210, 180, 140),
AxoColor.FromArgb(255, 0, 128, 128),
AxoColor.FromArgb(255, 216, 191, 216),
AxoColor.FromArgb(255, 255, 99, 71),
AxoColor.FromArgb(0, 255, 255, 255),
AxoColor.FromArgb(255, 64, 224, 208),
AxoColor.FromArgb(255, 238, 130, 238),
AxoColor.FromArgb(255, 245, 222, 179),
AxoColor.FromArgb(255, 255, 255, 255),
AxoColor.FromArgb(255, 245, 245, 245),
AxoColor.FromArgb(255, 255, 255, 0),
AxoColor.FromArgb(255, 154, 205, 50),
};

    public static AxoColor AliceBlue { get { return _colors[0]; } }

    public static AxoColor AntiqueWhite { get { return _colors[1]; } }

    public static AxoColor Aqua { get { return _colors[2]; } }

    public static AxoColor Aquamarine { get { return _colors[3]; } }

    public static AxoColor Azure { get { return _colors[4]; } }

    public static AxoColor Beige { get { return _colors[5]; } }

    public static AxoColor Bisque { get { return _colors[6]; } }

    public static AxoColor Black { get { return _colors[7]; } }

    public static AxoColor BlanchedAlmond { get { return _colors[8]; } }

    public static AxoColor Blue { get { return _colors[9]; } }

    public static AxoColor BlueViolet { get { return _colors[10]; } }

    public static AxoColor Brown { get { return _colors[11]; } }

    public static AxoColor BurlyWood { get { return _colors[12]; } }

    public static AxoColor CadetBlue { get { return _colors[13]; } }

    public static AxoColor Chartreuse { get { return _colors[14]; } }

    public static AxoColor Chocolate { get { return _colors[15]; } }

    public static AxoColor Coral { get { return _colors[16]; } }

    public static AxoColor CornflowerBlue { get { return _colors[17]; } }

    public static AxoColor Cornsilk { get { return _colors[18]; } }

    public static AxoColor Crimson { get { return _colors[19]; } }

    public static AxoColor Cyan { get { return _colors[20]; } }

    public static AxoColor DarkBlue { get { return _colors[21]; } }

    public static AxoColor DarkCyan { get { return _colors[22]; } }

    public static AxoColor DarkGoldenrod { get { return _colors[23]; } }

    public static AxoColor DarkGray { get { return _colors[24]; } }

    public static AxoColor DarkGreen { get { return _colors[25]; } }

    public static AxoColor DarkKhaki { get { return _colors[26]; } }

    public static AxoColor DarkMagenta { get { return _colors[27]; } }

    public static AxoColor DarkOliveGreen { get { return _colors[28]; } }

    public static AxoColor DarkOrange { get { return _colors[29]; } }

    public static AxoColor DarkOrchid { get { return _colors[30]; } }

    public static AxoColor DarkRed { get { return _colors[31]; } }

    public static AxoColor DarkSalmon { get { return _colors[32]; } }

    public static AxoColor DarkSeaGreen { get { return _colors[33]; } }

    public static AxoColor DarkSlateBlue { get { return _colors[34]; } }

    public static AxoColor DarkSlateGray { get { return _colors[35]; } }

    public static AxoColor DarkTurquoise { get { return _colors[36]; } }

    public static AxoColor DarkViolet { get { return _colors[37]; } }

    public static AxoColor DeepPink { get { return _colors[38]; } }

    public static AxoColor DeepSkyBlue { get { return _colors[39]; } }

    public static AxoColor DimGray { get { return _colors[40]; } }

    public static AxoColor DodgerBlue { get { return _colors[41]; } }

    public static AxoColor Firebrick { get { return _colors[42]; } }

    public static AxoColor FloralWhite { get { return _colors[43]; } }

    public static AxoColor ForestGreen { get { return _colors[44]; } }

    public static AxoColor Fuchsia { get { return _colors[45]; } }

    public static AxoColor Gainsboro { get { return _colors[46]; } }

    public static AxoColor GhostWhite { get { return _colors[47]; } }

    public static AxoColor Gold { get { return _colors[48]; } }

    public static AxoColor Goldenrod { get { return _colors[49]; } }

    public static AxoColor Gray { get { return _colors[50]; } }

    public static AxoColor Green { get { return _colors[51]; } }

    public static AxoColor GreenYellow { get { return _colors[52]; } }

    public static AxoColor Honeydew { get { return _colors[53]; } }

    public static AxoColor HotPink { get { return _colors[54]; } }

    public static AxoColor IndianRed { get { return _colors[55]; } }

    public static AxoColor Indigo { get { return _colors[56]; } }

    public static AxoColor Ivory { get { return _colors[57]; } }

    public static AxoColor Khaki { get { return _colors[58]; } }

    public static AxoColor Lavender { get { return _colors[59]; } }

    public static AxoColor LavenderBlush { get { return _colors[60]; } }

    public static AxoColor LawnGreen { get { return _colors[61]; } }

    public static AxoColor LemonChiffon { get { return _colors[62]; } }

    public static AxoColor LightBlue { get { return _colors[63]; } }

    public static AxoColor LightCoral { get { return _colors[64]; } }

    public static AxoColor LightCyan { get { return _colors[65]; } }

    public static AxoColor LightGoldenrodYellow { get { return _colors[66]; } }

    public static AxoColor LightGray { get { return _colors[67]; } }

    public static AxoColor LightGreen { get { return _colors[68]; } }

    public static AxoColor LightPink { get { return _colors[69]; } }

    public static AxoColor LightSalmon { get { return _colors[70]; } }

    public static AxoColor LightSeaGreen { get { return _colors[71]; } }

    public static AxoColor LightSkyBlue { get { return _colors[72]; } }

    public static AxoColor LightSlateGray { get { return _colors[73]; } }

    public static AxoColor LightSteelBlue { get { return _colors[74]; } }

    public static AxoColor LightYellow { get { return _colors[75]; } }

    public static AxoColor Lime { get { return _colors[76]; } }

    public static AxoColor LimeGreen { get { return _colors[77]; } }

    public static AxoColor Linen { get { return _colors[78]; } }

    public static AxoColor Magenta { get { return _colors[79]; } }

    public static AxoColor Maroon { get { return _colors[80]; } }

    public static AxoColor MediumAquamarine { get { return _colors[81]; } }

    public static AxoColor MediumBlue { get { return _colors[82]; } }

    public static AxoColor MediumOrchid { get { return _colors[83]; } }

    public static AxoColor MediumPurple { get { return _colors[84]; } }

    public static AxoColor MediumSeaGreen { get { return _colors[85]; } }

    public static AxoColor MediumSlateBlue { get { return _colors[86]; } }

    public static AxoColor MediumSpringGreen { get { return _colors[87]; } }

    public static AxoColor MediumTurquoise { get { return _colors[88]; } }

    public static AxoColor MediumVioletRed { get { return _colors[89]; } }

    public static AxoColor MidnightBlue { get { return _colors[90]; } }

    public static AxoColor MintCream { get { return _colors[91]; } }

    public static AxoColor MistyRose { get { return _colors[92]; } }

    public static AxoColor Moccasin { get { return _colors[93]; } }

    public static AxoColor NavajoWhite { get { return _colors[94]; } }

    public static AxoColor Navy { get { return _colors[95]; } }

    public static AxoColor OldLace { get { return _colors[96]; } }

    public static AxoColor Olive { get { return _colors[97]; } }

    public static AxoColor OliveDrab { get { return _colors[98]; } }

    public static AxoColor Orange { get { return _colors[99]; } }

    public static AxoColor OrangeRed { get { return _colors[100]; } }

    public static AxoColor Orchid { get { return _colors[101]; } }

    public static AxoColor PaleGoldenrod { get { return _colors[102]; } }

    public static AxoColor PaleGreen { get { return _colors[103]; } }

    public static AxoColor PaleTurquoise { get { return _colors[104]; } }

    public static AxoColor PaleVioletRed { get { return _colors[105]; } }

    public static AxoColor PapayaWhip { get { return _colors[106]; } }

    public static AxoColor PeachPuff { get { return _colors[107]; } }

    public static AxoColor Peru { get { return _colors[108]; } }

    public static AxoColor Pink { get { return _colors[109]; } }

    public static AxoColor Plum { get { return _colors[110]; } }

    public static AxoColor PowderBlue { get { return _colors[111]; } }

    public static AxoColor Purple { get { return _colors[112]; } }

    public static AxoColor Red { get { return _colors[113]; } }

    public static AxoColor RosyBrown { get { return _colors[114]; } }

    public static AxoColor RoyalBlue { get { return _colors[115]; } }

    public static AxoColor SaddleBrown { get { return _colors[116]; } }

    public static AxoColor Salmon { get { return _colors[117]; } }

    public static AxoColor SandyBrown { get { return _colors[118]; } }

    public static AxoColor SeaGreen { get { return _colors[119]; } }

    public static AxoColor SeaShell { get { return _colors[120]; } }

    public static AxoColor Sienna { get { return _colors[121]; } }

    public static AxoColor Silver { get { return _colors[122]; } }

    public static AxoColor SkyBlue { get { return _colors[123]; } }

    public static AxoColor SlateBlue { get { return _colors[124]; } }

    public static AxoColor SlateGray { get { return _colors[125]; } }

    public static AxoColor Snow { get { return _colors[126]; } }

    public static AxoColor SpringGreen { get { return _colors[127]; } }

    public static AxoColor SteelBlue { get { return _colors[128]; } }

    public static AxoColor Tan { get { return _colors[129]; } }

    public static AxoColor Teal { get { return _colors[130]; } }

    public static AxoColor Thistle { get { return _colors[131]; } }

    public static AxoColor Tomato { get { return _colors[132]; } }

    public static AxoColor Transparent { get { return _colors[133]; } }

    public static AxoColor Turquoise { get { return _colors[134]; } }

    public static AxoColor Violet { get { return _colors[135]; } }

    public static AxoColor Wheat { get { return _colors[136]; } }

    public static AxoColor White { get { return _colors[137]; } }

    public static AxoColor WhiteSmoke { get { return _colors[138]; } }

    public static AxoColor Yellow { get { return _colors[139]; } }

    public static AxoColor YellowGreen { get { return _colors[140]; } }

    #endregion Generated code
  }
}
