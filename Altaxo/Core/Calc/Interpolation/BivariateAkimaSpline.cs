#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{

  /// <summary>
  /// Class to spline bivariate function data (in gridded form).
  /// </summary>
  /// <remarks> The source originates from:
  /// <para>Hiroshi Akima</para>
  /// <para>"Algorithm 474: Bivariate Interpolation and smooth surface fitting based on local procedures"</para>
  /// <para>Communications of the ACM, Vol.17 (Jan.1974), Number 1</para>
  /// <para></para>
  /// <para>and where translated to C# by D.Lellinger 2005.</para>
  /// </remarks>
  public class BivariateAkimaSpline
  {
    /// <summary>
    /// This empty constructor is only be used to construct the instance by the internal static 
    /// interpolation functions.
    /// </summary>
    private BivariateAkimaSpline()
    {
    }

    IROVector _myX;
    IROVector _myY;
    IROMatrix _myZ;

    /// <summary>
    /// Constructs an Akima bivariate spline.
    /// </summary>
    /// <param name="x">ARRAY OF DIMENSION LX STORING THE X COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="y">ARRAY OF DIMENSION LY STORING THE Y COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="z">DOUBLY-DIMENSIONED ARRAY OF DIMENSION (LX,LY) STORING THE VALUES OF THE FUNCTION (Z VALUES) AT INPUT GRID POINTS</param>
    /// <param name="copyDataLocally">If true, the data where cloned before stored here in this instance. If false, the data
    /// are stored directly. Make sure then, that the data are not changed outside.</param>
    public BivariateAkimaSpline(IROVector x, IROVector y,  IROMatrix z, bool copyDataLocally)
    {
      if(copyDataLocally)
      {
        _myX = VectorMath.ToVector(new double[x.Length]);
        VectorMath.Copy(x,(IVector)_myX);

        _myY = VectorMath.ToVector(new double[y.Length]);
        VectorMath.Copy(y, (IVector)_myY);

        _myZ = new MatrixMath.BEMatrix(_myZ.Rows, _myZ.Columns);
        MatrixMath.Copy(z, (IMatrix)_myZ);
      }
      else
      {
        _myX = x;
        _myY = y;
        _myZ = z;
      }
    }
   
    public double Interpolate(double x, double y)
    {
      MatrixMath.Scalar z = new MatrixMath.Scalar();
      this.itplbv_(_myX.Length, _myY.Length, _myX, _myY, _myZ, 1, new MatrixMath.Scalar(x), new MatrixMath.Scalar(y), z);
      return z;
    }

    /// <summary>
    /// THIS SUBROUTINE INTERPOLATES, FROM VALUES OF THE FUNCTION 
    /// GIVEN AT INPUT GRID POINTS IN AN X-Y PLANE AND FOR A GIVEN 
    /// SET OF POINTS IN THE PLANE, THE VALUES OF A SINGLE-VALUED 
    /// BIVARIATE FUNCTION Z = Z(X,Y).
    /// THE METHOD IS BASED ON A PIECE-WISE FUNCTION COMPOSED OF
    /// A SET OF BICUBIC POLYNOMIALS IN X AND Y.  EACH POLYNOMIAL 
    /// IS APPLICABLE TO A RECTANGLE OF THE INPUT GRID IN THE X-Y 
    /// PLANE.  EACH POLYNOMIAL IS DETERMINED LOCALLY.
    /// </summary>
    /// <param name="x">VECTOR OF DIMENSION LX STORING THE X COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="y">VECTOR OF DIMENSION LY STORING THE Y COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="z">MATRIX OF DIMENSION (LX,LY) STORING THE VALUES OF THE FUNCTION (Z VALUES) AT INPUT GRID POINTS</param>
    /// <param name="u">VECTOR OF DIMENSION N STORING THE X COORDINATES OF DESIRED POINTS</param>
    /// <param name="v">VECTOR OF DIMENSION N STORING THE Y COORDINATES OF DESIRED POINTS</param>
    /// <param name="w">VECTOR OF DIMENSION N WHERE THE INTERPOLATED Z VALUES AT DESIRED POINTS ARE TO BE DISPLAYED</param>
    public static void Interpolate(IROVector x, IROVector y, IROMatrix z, IROVector u, IROVector v, IVector w)
    {
      BivariateAkimaSpline spline = new BivariateAkimaSpline();
      spline.itplbv_(x.Length,y.Length,x,y,z,w.Length,u,v,w);
    }

    /// <summary>
    /// Old interface to the akima spline. Since all arrays are converted to IVector or IMatrix, avoid this interface if possible.
    /// </summary>
    /// <param name="x">ARRAY OF DIMENSION LX STORING THE X COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="y">ARRAY OF DIMENSION LY STORING THE Y COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="z">DOUBLY-DIMENSIONED ARRAY OF DIMENSION (LX,LY) STORING THE VALUES OF THE FUNCTION (Z VALUES) AT INPUT GRID POINTS</param>
    /// <param name="u">ARRAY OF DIMENSION N STORING THE X COORDINATES OF DESIRED POINTS</param>
    /// <param name="v">ARRAY OF DIMENSION N STORING THE Y COORDINATES OF DESIRED POINTS</param>
    /// <param name="w">ARRAY OF DIMENSION N WHERE THE INTERPOLATED Z VALUES AT DESIRED POINTS ARE TO BE DISPLAYED</param>
    public static void Interpolate(double[] x, double[] y, double[] z, double[] u, double[] v, double[] w)
    {
      BivariateAkimaSpline spline = new BivariateAkimaSpline();
      spline.itplbv_(x.Length, y.Length, VectorMath.ToROVector(x), VectorMath.ToROVector(y), MatrixMath.ToROMatrix(z,x.Length),
        w.Length, VectorMath.ToROVector(u), VectorMath.ToROVector(v), VectorMath.ToVector(w));
    }

    #region Created from the FORTRAN sources
  
    double a { get { return zx[1]; } set { zx[1] = value; } }
    double b { get { return zx[4]; } set { zx[4] = value; } }
    double c__ { get { return zx[7]; } set { zx[7] = value; } }
    double d__ { get { return zy[1]; } set { zy[1] = value; } }
    double e { get { return zy[13]; } set { zy[13] = value; } }
    int k;
    double a1 { get { return zx[1]; } set { zx[1] = value; } }
    double b1 { get { return zx[1]; } set { zx[1] = value; } }
    double a2 { get { return zx[4]; } set { zx[4] = value; } }
    double a4 { get { return zx[7]; } set { zx[7] = value; } }
    double a5 { get { return zx[1]; } set { zx[1] = value; } }
    double b5 { get { return zx[1]; } set { zx[1] = value; } }
    double b2 { get { return zy[1]; } set { zy[1] = value; } }
    double b4 { get { return zy[13]; } set { zy[13] = value; } }
    double a3, b3;
    int n0;
    double q0 { get { return zx[1]; } set { zx[1] = value; } }
    double q1 { get { return zx[4]; } set { zx[4] = value; } }
    double q2 { get { return zx[7]; } set { zx[7] = value; } }
    double q3 { get { return zy[1]; } set { zy[1] = value; } }
    double[] equiv_86 = new double[1];
    double w1 { get { return equiv_86[0]; } set { equiv_86[0] = value; } }
    double[] equiv_85 = new double[1];
    double w2 { get { return equiv_85[0]; } set { equiv_85[0] = value; } }
    double x2 { get { return zx[2]; } set { zx[2] = value; } }
    double y2 { get { return zx[13]; } set { zx[13] = value; } }
    double x4 { get { return zx[8]; } set { zx[8] = value; } }
    double x5 { get { return zx[11]; } set { zx[11] = value; } }
    double y4 { get { return zy[2]; } set { zy[2] = value; } }
    double y5 { get { return zx[14]; } set { zx[14] = value; } }
    double w4 { get { return equiv_85[0]; } set { equiv_85[0] = value; } }
    double w3 { get { return equiv_86[0]; } set { equiv_86[0] = value; } }
    double w5 { get { return equiv_86[0]; } set { equiv_86[0] = value; } }
    double x3, y3;
    double[] equiv_41 = new double[1];
    double p00 { get { return equiv_41[0]; } set { equiv_41[0] = value; } }
    double p01 { get { return zy[5]; } set { zy[5] = value; } }
    double p10 { get { return zx[5]; } set { zx[5] = value; } }
    double p11 { get { return zxy[5]; } set { zxy[5] = value; } }
    double p02 { get { return zx[14]; } set { zx[14] = value; } }
    double p03 { get { return zy[4]; } set { zy[4] = value; } }
    double p12 { get { return zy[7]; } set { zy[7] = value; } }
    double p13 { get { return zy[8]; } set { zy[8] = value; } }
    double p20 { get { return zy[11]; } set { zy[11] = value; } }
    double p21 { get { return zy[14]; } set { zy[14] = value; } }
    double p22 { get { return zxy[1]; } set { zxy[1] = value; } }
    double[] za = new double[10]; // equiv_9
    double[] zb = new double[10]; // equiv_19
    double[] equiv_61 = new double[1];
    double dx { get { return equiv_61[0]; } set { equiv_61[0] = value; } }
    double[] equiv_62 = new double[1];
    double dy { get { return equiv_62[0]; } set { equiv_62[0] = value; } }
    double z33 { get { return equiv_41[0]; } set { equiv_41[0] = value; } }
    int ix;
    int iy;
    int[] equiv_57 = new int[1];
    int jx { get { return equiv_57[0]; } set { equiv_57[0] = value; } }
    int[] equiv_58 = new int[1];
    int jy { get { return equiv_58[0]; } set { equiv_58[0] = value; } }
    double uk { get { return equiv_61[0]; } set { equiv_61[0] = value; } }
    double vk { get { return equiv_62[0]; } set { equiv_62[0] = value; } }
    double z23 { get { return zy[4]; } set { zy[4] = value; } }
    double z24 { get { return zy[7]; } set { zy[7] = value; } }
    double z32 { get { return zy[8]; } set { zy[8] = value; } }
    double z34 { get { return zy[11]; } set { zy[11] = value; } }
    double z35 { get { return zy[14]; } set { zy[14] = value; } }
    double z42 { get { return zxy[1]; } set { zxy[1] = value; } }
    double z43 { get { return zxy[4]; } set { zxy[4] = value; } }
    double p23 { get { return zxy[4]; } set { zxy[4] = value; } }
    double z44 { get { return zxy[2]; } set { zxy[2] = value; } }
    double[] zx = new double[16];
    double[] zy = new double[16];
    double p30 { get { return zxy[2]; } set { zxy[2] = value; } }
    double z45 { get { return zxy[7]; } set { zxy[7] = value; } }
    double p31 { get { return zxy[7]; } set { zxy[7] = value; } }
    double z53 { get { return zxy[8]; } set { zxy[8] = value; } }
    double p32 { get { return zxy[8]; } set { zxy[8] = value; } }
    double z54 { get { return zxy[11]; } set { zxy[11] = value; } }
    double p33 { get { return zxy[11]; } set { zxy[11] = value; } }
    double sw;
    //double iu0;
    int[] equiv_59 = new int[1];
    int jx1 { get { return equiv_59[0]; } set { equiv_59[0] = value; } }
    int lx0;
    int ly0;
    int[] equiv_60 = new int[1];
    int jy1 { get { return equiv_60[0]; } set { equiv_60[0] = value; } }
    double wx2 { get { return zxy[13]; } set { zxy[13] = value; } }
    double wy2 { get { return equiv_85[0]; } set { equiv_85[0] = value; } }
    double wy3 { get { return equiv_86[0]; } set { equiv_86[0] = value; } }
    double wx3 { get { return zxy[14]; } set { zxy[14] = value; } }
    double[] zab = new double[9];
    double z3a1 { get { return za[0]; } set { za[0] = value; } }
    double z3a2 { get { return za[1]; } set { za[1] = value; } }
    double z3a3 { get { return za[2]; } set { za[2] = value; } }
    double z3a4 { get { return za[3]; } set { za[3] = value; } }
    double z3a5 { get { return za[4]; } set { za[4] = value; } }
    double z4a1 { get { return za[5]; } set { za[5] = value; } }
    double z4a2 { get { return za[6]; } set { za[6] = value; } }
    double z4a3 { get { return za[7]; } set { za[7] = value; } }
    double z4a4 { get { return za[8]; } set { za[8] = value; } }
    double z4a5 { get { return za[9]; } set { za[9] = value; } }
    double z3b1 { get { return zb[0]; } set { zb[0] = value; } }
    double z3b2 { get { return zb[2]; } set { zb[2] = value; } }
    double z3b3 { get { return zb[4]; } set { zb[4] = value; } }
    double z3b4 { get { return zb[6]; } set { zb[6] = value; } }
    double z3b5 { get { return zb[8]; } set { zb[8] = value; } }
    double z4b1 { get { return zb[1]; } set { zb[1] = value; } }
    double z4b2 { get { return zb[3]; } set { zb[3] = value; } }
    double z4b3 { get { return zb[5]; } set { zb[5] = value; } }
    double z4b4 { get { return zb[7]; } set { zb[7] = value; } }
    double z4b5 { get { return zb[9]; } set { zb[9] = value; } }
    int imn { get { return equiv_57[0]; } set { equiv_57[0] = value; } }
    int imx { get { return equiv_58[0]; } set { equiv_58[0] = value; } }
    double zx33 { get { return zx[5]; } set { zx[5] = value; } }
    double zx43 { get { return zx[6]; } set { zx[6] = value; } }
    double zx34 { get { return zx[9]; } set { zx[9] = value; } }
    double zx44 { get { return zx[10]; } set { zx[10] = value; } }
    double zy33 { get { return zy[5]; } set { zy[5] = value; } }
    double zy43 { get { return zy[6]; } set { zy[6] = value; } }
    double zy34 { get { return zy[9]; } set { zy[9] = value; } }
    double zy44 { get { return zy[10]; } set { zy[10] = value; } }
    double[] zxy = new double[16];
    double a3sq { get { return zx[2]; } set { zx[2] = value; } }
    double b3sq { get { return zy[2]; } set { zy[2] = value; } }
    int jxm2 { get { return equiv_59[0]; } set { equiv_59[0] = value; } }
    int lxm1;
    int lxm2;
    int lym1;
    int lxp1;
    int lym2;
    int lyp1;
    int jym2 { get { return equiv_60[0]; } set { equiv_60[0] = value; } }
    double za2b2 { get { return zab[0]; } set { zab[0] = value; } }
    double za3b2 { get { return zab[1]; } set { zab[1] = value; } }
    double za4b2 { get { return zab[2]; } set { zab[2] = value; } }
    double za2b3 { get { return zab[3]; } set { zab[3] = value; } }
    double za3b3 { get { return zab[4]; } set { zab[4] = value; } }
    double za4b3 { get { return zab[5]; } set { zab[5] = value; } }
    double za2b4 { get { return zab[6]; } set { zab[6] = value; } }
    double za3b4 { get { return zab[7]; } set { zab[7] = value; } }
    double za4b4 { get { return zab[8]; } set { zab[8] = value; } }
    double zx3b3;
    int jxml, jyml;
    double zx4b3, zy3a3, zy4a3;
    double ixpv;
    double iypv;
    double zxy33 { get { return zxy[5]; } set { zxy[5] = value; } }
    double zxy43 { get { return zxy[6]; } set { zxy[6] = value; } }
    double zxy34 { get { return zxy[9]; } set { zxy[9] = value; } }
    double zxy44 { get { return zxy[10]; } set { zxy[10] = value; } }




    /// <summary>
    /// THIS SUBROUTINE INTERPOLATES, FROM VALUES OF THE FUNCTION */
    /// GIVEN AT INPUT GRID POINTS IN AN X-Y PLANE AND FOR A GIVEN */
    /// SET OF POINTS IN THE PLANE, THE VALUES OF A SINGLE-VALUED */
    /// BIVARIATE FUNCTION Z = Z(X,Y). */
    /// THE METHOD IS BASED ON A PIECE-WISE FUNCTION COMPOSED OF */
    /// A SET OF BICUBIC POLYNOMIALS IN X AND Y.  EACH POLYNOMIAL */
    /// IS APPLICABLE TO A RECTANGLE OF THE INPUT GRID IN THE X-Y */
    /// PLANE.  EACH POLYNOMIAL IS DETERMINED LOCALLY. */
    /// THE INPUT PARAMETERS ARE */
    /// </summary>
    /// <param name="lx">NUMBER OF INPUT GRID POINTS IN THE X COORDINATE (MUST BE 2 OR GREATER)</param>
    /// <param name="ly">NUMBER OF INPUT GRID POINTS IN THE Y COORDINATE (MUST BE 2 OR GREATER)</param>
    /// <param name="x">ARRAY OF DIMENSION LX STORING THE X COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="y">ARRAY OF DIMENSION LY STORING THE Y COORDINATES OF INPUT GRID POINTS (IN ASCENDING ORDER)</param>
    /// <param name="z__">DOUBLY-DIMENSIONED ARRAY OF DIMENSION (LX,LY) STORING THE VALUES OF THE FUNCTION (Z VALUES) AT INPUT GRID POINTS</param>
    /// <param name="n">NUMBER OF POINTS AT WHICH INTERPOLATION OF THE Z VALUE IS DESIRED (MUST BE 1 OR GREATER)</param>
    /// <param name="u">ARRAY OF DIMENSION N STORING THE X COORDINATES OF DESIRED POINTS</param>
    /// <param name="v">ARRAY OF DIMENSION N STORING THE Y COORDINATES OF DESIRED POINTS</param>
    /// <param name="w">ARRAY OF DIMENSION N WHERE THE INTERPOLATED Z VALUES AT DESIRED POINTS ARE TO BE DISPLAYED</param>
    /// <returns></returns>
    /// <remarks>
    ///      SOME VARIABLES INTERNALLY USED ARE
    /// ZA  = DIVIDED DIFFERENCE OF Z WITH RESPECT TO X
    /// ZB  = DIVIDED DIFFERENCE OF Z WITH RESPECT TO Y 
    /// ZAB = SECOND ORDER DIVIDED DIFFERENCE OF Z WITH 
    ///       RESPECT TO X AND Y */
    /// ZX  = PARTIAL DERIVATIVE OF Z WITH RESPECT TO X 
    /// ZY  = PARTIAL DERIVATIVE OF Z WITH RESPECT TO Y
    /// ZXY = SECOND ORDER PARTIAL DERIVATIVE OF Z WITH 
    ///       RESPECT TO X AND Y 
    /// DECLARATION STATEMENTS
    /// PRELIMINARY PROCESSING 
    /// SETTING OF SOME INPUT PARAMETERS TO LOCAL VARIABLES 
    /// Parameter adjustments 
    ///</remarks>
    private int itplbv_(int lx, int ly, IROVector x,
      IROVector y, IROMatrix z__, int n, IROVector u, IROVector v, IVector w)
    {

      /* System generated locals */
      int z_dim1, i__1;
      double r__1;

      z_dim1 = lx;

      //iu0 = iu;
      lx0 = lx;
      lxm1 = lx0 - 1;
      lxm2 = lxm1 - 1;
      lxp1 = lx0 + 1;
      ly0 = ly;
      lym1 = ly0 - 1;
      lym2 = lym1 - 1;
      lyp1 = ly0 + 1;
      n0 = n;





      /* ERROR CHECK */
      if (lxm2 < 0)
      {
        goto L710;
      }
      if (lym2 < 0)
      {
        goto L720;
      }
      if (n0 < 1)
      {
        goto L730;
      }

      i__1 = lx0;
      for (ix = 1; ix < i__1; ++ix) // Lellid
      {
        if ((r__1 = x[ix - 1] - x[ix]) < 0)
        {
          goto L10;
        }
        else if (r__1 == 0)
        {
          goto L740;
        }
        else
        {
          goto L750;
        }
      L10:
        ;
      }
      i__1 = ly0;
      for (iy = 1; iy < i__1; ++iy) // LelliD
      {
        if ((r__1 = y[iy - 1] - y[iy]) < 0)
        {
          goto L20;
        }
        else if (r__1 == 0)
        {
          goto L770;
        }
        else
        {
          goto L780;
        }
      L20:
        ;
      }
      /* INITIAL SETTING OF PREVIOUS VALUES OF IX AND IY */
      ixpv = 0;
      iypv = 0;
      /* MAIN DO-LOOP */
      i__1 = n0;
      for (k = 0; k < i__1; ++k) // LelliD
      {
        uk = u[k];
        vk = v[k];
        /* ROUTINES TO LOCATE THE DESIRED POINT */
        /* TO FIND OUT THE IX VALUE FOR WHICH */
        /* (U(K).GE.X(IX-1)).AND.(U(K).LT.X(IX)) */
        if (lxm2 == 0)
        {
          goto L80;
        }
        if (uk >= x[lx0-1]) // LelliD
        {
          goto L70;
        }
        if (uk < x[0]) // LelliD
        {
          goto L60;
        }
        imn = 2; 
        imx = lx0;
      L30:
        ix = (imn + imx) / 2;
        if (uk >= x[ix-1]) // LelliD
        {
          goto L40;
        }
        imx = ix;
        goto L50;
      L40:
        imn = ix + 1;
      L50:
        if (imx > imn)
        {
          goto L30;
        }
        ix = imx;
        goto L90;
      L60:
        ix = 1;
        goto L90;
      L70:
        ix = lxp1;
        goto L90;
      L80:
        ix = 2;
        /* TO FIND OUT THE IY VALUE FOR WHICH */
        /* (V(K).GE.Y(IY-1)).AND.(V(K).LT.Y(IY)) */
      L90:
        if (lym2 == 0)
        {
          goto L150;
        }
        if (vk >= y[ly0-1]) // LelliD
        {
          goto L140;
        }
        if (vk < y[0]) // LelliD
        {
          goto L130;
        }
        imn = 2;
        imx = ly0;
      L100:
        iy = (imn + imx) / 2;
        if (vk >= y[iy-1]) // LelliD
        {
          goto L110;
        }
        imx = iy;
        goto L120;
      L110:
        imn = iy + 1;
      L120:
        if (imx > imn)
        {
          goto L100;
        }
        iy = imx;
        goto L160;
      L130:
        iy = 1;
        goto L160;
      L140:
        iy = lyp1;
        goto L160;
      L150:
        iy = 2;
        /* TO CHECK IF THE DESIRED POINT IS IN THE SAME RECTANGLE */
        /* AS THE PREVIOUS POINT.  IF YES, SKIP TO THE COMPUTATION */
        /* OF THE POLYNOMIAL */
      L160:
        if (ix == ixpv && iy == iypv)
        {
          goto L690;
        }
        ixpv = ix;
        iypv = iy;
        /* ROUTINES TO PICK UP NECESSARY X, Y, AND Z VALUES, TO */
        /* COMPUTE THE ZA, ZB, AND ZAB VALUES, AND TO ESTIMATE THEM */
        /* WHEN NECESSARY */
        jx = ix;
        if (jx == 1)
        {
          jx = 2;
        }
        if (jx == lxp1)
        {
          jx = lx0;
        }
        jy = iy;
        if (jy == 1)
        {
          jy = 2;
        }
        if (jy == lyp1)
        {
          jy = ly0;
        }
        jxm2 = jx - 2;
        jxml = jx - lx0;
        jym2 = jy - 2;
        jyml = jy - ly0;
        /* IN THE CORE AREA, I.E., IN THE RECTANGLE THAT CONTAINS */
        /* THE DESIRED POINT */
        x3 = x[jx - 2]; // LelliD
        x4 = x[jx-1]; // LelliD
        a3 = 1.0 / (x4 - x3);
        y3 = y[jy - 2]; // LelliD
        y4 = y[jy-1]; // LelliD
        b3 = 1.0 / (y4 - y3);
        z33 = z__[jx - 2, jy - 2]; // LelliD
        z43 = z__[jx -1, jy - 2 ]; // LelliD
        z34 = z__[jx - 2, jy -1]; // LelliD
        z44 = z__[jx -1, (jy -1)]; // LelliD
        z3a3 = (z43 - z33) * a3;
        z4a3 = (z44 - z34) * a3;
        z3b3 = (z34 - z33) * b3;
        z4b3 = (z44 - z43) * b3;
        za3b3 = (z4b3 - z3b3) * a3;
        /* IN THE X DIRECTION */
        if (lxm2 == 0)
        {
          goto L230;
        }
        if (jxm2 == 0)
        {
          goto L170;
        }
        x2 = x[jx - 3]; // LelliD 
        a2 = 1.0 / (x3 - x2);
        z23 = z__[jx - 3,(jy - 2)]; // LelliD
        z24 = z__[jx - 3,(jy -1)]; // LelliD
        z3a2 = (z33 - z23) * a2;
        z4a2 = (z34 - z24) * a2;
        if (jxml == 0)
        {
          goto L180;
        }
      L170:
        x5 = x[jx ]; // LelliD
        a4 = 1.0 / (x5 - x4);
        z53 = z__[jx + 0,(jy - 2)]; // LelliD
        z54 = z__[jx + 0,(jy -1)]; // LelliD
        z3a4 = (z53 - z43) * a4;
        z4a4 = (z54 - z44) * a4;
        if (jxm2 != 0)
        {
          goto L190;
        }
        z3a2 = z3a3 + z3a3 - z3a4;
        z4a2 = z4a3 + z4a3 - z4a4;
        goto L190;
      L180:
        z3a4 = z3a3 + z3a3 - z3a2;
        z4a4 = z4a3 + z4a3 - z4a2;
      L190:
        za2b3 = (z4a2 - z3a2) * b3;
        za4b3 = (z4a4 - z3a4) * b3;
        if (jx <= 3)
        {
          goto L200;
        }
        a1 = 1.0 / (x2 - x[jx - 4]); // LelliD
        z3a1 = (z23 - z__[jx - 4, (jy - 2)]) * a1; // LelliD
        z4a1 = (z24 - z__[jx - 4, (jy -1)]) * a1; // LelliD
        goto L210;
      L200:
        z3a1 = z3a2 + z3a2 - z3a3;
        z4a1 = z4a2 + z4a2 - z4a3;
      L210:
        if (jx >= lxm1)
        {
          goto L220;
        }
        a5 = 1.0 / (x[jx + 1] - x5); // LelliD
        z3a5 = (z__[jx + 1, (jy - 2)] - z53) * a5; // LelliD
        z4a5 = (z__[jx + 1, (jy - 1)] - z54) * a5; // LelliD
        goto L240;
      L220:
        z3a5 = z3a4 + z3a4 - z3a3;
        z4a5 = z4a4 + z4a4 - z4a3;
        goto L240;
      L230:
        z3a2 = z3a3;
        z4a2 = z4a3;
        goto L180;
        /* IN THE Y DIRECTION */
      L240:
        if (lym2 == 0)
        {
          goto L310;
        }
        if (jym2 == 0)
        {
          goto L250;
        }
        y2 = y[jy - 3]; // LelliD
        b2 = 1.0 / (y3 - y2);
        z32 = z__[jx - 2, (jy - 3) ]; // LelliD
        z42 = z__[jx -1,   (jy - 3)]; // LelliD
        z3b2 = (z33 - z32) * b2;
        z4b2 = (z43 - z42) * b2;
        if (jyml == 0)
        {
          goto L260;
        }
      L250:
        y5 = y[jy + 0]; // LelliD
        b4 = 1.0 / (y5 - y4);
        z35 = z__[jx - 2, (jy + 0)]; // LelliD
        z45 = z__[jx -1,  (jy + 0)]; // LelliD
        z3b4 = (z35 - z34) * b4;
        z4b4 = (z45 - z44) * b4;
        if (jym2 != 0)
        {
          goto L270;
        }
        z3b2 = z3b3 + z3b3 - z3b4;
        z4b2 = z4b3 + z4b3 - z4b4;
        goto L270;
      L260:
        z3b4 = z3b3 + z3b3 - z3b2;
        z4b4 = z4b3 + z4b3 - z4b2;
      L270:
        za3b2 = (z4b2 - z3b2) * a3;
        za3b4 = (z4b4 - z3b4) * a3;
        if (jy <= 3)
        {
          goto L280;
        }
        b1 = 1.0 / (y2 - y[jy - 4]); // LelliD
        z3b1 = (z32 - z__[jx - 2, (jy - 4)]) * b1; // LelliD
        z4b1 = (z42 - z__[jx - 1, (jy - 4)]) * b1; // LelliD
        goto L290;
      L280:
        z3b1 = z3b2 + z3b2 - z3b3;
        z4b1 = z4b2 + z4b2 - z4b3;
      L290:
        if (jy >= lym1)
        {
          goto L300;
        }
        b5 = 1.0 / (y[jy + 1] - y5); // LelliD
        z3b5 = (z__[jx - 2, (jy + 1)] - z35) * b5; // LelliD
        z4b5 = (z__[jx - 1, (jy + 1)] - z45) * b5; // LelliD
        goto L320;
      L300:
        z3b5 = z3b4 + z3b4 - z3b3;
        z4b5 = z4b4 + z4b4 - z4b3;
        goto L320;
      L310:
        z3b2 = z3b3;
        z4b2 = z4b3;
        goto L260;
        /* IN THE DIAGONAL DIRECTIONS */
      L320:
        if (lxm2 == 0)
        {
          goto L400;
        }
        if (lym2 == 0)
        {
          goto L410;
        }
        if (jxml == 0)
        {
          goto L350;
        }
        if (jym2 == 0)
        {
          goto L330;
        }
        za4b2 = ((z53 - z__[jx + 0, (jy - 3)]) * b2 - z4b2) * a4; // LelliD
        if (jyml == 0)
        {
          goto L340;
        }
      L330:
        za4b4 = ((z__[jx + 0, (jy + 0)] - z54) * b4 - z4b4) * a4; // LelliD
        if (jym2 != 0)
        {
          goto L380;
        }
        za4b2 = za4b3 + za4b3 - za4b4;
        goto L380;
      L340:
        za4b4 = za4b3 + za4b3 - za4b2;
        goto L380;
      L350:
        if (jym2 == 0)
        {
          goto L360;
        }
        za2b2 = (z3b2 - (z23 - z__[jx - 3, (jy - 3)]) * b2) * a2; // LelliD
        if (jyml == 0)
        {
          goto L370;
        }
      L360:
        za2b4 = (z3b4 - (z__[jx - 3, (jy + 0)] - z24) * b4) * a2; // LelliD
        if (jym2 != 0)
        {
          goto L390;
        }
        za2b2 = za2b3 + za2b3 - za2b4;
        goto L390;
      L370:
        za2b4 = za2b3 + za2b3 - za2b2;
        goto L390;
      L380:
        if (jxm2 != 0)
        {
          goto L350;
        }
        za2b2 = za3b2 + za3b2 - za4b2;
        za2b4 = za3b4 + za3b4 - za4b4;
        goto L420;
      L390:
        if (jxml != 0)
        {
          goto L420;
        }
        za4b2 = za3b2 + za3b2 - za2b2;
        za4b4 = za3b4 + za3b4 - za2b4;
        goto L420;
      L400:
        za2b2 = za3b2;
        za4b2 = za3b2;
        za2b4 = za3b4;
        za4b4 = za3b4;
        goto L420;
      L410:
        za2b2 = za2b3;
        za2b4 = za2b3;
        za4b2 = za4b3;
        za4b4 = za4b3;
        /* NUMERICAL DIFFERENTIATION   ---   TO DETERMINE PARTIAL */
        /* DERIVATIVES ZX, ZY, AND ZXY AS WEIGHTED MEANS OF DIVIDED */
        /* DIFFERENCES ZA, ZB, AND ZAB, RESPECTIVELY */
      L420:
        for (jy = 2; jy <= 3; ++jy)
        {
          for (jx = 2; jx <= 3; ++jx)
          {
            r__1 = za[jx + 2 + (jy - 1) * 5 - 6] - za[jx + 1 + (jy - 1) * 5 - 6];
            w2 = Math.Abs(r__1);
            r__1 = za[jx + (jy - 1) * 5 - 6] - za[jx - 1 + (jy - 1) * 5 - 6];
            w3 = Math.Abs(r__1);
            sw = w2 + w3;
            if (sw == 0)
            {
              goto L430;
            }
            wx2 = w2 / sw;
            wx3 = w3 / sw;
            goto L440;
          L430:
            wx2 = 0.5;
            wx3 = 0.5;
          L440:
            zx[jx + (jy << 2) - 5] = wx2 * za[jx + (jy - 1) * 5 - 6] + wx3 * za[jx + 1 + (jy - 1) * 5 - 6];
            r__1 = zb[jx - 1 + (jy + 2 << 1) - 3] - zb[jx - 1 + (jy + 1 << 1) - 3];
            w2 = Math.Abs(r__1);
            r__1 = zb[jx - 1 + (jy << 1) - 3] - zb[jx - 1 + (jy - 1 << 1) - 3];
            w3 = Math.Abs(r__1);
            sw = w2 + w3;
            if (sw == 0.0)
            {
              goto L450;
            }
            wy2 = w2 / sw;
            wy3 = w3 / sw;
            goto L460;
          L450:
            wy2 = 0.5;
            wy3 = 0.5;
          L460:
            zy[jx + (jy << 2) - 5] = wy2 * zb[jx - 1 + (jy << 1) - 3] + wy3 * zb[jx - 1 + (jy + 1 << 1) - 3];
            zxy[jx + (jy << 2) - 5] = wy2 * (wx2 * zab[jx - 1 + (jy - 1) * 3 - 4] + wx3 * zab[jx + (jy - 1) * 3 - 4])
              + wy3 * (wx2 * zab[jx - 1 + jy * 3 - 4] + wx3 * zab[jx + jy * 3 - 4]);
            /* L470: */
          }
          /* L480: */
        }
        /* WHEN (U(K).LT.X(1)).OR.(U(K).GT.X(LX)) */
        if (ix == lxp1)
        {
          goto L530;
        }
        if (ix != 1)
        {
          goto L590;
        }
        w2 = a4 * (a3 * 3.0 + a4);
        w1 = a3 * 2.0 * (a3 - a4) + w2;
        for (jy = 2; jy <= 3; ++jy)
        {
          zx[(jy << 2) - 4] = (w1 * za[(jy - 1) * 5 - 5] + w2 * za[(jy - 1) * 5 - 4]) / (w1 + w2);
          zy[(jy << 2) - 4] = zy[(jy << 2) - 3] + zy[(jy << 2) - 3] - zy[(jy << 2) - 2];
          zxy[(jy << 2) - 4] = zxy[(jy << 2) - 3] + zxy[(jy << 2) - 3] - zxy[(jy << 2) - 2];
          for (jx1 = 2; jx1 <= 3; ++(jx1))
          {
            jx = 5 - jx1;
            zx[jx + (jy << 2) - 5] = zx[jx - 1 + (jy << 2) - 5];
            zy[jx + (jy << 2) - 5] = zy[jx - 1 + (jy << 2) - 5];
            zxy[jx + (jy << 2) - 5] = zxy[jx - 1 + (jy << 2) - 5];
            /* L490: */
          }
          /* L500: */
        }
        x3 -= 1.0 / a4;
        z33 -= z3a2 / a4;
        for (jy = 1; jy <= 5; ++jy)
        {
          zb[(jy << 1) - 1] = zb[(jy << 1) - 2];
          /* L510: */
        }
        for (jy = 2; jy <= 4; ++jy)
        {
          zb[(jy << 1) - 2] -= zab[(jy - 1) * 3 - 3] / a4;
          /* L520: */
        }
        a3 = a4;
        jx = 1;
        goto L570;
      L530:
        w4 = a2 * (a3 * 3.0 + a2);
        w5 = a3 * 2.0 * (a3 - a2) + w4;
        for (jy = 2; jy <= 3; ++jy)
        {
          zx[(jy << 2) - 1] = (w4 * za[(jy - 1) * 5 - 2] + w5 * za[(jy - 1) * 5 - 1]) / (w4 + w5);
          zy[(jy << 2) - 1] = zy[(jy << 2) - 2] + zy[(jy << 2) - 2] - zy[(jy << 2) - 3];
          zxy[(jy << 2) - 1] = zxy[(jy << 2) - 2] + zxy[(jy << 2) - 2] - zxy[(jy << 2) - 3];
          for (jx = 2; jx <= 3; ++jx)
          {
            zx[jx + (jy << 2) - 5] = zx[jx + 1 + (jy << 2) - 5];
            zy[jx + (jy << 2) - 5] = zy[jx + 1 + (jy << 2) - 5];
            zxy[jx + (jy << 2) - 5] = zxy[jx + 1 + (jy << 2) - 5];
            /* L540: */
          }
          /* L550: */
        }
        x3 = x4;
        z33 = z43;
        for (jy = 1; jy <= 5; ++jy)
        {
          zb[(jy << 1) - 2] = zb[(jy << 1) - 1];
          /* L560: */
        }
        a3 = a2;
        jx = 3;
      L570:
        za[2] = za[jx];
        for (jy = 1; jy <= 3; ++jy)
        {
          zab[jy * 3 - 2] = zab[jx + jy * 3 - 4];
          /* L580: */
        }
        /* WHEN (V(K).LT.Y(1)).OR.(V(K).GT.Y(LY)) */
      L590:
        if (iy == lyp1)
        {
          goto L630;
        }
        if (iy != 1)
        {
          goto L680;
        }
        w2 = b4 * (b3 * 3.0 + b4);
        w1 = b3 * 2.0 * (b3 - b4) + w2;
        for (jx = 2; jx <= 3; ++jx)
        {
          if (jx == 3 && ix == lxp1)
          {
            goto L600;
          }
          if (jx == 2 && ix == 1)
          {
            goto L600;
          }
          zy[jx - 1] = (w1 * zb[jx - 2] + w2 * zb[jx]) / (w1 + w2);
          zx[jx - 1] = zx[jx + 3] + zx[jx + 3] - zx[jx + 7];
          zxy[jx - 1] = zxy[jx + 3] + zxy[jx + 3] - zxy[jx + 7];
        L600:
          for (jy1 = 2; jy1 <= 3; ++jy1)
          {
            jy = 5 - jy1;
            zy[jx + (jy << 2) - 5] = zy[jx + (jy - 1 << 2) - 5];
            zx[jx + (jy << 2) - 5] = zx[jx + (jy - 1 << 2) - 5];
            zxy[jx + (jy << 2) - 5] = zxy[jx + (jy - 1 << 2) - 5];
            /* L610: */
          }
          /* L620: */
        }
        y3 -= 1.0 / b4;
        z33 -= z3b2 / b4;
        z3a3 -= za3b2 / b4;
        z3b3 = z3b2;
        za3b3 = za3b2;
        b3 = b4;
        goto L670;
      L630:
        w4 = b2 * (b3 * 3.0 + b2);
        w5 = b3 * 2.0 * (b3 - b2) + w4;
        for (jx = 2; jx <= 3; ++jx)
        {
          if (jx == 3 && ix == lxp1)
          {
            goto L640;
          }
          if (jx == 2 && ix == 1)
          {
            goto L640;
          }
          zy[jx + 11] = (w4 * zb[jx + 4] + w5 * zb[jx + 6]) / (w4 + w5);
          zx[jx + 11] = zx[jx + 7] + zx[jx + 7] - zx[jx + 3];
          zxy[jx + 11] = zxy[jx + 7] + zxy[jx + 7] - zxy[jx + 3];
        L640:
          for (jy = 2; jy <= 3; ++jy)
          {
            zy[jx + (jy << 2) - 5] = zy[jx + (jy + 1 << 2) - 5];
            zx[jx + (jy << 2) - 5] = zx[jx + (jy + 1 << 2) - 5];
            zxy[jx + (jy << 2) - 5] = zxy[jx + (jy + 1 << 2) - 5];
            /* L650: */
          }
          /* L660: */
        }
        y3 = y4;
        z33 += z3b3 / b3;
        z3a3 += za3b3 / b3;
        z3b3 = z3b4;
        za3b3 = za3b4;
        b3 = b2;
      L670:
        if (ix != 1 && ix != lxp1)
        {
          goto L680;
        }
        jx = ix / lxp1 + 2;
        jx1 = 5 - jx;
        jy = iy / lyp1 + 2;
        jy1 = 5 - jy;
        zx[jx + (jy << 2) - 5] = zx[jx1 + (jy << 2) - 5] + zx[jx + (jy1
          << 2) - 5] - zx[jx1 + (jy1 << 2) - 5];
        zy[jx + (jy << 2) - 5] = zy[jx1 + (jy << 2) - 5] + zy[jx + (jy1
          << 2) - 5] - zy[jx1 + (jy1 << 2) - 5];
        zxy[jx + (jy << 2) - 5] = zxy[jx1 + (jy << 2) - 5] + zxy[jx + (
          jy1 << 2) - 5] - zxy[jx1 + (jy1 << 2) - 5];
        /* DETERMINATION OF THE COEFFICIENTS OF THE POLYNOMIAL */
      L680:
        zx3b3 = (zx34 - zx33) * b3;
        zx4b3 = (zx44 - zx43) * b3;
        zy3a3 = (zy43 - zy33) * a3;
        zy4a3 = (zy44 - zy34) * a3;
        a = za3b3 - zx3b3 - zy3a3 + zxy33;
        b = zx4b3 - zx3b3 - zxy43 + zxy33;
        c__ = zy4a3 - zy3a3 - zxy34 + zxy33;
        d__ = zxy44 - zxy43 - zxy34 + zxy33;
        e = a + a - b - c__;
        a3sq = a3 * a3;
        b3sq = b3 * b3;
        p02 = ((z3b3 - zy33) * 2.0 + z3b3 - zy34) * b3;
        p03 = (z3b3 * -2.0 + zy34 + zy33) * b3sq;
        p12 = ((zx3b3 - zxy33) * 2.0 + zx3b3 - zxy34) * b3;
        p13 = (zx3b3 * -2.0 + zxy34 + zxy33) * b3sq;
        p20 = ((z3a3 - zx33) * 2.0 + z3a3 - zx43) * a3;
        p21 = ((zy3a3 - zxy33) * 2.0 + zy3a3 - zxy43) * a3;
        p22 = ((a + e) * 3.0 + d__) * a3 * b3;
        p23 = (e * -3.0 - b - d__) * a3 * b3sq;
        p30 = (z3a3 * -2.0 + zx43 + zx33) * a3sq;
        p31 = (zy3a3 * -2.0 + zxy43 + zxy33) * a3sq;
        p32 = (e * -3.0 - c__ - d__) * b3 * a3sq;
        p33 = (d__ + e + e) * a3sq * b3sq;
        /* COMPUTATION OF THE POLYNOMIAL */
      L690:
        dy = vk - y3;
        q0 = p00 + dy * (p01 + dy * (p02 + dy * p03));
        q1 = p10 + dy * (p11 + dy * (p12 + dy * p13));
        q2 = p20 + dy * (p21 + dy * (p22 + dy * p23));
        q3 = p30 + dy * (p31 + dy * (p32 + dy * p33));
        dx = uk - x3;
        w[k] = q0 + dx * (q1 + dx * (q2 + dx * q3));
        /* L700: */
      }
      /* NORMAL EXIT */
      return 0;
      /* ERROR EXIT */
      L710:
        throw new ArgumentException("LX = 1 OR LESS");
      //goto L800;
      L720:
        throw new ArgumentException("LY = 1 OR LESS.");
      //goto L800;
      L730:
        throw new ArgumentException("N = 0 OR LESS.");
      //goto L800;
      L740:
        throw new ArgumentException("IDENTICAL X VALUES");
      //goto L760;
      L750:
        throw new ArgumentException("X VALUES OUT OF SEQUENCE");
      L760:
        throw new ArgumentException(string.Format("ix ={0} x[ix]={1})",ix,x[ix]));
           
      //goto L800;
      L770:
        throw new ArgumentException("IDENTICAL Y VALUES");
      //goto L790;
      L780:
        throw new ArgumentException("Y VALUES OUT OF SEQUENCE");
      L790:
        throw new ArgumentException(string.Format("iy ={0} y[iy]={1})",iy,y[iy]));
      L800:
        throw new ArgumentException(string.Format("LX ={0} LY ={1} N ={2} ERROR DETECTED IN ROUTINE ITPLBV)", ix, iy, n));
      //return 0;
      /* FORMAT STATEMENTS */

      return 0;
    } /* itplbv_ */

    #endregion

  }

}
