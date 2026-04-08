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
  /// Defines a linearly spaced closed interval defined by start, end and step size.
  /// </summary>
  public record LinearlySpacedIntervalByStartEndStep : ISpacedInterval
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
    /// Surrogate class for XML serialization of <see cref="LinearlySpacedIntervalByStartEndStep"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearlySpacedIntervalByStartEndStep), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>
      /// Serializes the specified <see cref="LinearlySpacedIntervalByStartEndStep"/> instance.
      /// </summary>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="info">The serialization information.</param>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearlySpacedIntervalByStartEndStep)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("End", s.End);
        info.AddValue("Step", s.Step);
      }

      /// <summary>
      /// Deserializes an object of type <see cref="LinearlySpacedIntervalByStartEndStep"/>.
      /// </summary>
      /// <param name="o">The object to deserialize.</param>
      /// <param name="info">The deserialization information.</param>
      /// <param name="parent">The parent object, if any.</param>
      /// <returns>
      /// The deserialized <see cref="LinearlySpacedIntervalByStartEndStep"/> object.
      /// </returns>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetDouble("Start");
        var end = info.GetDouble("End");
        var step = info.GetDouble("Step");

        return new LinearlySpacedIntervalByStartEndStep(start, end, step);
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="LinearlySpacedIntervalByStartEndStep"/> class.
    /// </summary>
    /// <param name="start">The start of the interval (inclusive).</param>
    /// <param name="end">The end of the interval (inclusive).</param>
    /// <param name="step">The step size.</param>
    public LinearlySpacedIntervalByStartEndStep(double start, double end, double step)
    {
      if (!(Math.Sign(end - start) == Math.Sign(step)))
        throw new ArgumentException($"Sign of {nameof(step)}={step} does not match sign of ({nameof(end)}-{nameof(start)})={end - start}");

      Start = start;
      End = end;
      Step = step;

      if (End - Start == 0)
      {
        Count = 1;
      }
      else
      {
        var r = (End - Start) / Step;
        var ru = Math.Ceiling(r);
        Count = Math.Abs((ru - r) / ru) < 1E-6 ? Math.Max(0, (int)(ru + 1)) : Math.Max(0, (int)ru);
      }
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="LinearlySpacedIntervalByStartEndStep"/> class,
    /// with an interval [0,100], step size of 1 and count=101.
    /// </summary>
    public LinearlySpacedIntervalByStartEndStep() : this(0, 100, 1)
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

    /// <summary>
    /// Gets a value indicating whether the start of the interval is editable.
    /// </summary>
    public bool IsStartEditable => true;

    /// <summary>
    /// Gets a value indicating whether the end of the interval is editable.
    /// </summary>
    public bool IsEndEditable => true;

    /// <summary>
    /// Gets a value indicating whether the step size is editable.
    /// </summary>
    public bool IsStepEditable => true;

    /// <summary>
    /// Gets a value indicating whether the count of elements is editable.
    /// </summary>
    public bool IsCountEditable => false;
  }
}
