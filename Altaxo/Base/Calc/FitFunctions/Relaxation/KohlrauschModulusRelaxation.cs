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
  /// Kohlrausch function in the frequency domain to fit modulus spectra. This is the inverse of the relaxation spectra,
  /// i.e. tau is the relaxation time (and not the relaxation time!).
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschModulusRelaxation : IFitFunction
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _logarithmizeResults;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschModulusRelaxation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschModulusRelaxation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschModulusRelaxation s = o != null ? (KohlrauschModulusRelaxation)o : new KohlrauschModulusRelaxation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschModulusRelaxation), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschModulusRelaxation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschModulusRelaxation s = o != null ? (KohlrauschModulusRelaxation)o : new KohlrauschModulusRelaxation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    #endregion Serialization

    public KohlrauschModulusRelaxation()
    {
    }

    public override string ToString()
    {
      return "Kohlrausch Modulus Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Relaxation/Modulus", 1, 2, 4)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.Generic.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschKernel.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateModulusOfOmega()
    {
      var result = new KohlrauschModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true
      };

      return result;
    }

    [FitFunctionCreator("Lg10 Kohlrausch Complex (Omega)", "Relaxation/Modulus", 1, 2, 4)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.Generic.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschKernel.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateLg10ModulusOfOmega()
    {
      var result = new KohlrauschModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _logarithmizeResults = true
      };

      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Relaxation/Modulus", 1, 2, 4)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.Generic.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschKernel.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateModulusOfFrequency()
    {
      var result = new KohlrauschModulusRelaxation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true
      };

      return result;
    }

    [FitFunctionCreator("Lg10 Kohlrausch Complex (Freq)", "Relaxation/Modulus", 1, 2, 4)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.Generic.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;" +
      "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschKernel.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateLg10ModulusOfFrequency()
    {
      var result = new KohlrauschModulusRelaxation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _logarithmizeResults = true
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

    private string[] _parameterName = new string[] { "m_0", "m_inf", "tau_relax", "beta", "invviscosity" };

    public int NumberOfParameters
    {
      get
      {
        return _useFlowTerm ? _parameterName.Length : _parameterName.Length - 1;
      }
    }

    public string ParameterName(int i)
    {
      return _parameterName[i];
    }

    public double DefaultParameterValue(int i)
    {
      switch (i)
      {
        case 0:
          return 1;

        case 1:
          return 2;

        case 2:
          return 1;

        case 3:
          return 1;
      }

      return 0;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    #endregion parameter definition

    /// <summary>
    ///
    /// </summary>
    /// <param name="X"></param>
    /// <param name="P"></param>
    /// <param name="Y"></param>
    /// <remarks>
    /// P[0]: M_0,
    /// P[1]: M_infinity
    /// P[2]: tau_relax
    /// P[3]: beta
    /// P[4]: invviscosity
    /// </remarks>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      double w_r = x * P[2]; // omega scaled with tau

      Complex result = P[1] + (P[0] - P[1]) * Kohlrausch.ReIm(P[3], w_r);

      if (_useFlowTerm)
      {
        result = 1 / ((1 / result) - Complex.I * P[4] / x);
      }

      if (_logarithmizeResults)
      {
        Y[0] = Math.Log10(result.Re);
        Y[1] = Math.Log10(result.Im);
      }
      else
      {
        Y[0] = result.Re;
        Y[1] = result.Im;
      }
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
