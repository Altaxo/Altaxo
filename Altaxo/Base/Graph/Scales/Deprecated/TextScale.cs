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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Rescaling;

namespace Altaxo.Graph.Scales.Deprecated
{
  /// <summary>
  /// Represents the deprecated text scale implementation.
  /// </summary>
  [Serializable]
  public class TextScale : Scale
  {
    /// <summary>Holds the <see cref="TextBoundaries"/> for that axis.</summary>
    protected TextBoundaries _dataBounds;

    /// <summary>
    /// Stores the rescaling conditions for this text scale.
    /// </summary>
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
      /// <inheritdoc />
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextScale)obj;

        info.AddValue("Bounds", s._dataBounds);
        info.AddValue("Rescaling", s._rescaling);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextScale?)o ?? new TextScale();

        s.InternalSetDataBounds((TextBoundaries)info.GetValue("Bounds", s));
        s.InternalSetRescaling((NumericScaleRescaleConditions)info.GetValue("Rescaling", s));
        s.ProcessDataBounds();

        return s;
      }




    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="TextScale"/> class.
    /// </summary>
    public TextScale()
    {
      _dataBounds = new TextBoundaries
      {
        ParentObject = this
      };

      _rescaling = new LinearScaleRescaleConditions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextScale"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public TextScale(TextScale from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_dataBounds), nameof(_rescaling))]
    private void CopyFrom(TextScale from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      _dataBounds = (TextBoundaries)from._dataBounds.Clone();
      _dataBounds.ParentObject = this;

      ChildCloneToMember(ref _rescaling, from._rescaling);
      _cachedAxisOrg = from._cachedAxisOrg;
      _cachedAxisEnd = from._cachedAxisEnd;
      _cachedAxisSpan = from._cachedAxisSpan;
      _cachedOneByAxisSpan = from._cachedOneByAxisSpan;
    }

    /// <inheritdoc />
    public override object Clone()
    {
      var result = new TextScale();
      result.CopyFrom(this);
      return result;
    }

    /// <inheritdoc />
    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataBounds is not null)
        yield return new Main.DocumentNodeAndName(_dataBounds, "DataBounds");
      if (_rescaling is not null)
        yield return new Main.DocumentNodeAndName(_rescaling, "Rescaling");
    }

    /// <summary>
    /// Sets the data bounds used by this scale.
    /// </summary>
    /// <param name="bounds">The bounds to assign.</param>
    protected void InternalSetDataBounds(TextBoundaries bounds)
    {
      ChildSetMember(ref _dataBounds, bounds);
    }

    /// <summary>
    /// Sets the rescaling object used by this scale.
    /// </summary>
    /// <param name="rescaling">The rescaling object to assign.</param>
    protected void InternalSetRescaling(NumericScaleRescaleConditions rescaling)
    {
      _rescaling = rescaling;
      _rescaling.ParentObject = this;
    }

    /// <summary>
    /// Handles boundary changes by recalculating the scale.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void EhBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new AltaxoVariant(_cachedAxisOrg + x * _cachedAxisSpan);
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      var result = new Altaxo.Data.AltaxoVariant[_dataBounds.NumberOfItems];
      for (int i = 0; i < result.Length; i++)
        result[i] = new AltaxoVariant(_dataBounds.GetItem(i));

      return result;
    }

    /// <inheritdoc />
    public override double[] GetMajorTicksNormal()
    {
      double[] result = new double[_dataBounds.NumberOfItems];
      for (int i = 0; i < result.Length; i++)
      {
        result[i] = ((i + 1) - _cachedAxisOrg) * _cachedOneByAxisSpan;
      }

      return result;
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
    {
      var result = new AltaxoVariant[_dataBounds.NumberOfItems + 1];
      for (int i = 0; i < result.Length; i++)
        result[i] = new Altaxo.Data.AltaxoVariant(i + 0.5);

      return result;
    }

    /// <inheritdoc />
    public override double[] GetMinorTicksNormal()
    {
      double[] result = new double[_dataBounds.NumberOfItems + 1];
      for (int i = 0; i < result.Length; i++)
        result[i] = ((i + 0.5) - _cachedAxisOrg) * _cachedOneByAxisSpan;

      return result;
    }

    /// <inheritdoc />
    public override object RescalingObject
    {
      get
      {
        return _rescaling;
      }
    }

    /// <inheritdoc />
    public override IPhysicalBoundaries DataBoundsObject
    {
      get { return _dataBounds; }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void ProcessDataBounds()
    {
      if (_dataBounds is null || _dataBounds.IsEmpty)
        return;

      ProcessDataBounds(1, _dataBounds.NumberOfItems, _rescaling);
    }

    /// <summary>
    /// Updates the scale from data bounds and the specified rescaling conditions.
    /// </summary>
    /// <param name="xorg">The lower data bound.</param>
    /// <param name="xend">The upper data bound.</param>
    /// <param name="rescaling">The rescaling conditions to apply.</param>
    public void ProcessDataBounds(double xorg, double xend, NumericScaleRescaleConditions rescaling)
    {
      rescaling.OnDataBoundsChanged(xorg, xend);
      ProcessDataBounds(rescaling.ResultingOrg, rescaling.IsResultingOrgFixed, rescaling.ResultingEnd, rescaling.IsResultingEndFixed);
    }

    /// <inheritdoc />
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
