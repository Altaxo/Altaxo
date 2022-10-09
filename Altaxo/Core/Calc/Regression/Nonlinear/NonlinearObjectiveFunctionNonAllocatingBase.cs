using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  public abstract class NonlinearObjectiveFunctionNonAllocatingBase : IObjectiveModel, IObjectiveModelNonAllocating
  {
    #region Private Variables

    protected readonly int _accuracyOrder; // the desired accuracy order to evaluate the jacobian by numerical approximaiton.
    protected Vector<double> _coefficients;
    protected bool _hasFunctionValue;
    protected double _functionValue; // the residual sum of squares, residuals * residuals.
    protected Vector<double> _residuals; // the weighted error values
    protected bool _hasJacobianValue;
    protected Matrix<double> _jacobianValue; // the Jacobian matrix.
    protected Matrix<double> _jacobianValueTransposed; // the Jacobian matrix, transposed.
    protected Vector<double> _negativeGradientValue; // the Gradient vector.
    protected Matrix<double> _hessianValue; // the Hessian matrix.
    protected Vector<double> _f1, _f2, _f3, _f4, _f5, _f6;

    #endregion Private Variables

    #region Public Variables

    /// <summary>
    /// Set or get the values of the weights for the observations.
    /// </summary>
    public Matrix<double> Weights { get; protected set; }

    protected Vector<double> L; // Weights = LL'

    /// <summary>
    /// Get whether parameters are fixed or free (by the user).
    /// </summary>
    public IReadOnlyList<bool> IsFixedByUser { get; protected set; }

    /// <summary>
    /// Array of the length <see cref="NumberOfParameters"/>. If an element is true, that parameter is either fixed by the user (see <see cref="IsFixed"/>), or
    /// is fixed because it has reached a boundary.
    /// This array will be updated only at the end of the minimization process.
    /// </summary>
    public IReadOnlyList<bool> IsFixedByUserOrBoundary { get; set; }

    /// <summary>
    /// Get the number of observations.
    /// </summary>
    public int NumberOfObservations => ObservedY?.Count ?? 0;

    /// <summary>
    /// Get the number of unknown parameters.
    /// </summary>
    public int NumberOfParameters => Point?.Count ?? 0;

    /// <summary>
    /// Set or get the values of the observations.
    /// </summary>
    public Vector<double> ObservedY { get; protected set; }

    /// <summary>
    /// Get the degree of freedom
    /// </summary>
    public int DegreeOfFreedom
    {
      get
      {
        var df = NumberOfObservations - NumberOfParameters;
        if (IsFixedByUserOrBoundary is not null)
        {
          df += IsFixedByUserOrBoundary.Count(p => p);
        }
        return df;
      }
    }

    /// <summary>
    /// Get the number of calls to function.
    /// </summary>
    public int FunctionEvaluations { get; set; }

    /// <summary>
    /// Get the number of calls to jacobian.
    /// </summary>
    public int JacobianEvaluations { get; set; }

    #endregion Public Variables

    public NonlinearObjectiveFunctionNonAllocatingBase(int accuracyOrder = 2)
    {
      _accuracyOrder = Math.Min(6, Math.Max(1, accuracyOrder));
    }

    public abstract IObjectiveModel Fork();

    public abstract IObjectiveModel CreateNew();

    /// <summary>
    /// Set or get the values of the parameters.
    /// </summary>
    public Vector<double> Point => _coefficients;

    /// <summary>
    /// Get the y-values of the fitted model that correspond to the independent values.
    /// </summary>
    public Vector<double> ModelValues { get; protected set; }

    /// <summary>
    /// Get the residual sum of squares.
    /// </summary>
    public double Value
    {
      get
      {
        if (!_hasFunctionValue)
        {
          EvaluateFunction();
          _hasFunctionValue = true;
        }
        return _functionValue;
      }
    }

    /// <summary>
    /// Gets Chi²/(N-F+1)
    /// </summary>
    public double SigmaSquare
    {
      get
      {
        if (!_hasFunctionValue)
        {
          EvaluateFunction();
          _hasFunctionValue = true;
        }
        return _functionValue / (DegreeOfFreedom + 1);
      }
    }

    /// <summary>
    /// Get the Gradient vector of x and p.
    /// </summary>
    public Vector<double> Gradient
    {
      get
      {
        if (!_hasJacobianValue)
        {
          EvaluateJacobian();
          _hasJacobianValue = true;
        }
        return -_negativeGradientValue;
      }
    }

    /// <summary>
    /// Get the negative gradient vector of x and p.
    /// </summary>
    public Vector<double> NegativeGradient
    {
      get
      {
        if (!_hasJacobianValue)
        {
          EvaluateJacobian();
          _hasJacobianValue = true;
        }
        return _negativeGradientValue;
      }
    }

    /// <summary>
    /// Get the Hessian matrix of x and p, J'WJ
    /// </summary>
    public Matrix<double> Hessian
    {
      get
      {
        if (!_hasJacobianValue)
        {
          EvaluateJacobian();
          _hasJacobianValue = true;
        }
        return _hessianValue;
      }
    }

    public bool IsGradientSupported => true;
    public bool IsHessianSupported => true;


    /// <summary>
    /// Set parameters and bounds.
    /// </summary>
    /// <param name="initialGuess">The initial values of parameters.</param>
    /// <param name="isFixed">The list to the parameters fix or free.</param>
    public void SetParameters(Vector<double> initialGuess, List<bool> isFixed = null)
    {
      SetParameters((IReadOnlyList<double>)initialGuess, isFixed);
    }

    /// <summary>
    /// Set parameters and bounds.
    /// </summary>
    /// <param name="initialGuess">The initial values of parameters.</param>
    /// <param name="isFixed">The list to the parameters fix or free.</param>
    public virtual void SetParameters(IReadOnlyList<double> initialGuess, IReadOnlyList<bool> isFixed = null)
    {
      _coefficients ??= Vector<double>.Build.Dense(initialGuess.Count);
      for (int i = 0; i < initialGuess.Count; ++i)
      {
        _coefficients[i] = initialGuess[i];
      }

      if (isFixed is not null && isFixed.Count != initialGuess.Count)
      {
        throw new ArgumentException("The isFixed can't have different size from the initial guess.");
      }
      if (isFixed is not null && isFixed.Count(p => p) == isFixed.Count)
      {
        throw new ArgumentException("All the parameters can't be fixed.");
      }
      IsFixedByUser = isFixed;
      IsFixedByUserOrBoundary = isFixed;

      // allocate already some
      _negativeGradientValue ??= Vector<double>.Build.Dense(initialGuess.Count);
      _hessianValue ??= Matrix<double>.Build.Dense(initialGuess.Count, initialGuess.Count);
      _jacobianValue ??= Matrix<double>.Build.Dense(NumberOfObservations, NumberOfParameters);
      _jacobianValueTransposed ??= Matrix<double>.Build.Dense(NumberOfParameters, NumberOfObservations);
    }

    public void EvaluateAt(Vector<double> parameters)
    {
      EvaluateAt((IReadOnlyList<double>)parameters);
    }

    public void EvaluateAt(IReadOnlyList<double> parameters)
    {
      if (parameters is null)
      {
        throw new ArgumentNullException(nameof(parameters));
      }
      if (parameters.Count(p => double.IsNaN(p) || double.IsInfinity(p)) > 0)
      {
        throw new ArgumentException("The parameters must be finite.");
      }

      for (int i = 0; i < parameters.Count; i++)
        _coefficients[i] = parameters[i];

      _hasFunctionValue = false;
      _hasJacobianValue = false;
    }

    public IObjectiveFunction ToObjectiveFunction()
    {
      (double, Vector<double>, Matrix<double>) Function(Vector<double> point)
      {
        EvaluateAt(point);
        return (Value, Gradient, Hessian);
      }

      var objective = new GradientHessianObjectiveFunction(Function);
      return objective;
    }

    #region Private Methods

    protected abstract void EvaluateFunction();

    protected abstract void EvaluateJacobian();

    protected abstract Matrix<double> NumericalJacobian(Vector<double> parameters, Vector<double> currentValues, int accuracyOrder = 2);

    #endregion Private Methods

  }
}
