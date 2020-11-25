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
  /// This color provider provides the colors of the visible light spectrum in the wavelength
  /// range between 350 nm and 780 nm.
  /// </summary>
  public class VisibleLightSpectrum : ColorProviderBase
  {
    /// <summary>Minimum wavelength of the light in nm which can be shown as color.</summary>
    public static readonly int MinVisibleWavelength_nm = 350;

    /// <summary>Maximum wavelength of the light in nm which can be shown as color.</summary>
    public static readonly int MaxVisibleWavelength_nm = 780;

    /// <summary>Default gamma value.</summary>
    public static readonly double DefaultGamma = 1;

    /// <summary>Default brightness value.</summary>
    public static readonly double DefaultBrightness = 1;

    /// <summary>Brightness value (0..1)</summary>
    private double _brightness = 1;

    /// <summary>Gamma value for colorization.</summary>
    private double _gamma = 1;

    private const double maxColorComponent = 255.999;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VisibleLightSpectrum), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VisibleLightSpectrum)obj;
        info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));
        info.AddValue("Gamma", s._gamma);
        info.AddValue("Brightness", s._brightness);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (VisibleLightSpectrum?)o ?? new VisibleLightSpectrum();
        info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);
        s._gamma = info.GetDouble("Gramma");
        s._brightness = info.GetDouble("Brightness");
        return s;
      }
    }

    #endregion Serialization

    public override bool Equals(IColorProvider? other)
    {
      if (!base.Equals(other))
        return false;

      var from = (VisibleLightSpectrum)other;

      return
        _gamma == from._gamma &&
        _brightness == from._brightness;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + (_gamma + _brightness).GetHashCode() * 13;
    }

    /// <summary>
    /// Default constructor. The maximum intensity and the gamma value are set to their default values.
    /// </summary>
    public VisibleLightSpectrum()
    {
      _gamma = DefaultGamma;
      _brightness = DefaultBrightness;
    }

    public double Gamma
    {
      get
      {
        return _gamma;
      }
    }

    public VisibleLightSpectrum WithGamma(double gamma)
    {
      if (!(gamma >= 0))
        throw new ArgumentOutOfRangeException(nameof(gamma), "Argument should be >=0");

      if (gamma == _gamma)
      {
        return this;
      }
      else
      {
        var result = (VisibleLightSpectrum)MemberwiseClone();
        result._gamma = gamma;
        return result;
      }
    }

    public double Brightness
    {
      get
      {
        return _brightness;
      }
    }

    public VisibleLightSpectrum WithBrightness(double brightness)
    {
      if (!((brightness >= 0) && (brightness <= 1)))
        throw new ArgumentOutOfRangeException(nameof(brightness), "Argument should be >=0 and <=1");

      if (brightness == _brightness)
      {
        return this;
      }
      else
      {
        var result = (VisibleLightSpectrum)MemberwiseClone();
        result._brightness = brightness;
        return result;
      }
    }

    /// <summary>
    /// Gets the color in dependence of the wavelength with <see cref="DefaultGamma"/> value, <see cref="DefaultBrightness"/> and no transparency.
    /// </summary>
    /// <param name="Wavelength">Wavelength in nm (ranging from 350 to 780).</param>
    /// <returns>Color in dependence of the provided wavelength value.</returns>
    public static Color GetColorFromWaveLength(double Wavelength)
    {
      return GetColorFromWaveLength(Wavelength, DefaultGamma, DefaultBrightness, 255);
    }

    /// <summary>
    /// Gets the color in dependence of the wavelength, gamma, intensity max and transparency.
    /// </summary>
    /// <param name="Wavelength">Wavelength in nm (ranging from 350 to 780).</param>
    /// <param name="Gamma">Gamma value (positive).</param>
    /// <param name="brightness">Maximum brightness value (0..1).</param>
    /// <param name="alphaChannel">Value of the alpha channel (0.255), corresponding to full transparent (0) to full opaque (255).</param>
    /// <returns>The color in dependence of the provided arguments.</returns>
    public static Color GetColorFromWaveLength(double Wavelength, double Gamma, double brightness, int alphaChannel)
    {
      double Blue;
      double Green;
      double Red;
      double Factor;

      if (Wavelength >= 350 && Wavelength < 440)
      {
        Red = -(Wavelength - 440d) / (440d - 350d);
        Green = 0.0;
        Blue = 1.0;
      }
      else if (Wavelength >= 440 && Wavelength < 490)
      {
        Red = 0.0;
        Green = (Wavelength - 440d) / (490d - 440d);
        Blue = 1.0;
      }
      else if (Wavelength >= 490 && Wavelength < 510)
      {
        Red = 0.0;
        Green = 1.0;
        Blue = -(Wavelength - 510d) / (510d - 490d);
      }
      else if (Wavelength >= 510 && Wavelength < 580)
      {
        Red = (Wavelength - 510d) / (580d - 510d);
        Green = 1.0;
        Blue = 0.0;
      }
      else if (Wavelength >= 580 && Wavelength < 645)
      {
        Red = 1.0;
        Green = -(Wavelength - 645d) / (645d - 580d);
        Blue = 0.0;
      }
      else if (Wavelength >= 645 && Wavelength <= 780)
      {
        Red = 1.0;
        Green = 0.0;
        Blue = 0.0;
      }
      else
      {
        Red = 0.0;
        Green = 0.0;
        Blue = 0.0;
      }

      if (Wavelength >= 350 && Wavelength < 420)
      {
        Factor = 0.3 + 0.7 * (Wavelength - 350d) / (420d - 350d);
      }
      else if (Wavelength >= 420 && Wavelength < 700)
      {
        Factor = 1.0;
      }
      else if (Wavelength >= 700 && Wavelength <= 780)
      {
        Factor = 0.3 + 0.7 * (780d - Wavelength) / (780d - 700d);
      }
      else
      {
        Factor = 0.0;
      }

      int R = AdjustFactor(Red, Factor, brightness, Gamma);
      int G = AdjustFactor(Green, Factor, brightness, Gamma);
      int B = AdjustFactor(Blue, Factor, brightness, Gamma);

      return Color.FromArgb(alphaChannel, R, G, B);
    }

    private static int AdjustFactor(double Color,
     double Factor,
     double brightness,
     double Gamma)
    {
      if (Color == 0.0)
      {
        return 0;
      }
      else
      {
        return (int)Math.Round(255 * brightness * Math.Pow(Color * Factor, Gamma));
      }
    }

    /// <summary>
    /// Gets the color in dependence of a relative value ranging from 0..1. The instance members
    /// for gamma, intensity maximum and transparency are used to calculate the color.
    /// </summary>
    /// <param name="relVal">Relative value (0..1), which is transformed linearly to the wavelength range of 350..780 nm.</param>
    /// <returns>Color in dependence of the relative value.</returns>
    protected override System.Drawing.Color GetColorFrom0To1Continuously(double relVal)
    {
      return GetColorFromWaveLength(
        MinVisibleWavelength_nm + relVal * (MaxVisibleWavelength_nm - MinVisibleWavelength_nm),
        _gamma,
        _brightness,
        _alphaChannel);
    }
  }
}
