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
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Rescaling;
using System;

namespace Altaxo.Graph.Scales.Deprecated
{
  [Serializable]
  public class TextScale : Scale
  {
    /// <summary>Holds the <see cref="TextBoundaries"/> for that axis.</summary>
    protected TextBoundaries _dataBounds;

    protected NumericScaleRescaleConditions _rescaling;

    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected double _cachedAxisOrg = 0;

    /// <summary>Current axis end (cached value).</summary>
    protected double _cachedAxisEnd = 1;

    /// <summary>Current axis span (i.e. end-org) (cached value).</summary>
    protected double _cachedAxisSpan = 1;

    /// <summary>Current inverse of axis span (cached value).</summary>
    protected double _cachedOneByAxisSpan = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.TextScale", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        TextScale s = (TextScale)obj;

        info.AddValue("Bounds", s._dataBounds);
        info.AddValue("Rescaling", s._rescaling);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TextScale s = SDeserialize(o, info, parent);
        OnAfterDeserialization(s);
        return s;
      }

      public virtual void OnAfterDeserialization(TextScale s)
      {
      }

      protected virtual TextScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TextScale s = null != o ? (TextScale)o : new TextScale();

        s.InternalSetDataBounds((TextBoundaries)info.GetValue("Bounds", s));
        s.InternalSetRescaling((NumericScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ProcessDataBounds();

        return s;
      }
    }

    #endregion Serialization

    public TextScale()
    {
      _dataBounds = new TextBoundaries();
      _dataBounds.ParentObject = this;

      _rescaling = new LinearScaleRescaleConditions();
    }

    public TextScale(TextScale from)
    {
      CopyFrom(from);
    }

    private void CopyFrom(TextScale from)
    {
      if (object.ReferenceEquals(this, from))
        return;

      _dataBounds = (TextBoundaries)from._dataBounds.Clone();
      _dataBounds.ParentObject = this;

      _rescaling = from._rescaling == null ? null : (NumericScaleRescaleConditions)from._rescaling.Clone();
      _rescaling.ParentObject = this;

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

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _dataBounds)
        yield return new Main.DocumentNodeAndName(_dataBounds, "DataBounds");
      if (null != _rescaling)
        yield return new Main.DocumentNodeAndName(_rescaling, "Rescaling");
    }

    protected void InternalSetDataBounds(TextBoundaries bounds)
    {
      if (this._dataBounds != null)
      {
        this._dataBounds = null;
      }
      this._dataBounds = bounds;
      this._dataBounds.ParentObject = this;
    }

    protected void InternalSetRescaling(NumericScaleRescaleConditions rescaling)
    {
      this._rescaling = rescaling;
      this._rescaling.ParentObject = this;
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
        return idx < 0 ? double.NaN : (1 + idx - _cachedAxisOrg) * _cachedOneByAxisSpan;
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
      AltaxoVariant[] result = new AltaxoVariant[_dataBounds.NumberOfItems + 1];
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

    public void ProcessDataBounds(double xorg, double xend, NumericScaleRescaleConditions rescaling)
    {
      rescaling.OnDataBoundsChanged(xorg, xend);
      ProcessDataBounds(rescaling.ResultingOrg, rescaling.IsResultingOrgFixed, rescaling.ResultingEnd, rescaling.IsResultingEndFixed);
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

      if (changed)
        OnChanged();
    }
  }
}
