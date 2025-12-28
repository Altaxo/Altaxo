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

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Represents an exponential decay with offset (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class ExponentialDecay : IFitFunctionWithDerivative, IImmutable
  {
    #region Serialization

    /// <summary>
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.General.ExponentialDecay", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialDecay), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialDecay)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        return new ExponentialDecay(numberOfTerms);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDecay"/> class with a single exponential term.
    /// </summary>
    public ExponentialDecay()
    {
      NumberOfTerms = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialDecay"/> class with the specified number of terms.
    /// </summary>
    /// <param name="numberOfTerms">The number of exponential terms. Must be greater than or equal to 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfTerms"/> is less than 1.</exception>
    public ExponentialDecay(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decay fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns>A new <see cref="ExponentialDecay"/> instance.</returns>
    [FitFunctionCreator("ExponentialDecay", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialDecay}")]
    public static IFitFunction CreateExponentialDecrease()
    {
      return new ExponentialDecay();
    }

    /// <summary>
    /// Event that would be raised when the instance changes. Not functional since this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    /// <summary>
    /// Gets the number of exponential terms. Must be greater than or equal to 1.
    /// </summary>
    /// <value>The number of terms.</value>
    public int NumberOfTerms { get; }

    /// <summary>
    /// Creates a new instance with the provided number of terms.
    /// </summary>
    /// <param name="value">The number of exponential terms. Must be greater than or equal to 1.</param>
    /// <returns>A new instance configured with the provided number of terms, or the same instance if unchanged.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is less than 1.</exception>
    public ExponentialDecay WithNumberOfTerms(int value)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      if (!(NumberOfTerms == value))
      {
        return new ExponentialDecay(value);
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
        return NumberOfTerms * 2 + 1;
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
      {
        return "y0";
      }
      else if (i - 1 < NumberOfTerms * 2)
      {
        return ((i - 1) % 2) switch
        {
          0 => FormattableString.Invariant($"a{(i - 1) / 2}"),
          1 => FormattableString.Invariant($"Tau{(i - 1) / 2}"),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        throw new ArgumentOutOfRangeException($"{nameof(i)} must be less than {NumberOfParameters}");
      }
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      if (i == 0)
      {
        return 0;
      }
      else if (i - 1 < NumberOfTerms * 2)
      {
        return ((i - 1) % 2) switch
        {
          0 => 0,
          1 => RMath.Pow(10, (i - 1) / 2),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        throw new ArgumentOutOfRangeException($"{nameof(i)} must be less than {NumberOfParameters}");
      }
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double sum = P[0];
      for (int i = 1; i < P.Length; i += 2)
      {
        sum += P[i] * Math.Exp(-X[0] / P[i + 1]);
      }
      Y[0] = sum;
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        double sum = P[0];
        for (int i = 1; i < P.Count; i += 2)
        {
          sum += P[i] * Math.Exp(-x / P[i + 1]);
        }
        FV[r] = sum;
      }
    }


    #endregion IFitFunction Members

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];

        DY[r, 0] = 1;
        for (int i = 1; i < P.Count; i += 2)
        {
          DY[r, i] = Math.Exp(-x / P[i + 1]);
          DY[r, i + 1] = P[i] * Math.Exp(-x / P[i + 1]) * x / RMath.Pow2(P[i + 1]);
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
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 1; i < NumberOfTerms; ++i, j += 2)
      {
        lowerBounds[j + 0] = 0; // Step amplitude
        upperBounds[j + 0] = null;

        lowerBounds[j + 1] = double.Epsilon; // characteristic time
        upperBounds[j + 1] = null;
      }
      return (lowerBounds, upperBounds);
    }
  }
}
