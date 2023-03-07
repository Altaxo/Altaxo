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
  /// Represents an stretched exponential equilibration function (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class StretchedExponentialDecay : IFitFunctionWithDerivative, IImmutable
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-05-19.
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.General.StretchedExponentialDecay", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StretchedExponentialDecay), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (StretchedExponentialDecay)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        return new StretchedExponentialDecay(numberOfTerms);
      }
    }

    #endregion Serialization

    public StretchedExponentialDecay()
    {
      NumberOfTerms = 1;
    }

    public StretchedExponentialDecay(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decrease fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns></returns>
    [FitFunctionCreator("StretchedExponentialDecay", "General", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.StretchedExponentialDecay}")]
    public static IFitFunction CreateFitFunction()
    {
      return new StretchedExponentialDecay(1);
    }

    [FitFunctionCreator("KohlrauschDecay", "Relaxation", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.StretchedExponentialDecay}")]
    public static IFitFunction CreateKohlrauschDecay()
    {
      return new StretchedExponentialDecay(1);
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
    public StretchedExponentialDecay WithNumberOfTerms(int value)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      if (!(NumberOfTerms == value))
      {
        return new StretchedExponentialDecay(value);
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
        return NumberOfTerms * 3 + 2;
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
      else
        return ((i - 2) % 3) switch
        {
          0 => FormattableString.Invariant($"a{(i - 2) / 3}"),
          1 => FormattableString.Invariant($"τ{(i - 2) / 3}"),
          2 => FormattableString.Invariant($"β{(i - 2) / 3}"),
          _ => throw new InvalidProgramException()
        };
    }

    public double DefaultParameterValue(int i)
    {
      if (i == 0 || i == 1)
        return 0;
      else
        return ((i - 2) % 3) switch
        {
          0 => 0,
          1 => 1,
          2 => 1,
          _ => throw new InvalidProgramException()
        };
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0] - P[0]; // P[1] is x0
      double sum = P[1]; // P[1] is offset 
      if (x > 0)
      {
        for (int i = 2, j = 0; j < NumberOfTerms; i += 3, ++j)
        {
          sum += P[i] * (Math.Exp(-Math.Pow(x / P[i + 1], P[i + 2])));
        }
      }
      else
      {
        for (int i = 2, j = 0; j < NumberOfTerms; i += 3, ++j)
        {
          sum += P[i];
        }
      }
      Y[0] = sum;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        double arg = x - P[0]; // P[1] is x0
        double sum = P[1]; // P[1] is offset 
        if (arg > 0)
        {
          for (int i = 2, j = 0; j < NumberOfTerms; i += 3, ++j)
          {
            sum += P[i] * (Math.Exp(-Math.Pow(arg / P[i + 1], P[i + 2])));
          }
        }
        else
        {
          for (int i = 2, j = 0; j < NumberOfTerms; i += 3, ++j)
          {
            sum += P[i];
          }
        }
        FV[r] = sum;
      }
    }

    #endregion IFitFunction Members

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool> dependentVariableChoice)
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
          for (int i = 2; i < P.Count; i += 3)
          {
            var tau = P[i + 1];
            var beta = P[i + 2];
            var arg = Math.Pow(x / tau, beta);
            var earg = Math.Exp(-arg);

            DY[r, i] = earg; // dy/da
            DY[r, i + 1] = P[i] * earg * arg * beta / tau; // dy/dtau
            DY[r, i + 2] = -P[i] * earg * arg * Math.Log(x / tau); // dy/dbeta
            sum += P[i] * earg * arg * beta / x;
          }
          DY[r, 0] = sum; // derivative w.r.t. to x0
        }
        else // x < 0
        {
          DY[r, 0] = 0; // derivative w.r.t.to x0 is 0 for x<0
          for (int i = 2; i < P.Count; i += 3)
          {
            DY[r, i] = 1; // dy/da is 1
            DY[r, i + 1] = 0;
            DY[r, i + 2] = 0;
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
