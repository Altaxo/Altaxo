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
  /// Options for a rational cubic spline (<see cref="RationalCubicSpline"/>).
  /// </summary>
  public record RationalCubicSplineOptions : IInterpolationFunctionOptions
  {
    private double _smoothing;

    /// <summary>
    /// Gets the value of the smoothing parameter.
    /// A value of <c>0</c> results in a standard cubic spline.
    /// A value of <c>p</c> with <c>-1 &lt; p &lt; 0</c> results in "unsmoothing", i.e. overshooting oscillations.
    /// A value of <c>p &gt; 0</c> gives increasing smoothness.
    /// <c>p</c> to infinity results in a linear interpolation.
    /// A value smaller than or equal to <c>-1.0</c> leads to an exception.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than -1.</exception>
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
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>
    /// 2022-08-14 Initial version.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RationalCubicSplineOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RationalCubicSplineOptions)obj;
        info.AddValue("Smoothing", s.Smoothing);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var smoothing = info.GetDouble("Smoothing");
        return new RationalCubicSplineOptions() { Smoothing = smoothing };
      }
    }

    #endregion

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var spline = new RationalCubicSpline() { Smoothing = Smoothing };
      spline.Interpolate(xvec, yvec);
      return spline;
    }

    /// <inheritdoc/>
    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
