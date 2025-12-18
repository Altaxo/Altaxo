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

using Xunit;
using Complex32T = Altaxo.Calc.Complex32;

namespace AltaxoTest.Calc.LinearAlgebra
{

  public class ComplexFloatTest
  {
    private const float TOLERANCE = 0.001f;

    [Fact]
    public void EqualsTest()
    {
      var cf1 = new Complex32T(-1.1f, 2.2f);
      var cf2 = new Complex32T(-1.1f, 2.2f);
      Assert.True(cf1 == cf2);
      Assert.True(cf1.Equals(cf2));
    }

    [Fact]
    public void ConversionTest()
    {
      Complex32T cf1 = 2.2f;
      Assert.Equal(2.2f, cf1.Real);
      Assert.Equal(0, cf1.Imaginary);
    }

    [Fact]
    public void OperatorsTest()
    {
      var cf1 = new Complex32T(1.1f, -2.2f);
      var cf2 = new Complex32T(-3.3f, 4.4f);
      Complex32T test = cf1 * cf2;
      Assert.Equal(6.05f, test.Real);
      Assert.Equal(12.1f, test.Imaginary);

      test = cf1 / cf2;
      Assert.Equal(-0.44f, test.Real);
      AssertEx.Equal(test.Imaginary, 0.08f, TOLERANCE);

      test = cf1 + cf2;
      Assert.Equal((1.1f - 3.3f), test.Real);
      Assert.Equal((-2.2f + 4.4f), test.Imaginary);

      test = cf1 - cf2;
      Assert.Equal((1.1f + 3.3f), test.Real);
      Assert.Equal((-2.2f - 4.4f), test.Imaginary);
    }

    [Fact]
    public void NaNTest()
    {
      var cf = new Complex32T(float.NaN, 1.1f);
      Assert.True(cf.IsNaN());
      cf = new Complex32T(1.1f, float.NaN);
      Assert.True(cf.IsNaN());
      cf = new Complex32T(1.1f, 2.2f);
      Assert.False(cf.IsNaN());
    }

    [Fact]
    public void InfinityTest()
    {
      var cf = new Complex32T(float.NegativeInfinity, 1.1f);
      Assert.True(cf.IsInfinity());
      cf = new Complex32T(1.1f, float.NegativeInfinity);
      Assert.True(cf.IsInfinity());
      cf = new Complex32T(float.PositiveInfinity, 1.1f);
      Assert.True(cf.IsInfinity());
      cf = new Complex32T(1.1f, float.PositiveInfinity);
      Assert.True(cf.IsInfinity());
      cf = new Complex32T(1.1f, 2.2f);
      Assert.False(cf.IsInfinity());
    }



    [Fact]
    public void HashTest()
    {
      var cd1 = new Complex32T(1.1f, 2.2f);
      var cd2 = new Complex32T(1.1f, 3.3f);
      var cd3 = new Complex32T(0.1f, 2.2f);
      Assert.NotEqual(cd1.GetHashCode(), cd2.GetHashCode());
      Assert.NotEqual(cd1.GetHashCode(), cd3.GetHashCode());
      Assert.NotEqual(cd2.GetHashCode(), cd3.GetHashCode());
    }


  }
}
