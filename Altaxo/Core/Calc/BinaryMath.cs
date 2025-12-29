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

namespace Altaxo.Calc
{
  /// <summary>
  /// BinaryMath provides static methods for "Bit" mathematics.
  /// </summary>
  public class BinaryMath
  {
    #region Ld

    /// <summary>
    /// Returns <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// A number <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>. If <paramref name="x"/> is less than 0,
    /// <see cref="int.MinValue"/> is returned.
    /// </returns>
    /// <remarks>
    /// If <paramref name="x"/> is 0, <see cref="int.MinValue"/> is returned.
    /// </remarks>
    public static int Ld(int x)
    {
      if (x < 0)
        return int.MinValue;
      return Ld((uint)x);
    }

    /// <summary>
    /// Returns <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// A number <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </returns>
    /// <remarks>
    /// If <paramref name="x"/> is 0, <see cref="int.MinValue"/> is returned.
    /// </remarks>
    public static int Ld(uint x)
    {
      if (0 == x)
        return int.MinValue;

      int r = 0;
      if ((x & 0xffff0000) != 0)
      { x >>= 16; r += 16; }
      if ((x & 0x0000ff00) != 0)
      { x >>= 8; r += 8; }
      if ((x & 0x000000f0) != 0)
      { x >>= 4; r += 4; }
      if ((x & 0x0000000c) != 0)
      { x >>= 2; r += 2; }
      if ((x & 0x00000002) != 0)
      { r += 1; }
      return r;
    }

    /// <summary>
    /// Returns <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// A number <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>. If <paramref name="x"/> is less than 0,
    /// <see cref="int.MinValue"/> is returned.
    /// </returns>
    /// <remarks>
    /// If <paramref name="x"/> is 0, <see cref="int.MinValue"/> is returned.
    /// </remarks>
    public static int Ld(long x)
    {
      if (x < 0)
        return int.MinValue;
      return Ld((ulong)x);
    }

    /// <summary>
    /// Returns <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// A number <c>k</c> such that <c>2^k &lt;= x &lt; 2^(k+1)</c>.
    /// </returns>
    /// <remarks>
    /// If <paramref name="x"/> is 0, <see cref="int.MinValue"/> is returned.
    /// </remarks>
    public static int Ld(ulong x)
    {
      if (0 == x)
        return int.MinValue;

      int r = 0;
      if ((x & (~0UL << 32)) != 0)
      { x >>= 32; r += 32; }
      if ((x & 0xffff0000) != 0)
      { x >>= 16; r += 16; }
      if ((x & 0x0000ff00) != 0)
      { x >>= 8; r += 8; }
      if ((x & 0x000000f0) != 0)
      { x >>= 4; r += 4; }
      if ((x & 0x0000000c) != 0)
      { x >>= 2; r += 2; }
      if ((x & 0x00000002) != 0)
      { r += 1; }
      return r;
    }

    #endregion Ld

    #region PowerOfTwoOrZero

    /// <summary>
    /// Returns <see langword="true"/> if the number is a power of two or is zero.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is a power of two or is equal to zero; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPowerOfTwoOrZero(int x)
    {
      return ((x & -x) == x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the number is a power of two or is zero.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is a power of two or is equal to zero; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPowerOfTwoOrZero(uint x)
    {
      return IsPowerOfTwoOrZero((int)x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the number is a power of two or is zero.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is a power of two or is equal to zero; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Note that 0 is considered a valid input and returns <see langword="true"/>.
    /// </remarks>
    public static bool IsPowerOfTwoOrZero(long x)
    {
      return ((x & -x) == x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the number is a power of two or is zero.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is a power of two or is equal to zero; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPowerOfTwoOrZero(ulong x)
    {
      return IsPowerOfTwoOrZero((long)x);
    }

    #endregion PowerOfTwoOrZero

    #region NonzeroPowerOfTwo

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNonzeroPowerOfTwo(int x)
    {
      return (0 != x) && ((x & -x) == x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNonzeroPowerOfTwo(uint x)
    {
      return IsNonzeroPowerOfTwo((int)x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNonzeroPowerOfTwo(long x)
    {
      return (0 != x) && ((x & -x) == x);
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is greater than 0 and is a power of two; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNonzeroPowerOfTwo(ulong x)
    {
      return IsNonzeroPowerOfTwo((long)x);
    }

    #endregion NonzeroPowerOfTwo

    #region NextPowerOfTwoGreaterOrEqualThan

    /// <summary>
    /// Returns <paramref name="x"/> if it is a power of two; otherwise, returns the smallest power of two that is greater than <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <paramref name="x"/> if it is a power of two; otherwise, the smallest power of two that is greater than <paramref name="x"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the result cannot be represented by an <see cref="int"/>.
    /// </exception>
    public static int NextPowerOfTwoGreaterOrEqualThan(int x)
    {
      if (x > 0x40000000)
        throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an Int32 value.");
      int i;
      for (i = 1; i < x; i <<= 1)
        ;
      return i;
    }

#pragma warning disable CA3002  // Function is not CLS-compliant
    /// <summary>
    /// Returns <paramref name="x"/> if it is a power of two; otherwise, returns the smallest power of two that is greater than <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <paramref name="x"/> if it is a power of two; otherwise, the smallest power of two that is greater than <paramref name="x"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the result cannot be represented by a <see cref="uint"/>.
    /// </exception>
    public static uint NextPowerOfTwoGreaterOrEqualThan(uint x)
    {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(x, 0x80000000u, "Provided value is too large. Result can not be represented by an UInt32 value.");
      uint i;
      for (i = 1; i < x; i <<= 1)
        ;
      return i;
    }
#pragma warning restore CA3002
    /// <summary>
    /// Returns <paramref name="x"/> if it is a power of two; otherwise, returns the smallest power of two that is greater than <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <paramref name="x"/> if it is a power of two; otherwise, the smallest power of two that is greater than <paramref name="x"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the result cannot be represented by an <see cref="long"/>.
    /// </exception>
    public static long NextPowerOfTwoGreaterOrEqualThan(long x)
    {
      if (x > 0x4000000000000000L)
        throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an Int64 value.");
      long i;
      for (i = 1; i < x; i <<= 1)
        ;
      return i;
    }

#pragma warning disable CA3002  // Function is not CLS-compliant
    /// <summary>
    /// Returns <paramref name="x"/> if it is a power of two; otherwise, returns the smallest power of two that is greater than <paramref name="x"/>.
    /// </summary>
    /// <param name="x">The argument to test.</param>
    /// <returns>
    /// <paramref name="x"/> if it is a power of two; otherwise, the smallest power of two that is greater than <paramref name="x"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the result cannot be represented by a <see cref="ulong"/>.
    /// </exception>
    public static ulong NextPowerOfTwoGreaterOrEqualThan(ulong x)
    {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(x, 0x8000000000000000UL, "Provided value is too large. Result can not be represented by an UInt64 value.");
      ulong i;
      for (i = 1; i < x; i <<= 1)
        ;
      return i;
    }
#pragma warning restore CA3002

    #endregion NextPowerOfTwoGreaterOrEqualThan

    #region Parity calculations

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(byte x)
    {
      uint xx = x;
      xx ^= xx >> 4;
      xx ^= xx >> 2;
      xx ^= xx >> 1;
      return 0 != (xx & 1u);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(sbyte x)
    {
      return IsParityOdd((byte)x);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(ushort x)
    {
      uint xx = x;
      xx ^= xx >> 8;
      xx ^= xx >> 4;
      xx ^= xx >> 2;
      xx ^= xx >> 1;
      return 0 != (xx & 1u);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(short x)
    {
      return IsParityOdd((ushort)x);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(uint x)
    {
      x ^= x >> 16;
      x ^= x >> 8;
      x ^= x >> 4;
      x ^= x >> 2;
      x ^= x >> 1;
      return 0 != (x & 1u);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(int x)
    {
      return IsParityOdd((uint)x);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(ulong x)
    {
      uint xx = (uint)(x) ^ (uint)(x >> 32);
      xx ^= xx >> 16;
      xx ^= xx >> 8;
      xx ^= xx >> 4;
      xx ^= xx >> 2;
      xx ^= xx >> 1;
      return 0 != (xx & 1u);
    }

    /// <summary>
    /// Determines whether <paramref name="x"/> contains an odd number of set bits (<c>1</c> bits).
    /// </summary>
    /// <param name="x">The argument.</param>
    /// <returns>
    /// <see langword="true"/> if an odd number of bits is set to 1; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsParityOdd(long x)
    {
      return IsParityOdd((ulong)x);
    }

    #endregion Parity calculations
  }
}
