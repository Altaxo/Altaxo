﻿#region Copyright

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Represents an stretched exponential equilibration function (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class StretchedExponentialEquilibration : IFitFunctionWithGradient
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-05-11.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StretchedExponentialEquilibration), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (StretchedExponentialEquilibration)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        return new StretchedExponentialEquilibration(numberOfTerms);
      }
    }

    #endregion Serialization

    public StretchedExponentialEquilibration()
    {
      NumberOfTerms = 1;
    }

    public StretchedExponentialEquilibration(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decrease fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns></returns>
    [FitFunctionCreator("StretchedExponentialEquilibration", "General", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.StretchedExponentialEquilibration}")]
    public static IFitFunction CreateFitFunction()
    {
      return new StretchedExponentialEquilibration(1);
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
    public StretchedExponentialEquilibration WithNumberOfTerms(int value)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      if (!(NumberOfTerms == value))
      {
        return new StretchedExponentialEquilibration(value);
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
        return NumberOfTerms*3 + 2;
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
      double sum = P[1]; // P[0] is offset
      if (x > 0)
      {
        for (int i = 2, j = 0; j < NumberOfTerms; i += 3, ++j) 
        {
          sum += P[i] * (1 - Math.Exp(-Math.Pow(x / P[i + 1], P[i+2])));
        }
      }
      Y[0] = sum;
    }

    

    #endregion IFitFunction Members

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      double x = X[0] - P[0]; // P[1] is x0

      DY[0][1] = 1; // offset
      if (x > 0)
      {
        double sum = 0;
        for (int i = 2; i < P.Length; i += 3)
        {
          var tau = P[i + 1];
          var beta = P[i + 2];
          var arg = Math.Pow(x / tau, beta);
          var earg = Math.Exp(-arg);

          DY[0][i] = 1 - earg;
          DY[0][i + 1] = -P[i] * earg * arg *  beta / tau;
          DY[0][i + 2] = P[i] * earg * arg * Math.Log(x / tau);
          sum += -P[i] * earg * arg * beta / x;
        }
        DY[0][0] = sum; // derivative w.r.t. to x0
      }
      else // x < 0
      {
        DY[0][0] = 0; // derivative w.r.t.to x0 is 0 for x<0
        for (int i = 2; i < P.Length; i += 3)
        {
          DY[0][i] = 0;
          DY[0][i + 1] = 0;
          DY[0][i + 2] = 0;
        }
      }
    }
  }
}