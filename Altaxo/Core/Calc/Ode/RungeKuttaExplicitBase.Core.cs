#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Ode
{
  public abstract partial class RungeKuttaExplicitBase
  {

    /// <summary>
    /// The core implements functionality common to all Runge-Kutta methods, like stepping, error evaluation, interpolation and initial step finding.
    /// </summary>
    protected class Core : ICore
    {
      protected const double StepSize_SafetyFactor = 0.8;
      protected const double StepSize_MaxFactor = 5; // Maximum possible step increase
      protected const double StepSize_MinFactor = 0.2; // Maximum possible step decrease

      protected readonly int _order;

      /// <summary>The number of stages of this method.</summary>
      protected readonly int _numberOfStages;

      /// <summary>Central coefficients of the Runge-Kutta scheme. See [1], page 135.</summary>
      protected double[][] _a;

      /// <summary>High order bottom side coefficients of the Runge-Kutta scheme.</summary>
      protected double[] _b;

      /// <summary>Differences between high order and low order bottom side coefficients of the Runge-Kutta scheme (for error estimation).</summary>
      protected double[]? _bhml;

      /// <summary>Left side coefficients of the Runge-Kutta scheme (partitions of the step).</summary>
      protected double[] _c;

      /// <summary>
      /// Squared value of the stiffness detection threshold.
      /// </summary>
      protected double _stiffnessDetectionThresholdValueSquared;

      /// <summary>
      /// Number of (successfull) steps between calls to stiffness detection. If this is null, then stiffness detection is disabled.
      /// </summary>
      protected int _stiffnessDetectionEveryNumberOfSteps;

      // State variables

      /// <summary>True if at least one solution point was evaluated.</summary>
      protected bool _wasSolutionPointEvaluated;

      /// <summary>
      /// Designates whether the slope at y_current was evaluated (for non-FSAL methods).
      /// For FSAL methods, this value is meaningless, because the last stage always contains k_next after a step.
      /// </summary>
      protected bool _was_Knext_Evaluated;

      /// <summary>True if the last point is the same as first point (FSAL property). This is for instance true for the Dormand-Prince (DOPRI) method.</summary>
      protected bool _isFirstSameAsLastMethod;

      protected double _x_previous;
      protected double _x_current;
      protected double _stepSize_previous;
      protected double _stepSize_current;
      protected double[] _y_previous;
      protected double[] _y_current;

      /// <summary>Array to accomodate y for calculation of the stages.
      /// At the end of <see cref="EvaluateNextSolutionPoint(double)"/>, this array usually contains the y
      /// of the last stage (non-FSAL methods), or the y of the stage before the last stage (FSAL methods).
      /// </summary>
      protected double[] _y_stages;

      protected double[] _y_current_LocalError;
      public double _absoluteTolerance;
      public double _relativeTolerance;

      protected Action<double, double[], double[]> _f;

      /// <summary>The number of evaluation results in <see cref="ThrowIfStiffnessDetected()"/>, for which the result was false (non-stiff).
      /// This counter is re-set to zero if a stiff condition is detected.
      /// </summary>
      protected int _numberOfNonstiffEvaluationResults;

      /// <summary>The number of evaluation results in <see cref="ThrowIfStiffnessDetected()"/>, for which the result was true (stiff).
      /// This counter is re-set to zero if a non-stiff condition is detected.
      /// </summary>
      protected int _numberOfStiffEvaluationResults;

      /// <summary>
      /// The number of rejected stiffness detection calls after the last stiffness evaluation.
      /// </summary>
      protected int _numberOfRejectedStiffnessDetectionCalls;


      // Helper variables

      /// <summary>Array of derivatives at the different stages.</summary>
      protected double[][] _k;


      /// <summary>True if dense output was prepared, i.e. the array _rcont contains valid values.</summary>
      protected bool _isDenseOutputPrepared;

      /// <summary>Contains the precalcuated polynomial coefficients for dense output.</summary>
      protected double[][]? _rcont;


      #region Properties

      /// <summary>
      /// Gets the current value of x.
      /// </summary>
      /// <value>
      /// The current value of x.
      /// </value>
      public double X => _x_current;

      /// <summary>
      /// Gets the value of x before the last step.
      /// </summary>
      /// <value>
      /// The value of x before the last step.
      /// </value>
      public double X_previous => _x_previous;

      /// <summary>
      /// Gets the current values of the variables. <b>Attention:</b> the returned array will change the next time you call <see cref="EvaluateNextSolutionPoint(double)"/>. Therefore,
      /// if you not intend to use the values immediately, <b>make a copy of this array!</b>.
      /// </summary>
      /// <value>
      /// The current values of the variables.
      /// </value>
      public double[] Y_volatile => _y_current;

      /// <summary>
      /// Gets a value indicating whether this instance of the core is initialized.
      /// </summary>
      /// <value>
      /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
      /// </value>
      public bool IsInitialized => _y_current is not null;

      /// <summary>
      /// Gets or sets the absolute tolerance.
      /// </summary>
      /// <value>
      /// The absolute tolerance.
      /// </value>
      /// <exception cref="ArgumentException">Must be &gt;= 0 - AbsoluteTolerance</exception>
      public double AbsoluteTolerance
      {
        get => _absoluteTolerance;
        set
        {
          if (!(value >= 0))
            throw new ArgumentException("Must be >= 0", nameof(AbsoluteTolerance));
          _absoluteTolerance = value;
        }
      }

      /// <summary>
      /// Gets or sets the relative tolerance.
      /// </summary>
      /// <value>
      /// The relative tolerance.
      /// </value>
      /// <exception cref="ArgumentException">Must be >= 0 - RelativeTolerance</exception>
      public double RelativeTolerance
      {
        get => _relativeTolerance;
        set
        {
          if (!(value >= 0))
            throw new ArgumentException("Must be >= 0", nameof(RelativeTolerance));
          _relativeTolerance = value;
        }
      }

      public double StiffnessDetectionThresholdValue
      {
        set
        {
          if (!(value > 0))
            throw new ArgumentException("Must be > 0", nameof(StiffnessDetectionThresholdValue));
          _stiffnessDetectionThresholdValueSquared = value * value;
        }
      }

      /// <summary>
      /// Sets the coefficients for the low order evaluation (used in order to guess the error).
      /// If set to null, the low order y is not evaluated.
      /// </summary>
      /// <value>
      /// The low order coefficients of the Runge-Kutta method.
      /// </value>
      public double[]? BL
      {
        set
        {
          _bhml = value;
        }
      }


      /// <summary>
      /// Gets or sets the interpolation coefficients for dense output.
      /// </summary>
      /// <value>
      /// The interpolation coefficients.
      /// </value>
      public double[][] InterpolationCoefficients
      {
        get => _interpolation_aij;
        set
        {
          _interpolation_aij = value;
        }
      }

      #endregion

      /// <summary>
      /// Initializes a new instance of the <see cref="Core" />.
      /// </summary>
      /// <param name="order">Order of the Runge-Kutta method (the highest order of the embedded pair).</param>
      /// <param name="stages">Number of stages including the stages needed for dense output</param>
      /// <param name="a">The central coefficients of the Runge-Kutta scheme.</param>
      /// <param name="b">The high order bottom side coefficients of the Runge-Kutta scheme used to calculate the next function values.</param>
      /// <param name="bhml">Differences between high order and low order bottom side coefficents of the Runge-Kutta scheme for local error estimation.</param>
      /// <param name="c">The left side coefficients of the Runge-Kutta scheme.</param>
      /// <param name="x0">The initial x value.</param>
      /// <param name="y">The initial y values.</param>
      /// <param name="f">Evaluation function to calculate the derivatives. 1st arg: x, 2nd arg: y, 3rd arg: array to hold the resulting derivatives.</param>
      public Core(int order, int stages, double[][] a, double[] b, double[]? bhml, double[] c, double x0, double[] y, Action<double, double[], double[]> f)
      {
        _order = order;
        _numberOfStages = stages;
        _a = a;
        _b = b;
        _bhml = bhml;
        _c = c;

        _wasSolutionPointEvaluated = false;
        _isFirstSameAsLastMethod = IsFirstSameAsLastMethod(a, b, c);

        _absoluteTolerance = 1E-12;
        _relativeTolerance = 1E-2;

        _x_previous = x0;
        _x_current = x0;
        _stepSize_current = 0;
        _y_previous = Clone(y);
        _y_current = Clone(y);
        _y_current_LocalError = Clone(y);
        _f = f;

        _k = new double[stages + (_isFirstSameAsLastMethod?0:1)][]; // Storage for derivatives at the interval points, for non FSAL method we add one array to accomodate slope at y_current if needed for dense output
        for (int i = 0; i < _k.Length; ++i)
          _k[i] = new double[y.Length];

        _y_stages = new double[y.Length];

        _interpolation_aij = null;
      }

      /// <summary>
      /// Reverts the state of the instance to the former solution point, by
      /// setting <see cref="X"/> to <see cref="X_previous"/> and <see cref="Y_volatile"/> y_previous.
      /// </summary>
      public void Revert()
      {
        _x_current = _x_previous;
        Exchange(ref _y_current, ref _y_previous);
        _wasSolutionPointEvaluated = false; // do not use existing k (derivatives). Instead force calculation of derivative k[0] anew
        _was_Knext_Evaluated = false;
      }

      /// <summary>
      /// Determines whether this is a FSAL (first same at last) Runge-Kutta method. For FSAL methods, the last entry in
      /// array c is 1, and the last row of array a is the same as c.
      /// </summary>
      /// <param name="a">Matrix a.</param>
      /// <param name="b">Array b.</param>
      /// <param name="c">Array c.</param>
      /// <returns>
      ///   <c>true</c> if [is first same at last method] [the specified a]; otherwise, <c>false</c>.
      /// </returns>
      private static bool IsFirstSameAsLastMethod(double[][] a, double[] b, double[] c)
      {

        if (c[c.Length - 1] != 1)
          return false;
        var alast = a[a.Length - 1];
        for (int i = 0; i < alast.Length; ++i)
          if (alast[i] != b[i])
            return false;

        return true;
      }

      /// <summary>
      /// Evaluates the next solution point in one step.
      /// To get the results, see <see cref="X"/> and <see cref="Y_volatile"/>.
      /// </summary>
      /// <param name="stepSize">Size of the step.</param>
      /// <remarks>At the end of this call, <see cref="_y_current"/> contains the current y values,
      /// <see cref="_y_stages"/> contains the y values of the last stage (non-FSAL) or of the stage before
      /// the last stage (FSAL), <see cref="_y_current_LocalError"/> contains the local errors, and <see cref="_y_previous"/>
      /// contains the y values at the beginning of the current step.</remarks>
      public virtual void EvaluateNextSolutionPoint(double stepSize)
      {
        var a = _a;
        var b = _b;
        var c = _c;
        var k = _k;

        int n = _y_current.Length; // number of variables
        int s = a.Length; // number of stages

        var h = stepSize;
        _stepSize_previous = _stepSize_current;
        _stepSize_current = stepSize;
        _x_previous = _x_current;
        _isDenseOutputPrepared = false;
        Exchange(ref _y_previous, ref _y_current); // swap the two arrays => what was current is now previous

        var x_previous = _x_previous;
        var y_previous = _y_previous;

        // calculate the derivatives k0 .. ks-1 (see [1] page 134)

        if (_was_Knext_Evaluated)
        {
          // if this is a FSAL method (e.g. DOPRI), then k[^1] already contains the derivatives, and thus we can reuse the last stage of the previous step
          // instead of copying the values from k[^1] to k[0], we simply exchange the arrays
          // if this is not an FSAL method, but k_next is already evaluated, we use it too, but then it is the array after the last stage
          Exchange(ref k[k.Length - 1], ref k[0]);
        }
        else
        {
          _f(x_previous, y_previous, k[0]); // else we have to calculate the 1st stage
        }

        int fasl_stage = _isFirstSameAsLastMethod ? s - 1 : -1; // if FASL method, then this is the last stage; else it is not relevant
          for (int si = 1; si < s; ++si) // Stages 1.. s
          {
            var asi = a[si];
            var ksim1 = k[si - 1];
          var ydest = si == fasl_stage ? _y_current : _y_stages; // if FASL method, the destination of last stage is y_current, else it is the temporary y array
            for (int ni = 0; ni < n; ++ni) // for all n
            {
              double sum = 0; // TODO test performance if instead of sum we use and array of sum and exchange order of loops
              for (int j = 0; j < si; ++j)
              {
                sum += asi[j] * k[j][ni];
              }
              ydest[ni] = sum * stepSize + y_previous[ni];
            }
            _f(x_previous + h * c[si], ydest, k[si]); // calculate derivative k
          } // end calculation of k0 .. k[s-1]

        if (!_isFirstSameAsLastMethod)
        {
          // Calculate y (high order)
          var y_current = _y_current;
          for (int ni = 0; ni < n; ++ni) // TODO Test if exchanging the order of sums is faster in calculation
          {
            double sum = 0;
            for (int si = 0; si < s; ++si)
            {
              sum += b[si] * k[si][ni];
            }
            y_current[ni] = y_previous[ni] + h * sum;
          }
        }

        // Calculate local error in y
        // the array _bhml must contain the differences between high order and low order bottom side scheme coefficients 
        if (_bhml is not null)
        {
          var bl = _bhml;
          var yl = _y_current_LocalError;
          for (int ni = 0; ni < n; ++ni) // TODO Test if exchanging the order of sums is faster in calculation
          {
            double sum = 0;
            for (int si = 0; si < s; ++si)
            {
              sum += bl[si] * k[si][ni];
            }
            yl[ni] = h * sum;
          }
        }

        _x_current += stepSize;
        _wasSolutionPointEvaluated = true;
        _was_Knext_Evaluated = _isFirstSameAsLastMethod;
      }

      #region Stiffness detection
      /// <summary>
      /// Function that is been called after every <b>successfull</b> step.
      /// Detects a stiffness condition. If it founds one, an exception will be thrown.
      /// </summary>
      /// <returns></returns>
      public virtual void ThrowIfStiffnessDetected()
      {
        if (_stiffnessDetectionEveryNumberOfSteps > 0 &&
             ((++_numberOfRejectedStiffnessDetectionCalls >= _stiffnessDetectionEveryNumberOfSteps) ||
                (_numberOfNonstiffEvaluationResults > 0)
             )
          )
        {
          if(!_was_Knext_Evaluated)
          {
            _was_Knext_Evaluated = true;
            _f(_x_current, _y_current, _k[_numberOfStages]);
          }

          _numberOfRejectedStiffnessDetectionCalls = 0;

          int n = _y_current.Length;
          double sumSquaredSlopeDifferences = 0;
          double sumSquaredValueDifferences = 0;
          double q;
          double h = _stepSize_current;
          for (int ni = 0; ni < n; ++ni)
          {
            q = _k[_numberOfStages][ni] - _k[_numberOfStages-1][ni]; // difference between slope k[6] and the slope k[5]
            sumSquaredSlopeDifferences += q * q;
            q = _y_current[ni] - _y_stages[ni]; // difference of the y at the end of the step used to calc k[6] and the y used to calculate k[5]
            sumSquaredValueDifferences += q * q;
          }

          if (sumSquaredValueDifferences > 0 && (h * h * sumSquaredSlopeDifferences) / sumSquaredValueDifferences > _stiffnessDetectionThresholdValueSquared)
          {
            _numberOfNonstiffEvaluationResults = 0;
            _numberOfStiffEvaluationResults++;
            if (_numberOfStiffEvaluationResults == 15)
            {
              throw new InvalidOperationException($"Stiffness condition detected in ODE at x={_x_current}");
            }
          }
          else
          {
            _numberOfNonstiffEvaluationResults++;
            if (_numberOfNonstiffEvaluationResults == 6)
              _numberOfStiffEvaluationResults = 0;
          }
        }
      }

      #endregion

      #region Error and step size evaluation

      /// <summary>
      /// Gets the relative error, which should be in the order of 1, if the step size is optimally chosen.
      /// </summary>
      /// <returns>The relative error (relative to the absolute and relative tolerance).</returns>
      public virtual double GetRelativeError()
      {
        // Compute error (see [1], page 168
        // error computation in L2 or L-infinity norm is possible
        // here, L-infinity is used

        if (_bhml is null)
        {
          throw new InvalidOperationException("In order to evaluate errors, the evaluation of the low order y has to be done, but the low order coefficients were not set!");
        }

        var ylocalerror = _y_current_LocalError;
        var ycurrent = _y_current;
        var yprevious = _y_previous;

        double e = double.MinValue;
        for (int i = 0; i < ycurrent.Length; ++i)
        {
          e = Math.Max(e, Math.Abs(ylocalerror[i]) / Math.Max(_absoluteTolerance, _relativeTolerance * Math.Max(Math.Abs(ycurrent[i]), Math.Abs(yprevious[i]))));
        }

        return e;
      }

      /// <summary>
      /// Gets the recommended step size.  
      /// </summary>
      /// <param name="error_current">The relative error of the current step.</param>
      /// <param name="error_previous">The relative error of the previous step.</param>
      /// <returns>The recommended step size in the context of the absolute and relative tolerances.</returns>
      public virtual double GetRecommendedStepSize(double error_current, double error_previous)
      {
        // PI-filter. Beta = 0.08
        return error_current == 0 ?
          _stepSize_current * StepSize_MaxFactor :
          _stepSize_current * Math.Min(StepSize_MaxFactor, Math.Max(StepSize_MinFactor, StepSize_SafetyFactor * Math.Pow(1 / error_current, 1.0 / _order) * (error_previous == 0 ? 1 : Math.Pow(error_previous, 0.08d))));
      }

      /// <summary>
      /// Gets the initial step size. The absolute and relative tolerances must be set before the call to this function.
      /// </summary>
      /// <returns>The initial step size in the context of the absolute and relative tolerances.</returns>
      /// <exception cref="InvalidOperationException">Either absolute tolerance or relative tolerance is required to be &gt; 0</exception>
      public virtual double GetInitialStepSize()
      {
        if (!((_absoluteTolerance > 0 && _relativeTolerance >= 0) || (_absoluteTolerance >= 0 && _relativeTolerance > 0)))
          throw new InvalidOperationException($"Either absolute tolerance or relative tolerance is required to be >0");


        var n = _y_current.Length;
        // we re-use the _k array here
        var f0 = _k[0]; // for the derivative at the current point
        var f1 = _k[1]; // derivative at the first guess of the ste size
        var delta = _k[2]; // allowed absolute tolerances
        var ytemp = _y_stages; // guess of y at the first guess of the step size


        _f(_x_current, _y_current, f0); // derivatives at the current point

        double d0 = 0;
        double d1 = 0;
        for (int i = 0; i < n; i++)
        {
          delta[i] = _absoluteTolerance + _relativeTolerance * Math.Abs(_y_current[i]);
          d0 = Math.Max(d0, Math.Abs(_y_current[i]) / delta[i]);
          d1 = Math.Max(d1, Math.Abs(f0[i]) / delta[i]);
        }
        var h0 = Math.Min(d0, d1) < 1e-5 ? 1e-6 : 1e-2 * (d0 / d1);

        // we have to guess y at x + h0 by calculating ytemp = _y_current + h0 * f0
        for (int i = n - 1; i >= 0; --i)
        {
          ytemp[i] = _y_current[i] + h0 * f0[i];
        }

        _f(_x_current + h0, ytemp, f1); // derivatives at the first guess of the step size

        double d2 = 0;
        for (int i = 0; i < n; i++)
        {
          d2 = Math.Max(d2, Math.Abs(f0[i] - f1[i]) / delta[i] / h0);
        }
        return Math.Min(100 * h0, Math.Max(d1, d2) <= 1e-15 ? Math.Max(1e-6, h0 * 1e-3) : Math.Pow(1e-2 / Math.Max(d1, d2), 1.0 / _order));
      }

      #endregion

      #region Interpolation (dense output)

      /// <summary>
      /// The interpolation coefficients. Note that zero to third order is calculated from the y and slopes at the start and end of the step.
      /// Thus, this coefficients only have to cover the orders 4.. n of the interpolation.
      /// </summary>
      protected double[][] _interpolation_aij = new double[0][];

      /// <summary>Get an interpolated point in the last evaluated interval.
      /// Please use the result immediately, or else make a copy of the result, since a internal array
      /// is returned, which is overwritten at the next operation.</summary>
      /// <param name="theta">Relative location (0..1) in the last evaluated interval.</param>
      /// <returns>Interpolated y values at the relative point of the last evaluated interval <paramref name="theta"/>.</returns>
      /// <remarks>This method is intended for FSAL methods only. We assume here, that k[_stages-1] contains
      /// the derivative of x_current.</remarks>
      public virtual double[] GetInterpolatedY_volatile(double theta)
      {
        var k0 = _k[0]; // derivatives at x_previous
        var yprev = _y_previous;
        var ycurr = _y_current;
        var ydest = _y_stages;
        int n = yprev.Length;
        var h = _stepSize_current;

        if (_rcont is null)
        {
          _rcont = new double[4 + _interpolation_aij.Length][];
          for (int i = 0; i < _rcont.Length; ++i)
            _rcont[i] = new double[n];
        }
        var rcont0 = _rcont[0];
        var rcont1 = _rcont[1];
        var rcont2 = _rcont[2];
        var rcont3 = _rcont[3];

        

        // derivatives at x_current
        var k_next = _isFirstSameAsLastMethod ? _k[_numberOfStages - 1] : _k[_numberOfStages];

        if (!_isDenseOutputPrepared)
        {
          _isDenseOutputPrepared = true;

          // for non-FSAL methods, we first need the slope at x_current
          if (!_isFirstSameAsLastMethod && !_was_Knext_Evaluated)
          {
            _was_Knext_Evaluated = true;
            _f(_x_current, _y_current, k_next); // we store the slope in the array after the last stage
          }

          // now calculate the polynomial coefficients for dense interpolation
          double valcont1, valcont2;
          for (int ni = 0; ni < n; ++ni)
          {
            rcont0[ni] = yprev[ni]; // values at begin of step
            rcont1[ni] = valcont1 = _y_current[ni] - yprev[ni]; // values at end of step minus values at begin of step
            rcont2[ni] = valcont2 = h * k0[ni] - valcont1;
            rcont3[ni] = valcont1 - h * k_next[ni] - valcont2;

            // further orders - for that we need the interpolation coefficients
            for (int oi = 0; oi < _interpolation_aij.Length; ++oi)
            {
              var interpolationk = _interpolation_aij[oi];
              double sum = 0;
              for (int ki = 0; ki < _numberOfStages; ++ki)
              {
                sum += interpolationk[ki] * _k[ki][ni];
              }
              _rcont[4+oi][ni] = h * sum;
            }
          }
        }

        var theta1 = 1 - theta;
        switch(_rcont.Length)
        {
          case 4:
            for (int ni = 0; ni < n; ++ni)
            {
              ydest[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni])));
            }
            break;
          case 5:
            for (int ni = 0; ni < n; ++ni)
            {
              ydest[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni] + theta1 * (_rcont[4][ni]))));
            }
            break;
          case 6:
            for (int ni = 0; ni < n; ++ni)
            {
              ydest[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni] + theta1 * (_rcont[4][ni] + theta * (_rcont[5][ni])))));
            }
            break;
          case 7:
            for (int ni = 0; ni < n; ++ni)
            {
              ydest[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni] + theta1 * (_rcont[4][ni] + theta * (_rcont[5][ni] + theta1 * (_rcont[6][ni]))))));
            }
            break;
          case 8:
            for (int ni = 0; ni < n; ++ni)
            {
              ydest[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni] + theta1 * (_rcont[4][ni] + theta * (_rcont[5][ni] + theta1 * (_rcont[6][ni] + theta *(_rcont[7][ni])))))));
            }
            break;
          default:
            {
              throw new NotImplementedException();
            }

        }

        return ydest;
      }

      #endregion
    }
  }
}
