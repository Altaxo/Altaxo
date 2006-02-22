#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Summary description for KohlrauschDecay.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschDecay : IFitFunction
  {
    
    public KohlrauschDecay()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public override string ToString()
    {
      return "KohlrauschDecay";
    }


    [FitFunctionCreator("KohlrauschDecay", "Relaxation", 1, 1, 4)]
    [System.ComponentModel.Description("FitFunctions.Relaxation.Kohlrausch.Decay")]
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
        return 4;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return (new string[]{"offset","amplitude","tau","beta"})[i];
    }

    public double DefaultParameterValue(int i)
    {
      switch (i)
      {
        case 0:
          return 0;
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

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1]*Math.Exp(-Math.Pow(X[0]/P[2],P[3]));
    }

    #endregion

    #region IFitFunction Members


   
    #endregion
  }



  /// <summary>
  /// Kohlrausch function in the frequency domain to fit compliance or dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschComplianceFrequencyDomain : IFitFunction
  {
    bool _useFrequencyInsteadOmega;
    bool _useFlowTerm;
    bool _isDielectricData;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschComplianceFrequencyDomain), 0)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        KohlrauschComplianceFrequencyDomain s = (KohlrauschComplianceFrequencyDomain)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschComplianceFrequencyDomain s = o != null ? (KohlrauschComplianceFrequencyDomain)o : new KohlrauschComplianceFrequencyDomain();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    #endregion

    public KohlrauschComplianceFrequencyDomain()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public override string ToString()
    {
      return "Kohlrausch Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Dielectrics", 1, 2, 4)]
    public static IFitFunction CreateFofOmega()
    {
      KohlrauschComplianceFrequencyDomain result = new KohlrauschComplianceFrequencyDomain();
      result._useFrequencyInsteadOmega = false;
      result._useFlowTerm = true;
      result._isDielectricData = true;
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Dielectrics", 1, 2, 4)]
    public static IFitFunction CreateFofFrequency()
    {
      KohlrauschComplianceFrequencyDomain result = new KohlrauschComplianceFrequencyDomain();
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
    string[] _parameterNameC = new string[] { "j_inf", "delta_j", "tau", "beta", "viscosity" };
    string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "beta", "conductivity" };
    public int NumberOfParameters
    {
      get
      {
        if (_isDielectricData)
         return this._useFlowTerm ?  _parameterNameD.Length : _parameterNameD.Length - 1 ;
        else
         return this._useFlowTerm ?  _parameterNameC.Length : _parameterNameC.Length - 1 ;
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
          Y[1] = -result.Im - P[4] / (x * 8.854187817e-12);
        else
          Y[1] = -result.Im - P[4] / (x);
      }
      else
      {
        Y[1] = -result.Im;
      }
    }

    #endregion
  }



  /// <summary>
  /// Kohlrausch function in the frequency domain to fit modulus spectra. This is the inverse of the retardation spectra,
  /// i.e. tau is the retardation time (and not the relaxation time!).
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschModulusFrequencyDomain : IFitFunction
  {
    bool _useFrequencyInsteadOmega;
    bool _useFlowTerm;
   

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschModulusFrequencyDomain), 0)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        KohlrauschModulusFrequencyDomain s = (KohlrauschModulusFrequencyDomain)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschModulusFrequencyDomain s = o != null ? (KohlrauschModulusFrequencyDomain)o : new KohlrauschModulusFrequencyDomain();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    #endregion

    public KohlrauschModulusFrequencyDomain()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public override string ToString()
    {
      return "Kohlrausch Modulus Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Modulus", 1, 2, 4)]
    public static IFitFunction CreateFofOmega()
    {
      KohlrauschModulusFrequencyDomain result = new KohlrauschModulusFrequencyDomain();
      result._useFrequencyInsteadOmega = false;
      result._useFlowTerm = true;
     
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Modulus", 1, 2, 4)]
    public static IFitFunction CreateFofFrequency()
    {
      KohlrauschModulusFrequencyDomain result = new KohlrauschModulusFrequencyDomain();
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

      Complex result = 1/P[1] + (1/P[0]-1/P[1]) * Kohlrausch.ReIm(P[3], w_r);

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


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class LinearFitWithGradient : IFitFunctionWithGradient
  {
    public LinearFitWithGradient()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("LinearFitWithGradient","Relaxation",1,1,2)]
    public static IFitFunction LinearFitWithGradientOrder2()
    {
      return new LinearFitWithGradient();
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
        return 2;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return (new string[]{"intercept","slope",})[i];
    }

    public double DefaultParameterValue(int i)
    {
      return 0;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1]*X[0];
    }

    #endregion

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      DY[0][0] = 1;
      DY[0][1] = X[0];
    }

  }


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class LinearFit : IFitFunction
  {
    public LinearFit()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("LinearFit", "Relaxation", 1, 1, 2)]
    public static IFitFunction LinearFitOrder2()
    {
      return new LinearFit();
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
        return 2;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return (new string[] { "intercept", "slope", })[i];
    }

    public double DefaultParameterValue(int i)
    {
      return 0;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1] * X[0];
    }

    #endregion




    #region IFitFunction Members


  

    #endregion

  
  }
}
