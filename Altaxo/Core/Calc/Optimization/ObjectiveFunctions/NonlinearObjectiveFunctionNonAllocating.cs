using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.ObjectiveFunctions
{
  public class NonlinearObjectiveFunctionNonAllocating : NonlinearObjectiveFunctionNonAllocatingBase
  {
    #region Private Variables

    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IVector<double>, IReadOnlyList<bool>?> _userFunction; // (x, p) => f(x; p)
    private readonly Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>, IReadOnlyList<bool>?> _userDerivative; // (x, p) => df(x; p)/dp

    #endregion Private Variables

    public NonlinearObjectiveFunctionNonAllocating(
      Action<IROMatrix<double>, IReadOnlyList<double>, IVector<double>, IReadOnlyList<bool>?> function,
      Action<IROMatrix<double>, IReadOnlyList<double>, IReadOnlyList<bool>?, IMatrix<double>, IReadOnlyList<bool>?>? derivative = null,
      int accuracyOrder = 2)
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
    public override void SetParameters(IReadOnlyList<double> initialGuess, IReadOnlyList<bool>? isFixed = null)
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

      _userFunction(_observedXAsMatrix, Point, ModelValues, null);
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
        _userDerivative(_observedXAsMatrix, Point, IsFixedByUserOrBoundary, _jacobianValue, null);
        JacobianEvaluations++;
      }
      else
      {
        // numerical jacobian
        _jacobianValue = NumericalJacobian(Point, ModelValues, _accuracyOrder);
        FunctionEvaluations += _accuracyOrder;
      }

      // weighted jacobian
      if (IsFixedByUserOrBoundary is not null)
      {
        for (int j = 0; j < NumberOfParameters; j++)
        {
          if (IsFixedByUserOrBoundary[j])
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
      const double deltaFactor = 0.000003;

      Matrix<double> derivatives = Matrix<double>.Build.Dense(NumberOfObservations, NumberOfParameters);
      var h = Vector<double>.Build.Dense(NumberOfParameters);
      var d = Vector<double>.Build.Dense(NumberOfParameters);

      for (int i = 0; i < d.Count; ++i)
      {
        d[i] = Math.Abs(deltaFactor * parameters[i]);
        if (d[i] == 0)
        {
          // if the parameter is 0, we use a somewhat higher value than the lowest value which leads to a change of the function values
          // we multiply it with 2/deltaFactor, so that in the next iteration, when we multipy the parameter with deltafactor,
          // we still have some variation
          d[i] = (2 / deltaFactor) * GetLowestParameterVariationToChangeFunctionValues(
            (para) => { _userFunction(_observedXAsMatrix, para, _f2, null); return _f2; },
            currentValues, i, parameters, h);
        }
      }


      for (int j = 0; j < NumberOfParameters; j++)
      {
        h[j] = d[j];

        if (accuracyOrder >= 6)
        {
          // f'(x) = {- f(x - 3h) + 9f(x - 2h) - 45f(x - h) + 45f(x + h) - 9f(x + 2h) + f(x + 3h)} / 60h + O(h^6)
          _userFunction(_observedXAsMatrix, parameters - 3 * h, _f1, null);
          _userFunction(_observedXAsMatrix, parameters - 2 * h, _f2, null);
          _userFunction(_observedXAsMatrix, parameters - h, _f3, null);
          _userFunction(_observedXAsMatrix, parameters + h, _f4, null);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, _f5, null);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, _f6, null);

          var prime = (-_f1 + 9 * _f2 - 45 * _f3 + 45 * _f4 - 9 * _f5 + _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 5)
        {
          // f'(x) = {-137f(x) + 300f(x + h) - 300f(x + 2h) + 200f(x + 3h) - 75f(x + 4h) + 12f(x + 5h)} / 60h + O(h^5)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, _f2, null);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, _f3, null);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, _f4, null);
          _userFunction(_observedXAsMatrix, parameters + 4 * h, _f5, null);
          _userFunction(_observedXAsMatrix, parameters + 5 * h, _f6, null);

          var prime = (-137 * f1 + 300 * _f2 - 300 * _f3 + 200 * _f4 - 75 * _f5 + 12 * _f6) / (60 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 4)
        {
          // f'(x) = {f(x - 2h) - 8f(x - h) + 8f(x + h) - f(x + 2h)} / 12h + O(h^4)
          _userFunction(_observedXAsMatrix, parameters - 2 * h, _f1, null);
          _userFunction(_observedXAsMatrix, parameters - h, _f2, null);
          _userFunction(_observedXAsMatrix, parameters + h, _f3, null);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, _f4, null);

          var prime = (_f1 - 8 * _f2 + 8 * _f3 - _f4) / (12 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 3)
        {
          // f'(x) = {-11f(x) + 18f(x + h) - 9f(x + 2h) + 2f(x + 3h)} / 6h + O(h^3)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, _f2, null);
          _userFunction(_observedXAsMatrix, parameters + 2 * h, _f3, null);
          _userFunction(_observedXAsMatrix, parameters + 3 * h, _f4, null);

          var prime = (-11 * f1 + 18 * _f2 - 9 * _f3 + 2 * _f4) / (6 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else if (accuracyOrder == 2)
        {
          // f'(x) = {f(x + h) - f(x - h)} / 2h + O(h^2)
          _userFunction(_observedXAsMatrix, parameters + h, _f1, null);
          _userFunction(_observedXAsMatrix, parameters - h, _f2, null);

          var prime = (_f1 - _f2) / (2 * h[j]);
          derivatives.SetColumn(j, prime);
        }
        else
        {
          // f'(x) = {- f(x) + f(x + h)} / h + O(h)
          var f1 = currentValues;
          _userFunction(_observedXAsMatrix, parameters + h, _f2, null);

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
    public void SetObserved(IReadOnlyList<double> observedX, IReadOnlyList<double> observedY, IReadOnlyList<double>? weights = null)
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
    /// If a parameter is zero, it is hard to find the right order of magnitude for a variation of that parameter.
    /// Here, the variation is guessed by starting with the lowest possible variation, and increase the variation, until
    /// the function values deviate from the original value.
    /// </summary>
    /// <param name="EvaluateModelValues">Function that evaluate the model values. Argument are the parameters, result are the model values.</param>
    /// <param name="currentValues">The current function values.</param>
    /// <param name="idxParameter">The index of the parameter for which to find a good guess for the variation.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="h">A scratch array.</param>
    /// <returns>The lowest variation (increased in steps of 2), for which the function values deviate from the original values.</returns>
    public static double GetLowestParameterVariationToChangeFunctionValues(Func<Vector<double>, Vector<double>> EvaluateModelValues, Vector<double> currentValues, int idxParameter, Vector<double> parameters, Vector<double> h)
    {
      // because we have no idea what the order of magnitude of the parameter is, we start from the lowest possible
      // value and increase h by a factor, until there is a notable difference between the original value and the new value
      h.Clear();

      var lo = Math.Log(double.Epsilon);
      var hi = Math.Log(double.MaxValue);
      int sign = 1;
      double diff;


      // First of all, test, if the smallest possible variation already leads to invalid results
      var dh = double.Epsilon;
      h[idxParameter] = dh;
      var f2 = EvaluateModelValues(parameters + h);
      diff = (f2 - currentValues).L1Norm();

      if (diff >= 0) // valid result
      {
      }
      else // invalid result with a positive delta, we try it now with a negative delta
      {
        sign = -1;
        dh = sign * double.Epsilon;
        h[idxParameter] = dh;
        f2 = EvaluateModelValues(parameters + h);
        diff = (f2 - currentValues).L1Norm();
        if (diff >= 0) // valid result
        {
        }
        else // invalid result also with a negative delta, so we are out of options now
        {
          throw new InvalidOperationException($"Variation of parameter {idxParameter} around zero lead to invalid function values");
        }
      }

      // first, find the highest possible value
      for (; (hi - lo) > 1;)
      {
        var mi = (lo + hi) * 0.5;
        dh = sign * Math.Exp(mi);
        h[idxParameter] = dh;
        f2 = EvaluateModelValues(parameters + h);
        diff = (f2 - currentValues).L1Norm();


        if (double.IsNaN(diff) || double.IsInfinity(diff))
        {
          hi = mi;
        }
        else
        {
          lo = mi;
        }
      }

      // now look how many values change with the low value
      var criterium = Math.Sqrt(2 * double.Epsilon);
      dh = sign * Math.Exp(lo);
      h[idxParameter] = dh;
      f2 = EvaluateModelValues(parameters + h);
      int numberOfChangingParameters = (f2 - currentValues).Count(x => Math.Abs(x) >= criterium);

      // now, find the lowest possible value that has the same number of changing function values
      hi = lo;
      lo = Math.Log(double.Epsilon);
      for (; (hi - lo) > 1;)
      {
        var mi = (lo + hi) * 0.5;
        dh = sign * Math.Exp(mi);
        h[idxParameter] = dh;
        f2 = EvaluateModelValues(parameters + h);
        int parametersChanged = (f2 - currentValues).Count(x => Math.Abs(x) >= criterium);
        if (numberOfChangingParameters == parametersChanged)
        {
          hi = mi;
        }
        else
        {
          lo = mi;
        }
      }


      return sign * Math.Exp(hi);
    }

  }
}
