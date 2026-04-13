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
  /// Gent model for planar loading.
  /// </summary>
  /// <remarks>
  /// The model evaluates the engineering stress as a function of engineering strain using the two material parameters <c>G</c> and <c>Jm</c>.
  /// <para>References:</para>
  /// <para>[1] A. N. Gent, „A New Constitutive Relation for Rubber“, Rubber Chemistry and Technology, Bd. 69, Nr. 1, S. 59–61, März 1996, doi: 10.5254/1.3538357.</para>
  /// </remarks>
  [FitFunctionClass]
  public record GentPlanar : IFitFunctionWithDerivative
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
    public int NumberOfParameters => 2;

    #region Serialization

    /// <summary>
    /// V0: 2026-03-19 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GentPlanar), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GentPlanar)o;
        info.AddValue(nameof(CrossSectionArea), s.CrossSectionArea);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble(nameof(CrossSectionArea));
        return new GentPlanar() { CrossSectionArea = crossSectionArea };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="GentPlanar"/> instance.</returns>
    [FitFunctionCreator("Gent (planar loading)", "RubberElasticity", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.GentPlanar}")]
    public static IFitFunction Create()
    {
      return new GentPlanar();
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
        0 => "G",
        1 => "Jm",
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
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
      };
    }

    /// <summary>
    /// Evaluates the Gent model (planar loading) for the specified strain and parameters.
    /// </summary>
    /// <param name="epsilon">Engineering strain.</param>
    /// <param name="G">Shear modulus.</param>
    /// <param name="Jm">Limiting strain.</param>
    /// <returns>The engineering stress predicted by the model.</returns>
    public static double Evaluate(double epsilon, double G, double Jm)
    {
      var lambda = 1 + epsilon;
      var lambda3 = lambda * lambda * lambda;
      var lambda4 = lambda3 * lambda;
      var lambda5 = lambda4 * lambda;
      return G * Jm * (lambda4 - 1) / (lambda3 * (Jm + 2) - lambda - lambda5);
    }


    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] dependent)
    {
      dependent[0] = CrossSectionArea * Evaluate(independent[0], parameters[0], parameters[1]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> dependent, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        dependent[i] = CrossSectionArea * Evaluate(independent[i, 0], parameters[0], parameters[1]);
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        var lambda = 1 + independent[i, 0];
        var lambda2 = lambda * lambda;
        var lambda4 = lambda2 * lambda2;

        var G = parameters[0];
        var Jm = parameters[1];

        DF[i, 0] = CrossSectionArea * Jm * (lambda4 - 1) / (lambda2 * lambda * (Jm + 2) - lambda - lambda4 * lambda);
        DF[i, 1] = CrossSectionArea * G * RMath.Pow3(1 - lambda2) * (1 + lambda2) / (lambda * RMath.Pow2(1 - (Jm + 2) * lambda2 + lambda4));
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
      return (new double?[] { 0, 0 }, null);
    }
  }
}
