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
  /// Implements the mass uptake of a sample in dependence of the water activity according to the Iglesias-Chirife model.
  /// </summary>
  /// <remarks>
  /// References:
  /// [1] Iglesias, H.A.; Chirife, J. . (1978). An Empirical Equation for Fitting Water Sorption Isotherms of Fruits and Related Products. Canadian Institute of Food Science and Technology Journal, 11(1), 12-15, doi:10.1016/S0315-5463(78)73153-6 
  /// [2] R. Andrade, R. Lemus M., C.E.Perez, "Models of sorption isotherms for food: uses and limitations", Vitae, Revista de la Facultad de Quimica Farmaceutica, 18 (2011), pp. 325-334
  /// </remarks>
  [FitFunctionClass]
  public record IglesiasChirifeModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-11-07 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IglesiasChirifeModel), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IglesiasChirifeModel)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new IglesiasChirifeModel();
      }
    }

    #endregion Serialization


    /// <summary>
    /// Creates the default Iglesias-Chirife model fit function.
    /// </summary>
    /// <returns>A new instance of <see cref="IglesiasChirifeModel"/>.</returns>
    [FitFunctionCreator("Mass uptake Iglesias-Chirife model", "Chemistry/SorptionIsotherms", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Chemistry.SorptionIsotherms.IglesiasChirifeModel}")]
    public static IFitFunction Create()
    {
      return new IglesiasChirifeModel();
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
        1 => "p",
        2 => "b",
        3 => "M05",
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 0,
        1 => 0.1,
        2 => 4,
        3 => 0.01,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Evaluates the mass uptake according to the Iglesias-Chirife model.
    /// </summary>
    /// <param name="x">The dependent variable. Usually water activity / relative humidity (0..1).</param>
    /// <param name="offset">The mass of the sample at x==0.</param>
    /// <param name="p">Absorption constant.</param>
    /// <param name="b">Absorption constant.</param>
    /// <param name="M05">Mass uptake at an water activity of 0.5 (although questionable, even in terms of unit).</param>
    /// <returns>The mass of the sample at x.</returns>
    public static double Evaluate(double x, double offset, double p, double b, double M05)
    {
      return offset + 0.5 * (Math.Exp(p + b * x) - M05 * Math.Exp(-p - b * x));
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
        double p = parameters[1];
        double b = parameters[2];
        double M05 = parameters[3];


        DF[i, 0] = 1; // Derivative with respect to offset
        DF[i, 1] = 0.5 * (Math.Exp(p + b * x) + M05 * Math.Exp(-p - b * x)); // w.r.t. p
        DF[i, 2] = 0.5 * x * Math.Exp(-p - b * x) * (Math.Exp(2 * (p + b * x)) + M05);
        DF[i, 3] = -0.5 * Math.Exp(-p - b * x);
      }
    }
  }
}
