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

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  /// <summary>
  /// Yeoh model for uniaxial loading.
  /// </summary>
  /// <remarks>
  /// The model evaluates the engineering stress as a function of engineering strain using the three material parameters <c>C10</c>, <c>C20</c> and <c>C30</c>.
  /// <para>Reference: [1] O. H. Yeoh, „Some Forms of the Strain Energy Function for Rubber“, Rubber Chemistry and Technology, Bd. 66, Nr. 5, S. 754–771, Nov. 1993, doi: 10.5254/1.3538343.</para>
  /// </remarks>
  [FitFunctionClass]
  public record YeohUniaxial : IFitFunctionWithDerivative
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the cross-sectional area of the sample.
    /// </summary>
    public double CrossSectionArea { get; init; } = 1;

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 3;

    #region Serialization

    /// <summary>
    /// V0: 2026-03-19 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YeohUniaxial), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YeohUniaxial)o;
        info.AddValue(nameof(CrossSectionArea), s.CrossSectionArea);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble(nameof(CrossSectionArea));
        return new YeohUniaxial() { CrossSectionArea = crossSectionArea };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="YeohUniaxial"/> instance.</returns>
    [FitFunctionCreator("Yeoh model (uniaxial loading)", "RubberElasticity", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.YeohUniaxial}")]
    public static IFitFunction Create()
    {
      return new YeohUniaxial();
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
        0 => "C10",
        1 => "C20",
        2 => "C30",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 2E6,
        1 => 5E6,
        2 => 2E6,
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
      };
    }

    /// <summary>
    /// Evaluates the Yeoh uniaxial model for the specified strain and parameters.
    /// </summary>
    /// <param name="epsilon">Engineering strain.</param>
    /// <param name="C10">First order Mooney-Rivlin parameter of I1.</param>
    /// <param name="C20">Second order Mooney-Rivlin parameter of I1.</param>
    /// <param name="C30">Third order Mooney-Rivlin parameter of I1.</param>
    /// <returns>The engineering stress predicted by the model.</returns>
    public static double Evaluate(double epsilon, double C10, double C20, double C30)
    {
      var lambda = 1 + epsilon;
      var lambda3 = lambda * lambda * lambda;
      return (2 * (lambda3 - 1) * (3 * C30 * RMath.Pow4(epsilon) * RMath.Pow2(2 + lambda) + lambda * (C10 * lambda + 2 * C20 * (2 - 3 * lambda + lambda3)))) / (lambda3 * lambda);
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] dependent)
    {
      dependent[0] = CrossSectionArea * Evaluate(independent[0], parameters[0], parameters[1], parameters[2]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> dependent, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        dependent[i] = CrossSectionArea * Evaluate(independent[i, 0], parameters[0], parameters[1], parameters[2]);
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        var epsilon = independent[i, 0];
        var lambda = 1 + epsilon;
        var lambda2 = lambda * lambda;
        var lambda3 = lambda2 * lambda;
        DF[i, 0] = CrossSectionArea * 2 * (lambda - 1 / lambda2);
        DF[i, 1] = CrossSectionArea * 4 * RMath.Pow3(epsilon) * (lambda + 2) * (1 + lambda + lambda2) / (lambda3);
        DF[i, 2] = CrossSectionArea * 6 * RMath.Pow5(epsilon) * RMath.Pow2(lambda + 2) * (1 + lambda + lambda2) / (lambda3 * lambda);
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return ([0, null, null], null);
    }
  }
}
