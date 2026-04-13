#region Translated by Jose Antonio De Santiago-Castillo.
// Copyright
//Translated by Jose Antonio De Santiago-Castillo.
//E-mail:JAntonioDeSantiago@gmail.com
//Website: www.DotNumerics.com
//
//Fortran to C# Translation.
//Translated by:
//F2CSharp Version 0.72 (Dicember 7, 2009)
//Code Optimizations: , assignment operator, for-loop: array indexes
//

#endregion Translated by Jose Antonio De Santiago-Castillo.

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Ode.Obsolete.Radau5
{
  #region Interface

  /// <summary>
  /// Defines the right-hand side callback used by the obsolete RADAU5 wrapper.
  /// </summary>
  public interface IFVPOL
  {
    /// <summary>
    /// Evaluates the right-hand side of the differential equation system.
    /// </summary>
    /// <param name="N">The number of equations.</param>
    /// <param name="X">The current value of the independent variable.</param>
    /// <param name="Y">The current solution vector.</param>
    /// <param name="offset_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="F">Receives the evaluated function values.</param>
    /// <param name="offset_f">The offset into <paramref name="F"/>.</param>
    /// <param name="RPAR">A user-supplied real parameter.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    void Run(int N, double X, double[] Y, int offset_y, ref double[] F, int offset_f, double RPAR, int IPAR);
  }

  /// <summary>
  /// Defines the Jacobian callback used by the obsolete RADAU5 wrapper.
  /// </summary>
  public interface IJVPOL
  {
    /// <summary>
    /// Evaluates the Jacobian matrix.
    /// </summary>
    /// <param name="N">The number of equations.</param>
    /// <param name="X">The current value of the independent variable.</param>
    /// <param name="Y">The current solution vector.</param>
    /// <param name="offset_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="DFY">Receives the Jacobian entries.</param>
    /// <param name="offset_dfy">The offset into <paramref name="DFY"/>.</param>
    /// <param name="LDFY">The leading dimension of <paramref name="DFY"/>.</param>
    /// <param name="RPAR">A user-supplied real parameter.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    void Run(int N, double X, double[] Y, int offset_y, ref double[] DFY, int offset_dfy, int LDFY, double RPAR
             , int IPAR);
  }

  /// <summary>
  /// Defines the mass-matrix callback used by the obsolete RADAU5 wrapper.
  /// </summary>
  public interface IBBAMPL
  {
    /// <summary>
    /// Fills the banded mass matrix.
    /// </summary>
    /// <param name="N">The number of equations.</param>
    /// <param name="B">Receives the matrix entries.</param>
    /// <param name="offset_b">The offset into <paramref name="B"/>.</param>
    /// <param name="LB">The leading dimension or bandwidth.</param>
    /// <param name="RPAR">User-supplied real parameters.</param>
    /// <param name="offset_rpar">The offset into <paramref name="RPAR"/>.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    void Run(int N, ref double[] B, int offset_b, int LB, double[] RPAR, int offset_rpar, int IPAR);
  }

  /// <summary>
  /// Defines the dense-output callback used by the obsolete RADAU5 wrapper.
  /// </summary>
  public interface ISOLOUTR
  {
    /// <summary>
    /// Handles intermediate output during integration.
    /// </summary>
    /// <param name="NR">The current step number.</param>
    /// <param name="XOLD">The previous grid point.</param>
    /// <param name="X">The current grid point.</param>
    /// <param name="Y">The current solution vector.</param>
    /// <param name="offset_y">The offset into <paramref name="Y"/>.</param>
    /// <param name="CONT">The continuous output coefficients.</param>
    /// <param name="offset_cont">The offset into <paramref name="CONT"/>.</param>
    /// <param name="LRC">The length of the continuous output array.</param>
    /// <param name="N">The number of equations.</param>
    /// <param name="RPAR">A user-supplied real parameter.</param>
    /// <param name="IPAR">A user-supplied integer parameter.</param>
    /// <param name="IRTRN">A return flag that can be used to influence integration.</param>
    void Run(int NR, double XOLD, double X, double[] Y, int offset_y, double[] CONT, int offset_cont, int LRC
             , int N, double RPAR, int IPAR, int IRTRN);
  }

  #endregion Interface

  #region The Class: SOLOUT

  ////----------------------------------------------------------------------------------------------------------------------------
  ////                                                     The Class: SOLOUT
  ////----------------------------------------------------------------------------------------------------------------------------

  //// C
  //// C

  ///// <summary>
  ///// NAME (EXTERNAL) OF SUBROUTINE PROVIDING THE
  ///// NUMERICAL SOLUTION DURING INTEGRATION.
  ///// IF IOUT.GE.1, IT IS CALLED AFTER EVERY SUCCESSFUL STEP.
  ///// SUPPLY A DUMMY SUBROUTINE IF IOUT=0.
  ///// IT MUST HAVE THE FORM
  ///// SUBROUTINE SOLOUTR (NR,XOLD,X,Y,N,CON,ICOMP,ND,
  ///// RPAR,IPAR,IRTRN)
  ///// DIMENSION Y(N),CON(5*ND),ICOMP(ND)
  ///// ....
  ///// SOLOUTR FURNISHES THE SOLUTION "Y" AT THE NR-TH
  ///// GRID-POINT "X" (THEREBY THE INITIAL VALUE IS
  ///// THE FIRST GRID-POINT).
  ///// "XOLD" IS THE PRECEEDING GRID-POINT.
  ///// "IRTRN" SERVES TO INTERRUPT THE INTEGRATION. IF IRTRN
  ///// IS SET .LT.0, DOPRI5 WILL RETURN TO THE CALLING PROGRAM.
  ///// IF THE NUMERICAL SOLUTION IS ALTERED IN SOLOUTR,
  ///// SET  IRTRN = 2
  /////
  ///// -----  CONTINUOUS OUTPUT: -----
  ///// DURING CALLS TO "SOLOUTR", A CONTINUOUS SOLUTION
  ///// FOR THE INTERVAL [XOLD,X] IS AVAILABLE THROUGH
  ///// THE FUNCTION
  ///// .GT..GT..GT.   CONTD5(I,S,CON,ICOMP,ND)   .LT..LT..LT.
  ///// WHICH PROVIDES AN APPROXIMATION TO THE I-TH
  ///// COMPONENT OF THE SOLUTION AT THE POINT S. THE VALUE
  ///// S SHOULD LIE IN THE INTERVAL [XOLD,X].
  ///// </summary>
  //internal class SOLOUTR : ISOLOUTR
  //{
  //    //private double T0 = 0;
  //    //private double TEnd = 1;
  //    //private double DeltaT=1;
  //    private double[,] MeSolution;
  //    private int MeNEquations;
  //    /// <summary>
  //    /// El valor que se esta calculando.
  //    /// </summary>
  //    double MeT = 0;
  //    /// <summary>
  //    /// El indice en el que se esta calculando.
  //    /// </summary>
  //    int MeIndex = 1;

  //    int MeSolutionLength;

  //    bool isDeltaPositive = true;

  //    CONTR5 Contr5;

  //    internal SOLOUTR(double[,] solution, CONTR5 contr5)
  //    {
  //        this.MeSolution = solution;
  //        this.MeNEquations = solution.GetLength(1) - 1;
  //        if (solution[1, 0] < solution[0, 0]) this.isDeltaPositive = false;
  //        this.Contr5 = contr5;

  //        this.MeIndex = 1;
  //        this.MeSolutionLength = solution.GetLength(0);
  //        this.MeT = solution[1, 0];
  //    }

  //    public void solout(int NR, double XOLD, double X, double[] Y, int o_y, double[] CONT, int o_cont, int LRC
  //                        , int N, double RPAR, int IPAR, int IRTRN)
  //    {
  //        #region                                         Array Index Correction
  //        //--------------------------------------------------------------------------------------------------------------------
  //        //                                              Array Index Correction
  //        //--------------------------------------------------------------------------------------------------------------------

  //        int c_y = -1 + o_y; int c_cont = -1 + o_cont;

  //        #endregion

  //        bool MyContinue = true;

  //        while (MyContinue && this.MeIndex < this.MeSolutionLength)
  //        {
  //            this.MeT = this.MeSolution[this.MeIndex, 0];
  //            if ((this.isDeltaPositive && X >= this.MeT) || (this.isDeltaPositive == false && X <= this.MeT))
  //            {
  //                for (int j = 1; j <= this.MeNEquations; j++)
  //                {
  //                    //double A = this.Contr5.Contr5ImplicitRK(j, this.MeT, CONT, o_cont, LRC);
  //                    this.MeSolution[this.MeIndex, j] = this.Contr5.contr5(j, this.MeT, CONT, o_cont, LRC);
  //                }
  //                this.MeIndex++;
  //            }
  //            else MyContinue = false;
  //        }
  //        return;
  //    }
  //}

  #endregion The Class: SOLOUT

  #region The Class: FAREN

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                     The Class: FAREN
  //----------------------------------------------------------------------------------------------------------------------------

  /// <summary>
  /// Adapts an <see cref="OdeFunction"/> to the translated Radau5 function callback interface.
  /// </summary>
  internal class FVPOL : IFVPOL
  {
    #region Fields

    private OdeFunction MeFunction;

    private double[] MeY;
    private double[] MeYDot;
    private int MeNEq;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="FVPOL"/> class.
    /// </summary>
    /// <param name="NEq">The number of equations.</param>
    /// <param name="Func">The function that evaluates the differential equation system.</param>
    internal FVPOL(int NEq, OdeFunction Func)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeYDot = new double[NEq];
      MeFunction = Func;
    }

    #endregion Constructor

    /// <inheritdoc/>
    public void Run(int N, double X, double[] Y, int offset_y, ref double[] F, int offset_f, double RPAR, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + offset_y;
      int c_f = -1 + offset_f;

      #endregion Array Index Correction

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + offset_y];
      }

      MeFunction(X, MeY, MeYDot);

      if (MeYDot.Length == MeNEq)
      {
        for (int i = 0; i < MeNEq; i++)
        {
          F[i + offset_f] = MeYDot[i];
        }
      }
      else
      {
        //Erroe
      }
      return;
    }
  }

  #region The Class: JVPOL

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                     The Class: JVPOL
  //----------------------------------------------------------------------------------------------------------------------------

  // C
  // C
  /// <summary>
  /// Adapts an <see cref="OdeJacobian"/> to the translated Radau5 Jacobian callback interface.
  /// </summary>
  internal class JVPOL : IJVPOL
  {
    private OdeJacobian MeJacobian;
    private double[] MeY;
    private double[,] MeJac;
    private int MeNEq;

    /// <summary>
    /// Initializes a new instance of the <see cref="JVPOL"/> class.
    /// </summary>
    /// <param name="NEq">The number of equations.</param>
    /// <param name="Jac">The Jacobian evaluator.</param>
    internal JVPOL(int NEq, OdeJacobian Jac)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeJacobian = Jac;
      MeJac = new double[NEq, NEq];
    }

    /// <inheritdoc/>
    public void Run(int N, double X, double[] Y, int offset_y, ref double[] DFY, int offset_dfy, int LDFY, double RPAR
                       , int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + offset_y;
      int c_dfy = -1 - LDFY + offset_dfy;

      #endregion Array Index Correction

      //// C --- JACOBIAN OF VAN DER POL'S EQUATION
      //DFY[1 + 1 * LDFY + c_dfy] = 0.0E0;
      //DFY[1 + 2 * LDFY + c_dfy] = 1.0E0;
      //DFY[2 + 1 * LDFY + c_dfy] = (-2.0E0 * Y[1 + c_y] * Y[2 + c_y] - 1.0E0) / RPAR;
      //DFY[2 + 2 * LDFY + c_dfy] = (1.0E0 - Math.Pow(Y[1 + c_y], 2)) / RPAR;
      //return;

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + offset_y];
      }

      MeJacobian(X, MeY, MeJac);

      for (int j = 0; j < MeNEq; j++)
      {
        for (int i = 0; i < MeNEq; i++)
        {
          DFY[i + j * LDFY + offset_dfy] = MeJac[i, j];
        }
      }

      return;
    }
  }

  #endregion The Class: JVPOL

  #endregion The Class: FAREN

  #region The Class: BBAMPL

  //----------------------------------------------------------------------------------------------------------------------------
  //                                                     The Class: BBAMPL
  //----------------------------------------------------------------------------------------------------------------------------

  /// <summary>
  /// Provides the translated amplitude callback required by the Radau5 solver.
  /// </summary>
  internal class BBAMPL : IBBAMPL
  {
    #region Declaracion de variables implicitas

    //------------------------------------------------------------------------------------------------------------------------
    //                                          Declaracion de variables implicitas
    //------------------------------------------------------------------------------------------------------------------------

    private int I = 0; private double C1 = 0; private double C2 = 0; private double C3 = 0; private double C4 = 0; private double C5 = 0;

    #endregion Declaracion de variables implicitas

    /// <summary>
    /// Initializes a new instance of the <see cref="BBAMPL"/> class.
    /// </summary>
    internal BBAMPL()
    {
    }

    /// <inheritdoc/>
    public void Run(int N, ref double[] B, int offset_b, int LB, double[] RPAR, int offset_rpar, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_b = -1 - LB + offset_b;
      int c_rpar = -1 + offset_rpar;

      #endregion Array Index Correction

      // C --- MATRIX "M" FOR THE AMPLIFIER PROBLEM

      #region The Code

      //--------------------------------------------------------------------------------------------------------------------
      //                                                     The Code
      //--------------------------------------------------------------------------------------------------------------------

      for (I = 1; I <= 8; I++)
      {
        B[1 + I * LB + c_b] = 0.0E0;
        B[3 + I * LB + c_b] = 0.0E0;
      }
      C1 = 1.0E-6;
      C2 = 2.0E-6;
      C3 = 3.0E-6;
      C4 = 4.0E-6;
      C5 = 5.0E-6;
      // C
      B[2 + 1 * LB + c_b] = -C5;
      B[1 + 2 * LB + c_b] = C5;
      B[3 + 1 * LB + c_b] = C5;
      B[2 + 2 * LB + c_b] = -C5;
      B[2 + 3 * LB + c_b] = -C4;
      B[2 + 4 * LB + c_b] = -C3;
      B[1 + 5 * LB + c_b] = C3;
      B[3 + 4 * LB + c_b] = C3;
      B[2 + 5 * LB + c_b] = -C3;
      B[2 + 6 * LB + c_b] = -C2;
      B[2 + 7 * LB + c_b] = -C1;
      B[1 + 8 * LB + c_b] = C1;
      B[3 + 7 * LB + c_b] = C1;
      B[2 + 8 * LB + c_b] = -C1;
      return;

      #endregion The Code
    }
  }

  #endregion The Class: BBAMPL
}
