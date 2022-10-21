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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Quotient of two polynomials, with coefficients a1..an (nominator) and b0..bm (denominator). The coefficent a0 of the nominator polynom is fixed to 1.
  /// </summary>
  [FitFunctionClass]
  public class RationalInverse
        : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>The order of the polynomial in the nominator.</summary>
    private readonly int _order_n;
    /// <summary>The order of the polynomial in the denominator.</summary>
    private readonly int _order_m;

    #region Serialization

    /// <summary>
    /// 2021-05-09 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RationalInverse), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RationalInverse)obj;
        info.AddValue("OrderNominator", s._order_n);
        info.AddValue("OrderDenominator", s._order_m);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_n = info.GetInt32("OrderNominator");
        var order_m = info.GetInt32("OrderDenominator");
        return new RationalInverse(order_n, order_m);
      }
    }


    #endregion Serialization



    public RationalInverse()
    {
      _order_n = 1;
      _order_m = 1;
    }

    public RationalInverse(int polynomialOrder_Nominator, int polynomialOrder_Denominator)
    {
      _order_n = polynomialOrder_Nominator;
      _order_m = polynomialOrder_Denominator;

      if (_order_n < 0)
        throw new ArgumentOutOfRangeException("Order for nominator polynom has to be greater than or equal to zero");
      if (_order_m < 0)
        throw new ArgumentOutOfRangeException("Order for denominator polynom has to be greater than or equal to zero");

    }

    [FitFunctionCreator("RationalInverse", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.RationalInverse}")]
    public static IFitFunction CreateRationalInverse_1_1()
    {
      return new RationalInverse(1, 1);
    }

    public int PolynomialOrder_Nominator => _order_n;

    /// <summary>
    /// Creates a new instance with the provided order for the nominator polynom.
    /// </summary>
    /// <param name="polynomialOrder_Nominator">The order for the positive exponents (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public RationalInverse WithPolynomialOrder_Nominator(int polynomialOrder_Nominator)
    {
      if (!(polynomialOrder_Nominator >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_Nominator)} must be greater than or equal to 0");

      if (!(_order_n == polynomialOrder_Nominator))
      {
        return new RationalInverse(polynomialOrder_Nominator, _order_m);
      }
      else
      {
        return this;
      }
    }

    public int PolynomialOrder_Denominator => _order_m;
    /// <summary>
    /// Creates a new instance with the provided order for the positive exponents.
    /// </summary>
    /// <param name="polynomialOrder_Denominator">The order for the positive exponents (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public RationalInverse WithPolynomialOrder_Denominator(int polynomialOrder_Denominator)
    {
      if (!(polynomialOrder_Denominator >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_Denominator)} must be greater than or equal to 0");

      if (!(_order_m == polynomialOrder_Denominator))
      {
        return new RationalInverse(_order_n, polynomialOrder_Denominator);
      }
      else
      {
        return this;
      }
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return _order_n + _order_m + 1;
      }
    }

    public string IndependentVariableName(int i)
    {
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return i < _order_n ? FormattableString.Invariant($"a{i + 1}") : FormattableString.Invariant($"b{i - _order_n}");
    }

    public double DefaultParameterValue(int i)
    {
      return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of nominator terms a0*x^0 .. an*x^n
      double nominator;
      if (_order_n == 0)
      {
        nominator = 1;
      }
      else
      {
        nominator = P[_order_n - 1];
        for (int i = _order_n - 2; i >= 0; i--)
        {
          nominator *= X[0];
          nominator += P[i];
        }
        nominator *= X[0];
        nominator += 1;
      }

      // evaluation of denominator terms 1 + .. + bm*x^m
      double denominator = P[_order_n + _order_m];
      for (int i = _order_n + _order_m - 1; i >= _order_n; i--)
      {
        denominator *= X[0];
        denominator += P[i];
      }

      Y[0] = nominator / denominator;
    }
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        // evaluation of nominator terms a0*x^0 .. an*x^n
        double nominator;
        if (_order_n == 0)
        {
          nominator = 1;
        }
        else
        {
          nominator = P[_order_n - 1];
          for (int i = _order_n - 2; i >= 0; i--)
          {
            nominator *= x;
            nominator += P[i];
          }
          nominator *= x;
          nominator += 1;
        }

        // evaluation of denominator terms 1 + .. + bm*x^m
        double denominator = P[_order_n + _order_m];
        for (int i = _order_n + _order_m - 1; i >= _order_n; i--)
        {
          denominator *= x;
          denominator += P[i];
        }

        FV[r] = nominator / denominator;
      }
    }


    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool> dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];
        // evaluation of nominator terms a0*x^0 .. an*x^n
        double nominator;
        if (_order_n == 0)
        {
          nominator = 1;
        }
        else
        {
          nominator = P[_order_n - 1];
          for (int i = _order_n - 2; i >= 0; i--)
          {
            nominator *= x;
            nominator += P[i];
          }
          nominator *= x;
          nominator += 1;
        }

        // evaluation of denominator terms 1 + .. + bm*x^m
        double denominator = P[_order_n + _order_m];
        for (int i = _order_n + _order_m - 1; i >= _order_n; i--)
        {
          denominator *= x;
          denominator += P[i];
        }


        var nomByDenomSqr = nominator / (denominator * denominator);

        double xn = 1;
        for (int i = 0; i < _order_n; ++i)
        {
          xn *= x;
          DY[r, i] = xn / denominator;
        }

        xn = 1;
        for (int i = 0; i <= _order_m; ++i)
        {
          DY[r, i + _order_n] = -xn * nomByDenomSqr;
          xn *= x;
        }
      }
    }
  }
}
