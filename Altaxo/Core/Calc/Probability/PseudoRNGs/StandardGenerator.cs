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
 * StandardGenerator.cs, 24.09.2006
 *
 * 09.08.2006: Initial version
 * 23.09.2006: Moved reinitialization code from virtual Reset method to new private ResetGenerator
 *               method, so it can be safely called from the constructor (and Reset too)
 * 24.09.2006: All exceptions are instantiated with localized messages
 *
 */

using System;

namespace Altaxo.Calc.Probability
{
  /// <summary>
  /// Represents a simple pseudo-random number generator.
  /// </summary>
  /// <remarks>
  /// The <see cref="StandardGenerator"/> type internally uses an instance of <see cref="System.Random"/>
  /// to generate pseudo-random numbers.
  /// </remarks>
  public class StandardGenerator : Generator
  {
    #region instance fields

    /// <summary>
    /// Stores an instance of <see cref="System.Random"/> that is used to generate random numbers.
    /// </summary>
    private System.Random generator;

    /// <summary>
    /// Stores the seed value used.
    /// </summary>
    private int seed;

    /// <summary>
    /// Stores a value used to generate up to 31 random <see cref="bool"/> values.
    /// </summary>
    private int bitBuffer;

    /// <summary>
    /// Stores how many random <see cref="bool"/> values still can be generated from <see cref="bitBuffer"/>.
    /// </summary>
    private int bitCount;

    #endregion instance fields

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardGenerator"/> class using a time-dependent default seed value.
    /// </summary>
    public StandardGenerator()
      : this(Environment.TickCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardGenerator"/> class using the specified seed value.
    /// </summary>
    /// <param name="seed">
    /// A number used to calculate a starting value for the pseudo-random number sequence.
    /// If a negative number is specified, the absolute value of the number is used.
    /// </param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public StandardGenerator(int seed)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
      this.seed = Math.Abs(seed);
      ResetGenerator();
    }

    #endregion construction

    #region instance methods

    /// <summary>
    /// Resets the <see cref="StandardGenerator"/> so that it produces the same pseudo-random number sequence again.
    /// </summary>
    private void ResetGenerator()
    {
      // Create a new Random object using the same seed.
      generator = new System.Random(seed);

      // Reset helper variables used for generation of random bools.
      bitBuffer = 0;
      bitCount = 0;
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
      return generator.Next();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxValue"/> is less than 0.
    /// </exception>
    public override int Next(int maxValue)
    {
      return generator.Next(maxValue);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    public override int Next(int minValue, int maxValue)
    {
      return generator.Next(minValue, maxValue);
    }

    /// <inheritdoc/>
    public override double NextDouble()
    {
      return generator.NextDouble();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxValue"/> is less than 0.
    /// </exception>
    public override double NextDouble(double maxValue)
    {
      if (maxValue < 0)
      {
        string message = string.Format(null, ExceptionMessages.ArgumentOutOfRangeGreaterEqual,
            "maxValue", "0.0");
        throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
      }

      return generator.NextDouble() * maxValue;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> is greater than
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

      return minValue + generator.NextDouble() * range;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Buffers 31 random bits (1 <see cref="int"/>) for future calls, so a new random number is generated only once
    /// per 31 calls.
    /// </remarks>
    public override bool NextBoolean()
    {
      if (bitCount == 0)
      {
        // Generate 31 more bits (1 int) and store it for future calls.
        bitBuffer = generator.Next();

        // Reset the bitCount and use rightmost bit of buffer to generate random bool.
        bitCount = 30;
        return (bitBuffer & 0x1) == 1;
      }

      // Decrease the bitCount and use rightmost bit of shifted buffer to generate random bool.
      bitCount--;
      return ((bitBuffer >>= 1) & 0x1) == 1;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="buffer"/> is a null reference (<see langword="Nothing"/> in Visual Basic).
    /// </exception>
    public override void NextBytes(byte[] buffer)
    {
      generator.NextBytes(buffer);
    }

    #endregion overridden Generator members
  }
}
