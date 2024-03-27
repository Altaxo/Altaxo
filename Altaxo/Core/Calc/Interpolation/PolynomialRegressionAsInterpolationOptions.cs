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
  /// Options for a polynomial regression used as interpolation method (<see cref=PolynomialRegressionAsInterpolation"/>).
  /// </summary>
  public record PolynomialRegressionAsInterpolationOptions : IInterpolationFunctionOptions
  {
    private int _order = 2;


    #region Serialization

    /// <summary>
    /// 2022-08-14 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialRegressionAsInterpolationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PolynomialRegressionAsInterpolationOptions)obj;
        info.AddValue("Order", s._order);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order = info.GetInt32("Order");
        return new PolynomialRegressionAsInterpolationOptions() { Order = order };
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PolynomialRegressionAsInterpolationOptions"/> class with Order=2.
    /// </summary>
    public PolynomialRegressionAsInterpolationOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolynomialRegressionAsInterpolationOptions"/> class with an order given by the argument.
    /// </summary>
    /// <param name="order">The order.</param>
    public PolynomialRegressionAsInterpolationOptions(int order)
    {
      Order = order;
    }

    public int Order
    {
      get => _order;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(Order));
        _order = value;
      }
    }



    /// <inheritdoc/>
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var spline = new PolynomialRegressionAsInterpolation() { RegressionOrder = Order };
      spline.Interpolate(xvec, yvec);
      return spline;
    }

    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }
  }
}
