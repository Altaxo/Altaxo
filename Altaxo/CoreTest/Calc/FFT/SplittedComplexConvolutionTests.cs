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
using NUnit.Framework;
using Altaxo.Calc.Fourier;

namespace AltaxoTest.Calc.Fourier
{
  /// <summary>
  /// Serves as template class for Tests of Convolution.
  /// </summary>
  public class SplittedComplexConvolutionTests
  {
    /// <summary>
    /// Delegate which is the function pointer type of the fourier transformation.
    /// </summary>
    public delegate void ConvolutionRoutine(double[] src1real, double[] src1imag, double[] src2real, double[] src2imag, double[] resultreal, double[] resultimag, int n);

    /// <summary>
    /// Function pointer to the used Fourier transformation routine.
    /// </summary>
    private ConvolutionRoutine _conv;

    private double maxTolerableEpsPerN=5E-15;

    public double SetTolerance(double tolerance)
    {
      double old = maxTolerableEpsPerN;
      maxTolerableEpsPerN=tolerance;
      return old;
    }

    private double max_conv_error(int n)
    {
      return (double)n * maxTolerableEpsPerN;
    }

    /// <summary>
    /// Initializes this class.
    /// </summary>
    /// <param name="routine">Pointer to the fourier transformation routine.</param>
    public SplittedComplexConvolutionTests(ConvolutionRoutine routine)
    {
      _conv = routine;
    }

    /// <summary>
    /// Tests the transform with both arrays filled with zeros.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestBothZero(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];
 
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual(0, re[i],max_conv_error(n),"Convolution of zero should give re=0");
        Assert.AreEqual( 0, im[i],max_conv_error(n),"Convolution of zero should give im=0");
      }
    }

    /// <summary>
    /// Tests the transform with both arrays filled with zeros.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestOneZero(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];
 
      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        im1[i] = 0;
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }

      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],max_conv_error(n),"Convolution with array 1 zero should give re=0");
        Assert.AreEqual( 0, im[i],max_conv_error(n),"Convolution with array 1 zero should give im=0");
      }

    
      
      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = 0;
        im2[i] = 0;
      }

      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],max_conv_error(n),"Convolution with array 2 zero should give re=0");
        Assert.AreEqual( 0, im[i],max_conv_error(n),"Convolution with array 2 zero should give im=0");
      }


    }

    public void TestReOne_ZeroPos(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      re1[0] = 1;
      re2[0] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {
        if(i==0)
        {
          Assert.AreEqual( 1, re[i],max_conv_error(n),"Convolution should give re=1 at pos 0");
          Assert.AreEqual( 0, im[i],max_conv_error(n),"Convolution should give im=0 at pos 0");
        }
        else
        {
          Assert.AreEqual( 0, re[i],max_conv_error(n),"Convolution should give re=0 at pos " + i.ToString());
          Assert.AreEqual( 0, im[i],max_conv_error(n),"Convolution should give im=0 at pos " + i.ToString());
        }
      }
    }

    public void TestOneReOne_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        im1[i] = 0;
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }


      re1[0] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re2[i], re[i],max_conv_error(n),"Convolution should give re=re2 at pos " + i.ToString());
        Assert.AreEqual( im2[i], im[i],max_conv_error(n),"Convolution should give im=im2 at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = 0;
        im2[i] = 0;
      }


      re2[0] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re1[i], re[i],max_conv_error(n),"Convolution should give re=re1 at pos " + i.ToString());
        Assert.AreEqual( im1[i], im[i],max_conv_error(n),"Convolution should give im=im1 at pos " + i.ToString());
      }

    }


    public void TestOneImOne_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        im1[i] = 0;
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }


      im1[0] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( -im2[i], re[i],max_conv_error(n),"Convolution should give re=-im2 at pos " + i.ToString());
        Assert.AreEqual( re2[i], im[i],max_conv_error(n),"Convolution should give im=re2 at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = 0;
        im2[i] = 0;
      }


      im2[0] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( -im1[i], re[i],max_conv_error(n),"Convolution should give re=-im1 at pos " + i.ToString());
        Assert.AreEqual( re1[i], im[i],max_conv_error(n),"Convolution should give im=im1 at pos " + i.ToString());
      }
    }

    public void TestReOne_OnePos_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        im1[i] = 0;
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }


      re1[1] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re2[(n+i-1)%n], re[i],max_conv_error(n),"Convolution should give re=re2[i-1] at pos " + i.ToString());
        Assert.AreEqual( im2[(n+i-1)%n], im[i],max_conv_error(n),"Convolution should give im=im2[i-1] at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = 0;
        im2[i] = 0;
      }


      re2[1] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re1[(n+i-1)%n], re[i],max_conv_error(n), "Convolution should give re=re1[i-1] at pos " + i.ToString());
        Assert.AreEqual(im1[(n+i-1)%n], im[i],max_conv_error(n), "Convolution should give im=im1[i-1] at pos " + i.ToString());
      }

    }

    public void TestImOne_OnePos_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        im1[i] = 0;
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }


      im1[1] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( -im2[(n+i-1)%n], re[i],max_conv_error(n),"Convolution should give re=-im2 at pos " + i.ToString());
        Assert.AreEqual( re2[(n+i-1)%n], im[i],max_conv_error(n),"Convolution should give im=re2 at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = 0;
        im2[i] = 0;
      }


      im2[1] = 1;
  
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( -im1[(n+i-1)%n], re[i],max_conv_error(n), "Convolution should give re=-im1 at pos " + i.ToString());
        Assert.AreEqual( re1[(n+i-1)%n], im[i],max_conv_error(n),"Convolution should give im=im1 at pos " + i.ToString());
      }
    }


    public void TestBothRandom(int n)
    {
      double[] re1 = new double[n];
      double[] im1 = new double[n];
      double[] re2 = new double[n];
      double[] im2 = new double[n];
      double[] re = new double[n];
      double[] im = new double[n];

      double[] recmp = new double[n];
      double[] imcmp = new double[n];


      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        im1[i] = rnd.NextDouble();
        re2[i] = rnd.NextDouble();
        im2[i] = rnd.NextDouble();
      }
 
      NativeFourierMethods.CyclicConvolution(re1,im1,re2,im2,recmp,imcmp,n);
      _conv(re1,im1, re2, im2, re, im, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( recmp[i], re[i],max_conv_error(n), "Convolution should give re=recmp at pos " + i.ToString());
        Assert.AreEqual( imcmp[i], im[i],max_conv_error(n),"Convolution should give im=imcmp at pos " + i.ToString());
      }
    }
  }
}
