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
using Altaxo.Collections;
using Xunit;

namespace AltaxoTest.Calc
{

  public class TestRoundings
  {
    [Fact]
    public void TestRoundUpDownMod1()
    {
      var rnd = new Random();

      for (int i = 0; i < 1000; ++i)
      {
        var n = rnd.Next(int.MinValue, int.MaxValue);

        Assert.Equal(n, Rounding.RoundUp(n, 1));
        Assert.Equal(n, Rounding.RoundDown(n, 1));
        Assert.Equal(n, Rounding.RoundUp((long)n, 1));
        Assert.Equal(n, Rounding.RoundDown((long)n, 1));
      }
    }

    [Fact]
    public void TestRoundUpIntMod5()
    {
      for (int i = -500; i < 500; ++i)
      {
        var r = Rounding.RoundUp(i, 5);
        AssertEx.GreaterOrEqual(r, i, "n=" + i.ToString());
        AssertEx.LessOrEqual(r - i, 4, "n=" + i.ToString());
      }
    }

    [Fact]
    public void TestRoundDownIntMod5()
    {
      for (int i = -500; i < 500; ++i)
      {
        var r = Rounding.RoundDown(i, 5);
        AssertEx.LessOrEqual(r, i, "n=" + i.ToString());
        AssertEx.LessOrEqual(i - r, 4, "n=" + i.ToString());
      }
    }

    [Fact]
    public void TestRoundUpLongMod5()
    {
      for (long i = -500; i < 500; ++i)
      {
        var r = Rounding.RoundUp(i, 5);
        AssertEx.GreaterOrEqual(r, i, "n=" + i.ToString());
        AssertEx.LessOrEqual(r - i, 4, "n=" + i.ToString());
      }
    }

    [Fact]
    public void TestRoundDownLongMod5()
    {
      for (long i = -500; i < 500; ++i)
      {
        var r = Rounding.RoundDown(i, 5);
        AssertEx.LessOrEqual(r, i, "n=" + i.ToString());
        AssertEx.LessOrEqual(i - r, 4, "n=" + i.ToString());
      }
    }

    [Fact]
    public void TestRoundToSignificantDigits()
    {
      AssertEx.Equal(0.01, Rounding.RoundToNumberOfSignificantDigits(0.01, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(0.1, Rounding.RoundToNumberOfSignificantDigits(0.1, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(1, Rounding.RoundToNumberOfSignificantDigits(1, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(0, Rounding.RoundToNumberOfSignificantDigits(0, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(10, Rounding.RoundToNumberOfSignificantDigits(10, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(100, Rounding.RoundToNumberOfSignificantDigits(100, 2, MidpointRounding.ToEven), 1E-6);

      AssertEx.Equal(0.012, Rounding.RoundToNumberOfSignificantDigits(0.012345678, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(1.2, Rounding.RoundToNumberOfSignificantDigits(1.2345678, 2, MidpointRounding.ToEven), 1E-6);
      AssertEx.Equal(120, Rounding.RoundToNumberOfSignificantDigits(123.45678, 2, MidpointRounding.ToEven), 1E-6);
    }

    [Fact]
    public void TestSplitIntoDecimalMantissaAndExponent01()
    {
      for (int j = 1; j <= 9; ++j)
      {
        for (int i = RMath.DoubleMinimalDecimalPowerWithoutPrecisionLoss; i < RMath.DoubleMaximalDecimalPower; ++i)
        {
          var mantissaString = j.ToString(System.Globalization.CultureInfo.InvariantCulture);
          var valueString = mantissaString + "E" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
          var x = double.Parse(valueString, System.Globalization.CultureInfo.InvariantCulture);
          var (m, e) = Rounding.SplitIntoDecimalMantissaAndExponent(x);
          var mantissaStringNow = m.ToString(System.Globalization.CultureInfo.InvariantCulture);
          Assert.True(mantissaString == mantissaStringNow, $"Expected: {mantissaString} but is: {mantissaStringNow}");
          Assert.Equal(e, i);
        }
      }
    }

    [Fact]
    public void TestSplitIntoDecimalMantissaAndExponent01a()
    {
      for (int j = 1; j <= 9; ++j)
      {
        for (int i = RMath.DoubleMinimalDecimalPowerWithoutPrecisionLoss; i < RMath.DoubleMaximalDecimalPower; ++i)
        {
          var mantissaString = j.ToString(System.Globalization.CultureInfo.InvariantCulture);
          var x = j * RMath.TenToThePowerOf(i);
          var (m, e) = Rounding.SplitIntoDecimalMantissaAndExponent(x);
          var mantissaStringNow = m.ToString(System.Globalization.CultureInfo.InvariantCulture);
          Assert.True(mantissaString == mantissaStringNow, $"Expected: {mantissaString} but is: {mantissaStringNow}");
          Assert.Equal(e, i);
        }
      }
    }

    [Fact]
    public void TestSplitIntoDecimalMantissaAndExponent02()
    {
      for (int j = 1; j <= 9; ++j)
      {
        for (int i = RMath.DoubleMinimalDecimalPowerWithoutPrecisionLoss; i < RMath.DoubleMaximalDecimalPower; ++i)
        {
          var mantissaString = j.ToString(System.Globalization.CultureInfo.InvariantCulture);
          mantissaString = mantissaString + "." + mantissaString;
          var valueString = mantissaString + "E" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
          var x = double.Parse(valueString, System.Globalization.CultureInfo.InvariantCulture);
          var (m, e) = Rounding.SplitIntoDecimalMantissaAndExponent(x);
          var mantissaStringNow = m.ToString(System.Globalization.CultureInfo.InvariantCulture);
          Assert.True(mantissaString == mantissaStringNow, $"Expected: {mantissaString} but is: {mantissaStringNow}");
          Assert.Equal(e, i);

        }
      }
    }

    [Fact]
    public void TestSplitIntoDecimalMantissaAndExponent02a()
    {
      for (int j = 1; j <= 9; ++j)
      {
        for (int i = RMath.DoubleMinimalDecimalPowerWithoutPrecisionLoss; i < RMath.DoubleMaximalDecimalPower; ++i)
        {
          var mantissaString = j.ToString(System.Globalization.CultureInfo.InvariantCulture);
          mantissaString = mantissaString + "." + mantissaString + mantissaString + mantissaString + mantissaString + mantissaString + mantissaString + mantissaString + mantissaString + mantissaString;
          var valueString = mantissaString + "E" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
          var x = double.Parse(valueString, System.Globalization.CultureInfo.InvariantCulture);
          var (m, e) = Rounding.SplitIntoDecimalMantissaAndExponent(x);
          var mantissaStringNow = m.ToString(System.Globalization.CultureInfo.InvariantCulture);
          Assert.True(mantissaString == mantissaStringNow, $"Expected: {mantissaString} but is: {mantissaStringNow}");
          Assert.Equal(e, i);

        }
      }
    }

    [Fact]
    public void TestSplitIntoDecimalMantissaAndExponent03()
    {
      for (int i = RMath.DoubleMinimalDecimalPowerWithoutPrecisionLoss; i <= RMath.DoubleMaximalDecimalPower; ++i)
      {
        var x = double.Parse("1.23456789E" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
        var (m, e) = Rounding.SplitIntoDecimalMantissaAndExponent(x);
        Assert.True("1.23456789" == m.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(e, i);
      }
    }
  }
}
