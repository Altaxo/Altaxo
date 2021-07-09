#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;


namespace Altaxo.Calc.Ode
{
  public partial class GearsBDFWithNordsieckState
  {
    /// <summary>
    /// Contains core functions for Gear's method in combination with the Nordsieck state.
    /// </summary>
    public class Core
    {
      /// <summary>
      /// Relative tolerance by which the step size can be exceeded without compromising the accuracy goal.
      /// </summary>
      private const double MaxAllowableStepSizeExceedTolerance = 1E-3;

      /// <summary>
      /// The maximum factor, by which the step size will be increased.
      /// </summary>
      private const double MaxStepSizeFactor = 4;

      /// <summary>
      /// The minimum factor, by which the step size will be decreased.
      /// </summary>
      private const double MinStepSizeFactor = 1 / 8d;

      /// <summary>
      /// Error safety factor. The current relative error is multiplied by this factor, to
      /// calculate the proposed step size.
      /// </summary>
      private const double ErrorSafetyFactor = 2;

      /// <summary>
      /// Gets or sets the minimum allowed order.
      /// </summary>
      public int MinOrder { get; set; } = 1;

      /// <summary>
      /// Gets or sets the maximum allowed order.
      /// </summary>
      public int MaxOrder { get; set; } = 5;

      /// <summary>
      /// The minimum factor for increasing step size (the step size will not be increased if the proposed step size factor is less than this value).
      /// </summary>
      private const double MinFactorForIncreasingStepSize = 18 / 16d;


      /// <summary>
      /// The absolute tolerance. This is either an array of length 1 (in this case the absolute tolerances of all y values
      /// are equal), or an array of the same size than y.
      /// </summary>
      public double[] _absoluteTolerances;

      /// <summary>
      /// The relativ tolerances. This is either an array of length 1 (in this case the relative tolerances of all y values
      /// are equal), or an array of the same size than y.
      /// </summary>
      public double[] _relativeTolerances;

      /// <summary>
      /// The vector norm, by which from the vector of relative errors (dimension <see cref="N"/>) the scalar relative error is calculated.
      /// </summary>
      public ErrorNorm _errorNorm;

      public OdeIterationMethod _iterationMethod;

      /// <summary>
      /// The differential equation to integrate. First arg is the independent variable (x), second arg are the current y-values, and the 3rd arg adopts the
      /// derivatives of y as calculated by this function.
      /// </summary> 
      protected Action<double, double[], double[]> _f;

      /// <summary>
      /// Gets or sets the function to evaluate the jacobian.
      /// </summary>
      public CalculateJacobian EvaluateJacobian { get; set; }

      /// <summary>
      /// Gets the current x value.
      /// </summary>
      public double X => _x[0].X;

      /// <summary>
      /// Gets the current y values. The elements of the returned array must not be changed, and are intended
      /// for immediate use only.
      /// </summary>
      public double[] Y_volatile => _nordsieckArray[0];

      /// <summary>
      /// The solver used to carry out Newton iterations.
      /// </summary>
      private GaussianEliminationSolver _solver;

      /// <summary>
      /// The dimension of the y array (number of y values).
      /// </summary>
      private readonly int N;

      /// <summary>
      /// Gets the number of steps taken so far.
      /// </summary>
      public int NumberOfStepsTaken { get; private set; }

      /// <summary>
      /// Gets the number of jacobian evaluations so far.
      /// </summary>
      public int NumberOfJacobianEvaluations { get; private set; }

      #region Working variables

      /// <summary>
      /// Contains the value of cn of the previous step (neccessary of error calculation of the higher order, see eq. 2.40 in [1]).
      /// </summary>
      private double _cn_previous;

      /// <summary>
      /// Contains the value of cn of the current step (neccessary of error calculation of the higher order, see eq. 2.40 in [1]).
      /// </summary>
      private double _cn_current;

      /// <summary>
      /// The number of steps at current order (is set to zero again if the order changed).
      /// </summary>
      private int _numberOfStepsAtCurrentOrder;

      /// <summary>
      /// The number of steps at current step size (is set to zero again if the step size changed).
      /// </summary>
      private int _numberOfStepsAtCurrentStepSize;

      /// <summary>
      /// The number of steps without jacobian evaluation. If this variable is set to a negative number,
      /// a jacobian evaluation is forced in the next step.
      /// </summary>
      private int _numberOfStepsWithoutJacobianEvaluation = -1;

      /// <summary>
      /// The Nordsieck state. The spine dimension designates the i-th derivative,
      /// the second dimension is the dimension of the y-values.
      /// </summary>
      private double[][] _nordsieckArray;

      /// <summary>
      /// The step size the <see cref="_nordsieckArray"/> is based on. 
      /// </summary>
      private double _nordsieckArray_h;

      /// <summary>
      /// The next x the <see cref="_nordsieckArray"/> is based on. This x is equal to the current x plus
      /// the <see cref="_nordsieckArray_h"/> step size. This variable is somewhat redundant, and is solely to avoid
      /// rounding errors.
      /// </summary>
      private double _nordsieckArray_x;

      /// <summary>
      /// The Nordsieck state, saved for step reversal. The spine dimension designates the i-th derivative,
      /// the second dimension is the dimension of the y-values.
      /// </summary>
      private double[][] _nordsieckArray_Saved;

      /// <summary>
      /// The step size the <see cref="_nordsieckArray_Saved"/> is based on.
      /// </summary>
      private double _nordsieckArray_h_Saved;

      /// <summary>
      /// The next x value the <see cref="_nordsieckArray_Saved"/> is based on.
      /// </summary>
      private double _nordsieckArray_x_Saved;

      /// <summary>
      /// Array of previous x. x[0] is the current x, x[1] is the previous x and so on.
      /// </summary>
      // public double[] _x;
      public (double X, double Step)[] _x;

      /// <summary>
      /// The current order.
      /// </summary>
      private int _q;

      /// <summary>
      /// The matrix to accomodate the jacobian. The type of matrix (dense, banded, sparse) is determined
      /// in the function that calculated the jacobian (at first, null is provided for the matrix), which tells
      /// the function to instantiate a new matrix according to its requirements.
      /// </summary>
      private IMatrix<double>? _jacobian;

      /// <summary>
      /// A matrix with the same structure than the jacobian matrix (<see cref="_jacobian"/>),
      /// which is used for Newton iterations.
      /// </summary>
      private IMatrix<double>? _jacobian_aux;

      /// <summary>
      /// Designates the number of iterations that were neccessary for convergence with a freshly calculated jacobian matrix.
      /// </summary>
      private int? _numberOfIterationsNeccessaryWithFreshJacobian;


      /// <summary>
      /// The array to accomodate polynomial coefficients of either eq. 2.15 or eq. 2.49 in Byrne and Hindmarsh [1].
      /// </summary>
      private double[] _l_array;

      /// <summary>
      /// Auxillary array (length: MaxOrder+1) for calculation of polynomial coefficients.
      /// </summary>
      private double[] _aux1 = new double[12];

      /// <summary>
      /// Auxillary array (length: MaxOrder+1) for calculation of polynomial coefficients.
      /// </summary>
      private double[] _aux2 = new double[12];

      /// <summary>
      /// Used i) to accomodate the next y values in the iterator loop,
      /// and ii) to accomodate interpolated output.
      /// </summary>
      private double[] _y_next;

      /// <summary>
      /// Auxillary array of length <see cref="N"/> for the iteration loop.
      /// </summary>
      private double[] _naux2;

      /// <summary>
      /// Auxillary array of length <see cref="N"/> for the iteration loop.
      /// </summary>
      private double[] _naux3;


      /// <summary>
      /// The current error array, i.e. the difference between the iterated y and the first estimation of y of the current step.
      /// See eq. 2.12 in Byrne/Hindmarsh 1975 [1].
      /// </summary>
      public double[] _en_current;

      /// <summary>
      /// The previous error array, i.e. the difference between the iterated y and the first estimation of y of the previous step.
      /// See eq. 2.12 in Byrne/Hindmarsh 1975 [1].
      /// </summary>
      public double[] _en_previous;

      /// <summary>
      /// The current relative error at current order q.
      /// (Evaluated in <see cref="EvaluateNextSolutionPoint"/>).
      /// </summary>
      private double _en_Q;

      /// <summary>
      /// Contains the value of the term in square brackets in eqs. 2.40, 2.41 and 2.44 in [1], which is neccessary for error calculation of the next higher and lower order.
      /// </summary>
      private double _onePlusProductTerm;

      #endregion

      #region Nordsieck state

      /// <summary>
      /// Saves the Nordsieck state (makes a copy from <see cref="_nordsieckArray"/> to <see cref="_nordsieckArray_h_Saved"/>),
      /// and saves the value of the step size in <see cref="_nordsieckArray_h_Saved"/>).
      /// </summary>
      public void SaveNordsieckState()
      {
        _nordsieckArray_h_Saved = _nordsieckArray_h;
        _nordsieckArray_x_Saved = _nordsieckArray_x;

        var n = N;

        if (_nordsieckArray_Saved.Length <= _q)
        {
          var old = _nordsieckArray_Saved;
          _nordsieckArray_Saved = new double[_q + 1][];
          Array.Copy(old, _nordsieckArray_Saved, old.Length);
          _nordsieckArray_Saved[_q] = new double[n];
        }

        for (int i = 0; i <= _q; ++i)
        {
          Array.Copy(_nordsieckArray[i], _nordsieckArray_Saved[i], n);
        }
      }

      /// <summary>
      /// Restores the Nordsieck state ( copies back from <see cref="_nordsieckArray_Saved"/> to <see cref="_nordsieckArray"/>,
      /// restores <see cref="_nordsieckArray_h"/>, and also restores the history of x values in <see cref="_x"/>.
      /// </summary>
      public void RestoreNordsieckState()
      {
        int n = N;
        for (int i = 0; i <= _q; ++i)
        {
          Array.Copy(_nordsieckArray_Saved[i], _nordsieckArray[i], n);
        }
        _nordsieckArray_h = _nordsieckArray_h_Saved;
        _nordsieckArray_x = _nordsieckArray_x_Saved;
        Array.Copy(_x, 1, _x, 0, _x.Length - 1);
      }

      /// <summary>
      /// Scales the step size by a factor.
      /// </summary>
      /// <param name="stepSizeScaleFactor">The factor by which to scale the step size.</param>
      public void ScaleStepSize(double stepSizeScaleFactor)
      {
        var fac = stepSizeScaleFactor;
        for (int qq = 1; qq <= _q; ++qq)
        {
          var deriv = _nordsieckArray[qq];
          for (int i = 0; i < deriv.Length; ++i)
          {
            deriv[i] *= fac;
          }
          fac *= stepSizeScaleFactor;
        }
        _nordsieckArray_h *= stepSizeScaleFactor;
        _nordsieckArray_x = X + _nordsieckArray_h;
        _numberOfStepsAtCurrentStepSize = 0;
      }

      /// <summary>
      /// Changes the size to a new value.
      /// </summary>
      /// <param name="newStepSize">The new value of the step size.</param>
      /// <param name="newX">The new value of x (only to avoid rounding errors, you can provide null).</param>
      public void ChangeStepSize(double newStepSize, double? newX = null)
      {
        var scale = newStepSize / _nordsieckArray_h;
        var fac = scale;
        for (int qq = 1; qq <= _q; ++qq)
        {
          var deriv = _nordsieckArray[qq];
          for (int i = 0; i < deriv.Length; ++i)
          {
            deriv[i] *= fac;
          }
          fac *= scale;
        }
        _nordsieckArray_h = newStepSize;
        _nordsieckArray_x = newX.HasValue ? newX.Value : _x[0].X + newStepSize;
        _numberOfStepsAtCurrentStepSize = 0;
      }

      /// <summary>
      /// Calculates a first guess of the new Nordsieck state from the old state, and
      /// advances the <see cref="_x"/> array to the new point.
      /// </summary>
      /// <remarks>See equation 2.7 in Byrne and Hindmarsh [1]</remarks>
      public void Predict()
      {
        int q = _q;
        int n = N;
        var zn = _nordsieckArray;

        for (int k = 0; k < q; ++k)
        {
          for (int j = q; j > k; --j)
          {
            for (int i = 0; i < n; ++i)
            {
              zn[j - 1][i] += zn[j][i];
            }
          }
        }
        Array.Copy(_x, 0, _x, 1, _x.Length - 1);
        _x[0] = (_nordsieckArray_x, _nordsieckArray_h);
      }

      /// <summary>
      /// Corrects the Nordsieck array using the corrected y values and the l array. See eq. 2.19 in [1].
      /// </summary>
      /// <param name="y_corrected">The y corrected.</param>
      /// <param name="l_array">The l array.</param>
      /// <remarks>To calculate the difference between iterated values and the first guess, the
      /// array y_corrected contains the iterated values, and the array _nordsieckArray[0] contains the first guess.</remarks>
      public void Correct(ref double[] y_corrected, double[] l_array)
      {
        var yn0 = _nordsieckArray[0];

        for (int qq = 1; qq <= _q; ++qq)
        {
          var l_q = l_array[qq];
          var d_q = _nordsieckArray[qq];
          for (int i = 0; i < yn0.Length; ++i)
          {
            d_q[i] += (y_corrected[i] - yn0[i]) * l_q;
          }
        }
        Swap(ref y_corrected, ref _nordsieckArray[0]); // finally, use the corrected y for the Nordsieck state
        _nordsieckArray_x = _x[0].X + _nordsieckArray_h;
      }

      /// <summary>
      /// Increases the order by 1. It is ensured that the Nordsieck state can accomodate the new derivative.
      /// The new derivative itself is set to zero. See the notes below eq. 2.49 in Byrne and Hindmarsh [1].
      /// </summary>
      public void IncreaseOrderBy1()
      {
        ++_q;

        if (_nordsieckArray.Length < _q + 1)
        {
          var old = _nordsieckArray;
          _nordsieckArray = new double[_q + 1][];
          Array.Copy(old, _nordsieckArray, old.Length);
        }

        // for increasing the order, we have to make sure
        // that the q-th derivative is set to zero
        if (_nordsieckArray[_q] is null)
        {
          _nordsieckArray[_q] = new double[N];
        }
        else
        {
          Array.Clear(_nordsieckArray[_q], 0, N); // zero q-th derivative
        }
        _numberOfStepsAtCurrentOrder = 0;
      }


      /// <summary>
      /// Decreases the order by 1. This method scales the Nordsieck array by the polynomial coefficients
      /// of eq. 2.49 in Byrne and Hindmarsh [1].
      /// </summary>
      /// <param name="l_array">An auxillary array to accomodate the polynomial coefficients of eq. 2.49 in [1].</param>
      public void DecreaseOrderBy1(double[] l_array)
      {
        // Correction of the Nordsieck array is neccessary only if q >= 3
        if (_q >= 3)
        {
          Array.Clear(_l_array, 0, _q - 1);
          // calculate the coefficients of the polynom x^2 * Product(x+ xi[i]), i=1..q-2
          // see eq. 2.49 in [1]
          var h = _x[0].Step;

          // Initialize for q=3
          _l_array[0] = 1; // Attention: index reversed: index 0 corresponds to highest order coefficient q
          _l_array[1] = 1; // coefficient q-1 for q=3 (is equal to xi[1])

          // Evaluate orders q= 4.. 5
          for (int qq = 4; qq <= _q; ++qq) // for order 4.. q recurse
          {
            var xi_qq = (_x[0].X - _x[qq - 2].X) / h; // xi[i] from eq. 2.14 in [1]
            for (int i = qq - 2; i > 0; --i)
            {
              _l_array[i] += _l_array[i - 1] * xi_qq;
            }
          }

          // now we can correct the Nordsieck array with the coefficients in l_array
          // see text below eq. 2.49 in [1]
          // as noted above, the highest order coefficient is located at index 0 in the l_array
          // at first, we need the second highest coefficient (index 1) to correct derivative q-1
          for (int qq = _q - 1, iqq = 1; qq >= 2; --qq, ++iqq)
          {
            for (int i = 0; i < N; ++i)
            {
              _nordsieckArray[qq][i] -= l_array[iqq] * _nordsieckArray[_q][i];
            }
          }

        }
        --_q;
        _numberOfStepsAtCurrentOrder = 0;
      }



      /// <summary>
      /// Computes the coefficients l. l is used to update the Nordsieck vector if
      /// the error vector is known: z_n = z_n0 + e_n * l with e_n = y_n - y_n0.
      /// </summary>
      /// <param name="l_array">The array to store the result (index 0.. q).</param>
      /// <remarks>See equations 2.15 - 2.20 in Byrne and Hindmarsh [1], and recursion rule in the text below eq. 2.20..</remarks>
      public void ComputeCoefficients_L(double[] l_array)
      {
        var h = _x[0].Step;

        Array.Clear(_l_array, 0, _q + 1);
        l_array[0] = 1; // l0 is always 1
        l_array[1] = 1; // l1 is always 1 for order q = 1;

        for (int qq = 2; qq <= _q; ++qq) // for order 2.. q recurse
        {
          var inv_xi_qq = h / (_x[0].X - _x[qq].X); // 1/xi[i] from eq. 2.14 in [1]
          for (int i = qq; i > 0; --i)
          {
            _l_array[i] += _l_array[i - 1] * inv_xi_qq;
          }
        }
      }

      /// <summary>
      /// Calculates the product of the differences of previous x (the term in square brackets in eqs. 2.40, 2.41 and 2.44 in Byrne and Hindmarsh [1]).
      /// </summary>
      /// <returns></returns>
      public double CalculateOnePlusProducts_tn()
      {
        if (_q == 1)
        {
          return 2;
        }
        else
        {
          double product = 1;
          for (int qq = 2; qq <= _q; ++qq)
          {
            product *= (_x[0].X - _x[qq].X) / (_x[1].X - _x[qq].X);
          }
          return 1 + product;
        }
      }


      #endregion

      /// <summary>
      /// Initializes a new instance of the <see cref="Core"/> class. The initial step size is 1.
      /// </summary>
      /// <param name="x0">The starting value of the independent variable x.</param>
      /// <param name="y0">The starting value of the dependent variables y.</param>
      /// <param name="f">The function to calculate the derivatives.</param>
      /// <param name="evaluateJacobian">The function to evaluate the jacobian.</param>
      /// <param name="options">The options for this ODE.</param>
      public Core(
        double x0,
        double[] y0,
        Action<double, double[], double[]> f,
        CalculateJacobian? evaluateJacobian,
        OdeMethodOptions options
        )
      {
        N = y0.Length;
        _f = f;
        _iterationMethod = OdeIterationMethod.UseJacobian;
        EvaluateJacobian = evaluateJacobian ?? new DenseJacobianMatrixEvaluator(f).EvaluateJacobian;

        _q = 1;

        // Initialize Nordsieck-State
        _x = new (double X, double Step)[13];
        _nordsieckArray = NewJaggedArray<double>(2, y0.Length);
        _nordsieckArray_Saved = NewJaggedArray<double>(2, y0.Length);
        Array.Copy(y0, _nordsieckArray[0], y0.Length); // Initialize 0th derivative = y values
        _f(x0, y0, _nordsieckArray[1]);
        _nordsieckArray_h = 1;
        _nordsieckArray_x = x0 + 1;
        _x[0] = (x0, double.NaN);

        _y_next = new double[N];
        _naux2 = new double[N];
        _naux3 = new double[N];
        _en_current = new double[N];
        _en_previous = new double[N];
        _l_array = new double[12];
        _solver = new GaussianEliminationSolver();

        // Options
        _absoluteTolerances = (double[])options.AbsoluteTolerances.Clone();
        _relativeTolerances = (double[])options.RelativeTolerances.Clone();
        _errorNorm = options.ErrorNorm;
        if (options is MultiStepMethodOptions mso)
        {
          MinOrder = Math.Max(1, mso.MinOrder);
          MaxOrder = Math.Min(5, mso.MaxOrder);
          _iterationMethod = mso.IterationMethod;
        }

      }


      /// <summary>
      /// Initializes a new instance of the <see cref="Core"/> class (mostly for testing purposes, since
      /// the kth derivatives at the starting point must be known in advance). The initial step size is 1.
      /// </summary>
      /// <param name="x0">The starting value of the independent variable x.</param>
      /// <param name="initDerivatives">The starting value of y, and its derivatives. The dimension
      /// of the spine array determines the order q with which the evaluation is started. (Example: if the dimension of the spine array is 3, the order q is set to 2). </param>
      /// <param name="f">The function to calculate the derivatives.</param>
      /// <param name="evaluateJacobian">The function to evaluate the jacobian.</param>
      /// <param name="options">The options for this ODE.</param>
      public Core(
        double x0,
        double[][] initDerivatives,
        Action<double, double[], double[]> f,
        CalculateJacobian? evaluateJacobian,
        OdeMethodOptions options
        )
        : this(x0, initDerivatives[0], f, evaluateJacobian, options)
      {
        // Initialize Nordsieck-State
        _nordsieckArray = NewJaggedArray<double>(initDerivatives.Length, N);
        _nordsieckArray_Saved = NewJaggedArray<double>(initDerivatives.Length, N);
        int fac = 1;
        for (int i = 0; i < initDerivatives.Length; ++i)
        {
          fac *= Math.Max(1, i);
          for (int k = 0; k < N; ++k)
          {
            _nordsieckArray[i][k] = initDerivatives[i][k] / fac;
          }
        }
        _nordsieckArray_h = 1;
        _nordsieckArray_x = x0 + _nordsieckArray_h;
        for (int i = 0; i <= initDerivatives.Length; ++i)
          _x[i] = (x0 - i * _nordsieckArray_h, _nordsieckArray_h);
      }



      /// <summary>
      /// Evaluates the next solution point.
      /// </summary>
      /// <param name="nextMandatoryX">The next mandatory value of the independent variable x. If there
      /// is no next mandatory value, you should provide <see cref="double.PositiveInfinity"/> instead.</param>
      /// <exception cref="InvalidOperationException">Ode does not converge</exception>
      public void EvaluateNextSolutionPoint(double? nextMandatoryX)
      {
        AdjustNextStepSizeForMandatorySolutionPoint(nextMandatoryX);

        for (int loops = 32; loops >= 0; --loops)
        {
          SaveNordsieckState();

          Predict();
          ComputeCoefficients_L(_l_array);

          bool iterationHasConverged = _iterationMethod switch
          {
            OdeIterationMethod.UseJacobian => IterationUsingJacobian(), // _en contains difference between iterated y and estimated y
            OdeIterationMethod.DoNotUseJacobian => IterationUsingFunctionalIteration(),
            _ => throw new NotImplementedException($"Sorry, iteration method {_iterationMethod} is currently not implemented")
          };

          if (iterationHasConverged) // Iteration has converged
          {
            _en_Q = Calculate_En_Q(out var cn, out _onePlusProductTerm);

            if (_en_Q >= 1) // Step will be not accepted
            {
              RestoreNordsieckState();
              var factor = GetProposedStepSizeFactorClamped(_en_Q, _q);
              var (h_new, x_new) = OptimizeStepSizeForNextMandatoryPoint(nextMandatoryX, _nordsieckArray_h * factor);
              ChangeStepSize(h_new, x_new);
              _numberOfStepsAtCurrentStepSize = 0;
              continue;
            }
            else // Step will be accepted
            {
              Correct(ref _y_next, _l_array);
              ++_numberOfStepsAtCurrentOrder;
              ++_numberOfStepsAtCurrentStepSize;
              ++NumberOfStepsTaken;
              _cn_previous = _cn_current;
              _cn_current = cn;

#if Details
              {
                var x = X;
                var yexp = Math.Exp(-X);
                var ydev = Y_volatile[0] - yexp;

                var ydexp = -yexp;
                var ydcurr = _nordsieckArray[1][0] / _nordsieckArray_h;
                System.Diagnostics.Debug.Write($"Q={_q} H={_x[0].Step} X={X}, YD={ydev}");
                for (int ii = 0; ii <= _q; ++ii)
                {
                  System.Diagnostics.Debug.Write($" Y{ii}={_nordsieckArray[ii][0] * Faculty(ii) / Math.Pow(_nordsieckArray_h, ii)}");
                }
                System.Diagnostics.Debug.WriteLine("");
              }
#endif

              return;
            }
          }
          else // iteration has not converged
          {
            RestoreNordsieckState();
            ScaleStepSize(0.25); // make step smaller
            _numberOfStepsWithoutJacobianEvaluation = -1;
            continue;
          }
        }
        throw new InvalidOperationException("Ode does not converge");
      }

      /// <summary>
      /// Makes small adjustments to the next step size in order to exactly hit the next mandatory solution point.
      /// Of course, no adjustments are being made if the mandatory point is too far away or does not exist at all.
      /// </summary>
      /// <param name="nextMandatoryX">The next mandatory x value (or null if no such point exist).</param>
      /// <remarks>The two members <see cref="_nordsieckArray_h"/> and <see cref="_nordsieckArray_x"/> are adjusted if neccessary.
      /// </remarks>
      private void AdjustNextStepSizeForMandatorySolutionPoint(double? nextMandatoryX)
      {
        if (nextMandatoryX.HasValue && !(nextMandatoryX.Value == _nordsieckArray_x))
        {
          var h_new = nextMandatoryX.Value - _x[0].X;
          if (AreEqual(h_new, _nordsieckArray_h, 4 * DBL_EPSILON))
          {
            // for such a small deviation, we don't rescale the Nordsieck array
            // we simple take the new h and the new proposed x
            _nordsieckArray_h = h_new;
            _nordsieckArray_x = nextMandatoryX.Value;
          }
          else // we are outside the tolerance of DBL_Epsilon
          {
            if (h_new < _nordsieckArray_h) // If the step to the next mandatory point is smaller, we rescale the Nordsieck array
            {
              ChangeStepSize(h_new, nextMandatoryX.Value);
            }
            else // step is greater
            {
              if (AreEqual(h_new, _nordsieckArray_h, MaxAllowableStepSizeExceedTolerance)) // if the step is only somewhat greater
              {
                ChangeStepSize(h_new, nextMandatoryX.Value);
              }
            }
          }
        }
      }

      /// <summary>
      /// Gets interpolated y values. The independent variable x must lie inbetween the x of the previous step and the current step.
      /// </summary>
      /// <param name="x">The independent variable x.</param>
      /// <returns>Array of interpolated y values. The elements of the array must not be changed, and are
      /// intended for immediate use only (because they will be changed at the next step).</returns>
      public double[] GetInterpolatedY_volatile(double x)
      {
        var r = (x - _x[0].X) / _nordsieckArray_h;
        double rr = r;
        var yresult = _y_next;

        Array.Copy(_nordsieckArray[0], yresult, N);

        for (int qq = 1; qq <= _q; ++qq)
        {
          var zq = _nordsieckArray[qq];
          for (int i = 0; i < N; ++i)
          {
            yresult[i] += rr * zq[i];
          }
          rr *= r;
        }
        return _y_next;
      }

      /// <summary>
      /// Compares the errors at the current order q, at order (q-1) and at order (q+1), using the proposed step
      /// sizes at these orders. The order which maximizes the step size is then chosen,
      /// and the step size is adjusted accordingly.
      /// </summary>
      public void AdjustStepSizeAndOrder(double? nextMandatoryX)
      {
        // think about changing the order
        if (_numberOfStepsAtCurrentOrder > _q)
        {
          double proposedStepSizeFactorQm1 = 0; // Step size factor for a by one lower order
          double proposedStepSizeFactorQ = 0; // Step size factor for the current order
          double proposedStepSizeFactorQp1 = 0; // Step size factor for a by one higher order
          double h = _x[0].Step;
          int proposedOrderChange = 0;

          double enqp1 = double.NaN, enqm1 = double.NaN;

          if (_q >= MinOrder && _q <= MaxOrder)
          {
            proposedStepSizeFactorQ = GetProposedStepSizeFactorUnclamped(_en_Q, _q);
          }

          if (_q < MaxOrder)
          {
            enqp1 = Calculate_En_QPlus1(_onePlusProductTerm);
            proposedStepSizeFactorQp1 = GetProposedStepSizeFactorUnclamped(enqp1, _q + 1);
            if (proposedStepSizeFactorQp1 > proposedStepSizeFactorQ)
              proposedOrderChange = 1;
          }
          if (_q > MinOrder)
          {
            enqm1 = Calculate_En_QMinus1();
            proposedStepSizeFactorQm1 = GetProposedStepSizeFactorUnclamped(enqm1, _q - 1);
            if (proposedStepSizeFactorQm1 > proposedStepSizeFactorQ && proposedStepSizeFactorQm1 > proposedStepSizeFactorQp1)
              proposedOrderChange = -1;
          }

          double? proposedStepSizeFactor = null;
          if (proposedOrderChange == 1 && proposedStepSizeFactorQp1 >= MinFactorForIncreasingStepSize)
          {
            IncreaseOrderBy1();
            proposedStepSizeFactor = GetProposedStepSizeFactorClamped(enqp1, _q); // here only q because we dont have q+1 yet
          }
          else if (proposedOrderChange == -1 && proposedStepSizeFactorQm1 >= MinFactorForIncreasingStepSize)
          {
            DecreaseOrderBy1(_l_array);
            proposedStepSizeFactor = GetProposedStepSizeFactorClamped(enqm1, _q - 1);
          }
          else if (proposedOrderChange == 0 && proposedStepSizeFactorQ >= MinFactorForIncreasingStepSize)
          {
            proposedStepSizeFactor = GetProposedStepSizeFactorClamped(_en_Q, _q);
          }

          if (proposedStepSizeFactor.HasValue)
          {
            var (h_new, x_new) = OptimizeStepSizeForNextMandatoryPoint(nextMandatoryX, h * proposedStepSizeFactor.Value);
            ChangeStepSize(h_new, x_new);
          }
        }
      }

      /// <summary>
      /// Optimizes the step size, in a way that the next mandatory solution point will be reached without too big or too small step sizes.
      /// </summary>
      /// <param name="nextMandatoryX">The x value of the next mandatory solution point (is null if there is no mandatory solution point).</param>
      /// <param name="proposedStepSize">Proposed step size.</param>
      /// <returns>A new step size, that is equal or smaller than the proposed step size, and guarantees that the next solution point is reached without a too small step.</returns>
      /// <exception cref="InvalidProgramException">$"The next mandatory solution point x ({nextMandatoryX.Value}) should always be greater than the current x ({_x[0].X})</exception>
      public (double H, double X) OptimizeStepSizeForNextMandatoryPoint(double? nextMandatoryX, double proposedStepSize)
      {
        if (nextMandatoryX.HasValue)
        {
          var span = nextMandatoryX.Value - _x[0].X;
          if (!(span > 0))
          {
            throw new InvalidProgramException($"The next mandatory solution point x ({nextMandatoryX.Value}) should always be greater than the current x ({_x[0].X})");
          }

          var steps1 = Math.Ceiling(span / (proposedStepSize * OnePlusDBL_EPSILON));
          var steps2 = Math.Floor(span / (proposedStepSize * OneMinusDBL_EPSILON));
          var denom = Math.Max(steps1, steps2);
          if (denom == 1)
          {
            return (span, nextMandatoryX.Value);
          }
          else
          {
            var h = span / denom;
            return (h, _x[0].X + h);
          }
        }
        else
        {
          return (proposedStepSize, _x[0].X + proposedStepSize);
        }
      }

      /// <summary>
      /// Does some iteration steps. At the end, the corrected y-values are stored in <see cref="_aux1"/>.
      /// </summary>
      /// <returns>True if the iteration has converged; otherwise, false.</returns>
      public bool IterationUsingJacobian()
      {
        const int MaxNumberOfIterations = 10;

        double l2NormOfCorrection;

        var n = N;
        var u = _y_next;
        var yn0 = _nordsieckArray[0];

        // Initialize u with yn0, see eq. 2.23 in [1]
        Array.Copy(yn0, u, n);
        var one_l1 = 1 / _l_array[1];
        var h = _x[0].Step;

        double? l2NormOfFirstCorrection = null;
        double l2NormOfPreviousCorrection = double.PositiveInfinity;

        if (_jacobian is null || _jacobian_aux is null || _numberOfStepsWithoutJacobianEvaluation < 0)
        {
          // method with full jacobian
          if (_jacobian_aux is null)
          {
            EvaluateJacobian(_x[0].X, yn0, ref _jacobian_aux); // allocate storage for auxilliary jacobian matrix
          }

          EvaluateJacobian(_x[0].X, yn0, ref _jacobian);
          _numberOfStepsWithoutJacobianEvaluation = -1;
          _numberOfIterationsNeccessaryWithFreshJacobian = null;
        }


        ++_numberOfStepsWithoutJacobianEvaluation;

        int loops;
        for (loops = 0; loops <= MaxNumberOfIterations; ++loops)
        {
          // Calculate derivative
          _f(_x[0].X, u, _naux2);

          // Calculate right side of eq. 2.22 in [1]
          for (int i = 0; i < n; ++i)
          {
            _naux2[i] = (u[i] - yn0[i]) - one_l1 * (h * _naux2[i] - _nordsieckArray[1][i]);
          }


          {
            // Instead of scaling the jacobian with h/l1, we scale I and the right side with l1/h
            var l1h = _l_array[1] / _nordsieckArray_h;
            MatrixMath.Copy(_jacobian, _jacobian_aux);
            for (int i = 0; i < n; ++i)
            {
              _jacobian_aux[i, i] -= l1h;
              _naux2[i] *= l1h;
            }
            _solver.SolveDestructive(_jacobian_aux, _naux2, _naux3);
            l2NormOfCorrection = L2Norm(_naux3);

            // update u
            for (int i = 0; i < n; ++i)
            {
              u[i] += _naux3[i];
            }
          }

          if (!l2NormOfFirstCorrection.HasValue)
            l2NormOfFirstCorrection = l2NormOfCorrection;
          else if (!(l2NormOfCorrection < l2NormOfFirstCorrection))
          {
            // either this is no convergence then, or the first correction was already so small, that there could be no improvement in
            // the second correction
            // we have to check wheter the first correction was an considerable improvement

            if (l2NormOfFirstCorrection < L2Norm(u) * 1E-15)
              return true;  // l2Norm of first correction was already very small, so there was already convergence
            else
              return false; // no convergence at all
          }

          if (loops == MaxNumberOfIterations)
            return false; // no convergence in max number of iterations

          if (l2NormOfCorrection >= l2NormOfPreviousCorrection)
            break;

          l2NormOfPreviousCorrection = l2NormOfCorrection;
        } // for some loops

        // calculate the current error (u-yn0), i.e. the difference between iterated y and the first predition of y
        Swap(ref _en_current, ref _en_previous);
        var en = _en_current;
        for (int i = 0; i < n; ++i)
        {
          en[i] = u[i] - yn0[i];
        }

        // Some additional measures to avoid too many iterations if the jacobian is outdated:
        if(_numberOfIterationsNeccessaryWithFreshJacobian is null)
        {
          // if the jacobian was freshly calculated, then store how many loops it has taken for convergence 
          _numberOfIterationsNeccessaryWithFreshJacobian = loops;
        }
        else if(loops > 3 + _numberOfIterationsNeccessaryWithFreshJacobian)
        {
          // if the jacobian is not fresh, and it takes more than 3 loops more compared with the fresh jacobian,
          // then a fresh calculation of the jacobian is enforced
          _numberOfStepsWithoutJacobianEvaluation = -1;
        }

        return true;
      }



      /// <summary>
      /// Does some iteration steps. This iteration method does not use the Jacobian for Newton-Raphson steps,
      /// instead a functional iteration is executed. This takes much longer to reach the same degree of accuracy.
      /// At the end, the corrected y-values are stored in <see cref="_aux1"/>.
      /// </summary>
      /// <returns>True if the iteration has converged; otherwise, false.</returns>
      public bool IterationUsingFunctionalIteration()
      {
        double l2NormOfThisCorrection;
        double l2NormOfTotalCorrection;

        var n = N;
        var u = _y_next;
        var yn0 = _nordsieckArray[0];

        // Initialize u with yn0, see eq. 2.23 in [1]
        Array.Copy(yn0, u, n);
        var one_l1 = 1 / _l_array[1];
        var h = _x[0].Step;
        Array.Clear(_naux3, 0, _naux3.Length);

        double? l2NormOfFirstCorrection = null;
        double l2NormOfPreviousCorrection = double.PositiveInfinity;

        for (int loops = 100; loops >= 0; --loops)
        {
          // Calculate derivative
          _f(_x[0].X, u, _naux2);

          // Calculate right side of eq. 2.22 in [1]
          for (int i = 0; i < n; ++i)
          {
            _naux2[i] = (u[i] - yn0[i]) - one_l1 * (h * _naux2[i] - _nordsieckArray[1][i]);
          }

          // update u
          double sum2 = 0;
          double sum3 = 0;
          for (int i = 0; i < n; ++i)
          {
            var diff = _naux2[i];
            u[i] -= diff;
            _naux3[i] -= diff;

            sum2 += diff * diff;
            sum3 += _naux3[i] * _naux3[i];
          }
          l2NormOfThisCorrection = Math.Sqrt(sum2 / N);
          l2NormOfTotalCorrection = Math.Sqrt(sum3 / N);

          if (!l2NormOfFirstCorrection.HasValue)
            l2NormOfFirstCorrection = l2NormOfThisCorrection;
          else if (!(l2NormOfThisCorrection < l2NormOfFirstCorrection))
            return false; // no convergence at all

          if (loops == 0)
            return false; // no convergence in max number of iterations

          if (l2NormOfThisCorrection >= l2NormOfPreviousCorrection)
            break;

          l2NormOfPreviousCorrection = l2NormOfThisCorrection;
        } // for some loops

        // calculate the current error (u-yn0), i.e. the difference between iterated y and the first predition of y
        Swap(ref _en_current, ref _en_previous);
        var en = _en_current;
        for (int i = 0; i < n; ++i)
        {
          en[i] = u[i] - yn0[i];
        }

        return true;
      }

      /// <summary>
      /// Gets the proposed step size factor (unclamped). This unclamped version should only be used for
      /// comparison of the errors of order q-1, q, and q+1. Use the clamped version for step size control.
      /// </summary>
      /// <param name="relativeError">The relative error.</param>
      /// <param name="q">The order q.</param>
      /// <returns>The factor, by which the step size should be adjusted to reach the accuraccy goal. A safety
      /// factor is already included.</returns>
      public virtual double GetProposedStepSizeFactorUnclamped(double relativeError, int q)
      {
        double result = Math.Pow(relativeError * ErrorSafetyFactor, -1.0 / (q + 1));
        return result;
      }

      /// <summary>
      /// Gets the proposed step size factor (clamped). This clamped version should be used for step size control.
      /// For comparison of the errors of order q-1, q, and q+1, use the unclamped version.
      /// </summary>
      /// <param name="relativeError">The relative error.</param>
      /// <param name="q">The order q.</param>
      /// <returns>The factor, by which the step size should be adjusted to reach the accuraccy goal, clamped to a minimum and a maximum allowed factor. A safety
      /// factor is already included.</returns>
      public virtual double GetProposedStepSizeFactorClamped(double relativeError, int q)
      {
        return Math.Max(MinStepSizeFactor, Math.Min(MaxStepSizeFactor, GetProposedStepSizeFactorUnclamped(relativeError, q)));
      }


      /// <summary>
      /// Gets the initial step size. The absolute and relative tolerances must be set before the call to this function.
      /// </summary>
      /// <returns>The initial step size in the context of the absolute and relative tolerances.</returns>
      /// <exception cref="InvalidOperationException">Either absolute tolerance or relative tolerance is required to be &gt; 0</exception>
      public virtual double GetInitialStepSize(int order)
      {
        var n = N;
        // we re-use the _k array here
        var y_current = _nordsieckArray[0];
        var f0 = _nordsieckArray[1]; // for the derivative at the current point
        var f1 = new double[n]; // derivative at the first guess of the step size
        var delta = new double[n]; // allowed absolute tolerances
        var ytemp = new double[n]; // guess of y at the first guess of the step size

        _f(_x[0].X, y_current, f0); // derivatives at the current point

        double d0 = 0;
        double d1 = 0;
        var abstollength = _absoluteTolerances.Length;
        var reltollength = _relativeTolerances.Length;
        for (int i = 0; i < n; i++)
        {
          delta[i] = _absoluteTolerances[i % abstollength] + _relativeTolerances[i % reltollength] * Math.Abs(y_current[i]);
          d0 = Math.Max(d0, Math.Abs(y_current[i]) / delta[i]);
          d1 = Math.Max(d1, Math.Abs(f0[i]) / delta[i]);
        }
        var h0 = Math.Min(d0, d1) < 1e-5 ? 1e-6 : 1e-2 * (d0 / d1);

        // we have to guess y at x + h0 by calculating ytemp = _y_current + h0 * f0
        for (int i = n - 1; i >= 0; --i)
        {
          ytemp[i] = y_current[i] + h0 * f0[i];
        }

        _f(_x[0].X + h0, ytemp, f1); // derivatives at the first guess of the step size

        double d2 = 0;
        for (int i = 0; i < n; i++)
        {
          d2 = Math.Max(d2, Math.Abs(f0[i] - f1[i]) / delta[i] / h0);
        }
        return Math.Min(100 * h0, Math.Max(d1, d2) <= 1e-15 ? Math.Max(1e-6, h0 * 1e-3) : Math.Pow(1e-2 / Math.Max(d1, d2), 1.0 / order));
      }


      /// <summary>
      /// Calculates the relative error En for the current order q. See eq. 2.41 in [1].
      /// </summary>
      /// <param name="cn">At return, contains the value of cn of the current step (neccessary of error calculation of the higher order, see eq. 2.40 in [1]).</param>
      /// <param name="onePlusProductTerm">At return, contains the value of the term in square brackets in eqs. 2.40, 2.41 and 2.44 in [1], which is neccessary for error calculation of the next higher and lower order.</param>
      /// <returns>The error estimate for the current order q.</returns>
      /// <remarks>This function must be called after <see cref="IterationUsingJacobian"/> is called, and before
      /// <see cref="Correct(ref double[], double[])"/> is called. It is assumed that the 0th element of the <see cref="_nordsieckArray_Saved"/>
      /// contains the y of the previous step, and the array <see cref="_y_next"/> contains the corrected (iterated) y of the current step.
      /// The array <see cref="_en_current"/> must contain the difference between predicted and corrected y of the current step.
      /// Eq. 2.41 in [1] calculates the absolute error, but here we calculate the relative error.
      /// </remarks>
      public double Calculate_En_Q(out double cn, out double onePlusProductTerm)
      {
        var result = _errorNorm switch
        {
          ErrorNorm.L2Norm => GetRelativeError_L2Norm(_en_current),
          ErrorNorm.InfinityNorm => GetRelativeError_InfinityNorm(_en_current),
          _ => throw new NotImplementedException()
        };

        // calculate the term in square brackets eq. 2.40, 2.41 and 2.44 in [1]
        onePlusProductTerm = CalculateOnePlusProducts_tn();

        // calculate cn eq. 2.40 in [1]; this will be of further use in calculation of En(q+1)
        // note that xi_1 * xi_2 *...* xi_q in eq. 2.40 is equal to 1/l_q, see eq. 2.20 in [1]
        cn = onePlusProductTerm / (_l_array[_q] * Faculty(_q + 1));

        return result / (_l_array[1] * onePlusProductTerm);
      }

      /// <summary>
      /// Calculates the relative error En(q+1) for the next higher order (q+1).
      /// </summary>
      /// <param name="onePlusProductTerm">The value of the term in square brackets in eqs. 2.40, 2.41 and 2.44 in [1]. Is calculated before in <see cref="Calculate_En_Q(out double, out double)"/>.</param>
      /// <returns>The relative error En(q+1) for the next higher order (q+1).</returns>
      /// <remarks>See eq. 2.44 in [1] (eq. 2.44 calculates the absolute error, but here we calculate the relative error).</remarks>
      public double Calculate_En_QPlus1(double onePlusProductTerm)
      {
        var n = N;
        var en = _en_current;
        var enm1 = _en_previous;
        var y_prev = _nordsieckArray_Saved[0];
        var y_next = _y_next;
        var q = _q;


        // Calculation of Qn (eq. 2.43 in [1])
        double Qn = (_cn_current / _cn_previous) * Math.Pow(_x[0].Step / _x[1].Step, _q + 1);

        // Xi_(q+1), see eq. 2.14 in [1]
        double xi_qp1 = (_x[0].X - _x[_q + 1].X) / _x[0].Step;

        // l_1(q+1) is l_1(q) + 1 / xi_q+1  (see eq. 2.20 in [1])
        double l1_qp1 = _l_array[1] + 1d / xi_qp1;

        // Prefactor of eq. 2.44 in [1]
        double prefactor = -xi_qp1 / ((_q + 2) * l1_qp1 * onePlusProductTerm);

        var result = _errorNorm switch
        {
          ErrorNorm.L2Norm => GetRelativeError_L2Norm(_en_current, _en_previous, Qn),
          ErrorNorm.InfinityNorm => GetRelativeError_InfinityNorm(_en_current, _en_previous, Qn),
          _ => throw new NotImplementedException()
        };



        return Math.Abs(prefactor) * result;
      }

      /// <summary>
      /// Calculates the relative error En(q-1) for the next lower order (q-1).
      /// </summary>
      /// <returns>The relative error En(q-1) for the next lower order (q-1).</returns>
      /// <remarks>
      /// See eq. 2.42 in [1] (but this calculates the absolute error, here we calculate the relative error).
      /// </remarks>
      public double Calculate_En_QMinus1()
      {
        var q = _q;
        var h = _x[0].Step;
        // Calculate prefactor of eq. 2.42 in [1]
        // Note: the product of xi1, xi2,..xi_q-1 in eq. 2.42 is equal to 1/(l_q*xi_q)
        var prefactor = 1 / (_l_array[q] * (_x[0].X - _x[q].X) / _x[0].Step);
        prefactor /= _l_array[1] * (q - 1);
        return prefactor * GetRelativeError_L2Norm(_nordsieckArray[q]);
      }

      /// <summary>
      /// Gets the relative error (L2 norm) from an array of absolute errors.
      /// </summary>
      /// <param name="en">The array of absolute errors.</param>
      /// <returns>The relative error (L2 norm).</returns>
      private double GetRelativeError_L2Norm(double[] en)
      {
        int n = en.Length;
        var y_prev = _nordsieckArray_Saved[0];
        var y_next = _y_next;

        double sumresqr = 0;

        if (_absoluteTolerances.Length == 1 && _relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_absoluteTolerances.Length > 1 && _relativeTolerances.Length > 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_absoluteTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }

        return Math.Sqrt(sumresqr / n);
      }

      /// <summary>
      /// Gets the relative error (L2 norm) from the absolute errors.
      /// The absolute errors are given by <c>en - Qn * enm1</c>.
      /// </summary>
      /// <param name="en">The array of absolute errors of the current step.</param>
      /// <param name="enm1">The array of absolute errors of the previous step.</param>
      /// <param name="Qn">A (negative) scale factor for the absolute errors of the previous step.</param>
      /// <returns>The relative error (L2 norm).</returns>
      public double GetRelativeError_L2Norm(double[] en, double[] enm1, double Qn)
      {
        var n = N;
        var y_prev = _nordsieckArray_Saved[0];
        var y_next = _y_next;
        var q = _q;

        double sumresqr = 0;

        if (_absoluteTolerances.Length == 1 && _relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_absoluteTolerances.Length > 1 && _relativeTolerances.Length > 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }
        else if (_absoluteTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            var re = Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i]))));
            sumresqr += re * re;
          }
        }

        return Math.Sqrt(sumresqr / n);
      }

      /// <summary>
      /// Gets the relative error (L2 norm) from an array of absolute errors.
      /// </summary>
      /// <param name="en">The array of absolute errors.</param>
      /// <returns>The relative error (L2 norm).</returns>
      private double GetRelativeError_InfinityNorm(double[] en)
      {
        int n = en.Length;
        var y_prev = _nordsieckArray_Saved[0];
        var y_next = _y_next;

        double re = double.MinValue;

        if (_absoluteTolerances.Length == 1 && _relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));

          }
        }
        else if (_absoluteTolerances.Length > 1 && _relativeTolerances.Length > 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));
          }
        }
        else if (_relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));
          }
        }
        else if (_absoluteTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));
          }
        }

        return re;
      }

      /// <summary>
      /// Gets the relative error (Infinity norm) from the absolute errors.
      /// The absolute errors are given by <c>en - Qn * enm1</c>.
      /// </summary>
      /// <param name="en">The array of absolute errors of the current step.</param>
      /// <param name="enm1">The array of absolute errors of the previous step.</param>
      /// <param name="Qn">A (negative) scale factor for the absolute errors of the previous step.</param>
      /// <returns>The relative error (Infinity norm).</returns>
      public double GetRelativeError_InfinityNorm(double[] en, double[] enm1, double Qn)
      {
        var n = N;
        var y_prev = _nordsieckArray_Saved[0];
        var y_next = _y_next;

        double re = double.MinValue;

        if (_absoluteTolerances.Length == 1 && _relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));

          }
        }
        else if (_absoluteTolerances.Length > 1 && _relativeTolerances.Length > 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));

          }
        }
        else if (_relativeTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[i], _relativeTolerances[0] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));

          }
        }
        else if (_absoluteTolerances.Length == 1)
        {
          for (int i = 0; i < n; ++i)
          {
            re = Math.Max(re, Math.Abs(en[i] - Qn * enm1[i]) / (Math.Max(_absoluteTolerances[0], _relativeTolerances[i] * Math.Max(Math.Abs(y_prev[i]), Math.Abs(y_next[i])))));

          }
        }

        return re;
      }


    }
  }
}
