#region Translated by Jose Antonio De Santiago-Castillo.
// Copyright(c) 2004, Ernst Hairer
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

namespace Altaxo.Calc.Ode.Obsolete.Dopri
{
  #region The Class: CONTD5

  // C
  public class CONTD5
  {
    #region Common variables

    #region Common Block: CONDO5 Declaration

    private CommonBlock _condo5;
    private Odouble XOLD; private Odouble H;

    #endregion Common Block: CONDO5 Declaration

    #endregion Common variables

    public CONTD5(CommonBlock CONDO5)
    {
      #region Common varaible Initialization

      #region Common Block: CONDO5 Initialization

      _condo5 = CONDO5;
      XOLD = CONDO5.doubleData[0];
      H = CONDO5.doubleData[1];

      #endregion Common Block: CONDO5 Initialization

      #endregion Common varaible Initialization
    }

    public CONTD5()
    {
      #region Initialization Common Blocks

      var CONDO5 = new CommonBlock(2, 0, 0, 0);

      #endregion Initialization Common Blocks

      #region Common varaible Initialization

      #region Common Block: CONDO5 Initialization

      _condo5 = CONDO5;
      XOLD = CONDO5.doubleData[0];
      H = CONDO5.doubleData[1];

      #endregion Common Block: CONDO5 Initialization

      #endregion Common varaible Initialization
    }

    public double Run(int II, double X, double[] CON, int offset_con, int[] ICOMP, int offset_icomp, int ND)
    {
      double contd5 = 0;

      #region Implicit Variables

      int I = 0;
      int J = 0;
      double THETA = 0;
      double THETA1 = 0;

      #endregion Implicit Variables

      #region Array Index Correction

      int o_con = -1 + offset_con;
      int o_icomp = -1 + offset_icomp;

      #endregion Array Index Correction

      // C ----------------------------------------------------------
      // C     THIS FUNCTION CAN BE USED FOR CONTINUOUS OUTPUT IN CONNECTION
      // C     WITH THE OUTPUT-SUBROUTINE FOR DOPRI5. IT PROVIDES AN
      // C     APPROXIMATION TO THE II-TH COMPONENT OF THE SOLUTION AT X.
      // C ----------------------------------------------------------
      // C ----- COMPUTE PLACE OF II-TH COMPONENT
      I = 0;
      for (J = 1; J <= ND; J++)
      {
        if (ICOMP[J + o_icomp] == II)
          I = J;
      }
      if (I == 0)
      {
        //ERROR-ERROR         WRITE (6,*) ' NO DENSE OUTPUT AVAILABLE FOR COMP.',II ;
        return contd5;
      }
      THETA = (X - XOLD.v) / H.v;
      THETA1 = 1.0E0 - THETA;
      contd5 = CON[I + o_con] + THETA * (CON[ND + I + o_con] + THETA1 * (CON[2 * ND + I + o_con] + THETA * (CON[3 * ND + I + o_con] + THETA1 * CON[4 * ND + I + o_con])));
      return contd5;
    }
  }

  #endregion The Class: CONTD5
}
