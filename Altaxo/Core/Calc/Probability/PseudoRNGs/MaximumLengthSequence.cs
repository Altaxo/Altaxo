#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Represents a maximum length sequence (MLS). Those sequences have a repeat length of 2^k-1, with k being an integer value (k=2..64). 
	/// This class allows the generation of sequences with a length of 2^2-1 up to 2^64-1.
	/// </summary>
	public class MaximumLengthSequence
	{
		#region Tap values
		/// <summary>
		/// Tap values for sequence lengths of 2^2-1 ... 2^64-1. The value at index k represents the tap value to generate a sequence of length 2^k-1. The values at index 0 and  1 are unused and set to zero.
		/// </summary>
		public static readonly ulong[] TapValues = new ulong[]
		{
			0,
			0,
			0x0002 | 0x0001, // 2^2-1
			0x0004 | 0x0002, // 2^3-1
			0x0008 | 0x0004, // 2^4-1
			0x0010 | 0x0004, // 2^5-1
			0x0020 | 0x0010, // 2^6-1
			0x0040 | 0x0020, // 2^7-1
			0x0080 | 0x0020 | 0x0010 | 0x0008, // 2^8-1
			0x0100 | 0x0008, // 2^9-1
			0x0200 | 0x0040, // 2^10-1
			0x0400 | 0x0100, // 2^11-1
			0x0800 | 0x0400 | 0x0200 | 0x0008, // 2^12-1
			0x1000 | 0x0800 | 0x0400 | 0x0080, // 2^13-1
			0x2000 | 0x1000 | 0x0800 | 0x0002, // 2^14-1
			0x4000 | 0x2000, // 2^15-1
			0x8000 | 0x2000 | 0x1000 | 0x0400, // 2^16-1
			0x00010000 | 0x00002000, // 2^17-1
			0x00020000 | 0x00000400, // 2^18-1
			0x00040000 | 0x00020000 | 0x00010000 | 0x00002000, // 2^19-1
			0x00080000 | 0x00010000, // 2^20-1
			0x00100000 | 0x00040000, // 2^21-1
			0x00200000 | 0x00100000, // 2^22-1
			0x00400000 | 0x00020000, // 2^23-1
			0x00800000 | 0x00400000 | 0x00200000 | 0x00010000, // 2^24-1
			0x01000000 | 0x00200000, // 2^25-1
			0x02000000 | 0x00000020 | 0x00000002 | 0x00000001, // 2^26-1
			0x04000000 | 0x00000010 | 0x00000002 | 0x00000001, // 2^27-1
			0x08000000 | 0x01000000, // 2^28-1
			0x10000000 | 0x04000000, // 2^29-1
			0x20000000 | 0x00000020 | 0x00000008 | 0x00000001, // 2^30-1
			0x40000000 | 0x08000000, // 2^31-1
			0x80000000 | 0x00200000 | 0x00000002 | 0x00000001, // 2^32-1
			0x0000000100000000 | 0x0000000000080000, // 2^33-1
			0x0000000200000000 | 0x0000000004000000 | 0x0000000000000002 | 0x0000000000000001, // 2^34-1
			0x0000000400000000 | 0x0000000100000000, // 2^35-1
			0x0000000800000000 | 0x0000000001000000, // 2^36-1
			0x0000001000000000 | 0x0000000000000010 | 0x0000000000000008 | 0x0000000000000004, // 2^37-1
			0x0000002000000000 | 0x0000000000000020 | 0x0000000000000010 | 0x0000000000000001, // 2^38-1
			0x0000004000000000 | 0x0000000400000000, // 2^39-1
			0x0000008000000000 | 0x0000002000000000 | 0x0000000000100000 | 0x0000000000040000, // 2^40-1
			0x0000010000000000 | 0x0000002000000000, // 2^41-1
			0x0000020000000000 | 0x0000010000000000 | 0x0000000000080000 | 0x0000000000040000, // 2^42-1
			0x0000040000000000 | 0x0000020000000000 | 0x0000002000000000 | 0x0000001000000000, // 2^43-1
			0x0000080000000000 | 0x0000040000000000 | 0x0000000000020000 | 0x0000000000010000, // 2^44-1
			0x0000100000000000 | 0x0000080000000000 | 0x0000020000000000 | 0x0000010000000000, // 2^45-1
			0x0000200000000000 | 0x0000100000000000 | 0x0000000002000000 | 0x0000000001000000, // 2^46-1
			0x0000400000000000 | 0x0000020000000000, // 2^47-1
			0x0000800000000000 | 0x0000400000000000 | 0x0000000000100000 | 0x0000000000080000, // 2^48-1
			0x0001000000000000 | 0x0000008000000000, // 2^49-1
			0x0002000000000000 | 0x0001000000000000 | 0x0000000000800000 | 0x0000000000400000, // 2^50-1
			0x0004000000000000 | 0x0002000000000000 | 0x0000000800000000 | 0x0000000400000000, // 2^51-1
			0x0008000000000000 | 0x0001000000000000, // 2^52-1
			0x0010000000000000 | 0x0008000000000000 | 0x0000002000000000 | 0x0000001000000000, // 2^53-1
			0x0020000000000000 | 0x0010000000000000 | 0x0000000000020000 | 0x0000000000010000, // 2^54-1
			0x0040000000000000 | 0x0000000040000000, // 2^55-1
			0x0080000000000000 | 0x0040000000000000 | 0x0000000400000000 | 0x0000000200000000, // 2^56-1
			0x0100000000000000 | 0x0002000000000000, // 2^57-1
			0x0200000000000000 | 0x0000004000000000, // 2^58-1
			0x0400000000000000 | 0x0200000000000000 | 0x0000002000000000 | 0x0000001000000000, // 2^59-1
			0x0800000000000000 | 0x0400000000000000, // 2^60-1
			0x1000000000000000 | 0x0800000000000000 | 0x0000200000000000 | 0x0000100000000000, // 2^61-1
			0x2000000000000000 | 0x1000000000000000 | 0x0000000000000020 | 0x0000000000000010, // 2^62-1
			0x4000000000000000 | 0x2000000000000000, // 2^63-1
			0x8000000000000000 | 0x4000000000000000 | 0x1000000000000000 | 0x0800000000000000, // 2^64-1
		};

		#endregion
		const int MinimumSequenceLength = 3;
		const int MinimumNumberOfStages = 2;

		int _numberOfStages;
		ulong _sequenceLength;
		ulong _tap;

		private MaximumLengthSequence()
		{
		}

		/// <summary>Constructs a new instance of the <see cref="MaximumLengthSequence"/> class froms the number of stages (bits, flip-flops).</summary>
		/// <param name="numberOfStages">The number of stages (2..64). The length of the resulting sequence is 2^numberOfStages-1.</param>
		/// <returns>The constructed instance of the <see cref="MaximumLengthSequence"/> class with the given number of stages.</returns>
		public static MaximumLengthSequence FromNumberOfStages(int numberOfStages)
		{
			if (!(numberOfStages>=2))
				throw new ArgumentOutOfRangeException("numberOfStages must be >= 2");
			if (!(numberOfStages <= 64))
				throw new ArgumentOutOfRangeException("numberOfStages must be <= 64");

			var result = new MaximumLengthSequence();
			result._numberOfStages = numberOfStages;
			result._sequenceLength = GetSequenceLengthFromNumberOfStages(numberOfStages);
			result._tap = TapValues[numberOfStages];

			return result;
		}


		/// <summary>Constructs a new instance of the <see cref="MaximumLengthSequence"/> class with a minimum length given by the argument.</summary>
		/// <param name="sequenceLength">The minimum length of the binary sequency. If the provided value is not a number (2^k-1), the value will be rounded up to the next possible sequence length.</param>
		/// <returns>The constructed instance of the <see cref="MaximumLengthSequence"/> class with the given minimum sequence length.</returns>
		public static MaximumLengthSequence FromMinimumSequenceLength(int sequenceLength)
		{
			if (!(sequenceLength >= MinimumSequenceLength))
				return FromMinimumSequenceLength(MinimumSequenceLength);
			else
				return FromMinimumSequenceLength((ulong)sequenceLength);
		}

		/// <summary>Constructs a new instance of the <see cref="MaximumLengthSequence"/> class with a minimum length given by the argument.</summary>
		/// <param name="sequenceLength">The minimum length of the binary sequency. If the provided value is not a number (2^k-1), the value will be rounded up to the next possible sequence length.</param>
		/// <returns>The constructed instance of the <see cref="MaximumLengthSequence"/> class with the given minimum sequence length.</returns>
		public static MaximumLengthSequence FromMinimumSequenceLength(ulong sequenceLength)
		{
			if (sequenceLength < MinimumSequenceLength)
				sequenceLength = MinimumSequenceLength;
			int stages = 1 + BinaryMath.Ld(sequenceLength);
			return FromNumberOfStages(stages);
		}

		/// <summary>Constructs a new instance of the <see cref="MaximumLengthSequence"/> class from a tap value given by the argument.</summary>
		/// <param name="tapValue">The tap value. A basic test of the validity of the tap value will be made, although it can not be guaranteed that the given tap value is able to generate a maximum length sequence. The highest set bit
		/// of the tap value determines the length of the maximum length sequence.</param>
		/// <returns>The constructed instance of the <see cref="MaximumLengthSequence"/> class with the given tap value.</returns>
		public static MaximumLengthSequence FromTapValue(ulong tapValue)
		{
			var result = new MaximumLengthSequence();
			result.TapValue = tapValue;
			return result;
		}

		/// <summary>Gets or sets the tap value. When setting the tap value, only basic tests will be made to ensure its validity. 
		/// Thus, it can not be fully ensured that the provided tap value will generate a maximum length sequence.</summary>
		/// <value>The tap value. The highest bit that is set in the tap value determines the length of the maximum length sequence.</value>
		public ulong TapValue
		{
			get
			{
				return _tap;
			}
			set
			{
				if (!(value >= 3))
					throw new ArgumentException("Invalid tap value: tap value has to be >= 3");

				int numberOfStages = 1+BinaryMath.Ld(value);
				ulong seqLen = GetSequenceLengthFromNumberOfStages(numberOfStages);
				ulong highestBit = seqLen ^ (seqLen >> 1);

				// when masking the highest bit of the tap value, the rest should have an odd parity
				if (!BinaryMath.IsParityOdd(value ^ highestBit))
					throw new ArgumentException("Invalid tap value: tap value should have odd parity after masking out the highest set bit");

				_numberOfStages = numberOfStages;
				_sequenceLength = seqLen;
				_tap = value;
			}
		}

		/// <summary>Gets the length of the sequence. This is the repeat period of the sequence.</summary>
		/// <value>The length of the sequence.</value>
		public ulong LongLength
		{
			get
			{
				return _sequenceLength;
			}
		}

		/// <summary>Gets the length of the sequence as an <see cref="System.Int32"/> value. If the sequence length is greater than int.MaxValue, an <see cref="System.InvalidCastException"/> will be thrown.</summary>
		/// <value>The length of the sequence.</value>
		public int Length
		{
			get
			{
				if (_sequenceLength > int.MaxValue)
					throw new InvalidCastException("Length can not be represented by an Int32 value because it is too large. Please use the property LongLength instead.");
				return (int)_sequenceLength;
			}
		}

		/// <summary>Gets the number of stages. This is the number of flip-flops that is needed to generate a maximum length sequence with discrete hardware. The sequence length is 2^numberOfStages-1.</summary>
		public int NumberOfStages
		{
			get
			{
				return _numberOfStages;
			}
		}

		private static ulong GetSequenceLengthFromNumberOfStages(int numberOfStages)
		{
			ulong result = 1UL << (numberOfStages - 1);
			result += (result - 1);
			return result;
		}


		#region Instance sequence getters

		/// <summary>Gets the sequence. The enumeration stops after yielding n values, with n being the sequence length.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public IEnumerable<T> GetSequence<T>(T logicalZero, T logicalOne)
		{
			if (_sequenceLength <= UInt32.MaxValue)
				return GetSequence32<T>((uint)_sequenceLength, (uint)_tap, (uint)_sequenceLength, logicalZero, logicalOne);
			else
				return GetSequence64<T>(_sequenceLength, _tap, _sequenceLength, logicalZero, logicalOne);
		}

		/// <summary>Gets the sequence. The enumeration stops after yielding n values, with n being the sequence length.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public IEnumerable<T> GetSequence<T>(T logicalZero, T logicalOne, ulong startValue)
		{
			if (_sequenceLength <= UInt32.MaxValue)
				return GetSequence32<T>((uint)_sequenceLength, (uint)_tap, (uint)startValue, logicalZero, logicalOne);
			else
				return GetSequence64<T>(_sequenceLength, _tap, startValue, logicalZero, logicalOne);
		}



		/// <summary>Gets the sequence. The enumeration never stops, thus you are resonsible for stopping it. The values are repeated after n values, where n is the <see cref="Length"/>.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public IEnumerable<T> GetInfiniteSequence<T>(T logicalZero, T logicalOne)
		{
			if (_sequenceLength <= UInt32.MaxValue)
				return GetInfiniteSequence32<T>((uint)_sequenceLength, (uint)_tap, (uint)_sequenceLength, logicalZero, logicalOne);
			else
				return GetInfiniteSequence64<T>(_sequenceLength, _tap, _sequenceLength, logicalZero, logicalOne);
		}

		/// <summary>Gets the sequence. The enumeration never stops, thus you are resonsible for stopping it. The values are repeated after n values, where n is the <see cref="Length"/>.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public IEnumerable<T> GetInfiniteSequence<T>(T logicalZero, T logicalOne, ulong startValue)
		{
			if (_sequenceLength <= UInt32.MaxValue)
				return GetInfiniteSequence32<T>((uint)_sequenceLength, (uint)_tap, (uint)startValue, logicalZero, logicalOne);
			else
				return GetInfiniteSequence64<T>(_sequenceLength, _tap, startValue, logicalZero, logicalOne);
		}

		#endregion Instance sequence getters

		#region static 32 bit methods

		/// <summary>Gets the sequence. The enumeration stops after yielding n values, with n being the sequence length.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="sequenceLength">Length of the sequence. Must be a number that is 2^k-1, where k is an integer value (k=2..32).</param>
		/// <param name="tap">Tap value to generate the sequence. If you don't know what a tap value is, you should probably use the instance function <see cref="GetSequence{T}(T,T)"/>.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public static IEnumerable<T> GetSequence32<T>(uint sequenceLength, uint tap, uint startValue, T logicalZero, T logicalOne)
		{
			uint upperbit = sequenceLength ^ (sequenceLength >> 1);
			uint seq = startValue & sequenceLength;
			if (0 == seq)
				throw new ArgumentOutOfRangeException("The provided start value is zero when and-ing it with the sequence length");

			for (var i = sequenceLength; i != 0; --i)
			{
				yield return 0 == (seq & upperbit) ? logicalZero : logicalOne;

				// determine the parity of seq & _tap => if parity is odd, at the end the least bit in x is set to 1  (see BinaryMath.IsParityOdd)
				uint x = seq & tap;
				x ^= x >> 16;
				x ^= x >> 8;
				x ^= x >> 4;
				x ^= x >> 2;
				x ^= x >> 1;
				seq = (seq << 1) | (x & 1u);
			}
		}

		/// <summary>Gets the sequence. The enumeration never stops, thus you are resonsible for stopping it. The values are repeated after n values, where n is the <see cref="Length"/>.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="sequenceLength">Length of the sequence. Must be a number that is 2^k-1, where k is an integer value (k=2..32).</param>
		/// <param name="tap">Tap value to generate the sequence. If you don't know what a tap value is, you should probably use the instance function <see cref="GetSequence{T}(T,T)"/>.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public static IEnumerable<T> GetInfiniteSequence32<T>(uint sequenceLength, uint tap, uint startValue, T logicalZero, T logicalOne)
		{
			uint upperbit = sequenceLength ^ (sequenceLength >> 1);
			uint seq = startValue & sequenceLength;
			if (0 == seq)
				throw new ArgumentOutOfRangeException("The provided start value is zero when and-ing it with the sequence length");

			for (; ; )
			{
				yield return 0 == (seq & upperbit) ? logicalZero : logicalOne;

				// determine the parity of seq & _tap => if parity is odd, at the end the least bit in x is set to 1  (see BinaryMath.IsParityOdd)
				uint x = seq & tap;
				x ^= x >> 16;
				x ^= x >> 8;
				x ^= x >> 4;
				x ^= x >> 2;
				x ^= x >> 1;
				seq = (seq << 1) | (x & 1u);
			}
		}

		#endregion 32 bit

		#region static 64 bit methods

		/// <summary>Gets the sequence. The enumeration stops after yielding n values, with n being the sequence length.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="sequenceLength">Length of the sequence. Must be a number that is 2^k-1, where k is an integer value (k=2..64).</param>
		/// <param name="tap">Tap value to generate the sequence. If you don't know what a tap value is, you should probably use the instance function <see cref="GetSequence{T}(T,T)"/>.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public static IEnumerable<T> GetSequence64<T>(ulong sequenceLength, ulong tap, ulong startValue, T logicalZero, T logicalOne)
		{
			ulong upperbit = sequenceLength ^ (sequenceLength >> 1);
			ulong seq = startValue & sequenceLength;
			if (0 == seq)
				throw new ArgumentOutOfRangeException("The provided start value is zero when and-ing it with the sequence length");

			for (var i = sequenceLength; i != 0; --i)
			{
				yield return 0 == (seq & upperbit) ? logicalZero : logicalOne;

				// determine the parity of seq & _tap => if parity is odd, at the end the least bit in x is set to 1  (see BinaryMath.IsParityOdd)
				ulong xx = seq & tap;
				uint x = ((uint)xx) ^ ((uint)(xx >> 32));
				x ^= x >> 16;
				x ^= x >> 8;
				x ^= x >> 4;
				x ^= x >> 2;
				x ^= x >> 1;
				seq = (seq << 1) | (x & 1);
			}
		}

		/// <summary>Gets the sequence. The enumeration never stops, thus you are resonsible for stopping it. The values are repeated after n values, where n is the <see cref="Length"/>.</summary>
		/// <typeparam name="T">Designates the type of the members of the sequence.</typeparam>
		/// <param name="sequenceLength">Length of the sequence. Must be a number that is 2^k-1, where k is an integer value (k=2..64).</param>
		/// <param name="tap">Tap value to generate the sequence. If you don't know what a tap value is, you should probably use the instance function <see cref="GetSequence{T}(T,T)"/>.</param>
		/// <param name="startValue">The initial value of the sequence. Normally, this value is initialized with the value of the sequence length. Here you can provide any other value that is non-zero when and-ing it with the sequence length.</param>
		/// <param name="logicalZero">The value that is returned if the value of the binary sequence is logical zero.</param>
		/// <param name="logicalOne">The value that is returned if the value of the binary sequence is logical one.</param>
		/// <returns>Values of the maximum length sequence, where a logical value of zero is mapped to the parameter <paramref name="logicalZero"/> and a logical one is mapped to <paramref name="logicalOne"/>.</returns>
		public static IEnumerable<T> GetInfiniteSequence64<T>(ulong sequenceLength, ulong tap, ulong startValue, T logicalZero, T logicalOne)
		{
			ulong upperbit = sequenceLength ^ (sequenceLength >> 1);
			ulong seq = startValue & sequenceLength;
			if (0 == seq)
				throw new ArgumentOutOfRangeException("The provided start value is zero when and-ing it with the sequence length");

			for (; ; )
			{
				yield return 0 == (seq & upperbit) ? logicalZero : logicalOne;

				// determine the parity of seq & _tap => if parity is odd, at the end the least bit in x is set to 1  (see BinaryMath.IsParityOdd)
				ulong xx = seq & tap;
				uint x = ((uint)xx) ^ ((uint)(xx >> 32));
				x ^= x >> 16;
				x ^= x >> 8;
				x ^= x >> 4;
				x ^= x >> 2;
				x ^= x >> 1;
				seq = (seq << 1) | (x & 1);
			}
		}

		#endregion 64 bit

	}
}
