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

using Altaxo.Data;
using System;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// PositiveFinitePhysicalBoundaries is intended to use for logarithmic axis
  /// it keeps track of the smallest positive and biggest positive value
  /// </summary>
  [Serializable]
  public class InverseNumericalBoundaries : NumericalBoundaries
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InverseNumericalBoundaries), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (InverseNumericalBoundaries)obj;
        info.AddBaseValueEmbedded(s, s.GetType().BaseType);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (InverseNumericalBoundaries)o ?? new InverseNumericalBoundaries();
        info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);
        return s;
      }
    }

    #endregion Serialization

    public InverseNumericalBoundaries()
      : base()
    {
    }

    public InverseNumericalBoundaries(InverseNumericalBoundaries c)
      : base(c)
    {
    }

    public override object Clone()
    {
      return new InverseNumericalBoundaries(this);
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

    public override double LowerBound
    {
      get
      {
        return 1 / _minValue;
      }
    }

    public override double UpperBound
    {
      get
      {
        return 1 / _maxValue;
      }
    }
  }
}
