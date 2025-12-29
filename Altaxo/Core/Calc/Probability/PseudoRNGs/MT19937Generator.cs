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
 * MT19937Generator.cs, 24.09.2006
 *
 * 09.08.2006: Initial version
 * 17.08.2006: Improved performance of Next(minValue, maxValue) for (maxValue - minValue) > int.MaxValue
 * 21.09.2006: Refined some comments
 *             Adjusted NextBytes so it checks whether the passed buffer is a null reference
 *               and throws an ArgumentNullException if needed
 * 23.09.2006: Moved reinitialization code from virtual Reset method to new private ResetGenerator
 *               method, so it can be safely called from the constructor (and Reset too)
 * 24.09.2006: All exceptions are instantiated with localized messages
 *
 */

#region original copyrights

/*
   A C-program for MT19937, with initialization improved 2002/1/26.
   Coded by Takuji Nishimura and Makoto Matsumoto.

   Before using, initialize the state by using init_genrand(seed)
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote
        products derived from this software without specific prior written
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/

#endregion original copyrights

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Represents a Mersenne Twister pseudo-random number generator with period 2^19937 - 1.
  /// </summary>
  /// <remarks>
  /// The <see cref="MT19937Generator"/> type is based on information and the implementation presented on the
  /// <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">Mersenne Twister Home Page</a>.
  /// </remarks>
  public class MT19937Generator : Generator
  {
    #region class fields

    /// <summary>
    /// Represents the number of unsigned random numbers generated at one time. This field is constant.
    /// </summary>
    /// <remarks>The value of this constant is 624.</remarks>
    private const int N = 624;

    /// <summary>
    /// Represents a constant used for generation of unsigned random numbers. This field is constant.
    /// </summary>
    /// <remarks>The value of this constant is 397.</remarks>
    private const int M = 397;

    /// <summary>
    /// Represents the constant vector a. This field is constant.
    /// </summary>
    /// <remarks>The value of this constant is 0x9908b0dfU.</remarks>
    private const uint VectorA = 0x9908b0dfU;

    /// <summary>
    /// Represents the most significant w-r bits. This field is constant.
    /// </summary>
    /// <remarks>The value of this constant is 0x80000000.</remarks>
    private const uint UpperMask = 0x80000000U;

    /// <summary>
    /// Represents the least significant r bits. This field is constant.
    /// </summary>
    /// <remarks>The value of this constant is 0x7fffffff.</remarks>
    private const uint LowerMask = 0x7fffffffU;

    /// <summary>
    /// Represents the multiplier that computes a double-precision floating-point number greater than or equal to 0.0
    /// and less than 1.0 when it is applied to a nonnegative 32-bit signed integer.
    /// </summary>
    private const double IntToDoubleMultiplier = 1.0 / (int.MaxValue + 1.0);

    /// <summary>
    /// Represents the multiplier that computes a double-precision floating-point number greater than or equal to 0.0
    /// and less than 1.0 when it is applied to a 32-bit unsigned integer.
    /// </summary>
    private const double UIntToDoubleMultiplier = 1.0 / (uint.MaxValue + 1.0);

    #endregion class fields

    #region instance fields

    /// <summary>
    /// Stores the state vector array.
    /// </summary>
    private uint[] mt;

    /// <summary>
    /// Stores an index for the state vector array element that will be accessed next.
    /// </summary>
    private uint mti;

    /// <summary>
    /// Stores the seed value used.
    /// </summary>
    private uint seed;

    /// <summary>
    /// Stores the seed array used.
    /// </summary>
    private uint[]? seedArray;

    /// <summary>
    /// Stores a value used to generate up to 32 random <see cref="bool"/> values.
    /// </summary>
    private uint bitBuffer;

    /// <summary>
    /// Stores how many random <see cref="bool"/> values still can be generated from <see cref="bitBuffer"/>.
    /// </summary>
    private int bitCount;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="MT19937Generator"/> class using a time-dependent default seed value.
    /// </summary>
    public MT19937Generator()
      : this((uint)Math.Abs(Environment.TickCount))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MT19937Generator"/> class using the specified seed value.
    /// </summary>
    /// <param name="seed">
    /// A number used to calculate a starting value for the pseudo-random number sequence.
    /// If a negative number is specified, the absolute value of the number is used.
    /// </param>
    public MT19937Generator(int seed)
      : this((uint)Math.Abs(seed))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MT19937Generator"/> class using the specified seed value.
    /// </summary>
    /// <param name="seed">An unsigned number used to calculate a starting value for the pseudo-random number sequence.</param>
    public MT19937Generator(uint seed)
    {
      mt = new uint[MT19937Generator.N];
      this.seed = seed;
      seedArray = null;
      ResetGenerator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MT19937Generator"/> class using the specified seed array.
    /// </summary>
    /// <param name="seedArray">
    /// An array of numbers used to calculate starting values for the pseudo-random number sequence.
    /// If negative numbers are specified, their absolute values are used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="seedArray"/> is <see langword="null"/> (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public MT19937Generator(int[] seedArray)
    {
      if (seedArray is null)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentNull, "seedArray");
        throw new ArgumentNullException("seedArray", message);
      }

      mt = new uint[MT19937Generator.N];
      seed = 19650218U;
      this.seedArray = new uint[seedArray.Length];
      for (int index = 0; index < seedArray.Length; index++)
      {
        this.seedArray[index] = (uint)Math.Abs(seedArray[index]);
      }
      ResetGenerator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MT19937Generator"/> class using the specified seed array.
    /// </summary>
    /// <param name="seedArray">An array of unsigned numbers used to calculate starting values for the pseudo-random number sequence.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="seedArray"/> is <see langword="null"/> (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public MT19937Generator(uint[] seedArray)
    {
      if (seedArray is null)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentNull, "seedArray");
        throw new ArgumentNullException("seedArray", message);
      }

      mt = new uint[MT19937Generator.N];
      seed = 19650218U;
      this.seedArray = seedArray;
      ResetGenerator();
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Resets the <see cref="MT19937Generator"/> so that it produces the same pseudo-random number sequence again.
    /// </summary>
    private void ResetGenerator()
    {
      mt[0] = seed & 0xffffffffU;
      for (mti = 1; mti < MT19937Generator.N; mti++)
      {
        mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
        // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
        // In the previous versions, MSBs of the seed affect only MSBs of the array mt[].
        // 2002/01/09 modified by Makoto Matsumoto
      }

      // If the object was instantiated with a seed array do some further (re)initialisation.
      if (seedArray is not null)
      {
        ResetBySeedArray();
      }

      // Reset helper variables used for generation of random bools.
      bitBuffer = 0;
      bitCount = 32;
    }

    /// <summary>
    /// Extends resetting of the <see cref="MT19937Generator"/> using the <see cref="seedArray"/>.
    /// </summary>
    private void ResetBySeedArray()
    {
      if (seedArray is null)
        throw new InvalidProgramException();

      uint i = 1;
      uint j = 0;
      int k = (MT19937Generator.N > (seedArray.Length)) ? MT19937Generator.N : (seedArray.Length);
      for (; k > 0; k--)
      {
        mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + seedArray[j] + j; // non linear
        i++;
        j++;
        if (i >= MT19937Generator.N)
        {
          mt[0] = mt[MT19937Generator.N - 1];
          i = 1;
        }
        if (j >= seedArray.Length)
        {
          j = 0;
        }
      }
      for (k = MT19937Generator.N - 1; k > 0; k--)
      {
        mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941U)) - i; // non linear
        i++;
        if (i >= MT19937Generator.N)
        {
          mt[0] = mt[MT19937Generator.N - 1];
          i = 1;
        }
      }

      mt[0] = 0x80000000U; // MSB is 1; assuring non-0 initial array
    }

    /// <summary>
    /// Generates <see cref="MT19937Generator.N"/> unsigned random numbers.
    /// </summary>
    /// <remarks>
    /// Generated random numbers are 32-bit unsigned integers greater than or equal to <see cref="uint.MinValue"/>
    /// and less than or equal to <see cref="uint.MaxValue"/>.
    /// </remarks>
    private void GenerateNUInts()
    {
      int kk;
      uint y;
      uint[] mag01 = new uint[2] { 0x0U, MT19937Generator.VectorA };

      for (kk = 0; kk < MT19937Generator.N - MT19937Generator.M; kk++)
      {
        y = (mt[kk] & MT19937Generator.UpperMask) | (mt[kk + 1] & MT19937Generator.LowerMask);
        mt[kk] = mt[kk + MT19937Generator.M] ^ (y >> 1) ^ mag01[y & 0x1U];
      }
      for (; kk < MT19937Generator.N - 1; kk++)
      {
        y = (mt[kk] & MT19937Generator.UpperMask) | (mt[kk + 1] & MT19937Generator.LowerMask);
        mt[kk] = mt[kk + (MT19937Generator.M - MT19937Generator.N)] ^ (y >> 1) ^ mag01[y & 0x1U];
      }
      y = (mt[MT19937Generator.N - 1] & MT19937Generator.UpperMask) | (mt[0] & MT19937Generator.LowerMask);
      mt[MT19937Generator.N - 1] = mt[MT19937Generator.M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];

      mti = 0;
    }

    /// <summary>
    /// Returns an unsigned random number.
    /// </summary>
    /// <returns>
    /// A 32-bit unsigned integer greater than or equal to <see cref="uint.MinValue"/> and less than or equal to
    /// <see cref="uint.MaxValue"/>.
    /// </returns>
    public uint NextUInt()
    {
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }

      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      return (y ^ (y >> 18));
    }

    /// <summary>
    /// Returns a nonnegative random number less than or equal to <see cref="int.MaxValue"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to 0 and less than or equal to <see cref="int.MaxValue"/>;
    /// that is, the range of return values includes 0 and <see cref="int.MaxValue"/>.
    /// </returns>
    public int NextInclusiveMaxValue()
    {
      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      return (int)(y >> 1);
    }

    #endregion instance methods

    #region overridden Generator members

    /// <inheritdoc/>
    public override bool CanReset
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc/>
    public override bool Reset()
    {
      ResetGenerator();
      return true;
    }

    /// <inheritdoc/>
    public override int Next()
    {
      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      int result = (int)(y >> 1);
      // Exclude Int32.MaxValue from the range of return values.
      if (result == int.MaxValue)
      {
        return Next();
      }
      else
      {
        return result;
      }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxValue"/> is less than 0.
    /// </exception>
    public override int Next(int maxValue)
    {
      if (maxValue < 0)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentOutOfRangeGreaterEqual,
            "maxValue", "0");
        throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
      }

      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      // The shift operation and extra int cast before the first multiplication give better performance.
      // See comment in NextDouble().
      return (int)((int)(y >> 1) * MT19937Generator.IntToDoubleMultiplier * maxValue);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    public override int Next(int minValue, int maxValue)
    {
      if (minValue > maxValue)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentOutOfRangeGreaterEqual,
            "maxValue", "minValue");
        throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
      }

      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      int range = maxValue - minValue;
      if (range < 0)
      {
        // The range is greater than Int32.MaxValue, so we have to use slower floating point arithmetic.
        // Also all 32 random bits (uint) have to be used which again is slower (See comment in NextDouble()).
        return minValue + (int)
            (y * MT19937Generator.UIntToDoubleMultiplier * (maxValue - (double)minValue));
      }
      else
      {
        // 31 random bits (int) will suffice which allows us to shift and cast to an int before the first multiplication and gain better performance.
        // See comment in NextDouble().
        return minValue + (int)((int)(y >> 1) * MT19937Generator.IntToDoubleMultiplier * range);
      }
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      // Here a ~2x speed improvement is gained by computing a value that can be cast to an int
      //   before casting to a double to perform the multiplication.
      // Casting a double from an int is a lot faster than from an uint and the extra shift operation
      //   and cast to an int are very fast (the allocated bits remain the same), so overall there's
      //   a significant performance improvement.
      return (int)(y >> 1) * MT19937Generator.IntToDoubleMultiplier;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxValue"/> is less than 0.
    /// </exception>
    public override double NextDouble(double maxValue)
    {
      if (maxValue < 0.0)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentOutOfRangeGreaterEqual,
            "maxValue", "0.0");
        throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
      }

      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      // The shift operation and extra int cast before the first multiplication give better performance.
      // See comment in NextDouble().
      return (int)(y >> 1) * MT19937Generator.IntToDoubleMultiplier * maxValue;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
    /// <see cref="double.MaxValue"/>.
    /// </exception>
    public override double NextDouble(double minValue, double maxValue)
    {
      if (minValue > maxValue)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentOutOfRangeGreaterEqual,
            "maxValue", "minValue");
        throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
      }

      double range = maxValue - minValue;

      if (range == double.PositiveInfinity)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentRangeLessEqual,
            "minValue", "maxValue", "Double.MaxValue");
        throw new ArgumentException(message);
      }

      // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
      if (mti >= MT19937Generator.N)
      {// generate N words at one time
        GenerateNUInts();
      }
      uint y = mt[mti++];
      // Tempering
      y ^= (y >> 11);
      y ^= (y << 7) & 0x9d2c5680U;
      y ^= (y << 15) & 0xefc60000U;
      y ^= (y >> 18);

      // The shift operation and extra int cast before the first multiplication give better performance.
      // See comment in NextDouble().
      return minValue + (int)(y >> 1) * MT19937Generator.IntToDoubleMultiplier * range;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Buffers 32 random bits (1 <see cref="uint"/>) for future calls, so a new random number is generated only once
    /// per 32 calls.
    /// </remarks>
    public override bool NextBoolean()
    {
      if (bitCount == 32)
      {
        // Generate 32 more bits (1 uint) and store it for future calls.
        // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
        if (mti >= MT19937Generator.N)
        {// generate N words at one time
          GenerateNUInts();
        }
        uint y = mt[mti++];
        // Tempering
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        bitBuffer = (y ^ (y >> 18));

        // Reset the bitCount and use rightmost bit of buffer to generate random bool.
        bitCount = 1;
        return (bitBuffer & 0x1) == 1;
      }

      // Increase the bitCount and use rightmost bit of shifted buffer to generate random bool.
      bitCount++;
      return ((bitBuffer >>= 1) & 0x1) == 1;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="buffer"/> is a null reference (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public override void NextBytes(byte[] buffer)
    {
      if (buffer is null)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentNull, "buffer");
        throw new ArgumentNullException("buffer", message);
      }

      // Fill the buffer with 4 bytes (1 uint) at a time.
      int i = 0;
      uint y;
      while (i < buffer.Length - 3)
      {
        // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
        if (mti >= MT19937Generator.N)
        {// generate N words at one time
          GenerateNUInts();
        }
        y = mt[mti++];
        // Tempering
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= (y >> 18);

        buffer[i++] = (byte)y;
        buffer[i++] = (byte)(y >> 8);
        buffer[i++] = (byte)(y >> 16);
        buffer[i++] = (byte)(y >> 24);
      }

      // Fill up any remaining bytes in the buffer.
      if (i < buffer.Length)
      {
        // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
        if (mti >= MT19937Generator.N)
        {// generate N words at one time
          GenerateNUInts();
        }
        y = mt[mti++];
        // Tempering
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= (y >> 18);

        buffer[i++] = (byte)y;
        if (i < buffer.Length)
        {
          buffer[i++] = (byte)(y >> 8);
          if (i < buffer.Length)
          {
            buffer[i++] = (byte)(y >> 16);
            if (i < buffer.Length)
            {
              buffer[i] = (byte)(y >> 24);
            }
          }
        }
      }
    }

    #endregion overridden Generator members
  }
}
