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
  /// This class provides reference methods concerning FFT that are slow and not very accurate.
  /// Do not use them except for comparism and testing purposes!
  /// The direction of the forward Fourier transform will be defined here as reference for all other Fourier methods as
  /// multiplication of f(x)*exp(iwt), i.e. as (wt) having a positive sign (forward) and wt having a negative sign (backward).
  /// </summary>
  public class NativeFourierMethods
  {
    

    /// <summary>
    /// Performes a cyclic correlation between array arr1 and arr2 and stores the result in resultarr. Resultarr must be
    /// different from the other two arrays. 
    /// </summary>
    /// <param name="arr1">First array (kernel).</param>
    /// <param name="arr2">Second array (data).</param>
    /// <param name="resultarr">The array that stores the correleation result.</param>
    /// <param name="count">Number of points to correlate.</param>
    /// <remarks>Correlation of src1 with src2 is not the same as correlation of src2 with src1.
    /// The correlation here is defined as corr(src1,src2)(j)=SUM(k) src1(k) src2(k+j).</remarks>
    public static  void CyclicCorrelation(double[] arr1, double[] arr2, double[] resultarr, int count)
    {
      if(object.ReferenceEquals(resultarr,arr1) || object.ReferenceEquals(resultarr,arr2))
        throw new ArgumentException("Resultarr must not be identical to arr1 or arr2!");

      int i,k;
      for(k=0;k<count;k++)
      {
        resultarr[k]=0;
        for(i=0;i<count;i++)
        {
          resultarr[k] += arr1[i]*arr2[(i+k)%count];
        }
      }
    }

    /// <summary>
    /// Performes a cyclic correlation between splitted complex arrays and stores the result in resultarr. Resultarr must be
    /// different from the input arrays. 
    /// </summary>
    /// <param name="src1real">First array (kernel, real part values).</param>
    /// <param name="src1imag">First array (kernel, imaginary part values).</param>
    /// <param name="src2real">Second array (data, real part values).</param>
    /// <param name="src2imag">Second array (data, imaginary part values).</param>
    /// <param name="resultreal">The array that stores the correlation result (real part values.</param>
    /// <param name="resultimag">The array that stores the correlation result (imaginary part values.</param>
    /// <param name="n">Number of points to correlate.</param>
    /// <remarks>Correlation of src1 with src2 is not the same as correlation of src2 with src1.
    /// The correlation here is defined as corr(src1,src2)(j)=SUM(k) src1(k) src2(k+j).</remarks>
    public static void CyclicCorrelation( 
      double[] src1real, double[]src1imag,
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      if(object.ReferenceEquals(resultreal,src1real) || 
        object.ReferenceEquals(resultreal,src1imag) ||
        object.ReferenceEquals(resultreal,src2real) ||
        object.ReferenceEquals(resultreal,src2imag))
        throw new ArgumentException("resultreal must not be identical to one of the input arrays!");
      if(object.ReferenceEquals(resultimag,src1real) || 
        object.ReferenceEquals(resultimag,src1imag) ||
        object.ReferenceEquals(resultimag,src2real) ||
        object.ReferenceEquals(resultimag,src2imag))
        throw new ArgumentException("resultimag must not be identical to one of the input arrays!");
      
      int i,k,s;
      for(k=0;k<n;k++)
      {
        resultreal[k]=0;
        resultimag[k]=0;
        for(i=0;i<n;i++)
        { 
          s = (i+k)%n;
          resultreal[k] += src1real[i]*src2real[s] - src1imag[i]*src2imag[s];
          resultimag[k] += src1real[i]*src2imag[s] + src1imag[i]*src2real[s];
        }
      }
    }


    /// <summary>
    /// Performes a cyclic convolution between array arr1 and arr2 and stores the result in resultarr. Resultarr must be
    /// different from the other two arrays. 
    /// </summary>
    /// <param name="arr1">First array.</param>
    /// <param name="arr2">Second array.</param>
    /// <param name="resultarr">The array that stores the correleation result.</param>
    /// <param name="count">Number of points to correlate.</param>
    public static void CyclicConvolution( double[] arr1, double[] arr2, double[] resultarr, int count)
    {
      if(object.ReferenceEquals(resultarr,arr1) || object.ReferenceEquals(resultarr,arr2))
        throw new ArgumentException("resultarr must not be identical to arr1 or arr2!");
      
      int i,k;
      for(k=0;k<count;k++)
      {
        resultarr[k]=0;
        for(i=0;i<count;i++)
        {
          resultarr[k] += arr1[i]*arr2[(count+k-i)%count];
        }
      }
    }


    /// <summary>
    /// Performes a cyclic convolution between splitted complex arrays and stores the result in resultarr. Resultarr must be
    /// different from the input arrays. 
    /// </summary>
    /// <param name="src1real">First array (real part values).</param>
    /// <param name="src1imag">First array (imaginary part values).</param>
    /// <param name="src2real">Second array (real part values).</param>
    /// <param name="src2imag">Second array (imaginary part values).</param>
    /// <param name="resultreal">The array that stores the correlation result (real part values.</param>
    /// <param name="resultimag">The array that stores the correlation result (imaginary part values.</param>
    /// <param name="n">Number of points to correlate.</param>
    public static void CyclicConvolution( 
      double[] src1real, double[]src1imag,
      double[] src2real, double[] src2imag,
      double[] resultreal, double[] resultimag,
      int n)
    {
      if(object.ReferenceEquals(resultreal,src1real) || 
        object.ReferenceEquals(resultreal,src1imag) ||
        object.ReferenceEquals(resultreal,src2real) ||
        object.ReferenceEquals(resultreal,src2imag))
        throw new ArgumentException("resultreal must not be identical to one of the input arrays!");
      if(object.ReferenceEquals(resultimag,src1real) || 
        object.ReferenceEquals(resultimag,src1imag) ||
        object.ReferenceEquals(resultimag,src2real) ||
        object.ReferenceEquals(resultimag,src2imag))
        throw new ArgumentException("resultimag must not be identical to one of the input arrays!");
      
      int i,k,s;
      for(k=0;k<n;k++)
      {
        resultreal[k]=0;
        resultimag[k]=0;
        for(i=0;i<n;i++)
        { 
          s = (n+k-i)%n;
          resultreal[k] += src1real[i]*src2real[s] - src1imag[i]*src2imag[s];
          resultimag[k] += src1real[i]*src2imag[s] + src1imag[i]*src2real[s];
        }
      }
    }

    /// <summary>
    /// Performs a native fouriertransformation of a real value array.
    /// </summary>
    /// <param name="arr">The double valued array to transform.</param>
    /// <param name="resultarr">Used to store the result of the transformation.</param>
    /// <param name="count">Number of points to transform.</param>
    /// <param name="direction">Direction of the Fourier transform.</param>
    public static void FFT(double[] arr, Complex[] resultarr, int count, FourierDirection direction)
    {
      int iss = direction==FourierDirection.Forward ? 1 : -1;
      for(int k=0;k<count;k++)
      {
        resultarr[k]=new Complex(0,0);
        for(int i=0;i<count;i++)
        {
          double phi = iss*2*Math.PI*((i*k)%count)/count;
          resultarr[k] += new Complex(arr[i]*Math.Cos(phi),arr[i]*Math.Sin(phi));
        }
      }
    }

    /// <summary>
    /// Performs a inline native fouriertransformation of real and imaginary part arrays.
    /// </summary>
    /// <param name="real">The real part of the array to transform.</param>
    /// <param name="imag">The real part of the array to transform.</param>
    /// <param name="direction">Direction of the Fourier transform.</param>
    public static void FFT(double[] real, double[] imag, FourierDirection direction)
    {
      if(real.Length!=imag.Length)
        throw new ArgumentException("Length of real and imaginary array do not match!");

      FFT(real, imag, real, imag, real.Length, direction);
    }

    /// <summary>
    /// Performs a native fouriertransformation of a complex value array.
    /// </summary>
    /// <param name="inputreal">The real part of the array to transform.</param>
    /// <param name="inputimag">The real part of the array to transform.</param>
    /// <param name="resultreal">Used to store the real part of the result of the transformation. May be equal to the input array.</param>
    /// <param name="resultimag">Used to store the imaginary part of the result of the transformation.  May be equal to the input array.</param>
    /// <param name="count">Number of points to transform.</param>
    /// <param name="direction">Direction of the Fourier transform.</param>
    public static void FFT(double[] inputreal, double[] inputimag, double[] resultreal, double[] resultimag, int count,FourierDirection direction)
    {
      bool useShadowCopy = false;
      double[] resre = resultreal;
      double[] resim = resultimag;

      if( object.ReferenceEquals(resultreal,inputreal) || 
        object.ReferenceEquals(resultreal,inputimag) ||
        object.ReferenceEquals(resultimag,inputreal) ||
        object.ReferenceEquals(resultimag,inputimag)
        )
        useShadowCopy = true;

      if(useShadowCopy)
      {
        resre = new double[count];
        resim = new double[count];
      }

      int iss = direction==FourierDirection.Forward ? 1 : -1;
      for(int k=0;k<count;k++)
      {
        double sumreal=0, sumimag=0;
        for(int i=0;i<count;i++)
        {
          double phi = iss*2*Math.PI*((i*k)%count)/count;
          double  vre = Math.Cos(phi);
          double vim = Math.Sin(phi);
          double addre = inputreal[i]*vre - inputimag[i]*vim;
          double addim = inputreal[i]*vim + inputimag[i]*vre;
          sumreal += addre;
          sumimag += addim;
        }
        resre[k] = sumreal;
        resim[k] = sumimag;
      }

      if(useShadowCopy)
      {
        Array.Copy(resre,0,resultreal,0,count);
        Array.Copy(resim,0,resultimag,0,count);
      }
    }


    /// <summary>
    /// Performs a inline native fouriertransformation of real and imaginary part arrays.
    /// </summary>
    /// <param name="real">The real part of the array to transform.</param>
    /// <param name="direction">Direction of the Fourier transform.</param>
    public static void FFT(double[] real, FourierDirection direction)
    {
      FFT(real, real, real.Length, direction);
    }

    /// <summary>
    /// Performs a native fouriertransformation of a real value array.
    /// </summary>
    /// <param name="inputreal">The real part of the array to transform.</param>
    /// <param name="resultreal">Used to store the real part of the result of the transformation. May be equal to the input array.</param>
    /// <param name="count">Number of points to transform.</param>
    /// <param name="direction">Direction of the Fourier transform.</param>
    public static void FFT(double[] inputreal, double[] resultreal,  int count,FourierDirection direction)
    {
      bool useShadowCopy = false;
      double[] resre = resultreal;

      if( object.ReferenceEquals(resultreal,inputreal))
        useShadowCopy = true;

      if(useShadowCopy)
      {
        resre = new double[count];
      }

      int iss = direction==FourierDirection.Forward ? 1 : -1;

      if(direction==FourierDirection.Forward)
      {
        for(int k=0;k<=count/2;k++)
        {
          double sumreal=0, sumimag=0;
          for(int i=0;i<count;i++)
          {
            double phi = iss*2*Math.PI*((i*k)%count)/count;
            double vre = Math.Cos(phi);
            double vim = Math.Sin(phi);
            double addre = inputreal[i]*vre;
            double addim = inputreal[i]*vim;
            sumreal += addre;
            sumimag += addim;
          }
          if(k!=0 && (k+k)!=count)
            resre[count-k] = sumimag; 
          resre[k] = sumreal;
        }

      }
      else // FourierDirection.Inverse
      {
        for(int k=0;k<count;k++)
        {
          double sumreal=0, sumimag=0;
          sumreal = inputreal[0];
          int i,j;
          for(i=1,j=count-1;i<=j;i++,j--)
          {
            double phi = iss*2*Math.PI*((i*k)%count)/count;
            double vre = Math.Cos(phi);
            double vim = Math.Sin(phi);
            double addre = inputreal[i]*vre - inputreal[count-i]*vim;
            double addim = inputreal[i]*vim + inputreal[count-i]*vre;

            if(i!=j)
            {
              sumreal += addre*2;
              sumimag += addim*2;
            }
            else
            {
              sumreal += addre;
              sumimag += addim;
            }
          }
          resre[k] = sumreal;
        }

      }


      if(useShadowCopy)
      {
        Array.Copy(resre,0,resultreal,0,count);
      }
    }


  }

}
