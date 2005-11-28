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
using NUnit.Framework;
using Altaxo.Calc.Fourier;

namespace AltaxoTest.Calc.Fourier
{
  /// <summary>
  /// Serves as template class for Tests of Convolution.
  /// </summary>
  public class RealConvolutionTests
  {
    /// <summary>
    /// Delegate which is the function pointer type of the fourier transformation.
    /// </summary>
    public delegate void ConvolutionRoutine(double[] src1, double[] src2, double[] result, int n);

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
    public RealConvolutionTests(ConvolutionRoutine routine)
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
      double[] re2 = new double[n];
      double[] re = new double[n];
 
      _conv(re1, re2, re,  n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],max_conv_error(n), "Convolution of zero should give re=0");
      }
    }

    /// <summary>
    /// Tests the transform with one arrays filled with zeros.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestOneZero(int n)
    {
      double[] re1 = new double[n];
      double[] re2 = new double[n];
      double[] re = new double[n];
 
      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        re2[i] = rnd.NextDouble();
      }

      _conv(re1, re2, re, n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],max_conv_error(n),"Convolution with array 1 zero should give re=0");
      }

    
      
      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        re2[i] = 0;
      }

      _conv(re1, re2, re, n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],max_conv_error(n),"Convolution with array 2 zero should give re=0");
      }


    }

    /// <summary>
    /// Tests the transform with both arrays zero except for position 0.
    /// </summary>
    /// <param name="n">Number of transformation points.</param>
    public void TestReOne_ZeroPos(int n)
    {
      double[] re1 = new double[n];
      double[] re2 = new double[n];
      double[] re = new double[n];

      re1[0] = 1;
      re2[0] = 1;
  
      _conv(re1,re2,re,n);

      for(int i=0;i<n;i++)
      {
        if(i==0)
        {
          Assert.AreEqual( 1, re[i],max_conv_error(n), "Convolution should give re=1 at pos 0");
        }
        else
        {
          Assert.AreEqual( 0, re[i],max_conv_error(n), "Convolution should give re=0 at pos " + i.ToString());
        }
      }
    }

    public void TestOneReOne_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] re2 = new double[n];
      double[] re = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        re2[i] = rnd.NextDouble();
      }


      re1[0] = 1;
  
      _conv(re1,re2, re,  n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re2[i], re[i],max_conv_error(n),"Convolution should give re=re2 at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        re2[i] = 0;
      }


      re2[0] = 1;
  
      _conv(re1, re2,  re, n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual(re1[i], re[i],max_conv_error(n),"Convolution should give re=re1 at pos " + i.ToString());
      }

    }



    public void TestReOne_OnePos_OtherRandom(int n)
    {
      double[] re1 = new double[n];
      double[] re2 = new double[n];
      double[] re = new double[n];

      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = 0;
        re2[i] = rnd.NextDouble();
      }


      re1[1] = 1;
  
      _conv(re1, re2,  re,  n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re2[(n+i-1)%n], re[i],max_conv_error(n),"Convolution should give re=re2[i-1] at pos " + i.ToString());
      }

      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        re2[i] = 0;
      }


      re2[1] = 1;
  
      _conv(re1, re2,  re,  n);

      for(int i=0;i<n;i++)
      {

        Assert.AreEqual( re1[(n+i-1)%n], re[i],max_conv_error(n),"Convolution should give re=re1[i-1] at pos " + i.ToString());
      }

    }



    public void TestBothRandom(int n)
    {
      double[] re1 = new double[n];
      double[] re2 = new double[n];
      double[] re = new double[n];

      double[] recmp = new double[n];


      System.Random rnd = new System.Random();
 
      
      for(int i=0;i<n;i++)
      {
        re1[i] = rnd.NextDouble();
        re2[i] = rnd.NextDouble();
      }
 
      NativeFourierMethods.CyclicConvolution(re1,re2,recmp,n);
      _conv(re1,re2,re,n);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( recmp[i], re[i],max_conv_error(n),"Convolution should give re=recmp at pos " + i.ToString());
      }
    }
  }
}
