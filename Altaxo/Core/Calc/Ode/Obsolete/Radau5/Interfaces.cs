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

  public interface IFVPOL
  {
    void Run(int N, double X, double[] Y, int offset_y, ref double[] F, int offset_f, double RPAR, int IPAR);
  }

  public interface IJVPOL
  {
    void Run(int N, double X, double[] Y, int offset_y, ref double[] DFY, int offset_dfy, int LDFY, double RPAR
             , int IPAR);
  }

  public interface IBBAMPL
  {
    void Run(int N, ref double[] B, int offset_b, int LB, double[] RPAR, int offset_rpar, int IPAR);
  }

  public interface ISOLOUTR
  {
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

  // C
  internal class FVPOL : IFVPOL
  {
    #region Fields

    private OdeFunction MeFunction;

    private double[] MeY;
    private double[] MeYDot;
    private int MeNEq;

    #endregion Fields

    #region Constructor

    internal FVPOL(int NEq, OdeFunction Func)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeYDot = new double[NEq];
      MeFunction = Func;
    }

    #endregion Constructor

    public void Run(int N, double X, double[] Y, int o_y, ref double[] F, int o_f, double RPAR, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_f = -1 + o_f;

      #endregion Array Index Correction

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + o_y];
      }

      MeFunction(X, MeY, MeYDot);

      if (MeYDot.Length == MeNEq)
      {
        for (int i = 0; i < MeNEq; i++)
        {
          F[i + o_f] = MeYDot[i];
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
  internal class JVPOL : IJVPOL
  {
    private OdeJacobian MeJacobian;
    private double[] MeY;
    private double[,] MeJac;
    private int MeNEq;

    internal JVPOL(int NEq, OdeJacobian Jac)
    {
      MeNEq = NEq;
      MeY = new double[NEq];
      MeJacobian = Jac;
      MeJac = new double[NEq, NEq];
    }

    public void Run(int N, double X, double[] Y, int o_y, ref double[] DFY, int o_dfy, int LDFY, double RPAR
                       , int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_y = -1 + o_y;
      int c_dfy = -1 - LDFY + o_dfy;

      #endregion Array Index Correction

      //// C --- JACOBIAN OF VAN DER POL'S EQUATION
      //DFY[1 + 1 * LDFY + c_dfy] = 0.0E0;
      //DFY[1 + 2 * LDFY + c_dfy] = 1.0E0;
      //DFY[2 + 1 * LDFY + c_dfy] = (-2.0E0 * Y[1 + c_y] * Y[2 + c_y] - 1.0E0) / RPAR;
      //DFY[2 + 2 * LDFY + c_dfy] = (1.0E0 - Math.Pow(Y[1 + c_y], 2)) / RPAR;
      //return;

      for (int i = 0; i < MeNEq; i++)
      {
        MeY[i] = Y[i + o_y];
      }

      MeJacobian(X, MeY, MeJac);

      for (int j = 0; j < MeNEq; j++)
      {
        for (int i = 0; i < MeNEq; i++)
        {
          DFY[i + j * LDFY + o_dfy] = MeJac[i, j];
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

  internal class BBAMPL : IBBAMPL
  {
    #region Declaracion de variables implicitas

    //------------------------------------------------------------------------------------------------------------------------
    //                                          Declaracion de variables implicitas
    //------------------------------------------------------------------------------------------------------------------------

    private int I = 0; private double C1 = 0; private double C2 = 0; private double C3 = 0; private double C4 = 0; private double C5 = 0;

    #endregion Declaracion de variables implicitas

    internal BBAMPL()
    {
    }

    public void Run(int N, ref double[] B, int o_b, int LB, double[] RPAR, int o_rpar, int IPAR)
    {
      #region Array Index Correction

      //--------------------------------------------------------------------------------------------------------------------
      //                                              Array Index Correction
      //--------------------------------------------------------------------------------------------------------------------

      int c_b = -1 - LB + o_b;
      int c_rpar = -1 + o_rpar;

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
