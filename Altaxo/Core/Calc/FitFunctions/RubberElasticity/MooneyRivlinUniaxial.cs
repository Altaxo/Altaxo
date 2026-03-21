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
  /// Mooney-Rivlin model for uniaxial loading.
  /// </summary>
  /// <remarks>
  /// The model evaluates the engineering stress as a function of engineering strain using the two material parameters <c>C10</c> and <c>C01</c>.
  /// <para>References:</para>
  /// <para>[1] M. Mooney, „A Theory of Large Elastic Deformation“, Journal of Applied Physics, Bd. 11, Nr. 9, S. 582–592, Sep. 1940, doi: 10.1063/1.1712836.</para>
  /// <para>[2] R. S. Rivlin, „Large elastic deformations of isotropic materials IV. further developments of the general theory“, Philosophical Transactions of the Royal Society of London. Series A, Mathematical and Physical Sciences, Bd. 241, Nr. 835, S. 379–397, Okt. 1948, doi: 10.1098/rsta.1948.0024.</para>
  /// <para>[3] M. C. Boyce und E. M. Arruda, „Constitutive Models of Rubber Elasticity: A Review“, Rubber Chemistry and Technology, Bd. 73, Nr. 3, S. 504–523, Juli 2000, doi: 10.5254/1.3547602.</para>
  /// </remarks>
  [FitFunctionClass]
  public record MooneyRivlinUniaxial : IFitFunctionWithDerivative
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MooneyRivlinUniaxial), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MooneyRivlinUniaxial)obj;
        info.AddValue(nameof(CrossSectionArea), s.CrossSectionArea);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble(nameof(CrossSectionArea));
        return new MooneyRivlinUniaxial() { CrossSectionArea = crossSectionArea };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="MooneyRivlinUniaxial"/> instance.</returns>
    [FitFunctionCreator("Mooney-Rivlin (uniaxial loading)", "RubberElasticity", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.MooneyRivlinUniaxial}")]
    public static IFitFunction Create()
    {
      return new MooneyRivlinUniaxial();
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
        1 => "C01",
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
    /// Evaluates the Mooney-Rivlin uniaxial model for the specified strain and parameters.
    /// </summary>
    /// <param name="epsilon">Engineering strain.</param>
    /// <param name="C10">First Mooney-Rivlin parameter.</param>
    /// <param name="C01">Second Mooney-Rivlin parameter.</param>
    /// <returns>The engineering stress predicted by the model.</returns>
    public static double Evaluate(double epsilon, double C10, double C01)
    {
      var lambda = 1 + epsilon;
      var lambda3 = lambda * lambda * lambda;
      return 2 * (C01 + C10 * lambda) * (lambda3 - 1) / lambda3;
    }


    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = CrossSectionArea * Evaluate(independent[0], parameters[0], parameters[1]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = CrossSectionArea * Evaluate(independent[i, 0], parameters[0], parameters[1]);
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
        DF[i, 0] = CrossSectionArea * (2 * lambda - 2 / lambda2);
        DF[i, 1] = CrossSectionArea * (2 - 2 / lambda3);
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
