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

  [TestFixture]
  public class TestFastHartleyTransform
  {
    const int nLowerLimit=4;
    const int nUpperLimit=16384;
    const double maxTolerableEpsPerN=1E-15;

    static CommonFFTTests _test = new CommonFFTTests(new CommonFFTTests.FFTRoutine(FastHartleyTransform.FFT));

    [Test]
    public void TestZero()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestZero(i);
    }

    [Test]
    public void TestReOne_ZeroPos()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestReOne_ZeroPos(i);
    }

    [Test]
    public void TestImOne_ZeroPos()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestImOne_ZeroPos(i);
    }

    [Test]
    public void TestReOne_OnePos()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestReOne_OnePos(i);
    }

    [Test]
    public void TestImOne_OnePos()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestImOne_OnePos(i);
    }

    [Test]
    public void TestReImOne_RandomPos()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=nUpperLimit;i*=2)
        _test.TestReImOne_RandomPos(i,10);
    }

    [Test]
    public void TestReImRandomValues()
    {
      // Testing 2^n
      for(int i=nLowerLimit;i<=256;i*=2)
        _test.TestReImRandomValues(i);
    }
  }
}
