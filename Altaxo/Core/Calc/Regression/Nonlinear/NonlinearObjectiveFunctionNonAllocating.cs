using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  public class NonlinearObjectiveFunctionNonAllocating : NonlinearObjectiveFunctionNonAllocatingBase
  {
    #region Private Variables

    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IVector<double>> _userFunction; // (x, p) => f(x; p)
    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>> _userDerivative; // (x, p) => df(x; p)/dp

    #endregion Private Variables

    public NonlinearObjectiveFunctionNonAllocating(Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IVector<double>> function,
        Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>> derivative = null, int accuracyOrder = 2)
      : base(accuracyOrder)
    {
      _userFunction = function;
      _userDerivative = derivative;
    }

    public override IObjectiveModel Fork()
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

    public override IObjectiveModel CreateNew()
    {
      return new NonlinearObjectiveFunctionNonAllocating(_userFunction, _userDerivative, _accuracyOrder);
    }

    #region Private Methods

    /// <summary>
    /// Set parameters and bounds.
    /// </summary>
    /// <param name="initialGuess">The initial values of parameters.</param>
    /// <param name="isFixed">The list to the parameters fix or free.</param>
    public override void SetParameters(IReadOnlyList<double> initialGuess, IReadOnlyList<bool> isFixed = null)
    {
      base.SetParameters(initialGuess, isFixed);

      // set the vectors neccessary to calculate the numerical jacobian
      if (_userDerivative is null)
      {
        if (_accuracyOrder <= 2)
        {
          _f1 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f2 ??= Vector<double>.Build.Dense(NumberOfObservations);
        }
        else if (_accuracyOrder <= 4)
        {
          _f1 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f2 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f3 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f4 ??= Vector<double>.Build.Dense(NumberOfObservations);
        }
        else
        {
          _f1 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f2 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f3 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f4 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f5 ??= Vector<double>.Build.Dense(NumberOfObservations);
          _f6 ??= Vector<double>.Build.Dense(NumberOfObservations);
        }
      }
    }

    protected override void EvaluateFunction()
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

    protected override void EvaluateJacobian()
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
      if (IsFixed is not null)
      {
        for (int j = 0; j < NumberOfParameters; j++)
        {
          if (IsFixed[j])
          {
            _jacobianValue.ClearColumn(j);
          }
          else if (Weights is not null)
          {
            for (int i = 0; i < NumberOfObservations; i++)
            {
              _jacobianValue[i, j] *= L[i];
            }
          }
        }
      }
      else if (Weights is not null)
      {
        for (int i = 0; i < NumberOfObservations; i++)
        {
          var li = L[i];
          for (int j = 0; j < NumberOfParameters; j++)
          {
            _jacobianValue[i, j] *= li;
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

    protected override Matrix<double> NumericalJacobian(Vector<double> parameters, Vector<double> currentValues, int accuracyOrder = 2)
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

    protected IROMatrix<double> _observedXAsMatrix;
    protected IReadOnlyList<double> _observedXAsVector;

    /// <summary>
    /// Set or get the values of the independent variable.
    /// </summary>
    public IReadOnlyList<double> ObservedX
    {
      get => _observedXAsVector;
      protected set
      {
        _observedXAsVector = value;
        _observedXAsMatrix = MatrixMath.ToROMatrixWithOneColumn(value);
      }
    }





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

  }
}
