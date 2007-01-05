#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Serialization;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Data;

namespace Altaxo.Graph.Scales
{
  [Serializable]
  public class TextScale : Scale
  {
    /// <summary>Holds the <see cref="TextBoundaries"/> for that axis.</summary>
    protected TextBoundaries _dataBounds;

    protected NumericAxisRescaleConditions _rescaling;


    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected double _cachedAxisOrg = 0;
    /// <summary>Current axis end (cached value).</summary>
    protected double _cachedAxisEnd = 1;
    /// <summary>Current axis span (i.e. end-org) (cached value).</summary>
    protected double _cachedAxisSpan = 1;
    /// <summary>Current inverse of axis span (cached value).</summary>
    protected double _cachedOneByAxisSpan = 1;


    public TextScale()
    {
      _dataBounds = new TextBoundaries();
      _dataBounds.BoundaryChanged += EhBoundariesChanged;
      
      _rescaling = new NumericAxisRescaleConditions();
    }

    public TextScale(TextScale from)
    {
      CopyFrom(from);
    }

    void CopyFrom(TextScale from)
    {
      _dataBounds = (TextBoundaries)from._dataBounds.Clone();
      _dataBounds.BoundaryChanged += EhBoundariesChanged;

      _rescaling = from._rescaling == null ? null : (NumericAxisRescaleConditions)from._rescaling.Clone();

      _cachedAxisOrg = from._cachedAxisOrg;
      _cachedAxisEnd = from._cachedAxisEnd;
      _cachedAxisSpan = from._cachedAxisSpan;
      _cachedOneByAxisSpan = from._cachedOneByAxisSpan;
    }

    public override object Clone()
    {
      TextScale result = new TextScale();
      result.CopyFrom(this);
      return result;
    }

    protected void EhBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
    }

    public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
    {
      if (x.IsType(Altaxo.Data.AltaxoVariant.Content.VString))
      {
        int idx = _dataBounds.IndexOf(x.ToString());
        return idx<0? double.NaN : (1+idx - _cachedAxisOrg) * _cachedOneByAxisSpan; 
      }
      else if (x.CanConvertedToDouble)
      {
        return (x.ToDouble() - _cachedAxisOrg) * _cachedOneByAxisSpan;
      }
      else
      {
        return double.NaN;
      }
    }

    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new AltaxoVariant(_cachedAxisOrg + x * _cachedAxisSpan);
    }

    public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      AltaxoVariant[] result = new Altaxo.Data.AltaxoVariant[_dataBounds.NumberOfItems];
      for (int i = 0; i < result.Length; i++)
        result[i] = new AltaxoVariant(_dataBounds.GetItem(i));

      return result;
    }

    public override double[] GetMajorTicksNormal()
    {
      double[] result = new double[_dataBounds.NumberOfItems];
      for (int i = 0; i < result.Length; i++)
      {
        result[i] = ((i + 1) - _cachedAxisOrg) * _cachedOneByAxisSpan;
      }

      return result;
    }

    public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
    {
      AltaxoVariant[] result = new AltaxoVariant[_dataBounds.NumberOfItems+1];
      for (int i = 0; i < result.Length; i++)
        result[i] = new Altaxo.Data.AltaxoVariant(i + 0.5);

      return result;
    }

    public override double[] GetMinorTicksNormal()
    {
      double[] result = new double[_dataBounds.NumberOfItems + 1];
      for (int i = 0; i < result.Length; i++)
        result[i] = ((i + 0.5) - _cachedAxisOrg) * _cachedOneByAxisSpan;

      return result;
    }

    public override object RescalingObject
    {
      get
      {
        return _rescaling;
      }
    }

    public override IPhysicalBoundaries DataBoundsObject
    {
      get { return _dataBounds; }
    }

    public override Altaxo.Data.AltaxoVariant OrgAsVariant
    {
      get 
      {
        return new AltaxoVariant(_cachedAxisOrg);
      }
      set
      {
        _cachedAxisOrg = value.ToDouble();
        ProcessDataBounds(_cachedAxisOrg, true, _cachedAxisEnd, true);
      }
    }

    public override Altaxo.Data.AltaxoVariant EndAsVariant
    {
      get
      {
        return new AltaxoVariant(_cachedAxisEnd); 
      }
      set
      {
        _cachedAxisEnd = value.ToDouble();
        ProcessDataBounds(_cachedAxisOrg, true, _cachedAxisEnd, true);
      }
    }

    public override void ProcessDataBounds()
    {
      if (null == this._dataBounds || this._dataBounds.IsEmpty)
        return;

      ProcessDataBounds(1, _dataBounds.NumberOfItems, _rescaling); 
    }

    public void ProcessDataBounds(double xorg, double xend, NumericAxisRescaleConditions rescaling)
    {
      bool isAutoOrg, isAutoEnd;
      rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);
      ProcessDataBounds(xorg, !isAutoOrg, xend, !isAutoEnd);
    }


    public override void ProcessDataBounds(Altaxo.Data.AltaxoVariant org, bool orgfixed, Altaxo.Data.AltaxoVariant end, bool endfixed)
    {
      double dorg = org.ToDouble();
      double dend = end.ToDouble();

      if (!orgfixed)
      {
        dorg = Math.Ceiling(dorg) - 0.5;
      }
      if (!endfixed)
      {
        dend = Math.Floor(dend) + 0.5;
      }

      bool changed = false;
      changed |= _cachedAxisOrg != dorg;
      _cachedAxisOrg = dorg;

      changed |= _cachedAxisEnd != dend;
      _cachedAxisEnd = dend;

      changed |= _cachedAxisSpan != (dend - dorg);
      _cachedAxisSpan = dend - dorg;

      changed |= _cachedOneByAxisSpan != (1 / _cachedAxisSpan);
      _cachedOneByAxisSpan = 1 / _cachedAxisSpan;

      if(changed)
        OnChanged();
    }
  }
 

}
