using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Defines a linear spaced closed interval defined by start, end and step.
  /// </summary>
  public record LinearlySpacedIntervalByStartEndStep : IImmutable, IROVector<double>
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
    public int Length { get; }

    /// <summary>The number of elements.</summary>
    public int Count => Length;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearlySpacedIntervalByStartEndStep), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearlySpacedIntervalByStartEndStep)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("End", s.End);
        info.AddValue("Step", s.Step);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var start = info.GetDouble("Start");
        var end = info.GetDouble("End");
        var step = info.GetDouble("Step");

        return new LinearlySpacedIntervalByStartEndStep(start, end, step);
      }
    }
    #endregion


    public LinearlySpacedIntervalByStartEndStep(double start, double end, double step)
    {
      if (!(Math.Sign(end - start) == Math.Sign(step)))
        throw new ArgumentException($"Sign of step={step} does not match sign of (end-start)={end - start}");

      Start = start;
      End = end;
      Step = step;

      if (End - Start == 0)
      {
        Length = 1;
      }
      else
      {
        var r = (End - Start) / Step;
        var ru = Math.Ceiling(r);
        Length = Math.Abs((ru - r) / ru) < 1E-6 ? Math.Max(0, (int)(ru + 1)) : Math.Max(0, (int)ru);
      }
    }



    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <returns>Element at the specified index.</returns>
    public double this[int index] => Start + index * Step;

    /// <inheritdoc/>
    public IEnumerator<double> GetEnumerator()
    {
      for (var i = 0; i < Length; ++i)
      {
        yield return this[i];
      }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      for (var i = 0; i < Length; ++i)
      {
        yield return this[i];
      }
    }
  }

  // TODO
  // By:
  // StartEndCount,
  // StartCountInterval,
  // EndCountInterval



}
