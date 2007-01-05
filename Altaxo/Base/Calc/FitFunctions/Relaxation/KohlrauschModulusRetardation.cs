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
  /// Kohlrausch function in the frequency domain to fit modulus spectra. This is the inverse of the retardation spectra,
  /// i.e. tau is the retardation time (and not the relaxation time!).
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschModulusRetardation : IFitFunction
  {
    bool _useFrequencyInsteadOmega;
    bool _useFlowTerm;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschModulusRetardation), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        KohlrauschModulusRetardation s = (KohlrauschModulusRetardation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschModulusRetardation s = o != null ? (KohlrauschModulusRetardation)o : new KohlrauschModulusRetardation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    #endregion

    public KohlrauschModulusRetardation()
    {
      
    }

    public override string ToString()
    {
      return "Kohlrausch Modulus Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Modulus", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.ModulusRetardation.Introduction;XML.MML.GenericRetardationModulus;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.ModulusRetardation.Part3")]
    public static IFitFunction CreateFofOmega()
    {
      KohlrauschModulusRetardation result = new KohlrauschModulusRetardation();
      result._useFrequencyInsteadOmega = false;
      result._useFlowTerm = true;

      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Modulus", 1, 2, 4)]
    [Description("FitFunctions.Relaxation.ModulusRetardation.Introduction;XML.MML.GenericRetardationModulus;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.ModulusRetardation.Part3")]
    public static IFitFunction CreateFofFrequency()
    {
      KohlrauschModulusRetardation result = new KohlrauschModulusRetardation();
      result._useFrequencyInsteadOmega = true;
      result._useFlowTerm = true;

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
    string[] _parameterName = new string[] { "m_0", "m_inf", "tau_retard", "beta", "viscosity" };
    public int NumberOfParameters
    {
      get
      {
        return this._useFlowTerm ? _parameterName.Length : _parameterName.Length - 1;
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

    #endregion

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      double w_r = x * P[2]; // omega scaled with tau

      Complex result = 1 / P[1] + (1 / P[0] - 1 / P[1]) * Kohlrausch.ReIm(P[3], w_r);

      if (this._useFlowTerm)
      {
        result.Im -= P[4] / (x);
      }


      result = 1 / result;
      Y[0] = result.Re;
      Y[1] = result.Im;
    }

    #endregion
  }
}
