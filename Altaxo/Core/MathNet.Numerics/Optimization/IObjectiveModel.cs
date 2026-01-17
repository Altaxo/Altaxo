using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  /// <summary>
  /// Provides read-only access to the evaluation results of an objective model.
  /// </summary>
  public interface IObjectiveModelEvaluation
  {
    /// <summary>
    /// Creates a new instance of the objective model with identical configuration but independent state.
    /// </summary>
    /// <returns>A new objective model instance.</returns>
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

    /// <summary>
    /// Gets a value indicating whether the gradient can be provided by the model.
    /// </summary>
    public bool IsGradientSupported { get; }

    /// <summary>
    /// Gets a value indicating whether the Hessian can be provided by the model.
    /// </summary>
    public bool IsHessianSupported { get; }
  }

  /// <summary>
  /// Defines an objective model that can be evaluated at parameter values and exposed as an objective function.
  /// </summary>
  public interface IObjectiveModel : IObjectiveModelEvaluation
  {
    /// <summary>
    /// Sets the model parameters and optional fixed flags for individual parameters.
    /// </summary>
    /// <param name="initialGuess">Initial parameter values.</param>
    /// <param name="isFixed">Optional list of flags indicating fixed parameters.</param>
    public void SetParameters(Vector<double> initialGuess, List<bool>? isFixed = null);

    /// <summary>
    /// Evaluates the model at the given parameter vector, updating dependent values.
    /// </summary>
    /// <param name="parameters">Parameter vector to evaluate.</param>
    public void EvaluateAt(Vector<double> parameters);

    /// <summary>
    /// Creates a forked copy of the model with independent mutable state.
    /// </summary>
    /// <returns>A new objective model instance.</returns>
    public IObjectiveModel Fork();

    /// <summary>
    /// Converts this model to an objective function suitable for minimizers.
    /// </summary>
    /// <returns>The objective function view of this model.</returns>
    public IObjectiveFunction ToObjectiveFunction();
  }
}
