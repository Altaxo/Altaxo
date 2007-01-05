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
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiSusceptibility : IFitFunctionWithGradient
  {
    bool _useFrequencyInsteadOmega;
    bool _useFlowTerm;
    bool _isDielectricData;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Calc.Regression.Nonlinear.HavriliakNegamiComplex", 0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
       HavriliakNegamiSusceptibility s = new HavriliakNegamiSusceptibility();
        s._isDielectricData = true;
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Calc.Regression.Nonlinear.HavriliakNegamiComplex", 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException();
        /*
        HavriliakNegamiComplex s = (HavriliakNegamiComplex)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("NegImSign", s._negativeImaginarySign);
        info.AddValue("ExcludeConductivity", s._excludeConductivity);
        */
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        HavriliakNegamiSusceptibility s = o != null ? (HavriliakNegamiSusceptibility)o : new HavriliakNegamiSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        info.GetBoolean("NegImSign");
        s._useFlowTerm = !info.GetBoolean("ExcludeConductivity");
        s._isDielectricData = true;
        return s;
      }
    }

     [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiSusceptibility),2)]
     class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        HavriliakNegamiSusceptibility s = (HavriliakNegamiSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        HavriliakNegamiSusceptibility s = o != null ? (HavriliakNegamiSusceptibility)o : new HavriliakNegamiSusceptibility();
         s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    #endregion

    public HavriliakNegamiSusceptibility()
    {
      
    }

    public override string ToString()
    {
      return "HavriliakNegami Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omeg)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description("FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;XML.MML.HavriliakNegamiSusceptibility.Dielectrics;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics")]
    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      HavriliakNegamiSusceptibility result = new HavriliakNegamiSusceptibility();
      result._useFrequencyInsteadOmega = false;
      result._isDielectricData = true;
      result._useFlowTerm = true;
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description("FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;XML.MML.HavriliakNegamiSusceptibility.Dielectrics;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics")]
    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      HavriliakNegamiSusceptibility result = new HavriliakNegamiSusceptibility();
      result._useFrequencyInsteadOmega = true;
      result._isDielectricData = true;
      result._useFlowTerm = true;
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omeg)", "Retardation/General", 1, 2, 5)]
    [Description("FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;XML.MML.HavriliakNegamiSusceptibility.General;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General")]
    public static IFitFunction CreateGeneralFunctionOfOmega()
    {
      HavriliakNegamiSusceptibility result = new HavriliakNegamiSusceptibility();
      result._useFrequencyInsteadOmega = false;
      result._isDielectricData = false;
      result._useFlowTerm = true;
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Retardation/General", 1, 2, 5)]
    [Description("FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;XML.MML.HavriliakNegamiSusceptibility.General;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      HavriliakNegamiSusceptibility result = new HavriliakNegamiSusceptibility();
      result._useFrequencyInsteadOmega = true;
      result._isDielectricData = false;
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
    private string[] _dependentVariableNameS = new string[]{"chi'","chi''"};
    private string[] _dependentVariableNameD = new string[] { "eps'", "eps''" };
    public int NumberOfDependentVariables
    {
      get
      {
        return _dependentVariableNameS.Length;
      }
    }
    public string DependentVariableName(int i)
    {
      return _isDielectricData ? _dependentVariableNameD[i] : _dependentVariableNameS[i];
    }
    #endregion

    #region parameter definition
    string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "alpha", "gamma", "conductivity" };
    string[] _parameterNameS = new string[] { "j_inf", "delta_j", "tau", "alpha", "gamma", "viscosity" };
    public int NumberOfParameters
    {
      get
      {
        return this._useFlowTerm ? _parameterNameS.Length : _parameterNameS.Length-1;
      }
    }
    public string ParameterName(int i)
    {
      return _isDielectricData ? _parameterNameD[i] : _parameterNameS[i];
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
        case 4:
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

      Complex result = P[0] + P[1]/ComplexMath.Pow(1+ComplexMath.Pow(Complex.I*x*P[2],P[3]),P[4]);
      Y[0] = result.Re;

      if (this._useFlowTerm)
      {
        if(this._isDielectricData)
          Y[1] = -result.Im + P[5] / (x * 8.854187817e-12);
        else
          Y[1] = -result.Im + P[5] / (x);
      }
      else
        Y[1] = -result.Im;  
    }

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      DY[0][0] = 1;
      DY[1][0] = 0;

      Complex OneByDenom = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex.I * x * P[2], P[3]), P[4]);
      DY[0][1] = OneByDenom.Re;
      DY[1][1] = -OneByDenom.Im;
      Complex IXP2 = Complex.I * x * P[2];
      Complex IXP2PowP3 = ComplexMath.Pow(IXP2, P[3]);
      Complex der2 = OneByDenom * -P[1] * P[2] * P[4] * IXP2PowP3 / (P[2] * (1 + IXP2PowP3));
      DY[0][2] = der2.Re;
      DY[1][2] = -der2.Im;
      Complex der3 = OneByDenom * -P[1] * P[4] * IXP2PowP3 * ComplexMath.Log(IXP2) / (1 + IXP2PowP3);
      DY[0][3] = der3.Re;
      DY[1][3] = -der3.Im;
      Complex der4 = OneByDenom * -P[1] * ComplexMath.Log(1 + IXP2PowP3);
      DY[0][4] = der4.Re;
      DY[1][4] = -der4.Im;

      if (_useFlowTerm)
      {
        
        DY[0][5] = 0;
        DY[1][5] = _isDielectricData ?  1 / (x * 8.854187817e-12) : 1/x;
      }
    }
    #endregion
  }

}
