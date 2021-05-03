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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Represents an exponential decay with offset (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class ExponentialGrowth : IFitFunctionWithGradient, IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialGrowth), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialGrowth)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        return new ExponentialGrowth(numberOfTerms);
      }
    }

    #endregion Serialization

    public ExponentialGrowth()
    {
      NumberOfTerms = 1;
    }

    public ExponentialGrowth(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decrease fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns></returns>
    [FitFunctionCreator("ExponentialGrowth", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialGrowth}")]
    public static IFitFunction CreateExponentialDecrease()
    {
      return new ExponentialGrowth();
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
    public ExponentialGrowth WithNumberOfTerms(int value)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

      if (!(NumberOfTerms == value))
      {
        return new ExponentialGrowth(value);
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
        return NumberOfTerms*2 + 1;
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
        return "Offset";
      else
      return (i - 1) % 2 == 0 ? FormattableString.Invariant($"A{(i - 1) / 2}") : FormattableString.Invariant($"Tau{(i - 1) / 2}");
    }

    public double DefaultParameterValue(int i)
    {
      if (i == 0)
        return 0;
      else if ((i - 1) % 2 == 0)
        return 0;
      else
        return RMath.Pow(10, (i - 1) / 2);
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double sum = P[0];
      for (int i = 1; i < P.Length; i+=2)
      {
        sum += P[i] * Math.Exp(X[0] / P[i + 1]);
      }
      Y[0] = sum;
    }

    

    #endregion IFitFunction Members

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      DY[0][0] = 1;
      for (int i = 1; i < P.Length; i += 2)
      {
        DY[0][i] = Math.Exp(X[0] / P[i + 1]);
        DY[0][i + 1] = -P[i] * Math.Exp(X[0] / P[i + 1]) * X[0] / RMath.Pow2(P[i + 1]);
      }
    }
  }
}
