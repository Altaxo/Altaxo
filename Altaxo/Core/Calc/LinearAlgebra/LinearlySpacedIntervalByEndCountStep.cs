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
using Altaxo.Main;

namespace Altaxo.Calc.LinearAlgebra
{
  public interface ISpacedInterval : IROVector<double>, IImmutable
  {
    /// <summary>
    /// Start of the interval (inclusive).
    /// </summary>
    double Start { get; }

    /// <summary>
    /// Start of the interval (inclusive if step can divide the interval by an integer number).
    /// </summary>
    double End { get; }

    /// <summary>
    /// Gets the step size.
    /// </summary>
    double Step { get; }

    bool IsStartEditable { get; }
    bool IsEndEditable { get; }
    bool IsStepEditable { get; }
    bool IsCountEditable { get; }


  }

  /// <summary>
  /// Defines a linearly spaced closed interval defined by end, number of elements, and step size.
  /// </summary>
  public record LinearlySpacedIntervalByEndCountStep : ISpacedInterval
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearlySpacedIntervalByEndCountStep), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearlySpacedIntervalByEndCountStep)obj;
        info.AddValue("End", s.End);
        info.AddValue("Count", s.Count);
        info.AddValue("Step", s.Step);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var end = info.GetDouble("End");
        var count = info.GetInt32("Count");
        var step = info.GetDouble("Step");

        return new LinearlySpacedIntervalByEndCountStep(end, count, step);
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="LinearlySpacedIntervalByEndCountStep"/> class.
    /// </summary>
    /// <param name="end">The end of the interval (inclusive).</param>
    /// <param name="count">The number of elements.</param>
    /// <param name="step">The step size.</param>
    public LinearlySpacedIntervalByEndCountStep(double end, int count, double step)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), "Must be >=0");

      End = end;
      Count = count;
      Step = step;
      Start = End - Math.Max(0, count - 1) * Step;
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="LinearlySpacedIntervalByEndCountStep"/> class,
    /// with an interval [0,100], step size of 1 and count=101.
    /// </summary>
    public LinearlySpacedIntervalByEndCountStep() : this(100, 101, 1)
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

    public bool IsStartEditable => false;
    public bool IsEndEditable => true;
    public bool IsStepEditable => true;
    public bool IsCountEditable => true;
  }
}
