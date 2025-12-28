using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Viscosity
{
  /// <summary>
  /// Implements the power law model for the dependence of the viscosity on the shear rate.
  /// </summary>
  /// <remarks>
  /// <para>Ref [1]: Markus Reiner et al., "Viskosimetrische Untersuchungen an Lösungen hochmolekularer Naturstoffe", Kolloid Zeitschrift (1933) 65 (1) 44-62</para>
  /// </remarks>
  [FitFunctionClass]
  public record PowerLawModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PowerLawModel), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PowerLawModel)obj;
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PowerLawModel();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of <see cref="PowerLawModel"/>.
    /// </summary>
    /// <returns>A new <see cref="IFitFunction"/> implementing the power law model.</returns>
    [FitFunctionCreator("Power law model", "Viscosity", 1, 1, 2)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Viscosity.PowerLawModel}")]
    public static IFitFunction Create()
    {
      return new PowerLawModel();
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
        0 => "K",
        1 => "n",
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1,
        1 => 0.5,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public static double Evaluate(double gammadot, double K, double n)
    {
      return K * Math.Pow(gammadot, n - 1);
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
      return ([0, null], [null, 1]);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        double gammadot = independent[i, 0];
        double K = parameters[0];
        double n = parameters[1];
        DF[i, 0] = Math.Pow(gammadot, n - 1);
        DF[i, 1] = Math.Pow(gammadot, n - 1) * K * Math.Log(gammadot);
      }
    }
  }
}
