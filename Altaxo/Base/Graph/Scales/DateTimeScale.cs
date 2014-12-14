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

namespace Altaxo.Graph.Scales
{
	using Boundaries;
	using Rescaling;

	/// <summary>
	/// Summary description for DateTimeAxis.
	/// </summary>
	[Serializable]
	public class DateTimeScale : Scale
	{
		// cached values
		/// <summary>Current axis origin (cached value).</summary>
		protected DateTime _axisOrg = DateTime.MinValue;

		/// <summary>Current axis end (cached value).</summary>
		protected DateTime _axisEnd = DateTime.MaxValue;

		/// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
		protected FiniteDateTimeBoundaries _dataBounds;

		protected DateTimeAxisRescaleConditions _rescaling;

		/// <summary>True if org is allowed to be extended to smaller values.</summary>
		protected bool _isOrgExtendable;

		/// <summary>True if end is allowed to be extended to higher values.</summary>
		protected bool _isEndExtendable;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeScale), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DateTimeScale s = (DateTimeScale)obj;

				info.AddValue("Org", s._axisOrg);
				info.AddValue("End", s._axisEnd);
				info.AddValue("Rescaling", s._rescaling);
				info.AddValue("Bounds", s._dataBounds);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeScale s = SDeserialize(o, info, parent);
				OnAfterDeserialization(s);
				return s;
			}

			public virtual void OnAfterDeserialization(DateTimeScale s)
			{
			}

			protected virtual DateTimeScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeScale s = null != o ? (DateTimeScale)o : new DateTimeScale();

				s._axisOrg = info.GetDateTime("Org");
				s._axisEnd = info.GetDateTime("End");
				s.InternalSetRescaling((DateTimeAxisRescaleConditions)info.GetValue("Rescaling", s));
				s.InternalSetDataBounds((FiniteDateTimeBoundaries)info.GetValue("Bounds", s));

				return s;
			}
		}

		#endregion Serialization

		#region ICloneable Members

		public void CopyFrom(DateTimeScale from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this._axisOrg = from._axisOrg;
			this._axisEnd = from._axisEnd;

			this.InternalSetDataBounds((FiniteDateTimeBoundaries)from._dataBounds.Clone());
			this.InternalSetRescaling((DateTimeAxisRescaleConditions)from._rescaling.Clone());
		}

		public DateTimeScale(DateTimeScale from)
		{
			CopyFrom(from);
		}

		public DateTimeScale()
		{
			this.InternalSetDataBounds(new FiniteDateTimeBoundaries());
			this.InternalSetRescaling(new DateTimeAxisRescaleConditions());
		}

		/// <summary>
		/// Creates a copy of the axis.
		/// </summary>
		/// <returns>The cloned copy of the axis.</returns>
		public override object Clone()
		{
			return new DateTimeScale(this);
		}

		#endregion ICloneable Members

		protected void InternalSetDataBounds(FiniteDateTimeBoundaries bounds)
		{
			if (this._dataBounds != null)
			{
				this._dataBounds.ParentObject = null;
				this._dataBounds = null;
			}
			this._dataBounds = bounds;
			this._dataBounds.ParentObject = this;
		}

		protected void InternalSetRescaling(DateTimeAxisRescaleConditions rescaling)
		{
			this._rescaling = rescaling;
		}

		/// <summary>
		/// PhysicalToNormal translates physical values into a normal value linear along the axis
		/// a physical value of the axis origin must return a value of zero
		/// a physical value of the axis end must return a value of one
		/// the function physicalToNormal must be provided by any derived class
		/// </summary>
		/// <param name="x">the physical value</param>
		/// <returns>
		/// the normalized value linear along the axis,
		/// 0 for axis origin, 1 for axis end</returns>
		public double PhysicalToNormal(DateTime x)
		{
			return (x - _axisOrg).TotalSeconds / (_axisEnd - _axisOrg).TotalSeconds;
		}

		/// <summary>
		/// NormalToPhysical is the inverse function to PhysicalToNormal
		/// It translates a normalized value (0 for the axis origin, 1 for the axis end)
		/// into the physical value
		/// </summary>
		/// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
		/// <returns>the corresponding physical value</returns>
		public DateTime NormalToPhysical(double x)
		{
			return _axisOrg.AddSeconds(x * (_axisEnd - _axisOrg).TotalSeconds);
		}

		/// <summary>
		/// PhysicalVariantToNormal translates physical values into a normal value linear along the axis
		/// a physical value of the axis origin must return a value of zero
		/// a physical value of the axis end must return a value of one
		/// the function physicalToNormal must be provided by any derived class
		/// </summary>
		/// <param name="x">the physical value</param>
		/// <returns>
		/// the normalized value linear along the axis,
		/// 0 for axis origin, 1 for axis end</returns>
		public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
		{
			if (x.IsType(AltaxoVariant.Content.VDateTime))
				return PhysicalToNormal((DateTime)x);
			else if (x.CanConvertedToDouble)
				return PhysicalToNormal(new DateTime((long)(x.ToDouble() * 10000000)));
			else throw new ArgumentException("Variant x is neither DateTime nor numeric");
		}

		/// <summary>
		/// NormalToPhysicalVariant is the inverse function to PhysicalToNormal
		/// It translates a normalized value (0 for the axis origin, 1 for the axis end)
		/// into the physical value
		/// </summary>
		/// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
		/// <returns>the corresponding physical value</returns>
		public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
		{
			return new Altaxo.Data.AltaxoVariant(NormalToPhysical(x));
		}

		public override AltaxoVariant OrgAsVariant
		{
			get
			{
				return new AltaxoVariant(Org);
			}
		}

		public override AltaxoVariant EndAsVariant
		{
			get
			{
				return new AltaxoVariant(End);
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
			DateTime o = (DateTime)org;
			DateTime e = (DateTime)end;

			if (!(o < e))
				return "org is not less than end";

			InternalSetOrgEnd(o, e, false, false);

			return null;
		}

		private void InternalSetOrgEnd(DateTime org, DateTime end, bool isOrgExtendable, bool isEndExtendable)
		{
			bool changed = _axisOrg != org ||
				_axisEnd != end ||
				_isOrgExtendable != isOrgExtendable ||
				_isEndExtendable != isEndExtendable;

			_axisOrg = org;
			_axisEnd = end;
			//_cachedAxisSpan = end - org;
			//_cachedOneByAxisSpan = 1 / _cachedAxisSpan;

			_isOrgExtendable = isOrgExtendable;
			_isEndExtendable = isEndExtendable;

			if (changed)
				EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Returns the rescaling conditions for this axis
		/// </summary>
		public DateTimeAxisRescaleConditions Rescaling
		{
			get
			{
				return this._rescaling;
			}
		}

		/// <summary>
		/// Returns the rescaling conditions for this axis
		/// </summary>
		public override object RescalingObject
		{
			get
			{
				return this._rescaling;
			}
		}

		/// <summary>
		/// Returns the <see cref="FiniteDateTimeBoundaries" /> object that is associated with that axis.
		/// </summary>
		public FiniteDateTimeBoundaries DataBounds
		{
			get
			{
				return this._dataBounds;
			}
		} // return a PhysicalBoundarie object that is associated with that axis

		/// <summary>
		/// Returns the <see cref="IPhysicalBoundaries"/> object that is associated with that axis.
		/// </summary>
		public override IPhysicalBoundaries DataBoundsObject
		{
			get
			{
				return this._dataBounds;
			}
		} // return a PhysicalBoundarie object that is associated with that axis

		/// <summary>The axis origin, i.e. the first point in physical units.</summary>
		public DateTime Org
		{
			get
			{
				return _axisOrg;
			}
		}

		/// <summary>The axis end point in physical units.</summary>
		public DateTime End
		{
			get
			{
				return _axisEnd;
			}
		}

		public override void Rescale()
		{
			DateTime xorg = DateTime.MinValue;
			DateTime xend = DateTime.MinValue.AddDays(1);

			if (null != _dataBounds && !_dataBounds.IsEmpty)
			{
				xorg = _dataBounds.LowerBound;
				xend = _dataBounds.UpperBound;
			}

			bool isAutoOrg, isAutoEnd;
			_rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);

			InternalSetOrgEnd(xorg, xend, isAutoOrg, isAutoEnd);
		}

		protected override void OnChanged(EventArgs e)
		{
			if (e is BoundariesChangedEventArgs)
			{
				Rescale();
			}

			base.OnChanged(e);
		}
	} // end of class
}