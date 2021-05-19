#region Copyright

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

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschDecay), 0)]
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
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschDecay), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        return 1 + 3 * _numberOfRelaxations;
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
        return namearr[0]; // eps_inf

      --i;
      var idx = i % 3;
      var term = i / 3;
      return namearr[idx + 1] + (term > 0 ? string.Format("_{0}", term + 1) : "");
    }

    public double DefaultParameterValue(int i)
    {
      if (0 == i)
        return 0; // offset
      --i;
      var idx = i % 3;
      var term = i / 3;

      if (term == 0)
        return 1; // 1 for all parameters in relaxation term1

      if (idx == 1 || idx == 2)
        return 1; // 1 for all taus and betas

      return 0; // 0 for all amplitudes
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

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>
   

    /// <summary>
    /// Unused because this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members
  }
}
