using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Viscosity
{
  /// <summary>
  /// Implements the Cross model for the dependence of the viscosity on the shear rate.
  /// </summary>
  /// <remarks>
  /// <para>Ref [1]: Cross MM (1965) Rheology of non-Newtonian fluids—a new flow equation for pseudoplastic systems. J Colloid Sci 20:417–437</para>
  /// <para>Ref [2]: Cross MM (1979) Relation between viscoelasticity and shear-thinning behaviour in liquids.Rheol Acta 18:609–614</para>
  /// </remarks>
  [FitFunctionClass]
  public record CrossModel : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-05-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossModel), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CrossModel)obj;
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new CrossModel();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance of <see cref="CrossModel"/>.
    /// </summary>
    /// <returns>A new <see cref="IFitFunction"/> implementing the Cross model.</returns>
    [FitFunctionCreator("Cross model", "Viscosity", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Viscosity.CrossModel}")]
    public static IFitFunction Create()
    {
      return new CrossModel();
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
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1000,
        1 => 1,
        2 => 1,
        3 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public static double Evaluate(double gammadot, double eta0, double etaInf, double lambda, double a)
    {
      return etaInf + (eta0 - etaInf) / (1 + Math.Pow(lambda * gammadot, a));
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
      return ([null, null, double.Epsilon, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return ([double.Epsilon, double.Epsilon, double.Epsilon, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        double gammadot = independent[i, 0];
        double eta0 = parameters[0];
        double etaInf = parameters[1];
        double lambda = parameters[2];
        double a = parameters[3];
        double denom = 1 + Math.Pow(lambda * gammadot, a);
        double denom2 = denom * denom;
        double dEta0 = 1 / denom;
        double dEtaInf = 1 - 1 / denom;
        double dLambda = -(eta0 - etaInf) * Math.Pow(lambda * gammadot, a) * a / (lambda * denom2);
        double da = -(eta0 - etaInf) * Math.Log(lambda * gammadot) * Math.Pow(lambda * gammadot, a) / denom2;
        DF[i, 0] = dEta0;
        DF[i, 1] = dEtaInf;
        DF[i, 2] = dLambda;
        DF[i, 3] = da;
      }
    }
  }
}
