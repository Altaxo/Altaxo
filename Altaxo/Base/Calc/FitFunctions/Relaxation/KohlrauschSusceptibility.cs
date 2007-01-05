#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

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
    bool _useFrequencyInsteadOmega;
    bool _useFlowTerm;
    bool _isDielectricData;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 0)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        KohlrauschSusceptibility s = (KohlrauschSusceptibility)obj;
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

    #endregion

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
      KohlrauschSusceptibility result = new KohlrauschSusceptibility();
      result._useFrequencyInsteadOmega = false;
      result._useFlowTerm = true;
      result._isDielectricData = true;
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/General", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.Susceptibility.Introduction;XML.MML.GenericSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschSusceptibility.Part3")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      KohlrauschSusceptibility result = new KohlrauschSusceptibility();
      result._useFrequencyInsteadOmega = true;
      result._useFlowTerm = true;
      result._isDielectricData = true;
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Dielectrics", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      KohlrauschSusceptibility result = new KohlrauschSusceptibility();
      result._useFrequencyInsteadOmega = false;
      result._useFlowTerm = true;
      result._isDielectricData = true;
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Dielectrics", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      KohlrauschSusceptibility result = new KohlrauschSusceptibility();
      result._useFrequencyInsteadOmega = true;
      result._useFlowTerm = true;
      result._isDielectricData = true;
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
      return this._useFrequencyInsteadOmega ? "Frequency" : "Omega";
    }
    #endregion

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
    #endregion

    #region parameter definition
    string[] _parameterNameC = new string[] { "chi_inf", "delta_chi", "tau", "beta", "viscosity" };
    string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "beta", "conductivity" };
    public int NumberOfParameters
    {
      get
      {
        if (_isDielectricData)
          return this._useFlowTerm ? _parameterNameD.Length : _parameterNameD.Length - 1;
        else
          return this._useFlowTerm ? _parameterNameC.Length : _parameterNameC.Length - 1;
      }
    }
    public string ParameterName(int i)
    {
      if (_isDielectricData)
        return _parameterNameD[i];
      else
        return _parameterNameC[i];
    }

    public double DefaultParameterValue(int i)
    {
      switch (i)
      {
        case 0:
          return 1;
        case 1:
          return 1;
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

    #endregion

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      double w_r = x * P[2]; // omega scaled with tau

      Complex result = P[0] + P[1] * Kohlrausch.ReIm(P[3], w_r);
      Y[0] = result.Re;

      if (this._useFlowTerm)
      {
        if (this._isDielectricData)
          Y[1] = -result.Im + P[4] / (x * 8.854187817e-12);
        else
          Y[1] = -result.Im + P[4] / (x);
      }
      else
      {
        Y[1] = -result.Im;
      }
    }

    #endregion
  }

}
