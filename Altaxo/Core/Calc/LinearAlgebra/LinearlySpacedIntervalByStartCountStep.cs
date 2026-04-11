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
  /// Defines a linearly spaced closed interval defined by start, number of elements, and step size.
  /// </summary>
  public record LinearlySpacedIntervalByStartCountStep : ISpacedInterval
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

    #region Serialization

    /// <summary>
    /// Serialization surrogate for <see cref="LinearlySpacedIntervalByStartCountStep"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearlySpacedIntervalByStartCountStep), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>
      /// Serializes the <see cref="LinearlySpacedIntervalByStartCountStep"/> object.
      /// </summary>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="info">The serialization info.</param>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearlySpacedIntervalByStartCountStep)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("Count", s.Count);
        info.AddValue("Step", s.Step);
      }

      /// <summary>
      /// Deserializes the <see cref="LinearlySpacedIntervalByStartCountStep"/> object.
      /// </summary>
      /// <param name="o">The object to deserialize into.</param>
      /// <param name="info">The deserialization info.</param>
      /// <param name="parent">The parent object.</param>
      /// <returns>
      /// The deserialized <see cref="LinearlySpacedIntervalByStartCountStep"/> object.
      /// </returns>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetDouble("Start");
        var count = info.GetInt32("Count");
        var step = info.GetDouble("Step");

        return new LinearlySpacedIntervalByStartCountStep(start, count, step);
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="LinearlySpacedIntervalByStartCountStep"/> class.
    /// </summary>
    /// <param name="start">The start of the interval (inclusive).</param>
    /// <param name="count">The number of elements.</param>
    /// <param name="step">The step size.</param>
    public LinearlySpacedIntervalByStartCountStep(double start, int count, double step)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), "Must be >=0");

      Start = start;
      Count = count;
      Step = step;
      End = Start + Math.Max(0, count - 1) * Step;
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="LinearlySpacedIntervalByStartCountStep"/> class,
    /// with an interval [0,100], step size of 1 and count=101.
    /// </summary>
    public LinearlySpacedIntervalByStartCountStep() : this(0, 101, 1)
    {
    }



    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based element index.</param>
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

    /// <summary>
    /// Gets a value indicating whether the start of the interval is editable.
    /// </summary>
    public bool IsStartEditable => true;

    /// <summary>
    /// Gets a value indicating whether the end of the interval is editable.
    /// </summary>
    public bool IsEndEditable => false;

    /// <summary>
    /// Gets a value indicating whether the step size is editable.
    /// </summary>
    public bool IsStepEditable => true;

    /// <summary>
    /// Gets a value indicating whether the count of elements is editable.
    /// </summary>
    public bool IsCountEditable => true;
  }
}
