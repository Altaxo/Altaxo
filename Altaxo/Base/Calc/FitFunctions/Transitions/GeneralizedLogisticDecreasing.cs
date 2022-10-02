#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.FitFunctions.Transitions
{

  /// <summary>
  /// Fit fuction with one or more increasing steps (generalized logistic function), with a background polynomial
  /// of variable order. 
  /// </summary>
  /// <remarks>
  /// Reference: <see href="https://en.wikipedia.org/wiki/Sigmoid_function"/>
  /// </remarks>
  [FitFunctionClass]
  public class GeneralizedLogisticDecreasing
        : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>The number of logistic step terms.</summary>
    private readonly int _numberOfTerms;

    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;

    #region Serialization

    /// <summary>
    /// 2021-06-18 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GeneralizedLogisticDecreasing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GeneralizedLogisticDecreasing)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new GeneralizedLogisticDecreasing(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralizedLogisticDecreasing"/> class with
    /// <see cref="NumberOfTerms"/>=1, and <see cref="OrderOfBackgroundPolynomial"/>=0.
    /// </summary>
    public GeneralizedLogisticDecreasing()
    {
      _numberOfTerms = 1;
      _orderOfBackgroundPolynomial = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralizedLogisticDecreasing"/> class.
    /// </summary>
    /// <param name="numberOfTerms">The number of terms.</param>
    /// <param name="orderOfBackgroundPolynomial">The order of background polynomial.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.
    /// or
    /// Number of step terms has to be greater than or equal to 1
    /// </exception>
    public GeneralizedLogisticDecreasing(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of step terms has to be greater than or equal to 1");

    }

    /// <summary>
    /// Creates a new instance of the <see cref="GeneralizedLogisticDecreasing"/> class,
    /// with <see cref="NumberOfTerms"/>=1 and <see cref="OrderOfBackgroundPolynomial"/>=0.
    /// </summary>
    /// <returns>New instance of the <see cref="GeneralizedLogisticDecreasing"/> class.</returns>
    [FitFunctionCreator("GeneralizedLogisticDecreasing", "Transitions", 1, 1, 6)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.GeneralizedLogisticDecreasing}")]
    public static IFitFunction Create_1_0()
    {
      return new GeneralizedLogisticDecreasing(1, 0);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial => _orderOfBackgroundPolynomial;

    /// <summary>
    /// Creates a new instance with the provided order of the background polynomial.
    /// </summary>
    /// <param name="orderOfBackgroundPolynomial">The order of the background polynomial. If set to -1, the background polynomial will be disabled.</param>
    /// <returns>New instance with the background polynomial of the provided order.</returns>
    public GeneralizedLogisticDecreasing WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new GeneralizedLogisticDecreasing(_numberOfTerms, orderOfBackgroundPolynomial);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the number of logistic step terms.
    /// </summary>
    public int NumberOfTerms => _numberOfTerms;

    /// <summary>
    /// Creates a new instance with the provided number of logistic step terms.
    /// </summary>
    /// <param name="numberOfTerms">The number of Llogistic step terms (should be greater than or equal to 1).</param>
    /// <returns>New instance with the provided number of logistic step terms.</returns>
    public GeneralizedLogisticDecreasing WithNumberOfTerms(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfTerms))
      {
        return new GeneralizedLogisticDecreasing(numberOfTerms, _orderOfBackgroundPolynomial);
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
        return _numberOfTerms * 5 + _orderOfBackgroundPolynomial + 1;
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
      int k = i - 5 * _numberOfTerms;
      if (k < 0)
      {
        int j = i / 5;
        return (i % 5) switch
        {
          0 => FormattableString.Invariant($"a{j}"),
          1 => FormattableString.Invariant($"xc{j}"),
          2 => FormattableString.Invariant($"w{j}"),
          3 => FormattableString.Invariant($"gamma{j}"),
          4 => FormattableString.Invariant($"delta{j}"),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        return FormattableString.Invariant($"b{k}");
      }
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      int k = i - 5 * _numberOfTerms;
      if (k < 0)
      {
        return (i % 5) switch
        {
          0 => 0,
          1 => 0,
          2 => 1,
          3 => 1,
          4 => 1,
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        return 0;
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 5)
      {
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumTerms += P[j] / Math.Pow(1 + Math.Pow(Math.Exp(x), P[j + 3]), P[j + 4] / P[j + 3]);
      }

      if (_orderOfBackgroundPolynomial >= 0)
      {
        int offset = 5 * _numberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
        for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
        {
          sumPolynomial *= X[0];
          sumPolynomial += P[i + offset];
        }
      }
      Y[0] = sumTerms + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        // evaluation of terms
        double sumTerms = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 5)
        {
          double arg = (x - P[j + 1]) / P[j + 2];
          sumTerms += P[j] / Math.Pow(1 + Math.Pow(Math.Exp(arg), P[j + 3]), P[j + 4] / P[j + 3]);
        }

        if (_orderOfBackgroundPolynomial >= 0)
        {
          int offset = 5 * _numberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
          for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
          {
            sumPolynomial *= x;
            sumPolynomial += P[i + offset];
          }
        }
        FV[r] = sumTerms + sumPolynomial;
      }
    }

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IMatrix<double> DY)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];
        // at first, the terms
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 3)
        {
          var a = P[j];
          var w = P[j + 2];
          var g = P[j + 3];
          var d = P[j + 4];
          var arg = (x - P[j + 1]) / w;
          var adw = a * d / w;
          var eterm = Math.Pow(Math.Exp(arg), g);
          var dterm = 1 / (1 + eterm);
          var term = 1 / Math.Pow(1 + eterm, d / g);

          DY[r, j + 0] = term;
          DY[r, j + 1] = adw * eterm * term * dterm;
          DY[r, j + 2] = adw * eterm * term * dterm * arg;
          DY[r, j + 3] = (-a * d * term / g) * (arg * eterm * dterm + Math.Log(dterm) / g);
          DY[r, j + 4] = a * term * Math.Log(dterm) / g;
        }

        // now, the background
        if (_orderOfBackgroundPolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = 5 * _numberOfTerms; i <= _orderOfBackgroundPolynomial; ++i, ++j)
          {
            DY[r, j] = xn;
            xn *= x;
          }
        }
      }
    }
  }
}
