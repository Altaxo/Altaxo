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
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiSusceptibility : IFitFunction, Main.IImmutable
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _isDielectricData;
    private int _numberOfTerms = 1;
    private bool _invertViscosity = true;
    private bool _invertResult;
    private bool _logarithmizeResults;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.HavriliakNegamiComplex", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException();
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = new HavriliakNegamiSusceptibility
        {
          _isDielectricData = true
        };
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.HavriliakNegamiComplex", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        HavriliakNegamiSusceptibility s = (HavriliakNegamiSusceptibility?)o ?? new HavriliakNegamiSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        info.GetBoolean("NegImSign");
        s._useFlowTerm = !info.GetBoolean("ExcludeConductivity");
        s._isDielectricData = true;
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiSusceptibility), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Trying to serialize old version");
        /*
                HavriliakNegamiSusceptibility s = (HavriliakNegamiSusceptibility)obj;
                info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
                info.AddValue("FlowTerm", s._useFlowTerm);
                info.AddValue("IsDielectric", s._isDielectricData);
                */
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        HavriliakNegamiSusceptibility s = (HavriliakNegamiSusceptibility?)o ?? new HavriliakNegamiSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiSusceptibility), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HavriliakNegamiSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
        info.AddValue("NumberOfTerms", s._numberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HavriliakNegamiSusceptibility?)o ?? new HavriliakNegamiSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        s._numberOfTerms = info.GetInt32("NumberOfTerms");

        return s;
      }
    }

    /// <summary>
    /// Extended 2013-02-07 by InvertViscosity, InvertResult and LogarithmizeResults
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiSusceptibility), 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HavriliakNegamiSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
        info.AddValue("InvertViscosity", s._invertViscosity);
        info.AddValue("NumberOfRelaxations", s._numberOfTerms);
        info.AddValue("InvertResult", s._invertResult);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HavriliakNegamiSusceptibility?)o ?? new HavriliakNegamiSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        s._numberOfTerms = info.GetInt32("NumberOfRelaxations");
        s._invertResult = info.GetBoolean("InvertResult");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

    #endregion Serialization

    public HavriliakNegamiSusceptibility()
    {
    }

    /// <summary>
    /// Gets a value indicating whether to use the frequency instead of omega.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the independent variable is the frequency; false if the independent variable is the circular frequency.
    /// </value>
    public bool UseFrequencyInsteadOfOmega => _useFrequencyInsteadOmega;
    public HavriliakNegamiSusceptibility WithUseFrequencyInsteadOfOmega(bool value)
    {
      if(!(_useFrequencyInsteadOmega == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._useFrequencyInsteadOmega = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool UseFlowTerm => _useFlowTerm;
    public HavriliakNegamiSusceptibility WithUseFlowTerm(bool value)
    {
      if (!(_useFlowTerm == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._useFlowTerm = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool IsRelativeDielectricPermittivity => _isDielectricData;
    public HavriliakNegamiSusceptibility WithIsRelativeDielectricPermittivity(bool value)
    {
      if (!(IsRelativeDielectricPermittivity == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._isDielectricData = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool InvertViscosity => _invertViscosity;
    public HavriliakNegamiSusceptibility WithInvertViscosity(bool value)
    {
      if (!(InvertViscosity == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._invertViscosity = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public int NumberOfTerms => _numberOfTerms;
    public HavriliakNegamiSusceptibility WithNumberOfTerms(int value)
    {
      if (value < 1)
        throw new ArgumentOutOfRangeException("Must be greater than or equal to 1", nameof(value));

      if (!(NumberOfTerms == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._numberOfTerms = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the complex dependent variable (the output of the fit function) should be inverted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is inverted; otherwise, <c>false</c>.
    /// </value>
    public bool InvertResult => _invertResult;
    public HavriliakNegamiSusceptibility WithInvertResult(bool value)
    {
      if (!(InvertResult == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._invertResult = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Indicates whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the result is logarithmized; otherwise, <c>false</c>.
    /// </value>
    public bool LogarithmizeResults => _logarithmizeResults;
    public HavriliakNegamiSusceptibility WithLogarithmizeResults(bool value)
    {
      if (!(LogarithmizeResults == value))
      {
        var result = (HavriliakNegamiSusceptibility)this.MemberwiseClone();
        result._logarithmizeResults = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public override string ToString()
    {
      if(_isDielectricData)
        return "HavriliakNegami RelativePermittivity " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
      else
       return "HavriliakNegami Susceptibility " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omega)", "Retardation/Dielectrics", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Dielectrics.HavriliakNegamiComplexOmega}")]
    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = true,
        _useFlowTerm = true,
        _invertViscosity = true
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Frequency)", "Retardation/Dielectrics", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Dielectrics.HavriliakNegamiComplexFrequency}")]
    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = true,
        _useFlowTerm = true,
        _invertViscosity = true
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex64T (Omega)", "Retardation/General", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.General.HavriliakNegamiComplexOmega}")]
    public static IFitFunction CreateGeneralFunctionOfOmega()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Frequency)", "Retardation/General", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.General.HavriliakNegamiComplexFrequency}")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false,
      };
      return result;
    }


    [FitFunctionCreator("HavriliakNegami Complex Multi (Omega)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.HavriliakNegamiMultiComplexOmega}")]
    public static IFitFunction CreateModulusFunctionOfOmega()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _invertResult = true,
        _numberOfTerms = 2
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex Multi (Frequency)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.HavriliakNegamiMultiComplexFrequency}")]
    public static IFitFunction CreateModulusFunctionOfFrequency()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _invertResult = true,
        _numberOfTerms =2
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

    public int NumberOfDependentVariables
    {
      get
      {
        return 2;
      }
    }

    public string DependentVariableName(int i)
    {
      var result = (_isDielectricData, _invertResult) switch
      {
        (false, false) => i == 0 ? "chi'" : "chi''",
        (false, true) => i == 0 ? "M'" : "M''",
        (true, false) => i == 0 ? "epsR'" : "epsR''",
        (true, true) => i == 0 ? "Md'" : "Md''",
      };

      return _logarithmizeResults ? "Lg " + result : result;
    }

    #endregion dependent variable definition

    #region parameter definition

    private string[] _parameterNameD = new string[] { "epsR_inf", "delta_epsR", "tau", "alpha", "gamma" };
    private string[] _parameterNameS = new string[] { "chi_inf", "delta_chi", "tau", "alpha", "gamma" };

    public int NumberOfParameters
    {
      get
      {
        var result = 1 + 4 * _numberOfTerms;
        if (_useFlowTerm)
          result += 1;
        return result;
      }
    }

    public string ParameterName(int i)
    {
      var namearr = _isDielectricData ? _parameterNameD : _parameterNameS;

      if (0 == i)
        return namearr[0]; // eps_inf

      --i;
      if (i < 4 * NumberOfTerms)
      {
        var idx = i % 4;
        var term = i / 4;
        return namearr[idx + 1] + (term > 0 ? string.Format("_{0}", term) : "");
      }

      // flow term

      if (_isDielectricData)
        return _invertViscosity ? "sigmaDC" : "rhoDC";
      else
        return _invertViscosity ? "sigma" : "eta";
    }

    public double DefaultParameterValue(int i)
    {
      if (i < (1 + 4 * _numberOfTerms))
        return 1;
      else
      {
        return _invertViscosity ? 0 : 1E33;
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }


    #endregion parameter definition

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
      {
        x *= (2 * Math.PI);
      }

      Complex64T result = P[0];
      int i, j;
      for (i = 0, j = 1; i < _numberOfTerms; ++i, j += 4)
      {
        result += P[j] / ComplexMath.Pow(1 + ComplexMath.Pow(Complex64T.ImaginaryOne * x * P[1 + j], P[2 + j]), P[3 + j]);
      }

      // note: because it is a susceptiblity, the imaginary part is still negative


      if (_useFlowTerm)
      {
        if (_isDielectricData)
        {
          if (_invertViscosity)
            result = new Complex64T(result.Real, result.Imaginary - P[j] / (x * 8.854187817e-12));
          else
            result = new Complex64T(result.Real, result.Imaginary - 1 / (P[j] * x * 8.854187817e-12));
        }
        else
        {
        if (_invertViscosity)
            result = new Complex64T(result.Real, result.Imaginary - P[j] / (x));
          else
            result = new Complex64T(result.Real, result.Imaginary - 1 / (P[j] * x));
        }
      }

      if (_invertResult)
      {
        var inv = 1 / result; // if we invert, i.e. we calculate the modulus, the imaginary part is now positive
      }
      else
      {
        result = new Complex64T(result.Real, -result.Imaginary); // else if we don't invert, i.e. we calculate susceptibility, we negate the imaginary part to make it positive
      }

      if (_logarithmizeResults)
      {
        result = new Complex64T(Math.Log10(result.Real), Math.Log10(result.Imaginary));
      }

      Y[0] = result.Real;
      Y[1] = result.Imaginary;
    }

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      throw new NotImplementedException();
      /*
            double x = X[0];
            if (_useFrequencyInsteadOmega)
                x *= (2 * Math.PI);

            DY[0][0] = 1;
            DY[1][0] = 0;

            Complex64T OneByDenom = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex64T.I * x * P[2], P[3]), P[4]);
            DY[0][1] = OneByDenom.Re;
            DY[1][1] = -OneByDenom.Im;
            Complex64T IXP2 = Complex64T.I * x * P[2];
            Complex64T IXP2PowP3 = ComplexMath.Pow(IXP2, P[3]);
            Complex64T der2 = OneByDenom * -P[1] * P[2] * P[4] * IXP2PowP3 / (P[2] * (1 + IXP2PowP3));
            DY[0][2] = der2.Re;
            DY[1][2] = -der2.Im;
            Complex64T der3 = OneByDenom * -P[1] * P[4] * IXP2PowP3 * ComplexMath.Log(IXP2) / (1 + IXP2PowP3);
            DY[0][3] = der3.Re;
            DY[1][3] = -der3.Im;
            Complex64T der4 = OneByDenom * -P[1] * ComplexMath.Log(1 + IXP2PowP3);
            DY[0][4] = der4.Re;
            DY[1][4] = -der4.Im;

            if (_useFlowTerm)
            {
                DY[0][5] = 0;
                DY[1][5] = _isDielectricData ? 1 / (x * 8.854187817e-12) : 1 / x;
            }
            */
    }

    #endregion IFitFunction Members
  }
}
