using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Viscosity
{
  /// <summary>
  /// Implements the Herschel-Bulkley model for the dependence of the viscosity on the shear rate.
  /// </summary>
  /// <remarks>
  /// <para>Ref [1]: Herschel, W.H.; Bulkley, R. (1926), "Konsistenzmessungen von Gummi-Benzollösungen", Kolloid Zeitschrift, 39 (4): 291–300, doi:10.1007/BF01432034, S2CID 97549389</para>
  /// </remarks>
  [FitFunctionClass]
  public record HerschelBulkleyModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HerschelBulkleyModel), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HerschelBulkleyModel)obj;
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new HerschelBulkleyModel();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of <see cref="HerschelBulkleyModel"/>.
    /// </summary>
    /// <returns>A new <see cref="IFitFunction"/> implementing the Herschel-Bulkley model.</returns>
    [FitFunctionCreator("Herschel-Bulkley model", "Viscosity", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Viscosity.HerschelBulkleyModel}")]
    public static IFitFunction Create()
    {
      return new HerschelBulkleyModel();
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 3;



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
        1 => "K",
        2 => "n",
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
        2 => 0.5,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public static double Evaluate(double gammadot, double tau0, double K, double n)
    {
      return tau0 / gammadot + K * Math.Pow(gammadot, n - 1);
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], parameters[0], parameters[1], parameters[2]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], parameters[0], parameters[1], parameters[2]);
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
      return ([0, 0, null], [null, null, 1]);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        double gammadot = independent[i, 0];
        double tau0 = parameters[0];
        double K = parameters[1];
        double n = parameters[2];
        DF[i, 0] = 1 / gammadot;
        DF[i, 1] = Math.Pow(gammadot, n - 1);
        DF[i, 2] = Math.Pow(gammadot, n - 1) * K * Math.Log(gammadot);
      }
    }
  }
}
