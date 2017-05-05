#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

// Modified by D. Lellinger 2017

#endregion Copyright

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Altaxo.Calc.Ode.Temp
{
  public class GearsBDF
  {
    private static double ToleranceNorm(IReadOnlyList<double> v, double RTol, double ATol, IReadOnlyList<double> a)
    {
      return v.LInfinityNorm() / (ATol + RTol * a.LInfinityNorm());
    }

    /// <summary>
    /// Calculates the jacobian, and stores all the temporary arrays and matrices neccessary for calculation.
    /// </summary>
    private struct JacobianEvaluator
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
      private double[][] f_new;

      private double[] variation;

      private DoubleMatrix J;

      private IROMatrix<double> J_ROWrapper;

      private Action<double, double[], double[]> f;

      public JacobianEvaluator(int N, Action<double, double[], double[]> f)
      {
        variatedX = new double[N];
        f_old = new double[N];
        f_new = new DoubleMatrix(N, N).data;
        variation = new double[N];
        J = new DoubleMatrix(N, N);
        J_ROWrapper = J.ToROMatrix();
        this.f = f;
      }

      /// <summary>Compute the Jacobian</summary>
      /// <param name="f">The derivative function. 1st arg is time, 2nd arg are current y, and the rates are returned in the 3rd arg.</param>
      /// <param name="x"></param>
      /// <param name="t">Current time.</param>
      /// <param name="J">The resulting Jacobian matrix.</param>
      /// <returns></returns>
      public IROMatrix<double> Jacobian(double t, double[] x)
      {
        int N = variatedX.Length;
        Array.Copy(x, variatedX, N);

        f(t, x, f_old); // evaluate rates at old point x

        for (int i = 0; i < N; ++i)
        {
          variatedX[i] += (variation[i] = Math.Sqrt(1e-6 * Math.Max(1e-5, Math.Abs(x[i]))));
          f(t, variatedX, f_new[i]); // calculate rates at x variated at index i
          variatedX[i] = x[i]; // restore old state
        }

        var jarray = J.data;
        for (int i = 0; i < N; ++i)
        {
          for (int j = 0; j < N; ++j)
          {
            jarray[i][j] = (f_new[j][i] - f_old[i]) / (variation[j]);
          }
        }

        return J_ROWrapper;
      }
    }

    /// <summary>
    /// Representation of current state in Nordsieck's method
    /// </summary>
    internal class NordsieckState
    {
      /// <summary>At the end of a predictor step, this is
			/// still the time of the previous state (!),
			/// the current time is tn + dt.</summary>
      public double tn;

      /// <summary>Current variable of this state.</summary>
      public double[] xn;

      /// <summary>Time difference to next state.</summary>
      public double dt;

      public double Dq;
      public double delta;

      /// <summary>Current method order, from 1 to qmax</summary>
      public int qn;

      /// <summary>Maximum method order, from 1 to 5</summary>
      public readonly int qmax;

      /// <summary>Successfull steps count</summary>
      public int nsuccess;

      /// <summary>Step size scale factor/// </summary>
      public double rFactor;

      public double[] en;
      private double[] ecurr; // we need this temporary variable, must not be shared among states!

      public double epsilon = 1e-12;

      // temporary variables
      private GaussianEliminationSolver gaussSolver = new GaussianEliminationSolver();

      private DoubleMatrix zcurr = new DoubleMatrix(1, 1); // n x (qmax+1)
      private DoubleMatrix z0 = new DoubleMatrix(1, 1); // n x (qmax+1)
      public DoubleMatrix zn; // n x (qmax+1)

      private DoubleMatrix P; // n x n

      private double[] ftdt; // to store the result of f(t + dt, x)
      private double[] colExtract; // to store the result GetColumn
      private double[] tmpVec1; // other temporary variable

      private double[] xprev; // length: n
      private double[] gm; // length n
      private double[] deltaE; // length n

      /// <summary>
      /// Initializes a new instance of the <see cref="NordsieckState"/> class.
      /// </summary>
      /// <param name="n">The number of variables of the ODE.</param>
      /// <param name="qmax">Maximum order of polynomial.</param>
      /// <param name="qcurr">Current order.</param>
      /// <param name="dt">Initial time step.</param>
      /// <param name="t0">Initial time.</param>
      /// <param name="x0">Initial state variables.</param>
      /// <param name="dxdt0">Initial derivatives of state variables.</param>
      public NordsieckState(int n, int qmax, int qcurr, double dt, double t0, double[] x0, double[] dxdt0)
      {
        if (n < 1)
          throw new ArgumentOutOfRangeException(nameof(n));

        this.qmax = qmax;
        this.qn = qcurr;
        this.delta = 0;
        this.Dq = 0;
        this.dt = dt;
        this.tn = t0;
        this.nsuccess = 0;
        this.rFactor = 1;

        // Copy x0
        this.xn = new double[n];
        VectorMath.Copy(x0, xn);

        //Compute Nordstieck's history matrix at t=t0;
        zn = new DoubleMatrix(n, qmax + 1);
        for (int i = 0; i < n; i++)
        {
          zn[i, 0] = x0[i]; // x0
          zn[i, 1] = dt * dxdt0[i]; // 1st derivative * dt
        }

        zcurr = new DoubleMatrix(n, qmax + 1);
        z0 = new DoubleMatrix(n, qmax + 1);

        P = new DoubleMatrix(n, n);

        en = new double[n];
        ecurr = new double[n];

        ftdt = new double[n]; // to store the result of f(t + dt, x)
        colExtract = new double[n]; // to store the result GetColumn
        tmpVec1 = new double[n]; // other temporary variable

        xprev = new double[n]; // length: n
        gm = new double[n]; // length n
        deltaE = new double[n]; // length n
      }

      public void DivideStepBy2()
      {
        dt = dt / 2.0;
        if (dt < epsilon)
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
        int q = zn.Columns;

        for (int j = 1; j < q; ++j)
        {
          R *= r;
          for (int i = 0; i < zn.Rows; ++i)
          {
            zn[i, j] *= R;
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
      /// <param name="arg">previous value of Nordsieck's matrix, so-called Z(n-1)</param>
      /// <returns>So-called Zn0, initial vaue of Z in new step</returns>
      public void ZNew()
      {
        int q = zn.Columns;
        int n = zn.Rows;

        for (int k = 0; k < q - 1; k++)
        {
          for (int j = q - 1; j > k; j--)
          {
            for (int i = 0; i < n; i++)
            {
              zn[i, j - 1] = zn[i, j] + zn[i, j - 1];
            }
          }
        }
      }

      /// <summary>
      /// Execute predictor-corrector scheme for Nordsieck's method
      /// </summary>
      /// <param name="currstate"></param>
      /// <param name="flag"></param>
      /// <param name="f">Evaluation of the deriatives. First argument is time, second arg are the state variables, and 3rd arg is the array to accomodate the derivatives.</param>
      /// <param name="jacobianEvaluator">Evaluation of the jacobian.</param>
      /// <param name="opts">current options</param>
      /// <returns>en - current error vector</returns>
      internal void PredictorCorrectorScheme(
        ref bool flag,
        Action<double, double[], double[]> f,
        Func<double, double[], IROMatrix<double>> jacobianEvaluator,
        Options opts,
        NordsieckState newstate
        )
      {
        NordsieckState currstate = this;
        int n = currstate.xn.Length;

        // allocate local variables
        if (zcurr.Rows != currstate.zn.Rows || zcurr.Columns != currstate.zn.Columns)
          zcurr = new DoubleMatrix(currstate.zn.Rows, currstate.zn.Columns);
        if (z0.Rows != currstate.zn.Rows || z0.Columns != currstate.zn.Columns)
          z0 = new DoubleMatrix(currstate.zn.Rows, currstate.zn.Columns);

        VectorMath.Copy(currstate.en, ecurr);
        VectorMath.Copy(currstate.en, newstate.en);

        var xcurr = currstate.xn;
        var x0 = currstate.xn;
        MatrixMath.Copy(currstate.zn, zcurr); // zcurr now is old nordsieck matrix
        var qcurr = currstate.qn; // current degree
        var qmax = currstate.qmax; // max degree
        var dt = currstate.dt;
        var t = currstate.tn;
        MatrixMath.Copy(currstate.zn, z0); // save Nordsieck matrix

        //Tolerance computation factors
        double Cq = Math.Pow(qcurr + 1, -1.0);
        double tau = 1.0 / (Cq * Factorial(qcurr) * l[qcurr - 1][qcurr]);

        int count = 0;

        double Dq = 0.0, DqUp = 0.0, DqDown = 0.0;
        double delta = 0.0;

        //Scaling factors for the step size changing
        //with new method order q' = q, q + 1, q - 1, respectively
        double rSame, rUp, rDown;

        if (opts.SparseJacobian == null)
        {
          var J = opts.Jacobian == null ? jacobianEvaluator(t + dt, xcurr) : opts.Jacobian;
          MatrixMath.Map(J, (x, i, j) => (i == j ? 1 : 0) - x * dt * b[qcurr - 1], P); // B = Identity - J*dt*b[qcurr-1]

          do
          {
            VectorMath.Copy(xcurr, xprev);
            f(t + dt, xcurr, ftdt);
            MatrixMath.CopyColumn(z0, 1, colExtract); // 1st derivative/dt
            VectorMath.Map(ftdt, colExtract, ecurr, (ff, c, e) => dt * ff - c - e, gm); // gm = dt * f(t + dt, xcurr) - z0.GetColumn(1) - ecurr;
            gaussSolver.SolveDestructive(P, gm, tmpVec1);
            VectorMath.Add(ecurr, tmpVec1, ecurr); //	ecurr = ecurr + P.SolveGE(gm);
            VectorMath.Map(x0, ecurr, (x, e) => x + e * b[qcurr - 1], xcurr); //	xcurr = x0 + b[qcurr - 1] * ecurr;

            //Row dimension is smaller than zcurr has
            int M_Rows = ecurr.Length;
            int M_Columns = l[qcurr - 1].Length;
            //So, "expand" the matrix
            MatrixMath.Map(z0, (z, i, j) => z + (i < M_Rows && j < M_Columns ? ecurr[i] * l[qcurr - 1][j] : 0.0d), zcurr);

            Dq = ToleranceNorm(ecurr, opts.RelativeTolerance, opts.AbsoluteTolerance, xprev);
            var factor_deltaE = (1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1]);
            VectorMath.Map(ecurr, currstate.en, (e, c) => (e - c) * factor_deltaE, deltaE); // deltaE = (ecurr - currstate.en)*(1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1])

            DqUp = ToleranceNorm(deltaE, opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            DqDown = ToleranceNorm(zcurr.GetColumn(qcurr - 1), opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            delta = Dq / (tau / (2 * (qcurr + 2)));
            count++;
          } while (delta > 1.0d && count < opts.NumberOfIterations);
        }
        else
        {
          SparseDoubleMatrix J = opts.SparseJacobian;
          SparseDoubleMatrix P = SparseDoubleMatrix.Identity(n, n) - J * dt * b[qcurr - 1];

          do
          {
            VectorMath.Copy(xcurr, xprev);
            f(t + dt, xcurr, ftdt);
            MatrixMath.CopyColumn(z0, 1, colExtract);
            VectorMath.Map(ftdt, colExtract, ecurr, (ff, c, e) => dt * ff - c - e, gm); // gm = dt * f(t + dt, xcurr) - z0.GetColumn(1) - ecurr;
            gaussSolver.SolveDestructive(P, gm, tmpVec1);
            VectorMath.Add(ecurr, tmpVec1, ecurr); //	ecurr = ecurr + P.SolveGE(gm);
            VectorMath.Map(x0, ecurr, (x, e) => x + e * b[qcurr - 1], xcurr); // xcurr = x0 + b[qcurr - 1] * ecurr;

            //Row dimension is smaller than zcurr has
            var M = DoubleMatrix.FromProductOf(ecurr, l[qcurr - 1]);
            //So, "expand" the matrix
            var MBig = DoubleMatrix.CreateIdentity(zcurr.Rows, zcurr.Columns);
            for (int i = 0; i < zcurr.Rows; i++)
            {
              for (int j = 0; j < zcurr.Columns; j++)
              {
                MBig[i, j] = i < M.Rows && j < M.Columns ? M[i, j] : 0.0d;
              }
            }
            zcurr = z0 + MBig;
            Dq = ToleranceNorm(ecurr, opts.RelativeTolerance, opts.AbsoluteTolerance, xprev);
            var factor_deltaE = (1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1]);
            VectorMath.Map(ecurr, currstate.en, (e, c) => (e - c) * factor_deltaE, deltaE); // deltaE = (ecurr - currstate.en)*(1.0 / (qcurr + 2) * l[qcurr - 1][qcurr - 1])

            DqUp = ToleranceNorm(deltaE, opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            DqDown = ToleranceNorm(zcurr.GetColumn(qcurr - 1), opts.RelativeTolerance, opts.AbsoluteTolerance, xcurr);
            delta = Dq / (tau / (2 * (qcurr + 2)));
            count++;
          } while (delta > 1.0d && count < opts.NumberOfIterations);
        }

        //======================================

        var nsuccess = count < opts.NumberOfIterations ? currstate.nsuccess + 1 : 0;

        if (count < opts.NumberOfIterations)
        {
          flag = false;
          MatrixMath.Copy(zcurr, newstate.zn);
          zcurr.CopyColumn(0, newstate.xn);
          VectorMath.Copy(ecurr, newstate.en);
        }
        else
        {
          flag = true;
          MatrixMath.Copy(currstate.zn, newstate.zn);
          currstate.zn.CopyColumn(0, newstate.xn);
          VectorMath.Copy(currstate.en, newstate.en);
        }

        //Compute step size scaling factors
        rUp = 0.0;

        if (currstate.qn < currstate.qmax)
        {
          rUp = rUp = 1.0 / 1.4 / (Math.Pow(DqUp, 1.0 / (qcurr + 2)) + 1e-6);
        }

        rSame = 1.0 / 1.2 / (Math.Pow(Dq, 1.0 / (qcurr + 1)) + 1e-6);

        rDown = 0.0;

        if (currstate.qn > 1)
        {
          rDown = 1.0 / 1.3 / (Math.Pow(DqDown, 1.0 / (qcurr)) + 1e-6);
        }

        //======================================
        newstate.nsuccess = nsuccess >= currstate.qn ? 0 : nsuccess;
        //Step size scale operations

        if (rSame >= rUp)
        {
          if (rSame <= rDown && nsuccess >= currstate.qn && currstate.qn > 1)
          {
            newstate.qn = currstate.qn - 1;
            newstate.Dq = DqDown;

            for (int i = 0; i < n; i++)
            {
              for (int j = newstate.qn + 1; j < qmax + 1; j++)
              {
                newstate.zn[i, j] = 0.0;
              }
            }
            nsuccess = 0;
            newstate.rFactor = rDown;
          }
          else
          {
            newstate.qn = currstate.qn;
            newstate.Dq = Dq;
            newstate.rFactor = rSame;
          }
        }
        else
        {
          if (rUp >= rDown)
          {
            if (rUp >= rSame && nsuccess >= currstate.qn && currstate.qn < currstate.qmax)
            {
              newstate.qn = currstate.qn + 1;
              newstate.Dq = DqUp;
              newstate.rFactor = rUp;
              nsuccess = 0;
            }
            else
            {
              newstate.qn = currstate.qn;
              newstate.Dq = Dq;
              newstate.rFactor = rSame;
            }
          }
          else
          {
            if (nsuccess >= currstate.qn && currstate.qn > 1)
            {
              newstate.qn = currstate.qn - 1;
              newstate.Dq = DqDown;

              for (int i = 0; i < n; i++)
              {
                for (int j = newstate.qn + 1; j < qmax + 1; j++)
                {
                  newstate.zn[i, j] = 0.0;
                }
              }
              nsuccess = 0;
              newstate.rFactor = rDown;
            }
            else
            {
              newstate.qn = currstate.qn;
              newstate.Dq = Dq;
              newstate.rFactor = rSame;
            }
          }
        }

        newstate.dt = dt;
        newstate.tn = t;
      }
    }

    // Vector l for Nordsieck algorithm (orders 1 to 5)
    private static double[][] l = new double[6][] {
            new double[] { 1, 1 },
            new double[] { 2 / 3d, 1, 1 / 3d },
            new double[] { 6 / 11d, 1, 6 / 11d, 1 / 11d },
            new double[] { 24 / 50d, 1, 35 / 50d, 10 / 50d, 1 / 50d },
            new double[] { 120 / 274d, 1, 225 / 274d, 85 / 274d, 15 / 274d, 1 / 274d },
            new double[] { 720 / 1764d, 1, 1624 / 1764d, 735 / 1764d, 175 / 1764d, 21 / 1764d, 1 / 1764d }
        };

    // Vector Beta for Nordsieck algorithm (orders 1 to 5)
    //private static double[] b = new double[] { 720 / 1764d, 1, 1624 / 1764d, 735 / 1764d, 175 / 1764d, 21 / 1764d, 1/1764d };
    private static double[] b = new double[] { 1.0d, 2.0d / 3.0d, 6.0d / 11.0d, 24.0d / 50.0d, 120.0d / 274.0d, 720.0 / 1764.0d };

    private NordsieckState currstate;
    private NordsieckState nextstate;

    private bool isIterationFailed;
    private int n;
    private Options opts;
    private double tout = double.NaN;
    private JacobianEvaluator jacobian;
    private DoubleMatrix _zn_saved; // used to save zn during iteration

    /// <summary>
    /// Evaluates the derivatives. First argument is the time, second arg are the current y values. The derivatives
    /// must be provided in the third argument.
    /// </summary>
    private Action<double, double[], double[]> f;

    private double[] xout;
		private double last_tout = double.NaN;
    private double r;


		// some diagnostic methods

		public int CurrentDegree { get { return currstate.qn; } }

		public double CurrentTime { get { return currstate.tn + currstate.dt; } }

		public double CurrentTimeStep { get { return currstate.dt; } }

		public DoubleMatrix CurrentNordsieckMatrix { get { return currstate.zn; } }
    /// <summary>
    /// Initialize Gear's BDF method with dynamically changed step size and order. Order changes between 1 and 3.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="y0">Initial values (at time <paramref name="t0"/>).</param>
    /// <param name="dydt">Evaluation function for the derivatives. First argument is the time, second argument are the current y values. The third argument is an array where the derivatives are expected to be placed into.</param>
    /// <returns>Sequence of infinite number of solution points.</returns>
    public void Initialize(double t0, double[] y0, Action<double, double[], double[]> dydt)
    {
      Initialize(t0, y0, dydt, Options.Default);
    }

    /// <summary>
    /// Implementation of Gear's BDF method with dynamically changed step size and order. Order changes between 1 and 3.
    /// </summary>
    /// <param name="t0">Initial time point</param>
    /// <param name="x0">Initial phase vector</param>
    /// <param name="f">Right parts of the system</param>
    /// <param name="opts">Options for accuracy control and initial step size</param>
    /// <returns>Sequence of infinite number of solution points.</returns>
    public void Initialize(double t0, double[] x0, Action<double, double[], double[]> f, Options opts)
    {
      jacobian = new JacobianEvaluator(x0.Length, f);

      double t = t0;
      var x = (double[])x0.Clone();

      var dx = (double[])x0.Clone(); // just to get the array.

      this.n = x0.Length;

      this.f = f;
      this.opts = opts;

      //Initial step size.
      f(t0, x0, dx); // rates now in dx
      double dt;
      if (opts.InitialStep != 0)
      {
        dt = opts.InitialStep;
      }
      else
      {
        var tol = opts.RelativeTolerance;
        var ewt = new double[n];
        var ywt = new double[n];
        var sum = 0.0;
        for (int i = 0; i < n; i++)
        {
          ewt[i] = opts.RelativeTolerance * Math.Abs(x[i]) + opts.AbsoluteTolerance;
          ywt[i] = ewt[i] / tol;
          sum = sum + (double)dx[i] * dx[i] / (ywt[i] * ywt[i]);
        }

        dt = Math.Sqrt(tol / ((double)1.0d / (ywt[0] * ywt[0]) + sum / n));
      }

      dt = Math.Min(dt, opts.MaxStep);
      var resdt = dt;

      int qmax = 5;
      int qcurr = 2;

      _zn_saved = new DoubleMatrix(n, qmax + 1);

      currstate = new NordsieckState(n, qmax, qcurr, dt, t, x0, dx);

      nextstate = new NordsieckState(n, qmax, qcurr, dt, t, x0, dx);

      isIterationFailed = false;

      this.tout = t0;
      this.xout = (double[])x0.Clone();

      // ---------------------------------------------------------------------------------------------------
      // End of initialize
      // ---------------------------------------------------------------------------------------------------

      // Firstly, return initial point
      Evaluate(t0, true, out t0, xout);
    }

    public void Evaluate(double? t_sol, out double t_result, double[] result)
    {
      if (t_sol.HasValue && !(t_sol.Value >= tout))
        throw new ArgumentOutOfRangeException(nameof(t_sol), "t must be greater than or equal than the last evaluated t");

      Evaluate(t_sol, false, out t_result, result);
    }

    private void Evaluate(double? tout, bool isFirstEvaluation, out double t_result, double[] result)
    {
     
			

      if (!isFirstEvaluation)
      {
				// we have to clone some of the code from below to here
				// this is no good style, but a goto statement with a jump inside another code block will not work here.

				if (tout.HasValue)
				{
					// Output data, but only if (i) we have requested a certain time point,
					// and ii) as long as we can interpolate this point from the previous point and the current point
					if (tout.HasValue && currstate.tn <= tout.Value && tout.Value <= currstate.tn + currstate.dt)
					{
						VectorMath.Lerp(tout.Value, currstate.tn, xout, currstate.tn + currstate.dt, currstate.xn, result);
						last_tout = t_result = tout.Value;
						return;
					}
				}
				else
				{
					if(double.IsNaN(last_tout)) // if true then return initial point
					{
						last_tout = t_result = currstate.tn;
						VectorMath.Copy(xout, result);
						return;
					}
					else if(currstate.tn==last_tout)
					{
						last_tout = t_result = currstate.tn + currstate.dt;
						VectorMath.Copy(currstate.xn, result);
						return;
					}
				}

        VectorMath.Copy(currstate.xn, xout); // save x of this step

        currstate.tn = currstate.tn + currstate.dt;

        if (opts.MaxStep < Double.MaxValue)
        {
          r = Math.Min(r, opts.MaxStep / currstate.dt);
        }

        if (opts.MinStep > 0)
        {
          r = Math.Max(r, opts.MinStep / currstate.dt);
        }

        r = Math.Min(r, opts.MaxScale);
        r = Math.Max(r, opts.MinScale);

        currstate.dt = currstate.dt * r;

        currstate.Rescale(r);
      }

      //Can produce any number of solution points
      while (true)
      {
        // Reset fail flag
        isIterationFailed = false;

        // Predictor step
        _zn_saved.CopyFrom(currstate.zn);
        currstate.ZNew();
        VectorMath.FillWith(currstate.en, 0); // TODO find out if this statement is neccessary
        currstate.zn.CopyColumn(0, currstate.xn);

        // Corrector step
        currstate.PredictorCorrectorScheme(ref isIterationFailed, f, jacobian.Jacobian, opts, nextstate);

        var temp_state = currstate;
        currstate = nextstate;
        nextstate = temp_state;

        if (isIterationFailed) // If iterations are not finished - bad convergence
        {
          currstate.zn.CopyFrom(_zn_saved); // copy saved state back
          currstate.nsuccess = 0;
          currstate.DivideStepBy2();
        }
        else // Iterations finished, i.e. did not fail
        {
          r = Math.Min(1.1d, Math.Max(0.2d, currstate.rFactor));

					if (currstate.delta >= 1.0d)
					{
						if (opts.MaxStep < Double.MaxValue)
						{
							r = Math.Min(r, opts.MaxStep / currstate.dt);
						}

						if (opts.MinStep > 0)
						{
							r = Math.Max(r, opts.MinStep / currstate.dt);
						}

						r = Math.Min(r, opts.MaxScale);
						r = Math.Max(r, opts.MinScale);

						currstate.dt = currstate.dt * r; // Decrease step
						currstate.Rescale(r);
					}
					else // Iteration finished successfully
					{
						// Output data
						if (tout.HasValue)
						{
							if (currstate.tn <= tout.Value && tout.Value <= currstate.tn + currstate.dt)
							{
								VectorMath.Lerp(tout.Value, currstate.tn, xout, currstate.tn + currstate.dt, currstate.xn, result);
								t_result = tout.Value;
								
								return;
							}
						}
						else
						{
							VectorMath.Copy(currstate.xn, result);
							t_result = currstate.tn + currstate.dt;
							if(!isFirstEvaluation)
								last_tout = t_result;
							
							return;
						}


            VectorMath.Copy(currstate.xn, xout);

            currstate.tn = currstate.tn + currstate.dt;

            if (opts.MaxStep < Double.MaxValue)
            {
              r = Math.Min(r, opts.MaxStep / currstate.dt);
            }

            if (opts.MinStep > 0)
            {
              r = Math.Max(r, opts.MinStep / currstate.dt);
            }

            r = Math.Min(r, opts.MaxScale);
            r = Math.Max(r, opts.MinScale);

            currstate.dt = currstate.dt * r;
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
      if (arg < 0) return -1;
      if (arg == 0) return 1;
      return arg * Factorial(arg - 1);
    }
  }
}