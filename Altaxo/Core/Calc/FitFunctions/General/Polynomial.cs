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
  /// A polynomial, with both terms with positive exponent and negative exponent.
  /// This is for completeness only, fitting should be done with a linear fit instead.
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.Polynomial}")]
  public class Polynomial
        : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>The order of the polynomial with positive exponents.</summary>
    private readonly int _order_n;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _order_m;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.General.PolynomialFit", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Polynomial)obj;
        info.AddValue("Order", s._order_n);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_n = info.GetInt32("Order");
        return new Polynomial(order_n, 0);
      }
    }

    /// <summary>
    /// V1: 2021-05-05 add terms with negative exponent.
    /// V2: 2022-10-14 Move to assembly AltaxoCore instead of AltaxoBase
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.General.Polynomial", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Polynomial), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Polynomial)obj;
        info.AddValue("OrderPositive", s._order_n);
        info.AddValue("OrderNegative", s._order_m);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_n = info.GetInt32("OrderPositive");
        var order_m = info.GetInt32("OrderNegative");
        return new Polynomial(order_n, order_m);
      }
    }


    #endregion Serialization



    public Polynomial()
    {
      _order_n = 2;
      _order_m = 0;
    }

    public Polynomial(int polynomialOrder_PositiveExponents, int polynomialOrder_NegativeExponents)
    {
      _order_n = polynomialOrder_PositiveExponents;
      _order_m = polynomialOrder_NegativeExponents;

      if (_order_n < 0)
        throw new ArgumentOutOfRangeException("Order for positive exponents has to be greater than or equal to zero");
      if (_order_m < 0)
        throw new ArgumentOutOfRangeException("Order for negative exponents has to be greater than or equal to zero");

    }

    [FitFunctionCreator("Polynomial", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.Polynomial}")]
    public static IFitFunction CreatePolynomial_2_0()
    {
      return new Polynomial(2, 0);
    }

    public int PolynomialOrder_PositiveExponents => _order_n;

    /// <summary>
    /// Creates a new instance with the provided order for the positive exponents.
    /// </summary>
    /// <param name="polynomialOrder_PositiveExponents">The order for the positive exponents (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public Polynomial WithPolynomialOrder_PositiveExponents(int polynomialOrder_PositiveExponents)
    {
      if (!(polynomialOrder_PositiveExponents >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_PositiveExponents)} must be greater than or equal to 0");

      if (!(_order_n == polynomialOrder_PositiveExponents))
      {
        return new Polynomial(polynomialOrder_PositiveExponents, _order_m);
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
    public Polynomial WithPolynomialOrder_NegativeExponents(int polynomialOrder_NegativeExponents)
    {
      if (!(polynomialOrder_NegativeExponents >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_NegativeExponents)} must be greater than or equal to 0");

      if (!(_order_m == polynomialOrder_NegativeExponents))
      {
        return new Polynomial(_order_n, polynomialOrder_NegativeExponents);
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
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return i <= _order_n ? FormattableString.Invariant($"a{i}") : FormattableString.Invariant($"b{i - _order_n}");
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
      // evaluation of terms x^0 .. x^n
      double sum = P[_order_n];
      for (int i = _order_n - 1; i >= 0; i--)
      {
        sum *= X[0];
        sum += P[i];
      }

      if (_order_m > 0)
      {
        // evaluation of terms x^-1 .. x^-m
        double isum = 0;
        for (int i = _order_n + _order_m; i > _order_n; i--)
        {
          isum += P[i];
          if (!(isum == 0)) // avoid NaN if x=0 and coefficients are fixed to zero
          {
            isum /= X[0];
          }
        }
        Y[0] = sum + isum;
      }
      else
      {
        Y[0] = sum;
      }
    }


    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        // evaluation of terms x^0 .. x^n
        double sum = P[_order_n];
        for (int i = _order_n - 1; i >= 0; i--)
        {
          sum *= x;
          sum += P[i];
        }

        if (_order_m > 0)
        {
          // evaluation of terms x^-1 .. x^-m
          double isum = 0;
          for (int i = _order_n + _order_m; i > _order_n; i--)
          {
            isum += P[i];
            if (!(isum == 0)) // avoid NaN if x=0 and coefficients are fixed to zero
            {
              isum /= x;
            }
          }
          FV[r] = sum + isum;
        }
        else
        {
          FV[r] = sum;
        }
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
        DY[r, 0] = 1;
        double xn = 1;
        for (int i = 1; i <= _order_n; i++)
        {
          xn *= x;
          DY[r, i] = xn;
        }

        if (_order_m > 0)
        {
          double xm = 1;
          for (int i = 1; i <= _order_m; ++i)
          {
            xm /= x;
            DY[r, i + _order_n] = xm;
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
