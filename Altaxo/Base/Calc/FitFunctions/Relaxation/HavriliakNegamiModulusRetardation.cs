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

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Havriliak-Negami retardation function to fit modulus values. This function essentially implements the same as <see cref="HavriliakNegamiSusceptibility"/>,
  /// but the parametrization is different (modulus values instead of susceptibilities).
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiModulusRetardation : IFitFunction, Main.IImmutable
  {
    private bool _useFrequencyInsteadOfOmega;
    private bool _useFlowTerm;
    private bool _logarithmizeResults;
    private bool _invertViscosity = true;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.RelaxationHavriliakNegamiModulusRetardation", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HavriliakNegamiModulusRetardation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOfOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HavriliakNegamiModulusRetardation?)o ?? new HavriliakNegamiModulusRetardation();
        s._useFrequencyInsteadOfOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

      /// <summary>
      /// 2021-07-13 added property InvertViscosity
      /// </summary>
      /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiModulusRetardation), 1)]
      private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
      {
        public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
        {
          var s = (HavriliakNegamiModulusRetardation)obj;
          info.AddValue("UseFrequency", s._useFrequencyInsteadOfOmega);
          info.AddValue("FlowTerm", s._useFlowTerm);
          info.AddValue("LogarithmizeResults", s._logarithmizeResults);
          info.AddValue("InvertViscosity", s._invertViscosity);
        }

        public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
        {
          var s = (HavriliakNegamiModulusRetardation?)o ?? new HavriliakNegamiModulusRetardation();
          s._useFrequencyInsteadOfOmega = info.GetBoolean("UseFrequency");
          s._useFlowTerm = info.GetBoolean("FlowTerm");
          s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");
          s._invertViscosity = info.GetBoolean("InvertViscosity");

          return s;
        }
      }

    #endregion Serialization

    public HavriliakNegamiModulusRetardation()
    {
    }

    public bool UseFrequencyInsteadOfOmega => _useFrequencyInsteadOfOmega;
    public HavriliakNegamiModulusRetardation WithUseFrequencyInsteadOfOmega(bool value)
    {
      if (!(_useFrequencyInsteadOfOmega == value))
      {
        var result = (HavriliakNegamiModulusRetardation)this.MemberwiseClone();
        result._useFrequencyInsteadOfOmega = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool UseFlowTerm => _useFlowTerm;
    public HavriliakNegamiModulusRetardation WithUseFlowTerm(bool value)
    {
      if (!(_useFlowTerm == value))
      {
        var result = (HavriliakNegamiModulusRetardation)this.MemberwiseClone();
        result._useFlowTerm = value;
        return result;
      }
      else
      {
        return this;
      }
    }

   

    public bool InvertViscosity => _invertViscosity;
    public HavriliakNegamiModulusRetardation WithInvertViscosity(bool value)
    {
      if (!(InvertViscosity == value))
      {
        var result = (HavriliakNegamiModulusRetardation)this.MemberwiseClone();
        result._invertViscosity = value;
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
    public HavriliakNegamiModulusRetardation WithLogarithmizeResults(bool value)
    {
      if (!(LogarithmizeResults == value))
      {
        var result = (HavriliakNegamiModulusRetardation)this.MemberwiseClone();
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
      return "HavriliakNegami Modulus Complex " + (_useFrequencyInsteadOfOmega ? "(Frequency)" : "(Omega)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omega)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.HavriliakNegamiComplexOmega}")]
    public static IFitFunction CreateModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRetardation
      {
        _useFrequencyInsteadOfOmega = false,
        _useFlowTerm = true,
        _invertViscosity = false
      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Omega)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.Lg10HavriliakNegamiComplexOmega}")]
    public static IFitFunction CreateLg10ModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRetardation
      {
        _useFrequencyInsteadOfOmega = false,
        _useFlowTerm = true,
        _logarithmizeResults = true,
        _invertViscosity = false
      };

      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Frequency)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.HavriliakNegamiComplexFrequency}")]
    public static IFitFunction CreateModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRetardation
      {
        _useFrequencyInsteadOfOmega = true,
        _useFlowTerm = true,
        _invertViscosity = false

      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Frequency)", "Retardation/Modulus", 1, 2, 6)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.Lg10HavriliakNegamiComplexFrequency}")]
    public static IFitFunction CreateLg10ModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRetardation
      {
        _useFrequencyInsteadOfOmega = true,
        _useFlowTerm = true,
        _logarithmizeResults = true,
        _invertViscosity = false
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
      return _useFrequencyInsteadOfOmega ? "Frequency" : "Omega";
    }

    #endregion independent variable definition

    #region dependent variable definition

    private string[] _dependentVariableNameS = new string[] { "M'", "M''" };

    public int NumberOfDependentVariables
    {
      get
      {
        return _dependentVariableNameS.Length;
      }
    }

    public string DependentVariableName(int i)
    {
      return _dependentVariableNameS[i];
    }

    #endregion dependent variable definition

    #region parameter definition

    public int NumberOfParameters
    {
      get
      {
        return _useFlowTerm ? 6 : 5;
      }
    }

    public string ParameterName(int i)
    {

      return i switch
      {
        0 => "M_0",
        1 => "M_inf",
        2 => "tau_retard",
        3 => "alpha",
        4 => "gamma",
        5 => _invertViscosity ? "sigma" : "eta",
        _ => throw new NotImplementedException()
      };
    }

    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1E6,
        1 => 1E9,
        2 => 1,
        3 => 1,
        4 => 1,
        5 => _invertViscosity ? 0 : 1E33,
        _ => throw new NotImplementedException()
      };

    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    #endregion parameter definition

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOfOmega)
        x *= (2 * Math.PI);

      // Model this first as compliance
      Complex result = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex.I * x * P[2], P[3]), P[4]);
      result = (1 / P[1]) + ((1 / P[0]) - (1 / P[1])) * result;

      if (_useFlowTerm)
      {
        if(_invertViscosity)
          result -= Complex.I * P[5] / x;
        else
          result -= Complex.I  / (x * P[5]);
      }

      result = 1 / result; // but now convert to modulus

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

    #endregion IFitFunction Members
  }
}
