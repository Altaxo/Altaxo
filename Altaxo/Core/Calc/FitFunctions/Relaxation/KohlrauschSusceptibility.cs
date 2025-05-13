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
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.FitFunctions.Relaxation
{
  /// <summary>
  /// Kohlrausch function in the frequency domain to fit compliance or dielectric spectra.
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschSusceptibility : IFitFunction, Main.IImmutable
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _isDielectricData;
    private bool _invertViscosity = true;
    private int _numberOfTerms = 1;
    private bool _invertResult;
    private bool _logarithmizeResults;
    private const int ParametersPerTerm = 3; // tau, beta, delta_chi or delta_epsR

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (KohlrauschSusceptibility?)o ?? new KohlrauschSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("IsDielectric", s._isDielectricData);
        info.AddValue("InvertViscosity", s._invertViscosity);
        info.AddValue("NumberOfRelaxations", s._numberOfTerms);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (KohlrauschSusceptibility?)o ?? new KohlrauschSusceptibility();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._isDielectricData = info.GetBoolean("IsDielectric");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        s._numberOfTerms = info.GetInt32("NumberOfRelaxations");
        return s;
      }
    }

    /// <summary>
    /// 2013-02-07 extended by InvertResult und LogarithmizeResults
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschSusceptibility", 2)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschSusceptibility)obj;
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
        var s = (KohlrauschSusceptibility?)o ?? new KohlrauschSusceptibility();
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

    /// <summary>
    /// Gets a value indicating whether to use the frequency instead of omega.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the independent variable is the frequency; false if the independent variable is the circular frequency.
    /// </value>
    public bool UseFrequencyInsteadOfOmega => _useFrequencyInsteadOmega;
    public KohlrauschSusceptibility WithUseFrequencyInsteadOfOmega(bool value)
    {
      if (!(_useFrequencyInsteadOmega == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
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
    public KohlrauschSusceptibility WithUseFlowTerm(bool value)
    {
      if (!(_useFlowTerm == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
        result._useFlowTerm = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public bool IsRelativeDielectricPermittivity => _isDielectricData;
    public KohlrauschSusceptibility WithIsRelativeDielectricPermittivity(bool value)
    {
      if (!(IsRelativeDielectricPermittivity == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
        result._isDielectricData = value;
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
    public KohlrauschSusceptibility WithInvertViscosity(bool value)
    {
      if (!(InvertViscosity == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
        result._invertViscosity = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public int NumberOfTerms => _numberOfTerms;
    public KohlrauschSusceptibility WithNumberOfTerms(int value)
    {
      if (value < 1)
        throw new ArgumentOutOfRangeException("Must be greater than or equal to 1", nameof(value));

      if (!(NumberOfTerms == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
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
    public KohlrauschSusceptibility WithInvertResult(bool value)
    {
      if (!(InvertResult == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
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

    /// <summary>
    /// Sets a value indicating whether the real and imaginary part of the dependent variable should be logarithmized (decadic logarithm).
    /// </summary>
    /// <param name="value"><c>true</c> if the real and imaginary part of the dependent variable should be logarithmized; otherwise, <c>false</c>.</param>
    /// <returns>New instance with the parameter set accordingly.</returns>
    public KohlrauschSusceptibility WithLogarithmizeResults(bool value)
    {
      if (!(LogarithmizeResults == value))
      {
        var result = (KohlrauschSusceptibility)this.MemberwiseClone();
        result._logarithmizeResults = value;
        return result;
      }
      else
      {
        return this;
      }
    }

    public KohlrauschSusceptibility()
    {
    }

    public override string ToString()
    {
      if (_isDielectricData)
        return "Kohlrausch RelativePermittivity " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
      else
        return "Kohlrausch Susceptibility " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/General", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.General.KohlrauschComplexOmega}")]
    public static IFitFunction CreateGeneralFunctionOfOmega()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Frequency)", "Retardation/General", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.General.KohlrauschComplexFrequency}")]
    public static IFitFunction CreateGeneralFunctionOfFrequency()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = false,
        _useFlowTerm = true,
        _invertViscosity = false,
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Dielectrics.KohlrauschComplexOmega}")]

    public static IFitFunction CreateDielectricFunctionOfOmega()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = false,
        _isDielectricData = true,
        _useFlowTerm = true,
        _invertViscosity = true
      };
      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Frequency)", "Retardation/Dielectrics", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Dielectrics.KohlrauschComplexFrequency}")]

    public static IFitFunction CreateDielectricFunctionOfFrequency()
    {
      var result = new KohlrauschSusceptibility
      {
        _useFrequencyInsteadOmega = true,
        _isDielectricData = true,
        _useFlowTerm = true,
        _invertViscosity = true
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

    private string[] _parameterNameD = new string[] { "epsR_inf", "delta_epsR", "tau", "beta", "conductivity", };
    private string[] _parameterNameS = new string[] { "chi_inf", "delta_chi", "tau", "beta", "invviscosity" };

    public int NumberOfParameters
    {
      get
      {
        if (_useFlowTerm)
          return 2 + ParametersPerTerm * _numberOfTerms;
        else
          return 1 + ParametersPerTerm * _numberOfTerms;
      }
    }

    public string ParameterName(int i)
    {
      var namearr = _isDielectricData ? _parameterNameD : _parameterNameS;

      if (0 == i)
      {
        return namearr[0]; // eps_inf
      }
      else if (i < 1 + ParametersPerTerm * _numberOfTerms)
      {
        --i;

        var idx = i % 3;
        var term = i / 3;
        return namearr[idx + 1] + (term > 0 ? string.Format("_{0}", term) : "");
      }
      else if (_useFlowTerm && i == 1 + ParametersPerTerm * _numberOfTerms)
      {
        // flow term
        if (_isDielectricData)
          return _invertViscosity ? "sigmaDC" : "rhoDC";
        else
          return _invertViscosity ? "sigma" : "eta";
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), i, "Parameter index out of range.");
      }
    }

    public double DefaultParameterValue(int i)
    {
      if (i < (1 + ParametersPerTerm * _numberOfTerms))
      {
        return 1;
      }
      else if (i == (1 + ParametersPerTerm * _numberOfTerms))
      {
        return _invertViscosity ? 0 : 1E33;
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), i, "Parameter index out of range.");
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    #endregion parameter definition

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double x = X[0];
      if (_useFrequencyInsteadOmega)
        x *= (2 * Math.PI);

      Complex64T result = P[0];

      int iPar, i;
      for (i = 0, iPar = 1; i < _numberOfTerms; i++, iPar += 3)
      {
        result += P[0 + iPar] * Kohlrausch.ReIm(P[2 + iPar], P[1 + iPar] * x);
      }

      // note: because it is a susceptiblity, the imaginary part is still negative

      if (_useFlowTerm)
      {
        if (_isDielectricData)
        {
          if (_invertViscosity)
            result = new Complex64T(result.Real, result.Imaginary - P[iPar] / (x * 8.854187817e-12));
          else
            result = new Complex64T(result.Real, result.Imaginary - 1 / (P[iPar] * x * 8.854187817e-12));
        }
        else
        {
          if (_invertViscosity)
            result = new Complex64T(result.Real, result.Imaginary - P[iPar] / (x));
          else
            result = new Complex64T(result.Real, result.Imaginary - 1 / (P[iPar] * x));
        }
      }


      if (_invertResult)
        result = 1 / result; // if we invert, i.e. we calculate the modulus, the imaginary part is now positive
      else
        result = new Complex64T(result.Real, -result.Imaginary); // else if we don't invert, i.e. we calculate susceptibility, we negate the imaginary part to make it positive

      if (_logarithmizeResults)
      {
        result = new Complex64T(Math.Log10(result.Real), Math.Log10(result.Imaginary));
      }

      Y[0] = result.Real;
      Y[1] = result.Imaginary;
    }
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      int rd = 0;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        if (_useFrequencyInsteadOmega)
          x *= (2 * Math.PI);

        Complex64T result = P[0];

        int iPar, i;
        for (i = 0, iPar = 1; i < _numberOfTerms; i++, iPar += 3)
        {
          result += P[0 + iPar] * Kohlrausch.ReIm(P[2 + iPar], P[1 + iPar] * x);
        }

        // note: because it is a susceptiblity, the imaginary part is still negative

        if (_useFlowTerm)
        {
          if (_isDielectricData)
          {
            if (_invertViscosity)
              result = new Complex64T(result.Real, result.Imaginary - P[iPar] / (x * 8.854187817e-12));
            else
              result = new Complex64T(result.Real, result.Imaginary - 1 / (P[iPar] * x * 8.854187817e-12));
          }
          else
          {
            if (_invertViscosity)
              result = new Complex64T(result.Real, result.Imaginary - P[iPar] / (x));
            else
              result = new Complex64T(result.Real, result.Imaginary - 1 / (P[iPar] * x));
          }
        }


        if (_invertResult)
          result = 1 / result; // if we invert, i.e. we calculate the modulus, the imaginary part is now positive
        else
          result = new Complex64T(result.Real, -result.Imaginary); // else if we don't invert, i.e. we calculate susceptibility, we negate the imaginary part to make it positive

        if (_logarithmizeResults)
        {
          result = new Complex64T(Math.Log10(result.Real), Math.Log10(result.Imaginary));
        }

        if (dependentVariableChoice is null)
        {
          FV[rd++] = result.Real;
          FV[rd++] = result.Imaginary;
        }
        else
        {
          if (dependentVariableChoice[0] == true)
          {
            FV[rd++] = result.Real;
          }

          if (dependentVariableChoice[1] == true)
          {
            FV[rd++] = result.Imaginary;
          }
        }
      }
    }
    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

  }
}
