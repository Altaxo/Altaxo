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

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Options for an exponential spline (<see cref=ExponentialSpline"/>).
  /// </summary>
  public record ExponentialSplineOptions : IInterpolationFunctionOptions
  {
    private double _smoothing;

    /// <summary>
    /// Set the value of the smoothing paramenter. A value of 0
    /// for the smoothing parameter results in a standard cubic spline.
    /// A value of p with -1 &lt; p &lt; 0 results in "unsmoothing" that means
    /// overshooting oscillations. A value of p with p &gt; 0 gives increasing
    /// smoothness. p to infinity results in a linear interpolation. A value
    /// smaller or equal to -1.0 leads to an exception.
    /// </summary>
    public double Smoothing
    {
      get
      {
        return _smoothing;
      }
      init
      {
        if (!(value >= -1))
          throw new ArgumentOutOfRangeException("The value must be >= -1", nameof(Smoothing));

        _smoothing = value;
      }
    }

    #region Serialization

    /// <summary>
    /// 2022-08-14 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialSplineOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialSplineOptions)obj;
        info.AddValue("Smoothing", s.Smoothing);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var smoothing = info.GetDouble("Smoothing");
        return new ExponentialSplineOptions() { Smoothing = smoothing };
      }
    }

    #endregion

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var spline = new ExponentialSpline() { Smoothing = Smoothing };
      spline.Interpolate(xvec, yvec);
      return spline;
    }

    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
