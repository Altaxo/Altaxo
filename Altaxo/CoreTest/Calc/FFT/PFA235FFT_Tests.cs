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

  [TestFixture]
  public class TestPFA235FFT_1D
  {
    SplittedComplexFFTTests _test;
    int[] _testLengths = { 2, 3, 2*2, 5, 2*3, 2*2*2, 3*3, 2*3*5, 3*5*5, 2*2*3*3*5*5 };

    public TestPFA235FFT_1D()
    {
      _test = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(MyFFT));
     
    }

    void MyFFT(double[] real, double[] imag, FourierDirection direction)
    {
      Pfa235FFT fft = new Pfa235FFT(real.Length);
      fft.FFT(real, imag, direction);
    }


    [Test]
    public void Test01Zero()
    {
      foreach(int i in _testLengths)
        _test.TestZero(i);
    }

    [Test]
    public void Test02ReOne_ZeroPos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_ZeroPos(i);
    }

    [Test]
    public void Test03ImOne_ZeroPos()
    {
      foreach(int i in _testLengths)
        _test.TestImOne_ZeroPos(i);
    }

    [Test]
    public void Test04ReOne_OnePos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_OnePos(i);
    }

    [Test]
    public void Test05ImOne_OnePos()
    {
      foreach(int i in _testLengths)
        _test.TestImOne_OnePos(i);
    }

    [Test]
    public void Test06ReImOne_RandomPos()
    {
      foreach(int i in _testLengths)
        _test.TestReImOne_RandomPos(i,10);
    }

    [Test]
    public void Test07ReImRandomValues()
    {
      foreach(int i in _testLengths)
        _test.TestReImRandomValues(i);
    }
  }


  [TestFixture]
  public class TestPFA235FFT_2D_Inverse
  {
    static System.Random rnd = new System.Random();
    
    
    static int GetRandomN(int max)
    {
      int[] pqr = new int[3];

      int n=0;
      do  
      {
        n = rnd.Next(max);
      } while(n<2 || n>max || !Pfa235FFT.Factorize(n,pqr));

      return n;
    }


    [Test]
    public void TestZero2N()
    {
      // Testing 2^n
      for(int i=2;i<=64;i*=2)
        zzTestZero(i,i);
    }

    [Test]
    public void TestZero3N()
    {
      // Testing 3^n
      for(int i=3;i<=81;i*=3)
        zzTestZero(i,i);
    }
  
    [Test]
    public void TestZero5N()
    {
      // Testing 5^n
      for(int i=5;i<=125;i*=5)
        zzTestZero(i,i);
    }

    [Test]
    public void TestZero10N()
    {
      // Testing 5^n
      for(int i=10;i<=100;i*=10)
        zzTestZero(i,i);
    }

    [Test]
    public void TestZeroRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        Console.WriteLine("TestZero({0},{1})",u,v);
        zzTestZero(u,v);
      }
    }


    private static void zzTestZero(int u, int v)
    {
      int n = u*v;
      double[] re = new double[n];
      double[] im = new double[n];

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<n;i++)
      {
        Assert.AreEqual( 0, re[i],0, "FFT of zero should give re=0");
        Assert.AreEqual( 0, im[i],0,"FFT of zero should give im=0");
      }
    }

    [Test]
    public void TestReOne_OnePos1stDimRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        zzTestReOne_OnePos1stDim(u,v);
      }
    }

    private static void zzTestReOne_OnePos1stDim(int u, int v)
    {
      Console.WriteLine("TestReOn_OnePos1stDim({0},{1})",u,v);

      int n=u*v;
      double[] re = new double[n];
      double[] im = new double[n];

      re[1] = 1;

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<u;i++)
      {
        for(int j=0;j<v;j++)
        {
          Assert.AreEqual( Math.Cos((2*Math.PI*j)/v), re[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j));
          Assert.AreEqual( -Math.Sin((2*Math.PI*j)/v), im[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j));
        }
      }
    }

    [Test]
    public void TestReOne_OnePos2ndDimRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        zzTestReOne_OnePos2ndDim(u,v);
      }
    }

    private static void zzTestReOne_OnePos2ndDim(int u, int v)
    {
      Console.WriteLine("TestReOn_OnePos2ndDim({0},{1})",u,v);

      int n=u*v;
      double[] re = new double[n];
      double[] im = new double[n];

      re[1*v] = 1;

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<u;i++)
      {
        for(int j=0;j<v;j++)
        {
          Assert.AreEqual( Math.Cos((2*Math.PI*i)/u), re[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j));
          Assert.AreEqual( -Math.Sin((2*Math.PI*i)/u), im[i*v+j],n*1E-15,string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j));
        }
      }
    }


    [Test]
    public void TestReOne_OnePosBothDimRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        zzTestReOne_OnePosBothDim(u,v);
      }
    }

    private static void zzTestReOne_OnePosBothDim(int u, int v)
    {
      Console.WriteLine("TestReOn_OnePosBothDim({0},{1})",u,v);

      int n=u*v;
      double[] re = new double[n];
      double[] im = new double[n];

      re[1*v+1] = 1;

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<u;i++)
      {
        for(int j=0;j<v;j++)
        {
          Assert.AreEqual( Math.Cos(2*Math.PI*(((double)i)/u + ((double)j)/v)), re[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j));
          Assert.AreEqual( -Math.Sin(2*Math.PI*(((double)i)/u + ((double)j)/v)),im[i*v+j],n*1E-15,string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j));
        }
      }
    }

    [Test]
    public void TestReOne_ArbPosBothDimRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        zzTestReOne_ArbPosBothDim(u,v);
      }
    }

    private static void zzTestReOne_ArbPosBothDim(int u, int v)
    {
      int upos = rnd.Next(u);
      int vpos = rnd.Next(v);
      
      Console.WriteLine("TestReOn_ArbPosBothDim({0},{1}), pos({2},{3})",u,v,upos,vpos);

      int n=u*v;
      double[] re = new double[n];
      double[] im = new double[n];

  
      re[upos*v+vpos] = 1;

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<u;i++)
      {
        for(int j=0;j<v;j++)
        {
          Assert.AreEqual( Math.Cos(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), re[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j));
          Assert.AreEqual( -Math.Sin(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), im[i*v+j],n*1E-15,string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j));
        }
      }
    }


    [Test]
    public void TestImOne_ArbPosBothDimRandomN()
    {
      // Testing 10 times random dimensions
      for(int i=0;i<10;i++)
      {
        int u = GetRandomN(100);
        int v = GetRandomN(100);
        zzTestImOne_ArbPosBothDim(u,v);
      }
    }

    private static void zzTestImOne_ArbPosBothDim(int u, int v)
    {
      int upos = rnd.Next(u);
      int vpos = rnd.Next(v);
      Console.WriteLine("TestImOn_ArbPosBothDim({0},{1}), pos({2},{3})",u,v,upos,vpos);

      int n=u*v;
      double[] re = new double[n];
      double[] im = new double[n];



      im[upos*v+vpos] = 1;

      Pfa235FFT fft = new Pfa235FFT(u,v);

      fft.FFT(re,im,FourierDirection.Inverse);

      for(int i=0;i<u;i++)
      {
        for(int j=0;j<v;j++)
        {
          Assert.AreEqual( Math.Sin(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), re[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j));
          Assert.AreEqual( Math.Cos(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), im[i*v+j],n*1E-15, string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j));
        }
      }
    }

  }

  [TestFixture]
  public class TestPfa235FFTRealFFT
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

    int[] _testLengths = { 2,3,4,5,6,8,9,10,12,15,16,18,25 };

    RealFFTTests _test;
    RealFFTTests _test2;

    public TestPfa235FFTRealFFT()
    {
      _test = new RealFFTTests(new RealFFTTests.FFTRoutine(MyRoutine1));
      _test2 = new RealFFTTests(new RealFFTTests.FFTRoutine(MyRoutine2));

    }

    void MyRoutine1(double[] real1, FourierDirection dir)
    {
      int n = real1.Length;
      System.Random rnd = new System.Random();
      double[] real2 = new double[n];
      for(int i=0;i<n;i++)
        real2[i] = rnd.NextDouble()/n;

      Pfa235FFT fft = new Pfa235FFT(n);
      fft.RealFFT(real1,real2,dir);
    }

    void MyRoutine2(double[] real1, FourierDirection dir)
    {
      int n = real1.Length;
      System.Random rnd = new System.Random();
      double[] real2 = new double[n];
      for(int i=0;i<n;i++)
        real2[i] = rnd.NextDouble()/n;

      Pfa235FFT fft = new Pfa235FFT(n);
      fft.RealFFT(real2,real1,dir);
    }

    [Test]
    public void Test01Zero()
    {
      
      foreach(int i in _testLengths)
      {
        _test.TestZero(i);
        _test2.TestZero(i);
      }
    }

    [Test]
    public void Test02ReOne_ZeroPos()
    {
      foreach(int i in _testLengths)
      {
        _test.TestReOne_ZeroPos(i);
        _test2.TestReOne_ZeroPos(i);
      }
    }

  

    [Test]
    public void Test03ReOne_OnePos()
    {
      foreach(int i in _testLengths)
      {
        _test.TestReOne_OnePos(i);
        _test2.TestReOne_OnePos(i);
      }
    }
    
  
    [Test]
    public void Test04ReOne_RandomPos()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach(int i in _testLengths)
      {
        _test.TestReOne_RandomPos(i,5);
        _test2.TestReOne_RandomPos(i,5);
      }

      _test.SetTolerance(oldTolerance);
    }

    [Test]
    public void Test05ReRandomValues()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach(int i in _testLengths)
      {
        _test.TestReRandomValues(i);
        _test2.TestReRandomValues(i);
      }
      _test.SetTolerance(oldTolerance);
    }

  }
}
