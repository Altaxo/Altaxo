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

using System;
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Kohlrausch function in the frequency domain to fit compliance or dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschSusceptibility : IFitFunction
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _isDielectricData;
    private bool _invertViscosity = true;
    private int _numberOfRelaxations = 1;
    private bool _invertResult;
    private bool _logarithmizeResults;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschSusceptibility s = o != null ? (KohlrauschSusceptibility)o : new KohlrauschSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
        info.AddValue("InvertViscosity", s._invertViscosity);
        info.AddValue("NumberOfRelaxations", s._numberOfRelaxations);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschSusceptibility s = o != null ? (KohlrauschSusceptibility)o : new KohlrauschSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        s._numberOfRelaxations = info.GetInt32("NumberOfRelaxations");
        return s;
      }
    }

    /// <summary>
    /// 2013-02-07 extended by InvertResult und LogarithmizeResults
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
        info.AddValue("InvertViscosity", s._invertViscosity);
        info.AddValue("NumberOfRelaxations", s._numberOfRelaxations);
        info.AddValue("InvertResult", s._invertResult);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschSusceptibility s = o != null ? (KohlrauschSusceptibility)o : new KohlrauschSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        s._numberOfRelaxations = info.GetInt32("NumberOfRelaxations");
        s._invertResult = info.GetBoolean("InvertResult");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

    #endregion Serialization

    public bool UseFrequencyInsteadOmega
    {
      get { return _useFrequencyInsteadOmega; }
      set
      {
        var oldValue = _useFrequencyInsteadOmega;
        _useFrequencyInsteadOmega = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool UseFlowTerm
    {
      get { return _useFlowTerm; }
      set
      {
        var oldValue = _useFlowTerm;
        _useFlowTerm = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool IsDielectricData
    {
      get { return _isDielectricData; }
      set
      {
        var oldValue = _isDielectricData;
        _isDielectricData = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool InvertViscosity
    {
      get { return _invertViscosity; }
      set
      {
        var oldValue = _invertViscosity;
        _invertViscosity = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public int NumberOfRelaxations
    {
      get { return _numberOfRelaxations; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("NumberOfRelaxations has to be a positive number");
        var oldValue = _numberOfRelaxations;
        _numberOfRelaxations = value;

        if (oldValue != value)
          OnChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the complex dependent variable (the output of the fit function) should be inverted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is inverted; otherwise, <c>false</c>.
    /// </value>
    public bool InvertResult
    {
      get
      {
        return _invertResult;
      }
      set
      {
        var oldValue = _invertResult;
        _invertResult = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    /// <summary>
    /// Indicates whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the result is logarithmized; otherwise, <c>false</c>.
    /// </value>
    public bool LogarithmizeResults
    {
      get
      {
        return _logarithmizeResults;
      }
      set
      {
        var oldValue = _logarithmizeResults;
        _logarithmizeResults = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    public KohlrauschSusceptibility()
    {
    }

    public override string ToString()
    {
      return "Kohlrausch Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/General", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.Susceptibility.Introduction;XML.MML.GenericSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.KohlrauschSusceptibility.Part3")]
    public static IFitFunction CreateGeneralFunctionOfOmega()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _isDielectricData = true
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/General", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.Susceptibility.Introduction;XML.MML.GenericSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschSusceptibility.Part3")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _isDielectricData = true
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Dielectrics", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _isDielectricData = true
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Dielectrics", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _isDielectricData = true
      };
      return result;
    }

    #region IFitFunction Members

    #region independent variable definition

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public string IndependentVariableName(int i)
    {
      return _useFrequencyInsteadOmega ? "Frequency" : "Omega";
    }

    #endregion independent variable definition

    #region dependent variable definition

    private string[] _dependentVariableName = new string[] { "re", "im" };

    public int NumberOfDependentVariables
    {
      get
      {
        return _dependentVariableName.Length;
      }
    }

    public string DependentVariableName(int i)
    {
      return _dependentVariableName[i];
    }

    #endregion dependent variable definition

    #region parameter definition

    private string[] _parameterNameC = new string[] { "chi_inf", "delta_chi", "tau", "beta", "invviscosity" };
    private string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "beta", "conductivity", };

    public int NumberOfParameters
    {
      get
      {
        if (_useFlowTerm)
          return 2 + 3 * _numberOfRelaxations;
        else
          return 1 + 3 * _numberOfRelaxations;
      }
    }

    public string ParameterName(int i)
    {
      string[] names = _isDielectricData ? _parameterNameD : _parameterNameC;
      if (_numberOfRelaxations == 1)
        return names[i];
      else if (_numberOfRelaxations == 0)
        return i == 1 ? names[4] : names[0];
      else
      {
        if (i == 0)
          return names[0];
        else if (i == NumberOfParameters - 1)
          return names[4];
        else
        {
          int k = (i - 1) / 3;
          int l = (i - 1) % 3;
          return names[l + 1] + (k + 1).ToString();
        }
      }
    }

    public double DefaultParameterValue(int i)
    {
      if (_useFlowTerm && i == (NumberOfParameters - 1))
        return _isDielectricData ? 0 : _invertViscosity ? 0 : double.PositiveInfinity;
      else
        return 1;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    #endregion parameter definition

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      Complex result = P[0];

      int iPar = 1;
      int i;
      for (i = 0, iPar = 1; i < _numberOfRelaxations; i++, iPar += 3)
      {
        result += P[0 + iPar] * Kohlrausch.ReIm(P[2 + iPar], P[1 + iPar] * x);
      }

      // note: because it is a susceptiblity, the imaginary part is still negative

      if (_useFlowTerm)
      {
        if (_isDielectricData)
          result.Im -= P[iPar] / (x * 8.854187817e-12);
        else if (_invertViscosity)
          result.Im -= P[iPar] / (x);
        else
          result.Im -= 1 / (P[iPar] * x);
      }

      if (_invertResult)
        result = 1 / result; // if we invert, i.e. we calculate the modulus, the imaginary part is now positive
      else
        result.Im = -result.Im; // else if we don't invert, i.e. we calculate susceptibility, we negate the imaginary part to make it positive

      if (_logarithmizeResults)
      {
        result.Re = Math.Log10(result.Re);
        result.Im = Math.Log10(result.Im);
      }

      Y[0] = result.Re;
      Y[1] = result.Im;
    }

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>
    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fired when the fit function changed.
    /// </summary>
    public event EventHandler Changed;

    #endregion IFitFunction Members
  }
}
