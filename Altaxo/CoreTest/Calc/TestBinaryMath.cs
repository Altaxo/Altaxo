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
using NUnit.Framework;

namespace AltaxoTest.Calc
{
  [TestFixture]
  public class TestBinaryMath
  {
    [Test]
    public void TestLd32()
    {
      Assert.AreEqual(int.MinValue, BinaryMath.Ld(0u));

      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(i, BinaryMath.Ld(x));
        x += x - 1;
        Assert.AreEqual(i, BinaryMath.Ld(x));
      }
    }

    [Test]
    public void TestLd64()
    {
      Assert.AreEqual(int.MinValue, BinaryMath.Ld(0u));

      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(i, BinaryMath.Ld(x));
        x += x - 1;
        Assert.AreEqual(i, BinaryMath.Ld(x));
      }
    }

    [Test]
    public void TestIsPowerOfTwoOrZero()
    {
      // 32 bit, signed
      Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero(0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero((int)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsPowerOfTwoOrZero((int)x));
        }
      }

      // 32 bit, unsigned
      Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsPowerOfTwoOrZero(x));
        }
      }

      // 64 bit, signed
      Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero((long)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero((long)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsPowerOfTwoOrZero((long)x));
        }
      }

      // 64 bit, unsigned
      Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(true, BinaryMath.IsPowerOfTwoOrZero(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsPowerOfTwoOrZero(x));
        }
      }
    }

    [Test]
    public void TestIsNonzeroPowerOfTwo()
    {
      // 32 bit, signed
      Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo(0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(true, BinaryMath.IsNonzeroPowerOfTwo((int)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo((int)x));
        }
      }

      // 32 bit, unsigned
      Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(true, BinaryMath.IsNonzeroPowerOfTwo(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo(x));
        }
      }

      // 64 bit, signed
      Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo((long)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(true, BinaryMath.IsNonzeroPowerOfTwo((long)x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo((long)x));
        }
      }

      // 64 bit, unsigned
      Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(true, BinaryMath.IsNonzeroPowerOfTwo(x));
        if (i > 0)
        {
          x += x - 1;
          Assert.AreEqual(false, BinaryMath.IsNonzeroPowerOfTwo(x));
        }
      }
    }

    [Test]
    public void TestNextPowerOfTwoGreaterOrEqual()
    {
      // 32 bit, signed
      Assert.AreEqual(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(0));
      for (int i = 0; i < 31; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((int)x));
        if (i < 30)
        {
          Assert.AreEqual(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((int)(x + 1)));
        }
      }

      // 32 bit, unsigned
      Assert.AreEqual(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((uint)0));
      for (int i = 0; i < 32; ++i)
      {
        uint x = 1u << i;
        Assert.AreEqual(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x));
        if (i < 31)
        {
          Assert.AreEqual(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x + 1));
        }
      }

      // 64 bit, signed
      Assert.AreEqual(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)0));
      for (int i = 0; i < 63; ++i)
      {
        ulong x = 1ul << i;
        Assert.AreEqual(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)x));
        if (i < 62)
        {
          Assert.AreEqual(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((long)(x + 1)));
        }
      }

      // 64 bit, unsigned
      Assert.AreEqual(1, BinaryMath.NextPowerOfTwoGreaterOrEqualThan((ulong)0));
      for (int i = 0; i < 64; ++i)
      {
        ulong x = 1UL << i;
        Assert.AreEqual(x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x));
        if (i < 63)
        {
          Assert.AreEqual(x + x, BinaryMath.NextPowerOfTwoGreaterOrEqualThan(x + 1));
        }
      }
    }

    [Test]
    public void TestParityOfByte()
    {
      Assert.AreEqual(false, BinaryMath.IsParityOdd((byte)0x00));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((byte)0xFF));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((byte)0x81));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((byte)0x18));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((byte)0x22));

      Assert.AreEqual(true, BinaryMath.IsParityOdd((byte)0x01));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((byte)0x10));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((byte)0x80));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((byte)0x08));
    }

    [Test]
    public void TestParityOfUShort()
    {
      Assert.AreEqual(false, BinaryMath.IsParityOdd((ushort)0x0000));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((ushort)0xFFFF));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((ushort)0x8001));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((ushort)0x1008));
      Assert.AreEqual(false, BinaryMath.IsParityOdd((ushort)0x2002));

      Assert.AreEqual(true, BinaryMath.IsParityOdd((ushort)0x0001));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((ushort)0x1000));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((ushort)0x8000));
      Assert.AreEqual(true, BinaryMath.IsParityOdd((ushort)0x0008));
    }

    [Test]
    public void TestParityOfUInt()
    {
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x00000000u));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0xFFFFFFFFu));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x80000001u));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x10000008u));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x20000002u));

      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x00000001u));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x10000000u));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x80000000u));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x00000008u));
    }

    [Test]
    public void TestParityOfULong()
    {
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x0000000000000000ul));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0xFFFFFFFFFFFFFFFFul));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x8000000000000001ul));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x1000000000000008ul));
      Assert.AreEqual(false, BinaryMath.IsParityOdd(0x2000000000000002ul));

      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x0000000000000001ul));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x1000000000000000ul));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x8000000000000000ul));
      Assert.AreEqual(true, BinaryMath.IsParityOdd(0x0000000000000008ul));
    }
  }
}
