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

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiComplex : IFitFunctionWithGradient
  {
    bool _useFrequencyInsteadOmega;
    bool _negativeImaginarySign;
    bool _excludeConductivity;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiComplex),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return new HavriliakNegamiComplex();;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiComplex), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        HavriliakNegamiComplex s = (HavriliakNegamiComplex)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("NegImSign", s._negativeImaginarySign);
        info.AddValue("ExcludeConductivity", s._excludeConductivity);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        HavriliakNegamiComplex s = o != null ? (HavriliakNegamiComplex)o : new HavriliakNegamiComplex();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._negativeImaginarySign = info.GetBoolean("NegImSign");
        s._excludeConductivity = info.GetBoolean("ExcludeConductivity");
        return s;
      }
    }

    #endregion

    public HavriliakNegamiComplex()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omeg)", "Relaxation", 1, 2, 5)]
    public static IFitFunction CreateFofOmega()
    {
      HavriliakNegamiComplex result =  new HavriliakNegamiComplex();
      result._useFrequencyInsteadOmega = false;
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Relaxation", 1, 2, 5)]
    public static IFitFunction CreateFofFrequency()
    {
      HavriliakNegamiComplex result =  new HavriliakNegamiComplex();
      result._useFrequencyInsteadOmega = true;
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
    private string[] _dependentVariableName = new string[]{"re","im"};
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
    string[] _parameterName = new string[] { "eps_inf", "delta_eps", "tau", "alpha", "gamma", "conductivity" };
    public int NumberOfParameters
    {
      get
      {
        return this._excludeConductivity ? _parameterName.Length-1 : _parameterName.Length;
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
    #endregion

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      Complex result = P[0] + P[1]/ComplexMath.Pow(1+ComplexMath.Pow(Complex.I*x*P[2],P[3]),P[4]);
      Y[0] = result.Re;
      
      if(this._excludeConductivity)
        Y[1] = -result.Im;
      else
        Y[1] = -result.Im + P[5] / (x * 8.854187817e-12);


      if (this._negativeImaginarySign)
        Y[1] = -Y[1];

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

      if (!_excludeConductivity)
      {
        DY[0][5] = 0;
        DY[1][5] = 1 / (x * 8.854187817e-12);
      }

      if (_negativeImaginarySign)
      {
        int len = this.NumberOfParameters;
        for (int i = 0; i < len; ++i)
          DY[1][i] = -DY[1][i];
      }




    }
    #endregion
  }

}
