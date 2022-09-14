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
using System.Collections.Generic;
using Altaxo.Calc;
using Altaxo.Calc.RootFinding;
using Xunit;
using Complex64T = System.Numerics.Complex;

namespace Calc.RootFinding
{

  public class PolynomialRootFindingTest
  {
    /// <summary>Coefficients of the real polynomial (x-0)*(x-1)*(x-2)*...*(x-9). Lowest order coefficient comes first.</summary>
    private static readonly double[] realCoefficients1 = { 0, -362880, 1026576, -1172700, 723680, -269325, 63273, -9450, 870, -45, 1 };

    /// <summary>
    /// Coefficients of the complex polynomial (x-4-4i)*(x-3-3i)*... *(x+3+3i)*(x+4+4i)
    /// </summary>
    private static readonly Complex64T[] complexCoefficients1 = { new Complex64T(0, 0), new Complex64T(9216, 0), new Complex64T(0, 0), new Complex64T(0, 6560), new Complex64T(0, 0), new Complex64T(-1092, 0), new Complex64T(0, 0), new Complex64T(0, -60), new Complex64T(0, 0), new Complex64T(1, 0) };

    /// <summary>
    /// Polynomial (x-0)*(x-1)*(x-2)*... *(x-9) should return the real roots 0, 1, 2, ... 9.
    /// </summary>
    [Fact]
    public void Test10DegreeRealPolynomial()
    {
      var roots = RealPolynomialRootFinder_JenkinsTraub.FindRoots(realCoefficients1);

      Assert.Equal(10, roots.Count);
      for (int i = 0; i < roots.Count; ++i)
        Assert.Equal(0, roots[i].Imaginary);

      roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Real, y.Real));

      for (int i = 0; i < roots.Count; ++i)
        AssertEx.Equal(i, roots[i].Real, 1E-7);
    }

    /// <summary>
    /// Polynomial (x-0)*(x-1)*(x-2)*... *(x-9) should return the real roots 0, 1, 2, ... 9.
    /// </summary>
    [Fact]
    public void Test10DegreeComplexPolynomial()
    {
      var ccoeffs = new Complex64T[realCoefficients1.Length];
      for (int i = 0; i < realCoefficients1.Length; ++i)
        ccoeffs[i] = realCoefficients1[i];

      var roots = ComplexPolynomialRootFinder_JenkinsTraub.FindRoots(ccoeffs);

      Assert.Equal(10, roots.Count);
      for (int i = 0; i < roots.Count; ++i)
        AssertEx.Equal(0, roots[i].Imaginary, 1E-11);

      roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Real, y.Real));

      for (int i = 0; i < roots.Count; ++i)
        AssertEx.Equal(i, roots[i].Real, 1E-7);
    }

    /// <summary>
    /// Polynomial (x-4-4i)*(x-3-3i)*... *(x+3+3i)*(x+4+4i) should return the complex roots -4-4i, -3-3i, ... 4+4i
    /// </summary>
    [Fact]
    public void Test9DegreeComplexPolynomial()
    {
      var roots = ComplexPolynomialRootFinder_JenkinsTraub.FindRoots(complexCoefficients1);
      Assert.Equal(9, roots.Count);

      roots.Sort((x, y) => Comparer<double>.Default.Compare(x.Real, y.Real));

      for (int i = 0; i < roots.Count; ++i)
      {
        AssertEx.Equal(i - 4, roots[i].Imaginary, 1E-12);
        AssertEx.Equal(i - 4, roots[i].Real, 1E-12);
      }
    }

    [Fact]
    public void TestRootsToCoefficientsReal()
    {
      var r = new double[10];
      for (int i = 0; i < r.Length; ++i)
        r[i] = i;

      var c = CoefficientsFromRoots(r);
      Assert.Equal(realCoefficients1.Length, c.Length);
      for (int i = 0; i < c.Length; ++i)
        Assert.Equal(realCoefficients1[i], c[i]);
    }

    [Fact]
    public void TestRootsToCoefficientsComplex()
    {
      var r = new Complex64T[9];
      for (int i = 0; i < r.Length; ++i)
        r[i] = new Complex64T(i - 4, i - 4);

      var c = CoefficientsFromRoots(r);
      Assert.Equal(complexCoefficients1.Length, c.Length);
      for (int i = 0; i < c.Length; ++i)
      {
        Assert.Equal(complexCoefficients1[i].Real, c[i].Real);
        Assert.Equal(complexCoefficients1[i].Imaginary, c[i].Imaginary);
      }
    }

    /// <summary>
    /// Calculate the coefficients of a polynom from it's roots.
    /// </summary>
    /// <param name="roots">The roots.</param>
    /// <returns>The coefficients of the polynom, with the lowest order coefficient at index 0. The highest order coefficient is at index [number of roots] and is always 1.</returns>
    private static double[] CoefficientsFromRoots(double[] roots)
    {
      var coeff = new double[roots.Length + 1];
      coeff[0] = 1;

      for (int i = 0; i < roots.Length; ++i)
      {
        var root = -roots[i];

        for (int j = i + 1; j > 0; --j)
          coeff[j] = coeff[j] * root + coeff[j - 1];

        coeff[0] = coeff[0] * root;
      }

      return coeff;
    }

    /// <summary>
    /// Calculate the coefficients of a polynom from it's roots.
    /// </summary>
    /// <param name="roots">The roots.</param>
    /// <returns>The coefficients of the polynom, with the lowest order coefficient at index 0. The highest order coefficient is at index [number of roots] and is always 1.</returns>
    private static Complex64T[] CoefficientsFromRoots(Complex64T[] roots)
    {
      var coeff = new Complex64T[roots.Length + 1];
      coeff[0] = 1;

      for (int i = 0; i < roots.Length; ++i)
      {
        var root = -roots[i];

        for (int j = i + 1; j > 0; --j)
          coeff[j] = coeff[j] * root + coeff[j - 1];

        coeff[0] = coeff[0] * root;
      }

      return coeff;
    }
  }
}
