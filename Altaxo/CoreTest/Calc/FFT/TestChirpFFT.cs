﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using Altaxo.Calc.Fourier;
using Xunit;

namespace AltaxoTest.Calc.Fourier
{

  public class TestChirpFFT
  {
    private const int nLowerLimit = 5;
    private const int nUpperLimit = 100;
    private const double maxTolerableEpsPerN = 1E-15;

    private SplittedComplexFFTTests _test;

    public TestChirpFFT()
    {
      _test = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(ChirpFFT.FFT));
    }

    [Fact]
    public void Test01Zero()
    {
      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestZero(i);
    }

    [Fact]
    public void Test02ReOne_ZeroPos()
    {
      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestReOne_ZeroPos(i);
    }

    [Fact]
    public void Test03ImOne_ZeroPos()
    {
      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestImOne_ZeroPos(i);
    }

    [Fact]
    public void Test04ReOne_OnePos()
    {
      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestReOne_OnePos(i);
    }

    [Fact]
    public void Test05ImOne_OnePos()
    {
      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestImOne_OnePos(i);
    }

    [Fact]
    public void Test06ReImOne_RandomPos()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestReImOne_RandomPos(i, 5);

      _test.SetTolerance(oldTolerance);
    }

    [Fact]
    public void Test07ReImRandomValues()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      for (int i = nLowerLimit; i <= nUpperLimit; i++)
        _test.TestReImRandomValues(i);

      _test.SetTolerance(oldTolerance);
    }
  }
}
