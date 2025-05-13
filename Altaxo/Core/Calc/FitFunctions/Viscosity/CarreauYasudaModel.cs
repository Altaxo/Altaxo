using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Viscosity
{
  /// <summary>
  /// Implements the Carreau-Yasuda model for the dependence of the viscosity on the shear rate.
  /// </summary>
  /// <remarks>
  /// <para>Ref [1]: Yasuda K, Armstrong RC, Cohen RE (1981) Shear flow properties of
  /// concentrated solutions of linear and star branched polystyrenes. Rheol Acta 20:163–178</para>
  /// </remarks>
  [FitFunctionClass]
  public record CarreauYasudaModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CarreauYasudaModel), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CarreauYasudaModel)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CarreauYasudaModel();
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Carreau-Yasuda model", "Viscosity", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Viscosity.CarreauYasudaModel}")]
    public static IFitFunction Create()
    {
      return new CarreauYasudaModel();
    }


    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 5;



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
        0 => "η0",
        1 => "η∞",
        2 => "λ",
        3 => "a",
        4 => "n",
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
        2 => 1,
        3 => 1,
        4 => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public static double Evaluate(double gammadot, double eta0, double etaInf, double lambda, double a, double n)
    {
      return etaInf + (eta0 - etaInf) * Math.Pow(1 + Math.Pow(lambda * gammadot, a), (n - 1) / a);
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return ([null, null, double.Epsilon, double.Epsilon, null], null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return ([double.Epsilon, double.Epsilon, double.Epsilon, double.Epsilon, null], null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        var gammadot = independent[i, 0];
        var eta0 = parameters[0];
        var etaInf = parameters[1];
        var lambda = parameters[2];
        var a = parameters[3];
        var n = parameters[4];
        var termP0 = Math.Pow(lambda * gammadot, a);
        var termP1 = 1 + termP0;
        var termP2 = Math.Pow(termP1, (n - 1) / a);
        DF[i, 0] = termP2;
        DF[i, 1] = 1 - termP2;
        DF[i, 2] = ((eta0 - etaInf) * (n - 1) / lambda) * termP0 * termP2 / termP1;
        DF[i, 3] = (eta0 - etaInf) * (n - 1) * (termP0 * Math.Log(termP0) - termP1 * Math.Log(termP1)) * termP2 / (a * a * termP1);
        DF[i, 4] = (eta0 - etaInf) * Math.Log(termP1) * termP2 / a;
      }
    }
  }
}
