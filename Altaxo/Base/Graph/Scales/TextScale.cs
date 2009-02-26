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

		/// <summary>True if org is allowed to be extended to smaller values.</summary>
		protected bool _isOrgExtendable;

		/// <summary>True if end is allowed to be extended to higher values.</summary>
		protected bool _isEndExtendable;


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextScale), 1)]
		class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				TextScale s = (TextScale)obj;

				info.AddValue("Org", s._cachedAxisOrg);
				info.AddValue("End", s._cachedAxisEnd);  

				info.AddValue("Rescaling", s._rescaling);
				info.AddValue("Bounds", s._dataBounds);
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

				s._cachedAxisOrg = (double)info.GetDouble("Org");
				s._cachedAxisEnd = (double)info.GetDouble("End");
				s._cachedAxisSpan = s._cachedAxisEnd - s._cachedAxisOrg;
				s._cachedOneByAxisSpan = 1 / s._cachedAxisSpan;
				s._isOrgExtendable = false;
				s._isEndExtendable = false;


				s.InternalSetRescaling((NumericAxisRescaleConditions)info.GetValue("Rescaling", s));

				s.InternalSetDataBounds((TextBoundaries)info.GetValue("Bounds", s));
		
				return s;
			}
		}
		#endregion

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


		protected void InternalSetDataBounds(TextBoundaries bounds)
		{
			if (this._dataBounds != null)
			{
				this._dataBounds.BoundaryChanged -= new BoundaryChangedHandler(this.EhBoundariesChanged);
				this._dataBounds = null;
			}
			this._dataBounds = bounds;
			this._dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.EhBoundariesChanged);
		}

		protected void InternalSetRescaling(NumericAxisRescaleConditions rescaling)
		{
			this._rescaling = rescaling;
		}

    protected void EhBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      Rescale(); // calculate new bounds and fire AxisChanged event
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
    }

    public override Altaxo.Data.AltaxoVariant EndAsVariant
    {
      get
      {
        return new AltaxoVariant(_cachedAxisEnd); 
      }
    }

		/// <summary>Returns true if it is allowed to extend the origin (to lower values).</summary>
		public override bool IsOrgExtendable
		{
			get { return _isOrgExtendable; }
		}

		/// <summary>Returns true if it is allowed to extend the scale end (to higher values).</summary>
		public override bool IsEndExtendable
		{
			get { return _isEndExtendable; }
		}

		public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
			double o = org.ToDouble();
			double e = end.ToDouble();

			if (!(o < e))
				return "org is not less than end";

			InternalSetOrgEnd(o, e, false, false);

			return null;
		}

		private void InternalSetOrgEnd(double org, double end, bool isOrgExtendable, bool isEndExtendable)
		{
			bool changed = _cachedAxisOrg != org ||
				_cachedAxisEnd != end ||
				_isOrgExtendable != isOrgExtendable ||
				_isEndExtendable != isEndExtendable;

			_cachedAxisOrg = org;
			_cachedAxisEnd = end;
			_cachedAxisSpan = end - org;
			_cachedOneByAxisSpan = 1 / _cachedAxisSpan;

			_isOrgExtendable = isOrgExtendable;
			_isEndExtendable = isEndExtendable;

			if (changed)
				OnChanged();

		}

		public override void Rescale()
		{
			double xorg = 0;
			double xend = 1;

			if (null != _dataBounds && !_dataBounds.IsEmpty)
			{
				xorg = 0.5;
				xend = _dataBounds.NumberOfItems + 0.5;
			}

			bool isAutoOrg, isAutoEnd;
			_rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);

			InternalSetOrgEnd(xorg, xend, isAutoOrg, isAutoEnd);
		}

  }

}
