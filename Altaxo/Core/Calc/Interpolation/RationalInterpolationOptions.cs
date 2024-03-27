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
  /// Options for a rational interpolation (<see cref=RationalInterpolation"/>).
  /// </summary>
  public record RationalInterpolationOptions : IInterpolationFunctionOptions
  {
    private int _numeratorDegree = 2;
    private double _precision = CurveBase.DBL_EPSILON;


    #region Serialization

    /// <summary>
    /// 2022-08-14 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RationalInterpolationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RationalInterpolationOptions)obj;
        info.AddValue("NumeratorDegree", s._numeratorDegree);
        info.AddValue("Precision", s._precision);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numeratorDegree = info.GetInt32("NumeratorDegree");
        var precision = info.GetDouble("Precision");
        return new RationalInterpolationOptions() { _numeratorDegree = numeratorDegree, _precision = precision };
      }
    }

    #endregion

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

    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
