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
    protected struct Core
    {
      private const double StepSize_SafetyFactor = 0.8;
      private const double StepSize_MaxFactor = 5; // Maximum possible step increase
      private const double StepSize_MinFactor = 0.2; // Maximum possible step decrease

      /// <summary>Central coefficients of the Runge-Kutta scheme. See [1], page 135.</summary>
      private double[][] _a;

      /// <summary>High order coefficients (lower side of the Runge-Kutta scheme).</summary>
      private double[] _b;

      /// <summary>Low order coefficients (for error estimation).</summary>
      private double[]? _bl;

      /// <summary>Left side coefficients of the Runge-Kutta scheme.</summary>
      private double[] _c;

      // State variables

      /// <summary>True if at least one solution point was evaluated.</summary>
      private bool _wasSolutionPointEvaluated;

      /// <summary>True if the last point is the same as first point (FSAL property). This is for instance true for the Dormand-Prince (DOPRI) method.</summary>
      private bool _isFirstSameAtLastMethod;

      private double _x_previous;
      private double _x_current;
      private double _stepSize;
      private double[] _y_previous;
      private double[] _y_current;
      private double[] _y_current_lowPrecision;
      public double _absoluteTolerance;
      public double _relativeTolerance;

      private Action<double, double[], double[]> _f;

      // Helper variables

      /// <summary>Array of derivatives at the different stages.</summary>
      private double[][] _k;

      /// <summary>Temporary helper array.</summary>
      private double[] _ytemp;

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
          _bl = value;
        }
      }


      /// <summary>
      /// Gets or sets the interpolation coefficients for dense output.
      /// </summary>
      /// <value>
      /// The interpolation coefficients.
      /// </value>
      public double[][]? InterpolationCoefficients
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
      /// <param name="a">.</param>
      /// <param name="b">The b.</param>
      /// <param name="bl">The bl.</param>
      /// <param name="c">The c.</param>
      /// <param name="x0">The initial x value.</param>
      /// <param name="y">The initial y values.</param>
      /// <param name="f">Evaluation function to calculate the derivatives. 1st arg: x, 2nd arg: y, 3rd arg: array to hold the resulting derivatives.</param>
      public Core(double[][] a, double[] b, double[]? bl, double[] c, double x0, double[] y, Action<double, double[], double[]> f)
      {
        _a = a;
        _b = b;
        _bl = bl;
        _c = c;

        _wasSolutionPointEvaluated = false;
        _isFirstSameAtLastMethod = IsFirstSameAtLastMethod(a, b, c);

        _absoluteTolerance = 1E-12;
        _relativeTolerance = 1E-2;

        _x_previous = x0;
        _x_current = x0;
        _stepSize = 0;
        _y_previous = Clone(y);
        _y_current = Clone(y);
        _y_current_lowPrecision = Clone(y);
        _f = f;

        _k = new double[_a.Length][]; // Storage for derivatives at the interval points
        for (int i = 0; i < _a.Length; ++i)
          _k[i] = new double[y.Length];

        _ytemp = new double[y.Length];

        _interpolation_thetai = null;
        _interpolation_bj = null;
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
      private static bool IsFirstSameAtLastMethod(double[][] a, double[] b, double[] c)
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
      /// Evaluates the next solution point in one step. To get the results, see <see cref="X"/> and <see cref="Y_volatile"/>.
      /// </summary>
      /// <param name="stepSize">Size of the step.</param>
      public void EvaluateNextSolutionPoint(double stepSize)
      {
        var a = _a;
        var b = _b;
        var c = _c;
        var k = _k;

        int n = _y_current.Length; // number of variables
        int s = a.Length; // number of stages

        var h = stepSize;
        _stepSize = stepSize;
        _x_previous = _x_current;
        Exchange(ref _y_previous, ref _y_current); // swap the two arrays => what was current is now previous

        var x_previous = _x_previous;
        var y_previous = _y_previous;

        // calculate the derivatives k0 .. ks-1 (see [1] page 134)

        if (_wasSolutionPointEvaluated && _isFirstSameAtLastMethod)
        {
          // if this is a FASL method (e.g. DOPRI), then k[^1] already contains the derivatives, and thus we can reuse the last stage of the previous step
          // instead of copying the values from k[^1] to k[0], we simply exchange the arrays
          Exchange(ref k[k.Length - 1], ref k[0]);
        }
        else
        {
          _f(x_previous, y_previous, k[0]); // else we have to calculate the 1st stage
        }


        var ysi = _ytemp;
        for (int si = 1; si < s; ++si) // Stages 1.. s
        {
          var asi = a[si];
          var ksim1 = k[si - 1];
          for (int ni = 0; ni < n; ++ni) // for all n
          {
            double sum = 0; // TODO test performance if instead of sum we use and array of sum and exchange order of loops
            for (int j = 0; j < si; ++j)
            {
              sum += asi[j] * k[j][ni];
            }
            ysi[ni] = sum * stepSize + y_previous[ni];
          }
          _f(x_previous + h * c[si], ysi, k[si]); // calculate derivative k
        } // end calculation of k0 .. k[s-1]


        // Calculate y (low order) - for that we use the current y, so here y must not be already updated
        if (_bl is not null)
        {
          var bl = _bl;
          var yl = _y_current_lowPrecision;
          for (int ni = 0; ni < n; ++ni) // TODO Test if exchanging the order of sums is faster in calculation
          {
            double sum = 0;
            for (int si = 0; si < s; ++si)
            {
              sum += bl[si] * k[si][ni];
            }
            yl[ni] = y_previous[ni] + h * sum;
          }
        }

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

        _x_current += stepSize;
        _wasSolutionPointEvaluated = true;
      }

      #region Error and step size evaluation

      /// <summary>
      /// Gets the relative error, which should be in the order of 1, if the step size is optimally chosen.
      /// </summary>
      /// <returns>The relative error (relative to the absolute and relative tolerance).</returns>
      public double GetRelativeError()
      {
        // Compute error (see [1], page 168
        // error computation in L2 or L-infinity norm is possible
        // here, L-infinity is used

        if (_bl is null)
        {
          throw new InvalidOperationException("In order to evaluate errors, the evaluation of the low order y has to be done, but the low order coefficients were not set!");
        }

        var y = _y_current;
        var yl = _y_current_lowPrecision;
        var yp = _y_previous;

        double e = double.MinValue;
        for (int i = 0; i < y.Length; ++i)
        {
          e = Math.Max(e, Math.Abs(y[i] - yl[i]) / Math.Max(_absoluteTolerance, _relativeTolerance * Math.Max(Math.Abs(y[i]), Math.Abs(yp[i]))));
        }

        return e;
      }

      /// <summary>
      /// Gets the recommended step size.  
      /// </summary>
      /// <param name="error_current">The relative error of the current step.</param>
      /// <param name="error_previous">The relative error of the previous step.</param>
      /// <returns>The recommended step size in the context of the absolute and relative tolerances.</returns>
      public double GetRecommendedStepSize(double error_current, double error_previous)
      {
        // PI-filter. Beta = 0.08
        return error_current == 0 ?
          _stepSize :
          _stepSize * Math.Min(StepSize_MaxFactor, Math.Max(StepSize_MinFactor, StepSize_SafetyFactor * Math.Pow(1 / error_current, 1 / 5d) * Math.Pow(error_previous, 0.08d)));
      }

      /// <summary>
      /// Gets the initial step size. The absolute and relative tolerances must be set before the call to this function.
      /// </summary>
      /// <returns>The initial step size in the context of the absolute and relative tolerances.</returns>
      /// <exception cref="InvalidOperationException">Either absolute tolerance or relative tolerance is required to be &gt; 0</exception>
      public double GetInitialStepSize()
      {
        if (!((_absoluteTolerance > 0 && _relativeTolerance >= 0) || (_absoluteTolerance >= 0 && _relativeTolerance > 0)))
          throw new InvalidOperationException($"Either absolute tolerance or relative tolerance is required to be >0");


        var n = _y_current.Length;
        // we re-use the _k array here
        var f0 = _k[0]; // for the derivative at the current point
        var f1 = _k[1]; // derivative at the first guess of the ste size
        var delta = _k[2]; // allowed absolute tolerances
        var ytemp = _ytemp; // guess of y at the first guess of the step size


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
        return Math.Min(100 * h0, Math.Max(d1, d2) <= 1e-15 ? Math.Max(1e-6, h0 * 1e-3) : Math.Pow(1e-2 / Math.Max(d1, d2), 1 / 5d));
      }

      #endregion

      #region Interpolation (dense output)

      // For explanation of the coefficients, see reference [2]

      /// <summary>Temporary array of size 4 intented for interpolation. Contains the theta^i (i=1,2,3,4) from [2], eq.11.</summary>
      private double[]? _interpolation_thetai;

      /// <summary>Temporary array of size N intented for interpolation. Contains the b_j from [2], eq.11.</summary>
      private double[]? _interpolation_bj;

      /// <summary>
      /// The interpolation coefficients aij from [2], eq.11 and unnumbered equation shortly below eq. 12. Values from [2], table 2.
      /// </summary>
      private double[][]? _interpolation_aij;

      /// <summary>Get an interpolated point in the last evaluated interval.
      /// Please use the result immediately, or else make a copy of the result, since a internal array
      /// is returned, which is overwritten at the next operation.</summary>
      /// <param name="theta">Relative location (0..1) in the last evaluated interval.</param>
      /// <returns>Interpolated y values at the relative point of the last evaluated interval <paramref name="theta"/>.</returns>
      /// <remarks>See ref. [2] section 3.3.</remarks>
      public double[] GetInterpolatedY_volatile(double theta)
      {
        var k = _k;
        var y = _y_previous;
        var ys = _ytemp;

        int n = y.Length;


        var aij = _interpolation_aij ?? throw new InvalidOperationException($"This method does not allow interpolation  (interpolation coefficients not known).");
        var bj = (_interpolation_bj ??= new double[k.Length]);

        // Create power 1, 2, 3, and 4 of theta
        // theta, theta², theta³ ..
        var thetai = (_interpolation_thetai ??= new double[aij[0].Length]);
        var th = theta;
        for (int j = 0; j < thetai.Length; ++j)
        {
          thetai[j] = th;
          th *= theta;
        }

        for (int j = 0; j < k.Length; ++j)
        {
          double b = 0;
          for (int i = 3; i >= 0; --i)
          {
            b += aij[j][i] * thetai[i];
          }
          bj[j] = b;
        }

        for (int ni = 0; ni < n; ni++)
        {
          double slope = 0;
          for (int j = 0; j < k.Length; ++j)
          {
            slope += bj[j] * k[j][ni];
          }
          ys[ni] = y[ni] + _stepSize * slope;
        }

        return ys;
      }

      #endregion
    }
  }
}
