#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Defines a linearly spaced closed interval defined by start, end, and number of elements.
  /// </summary>
  public record LinearlySpacedIntervalByStartEndCount : ISpacedInterval
  {
    /// <summary>
    /// Start of the interval (inclusive).
    /// </summary>
    public double Start { get; }

    /// <summary>
    /// Start of the interval (inclusive if step can divide the interval by an integer number).
    /// </summary>
    public double End { get; }

    /// <summary>
    /// Gets the step size.
    /// </summary>
    public double Step { get; }

    /// <summary>The number of elements.</summary>
    public int Count { get; }

    /// <summary>The number of elements.</summary>
    public int Length => Count;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearlySpacedIntervalByStartEndCount), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearlySpacedIntervalByStartEndCount)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("End", s.End);
        info.AddValue("Count", s.Count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetDouble("Start");
        var end = info.GetDouble("End");
        var count = info.GetInt32("Count");

        return new LinearlySpacedIntervalByStartEndCount(start, end, count);
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="LinearlySpacedIntervalByStartEndCount"/> class.
    /// </summary>
    /// <param name="start">The start of the interval (inclusive).</param>
    /// <param name="end">The end of the interval (inclusive).</param>
    /// <param name="count">The number of elements.</param>
    public LinearlySpacedIntervalByStartEndCount(double start, double end, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), "Must be >=0");

      if (count <= 1 && !(start == end))
        throw new ArgumentException($"If {nameof(count)}={count}, then {nameof(start)} must be equal to {nameof(end)}");

      Start = start;
      End = end;
      Count = count;
      Step = count <= 1 ? 0 : (End - Start) / (count - 1);
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="LinearlySpacedIntervalByStartEndCount"/> class,
    /// with an interval [0,100], step size of 1 and count=101.
    /// </summary>
    public LinearlySpacedIntervalByStartEndCount() : this(0, 100, 101)
    {
    }

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <returns>Element at the specified index.</returns>
    public double this[int index] => Start + index * Step;

    /// <inheritdoc/>
    public IEnumerator<double> GetEnumerator()
    {
      for (var i = 0; i < Count; ++i)
      {
        yield return this[i];
      }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      for (var i = 0; i < Count; ++i)
      {
        yield return this[i];
      }
    }

    public bool IsStartEditable => true;
    public bool IsEndEditable => true;
    public bool IsStepEditable => false;
    public bool IsCountEditable => true;
  }
}
