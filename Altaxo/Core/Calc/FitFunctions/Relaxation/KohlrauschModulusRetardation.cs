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
  /// Kohlrausch function in the frequency domain to fit modulus spectra. This is the inverse of the retardation spectra,
  /// i.e. tau is the retardation time (and not the relaxation time!).
  /// </summary>
  [FitFunctionClass]
  public class KohlrauschModulusRetardation : IFitFunction
  {
    private bool _useFrequencyInsteadOmega;
    private bool _useFlowTerm;
    private bool _logarithmizeResults;
    private bool _invertViscosity = true;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschModulusRetardation", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschModulusRetardation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        KohlrauschModulusRetardation s = o is not null ? (KohlrauschModulusRetardation)o : new KohlrauschModulusRetardation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschModulusRetardation", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschModulusRetardation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
        //info.AddValue("IsDielectric", s._isDielectricData);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        KohlrauschModulusRetardation s = o is not null ? (KohlrauschModulusRetardation)o : new KohlrauschModulusRetardation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");
        //s._isDielectricData = info.GetBoolean("IsDielectric");
        return s;
      }
    }

    /// <summary>
    /// 2021-07-15 added property InvertViscosity
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Relaxation.KohlrauschModulusRetardation", 2)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschModulusRetardation), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KohlrauschModulusRetardation)obj;
        info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
        info.AddValue("FlowTerm", s._useFlowTerm);
        info.AddValue("LogarithmizeResults", s._logarithmizeResults);
        info.AddValue("InvertViscosity", s._invertViscosity);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        KohlrauschModulusRetardation s = o is not null ? (KohlrauschModulusRetardation)o : new KohlrauschModulusRetardation();
        s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
        s._useFlowTerm = info.GetBoolean("FlowTerm");
        s._logarithmizeResults = info.GetBoolean("LogarithmizeResults");
        s._invertViscosity = info.GetBoolean("InvertViscosity");
        return s;
      }
    }

    #endregion Serialization

    public KohlrauschModulusRetardation()
    {
    }

    /// <summary>
    /// Gets a value indicating whether to use the frequency instead of omega.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the independent variable is the frequency; false if the independent variable is the circular frequency.
    /// </value>
    public bool UseFrequencyInsteadOfOmega => _useFrequencyInsteadOmega;
    public KohlrauschModulusRetardation WithUseFrequencyInsteadOfOmega(bool value)
    {
      if (!(_useFrequencyInsteadOmega == value))
      {
        var result = (KohlrauschModulusRetardation)this.MemberwiseClone();
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
    public KohlrauschModulusRetardation WithUseFlowTerm(bool value)
    {
      if (!(_useFlowTerm == value))
      {
        var result = (KohlrauschModulusRetardation)this.MemberwiseClone();
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
    public KohlrauschModulusRetardation WithInvertViscosity(bool value)
    {
      if (!(InvertViscosity == value))
      {
        var result = (KohlrauschModulusRetardation)this.MemberwiseClone();
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
    public KohlrauschModulusRetardation WithLogarithmizeResults(bool value)
    {
      if (!(LogarithmizeResults == value))
      {
        var result = (KohlrauschModulusRetardation)this.MemberwiseClone();
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
      return "Kohlrausch Modulus Complex " + (_useFrequencyInsteadOmega ? "(Frequency)" : "(Omega)");
    }




    [FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.KohlrauschComplexOmega}")]
    public static IFitFunction CreateModulusOfOmega()
    {
      var result = new KohlrauschModulusRetardation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = false,
      };

      return result;
    }

    [FitFunctionCreator("Lg10 Kohlrausch Complex (Omega)", "Retardation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.Lg10KohlrauschComplexOmega}")]

    public static IFitFunction CreateLg10ModulusOfOmega()
    {
      var result = new KohlrauschModulusRetardation
      {
        _useFrequencyInsteadOmega = false,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = true,
      };

      return result;
    }

    [FitFunctionCreator("Kohlrausch Complex (Frequency)", "Retardation/Modulus", 1, 2, 5)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.KohlrauschComplexFrequency}")]
    public static IFitFunction CreateModulusOfFrequency()
    {
      var result = new KohlrauschModulusRetardation
      {
        _useFrequencyInsteadOmega = true,
        _useFlowTerm = true,
        _invertViscosity = false,
        _logarithmizeResults = false,
      };

      return result;
    }

    [FitFunctionCreator("Lg10 Kohlrausch Complex (Frequency)", "Retardation/Modulus", 1, 2, 4)]
    [Description("${res:Altaxo.Calc.FitFunctions.Retardation.Modulus.Lg10KohlrauschComplexFrequency}")]
    public static IFitFunction CreateLg10ModulusOfFrequency()
    {
      var result = new KohlrauschModulusRetardation
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
      return i == 0 ? "M'" : "M''";
    }

    #endregion dependent variable definition

    #region parameter definition

    public int NumberOfParameters
    {
      get
      {
        return _useFlowTerm ? 5 : 4;
      }
    }

    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "m_0",
        1 => "m_inf",
        2 => "tau_retard",
        3 => "beta",
        4 => _invertViscosity ? "sigma" : "eta",
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
        4 => 1E33,
        _ => throw new NotImplementedException()
      };
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

      double w_r = x * P[2]; // omega scaled with tau

      Complex64T result = 1 / P[1] + (1 / P[0] - 1 / P[1]) * Kohlrausch.ReIm(P[3], w_r);

      if (_useFlowTerm)
      {
        if (_invertViscosity)
          result = new Complex64T(result.Real, result.Imaginary - P[4] / (x));
        else
          result = new Complex64T(result.Real, result.Imaginary - 1 / (x * P[4]));
      }

      result = 1 / result;

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
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      int rd = 0;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        if (_useFrequencyInsteadOmega)
          x *= (2 * Math.PI);

        double w_r = x * P[2]; // omega scaled with tau

        Complex64T result = 1 / P[1] + (1 / P[0] - 1 / P[1]) * Kohlrausch.ReIm(P[3], w_r);

        if (_useFlowTerm)
        {
          if (_invertViscosity)
            result = new Complex64T(result.Real, result.Imaginary - P[4] / (x));
          else
            result = new Complex64T(result.Real, result.Imaginary - 1 / (x * P[4]));
        }

        result = 1 / result;

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

        if (dependentVariableChoice is null)
        {
          FV[rd++] = yre;
          FV[rd++] = yim;
        }
        else
        {
          if (dependentVariableChoice[0] == true)
          {
            FV[rd++] = yre;
          }

          if (dependentVariableChoice[1] == true)
          {
            FV[rd++] = yim;
          }
        }

      }
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
