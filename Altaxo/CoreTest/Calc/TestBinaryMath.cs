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

namespace AltaxoTest.Calc
{

  public class TestBinaryMath
  {
    [Fact]
    public void TestLd32()
    {
      Assert.Equal(int.MinValue, BinaryMath.Ld(0u));

      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.Equal(i, BinaryMath.Ld(x));
        x += x - 1;
        Assert.Equal(i, BinaryMath.Ld(x));
      }
    }

    [Fact]
    public void TestLd64()
    {
      Assert.Equal(int.MinValue, BinaryMath.Ld(0u));

      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.Equal(i, BinaryMath.Ld(x));
        x += x - 1;
        Assert.Equal(i, BinaryMath.Ld(x));
      }
    }

    [Fact]
    public void TestIsPowerOfTwoOrZero()
    {
      // 32 bit, signed
      Assert.True(BinaryMath.IsPowerOfTwoOrZero(0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.True(BinaryMath.IsPowerOfTwoOrZero((int)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsPowerOfTwoOrZero((int)x));
        }
      }

      // 32 bit, unsigned
      Assert.True(BinaryMath.IsPowerOfTwoOrZero((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.True(BinaryMath.IsPowerOfTwoOrZero(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsPowerOfTwoOrZero(x));
        }
      }

      // 64 bit, signed
      Assert.True(BinaryMath.IsPowerOfTwoOrZero((long)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.True(BinaryMath.IsPowerOfTwoOrZero((long)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsPowerOfTwoOrZero((long)x));
        }
      }

      // 64 bit, unsigned
      Assert.True(BinaryMath.IsPowerOfTwoOrZero((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.True(BinaryMath.IsPowerOfTwoOrZero(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsPowerOfTwoOrZero(x));
        }
      }
    }

    [Fact]
    public void TestIsNonzeroPowerOfTwo()
    {
      // 32 bit, signed
      Assert.False(BinaryMath.IsNonzeroPowerOfTwo(0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.True(BinaryMath.IsNonzeroPowerOfTwo((int)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsNonzeroPowerOfTwo((int)x));
        }
      }

      // 32 bit, unsigned
      Assert.False(BinaryMath.IsNonzeroPowerOfTwo((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.True(BinaryMath.IsNonzeroPowerOfTwo(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsNonzeroPowerOfTwo(x));
        }
      }

      // 64 bit, signed
      Assert.False(BinaryMath.IsNonzeroPowerOfTwo((long)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.True(BinaryMath.IsNonzeroPowerOfTwo((long)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsNonzeroPowerOfTwo((long)x));
        }
      }

      // 64 bit, unsigned
      Assert.False(BinaryMath.IsNonzeroPowerOfTwo((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.True(BinaryMath.IsNonzeroPowerOfTwo(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.False(BinaryMath.IsNonzeroPowerOfTwo(x));
        }
      }
    }

    [Fact]
    public void TestNextPowerOfTwoGreaterOrEqual()
    {
      // 32 bit, signed
      Assert.Equal(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(0));
      for (int i = 0; i < 31; ++i)
      {
        uint x = 1u << i;
        AssertEx.Equal(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((int)x));
        if (i < 30)
        {
          AssertEx.Equal(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((int)(x + 1)));
        }
      }

      // 32 bit, unsigned
      AssertEx.Equal(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.Equal(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x));
        if (i < 31)
        {
          Assert.Equal(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x + 1));
        }
      }

      // 64 bit, signed
      Assert.Equal(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)0));
      for (int i = 0; i < 63; ++i)
      {
        ulong x = 1ul << i;
        AssertEx.Equal(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)x));
        if (i < 62)
        {
          AssertEx.Equal(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)(x + 1)));
        }
      }

      // 64 bit, unsigned
      AssertEx.Equal(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1UL << i;
        Assert.Equal(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x));
        if (i < 63)
        {
          Assert.Equal(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x + 1));
        }
      }
    }

    [Fact]
    public void TestParityOfByte()
    {
      Assert.False(BinaryMath.IsParityOdd((byte)0x00));
      Assert.False(BinaryMath.IsParityOdd((byte)0xFF));
      Assert.False(BinaryMath.IsParityOdd((byte)0x81));
      Assert.False(BinaryMath.IsParityOdd((byte)0x18));
      Assert.False(BinaryMath.IsParityOdd((byte)0x22));

      Assert.True(BinaryMath.IsParityOdd((byte)0x01));
      Assert.True(BinaryMath.IsParityOdd((byte)0x10));
      Assert.True(BinaryMath.IsParityOdd((byte)0x80));
      Assert.True(BinaryMath.IsParityOdd((byte)0x08));
    }

    [Fact]
    public void TestParityOfUShort()
    {
      Assert.False(BinaryMath.IsParityOdd((ushort)0x0000));
      Assert.False(BinaryMath.IsParityOdd((ushort)0xFFFF));
      Assert.False(BinaryMath.IsParityOdd((ushort)0x8001));
      Assert.False(BinaryMath.IsParityOdd((ushort)0x1008));
      Assert.False(BinaryMath.IsParityOdd((ushort)0x2002));

      Assert.True(BinaryMath.IsParityOdd((ushort)0x0001));
      Assert.True(BinaryMath.IsParityOdd((ushort)0x1000));
      Assert.True(BinaryMath.IsParityOdd((ushort)0x8000));
      Assert.True(BinaryMath.IsParityOdd((ushort)0x0008));
    }

    [Fact]
    public void TestParityOfUInt()
    {
      Assert.False(BinaryMath.IsParityOdd(0x00000000u));
      Assert.False(BinaryMath.IsParityOdd(0xFFFFFFFFu));
      Assert.False(BinaryMath.IsParityOdd(0x80000001u));
      Assert.False(BinaryMath.IsParityOdd(0x10000008u));
      Assert.False(BinaryMath.IsParityOdd(0x20000002u));

      Assert.True(BinaryMath.IsParityOdd(0x00000001u));
      Assert.True(BinaryMath.IsParityOdd(0x10000000u));
      Assert.True(BinaryMath.IsParityOdd(0x80000000u));
      Assert.True(BinaryMath.IsParityOdd(0x00000008u));
    }

    [Fact]
    public void TestParityOfULong()
    {
      Assert.False(BinaryMath.IsParityOdd(0x0000000000000000ul));
      Assert.False(BinaryMath.IsParityOdd(0xFFFFFFFFFFFFFFFFul));
      Assert.False(BinaryMath.IsParityOdd(0x8000000000000001ul));
      Assert.False(BinaryMath.IsParityOdd(0x1000000000000008ul));
      Assert.False(BinaryMath.IsParityOdd(0x2000000000000002ul));

      Assert.True(BinaryMath.IsParityOdd(0x0000000000000001ul));
      Assert.True(BinaryMath.IsParityOdd(0x1000000000000000ul));
      Assert.True(BinaryMath.IsParityOdd(0x8000000000000000ul));
      Assert.True(BinaryMath.IsParityOdd(0x0000000000000008ul));
    }
  }
}
