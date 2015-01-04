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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Scales
{
	/// <summary>
	/// Scales a full circle, either by degree or by radian. The origin is choosable, and the ticks default to ratios of 180° (or Pi, respectively).
	/// </summary>
	public abstract class AngularScale : NumericalScale
	{
		/// <summary>
		/// The value where this scale starts. Default is 0. The user/programmer can set this value manually.
		/// </summary>
		protected double _cachedAxisOrg;

		protected double _cachedAxisSpan;
		protected double _cachedOneByAxisSpan;
		protected Boundaries.NumericalBoundaries _dataBounds;
		protected Rescaling.AngularRescaleConditions _rescaling;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularScale), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AngularScale s = (AngularScale)obj;

				info.AddValue("Rescaling", s._rescaling);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularScale s = SDeserialize(o, info, parent);
				OnAfterDeserialization(s);
				return s;
			}

			protected virtual void OnAfterDeserialization(AngularScale s)
			{
				s.SetCachedValues();
			}

			protected virtual AngularScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularScale s = (AngularScale)o;

				s._rescaling = (Rescaling.AngularRescaleConditions)info.GetValue("Rescaling", s);
				s._rescaling.ParentObject = s;
				// set cached values is called by the enclosing function
				return s;
			}
		}

		#endregion Serialization

		public AngularScale()
		{
			_cachedAxisSpan = 2 * Math.PI;
			_cachedOneByAxisSpan = 1 / _cachedAxisSpan;
			_dataBounds = new Boundaries.DummyNumericalBoundaries();
			_rescaling = new Rescaling.AngularRescaleConditions() { ParentObject = this };
			SetCachedValues();
		}

		public AngularScale(AngularScale from)
		{
			this._cachedAxisOrg = from._cachedAxisOrg;
			this._cachedAxisSpan = from._cachedAxisSpan;
			this._cachedOneByAxisSpan = from._cachedOneByAxisSpan;
			this._rescaling = (Rescaling.AngularRescaleConditions)from._rescaling.Clone();
			this._rescaling.ParentObject = this;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataBounds)
				yield return new Main.DocumentNodeAndName(_dataBounds, "DataBounds");
			if (null != _rescaling)
				yield return new Main.DocumentNodeAndName(_rescaling, "Rescaling");
		}

		private void SetCachedValues()
		{
			double scaleOrigin = _rescaling.ScaleOrigin % 360;
			if (UseDegree)
			{
				_cachedAxisOrg = scaleOrigin;
				_cachedAxisSpan = 360;
				_cachedOneByAxisSpan = 1.0 / 360;
			}
			else
			{
				_cachedAxisOrg = scaleOrigin * Math.PI / 180;
				_cachedAxisSpan = 2 * Math.PI;
				_cachedOneByAxisSpan = 1 / (2 * Math.PI);
			}
		}

		#region Properties

		/// <summary>If true, use degree instead of radian.</summary>
		protected abstract bool UseDegree { get; }

		#endregion Properties

		#region NumericalScale

		public override double PhysicalToNormal(double x)
		{
			return (x - _cachedAxisOrg) * _cachedOneByAxisSpan;
		}

		public override double NormalToPhysical(double x)
		{
			return _cachedAxisOrg + x * _cachedAxisSpan;
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
			return PhysicalToNormal(x.ToDouble());
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

		private bool IsDoubleEqual(double x, double y, double dev)
		{
			return Math.Abs(x - y) < dev;
		}

		private double GetOriginInDegrees()
		{
			return _rescaling.ScaleOrigin % 360;
		}

		public override Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions Rescaling
		{
			get
			{
				return null;
			}
		}

		public override Altaxo.Graph.Scales.Boundaries.NumericalBoundaries DataBounds
		{
			get
			{
				return _dataBounds;
			}
		}

		public override double Org
		{
			get
			{
				return _cachedAxisOrg;
			}
		}

		public override double End
		{
			get
			{
				return _cachedAxisOrg + _cachedAxisSpan;
			}
		}

		/// <summary>Returns true if it is allowed to extend the origin (to lower values).</summary>
		public override bool IsOrgExtendable
		{
			get { return false; }
		}

		/// <summary>Returns true if it is allowed to extend the scale end (to higher values).</summary>
		public override bool IsEndExtendable
		{
			get { return false; }
		}

		public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
			// ignore all this stuff, org and end are fixed here!
			/*
			double o = org.ToDouble();
			double e = end.ToDouble();

			if (!(o < e))
				return "org is not less than end";

			InternalSetOrgEnd(o, e, false, false);
			*/
			return null;
		}

		private void InternalSetOrgEnd(double org, double end, bool isOrgExtendable, bool isEndExtendable)
		{
			double angle = UseDegree ? org : org * 180 / Math.PI;
			// round the angle to full 90°
			angle = Math.Round(angle / 90);
			angle = Math.IEEERemainder(angle, 4);
			int scaleOrigin = (int)angle;
			org = UseDegree ? GetOriginInDegrees() : GetOriginInDegrees() * Math.PI / 180;
			double span = Math.Abs(end - org);

			bool changed = _cachedAxisOrg != org ||
				_cachedAxisSpan != span;

			_cachedAxisOrg = org;
			_cachedAxisSpan = span;
			_cachedOneByAxisSpan = 1 / _cachedAxisSpan;

			if (changed)
				EhSelfChanged(EventArgs.Empty);
		}

		public override void Rescale()
		{
		}

		#endregion NumericalScale
	}
}