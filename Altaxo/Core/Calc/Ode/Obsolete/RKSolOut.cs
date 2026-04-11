#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Calc.Ode.Obsolete.Dopri;
using Altaxo.Calc.Ode.Obsolete.Radau5;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// NAME (EXTERNAL) OF SUBROUTINE PROVIDING THE
  /// NUMERICAL SOLUTION DURING INTEGRATION.
  /// IF IOUT.GE.1, IT IS CALLED AFTER EVERY SUCCESSFUL STEP.
  /// SUPPLY A DUMMY SUBROUTINE IF IOUT=0.
  /// IT MUST HAVE THE FORM
  /// SUBROUTINE SOLOUTR (NR,XOLD,X,Y,N,CON,ICOMP,ND,
  /// RPAR,IPAR,IRTRN)
  /// DIMENSION Y(N),CON(5*ND),ICOMP(ND)
  /// ....
  /// SOLOUTR FURNISHES THE SOLUTION "Y" AT THE NR-TH
  /// GRID-POINT "X" (THEREBY THE INITIAL VALUE IS
  /// THE FIRST GRID-POINT).
  /// "XOLD" IS THE PRECEEDING GRID-POINT.
  /// "IRTRN" SERVES TO INTERRUPT THE INTEGRATION. IF IRTRN
  /// IS SET .LT.0, DOPRI5 WILL RETURN TO THE CALLING PROGRAM.
  /// IF THE NUMERICAL SOLUTION IS ALTERED IN SOLOUTR,
  /// SET  IRTRN = 2
  ///
  /// -----  CONTINUOUS OUTPUT: -----
  /// DURING CALLS TO "SOLOUTR", A CONTINUOUS SOLUTION
  /// FOR THE INTERVAL [XOLD,X] IS AVAILABLE THROUGH
  /// THE FUNCTION
  /// .GT..GT..GT.   CONTD5(I,S,CON,ICOMP,ND)   .LT..LT..LT.
  /// WHICH PROVIDES AN APPROXIMATION TO THE I-TH
  /// COMPONENT OF THE SOLUTION AT THE POINT S. THE VALUE
  /// S SHOULD LIE IN THE INTERVAL [XOLD,X].
  /// </summary>
  internal class RKSolOut : ISOLOUT, ISOLOUTR
  {
    /// <summary>
    /// Specifies how solution values are returned by the output helper.
    /// </summary>
    protected enum SolutionOutType
    {
      /// <summary>
      /// Store the solution in an array.
      /// </summary>
      Array,

      /// <summary>
      /// Forward the solution to a delegate.
      /// </summary>
      Delegate
    }

    #region Fields
#nullable disable

    private CONTD5 _Contd5ExpicitRK;

    private CONTR5 _Contr5ImplicitRK;

    /// <summary>
    /// Number of equations
    /// </summary>
    protected int _NEquations;

    /// <summary>
    /// Valores iniciales.
    /// </summary>
    protected double[] _Y0;

    /// <summary>
    /// Indica si se usa un array para definir los puntos de inegracion.
    /// </summary>
    protected bool _IsTimeArrayUsed = false;

    /// <summary>
    /// El tiempo inicial de integracion
    /// </summary>
    protected double _T0 = 0;

    /// <summary>
    /// El tiempo final de inegracion
    /// </summary>
    protected double _TF = 1;

    /// <summary>
    /// The time increment used for integration output.
    /// </summary>
    protected double _DeltaT = 1;

    /// <summary>
    /// El array que contiene los tiempos de integracion
    /// </summary>
    protected double[] _TSpan;

    /// <summary>
    /// El valor indice maximo para el cual la soluciones calculada.
    /// </summary>
    protected int _MaxIndex = 0;

    /// <summary>
    /// El array que contiene el tiempo y los valores de la solucion
    /// </summary>
    protected double[,] _SolutionArray;

    /// <summary>
    /// El delegado que se llama cuando se calcula un punto en la inegracion (si es requerido).
    /// </summary>
    protected OdeSolution _SolutionOut;

    ///// <summary>
    ///// A value that indicate when a delegate is used for the solution output.
    ///// </summary>
    //private bool MeUseDelegateSolutionOut = true;

    /// <summary>
    /// Especifica como se regresara la solucion
    /// </summary>
    protected SolutionOutType _SolutionOutType;

    /// <summary>
    /// Un areglo que array que almacena la solucion para cada punto
    /// </summary>
    protected double[] _TemporalSolution;

    /// <summary>
    /// Indica si la integracion va en incrementos positivos del tiempo.
    /// </summary>
    protected bool _isDeltaPositive = true;

    /// <summary>
    /// The current output index.
    /// </summary>
    protected int _Index = 1;

    //protected int MeSolutionLength;

    /// <summary>
    /// El valor que se esta calculando.
    /// </summary>
    protected double _T = 0;

    ///// <summary>
    ///// El indice en el que se esta calculando.
    ///// </summary>
    /////
#nullable enable
    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="RKSolOut"/> class for explicit Runge-Kutta dense output.
    /// </summary>
    /// <param name="contd5ExpicitRK">The dense output evaluator for the explicit solver.</param>
    public RKSolOut(CONTD5 contd5ExpicitRK)
    {
      _Contd5ExpicitRK = contd5ExpicitRK;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RKSolOut"/> class for implicit Runge-Kutta dense output.
    /// </summary>
    /// <param name="contr5ImplicitRK">The dense output evaluator for the implicit solver.</param>
    public RKSolOut(CONTR5 contr5ImplicitRK)
    {
      _Contr5ImplicitRK = contr5ImplicitRK;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Initializes solution output using a regular time grid and a callback delegate.
    /// </summary>
    /// <param name="y0">The initial values.</param>
    /// <param name="t0">The initial time.</param>
    /// <param name="deltaT">The output time increment.</param>
    /// <param name="tf">The final time.</param>
    /// <param name="solution">The callback that receives solution values.</param>
    internal void Initialize(double[] y0, double t0, double deltaT, double tf, OdeSolution solution)
    {
      _SolutionOutType = SolutionOutType.Delegate;
      InitializeValues(y0, t0, deltaT, tf);

      _SolutionOut = solution;
    }

    /// <summary>
    /// Initializes solution output using a regular time grid and an output array.
    /// </summary>
    /// <param name="y0">The initial values.</param>
    /// <param name="t0">The initial time.</param>
    /// <param name="deltaT">The output time increment.</param>
    /// <param name="tf">The final time.</param>
    /// <param name="solutionArray">Receives the allocated solution array.</param>
    internal void Initialize(double[] y0, double t0, double deltaT, double tf, out double[,] solutionArray)
    {
      _SolutionOutType = SolutionOutType.Array;
      InitializeValues(y0, t0, deltaT, tf);

      int NCols = y0.Length + 1;
      solutionArray = new double[_MaxIndex, NCols];

      _SolutionArray = solutionArray;
    }

    /// <summary>
    /// Initializes solution output using explicit output times and a callback delegate.
    /// </summary>
    /// <param name="y0">The initial values.</param>
    /// <param name="tspan">The requested output times.</param>
    /// <param name="solution">The callback that receives solution values.</param>
    internal void Initialize(double[] y0, double[] tspan, OdeSolution solution)
    {
      _SolutionOutType = SolutionOutType.Delegate;
      InitializeValues(y0, tspan);

      _SolutionOut = solution;
    }

    /// <summary>
    /// Initializes solution output using explicit output times and an output array.
    /// </summary>
    /// <param name="y0">The initial values.</param>
    /// <param name="tspan">The requested output times.</param>
    /// <param name="solutionArray">Receives the allocated solution array.</param>
    internal void Initialize(double[] y0, double[] tspan, out double[,] solutionArray)
    {
      _SolutionOutType = SolutionOutType.Array;
      InitializeValues(y0, tspan);

      int NCols = y0.Length + 1;
      solutionArray = new double[_MaxIndex, NCols];

      _SolutionArray = solutionArray;
    }

    private void InitializeValues(double[] y0, double t0, double deltaT, double tf)
    {
      _IsTimeArrayUsed = false;

      _NEquations = y0.Length;
      _Y0 = y0;
      _T0 = t0;
      _DeltaT = deltaT;
      _TF = tf;
      _MaxIndex = (int)(Math.Abs(tf - t0) / Math.Abs(deltaT)) + 1;

      if (deltaT > 0)
        _isDeltaPositive = true;
      else
        _isDeltaPositive = false;

      _TemporalSolution = new double[y0.Length];
    }

    private void InitializeValues(double[] y0, double[] tspan)
    {
      _IsTimeArrayUsed = true;

      _NEquations = y0.Length;
      _Y0 = y0;

      _TSpan = tspan;
      _MaxIndex = tspan.Length;

      if ((tspan[1] - tspan[0]) > 0)
        _isDeltaPositive = true;
      else
        _isDeltaPositive = false;

      _TemporalSolution = new double[y0.Length];
    }

    //protected void SetSolutionDimension(double[] y0, double t0, double deltaT, double tEnd)
    //{
    //    int NCols = y0.Length + 1;
    //    int NRens = (int)(Math.Abs(tEnd - t0) / Math.Abs(deltaT)) + 1;
    //    this.MeSolution = new double[NRens, NCols];

    //    for (int i = 0; i < NRens; i++)
    //    {
    //        this.MeSolution[i, 0] = t0 + deltaT * i;
    //    }
    //    //for (int j = 1; j < NCols; j++)
    //    //{
    //    //    this.MeSolution[0, j] = y0[j - 1];
    //    //}

    //}

    //private int CalculateMaxIndex(double t0, double deltaT, double tEnd)
    //{
    //    return
    //}

    /// <summary>
    /// Calcula el valor temporal de la integracion para un idice dado.
    /// </summary>
    /// <param name="index">El indice corresponiente al tiempo deseado.</param>
    /// <returns>El tiempo de integracio.</returns>
    protected double GetTime(int index)
    {
      double t = 0;

      if (_IsTimeArrayUsed == true)
      {
        t = _TSpan[index];
      }
      else
      {
        t = _T0 + _DeltaT * index;
      }

      return t;
    }

    #endregion Methods

    #region ISOLOUT Members

    /// <summary>
    /// Stores or forwards dense output produced by the explicit Runge-Kutta solver.
    /// </summary>
    /// <param name="NR">The current output step number.</param>
    /// <param name="XOLD">The previous grid point.</param>
    /// <param name="X">The current grid point.</param>
    /// <param name="Y">The current solution vector.</param>
    /// <param name="o_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="N">The number of equations.</param>
    /// <param name="CON">The dense output coefficients.</param>
    /// <param name="o_con">The offset into <paramref name="CON"/>.</param>
    /// <param name="ICOMP">The component map.</param>
    /// <param name="o_icomp">The offset into <paramref name="ICOMP"/>.</param>
    /// <param name="ND">The number of dense output components.</param>
    /// <param name="RPAR">A user-supplied real parameter.</param>
    /// <param name="o_rpar">The offset into <paramref name="RPAR"/>.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    /// <param name="IRTRN">The solver return flag.</param>
    void ISOLOUT.Run(int NR, double XOLD, double X, double[] Y, int o_y, int N, double[] CON,
        int o_con, int[] ICOMP, int o_icomp, int ND, double[] RPAR, int o_rpar, int IPAR, int IRTRN)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_con = -1 + o_con;
      int c_icomp = -1 + o_icomp;
      int c_rpar = -1 + o_rpar;

      #endregion Array Index Correction

      if (NR == 1)
      {
        if (_SolutionOutType == SolutionOutType.Array)
        {
          _SolutionArray[0, 0] = _T0;
          for (int j = 1; j <= _NEquations; j++)
          {
            _SolutionArray[0, j] = _Y0[j - 1];
          }
        }
        else
        {
          _SolutionOut(_T0, _Y0);
        }

        _Index = 1;
      }

      bool MyContinue = true;

      while (MyContinue && _Index < _MaxIndex)
      {
        _T = GetTime(_Index);

        if ((_isDeltaPositive && X >= _T) || (_isDeltaPositive == false && X <= _T))
        {
          if (_SolutionOutType == SolutionOutType.Array)
          {
            _SolutionArray[_Index, 0] = _T;
            for (int j = 1; j <= _NEquations; j++)
            {
              _SolutionArray[_Index, j] = _Contd5ExpicitRK.Run(j, _T, CON, o_con, ICOMP, o_icomp, ND);
            }
          }
          else
          {
            for (int j = 0; j < _NEquations; j++)
            {
              _TemporalSolution[j] = _Contd5ExpicitRK.Run(j + 1, _T, CON, o_con, ICOMP, o_icomp, ND);
            }
            _SolutionOut(_T, _TemporalSolution);
          }

          _Index++;
        }
        else
          MyContinue = false;
      }
      return;
    }

    #endregion ISOLOUT Members

    #region ISOLOUTR Members

    /// <summary>
    /// Stores or forwards dense output produced by the implicit Runge-Kutta solver.
    /// </summary>
    /// <param name="NR">The current output step number.</param>
    /// <param name="XOLD">The previous grid point.</param>
    /// <param name="X">The current grid point.</param>
    /// <param name="Y">The current solution vector.</param>
    /// <param name="o_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="CONT">The dense output coefficients.</param>
    /// <param name="o_cont">The offset into <paramref name="CONT"/>.</param>
    /// <param name="LRC">The leading dimension of <paramref name="CONT"/>.</param>
    /// <param name="N">The number of equations.</param>
    /// <param name="RPAR">A user-supplied real parameter.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    /// <param name="IRTRN">The solver return flag.</param>
    void ISOLOUTR.Run(int NR, double XOLD, double X, double[] Y, int o_y, double[] CONT, int o_cont,
        int LRC, int N, double RPAR, int IPAR, int IRTRN)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_cont = -1 + o_cont;

      #endregion Array Index Correction

      if (NR == 1)
      {
        if (_SolutionOutType == SolutionOutType.Array)
        {
          _SolutionArray[0, 0] = _T0;
          for (int j = 1; j <= _NEquations; j++)
          {
            _SolutionArray[0, j] = _Y0[j - 1];
          }
        }
        else
        {
          _SolutionOut(_T0, _Y0);
        }

        _Index = 1;
      }

      bool MyContinue = true;

      while (MyContinue && _Index < _MaxIndex)
      {
        _T = GetTime(_Index);

        if ((_isDeltaPositive && X >= _T) || (_isDeltaPositive == false && X <= _T))
        {
          if (_SolutionOutType == SolutionOutType.Array)
          {
            _SolutionArray[_Index, 0] = _T;
            for (int j = 1; j <= _NEquations; j++)
            {
              _SolutionArray[_Index, j] = _Contr5ImplicitRK.Run(j, _T, CONT, o_cont, LRC);
            }
          }
          else
          {
            for (int j = 0; j < _NEquations; j++)
            {
              _TemporalSolution[j] = _Contr5ImplicitRK.Run(j + 1, _T, CONT, o_cont, LRC);
            }
            _SolutionOut(_T, _TemporalSolution);
          }

          _Index++;
        }
        else
          MyContinue = false;
      }
      return;
    }

    #endregion ISOLOUTR Members
  }
}
