#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  /// <summary>
  /// Peak enhancement by evaluating the 2nd derivative of a spectrum. This is done by modelling the spectrum by a smoothing spline,
  /// and then use the 2nd derivative of the spline.
  /// </summary>
  public record PeakEnhancement2ndDerivativeOfSmoothingSpline : IPeakEnhancement
  {
    public double SmoothnessDefaultValue = 1;

    private double? _smoothness = null;

    /// <summary>
    /// Get/sets the smoothness parameter.
    /// If null, this parameter is set automatically.
    /// If not null, it must be in the interval [0,Infinity], where a
    /// value of 0 means no smoothing (evaluation of a cubic spline), while a value of Infinity
    /// means evaluation of a regression.
    /// </summary>
    public double? Smoothness
    {
      get
      {
        return _smoothness;
      }
      init
      {
        if (value is not null && !(0 <= value.Value))
          throw new ArgumentOutOfRangeException("Must be a value >=0", nameof(value));
        _smoothness = value;
      }
    }

    #region Serialization

    /// <summary>
    /// 2023-03-16 V0 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakEnhancement2ndDerivativeOfSmoothingSpline), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakEnhancement2ndDerivativeOfSmoothingSpline)obj;
        info.AddValue("Smoothness", s.Smoothness);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var smoothness = info.GetNullableDouble("Smoothness");

        return o is null ? new PeakEnhancement2ndDerivativeOfSmoothingSpline
        {
          Smoothness = smoothness,
        } :
          ((PeakEnhancement2ndDerivativeOfSmoothingSpline)o) with
          {
            Smoothness = smoothness,
          };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      const int resolutionMagnification = 4; // the spectral resolution is enhanced by 4
      const double DerivativeIntervalScale = 1 / (double)resolutionMagnification * 16; // the 2nd derivative is evaluated by finite difference from the 1st derivative of the spline

      var s = new Altaxo.Calc.Interpolation.SmoothingCubicSpline() { Smoothness = Smoothness ?? SmoothnessDefaultValue };
      s.Interpolate(x, y);

      int numberOfPoints = (x.Length - 1) * resolutionMagnification + 1;
      var xx = new double[numberOfPoints];
      var yy = new double[numberOfPoints];

      double dx = 0;
      for (int i = 0; i < numberOfPoints; ++i)
      {
        int p = i / resolutionMagnification;
        int q = i % resolutionMagnification;

        if (q == 0)
        {
          dx = p == x.Length - 1 ? x[p] - x[p - 1] : x[p + 1] - x[p];
          dx *= DerivativeIntervalScale;
        }

        double t = q / (double)resolutionMagnification;
        double xm = q == 0 ? x[p] : x[p] * (1 - t) + x[p + 1] * (t);
        double ysd = (s.GetY1stDerivativeOfX(xm - dx) - s.GetY1stDerivativeOfX(xm + dx)) / (2 * dx);
        xx[i] = xm;
        yy[i] = ysd;
      }
      return (xx, yy, null);
    }
  }
}
