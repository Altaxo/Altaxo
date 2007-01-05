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

namespace Altaxo.Calc.Fourier
{

  /// <summary>
  ///  Generalized prime factor complex fast Fourier transform and
  ///  backtransform in one, two, and three dimensions (d = 1,2,3).
  ///  Each dimension must be of the form n = (2**p) * (3**q) * (5**r).
  ///  The complex d-dimensional data can be either given in a vector of
  ///  double precision complex numbers or in two seperate vectors of doubles
  ///  for the real and imaginary parts respectively, or in single precision,
  ///  either in a vector of complex or in two seperate vectors of float.
  ///  A leading dimension different from the first data dimension can be 
  ///  specified - this can prevent memory-bank conflicts and therefore  
  ///  dramatically improves performance on vector machines with interleaved memory.
  ///  The Fourier transform is always perfored inplace. The data array can be
  ///  stored either in column (Fortran convention) or row (C convention) order.
  ///  This makes about 40 different combinations (float - double, dimension,
  ///  order, complex - real) which can be easily accessed 
  ///  by a simple class definition:  
  /// </summary>
  public class Pfa235FFT 
  {
    //-----------------------------------------------------------------------------//
    // Important optimization options
    //-----------------------------------------------------------------------------//

    /// <summary>
    ///   lvr = length of vector registers, set to 128 for a Cray C90.
    ///   Reset to 64 for other Cray machines, or to any large value
    ///   (greater than or equal to lot) for a scalar computer.
    /// </summary>
    const int lvr = 1024;

    /// <summary>
    /// The last three loops in function gpfft3d() in gpfft3d.cc are prime candidates 
    /// for parallelism as they call independent multi-1D ffts.  Just set the  
    /// constant "nthreads" to the number of processors to use. 
    /// </summary>
    const int nthreads = 1;



    //    public enum Direction { Forward = -1, Inverse = 1 };
    // constructors and assigment


    enum ROW_ORDER { def_row_order = 1 };  // set to true for the C convention
    
    protected int id, ndim, trisize;
    protected bool row_order=true;
    protected int[] dim = new int[3];
    protected int[] trindex = new int[3];

    protected double[] trigs;

    /// <summary>
    /// uninitialized setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one, two or three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// otherwise an error will be generated and the error handler function
    /// Matpack.Error() is called. On instantiation some trigonometric tables
    /// will be allocated and calculated. This approach avoids multiple
    /// twiddle factor recalculations if several FFTs are calculated for data 
    /// with the same dimensions. Sometimes it is convenient to define an
    /// "empty" setup (first constructor) and assign a setup later (see
    /// copying and assignment). As default the multi-dimensional data
    /// are expected in row order (C convention). If you want to transform
    /// data stored in column order (Fortran convention) use the member
    /// function SetOrder() to change the order - see below. For optimizations
    /// on vector machines with separate memory banks an extra leading dimension
    /// can be defined to avoid bank conflicts - also see SetOrder(). 
    /// </remarks>
    
    public Pfa235FFT()
    {
    }

    /// <summary>
    /// 1-dimensional setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one, two or three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// otherwise an error will be generated and the error handler function
    /// Matpack.Error() is called. On instantiation some trigonometric tables
    /// will be allocated and calculated. This approach avoids multiple
    /// twiddle factor recalculations if several FFTs are calculated for data 
    /// with the same dimensions. Sometimes it is convenient to define an
    /// "empty" setup (first constructor) and assign a setup later (see
    /// copying and assignment). As default the multi-dimensional data
    /// are expected in row order (C convention). If you want to transform
    /// data stored in column order (Fortran convention) use the member
    /// function SetOrder() to change the order - see below. For optimizations
    /// on vector machines with separate memory banks an extra leading dimension
    /// can be defined to avoid bank conflicts - also see SetOrder(). 
    /// </remarks>
    public Pfa235FFT (int n1)
    {
      ndim = 1;
      row_order = true;

      int[] pqr = new int[3];
      if ( Factorize(n1,pqr) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension",n1));
      dim[0] = id = n1;
      trisize = 2*(powii(2,pqr[0])+powii(3,pqr[1])+powii(5,pqr[2]));
      trigs = new double[trisize];
      gpfasetup(trigs,0,n1);
    }
      
    /// <summary>
    /// 2-dimensional setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one, two or three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// otherwise an error will be generated and the error handler function
    /// Matpack.Error() is called. On instantiation some trigonometric tables
    /// will be allocated and calculated. This approach avoids multiple
    /// twiddle factor recalculations if several FFTs are calculated for data 
    /// with the same dimensions. Sometimes it is convenient to define an
    /// "empty" setup (first constructor) and assign a setup later (see
    /// copying and assignment). As default the multi-dimensional data
    /// are expected in row order (C convention). If you want to transform
    /// data stored in column order (Fortran convention) use the member
    /// function SetOrder() to change the order - see below. For optimizations
    /// on vector machines with separate memory banks an extra leading dimension
    /// can be defined to avoid bank conflicts - also see SetOrder(). 
    /// </remarks>  
    public Pfa235FFT (int n1, int n2)
    {
      ndim = 2;
      row_order = true;


      int[][] pqr = new int[2][];
      pqr[0] = new int[3];
      pqr[1] = new int[3];
      int[] d = new int[2];

      if ( Factorize(n1, pqr[0]) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension 1", n1));
      if ( Factorize(n2,pqr[1]) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension 2", n2));

      dim[0] = n1; 
      dim[1] = n2;
      id = row_order ? n2 : n1;
      for (int i = 0; i <= 1; i++)
        d[i] = 2*(powii(2,pqr[i][0])+powii(3,pqr[i][1])+powii(5,pqr[i][2]));
      trisize = d[0]+d[1];
      trigs = new double[trisize];
      trindex[0] = 0;
      trindex[1] = d[0]; 
      for (int i = 0; i <= 1; i++)
        gpfasetup(trigs,trindex[i],dim[i]);
    }


    /// <summary>
    /// 3-dimensional setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one, two or three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// otherwise an error will be generated and the error handler function
    /// Matpack.Error() is called. On instantiation some trigonometric tables
    /// will be allocated and calculated. This approach avoids multiple
    /// twiddle factor recalculations if several FFTs are calculated for data 
    /// with the same dimensions. Sometimes it is convenient to define an
    /// "empty" setup (first constructor) and assign a setup later (see
    /// copying and assignment). As default the multi-dimensional data
    /// are expected in row order (C convention). If you want to transform
    /// data stored in column order (Fortran convention) use the member
    /// function SetOrder() to change the order - see below. For optimizations
    /// on vector machines with separate memory banks an extra leading dimension
    /// can be defined to avoid bank conflicts - also see SetOrder(). 
    /// </remarks>
    public Pfa235FFT (int n1, int n2, int n3)
    {
      ndim = 3;
      row_order = true;

      int[][] pqr = new int[3][];
      pqr[0] = new int[3];
      pqr[1] = new int[3];
      pqr[2] = new int[3];

      int[] d = new int[3];
      if ( Factorize(n1,pqr[0]) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension 1", n1));
      if ( Factorize(n2,pqr[1]) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension 2", n2));
      if ( Factorize(n3,pqr[2]) == false )
        throw new ArithmeticException(string.Format("Pfa235FFT: {0} is not a legal value for dimension 3", n3));

      dim[0] = n1;
      dim[1] = n2; 
      dim[2] = n3;
      id = row_order ? n3 : n1;
      for (int i = 0; i <= 2; i++)
        d[i] = 2*(powii(2,pqr[i][0])+powii(3,pqr[i][1])+powii(5,pqr[i][2]));
      trisize = d[0]+d[1]+d[2];
      trigs = new double[trisize];
      trindex[0] = 0;  
      trindex[1] = d[0]; 
      trindex[2] = d[0]+d[1];
      for (int i = 0; i <= 2; i++)
        gpfasetup(trigs,trindex[i],dim[i]);
    }



    /// <summary>
    ///  Copy-Constructor
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one, two or three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// otherwise an error will be generated and the error handler function
    /// Matpack.Error() is called. On instantiation some trigonometric tables
    /// will be allocated and calculated. This approach avoids multiple
    /// twiddle factor recalculations if several FFTs are calculated for data 
    /// with the same dimensions. Sometimes it is convenient to define an
    /// "empty" setup (first constructor) and assign a setup later (see
    /// copying and assignment). As default the multi-dimensional data
    /// are expected in row order (C convention). If you want to transform
    /// data stored in column order (Fortran convention) use the member
    /// function SetOrder() to change the order - see below. For optimizations
    /// on vector machines with separate memory banks an extra leading dimension
    /// can be defined to avoid bank conflicts - also see SetOrder(). 
    /// </remarks>  
    public Pfa235FFT (Pfa235FFT fft)
    {
      // copy all elements
      id = fft.id;
      ndim = fft.ndim;
      trisize = fft.trisize;
      row_order = fft.row_order;
      for (int i = 0; i < 3; i++) 
      {
        dim[i] = fft.dim[i];
        trindex[i] = fft.trindex[i];
      }

      // allocate and copy trigs
      trigs = new double[trisize];
      Array.Copy(fft.trigs,0,this.trigs,0,fft.trigs.Length);
        
    }
      
    // FFT functions
                    
                    
    /// <summary>
    /// Factorize the number into powers of 2, 3, and 5
    /// </summary>
    /// <param name="n">The dimension n to be factorized into the valid factors n = (2**p) * (3**q) * (5**r)</param>
    /// <param name="pqr">Return the powers of the basic prime factors 2, 3 and 5 for the given argument n. Must be at least int[3].</param>
    /// <returns>True if factorization is successful, False if n can not be factorized into powers of 2, 3, and 5.</returns>
    public static bool  Factorize (int n, int[] pqr)
    {
      int k, ifac = 2;
      for (int l = 1; l <= 3; l++) 
      {
        k = 0;
      L10:
        if (n % ifac != 0) goto L20;
        ++k;
        n /= ifac;
        goto L10;
      L20:
        pqr[l-1] = k;
        ifac += l;
      }
      return (n == 1); // return false if decomposition failed
    }


    /// <summary>
    /// Test if the number n can be factorized into powers of 2, 3 and 5.
    /// </summary>
    /// <param name="n">The number to test.</param>
    /// <returns>True if n can be factorized into powers of 2, 3 and 5, or false if not.</returns>
    public static bool  CanFactorized (int n)
    {
      int[] pqr = new int[3];
      return Factorize(n,pqr);
    }

      
    /// <summary>
    /// Set information about the row order and the leading dimension.
    /// If the row order argument is non-zero then the d-dimensional data
    /// are assumed to be stored in row order (the C convention), otherwise if
    /// zero then column order (the Fortran convention) is assumed.
    /// The leading dimension can be choosen different from the first/last 
    /// dimension of the array. This can give a significant
    /// speed increase on some vector machines avoiding memory-bank conflicts.
    /// If the data are stored column-ordered (Fortran style) then the leading
    /// dimension is the first dimension, otherwise if the data are stored row-
    /// ordered (C style) then the last dimension is the leading dimension 
    /// and will be padded!
    /// </summary>
    /// <param name="row">If the row order argument is non-zero then the 
    ///       d-dimensional data are assumed to be stored in 
    ///       row order (the C convention), otherwise if
    ///         zero then column order (the Fortran convention) 
    ///       is assumed. Initially row order is assumed!</param>
    /// <param name="lead">The leading dimension can be choosen different
    ///       from the first/last dimension of the array. This
    ///       can give a significant speed increase on some 
    ///       vector machines avoiding memory-bank conflicts.
    ///         If the data are stored column-ordered (Fortran 
    ///       style) then the leading dimension is the first 
    ///       dimension, otherwise if the data are stored row-
    ///         ordered (C style) then the last dimension is the 
    ///       leading dimension and will be padded!</param> 
    void SetOrder (int row, int lead)
    {
      if (ndim == 0) 
        throw new ArithmeticException("Pfa235FFT::SetOrder: no dimensions are specified");

      // set row/column ordering
      row_order = 0!=row;

      // set corresponding leading dimension
      if (row_order) /* C convention */ 
      {
    
        if (lead <= 0)
          id = dim[ndim-1];
        else if (lead < dim[ndim-1])
          throw new ArithmeticException(string.Format("Pfa235FFT::SetOrder: ({0}) is smaller than last dimension ({1})",
            lead, dim[ndim-1]));
        else
          id = lead;

      } 
      else /* column order, Fortran convention */ 
      {
    
        if (lead <= 0) 
          id = dim[0];
        else if (lead < dim[0])
          throw new ArithmeticException(string.Format("Pfa235FFT::SetOrder: ({0}) is smaller than first dimension ({1})",
            lead, dim[0]));
        else
          id = lead;
      }
    }



    /// <summary>
    /// Get information about the row order and the leading dimension.
    /// </summary>
    /// <param name="row">If the row order argument is non-zero then the 
    ///       d-dimensional data are assumed to be stored in 
    ///       row order (the C convention), otherwise if
    ///         zero then column order (the Fortran convention) 
    ///       is assumed. Initially row order is assumed!</param>
    /// <param name="lead">The leading dimension can be choosen different
    ///       from the first/last dimension of the array. This
    ///       can give a significant speed increase on some 
    ///       vector machines avoiding memory-bank conflicts.
    ///         If the data are stored column-ordered (Fortran 
    ///       style) then the leading dimension is the first 
    ///       dimension, otherwise if the data are stored row-
    ///         ordered (C style) then the last dimension is the 
    ///       leading dimension and will be padded!</param>
    void GetOrder (out int row, out int lead)
    {
      row  = row_order ? 1 : 0;
      lead = id;
    }


    //-----------------------------------------------------------------------------//
    // Complex forward/backward FFT for 1/2/3 dimensions
    // Interface with complex<FLOAT> data vector 
    //-----------------------------------------------------------------------------//

    /*
int FFT (complex<FLOAT> c[], int isign)
{
  // access as FLOAT - it is important that complex class has base type FLOAT
  FLOAT *d = (FLOAT*)c;

  if (ndim == 0) {

    Matpack.Error("Pfa235FFT: no dimensions have been specified");

  } else if (ndim == 1) {

    // leading dimension is ignored, row_order doesn't matter
    gpfa(d, d+1, trigs, 2, 0, dim[0], 1, -isign);

  } else if (ndim == 2) {

    int one,two;
    if (row_order) {  // C style
      one = 0; two = 1; 
    } else {    // column order (Fortran style)
      one = 1; two = 0; 
    }

    int lot = (dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * id * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[two], 
     2, 2*id, dim[two], min(lot,dim[one]-i*lot), -isign);
    }
    gpfa(d, d+1, trigs+trindex[one], 
   2*id, 2, dim[one], dim[two], -isign);

  } else if (ndim == 3) {

    int one,two,three;
    if (row_order) {  // C style
      one = 0; two = 1; three = 2;
    } else {    // column order (Fortran style)
      one = 2; two = 1; three = 0; 
    }

    int lot = (dim[two] * dim[one] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * id * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[three], 
     2, 2*id, dim[three], min(lot,dim[two]*dim[one]-i*lot), -isign);
    }
    for (int i = 0; i < dim[one]; ++i) {
      int offset = 2 * id * dim[two] * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[two], 
     2*id, 2, dim[two], dim[three], -isign);
    }
    lot = (id * dim[two] + nthreads - 1) / nthreads;
    for (int i = 0; i < nthreads; ++i) {
      int offset = 2 * lot * i;
      gpfa(d+offset, d+offset+1, trigs+trindex[one],
     2*id*dim[two], 2, dim[one], min(lot,id*dim[two]-i*lot), -isign);
    }
  }
  return 1;
}
*/


    /// <summary>
    /// Complex forward/backward FFT for 1/2/3 dimensions
    /// Interface with separate FLOAT vectors for real and imaginary part
    /// </summary>
    /// <param name="re">Input/Output vector of real part.</param>
    /// <param name="im">Input/Output vector of imaginary part.</param>
    /// <param name="isign">Forward (-1) or reverse (1) transform. </param>
    /// <returns>Currently undefined, not used.</returns>
    //-----------------------------------------------------------------------------//

    public int FFT (double[] re, double[] im, FourierDirection isign)
    {
      if (ndim == 0) 
      {

        throw new ArithmeticException("Pfa235FFT: no dimensions have been specified");

      } 
      else if (ndim == 1) 
      {

        // leading dimension is ignored, row_order doesn't matter
        gpfa(re, 0, im, 0, trigs,0, 1, 0, dim[0], 1, (int)isign);

      } 
      else if (ndim == 2) 
      {

        int one,two;
        if (row_order) 
        { // C style
          one = 0; two = 1; 
        } 
        else 
        {     // column order (Fortran style)
          one = 1; two = 0; 
        }

        int lot = (dim[one] + nthreads - 1) / nthreads;
        for (int i = 0; i < nthreads; ++i) 
        {
          int offset = id * lot * i;
          gpfa(re,offset, im,offset, trigs,trindex[two], 
            1, id, dim[two], Math.Min(lot,dim[one]-i*lot), (int)isign);
        }
        gpfa(re, 0, im, 0, trigs,trindex[one], 
          id, 1, dim[one], dim[two], (int)isign);

      } 
      else if (ndim == 3) 
      {

        int one,two,three;
        if (row_order) 
        { // C style
          one = 0; two = 1; three = 2;
        } 
        else 
        {     // column order (Fortran style)
          one = 2; two = 1; three = 0; 
        }

        int lot = (dim[two] * dim[one] + nthreads - 1) / nthreads;
        for (int i = 0; i < nthreads; ++i) 
        {
          int offset = id * lot * i;
          gpfa(re,offset, im,offset, trigs,trindex[three], 
            1, id, dim[three], Math.Min(lot,dim[two]*dim[one]-i*lot), (int)isign);
        }
        for (int i = 0; i < dim[one]; ++i) 
        {
          int offset = id * dim[two] * i;
          gpfa(re,offset, im,offset, trigs,trindex[two], 
            id, 1, dim[two], dim[three], (int)isign);
        }
        lot = (id * dim[two] + nthreads - 1) / nthreads;
        for (int i = 0; i < nthreads; ++i) 
        {
          int offset = lot * i;
          gpfa(re,offset, im,offset, trigs,trindex[one],
            id*dim[two], 1, dim[one], Math.Min(lot,id*dim[two]-i*lot), (int)isign);
        }
      }
      return 1;
    }


    /// <summary>
    /// Performs two FFTs of the two real values arrays and store the result
    /// in the arrays.
    /// </summary>
    /// <param name="real1">Input/Output vector of first real array.</param>
    /// <param name="real2">Input/Output vector of second real array.</param>
    /// <param name="isign">Forward (-1) or reverse (1) transform. </param>
    /// <returns>Currently undefined, not used.</returns>
    //-----------------------------------------------------------------------------//

    public int RealFFT (double[] real1, double[] real2, FourierDirection isign)
    {
      if (ndim == 0) 
        throw new ArithmeticException("Pfa235FFT: no dimensions have been specified");
      if (ndim == 0) 
        throw new ArithmeticException("Pfa235FFT: Sorry, RealFFT is implemented only for one dimension!");

      double re1, im1, re2, im2;
      int n = dim[0];
      if(isign == FourierDirection.Forward)
      {
 
        this.FFT(real1,real2,isign);
        int i,j;
        for(i=1,j=n-1;i<j;i++,j--)
        {
          re1 = real1[i] + real1[j];
          im1 = real2[i] - real2[j];

          re2 = real2[i] + real2[j];
          im2 = real1[j] - real1[i];
        
          real1[i] = 0.5*re1;
          real1[j] = 0.5*im1;
          real2[i] = 0.5*re2;
          real2[j] = 0.5*im2;
        }
      }
      else // Backward transform
      {
        int i,j;
        for(i=1,j=n-1;i<j;i++,j--)
        {
          re1 = real1[i];
          im1 = real1[j];

          re2 = real2[i];
          im2 = real2[j];
        
          real1[i] = re1-im2;
          real1[j] = re1+im2;
          real2[i] = im1+re2;
          real2[j] = re2-im1;
        }

        this.FFT(real1,real2,isign);
      }

      return 0;
    }

    /// <summary>
    ///   Raise the integer x to an integer power n with the minimal number of 
    ///   multiplications. The exponent n must be non-negative.
    /// </summary>
    /// <param name="x">The integer to rise.</param>
    /// <param name="n">The power, must be non-negative.</param>
    /// <returns>x^n</returns>
    static int powii (int x, int n)
    {
      if (n < 0)
        throw new ArithmeticException("powii: exponent must be non-negative");
      int g = 1;

      iterate:

        if (0!=(n & 1)) 
          g *= x; // n is odd
      if ( (n /= 2)!=0 ) 
      { 
        // n/2 is non zero
        x *= x;
        goto iterate;
      }

      return g;
    }
  
    #region GPFA Algorithms


    static void gpfa2f    (double[] a, int aOffs, double[] b, int bOffs, double[] trigs, int trOffs, int inc, int jump, int n, int mm, int lot, int isign)
    {
      // *************************************************************** 
      // *                                                             * 
      // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
      // *  reset to 64 for other cray machines, or to any large value * 
      // *  (greater than or equal to lot) for a scalar computer.      * 
      // *                                                             * 
      // *************************************************************** 

      // System generated locals 
      int i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

      int    ninc, left, nvex, j, k, l, m = 0, ipass, nblox, jstep, m2, n2, m8,
        ja, jb, la, jc, jd, nb, je, jf, jg, jh, mh, kk, ji, ll, jj, jk, 
        jl, jm, jn, jo, jp, mu, nu, laincl, jstepl, istart, jstepx, jjj,
        ink, inq;
      double  c1, s, c2, c3, t0, t2, t1, t3, u0, u2, u1, u3, ss, co1, co2, co3,
        co4, co5, co6, co7, si1, si2, si3, si4, si5, si6, si7, aja, ajb, 
        ajc, ajd, bja, bjc, bjb, bjd, aje, ajg, ajf, ajh, bje, bjg, bjf, 
        bjh, aji, bjm, ajj, bjj, ajk, ajl, bji, bjk, ajo, bjl, bjo, ajm, 
        ajn, ajp, bjn, bjp;

      // Parameter adjustments 
      --trOffs;
      --bOffs;
      --aOffs;

      n2 = powii(2,mm);
      inq = n / n2;
      jstepx = (n2 - n) * inc;
      ninc = n * inc;
      ink = inc * inq;

      m2 = 0;
      m8 = 0;
      if (mm % 2 == 0) 
      {
        m = mm / 2;
      } 
      else if (mm % 4 == 1) 
      {
        m = (mm - 1) / 2;
        m2 = 1;
      } 
      else if (mm % 4 == 3) 
      {
        m = (mm - 3) / 2;
        m8 = 1;
      }
      mh = (m + 1) / 2;

      nblox = (lot - 1) / lvr + 1;
      left = lot;
      s = (double) isign;
      istart = 1;

      //  loop on blocks of lvr transforms 
      //  -------------------------------- 

      for (nb = 1; nb <= nblox; ++nb) 
      {

        if (left <= lvr) 
        {
          nvex = left;
        } 
        else if (left < lvr << 1) 
        {
          nvex = left / 2;
          nvex += nvex % 2;
        } 
        else 
        {
          nvex = lvr;
        }
        left -= nvex;

        la = 1;

        //  loop on type I radix-4 passes 
        //  ----------------------------- 
        mu = inq % 4;
        if (isign == -1) mu = 4 - mu;
        ss = 1.0;
        if (mu == 3) ss = -1.0;
        if (mh == 0) goto L200;

        i__2 = mh;
        for (ipass = 1; ipass <= i__2; ++ipass) 
        {
          jstep = n * inc / (la << 2);
          jstepl = jstep - ninc;

          //  k = 0 loop (no twiddle factors) 
          //  ------------------------------- 
          i__3 = (n - 1) * inc;
          i__4 = jstep << 2;
          for (jjj = 0; i__4 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__4) 
          {
            ja = istart + jjj;

            //     "transverse" loop 
            //     ----------------- 
            i__5 = inq;
            for (nu = 1; nu <= i__5; ++nu) 
            {
              jb = ja + jstepl;
              if (jb < istart) 
              {
                jb += ninc;
              }
              jc = jb + jstepl;
              if (jc < istart) 
              {
                jc += ninc;
              }
              jd = jc + jstepl;
              if (jd < istart) 
              {
                jd += ninc;
              }
              j = 0;

              //  loop across transforms 
              //  ---------------------- 

              i__6 = nvex;
              for (l = 1; l <= i__6; ++l) 
              {
                aja = a[aOffs+ja + j];
                ajc = a[aOffs+jc + j];
                t0 = aja + ajc;
                t2 = aja - ajc;
                ajb = a[aOffs+jb + j];
                ajd = a[aOffs+jd + j];
                t1 = ajb + ajd;
                t3 = ss * (ajb - ajd);
                bja = b[bOffs+ja + j];
                bjc = b[bOffs+jc + j];
                u0 = bja + bjc;
                u2 = bja - bjc;
                bjb = b[bOffs+jb + j];
                bjd = b[bOffs+jd + j];
                u1 = bjb + bjd;
                u3 = ss * (bjb - bjd);
                a[aOffs+ja + j] = t0 + t1;
                a[aOffs+jc + j] = t0 - t1;
                b[bOffs+ja + j] = u0 + u1;
                b[bOffs+jc + j] = u0 - u1;
                a[aOffs+jb + j] = t2 - u3;
                a[aOffs+jd + j] = t2 + u3;
                b[bOffs+jb + j] = u2 + t3;
                b[bOffs+jd + j] = u2 - t3;
                j += jump;
              }
              ja += jstepx;
              if (ja < istart) 
              {
                ja += ninc;
              }
            }
          }

          //  finished if n2 = 4 
          //  ------------------ 
          if (n2 == 4) goto L490;
          kk = la << 1;

          //  loop on nonzero k 
          //  ----------------- 
          i__4 = jstep - ink;
          i__3 = ink;
          for (k = ink; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) 
          {
            co1 = trigs[trOffs+kk + 1];
            si1 = s * trigs[trOffs+kk + 2];
            co2 = trigs[trOffs+(kk << 1) + 1];
            si2 = s * trigs[trOffs+(kk << 1) + 2];
            co3 = trigs[trOffs+kk * 3 + 1];
            si3 = s * trigs[trOffs+kk * 3 + 2];

            //  loop along transform 
            //  -------------------- 
            i__5 = (n - 1) * inc;
            i__6 = jstep << 2;
            for (jjj = k; i__6 < 0 ? jjj >= i__5 : jjj <= i__5; jjj += i__6) 
            {
              ja = istart + jjj;

              //     "transverse" loop 
              //     ----------------- 
              i__7 = inq;
              for (nu = 1; nu <= i__7; ++nu) 
              {
                jb = ja + jstepl;
                if (jb < istart) 
                {
                  jb += ninc;
                }
                jc = jb + jstepl;
                if (jc < istart) 
                {
                  jc += ninc;
                }
                jd = jc + jstepl;
                if (jd < istart) 
                {
                  jd += ninc;
                }
                j = 0;

                //  loop across transforms 
                //  ---------------------- 

                i__8 = nvex;
                for (l = 1; l <= i__8; ++l) 
                {
                  aja = a[aOffs+ja + j];
                  ajc = a[aOffs+jc + j];
                  t0 = aja + ajc;
                  t2 = aja - ajc;
                  ajb = a[aOffs+jb + j];
                  ajd = a[aOffs+jd + j];
                  t1 = ajb + ajd;
                  t3 = ss * (ajb - ajd);
                  bja = b[bOffs+ja + j];
                  bjc = b[bOffs+jc + j];
                  u0 = bja + bjc;
                  u2 = bja - bjc;
                  bjb = b[bOffs+jb + j];
                  bjd = b[bOffs+jd + j];
                  u1 = bjb + bjd;
                  u3 = ss * (bjb - bjd);
                  a[aOffs+ja + j] = t0 + t1;
                  b[bOffs+ja + j] = u0 + u1;
                  a[aOffs+jb + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                  b[bOffs+jb + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                  a[aOffs+jc + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
                  b[bOffs+jc + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
                  a[aOffs+jd + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
                  b[bOffs+jd + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
                  j += jump;
                }
                // -----( end of loop across transforms ) 
                ja += jstepx;
                if (ja < istart) 
                {
                  ja += ninc;
                }
              }
            }
            // -----( end of loop along transforms ) 
            kk += la << 1;
          }
          // -----( end of loop on nonzero k ) 
          la <<= 2;
        }
        // -----( end of loop on type I radix-4 passes) 

        //  central radix-2 pass 
        //  -------------------- 
      L200:
        if (m2 == 0) goto L300;

        jstep = n * inc / (la << 1);
        jstepl = jstep - ninc;

        //  k=0 loop (no twiddle factors) 
        //  ----------------------------- 
        i__2 = (n - 1) * inc;
        i__3 = jstep << 1;
        for (jjj = 0; i__3 < 0 ? jjj >= i__2 : jjj <= i__2; jjj += i__3) 
        {
          ja = istart + jjj;

          //     "transverse" loop 
          //     ----------------- 
          i__4 = inq;
          for (nu = 1; nu <= i__4; ++nu) 
          {
            jb = ja + jstepl;
            if (jb < istart) 
            {
              jb += ninc;
            }
            j = 0;

            //  loop across transforms 
            //  ---------------------- 

            i__6 = nvex;
            for (l = 1; l <= i__6; ++l) 
            {
              aja = a[aOffs+ja + j];
              ajb = a[aOffs+jb + j];
              t0 = aja - ajb;
              a[aOffs+ja + j] = aja + ajb;
              a[aOffs+jb + j] = t0;
              bja = b[bOffs+ja + j];
              bjb = b[bOffs+jb + j];
              u0 = bja - bjb;
              b[bOffs+ja + j] = bja + bjb;
              b[bOffs+jb + j] = u0;
              j += jump;
            }
            // -----(end of loop across transforms) 
            ja += jstepx;
            if (ja < istart) 
            {
              ja += ninc;
            }
          }
        }

        //  finished if n2=2 
        //  ---------------- 
        if (n2 == 2) 
        {
          goto L490;
        }

        kk = la << 1;

        //  loop on nonzero k 
        //  ----------------- 
        i__3 = jstep - ink;
        i__2 = ink;
        for (k = ink; i__2 < 0 ? k >= i__3 : k <= i__3; k += i__2) 
        {
          co1 = trigs[trOffs+kk + 1];
          si1 = s * trigs[trOffs+kk + 2];

          //  loop along transforms 
          //  --------------------- 
          i__4 = (n - 1) * inc;
          i__6 = jstep << 1;
          for (jjj = k; i__6 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__6) 
          {
            ja = istart + jjj;

            //     "transverse" loop 
            //     ----------------- 
            i__5 = inq;
            for (nu = 1; nu <= i__5; ++nu) 
            {
              jb = ja + jstepl;
              if (jb < istart) 
              {
                jb += ninc;
              }
              j = 0;

              //  loop across transforms 
              //  ---------------------- 
              if (kk == n2 / 2) 
              {

                i__7 = nvex;
                for (l = 1; l <= i__7; ++l) 
                {
                  aja = a[aOffs+ja + j];
                  ajb = a[aOffs+jb + j];
                  t0 = ss * (aja - ajb);
                  a[aOffs+ja + j] = aja + ajb;
                  bjb = b[bOffs+jb + j];
                  bja = b[bOffs+ja + j];
                  a[aOffs+jb + j] = ss * (bjb - bja);
                  b[bOffs+ja + j] = bja + bjb;
                  b[bOffs+jb + j] = t0;
                  j += jump;
                }

              } 
              else 
              {

                i__7 = nvex;
                for (l = 1; l <= i__7; ++l) 
                {
                  aja = a[aOffs+ja + j];
                  ajb = a[aOffs+jb + j];
                  t0 = aja - ajb;
                  a[aOffs+ja + j] = aja + ajb;
                  bja = b[bOffs+ja + j];
                  bjb = b[bOffs+jb + j];
                  u0 = bja - bjb;
                  b[bOffs+ja + j] = bja + bjb;
                  a[aOffs+jb + j] = co1 * t0 - si1 * u0;
                  b[bOffs+jb + j] = si1 * t0 + co1 * u0;
                  j += jump;
                }

              }

              // -----(end of loop across transforms) 
              ja += jstepx;
              if (ja < istart) 
              {
                ja += ninc;
              }
            }
          }
          // -----(end of loop along transforms) 
          kk += la << 1;
        }
        // -----(end of loop on nonzero k) 
        // -----(end of radix-2 pass) 

        la <<= 1;
        goto L400;

        //  central radix-8 pass 
        //  -------------------- 
      L300:
        if (m8 == 0) goto L400;
        jstep = n * inc / (la << 3);
        jstepl = jstep - ninc;
        mu = inq % 8;
        if (isign == -1) mu = 8 - mu;
        c1 = 1.0;
        if (mu == 3 || mu == 7) c1 = -1.0;
        c2 = Math.Sqrt(0.5);
        if (mu == 3 || mu == 5) c2 = -c2;
        c3 = c1 * c2;

        //  stage 1 
        //  ------- 
        i__2 = jstep - ink;
        i__3 = ink;
        for (k = 0; i__3 < 0 ? k >= i__2 : k <= i__2; k += i__3) 
        {
          i__6 = (n - 1) * inc;
          i__4 = jstep << 3;
          for (jjj = k; i__4 < 0 ? jjj >= i__6 : jjj <= i__6; jjj += i__4) 
          {
            ja = istart + jjj;

            //     "transverse" loop 
            //     ----------------- 
            i__5 = inq;
            for (nu = 1; nu <= i__5; ++nu) 
            {
              jb = ja + jstepl;
              if (jb < istart) 
              {
                jb += ninc;
              }
              jc = jb + jstepl;
              if (jc < istart) 
              {
                jc += ninc;
              }
              jd = jc + jstepl;
              if (jd < istart) 
              {
                jd += ninc;
              }
              je = jd + jstepl;
              if (je < istart) 
              {
                je += ninc;
              }
              jf = je + jstepl;
              if (jf < istart) 
              {
                jf += ninc;
              }
              jg = jf + jstepl;
              if (jg < istart) 
              {
                jg += ninc;
              }
              jh = jg + jstepl;
              if (jh < istart) 
              {
                jh += ninc;
              }
              j = 0;

              i__7 = nvex;
              for (l = 1; l <= i__7; ++l) 
              {
                aja = a[aOffs+ja + j];
                aje = a[aOffs+je + j];
                t0 = aja - aje;
                a[aOffs+ja + j] = aja + aje;
                ajc = a[aOffs+jc + j];
                ajg = a[aOffs+jg + j];
                t1 = c1 * (ajc - ajg);
                a[aOffs+je + j] = ajc + ajg;
                ajb = a[aOffs+jb + j];
                ajf = a[aOffs+jf + j];
                t2 = ajb - ajf;
                a[aOffs+jc + j] = ajb + ajf;
                ajd = a[aOffs+jd + j];
                ajh = a[aOffs+jh + j];
                t3 = ajd - ajh;
                a[aOffs+jg + j] = ajd + ajh;
                a[aOffs+jb + j] = t0;
                a[aOffs+jf + j] = t1;
                a[aOffs+jd + j] = c2 * (t2 - t3);
                a[aOffs+jh + j] = c3 * (t2 + t3);
                bja = b[bOffs+ja + j];
                bje = b[bOffs+je + j];
                u0 = bja - bje;
                b[bOffs+ja + j] = bja + bje;
                bjc = b[bOffs+jc + j];
                bjg = b[bOffs+jg + j];
                u1 = c1 * (bjc - bjg);
                b[bOffs+je + j] = bjc + bjg;
                bjb = b[bOffs+jb + j];
                bjf = b[bOffs+jf + j];
                u2 = bjb - bjf;
                b[bOffs+jc + j] = bjb + bjf;
                bjd = b[bOffs+jd + j];
                bjh = b[bOffs+jh + j];
                u3 = bjd - bjh;
                b[bOffs+jg + j] = bjd + bjh;
                b[bOffs+jb + j] = u0;
                b[bOffs+jf + j] = u1;
                b[bOffs+jd + j] = c2 * (u2 - u3);
                b[bOffs+jh + j] = c3 * (u2 + u3);
                j += jump;
              }
              ja += jstepx;
              if (ja < istart) 
              {
                ja += ninc;
              }
            }
          }
        }

        //  stage 2 
        //  ------- 

        //  k=0 (no twiddle factors) 
        //  ------------------------ 
        i__3 = (n - 1) * inc;
        i__2 = jstep << 3;
        for (jjj = 0; i__2 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__2) 
        {
          ja = istart + jjj;

          //     "transverse" loop 
          //     ----------------- 
          i__4 = inq;
          for (nu = 1; nu <= i__4; ++nu) 
          {
            jb = ja + jstepl;
            if (jb < istart) 
            {
              jb += ninc;
            }
            jc = jb + jstepl;
            if (jc < istart) 
            {
              jc += ninc;
            }
            jd = jc + jstepl;
            if (jd < istart) 
            {
              jd += ninc;
            }
            je = jd + jstepl;
            if (je < istart) 
            {
              je += ninc;
            }
            jf = je + jstepl;
            if (jf < istart) 
            {
              jf += ninc;
            }
            jg = jf + jstepl;
            if (jg < istart) 
            {
              jg += ninc;
            }
            jh = jg + jstepl;
            if (jh < istart) 
            {
              jh += ninc;
            }
            j = 0;

            i__6 = nvex;
            for (l = 1; l <= i__6; ++l) 
            {
              aja = a[aOffs+ja + j];
              aje = a[aOffs+je + j];
              t0 = aja + aje;
              t2 = aja - aje;
              ajc = a[aOffs+jc + j];
              ajg = a[aOffs+jg + j];
              t1 = ajc + ajg;
              t3 = c1 * (ajc - ajg);
              bja = b[bOffs+ja + j];
              bje = b[bOffs+je + j];
              u0 = bja + bje;
              u2 = bja - bje;
              bjc = b[bOffs+jc + j];
              bjg = b[bOffs+jg + j];
              u1 = bjc + bjg;
              u3 = c1 * (bjc - bjg);
              a[aOffs+ja + j] = t0 + t1;
              a[aOffs+je + j] = t0 - t1;
              b[bOffs+ja + j] = u0 + u1;
              b[bOffs+je + j] = u0 - u1;
              a[aOffs+jc + j] = t2 - u3;
              a[aOffs+jg + j] = t2 + u3;
              b[bOffs+jc + j] = u2 + t3;
              b[bOffs+jg + j] = u2 - t3;
              ajb = a[aOffs+jb + j];
              ajd = a[aOffs+jd + j];
              t0 = ajb + ajd;
              t2 = ajb - ajd;
              ajf = a[aOffs+jf + j];
              ajh = a[aOffs+jh + j];
              t1 = ajf - ajh;
              t3 = ajf + ajh;
              bjb = b[bOffs+jb + j];
              bjd = b[bOffs+jd + j];
              u0 = bjb + bjd;
              u2 = bjb - bjd;
              bjf = b[bOffs+jf + j];
              bjh = b[bOffs+jh + j];
              u1 = bjf - bjh;
              u3 = bjf + bjh;
              a[aOffs+jb + j] = t0 - u3;
              a[aOffs+jh + j] = t0 + u3;
              b[bOffs+jb + j] = u0 + t3;
              b[bOffs+jh + j] = u0 - t3;
              a[aOffs+jd + j] = t2 + u1;
              a[aOffs+jf + j] = t2 - u1;
              b[bOffs+jd + j] = u2 - t1;
              b[bOffs+jf + j] = u2 + t1;
              j += jump;
            }
            ja += jstepx;
            if (ja < istart) 
            {
              ja += ninc;
            }
          }
        }

        if (n2 == 8) goto L490;

        //  loop on nonzero k 
        //  ----------------- 
        kk = la << 1;

        i__2 = jstep - ink;
        i__3 = ink;
        for (k = ink; i__3 < 0 ? k >= i__2 : k <= i__2; k += i__3) 
        {

          co1 = trigs[trOffs+kk + 1];
          si1 = s * trigs[trOffs+kk + 2];
          co2 = trigs[trOffs+(kk << 1) + 1];
          si2 = s * trigs[trOffs+(kk << 1) + 2];
          co3 = trigs[trOffs+kk * 3 + 1];
          si3 = s * trigs[trOffs+kk * 3 + 2];
          co4 = trigs[trOffs+(kk << 2) + 1];
          si4 = s * trigs[trOffs+(kk << 2) + 2];
          co5 = trigs[trOffs+kk * 5 + 1];
          si5 = s * trigs[trOffs+kk * 5 + 2];
          co6 = trigs[trOffs+kk * 6 + 1];
          si6 = s * trigs[trOffs+kk * 6 + 2];
          co7 = trigs[trOffs+kk * 7 + 1];
          si7 = s * trigs[trOffs+kk * 7 + 2];

          i__4 = (n - 1) * inc;
          i__6 = jstep << 3;
          for (jjj = k; i__6 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__6) 
          {
            ja = istart + jjj;

            //     "transverse" loop 
            //     ----------------- 
            i__5 = inq;
            for (nu = 1; nu <= i__5; ++nu) 
            {
              jb = ja + jstepl;
              if (jb < istart) 
              {
                jb += ninc;
              }
              jc = jb + jstepl;
              if (jc < istart) 
              {
                jc += ninc;
              }
              jd = jc + jstepl;
              if (jd < istart) 
              {
                jd += ninc;
              }
              je = jd + jstepl;
              if (je < istart) 
              {
                je += ninc;
              }
              jf = je + jstepl;
              if (jf < istart) 
              {
                jf += ninc;
              }
              jg = jf + jstepl;
              if (jg < istart) 
              {
                jg += ninc;
              }
              jh = jg + jstepl;
              if (jh < istart) 
              {
                jh += ninc;
              }
              j = 0;

              i__7 = nvex;
              for (l = 1; l <= i__7; ++l) 
              {
                aja = a[aOffs+ja + j];
                aje = a[aOffs+je + j];
                t0 = aja + aje;
                t2 = aja - aje;
                ajc = a[aOffs+jc + j];
                ajg = a[aOffs+jg + j];
                t1 = ajc + ajg;
                t3 = c1 * (ajc - ajg);
                bja = b[bOffs+ja + j];
                bje = b[bOffs+je + j];
                u0 = bja + bje;
                u2 = bja - bje;
                bjc = b[bOffs+jc + j];
                bjg = b[bOffs+jg + j];
                u1 = bjc + bjg;
                u3 = c1 * (bjc - bjg);
                a[aOffs+ja + j] = t0 + t1;
                b[bOffs+ja + j] = u0 + u1;
                a[aOffs+je + j] = co4 * (t0 - t1) - si4 * (u0 - u1);
                b[bOffs+je + j] = si4 * (t0 - t1) + co4 * (u0 - u1);
                a[aOffs+jc + j] = co2 * (t2 - u3) - si2 * (u2 + t3);
                b[bOffs+jc + j] = si2 * (t2 - u3) + co2 * (u2 + t3);
                a[aOffs+jg + j] = co6 * (t2 + u3) - si6 * (u2 - t3);
                b[bOffs+jg + j] = si6 * (t2 + u3) + co6 * (u2 - t3);
                ajb = a[aOffs+jb + j];
                ajd = a[aOffs+jd + j];
                t0 = ajb + ajd;
                t2 = ajb - ajd;
                ajf = a[aOffs+jf + j];
                ajh = a[aOffs+jh + j];
                t1 = ajf - ajh;
                t3 = ajf + ajh;
                bjb = b[bOffs+jb + j];
                bjd = b[bOffs+jd + j];
                u0 = bjb + bjd;
                u2 = bjb - bjd;
                bjf = b[bOffs+jf + j];
                bjh = b[bOffs+jh + j];
                u1 = bjf - bjh;
                u3 = bjf + bjh;
                a[aOffs+jb + j] = co1 * (t0 - u3) - si1 * (u0 + t3);
                b[bOffs+jb + j] = si1 * (t0 - u3) + co1 * (u0 + t3);
                a[aOffs+jh + j] = co7 * (t0 + u3) - si7 * (u0 - t3);
                b[bOffs+jh + j] = si7 * (t0 + u3) + co7 * (u0 - t3);
                a[aOffs+jd + j] = co3 * (t2 + u1) - si3 * (u2 - t1);
                b[bOffs+jd + j] = si3 * (t2 + u1) + co3 * (u2 - t1);
                a[aOffs+jf + j] = co5 * (t2 - u1) - si5 * (u2 + t1);
                b[bOffs+jf + j] = si5 * (t2 - u1) + co5 * (u2 + t1);
                j += jump;
              }
              ja += jstepx;
              if (ja < istart) 
              {
                ja += ninc;
              }
            }
          }
          kk += la << 1;
        }

        la <<= 3;

        //  loop on type II radix-4 passes 
        //  ------------------------------ 
      L400:
        mu = inq % 4;
        if (isign == -1) mu = 4 - mu;
        ss = 1.0;
        if (mu == 3) ss = -1.0;

        i__3 = m;
        for (ipass = mh + 1; ipass <= i__3; ++ipass) 
        {
          jstep = n * inc / (la << 2);
          jstepl = jstep - ninc;
          laincl = la * ink - ninc;

          //  k=0 loop (no twiddle factors) 
          //  ----------------------------- 
          i__2 = (la - 1) * ink;
          i__6 = jstep << 2;
          for (ll = 0; i__6 < 0 ? ll >= i__2 : ll <= i__2; ll += i__6) 
          {

            i__4 = (n - 1) * inc;
            i__5 = (la << 2) * ink;
            for (jjj = ll; i__5 < 0 ? jjj >= i__4 : jjj <= i__4; jjj += i__5) 
            {
              ja = istart + jjj;

              //     "transverse" loop 
              //     ----------------- 
              i__7 = inq;
              for (nu = 1; nu <= i__7; ++nu) 
              {
                jb = ja + jstepl;
                if (jb < istart) 
                {
                  jb += ninc;
                }
                jc = jb + jstepl;
                if (jc < istart) 
                {
                  jc += ninc;
                }
                jd = jc + jstepl;
                if (jd < istart) 
                {
                  jd += ninc;
                }
                je = ja + laincl;
                if (je < istart) 
                {
                  je += ninc;
                }
                jf = je + jstepl;
                if (jf < istart) 
                {
                  jf += ninc;
                }
                jg = jf + jstepl;
                if (jg < istart) 
                {
                  jg += ninc;
                }
                jh = jg + jstepl;
                if (jh < istart) 
                {
                  jh += ninc;
                }
                ji = je + laincl;
                if (ji < istart) 
                {
                  ji += ninc;
                }
                jj = ji + jstepl;
                if (jj < istart) 
                {
                  jj += ninc;
                }
                jk = jj + jstepl;
                if (jk < istart) 
                {
                  jk += ninc;
                }
                jl = jk + jstepl;
                if (jl < istart) 
                {
                  jl += ninc;
                }
                jm = ji + laincl;
                if (jm < istart) 
                {
                  jm += ninc;
                }
                jn = jm + jstepl;
                if (jn < istart) 
                {
                  jn += ninc;
                }
                jo = jn + jstepl;
                if (jo < istart) 
                {
                  jo += ninc;
                }
                jp = jo + jstepl;
                if (jp < istart) 
                {
                  jp += ninc;
                }
                j = 0;

                //  loop across transforms 
                //  ---------------------- 

                i__8 = nvex;
                for (l = 1; l <= i__8; ++l) 
                {
                  aja = a[aOffs+ja + j];
                  ajc = a[aOffs+jc + j];
                  t0 = aja + ajc;
                  t2 = aja - ajc;
                  ajb = a[aOffs+jb + j];
                  ajd = a[aOffs+jd + j];
                  t1 = ajb + ajd;
                  t3 = ss * (ajb - ajd);
                  aji = a[aOffs+ji + j];
                  ajc = aji;
                  bja = b[bOffs+ja + j];
                  bjc = b[bOffs+jc + j];
                  u0 = bja + bjc;
                  u2 = bja - bjc;
                  bjb = b[bOffs+jb + j];
                  bjd = b[bOffs+jd + j];
                  u1 = bjb + bjd;
                  u3 = ss * (bjb - bjd);
                  aje = a[aOffs+je + j];
                  ajb = aje;
                  a[aOffs+ja + j] = t0 + t1;
                  a[aOffs+ji + j] = t0 - t1;
                  b[bOffs+ja + j] = u0 + u1;
                  bjc = u0 - u1;
                  bjm = b[bOffs+jm + j];
                  bjd = bjm;
                  a[aOffs+je + j] = t2 - u3;
                  ajd = t2 + u3;
                  bjb = u2 + t3;
                  b[bOffs+jm + j] = u2 - t3;
                  // ---------------------- 
                  ajg = a[aOffs+jg + j];
                  t0 = ajb + ajg;
                  t2 = ajb - ajg;
                  ajf = a[aOffs+jf + j];
                  ajh = a[aOffs+jh + j];
                  t1 = ajf + ajh;
                  t3 = ss * (ajf - ajh);
                  ajj = a[aOffs+jj + j];
                  ajg = ajj;
                  bje = b[bOffs+je + j];
                  bjg = b[bOffs+jg + j];
                  u0 = bje + bjg;
                  u2 = bje - bjg;
                  bjf = b[bOffs+jf + j];
                  bjh = b[bOffs+jh + j];
                  u1 = bjf + bjh;
                  u3 = ss * (bjf - bjh);
                  b[bOffs+je + j] = bjb;
                  a[aOffs+jb + j] = t0 + t1;
                  a[aOffs+jj + j] = t0 - t1;
                  bjj = b[bOffs+jj + j];
                  bjg = bjj;
                  b[bOffs+jb + j] = u0 + u1;
                  b[bOffs+jj + j] = u0 - u1;
                  a[aOffs+jf + j] = t2 - u3;
                  ajh = t2 + u3;
                  b[bOffs+jf + j] = u2 + t3;
                  bjh = u2 - t3;
                  // ---------------------- 
                  ajk = a[aOffs+jk + j];
                  t0 = ajc + ajk;
                  t2 = ajc - ajk;
                  ajl = a[aOffs+jl + j];
                  t1 = ajg + ajl;
                  t3 = ss * (ajg - ajl);
                  bji = b[bOffs+ji + j];
                  bjk = b[bOffs+jk + j];
                  u0 = bji + bjk;
                  u2 = bji - bjk;
                  ajo = a[aOffs+jo + j];
                  ajl = ajo;
                  bjl = b[bOffs+jl + j];
                  u1 = bjg + bjl;
                  u3 = ss * (bjg - bjl);
                  b[bOffs+ji + j] = bjc;
                  a[aOffs+jc + j] = t0 + t1;
                  a[aOffs+jk + j] = t0 - t1;
                  bjo = b[bOffs+jo + j];
                  bjl = bjo;
                  b[bOffs+jc + j] = u0 + u1;
                  b[bOffs+jk + j] = u0 - u1;
                  a[aOffs+jg + j] = t2 - u3;
                  a[aOffs+jo + j] = t2 + u3;
                  b[bOffs+jg + j] = u2 + t3;
                  b[bOffs+jo + j] = u2 - t3;
                  // ---------------------- 
                  ajm = a[aOffs+jm + j];
                  t0 = ajm + ajl;
                  t2 = ajm - ajl;
                  ajn = a[aOffs+jn + j];
                  ajp = a[aOffs+jp + j];
                  t1 = ajn + ajp;
                  t3 = ss * (ajn - ajp);
                  a[aOffs+jm + j] = ajd;
                  u0 = bjd + bjl;
                  u2 = bjd - bjl;
                  bjn = b[bOffs+jn + j];
                  bjp = b[bOffs+jp + j];
                  u1 = bjn + bjp;
                  u3 = ss * (bjn - bjp);
                  a[aOffs+jn + j] = ajh;
                  a[aOffs+jd + j] = t0 + t1;
                  a[aOffs+jl + j] = t0 - t1;
                  b[bOffs+jd + j] = u0 + u1;
                  b[bOffs+jl + j] = u0 - u1;
                  b[bOffs+jn + j] = bjh;
                  a[aOffs+jh + j] = t2 - u3;
                  a[aOffs+jp + j] = t2 + u3;
                  b[bOffs+jh + j] = u2 + t3;
                  b[bOffs+jp + j] = u2 - t3;
                  j += jump;
                }
                // -----( end of loop across transforms ) 
                ja += jstepx;
                if (ja < istart) 
                {
                  ja += ninc;
                }
              }
            }
          }
          // -----( end of double loop for k=0 ) 

          //  finished if last pass 
          //  --------------------- 
          if (ipass == m) goto L490;

          kk = la << 1;

          //     loop on nonzero k 
          //     ----------------- 
          i__6 = jstep - ink;
          i__2 = ink;
          for (k = ink; i__2 < 0 ? k >= i__6 : k <= i__6; k += i__2) 
          {
            co1 = trigs[trOffs+kk + 1];
            si1 = s * trigs[trOffs+kk + 2];
            co2 = trigs[trOffs+(kk << 1) + 1];
            si2 = s * trigs[trOffs+(kk << 1) + 2];
            co3 = trigs[trOffs+kk * 3 + 1];
            si3 = s * trigs[trOffs+kk * 3 + 2];

            //  double loop along first transform in block 
            //  ------------------------------------------ 
            i__5 = (la - 1) * ink;
            i__4 = jstep << 2;
            for (ll = k; i__4 < 0 ? ll >= i__5 : ll <= i__5; ll += i__4) 
            {

              i__7 = (n - 1) * inc;
              i__8 = (la << 2) * ink;
              for (jjj = ll; i__8 < 0 ? jjj >= i__7 : jjj <= i__7; jjj += i__8) 
              {
                ja = istart + jjj;

                //     "transverse" loop 
                //     ----------------- 
                i__9 = inq;
                for (nu = 1; nu <= i__9; ++nu) 
                {
                  jb = ja + jstepl;
                  if (jb < istart) 
                  {
                    jb += ninc;
                  }
                  jc = jb + jstepl;
                  if (jc < istart) 
                  {
                    jc += ninc;
                  }
                  jd = jc + jstepl;
                  if (jd < istart) 
                  {
                    jd += ninc;
                  }
                  je = ja + laincl;
                  if (je < istart) 
                  {
                    je += ninc;
                  }
                  jf = je + jstepl;
                  if (jf < istart) 
                  {
                    jf += ninc;
                  }
                  jg = jf + jstepl;
                  if (jg < istart) 
                  {
                    jg += ninc;
                  }
                  jh = jg + jstepl;
                  if (jh < istart) 
                  {
                    jh += ninc;
                  }
                  ji = je + laincl;
                  if (ji < istart) 
                  {
                    ji += ninc;
                  }
                  jj = ji + jstepl;
                  if (jj < istart) 
                  {
                    jj += ninc;
                  }
                  jk = jj + jstepl;
                  if (jk < istart) 
                  {
                    jk += ninc;
                  }
                  jl = jk + jstepl;
                  if (jl < istart) 
                  {
                    jl += ninc;
                  }
                  jm = ji + laincl;
                  if (jm < istart) 
                  {
                    jm += ninc;
                  }
                  jn = jm + jstepl;
                  if (jn < istart) 
                  {
                    jn += ninc;
                  }
                  jo = jn + jstepl;
                  if (jo < istart) 
                  {
                    jo += ninc;
                  }
                  jp = jo + jstepl;
                  if (jp < istart) 
                  {
                    jp += ninc;
                  }
                  j = 0;

                  //  loop across transforms 
                  //  ---------------------- 

                  i__10 = nvex;
                  for (l = 1; l <= i__10; ++l) 
                  {
                    aja = a[aOffs+ja + j];
                    ajc = a[aOffs+jc + j];
                    t0 = aja + ajc;
                    t2 = aja - ajc;
                    ajb = a[aOffs+jb + j];
                    ajd = a[aOffs+jd + j];
                    t1 = ajb + ajd;
                    t3 = ss * (ajb - ajd);
                    aji = a[aOffs+ji + j];
                    ajc = aji;
                    bja = b[bOffs+ja + j];
                    bjc = b[bOffs+jc + j];
                    u0 = bja + bjc;
                    u2 = bja - bjc;
                    bjb = b[bOffs+jb + j];
                    bjd = b[bOffs+jd + j];
                    u1 = bjb + bjd;
                    u3 = ss * (bjb - bjd);
                    aje = a[aOffs+je + j];
                    ajb = aje;
                    a[aOffs+ja + j] = t0 + t1;
                    b[bOffs+ja + j] = u0 + u1;
                    a[aOffs+je + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    bjb = si1 * (t2 - u3) + co1 * (u2 + t3);
                    bjm = b[bOffs+jm + j];
                    bjd = bjm;
                    a[aOffs+ji + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
                    bjc = si2 * (t0 - t1) + co2 * (u0 - u1);
                    ajd = co3 * (t2 + u3) - si3 * (u2 - t3);
                    b[bOffs+jm + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
                    // ---------------------------------------- 
                    ajg = a[aOffs+jg + j];
                    t0 = ajb + ajg;
                    t2 = ajb - ajg;
                    ajf = a[aOffs+jf + j];
                    ajh = a[aOffs+jh + j];
                    t1 = ajf + ajh;
                    t3 = ss * (ajf - ajh);
                    ajj = a[aOffs+jj + j];
                    ajg = ajj;
                    bje = b[bOffs+je + j];
                    bjg = b[bOffs+jg + j];
                    u0 = bje + bjg;
                    u2 = bje - bjg;
                    bjf = b[bOffs+jf + j];
                    bjh = b[bOffs+jh + j];
                    u1 = bjf + bjh;
                    u3 = ss * (bjf - bjh);
                    b[bOffs+je + j] = bjb;
                    a[aOffs+jb + j] = t0 + t1;
                    b[bOffs+jb + j] = u0 + u1;
                    bjj = b[bOffs+jj + j];
                    bjg = bjj;
                    a[aOffs+jf + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+jf + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    a[aOffs+jj + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
                    b[bOffs+jj + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
                    ajh = co3 * (t2 + u3) - si3 * (u2 - t3);
                    bjh = si3 * (t2 + u3) + co3 * (u2 - t3);
                    // ---------------------------------------- 
                    ajk = a[aOffs+jk + j];
                    t0 = ajc + ajk;
                    t2 = ajc - ajk;
                    ajl = a[aOffs+jl + j];
                    t1 = ajg + ajl;
                    t3 = ss * (ajg - ajl);
                    bji = b[bOffs+ji + j];
                    bjk = b[bOffs+jk + j];
                    u0 = bji + bjk;
                    u2 = bji - bjk;
                    ajo = a[aOffs+jo + j];
                    ajl = ajo;
                    bjl = b[bOffs+jl + j];
                    u1 = bjg + bjl;
                    u3 = ss * (bjg - bjl);
                    b[bOffs+ji + j] = bjc;
                    a[aOffs+jc + j] = t0 + t1;
                    b[bOffs+jc + j] = u0 + u1;
                    bjo = b[bOffs+jo + j];
                    bjl = bjo;
                    a[aOffs+jg + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+jg + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    a[aOffs+jk + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
                    b[bOffs+jk + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
                    a[aOffs+jo + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
                    b[bOffs+jo + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
                    // ---------------------------------------- 
                    ajm = a[aOffs+jm + j];
                    t0 = ajm + ajl;
                    t2 = ajm - ajl;
                    ajn = a[aOffs+jn + j];
                    ajp = a[aOffs+jp + j];
                    t1 = ajn + ajp;
                    t3 = ss * (ajn - ajp);
                    a[aOffs+jm + j] = ajd;
                    u0 = bjd + bjl;
                    u2 = bjd - bjl;
                    a[aOffs+jn + j] = ajh;
                    bjn = b[bOffs+jn + j];
                    bjp = b[bOffs+jp + j];
                    u1 = bjn + bjp;
                    u3 = ss * (bjn - bjp);
                    b[bOffs+jn + j] = bjh;
                    a[aOffs+jd + j] = t0 + t1;
                    b[bOffs+jd + j] = u0 + u1;
                    a[aOffs+jh + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+jh + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    a[aOffs+jl + j] = co2 * (t0 - t1) - si2 * (u0 - u1);
                    b[bOffs+jl + j] = si2 * (t0 - t1) + co2 * (u0 - u1);
                    a[aOffs+jp + j] = co3 * (t2 + u3) - si3 * (u2 - t3);
                    b[bOffs+jp + j] = si3 * (t2 + u3) + co3 * (u2 - t3);
                    j += jump;
                  }
                  // -----(end of loop across transforms) 
                  ja += jstepx;
                  if (ja < istart) 
                  {
                    ja += ninc;
                  }
                }
              }
            }
            // -----( end of double loop for this k ) 
            kk += la << 1;
          }
          // -----( end of loop over values of k ) 
          la <<= 2;
        }
        // -----( end of loop on type II radix-4 passes ) 
        // -----( nvex transforms completed) 
      L490:
        istart += nvex * jump;
      }
      // -----( end of loop on blocks of transforms )
    }

    static void gpfa3f    (double[] a, int aOffs, double[] b, int bOffs, double[] trigs,  int trOffs, int inc, int jump, int n, int mm, int lot, int isign)
    {
      const double sin60 = 0.866025403784437;

      // *************************************************************** 
      // *                                                             * 
      // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
      // *  reset to 64 for other cray machines, or to any large value * 
      // *  (greater than or equal to lot) for a scalar computer.      * 
      // *                                                             * 
      // *************************************************************** 

      // System generated locals 
      int i__1, i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

      double  s, c1, t1, t2, t3, u1, u2, u3, co1, co2, si1, si2, aja, ajb, 
        ajc, bjb, bjc, bja, ajd, bjd, aje, ajf, ajh, bje, bjf, bjh, 
        aji, ajg, bji, bjg;

      int    ninc, left, nvex, j, k, l, m, ipass, nblox, jstep, n3, ja, jb, 
        la, jc, jd, nb, je, jf, jg, jh, mh, kk, ji, ll, mu, nu, laincl, 
        jstepl, istart, jstepx, jjj, ink, inq;

      // Parameter adjustments
      --trOffs;
      --bOffs;
      --aOffs;

      n3 = powii(3,mm);
      inq = n / n3;
      jstepx = (n3 - n) * inc;
      ninc = n * inc;
      ink = inc * inq;
      mu = inq % 3;
      if (isign == -1) mu = 3 - mu;
      m = mm;
      mh = (m + 1) / 2;
      s = (double) isign;
      c1 = sin60;
      if (mu == 2) c1 = -c1;

      nblox = (lot - 1) / lvr + 1;
      left = lot;
      s = (double) isign;
      istart = 1;

      //  loop on blocks of lvr transforms 
      //  -------------------------------- 
      i__1 = nblox;
      for (nb = 1; nb <= i__1; ++nb) 
      {

        if (left <= lvr) 
        {
          nvex = left;
        } 
        else if (left < lvr << 1) 
        {
          nvex = left / 2;
          nvex += nvex % 2;
        } 
        else 
        {
          nvex = lvr;
        }
        left -= nvex;

        la = 1;

        //  loop on type I radix-3 passes 
        //  ----------------------------- 
        i__2 = mh;
        for (ipass = 1; ipass <= i__2; ++ipass) 
        {
          jstep = n * inc / (la * 3);
          jstepl = jstep - ninc;

          //  k = 0 loop (no twiddle factors) 
          //  ------------------------------- 
          i__3 = (n - 1) * inc;
          i__4 = jstep * 3;
          for (jjj = 0; i__4 < 0 ? jjj >= i__3 : jjj <= i__3; jjj += i__4) 
          {
            ja = istart + jjj;

            //  "transverse" loop 
            //  ----------------- 
            i__5 = inq;
            for (nu = 1; nu <= i__5; ++nu) 
            {
              jb = ja + jstepl;
              if (jb < istart) 
              {
                jb += ninc;
              }
              jc = jb + jstepl;
              if (jc < istart) 
              {
                jc += ninc;
              }
              j = 0;

              //  loop across transforms 
              //  ---------------------- 

              i__6 = nvex;
              for (l = 1; l <= i__6; ++l) 
              {
                ajb = a[aOffs+jb + j];
                ajc = a[aOffs+jc + j];
                t1 = ajb + ajc;
                aja = a[aOffs+ja + j];
                t2 = aja - t1 * .5;
                t3 = c1 * (ajb - ajc);
                bjb = b[bOffs+jb + j];
                bjc = b[bOffs+jc + j];
                u1 = bjb + bjc;
                bja = b[bOffs+ja + j];
                u2 = bja - u1 * .5;
                u3 = c1 * (bjb - bjc);
                a[aOffs+ja + j] = aja + t1;
                b[bOffs+ja + j] = bja + u1;
                a[aOffs+jb + j] = t2 - u3;
                b[bOffs+jb + j] = u2 + t3;
                a[aOffs+jc + j] = t2 + u3;
                b[bOffs+jc + j] = u2 - t3;
                j += jump;
              }
              ja += jstepx;
              if (ja < istart) 
              {
                ja += ninc;
              }
            }
          }

          //  finished if n3 = 3 
          //  ------------------ 
          if (n3 == 3) goto L490;
          kk = la << 1;

          //  loop on nonzero k 
          //  ----------------- 
          i__4 = jstep - ink;
          i__3 = ink;
          for (k = ink; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) 
          {
            co1 = trigs[trOffs+kk + 1];
            si1 = s * trigs[trOffs+kk + 2];
            co2 = trigs[trOffs+(kk << 1) + 1];
            si2 = s * trigs[trOffs+(kk << 1) + 2];

            //  loop along transform 
            //  -------------------- 
            i__5 = (n - 1) * inc;
            i__6 = jstep * 3;
            for (jjj = k; i__6 < 0 ? jjj >= i__5 : jjj <= i__5; jjj += i__6) 
            {
              ja = istart + jjj;

              //  "transverse" loop 
              //  ----------------- 
              i__7 = inq;
              for (nu = 1; nu <= i__7; ++nu) 
              {
                jb = ja + jstepl;
                if (jb < istart) 
                {
                  jb += ninc;
                }
                jc = jb + jstepl;
                if (jc < istart) 
                {
                  jc += ninc;
                }
                j = 0;

                //  loop across transforms 
                //  ---------------------- 

                i__8 = nvex;
                for (l = 1; l <= i__8; ++l) 
                {
                  ajb = a[aOffs+jb + j];
                  ajc = a[aOffs+jc + j];
                  t1 = ajb + ajc;
                  aja = a[aOffs+ja + j];
                  t2 = aja - t1 * .5;
                  t3 = c1 * (ajb - ajc);
                  bjb = b[bOffs+jb + j];
                  bjc = b[bOffs+jc + j];
                  u1 = bjb + bjc;
                  bja = b[bOffs+ja + j];
                  u2 = bja - u1 * .5;
                  u3 = c1 * (bjb - bjc);
                  a[aOffs+ja + j] = aja + t1;
                  b[bOffs+ja + j] = bja + u1;
                  a[aOffs+jb + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                  b[bOffs+jb + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                  a[aOffs+jc + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
                  b[bOffs+jc + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
                  j += jump;
                }
                // -----( end of loop across transforms ) 
                ja += jstepx;
                if (ja < istart) 
                {
                  ja += ninc;
                }
              }
            }
            // -----( end of loop along transforms ) 
            kk += la << 1;
          }
          // -----( end of loop on nonzero k ) 
          la *= 3;
        }
        // -----( end of loop on type I radix-3 passes) 

        //  loop on type II radix-3 passes 
        //  ------------------------------ 

        i__2 = m;
        for (ipass = mh + 1; ipass <= i__2; ++ipass) 
        {
          jstep = n * inc / (la * 3);
          jstepl = jstep - ninc;
          laincl = la * ink - ninc;

          //  k=0 loop (no twiddle factors) 
          //  ----------------------------- 
          i__3 = (la - 1) * ink;
          i__4 = jstep * 3;
          for (ll = 0; i__4 < 0 ? ll >= i__3 : ll <= i__3; ll += i__4) 
          {

            i__6 = (n - 1) * inc;
            i__5 = la * 3 * ink;
            for (jjj = ll; i__5 < 0 ? jjj >= i__6 : jjj <= i__6; jjj += i__5) 
            {
              ja = istart + jjj;

              //  "transverse" loop 
              //  ----------------- 
              i__7 = inq;
              for (nu = 1; nu <= i__7; ++nu) 
              {
                jb = ja + jstepl;
                if (jb < istart) 
                {
                  jb += ninc;
                }
                jc = jb + jstepl;
                if (jc < istart) 
                {
                  jc += ninc;
                }
                jd = ja + laincl;
                if (jd < istart) 
                {
                  jd += ninc;
                }
                je = jd + jstepl;
                if (je < istart) 
                {
                  je += ninc;
                }
                jf = je + jstepl;
                if (jf < istart) 
                {
                  jf += ninc;
                }
                jg = jd + laincl;
                if (jg < istart) 
                {
                  jg += ninc;
                }
                jh = jg + jstepl;
                if (jh < istart) 
                {
                  jh += ninc;
                }
                ji = jh + jstepl;
                if (ji < istart) 
                {
                  ji += ninc;
                }
                j = 0;

                //  loop across transforms 
                //  ---------------------- 

                i__8 = nvex;
                for (l = 1; l <= i__8; ++l) 
                {
                  ajb = a[aOffs+jb + j];
                  ajc = a[aOffs+jc + j];
                  t1 = ajb + ajc;
                  aja = a[aOffs+ja + j];
                  t2 = aja - t1 * .5;
                  t3 = c1 * (ajb - ajc);
                  ajd = a[aOffs+jd + j];
                  ajb = ajd;
                  bjb = b[bOffs+jb + j];
                  bjc = b[bOffs+jc + j];
                  u1 = bjb + bjc;
                  bja = b[bOffs+ja + j];
                  u2 = bja - u1 * .5;
                  u3 = c1 * (bjb - bjc);
                  bjd = b[bOffs+jd + j];
                  bjb = bjd;
                  a[aOffs+ja + j] = aja + t1;
                  b[bOffs+ja + j] = bja + u1;
                  a[aOffs+jd + j] = t2 - u3;
                  b[bOffs+jd + j] = u2 + t3;
                  ajc = t2 + u3;
                  bjc = u2 - t3;
                  // ---------------------- 
                  aje = a[aOffs+je + j];
                  ajf = a[aOffs+jf + j];
                  t1 = aje + ajf;
                  t2 = ajb - t1 * .5;
                  t3 = c1 * (aje - ajf);
                  ajh = a[aOffs+jh + j];
                  ajf = ajh;
                  bje = b[bOffs+je + j];
                  bjf = b[bOffs+jf + j];
                  u1 = bje + bjf;
                  u2 = bjb - u1 * .5;
                  u3 = c1 * (bje - bjf);
                  bjh = b[bOffs+jh + j];
                  bjf = bjh;
                  a[aOffs+jb + j] = ajb + t1;
                  b[bOffs+jb + j] = bjb + u1;
                  a[aOffs+je + j] = t2 - u3;
                  b[bOffs+je + j] = u2 + t3;
                  a[aOffs+jh + j] = t2 + u3;
                  b[bOffs+jh + j] = u2 - t3;
                  // ---------------------- 
                  aji = a[aOffs+ji + j];
                  t1 = ajf + aji;
                  ajg = a[aOffs+jg + j];
                  t2 = ajg - t1 * .5;
                  t3 = c1 * (ajf - aji);
                  t1 = ajg + t1;
                  a[aOffs+jg + j] = ajc;
                  bji = b[bOffs+ji + j];
                  u1 = bjf + bji;
                  bjg = b[bOffs+jg + j];
                  u2 = bjg - u1 * .5;
                  u3 = c1 * (bjf - bji);
                  u1 = bjg + u1;
                  b[bOffs+jg + j] = bjc;
                  a[aOffs+jc + j] = t1;
                  b[bOffs+jc + j] = u1;
                  a[aOffs+jf + j] = t2 - u3;
                  b[bOffs+jf + j] = u2 + t3;
                  a[aOffs+ji + j] = t2 + u3;
                  b[bOffs+ji + j] = u2 - t3;
                  j += jump;
                }
                // -----( end of loop across transforms ) 
                ja += jstepx;
                if (ja < istart) 
                {
                  ja += ninc;
                }
              }
            }
          }
          // -----( end of double loop for k=0 ) 

          //  finished if last pass 
          //  --------------------- 
          if (ipass == m) goto L490;
          kk = la << 1;

          //     loop on nonzero k 
          //     ----------------- 
          i__4 = jstep - ink;
          i__3 = ink;
          for (k = ink; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) 
          {
            co1 = trigs[trOffs+kk + 1];
            si1 = s * trigs[trOffs+kk + 2];
            co2 = trigs[trOffs+(kk << 1) + 1];
            si2 = s * trigs[trOffs+(kk << 1) + 2];

            //  double loop along first transform in block 
            //  ------------------------------------------ 
            i__5 = (la - 1) * ink;
            i__6 = jstep * 3;
            for (ll = k; i__6 < 0 ? ll >= i__5 : ll <= i__5; ll += i__6) 
            {

              i__7 = (n - 1) * inc;
              i__8 = la * 3 * ink;
              for (jjj = ll; i__8 < 0 ? jjj >= i__7 : jjj <= i__7; jjj += i__8) 
              {
                ja = istart + jjj;

                //  "transverse" loop 
                //  ----------------- 
                i__9 = inq;
                for (nu = 1; nu <= i__9; ++nu) 
                {
                  jb = ja + jstepl;
                  if (jb < istart) 
                  {
                    jb += ninc;
                  }
                  jc = jb + jstepl;
                  if (jc < istart) 
                  {
                    jc += ninc;
                  }
                  jd = ja + laincl;
                  if (jd < istart) 
                  {
                    jd += ninc;
                  }
                  je = jd + jstepl;
                  if (je < istart) 
                  {
                    je += ninc;
                  }
                  jf = je + jstepl;
                  if (jf < istart) 
                  {
                    jf += ninc;
                  }
                  jg = jd + laincl;
                  if (jg < istart) 
                  {
                    jg += ninc;
                  }
                  jh = jg + jstepl;
                  if (jh < istart) 
                  {
                    jh += ninc;
                  }
                  ji = jh + jstepl;
                  if (ji < istart) 
                  {
                    ji += ninc;
                  }
                  j = 0;

                  //  loop across transforms 
                  //  ---------------------- 

                  i__10 = nvex;
                  for (l = 1; l <= i__10; ++l) 
                  {
                    ajb = a[aOffs+jb + j];
                    ajc = a[aOffs+jc + j];
                    t1 = ajb + ajc;
                    aja = a[aOffs+ja + j];
                    t2 = aja - t1 * .5;
                    t3 = c1 * (ajb - ajc);
                    ajd = a[aOffs+jd + j];
                    ajb = ajd;
                    bjb = b[bOffs+jb + j];
                    bjc = b[bOffs+jc + j];
                    u1 = bjb + bjc;
                    bja = b[bOffs+ja + j];
                    u2 = bja - u1 * .5;
                    u3 = c1 * (bjb - bjc);
                    bjd = b[bOffs+jd + j];
                    bjb = bjd;
                    a[aOffs+ja + j] = aja + t1;
                    b[bOffs+ja + j] = bja + u1;
                    a[aOffs+jd + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+jd + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    ajc = co2 * (t2 + u3) - si2 * (u2 - t3);
                    bjc = si2 * (t2 + u3) + co2 * (u2 - t3);
                    // ---------------------- 
                    aje = a[aOffs+je + j];
                    ajf = a[aOffs+jf + j];
                    t1 = aje + ajf;
                    t2 = ajb - t1 * .5;
                    t3 = c1 * (aje - ajf);
                    ajh = a[aOffs+jh + j];
                    ajf = ajh;
                    bje = b[bOffs+je + j];
                    bjf = b[bOffs+jf + j];
                    u1 = bje + bjf;
                    u2 = bjb - u1 * .5;
                    u3 = c1 * (bje - bjf);
                    bjh = b[bOffs+jh + j];
                    bjf = bjh;
                    a[aOffs+jb + j] = ajb + t1;
                    b[bOffs+jb + j] = bjb + u1;
                    a[aOffs+je + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+je + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    a[aOffs+jh + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
                    b[bOffs+jh + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
                    // ---------------------- 
                    aji = a[aOffs+ji + j];
                    t1 = ajf + aji;
                    ajg = a[aOffs+jg + j];
                    t2 = ajg - t1 * .5;
                    t3 = c1 * (ajf - aji);
                    t1 = ajg + t1;
                    a[aOffs+jg + j] = ajc;
                    bji = b[bOffs+ji + j];
                    u1 = bjf + bji;
                    bjg = b[bOffs+jg + j];
                    u2 = bjg - u1 * .5;
                    u3 = c1 * (bjf - bji);
                    u1 = bjg + u1;
                    b[bOffs+jg + j] = bjc;
                    a[aOffs+jc + j] = t1;
                    b[bOffs+jc + j] = u1;
                    a[aOffs+jf + j] = co1 * (t2 - u3) - si1 * (u2 + t3);
                    b[bOffs+jf + j] = si1 * (t2 - u3) + co1 * (u2 + t3);
                    a[aOffs+ji + j] = co2 * (t2 + u3) - si2 * (u2 - t3);
                    b[bOffs+ji + j] = si2 * (t2 + u3) + co2 * (u2 - t3);
                    j += jump;
                  }
                  // -----(end of loop across transforms) 
                  ja += jstepx;
                  if (ja < istart) 
                  {
                    ja += ninc;
                  }
                }
              }
            }
            // -----( end of double loop for this k ) 
            kk += la << 1;
          }
          // -----( end of loop over values of k ) 
          la *= 3;
        }
        // -----( end of loop on type II radix-3 passes ) 
        // -----( nvex transforms completed) 
      L490:
        istart += nvex * jump;
      }
      // -----( end of loop on blocks of transforms )
    }

    static void gpfa5f    (double[] a, int aOffs, double[] b, int bOffs, double[] trigs, int trOffs, int inc, int jump, int n, int mm, int lot, int isign)
    {
      const double  sin36 = 0.587785252292473,
              sin72 = 0.951056516295154,
              qrt5  = 0.559016994374947;

      // *************************************************************** 
      // *                                                             * 
      // *  n.b. lvr = length of vector registers, set to 128 for c90. * 
      // *  reset to 64 for other cray machines, or to any large value * 
      // *  (greater than or equal to lot) for a scalar computer.      * 
      // *                                                             * 
      // *************************************************************** 

      // System generated locals 
      int i__1, i__2, i__3, i__4, i__5, i__6, i__7, i__8, i__9, i__10;

      double  s, c1, c2, c3, t1, t2, t3, t4, t5, t6, t7, t8, t9, u1, u2, u3, 
        u4, u5, u6, u7, u8, u9, t10, t11, u10, u11, ax, bx, 
        co1=0.0, co2=0.0, co3=0.0, co4=0.0, si1=0.0, si2=0.0, si3=0.0, si4=0.0, 
        aja, ajb, ajc, ajd, aje, bjb, bje, 
        bjc, bjd, bja, ajf, ajk, bjf, bjk, ajg, ajj, ajh, aji, ajl, ajq, 
        bjg, bjj, bjh, bji, bjl, bjq, ajo, ajm, ajn, ajr, ajw, bjo, bjm, 
        bjn, bjr, bjw, ajt, ajs, ajx, ajp, bjt, bjs, bjx, bjp, ajv, ajy, 
        aju, bjv, bjy, bju;

      int    ninc, left, nvex, j, k, l, m, ipass, nblox, jstep, n5, ja, jb, la, 
        jc, jd, nb, je, jf, jg, jh, mh, kk, ll, ji, jj, jk, jl, jm, mu, nu, 
        laincl, jn, jo, jp, jq, jr, js, jt, ju, jv, jw, jx, jy, jstepl,
        istart, jstepx, jjj, ink, inq;

      // Parameter adjustments 
      --trOffs;
      --bOffs;
      --aOffs;

      n5 = powii(5, mm);
      inq = n / n5;
      jstepx = (n5 - n) * inc;
      ninc = n * inc;
      ink = inc * inq;
      mu = inq % 5;
      if (isign == -1) mu = 5 - mu;

      m = mm;
      mh = (m + 1) / 2;
      s = (double) isign;
      c1 = qrt5;
      c2 = sin72;
      c3 = sin36;
      if (mu == 2 || mu == 3) 
      {
        c1 = -c1;
        c2 = sin36;
        c3 = sin72;
      }
      if (mu == 3 || mu == 4) c2 = -c2;
      if (mu == 2 || mu == 4) c3 = -c3;

      nblox = (lot - 1) / lvr + 1;
      left = lot;
      s = (double) isign;
      istart = 1;

      //  loop on blocks of lvr transforms 
      //  -------------------------------- 
      i__1 = nblox;
      for (nb = 1; nb <= i__1; ++nb) 
      {

        if (left <= lvr) 
        {
          nvex = left;
        } 
        else if (left < lvr << 1) 
        {
          nvex = left / 2;
          nvex += nvex % 2;
        } 
        else 
        {
          nvex = lvr;
        }
        left -= nvex;

        la = 1;

        //  loop on type I radix-5 passes 
        //  ----------------------------- 
        i__2 = mh;
        for (ipass = 1; ipass <= i__2; ++ipass) 
        {
          jstep = n * inc / (la * 5);
          jstepl = jstep - ninc;
          kk = 0;

          //  loop on k 
          //  --------- 
          i__3 = jstep - ink;
          i__4 = ink;
          for (k = 0; i__4 < 0 ? k >= i__3 : k <= i__3; k += i__4) 
          {

            if (k > 0) 
            {
              co1 = trigs[trOffs+kk + 1];
              si1 = s * trigs[trOffs+kk + 2];
              co2 = trigs[trOffs+(kk << 1) + 1];
              si2 = s * trigs[trOffs+(kk << 1) + 2];
              co3 = trigs[trOffs+kk * 3 + 1];
              si3 = s * trigs[trOffs+kk * 3 + 2];
              co4 = trigs[trOffs+(kk << 2) + 1];
              si4 = s * trigs[trOffs+(kk << 2) + 2];
            }

            //  loop along transform 
            //  -------------------- 
            i__5 = (n - 1) * inc;
            i__6 = jstep * 5;
            for (jjj = k; i__6 < 0 ? jjj >= i__5 : jjj <= i__5; jjj += i__6) 
            {
              ja = istart + jjj;

              //     "transverse" loop 
              //     ----------------- 
              i__7 = inq;
              for (nu = 1; nu <= i__7; ++nu) 
              {
                jb = ja + jstepl;
                if (jb < istart) 
                {
                  jb += ninc;
                }
                jc = jb + jstepl;
                if (jc < istart) 
                {
                  jc += ninc;
                }
                jd = jc + jstepl;
                if (jd < istart) 
                {
                  jd += ninc;
                }
                je = jd + jstepl;
                if (je < istart) 
                {
                  je += ninc;
                }
                j = 0;

                //  loop across transforms 
                //  ---------------------- 
                if (k == 0) 
                {

                  // dir$ ivdep, shortloop 
                  i__8 = nvex;
                  for (l = 1; l <= i__8; ++l) 
                  {
                    ajb = a[aOffs+jb + j];
                    aje = a[aOffs+je + j];
                    t1 = ajb + aje;
                    ajc = a[aOffs+jc + j];
                    ajd = a[aOffs+jd + j];
                    t2 = ajc + ajd;
                    t3 = ajb - aje;
                    t4 = ajc - ajd;
                    t5 = t1 + t2;
                    t6 = c1 * (t1 - t2);
                    aja = a[aOffs+ja + j];
                    t7 = aja - t5 * .25;
                    a[aOffs+ja + j] = aja + t5;
                    t8 = t7 + t6;
                    t9 = t7 - t6;
                    t10 = c3 * t3 - c2 * t4;
                    t11 = c2 * t3 + c3 * t4;
                    bjb = b[bOffs+jb + j];
                    bje = b[bOffs+je + j];
                    u1 = bjb + bje;
                    bjc = b[bOffs+jc + j];
                    bjd = b[bOffs+jd + j];
                    u2 = bjc + bjd;
                    u3 = bjb - bje;
                    u4 = bjc - bjd;
                    u5 = u1 + u2;
                    u6 = c1 * (u1 - u2);
                    bja = b[bOffs+ja + j];
                    u7 = bja - u5 * .25;
                    b[bOffs+ja + j] = bja + u5;
                    u8 = u7 + u6;
                    u9 = u7 - u6;
                    u10 = c3 * u3 - c2 * u4;
                    u11 = c2 * u3 + c3 * u4;
                    a[aOffs+jb + j] = t8 - u11;
                    b[bOffs+jb + j] = u8 + t11;
                    a[aOffs+je + j] = t8 + u11;
                    b[bOffs+je + j] = u8 - t11;
                    a[aOffs+jc + j] = t9 - u10;
                    b[bOffs+jc + j] = u9 + t10;
                    a[aOffs+jd + j] = t9 + u10;
                    b[bOffs+jd + j] = u9 - t10;
                    j += jump;
                  }

                } 
                else 
                {

                  // dir$ ivdep,shortloop 
                  i__8 = nvex;
                  for (l = 1; l <= i__8; ++l) 
                  {
                    ajb = a[aOffs+jb + j];
                    aje = a[aOffs+je + j];
                    t1 = ajb + aje;
                    ajc = a[aOffs+jc + j];
                    ajd = a[aOffs+jd + j];
                    t2 = ajc + ajd;
                    t3 = ajb - aje;
                    t4 = ajc - ajd;
                    t5 = t1 + t2;
                    t6 = c1 * (t1 - t2);
                    aja = a[aOffs+ja + j];
                    t7 = aja - t5 * .25;
                    a[aOffs+ja + j] = aja + t5;
                    t8 = t7 + t6;
                    t9 = t7 - t6;
                    t10 = c3 * t3 - c2 * t4;
                    t11 = c2 * t3 + c3 * t4;
                    bjb = b[bOffs+jb + j];
                    bje = b[bOffs+je + j];
                    u1 = bjb + bje;
                    bjc = b[bOffs+jc + j];
                    bjd = b[bOffs+jd + j];
                    u2 = bjc + bjd;
                    u3 = bjb - bje;
                    u4 = bjc - bjd;
                    u5 = u1 + u2;
                    u6 = c1 * (u1 - u2);
                    bja = b[bOffs+ja + j];
                    u7 = bja - u5 * .25;
                    b[bOffs+ja + j] = bja + u5;
                    u8 = u7 + u6;
                    u9 = u7 - u6;
                    u10 = c3 * u3 - c2 * u4;
                    u11 = c2 * u3 + c3 * u4;
                    a[aOffs+jb + j] = co1 * (t8 - u11) - si1 * (u8 + 
                      t11);
                    b[bOffs+jb + j] = si1 * (t8 - u11) + co1 * (u8 + 
                      t11);
                    a[aOffs+je + j] = co4 * (t8 + u11) - si4 * (u8 - 
                      t11);
                    b[bOffs+je + j] = si4 * (t8 + u11) + co4 * (u8 - 
                      t11);
                    a[aOffs+jc + j] = co2 * (t9 - u10) - si2 * (u9 + 
                      t10);
                    b[bOffs+jc + j] = si2 * (t9 - u10) + co2 * (u9 + 
                      t10);
                    a[aOffs+jd + j] = co3 * (t9 + u10) - si3 * (u9 - 
                      t10);
                    b[bOffs+jd + j] = si3 * (t9 + u10) + co3 * (u9 - 
                      t10);
                    j += jump;
                  }

                }

                // -----( end of loop across transforms ) 

                ja += jstepx;
                if (ja < istart) 
                {
                  ja += ninc;
                }
              }
            }
            // -----( end of loop along transforms ) 
            kk += la << 1;
          }
          // -----( end of loop on nonzero k ) 
          la *= 5;
        }
        // -----( end of loop on type I radix-5 passes) 

        if (n == 5) goto L490;

        //  loop on type II radix-5 passes 
        //  ------------------------------ 

        i__2 = m;
        for (ipass = mh + 1; ipass <= i__2; ++ipass) 
        {
          jstep = n * inc / (la * 5);
          jstepl = jstep - ninc;
          laincl = la * ink - ninc;
          kk = 0;

          //     loop on k 
          //     --------- 
          i__4 = jstep - ink;
          i__3 = ink;
          for (k = 0; i__3 < 0 ? k >= i__4 : k <= i__4; k += i__3) 
          {

            if (k > 0) 
            {
              co1 = trigs[trOffs+kk + 1];
              si1 = s * trigs[trOffs+kk + 2];
              co2 = trigs[trOffs+(kk << 1) + 1];
              si2 = s * trigs[trOffs+(kk << 1) + 2];
              co3 = trigs[trOffs+kk * 3 + 1];
              si3 = s * trigs[trOffs+kk * 3 + 2];
              co4 = trigs[trOffs+(kk << 2) + 1];
              si4 = s * trigs[trOffs+(kk << 2) + 2];
            }

            //  double loop along first transform in block 
            //  ------------------------------------------ 
            i__6 = (la - 1) * ink;
            i__5 = jstep * 5;
            for (ll = k; i__5 < 0 ? ll >= i__6 : ll <= i__6; ll += i__5) 
            {

              i__7 = (n - 1) * inc;
              i__8 = la * 5 * ink;
              for (jjj = ll; i__8 < 0 ? jjj >= i__7 : jjj <= i__7; jjj += i__8) 
              {
                ja = istart + jjj;

                //     "transverse" loop 
                //     ----------------- 
                i__9 = inq;
                for (nu = 1; nu <= i__9; ++nu) 
                {
                  jb = ja + jstepl;
                  if (jb < istart) 
                  {
                    jb += ninc;
                  }
                  jc = jb + jstepl;
                  if (jc < istart) 
                  {
                    jc += ninc;
                  }
                  jd = jc + jstepl;
                  if (jd < istart) 
                  {
                    jd += ninc;
                  }
                  je = jd + jstepl;
                  if (je < istart) 
                  {
                    je += ninc;
                  }
                  jf = ja + laincl;
                  if (jf < istart) 
                  {
                    jf += ninc;
                  }
                  jg = jf + jstepl;
                  if (jg < istart) 
                  {
                    jg += ninc;
                  }
                  jh = jg + jstepl;
                  if (jh < istart) 
                  {
                    jh += ninc;
                  }
                  ji = jh + jstepl;
                  if (ji < istart) 
                  {
                    ji += ninc;
                  }
                  jj = ji + jstepl;
                  if (jj < istart) 
                  {
                    jj += ninc;
                  }
                  jk = jf + laincl;
                  if (jk < istart) 
                  {
                    jk += ninc;
                  }
                  jl = jk + jstepl;
                  if (jl < istart) 
                  {
                    jl += ninc;
                  }
                  jm = jl + jstepl;
                  if (jm < istart) 
                  {
                    jm += ninc;
                  }
                  jn = jm + jstepl;
                  if (jn < istart) 
                  {
                    jn += ninc;
                  }
                  jo = jn + jstepl;
                  if (jo < istart) 
                  {
                    jo += ninc;
                  }
                  jp = jk + laincl;
                  if (jp < istart) 
                  {
                    jp += ninc;
                  }
                  jq = jp + jstepl;
                  if (jq < istart) 
                  {
                    jq += ninc;
                  }
                  jr = jq + jstepl;
                  if (jr < istart) 
                  {
                    jr += ninc;
                  }
                  js = jr + jstepl;
                  if (js < istart) 
                  {
                    js += ninc;
                  }
                  jt = js + jstepl;
                  if (jt < istart) 
                  {
                    jt += ninc;
                  }
                  ju = jp + laincl;
                  if (ju < istart) 
                  {
                    ju += ninc;
                  }
                  jv = ju + jstepl;
                  if (jv < istart) 
                  {
                    jv += ninc;
                  }
                  jw = jv + jstepl;
                  if (jw < istart) 
                  {
                    jw += ninc;
                  }
                  jx = jw + jstepl;
                  if (jx < istart) 
                  {
                    jx += ninc;
                  }
                  jy = jx + jstepl;
                  if (jy < istart) 
                  {
                    jy += ninc;
                  }
                  j = 0;

                  //  loop across transforms 
                  //  ---------------------- 
                  if (k == 0) 
                  {

                    // dir$ ivdep, shortloop 
                    i__10 = nvex;
                    for (l = 1; l <= i__10; ++l) 
                    {
                      ajb = a[aOffs+jb + j];
                      aje = a[aOffs+je + j];
                      t1 = ajb + aje;
                      ajc = a[aOffs+jc + j];
                      ajd = a[aOffs+jd + j];
                      t2 = ajc + ajd;
                      t3 = ajb - aje;
                      t4 = ajc - ajd;
                      ajf = a[aOffs+jf + j];
                      ajb = ajf;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      aja = a[aOffs+ja + j];
                      t7 = aja - t5 * .25;
                      a[aOffs+ja + j] = aja + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajk = a[aOffs+jk + j];
                      ajc = ajk;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjb = b[bOffs+jb + j];
                      bje = b[bOffs+je + j];
                      u1 = bjb + bje;
                      bjc = b[bOffs+jc + j];
                      bjd = b[bOffs+jd + j];
                      u2 = bjc + bjd;
                      u3 = bjb - bje;
                      u4 = bjc - bjd;
                      bjf = b[bOffs+jf + j];
                      bjb = bjf;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bja = b[bOffs+ja + j];
                      u7 = bja - u5 * .25;
                      b[bOffs+ja + j] = bja + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjk = b[bOffs+jk + j];
                      bjc = bjk;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jf + j] = t8 - u11;
                      b[bOffs+jf + j] = u8 + t11;
                      aje = t8 + u11;
                      bje = u8 - t11;
                      a[aOffs+jk + j] = t9 - u10;
                      b[bOffs+jk + j] = u9 + t10;
                      ajd = t9 + u10;
                      bjd = u9 - t10;
                      // ---------------------- 
                      ajg = a[aOffs+jg + j];
                      ajj = a[aOffs+jj + j];
                      t1 = ajg + ajj;
                      ajh = a[aOffs+jh + j];
                      aji = a[aOffs+ji + j];
                      t2 = ajh + aji;
                      t3 = ajg - ajj;
                      t4 = ajh - aji;
                      ajl = a[aOffs+jl + j];
                      ajh = ajl;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      t7 = ajb - t5 * .25;
                      a[aOffs+jb + j] = ajb + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajq = a[aOffs+jq + j];
                      aji = ajq;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjg = b[bOffs+jg + j];
                      bjj = b[bOffs+jj + j];
                      u1 = bjg + bjj;
                      bjh = b[bOffs+jh + j];
                      bji = b[bOffs+ji + j];
                      u2 = bjh + bji;
                      u3 = bjg - bjj;
                      u4 = bjh - bji;
                      bjl = b[bOffs+jl + j];
                      bjh = bjl;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      u7 = bjb - u5 * .25;
                      b[bOffs+jb + j] = bjb + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjq = b[bOffs+jq + j];
                      bji = bjq;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jg + j] = t8 - u11;
                      b[bOffs+jg + j] = u8 + t11;
                      ajj = t8 + u11;
                      bjj = u8 - t11;
                      a[aOffs+jl + j] = t9 - u10;
                      b[bOffs+jl + j] = u9 + t10;
                      a[aOffs+jq + j] = t9 + u10;
                      b[bOffs+jq + j] = u9 - t10;
                      // ---------------------- 
                      ajo = a[aOffs+jo + j];
                      t1 = ajh + ajo;
                      ajm = a[aOffs+jm + j];
                      ajn = a[aOffs+jn + j];
                      t2 = ajm + ajn;
                      t3 = ajh - ajo;
                      t4 = ajm - ajn;
                      ajr = a[aOffs+jr + j];
                      ajn = ajr;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      t7 = ajc - t5 * .25;
                      a[aOffs+jc + j] = ajc + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajw = a[aOffs+jw + j];
                      ajo = ajw;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjo = b[bOffs+jo + j];
                      u1 = bjh + bjo;
                      bjm = b[bOffs+jm + j];
                      bjn = b[bOffs+jn + j];
                      u2 = bjm + bjn;
                      u3 = bjh - bjo;
                      u4 = bjm - bjn;
                      bjr = b[bOffs+jr + j];
                      bjn = bjr;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      u7 = bjc - u5 * .25;
                      b[bOffs+jc + j] = bjc + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjw = b[bOffs+jw + j];
                      bjo = bjw;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jh + j] = t8 - u11;
                      b[bOffs+jh + j] = u8 + t11;
                      a[aOffs+jw + j] = t8 + u11;
                      b[bOffs+jw + j] = u8 - t11;
                      a[aOffs+jm + j] = t9 - u10;
                      b[bOffs+jm + j] = u9 + t10;
                      a[aOffs+jr + j] = t9 + u10;
                      b[bOffs+jr + j] = u9 - t10;
                      // ---------------------- 
                      ajt = a[aOffs+jt + j];
                      t1 = aji + ajt;
                      ajs = a[aOffs+js + j];
                      t2 = ajn + ajs;
                      t3 = aji - ajt;
                      t4 = ajn - ajs;
                      ajx = a[aOffs+jx + j];
                      ajt = ajx;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      ajp = a[aOffs+jp + j];
                      t7 = ajp - t5 * .25;
                      ax = ajp + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      a[aOffs+jp + j] = ajd;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      a[aOffs+jd + j] = ax;
                      bjt = b[bOffs+jt + j];
                      u1 = bji + bjt;
                      bjs = b[bOffs+js + j];
                      u2 = bjn + bjs;
                      u3 = bji - bjt;
                      u4 = bjn - bjs;
                      bjx = b[bOffs+jx + j];
                      bjt = bjx;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bjp = b[bOffs+jp + j];
                      u7 = bjp - u5 * .25;
                      bx = bjp + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      b[bOffs+jp + j] = bjd;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      b[bOffs+jd + j] = bx;
                      a[aOffs+ji + j] = t8 - u11;
                      b[bOffs+ji + j] = u8 + t11;
                      a[aOffs+jx + j] = t8 + u11;
                      b[bOffs+jx + j] = u8 - t11;
                      a[aOffs+jn + j] = t9 - u10;
                      b[bOffs+jn + j] = u9 + t10;
                      a[aOffs+js + j] = t9 + u10;
                      b[bOffs+js + j] = u9 - t10;
                      // ---------------------- 
                      ajv = a[aOffs+jv + j];
                      ajy = a[aOffs+jy + j];
                      t1 = ajv + ajy;
                      t2 = ajo + ajt;
                      t3 = ajv - ajy;
                      t4 = ajo - ajt;
                      a[aOffs+jv + j] = ajj;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      aju = a[aOffs+ju + j];
                      t7 = aju - t5 * .25;
                      ax = aju + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      a[aOffs+ju + j] = aje;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      a[aOffs+je + j] = ax;
                      bjv = b[bOffs+jv + j];
                      bjy = b[bOffs+jy + j];
                      u1 = bjv + bjy;
                      u2 = bjo + bjt;
                      u3 = bjv - bjy;
                      u4 = bjo - bjt;
                      b[bOffs+jv + j] = bjj;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bju = b[bOffs+ju + j];
                      u7 = bju - u5 * .25;
                      bx = bju + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      b[bOffs+ju + j] = bje;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      b[bOffs+je + j] = bx;
                      a[aOffs+jj + j] = t8 - u11;
                      b[bOffs+jj + j] = u8 + t11;
                      a[aOffs+jy + j] = t8 + u11;
                      b[bOffs+jy + j] = u8 - t11;
                      a[aOffs+jo + j] = t9 - u10;
                      b[bOffs+jo + j] = u9 + t10;
                      a[aOffs+jt + j] = t9 + u10;
                      b[bOffs+jt + j] = u9 - t10;
                      j += jump;
                    }

                  } 
                  else 
                  {

                    // dir$ ivdep, shortloop 
                    i__10 = nvex;
                    for (l = 1; l <= i__10; ++l) 
                    {
                      ajb = a[aOffs+jb + j];
                      aje = a[aOffs+je + j];
                      t1 = ajb + aje;
                      ajc = a[aOffs+jc + j];
                      ajd = a[aOffs+jd + j];
                      t2 = ajc + ajd;
                      t3 = ajb - aje;
                      t4 = ajc - ajd;
                      ajf = a[aOffs+jf + j];
                      ajb = ajf;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      aja = a[aOffs+ja + j];
                      t7 = aja - t5 * .25;
                      a[aOffs+ja + j] = aja + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajk = a[aOffs+jk + j];
                      ajc = ajk;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjb = b[bOffs+jb + j];
                      bje = b[bOffs+je + j];
                      u1 = bjb + bje;
                      bjc = b[bOffs+jc + j];
                      bjd = b[bOffs+jd + j];
                      u2 = bjc + bjd;
                      u3 = bjb - bje;
                      u4 = bjc - bjd;
                      bjf = b[bOffs+jf + j];
                      bjb = bjf;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bja = b[bOffs+ja + j];
                      u7 = bja - u5 * .25;
                      b[bOffs+ja + j] = bja + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjk = b[bOffs+jk + j];
                      bjc = bjk;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jf + j] = co1 * (t8 - u11) - si1 * (u8 
                        + t11);
                      b[bOffs+jf + j] = si1 * (t8 - u11) + co1 * (u8 
                        + t11);
                      aje = co4 * (t8 + u11) - si4 * (u8 - t11);
                      bje = si4 * (t8 + u11) + co4 * (u8 - t11);
                      a[aOffs+jk + j] = co2 * (t9 - u10) - si2 * (u9 
                        + t10);
                      b[bOffs+jk + j] = si2 * (t9 - u10) + co2 * (u9 
                        + t10);
                      ajd = co3 * (t9 + u10) - si3 * (u9 - t10);
                      bjd = si3 * (t9 + u10) + co3 * (u9 - t10);
                      // ---------------------- 
                      ajg = a[aOffs+jg + j];
                      ajj = a[aOffs+jj + j];
                      t1 = ajg + ajj;
                      ajh = a[aOffs+jh + j];
                      aji = a[aOffs+ji + j];
                      t2 = ajh + aji;
                      t3 = ajg - ajj;
                      t4 = ajh - aji;
                      ajl = a[aOffs+jl + j];
                      ajh = ajl;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      t7 = ajb - t5 * .25;
                      a[aOffs+jb + j] = ajb + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajq = a[aOffs+jq + j];
                      aji = ajq;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjg = b[bOffs+jg + j];
                      bjj = b[bOffs+jj + j];
                      u1 = bjg + bjj;
                      bjh = b[bOffs+jh + j];
                      bji = b[bOffs+ji + j];
                      u2 = bjh + bji;
                      u3 = bjg - bjj;
                      u4 = bjh - bji;
                      bjl = b[bOffs+jl + j];
                      bjh = bjl;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      u7 = bjb - u5 * .25;
                      b[bOffs+jb + j] = bjb + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjq = b[bOffs+jq + j];
                      bji = bjq;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jg + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
                      b[bOffs+jg + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
                      ajj = co4 * (t8 + u11) - si4 * (u8 - t11);
                      bjj = si4 * (t8 + u11) + co4 * (u8 - t11);
                      a[aOffs+jl + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
                      b[bOffs+jl + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
                      a[aOffs+jq + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
                      b[bOffs+jq + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
                      // ---------------------- 
                      ajo = a[aOffs+jo + j];
                      t1 = ajh + ajo;
                      ajm = a[aOffs+jm + j];
                      ajn = a[aOffs+jn + j];
                      t2 = ajm + ajn;
                      t3 = ajh - ajo;
                      t4 = ajm - ajn;
                      ajr = a[aOffs+jr + j];
                      ajn = ajr;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      t7 = ajc - t5 * .25;
                      a[aOffs+jc + j] = ajc + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      ajw = a[aOffs+jw + j];
                      ajo = ajw;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      bjo = b[bOffs+jo + j];
                      u1 = bjh + bjo;
                      bjm = b[bOffs+jm + j];
                      bjn = b[bOffs+jn + j];
                      u2 = bjm + bjn;
                      u3 = bjh - bjo;
                      u4 = bjm - bjn;
                      bjr = b[bOffs+jr + j];
                      bjn = bjr;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      u7 = bjc - u5 * .25;
                      b[bOffs+jc + j] = bjc + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      bjw = b[bOffs+jw + j];
                      bjo = bjw;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      a[aOffs+jh + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
                      b[bOffs+jh + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
                      a[aOffs+jw + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
                      b[bOffs+jw + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
                      a[aOffs+jm + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
                      b[bOffs+jm + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
                      a[aOffs+jr + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
                      b[bOffs+jr + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
                      // ---------------------- 
                      ajt = a[aOffs+jt + j];
                      t1 = aji + ajt;
                      ajs = a[aOffs+js + j];
                      t2 = ajn + ajs;
                      t3 = aji - ajt;
                      t4 = ajn - ajs;
                      ajx = a[aOffs+jx + j];
                      ajt = ajx;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      ajp = a[aOffs+jp + j];
                      t7 = ajp - t5 * .25;
                      ax = ajp + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      a[aOffs+jp + j] = ajd;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      a[aOffs+jd + j] = ax;
                      bjt = b[bOffs+jt + j];
                      u1 = bji + bjt;
                      bjs = b[bOffs+js + j];
                      u2 = bjn + bjs;
                      u3 = bji - bjt;
                      u4 = bjn - bjs;
                      bjx = b[bOffs+jx + j];
                      bjt = bjx;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bjp = b[bOffs+jp + j];
                      u7 = bjp - u5 * .25;
                      bx = bjp + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      b[bOffs+jp + j] = bjd;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      b[bOffs+jd + j] = bx;
                      a[aOffs+ji + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
                      b[bOffs+ji + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
                      a[aOffs+jx + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
                      b[bOffs+jx + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
                      a[aOffs+jn + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
                      b[bOffs+jn + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
                      a[aOffs+js + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
                      b[bOffs+js + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
                      // ---------------------- 
                      ajv = a[aOffs+jv + j];
                      ajy = a[aOffs+jy + j];
                      t1 = ajv + ajy;
                      t2 = ajo + ajt;
                      t3 = ajv - ajy;
                      t4 = ajo - ajt;
                      a[aOffs+jv + j] = ajj;
                      t5 = t1 + t2;
                      t6 = c1 * (t1 - t2);
                      aju = a[aOffs+ju + j];
                      t7 = aju - t5 * .25;
                      ax = aju + t5;
                      t8 = t7 + t6;
                      t9 = t7 - t6;
                      a[aOffs+ju + j] = aje;
                      t10 = c3 * t3 - c2 * t4;
                      t11 = c2 * t3 + c3 * t4;
                      a[aOffs+je + j] = ax;
                      bjv = b[bOffs+jv + j];
                      bjy = b[bOffs+jy + j];
                      u1 = bjv + bjy;
                      u2 = bjo + bjt;
                      u3 = bjv - bjy;
                      u4 = bjo - bjt;
                      b[bOffs+jv + j] = bjj;
                      u5 = u1 + u2;
                      u6 = c1 * (u1 - u2);
                      bju = b[bOffs+ju + j];
                      u7 = bju - u5 * .25;
                      bx = bju + u5;
                      u8 = u7 + u6;
                      u9 = u7 - u6;
                      b[bOffs+ju + j] = bje;
                      u10 = c3 * u3 - c2 * u4;
                      u11 = c2 * u3 + c3 * u4;
                      b[bOffs+je + j] = bx;
                      a[aOffs+jj + j] = co1 * (t8 - u11) - si1 * (u8 + t11);
                      b[bOffs+jj + j] = si1 * (t8 - u11) + co1 * (u8 + t11);
                      a[aOffs+jy + j] = co4 * (t8 + u11) - si4 * (u8 - t11);
                      b[bOffs+jy + j] = si4 * (t8 + u11) + co4 * (u8 - t11);
                      a[aOffs+jo + j] = co2 * (t9 - u10) - si2 * (u9 + t10);
                      b[bOffs+jo + j] = si2 * (t9 - u10) + co2 * (u9 + t10);
                      a[aOffs+jt + j] = co3 * (t9 + u10) - si3 * (u9 - t10);
                      b[bOffs+jt + j] = si3 * (t9 + u10) + co3 * (u9 - t10);
                      j += jump;
                    }

                  }

                  // -----(end of loop across transforms) 

                  ja += jstepx;
                  if (ja < istart) 
                  {
                    ja += ninc;
                  }
                }
              }
            }
            // -----( end of double loop for this k ) 
            kk += la << 1;
          }
          // -----( end of loop over values of k ) 
          la *= 5;
        }
        // -----( end of loop on type II radix-5 passes ) 
        // -----( nvex transforms completed) 
      L490:
        istart += nvex * jump;
      }
      // -----( end of loop on blocks of transforms )

    }

    //-----------------------------------------------------------------------------//
    //
    //    gpfasetup(trigs,n)
    //
    //        setup routine for self-sorting in-place 
    //        generalized prime factor (complex) fft
    //
    //    input : 
    //    ----- 
    //        n is the length of the transforms. n must be of the form: 
    //          ----------------------------------- 
    //            n = (2**ip) * (3**iq) * (5**ir) 
    //          ----------------------------------- 
    //
    //    output: 
    //    ------ 
    //        trigs is a table of twiddle factors, 
    //          of length 2*pqr (real) words, where: 
    //          -------------------------------------- 
    //            pqr = (2**ip) + (3**iq) + (5**ir) 
    //          -------------------------------------- 
    //
    //    Written by Clive Temperton 1990.
    //
    //-----------------------------------------------------------------------------//
    static void gpfasetup (double[] trigs, int trOffs, int n)
    {
      int ifac, kink, irot, i, k, kk, ni,  ll, ip, iq, nn, ir;
      int[] nj = new int[3];
      double angle, del;

      const double twopi = 2*Math.PI;

      // Decompose n into factors 2,3,5 
      // ------------------------------ 

      // Parameter adjustments 
      --trOffs;

      nn = n;
      ifac = 2;

      for (ll = 1; ll <= 3; ++ll) 
      {
        kk = 0;
      L10:
        if (nn % ifac != 0) goto L20;
        ++kk;
        nn /= ifac;
        goto L10;
      L20:
        nj[ll - 1] = kk;
        ifac += ll;
      }

      if (nn != 1) 
      {
        throw new ArithmeticException(string.Format("gpfasetup: {0} is not a legal value of n",n));
      }

      ip = nj[0];
      iq = nj[1];
      ir = nj[2];

      // Compute list of rotated twiddle factors 
      // ---------------------------------------

      nj[0] = powii(2,ip);
      nj[1] = powii(3,iq);
      nj[2] = powii(5,ir);

      i = 1;
      for (ll = 1; ll <= 3; ++ll) 
      {
        ni = nj[ll - 1];
        if (ni != 1) 
        {
          del = twopi / (double) ni;
          irot = n / ni;
          kink = irot % ni;
          kk = 0;

          for (k = 1; k <= ni; ++k) 
          {
            angle = (double) kk * del;
            trigs[trOffs+i] = Math.Cos(angle);
            trigs[trOffs+i + 1] = Math.Sin(angle);
            i += 2;
            kk += kink;
            if (kk > ni) kk -= ni;
          }
        }
      }
    }

    //-----------------------------------------------------------------------------//
    //
    // gpfa: self-sorting in-place generalized prime factor (complex) fft 
    //
    // Definition:
    // -----------
    //        x(j) = sum(k=0,...,n-1) ( c(k) * exp(isign*2*i*j*k*pi/n) ) 
    //
    // Prototype:
    // ----------
    //   void gpfa (FLOAT a[], FLOAT b[], FLOAT trigs[], 
    //              int inc, int jump, int n, int lot, int isign)
    //
    // Arguments:
    // ----------
    //   a      is first real input/output vector. The first element is 
    //          indexed by a[0].
    //
    //   b      is first imaginary input/output vector. The first element is 
    //          indexed by b[0].
    //
    //   trigs  is a table of twiddle factors, precalculated 
    //          by calling function 'gpfasetup'. The first element is 
    //          indexed by trigs[0].
    //
    //   inc    is the increment within each data vector 
    //
    //   jump   is the increment between data vectors 
    //
    //   n      is the length of the transforms which must be of the form
    //          n = (2**ip) * (3**iq) * (5**ir) 
    //
    //   lot    is the number of transforms 
    //
    //   isign  = +1 for forward transform 
    //          = -1 for inverse transform 
    //
    // References:
    // ----------- 
    //  1) Originally written by:
    //
    //           Clive Temperton 
    //           Recherche en Prevision Numerique 
    //           Atmospheric Environment Service, Canada 
    //
    //   2) For a mathematical development of the algorithm used, see: 
    //
    //            C. Temperton, "A Generalized Prime Factor FFT Algorithm for 
    //            any n = (2**p)(3**q)(5**r)", SIAM J.Sci.Stat.Comp., May 1992. 
    //
    //  3) Conversion to C++ based on Clive Tempertons original algorithm 
    //     by B. M. Gammel, Jan 1997. Worked over the whole code.
    //
    //  4) The original Fortran source files can be obtained freely from
    //     ftp://ftp.earth.ox.ac.uc/pub/gpfa.tar.gz
    //
    //-----------------------------------------------------------------------------//
    // the basic gpfa routine can also be used directly
    static void gpfa      (double[] a, int aOffs, double[] b, int bOffs, double[] trigs, int trOffs, int inc, int jump, int n, int lot, int isign)
    {
      int i,kk;
      int[] nj = new int[3];

      // adjust arguments
      --trOffs;
      --bOffs;
      --aOffs;

      // decompose n into factors 2,3,5 
      // ------------------------------ 

      int nn = n,
        ifac = 2;

      for (int ll = 1; ll <= 3; ++ll) 
      {
        kk = 0;
      L10:
        if (nn % ifac != 0) goto L20;
        ++kk;
        nn /= ifac;
        goto L10;
      L20:
        nj[ll - 1] = kk;
        ifac += ll;
      }

      // test arguments
      // --------------

      if (nn != 1) 
      {
        throw new ArithmeticException(string.Format("gpfa: {0} is not a legal value of n", n)); 
      }

      if (isign != 1 && isign != -1) 
      {
        throw new ArithmeticException(string.Format("gpfa: {0} is not a legal value of isign = +1/-1", isign)); 
        
      }

      int ip = nj[0], 
        iq = nj[1],
        ir = nj[2];

      // compute the transform
      // ---------------------

      i = 1;
      if (ip > 0) 
      {
        gpfa2f (a, 1+aOffs, b, 1+bOffs, trigs, 1+trOffs, inc, jump, n, ip, lot, isign);
        i += powii(2,ip) * 2;
      }
      if (iq > 0) 
      {
        gpfa3f (a, 1+aOffs, b, 1+bOffs, trigs, i+trOffs, inc, jump, n, iq, lot, isign);
        i += powii(3,iq) * 2;
      }
      if (ir > 0) 
      {
        gpfa5f (a, 1+aOffs, b, 1+bOffs, trigs, i+trOffs, inc, jump, n, ir, lot, isign);
      }
    }

    #endregion


  }
}
