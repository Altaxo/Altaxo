/*
 * Copyright © 2006 Stefan Troschütz (stefan@troschuetz.de)
 *
 * This file is part of Troschuetz.Random Class Library.
 *
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * Generator.cs, 09.08.2006
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
	/// <summary>
	/// Declares common functionality for all random number generators.
	/// </summary>
	public abstract class Generator
	{
		/// <summary>
		/// Gets a value indicating whether the random number generator can be reset, so that it produces the same
		///   random number sequence again.
		/// </summary>
		public abstract bool CanReset
		{
			get;
		}

		/// <summary>
		/// Resets the random number generator, so that it produces the same random number sequence again.
		/// </summary>
		/// <returns>
		/// <see langword="true"/>, if the random number generator was reset; otherwise, <see langword="false"/>.
		/// </returns>
		public abstract bool Reset();

		/// <summary>
		/// Gets the maximum number that a call to <see cref="Next()"/> can return.
		/// </summary>
		public virtual int Maximum { get { return int.MaxValue - 1; } }

		/// <summary>
		/// Returns a nonnegative random number less than <see cref="Int32.MaxValue"/>.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to 0, and less than <see cref="Int32.MaxValue"/>; that is,
		///   the range of return values includes 0 but not <see cref="Int32.MaxValue"/>.
		/// </returns>
		public abstract int Next();

		/// <summary>
		/// Returns a nonnegative random number less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number to be generated.
		/// <paramref name="maxValue"/> must be greater than or equal to 0.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to 0, and less than <paramref name="maxValue"/>; that is,
		///   the range of return values includes 0 but not <paramref name="maxValue"/>.
		/// </returns>
		public abstract int Next(int maxValue);

		/// <summary>
		/// Returns a random number within the specified range.
		/// </summary>
		/// <param name="minValue">
		/// The inclusive lower bound of the random number to be generated.
		/// </param>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number to be generated.
		/// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to <paramref name="minValue"/>, and less than
		///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but
		///   not <paramref name="maxValue"/>.
		/// </returns>
		public abstract int Next(int minValue, int maxValue);

		/// <summary>
		/// Returns a nonnegative floating point random number less than 1.0.
		/// </summary>
		/// <returns>
		/// A double-precision floating point number greater than or equal to 0.0, and less than 1.0; that is,
		///   the range of return values includes 0.0 but not 1.0.
		/// </returns>
		public abstract double NextDouble();

		/// <summary>
		/// Returns a floating point random number in the range (0,1), i.e. greater than 0.0 and less than 1.0.
		/// </summary>
		/// <returns>
		/// A double-precision floating point number greater than or equal to 0.0, and less than 1.0; that is,
		///   the range of return values includes 0.0 but not 1.0.
		/// </returns>
		public virtual double NextPositiveDouble()
		{
			double result;
			do
			{
				result = NextDouble();
			} while (0 == result);
			return result;
		}

		/// <summary>
		/// Returns a nonnegative floating point random number less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number to be generated.
		/// <paramref name="maxValue"/> must be greater than or equal to 0.0.
		/// </param>
		/// <returns>
		/// A double-precision floating point number greater than or equal to 0.0, and less than <paramref name="maxValue"/>;
		///   that is, the range of return values includes 0 but not <paramref name="maxValue"/>.
		/// </returns>
		public abstract double NextDouble(double maxValue);

		/// <summary>
		/// Returns a floating point random number within the specified range.
		/// </summary>
		/// <param name="minValue">
		/// The inclusive lower bound of the random number to be generated.
		/// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
		///   <see cref="Double.MaxValue"/>
		/// </param>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number to be generated.
		/// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
		/// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
		///   <see cref="Double.MaxValue"/>.
		/// </param>
		/// <returns>
		/// A double-precision floating point number greater than or equal to <paramref name="minValue"/>, and less than
		///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but
		///   not <paramref name="maxValue"/>.
		/// </returns>
		public abstract double NextDouble(double minValue, double maxValue);

		/// <summary>
		/// Returns a random Boolean value.
		/// </summary>
		/// <remarks>
		/// Buffers 31 random bits for future calls, so the random number generator is only invoked once in every 31 calls.
		/// </remarks>
		/// <returns>A <see cref="Boolean"/> value.</returns>
		public abstract bool NextBoolean();

		/// <summary>
		/// Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <remarks>
		/// Each element of the array of bytes is set to a random number greater than or equal to 0, and less than or
		///   equal to <see cref="Byte.MaxValue"/>.
		/// </remarks>
		/// <param name="buffer">An array of bytes to contain random numbers.</param>
		public abstract void NextBytes(byte[] buffer);
	}
}