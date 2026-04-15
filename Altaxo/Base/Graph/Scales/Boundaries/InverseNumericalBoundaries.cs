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

#nullable enable
using System;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// Tracks inverse-transformed numerical values for inverse scales.
  /// </summary>
  [Serializable]
  public class InverseNumericalBoundaries : NumericalBoundaries
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InverseNumericalBoundaries), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (InverseNumericalBoundaries)o;
        info.AddBaseValueEmbedded(s, s.GetType().BaseType!);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (InverseNumericalBoundaries?)o ?? new InverseNumericalBoundaries();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="InverseNumericalBoundaries"/> class.
    /// </summary>
    public InverseNumericalBoundaries()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InverseNumericalBoundaries"/> class by copying another instance.
    /// </summary>
    /// <param name="c">The instance to copy.</param>
    public InverseNumericalBoundaries(InverseNumericalBoundaries c)
      : base(c)
    {
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new InverseNumericalBoundaries(this);
    }

    /// <inheritdoc />
    public override bool Add(IReadableColumn col, int idx)
    {
      return Add((col is INumericColumn) ? ((INumericColumn)col)[idx] : idx);
    }

    /// <inheritdoc />
    public override bool Add(Altaxo.Data.AltaxoVariant item)
    {
      return Add(item.ToDouble());
    }

    /// <summary>
    /// Adds a numeric value to the inverse boundary tracker.
    /// </summary>
    /// <param name="d">The value to add.</param>
    /// <returns><see langword="true"/> if the value contributed to the boundaries; otherwise, <see langword="false"/>.</returns>
    public bool Add(double d)
    {
      if (IsSuspended) // when suspended: performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
      {
        if (d != 0 && !double.IsNaN(d))
        {
          d = 1 / d;
          if (d < _minValue)
            _minValue = d;
          if (d > _maxValue)
            _maxValue = d;
          _numberOfItems++;
          return true;
        }
      }
      else  // not suspended: normal behaviour with change notification
      {
        if (d != 0 && !double.IsNaN(d))
        {
          d = 1 / d;
          BoundariesChangedData data = BoundariesChangedData.NumberOfItemsChanged;
          if (d < _minValue)
          { _minValue = d; data |= BoundariesChangedData.LowerBoundChanged; }
          if (d > _maxValue)
          { _maxValue = d; data |= BoundariesChangedData.UpperBoundChanged; }
          _numberOfItems++;

          EhSelfChanged(new BoundariesChangedEventArgs(data));

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Gets the lower boundary in original scale coordinates.
    /// </summary>
    public override double LowerBound
    {
      get
      {
        return 1 / _minValue;
      }
    }

    /// <summary>
    /// Gets the upper boundary in original scale coordinates.
    /// </summary>
    public override double UpperBound
    {
      get
      {
        return 1 / _maxValue;
      }
    }
  }
}
