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
  public class TestRealFourierTransformClass
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

    int[] _testLengths = { 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};


    RealFFTTests _test = new RealFFTTests(new RealFFTTests.FFTRoutine(RealFFT));
 

    static void RealFFT(double[] x, FourierDirection dir)
    {
      new RealFourierTransform(x.Length).Transform(x,dir);
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
    public void Test03ReOne_OnePos()
    {
      foreach(int i in _testLengths)
        _test.TestReOne_OnePos(i);
    }
    
  
    [Test]
    public void Test04ReOne_RandomPos()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach(int i in _testLengths)
        _test.TestReOne_RandomPos(i,5);

      _test.SetTolerance(oldTolerance);
    }

    [Test]
    public void Test05ReRandomValues()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach(int i in _testLengths)
        _test.TestReRandomValues(i);

      _test.SetTolerance(oldTolerance);
    }

  }
}
