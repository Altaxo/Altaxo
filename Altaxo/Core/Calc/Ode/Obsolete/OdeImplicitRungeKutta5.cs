#region Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

//Copyright © 2009 Jose Antonio De Santiago-Castillo
//E-mail:JAntonioDeSantiago@gmail.com
//Web: www.DotNumerics.com
//

#endregion Copyright © 2009, De Santiago-Castillo JA. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Altaxo.Calc.Ode.Obsolete.Dopri;
using Altaxo.Calc.Ode.Obsolete.Radau5;

namespace Altaxo.Calc.Ode.Obsolete
{
  /// <summary>
  /// Solves an initial-value problem for stiff ordinary differential equations using
  /// the implicit Runge-Kutta method of order 5.
  /// dy(i)/dt = f(i,t,y(1),y(2),...,y(N)).
  /// </summary>
  public sealed class OdeImplicitRungeKutta5 : xBaseOdeRungeKutta
  {
    #region Fields
#nullable disable

    //internal SOLOUTR solout;
    internal FVPOL fvpol; internal JVPOL jvpol;

    internal BBAMPL bbampl; internal DECOMR decomr; internal DECOMC decomc; internal SLVRAR slvrar; internal SLVRAI slvrai;
    internal SLVRAD slvrad; internal ESTRAD estrad; internal ESTRAV estrav; internal SLVROD slvrod; internal SLVSEU slvseu;
    internal DEC dec; internal SOL sol; internal DECH dech; internal SOLH solh; internal DECC decc; internal SOLC solc;
    internal DECHC dechc; internal SOLHC solhc; internal DECB decb; internal SOLB solb; internal DECBC decbc;
    internal SOLBC solbc; internal ELMHES elmhes; internal RADAU5 radau5; internal RADCOR radcor; internal CONTR5 contr5;

#nullable enable

    private CommonBlock INTERN = new CommonBlock(1, 0, 0, 0);
    private CommonBlock LINAL = new CommonBlock(0, 7, 0, 0);
    private CommonBlock CONRA5 = new CommonBlock(4, 4, 0, 0);

    #region Valores especificos para esta subrutina

    /// <summary>
    /// INITIAL STEP SIZE GUESS;
    /// FOR STIFF EQUATIONS WITH INITIAL TRANSIENT,
    /// H=1.D0/(NORM OF F'), USUALLY 1.D-3 OR 1.D-5, IS GOOD.
    /// THIS CHOICE IS NOT VERY IMPORTANT, THE STEP SIZE IS
    /// QUICKLY ADAPTED. (IF H=0.D0, THE CODE PUTS H=1.D-6).
    /// </summary>
    private double MeH = 1E-5;

    /// <summary>
    /// SWITCH FOR THE COMPUTATION OF THE JACOBIAN:
    /// IJAC=0: JACOBIAN IS COMPUTED INTERNALLY BY FINITE
    /// DIFFERENCES, SUBROUTINE "JAC" IS NEVER CALLED.
    /// IJAC=1: JACOBIAN IS SUPPLIED BY SUBROUTINE JAC.
    /// </summary>
    private int MeIJAC = 1;

    //private bool MeUserJacobian = true;

    /// <summary>
    /// SWITCH FOR THE BANDED STRUCTURE OF THE JACOBIAN:
    /// MLJAC=N: JACOBIAN IS A FULL MATRIX. THE LINEAR
    /// ALGEBRA IS DONE BY FULL-MATRIX GAUSS-ELIMINATION.
    /// 0.LE.MLJAC.LT.N: MLJAC IS THE LOWER BANDWITH OF JACOBIAN
    /// MATRIX (.GE. NUMBER OF NON-ZERO DIAGONALS BELOW
    /// THE MAIN DIAGONAL).
    /// </summary>
    private int MeMLJAC;

    /// <summary>
    /// UPPER BANDWITH OF JACOBIAN  MATRIX (.GE. NUMBER OF NON-
    /// ZERO DIAGONALS ABOVE THE MAIN DIAGONAL).
    /// NEED NOT BE DEFINED IF MLJAC=N.
    /// </summary>
    private int MeMUJAC = 0;

    /// <summary>
    /// GIVES INFORMATION ON THE MASS-MATRIX:
    /// IMAS=0: M IS SUPPOSED TO BE THE IDENTITY
    /// MATRIX, MAS IS NEVER CALLED.
    /// IMAS=1: MASS-MATRIX  IS SUPPLIED.
    /// </summary>
    private int MeIMAS = 0;

    /// <summary>
    /// SWITCH FOR THE BANDED STRUCTURE OF THE MASS-MATRIX:
    /// MLMAS=N: THE FULL MATRIX CASE. THE LINEAR
    /// ALGEBRA IS DONE BY FULL-MATRIX GAUSS-ELIMINATION.
    /// 0.LE.MLMAS.LT.N: MLMAS IS THE LOWER BANDWITH OF THE
    /// MATRIX (.GE. NUMBER OF NON-ZERO DIAGONALS BELOW
    /// THE MAIN DIAGONAL).
    /// MLMAS IS SUPPOSED TO BE .LE. MLJAC.
    /// </summary>
    private int MeMLMAS;

    /// <summary>
    /// UPPER BANDWITH OF MASS-MATRIX (.GE. NUMBER OF NON-
    /// ZERO DIAGONALS ABOVE THE MAIN DIAGONAL).
    /// NEED NOT BE DEFINED IF MLMAS=N.
    /// MUMAS IS SUPPOSED TO BE .LE. MUJAC.
    /// </summary>
    private int MeMUMAS = 0;

    #endregion Valores especificos para esta subrutina

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the OdeImplicitRungeKutta5 class.
    /// </summary>
    public OdeImplicitRungeKutta5()
    {
      InitializeRungeKuttaClasses();
    }

    /// <summary>
    /// Initializes a new instance of the OdeImplicitRungeKutta5 class.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public OdeImplicitRungeKutta5(OdeFunction function, OdeJacobian jacobian, int numEquations)
    {
      InitializeRungeKuttaClasses();

      // C     IOUT        SWITCH FOR CALLING THE SUBROUTINE SOLOUTR:
      // C                    IOUT=0: SUBROUTINE IS NEVER CALLED
      // C
      _IOut = 1;
      MeIJAC = 1;
      base.InitializeRungeKutta(function, jacobian, numEquations);
    }

    /// <summary>
    /// Initializes a new instance of the OdeImplicitRungeKutta5 class.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public OdeImplicitRungeKutta5(OdeFunction function, int numEquations)
    {
      InitializeRungeKuttaClasses();

      // C     IOUT        SWITCH FOR CALLING THE SUBROUTINE SOLOUTR:
      // C                    IOUT=0: SUBROUTINE IS NEVER CALLED
      // C
      _IOut = 1;
      MeIJAC = 0;
      base.InitializeRungeKutta(function, null, numEquations);
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public override void InitializeODEs(OdeFunction function, int numEquations)
    {
      // C     IOUT        SWITCH FOR CALLING THE SUBROUTINE SOLOUTR:
      // C                    IOUT=0: SUBROUTINE IS NEVER CALLED
      // C
      _IOut = 1;
      MeIJAC = 0;
      base.InitializeRungeKutta(function, null, numEquations);

      _InvokeSetInitialValues = true;
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    public override void InitializeODEs(OdeFunction function, int numEquations, double t0, double[] y0)
    {
      // C     IOUT        SWITCH FOR CALLING THE SUBROUTINE SOLOUTR:
      // C                    IOUT=0: SUBROUTINE IS NEVER CALLED
      // C
      _IOut = 1;
      MeIJAC = 0;
      base.InitializeRungeKutta(function, null, numEquations);
      SetInitialValues(t0, y0);
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    public void InitializeODEs(OdeFunction function, OdeJacobian jacobian, int numEquations)
    {
      _IOut = 1;
      MeIJAC = 1;
      base.InitializeRungeKutta(function, jacobian, numEquations);

      _InvokeSetInitialValues = true;
    }

    /// <summary>
    /// Method that initialize the ODE to solve.
    /// </summary>
    /// <param name="function">A function that evaluates the right side of the differential equations.</param>
    /// <param name="jacobian">A function that evaluates the jacobian matrix.</param>
    /// <param name="numEquations">The number of differential equations.</param>
    /// <param name="t0">The initial value for the independent variable.</param>
    /// <param name="y0">A vector of size N containing the initial conditions. N is the number of differential equations.</param>
    public void InitializeODEs(OdeFunction function, OdeJacobian jacobian, int numEquations, double t0, double[] y0)
    {
      _IOut = 1;
      MeIJAC = 1;
      base.InitializeRungeKutta(function, jacobian, numEquations);

      SetInitialValues(t0, y0);
    }

    #endregion Methods

    #region Public Methods

    internal override void InitializeRungeKuttaClasses()
    {
      bbampl = new BBAMPL();
      dec = new DEC();
      sol = new SOL();
      dech = new DECH();
      solh = new SOLH();
      decc = new DECC();
      solc = new SOLC();
      dechc = new DECHC();
      solhc = new SOLHC();
      decb = new DECB();
      solb = new SOLB();
      decbc = new DECBC();
      solbc = new SOLBC();
      elmhes = new ELMHES();
      //solout = new SOLOUTR(INTERN);
      contr5 = new CONTR5(CONRA5);
      decomr = new DECOMR(dec, decb, elmhes, dech, LINAL);
      decomc = new DECOMC(decc, decbc, dechc, LINAL);
      slvrar = new SLVRAR(sol, solb, solh, LINAL);
      slvrai = new SLVRAI(solc, solbc, solhc, LINAL);
      slvrad = new SLVRAD(sol, solc, solb, solbc, solh, solhc, LINAL);
      estrad = new ESTRAD(sol, solb, solh, LINAL);
      estrav = new ESTRAV(sol, solb, solh, LINAL);
      slvrod = new SLVROD(sol, solb, LINAL);
      slvseu = new SLVSEU(sol, solb, solh, LINAL);
      radcor = new RADCOR(decomr, decomc, slvrad, estrad, CONRA5, LINAL);
      radau5 = new RADAU5(radcor);
      //dr1_radau5 = new DR1_RADAU5(fvpol, jvpol, solout, bbampl, radau5);

      base._RKSolOut = new RKSolOut(contr5);
    }

    internal override void InitializeExceptionMessages()
    {
      _Errors = new string[5];
      _Errors[0] = "";
      _Errors[1] = "INPUT IS NOT CONSISTENT.";
      _Errors[2] = "LARGER NMAX IS NEEDED.";
      _Errors[3] = "STEP SIZE BECOMES TOO SMALL.";
      _Errors[4] = "MATRIX IS REPEATEDLY SINGULAR.";
    }

    internal override void InitializeFunctionAndJacobian(OdeFunction fun, OdeJacobian? jac)
    {
      fvpol = new FVPOL(_NEquations, fun);
      if (!(jac is null))
        jvpol = new JVPOL(_NEquations, jac);
    }

    internal override void InitializeWorkingSpace()
    {
      // C     WORK        ARRAY OF WORKING SPACE OF LENGTH "LWORK".
      // C                 WORK(1), WORK(2),.., WORK(20) SERVE AS PARAMETERS
      // C                 FOR THE CODE. FOR STANDARD USE OF THE CODE
      // C                 WORK(1),..,WORK(20) MUST BE SET TO ZERO BEFORE
      // C                 CALLING. SEE BELOW FOR A MORE SOPHISTICATED USE.
      // C                 WORK(21),..,WORK(LWORK) SERVE AS WORKING SPACE
      // C                 FOR ALL VECTORS AND MATRICES.
      // C                 "LWORK" MUST BE AT LEAST
      // C                             N*(LJAC+LMAS+3*LE+12)+20
      // C                 WHERE
      // C                    LJAC=N              IF MLJAC=N (FULL JACOBIAN)
      // C                    LJAC=MLJAC+MUJAC+1  IF MLJAC<N (BANDED JAC.)
      // C                 AND
      // C                    LMAS=0              IF IMAS=0
      // C                    LMAS=N              IF IMAS=1 AND MLMAS=N (FULL)
      // C                    LMAS=MLMAS+MUMAS+1  IF MLMAS<N (BANDED MASS-M.)
      // C                 AND
      // C                    LE=N               IF MLJAC=N (FULL JACOBIAN)
      // C                    LE=2*MLJAC+MUJAC+1 IF MLJAC<N (BANDED JAC.)
      // C
      // C                 IN THE USUAL CASE WHERE THE JACOBIAN IS FULL AND THE
      // C                 MASS-MATRIX IS THE INDENTITY (IMAS=0), THE MINIMUM
      // C                 STORAGE REQUIREMENT IS
      // C                             LWORK = 4*N*N+12*N+20.
      // C                 IF IWORK(9)=M1>0 THEN "LWORK" MUST BE AT LEAST
      // C                          N*(LJAC+12)+(N-M1)*(LMAS+3*LE)+20
      // C                 WHERE IN THE DEFINITIONS OF LJAC, LMAS AND LE THE
      // C                 NUMBER N CAN BE REPLACED BY N-M1.
      // C
      // C     LWORK       DECLARED LENGTH OF ARRAY "WORK".
      // C
      // C     IWORK       INTEGER WORKING SPACE OF LENGTH "LIWORK".
      // C                 IWORK(1),IWORK(2),...,IWORK(20) SERVE AS PARAMETERS
      // C                 FOR THE CODE. FOR STANDARD USE, SET IWORK(1),..,
      // C                 IWORK(20) TO ZERO BEFORE CALLING.
      // C                 IWORK(21),...,IWORK(LIWORK) SERVE AS WORKING AREA.
      // C                 "LIWORK" MUST BE AT LEAST 3*N+20.
      // C
      // C     LIWORK      DECLARED LENGTH OF ARRAY "IWORK".

      // C                 IN THE USUAL CASE WHERE THE JACOBIAN IS FULL AND THE
      // C                 MASS-MATRIX IS THE INDENTITY (IMAS=0), THE MINIMUM
      // C                 STORAGE REQUIREMENT IS
      // C                             LWORK = 4*N*N+12*N+20.
      _Lrw = 4 * _NEquations * _NEquations + 12 * _NEquations + 20;

      // C     IWORK       INTEGER WORKING SPACE OF LENGTH "LIWORK".
      // C                 IWORK(1),IWORK(2),...,IWORK(20) SERVE AS PARAMETERS
      // C                 FOR THE CODE. FOR STANDARD USE, SET IWORK(1),..,
      // C                 IWORK(20) TO ZERO BEFORE CALLING.
      // C                 IWORK(21),...,IWORK(LIWORK) SERVE AS WORKING AREA.
      // C                 "LIWORK" MUST BE AT LEAST 3*N+20.
      _Liw = 3 * _NEquations + 20;

      _RWork = new double[_Lrw];
      _IWork = new int[_Liw];
    }

    internal override void Solve()
    {
      bool WasSuccessfully = true;

      MeMLJAC = _NEquations;
      MeMLMAS = _NEquations;

      radau5.Run(_NEquations, fvpol, ref _T0, ref _Y0, 0, MeTf, ref MeH, ref _RelTolArray, 0
      , ref _AbsTolArray, 0, _ITolRK, jvpol, MeIJAC, ref MeMLJAC, ref MeMUJAC, bbampl, MeIMAS, MeMLMAS, ref MeMUMAS, _RKSolOut, _IOut, ref _RWork, 0, _Lrw
      , ref _IWork, 0, _Liw, _RPar, 0, _IPar, 0, ref _IDID);

      if (_IDID < 0)
        WasSuccessfully = false;

      if (WasSuccessfully == false)
      {
        throw new Exception(_Errors[-_IDID]);
      }
    }

    #endregion Public Methods
  }
}
