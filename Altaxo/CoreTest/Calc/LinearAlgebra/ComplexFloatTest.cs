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

  public class ComplexFloatTest
  {
    private const float TOLERANCE = 0.001f;

    [Fact]
    public void EqualsTest()
    {
      var cf1 = new ComplexFloat(-1.1f, 2.2f);
      var cf2 = new ComplexFloat(-1.1f, 2.2f);
      Assert.True(cf1 == cf2);
      Assert.True(cf1.Equals(cf2));
    }

    [Fact]
    public void ConversionTest()
    {
      ComplexFloat cf1 = 2.2f;
      Assert.Equal(2.2f, cf1.Real);
      Assert.Equal(0, cf1.Imag);
    }

    [Fact]
    public void OperatorsTest()
    {
      var cf1 = new ComplexFloat(1.1f, -2.2f);
      var cf2 = new ComplexFloat(-3.3f, 4.4f);
      ComplexFloat test = cf1 * cf2;
      Assert.Equal(6.05f, test.Real);
      Assert.Equal(12.1f, test.Imag);

      test = cf1 / cf2;
      Assert.Equal(test.Real, -0.44f);
      AssertEx.Equal(test.Imag, 0.08f, TOLERANCE);

      test = cf1 + cf2;
      Assert.Equal(test.Real, (1.1f - 3.3f));
      Assert.Equal(test.Imag, (-2.2f + 4.4f));

      test = cf1 - cf2;
      Assert.Equal(test.Real, (1.1f + 3.3f));
      Assert.Equal(test.Imag, (-2.2f - 4.4f));
    }

    [Fact]
    public void NaNTest()
    {
      var cf = new ComplexFloat(float.NaN, 1.1f);
      Assert.True(cf.IsNaN());
      cf = new ComplexFloat(1.1f, float.NaN);
      Assert.True(cf.IsNaN());
      cf = new ComplexFloat(1.1f, 2.2f);
      Assert.False(cf.IsNaN());
    }

    [Fact]
    public void InfinityTest()
    {
      var cf = new ComplexFloat(float.NegativeInfinity, 1.1f);
      Assert.True(cf.IsInfinity());
      cf = new ComplexFloat(1.1f, float.NegativeInfinity);
      Assert.True(cf.IsInfinity());
      cf = new ComplexFloat(float.PositiveInfinity, 1.1f);
      Assert.True(cf.IsInfinity());
      cf = new ComplexFloat(1.1f, float.PositiveInfinity);
      Assert.True(cf.IsInfinity());
      cf = new ComplexFloat(1.1f, 2.2f);
      Assert.False(cf.IsInfinity());
    }

    [Fact]
    public void CloneTest()
    {
      var cf1 = new ComplexFloat(1.1f, 2.2f);
      var cf2 = (ComplexFloat)((ICloneable)cf1).Clone();
      Assert.Equal(cf1, cf2);
    }

    [Fact]
    public void HashTest()
    {
      var cd1 = new ComplexFloat(1.1f, 2.2f);
      var cd2 = new ComplexFloat(1.1f, 3.3f);
      var cd3 = new ComplexFloat(0.1f, 2.2f);
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
        var cf = new ComplexFloat(s);
      });
    }

    [Fact]
    public void FormatExceptionTest1()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "";
        var cf = new ComplexFloat(s);
      });
    }

    [Fact]
    public void FormatExceptionTest2()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "+";
        var cf = new ComplexFloat(s);
      });
    }

    [Fact]
    public void FormatExceptionTest3()
    {
      Assert.Throws<FormatException>(() =>
      {
        string s = "1i+2";
        var cf = new ComplexFloat(s);
      });
    }

    [Fact]
    public void ParseTest()
    {
      string s = "1";
      var cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(0, cf.Imag);

      s = "i";
      cf = new ComplexFloat(s);
      Assert.Equal(0, cf.Real);
      Assert.Equal(1, cf.Imag);

      s = "2i";
      cf = new ComplexFloat(s);
      Assert.Equal(0, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "1 + 2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "1+2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "1 - 2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(cf.Imag, -2);

      s = "1-2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(cf.Imag, -2);

      s = "1+-2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(cf.Imag, -2);

      s = "1 - 2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(cf.Imag, -2);

      s = "1,2";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "1 , 2 ";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "1,2i";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "-1, -2i";
      cf = new ComplexFloat(s);
      Assert.Equal(cf.Real, -1);
      Assert.Equal(cf.Imag, -2);

      s = "(+1,2i)";
      cf = new ComplexFloat(s);
      Assert.Equal(1, cf.Real);
      Assert.Equal(2, cf.Imag);

      s = "(-1 , -2)";
      cf = new ComplexFloat(s);
      Assert.Equal(cf.Real, -1);
      Assert.Equal(cf.Imag, -2);

      s = "(-1 , -2i)";
      cf = new ComplexFloat(s);
      Assert.Equal(cf.Real, -1);
      Assert.Equal(cf.Imag, -2);

      s = "(+1e1 , -2e-2i)";
      cf = new ComplexFloat(s);
      Assert.Equal(10, cf.Real);
      Assert.Equal(cf.Imag, -0.02f);

      s = "(-1e1 + -2e2i)";
      cf = new ComplexFloat(s);
      Assert.Equal(cf.Real, -10);
      Assert.Equal(cf.Imag, -200);
    }
  }
}
