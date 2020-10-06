#region Copyright

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
using System.Threading.Tasks;
using Altaxo.Calc.Fourier;
using Xunit;

namespace AltaxoTest.Calc.Fourier
{

  public class TestFastHartleyTransform
  {
    private const int nLowerLimit = 4;
    private const int nUpperLimit = 1024;
    private const double maxTolerableEpsPerN = 1E-15;

    private SplittedComplexFFTTests _test;

    public TestFastHartleyTransform()
    {
      _test = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(FastHartleyTransform.FFT));
    }

    [Fact]
    public void Test01Zero()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestZero(i);
    }

    [Fact]
    public void Test02ReOne_ZeroPos()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestReOne_ZeroPos(i);
    }

    [Fact]
    public void Test03ImOne_ZeroPos()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestImOne_ZeroPos(i);
    }

    [Fact]
    public void Test04ReOne_OnePos()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestReOne_OnePos(i);
    }

    [Fact]
    public void Test05ImOne_OnePos()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestImOne_OnePos(i);
    }

    [Fact]
    public void Test06ReImOne_RandomPos()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
        _test.TestReImOne_RandomPos(i, 10);
    }

    [Fact]
    public void Test07ReImRandomValues()
    {
      // Testing 2^n
      for (int i = nLowerLimit; i <= 256; i *= 2)
        _test.TestReImRandomValues(i);
    }

    [Fact]
    public void Test08_ConcurrencyTest()
    {
      var test1 = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(FastHartleyTransform.FFT));
      var test2 = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(FastHartleyTransform.FFT));

      var t1 = Task.Run(() =>
      {
        for (int n = 0; n < 100; ++n)
        {
          // Testing 2^n
          for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
            _test.TestReOne_OnePos(i);
        }
      }
      );

      var t2 = Task.Run(() =>
        {
          for (int n = 0; n < 100; ++n)
          {
            // Testing 2^n
            for (int i = nLowerLimit; i <= nUpperLimit; i *= 2)
              _test.TestImOne_OnePos(i);
          }
        }
      );

      Task.WaitAll(t1, t2);
    }

    [Fact]
    public void Test09_ConcurrencyTest()
    {
      var test1 = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(FastHartleyTransform.FFT));
      var test2 = new SplittedComplexFFTTests(new SplittedComplexFFTTests.FFTRoutine(FastHartleyTransform.FFT));

      var t1 = Task.Run(() =>
      {
        double max_fft_error(int n) => 5E-15 * n;

        for (int k = 0; k < 100; ++k)
        {
          // Testing 2^n
          for (int n = nUpperLimit; n <= nUpperLimit; n *= 2)
          {
            double[] re = new double[n];
            double[] im = new double[n];

            re[1] = 1;

            for (int i = 0; i < n; i++)
            {
              Assert.Equal(i == 1 ? 1 : 0, re[i]);
              Assert.Equal(0, im[i]);
            }

            FastHartleyTransform.FFT(re, im, FourierDirection.Forward);

            for (uint i = 0; i < n; i++)
            {
              AssertEx.Equal(Math.Cos((2 * Math.PI * i) / n), re[i], max_fft_error(n), string.Format("FFT({0}) of re 1 at pos 1 re[{1}]", n, i));
              AssertEx.Equal(Math.Sin((2 * Math.PI * i) / n), im[i], max_fft_error(n), string.Format("FFT({0}) of re 1 at pos 1 im[{1}]", n, i));
            }
          }
        }
      }
      );

      var t2 = Task.Run(() =>
      {
        double max_fft_error(int n) => 5E-15 * n;

        for (int k = 0; k < 100; ++k)
        {
          // Testing 2^n
          for (int n = nUpperLimit; n <= nUpperLimit; n *= 2)
          {
            double[] re = new double[n];
            double[] im = new double[n];
            im[1] = 1;

            for (int i = 0; i < n; i++)
            {
              Assert.Equal(0, re[i]);
              Assert.Equal(i == 1 ? 1 : 0, im[i]);
            }


            FastHartleyTransform.FFT(re, im, FourierDirection.Forward);

            for (int i = 0; i < n; i++)
            {
              AssertEx.Equal(-Math.Sin((2 * Math.PI * i) / n), re[i], max_fft_error(n), string.Format("FFT({0}) of im 1 at pos 1 re[{1}]", n, i));
              AssertEx.Equal(Math.Cos((2 * Math.PI * i) / n), im[i], max_fft_error(n), string.Format("FFT({0}) of im 1 at pos 1 im[{1}]", n, i));
            }
          }
        }
      }
      );

      Task.WaitAll(t1, t2);
    }
  }


  public class TestFastHartleyTransformRealFFT
  {
    private const int nLowerLimit = 5;
    private const int nUpperLimit = 100;
    private const double maxTolerableEpsPerN = 1E-15;

    private int[] _testLengths = { 4, 8, 16, 32, 64, 256 };

    private RealFFTTests _test = new RealFFTTests(new RealFFTTests.FFTRoutine(FastHartleyTransform.RealFFT));

    [Fact]
    public void Test01Zero()
    {
      foreach (int i in _testLengths)
        _test.TestZero(i);
    }

    [Fact]
    public void Test02ReOne_ZeroPos()
    {
      foreach (int i in _testLengths)
        _test.TestReOne_ZeroPos(i);
    }

    [Fact]
    public void Test03ReOne_OnePos()
    {
      foreach (int i in _testLengths)
        _test.TestReOne_OnePos(i);
    }

    [Fact]
    public void Test04ReOne_RandomPos()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach (int i in _testLengths)
        _test.TestReOne_RandomPos(i, 5);

      _test.SetTolerance(oldTolerance);
    }

    [Fact]
    public void Test05ReRandomValues()
    {
      double oldTolerance = _test.SetTolerance(1E-14);

      foreach (int i in _testLengths)
        _test.TestReRandomValues(i);

      _test.SetTolerance(oldTolerance);
    }
  }
}
