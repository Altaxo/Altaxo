using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  public interface IObjectiveModelEvaluation
  {
    public IObjectiveModel CreateNew();

    /// <summary>
    /// Get the y-values of the observations.
    /// </summary>
    public Vector<double> ObservedY { get; }

    /// <summary>
    /// Get the values of the weights for the observations.
    /// </summary>
    public Matrix<double> Weights { get; }

    /// <summary>
    /// Get the y-values of the fitted model that correspond to the independent values.
    /// </summary>
    public Vector<double> ModelValues { get; }

    /// <summary>
    /// Get the values of the parameters.
    /// </summary>
    public Vector<double> Point { get; }

    /// <summary>
    /// Get the residual sum of squares.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Get the Gradient vector. G = J'(y - f(x; p))
    /// </summary>
    public Vector<double> Gradient { get; }

    /// <summary>
    /// Get the approximated Hessian matrix. H = J'J
    /// </summary>
    public Matrix<double> Hessian { get; }

    /// <summary>
    /// Get the number of calls to function.
    /// </summary>
    public int FunctionEvaluations { get; set; }

    /// <summary>
    /// Get the number of calls to jacobian.
    /// </summary>
    public int JacobianEvaluations { get; set; }

    /// <summary>
    /// Get the degree of freedom.
    /// </summary>
    public int DegreeOfFreedom { get; }

    public bool IsGradientSupported { get; }
    public bool IsHessianSupported { get; }
  }

  public interface IObjectiveModel : IObjectiveModelEvaluation
  {
    public void SetParameters(Vector<double> initialGuess, List<bool>? isFixed = null);

    public void EvaluateAt(Vector<double> parameters);

    public IObjectiveModel Fork();

    public IObjectiveFunction ToObjectiveFunction();
  }
}
