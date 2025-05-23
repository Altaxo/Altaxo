﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Summary description for KohlrauschDecay.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschDecay : IFitFunction, IImmutable
  {
    private int _numberOfRelaxations = 1;
    private bool _logarithmizeResult;

    public const int ParametersPerTerm = 3; // amplitude, tau, beta

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschDecay", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschDecay)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (KohlrauschDecay?)o ?? new KohlrauschDecay();
        return s;
      }
    }

    /// <summary>
    /// 2013-02-07 extended by NumberOfRelaxations and LogarithmizeResult
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschDecay", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschDecay), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschDecay)obj;
        info.AddValue("NumberOfRelaxations", s._numberOfRelaxations);
        info.AddValue("LogarithmizeResult", s._logarithmizeResult);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (KohlrauschDecay?)o ?? new KohlrauschDecay();
        s._numberOfRelaxations = info.GetInt32("NumberOfRelaxations");
        s._logarithmizeResult = info.GetBoolean("LogarithmizeResult");
        return s;
      }
    }

    #endregion Serialization

    public KohlrauschDecay()
    {
    }

    public int NumberOfRelaxations
    {
      get
      {
        return _numberOfRelaxations;
      }
      set
      {
        throw new NotImplementedException("Sorry, this function is deprecated. Use General/StretchedExponentialDecay.");
      }
    }

    /// <summary>
    /// Indicates whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the result is logarithmized; otherwise, <c>false</c>.
    /// </value>
    public bool LogarithmizeResult
    {
      get
      {
        return _logarithmizeResult;
      }
      set
      {
        throw new NotImplementedException("Sorry, this function is deprecated. Use General/StretchedExponentialDecay.");
      }
    }

    public override string ToString()
    {
      return "KohlrauschDecay";
    }

    public static IFitFunction CreateDefault()
    {
      return new KohlrauschDecay();
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
        return 1 + ParametersPerTerm * _numberOfRelaxations;
      }
    }

    public string IndependentVariableName(int i)
    {
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return _logarithmizeResult ? "lg y" : "y";
    }

    private static readonly string[] _parameterNames = new string[] { "offset", "amplitude", "tau", "beta" };

    public string ParameterName(int i)
    {
      var namearr = _parameterNames;
      if (0 == i)
      {
        return namearr[0]; // eps_inf
      }
      else if (i < 1 + ParametersPerTerm * _numberOfRelaxations)
      {
        var idx = (i - 1) % ParametersPerTerm;
        var term = (i - 1) / ParametersPerTerm;
        return namearr[idx + 1] + (term > 0 ? string.Format("_{0}", term + 1) : "");
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), "Parameter index out of range.");
      }
    }

    public double DefaultParameterValue(int i)
    {
      if (0 == i)
      {
        return 0; // offset
      }
      else if (i < 1 + ParametersPerTerm * _numberOfRelaxations)
      {
        --i;
        var idx = i % ParametersPerTerm;
        var term = i / ParametersPerTerm;

        if (term == 0)
          return 1; // 1 for all parameters in relaxation term1

        if (idx == 1 || idx == 2)
          return 1; // 1 for all taus and betas

        return 0; // 0 for all amplitudes
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), "Parameter index out of range.");
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double sum = P[0];

      for (int i = 0, j = 1; i < _numberOfRelaxations; ++i, j += 3)
        sum += P[j] * Math.Exp(-Math.Pow(X[0] / P[j + 1], P[j + 2]));

      Y[0] = _logarithmizeResult ? Math.Log10(sum) : sum;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        double sum = P[0];

        for (int i = 0, j = 1; i < _numberOfRelaxations; ++i, j += 3)
          sum += P[j] * Math.Exp(-Math.Pow(x / P[j + 1], P[j + 2]));

        FV[r] = _logarithmizeResult ? Math.Log10(sum) : sum;
      }
    }

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>


    /// <summary>
    /// Unused because this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

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
