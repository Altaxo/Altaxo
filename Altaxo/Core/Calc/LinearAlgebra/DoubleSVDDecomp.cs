#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
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

/*
 * DoubleSVDDecomp.cs
 * Managed code is a port of linpack's svdc.
 * Copyright (c) 2003-2005, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>This class computes the SVD factorization of a general <c>DoubleMatrix</c>.</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public sealed class DoubleSVDDecomp : Algorithm 
  {
    private const int MAXITER = 1000;

    private DoubleMatrix u;
    private DoubleMatrix v;
    private DoubleMatrix w;
    private DoubleMatrix matrix;
    private DoubleVector s;
    
    private bool computeVectors;
    private int rank;
    private int rows;
    private int cols;
    
    ///<summary>Returns the left singular vectors.</summary>
    ///<returns>the left singular vectors. The vectors will be <c>null</c> if,
    ///computerVectors is set to false.</returns>
    public DoubleMatrix U 
    {
      get
      {
        Compute();
        return u;
      }
    }

    ///<summary>Returns the right singular vectors.</summary>
    ///<returns>the right singular vectors. The vectors will be <c>null</c> if,
    ///computerVectors is set to false.</returns>
    public DoubleMatrix V
    {
      get
      {
        Compute();
        return v;
      }
    }

    ///<summary>Returns the singular values as a diagonal matrix.</summary>
    ///<returns>the singular values as a diagonal matrix.</returns>
    public DoubleMatrix W 
    {
      get
      {
        Compute();
        return w;
      }
    }

    ///<summary>Returns the singular values as a <c>DoubleVector</c>.</summary>
    ///<returns>the singular values as a diagonal <c>DoubleVector</c>.</returns>
    public DoubleVector S 
    {
      get
      {
        Compute();
        return s;
      }
    }

    ///<summary>Returns the two norm of the matrix.</summary>
    ///<returns>the two norm of the matrix.</returns>
    public double Norm2
    {
      get
      {
        Compute();
        return s[0];
      }
    }

    ///<summary>Returns the condition number <c>max(S) / min(S)</c>.</summary>
    ///<returns>the condition number.</returns>
    public double Condition 
    {
      get
      {
        Compute();
        int tmp = System.Math.Min(rows,cols)-1;
        return s[0]/s[tmp];
      }
    }

    ///<summary>Returns the effective numerical matrix rank>.</summary>
    ///<returns>the number of non-negligible singular values.</returns>
    public int Rank
    {
      get
      {
        Compute();
        return rank;
      }
    }

    ///<summary>Constructor for SVD decomposition class.</summary>
    ///<param name="matrix">The matrix to decompose.</param>
    ///<param name="computeVectors">Whether to compute the singular vectors or not.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public DoubleSVDDecomp(IROMatrix matrix, bool computeVectors)
    {
      if ( matrix == null ) 
      {
        throw new System.ArgumentNullException("matrix cannot be null.");
      }
      this.matrix = new DoubleMatrix(matrix);
      this.computeVectors = computeVectors;
    }

    ///<summary>Constructor for SVD decomposition class.</summary>
    ///<param name="matrix">The matrix to decompose.</param>
    ///<exception cref="ArgumentNullException">matrix is null.</exception>
    public DoubleSVDDecomp(IROMatrix matrix)
    {
      if ( matrix == null ) 
      {
        throw new System.ArgumentNullException("matrix cannot be null.");
      }
      this.matrix = new DoubleMatrix(matrix);
    }
    
    ///<summary>Computes the algorithm.</summary>
    protected override void InternalCompute()
    {
      rows = matrix.RowLength;
      cols = matrix.ColumnLength;
      int mm=System.Math.Min(rows+1,cols);
      s = new DoubleVector(mm); // singular values

#if MANAGED
      // Derived from LINPACK code.
      // Initialize.

      u=new DoubleMatrix(rows,rows); // left vectors
      v=new DoubleMatrix(cols,cols); // right vectors
      DoubleVector e=new DoubleVector(cols);
      DoubleVector work=new DoubleVector(rows);

      int i, iter, j, k, kase, l, lp1, ls=0, lu, m, nct, nctp1, ncu, nrt, nrtp1;
      double b, c, cs=0.0, el, emm1, f, g, scale, shift, sl,
        sm, sn=0.0, smm1, t1, test, ztest, xnorm, enorm;
      double t,r;

      ncu = rows;

      //   reduce matrix to bidiagonal form, storing the diagonal elements
      //   in s and the super-diagonal elements in e.
      int info = 0;
      nct = System.Math.Min(rows-1,cols);
      nrt = System.Math.Max(0,System.Math.Min(cols-2,rows));
      lu = System.Math.Max(nct,nrt);

      for(l=0; l<lu; l++) 
      { 
        lp1 = l + 1;
        if (l  <  nct) 
        { 
          // compute the transformation for the l-th column and
          // place the l-th diagonal in s[l].
          xnorm=dnrm2Column(matrix, l, l); 
          s[l]=xnorm;
          if (s[l]  !=  0.0) 
          { 
            if (matrix[l,l]  !=  0.0) 
            {
              s[l] = dsign(s[l],matrix[l,l]);
            }
            dscalColumn(matrix, l, l, 1.0/s[l]); 
            matrix[l,l] = 1.0 + matrix[l,l];
          } 

          s[l] = -s[l];
        }  

        for(j=lp1; j<cols; j++) 
        { 
          if (l  <  nct) 
          { 
            if (s[l] !=  0.0) 
            { 
              // apply the transformation.
              t = -ddot(matrix, l, j, l)/matrix[l,l]; 
              for(int ii=l; ii < matrix.RowLength; ii++) 
              {
                matrix[ii,j]+=t*matrix[ii,l];
              }
            } 
          }

          //place the l-th row of matrix into  e for the
          //subsequent calculation of the row transformation.
          e[j] = matrix[l,j];
        } 
       
        if (computeVectors && l < nct) 
        { 
          // place the transformation in u for subsequent back multiplication.
          for(i=l; i < rows; i++) 
          { 
            u[i,l] = matrix[i,l];
          } 
        } 

        if (l < nrt) 
        {    
          // compute the l-th row transformation and place the l-th super-diagonal in e(l).
          enorm=dnrm2Vector(e,lp1);
          e[l]=enorm; 
          if (e[l]  !=  0.0) 
          { 
            if (e[lp1]  !=  0.0) 
            {
              e[l] = dsign(e[l],e[lp1]);
            }
            dscalVector(e, lp1, 1.0/e[l]); 
            e[lp1] = 1.0 + e[lp1];
          }  
          e[l] = -e[l];

          if (lp1  <  rows  &&  e[l]  !=  0.0) 
          { 
            // apply the transformation.
            for(i=lp1; i<rows; i++) 
            { 
              work[i] = 0.0;
            } 

            for(j=lp1; j<cols; j++) 
            {  
              for(int ii=lp1; ii < matrix.RowLength; ii++) 
              {
                work[ii]+=e[j]*matrix[ii,j];
              }
            }  

            for(j=lp1; j<cols; j++) 
            { 
              double ww=-e[j]/e[lp1];
              for(int ii=lp1; ii < matrix.RowLength; ii++) 
              {
                matrix[ii,j]+=ww*work[ii];
              }
            } 
          } 

          if (computeVectors) 
          { 
            // place the transformation in v for subsequent back multiplication.
            for(i=lp1; i < cols; i++) 
            { 
              v[i,l] = e[i];
            } 
          } 
        } 
      }

      //   set up the final bidiagonal matrix or order m.
      m = System.Math.Min(cols,rows+1);
      nctp1 = nct + 1;
      nrtp1 = nrt + 1;
      if (nct  <  cols) 
      {
        s[nctp1-1] = matrix[nctp1-1,nctp1-1];
      }
      if (rows  <  m) 
      {
        s[m-1] = 0.0;
      }
      if (nrtp1  <  m) 
      {
        e[nrtp1-1] = matrix[nrtp1-1,m-1];
      }
      e[m-1] = 0.0;

      //   if required, generate u.
      if (computeVectors) 
      {
        for(j=nctp1-1; j<ncu; j++) 
        { 
          for(i=0; i < rows; i++) 
          {
            u[i,j] = 0.0;
          } 
          u[j,j] = 1.0;
        } 
 
        for(l=nct-1; l>=0; l--) 
        { 
          if (s[l]  !=  0.0) 
          {  
            for(j=l+1; j < ncu; j++) 
            {  
              t = -ddot(u, l, j, l)/u[l,l]; 
              for(int ii=l; ii < u.RowLength; ii++) 
              {
                u[ii,j]+=t*u[ii,l];
              }
            }
            dscalColumn(u, l, l, -1.0);
            u[l,l] = 1.0 + u[l,l];
            for(i=0; i<l; i++) 
            {
              u[i,l] = 0.0;
            }
          } 
          else 
          {   
            for(i=0; i<rows; i++) 
            {
              u[i,l] = 0.0;
            }
            u[l,l] = 1.0;
          } 
        }
      }  

      //   if it is required, generate v.
      if (computeVectors) 
      { 
        for(l=cols-1; l>=0; l--) 
        {
          lp1 = l + 1;
          if (l  <  nrt) 
          { 
            if (e[l]  !=  0.0) 
            {
              for(j=lp1; j < cols; j++) 
              {
                t = -ddot(v, l, j, lp1)/v[lp1,l];
                for(int ii=l; ii < v.RowLength; ii++) 
                {
                  v[ii,j]+=t*v[ii,l];
                }
              } 
            }
          } 
          for(i=0; i < cols; i++) 
          { 
            v[i,l] = 0.0;
          }
          v[l,l] = 1.0;
        }
      }

      //   transform s and e so that they are  double .
      for(i=0; i < m; i++) 
      {
        if (s[i]  !=  0.0) 
        { 
          t = s[i];
          r = s[i]/t;
          s[i] = t;
          if (i  <  m-1) 
          {
            e[i] = e[i]/r;
          }
          if (computeVectors) 
          {
            dscalColumn(u, i, 0, r);
          }
        }
        //   ...exit
        if (i  ==  m-1) 
        {
          break;  
        }
        if (e[i]  !=  0.0) 
        { 
          t = e[i];
          r = t/e[i];
          e[i] = t;
          s[i+1] = s[i+1]*r;
          if (computeVectors) 
          {
            dscalColumn(v, i+1, 0, r); 
          }
        } 
      }

      //   main iteration loop for the singular values.
      mm = m;
      iter = 0;

      while(m > 0) 
      { // quit if all the singular values have been found.
        // if too many iterations have been performed, set
        //      flag and return.
        if (iter  >=  MAXITER) 
        {
          info = m;
          //   ......exit
          break;
        }

        //      this section of the program inspects for
        //      negligible elements in the s and e arrays.  on
        //      completion the variables kase and l are set as follows.

        //         kase = 1     if s[m] and e[l-1] are negligible and l < m
        //         kase = 2     if s[l] is negligible and l < m
        //         kase = 3     if e[l-1] is negligible, l < m, and
        //                      s[l, ..., s[m] are not negligible (qr step).
        //         kase = 4     if e[m-1] is negligible (convergence).

        for(l=m-2; l>=0; l--) 
        { 
          test = ComplexMath.Absolute(s[l]) + ComplexMath.Absolute(s[l+1]);
          ztest = test + ComplexMath.Absolute(e[l]);
          if (ztest  ==  test) 
          { 
            e[l] = 0.0;
            break;
          } 
        }
          
        if (l  ==  m - 2) 
        {
          kase = 4;
        } 
        else
        {
          for(ls=m-1; ls > l; ls--) 
          { 
            test = 0.0;
            if (ls  !=  m-1) 
            {
              test = test + ComplexMath.Absolute(e[ls]);
            }
            if (ls  !=  l + 1) 
            {
              test = test + ComplexMath.Absolute(e[ls-1]);
            }
            ztest = test + ComplexMath.Absolute(s[ls]);
            if (ztest ==  test) 
            { 
              s[ls] = 0.0;
              break; 
            } 
          } 

          if (ls  ==  l) 
          {
            kase = 3;
          } 
          else if (ls  ==  m-1) 
          { 
            kase = 1;
          } 
          else
          {
            kase = 2;
            l = ls;
          }
        }
  
        l = l + 1;

        //      perform the task indicated by kase.
        switch(kase) 
        {
            // deflate negligible s[m].
          case 1:
            f = e[m-2];
            e[m-2] = 0.0;
            for(k=m-2; k>=0; k--) 
            { 
              t1 = s[k];
              drotg(ref t1,ref f,ref cs,ref sn);
              s[k] = t1;
              if (k  !=  l) 
              { 
                f = -sn*e[k-1];
                e[k-1] = cs*e[k-1];
              } 
              if (computeVectors) 
              {
                drot (v, k, m-1, cs, sn); 
              }
            } 
            break; 

            // split at negligible s[l].
          case 2:
            f = e[l-1];
            e[l-1] = 0.0;
            for(k=l; k <m; k++) 
            {
              t1 = s[k];
              drotg(ref t1, ref f, ref cs, ref sn);
              s[k] = t1;
              f = -sn*e[k];
              e[k] = cs*e[k];
              if (computeVectors) 
              {
                drot (u, k, l-1, cs, sn); 
              }
            } 
            break;
         
            // perform one qr step.
          case 3:
            // calculate the shift.
            scale=0.0;
            scale=System.Math.Max(scale,ComplexMath.Absolute(s[m-1]));
            scale=System.Math.Max(scale,ComplexMath.Absolute(s[m-2]));
            scale=System.Math.Max(scale,ComplexMath.Absolute(e[m-2]));
            scale=System.Math.Max(scale,ComplexMath.Absolute(s[l]));
            scale=System.Math.Max(scale,ComplexMath.Absolute(e[l]));
            sm = s[m-1]/scale; 
            smm1 = s[m-2]/scale;
            emm1 = e[m-2]/scale;
            sl = s[l]/scale;
            el = e[l]/scale;
            b = ((smm1 + sm)*(smm1 - sm) + emm1*emm1)/2.0;
            c = (sm*emm1)*(sm*emm1); 
            shift = 0.0;
            if (b  !=  0.0  ||  c  !=  0.0) 
            { 
              shift = System.Math.Sqrt(b*b+c);
              if (b  <  0.0)
              {
                shift = -shift;
              }
              shift = c/(b + shift);
            } 
            f = (sl + sm)*(sl - sm) + shift;
            g = sl*el;

            // chase zeros.
            for(k=l; k < m-1; k++) 
            {
              drotg(ref f, ref g, ref cs, ref sn);
              if (k  !=  l) e[k-1] = f;
              f = cs*s[k] + sn*e[k];
              e[k] = cs*e[k] - sn*s[k];
              g = sn*s[k+1];
              s[k+1] = cs*s[k+1];
              if (computeVectors) 
              {
                drot (v, k, k+1, cs, sn); 
              }
              drotg(ref f, ref g, ref cs, ref sn);
              s[k] = f;
              f = cs*e[k] + sn*s[k+1];
              s[k+1] = -sn*e[k] + cs*s[k+1];
              g = sn*e[k+1];
              e[k+1] = cs*e[k+1];
              if (computeVectors  &&  k < rows)
              {
                drot(u, k, k+1, cs, sn); 
              }
            } 
            e[m-2] =f;
            iter = iter + 1;
            break; 

            // convergence.
          case 4: 
            // make the singular value  positive
            if (s[l]  < 0.0) 
            {
              s[l] = -s[l];
              if (computeVectors) 
              {
                dscalColumn(v, l, 0, -1.0); 
              }
            }

            // order the singular value.
            while (l != mm-1) 
            {
              if (s[l]  >= s[l+1]) 
              {
                break;
              }
              t = s[l];
              s[l] = s[l+1];
              s[l+1] = t;
              if (computeVectors && l < cols) 
              { 
                dswap(v,l,l+1); 
              }
              if (computeVectors && l < rows) 
              {
                dswap(u,l,l+1); 
              }
              l = l + 1;
            } 
            iter = 0;
            m = m - 1;
            break;
        } 
      }

      // make matrix w from vector s
      // there is no constructor, creating diagonal matrix from vector
      // doing it ourselves
      mm=System.Math.Min(matrix.RowLength,matrix.ColumnLength);
#else
      u = new DoubleMatrix(rows);
      v = new DoubleMatrix(cols);
      double[] a = new double[matrix.data.Length];
      Array.Copy(matrix.data, a, matrix.data.Length);
      Lapack.Gesvd.Compute(rows, cols, a, s.data, u.data, v.data );
      v.Transpose();
#endif    
      w=new DoubleMatrix(matrix.RowLength,matrix.ColumnLength);
      for(int ii=0; ii<matrix.RowLength; ii++) 
      {
        for(int jj=0; jj<matrix.ColumnLength; jj++) 
        {
          if(ii==jj) 
          {
            w[ii,ii]=s[ii];
          } 
        }
      }
      
      double eps = System.Math.Pow(2.0,-52.0);
      double tol = System.Math.Max(matrix.RowLength,matrix.ColumnLength)*s[0]*eps;
      rank = 0;
      
      for (int h = 0; h < mm; h++) 
      {
        if (s[h] > tol) 
        {
          rank++;
        }
      }

      if( !computeVectors )
      {
        u = null;
        v = null;
      }
      matrix = null;
    }

#if MANAGED
    private static double dsign(double z1, double z2) 
    {
      double ret=System.Math.Abs(z1)*(z2/System.Math.Abs(z2));
      return ret;
    }

    private static void dswap(DoubleMatrix A, int Col1, int Col2) 
    {
      // swap columns Col1,Col2
      double z;
      for(int i=0; i < A.RowLength; i++) 
      {
        z=A[i,Col1];
        A[i,Col1]=A[i,Col2];
        A[i,Col2]=z;
      } 
    }

    private static void dscalColumn(DoubleMatrix A, int Col, int Start, double z) 
    {
      // A part of column Col of matrix A from row Start to end multiply by z
      for(int i=Start; i<A.RowLength; i++) 
      {
        A[i,Col]=A[i,Col]*z;
      }
    }

    private static void dscalVector(DoubleVector A, int Start, double z) 
    {
      // A part of vector  A from Start to end multiply by z
      for(int i=Start; i<A.Length; i++) 
      {
        A[i]=A[i]*z;
      }
    }

    private static void drotg(ref double da, ref double db, ref double c, ref double s) 
    {
      //     construct givens plane rotation.
      //     jack dongarra, linpack, 3/11/78.
      double roe,scale,r,z,Absda,Absdb,sda,sdb;
      
      roe = db;
      Absda=System.Math.Abs(da);
      Absdb=System.Math.Abs(db);
      if( Absda > Absdb ) roe = da;
      scale = Absda + Absdb;
      if( scale == 0.0 ) 
      { 
        c = 1.0;
        s = 0.0;
        r = 0.0;
        z = 0.0;
      } 
      else 
      { 
        sda=da/scale;
        sdb=db/scale;     
        r = scale*System.Math.Sqrt(sda*sda + sdb*sdb);
        if(roe < 0.0) r=-r;  
        c = da/r;
        s = db/r;
        z = 1.0;
        if( Absda > Absdb ) z = s;
        if( Absdb >= Absda && c != 0.0 ) z = 1.0/c;
      } 
      da = r;
      db = z;
      return;
    } 

    private static double dnrm2Column(DoubleMatrix A, int Col, int Start) 
    {
      //  dznrm2Column returns the euclidean norm of a vector,
      // which is a part of column Col in matrix A, beginning from Start to end of column
      // so that dznrm2Column := sqrt( conjg( matrix' )*matrix )
      double s=0;
      for(int i=Start; i < A.RowLength; i++) 
      {
        s+=(A[i,Col]*A[i,Col]);
      }
      return System.Math.Sqrt(s);
    }

    private static double dnrm2Vector(DoubleVector A, int Start)
    {
      //  dznrm2Vector returns the euclidean norm of a vector,
      // which is a part of A, beginning from Start to end of vector
      // so that dznrm2Vector := sqrt( conjg( matrix' )*matrix )
      double s=0;
      for(int i=Start; i < A.Length; i++)
      {
        s+=(A[i]*A[i]);
      }
      return System.Math.Sqrt(s);
    }

    private static double ddot(DoubleMatrix A, int Col1, int Col2, int Start)
    {
      double z=0.0;
      for(int i=Start; i < A.RowLength; i++)
      { 
        z+=A[i,Col2]*A[i,Col1];
      }
      return z;
    }

    private static void drot (DoubleMatrix A, int Col1, int Col2, double c, double s)
    {
      // applies a plane rotation, where the c=cos and s=sin
      // Col1, Col2 - rotated columns of A

      double z;
      for(int i=0; i < A.RowLength; i++)
      {
        z=c*A[i,Col1]+s*A[i,Col2];
        A[i,Col2]=c*A[i,Col2]-s*A[i,Col1];
        A[i,Col1]=z;
      }
    }
#endif
  }
}