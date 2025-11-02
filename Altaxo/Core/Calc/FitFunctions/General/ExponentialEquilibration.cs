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

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Represents an exponential equilibration (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class ExponentialEquilibration : IFitFunctionWithDerivative
  {
    #region Serialization

    /// <summary>
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.General.ExponentialEquilibration", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialEquilibration), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialEquilibration)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        return new ExponentialEquilibration(numberOfTerms);
      }
    }

    #endregion Serialization

    public ExponentialEquilibration()
    {
      NumberOfTerms = 1;
    }

    public ExponentialEquilibration(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decrease fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns></returns>
    [FitFunctionCreator("ExponentialEquilibration", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialEquilibration}")]
    public static IFitFunction CreateExponentialDecrease()
    {
      return new ExponentialEquilibration();
    }



    /// <summary>
    /// Not functional since this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    /// <summary>
    /// Gets the number of exponential terms. Must be greater than or equal to 1.
    /// </summary>
    /// <value>
    /// The number of terms.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int NumberOfTerms { get; }

    /// <summary>
    /// Creates a new instance with the provided number of terms.
    /// </summary>
    /// <param name="value">The number of exponential terms.</param>
    /// <returns>New instance with the provided number of terms.</returns>
    /// <exception cref="ArgumentOutOfRangeException">$"{nameof(NumberOfTerms)} must be greater than or equal to 1</exception>
    public ExponentialEquilibration WithNumberOfTerms(int value)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      if (!(NumberOfTerms == value))
      {
        return new ExponentialEquilibration(value);
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
        return NumberOfTerms * 2 + 2;
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
        return "x0";
      if (i == 1)
        return "y0";
      else if (i - 2 < NumberOfTerms * 2)
      {
        return ((i - 2) % 2) switch
        {
          0 => FormattableString.Invariant($"a{(i - 2) / 2}"),
          1 => FormattableString.Invariant($"Tau{(i - 2) / 2}"),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        throw new ArgumentOutOfRangeException($"{nameof(i)} must be less than {NumberOfParameters}");
      }
    }

    public double DefaultParameterValue(int i)
    {
      if (i == 0 || i == 1)
        return 0;
      else if (i - 2 < NumberOfTerms * 2)
      {
        return ((i - 2) % 2) switch
        {
          0 => 0,
          1 => RMath.Pow(10, (i - 2) / 2),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        throw new ArgumentOutOfRangeException($"{nameof(i)} must be less than {NumberOfParameters}");
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0] - P[0]; // P[1] is x0
      double sum = P[1]; // P[0] is offset
      if (x > 0)
      {
        for (int i = 2; i < P.Length; i += 2)
        {
          sum += P[i] * (1 - Math.Exp(-x / P[i + 1]));
        }
      }
      Y[0] = sum;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var xx = independent[r, 0];

        double x = xx - P[0]; // P[1] is x0
        double sum = P[1]; // P[0] is offset
        if (x > 0)
        {
          for (int i = 2; i < P.Count; i += 2)
          {
            sum += P[i] * (1 - Math.Exp(-x / P[i + 1]));
          }
        }
        FV[r] = sum;
      }
    }


    #endregion IFitFunction Members

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var xx = X[r, 0];

        double x = xx - P[0]; // P[1] is x0

        DY[r, 1] = 1; // offset
        if (x > 0)
        {
          double sum = 0;
          for (int i = 2; i < P.Count; i += 2)
          {
            DY[r, i] = 1 - Math.Exp(-x / P[i + 1]);
            DY[r, i + 1] = -P[i] * Math.Exp(-x / P[i + 1]) * x / RMath.Pow2(P[i + 1]);
            sum += -P[i] * Math.Exp(-x / P[i + 1]) / P[i + 1];
          }
          DY[r, 0] = sum; // derivative w.r.t. to x0
        }
        else // x < 0
        {
          DY[r, 0] = 0; // derivative w.r.t.to x0 is 0 for x<0
          for (int i = 2; i < P.Count; i += 2)
          {
            DY[r, i] = 0;
            DY[r, i + 1] = 0;
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
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 2; i < NumberOfTerms; ++i, j += 2)
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
