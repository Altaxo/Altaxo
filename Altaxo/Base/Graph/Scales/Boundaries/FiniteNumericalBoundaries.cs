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

using System;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// FinitePhysicalBoundaries is intended to use for LinearAxis
  /// it keeps track of the most negative and most positive finite value
  /// </summary>
  [Serializable]
  public class FiniteNumericalBoundaries : NumericalBoundaries
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.FinitePhysicalBoundaries", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Boundaries.FiniteNumericalBoundaries", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FiniteNumericalBoundaries), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FiniteNumericalBoundaries)obj;
        info.AddBaseValueEmbedded(s, typeof(FiniteNumericalBoundaries).BaseType);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FiniteNumericalBoundaries s = null != o ? (FiniteNumericalBoundaries)o : new FiniteNumericalBoundaries();
        info.GetBaseValueEmbedded(s, typeof(FiniteNumericalBoundaries).BaseType, parent);
        return s;
      }
    }

    #endregion Serialization

    public FiniteNumericalBoundaries()
        : base()
    {
    }

    public FiniteNumericalBoundaries(FiniteNumericalBoundaries c)
        : base(c)
    {
    }

    public override object Clone()
    {
      return new FiniteNumericalBoundaries(this);
    }

    public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
    {
      var v = col[idx];
      // if column is not numeric, use the index instead
      double d = v.IsNativeNumeric ? v.ToDouble() : idx; // Note: string should _not_ convert to double, even if it represents a number!
      return Add(d);
    }

    public override bool Add(Altaxo.Data.AltaxoVariant item)
    {
      return Add(item.ToDouble());
    }

    public virtual bool Add(double d)
    {
      if (IsSuspended) // when suspended: performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
      {
        if (!(double.IsNaN(d) || double.IsInfinity(d)))
        {
          if (d < _minValue)
            _minValue = d;
          if (d > _maxValue)
            _maxValue = d;
          _numberOfItems++;
          return true;
        }
      }
      else // not suspended: normal behaviour with change notification
      {
        if (!(double.IsNaN(d) || double.IsInfinity(d)))
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
  }
}
