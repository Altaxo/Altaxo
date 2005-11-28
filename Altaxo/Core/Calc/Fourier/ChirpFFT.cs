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

using System;



namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Provides a method to perform a Fourier transformation of arbirtrary length.
  /// </summary>
  /// <remarks>The following code is adopted from the FFT library, see www.jjj.de.</remarks>
  public class ChirpFFT
  {
    
    static void CopyFromComplexToSplittedArrays(double[] destreal, double[] destimag, Complex[] src, int count)
    {
      for(int i=0;i<count;i++)
      {
        destreal[i]=src[i].Re;
        destimag[i]=src[i].Im;
      }
    }

    static void CopyFromSplittedArraysToComplex(Complex[] dest, double[] srcreal, double[] srcimag, int count)
    {
      for(int i=0;i<count;i++)
      {
        dest[i].Re = srcreal[i];
        dest[i].Im = srcimag[i];
      }
    }

    static void MultiplySplittedComplexArrays(double[] resreal, double[] resimag,
      double[] arr1real, double[] arr1imag, double[] arr2real, double[] arr2imag, int count)
    {
      for(int i=0;i<count;i++)
      {
                                
        double real = arr1real[i]*arr2real[i] - arr1imag[i]*arr2imag[i];
        double imag = arr1real[i]*arr2imag[i] + arr1imag[i]*arr2real[i];
        resreal[i]=real;
        resimag[i]=imag;
      }
    }

    static void NormalizeArrays(double[] arr1, double[] arr2, double factor, int count)
    {
      for(int i=0;i<count;i++)
      {
        arr1[i]*=factor;
        arr2[i]*=factor;
      }
    }


    static void fhtconvolution( Complex[] resarray,
      Complex[] arr1, Complex[] arr2, int arrsize)
    {
      double[] fht1real = new double[arrsize];
      double[] fht1imag = new double[arrsize];
      double[] fht2real = new double[arrsize];
      double[] fht2imag = new double[arrsize];

      CopyFromComplexToSplittedArrays(fht1real,fht1imag,arr1,arrsize);
      CopyFromComplexToSplittedArrays(fht2real,fht2imag,arr2,arrsize);

      // do a convolution by fourier transform both parts, multiply and inverse fft
      FastHartleyTransform.FFT(fht1real, fht1imag, arrsize);
      FastHartleyTransform.FFT(fht2real, fht2imag, arrsize);
      MultiplySplittedComplexArrays(fht1real, fht1imag, fht1real, fht1imag, fht2real, fht2imag, arrsize);
      FastHartleyTransform.IFFT( fht1real,fht1imag, arrsize);
      NormalizeArrays(fht1real, fht1imag, 1.0/arrsize, arrsize);
      CopyFromSplittedArraysToComplex(resarray,fht1real,fht1imag,arrsize);
    }


    /// <summary>
    /// Performs a convolution of two comlex arrays with are in splitted form (i.e. real and imaginary part are separate arrays). Attention: the values of the
    /// input arrays will be destroyed!
    /// </summary>
    /// <param name="resultreal">The real part of the result. (may be identical with arr1 or arr2).</param>
    /// <param name="resultimag">The imaginary part of the result (may be identical with arr1 or arr2).</param>
    /// <param name="arr1real">The real part of the first input array.</param>
    /// <param name="arr1imag">The imaginary part of the first input array.</param>
    /// <param name="arr2real">The real part of the second input array.</param>
    /// <param name="arr2imag">The imaginary part of the second input array.</param>
    /// <param name="arrsize">The length of the convolution. Has to be equal or smaller than the array size. Has to be a power of 2!</param>
    static void fhtconvolution(double[] resultreal, double[] resultimag, double[] arr1real, double[] arr1imag, double[] arr2real, double[] arr2imag, int arrsize)
    {
      FastHartleyTransform.FFT( arr1real, arr1imag,arrsize);
      FastHartleyTransform.FFT( arr2real, arr2imag,arrsize);
      MultiplySplittedComplexArrays(resultreal, resultimag, arr1real, arr1imag, arr2real, arr2imag, arrsize);
      FastHartleyTransform.IFFT( resultreal,resultimag,arrsize);
      NormalizeArrays(resultreal, resultimag, 1.0/arrsize, arrsize);
    }
   
    /// <summary>
    /// Returns the neccessary transformation size for a chirp transformation of length n.
    /// </summary>
    /// <param name="n">The length of the chirp transformation.</param>
    /// <returns>Neccessary length of the transformation arrays. Note that this is based on a convolution of
    /// base 2.</returns>
    public static int GetNecessaryTransformationSize(int n)
    {
      if(n<=2)
        return 2;

      int ldnn = 2+ld((uint)(n-2));
      return (1<<ldnn);
    }

    // der Chirpalgorithmus funktioniert auch noch bei arrsize=1+2^n mit der nächstgelegenen Potenz 2^(n+1) !!!

    private static void chirpnativefft(Complex[] result, Complex[] arr, int arrsize, FourierDirection direction)
    {
      int phasesign = direction==FourierDirection.Forward ? 1 : -1;
      int arrsize2 = arrsize+arrsize;
     
      if(arrsize<=2)
        throw new ArgumentException("This algorithm works for array sizes > 2 only.");

      int msize = GetNecessaryTransformationSize(arrsize);
     
      Complex[] xjfj = new Complex[msize];
      Complex[] fserp = new Complex[msize];
      Complex[] resarray = new Complex[msize];
      //Complex[] cmparray = new Complex[arrsize];


      // bilde xj*fj
      double prefactor = phasesign * Math.PI / arrsize;
      int np = 0;
      for(int i=0;i<arrsize;i++)
      {
        double phi = prefactor*np; // np should be equal to (i*i)%arrsize2
        Complex val = new Complex(Math.Cos(phi),Math.Sin(phi));
        xjfj[i] = arr[i] * val;

        np += i+i+1;  // np == (k*k)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;

      }

      // fill positive and negative part of fserp
      fserp[0]=new Complex(1,0);
      prefactor = -phasesign * Math.PI / arrsize;
      np = 1; // since we start the loop with 1
      for(int i=1;i<arrsize;i++)
      {
        double phi = prefactor * np; // np should be equal to (i*i)%arrsize2
        fserp[i].Re = fserp[msize-i].Re = Math.Cos(phi);
        fserp[i].Im = fserp[msize-i].Im = Math.Sin(phi);
        
        np += i+i+1;  // np == (k*k)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;
      }

      // convolute xjfj with fserp
      //NativeCyclicConvolution(resarray,xjfj,fserp,msize);
      fhtconvolution(resarray,xjfj,fserp,msize);
      //printArray("Result of convolution",resarray,msize);

      // multipliziere mit fserpschlange
      prefactor = phasesign * Math.PI / arrsize;
      np = 0;
      for(int i=0;i<arrsize;i++)
      {
        double phi = prefactor * np; // np should be equal to (i*i)%arrsize2
        result[i] = resarray[i]* new Complex(Math.Cos(phi),Math.Sin(phi));
            
        np += i+i+1;  // np == (i*i)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;
      }
    }

    private class ChirpNativeFFTStorage
    {
      public double[] xjfj_real;
      public double[] xjfj_imag;
      public double[] fserp_real;
      public double[] fserp_imag;
      public double[] resarray_real;
      public double[] resarray_imag;

      public ChirpNativeFFTStorage(int msize)
      {
        xjfj_real  = new double[msize];
        xjfj_imag  = new double[msize];
        fserp_real = new double[msize];
        fserp_imag = new double[msize];
        resarray_real = new double[msize];
        resarray_imag = new double[msize];
      }

      public int Length
      {
        get 
        {
          return xjfj_real.Length; 
        }
      }
    }

    private static void chirpnativefft(
      double[] resultreal, 
      double[] resultimag,
      double[] inputreal,
      double[] inputimag,
      int arrsize,
      FourierDirection direction,
      ref ChirpNativeFFTStorage s)
    {
      int phasesign = direction==FourierDirection.Forward ? 1 : -1;
      int arrsize2 = arrsize+arrsize;
     
      if(arrsize<=2)
        throw new ArgumentException("This algorithm works for array sizes > 2 only.");

      int msize = GetNecessaryTransformationSize(arrsize);
     
      if(s==null || s.Length!=msize)
      {
        s = new ChirpNativeFFTStorage(msize);
      }

      double[] xjfj_real  = s.xjfj_real;
      double[] xjfj_imag  = s.xjfj_imag;
      double[] fserp_real = s.fserp_real;
      double[] fserp_imag = s.fserp_imag;
      double[] resarray_real = s.resarray_real;
      double[] resarray_imag = s.resarray_imag;

      // bilde xj*fj
      double prefactor = phasesign * Math.PI / arrsize;
      int np = 0;
      for(int i=0;i<arrsize;i++)
      {
        double phi = prefactor*np; // np should be equal to (i*i)%arrsize2
        double vre = Math.Cos(phi);
        double vim = Math.Sin(phi);
        xjfj_real[i] = inputreal[i] * vre - inputimag[i] * vim;
        xjfj_imag[i] = inputreal[i] * vim + inputimag[i] * vre;

        np += i+i+1;  // np == (k*k)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;

      }

      // fill positive and negative part of fserp
      fserp_real[0]=1; fserp_imag[0]=0;
      prefactor = -phasesign * Math.PI / arrsize;
      np = 1; // since we start the loop with 1
      for(int i=1;i<arrsize;i++)
      {
        double phi = prefactor * np; // np should be equal to (i*i)%arrsize2
        fserp_real[i] = fserp_real[msize-i] = Math.Cos(phi);
        fserp_imag[i] = fserp_imag[msize-i] = Math.Sin(phi);
        
        np += i+i+1;  // np == (k*k)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;
      }

      // convolute xjfj with fserp
      //NativeCyclicConvolution(resarray,xjfj,fserp,msize);
      fhtconvolution(resarray_real,resarray_imag,xjfj_real,xjfj_imag,fserp_real,fserp_imag,msize);

      // multipliziere mit fserpschlange
      prefactor = phasesign * Math.PI / arrsize;
      np = 0;
      for(int i=0;i<arrsize;i++)
      {
        double phi = prefactor * np; // np should be equal to (i*i)%arrsize2
        double vre = Math.Cos(phi);
        double vim = Math.Sin(phi);
        resultreal[i] = resarray_real[i]*vre - resarray_imag[i]*vim;
        resultimag[i] = resarray_real[i]*vim + resarray_imag[i]*vre;
            
        np += i+i+1;  // np == (i*i)%n2
        if ( np >= arrsize2 ) 
          np -= arrsize2;
      }
    }


    /// <summary>
    /// Performs an FFT of arbitrary length by the chirp method. Use this method only if no other
    /// FFT is applicable.
    /// </summary>
    /// <param name="x">Array of real values.</param>
    /// <param name="y">Array of imaginary values.</param>
    /// <param name="direction">Direction of Fourier transform.</param>
    public static void
      FFT(double[] x, double[] y, FourierDirection direction)
    {
      object storage=null;
      FFT(x,y,direction, ref storage);
    }

    /// <summary>
    /// Performs an FFT of arbitrary length by the chirp method. Use this method only if no other
    /// FFT is applicable.
    /// </summary>
    /// <param name="x">Array of real values.</param>
    /// <param name="y">Array of imaginary values.</param>
    /// <param name="direction">Direction of Fourier transform.</param>
    /// <param name="temporaryStorage">On return, this reference holds an object for temporary storage. You can use this in subsequent FFTs of the same size.</param>
    public static void
      FFT(double[] x, double[] y, FourierDirection direction, ref object temporaryStorage)
    {
      if(x.Length!=y.Length)
        throw new ArgumentException("Length of real and imaginary array do not match!");


      ChirpNativeFFTStorage s = temporaryStorage as ChirpNativeFFTStorage;
      chirpnativefft(x, y, x, y, x.Length, direction, ref s);
      temporaryStorage = s;
    }

    /// <summary>
    /// Performs an FFT of arbitrary length by the chirp method. Use this method only if no other
    /// FFT is applicable.
    /// </summary>
    /// <param name="x">Array of real values.</param>
    /// <param name="y">Array of imaginary values.</param>
    /// <param name="n">Number of points to transform.</param>
    /// <param name="direction">Direction of Fourier transform.</param>
    public static void
      FFT(double[] x, double[] y, uint n, FourierDirection direction)
    {
      object storage=null;
      FFT(x,y,n,direction, ref storage);
    }

    /// <summary>
    /// Performs an FFT of arbitrary length by the chirp method. Use this method only if no other
    /// FFT is applicable.
    /// </summary>
    /// <param name="x">Array of real values.</param>
    /// <param name="y">Array of imaginary values.</param>
    /// <param name="n">Number of points to transform.</param>
    /// <param name="direction">Direction of Fourier transform.</param>
    /// <param name="temporaryStorage">On return, this reference holds an object for temporary storage. You can use this in subsequent FFTs of the same size.</param>

    public static void
      FFT(double[] x, double[] y, uint n, FourierDirection direction, ref object temporaryStorage)
    {
      ChirpNativeFFTStorage s = temporaryStorage as ChirpNativeFFTStorage;
      chirpnativefft(x,y,x,y,(int)n, direction, ref s);
      temporaryStorage = s;
    }

    /// <summary>
    /// Returns the neccessary size for a chirp convolution of length n.
    /// </summary>
    /// <param name="n">The length of the convolution.</param>
    /// <returns>Neccessary length of the scratch arrays. Note that this is based on a convolution of
    /// base 2.</returns>
    public static int GetNecessaryConvolutionSize(int n)
    {
      if(n<=2)
        return 2;

      int ldnn = 2+ld((uint)(n-1));
      return (1<<ldnn);
    }


    /// <summary>
    /// Performs a cyclic convolution of splitted complex data of arbitrary length.
    /// </summary>
    /// <param name="datare">Real part of the data array (first input array).</param>
    /// <param name="dataim">Imaginary part of the data array (first input array).</param>
    /// <param name="responsere">Real part of the response array (second input array).</param>
    /// <param name="responseim">Imaginary part of the response array (second input array).</param>
    /// <param name="resultre">The real part of the resulting array.</param>
    /// <param name="resultim">The imaginary part of the resulting array.</param>
    /// <param name="n">The convolution size. The input and result arrays may be larger, but of course not smaller than this number.</param>
    public static void CyclicConvolution(
      double[] datare, double[] dataim,
      double[] responsere, double[] responseim,
      double[] resultre, double[] resultim,
      int n)
    {
      int msize = GetNecessaryConvolutionSize(n);
      // System.Diagnostics.Trace.WriteLine(string.Format("Enter chirp convolution with n={0}, m={1}",n,msize));
      
      double[] srcre = new double[msize];
      double[] srcim = new double[msize];
      double[] rspre = new double[msize];
      double[] rspim = new double[msize];
       

      Array.Copy(datare,srcre,n);
      Array.Copy(dataim,srcim,n);

      // Copy the response not only to the beginning, but also to the end of the scratch array
      Array.Copy(responsere,rspre,n);
      Array.Copy(responsere,1,rspre,msize-(n-1),n-1);
      Array.Copy(responseim,rspim,n);
      Array.Copy(responseim,1,rspim,msize-(n-1),n-1);

      FastHartleyTransform.CyclicDestructiveConvolution(srcre,srcim,rspre,rspim,srcre,srcim,msize);

      Array.Copy(srcre,resultre,n);
      Array.Copy(srcim,resultim,n);
    }


    /// <summary>
    /// Returns the neccessary size of the padding arrays for a cyclic correlation of length n.
    /// </summary>
    /// <param name="n">The length of the cyclic correlation.</param>
    /// <returns>Neccessary length of the scratch arrays. Note that this is based on a convolution of
    /// base 2.</returns>
    public static int GetNecessaryCorrelationSize(int n)
    {
      if(n<=2)
        return 2;

      int ldnn = 2+ld((uint)(n-1));
      return (1<<ldnn);
    }


    /// <summary>
    /// Performs a cyclic correlation of splitted complex data of arbitrary length.
    /// </summary>
    /// <param name="datare">Real part of the data array (first input array).</param>
    /// <param name="dataim">Imaginary part of the data array (first input array).</param>
    /// <param name="responsere">Real part of the response array (second input array).</param>
    /// <param name="responseim">Imaginary part of the response array (second input array).</param>
    /// <param name="resultre">The real part of the resulting array.</param>
    /// <param name="resultim">The imaginary part of the resulting array.</param>
    /// <param name="n">The convolution size. The input and result arrays may be larger, but of course not smaller than this number.</param>
    public static void CyclicCorrelation(
      double[] datare, double[] dataim,
      double[] responsere, double[] responseim,
      double[] resultre, double[] resultim,
      int n)
    {
      int msize = GetNecessaryCorrelationSize(n);
      // System.Diagnostics.Trace.WriteLine(string.Format("Enter chirp convolution with n={0}, m={1}",n,msize));
      
      double[] srcre = new double[msize];
      double[] srcim = new double[msize];
      double[] rspre = new double[msize];
      double[] rspim = new double[msize];
       

      Array.Copy(datare,srcre,n);
      Array.Copy(dataim,srcim,n);

      // Copy the response not only to the beginning, but also immediatly after the data
      Array.Copy(responsere,rspre,n);
      Array.Copy(responsere,0,rspre,n,n-1);
      Array.Copy(responseim,rspim,n);
      Array.Copy(responseim,0,rspim,n,n-1);

      FastHartleyTransform.CyclicCorrelationDestructive(srcre,srcim,rspre,rspim,srcre,srcim,msize);

      Array.Copy(srcre,resultre,n);
      Array.Copy(srcim,resultim,n);
    }


    /// <summary>
    /// Performs a cyclic correlation of splitted complex data of arbitrary length.
    /// </summary>
    /// <param name="src1">The first input array.</param>
    /// <param name="src2">The second input array.</param>
    /// <param name="result">The resulting array.</param>
    /// <param name="n">The correlation size. The input and result arrays may be larger, but of course not smaller than this number.</param>
    public static void CyclicCorrelation(
      double[] src1,
      double[] src2, 
      double[] result,
      int n)
    {
      int msize = GetNecessaryCorrelationSize(n);
      // System.Diagnostics.Trace.WriteLine(string.Format("Enter chirp convolution with n={0}, m={1}",n,msize));
      
      double[] srcre = new double[msize];
      double[] rspre = new double[msize];
       

      Array.Copy(src1,srcre,n);

      // Copy the response not only to the beginning, but also immediatly after the data
      Array.Copy(src2,rspre,n);
      Array.Copy(src2,0,rspre,n,n-1);

      FastHartleyTransform.CyclicCorrelationDestructive(srcre,rspre,srcre,msize);

      Array.Copy(srcre,result,n);
    }

    /*
    public static void
      FFT(double[] x, double[] y, uint n, bool backward)
    {
      Complex[] arr = new Complex[n];
      CopyFromSplittedArraysToComplex(arr,x,y,(int)n);
      Complex[] result = new Complex[n];
      chirpnativefft(result,arr,(int)n, backward);
      CopyFromComplexToSplittedArrays(x,y,result,(int)n);
    }
*/

    static void make_fft_chirp(double[] wr, double[] wi, uint n, bool backward)
    {

      double phi = backward ? -Math.PI/n : Math.PI/n;

      uint n2 = 2*n;
      for (uint np=0,k=0; k<n; ++k)
      {
        double c = Math.Cos(phi*np);
        double s = Math.Sin(phi*np);

        np += ((k<<1)+1);  // np == (k*k)%n2

        if ( np>=n2 ) 
          np -= n2;

        wr[k] = c;
        wi[k] = s;
      }
    }


    static void complete_fft_chirp(double[] wr, double[] wi, uint n, uint nn)
    {
      if ( (n&1)!=0 )  // n odd
      {
        for (uint k=n; k<nn; ++k)  
          wr[k] = -wr[k-n];
        for (uint k=n; k<nn; ++k)
          wi[k] = -wi[k-n];
      }
      else
      {
        for (uint k=n; k<nn; ++k)
          wr[k] = wr[k-n];
        for (uint k=n; k<nn; ++k)
          wi[k] = wi[k-n];
      }
    }
   
    static void make_fft_fract_chirp(double[] wr, double[] wi, double v, uint n, uint nn)
    {
      double phi = v*Math.PI/n;

      uint n2 = 2*n;
      uint np=0;
      for (uint k=0; k<nn; ++k)
      {
        double c = Math.Cos(phi*np);
        double s = Math.Sin(phi*np);
      
        np += ((k<<1)+1);  // np == (k*k)%n2
        if ( np>=n2 ) 
          np -= n2;

        wr[k] = c;
        wi[k] = s;
      }
    }

    static void cmult(double c, double s, ref double u, ref double v)
    {
      double t = u*s+v*c;  
      u *= c;  
      u -= v*s; 
      v = t; 
    }

    // complex multiply array g[] by f[]:
    //  Complex(gr[],gi[]) *= Complex(fr[],fi[])
    static void ri_multiply(double[] fr, double[] fi, double[] gr, double[] gi, uint n)
    {
      while ( (n--)!=0 )  cmult(fr[n], fi[n], ref gr[n], ref gi[n]);
    }

    static void negate(double[] f, uint n)
      // negate every element of f[]
    {
      while ( (n--)!=0 )  f[n] = -f[n];
    }

    /// <summary>
    /// Returns k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.</returns>
    static int ld(uint x)
    {
      if ( 0==x )  return  0;

      int r = 0;
      if ( (x & 0xffff0000)!=0 )  { x >>= 16;  r += 16; }
      if ( (x & 0x0000ff00)!=0 )  { x >>=  8;  r +=  8; }
      if ( (x & 0x000000f0)!=0 )  { x >>=  4;  r +=  4; }
      if ( (x & 0x0000000c)!=0 )  { x >>=  2;  r +=  2; }
      if ( (x & 0x00000002)!=0 )  {            r +=  1; }
      return r;
    }

    /// <summary>
    /// Returns k so that 2^k &lt;= x &lt; 2^(k+1).
    /// If x==0 then 0 is returned.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.</returns>
    static int ld(ulong x)
    {
      if ( 0==x )  return  0;

      int r = 0;
      if ( (x & (~0UL<<32))!=0 )  { x >>= 32;  r += 32; }
      if ( (x & 0xffff0000)!=0 )  { x >>= 16;  r += 16; }
      if ( (x & 0x0000ff00)!=0 )  { x >>=  8;  r +=  8; }
      if ( (x & 0x000000f0)!=0 )  { x >>=  4;  r +=  4; }
      if ( (x & 0x0000000c)!=0 )  { x >>=  2;  r +=  2; }
      if ( (x & 0x00000002)!=0 )  {            r +=  1; }
      return r;
    }

    static void
      fft_complex_convolution(double[] fr, double[] fi,
      double[] gr, double[] gi,
      uint ldn, double v /*=0.0*/ )
      //
      // (complex, cyclic) convolution:  (gr,gi)[] :=  (fr,fi)[] (*) (gr,gi)[]
      // (use zero padded data for usual conv.)
      // ldn := base-2 logarithm of the array length
      //
      // supply a value for v for a normalization factor != 1/n
      //
    {
      // const int is = 1;
      int n = (1<<(int)ldn);

      FastHartleyTransform.FFT(fr,fi,n); //FFT(fr, fi, ldn, is);
      FastHartleyTransform.FFT(gr,gi,n); //FFT(gr, gi, ldn, is);

      if ( v==0.0 )  v = 1.0/n;
      for (uint k=0; k<n; ++k)
      {
        double tr = fr[k];
        double ti = fi[k];
        cmult(gr[k], gi[k], ref tr, ref ti);
        gr[k] = tr * v;
        gi[k] = ti * v;

        cmult(fr[k], fi[k], ref gr[k], ref gi[k]);
      }

      FastHartleyTransform.IFFT(gr,gi,n); // FFT(gr, gi, ldn, -is);
    }


    /// <summary>
    /// Performs an FFT of arbitrary length by the chirp method. Use this method only if no other
    /// FFT is applicable.
    /// </summary>
    /// <param name="x">Array of real values.</param>
    /// <param name="y">Array of imaginary values.</param>
    /// <param name="n">Number of points to transform.</param>
    /// <param name="backward">If false, a forward FFT is performed. If true, a inverse FFT is performed.</param>
    static void
      FFT_DONTUSEIT(double[] x, double[] y, uint n, bool backward)
      //
      // arbitrary length fft
      //
    {
      int ldnn = ld(n);
      if ( n==(1U<<ldnn) )  ldnn += 1;
      else                   ldnn += 2;
      uint nn = (1U<<ldnn);

      // allocate temporary arrays
      double[] fr = new double[nn];
      double[] fi = new double[nn];

      //    ri_copy(x,y,fr,fi,n);
      Array.Copy(x,0,fr,0,n);
      Array.Copy(y,0,fi,0,n);

      // note: the rest of the array is already zero, but in other languages it
      // must be set to zero here
      //null(fr+n, nn-n);
      //null(fi+n, nn-n);

      double[] wr = new double[nn];
      double[] wi = new double[nn];

      //    make_fft_fract_chirp(wr,wi,1.0*is,n,nn);
      make_fft_chirp(wr,wi,n,backward);
      complete_fft_chirp(wr,wi,n,nn);

      ri_multiply(wr,wi,fr,fi,n);

      negate(wi,nn);
      fft_complex_convolution(wr,wi,fr,fi,(uint)ldnn,0);

      //    make_fft_fract_chirp(wr,wi,1.0*is,n,nn);
      make_fft_chirp(wr,wi,n,backward);
      complete_fft_chirp(wr,wi,n,nn);

      ri_multiply(fr,fi,wr,wi,nn);

      //    ri_copy(wr+n,wi+n,x,y,n);
      // copy(wr+n, x, n);
      // copy(wi+n, y, n);

      Array.Copy(wr,n,x,0,n);
      Array.Copy(wi,n,y,0,n);
    
    }


  }
}
