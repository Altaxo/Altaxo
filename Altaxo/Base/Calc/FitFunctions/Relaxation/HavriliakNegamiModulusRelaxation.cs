#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiModulusRelaxation : IFitFunction
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _logarithmizeResults;
    private bool _invertViscosity = true;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiModulusRelaxation", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HavriliakNegamiModulusRelaxation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HavriliakNegamiModulusRelaxation?)o ?? new HavriliakNegamiModulusRelaxation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

    /// <summary>
    /// 2021-07-15 InvertViscosity added
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiModulusRelaxation), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HavriliakNegamiModulusRelaxation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("InvertViscosity", s._invertViscosity);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HavriliakNegamiModulusRelaxation?)o ?? new HavriliakNegamiModulusRelaxation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

    #endregion Serialization

    public HavriliakNegamiModulusRelaxation()
    {
    }

    #region Properties

    public HavriliakNegamiModulusRelaxation(bool useFrequencyInsteadOfOmega, bool useFlowTerm, bool invertViscosity, bool logarithmizeResult)
    {
      _useFrequencyInsteadOmega = useFrequencyInsteadOfOmega;
      _useFlowTerm = useFlowTerm;
      _invertViscosity = invertViscosity;
      _logarithmizeResults = logarithmizeResult;
    }


    /// <summary>
    /// Gets a value indicating whether to use the frequency instead of omega.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the independent variable is the frequency; false if the independent variable is the circular frequency.
    /// </value>
    public bool UseFrequencyInsteadOfOmega => _useFrequencyInsteadOmega;
    public HavriliakNegamiModulusRelaxation WithUseFrequencyInsteadOfOmega(bool value)
    {
      if (!(_useFrequencyInsteadOmega == value))
      {
        var result = (HavriliakNegamiModulusRelaxation)this.MemberwiseClone();
        result._useFrequencyInsteadOmega = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets a value indicating whether to use a flow term.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a flow term is included; otherwise, <c>false</c>.
    /// </value>
    public bool UseFlowTerm => _useFlowTerm;

    /// <summary>
    /// Sets a value indicating whether to use a flow term.
    /// </summary>
    /// <param name="value"><c>true</c> if a flow term is included; otherwise, <c>false</c>.</param>
    /// <returns>New instance with the parameter set accordingly.</returns>
    public HavriliakNegamiModulusRelaxation WithUseFlowTerm(bool value)
    {
      if (!(_useFlowTerm == value))
      {
        var result = (HavriliakNegamiModulusRelaxation)this.MemberwiseClone();
        result._useFlowTerm = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets a value indicating whether to invert the viscosity (then a general fluidity is used as parameter).
    /// </summary>
    /// <value>
    ///   <c>true</c> if a fluidity is used instead of viscosity; otherwise, <c>false</c>.
    /// </value>
    public bool InvertViscosity => _invertViscosity;

    /// <summary>
    /// Sets a value indicating whether to invert the viscosity (then a general fluidity is used as parameter).
    /// </summary>
    /// <param name="value"><c>true</c> if a fluidity is used instead of viscosity; otherwise, <c>false</c>.</param>
    /// <returns>New instance with the parameter set accordingly.</returns>
    public HavriliakNegamiModulusRelaxation WithInvertViscosity(bool value)
    {
      if (!(InvertViscosity == value))
      {
        var result = (HavriliakNegamiModulusRelaxation)this.MemberwiseClone();
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

    /// <summary>
    /// Sets a value indicating whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <param name="value"><c>true</c> if the real and imaginary part of the dependent variable should be logarithmized; otherwise, <c>false</c>.</param>
    /// <returns>New instance with the parameter set accordingly.</returns>
    public HavriliakNegamiModulusRelaxation WithLogarithmizeResults(bool value)
    {
      if (!(LogarithmizeResults == value))
      {
        var result = (HavriliakNegamiModulusRelaxation)this.MemberwiseClone();
        result._logarithmizeResults = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion


    public override string ToString()
    {
      if (_logarithmizeResults)
        return "Lg10 HavriliakNegami Complex " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
      else
        return "HavriliakNegami Complex " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omega)", "Relaxation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Relaxation.Modulus.HavriliakNegamiModulusRelaxationOmega}")]
    public static IFitFunction CreateModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = false,
      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Omega)", "Relaxation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Relaxation.Modulus.Lg10HavriliakNegamiModulusRelaxationOmega}")]
    public static IFitFunction CreateLg10ModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = true
      };

      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Frequency)", "Relaxation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Relaxation.Modulus.HavriliakNegamiModulusRelaxationFrequency}")]
    public static IFitFunction CreateModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = false,
      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Frequency)", "Relaxation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Relaxation.Modulus.Lg10HavriliakNegamiModulusRelaxationFrequency}")]
    public static IFitFunction CreateLg10ModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _invertViscosity = false,
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

    public int NumberOfDependentVariables
    {
      get
      {
        return 2;
      }
    }

    public string DependentVariableName(int i)
    {
      if (_logarithmizeResults)
        return i == 0 ? "lg10 M'" : "lg10 M''";
      else
        return i == 0 ? "M'" : "M''";
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
        2 => "tau_relax",
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
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      var result = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex64T.ImaginaryOne * x * P[2], P[3]), P[4]);
      result = P[1] + (P[0] - P[1]) * result;

      if (_useFlowTerm)
      {
        if (_invertViscosity)
          result = 1 / ((1 / result) - Complex64T.ImaginaryOne * P[5] / x);
        else
          result = 1 / ((1 / result) - Complex64T.ImaginaryOne / (x * P[5]));
      }

      if (_logarithmizeResults)
      {
        Y[0] = Math.Log10(result.Real);
        Y[1] = Math.Log10(result.Imaginary);
      }
      else
      {
        Y[0] = result.Real;
        Y[1] = result.Imaginary;
      }
    }

    public void EvaluateMultiple(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      var rowCount = independent.RowCount;
      int rd = 0;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        if (_useFrequencyInsteadOmega)
          x *= (2 * Math.PI);

        var result = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex64T.ImaginaryOne * x * P[2], P[3]), P[4]);
        result = P[1] + (P[0] - P[1]) * result;

        if (_useFlowTerm)
        {
          if (_invertViscosity)
            result = 1 / ((1 / result) - Complex64T.ImaginaryOne * P[5] / x);
          else
            result = 1 / ((1 / result) - Complex64T.ImaginaryOne / (x * P[5]));
        }

        double yre, yim;
        if (_logarithmizeResults)
        {
          yre = Math.Log10(result.Real);
          yim = Math.Log10(result.Imaginary);
        }
        else
        {
          yre = result.Real;
          yim = result.Imaginary;
        }

        if (independentVariableChoice is null)
        {
          FV[rd++] = yre;
          FV[rd++] = yim;
        }
        else
        {
          if (independentVariableChoice[0] == true)
          {
            FV[rd++] = yre;
          }

          if (independentVariableChoice[1] == true)
          {
            FV[rd++] = yim;
          }
        }
      }
    }

    #endregion IFitFunction Members
  }
}
