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


    /// <summary>
    /// Initializes a new instance of the <see cref="TwoPolynomialSegments"/> class with linear segments.
    /// </summary>
    public TwoPolynomialSegments()
    {
      _order_n = 1;
      _order_m = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoPolynomialSegments"/> class.
    /// </summary>
    /// <param name="polynomialOrderLeftSegment">The polynomial order of the left segment.</param>
    /// <param name="polynomialOrderRightSegment">The polynomial order of the right segment.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Order for left segment has to be greater than or equal to zero
    /// or
    /// Order for right segment has to be greater than or equal to zero
    /// or
    /// Order for either the left segment or the right segment has to be greater than zero.
    /// </exception>
    public TwoPolynomialSegments(int polynomialOrderLeftSegment, int polynomialOrderRightSegment)
    {
      _order_n = polynomialOrderLeftSegment;
      _order_m = polynomialOrderRightSegment;

      if (_order_n < 0)
        throw new ArgumentOutOfRangeException("Order for left segment has to be greater than or equal to zero");
      if (_order_m < 0)
        throw new ArgumentOutOfRangeException("Order for right segment has to be greater than or equal to zero");
      if (_order_n == 0 && _order_m == 0)
        throw new ArgumentOutOfRangeException("Order for either the left segment or the right segment has to be greater than zero.");
    }

    /// <summary>Creates a new instance of <see cref="TwoPolynomialSegments"/> with linear segments.</summary>
    /// <returns>New instance of <see cref="TwoPolynomialSegments"/> with linear segments.</returns>
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

    /// <inheritdoc/>
    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfParameters
    {
      get
      {
        return 2 + _order_n + _order_m;
      }
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "x";
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "y";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      if (i == 0)
        return "xc";
      else if (i == 1)
        return "y0";
      else if (i > 1 && i < 2 + _order_n)
        return FormattableString.Invariant($"a{i - 1}");
      else if (i > 1 && i < 2 + _order_n + _order_m)
        return FormattableString.Invariant($"b{i - 2 - _order_n}");
      else
        throw new ArgumentOutOfRangeException(nameof(i));
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i >= 0 && i < NumberOfParameters ? 0 : throw new ArgumentOutOfRangeException(nameof(i));
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return i >= 0 && i < NumberOfParameters ? null : throw new ArgumentOutOfRangeException(nameof(i));
    }

    /// <summary>
    /// Evaluates the function for the specified x value.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="xc">The xc value which is the x-value of the transition between the two segments.</param>
    /// <param name="y0">The y0 value of the transition. This is the y-value of the curve at xc if the transition is sharp (sigma == 0).</param>
    /// <param name="coeffs_left">The polynomial coefficients of the left polynomial (order 1, order 2, ...).</param>
    /// <param name="coeffs_right">The polynomial coefficients of the right polynomial (order1, order 2, ...).</param>
    /// <returns>The y value of the function at x.</returns>
    public static double Evaluate(double x, double xc, double y0, ReadOnlySpan<double> coeffs_left, ReadOnlySpan<double> coeffs_right)
    {
      var arg = x - xc;

      if (arg == 0)
      {
        return y0;
      }
      else
      {
        var coeffs = arg < 0 ? coeffs_left : coeffs_right;
        double sum = 0;
        for (int i = coeffs.Length - 1; i >= 0; --i)
        {
          sum += coeffs[i];
          sum *= arg;
        }
        return sum + y0;
      }
    }


    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P[0], P[1], P.AsSpan(2, _order_n), P.AsSpan(2 + _order_n, _order_m));
    }


    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var pa = P as double[];
      var coeffsLeft = _order_n == 0 ? Span<double>.Empty : ((pa is not null) ? pa.AsSpan(2, _order_n) : stackalloc double[_order_n]);
      var coeffsRight = _order_n == 0 ? Span<double>.Empty : ((pa is not null) ? pa.AsSpan(2 + _order_n, _order_m) : stackalloc double[_order_m]);
      if (pa is null)
      {
        for (int i = 0; i < _order_n; i++)
          coeffsLeft[i] = P[2 + i];
        for (int i = 0; i < _order_m; i++)
          coeffsRight[i] = P[2 + _order_n + i];
      }

      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        FV[r] = Evaluate(independent[r, 0], P[0], P[1], coeffsLeft, coeffsRight);
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
      var xc = P[0];
      var y0 = P[1];

      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];
        var arg = x - xc;


        if (arg <= 0)
        {
          // w.r.t  xc
          double prod = 1;
          double sum = 0;
          for (int i = 0; i < _order_n; i++)
          {
            sum += P[i + 2] * (i + 1) * prod;
            prod *= arg;
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
            sum += P[i + offset] * (i + 1) * prod;
            prod *= arg;
          }
          DY[r, 0] = -sum; // derivative w.r.t. xc
          DY[r, 1] = 1; // derivative wrt y0

          for (int i = 0; i < _order_n; i++)
          {
            DY[r, i + 2] = 0; // derivative wrt a_i is zero
          }

          // w.r.t. b_i
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
