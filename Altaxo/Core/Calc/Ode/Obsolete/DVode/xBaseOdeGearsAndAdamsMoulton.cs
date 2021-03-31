#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete.DVode
{
  /// <summary>
  /// Represents a base class for the Adams-Moulton and Gear’s BDF classes.
  /// </summary>
  public abstract class xBaseOdeGearsAndAdamsMoulton : xOdeBase
  {
    #region Fields

    private DVODE _dvode = new DVODE();
#nullable disable
    internal FEX _fex;
    internal JEX _jex;
#nullable enable

    internal enum ODEType { NonStiff, Stiff }

    //private double MeT = 0d;

    private ODEType _Type = ODEType.NonStiff;

    private bool _UserJacobian = false;

    /// <summary>
    ///MeITask=1 means normal computation of output values of y(t) at t = TOUT (by overshooting and interpolating).
    /// </summary>
    private int _ITask = 1;

    /// <summary>
    /// MeIState= an index used for input and output to specify the the state of the calculation.
    /// In the input, the values of ISTATE are as follows.
    /// MeIState=1  means this is the first call for the problem (initializations will be done).
    /// MeIState=2  means this is not the first call, and the calculation is to continue normally.
    /// In the output, ISTATE has the following values and meanings.
    /// MeIState=1 means nothing was done, as TOUT was equal to T with ISTATE = 1 in the input.
    /// MeIState=2  means the integration was performed successfully.
    /// MeIState .LT. 0 Error
    /// MeIState=-1 means an excessive amount of work (more than MXSTEP steps) was done on this call, before completing the
    /// requested task, but the integration was otherwise successful as far as T.  (MXSTEP is an optional input and is normally 500.)
    /// MeIState=-2  means too much accuracy was requested for the precision of the machine being used.
    /// MeIState=-3  means illegal input was detected
    /// </summary>
    private int _IState = 1;

    /// <summary>
    /// MeIOpt= An integer flag to specify whether or not any optional input is being used on this call.
    /// MeIOpt= 0 means no optional input is being used.
    /// </summary>
    private int _IOpt = 0;

    /// <summary>
    /// MeMf= The method flag.
    /// MeMf=10, NonStiff.
    /// MeMf=21 Stiff  con Jacobiano.
    /// MeMf=22 Stiff sin Jacobiano.
    /// </summary>
    private int _Mf;

    //private OdeFun MeFunction;
    //OdeJac MeJacobian;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initialize the ODE solver without a Jacobian
    /// </summary>
    /// <param name="Func">The function that define the ODEs.</param>
    /// <param name="type">The Ode type (stiff, nonstiff).</param>
    /// <param name="numEquations">The number of equatins.</param>
    internal void InitializationWithoutJacobian(OdeFunction Func, ODEType type, int numEquations)
    {
      //dvode = new DVODE();

      _IState = 1;
      _UserJacobian = false;
      _Type = type;

      OdeJacobian? jac = null;

      base.InitializeInternal(Func, jac, numEquations);
    }

    /// <summary>
    /// Inicialize the ODE solver with a Jacobiano
    /// </summary>
    /// <param name="Func">The function that define the ODEs.</param>
    /// <param name="Jac">The Ode type (stiff, nonstiff).</param>
    /// <param name="numEquations">The number of equatins.</param>
    protected internal void InitializationWithJacobian(OdeFunction Func, OdeJacobian Jac, int numEquations)
    {
      //dvode = new DVODE();

      _IState = 1;
      _UserJacobian = true;
      _Type = ODEType.Stiff;

      base.InitializeInternal(Func, Jac, numEquations);
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    /// Computes the solution of the differential equations at a point <paramref name="tEnd"/>.
    /// </summary>
    /// <param name="tEnd">Value of t where the solution is required.</param>
    /// <returns>A array containing the solution of the differential equations at the point <paramref name="tEnd"/>.</returns>
    public double[] Solve(double tEnd)
    {
      CheckInitialization();

      bool WasSuccessfully = true;
      _dvode.Run(_fex, _NEquations, ref _Y, 0, ref _T0, tEnd, _ITolAdamsGears,
          _RelTolArray, 0, _AbsTolArray, 0,
          _ITask, ref _IState, _IOpt, ref _RWork, 0,
          _Lrw, ref _IWork, 0, _Liw, _jex, _Mf, _RPar, 0,
          _IPar, 0);
      if (_IState < 0)
        WasSuccessfully = false;

      if (WasSuccessfully == false)
      {
        throw new Exception(_Errors[-_IState]);
      }

      return _Y;
    }

    /// <summary>
    ///  Computes the solution of the differntial equations.
    /// </summary>
    /// <param name="y0">A vector of size N containing the initial conditions ( at t0). N is the number of differential equations.</param>
    /// <param name="tspan">A vector specifying the interval of integration (t0,..,tf).</param>
    /// <returns>
    /// A matrix that contains the solution of the differential equations [T, y1,..,yN].
    /// The first column contains the time points and each row corresponds to the solution at a time returned in the corresponding row.
    /// </returns>
    public double[,] Solve(double[] y0, double[] tspan)
    {
      CheckTArray(tspan);

      SetInitialValues(tspan[0], y0);

      double[,] solution = new double[tspan.Length, _NEquations + 1];

      double[] tempY;

      double time = 0;

      for (int i = 0; i < tspan.Length; i++)
      {
        time = tspan[i];
        tempY = Solve(time);

        solution[i, 0] = time;

        for (int j = 0; j < tempY.Length; j++)
        {
          solution[i, j + 1] = tempY[j];
        }
      }

      return solution;
    }

    /// <summary>
    /// Computes the solution of the differntial equations.
    /// </summary>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    /// <param name="t0">The initial independent variable value.</param>
    /// <param name="deltaT">The step for the interval of integration (t0, t0+deltaT, t0+2*deltaT,...,tf).</param>
    /// <param name="tf">The final independent variable value.</param>
    /// <returns>
    /// A matrix that contains the solution of the differential equations [T, y1,..,yN].
    /// The first column contains the time points and each row corresponds to the solution at a time returned in the corresponding row.
    /// </returns>
    public double[,] Solve(double[] y0, double t0, double deltaT, double tf)
    {
      CheckArguments(t0, deltaT, tf);

      SetInitialValues(t0, y0);

      int maxIndex = (int)(Math.Abs(tf - t0) / Math.Abs(deltaT)) + 1;

      double[,] solution = new double[maxIndex, _NEquations + 1];

      double[] tempY;
      double time = t0;

      for (int i = 0; i < maxIndex; i++)
      {
        tempY = Solve(time);

        solution[i, 0] = time;

        for (int j = 0; j < tempY.Length; j++)
        {
          solution[i, j + 1] = tempY[j];
        }

        time += deltaT;
      }

      return solution;
    }

    /// <summary>
    /// Computes the solution of the differential equations.
    /// </summary>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    /// <param name="t0">The initial independent variable value.</param>
    /// <param name="deltaT">The step for the interval of integration (t0, t0+deltaT, t0+2*deltaT,...,tf).</param>
    /// <param name="tf">The final independent variable value.</param>
    /// <param name="solution">A delegate where to return the solution.</param>
    public void Solve(double[] y0, double t0, double deltaT, double tf, OdeSolution solution)
    {
      CheckArguments(t0, deltaT, tf);

      SetInitialValues(t0, y0);

      int maxIndex = (int)(Math.Abs(tf - t0) / Math.Abs(deltaT)) + 1;

      double[] tempY;
      double time = t0;

      for (int i = 0; i < maxIndex; i++)
      {
        tempY = Solve(time);

        solution(time, tempY);

        time += deltaT;
      }
    }

    /// <summary>
    ///  Computes the solution of the differential equations.
    /// </summary>
    /// <param name="y0">A vector of size N containing the initial conditions ( at t0). N is the number of differential equations.</param>
    /// <param name="tspan">A vector specifying the interval of integration (t0,..,tf).</param>
    /// <param name="solution">A delegate where to return the solution.</param>
    public void Solve(double[] y0, double[] tspan, OdeSolution solution)
    {
      CheckTArray(tspan);

      base.SetInitialValues(tspan[0], y0);

      double[] tempY;

      double time = 0;

      for (int i = 0; i < tspan.Length; i++)
      {
        time = tspan[i];

        tempY = Solve(time);

        solution(time, tempY);
      }
    }

    /// <summary>
    /// Sets the initial values for the differential equations.
    /// </summary>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    /// <remarks>
    /// This method should be invoked before to start the integration.
    /// When this method is invoked, the ODE solver is restarted.
    /// </remarks>
    public override void SetInitialValues(double t0, double[] y0)
    {
      base.SetInitialValues(t0, y0);

      _IState = 1;
    }

    #endregion Public Methods

    #region inernal Methods

    /// <summary>
    /// Inicializa elespacio nesesitado por la surutinas. Se requiere que estend dedfinidas las proiedades
    /// que definen dicho espacio, por ejemplo el numero de equaciones.
    /// </summary>
    internal override void InitializeWorkingSpace()
    {
      //RWORK= A real working array (double precision).
      // this length is:
      // 20 + 16*NEQ                    for MF = 10,
      // 22 + 16*NEQ + 2*NEQ**2         for MF = 11 or 12,
      // 22 + 16*NEQ + NEQ**2           for MF = -11 or -12,
      // 22 + 17*NEQ                    for MF = 13,
      // 22 + 18*NEQ + (3*ML+2*MU)*NEQ  for MF = 14 or 15,
      // 22 + 17*NEQ + (2*ML+MU)*NEQ    for MF = -14 or -15,
      // 20 +  9*NEQ                    for MF = 20,
      // 22 +  9*NEQ + 2*NEQ**2         for MF = 21 or 22,
      // 22 +  9*NEQ + NEQ**2           for MF = -21 or -22,
      // 22 + 10*NEQ                    for MF = 23,
      // 22 + 11*NEQ + (3*ML+2*MU)*NEQ  for MF = 24 or 25.
      // 22 + 10*NEQ + (2*ML+MU)*NEQ    for MF = -24 or -25.

      // C MF     = Method flag.  Standard values are:
      // C          10 for nonstiff (Adams) method, no Jacobian used.
      // C          21 for stiff (BDF) method, user-supplied full Jacobian.
      // C          22 for stiff method, internally generated full Jacobian.
      // C          24 for stiff method, user-supplied banded Jacobian.
      // C          25 for stiff method, internally generated banded Jacobian.

      // C IWORK  = An integer work array.  The length of IWORK must be at least
      // C             30        if MITER = 0 or 3 (MF = 10, 13, 20, 23), or
      // C             30 + NEQ  otherwise (abs(MF) = 11,12,14,15,21,22,24,25).

      if (_Type == ODEType.NonStiff)
      {
        _Mf = 10; //  10 for nonstiff (Adams) method, no Jacobian used.
        _Liw = 30; //if MITER = 0 or 3 (MF = 10, 13, 20, 23)
        _Lrw = 20 + 16 * _NEquations; //// 20 + 16*NEQ                    for MF = 10,
      }
      else if (_Type == ODEType.Stiff)
      {
        if (_UserJacobian == false)
        {
          _Mf = 22; //  22 for stiff method, internally generated full Jacobian.
        }
        if (_UserJacobian == true)
        {
          _Mf = 21; //  21 for stiff (BDF) method, user-supplied full Jacobian.
        }
        _Liw = 30 + _NEquations; //30 + NEQ  if (abs(MF) = 11,12,14,15,21,22,24,25).
        _Lrw = Convert.ToInt32(22 + 9 * _NEquations + 2 * Math.Pow(_NEquations, 2)); // 22 +  9*NEQ + 2*NEQ**2         for MF = 21 or 22,
      }
      _RWork = new double[_Lrw];
      _IWork = new int[_Liw];
    }

    [MemberNotNull(nameof(_fex))]
    internal override void InitializeFunctionAndJacobian(OdeFunction fun, OdeJacobian? jac)
    {
      _fex = new FEX(_NEquations, fun);
      if (!(jac is null))
        _jex = new JEX(_NEquations, jac);
    }

    internal override void InitializeExceptionMessages()
    {
      _Errors = new string[7];
      _Errors[0] = "";
      _Errors[1] = "Excessive amount of work (more than MXSTEP steps) was done on this call, before completing the requested task, but the integration was otherwise successful as far as T.";
      _Errors[2] = "Too much accuracy was requested for the precision of the machine being used.";
      _Errors[3] = "Illegal input was detected.";
      _Errors[4] = "There were repeated error test failures on one attempted step, before completing the requested task, but the integration was successful as far as T.";
      _Errors[5] = "There were repeated convergence test failures on one attempted step, before completing the requested task, but the integration was successful as far as T. This may be caused by an inaccurate Jacobian matrix, if one is being used.";
      _Errors[6] = "EWT(i) became zero for some i during the integration.   Pure relative error control (AbsTol(i)=0.0) was requested on a variable which has now vanished. The integration was successful as far as T.";
    }

    ///// <summary>
    ///// Resets the integration at the especified point.
    ///// </summary>
    ///// <param name="t0">The initial independent variable value .</param>
    ///// <param name="y0"></param>
    //public void ResetIntegration(double initialVariableValue, double[] initialValues)
    //{
    //    this.MeIState = 1;

    //    if (initialValues.Length != this.MeNEquations)
    //    {
    //        throw new ArgumentException("initialValues.Length != Number of equations.");
    //    }

    //    base.InitializeSizeDependentVariables(initialVariableValue, initialValues);
    //}

    #endregion inernal Methods

    private double[,] InternalJacobian(double T, double[] Y)
    {
      double[,] TempJac = new double[1, 1];

      return TempJac;
    }
  }

  #region Interface

  public interface IFEX
  {
    void Run(int NEQ, double T, double[] Y, int offset_y, ref double[] YDOT, int offset_ydot, double RPAR, int IPAR);
  }

  public interface IJEX
  {
    void Run(int NEQ, double T, double[] Y, int offset_y, int ML, int MU, ref double[] PD, int offset_pd
             , int NRPD, double RPAR, int IPAR);
  }

  public interface IDVNLSD
  {
    void Run(ref double[] Y, int offset_y, double[] YH, int offset_yh, int LDYH, double[] VSAV, int offset_vsav, ref double[] SAVF, int offset_savf, double[] EWT, int offset_ewt
             , ref double[] ACOR, int offset_acor, ref int[] IWM, int offset_iwm, ref double[] WM, int offset_wm, IFEX F, IJEX JAC, IFEX PDUM
             , ref int NFLAG, double[] RPAR, int offset_rpar, int[] IPAR, int offset_ipar);
  }

  #endregion Interface

  #region Interfaces

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                 Interfaces
  //----------------------------------------------------------------------------------------------------------------------------

  #region The Class: FEX

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                      The Class: FEX
  //----------------------------------------------------------------------------------------------------------------------------

  // C
  internal class FEX : IFEX
  {
    #region Fields

    private OdeFunction MeFunction;

    private double[] MeY;
    private double[] MeYDot;
    private int MeNEq;

    #endregion Fields

    #region Constructor

    internal FEX(int NEq, OdeFunction Func)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeYDot = new double[NEq];
      MeFunction = Func;
    }

    #endregion Constructor

    public void Run(int NEQ, double T, double[] Y, int o_y, ref double[] YDOT, int o_ydot, double RPAR, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_ydot = -1 + o_ydot;

      #endregion Array Index Correction

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + o_y];
      }

      MeFunction(T, MeY, MeYDot);

      for (int i = 0; i < MeNEq; i++)
      {
        YDOT[i + o_ydot] = MeYDot[i];
      }

      return;
    }
  }

  #endregion The Class: FEX

  #region The Class: JEX

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                      The Class: JEX
  //----------------------------------------------------------------------------------------------------------------------------

  // C
  internal class JEX : IJEX
  {
    private OdeJacobian MeJacobian;
    private double[] MeY;
    private double[,] MeJac;
    private int MeNEq;

    internal JEX(int NEq, OdeJacobian Jac)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeJacobian = Jac;
      MeJac = new double[NEq, NEq];
    }

    public void Run(int NEQ, double T, double[] Y, int o_y, int ML, int MU, ref double[] PD, int o_pd
                     , int NRPD, double RPAR, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_pd = -1 - NRPD + o_pd;

      #endregion Array Index Correction

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + o_y];
      }

      MeJacobian(T, MeY, MeJac);

      for (int j = 0; j < MeNEq; j++)
      {
        for (int i = 0; i < MeNEq; i++)
        {
          PD[i + j * NRPD + o_pd] = MeJac[i, j];
        }
      }

      return;
    }
  }

  #endregion The Class: JEX

  #endregion Interfaces
}
