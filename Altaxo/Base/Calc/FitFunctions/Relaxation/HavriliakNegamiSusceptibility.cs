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
  public class HavriliakNegamiSusceptibility : IFitFunctionWithGradient
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
        s.NumberOfRelaxations = info.GetInt32("NumberOfTerms");

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
        s.NumberOfRelaxations = info.GetInt32("NumberOfRelaxations");
        s._invertResult = info.GetBoolean("InvertResult");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");

        return s;
      }
    }

    #endregion Serialization

    public HavriliakNegamiSusceptibility()
    {
    }

    public bool UseFrequencyInsteadOmega
    {
      get { return _useFrequencyInsteadOmega; }
      set
      {
        var oldValue = _useFrequencyInsteadOmega;
        _useFrequencyInsteadOmega = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool UseFlowTerm
    {
      get { return _useFlowTerm; }
      set
      {
        var oldValue = _useFlowTerm;
        _useFlowTerm = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool IsDielectricData
    {
      get { return _isDielectricData; }
      set
      {
        var oldValue = _isDielectricData;
        _isDielectricData = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public bool InvertViscosity
    {
      get { return _invertViscosity; }
      set
      {
        var oldValue = _invertViscosity;
        _invertViscosity = value;
        if (oldValue != value)
          OnChanged();
      }
    }

    public int NumberOfRelaxations
    {
      get
      {
        return _numberOfTerms;
      }
      set
      {
        var oldValue = _numberOfTerms;
        value = Math.Max(value, 0);
        _numberOfTerms = value;

        if (oldValue != value)
        {
          OnChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the complex dependent variable (the output of the fit function) should be inverted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the result is inverted; otherwise, <c>false</c>.
    /// </value>
    public bool InvertResult
    {
      get
      {
        return _invertResult;
      }
      set
      {
        var oldValue = _invertResult;
        _invertResult = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    /// <summary>
    /// Indicates whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the result is logarithmized; otherwise, <c>false</c>.
    /// </value>
    public bool LogarithmizeResults
    {
      get
      {
        return _logarithmizeResults;
      }
      set
      {
        var oldValue = _logarithmizeResults;
        _logarithmizeResults = value;
        if (value != oldValue)
          OnChanged();
      }
    }

    public override string ToString()
    {
      return "HavriliakNegami Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omeg)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics.Quantities")]
    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = true,
        _useFlowTerm = true
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Dielectrics.Quantities")]
    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = true,
        _useFlowTerm = true
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Omeg)", "Retardation/General", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.Omega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General.Quantities")]
    public static IFitFunction CreateGeneralFunctionOfOmega()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = false,
        _useFlowTerm = true
      };
      return result;
    }

    [FitFunctionCreator("HavriliakNegami Complex (Freq)", "Retardation/General", 1, 2, 5)]
    [Description(
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.Introduction;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General.Formula;" +
      "Altaxo.Calc.FitFunctions.IndependentVariable.FrequencyAsOmega;" +
      "Altaxo.Calc.FitFunctions.Relaxation.HavriliakNegamiSusceptibility.General.Quantities")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      var result = new HavriliakNegamiSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = false,
        _useFlowTerm = true
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

    #endregion dependent variable definition

    #region parameter definition

    private string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "alpha", "gamma", "conductivity" };
    private string[] _parameterNameS = new string[] { "j_inf", "delta_j", "tau", "alpha", "gamma", "viscosity" };

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

      var idx = i % 4;
      var term = i / 4;
      if (term < NumberOfRelaxations)
        return namearr[idx + 1] + (term > 0 ? string.Format("_{0}", term + 1) : "");
      else
        return namearr[namearr.Length - 1];
    }

    public double DefaultParameterValue(int i)
    {
      if (i < (1 + 4 * _numberOfTerms))
        return 1;
      else
        return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

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

    #endregion parameter definition

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      Complex result = P[0];
      int i, j;
      for (i = 0, j = 1; i < _numberOfTerms; ++i, j += 4)
      {
        result += P[j] / ComplexMath.Pow(1 + ComplexMath.Pow(Complex.I * x * P[1 + j], P[2 + j]), P[3 + j]);
      }

      // note: because it is a susceptiblity, the imaginary part is still negative

      if (_useFlowTerm)
      {
        if (_isDielectricData)
          result.Im -= P[j] / (x * 8.854187817e-12);
        else if (_invertViscosity)
          result.Im -= P[j] / (x);
        else
          result.Im -= 1 / (P[j] * x);
      }

      if (_invertResult)
        result = 1 / result; // if we invert, i.e. we calculate the modulus, the imaginary part is now positive
      else
        result.Im = -result.Im; // else if we don't invert, i.e. we calculate susceptibility, we negate the imaginary part to make it positive

      if (_logarithmizeResults)
      {
        result.Re = Math.Log10(result.Re);
        result.Im = Math.Log10(result.Im);
      }

      Y[0] = result.Re;
      Y[1] = result.Im;
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
                DY[1][5] = _isDielectricData ? 1 / (x * 8.854187817e-12) : 1 / x;
            }
            */
    }

    #endregion IFitFunction Members
  }
}
