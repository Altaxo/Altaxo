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
using Altaxo.Calc;
using Xunit;
using Complex64T = System.Numerics.Complex;
using Complex32T = Altaxo.Calc.Complex32;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexMathTest
  {
    private const float TOLERANCE = 0.001f;

    [Fact]
    public void Absolute()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);
      AssertEx.Equal(ComplexMath.Absolute(cd1), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cd2), 2.2, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cd3), 1.1, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cd4), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cf1), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cf2), 2.2, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cf3), 1.1, TOLERANCE);
      AssertEx.Equal(ComplexMath.Absolute(cf4), 2.460, TOLERANCE);
    }

    [Fact]
    public void Argument()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);
      AssertEx.Equal(ComplexMath.Argument(cd1), -1.107, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cd2), -1.571, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cd3), 0, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cd4), -1.107, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cf1), -1.107, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cf2), -1.571, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cf3), 0, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument(cf4), -1.107, TOLERANCE);
    }

    [Fact]
    public void Argument2()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);
      AssertEx.Equal(ComplexMath.Argument2(cd1), -1.107, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cd2), -1.571, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cd3), 0, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cd4), 2.034, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cf1), -1.107, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cf2), -1.571, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cf3), 0, TOLERANCE);
      AssertEx.Equal(ComplexMath.Argument2(cf4), 2.034, TOLERANCE);
    }

    [Fact]
    public void Conjugate()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);
      Assert.Equal(ComplexMath.Conjugate(cd1), new Complex64T(1.1, 2.2));
      Assert.Equal(ComplexMath.Conjugate(cd2), new Complex64T(0, 2.2));
      Assert.Equal(ComplexMath.Conjugate(cd3), new Complex64T(1.1, 0));
      Assert.Equal(ComplexMath.Conjugate(cd4), new Complex64T(-1.1, -2.2));
      Assert.Equal(ComplexMath.Conjugate(cf1), new Complex32T(1.1f, 2.2f));
      Assert.Equal(ComplexMath.Conjugate(cf2), new Complex32T(0, 2.2f));
      Assert.Equal(ComplexMath.Conjugate(cf3), new Complex32T(1.1f, 0));
      Assert.Equal(ComplexMath.Conjugate(cf4), new Complex32T(-1.1f, -2.2f));
    }

    [Fact]
    public void Cos()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Cos(cd1);
      AssertEx.Equal(cdt.Real, 2.072, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 3.972, TOLERANCE);

      cdt = ComplexMath.Cos(cd2);
      AssertEx.Equal(cdt.Real, 4.568, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Cos(cd3);
      AssertEx.Equal(cdt.Real, 0.454, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Cos(cd4);
      AssertEx.Equal(cdt.Real, 2.072, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 3.972, TOLERANCE);

      Complex32T cft = ComplexMath.Cos(cf1);
      AssertEx.Equal(cft.Real, 2.072, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 3.972, TOLERANCE);

      cft = ComplexMath.Cos(cf2);
      AssertEx.Equal(cft.Real, 4.568, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Cos(cf3);
      AssertEx.Equal(cft.Real, 0.454, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Cos(cf4);
      AssertEx.Equal(cft.Real, 2.072, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 3.972, TOLERANCE);
    }

    [Fact]
    public void Cosh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Cosh(cd1);
      AssertEx.Equal(cdt.Real, -.982, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.08, TOLERANCE);

      cdt = ComplexMath.Cosh(cd2);
      AssertEx.Equal(cdt.Real, -0.589, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Cosh(cd3);
      AssertEx.Equal(cdt.Real, 1.669, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Cosh(cd4);
      AssertEx.Equal(cdt.Real, -.982, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.08, TOLERANCE);

      Complex32T cft = ComplexMath.Cosh(cf1);
      AssertEx.Equal(cft.Real, -.982, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.08, TOLERANCE);

      cft = ComplexMath.Cosh(cf2);
      AssertEx.Equal(cft.Real, -0.589, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Cosh(cf3);
      AssertEx.Equal(cft.Real, 1.669, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Cosh(cf4);
      AssertEx.Equal(cft.Real, -.982, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.08, TOLERANCE);
    }

    [Fact]
    public void Exp()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Exp(cd1);
      AssertEx.Equal(cdt.Real, -1.768, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -2.429, TOLERANCE);

      cdt = ComplexMath.Exp(cd2);
      AssertEx.Equal(cdt.Real, -0.589, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.808, TOLERANCE);

      cdt = ComplexMath.Exp(cd3);
      AssertEx.Equal(cdt.Real, 3.004, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Exp(cd4);
      AssertEx.Equal(cdt.Real, -0.196, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0.269, TOLERANCE);

      Complex32T cft = ComplexMath.Exp(cf1);
      AssertEx.Equal(cft.Real, -1.768, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -2.429, TOLERANCE);

      cft = ComplexMath.Exp(cf2);
      AssertEx.Equal(cft.Real, -0.589, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.808, TOLERANCE);

      cft = ComplexMath.Exp(cf3);
      AssertEx.Equal(cft.Real, 3.004, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Exp(cf4);
      AssertEx.Equal(cft.Real, -0.196, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0.269, TOLERANCE);
    }

    [Fact]
    public void Log()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Log(cd1);
      AssertEx.Equal(cdt.Real, 0.900, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.107, TOLERANCE);

      cdt = ComplexMath.Log(cd2);
      AssertEx.Equal(cdt.Real, 0.788, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.571, TOLERANCE);

      cdt = ComplexMath.Log(cd3);
      AssertEx.Equal(cdt.Real, 0.0953, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Log(cd4);
      AssertEx.Equal(cdt.Real, 0.900, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 2.034, TOLERANCE);

      Complex32T cft = ComplexMath.Log(cf1);
      AssertEx.Equal(cft.Real, 0.900, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.107, TOLERANCE);

      cft = ComplexMath.Log(cf2);
      AssertEx.Equal(cft.Real, 0.788, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.571, TOLERANCE);

      cft = ComplexMath.Log(cf3);
      AssertEx.Equal(cft.Real, 0.095, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Log(cf4);
      AssertEx.Equal(cft.Real, 0.900, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 2.034, TOLERANCE);
    }

    [Fact]
    public void Max()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);

      Complex64T cdt = ComplexMath.Max(cd1, cd2);
      Assert.Equal(cd1, cdt);

      Complex32T cft = ComplexMath.Max(cf1, cf2);
      Assert.Equal(cf1, cft);
    }

    [Fact]
    public void Norm()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);
      AssertEx.Equal(ComplexMath.Norm(cd1), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cd2), 2.2, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cd3), 1.1, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cd4), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cf1), 2.460, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cf2), 2.2, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cf3), 1.1, TOLERANCE);
      AssertEx.Equal(ComplexMath.Norm(cf4), 2.460, TOLERANCE);
    }

    [Fact]
    public void Polar()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Polar(cd1);
      AssertEx.Equal(cdt.Real, 2.460, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.107, TOLERANCE);

      cdt = ComplexMath.Polar(cd2);
      AssertEx.Equal(cdt.Real, 2.2, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.571, TOLERANCE);

      cdt = ComplexMath.Polar(cd3);
      AssertEx.Equal(cdt.Real, 1.1, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Polar(cd4);
      AssertEx.Equal(cdt.Real, 2.460, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 2.034, TOLERANCE);

      Complex32T cft = ComplexMath.Polar(cf1);
      AssertEx.Equal(cft.Real, 2.460, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.107, TOLERANCE);

      cft = ComplexMath.Polar(cf2);
      AssertEx.Equal(cft.Real, 2.2, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.571, TOLERANCE);

      cft = ComplexMath.Polar(cf3);
      AssertEx.Equal(cft.Real, 1.1, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Polar(cf4);
      AssertEx.Equal(cft.Real, 2.460, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 2.034, TOLERANCE);
    }

    [Fact]
    public void Sin()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Sin(cd1);
      AssertEx.Equal(cdt.Real, 4.071, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -2.022, TOLERANCE);

      cdt = ComplexMath.Sin(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -4.457, TOLERANCE);

      cdt = ComplexMath.Sin(cd3);
      AssertEx.Equal(cdt.Real, 0.891, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Sin(cd4);
      AssertEx.Equal(cdt.Real, -4.071, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 2.022, TOLERANCE);

      Complex32T cft = ComplexMath.Sin(cf1);
      AssertEx.Equal(cft.Real, 4.071, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -2.022, TOLERANCE);

      cft = ComplexMath.Sin(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -4.457, TOLERANCE);

      cft = ComplexMath.Sin(cf3);
      AssertEx.Equal(cft.Real, 0.891, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Sin(cf4);
      AssertEx.Equal(cft.Real, -4.071, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 2.022, TOLERANCE);
    }

    [Fact]
    public void Sinh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Sinh(cd1);
      AssertEx.Equal(cdt.Real, -0.786, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.349, TOLERANCE);

      cdt = ComplexMath.Sinh(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.808, TOLERANCE);

      cdt = ComplexMath.Sinh(cd3);
      AssertEx.Equal(cdt.Real, 1.336, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Sinh(cd4);
      AssertEx.Equal(cdt.Real, 0.786, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.349, TOLERANCE);

      Complex32T cft = ComplexMath.Sinh(cf1);
      AssertEx.Equal(cft.Real, -0.786, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.349, TOLERANCE);

      cft = ComplexMath.Sinh(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.808, TOLERANCE);

      cft = ComplexMath.Sinh(cf3);
      AssertEx.Equal(cft.Real, 1.336, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Sinh(cf4);
      AssertEx.Equal(cft.Real, 0.786, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.349, TOLERANCE);
    }

    [Fact]
    public void Sqrt()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Sqrt(cd1);
      AssertEx.Equal(cdt.Real, 1.334, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.824, TOLERANCE);

      cdt = ComplexMath.Sqrt(cd2);
      AssertEx.Equal(cdt.Real, 1.049, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.049, TOLERANCE);

      cdt = ComplexMath.Sqrt(cd3);
      AssertEx.Equal(cdt.Real, 1.049, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Sqrt(cd4);
      AssertEx.Equal(cdt.Real, 0.824, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.334, TOLERANCE);

      Complex32T cft = ComplexMath.Sqrt(cf1);
      AssertEx.Equal(cft.Real, 1.334, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.824, TOLERANCE);

      cft = ComplexMath.Sqrt(cf2);
      AssertEx.Equal(cft.Real, 1.0489, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.049, TOLERANCE);

      cft = ComplexMath.Sqrt(cf3);
      AssertEx.Equal(cft.Real, 1.049, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Sqrt(cf4);
      AssertEx.Equal(cft.Real, 0.824, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.334, TOLERANCE);
    }

    [Fact]
    public void Tan()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Tan(cd1);
      AssertEx.Equal(cdt.Real, 0.020, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.014, TOLERANCE);

      cdt = ComplexMath.Tan(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.976, TOLERANCE);

      cdt = ComplexMath.Tan(cd3);
      AssertEx.Equal(cdt.Real, 1.965, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Tan(cd4);
      AssertEx.Equal(cdt.Real, -0.020, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.014, TOLERANCE);

      Complex32T cft = ComplexMath.Tan(cf1);
      AssertEx.Equal(cft.Real, 0.020, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.014, TOLERANCE);

      cft = ComplexMath.Tan(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.976, TOLERANCE);

      cft = ComplexMath.Tan(cf3);
      AssertEx.Equal(cft.Real, 1.965, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Tan(cf4);
      AssertEx.Equal(cft.Real, -0.020, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.014, TOLERANCE);
    }

    [Fact]
    public void Tanh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Tanh(cd1);
      AssertEx.Equal(cdt.Real, 1.046, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0.223, TOLERANCE);

      cdt = ComplexMath.Tanh(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.374, TOLERANCE);

      cdt = ComplexMath.Tanh(cd3);
      AssertEx.Equal(cdt.Real, 0.800, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Tanh(cd4);
      AssertEx.Equal(cdt.Real, -1.046, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.223, TOLERANCE);

      Complex32T cft = ComplexMath.Tanh(cf1);
      AssertEx.Equal(cft.Real, 1.046, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0.223, TOLERANCE);

      cft = ComplexMath.Tanh(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.374, TOLERANCE);

      cft = ComplexMath.Tanh(cf3);
      AssertEx.Equal(cft.Real, 0.800, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Tanh(cf4);
      AssertEx.Equal(cft.Real, -1.046, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.223, TOLERANCE);
    }

    [Fact]
    public void Asin()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Asin(cd1);
      AssertEx.Equal(cdt.Real, 0.433, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.618, TOLERANCE);

      cdt = ComplexMath.Asin(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.530, TOLERANCE);

      cdt = ComplexMath.Asin(cd3);
      AssertEx.Equal(cdt.Real, 1.571, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.444, TOLERANCE);

      cdt = ComplexMath.Asin(cd4);
      AssertEx.Equal(cdt.Real, -0.433, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.618, TOLERANCE);

      Complex32T cft = ComplexMath.Asin(cf1);
      AssertEx.Equal(cft.Real, 0.433, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.618, TOLERANCE);

      cft = ComplexMath.Asin(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.530, TOLERANCE);

      cft = ComplexMath.Asin(cf3);
      AssertEx.Equal(cft.Real, 1.571, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.444, TOLERANCE);

      cft = ComplexMath.Asin(cf4);
      AssertEx.Equal(cft.Real, -0.433, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.618, TOLERANCE);
    }

    [Fact]
    public void Acos()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Acos(cd1);
      AssertEx.Equal(cdt.Real, 1.1388414556, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.618, TOLERANCE);

      cdt = ComplexMath.Acos(cd2);
      AssertEx.Equal(cdt.Real, 1.571, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.530, TOLERANCE);

      cdt = ComplexMath.Acos(cd3);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0.444, TOLERANCE);

      cdt = ComplexMath.Acos(cd4);
      AssertEx.Equal(cdt.Real, 2.004, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.618, TOLERANCE);

      Complex32T cft = ComplexMath.Acos(cf1);
      AssertEx.Equal(cft.Real, 1.138, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.618, TOLERANCE);

      cft = ComplexMath.Acos(cf2);
      AssertEx.Equal(cft.Real, 1.571, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.530, TOLERANCE);

      cft = ComplexMath.Acos(cf3);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0.444, TOLERANCE);

      cft = ComplexMath.Acos(cf4);
      AssertEx.Equal(cft.Real, 2.004, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.618, TOLERANCE);
    }

    [Fact]
    public void Atan()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Atan(cd1);
      AssertEx.Equal(cdt.Real, 1.365, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.366, TOLERANCE);

      cdt = ComplexMath.Atan(cd2);
      AssertEx.Equal(cdt.Real, -1.5708, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -0.490415, TOLERANCE);

      cdt = ComplexMath.Atan(cd3);
      AssertEx.Equal(cdt.Real, 0.833, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Atan(cd4);
      AssertEx.Equal(cdt.Real, -1.365, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0.366, TOLERANCE);

      Complex32T cft = ComplexMath.Atan(cf1);
      AssertEx.Equal(cft.Real, 1.365, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.366, TOLERANCE);

      cft = ComplexMath.Atan(cf2);
      AssertEx.Equal(cft.Real, -1.571, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -0.490, TOLERANCE);

      cft = ComplexMath.Atan(cf3);
      AssertEx.Equal(cft.Real, 0.833, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Atan(cf4);
      AssertEx.Equal(cft.Real, -1.365, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0.366, TOLERANCE);
    }

    [Fact]
    public void Asinh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Asinh(cd1);
      AssertEx.Equal(cdt.Real, 1.569, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.072, TOLERANCE);

      cdt = ComplexMath.Asinh(cd2);
      AssertEx.Equal(cdt.Real, -1.425, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.571, TOLERANCE);

      cdt = ComplexMath.Asinh(cd3);
      AssertEx.Equal(cdt.Real, 0.950, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Asinh(cd4);
      AssertEx.Equal(cdt.Real, -1.569, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.0716, TOLERANCE);

      Complex32T cft = ComplexMath.Asinh(cf1);
      AssertEx.Equal(cft.Real, 1.569, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.072, TOLERANCE);

      cft = ComplexMath.Asinh(cf2);
      AssertEx.Equal(cft.Real, -1.425, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.571, TOLERANCE);

      cft = ComplexMath.Asinh(cf3);
      AssertEx.Equal(cft.Real, 0.950, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Asinh(cf4);
      AssertEx.Equal(cft.Real, -1.569, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.0716, TOLERANCE);
    }

    [Fact]
    public void Acosh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Acosh(cd1);
      AssertEx.Equal(cdt.Real, 1.618, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.138, TOLERANCE);

      cdt = ComplexMath.Acosh(cd2);
      AssertEx.Equal(cdt.Real, 1.530, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.571, TOLERANCE);

      cdt = ComplexMath.Acosh(cd3);
      AssertEx.Equal(cdt.Real, 0.444, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 0, TOLERANCE);

      cdt = ComplexMath.Acosh(cd4);
      AssertEx.Equal(cdt.Real, 1.618, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 2.004, TOLERANCE);

      Complex32T cft = ComplexMath.Acosh(cf1);
      AssertEx.Equal(cft.Real, 1.618, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.138, TOLERANCE);

      cft = ComplexMath.Acosh(cf2);
      AssertEx.Equal(cft.Real, 1.530, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.571, TOLERANCE);

      cft = ComplexMath.Acosh(cf3);
      AssertEx.Equal(cft.Real, 0.444, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 0, TOLERANCE);

      cft = ComplexMath.Acosh(cf4);
      AssertEx.Equal(cft.Real, 1.618, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 2.004, TOLERANCE);
    }

    [Fact]
    public void Atanh()
    {
      var cd1 = new Complex64T(1.1, -2.2);
      var cd2 = new Complex64T(0, -2.2);
      var cd3 = new Complex64T(1.1, 0);
      var cd4 = new Complex64T(-1.1, 2.2);
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(0, -2.2f);
      var cf3 = new Complex32T(1.1f, 0);
      var cf4 = new Complex32T(-1.1f, 2.2f);

      Complex64T cdt = ComplexMath.Atanh(cd1);
      AssertEx.Equal(cdt.Real, 0.161, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.212, TOLERANCE);

      cdt = ComplexMath.Atanh(cd2);
      AssertEx.Equal(cdt.Real, 0, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.144, TOLERANCE);

      cdt = ComplexMath.Atanh(cd3);
      AssertEx.Equal(cdt.Real, 1.522, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, -1.571, TOLERANCE);

      cdt = ComplexMath.Atanh(cd4);
      AssertEx.Equal(cdt.Real, -0.161, TOLERANCE);
      AssertEx.Equal(cdt.Imaginary, 1.212, TOLERANCE);

      Complex32T cft = ComplexMath.Atanh(cf1);
      AssertEx.Equal(cft.Real, 0.161, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.212, TOLERANCE);

      cft = ComplexMath.Atanh(cf2);
      AssertEx.Equal(cft.Real, 0, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.144, TOLERANCE);

      cft = ComplexMath.Atanh(cf3);
      AssertEx.Equal(cft.Real, 1.522, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, -1.571, TOLERANCE);

      cft = ComplexMath.Atanh(cf4);
      AssertEx.Equal(cft.Real, -0.161, TOLERANCE);
      AssertEx.Equal(cft.Imaginary, 1.212, TOLERANCE);
    }
  }
}
