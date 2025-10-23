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
  /// This fit function describes two polynomial segments which are connected at a center point (xc, y0).
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.TwoPolynomialSegments}")]
  public class TwoPolynomialSegments
        : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>The order of the polynomial on the left segment.</summary>
    private readonly int _order_n;
    /// <summary>The order of the polynomial on the right segment.</summary>
    private readonly int _order_m;

    #region Serialization

    /// <summary>
    /// V0: 2025-10-23 initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TwoPolynomialSegments), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TwoPolynomialSegments)obj;
        info.AddValue("OrderLeftSide", s._order_n);
        info.AddValue("OrderRightSide", s._order_m);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_n = info.GetInt32("OrderLeftSide");
        var order_m = info.GetInt32("OrderRightSide");
        return new TwoPolynomialSegments(order_n, order_m);
      }
    }


    #endregion Serialization



    public TwoPolynomialSegments()
    {
      _order_n = 1;
      _order_m = 1;
    }

    public TwoPolynomialSegments(int polynomialOrder_LeftSegment, int polynomialOrder_RightSegment)
    {
      _order_n = polynomialOrder_LeftSegment;
      _order_m = polynomialOrder_RightSegment;

      if (_order_n < 0)
        throw new ArgumentOutOfRangeException("Order for left segment has to be greater than or equal to zero");
      if (_order_m < 0)
        throw new ArgumentOutOfRangeException("Order for right segment has to be greater than or equal to zero");
      if (_order_n == 0 && _order_m == 0)
        throw new ArgumentOutOfRangeException("Order for either the left segment or the right segment has to be greater than zero.");
    }

    [FitFunctionCreator("Two polynomial segments", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.TwoPolynomialSegment}")]
    public static IFitFunction CreateTwoPolynomialSegments_1_1()
    {
      return new TwoPolynomialSegments(1, 1);
    }

    /// <summary>
    /// Gets the order of the left polynomial segment.
    /// </summary>
    public int PolynomialOrder_LeftSegment => _order_n;

    /// <summary>
    /// Creates a new instance with the provided order for the left polynomial segment.
    /// </summary>
    /// <param name="polynomialOrder_LeftSegment">The order for the left polynomial segment (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public TwoPolynomialSegments WithPolynomialOrder_LeftSegment(int polynomialOrder_LeftSegment)
    {
      if (!(polynomialOrder_LeftSegment >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_LeftSegment)} must be greater than or equal to 0");

      if (!(_order_n == polynomialOrder_LeftSegment))
      {
        return new TwoPolynomialSegments(polynomialOrder_LeftSegment, _order_m);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the polynomial order of the right segment.
    /// </summary>
    public int PolynomialOrder_RightSegment => _order_m;

    /// <summary>
    /// Creates a new instance with the provided order for the right polynomial segment.
    /// </summary>
    /// <param name="polynomialOrder_RightSegment">The order for the right polynomial segment (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public TwoPolynomialSegments WithPolynomialOrder_RightSegment(int polynomialOrder_RightSegment)
    {
      if (!(polynomialOrder_RightSegment >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrder_RightSegment)} must be greater than or equal to 0");

      if (!(_order_m == polynomialOrder_RightSegment))
      {
        return new TwoPolynomialSegments(_order_n, polynomialOrder_RightSegment);
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
        return 2 + _order_n + _order_m;
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
      if (i == 0)
        return "xc";
      else if (i == 1)
        return "y0";
      else if (i > 1 && i < 2 + _order_n)
        return FormattableString.Invariant($"a{i - 1}");
      else if (i > 1 && i < 2 + _order_n + _order_m)
        return FormattableString.Invariant($"b{i - 1 - _order_n}");
      else
        throw new IndexOutOfRangeException(nameof(i));
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
      var arg = X[0] - P[0];
      if (arg == 0)
      {
        Y[0] = P[1];
      }
      else if (arg < 0)
      {
        double sum = 0;
        for (int i = 1 + _order_n; i >= 1; --i)
        {
          sum *= arg;
          sum += P[i];
        }
        Y[0] = sum;
      }
      else
      {
        double sum = 0;
        for (int i = 1 + _order_n + _order_m; i >= 2 + _order_n; --i)
        {
          sum *= arg;
          sum += P[i];
        }
        Y[0] = sum * arg + P[1];
      }
    }


    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var arg = independent[r, 0] - P[0];

        if (arg <= 0)
        {
          double sum = 0;
          for (int i = 1 + _order_n; i >= 1; --i)
          {
            sum *= arg;
            sum += P[i];
          }
          FV[r] = sum;
        }
        else
        {
          double sum = 0;
          for (int i = 1 + _order_n + _order_m; i >= 2 + _order_n; --i)
          {
            sum *= arg;
            sum += P[i];
          }
          FV[r] = sum * arg + P[1];
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
        var arg = x - P[0];

        if (arg <= 0)
        {
          // w.r.t  xc
          double prod = 1;
          double sum = 0;
          for (int i = 0; i < _order_n; i++)
          {
            sum += P[i + 2] * prod;
            prod *= (i + 2) * arg;
          }
          DY[r, 0] = -sum; // derivative w.r.t. xc
          DY[r, 1] = 1; // derivative wrt y0

          // w.r.t. a_i
          prod = arg;
          for (int i = 0; i < _order_n; i++)
          {
            DY[r, i + 2] = prod;
            prod *= arg;
          }

          int offset = 2 + _order_n;
          for (int i = 0; i < _order_m; i++)
          {
            DY[r, i + offset] = 0; // derivative w.r.t b_i is zero
          }
        }
        else
        {
          // w.r.t  xc
          double prod = 1;
          double sum = 0;
          int offset = 2 + _order_n;
          for (int i = 0; i < _order_m; i++)
          {
            sum += P[i + offset] * prod;
            prod *= (i + 2) * arg;
          }
          DY[r, 0] = -sum; // derivative w.r.t. xc
          DY[r, 1] = 1; // derivative wrt y0

          for (int i = 0; i < _order_m; i++)
          {
            DY[r, i + 2] = 0; // derivative wrt a_i is zero
          }

          // w.r.t. a_i
          prod = arg;
          for (int i = 0; i < _order_m; i++)
          {
            DY[r, i + offset] = prod;
            prod *= arg;
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
