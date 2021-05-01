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
  /// Represents an exponential decrease with offset (multiple exponential terms possible).
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Regression.Nonlinear.IFitFunction" />
  [FitFunctionClass]
  public class ExponentialDecrease : IFitFunctionWithGradient
  {
    private int _numberOfTerms;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExponentialDecrease), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExponentialDecrease)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ExponentialDecrease?)o ?? new ExponentialDecrease();
        s._numberOfTerms = info.GetInt32("NumberOfTerms");
        return s;
      }
    }

    #endregion Serialization

    public ExponentialDecrease()
    {
      _numberOfTerms = 1;
    }

    public ExponentialDecrease(int numberOfTerms)
    {
      NumberOfTerms = numberOfTerms;
    }



    /// <summary>
    /// Creates an exponential decrease fit function with one exponential term (3 parameters).
    /// </summary>
    /// <returns></returns>
    [FitFunctionCreator("ExponentialDecrease", "General", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.General.ExponentialDecrease}")]
    public static IFitFunction CreateExponentialDecrease()
    {
      return new ExponentialDecrease();
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
        return _numberOfTerms*2 + 1;
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
        sum += P[i] * Math.Exp(-X[0] / P[i + 1]);
      }
      Y[0] = sum;
    }

    

    #endregion IFitFunction Members

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      for (int i = 1; i < P.Length; i += 2)
      {
        DY[0][i] = Math.Exp(-X[0] / P[i + 1]);
        DY[0][i + 1] = P[i] * Math.Exp(-X[0] / P[i + 1]) * X[0] / RMath.Pow2(P[i + 1]);
      }
    }
  }
}
