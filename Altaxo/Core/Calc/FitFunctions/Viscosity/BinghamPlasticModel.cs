using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Viscosity
{
  /// <summary>
  /// Implements the Bingham plastic model for the dependence of the viscosity on the shear rate.
  /// </summary>
  /// <remarks>
  /// <para>Ref [1]: Bingham, E.C. (1916). "An Investigation of the Laws of Plastic Flow". Bulletin of the Bureau of Standards. 13 (2): 309–353. doi:10.6028/bulletin.304. hdl:2027/mdp.39015086559054</para>
  /// </remarks>
  [FitFunctionClass]
  public record BinghamPlasticModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BinghamPlasticModel), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BinghamPlasticModel)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new BinghamPlasticModel();
      }
    }

    #endregion Serialization


    [FitFunctionCreator("Bingham plastic model", "Viscosity", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Viscosity.BinghamPlasticModel}")]
    public static IFitFunction Create()
    {
      return new BinghamPlasticModel();
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 2;



    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "̇γ";
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "η";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "τ0",
        1 => "ηp",
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1,
        1 => 10,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Evaluates the Bingham plastic model for a single shear rate.
    /// </summary>
    /// <param name="gammadot">Shear rate.</param>
    /// <param name="tau0">Yield stress parameter τ0.</param>
    /// <param name="etaP">Plastic viscosity ηp.</param>
    /// <returns>The viscosity η for the given shear rate.</returns>
    public static double Evaluate(double gammadot, double tau0, double etaP)
    {
      return tau0 / gammadot + etaP;
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], parameters[0], parameters[1]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], parameters[0], parameters[1]);
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
        double gammadot = independent[i, 0];
        DF[i, 0] = 1 / gammadot;
        DF[i, 1] = 1;
      }
    }
  }
}
