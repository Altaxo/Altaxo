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

  [TestFixture]
  public class TestFastHartleyCorrelationComplexSplittedDestructive
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

    int[] _testLengths = { 4, 8, 16, 32, 64, 128 };


    SplittedComplexCorrelationTests _test;
    public TestFastHartleyCorrelationComplexSplittedDestructive()
    {
      _test = new SplittedComplexCorrelationTests(new SplittedComplexCorrelationTests.CorrelationRoutine(MyCorrelationRoutine));
    }


    void MyCorrelationRoutine(double[] src1real, double[] src1imag, double[] src2real, double[] src2imag, double[] resultreal, double[] resultimag, int n)
    {
      double[] inp1re = new double[n];
      double[] inp2re = new double[n];
      double[] inp1im = new double[n];
      double[] inp2im = new double[n];
      Array.Copy(src1real,inp1re,n);
      Array.Copy(src1imag,inp1im,n);
      Array.Copy(src2real,inp2re,n);
      Array.Copy(src2imag,inp2im,n);
      FastHartleyTransform.CyclicCorrelationDestructive(inp1re,inp1im,inp2re,inp2im,resultreal,resultimag,n);
    }
 

    [Test]
    public void Test01BothZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestBothZero(i);
    }

    [Test]
    public void Test02OneZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestOneZero(i);
    }


    [Test]
    public void Test03ReOne_ZeroPos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_ZeroPos(i);
    }

    [Test]
    public void Test04OneReOne_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestOneReOne_OtherRandom(i);
    }

    [Test]
    public void Test05OneImOne_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestOneImOne_OtherRandom(i);
    }
    
    [Test]
    public void Test06ReOne_OnePos_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_OnePos_OtherRandom(i);
    }
    
    [Test]
    public void Test07ImOne_OnePos_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestImOne_OnePos_OtherRandom(i);
    }

    [Test]
    public void Test08BothRandom()
    {
      foreach(int i in _testLengths)
        _test.TestBothRandom(i);
    }
  }




  [TestFixture]
  public class TestFastHartleyCorrelationSplittedComplex
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

    int[] _testLengths = { 4, 8, 16, 32, 64, 128 };


    SplittedComplexCorrelationTests _test;
    public TestFastHartleyCorrelationSplittedComplex()
    {
      _test = new SplittedComplexCorrelationTests(new SplittedComplexCorrelationTests.CorrelationRoutine(FastHartleyTransform.CyclicCorrelation));
    }

    [Test]
    public void Test01BothZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestBothZero(i);
    }

    [Test]
    public void Test02OneZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestOneZero(i);
    }


    [Test]
    public void Test03ReOne_ZeroPos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_ZeroPos(i);
    }

    [Test]
    public void Test04OneReOne_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestOneReOne_OtherRandom(i);
    }

    [Test]
    public void Test05OneImOne_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestOneImOne_OtherRandom(i);
    }
    
    [Test]
    public void Test06ReOne_OnePos_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_OnePos_OtherRandom(i);
    }
    
    [Test]
    public void Test07ImOne_OnePos_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestImOne_OnePos_OtherRandom(i);
    }

    [Test]
    public void Test08BothRandom()
    {
      foreach(int i in _testLengths)
        _test.TestBothRandom(i);
    }
  }

  [TestFixture]
  public class TestFastHartleyCorrelationRealDestructive
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

    int[] _testLengths = { 4, 8, 16, 32, 64, 128 };


    RealCorrelationTests _test;
    public TestFastHartleyCorrelationRealDestructive()
    {
      _test = new RealCorrelationTests(new RealCorrelationTests.CorrelationRoutine(MyCorrelationRoutine));
    }


    void MyCorrelationRoutine(double[] src1real, double[] src2real, double[] resultreal, int n)
    {
      double[] inp1re = new double[n];
      double[] inp2re = new double[n];
      Array.Copy(src1real,inp1re,n);
      Array.Copy(src2real,inp2re,n);
      FastHartleyTransform.CyclicCorrelationDestructive(inp1re,inp2re,resultreal,n);
    }
 

    [Test]
    public void Test01BothZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestBothZero(i);
    }

    [Test]
    public void Test02OneZero()
    {
      
      foreach(int i in _testLengths)
        _test.TestOneZero(i);
    }


    [Test]
    public void Test03ReOne_ZeroPos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_ZeroPos(i);
    }

    [Test]
    public void Test04OneReOne_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestOneReOne_OtherRandom(i);
    }
   
    
    [Test]
    public void Test05ReOne_OnePos_OtherRandom()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_OnePos_OtherRandom(i);
    }

    [Test]
    public void Test06BothRandom()
    {
      foreach(int i in _testLengths)
        _test.TestBothRandom(i);
    }
  }


}
