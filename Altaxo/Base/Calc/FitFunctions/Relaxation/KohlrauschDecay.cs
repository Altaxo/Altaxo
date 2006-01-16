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

    [FitFunctionCreator("KohlrauschDecay", "Relaxation", 1, 1, 4)]
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

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1]*Math.Exp(-Math.Pow(X[0]/P[2],P[3]));
    }

    #endregion
  }



  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschFrequencyDomain : IFitFunction
  {
    bool _useFrequencyInsteadOmega;
    bool _modulus;
    bool _withConductivity;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschFrequencyDomain), 0)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        KohlrauschFrequencyDomain s = (KohlrauschFrequencyDomain)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("Modulus", s._modulus);
        info.AddValue("Conductivity", s._withConductivity);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        KohlrauschFrequencyDomain s = o != null ? (KohlrauschFrequencyDomain)o : new KohlrauschFrequencyDomain();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._modulus = info.GetBoolean("Modulus");
        s._withConductivity = info.GetBoolean("Conductivity");
        return s;
      }
    }

    #endregion

    public KohlrauschFrequencyDomain()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Relaxation", 1, 2, 4)]
    public static IFitFunction CreateFofOmega()
    {
      KohlrauschFrequencyDomain result = new KohlrauschFrequencyDomain();
      result._useFrequencyInsteadOmega = false;
      result._withConductivity = true;
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Freq)", "Relaxation", 1, 2, 4)]
    public static IFitFunction CreateFofFrequency()
    {
      KohlrauschFrequencyDomain result = new KohlrauschFrequencyDomain();
      result._useFrequencyInsteadOmega = true;
      result._withConductivity = true;
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
    string[] _parameterName = new string[] { "offset", "amplitude", "tau", "beta", "conductivity" };
    public int NumberOfParameters
    {
      get
      {
        return this._withConductivity ?  _parameterName.Length : _parameterName.Length - 1 ;
      }
    }
    public string ParameterName(int i)
    {
      return _parameterName[i];
    }
    #endregion

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      double w_r = x * P[2]; // omega scaled with tau

      Complex result = P[0] + P[1] * Complex.FromRealImaginary(Kohlrausch.Re(P[3], w_r), Kohlrausch.Im(P[3], w_r));
      Y[0] = result.Re;

      if (this._withConductivity)
        Y[1] = result.Im + P[4] / (x * 8.854187817e-12);
      else
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

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1] * X[0];
    }

    #endregion

   

  }
}
