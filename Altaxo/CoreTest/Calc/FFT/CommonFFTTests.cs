#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using NUnit.Framework;
using Altaxo.Calc.FFT;

namespace AltaxoTest.Calc.FFT
{
  #region SplittedComplexFFTTests
  /// <summary>
  /// Serves as template class for FFT-Tests.
  /// </summary>
  public class SplittedComplexFFTTests
  {
    /// <summary>
    /// Delegate which is the function pointer type of the fourier transformation.
    /// </summary>
    public delegate void FFTRoutine(double[] re, double[] im, FourierDirection dir);

    /// <summary>
    /// Function pointer to the used Fourier transformation routine.
    /// </summary>
    private FFTRoutine _fft;

    private double maxTolerableEpsPerN=5E-15;

    public double SetTolerance(double tolerance)
    {
      double old = maxTolerableEpsPerN;
      maxTolerableEpsPerN=tolerance;
      return old;
    }

    private double max_fft_error(int n)
    {
      return (double)n * maxTolerableEpsPerN;
    }

    private double max_ifft_error(int n)
    {
      return (double)n * (double)n * maxTolerableEpsPerN;
    }



    /// <summary>
    /// Initializes this class.
    /// </summary>
    /// <param name="routine">Pointer to the fourier transformation routine.</param>
    public SplittedComplexFFTTests(FFTRoutine routine)
    {
      _fft = routine;
    }


    /// <summary>
    /// Tests the transform with both arrays filled with zeros.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestZero(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];
 
      _fft(re,im,FourierDirection.Forward);

      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of zero should give re=0", 0, re[i],max_fft_error(n));
        Assertion.AssertEquals("FFT of zero should give im=0", 0, im[i],max_fft_error(n));
      }

      _fft(re,im,FourierDirection.Inverse);

      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals("IFFT of zero should give re=0", 0, re[i],max_ifft_error(n));
        Assertion.AssertEquals("IFFT of zero should give im=0", 0, im[i],max_ifft_error(n));
      }

    }

    public void TestReOne_ZeroPos(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      re[0] = 1;
  
      _fft(re,im,FourierDirection.Forward);
      
      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[i],max_fft_error(n));
        Assertion.AssertEquals("FFT of 1 at pos 0 should give im=0", 0, im[i],max_fft_error(n));
      }

      _fft(re,im,FourierDirection.Inverse);
      for(int i=0;i<n;i++)
      {
        if(i==0)
        {
          Assertion.AssertEquals("IFFT at pos 0 should give re=n", n, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at pos 0 should give im=0", 0, im[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=0 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at all pos!=0 should give im=0", 0, im[i],max_ifft_error(n));
        }
      }
    }

    /// <summary>
    /// Tests all real and imaginary part zero, but im[0]=1.
    /// </summary>
    /// <param name="n">Length of fourier transform.</param>
    public void TestImOne_ZeroPos(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      im[0] = 1;

      _fft(re,im,FourierDirection.Forward);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of im 1 at pos 0 should give re=0", 0, re[i],max_fft_error(n));
        Assertion.AssertEquals("FFT of im 1 at pos 0 should give im=1", 1, im[i],max_fft_error(n));
      }

      _fft(re,im,FourierDirection.Inverse);

      for(uint i=0;i<n;i++)
      {
        if(i==0)
        {
          Assertion.AssertEquals("IFFT at pos 0 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at pos 0 should give im=n", n, im[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=0 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at all pos!=0 should give im=0", 0, im[i],max_ifft_error(n));
        }
      }
    }

    /// <summary>
    /// Test: all real and imag is zero, but re[1]=1.
    /// </summary>
    /// <param name="n">Length of FFT.</param>
    public void TestReOne_OnePos(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      re[1] = 1;

      _fft(re,im,FourierDirection.Forward);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 re[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), re[i],max_fft_error(n));
        Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 im[{1}]",n,i), Math.Sin((2*Math.PI*i)/n), im[i],max_fft_error(n));
      }

      _fft(re,im,FourierDirection.Inverse);

      for(uint i=0;i<n;i++)
      {
        if(i==1)
        {
          Assertion.AssertEquals("IFFT at pos 1 should give re=n", n, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at pos 1 should give im=0", 0, im[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=1 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at all pos!=1 should give im=0", 0, im[i],max_ifft_error(n));
        }
 
      }
    }

    public void TestImOne_OnePos(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      im[1] = 1;

     
      _fft(re,im,FourierDirection.Forward);

      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 re[{1}]",n,i), -Math.Sin((2*Math.PI*i)/n), re[i],max_fft_error(n));
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 im[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), im[i],max_fft_error(n));
      }
    
      _fft(re,im,FourierDirection.Inverse);

      for(int i=0;i<n;i++)
      {
 
        if(i==1)
        {
          Assertion.AssertEquals("IFFT at pos 1 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at pos 1 should give im=n", n, im[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=1 should give re=0", 0, re[i],max_ifft_error(n));
          Assertion.AssertEquals("IFFT at all pos!=1 should give im=0", 0, im[i],max_ifft_error(n));
        }
      }
    }

    public void TestReImOne_RandomPos(int n, int repeats)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      
      System.Random rnd = new System.Random();

      for(;repeats>=0;--repeats)
      {
        for(int i=0;i<n;i++)
        {
          re[i] = 0;
          im[i] = 0;
        }

        int repos = rnd.Next((int)n);
        int impos = rnd.Next((int)n);

        re[repos]=1;
        im[impos]=1;

     

        _fft(re,im,FourierDirection.Forward);

        for(int i=0;i<n;i++)
        {
          Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) re[{3}]",n,repos,impos,i), 
            Math.Cos((2*Math.PI*i*(double)repos)/n) - Math.Sin((2*Math.PI*i*(double)impos)/n),
            re[i],max_fft_error(n));
          Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) arb im[{3}]",n,repos,impos,i), 
            Math.Sin((2*Math.PI*i*(double)repos)/n) + Math.Cos((2*Math.PI*i*(double)impos)/n), 
            im[i],max_fft_error(n));
        }

        _fft(re,im,FourierDirection.Inverse);

        for(int i=0;i<n;i++)
        {

          if(i==repos)
            Assertion.AssertEquals("IFFT at pos==repos should give re=n", n, re[i],max_ifft_error(n));
          else
            Assertion.AssertEquals("IFFT at all pos!=repos should give re=0", 0, re[i],max_ifft_error(n));

          if(i==impos)
            Assertion.AssertEquals("IFFT at pos==impos should give im=n", n, im[i],max_ifft_error(n));
          else
            Assertion.AssertEquals("IFFT at all pos!=impos should give im=0", 0, im[i],max_ifft_error(n));
        }
        
      }
    }

    public void TestReImRandomValues(int n)
    {
      double[] re = new double[n];
      double[] im = new double[n];
    
      double[] re1 = new double[n];
      double[] im1 = new double[n];

      System.Random rnd = new System.Random();

      // fill re and im with random values
      for(int i=0;i<n;i++)
      {
        re[i] = rnd.NextDouble();
        im[i] = rnd.NextDouble();
      }


      Array.Copy(re,0,re1,0,n);
      Array.Copy(im,0,im1,0,n);

      _fft(re,im,FourierDirection.Forward);
      NativeFourierMethods.FFT(re1,im1,re1,im1,(int)n, FourierDirection.Forward);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT (real part) at pos {0}",i), 
          re1[i],
          re[i],max_fft_error(n));
        Assertion.AssertEquals(string.Format("FFT (imaginary part) at pos {0}",i), 
          im1[i], 
          im[i],max_fft_error(n));
      }


      // now test the IFFT
      // fill re and im with random values
      for(int i=0;i<n;i++)
      {
        re[i] = rnd.NextDouble();
        im[i] = rnd.NextDouble();
      }

      Array.Copy(re,0,re1,0,n);
      Array.Copy(im,0,im1,0,n);

      _fft(re,im,FourierDirection.Inverse);
      NativeFourierMethods.FFT(re1,im1,re1,im1,(int)n, FourierDirection.Inverse);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("IFFT (real part) at pos {0}",i), 
          re1[i],
          re[i],max_fft_error(n));
        Assertion.AssertEquals(string.Format("IFFT (imaginary part) at pos {0}",i), 
          im1[i], 
          im[i],max_fft_error(n));
      }


    }
  }

  #endregion

  #region RealFFTTests
  /// <summary>
  /// Serves as template class for FFT-Tests.
  /// </summary>
  public class RealFFTTests
  {
    /// <summary>
    /// Delegate which is the function pointer type of the fourier transformation.
    /// </summary>
    public delegate void FFTRoutine(double[] re, FourierDirection dir);

    /// <summary>
    /// Function pointer to the used Fourier transformation routine.
    /// </summary>
    private FFTRoutine _fft;

    private double maxTolerableEpsPerN=5E-15;

    public double SetTolerance(double tolerance)
    {
      double old = maxTolerableEpsPerN;
      maxTolerableEpsPerN=tolerance;
      return old;
    }

    private double max_fft_error(int n)
    {
      return (double)n * maxTolerableEpsPerN;
    }

    private double max_ifft_error(int n)
    {
      return (double)n * (double)n * maxTolerableEpsPerN;
    }



    /// <summary>
    /// Initializes this class.
    /// </summary>
    /// <param name="routine">Pointer to the fourier transformation routine.</param>
    public RealFFTTests(FFTRoutine routine)
    {
      _fft = routine;
    }


    /// <summary>
    /// Tests the transform with both arrays filled with zeros.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestZero(int n)
    {
      double[] re = new double[n];
 
      _fft(re,FourierDirection.Forward);

      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of zero should give re=0", 0, re[i],max_fft_error(n));
      }

      _fft(re,FourierDirection.Inverse);

      for(int i=0;i<n;i++)
      {
        Assertion.AssertEquals("IFFT of zero should give re=0", 0, re[i],max_ifft_error(n));
      }

    }

    public void TestReOne_ZeroPos(int n)
    {
      double[] re = new double[n];

      re[0] = 1;
  
      _fft(re,FourierDirection.Forward);
 
      Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[0],max_fft_error(n));
      for(int i=1,j=n-1;i<=j;i++,j--)
      {
        Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[i],max_fft_error(n));
        if(i<j)
          Assertion.AssertEquals("FFT of 1 at pos 0 should give im=0", 0, re[n-i],max_fft_error(n));
      }

      _fft(re,FourierDirection.Inverse);
      for(int i=0;i<n;i++)
      {
        if(i==0)
        {
          Assertion.AssertEquals("IFFT at pos 0 should give re=n", n, re[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=0 should give re=0", 0, re[i],max_ifft_error(n));
        }
      }
    }

    /// <summary>
    /// Test: all real and imag is zero, but re[1]=1.
    /// </summary>
    /// <param name="n">Length of FFT.</param>
    public void TestReOne_OnePos(int n)
    {
      double[] re = new double[n];

      re[1] = 1;

      _fft(re,FourierDirection.Forward);

      Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[0],max_fft_error(n));
      for(int i=1,j=n-1;i<=j;i++,j--)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 re[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), re[i],max_fft_error(n));
       
        if(i<j)
          Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 im[{1}]",n,i), Math.Sin((2*Math.PI*i)/n), re[n-i],max_fft_error(n));
      }

      _fft(re,FourierDirection.Inverse);

      for(uint i=0;i<n;i++)
      {
        if(i==1)
        {
          Assertion.AssertEquals("IFFT at pos 1 should give re=n", n, re[i],max_ifft_error(n));
        }
        else
        {
          Assertion.AssertEquals("IFFT at all pos!=1 should give re=0", 0, re[i],max_ifft_error(n));
        }
 
      }
    }

    public void TestReOne_RandomPos(int n, int repeats)
    {
      double[] re = new double[n];

      
      System.Random rnd = new System.Random();

      for(;repeats>=0;--repeats)
      {
        for(int i=0;i<n;i++)
        {
          re[i] = 0;
        }

        int repos = rnd.Next((int)n);

        re[repos]=1;

        _fft(re,FourierDirection.Forward);

        Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[0],max_fft_error(n));
        for(int i=1,j=n-1;i<=j;i++,j--)
        {
          Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1}) re[{2}]",n,repos,i), 
            Math.Cos((2*Math.PI*i*(double)repos)/n),
            re[i],max_fft_error(n));
          if(i<j)
            Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1}) arb im[{2}]",n,repos,i), 
              Math.Sin((2*Math.PI*i*(double)repos)/n), 
              re[n-i],max_fft_error(n));
        }

        _fft(re,FourierDirection.Inverse);

        for(int i=0;i<n;i++)
        {

          if(i==repos)
            Assertion.AssertEquals("IFFT at pos==repos should give re=n", n, re[i],max_ifft_error(n));
          else
            Assertion.AssertEquals("IFFT at all pos!=repos should give re=0", 0, re[i],max_ifft_error(n));
        }
        
      }
    }

    public void TestReRandomValues(int n)
    {
      double[] re = new double[n];
    
      double[] re1 = new double[n];

      System.Random rnd = new System.Random();

      // fill re and im with random values
      for(int i=0;i<n;i++)
      {
        re[i] = rnd.NextDouble();
      }


      Array.Copy(re,0,re1,0,n);

      _fft(re,FourierDirection.Forward);
      NativeFourierMethods.FFT(re1,re1,(int)n, FourierDirection.Forward);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT (real part) at pos {0}",i), 
          re1[i],
          re[i],max_fft_error(n));
      }


      // now test the IFFT
      // fill re and im with random values
      for(int i=0;i<n;i++)
      {
        re[i] = rnd.NextDouble();
      }

      Array.Copy(re,0,re1,0,n);

      _fft(re,FourierDirection.Inverse);
      NativeFourierMethods.FFT(re1,re1,(int)n, FourierDirection.Inverse);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("IFFT (real part) at pos {0}",i), 
          re1[i],
          re[i],max_fft_error(n));
      }


    }
  }

  #endregion
}
