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
  /// Options for a rational interpolation (<see cref="RationalInterpolation"/>).
  /// </summary>
  public record RationalInterpolationOptions : IInterpolationFunctionOptions
  {
    private int _numeratorDegree = 2;
    private double _precision = CurveBase.DBL_EPSILON;


    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    /// <remarks>
    /// 2022-08-14 Initial version.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RationalInterpolationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RationalInterpolationOptions)obj;
        info.AddValue("NumeratorDegree", s._numeratorDegree);
        info.AddValue("Precision", s._precision);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numeratorDegree = info.GetInt32("NumeratorDegree");
        var precision = info.GetDouble("Precision");
        return new RationalInterpolationOptions() { _numeratorDegree = numeratorDegree, _precision = precision };
      }
    }

    #endregion

    /// <summary>
    /// Gets the degree of the numerator polynomial used for the rational interpolation.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
    public int NumeratorDegree
    {
      get => _numeratorDegree;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(NumeratorDegree));
        _numeratorDegree = value;
      }
    }

    /// <summary>
    /// Gets the precision parameter used by the interpolation algorithm.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than or equal to zero.</exception>
    public double Precision
    {
      get => _precision;
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException(nameof(Precision));
        _precision = value;
      }
    }

    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var spline = new RationalInterpolation() { NumeratorDegree = NumeratorDegree, Precision = Precision };
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
