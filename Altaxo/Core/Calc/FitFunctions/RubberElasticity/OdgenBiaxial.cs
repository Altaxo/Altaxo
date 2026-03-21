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
  /// Odgen model for biaxial loading.
  /// </summary>
  /// <remarks>
  /// The model can consist of multiple terms, each having a prefactor and an exponent.
  /// <para>Reference: [1] R. W. Ogden, „Large deformation isotropic elasticity – on the correlation of theory and experiment for incompressible rubberlike solids“, Proceedings of the Royal Society of London. A. Mathematical and Physical Sciences, Bd. 326, Nr. 1567, S. 565–584, Feb. 1972, doi: 10.1098/rspa.1972.0026.</para>
  /// </remarks>
  [FitFunctionClass]
  public record OdgenBiaxial : IFitFunctionWithDerivative
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    /// <summary>
    /// Gets the cross-sectional area of the sample.
    /// </summary>
    public double CrossSectionArea { get; init; } = 1;

    /// <summary>
    /// Gets the number of terms in the model (order of the model.
    /// </summary>
    public int NumberOfTerms
    {
      get;
      init
      {
        if (value < 1)
        {
          throw new ArgumentOutOfRangeException(nameof(value), $"Number of terms must be at least 1, but was {value}.");
        }
        field = value;
      }
    } = 1;

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 2 * NumberOfTerms;

    #region Serialization

    /// <summary>
    /// V0: 2026-03-19 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OdgenBiaxial), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OdgenBiaxial)obj;
        info.AddValue(nameof(CrossSectionArea), s.CrossSectionArea);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble(nameof(CrossSectionArea));
        return new OdgenBiaxial() { CrossSectionArea = crossSectionArea };
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="OdgenBiaxial"/> instance.</returns>
    [FitFunctionCreator("Odgen (biaxial loading)", "RubberElasticity", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.OdgenBiaxial}")]
    public static IFitFunction Create()
    {
      return new OdgenBiaxial();
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
      if (i >= 0 && i < NumberOfParameters)
      {
        return i % 2 == 0 ? $"µ{i / 2 + 1}" : $"α{i / 2 + 1}";
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
      ;
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      if (i >= 0 && i < NumberOfParameters)
      {
        return i % 2 == 0 ? 5e6 : 1.0;
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
      ;
    }

    /// <summary>
    /// Evaluates one term of the Odgen model with biaxial loading (pure shear) for the specified strain and parameters.
    /// </summary>
    /// <param name="epsilon">Engineering strain.</param>
    /// <param name="µ">The coefficent of the term.</param>
    /// <param name="α">The exponent of the term.</param>
    /// <returns>The engineering stress predicted by the model.</returns>
    public static double EvaluateOneTerm(double epsilon, double µ, double α)
    {
      var lambda = 1 + epsilon;
      return µ * (Math.Pow(lambda, α - 1) - Math.Pow(lambda, -1 - 2 * α));
    }


    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      var epsilon = independent[0];
      var sum = 0d;
      for (int i = 0; i < NumberOfTerms; i++)
      {
        var µ = parameters[2 * i];
        var α = parameters[2 * i + 1];
        sum += EvaluateOneTerm(epsilon, µ, α);
      }

      FV[0] = CrossSectionArea * sum;
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        var epsilon = independent[i, 0];
        var sum = 0d;
        for (int k = 0; k < NumberOfTerms; k++)
        {
          var µ = parameters[2 * k];
          var α = parameters[2 * k + 1];
          sum += EvaluateOneTerm(epsilon, µ, α);
        }
        FV[i] = CrossSectionArea * sum;
      }
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        var epsilon = independent[i, 0];
        var lambda = 1 + epsilon;
        for (int k = 0; k < NumberOfTerms; k++)
        {
          var µ = parameters[2 * k];
          var α = parameters[2 * k + 1];
          DF[i, 2 * k] = CrossSectionArea * (Math.Pow(lambda, 3 * α) - 1) * Math.Pow(lambda, -1 - 2 * α);
          DF[i, 2 * k + 1] = CrossSectionArea * µ * Math.Log(lambda) * (Math.Pow(lambda, -1 - 2 * α) * (2 + Math.Pow(lambda, 3 * α)));
        }
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
      return (null, null);
    }
  }
}
