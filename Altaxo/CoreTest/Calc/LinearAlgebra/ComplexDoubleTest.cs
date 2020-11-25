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

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexDoubleTest
  {
    private const double TOLERANCE = 0.001;
    private const double DBL_EPSILON = DoubleConstants.DBL_EPSILON;

    [Fact]
    public void EqualsTest()
    {
      var cd1 = new Complex(-1.1, 2.2);
      var cd2 = new Complex(-1.1, 2.2);
      var cd3 = new Complex(-1, 2);
      var cf = new ComplexFloat(-1, 2);
      Assert.True(cd1 == cd2);
      Assert.True(cd1.Equals(cd2));
      Assert.True(cd3 == cf);
      Assert.True(cd3.Equals(cf));
    }

    [Fact]
    public void ConversionTest()
    {
      Complex cd1 = 2.2;
      var cf = new ComplexFloat(-1.1f, 2.2f);
      Complex cd2 = cf;
      Assert.Equal(2.2, cd1.Real);
      Assert.Equal(0, cd1.Imag);
      AssertEx.Equal(cd2.Real, -1.1, TOLERANCE);
      AssertEx.Equal(cd2.Imag, 2.2, TOLERANCE);
    }

    [Fact]
    public void OperatorsTest()
    {
      var cd1 = new Complex(1.1, -2.2);
      var cd2 = new Complex(-3.3, 4.4);
      Complex test = cd1 * cd2;
      AssertEx.Equal(test.Real, 6.05, 20 * DBL_EPSILON);
      AssertEx.Equal(test.Imag, 12.1, 25 * DBL_EPSILON);

      test = cd1 / cd2;
      AssertEx.Equal(test.Real, -0.44, 2 * DBL_EPSILON);
      AssertEx.Equal(test.Imag, 0.08, 2 * DBL_EPSILON);

      test = cd1 + cd2;
      AssertEx.Equal(test.Real, -2.2, 10 * DBL_EPSILON);
      AssertEx.Equal(test.Imag, 2.2, 10 * DBL_EPSILON);

      test = cd1 - cd2;
      AssertEx.Equal(test.Real, 4.4, 10 * DBL_EPSILON);
      AssertEx.Equal(test.Imag, -6.6, 10 * DBL_EPSILON);

      //test = cd1 ^ cd2;
      //Assert.Equal(test.Real,1.593,TOLERANCE);
      //Assert.Equal(test.Imag,6.503,TOLERANCE);
    }

    [Fact]
    public void NaNTest()
    {
      var cd = new Complex(double.NaN, 1.1);
      Assert.True(cd.IsNaN());
      cd = new Complex(1.1, double.NaN);
      Assert.True(cd.IsNaN());
      cd = new Complex(1.1, 2.2);
      Assert.False(cd.IsNaN());
    }

    [Fact]
    public void InfinityTest()
    {
      var cd = new Complex(double.NegativeInfinity, 1.1);
      Assert.True(cd.IsInfinity());
      cd = new Complex(1.1, double.NegativeInfinity);
      Assert.True(cd.IsInfinity());
      cd = new Complex(double.PositiveInfinity, 1.1);
      Assert.True(cd.IsInfinity());
      cd = new Complex(1.1, double.PositiveInfinity);
      Assert.True(cd.IsInfinity());
      cd = new Complex(1.1, 2.2);
      Assert.False(cd.IsInfinity());
    }

    [Fact]
    public void CloneTest()
    {
      var cd1 = new Complex(1.1, 2.2);
      var cd2 = (Complex)((ICloneable)cd1).Clone();
      Assert.Equal(cd1, cd2);
    }

    [Fact]
    public void HashTest()
    {
      var cd1 = new Complex(1.1, 2.2);
      var cd2 = new Complex(1.1, 3.3);
      var cd3 = new Complex(0.1, 2.2);
      Assert.NotEqual(cd1.GetHashCode(), cd2.GetHashCode());
      Assert.NotEqual(cd1.GetHashCode(), cd3.GetHashCode());
      Assert.NotEqual(cd2.GetHashCode(), cd3.GetHashCode());
    }

    [Fact]
    public void NullString()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        string s = null;
        var cd = new Complex(s);
      });
    }

    [Fact]
    public void FormatExceptionTest1()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "";
        var cd = new Complex(s);
      });
    }

    [Fact]
    public void FormatExceptionTest2()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "+";
        var cd = new Complex(s);
      });
    }

    [Fact]
    public void FormatExceptionTest3()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "1i+2";
        var cd = new Complex(s);
      });
    }

    [Fact]
    public void ParseTest()
    {
      string s = "1";
      var cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(0, cd.Imag);

      s = "i";
      cd = new Complex(s);
      Assert.Equal(0, cd.Real);
      Assert.Equal(1, cd.Imag);

      s = "2i";
      cd = new Complex(s);
      Assert.Equal(0, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "1 + 2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "1+2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "1 - 2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(cd.Imag, -2);

      s = "1-2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(cd.Imag, -2);

      s = "1+-2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(cd.Imag, -2);

      s = "1 - 2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(cd.Imag, -2);

      s = "1,2";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "1 , 2 ";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "1,2i";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "-1, -2i";
      cd = new Complex(s);
      Assert.Equal(cd.Real, -1);
      Assert.Equal(cd.Imag, -2);

      s = "(+1,2i)";
      cd = new Complex(s);
      Assert.Equal(1, cd.Real);
      Assert.Equal(2, cd.Imag);

      s = "(-1 , -2)";
      cd = new Complex(s);
      Assert.Equal(cd.Real, -1);
      Assert.Equal(cd.Imag, -2);

      s = "(-1 , -2i)";
      cd = new Complex(s);
      Assert.Equal(cd.Real, -1);
      Assert.Equal(cd.Imag, -2);

      s = "(+1e1 , -2e-2i)";
      cd = new Complex(s);
      Assert.Equal(10, cd.Real);
      Assert.Equal(cd.Imag, -.02);

      s = "(-1e1 + -2e2i)";
      cd = new Complex(s);
      Assert.Equal(cd.Real, -10);
      Assert.Equal(cd.Imag, -200);
    }
  }
}
