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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  /// <summary>
  /// Base implementation for a nonlinear objective model that supports allocation-free evaluation
  /// for Levenberg-Marquardt-style algorithms.
  /// </summary>
  /// <remarks>
  /// This class provides cached values for the function value, Jacobian-derived quantities, and auxiliary vectors/matrices
  /// to minimize allocations during repeated evaluations.
  /// </remarks>
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
    /// Gets or sets the values of the weights for the observations.
    /// </summary>
    public Matrix<double>? Weights { get; protected set; }

    protected Vector<double>? L; // Weights = LL'

    /// <summary>
    /// Gets whether parameters are fixed or free (by the user).
    /// </summary>
    public IReadOnlyList<bool> IsFixedByUser { get; protected set; }

    /// <inheritdoc/>
    public IReadOnlyList<bool> IsFixedByUserOrBoundary { get; set; }

    /// <summary>
    /// Gets the number of observations.
    /// </summary>
    public int NumberOfObservations => ObservedY?.Count ?? 0;

    /// <summary>
    /// Gets the number of unknown parameters.
    /// </summary>
    public int NumberOfParameters => Point?.Count ?? 0;

    /// <inheritdoc/>
    public Vector<double> ObservedY { get; protected set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public int FunctionEvaluations { get; set; }

    /// <inheritdoc/>
    public int JacobianEvaluations { get; set; }

    #endregion Public Variables

    /// <summary>
    /// Initializes a new instance of the <see cref="NonlinearObjectiveFunctionNonAllocatingBase"/> class.
    /// </summary>
    /// <param name="accuracyOrder">
    /// Desired accuracy order used when numerically approximating the Jacobian.
    /// The value is clamped to the range [1, 6].
    /// </param>
    public NonlinearObjectiveFunctionNonAllocatingBase(int accuracyOrder = 2)
    {
      _accuracyOrder = Math.Min(6, Math.Max(1, accuracyOrder));
    }

    /// <inheritdoc/>
    public abstract IObjectiveModel Fork();

    /// <inheritdoc/>
    public abstract IObjectiveModel CreateNew();

    /// <inheritdoc/>
    public Vector<double> Point => _coefficients;

    /// <inheritdoc/>
    public Vector<double> ModelValues { get; protected set; }

    /// <inheritdoc/>
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
    /// Gets Chi²/(N-F+1).
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool IsGradientSupported => true;

    /// <inheritdoc/>
    public bool IsHessianSupported => true;


    /// <inheritdoc/>
    public void SetParameters(Vector<double> initialGuess, List<bool>? isFixed = null)
    {
      SetParameters((IReadOnlyList<double>)initialGuess, isFixed);
    }

    /// <summary>
    /// Sets model parameters and optional fixed flags for individual parameters.
    /// </summary>
    /// <param name="initialGuess">The initial values of the parameters.</param>
    /// <param name="isFixed">
    /// Optional list with the same length as <paramref name="initialGuess"/>.
    /// For every fixed parameter, the corresponding element is <see langword="true"/>.
    /// </param>
    public virtual void SetParameters(IReadOnlyList<double> initialGuess, IReadOnlyList<bool>? isFixed = null)
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

    /// <inheritdoc/>
    public void EvaluateAt(Vector<double> parameters)
    {
      EvaluateAt((IReadOnlyList<double>)parameters);
    }

    /// <summary>
    /// Evaluates the model at the given parameter vector and invalidates cached dependent values.
    /// </summary>
    /// <param name="parameters">The parameter vector.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when any element of <paramref name="parameters"/> is not finite.</exception>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Evaluates the objective function value and updates cached values.
    /// </summary>
    protected abstract void EvaluateFunction();

    /// <summary>
    /// Evaluates the Jacobian and updates cached Jacobian-derived values (gradient and Hessian).
    /// </summary>
    protected abstract void EvaluateJacobian();

    /// <summary>
    /// Numerically approximates the Jacobian at the specified parameter vector.
    /// </summary>
    /// <param name="parameters">The parameter vector at which to evaluate the Jacobian.</param>
    /// <param name="currentValues">The model values corresponding to <paramref name="parameters"/>.</param>
    /// <param name="accuracyOrder">The desired accuracy order for the numerical approximation.</param>
    /// <returns>The numerically approximated Jacobian matrix.</returns>
    protected abstract Matrix<double> NumericalJacobian(Vector<double> parameters, Vector<double> currentValues, int accuracyOrder = 2);

    #endregion Private Methods

  }
}
