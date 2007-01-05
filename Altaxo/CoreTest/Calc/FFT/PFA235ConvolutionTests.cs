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
  public class TestPFA235ConvolutionReal1D
  {
    const int nLowerLimit=4;
    const int nUpperLimit=128;
    const double maxTolerableEpsPerN=1E-15;
    RealConvolutionTests _test ;

    int[] _testLengths = { 2, 3, 2*2, 5, 2*2*2, 3*3, 2*5, 2*2*3, 2*3*5, 2*2*2*2*5, 2*2*3*3*5 };

    public TestPFA235ConvolutionReal1D()
    {
      _test = new RealConvolutionTests(new RealConvolutionTests.ConvolutionRoutine(MyConvolution));
    }

    void MyConvolution(double[] re1, double[] re2, double[] re, int n)
    {
      Pfa235Convolution conv = new Pfa235Convolution(n);
      conv.Convolute(re1,re2,re,null,FourierDirection.Forward);
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

  [TestFixture]
  public class TestPFA235ConvolutionSplittedComplex1D
  {
    const int nLowerLimit=4;
    const int nUpperLimit=128;
    const double maxTolerableEpsPerN=1E-15;
    SplittedComplexConvolutionTests _test ;
    int[] _testLengths = { 2, 3, 2*2, 5, 2*2*2, 3*3, 2*5, 2*2*3, 2*3*5, 2*2*2*2*5, 2*2*3*3*5 };

    public TestPFA235ConvolutionSplittedComplex1D()
    {
      _test = new SplittedComplexConvolutionTests(new SplittedComplexConvolutionTests.ConvolutionRoutine(MyConvolution));
    }

    void MyConvolution(double[] re1, double[] im1, double[] re2, double[] im2, double[] re, double[] im, int n)
    {
      Pfa235Convolution conv = new Pfa235Convolution(n);
      conv.Convolute(re1,im1,re2,im2,re,im,null,null,FourierDirection.Forward);
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
}