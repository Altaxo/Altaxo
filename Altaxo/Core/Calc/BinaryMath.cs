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
		/// Returns k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x&lt;0 then <see cref="System.Int32.MinValue"/> is returned.</returns>
		public static int Ld(int x)
		{
			if (x < 0) return int.MinValue;
			return Ld((uint)x);
		}

		/// <summary>
		/// Returns k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then 0 is returned.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then <see cref="System.Int32.MinValue"/> is returned.</returns>
		public static int Ld(uint x)
		{
			if (0 == x) return int.MinValue;

			int r = 0;
			if ((x & 0xffff0000) != 0) { x >>= 16; r += 16; }
			if ((x & 0x0000ff00) != 0) { x >>= 8; r += 8; }
			if ((x & 0x000000f0) != 0) { x >>= 4; r += 4; }
			if ((x & 0x0000000c) != 0) { x >>= 2; r += 2; }
			if ((x & 0x00000002) != 0) { r += 1; }
			return r;
		}

		/// <summary>
		/// Returns k so that 2^k &lt;= x &lt; 2^(k+1).
		/// If x==0 then 0 is returned.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x&lt;0 then <see cref="System.Int32.MinValue"/> is returned.</returns>
		public static int Ld(long x)
		{
			if (x < 0)
				return int.MinValue;
			return Ld((ulong)x);
		}

		/// <summary>
		/// Returns k so that 2^k &lt;= x &lt; 2^(k+1).
		/// If x==0 then 0 is returned.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>A number k so that 2^k &lt;= x &lt; 2^(k+1). If x==0 then <see cref="System.Int32.MinValue"/> is returned.</returns>
		public static int Ld(ulong x)
		{
			if (0 == x) return int.MinValue;

			int r = 0;
			if ((x & (~0UL << 32)) != 0) { x >>= 32; r += 32; }
			if ((x & 0xffff0000) != 0) { x >>= 16; r += 16; }
			if ((x & 0x0000ff00) != 0) { x >>= 8; r += 8; }
			if ((x & 0x000000f0) != 0) { x >>= 4; r += 4; }
			if ((x & 0x0000000c) != 0) { x >>= 2; r += 2; }
			if ((x & 0x00000002) != 0) { r += 1; }
			return r;
		}

		#endregion Ld

		#region PowerOfTwoOrZero

		/// <summary>
		/// Returns true if number is a power of two or is zero.
		/// </summary>
		/// <param name="x">Argument to test.</param>
		/// <returns>Returns <c>true</c> if the number is a power of two or is equal to zero.</returns>
		public static bool IsPowerOfTwoOrZero(int x)
		{
			return ((x & -x) == x);
		}

		/// <summary>
		/// Returns true if number is a power of two or is zero.
		/// </summary>
		/// <param name="x">Argument to test.</param>
		/// <returns>Returns <c>true</c> if the number is a power of two or is equal to zero.</returns>
		public static bool IsPowerOfTwoOrZero(uint x)
		{
			return IsPowerOfTwoOrZero((int)x);
		}

		/// <summary>
		/// Return true if number is 0 (!) or a power of two
		/// </summary>
		/// <param name="x">Argument to test.</param>
		/// <returns>Return true if number is 0 (!) or a power of two.</returns>
		public static bool IsPowerOfTwoOrZero(long x)
		{
			return ((x & -x) == x);
		}

		/// <summary>
		/// Returns true if number is a power of two or is zero.
		/// </summary>
		/// <param name="x">Argument to test.</param>
		/// <returns>Returns <c>true</c> if the number is a power of two or is equal to zero.</returns>
		public static bool IsPowerOfTwoOrZero(ulong x)
		{
			return IsPowerOfTwoOrZero((long)x);
		}

		#endregion PowerOfTwoOrZero

		#region NonzeroPowerOfTwo

		/// <summary>
		/// Return true if x &gt; 0 and x is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>True if x &gt; 0 and x is a power of two.</returns>
		public static bool IsNonzeroPowerOfTwo(int x)
		{
			return (0 != x) && ((x & -x) == x);
		}

		/// <summary>
		/// Return true if x &gt; 0 and x is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>True if x &gt; 0 and x is a power of two.</returns>
		public static bool IsNonzeroPowerOfTwo(uint x)
		{
			return IsNonzeroPowerOfTwo((int)x);
		}

		/// <summary>
		/// Return true if x &gt; 0 and x is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>True if x &gt; 0 and x is a power of two.</returns>
		public static bool IsNonzeroPowerOfTwo(long x)
		{
			return (0 != x) && ((x & -x) == x);
		}

		/// <summary>
		/// Return true if x &gt; 0 and x is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>True if x &gt; 0 and x is a power of two.</returns>
		public static bool IsNonzeroPowerOfTwo(ulong x)
		{
			return IsNonzeroPowerOfTwo((long)x);
		}

		#endregion NonzeroPowerOfTwo

		#region NextPowerOfTwoGreaterOrEqualThan

		/// <summary>
		/// Return x if x is is a power of two, else return the smallest number &gt;x, which is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>The argument, if it is a power of two. Else the next greater number which is a power of two.</returns>
		public static int NextPowerOfTwoGreaterOrEqualThan(int x)
		{
			if (x > 0x40000000)
				throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an Int32 value.");
			int i;
			for (i = 1; i < x; i <<= 1) ;
			return i;
		}

		/// <summary>
		/// Return x if x is is a power of two, else return the smallest number &gt;x, which is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>The argument, if it is a power of two. Else the next greater number which is a power of two.</returns>
		public static uint NextPowerOfTwoGreaterOrEqualThan(uint x)
		{
			if (x > 0x80000000)
				throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an UInt32 value.");
			uint i;
			for (i = 1; i < x; i <<= 1) ;
			return i;
		}

		/// <summary>
		/// Return x if x is is a power of two, else return the smallest number &gt;x, which is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>The argument, if it is a power of two. Else the next greater number which is a power of two.</returns>
		public static long NextPowerOfTwoGreaterOrEqualThan(long x)
		{
			if (x > 0x4000000000000000L)
				throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an Int64 value.");
			long i;
			for (i = 1; i < x; i <<= 1) ;
			return i;
		}

		/// <summary>
		/// Return x if x is is a power of two, else return the smallest number &gt;x, which is a power of two.
		/// </summary>
		/// <param name="x">The argument to test.</param>
		/// <returns>The argument, if it is a power of two. Else the next greater number which is a power of two.</returns>
		public static ulong NextPowerOfTwoGreaterOrEqualThan(ulong x)
		{
			if (x > 0x8000000000000000UL)
				throw new ArgumentOutOfRangeException("Provided value is too large. Result can not be represented by an UInt64 value.");
			ulong i;
			for (i = 1; i < x; i <<= 1) ;
			return i;
		}

		#endregion NextPowerOfTwoGreaterOrEqualThan

		#region Parity calculations

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(byte x)
		{
			uint xx = x;
			xx ^= xx >> 4;
			xx ^= xx >> 2;
			xx ^= xx >> 1;
			return 0 != (xx & 1u);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(sbyte x)
		{
			return IsParityOdd((byte)x);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(UInt16 x)
		{
			uint xx = x;
			xx ^= xx >> 8;
			xx ^= xx >> 4;
			xx ^= xx >> 2;
			xx ^= xx >> 1;
			return 0 != (xx & 1u);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(Int16 x)
		{
			return IsParityOdd((UInt16)x);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(UInt32 x)
		{
			x ^= x >> 16;
			x ^= x >> 8;
			x ^= x >> 4;
			x ^= x >> 2;
			x ^= x >> 1;
			return 0 != (x & 1u);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(Int32 x)
		{
			return IsParityOdd((UInt32)x);
		}

		/// <summary>
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(UInt64 x)
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
		/// Determines, if x contains an odd number of '1' bits.
		/// </summary>
		/// <param name="x">The argument.</param>
		/// <returns>True, if an odd number of bits is set to 1, or false, if an even number of bits is set to 1.</returns>
		public static bool IsParityOdd(Int64 x)
		{
			return IsParityOdd((UInt64)x);
		}

		#endregion Parity calculations
	}
}