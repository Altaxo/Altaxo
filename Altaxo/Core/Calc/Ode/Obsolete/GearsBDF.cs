#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

// Heavily modified by Dr. Dirk Lellinger 2017

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Gears BDF ODE solver.
  /// </summary>
  public class GearsBDF
  {
    #region Members

    private static double ToleranceNorm(IReadOnlyList<double> v, double RTol, double ATol, IReadOnlyList<double> a)
    {
      return v.LInfinityNorm() / (ATol + RTol * a.LInfinityNorm());
    }

    /// <summary>
    /// Calculates the jacobian, and stores all the temporary arrays and matrices neccessary for calculation.
    /// </summary>
    private class DenseJacobianEvaluator
    {
      /// <summary>
      /// Temporary array to hold the x variated in one index to get the rates of the new point.
      /// </summary>
      private double[] variatedX;

      /// <summary>
      /// The rates at the old point x.
      /// </summary>
      private double[] f_old;

      /// <summary>
      /// The rates at the variated point x + variation at index i.
      /// </summary>
      private double[] f_new;

      private DoubleMatrix J;

      private IROMatrix<double> J_ROWrapper;

      private Action<double, double[], double[]> f;

      public DenseJacobianEvaluator(int N, Action<double, double[], double[]> f)
      {
        variatedX = new double[N];
        f_old = new double[N];
        f_new = new double[N];
        J = new DoubleMatrix(N, N);
        J_ROWrapper = J.ToROMatrix();
        this.f = f;
      }

      /// <summary>Compute the Jacobian</summary>
      /// <param name="t">Current time.</param>
      /// <param name="y">Current value of the variables of the ODE.</param>
      /// <returns>The (approximated) Jacobian matrix.</returns>
      public IROMatrix<double> Jacobian(double t, double[] y)
      {
        int N = variatedX.Length;
        Array.Copy(y, variatedX, N);

        f(t, y, f_old); // evaluate rates at old point x

        double variation;
        var jarray = J.data;
        for (int i = 0; i < N; ++i)
        {
          variatedX[i] += (variation = Math.Sqrt(1e-6 * Math.Max(1e-5, Math.Abs(y[i]))));
          f(t, variatedX, f_new); // calculate rates at x variated at index i
          variatedX[i] = y[i]; // restore old state

          for (int c = 0; c < N; ++c)
          {
            jarray[c][i] = (f_new[c] - f_old[c]) / (variation);
          }
        }

        return J_ROWrapper;
      }
    }

    /// <summary>
    /// Calculates the jacobian, and stores all the temporary arrays and matrices neccessary for calculation.
    /// </summary>
    private class SparseJacobianEvaluator
    {
      /// <summary>
      /// Temporary array to hold the x variated in one index to get the rates of the new point.
      /// </summary>
      private double[] variatedX;

      /// <summary>
      /// The rates at the old point x.
      /// </summary>
      private double[] f_old;

      /// <summary>
      /// The rates at the variated point x + variation at index i.
      /// </summary>
      private double[] f_new;

      private SparseDoubleMatrix J;

      private Action<double, double[], double[]> f;

      public SparseJacobianEvaluator(int N, Action<double, double[], double[]> f)
      {
        variatedX = new double[N];
        f_old = new double[N];
        f_new = new double[N];
        J = new SparseDoubleMatrix(N, N);
        this.f = f;
      }

      /// <summary>Compute the Jacobian</summary>
      /// <param name="t">Current time.</param>
      /// <param name="y">Current value of the variables of the ODE.</param>
      /// <returns>The (approximated) Jacobian matrix.</returns>
      public SparseDoubleMatrix Jacobian(double t, double[] y) // TODO replace, if available, with a read-only version of sparse matrix
      {
        int N = variatedX.Length;
        Array.Copy(y, variatedX, N);

        f(t, y, f_old); // evaluate rates at old point x

        double variation;
        double val;

        J.Clear(); // set all elements to zero

        for (int i = 0; i < N; ++i)
        {
          variatedX[i] += (variation = Math.Sqrt(1e-6 * Math.Max(1e-5, Math.Abs(y[i]))));
          f(t, variatedX, f_new); // calculate rates at x variated at index i
          variatedX[i] = y[i]; // restore old state

          for (int c = 0; c < N; ++c)
          {
            val = (f_new[c] - f_old[c]) / (variation);

            if (!(0 == val))
              J[c, i] = val;
          }
        }

        return J;
      }
    }

    /// <summary>
    /// Representation of current state in Nordsieck's method
    /// </summary>
    private class NordsieckState
    {
#nullable disable
      /// <summary>At the end of a predictor step, this is
      /// still the time of the previous state (!),
      /// the current time is tn + dt.</summary>
      public double _tn;

      /// <summary>Current variable of this state.</summary>
      public double[] _xn;

      /// <summary>Time difference to next state.</summary>
      public double _dt;

      public double _Dq;
      public double _delta;

      /// <summary>Current method order, from 1 to qmax</summary>
      public int _qn;

      /// <summary>Maximum method order, from 1 to 5</summary>
      public readonly int _qmax;

      /// <summary>Successfull steps count</summary>
      public int _nsuccess;

      /// <summary>Step size scale factor/// </summary>
      public double _rFactor;

      public double[] _en;

      public double epsilon = 1e-12;

      // temporary variables
      private GaussianEliminationSolver gaussSolver = new GaussianEliminationSolver();

      public DoubleMatrix _zn; // n x (qmax+1)

      // following could be local variables, but because they require allocation, they are
      // allocated once in the constructor
      private IMatrix<double> P; // n x n

      private double[] ecurr; // we need this temporary variable, must not be shared among states!
      private DoubleMatrix zcurr = new DoubleMatrix(1, 1); // n x (qmax+1)
      private DoubleMatrix z0 = new DoubleMatrix(1, 1); // n x (qmax+1)
      private double[] ftdt; // to store the result of f(t + dt, x)
      private double[] colExtract; // to store the result GetColumn
      private double[] tmpVec1; // other temporary variable

      private double[] xprev; // length: n
      private double[] xcurr; // length: n
      private double[] gm; // length n
      private double[] deltaE; // length n

#nullable enable
      /// <summary>
      /// Initializes a new instance of the <see cref="NordsieckState"/> class.
      /// </summary>
      /// <param name="n">The number of variables of the ODE.</param>
      /// <param name="qmax">Maximum order of polynomial.</param>
      /// <param name="qcurr">Current order.</param>
      /// <param name="dt">Initial time step.</param>
      /// <param name="t0">Initial time.</param>
      /// <param name="x0">Initial state variables.</param>
      /// <param name="dydt0">Initial derivatives of state variables.</param>
      public NordsieckState(int n, int qmax, int qcurr, double dt, double t0, double[] x0, double[] dydt0)
      {
        if (n < 1)
          throw new ArgumentOutOfRangeException(nameof(n));

        _qmax = qmax;
        _qn = qcurr;
        _delta = 0;
        _Dq = 0;
        _nsuccess = 0;
        _rFactor = 1;
        _xn = new double[n];
        _zn = new DoubleMatrix(n, qmax + 1);
        zcurr = new DoubleMatrix(n, qmax + 1);
        z0 = new DoubleMatrix(n, qmax + 1);

        _en = new double[n];
        ecurr = new double[n];

        ftdt = new double[n]; // to store the result of f(t + dt, x)
        colExtract = new double[n]; // to store the result GetColumn
        tmpVec1 = new double[n]; // other temporary variable

        xprev = new double[n]; // length: n
        xcurr = new double[n]; // length: n
        gm = new double[n]; // length n
        deltaE = new double[n]; // length n

        Reinitialize(dt, t0, x0, dydt0);
      }

      internal void Reinitialize(double dt, double t0, double[] x0, double[] dydt0)
      {
        _dt = dt;
        _tn = t0;

        // Copy x0
        Array.Copy(x0, _xn, _xn.Length);

        //Compute Nordstieck's history matrix at t=t0;
        for (int i = 0; i < _xn.Length; i++)
        {
          _zn[i, 0] = x0[i]; // x0
          _zn[i, 1] = dt * dydt0[i]; // 1st derivative * dt
        }
      }

      /// <summary>
      /// Evaluates the y values at a provided time, which should be smaller than tn+dt.
      /// The y-values are calculated from the approximating polynomial, whose coefficients
      /// are stored in zn.
      /// </summary>
      /// <param name="time">The time.</param>
      /// <param name="y_result">The y result.</param>
      public void EvaluateYAtTime(double time, double[] y_result)
      {
        var relativeTime = (time - _tn - _dt) / _dt; // relativeTime should be in the range [-1..0], because time should be <= (tn+dt)
        var zndata = _zn.data; // direct access to zn
        for (int i = 0; i < y_result.Length; ++i)
        {
          double sum = zndata[i][_qn];
          for (int j = _qn - 1; j >= 0; --j)
          {
            sum *= relativeTime;
            sum += zndata[i][j];
          }
          y_result[i] = sum;
        }
      }

      public void DivideStepBy2()
      {
        _dt = _dt / 2.0;
        if (_dt < epsilon)
          throw new ArgumentException("Cannot generate numerical solution");
        Rescale(0.5);
      }

      /// <summary>
      /// The following function rescales Nordsieck's matrix in more effective way than
      /// only compute two matrixes. Current algorithm is taken from book of
      /// Krishnan Radhakrishnan and Alan C.Hindmarsh "Description and Use of LSODE,
      /// the Livermore Solver of Ordinary Differential Equations"
      /// </summary>
      /// <param name="r">Scale factor (new time step)/(old time step)</param>
      /// <returns>Rescaled history matrix</returns>
      public void Rescale(double r)
      {
        double R = 1;
        int q = _zn.ColumnCount;

        for (int j = 1; j < q; ++j)
        {
          R *= r;
          for (int i = 0; i < _zn.RowCount; ++i)
          {
            _zn[i, j] *= R;
          }
        }
      }

      /// <summary>
      /// The following function compute Nordsieck's matrix Zn0 = Z(n-1)*A
      /// in more effective way than just multiply two matrixes.
      ///  Current algoritm is taken from book of
      /// Krishnan Radhakrishnan and Alan C.Hindmarsh "Description and Use of LSODE,
      /// the Livermore Solver of Ordinary Differential Equations"
      /// </summary>
      public void ZNew()
      {
        int q = _zn.ColumnCount;
        int n = _zn.RowCount;
        var zn = _zn.InternalData.Array;

        for (int k = 0; k < q - 1; k++)
        {
          for (int j = q - 1; j > k; j--)
          {
            for (int i = 0; i < n; i++)
            {
              zn[i][j - 1] = zn[i][j] + zn[i][j - 1];
            }
          }
        }
      }
      [MemberNotNull(nameof(P))]
      private void AllocatePMatrixForJacobian(IROMatrix<double> J)
      {
        switch (J)
        {
          case IROSparseMatrix<double> sm:
            P = new SparseDoubleMatrix(J.RowCount, J.ColumnCount);
            break;

          case IROBandMatrix<double> bm:
            P = new BandDoubleMatrix(J.RowCount, J.ColumnCount, bm.LowerBandwidth, bm.UpperBandwidth);
            break;

          case DoubleMatrix dm:
            P = new DoubleMatrix(J.RowCount, J.ColumnCount);
            break;

          case IROMatrix<double> rm:
            P = new DoubleMatrix(J.RowCount, J.ColumnCount);
            break;

          default:
            throw new NotImplementedException(string.Format("Jacobian is a matrix of type {0}, which is not implemented here!", J.GetType()));
        }
      }

      /// <summary>
      /// Execute predictor-corrector scheme for Nordsieck's method
      /// </summary>
      /// <param name="flag"></param>
      /// <param name="f">Evaluation of the deriatives. First argument is time, second arg are the state variables, and 3rd arg is the array to accomodate the derivatives.</param>
      /// <param name="denseJacobianEvaluation">Evaluation of the jacobian.</param>
      /// <param name="sparseJacobianEvaluation">Evaluation of the jacobian as a sparse matrix. Either this or the previous arg must be valid.</param>
      /// <param name="opts">current options</param>
      /// <returns>en - current error vector</returns>
      internal void PredictorCorrectorScheme(
          ref bool flag,
          Action<double, double[], double[]> f,
          Func<double, double[], IROMatrix<double>> denseJacobianEvaluation,
          Func<double, double[], SparseDoubleMatrix> sparseJacobianEvaluation,
          GearsBDFOptions opts
          )
      {
        NordsieckState currstate = this;
        NordsieckState newstate = this;
        int n = currstate._xn.Length;

        VectorMath.Copy(currstate._en, ecurr);
        VectorMath.Copy(currstate._xn, xcurr);
        var x0 = currstate._xn;
        MatrixMath.Copy(currstate._zn, zcurr); // zcurr now is old nordsieck matrix
        var qcurr = currstate._qn; // current degree
        var qmax = currstate._qmax; // max degree
        var dt = currstate._dt;
        var t = currstate._tn;
        MatrixMath.Copy(currstate._zn, z0); // save Nordsieck matrix

        //Tolerance computation factors
        double Cq = Math.Pow(qcurr + 1, -1.0);
        double tau = 1.0 / (Cq * Factorial(qcurr) * l[qcurr - 1][qcurr]);

        int count = 0;

        double Dq = 0.0, DqUp = 0.0, DqDown = 0.0;
        double delta = 0.0;

        //Scaling factors for the step size changing
        //with new method order q' = q, q + 1, q - 1, respectively
        double rSame, rUp, rDown;

        if (denseJacobianEvaluation is not null)
        {
          var J = denseJacobianEvaluation(t + dt, xcurr);
          if (J.GetType() != P?.GetType())
            AllocatePMatrixForJacobian(J);

          do
          {
            MatrixMath.MapIndexed(J, dt * b[qcurr - 1], (i, j, aij, factor) => (i == j ? 1 : 0) - aij * factor, P, Zeros.AllowSkip); // P = Identity - J*dt*b[qcurr-1]
            VectorMath.Copy(xcurr, xprev);
            f(t + dt, xcurr, ftdt);
            MatrixMath.CopyColumn(z0, 1, colExtract); // 1st derivative/dt
            VectorMath.Map(ftdt, colExtract, ecurr, dt, (ff, c, e, local_dt) => local_dt * ff - c - e, gm); // gm = dt * f(t + dt, xcurr) - z0.GetColumn(1) - ecurr;
            gaussSolver.SolveDestructive(P, gm, tmpVec1);
            VectorMath.Add(ecurr, tmpVec1, ecurr); //	ecurr = ecurr + P.SolveGE(gm);
            VectorMath.Map(x0, ecurr, b[qcurr - 1], (x, e, local_b) => x + e * local_b, xcurr); //	xcurr = x0 + b[qcurr - 1] * ecurr;

            //Row dimension is smaller than zcurr has
            int M_Rows = ecurr.Length;
            int M_Columns = l[qcurr - 1].Length;
            //So, "expand" the matrix
            MatrixMath.MapIndexed(z0, (i, j, z) => z + (i < M_Rows && j < M_Columns ? ecurr[i] * l[qcurr - 1][j] : 0.0d), zcurr);

            Dq = ToleranceNorm(ecurr, opts.RelativeTolerance, opts.AbsoluteTolerance, xprev);
            var factor_deltaE = (1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1]);
            VectorMath.Map(ecurr, currstate._en, factor_deltaE, (e, c, factor) => (e - c) * factor, deltaE); // deltaE = (ecurr - currstate.en)*(1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1])

            DqUp = ToleranceNorm(deltaE, opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            zcurr.CopyColumn(qcurr - 1, colExtract);
            DqDown = ToleranceNorm(colExtract, opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            delta = Dq / (tau / (2 * (qcurr + 2)));
            count++;
          } while (delta > 1.0d && count < opts.NumberOfIterations);
        }
        else if (sparseJacobianEvaluation is not null)
        {
          SparseDoubleMatrix J = sparseJacobianEvaluation(t + dt, xcurr);
          var P = new SparseDoubleMatrix(J.RowCount, J.ColumnCount);

          do
          {
            J.MapSparseIncludingDiagonal((x, i, j) => (i == j ? 1 : 0) - x * dt * b[qcurr - 1], P);
            VectorMath.Copy(xcurr, xprev);
            f(t + dt, xcurr, ftdt);
            MatrixMath.CopyColumn(z0, 1, colExtract);
            VectorMath.Map(ftdt, colExtract, ecurr, (ff, c, e) => dt * ff - c - e, gm); // gm = dt * f(t + dt, xcurr) - z0.GetColumn(1) - ecurr;
            gaussSolver.SolveDestructive(P, gm, tmpVec1);
            VectorMath.Add(ecurr, tmpVec1, ecurr); //	ecurr = ecurr + P.SolveGE(gm);
            VectorMath.Map(x0, ecurr, (x, e) => x + e * b[qcurr - 1], xcurr); // xcurr = x0 + b[qcurr - 1] * ecurr;

            //Row dimension is smaller than zcurr has
            int M_Rows = ecurr.Length;
            int M_Columns = l[qcurr - 1].Length;
            //So, "expand" the matrix
            MatrixMath.MapIndexed(z0, (i, j, z) => z + (i < M_Rows && j < M_Columns ? ecurr[i] * l[qcurr - 1][j] : 0.0d), zcurr);

            Dq = ToleranceNorm(ecurr, opts.RelativeTolerance, opts.AbsoluteTolerance, xprev);
            var factor_deltaE = (1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1]);
            VectorMath.Map(ecurr, currstate._en, (e, c) => (e - c) * factor_deltaE, deltaE); // deltaE = (ecurr - currstate.en)*(1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1])

            DqUp = ToleranceNorm(deltaE, opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            DqDown = ToleranceNorm(zcurr.GetColumn(qcurr - 1), opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            delta = Dq / (tau / (2 * (qcurr + 2)));
            count++;
          } while (delta > 1.0d && count < opts.NumberOfIterations);
        }
        else // neither denseJacobianEvaluation nor sparseJacobianEvaluation valid
        {
          throw new ArgumentNullException(nameof(denseJacobianEvaluation), "Either denseJacobianEvaluation or sparseJacobianEvaluation must be set!");
        }

        //======================================

        var nsuccess = count < opts.NumberOfIterations ? currstate._nsuccess + 1 : 0;

        if (count < opts.NumberOfIterations)
        {
          flag = false;
          MatrixMath.Copy(zcurr, newstate._zn);
          zcurr.CopyColumn(0, newstate._xn);
          VectorMath.Copy(ecurr, newstate._en);
        }
        else
        {
          flag = true;
          // MatrixMath.Copy(currstate.zn, newstate.zn); // null operation since currstate and newstate are identical
          currstate._zn.CopyColumn(0, newstate._xn);
          VectorMath.Copy(currstate._en, newstate._en); // null operation since currstate and newstate are identical
        }

        //Compute step size scaling factors
        rUp = 0.0;

        if (currstate._qn < currstate._qmax)
        {
          rUp = rUp = 1.0 / 1.4 / (Math.Pow(DqUp, 1.0 / (qcurr + 2)) + 1e-6);
        }

        rSame = 1.0 / 1.2 / (Math.Pow(Dq, 1.0 / (qcurr + 1)) + 1e-6);

        rDown = 0.0;

        if (currstate._qn > 1)
        {
          rDown = 1.0 / 1.3 / (Math.Pow(DqDown, 1.0 / (qcurr)) + 1e-6);
        }

        //======================================
        _nsuccess = nsuccess >= _qn ? 0 : nsuccess;
        //Step size scale operations

        if (rSame >= rUp)
        {
          if (rSame <= rDown && nsuccess >= _qn && _qn > 1)
          {
            _qn = _qn - 1;
            _Dq = DqDown;

            for (int i = 0; i < n; i++)
            {
              for (int j = newstate._qn + 1; j < qmax + 1; j++)
              {
                _zn[i, j] = 0.0;
              }
            }
            nsuccess = 0;
            _rFactor = rDown;
          }
          else
          {
            // _qn = _qn;
            _Dq = Dq;
            _rFactor = rSame;
          }
        }
        else
        {
          if (rUp >= rDown)
          {
            if (rUp >= rSame && nsuccess >= _qn && _qn < _qmax)
            {
              _qn = _qn + 1;
              _Dq = DqUp;
              _rFactor = rUp;
              nsuccess = 0;
            }
            else
            {
              // _qn = _qn;
              _Dq = Dq;
              _rFactor = rSame;
            }
          }
          else
          {
            if (nsuccess >= _qn && _qn > 1)
            {
              _qn = _qn - 1;
              _Dq = DqDown;

              for (int i = 0; i < n; i++)
              {
                for (int j = newstate._qn + 1; j < qmax + 1; j++)
                {
                  _zn[i, j] = 0.0;
                }
              }
              nsuccess = 0;
              _rFactor = rDown;
            }
            else
            {
              // _qn = _qn;
              _Dq = Dq;
              _rFactor = rSame;
            }
          }
        }

        _dt = dt;
        _tn = t;
      }
    }

#nullable disable
    // Vector l for Nordsieck algorithm (orders 1 to 5)
    private static readonly double[][] l = new double[6][] {
                        new double[] { 1, 1 },
                        new double[] { 2 / 3d, 1, 1 / 3d },
                        new double[] { 6 / 11d, 1, 6 / 11d, 1 / 11d },
                        new double[] { 24 / 50d, 1, 35 / 50d, 10 / 50d, 1 / 50d },
                        new double[] { 120 / 274d, 1, 225 / 274d, 85 / 274d, 15 / 274d, 1 / 274d },
                        new double[] { 720 / 1764d, 1, 1624 / 1764d, 735 / 1764d, 175 / 1764d, 21 / 1764d, 1 / 1764d }
                };

    // Vector Beta for Nordsieck algorithm (orders 1 to 5)
    //private static double[] b = new double[] { 720 / 1764d, 1, 1624 / 1764d, 735 / 1764d, 175 / 1764d, 21 / 1764d, 1/1764d };
    private static readonly double[] b = new double[] { 1.0d, 2.0d / 3.0d, 6.0d / 11.0d, 24.0d / 50.0d, 120.0d / 274.0d, 720.0 / 1764.0d };

    private NordsieckState currstate;

    private bool isIterationFailed;
    private int n;
    private GearsBDFOptions opts;
    private double tout = double.NaN;

    /// <summary>
    /// Calculates the jacobian as a dense matrix. 1st arg is time, 2nd arg is the state variable vector. Result is the dense jacobian matrix.
    /// </summary>
    /// <remarks>Either this field or <see cref="_sparseJacobianEvaluation"/> must be set!</remarks>
    private Func<double, double[], IROMatrix<double>> _denseJacobianEvaluation;

    /// <summary>
    /// Calculates the jacobian as a sparse matrix. 1st arg is time, 2nd arg is the state variable vector. Result is the sparse jacobian matrix.
    /// </summary>
    /// <remarks>Either this field or <see cref="_denseJacobianEvaluation"/> must be set!</remarks>
    private Func<double, double[], SparseDoubleMatrix> _sparseJacobianEvaluation;

    private DoubleMatrix _zn_saved; // used to save zn during iteration

    /// <summary>
    /// Evaluates the derivatives. First argument is the time, second arg are the current y values. The derivatives
    /// must be provided in the third argument.
    /// </summary>
    private Action<double, double[], double[]> f;

    private enum InitializationState { NotInitialized, InitialValueReturned, Initialized };

    private InitializationState _initializationState; // true when the Nordsiek matrix is valid
    private double[] xout;
    private double last_tout = double.NaN;
    private double r;

#nullable enable

    #endregion Members

    /// <summary>
    /// Gets the current degree of the interpolating polynomial.
    /// </summary>
    /// <value>
    /// The current degree of the interpolating polynomial.
    /// </value>
    public int CurrentDegree { get { return currstate._qn; } }

    /// <summary>
    /// Gets the time until which the ODE is already evaluated.
    /// </summary>
    /// <value>
    /// The current time.
    /// </value>
    public double CurrentTime { get { return currstate._tn + currstate._dt; } }

    /// <summary>
    /// Gets the current time step. This is the step which was carried out from the previous time to the current time.
    /// </summary>
    /// <value>
    /// The current time step.
    /// </value>
    public double CurrentTimeStep { get { return currstate._dt; } }

    /// <summary>
    /// Gets or sets the maximum time step.
    /// </summary>
    /// <value>
    /// The maximum time step.
    /// </value>
    public double MaximumTimeStep { get { return opts.MaxStep; } set { opts.MaxStep = value; } }

    /// <summary>
    /// Gets the current nordsieck matrix. Rows corresponds to the state variables y0..yn, colums are the
    /// variable y, then y'dt, y''dt²/2, y'''dt³/6, and so on. Can be used to get the coefficients of the interpolating
    /// polynomial(s) between <see cref="CurrentTime"/>-<see cref="CurrentTimeStep"/> to <see cref="CurrentTime"/>.
    /// </summary>
    /// <value>
    /// The current nordsieck matrix.
    /// </value>
    public DoubleMatrix CurrentNordsieckMatrix { get { return currstate._zn; } }

    /// <summary>
    /// Initialize Gear's BDF method with dynamically changed step size and order.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="y0">Initial values (at time <paramref name="t0"/>).</param>
    /// <param name="dydt">Evaluation function for the derivatives. First argument is the time, second argument are the current y values. The third argument is an array where the derivatives are expected to be placed into.</param>
    public void Initialize(double t0, double[] y0, Action<double, double[], double[]> dydt)
    {
      if (y0 is null)
        throw new ArgumentNullException(nameof(y0));
      if (dydt is null)
        throw new ArgumentNullException(nameof(dydt));

      _denseJacobianEvaluation = new DenseJacobianEvaluator(y0.Length, dydt).Jacobian;
      InternalInitialize(t0, y0, dydt, GearsBDFOptions.Default);
    }

    /// <summary>
    /// Initialize Gear's BDF method with dynamically changed step size and order.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="y0">Initial values (at time <paramref name="t0"/>).</param>
    /// <param name="dydt">Evaluation function for the derivatives. First argument is the time, second argument are the current y values. The third argument is an array where the derivatives are expected to be placed into.</param>
    /// <param name="opts">Options for the ODE method (can be null).</param>
    public void Initialize(double t0, double[] y0, Action<double, double[], double[]> dydt, GearsBDFOptions opts)
    {
      if (y0 is null)
        throw new ArgumentNullException(nameof(y0));
      if (dydt is null)
        throw new ArgumentNullException(nameof(dydt));

      _denseJacobianEvaluation = new DenseJacobianEvaluator(y0.Length, dydt).Jacobian;
      InternalInitialize(t0, y0, dydt, opts ?? GearsBDFOptions.Default);
    }

    /// <summary>
    /// Initialize Gear's BDF method with dynamically changed step size and order.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="y0">Initial values (at time <paramref name="t0"/>).</param>
    /// <param name="dydt">Evaluation function for the derivatives. First argument is the time, second argument are the current y values. The third argument is an array where the derivatives are expected to be placed into.</param>
    /// <param name="denseJacobianEvaluator">Evaluation for the dense jacobian matrix. First argument is the time, second argument are the current y values. If null is passed for this argument, a default evaluator is used.</param>
    /// <param name="opts">Options for the ODE method (can be null).</param>
    public void Initialize(double t0, double[] y0, Action<double, double[], double[]> dydt, Func<double, double[], IROMatrix<double>> denseJacobianEvaluator, GearsBDFOptions opts)
    {
      if (y0 is null)
        throw new ArgumentNullException(nameof(y0));
      if (dydt is null)
        throw new ArgumentNullException(nameof(dydt));

      _denseJacobianEvaluation = denseJacobianEvaluator ?? new DenseJacobianEvaluator(y0.Length, dydt).Jacobian;
      InternalInitialize(t0, y0, dydt, opts ?? GearsBDFOptions.Default);
    }

    /// <summary>
    /// Initialize Gear's BDF method with dynamically changed step size and order.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="y0">Initial values (at time <paramref name="t0"/>).</param>
    /// <param name="dydt">Evaluation function for the derivatives. First argument is the time, second argument are the current y values. The third argument is an array where the derivatives are expected to be placed into.</param>
    /// <param name="sparseJacobianEvaluation">Evaluation for the dense jacobian matrix. First argument is the time, second argument are the current y values. If null is passed for this argument, a default evaluator is used.</param>
    /// <param name="opts">Options for the ODE method (can be null).</param>
    public void InitializeSparse(double t0, double[] y0, Action<double, double[], double[]> dydt, Func<double, double[], SparseDoubleMatrix> sparseJacobianEvaluation, GearsBDFOptions opts)
    {
      if (y0 is null)
        throw new ArgumentNullException(nameof(y0));
      if (dydt is null)
        throw new ArgumentNullException(nameof(dydt));

      _sparseJacobianEvaluation = sparseJacobianEvaluation ?? new SparseJacobianEvaluator(y0.Length, dydt).Jacobian;
      InternalInitialize(t0, y0, dydt, opts ?? GearsBDFOptions.Default);
    }

    // Arrays needed for initialization and at discontinuities
    // All arrays have length n
#nullable disable
    private double[] _dydt;
    private double[] _ewt;
    private double[] _ywt;
#nullable enable

    /// <summary>
    /// Implementation of Gear's BDF method with dynamically changed step size and order. Order changes between 1 and 3.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="x0">Initial phase vector</param>
    /// <param name="f">Right parts of the system</param>
    /// <param name="opts">Options for accuracy control and initial step size</param>
    /// <returns>Sequence of infinite number of solution points.</returns>
    private void InternalInitialize(double t0, double[] x0, Action<double, double[], double[]> f, GearsBDFOptions opts)
    {
      if (_denseJacobianEvaluation is null && _sparseJacobianEvaluation is null)
        throw new InvalidProgramException("Ooops, how could this happen?");

      double t = t0;
      var x = (double[])x0.Clone();
      n = x0.Length;

      this.f = f;
      this.opts = opts;

      //Initial step size.
      _dydt = _dydt ?? new double[n];
      var dt = EvaluateRatesAndGetDt(t0, x0, _dydt);
      var resdt = dt;

      int qmax = 5;
      int qcurr = 2;

      _zn_saved = new DoubleMatrix(n, qmax + 1);

      currstate = new NordsieckState(n, qmax, qcurr, dt, t, x0, _dydt);

      isIterationFailed = false;

      tout = t0;
      xout = (double[])x0.Clone();

      // ---------------------------------------------------------------------------------------------------
      // End of initialize
      // ---------------------------------------------------------------------------------------------------

      // Firstly, return initial point
      // EvaluateInternally(t0, true, out t0, xout);
      _initializationState = InitializationState.NotInitialized;
    }

    /// <summary>
    /// Marks a discontinuity at the last point that was evaluated.
    /// </summary>
    /// <remarks>This function is indended to be used if discontinuous boundary conditions exist. The function evaluates
    /// the rate at the last point, and effectively restarts the ODE. See example for details.
    /// Example: if a discontinuity exist at t=1000,
    /// then
    /// then (i) evaluate the ODE until t=1000,
    /// then (ii) change the boundary conditions,
    /// and then (iii) call this function.</remarks>
    public void MarkDiscontinuity()
    {
      var t0 = last_tout;
      var x0 = xout;

      _dydt = _dydt ?? new double[n];
      var dt = EvaluateRatesAndGetDt(t0, x0, _dydt);
      currstate.Reinitialize(dt, t0, xout, _dydt);
      _initializationState = InitializationState.NotInitialized;
    }

    private double EvaluateRatesAndGetDt(double t0, double[] x0, double[] dydt)
    {
      f(t0, x0, dydt); // rates now in dx

      double dt;
      if (opts.InitialStep != 0)
      {
        dt = opts.InitialStep;
      }
      else
      {
        _ewt = _ewt ?? new double[n];
        _ywt = _ywt ?? new double[n];

        var tol = opts.RelativeTolerance;
        var sum = 0.0;
        for (int i = 0; i < n; i++)
        {
          _ewt[i] = opts.RelativeTolerance * Math.Abs(x0[i]) + opts.AbsoluteTolerance;
          _ywt[i] = _ewt[i] / tol;
          sum = sum + dydt[i] * dydt[i] / (_ywt[i] * _ywt[i]);
        }

        dt = Math.Sqrt(tol / (1.0d / (_ywt[0] * _ywt[0]) + sum / n));
      }

      dt = Math.Min(dt, opts.MaxStep);
      return dt;
    }

    public void Evaluate(out double t_result, double[] result)
    {
      EvaluateInternally(null, out t_result, result);
    }

    public void Evaluate(double t_sol, double[] result)
    {
      if (!(t_sol >= tout))
        throw new ArgumentOutOfRangeException(nameof(t_sol), "t must be greater than or equal than the last evaluated t");

      EvaluateInternally(t_sol, out var _, result);
    }

    public IEnumerable<(double time, IROVector<double> y)> SolutionPointsUntil(double maxTime)
    {
      var resultArray = new double[n];
      var roWrapper = VectorMath.ToROVector(resultArray);

      for (; ; )
      {
        EvaluateInternally(null, out var time, resultArray);

        if (!(time <= maxTime))
          break;
        yield return (time, roWrapper);
      }
    }

    private void EvaluateInternally(double? tout, out double t_result, double[] result)
    {
      if (_initializationState == InitializationState.NotInitialized) // not initialized so far
      {
        _initializationState = InitializationState.InitialValueReturned;

        if (tout is null)
        {
          last_tout = t_result = currstate._tn;
          currstate._zn.CopyColumn(0, result);
          return;
        }
      }
      else if (_initializationState == InitializationState.Initialized)
      {
        // we have to clone some of the code from below to here
        // this is no good style, but a goto statement with a jump inside another code block will not work here.

        if (tout.HasValue)
        {
          // Output data, but only if (i) we have requested a certain time point,
          // and ii) as long as we can interpolate this point from the previous point and the current point
          if (currstate._tn <= tout.Value && tout.Value <= currstate._tn + currstate._dt)
          {
            // VectorMath.Lerp(tout.Value, currstate.tn, xout, currstate.tn + currstate.dt, currstate.xn, result);
            currstate.EvaluateYAtTime(tout.Value, result);
            last_tout = t_result = tout.Value;
            return;
          }
        }
        else
        {
          if (currstate._tn == last_tout)
          {
            last_tout = t_result = currstate._tn + currstate._dt;
            VectorMath.Copy(currstate._xn, result);
            return;
          }
        }

        VectorMath.Copy(currstate._xn, xout); // save x of this step

        currstate._tn = currstate._tn + currstate._dt;

        if (opts.MaxStep < double.MaxValue)
        {
          r = Math.Min(r, opts.MaxStep / currstate._dt);
        }

        if (opts.MinStep > 0)
        {
          r = Math.Max(r, opts.MinStep / currstate._dt);
        }

        r = Math.Min(r, opts.MaxScale);
        r = Math.Max(r, opts.MinScale);

        currstate._dt = currstate._dt * r;

        currstate.Rescale(r);
      }

      _initializationState = InitializationState.Initialized;
      //Can produce any number of solution points
      while (true)
      {
        // Reset fail flag
        isIterationFailed = false;

        // Predictor step
        _zn_saved.CopyFrom(currstate._zn);
        currstate.ZNew();
        VectorMath.FillWith(currstate._en, 0); // TODO find out if this statement is neccessary
        currstate._zn.CopyColumn(0, currstate._xn);

        // Corrector step
        currstate.PredictorCorrectorScheme(ref isIterationFailed, f, _denseJacobianEvaluation, _sparseJacobianEvaluation, opts);

        if (isIterationFailed) // If iterations are not finished - bad convergence
        {
          currstate._zn.CopyFrom(_zn_saved); // copy saved state back
          currstate._nsuccess = 0;
          currstate.DivideStepBy2();
        }
        else // Iterations finished, i.e. did not fail
        {
          r = Math.Min(1.1d, Math.Max(0.2d, currstate._rFactor));

          if (currstate._delta >= 1.0d)
          {
            if (opts.MaxStep < double.MaxValue)
            {
              r = Math.Min(r, opts.MaxStep / currstate._dt);
            }

            if (opts.MinStep > 0)
            {
              r = Math.Max(r, opts.MinStep / currstate._dt);
            }

            r = Math.Min(r, opts.MaxScale);
            r = Math.Max(r, opts.MinScale);

            currstate._dt = currstate._dt * r; // Decrease step
            currstate.Rescale(r);
          }
          else // Iteration finished successfully
          {
            // Output data
            if (tout.HasValue)
            {
              if (currstate._tn <= tout.Value && tout.Value <= currstate._tn + currstate._dt)
              {
                // VectorMath.Lerp(tout.Value, currstate.tn, xout, currstate.tn + currstate.dt, currstate.xn, result);
                currstate.EvaluateYAtTime(tout.Value, result);
                t_result = tout.Value;

                return;
              }
            }
            else
            {
              VectorMath.Copy(currstate._xn, result);
              t_result = last_tout = currstate._tn + currstate._dt;
              return;
            }

            VectorMath.Copy(currstate._xn, xout);

            currstate._tn = currstate._tn + currstate._dt;

            if (opts.MaxStep < double.MaxValue)
            {
              r = Math.Min(r, opts.MaxStep / currstate._dt);
            }

            if (opts.MinStep > 0)
            {
              r = Math.Max(r, opts.MinStep / currstate._dt);
            }

            r = Math.Min(r, opts.MaxScale);
            r = Math.Max(r, opts.MinScale);

            currstate._dt = currstate._dt * r;
            currstate.Rescale(r);
          }
        }
      }
    }

    /// <summary>
    /// Compute factorial
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal static int Factorial(int arg)
    {
      switch (arg)
      {
        case 0:
          return 1;

        case 1:
          return 1;

        case 2:
          return 2;

        case 3:
          return 6;

        case 4:
          return 24;

        case 5:
          return 120;

        case 6:
          return 720;

        default:
          throw new NotImplementedException();
      }
    }
  }
}
