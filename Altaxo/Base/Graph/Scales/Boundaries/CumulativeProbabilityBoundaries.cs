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
  /// This type of boundary is intended for cumulative probability data in the range (0,1) (note the open boundaries)
  /// it keeps track of the smallest and the highest probability value that are neither 0 nor 1.
  /// </summary>
  [Serializable]
  public class CumulativeProbabilityBoundaries : NumericalBoundaries
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CumulativeProbabilityBoundaries), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CumulativeProbabilityBoundaries)obj;
        info.AddBaseValueEmbedded(s, s.GetType().BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (CumulativeProbabilityBoundaries?)o ?? new CumulativeProbabilityBoundaries();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    public CumulativeProbabilityBoundaries()
      : base()
    {
    }

    public CumulativeProbabilityBoundaries(CumulativeProbabilityBoundaries c)
      : base(c)
    {
    }

    public override object Clone()
    {
      return new CumulativeProbabilityBoundaries(this);
    }

    public override bool Add(IReadableColumn col, int idx)
    {
      return Add((col is INumericColumn) ? ((INumericColumn)col)[idx] : idx);
    }

    public override bool Add(Altaxo.Data.AltaxoVariant val)
    {
      return Add(val.ToDouble());
    }

    public bool Add(double d)
    {
      if (IsSuspended) // when suspended: performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
      {
        if ((d > 0) && (d < 1))
        {
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
        if ((d > 0) && (d < 1))
        {
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

    public override double LowerBound
    {
      get
      {
        return _minValue;
      }
    }

    public override double UpperBound
    {
      get
      {
        return _maxValue;
      }
    }
  }
}
