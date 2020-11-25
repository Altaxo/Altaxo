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
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class HavriliakNegamiModulusRelaxation : IFitFunction
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _logarithmizeResults;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiModulusRelaxation), 0)]
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

    #endregion Serialization

    public HavriliakNegamiModulusRelaxation()
    {
    }

    public override string ToString()
    {
      return "HavriliakNegami Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omega)", "Relaxation/Modulus", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegami.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true
      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Omega)", "Relaxation/Modulus", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegami.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateLg10ModulusOfOmega()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _logarithmizeResults = true
      };

      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Relaxation/Modulus", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegami.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRelaxation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true
      };

      return result;
    }

    [FitFunctionCreator("Lg10 HavriliakNegami Complex (Freq)", "Relaxation/Modulus", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegami.Modulus.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.ModulusRelaxation.Part3")]
    public static IFitFunction CreateLg10ModulusOfFrequency()
    {
      var result = new HavriliakNegamiModulusRelaxation
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

    private string[] _dependentVariableNameS = new string[] { "chi'", "chi''" };

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

    private string[] _parameterNameS = new string[] { "m_0", "m_inf", "tau_relax", "alpha", "gamma", "invviscosity" };

    public int NumberOfParameters
    {
      get
      {
        return _useFlowTerm ? _parameterNameS.Length : _parameterNameS.Length - 1;
      }
    }

    public string ParameterName(int i)
    {
      return _parameterNameS[i];
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

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    #endregion parameter definition

    #region Change event

    /// <summary>
    /// Called when anything in this fit function has changed.
    /// </summary>
    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fired when the fit function changed.
    /// </summary>
    public event EventHandler? Changed;

    #endregion Change event

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      Complex result = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex.I * x * P[2], P[3]), P[4]);
      result = P[1] + (P[0] - P[1]) * result;

      if (_useFlowTerm)
      {
        result = 1 / ((1 / result) - Complex.I * P[5] / x);
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

    #endregion IFitFunction Members
  }
}
