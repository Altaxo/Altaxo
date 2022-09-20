using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  public class NonlinearObjectiveFunctionNonAllocating : IObjectiveModel, IObjectiveModelNonAllocating
  {
    #region Private Variables

    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IVector<double>> _userFunction; // (x, p) => f(x; p)
    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>> _userDerivative; // (x, p) => df(x; p)/dp
    private readonly int _accuracyOrder; // the desired accuracy order to evaluate the jacobian by numerical approximaiton.

    private Vector<double> _coefficients;
    private bool _hasFunctionValue;
    private double _functionValue; // the residual sum of squares, residuals * residuals.
    private Vector<double> _residuals; // the weighted error values

    private bool _hasJacobianValue;
    private Matrix<double> _jacobianValue; // the Jacobian matrix.
    private Matrix<double> _jacobianValueTransposed; // the Jacobian matrix, transposed.
    private Vector<double> _negativeGradientValue; // the Gradient vector.
    private Matrix<double> _hessianValue; // the Hessian matrix.

    private Vector<double> _f1, _f2, _f3, _f4, _f5, _f6;

    #endregion Private Variables

    #region Public Variables

    private IROMatrix<double> _observedXAsMatrix;
    private IReadOnlyList<double> _observedXAsVector;

    /// <summary>
    /// Set or get the values of the independent variable.
    /// </summary>
    public IReadOnlyList<double> ObservedX
    {
      get => _observedXAsVector;
      private set
      {
        _observedXAsVector = value;
        _observedXAsMatrix = MatrixMath.ToROMatrixWithOneColumn(value);
      }
    }

    /// <summary>
    /// Set or get the values of the observations.
    /// </summary>
    public Vector<double> ObservedY { get; private set; }

    /// <summary>
    /// Set or get the values of the weights for the observations.
    /// </summary>
    public Matrix<double> Weights { get; private set; }

    private Vector<double> L; // Weights = LL'

    /// <summary>
    /// Get whether parameters are fixed or free.
    /// </summary>
    public IReadOnlyList<bool> IsFixed { get; private set; }

    /// <summary>
    /// Get the number of observations.
    /// </summary>
    public int NumberOfObservations => ObservedY?.Count ?? 0;

    /// <summary>
    /// Get the number of unknown parameters.
    /// </summary>
    public int NumberOfParameters => Point?.Count ?? 0;

    /// <summary>
    /// Get the degree of freedom
    /// </summary>
    public int DegreeOfFreedom
    {
      get
      {
        var df = NumberOfObservations - NumberOfParameters;
        if (IsFixed is not null)
        {
          df += IsFixed.Count(p => p);
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

    public NonlinearObjectiveFunctionNonAllocating(Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IVector<double>> function,
        Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>> derivative = null, int accuracyOrder = 2)
    {
      _userFunction = function;
      _userDerivative = derivative;
      _accuracyOrder = Math.Min(6, Math.Max(1, accuracyOrder));
    }

    public IObjectiveModel Fork()
    {
      return new NonlinearObjectiveFunctionNonAllocating(_userFunction, _userDerivative, _accuracyOrder)
      {
        ObservedX = ObservedX,
        ObservedY = ObservedY,
        Weights = Weights,

        _coefficients = _coefficients,

        _hasFunctionValue = _hasFunctionValue,
        _functionValue = _functionValue,

        _hasJacobianValue = _hasJacobianValue,
        _jacobianValue = _jacobianValue,
        _negativeGradientValue = _negativeGradientValue,
        _hessianValue = _hessianValue
      };
    }

    public IObjectiveModel CreateNew()
    {
      return new NonlinearObjectiveFunctionNonAllocating(_userFunction, _userDerivative, _accuracyOrder);
    }

    /// <summary>
    /// Set or get the values of the parameters.
    /// </summary>
    public Vector<double> Point => _coefficients;

    /// <summary>
    /// Get the y-values of the fitted model that correspond to the independent values.
    /// </summary>
    public Vector<double> ModelValues { get; private set; }

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
    /// Set observed data to fit.
    /// </summary>
    public void SetObserved(IReadOnlyList<double> observedX, IReadOnlyList<double> observedY, IReadOnlyList<double> weights = null)
    {
      if (observedX is null || observedY is null)
      {
        throw new ArgumentNullException("The data set can't be null.");
      }
      if (observedX.Count != observedY.Count)
      {
        throw new ArgumentException("The observed x data can't have different from observed y data.");
      }

      ObservedX = observedX;
      ObservedY = Vector<double>.Build.DenseOfEnumerable(observedY);


      if (weights is not null && weights.Count != observedY.Count)
      {
        throw new ArgumentException("The weightings can't have different from observations.");
      }
      if (weights is not null && weights.Count(x => double.IsInfinity(x) || double.IsNaN(x)) > 0)
      {
        throw new ArgumentException("The weightings are not well-defined.");
      }
      if (weights is not null && weights.Count(x => x == 0) == weights.Count)
      {
        throw new ArgumentException("All the weightings can't be zero.");
      }
      if (weights is not null)
      {
        var weightsV = Vector<double>.Build.DenseOfEnumerable(weights);
        if (weights.Count(x => x < 0) > 0)
        {
          weightsV = weightsV.PointwiseAbs();
        }

        Weights = Matrix<double>.Build.DenseOfDiagonalVector(weightsV);
        L = Weights.Diagonal().PointwiseSqrt();
      }
      else
      {
        Weights = null;
        L = null;
      }
    }

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
    public void SetParameters(IReadOnlyList<double> initialGuess, IReadOnlyList<bool> isFixed = null)
    {
      _coefficients ??= Vector<double>.Build.Dense(initialGuess.Count);
      for (int i = 0; i < initialGuess.Count; ++i)
        _coefficients[i] = initialGuess[i];

      if (isFixed is not null && isFixed.Count != initialGuess.Count)
      {
        throw new ArgumentException("The isFixed can't have different size from the initial guess.");
      }
      if (isFixed is not null && isFixed.Count(p => p) == isFixed.Count)
      {
        throw new ArgumentException("All the parameters can't be fixed.");
      }
      IsFixed = isFixed;

      // allocate already some
      _negativeGradientValue = Vector<double>.Build.Dense(initialGuess.Count);
      _hessianValue = Matrix<double>.Build.Dense(initialGuess.Count, initialGuess.Count);
      _jacobianValue = Matrix<double>.Build.Dense(NumberOfObservations, NumberOfParameters);
      _jacobianValueTransposed = Matrix<double>.Build.Dense(NumberOfParameters, NumberOfObservations);
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

    private void EvaluateFunction()
    {
      // Calculates the residuals, (y[i] - f(x[i]; p)) * L[i]
      if (ModelValues is null)
      {
        ModelValues = Vector<double>.Build.Dense(NumberOfObservations);
      }

      _userFunction(_observedXAsMatrix, Point, null, ModelValues);
      FunctionEvaluations++;

      // calculate the weighted residuals
      _residuals = (Weights is null)
          ? ObservedY - ModelValues
          : (ObservedY - ModelValues).PointwiseMultiply(L);

      // Calculate the residual sum of squares
      _functionValue = _residuals.DotProduct(_residuals);
    }

    private void EvaluateJacobian()
    {
      // Calculates the jacobian of x and p.
      if (_userDerivative is not null)
      {
        // analytical jacobian
        _userDerivative(_observedXAsMatrix, Point, null, _jacobianValue);
        JacobianEvaluations++;
      }
      else
      {
        // numerical jacobian
        _jacobianValue = NumericalJacobian(Point, ModelValues, _accuracyOrder);
        FunctionEvaluations += _accuracyOrder;
      }

      // weighted jacobian
      for (int i = 0; i < NumberOfObservations; i++)
      {
        for (int j = 0; j < NumberOfParameters; j++)
        {
          if (IsFixed is not null && IsFixed[j])
          {
            // if j-th parameter is fixed, set J[i, j] = 0
            _jacobianValue[i, j] = 0.0;
          }
          else if (Weights is not null)
          {
            _jacobianValue[i, j] = _jacobianValue[i, j] * L[i];
          }
        }
      }

      // Gradient, g = -J'W(y − f(x; p)) = -J'L(L'E) = -J'LR
      // _gradientValue = -_jacobianValue.Transpose() * _residuals;
      _jacobianValue.Transpose(_jacobianValueTransposed);
      _jacobianValueTransposed.Multiply(_residuals, _negativeGradientValue);

      // approximated Hessian, H = J'WJ + ∑LRiHi ~ J'WJ near the minimum
      // _hessianValue = _jacobianValue.Transpose() * _jacobianValue;
      _jacobianValueTransposed.Multiply(_jacobianValue, _hessianValue);
    }

    private Matrix<double> NumericalJacobian(Vector<double> parameters, Vector<double> currentValues, int accuracyOrder = 2)
    {
      const double sqrtEpsilon = 1.4901161193847656250E-8; // sqrt(machineEpsilon)

      Matrix<double> derivatives = Matrix<double>.Build.Dense(NumberOfObservations, NumberOfParameters);

      var d = 0.000003 * parameters.PointwiseAbs().PointwiseMaximum(sqrtEpsilon);

      var h = Vector<double>.Build.Dense(NumberOfParameters);
      for (int j = 0; j < NumberOfParameters; j++)
      {
        h[j] = d[j];

        if (accuracyOrder >= 6)
        {
          // f'(x) = {- f(x - 3h) + 9f(x - 2h) - 45f(x - h) + 45f(x + h) - 9f(x + 2h) + f(x + 3h)} / 60h + O(h^6)
          _userFunction(_observedXAsMatrix, parameters - 3 * h, null, _f1);
          _userFunction(_observedXAsMatrix, parameters - 2 * h, null, _f2);
          _userFunction(_observedXAsMatrix, parameters - h, null, _f3);
          _userFunction(_observedXAsMatrix, parameters + h, null, _f4);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, null, _f5);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, null, _f6);

          var prime = (-_f1 + 9 * _f2 - 45 * _f3 + 45 * _f4 - 9 * _f5 + _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 5)
        {
          // f'(x) = {-137f(x) + 300f(x + h) - 300f(x + 2h) + 200f(x + 3h) - 75f(x + 4h) + 12f(x + 5h)} / 60h + O(h^5)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, null, _f2);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, null, _f3);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, null, _f4);
          _userFunction(_observedXAsMatrix, parameters + 4 * h, null, _f5);
          _userFunction(_observedXAsMatrix, parameters + 5 * h, null, _f6);

          var prime = (-137 * f1 + 300 * _f2 - 300 * _f3 + 200 * _f4 - 75 * _f5 + 12 * _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 4)
        {
          // f'(x) = {f(x - 2h) - 8f(x - h) + 8f(x + h) - f(x + 2h)} / 12h + O(h^4)
          _userFunction(_observedXAsMatrix, parameters - 2 * h, null, _f1);
          _userFunction(_observedXAsMatrix, parameters - h, null, _f2);
          _userFunction(_observedXAsMatrix, parameters + h, null, _f3);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, null, _f4);

          var prime = (_f1 - 8 * _f2 + 8 * _f3 - _f4) / (12 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 3)
        {
          // f'(x) = {-11f(x) + 18f(x + h) - 9f(x + 2h) + 2f(x + 3h)} / 6h + O(h^3)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, null, _f2);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, null, _f3);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, null, _f4);

          var prime = (-11 * f1 + 18 * _f2 - 9 * _f3 + 2 * _f4) / (6 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 2)
        {
          // f'(x) = {f(x + h) - f(x - h)} / 2h + O(h^2)
          _userFunction(_observedXAsMatrix, parameters + h, null, _f1);
          _userFunction(_observedXAsMatrix, parameters - h, null, _f2);

          var prime = (_f1 - _f2) / (2 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else
        {
          // f'(x) = {- f(x) + f(x + h)} / h + O(h)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, null, _f2);

          var prime = (-f1 + _f2) / h[j];
          derivatives.SetColumn(j, prime);
        }

        h[j] = 0;
      }

      return derivatives;
    }



    #endregion Private Methods
  }
}
