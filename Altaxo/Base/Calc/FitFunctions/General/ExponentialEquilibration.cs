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

namespace Altaxo.Calc.FitFunctions.General
{
  /// <summary>
  /// Represents an exponential equilibration (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class ExponentialEquilibration : IFitFunctionWithGradient
  {
    private int _numberOfTerms;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialEquilibration), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialEquilibration)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ExponentialEquilibration?)o ?? new ExponentialEquilibration();
        s._numberOfTerms = info.GetInt32("NumberOfTerms");
        return s;
      }
    }

    #endregion Serialization

    public ExponentialEquilibration()
    {
      _numberOfTerms = 1;
    }

    public ExponentialEquilibration(int numberOfTerms)
    {
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
    /// Called when anything in this fit function has changed.
    /// </summary>
    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fired when the fit function changed.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets or sets the number of exponential terms. Must be greater then or equal to 1.
    /// </summary>
    /// <value>
    /// The number of terms.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int NumberOfTerms
    {
      get
      {
        return _numberOfTerms;
      }
      set
      {
        if (!(value >=1))
          throw new ArgumentOutOfRangeException($"{nameof(NumberOfTerms)} must be greater than or equal to 1");

        if (!(_numberOfTerms == value))
        {
          _numberOfTerms = value;
          OnChanged();
        }
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
        return _numberOfTerms*2 + 2;
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
      if (i == 1)
        return "x0";
      else
        return (i - 2) % 2 == 0 ? FormattableString.Invariant($"A{(i - 2) / 2}") : FormattableString.Invariant($"Tau{(i - 2) / 2}");
    }

    public double DefaultParameterValue(int i)
    {
      if (i == 0 || i==1)
        return 0;
      else if ((i - 2) % 2 == 0)
        return 0;
      else
        return RMath.Pow(10, (i - 2) / 2);
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double sum = P[0]; // P[0] is offset
      double x = X[0] - P[1]; // P[1] is x0

      if (x > 0)
      {
        for (int i = 2; i < P.Length; i += 2)
        {
          sum += P[i] * (1 - Math.Exp(-x / P[i + 1]));
        }
      }
      Y[0] = sum;
    }

    

    #endregion IFitFunction Members

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      double x = X[0] - P[1]; // P[1] is x0

      DY[0][0] = 1; // offset
      if (x > 0)
      {
        double sum = 0;
        for (int i = 2; i < P.Length; i += 2)
        {
          DY[0][i] = 1 - Math.Exp(-x / P[i + 1]);
          DY[0][i + 1] = -P[i] * Math.Exp(-x / P[i + 1]) * x / RMath.Pow2(P[i + 1]);
          sum += -P[i] * Math.Exp(-x / P[i + 1]) / P[i + 1];
        }
        DY[0][1] = sum; // derivative w.r.t. to x0
      }
      else // x < 0
      {
        DY[0][1] = 0; // derivative w.r.t.to x0 is 0 for x<0
        for (int i = 2; i < P.Length; i += 2)
        {
          DY[0][i] = 0;
          DY[0][i + 1] = 0;
        }
      }
    }
  }
}
