#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

// The following code was translated using Matpack sources (http://www.matpack.de) (Author B.Gammel)


using System;

namespace Altaxo.Calc.Fourier
{
  //-----------------------------------------------------------------------------//
  // template class MpConvolution:
  //
  // Short Description:
  // ------------------
  //  This class is used to convolve or deconvolve a real-valued data set (including 
  //  any user supplied zero padding) with a response function. All arrays must
  //  have the same dimensions. The data set (and of course the other arrays) 
  //  can be either one-dimensional, two-dimensional, or three-dimensional, 
  //  i.e. d = 1,2,3.  Each dimension must be of the form n = (2**p)*(3**q)*(5**r), 
  //  because of the underlying FFT. The d-dimensional data can be either single 
  //  precision (FLOAT := float) or double precision (FLOAT := double).
  //  This class is derived from class MpFFT, thus it owns all member functions
  //  of class MpFFT.
  //
  // Definition:
  // -----------
  //
  //   *--------------------------------------------------*
  //   | MpConvolution<FLOAT> (void)                      |  
  //   | MpConvolution<FLOAT> (int n1)                    |  
  //   | MpConvolution<FLOAT> (int n1, int n2)            |  
  //   | MpConvolution<FLOAT> (int n1, int n2, int n3)    |  
  //   *--------------------------------------------------*
  //
  //      "FLOAT" is to be replaced by either "float" or "double".
  //  Setup convolution/deconvolution for one, two or three 
  //  dimensions. The dimensions n1,n2,and n3 must be of the form
  //               n = (2**p) * (3**q) * (5**r)
  //  otherwise an error will be generated and the error handler function
  //  Matpack.Error() is called. On instantiation the underlying class MpFFT
  //      will allocate and calculate some trigonometric tables (cf. MpFFT above).
  //  
  // Convolution Functions:
  // ----------------------
  //
  //   *-------------------------------------------------------------------------*
  //   | int MpConvolution<FLOAT>::operator () (FLOAT data[], FLOAT response[],  |
  //   |                        FLOAT result[],                  |
  //   |                        FLOAT scratch[] = 0,             |
  //   |                      int isign = forward)             |
  //   *-------------------------------------------------------------------------*
  //
  //     Description
  //     -----------
  //     Convolves or deconvolves a real-valued data set data[] (including any
  //     user supplied zero padding) with a response function response[].
  //     The result is returned in the array result[]. All arrays including
  //     the scratch[] array must have the same dimensions (or larger).
  //     The data set (and of course the other arrays) can be either one-dimensional,
  //     two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
  //     of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
  //     The d-dimensional data can be either single precision (FLOAT := float) 
  //     or double precision (FLOAT := double).
  //
  //     Arguments
  //     ---------
  //     FLOAT data[]          The real-valued data set. Note, that you have to
  //                           care for end effects by zero padding. This means, 
  //                           that you have to pad the data with a number of zeros
  //                           on one end equal to the maximal positive duration
  //                           or maximal negative duration of the response function,
  //                           whichever is larger!!
  //
  //     FLOAT response[]      The response function must be stored in wrap-around
  //                           order. This means that the first half of the array
  //                           response[] (in each dimension) contains the impulse
  //                           response function at positive times, while the second
  //                           half of the array contains the impulse response
  //                           function at negative times, counting down from the
  //                           element with the highest index. The array must have 
  //                           at least the size of the data array.
  //
  //     FLOAT result[]        The result array. It must have 
  //                           at least the size of the data array.
  //
  //     FLOAT scratch[]       A work array. If a NULL pointer is passed the
  //           work array is allocated and freed auotomatically.
  //                           If the array is given by the user it must have 
  //                           at least the size of the data array.
  //
  //     int isign = forward   If isign == forward a convolution is performed. 
  //                           If isign == inverse then a deconvolution is performed.
  //
  //     Return values
  //     -------------
  //     In the case of a convolution (isign == forward) the value "true" is returned
  //     always. In the case of deconvolution (isign == inverse) the value "false" is
  //     returned if the FFT transform of the response function is exactly zero for 
  //     some value. This indicates that the original convolution has lost all 
  //     information at this particular frequency, so that a reconstruction is not
  //     possible. If the transform of the response function is non-zero everywhere
  //     the deconvolution can be performed and the value "true" is returned.
  //
  //-----------------------------------------------------------------------------//


  /// <summary>
  ///     Convolves or deconvolves a real-valued data set data[] (including any
  ///     user supplied zero padding) with a response function response[].
  ///     The result is returned in the array result[]. All arrays including
  ///     the scratch[] array must have the same dimensions (or larger).
  ///     The data set (and of course the other arrays) can be either one-dimensional,
  ///     two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
  ///     of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
  ///     The d-dimensional data can be either single precision (FLOAT := float) 
  ///     or double precision (FLOAT := double).  
  /// </summary>
  public class Pfa235Convolution: Pfa235FFT 
  {

    /// <summary>
    /// Uninitialized setup.
    /// </summary>
    public Pfa235Convolution()
      : base()
    {
    }

    /// <summary>
    /// 1-dimensional setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for one 
    /// dimensions. The dimension n1 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// </remarks>
    public Pfa235Convolution (int n1) 
      : base(n1)
    {
    }

    /// <summary>
    /// 2-dimensional setup.
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for two  
    /// dimensions. The dimensions n1 andn2 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// </remarks>
  
    public Pfa235Convolution(int n1, int n2)
      : base(n1,n2)
    {
    }

    /// <summary>
    /// 3-dimensional setup
    /// </summary>
    /// <remarks>
    /// Setup fast Fourier transform / back-transform for three 
    /// dimensions. The dimensions n1,n2,and n3 must be of the form
    ///              n = (2**p) * (3**q) * (5**r)
    /// </remarks>
    public Pfa235Convolution(int n1, int n2, int n3)
      : base(n1, n2, n3)
    {
    }

    /// <summary>
    /// Convolves or deconvolves a real-valued data set data[] (including any
    /// user supplied zero padding) with a response function response[].
    /// The result is returned in the array result[]. All arrays including
    /// the scratch[] array must have the same dimensions (or larger).
    /// The data set (and of course the other arrays) can be either one-dimensional,
    /// two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
    /// of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
    /// The d-dimensional data can be either single precision (FLOAT := float) 
    /// or double precision (FLOAT := double).    /// </summary>
    /// <param name="data">
    ///The real-valued data set. Note, that you have to
    ///                           care for end effects by zero padding. This means, 
    ///                           that you have to pad the data with a number of zeros
    ///                           on one end equal to the maximal positive duration
    ///                           or maximal negative duration of the response function,
    ///                           whichever is larger!!   /// </param>
    /// <param name="response">
    ///  The response function must be stored in wrap-around
    ///  order. This means that the first half of the array
    ///  response[] (in each dimension) contains the impulse
    ///  response function at positive times, while the second
    ///  half of the array contains the impulse response
    ///  function at negative times, counting down from the
    ///  element with the highest index. The array must have 
    ///  at least the size of the data array.
    /// </param>
    /// <param name="result">
    /// The result array. It must have 
    /// at least the size of the data array.
    /// </param>
    /// <param name="scratch">
    ///  A work array. If a NULL pointer is passed the
    /// work array is allocated and freed auotomatically.
    /// If the array is given by the user it must have 
    /// at least the size of the data array.
    /// </param>
    /// <param name="isign">
    /// If isign == forward a convolution is performed. 
    /// If isign == inverse then a deconvolution is performed.
    /// </param>
    /// <returns>
    /// In the case of a convolution (isign == forward) the value "true" is returned
    /// always. In the case of deconvolution (isign == inverse) the value "false" is
    /// returned if the FFT transform of the response function is exactly zero for 
    /// some value. This indicates that the original convolution has lost all 
    /// information at this particular frequency, so that a reconstruction is not
    /// possible. If the transform of the response function is non-zero everywhere
    /// the deconvolution can be performed and the value "true" is returned.
    ///</returns>
    ///<remarks>
    /// Implementation notes
    /// --------------------
    /// The FFT of the real-valued data array and the real-valued response array is
    /// calculated in one step. This is done by regarding the two arrays
    /// as the real part and the imaginary part of one complex-valued array.
    /// 
    /// Possible improvements
    /// ---------------------
    /// * When doing the backtransform only a real transform is necessary.
    ///   The upper half of the result/scratch arrays is redundant.
    ///   (comment: "symmetry"). This should be used to speed-up the backtransform.
    ///
    /// * 2D and 3D versions are not yet available !!!
    ///</remarks>
    public bool Convolute (double[] data, double[] response, double[] result, double[] scratch, FourierDirection isign)
    {
      // return status
      bool status = true;

      // get total size of data array
      int size = 0;
      if (ndim == 0) 
      {
        throw new ArithmeticException("MpConvolution::Convolute no dimensions have been specified");
      } 
      else if (ndim == 1) 
      {
        size = dim[0];
      } 
      else if (ndim == 2) 
      {
        size = row_order ? (dim[0] * id) : (id * dim[1]);
      } 
      else if (ndim == 3) 
      {
        size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
      }

      // allocate the scratch array
      //bool auto_scratch = false;
      if ( null==scratch ) 
      {
        scratch = new  double[size];
        //auto_scratch = true;
      }

      //---------------------------------------------------------------------------//
      //  1-dimensional convolution (original data not are overwritten)
      //---------------------------------------------------------------------------//

      if (ndim == 1) 
      {

        // First copy the arrays data and response to result and scratch,
        // respectively, to prevent overwriting of the original data.
        Array.Copy(data,result,size);
        Array.Copy(response,scratch, size);

        // transform both arrays simultaneously - this is a forward FFT
        base.FFT(result,scratch, FourierDirection.Forward);
    
        // multiply FFTs to convolve
        int n = dim[0], n2 = n/2;

        if (isign == FourierDirection.Forward) 
        {
      
          double scale = 0.25/n;
          result[0] *= scratch[0] / n;
          scratch[0] = 0;
          for (int i = 1; i <= n2; i++) 
          {
            double rr = result[i]  + result[n-i], 
              ri = result[i]  - result[n-i], 
              sr = scratch[i] + scratch[n-i],
              si = scratch[i] - scratch[n-i];
            result[i]  = scale * (rr*sr + ri*si);   // real part
            scratch[i] = scale * (si*sr - ri*rr);   // imaginary part 
            result[n-i]  = result[i];     // symmetry
            scratch[n-i] = -scratch[i];       // symmetry
          }
      
        } 
        else /* isign == inverse */ 
        {

          double mag;
          if ((mag = Square(scratch[0])) == 0.0) 
          {   // check for zero divide
            status = false;
            goto ErrorExit;
          }
          result[0] *= scratch[0] / mag / n;
          scratch[0] = 0;
          for (int i = 1; i <= n2; i++) 
          {
            double rr = result[i] + result[n-i], 
              ri = result[i] - result[n-i], 
              sr = scratch[i] + scratch[n-i],
              si = scratch[i] - scratch[n-i];
            if ((mag = sr*sr + ri*ri) == 0.0)  
            {   // check for zero divide
              status = false;
              goto ErrorExit;
            }
            result[i]  =  (rr*sr - ri*si) / (n*mag);  // real part
            scratch[i] =  (si*sr + ri*rr) / (n*mag);  // imaginary part 
            result[n-i]  = result[i];     // symmetry
            scratch[n-i] = -scratch[i];       // symmetry
          }
        }

        // transform back - this is an inverse FFT
        base.FFT(result,scratch, FourierDirection.Inverse);

        //---------------------------------------------------------------------------//
        //  2-dimensional convolution
        //---------------------------------------------------------------------------//

      } 
      else if (ndim == 2) 
      {
    
        int n = dim[0],
          m = dim[1];

        // set imaginary parts to zero
        FillZero(result);  // imaginary part of data
        FillZero(scratch); // imaginary part of response

        // transform both arrays - this is a forward FFT
        base.FFT(data,result, FourierDirection.Forward);
        base.FFT(response,scratch,FourierDirection.Forward);

        if (isign == FourierDirection.Forward) 
        { 
          double scale = 1.0/n/m;
          for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) 
            {
              int l = row_order ? i*id+j : j*id+i;
              // do complex multiplication with three real multiplications
              // (a+ib)(c+id) = ( ac-bd ) + i( (a+b)(c+d)-ac-bd )
              double ac = data[l]*response[l],
                bd = result[l]*scratch[l];
              scratch[l] = scale*((data[l]+result[l])*(response[l]+scratch[l])-ac-bd);
              result[l]  = scale*(ac-bd);
            }
        } 
        else /* isign == inverse */ 
        {
          double mag, scale = 1.0/n/m;
          for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) 
            {
              int l = row_order ? i*id+j : j*id+i;
              // do complex division (a+ib)/(c+id) = 
              // (a+ib)(c-id)/(cc+dd) = [(ac+bd) + i((a+b)(c-d)-ac+bd)]/(cc+dd)
              if ((mag = Square(response[l])+Square(scratch[l])) == 0.0) 
              {
                status = false;
                goto ErrorExit;
              }
              mag = scale/mag;
              double ac = data[l]*response[l],
                bd = result[l]*scratch[l];
              scratch[l] = mag*((data[l]+result[l])*(response[l]-scratch[l])-ac+bd);
              result[l]  = mag*(ac+bd);
            }
        } 

        // transform back - this is an inverse FFT
        base.FFT(result,scratch,FourierDirection.Inverse);

        //---------------------------------------------------------------------------//
        //  3-dimensional convolution
        //---------------------------------------------------------------------------//

      } 
      else if (ndim == 3) 
      {

        int n = dim[0],
          m = dim[1],
          p = dim[2];

        // set imaginary parts to zero
        FillZero(result);  // imaginary part of data
        FillZero(scratch); // imaginary part of response

        // transform both arrays - this is a forward FFT
        base.FFT(data,result,FourierDirection.Forward);
        base.FFT(response,scratch,FourierDirection.Forward);

        if (isign == FourierDirection.Forward) 
        {
          double scale = 1.0/n/m/p;
          for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) 
              for (int k = 0;  k < p; k++) 
              {
                int l = row_order ? (i*m+j)*id+k : (k*m+j)*id+i;
                // do complex multiplication with three real multiplications
                // (a+ib)(c+id) = ( ac-bd ) + i( (a+b)(c+d)-ac-bd )
                double ac = data[l]*response[l],
                  bd = result[l]*scratch[l];
                scratch[l] = scale*((data[l]+result[l])*(response[l]+scratch[l])-ac-bd);
                result[l]  = scale*(ac-bd);
              }
        } 
        else /* isign == inverse */ 
        {
          double mag, scale = 1.0/n/m/p;
          for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) 
              for (int k = 0;  k < p; k++) 
              {
                int l = row_order ? (i*m+j)*id+k : (k*m+j)*id+i;
                // do complex division (a+ib)/(c+id) = 
                // (a+ib)(c-id)/(cc+dd) = [(ac+bd) + i((a+b)(c-d)-ac+bd)]/(cc+dd)
                if ((mag = Square(response[l])+Square(scratch[l])) == 0.0) 
                {
                  status = false;
                  goto ErrorExit;
                }
                mag = scale/mag;
                double ac = data[l]*response[l],
                  bd = result[l]*scratch[l];
                scratch[l] = mag*((data[l]+result[l])*(response[l]-scratch[l])-ac+bd);
                result[l]  = mag*(ac+bd);
              } 
        }

        // transform back - this is an inverse FFT
        base.FFT(result,scratch,FourierDirection.Inverse);
      }

      ErrorExit:
  
          
        return status;
    }



    /// <summary>
    /// Convolves or deconvolves a splitted complex-valued data set data[] (including any
    /// user supplied zero padding) with a response function response[].
    /// The result is returned in the splitted complex arrays resultre[] and resultim[]. All arrays including
    /// the scratch[] arrays must have the same dimensions (or larger).
    /// The data set (and of course the other arrays) can be either one-dimensional,
    /// two-dimensional, or three-dimensional, d = 1,2,3.  Each dimension must be 
    /// of the form n = (2**p) * (3**q) * (5**r), because of the underlying FFT.
    /// </summary>
    /// <param name="datare">
    /// The splitted complex-valued data set. Note, that you have to
    /// care for end effects by zero padding. This means, 
    /// that you have to pad the data with a number of zeros
    /// on one end equal to the maximal positive duration
    /// or maximal negative duration of the response function,
    /// whichever is larger!!</param>
    /// <param name="dataim">The imaginary part of the data array.</param>
    /// <param name="responsere">
    ///  The response function must be stored in wrap-around
    ///  order. This means that the first half of the array
    ///  response[] (in each dimension) contains the impulse
    ///  response function at positive times, while the second
    ///  half of the array contains the impulse response
    ///  function at negative times, counting down from the
    ///  element with the highest index. The array must have 
    ///  at least the size of the data array.
    /// </param>
    /// <param name="responseim">The imaginary part of the response array.</param>
    /// <param name="resultre">
    /// The real part of the result array. It must have 
    /// at least the size of the data array.
    /// </param>
    /// <param name="resultim">The imaginary part of the result array.</param>
    /// <param name="scratchre">
    ///  A work array. If a NULL pointer is passed the
    /// work array is allocated and freed auotomatically.
    /// If the array is given by the user it must have 
    /// at least the size of the data array.
    /// </param>
    /// <param name="scratchim">
    ///  A work array. If a NULL pointer is passed the
    /// work array is allocated and freed auotomatically.
    /// If the array is given by the user it must have 
    /// at least the size of the data array.
    /// </param>
    /// <param name="isign">
    /// If isign == forward a convolution is performed. 
    /// If isign == inverse then a deconvolution is performed.
    /// </param>
    /// <returns>
    /// In the case of a convolution (isign == forward) the value "true" is returned
    /// always. In the case of deconvolution (isign == inverse) the value "false" is
    /// returned if the FFT transform of the response function is exactly zero for 
    /// some value. This indicates that the original convolution has lost all 
    /// information at this particular frequency, so that a reconstruction is not
    /// possible. If the transform of the response function is non-zero everywhere
    /// the deconvolution can be performed and the value "true" is returned.
    /// </returns>
 
    public bool Convolute (
      double[] datare, double[] dataim, 
      double[] responsere, double[] responseim,
      double[] resultre, double[] resultim,
      double[] scratchre, double[] scratchim,
      FourierDirection isign)
    {
      // return status
      bool status = true;

      // get total size of data array
      int size = 0;
      if (ndim == 0) 
      {
        throw new ArithmeticException("Convolute: no dimensions have been specified");
      } 
      else if (ndim == 1) 
      {
        size = dim[0];
      } 
      else if (ndim == 2) 
      {
        size = row_order ? (dim[0] * id) : (id * dim[1]);
      } 
      else if (ndim == 3) 
      {
        size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
      }

      // allocate the scratch array
      //bool auto_scratch = false;
      if ( null==scratchre )
        scratchre = new  double[size];
      if ( null==scratchim )
        scratchim = new  double[size];

      //---------------------------------------------------------------------------//
      //  1-dimensional convolution (original data not are overwritten)
      //---------------------------------------------------------------------------//

      if (ndim == 1) 
      {

        // First copy the arrays data and response to result and scratch,
        // respectively, to prevent overwriting of the original data.
        Array.Copy(datare,resultre,size);
        Array.Copy(dataim,resultim,size);
        Array.Copy(responsere,scratchre, size);
        Array.Copy(responseim,scratchim, size);

        // transform both arrays simultaneously - this is a forward FFT
        base.FFT(resultre,resultim, FourierDirection.Forward);
        base.FFT(scratchre,scratchim, FourierDirection.Forward);
    
        // multiply FFTs to convolve
        int n = dim[0];

        if (isign == FourierDirection.Forward) 
        {
      
          double scale = 1.0/n;
          for (int i = 0; i < n; i++) 
          {
            double re = resultre[i]*scratchre[i] - resultim[i]*scratchim[i];
            double im = resultre[i]*scratchim[i] + resultim[i]*scratchre[i];
            resultre[i] = re*scale; 
            resultim[i] = im*scale;
          }
        } 
        else /* isign == inverse */ 
        {

          double mag;
          if ((mag = Square(scratchre[0])+Square(scratchim[0])) == 0.0) 
          {   // check for zero divide
            status = false;
            goto ErrorExit;
          }
          for (int i = 0; i < n; i++) 
          {
            double rr = resultre[i]; 
            double ri = resultim[i]; 
            double sr = scratchre[i];
            double si = scratchim[i];
            if ((mag = sr*sr + ri*ri) == 0.0)  
            {   // check for zero divide
              status = false;
              goto ErrorExit;
            }
            resultre[i] =  (rr*sr - ri*si) / (n*mag);  // real part
            resultim[i] =  (si*sr + ri*rr) / (n*mag);  // imaginary part 
          }
        }
        // transform back - this is an inverse FFT
        base.FFT(resultre,resultim, FourierDirection.Inverse);
      }
      else
      {
        throw new NotImplementedException("Sorry, convolution of dimension 2 or 3 is not implemented yet. Will you do it?");
      }

      ErrorExit:
        return status;
    }



    /// <summary>
    /// Performs a correlation of two comlex arrays which are in splitted form (i.e. real and imaginary part are separate arrays). Attention: the data into the
    /// input arrays will be destroyed!
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    public void CyclicCorrelationDestructive(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      // return status
      // bool status = true;

      // get total size of data array
      int size = 0;
      if (ndim == 0) 
      {
        throw new ArithmeticException("Convolute: no dimensions have been specified");
      } 
      else if (ndim == 1) 
      {
        size = dim[0];
      } 
      else if (ndim == 2) 
      {
        size = row_order ? (dim[0] * id) : (id * dim[1]);
      } 
      else if (ndim == 3) 
      {
        size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
      }


      if(ndim==1)
      {
        base.FFT( src1real, src1imag, FourierDirection.Forward);
        base.FFT( src2real, src2imag, FourierDirection.Forward);
        ArrayMath.MultiplySplittedComplexArraysCrossed(src1real, src1imag, src2real, src2imag, resultreal, resultimag, n, 1.0/n);
        base.FFT( resultreal,resultimag, FourierDirection.Forward);
      }
      else
        throw new NotImplementedException("Sorry, correlation for dimensions > 1 is not implemented yet! Will you do it?"); 
    }


    /// <summary>
    /// Performs a cyclic correlation of two complex arrays which are in splitted form. The input arrays will leave intact.
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    /// <remarks>Two helper arrays of length n are automatially allocated and freed during the operation.</remarks>
    public void CyclicCorrelation(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      double[] help1=null, help2=null;
      CyclicCorrelation(src1real, src1imag,src2real,src2imag,resultreal,resultimag,
        n,ref help1, ref help2);
    }

    /// <summary>
    /// Performs a cyclic correlation of two complex arrays which are in splitted form. The input arrays will leave intact.
    /// </summary>
    /// <param name="src1real">The real part of the first input array (will be destroyed).</param>
    /// <param name="src1imag">The imaginary part of the first input array (will be destroyed).</param>
    /// <param name="src2real">The real part of the second input array (will be destroyed).</param>
    /// <param name="src2imag">The imaginary part of the second input array (will be destroyed).</param>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="n">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    /// <param name="scratchreal">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    /// <param name="scratchimag">A helper array. Must be at least of length n. If null is provided here, a new scatch array will be allocated.</param>
    public void CyclicCorrelation(
      double[] src1real, double[] src1imag, 
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n,
      ref double[] scratchreal, ref double[] scratchimag
      )
    {
      // return status
      // bool status = true;

      // get total size of data array
      int size = 0;
      if (ndim == 0) 
      {
        throw new ArithmeticException("Convolute: no dimensions have been specified");
      } 
      else if (ndim == 1) 
      {
        size = dim[0];
      } 
      else if (ndim == 2) 
      {
        size = row_order ? (dim[0] * id) : (id * dim[1]);
      } 
      else if (ndim == 3) 
      {
        size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
      }


      if ( null==scratchreal || scratchreal.Length<n)
        scratchreal = new  double[size];
      if ( null==scratchimag || scratchimag.Length<n )
        scratchimag = new  double[size];

      // First copy the arrays data and response to result and scratch,
      // respectively, to prevent overwriting of the original data.
      Array.Copy(src1real,resultreal, n);
      Array.Copy(src1imag,resultimag, n);
      Array.Copy(src2real,scratchreal, n);
      Array.Copy(src2imag,scratchimag, n);

      if(ndim==1)
      {
        base.FFT( resultreal, resultimag, FourierDirection.Forward);
        base.FFT( scratchreal, scratchimag, FourierDirection.Forward);
        ArrayMath.MultiplySplittedComplexArraysCrossed(resultreal, resultimag, scratchreal, scratchimag, resultreal, resultimag, n, 1.0/n);
        base.FFT( resultreal,resultimag, FourierDirection.Forward);
      }
      else
        throw new NotImplementedException("Sorry, correlation for dimensions > 1 is not implemented yet! Will you do it?"); 

    }

    public bool CyclicCorrelation(double[] data, double[] response, double[] result, int nn, double[] scratch)
    {
      // return status
      // bool status = true;

      // get total size of data array
      int size = 0;
      if (ndim == 0) 
      {
        throw new ArithmeticException("MpConvolution::Convolute no dimensions have been specified");
      } 
      else if (ndim == 1) 
      {
        size = dim[0];
      } 
      else if (ndim == 2) 
      {
        size = row_order ? (dim[0] * id) : (id * dim[1]);
      } 
      else if (ndim == 3) 
      {
        size = row_order ? (dim[0] * dim[1] * id) : (id * dim[1] * dim[2]);
      }

      // allocate the scratch array
      //bool auto_scratch = false;
      if ( null==scratch ) 
      {
        scratch = new  double[size];
        //auto_scratch = true;
      }

      //---------------------------------------------------------------------------//
      //  1-dimensional correlation (original data not are overwritten)
      //---------------------------------------------------------------------------//

      if (ndim == 1) 
      {

        // First copy the arrays data and response to result and scratch,
        // respectively, to prevent overwriting of the original data.
        Array.Copy(data,result,size);
        Array.Copy(response,scratch, size);

        // transform both arrays simultaneously - this is a forward FFT
        base.FFT(result,scratch, FourierDirection.Forward);
    
        // multiply FFTs to convolve
        int n = dim[0], n2 = n/2;
     
        double scale = 0.25/n;
        result[0] *= scratch[0] / n;
        scratch[0] = 0;
        for (int i = 1; i <= n2; i++) 
        {
          double rr = result[i]  + result[n-i], 
            ri = scratch[i] - scratch[n-i],
            sr = scratch[i] + scratch[n-i],
            si = result[n-i]  - result[i]; 
          result[i]  = scale * (rr*sr + ri*si);   // real part
          scratch[i] = scale * (rr*si - ri*sr);   // imaginary part 
          result[n-i]  = result[i];     // symmetry
          scratch[n-i] = -scratch[i];       // symmetry
        }
      } 

      // transform back - this is an inverse FFT
      base.FFT(result,scratch, FourierDirection.Inverse);


      return true;
    } 
  
    protected static void FillZero(double[] array)
    {
      for(int i=0;i<array.Length;i++)
        array[i]=0;
    }

    protected static double Square(double x)
    {
      return x*x;
    }
  }

}
