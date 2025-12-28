#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.FitFunctions.Chemistry.SorptionIsotherms
{
  /// <summary>
  /// Implements the mass uptake of a sample in dependence of the water activity according to the Guggenheimer-Anderson-de Boer (GAB) model.
  /// </summary>
  /// <remarks>
  /// References:
  /// [1] R.B. Anderson, "Modification of the Brunauer, Emmett and Teller equation", Journal of the American Chemical Society, 68 (1946), pp. 686–691
  /// [2] R.B. Anderson, W.K. Hall, "Modification of the Brunauer, Emmett and Teller equation II.", Journal of the American Chemical Society, 70 (1948), pp. 1727–1734
  /// [3] R. Andrade, R. Lemus M., C.E.Perez, "Models of sorption isotherms for food: uses and limitations", Vitae, Revista de la Facultad de Quimica Farmaceutica, 18 (2011), pp. 325-334
  /// </remarks>
  [FitFunctionClass]
  public record GuggenheimAndersonDeBoerModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-11-07 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GuggenheimAndersonDeBoerModel), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GuggenheimAndersonDeBoerModel)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new GuggenheimAndersonDeBoerModel();
      }
    }

    #endregion Serialization


    /// <summary>
    /// Creates the default GAB (Guggenheim-Anderson-de Boer) model fit function.
    /// </summary>
    /// <returns>A new instance of <see cref="GuggenheimAndersonDeBoerModel"/>.</returns>
    [FitFunctionCreator("Mass uptake GAB model", "Chemistry/SorptionIsotherms", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Chemistry.SorptionIsotherms.GuggenheimAndersonDeBoerModel}")]
    public static IFitFunction Create()
    {
      return new GuggenheimAndersonDeBoerModel();
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 4;



    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "x";
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "m";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "offset",
        1 => "M0",
        2 => "C",
        3 => "K",
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 0,
        1 => 1,
        2 => 10,
        3 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Evaluates the mass uptake according to the Guggenheimer-Anderson-de Boer (GAB) model.
    /// </summary>
    /// <param name="x">The dependent variable. Usually water activity / relative humidity (0..1).</param>
    /// <param name="offset">The mass of the sample at x==0.</param>
    /// <param name="M0">The monolayer moisture content.</param>
    /// <param name="C">Absorption constant.</param>
    /// <param name="K">Absorption constant.</param>
    /// <returns>The mass of the sample at x.</returns>
    public static double Evaluate(double x, double offset, double M0, double C, double K)
    {
      return offset + M0 * K * C * x / ((1 - K * x) * (1 + (C - 1) * K * x));
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], parameters[0], parameters[1], parameters[2], parameters[3]);
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

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        double x = independent[i, 0];
        double M0 = parameters[1];
        double C = parameters[2];
        double K = parameters[3];
        double Kx = K * x;
        double OneMinusKx = 1 - Kx;


        DF[i, 0] = 1; // Derivative with respect to offset
        DF[i, 1] = C * Kx / (OneMinusKx * (1 + (C - 1) * Kx));
        DF[i, 2] = M0 * Kx / RMath.Pow2(1 + (C - 1) * Kx);
        DF[i, 3] = C * M0 * (x + (C - 1) * Kx * Kx * x) / (RMath.Pow2(1 - Kx) * RMath.Pow2(1 + (C - 1) * Kx));
      }
    }
  }
}
