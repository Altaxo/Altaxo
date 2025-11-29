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

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Transitions
{
  /// <summary>
  /// This function produces a continuous transition with the Gompertz function from a left to a right polynomial,
  /// centered at xc, with the width of the transition determined by σ. It is commonly used for
  /// modeling tumor growth rates, and mortality and survival analysis.
  /// </summary>
  /// <remarks>This function is defined by at least four parameters: 'xc', 'r', which determine the position and the growth rate of the transition, and a0 and b0, which are the polynomial coefficients of zero order
  /// of the left and the right polynomial, respectively. It produces a
  /// Gompertz transition from the 'a0' value to the 'b0' value, centered at 'xc' with the width inversely dependent on 'r'.</remarks>
  [FitFunctionClass]
  public record GompertzTransition : IFitFunctionWithDerivative, Main.IImmutable
  {
    const double Sqrt2 = 1.4142135623730950488;
    const double Sqrt2Pi = 2.5066282746310005024;
    const double SqrtPi = 1.7724538509055160273;

    /// <summary>The order of the polynomial on the left polynomial.</summary>
    private int _order_l;
    /// <summary>The order of the polynomial on the right polynomial.</summary>
    private int _order_r;

    #region Serialization

    /// <summary>
    /// 2025-11-28 Initial version
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(GompertzTransition), 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GompertzTransition)obj;
        info.AddValue("OrderLeft", s._order_l);
        info.AddValue("OrderRight", s._order_r);
      }

      public virtual object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order_l = info.GetInt32("OrderLeft");
        var order_r = info.GetInt32("OrderRight");
        return new GompertzTransition(order_l, order_r);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="GompertzTransition"/> class, with both polynomial orders set to zero.
    /// </summary>
    public GompertzTransition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GompertzTransition"/> class.
    /// </summary>
    /// <param name="polynomialOrderLeft">The polynomial order of the left polynomial.</param>
    /// <param name="polynomialOrderRight">The polynomial order of the right polynomial.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Order for left segment has to be greater than or equal to zero
    /// or
    /// Order for right segment has to be greater than or equal to zero
    /// </exception>
    public GompertzTransition(int polynomialOrderLeft, int polynomialOrderRight)
    {
      _order_l = polynomialOrderLeft;
      _order_r = polynomialOrderRight;

      if (_order_l < 0)
        throw new ArgumentOutOfRangeException("Order for left segment has to be greater than or equal to zero");
      if (_order_r < 0)
        throw new ArgumentOutOfRangeException("Order for right segment has to be greater than or equal to zero");
    }

    [FitFunctionCreator("GompertzTransition", "Transitions", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.GompertzTransition}")]
    public static IFitFunction Create()
    {
      return new GompertzTransition(0, 0);
    }

    /// <summary>
    /// Gets the order of the left polynomial segment.
    /// </summary>
    public int PolynomialOrderLeft => _order_l;

    /// <summary>
    /// Creates a new instance with the provided order for the left polynomial segment.
    /// </summary>
    /// <param name="polynomialOrderLeft">The order for the left polynomial segment (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public GompertzTransition WithPolynomialOrderLeft(int polynomialOrderLeft)
    {
      if (!(polynomialOrderLeft >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrderLeft)} must be greater than or equal to 0");

      if (!(_order_l == polynomialOrderLeft))
      {
        return new GompertzTransition(polynomialOrderLeft, _order_r);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the polynomial order of the right segment.
    /// </summary>
    public int PolynomialOrderRight => _order_r;

    /// <summary>
    /// Creates a new instance with the provided order for the right polynomial segment.
    /// </summary>
    /// <param name="polynomialOrderRight">The order for the right polynomial segment (e.g. 2 will create a quadratic polynom).</param>
    /// <returns>New instance with the provided order.</returns>
    public GompertzTransition WithPolynomialOrderRight(int polynomialOrderRight)
    {
      if (!(polynomialOrderRight >= 0))
        throw new ArgumentOutOfRangeException($"{nameof(polynomialOrderRight)} must be greater than or equal to 0");

      if (!(_order_r == polynomialOrderRight))
      {
        return new GompertzTransition(_order_l, polynomialOrderRight);
      }
      else
      {
        return this;
      }
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 4 + _order_l + _order_r;

    /// <inheritdoc/>
    public event EventHandler? Changed;

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      if (i == 0)
        return 0; // xc
      else if (i == 1)
        return 1; // r
      else if (i >= 2 && i < 3 + _order_l)
        return 0; // a0, a1, ...
      else if (i >= 3 + _order_l && i < 4 + _order_l + _order_r)
        return 0; // b0, b1, ...
      else
        throw new ArgumentOutOfRangeException(nameof(i));

    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return i switch
      {
        0 => "y",
        _ => throw new ArgumentOutOfRangeException(nameof(i), "Dependent variable index is out of range.")
      };
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return i switch
      {
        0 => "x",
        _ => throw new ArgumentOutOfRangeException(nameof(i), "Independent variable index is out of range.")
      };
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      if (i == 0)
        return "xc";
      else if (i == 1)
        return "r";
      else if (i >= 2 && i < 3 + _order_l)
        return FormattableString.Invariant($"a{i - 2}");
      else if (i >= 3 + _order_l && i < 4 + _order_l + _order_r)
        return FormattableString.Invariant($"b{i - 3 - _order_l}");
      else
        throw new ArgumentOutOfRangeException(nameof(i));
    }

    /// <summary>
    /// This function produces a continuous transition with the Gompertz function from a left to a right polynomial,
    /// centered at xc, with the width of the transition determined by σ. It is commonly used for
    /// modeling tumor growth rates, and mortality and survival analysis.
    /// </summary>
    /// <param name="x">The input value at which to evaluate the function.</param>
    /// <param name="xc">The center point of the transition, where the function value is halfway between the starting and ending values.</param>
    /// <param name="r">The growth rate that controls the width of the transition region. Should be positive.</param>
    /// <param name="coeffs_left">The coefficients of the polynomial for the left segment (before the transition).</param>
    /// <param name="coeffs_right">The coefficients of the polynomial for the right segment (after the transition).</param>
    /// <returns>The value of the smoothed step function at the specified input value.</returns>
    public static double Evaluate(double x, double xc, double r, ReadOnlySpan<double> coeffs_left, ReadOnlySpan<double> coeffs_right)
    {
      var arg = r * (xc - x);
      // for reason of numerical stability we use Erfc here, because it really goes to zero for large arguments
      var transb = Math.Exp(-Math.Exp(arg));
      var transa = RMath.OneMinusExp(-Math.Exp(arg));

      double suma = coeffs_left[^1];
      for (int i = coeffs_left.Length - 2; i >= 0; --i)
      {
        suma *= x;
        suma += coeffs_left[i];
      }
      double sumb = coeffs_right[^1];
      for (int i = coeffs_right.Length - 2; i >= 0; --i)
      {
        sumb *= x;
        sumb += coeffs_right[i];
      }
      return suma * transa + sumb * transb;

    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], parameters[0], parameters[1], parameters.AsSpan(2, _order_l + 1), parameters.AsSpan(2 + _order_l + 1, _order_r + 1));
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var pa = parameters as double[];
      var coeffsLeft = (pa is not null) ? pa.AsSpan(2, _order_l + 1) : stackalloc double[_order_l + 1];
      var coeffsRight = (pa is not null) ? pa.AsSpan(3 + _order_l, _order_r + 1) : stackalloc double[_order_r + 1];
      if (pa is null)
      {
        for (int i = 0; i <= _order_l; i++)
          coeffsLeft[i] = parameters[2 + i];
        for (int i = 0; i <= _order_r; i++)
          coeffsRight[i] = parameters[3 + _order_l + i];
      }

      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], parameters[0], parameters[1], coeffsLeft, coeffsRight);
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      double xc = parameters[0];
      double r = parameters[1];
      int startCoeffsLeft = 2;
      int startCoeffsRight = 3 + _order_l;

      for (int i = 0; i < independent.RowCount; i++)
      {
        double x = independent[i, 0];
        var arg = r * (xc - x);
        var transb = Math.Exp(-Math.Exp(arg));
        var transa = RMath.OneMinusExp(-Math.Exp(arg));
        double expTerm = transb == 0 ? 0 : transb * Math.Exp(arg);

        double suma = parameters[startCoeffsRight - 1];
        for (int k = startCoeffsRight - 2; k >= startCoeffsLeft; --k)
        {
          suma *= x;
          suma += parameters[k];
        }
        double sumb = parameters[^1];
        for (int k = parameters.Count - 2; k >= startCoeffsRight; --k)
        {
          sumb *= x;
          sumb += parameters[k];
        }

        // Derivative with respect to "from"
        DF[i, 0] = expTerm * r * (suma - sumb); // Derivative with respect to "xc"
        DF[i, 1] = expTerm == 0 ? 0 : expTerm * (xc - x) * (suma - sumb); // Derivative with respect to "sigma"

        for (int k = 0; k <= _order_l; k++)
        {
          DF[i, startCoeffsLeft + k] = transa; // Derivative with respect to "a_k"
          transa *= x;
        }
        for (int k = 0; k <= _order_r; k++)
        {
          DF[i, startCoeffsRight + k] = transb; // Derivative with respect to "b_k"
          transb *= x;
        }
        // Note: transa and transb have been modified in the loops above, so they no longer hold their original values.
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null); // no boundaries for the parameter, the fact that r should be positive is rather a soft limit
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      var lb = new double?[NumberOfParameters];
      lb[1] = double.Epsilon; // r should be positive, but this is not a must
      return (lb, null);
    }
  }
}
