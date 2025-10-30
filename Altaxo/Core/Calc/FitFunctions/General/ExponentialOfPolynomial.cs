#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Exponential transformation of a polynomial, with both terms with positive exponent and negative exponent, plus an offset.
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialOfPolynomial}")]
  public class ExponentialOfPolynomial : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>The order of the polynomial with positive exponents.</summary>
    private readonly int _order_n;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _order_m;

    #region Serialization


    /// <summary>
    /// V0: 2025-10-30 
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialOfPolynomial), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialOfPolynomial)obj;
        info.AddValue("OrderPositive", s._order_n);
        info.AddValue("OrderNegative", s._order_m);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_n = info.GetInt32("OrderPositive");
        var order_m = info.GetInt32("OrderNegative");
        return new ExponentialOfPolynomial(order_n, order_m);
      }
    }


    #endregion Serialization

    public ExponentialOfPolynomial()
    {
      _order_n = 2;
      _order_m = 0;
    }

    public ExponentialOfPolynomial(int polynomialOrder_PositiveExponents, int polynomialOrder_NegativeExponents)
    {
      _order_n = polynomialOrder_PositiveExponents;
      _order_m = polynomialOrder_NegativeExponents;

      if (_order_n < 0)
        throw new ArgumentOutOfRangeException("Order for positive exponents has to be greater than or equal to zero");
      if (_order_m < 0)
        throw new ArgumentOutOfRangeException("Order for negative exponents has to be greater than or equal to zero");

    }

    [FitFunctionCreator("ExponentialOfPolynomial", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialOfPolynomial}")]
    public static IFitFunction CreatePolynomial_1_0()
    {
      return new ExponentialOfPolynomial(1, 0);
    }

    public int PolynomialOrder_PositiveExponents => _order_n;

    /// <summary>
    /// Creates a new instance with the provided order for the positive exponents.
    /// </summary>
    /// <param name="polynomialOrder_PositiveExponents">The order for the positive exponents (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public ExponentialOfPolynomial WithPolynomialOrder_PositiveExponents(int polynomialOrder_PositiveExponents)
    {
      if (!(polynomialOrder_PositiveExponents >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_PositiveExponents)} must be greater than or equal to 0");

      if (!(_order_n == polynomialOrder_PositiveExponents))
      {
        return new ExponentialOfPolynomial(polynomialOrder_PositiveExponents, _order_m);
      }
      else
      {
        return this;
      }
    }

    public int PolynomialOrder_NegativeExponents => _order_m;
    /// <summary>
    /// Creates a new instance with the provided order for the positive exponents.
    /// </summary>
    /// <param name="polynomialOrder_NegativeExponents">The order for the positive exponents (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public ExponentialOfPolynomial WithPolynomialOrder_NegativeExponents(int polynomialOrder_NegativeExponents)
    {
      if (!(polynomialOrder_NegativeExponents >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_NegativeExponents)} must be greater than or equal to 0");

      if (!(_order_m == polynomialOrder_NegativeExponents))
      {
        return new ExponentialOfPolynomial(_order_n, polynomialOrder_NegativeExponents);
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
        return _order_n + _order_m + 2;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      if (i < 0 || i >= NumberOfParameters)
        throw new ArgumentOutOfRangeException($"Parameter index {i} is out of range [0, {NumberOfParameters - 1}]");

      return i == 0 ? "offset" : i <= _order_n + 1 ? FormattableString.Invariant($"a{i - 1}") : FormattableString.Invariant($"b{i - _order_n - 1}");
    }

    public double DefaultParameterValue(int i)
    {
      if (i < 0 || i >= NumberOfParameters)
        throw new ArgumentOutOfRangeException($"Parameter index {i} is out of range [0, {NumberOfParameters - 1}]");

      return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of terms x^0 .. x^n
      double sum = P[_order_n + 1];
      for (int i = _order_n; i > 0; i--)
      {
        sum *= X[0];
        sum += P[i];
      }

      if (_order_m > 0)
      {
        // evaluation of terms x^-1 .. x^-m
        double isum = 0;
        for (int i = _order_n + _order_m + 1; i > _order_n + 1; i--)
        {
          isum += P[i];
          if (!(isum == 0)) // avoid NaN if x=0 and coefficients are fixed to zero
          {
            isum /= X[0];
          }
        }
        Y[0] = P[0] + Math.Exp(sum + isum);
      }
      else
      {
        Y[0] = P[0] + Math.Exp(sum);
      }
    }


    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        // evaluation of terms x^0 .. x^n
        double sum = P[_order_n + 1];
        for (int i = _order_n; i > 0; i--)
        {
          sum *= x;
          sum += P[i];
        }

        if (_order_m > 0)
        {
          // evaluation of terms x^-1 .. x^-m
          double isum = 0;
          for (int i = _order_n + _order_m + 1; i > _order_n + 1; i--)
          {
            isum += P[i];
            if (!(isum == 0)) // avoid NaN if x=0 and coefficients are fixed to zero
            {
              isum /= x;
            }
          }
          FV[r] = P[0] + Math.Exp(sum + isum);
        }
        else
        {
          FV[r] = P[0] + Math.Exp(sum);
        }
      }
    }


    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        double y;
        var x = X[r, 0];

        // evaluation of terms x^0 .. x^n
        double sum = P[_order_n + 1];
        for (int i = _order_n; i > 0; i--)
        {
          sum *= x;
          sum += P[i];
        }

        if (_order_m > 0)
        {
          // evaluation of terms x^-1 .. x^-m
          double isum = 0;
          for (int i = _order_n + _order_m + 1; i > _order_n + 1; i--)
          {
            isum += P[i];
            if (!(isum == 0)) // avoid NaN if x=0 and coefficients are fixed to zero
            {
              isum /= x;
            }
          }
          y = P[0] + Math.Exp(sum + isum);
        }
        else
        {
          y = P[0] + Math.Exp(sum);
        }

        DY[r, 0] = 1; // derivative with respect to offset
        DY[r, 1] = y; // derivative with respect to a0
        double xn = y;
        for (int i = 1; i <= _order_n; i++)
        {
          xn *= x;
          DY[r, 1 + i] = xn;
        }

        if (_order_m > 0)
        {
          double xm = y;
          for (int i = 1; i <= _order_m; ++i)
          {
            xm /= x;
            DY[r, 1 + _order_n + i] = xm;
          }
        }
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

  }
}
