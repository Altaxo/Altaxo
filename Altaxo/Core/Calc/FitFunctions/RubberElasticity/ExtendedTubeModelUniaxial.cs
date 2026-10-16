#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  /// <summary>
  /// Extended Tube Model for uniaxial loading.
  /// </summary>
  /// <remarks>
  /// The model evaluates the engineering stress as a function of engineering strain using 4 parameters.
  /// <para>Reference: [1] M. Kaliske and G. Heinrich, “An Extended Tube-Model for Rubber Elasticity: Statistical-Mechanical Theory and Finite Element Implementation,” Rubber Chemistry and Technology, vol. 72, no. 4, pp. 602–632, Sep. 1999, doi: 10.5254/1.3538822.</para>
  /// </remarks>
  [FitFunctionClass]
  public record ExtendedTubeModelUniaxial : IFitFunctionWithDerivative, IImmutable
  {
    /// <summary>
    /// Gets the cross-sectional area of the sample.
    /// </summary>
    public double CrossSectionArea { get; init; } = 1;

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 4;

    #region Serialization

    /// <summary>
    /// V0: 2026-10-16 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExtendedTubeModelUniaxial), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExtendedTubeModelUniaxial)o;
        info.AddValue(nameof(CrossSectionArea), s.CrossSectionArea);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble(nameof(CrossSectionArea));
        return new ExtendedTubeModelUniaxial() { CrossSectionArea = crossSectionArea };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="ExtendedTubeModelUniaxial"/> instance.</returns>
    [FitFunctionCreator("Extended Tube Model (uniaxial loading)", "RubberElasticity", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.ExtendedTubeModelUniaxial}")]
    public static IFitFunction Create()
    {
      return new ExtendedTubeModelUniaxial();
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return i switch
      {
        0 => "EngineeringStrain",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Independent variable index {i} is out of range.")
      };
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return i switch
      {
        0 => "EngineeringStress",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Dependent variable index {i} is out of range.")
      };
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "Gc",
        1 => "δ",
        2 => "Ge",
        3 => "β",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1.0E6,
        1 => 0.5,
        2 => 1.0E6,
        3 => 0.5,
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
      };
    }

    /// <summary>
    /// Evaluates one term of the extended tube model with uniaxial loading for the specified strain and parameters.
    /// </summary>
    /// <param name="epsilon">Engineering strain.</param>
    /// <param name="Gc">The coefficient of the first term.</param>
    /// <param name="δ">The exponent of the first term.</param>
    /// <param name="Ge">The coefficient of the second term.</param>
    /// <param name="β">The exponent of the second term.</param>
    /// <returns>The engineering stress predicted by the model.</returns>
    public static double EvaluateOneTerm(double epsilon, double Gc, double δ, double Ge, double β)
    {
      var lambda = 1 + epsilon;
      var lambda3 = lambda * lambda * lambda;
      var δ2 = δ * δ;
      var GcTerm = (lambda3 - 1) * (lambda - lambda * 2 * δ2 + δ2 * δ2 * (2 - 3 * lambda + lambda3)) / (lambda * RMath.Pow2(lambda - δ2 * (2 - 3 * lambda + lambda3)));
      var GeTerm = β == 0 ? 3 * Math.Log(lambda) / lambda : 2 * Math.Pow(lambda, -1 - β) * (-1 + Math.Pow(lambda, 3 * β / 2)) / β;

      return Gc * GcTerm + Ge * GeTerm;
    }


    /// <inheritdoc/>
    public void Evaluate(ReadOnlySpan<double> independent, ReadOnlySpan<double> parameters, Span<double> dependent)
    {
      dependent[0] = CrossSectionArea * EvaluateOneTerm(independent[0], parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> dependent, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        dependent[i] = CrossSectionArea * EvaluateOneTerm(independent[i, 0], parameters[0], parameters[1], parameters[2], parameters[3]);
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var Gc = parameters[0];
      var δ = parameters[1];
      var Ge = parameters[2];
      var β = parameters[3];

      for (int i = 0; i < independent.RowCount; i++)
      {
        var epsilon = independent[i, 0];
        var lambda = 1 + epsilon;
        var lambda3 = lambda * lambda * lambda;
        var lambda4 = lambda3 * lambda;
        var lambda6 = lambda3 * lambda3;

        var δ2 = δ * δ;
        var GcTerm = (lambda3 - 1) * (lambda - lambda * 2 * δ2 + δ2 * δ2 * (2 - 3 * lambda + lambda3)) / (lambda * RMath.Pow2(lambda - δ2 * (2 - 3 * lambda + lambda3)));
        var GeTerm = β == 0 ? 3 * Math.Log(lambda) / lambda : 2 * Math.Pow(lambda, -1 - β) * (-1 + Math.Pow(lambda, 3 * β / 2)) / β;
        var δTerm = -Gc * 4 * δ * (lambda6 - 4 * lambda4 + lambda3 + 4 * lambda - 2) / RMath.Pow3(δ2 * ((2 - 3 * lambda + lambda3)) - lambda);
        var βTerm = β == 0 ?
                    -0.75 * Ge * RMath.Pow2(Math.Log(lambda)) / lambda :
                    Ge * Math.Pow(lambda, -1 - β) * (2 + 2 * β * Math.Log(lambda) + Math.Pow(lambda, 3 * β / 2) * (-2 + β * Math.Log(lambda))) / (β * β);

        DF[i, 0] = CrossSectionArea * GcTerm;
        DF[i, 1] = CrossSectionArea * δTerm;
        DF[i, 2] = CrossSectionArea * GeTerm;
        DF[i, 3] = CrossSectionArea * βTerm;
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return ([0, 0, 0, null], [null, 1, null, null]);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }
  }
}
