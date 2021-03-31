﻿#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

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

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  ///Represents the base class for the Odes.
  /// </summary>
  public abstract class xOdeBase
  {
    #region Fields
#nullable disable

    //internal protected bool MeIsInitialized = false;

    /// <summary>
    /// The number of equations
    /// </summary>
    protected internal int _NEquations;

    /// <summary>
    /// The initial independent variable value.
    /// </summary>
    protected internal double _T0 = 0d;

    /// <summary>
    /// The initial conditions.
    /// </summary>
    protected internal double[] _Y0;

    /// <summary>
    /// Array used to set the initial values and to return the solution in some ODE solvers.
    /// This array must be initialized equal to the initial values in the first call.
    /// </summary>
    protected internal double[] _Y;

    /// <summary>
    /// Indicated if the SetInitialValues method need to be invoked.
    /// </summary>
    protected bool _InvokeSetInitialValues = true;

    /// <summary>
    /// Indicated if the InitializeODEs method need to be invoked.
    /// </summary>
    protected bool _InvokeInitializeODEs = true;

    /// <summary>
    /// Array containing the exception messages.
    /// </summary>
    protected string[] _Errors;

    /// <summary>
    /// For AdamsMoulton and  OdeGearsBDF:
    ///
    /// MeITol = An indicator for the type of error control.
    ///
    ///   ITOL         RTOL       ATOL          EWT(i)
    /// MeITol =1     scalar     scalar     RTOL*ABS(Y(i)) + ATOL
    /// MeITol =2     scalar     array      RTOL*ABS(Y(i)) + ATOL(i)
    /// MeITol =3     array      scalar     RTOL(i)*ABS(Y(i)) + ATOL
    /// MeITol =4     array      array      RTOL(i)*ABS(Y(i)) + ATOL(i)
    /// </summary>
    protected internal int _ITolAdamsGears = 1;

    /// <summary>
    ///For Runge-Kutta
    ///
    /// ITol = An indicator for the type of error control.
    ///
    /// ITOL=0: BOTH RTOL AND ATOL ARE SCALARS.
    /// ITOL=1: BOTH RTOL AND ATOL ARE VECTORS.
    /// </summary>
    protected internal int _ITolRK = 0;

    private ErrorToleranceEnum _ErrorToleranceType = ErrorToleranceEnum.Scalar;

    /// <summary>
    /// A relative error tolerance parameter.
    /// The input parameters ITOL, RTOL, and ATOL determine
    /// the error control performed by the solver.  The solver will
    /// control the vector e = (e(i)) of estimated local errors
    /// in Y, according to an inequality of the form
    /// rms-norm of ( e(i)/EWT(i) )   .le.   1,
    /// where       EWT(i) = RTOL(i)*abs(Y(i)) + ATOL(i),
    /// </summary>
    protected internal double _RelTol = 1.0E-3;

    /// <summary>
    /// A relative error tolerance parameter, either a scalar or an array of length NEQ.
    /// </summary>
    protected internal double[] _RelTolArray;

    /// <summary>
    /// An absolute error tolerance parameter
    /// </summary>
    protected internal double _AbsTol = 1.0E-6;

    /// <summary>
    /// An absolute error tolerance parameter(array of length NEQ)
    /// </summary>
    protected internal double[] _AbsTolArray;

    /// <summary>
    /// MeRWork= A real working array (double precision)
    /// </summary>
    protected internal double[] _RWork;

    /// <summary>
    /// MeLrw= The length of the array RWORK
    /// </summary>
    protected internal int _Lrw;

    /// <summary>
    /// MeIWork= An integer work array.
    /// </summary>
    protected internal int[] _IWork;

    /// <summary>
    /// MeLiw= the length of the array IWORK
    /// </summary>
    protected internal int _Liw;

    /// <summary>
    /// User-specified array used to communicate real parameters
    /// </summary>
    protected internal double[] _RPar = new double[1];

    /// <summary>
    /// User-specified array used to communicate integer parameter
    /// </summary>
    protected internal int[] _IPar = new int[1];

#nullable enable

    #endregion Fields

    #region Properties

    ///// <summary>
    ///// MeITol1 = An indicator for the type of error control.
    /////                 ITOL    RTOL       ATOL          EWT(i)
    ///// MeITol1 =1     scalar     scalar     RTOL*ABS(Y(i)) + ATOL
    ///// MeITol1 =2     scalar     array      RTOL*ABS(Y(i)) + ATOL(i)
    ///// MeITol1 =3     array      scalar     RTOL(i)*ABS(Y(i)) + ATOL
    ///// MeITol1 =4     array      array      RTOL(i)*ABS(Y(i)) + ATOL(i)
    ///// </summary>
    //public int ITol
    //{
    //    get { return MeITol; }
    //    set { MeITol = value; }
    //}

    /// <summary>
    /// A relative error tolerance array ( length numEquations).
    /// </summary>
    /// <remarks>
    /// If ErrorToleranceType = ErrorToleranceEnum.Array the estimated local error in Y(i) is controlled to be less than RelTolArray[I]*Abs(Y(I))+AbsTolArray[i].
    /// </remarks>
    public double[] RelTolArray
    {
      get { return _RelTolArray; }
      set
      {
        if (value is null || value.Length != _NEquations)
        {
          throw new ArgumentException("RelTolArray.Length is invalid, RelTolArray.Length must be equal to the number of equations.");
        }
        _RelTolArray = value;
      }
    }

    /// <summary>
    /// A relative error tolerance parameter.
    /// </summary>
    /// <remarks>
    /// If ErrorToleranceType = ErrorToleranceEnum.Scalar the estimated local error in Y(i) is controlled to be less than RelTol*Abs(Y[i]) + AbsTol.
    /// </remarks>
    public double RelTol
    {
      get { return _RelTol; }
      set
      {
        _RelTol = value;
        for (int i = 0; i < _NEquations; i++)
        {
          _RelTolArray[i] = _RelTol;
        }
      }
    }

    /// <summary>
    /// An absolute error tolerance parameter
    /// </summary>
    /// <remarks>
    /// If ErrorToleranceType = ErrorToleranceEnum.Scalar the estimated local error in Y(i) is controlled to be less than RelTol*Abs(Y[i]) + AbsTol.
    /// </remarks>
    public double AbsTol
    {
      get { return _AbsTol; }
      set
      {
        _AbsTol = value;
        for (int i = 0; i < _NEquations; i++)
        {
          _AbsTolArray[i] = _AbsTol;
        }
      }
    }

    /// <summary>
    /// An absolute error tolerance array (length numEquations).
    /// </summary>
    /// <remarks>
    /// If ErrorToleranceType = ErrorToleranceEnum.Array the estimated local error in Y(i) is controlled to be less than RelTolArray[I]*Abs(Y(I))+AbsTolArray[i].
    /// </remarks>
    public double[] AbsTolArray
    {
      get { return _AbsTolArray; }
      set
      {
        if (value is null || value.Length != _NEquations)
        {
          throw new ArgumentException("AbsTolArray.Length is invalid, AbsTolArray.Length must be equal to the number of equations.");
        }
        _AbsTolArray = value;
      }
    }

    /// <summary>
    /// Specifies the type of the relative error and absolute error tolerances.
    /// </summary>
    public ErrorToleranceEnum ErrorToleranceType
    {
      get { return _ErrorToleranceType; }
      set
      {
        _ErrorToleranceType = value;
        if (value == ErrorToleranceEnum.Scalar)
        {
          _ITolAdamsGears = 1;
          _ITolRK = 0;
        }
        else if (value == ErrorToleranceEnum.Array)
        {
          _ITolAdamsGears = 4;
          _ITolRK = 1;
        }
      }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Sets the initial values for the differential equations.
    /// </summary>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    /// <remarks>
    /// This method should be invoked before to start the integration.
    /// When this method is invoked, the ODE solver is restarted.
    /// </remarks>
    [MemberNotNull(nameof(_Y0), nameof(_Y), nameof(_T0))]
    public virtual void SetInitialValues(double t0, double[] y0)
    {
      if (_InvokeInitializeODEs == true)
      {
        throw new Exception("Before to start the integration you must specify the ODE system. Please invoke the method InitializeODEs before this method.");
      }

      if (_NEquations != y0.Length)
      {
        throw new ArgumentException("The y0.Length is invalid, y0.Length must be equal to the number of equations");
      }

      _InvokeSetInitialValues = false;

      _Y0 = new double[_NEquations];
      _Y = new double[_NEquations];
      for (int i = 0; i < y0.Length; i++)
      {
        _Y0[i] = y0[i];
        _Y[i] = y0[i];
      }
      _T0 = t0;
    }

    /// <summary>
    /// InitializeInternal the size dependent variables:
    /// Number of equations, Relative tolerances,  Absolute tolerances, Working space
    /// </summary>
    /// <param name="numEquations">The number of equations.</param>
    [MemberNotNull(nameof(_RelTolArray), nameof(_AbsTolArray))]
    internal void InitializeSizeDependentVariables(int numEquations)
    {
      _NEquations = numEquations;
      if (_NEquations < 1)
      {
        throw new ArgumentException("numEquations <1.");
      }

      _RelTolArray = new double[_NEquations];
      _AbsTolArray = new double[_NEquations];
      for (int i = 0; i < _NEquations; i++)
      {
        _RelTolArray[i] = _RelTol;
        _AbsTolArray[i] = _AbsTol;
      }

      InitializeWorkingSpace();
    }

    internal virtual void InitializeInternal(OdeFunction function, OdeJacobian? jacobian, int numEquations)
    {
      //this.MeIsInitialized = true;

      //if (this.MeIsInitialized == false)
      //{
      //    throw new ArgumentException("Please call the Inizialize method first.");
      //}

      _InvokeInitializeODEs = false;

      InitializeSizeDependentVariables(numEquations);
      InitializeFunctionAndJacobian(function, jacobian);
      InitializeExceptionMessages();
    }

    protected void CheckTArray(double[] tspan)
    {
      if (tspan is null)
        throw new ArgumentException("tspan = null");
      if (tspan.Length < 2)
        throw new ArgumentException("tspan.Length<2, deltaT=0, deltaT must be different of zero");

      bool isDeltaPositive = true;
      bool isTemDeltaPositive = true;
      if (tspan[1] - tspan[0] > 0)
        isDeltaPositive = true;
      else
        isDeltaPositive = false;

      for (int i = 1; i < tspan.Length; i++)
      {
        if (tspan[i] - tspan[i - 1] > 0)
          isTemDeltaPositive = true;
        else
          isTemDeltaPositive = false;
        if (isTemDeltaPositive != isDeltaPositive)
        {
          if (isDeltaPositive == true)
            throw new ArgumentException("tspan[i] - tspan[i - 1] is not always positive.");
          else
            throw new ArgumentException("tspan[i] - tspan[i - 1] is not always negative.");
        }
      }
    }

    protected void CheckArguments(double t0, double deltaT, double tf)
    {
      if (tf == t0)
        throw new ArgumentException("tf = t0");
      if (deltaT > 0)
      {
        if (tf < t0)
          throw new ArgumentException("if tf < t0 then deltaT must be < 0");
      }
      else if (deltaT < 0)
      {
        if (tf > t0)
          throw new ArgumentException("If tf > t0 then deltaT  must be > 0");
      }
      else if (deltaT == 0)
      {
        throw new ArgumentException("deltaT=0, deltaT must be different of zero");
      }
    }

    protected internal void CheckInitialization()
    {
      if (_InvokeInitializeODEs == true)
      {
        throw new Exception("Before to start the integration you must specify the ODE system. Please invoke the method InitializeODEs before this method.");
      }

      if (_InvokeSetInitialValues == true)
      {
        throw new Exception("Before to start the integration you must set the initial values. Please invoke the method SetInitialValues before this method.");
      }
    }

    //internal void IsInizialized()
    //{
    //    if (this.MeIsInitialized == false)
    //    {
    //        throw new ArgumentException("Please call the Inizialize method first.");
    //    }
    //}

    /// <summary>
    /// Inicializa elespacio nesesitado por la surutinas. Se requiere que estend dedfinidas las proiedades
    /// que definen dicho espacio, por ejemplo el numero de equaciones.
    /// </summary>
    internal abstract void InitializeWorkingSpace();

    internal abstract void InitializeFunctionAndJacobian(OdeFunction fun, OdeJacobian? jac);

    internal abstract void InitializeExceptionMessages();

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public abstract void InitializeODEs(OdeFunction function, int numEquations);

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    public abstract void InitializeODEs(OdeFunction function, int numEquations, double t0, double[] y0);

    #endregion Methods
  }
}
