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

using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Rescaling;
using System;

namespace Altaxo.Graph.Scales
{
	/// <summary>
	/// A linear axis, i.e a axis where physical values v can be translated to logical values l by v=a+b*l.
	/// </summary>

	[Serializable]
	public class LinearScale : NumericalScale
	{
		/// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
		protected NumericalBoundaries _dataBounds;

		protected NumericScaleRescaleConditions _rescaling;

		protected Ticks.TickSpacing _tickSpacing;

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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.LinearScale", 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				LinearScale s = (LinearScale)obj;
				info.AddValue("Org", s._cachedAxisOrg);
				info.AddValue("End", s._cachedAxisEnd);
				info.AddValue("Rescaling", s._rescaling);
				info.AddValue("Bounds", s._dataBounds);
				 */
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinearScale s = (LinearScale)o ?? new LinearScale(info);

				s._cachedAxisOrg = (double)info.GetDouble("Org");
				s._cachedAxisEnd = (double)info.GetDouble("End");
				s._cachedAxisSpan = s._cachedAxisEnd - s._cachedAxisOrg;
				s._cachedOneByAxisSpan = 1 / s._cachedAxisSpan;

				s._rescaling = (NumericScaleRescaleConditions)info.GetValue("Rescaling", s);
				s._rescaling.ParentObject = s;

				s._dataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds", s);
				s._dataBounds.ParentObject = s;

				s._tickSpacing = new Ticks.LinearTickSpacing();
				s._tickSpacing.ParentObject = s;

				s.EhChildChanged(s._dataBounds, EventArgs.Empty); // for this old version, rescaling is not fully serialized, thus we have to simulate a DataBoundChanged event to get _rescaling updated, and finally _tickSpacing updated

				return s;
			}
		}

		/// <summary>
		/// 2015-02-11 Add Tickspacing
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearScale), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinearScale s = (LinearScale)obj;
				info.AddValue("Org", s._cachedAxisOrg);
				info.AddValue("End", s._cachedAxisEnd);
				info.AddValue("Bounds", s._dataBounds);
				info.AddValue("Rescaling", s._rescaling);
				info.AddValue("TickSpacing", s._tickSpacing);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinearScale s = (LinearScale)o ?? new LinearScale(info);

				s._cachedAxisOrg = (double)info.GetDouble("Org");
				s._cachedAxisEnd = (double)info.GetDouble("End");
				s._cachedAxisSpan = s._cachedAxisEnd - s._cachedAxisOrg;
				s._cachedOneByAxisSpan = 1 / s._cachedAxisSpan;

				s._dataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds", s);
				s._dataBounds.ParentObject = s;

				s._rescaling = (NumericScaleRescaleConditions)info.GetValue("Rescaling", s);
				s._rescaling.ParentObject = s;

				s._tickSpacing = (Ticks.TickSpacing)info.GetValue("TickSpacing", s);
				s._tickSpacing.ParentObject = s;

				s.UpdateTicksAndOrgEndUsingRescalingObject();

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Constructor for deserialization only.
		/// </summary>
		protected LinearScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		/// <summary>
		/// Creates a default linear axis with org=0 and end=1.
		/// </summary>
		public LinearScale()
		{
			_dataBounds = new FiniteNumericalBoundaries() { ParentObject = this };
			_rescaling = new LinearScaleRescaleConditions() { ParentObject = this };
			_tickSpacing = new Ticks.LinearTickSpacing() { ParentObject = this };
			UpdateTicksAndOrgEndUsingRescalingObject();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">A other linear axis from which to copy from.</param>
		public LinearScale(LinearScale from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as LinearScale;
			if (null != from)
			{
				this._cachedAxisEnd = from._cachedAxisEnd;
				this._cachedAxisOrg = from._cachedAxisOrg;
				this._cachedAxisSpan = from._cachedAxisSpan;
				this._cachedOneByAxisSpan = from._cachedOneByAxisSpan;

				ChildCopyToMemberOrCreateNew(ref _dataBounds, from._dataBounds, () => new FiniteNumericalBoundaries());
				ChildCopyToMemberOrCreateNew(ref _rescaling, from._rescaling, () => new LinearScaleRescaleConditions());
				ChildCopyToMemberOrCreateNew(ref _tickSpacing, from._tickSpacing, () => new Ticks.LinearTickSpacing());
				return true;
			}
			return false;
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataBounds)
				yield return new Main.DocumentNodeAndName(_dataBounds, () => _dataBounds = null, "DataBounds");

			if (null != _rescaling)
				yield return new Main.DocumentNodeAndName(_rescaling, () => _rescaling = null, "Rescaling");

			if (null != _tickSpacing)
				yield return new Main.DocumentNodeAndName(_tickSpacing, () => _tickSpacing = null, "TickSpacing");
		}

		public override object Clone()
		{
			return new LinearScale(this);
		}

		/// <summary>
		/// Get/sets the axis origin (physical units).
		/// </summary>
		public override double Org
		{
			get { return _cachedAxisOrg; }
		}

		/// <summary>
		/// Get/sets the axis end (physical units).
		/// </summary>
		public override double End
		{
			get { return _cachedAxisEnd; }
		}

		/// <summary>
		/// Returns the rescaling conditions for this axis
		/// </summary>
		public override NumericScaleRescaleConditions Rescaling
		{
			get
			{
				return _rescaling;
			}
		}

		/// <summary>
		/// Get the internal DataBound object (mostly for merging).
		/// </summary>
		public override NumericalBoundaries DataBounds
		{
			get { return _dataBounds; }
		}

		public override Ticks.TickSpacing TickSpacing
		{
			get
			{
				return _tickSpacing;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				if (ChildSetMember(ref _tickSpacing, value))
					EhChildChanged(Rescaling, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Converts a value in axis (physical) units to a "normalized" value, which is 0 for the axis org and 1 for the axis end.
		/// </summary>
		/// <param name="x">Value to convert (physical units).</param>
		/// <returns>Normalized value.</returns>
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
			return PhysicalToNormal(x.ToDoubleOrNaN());
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

		/// <summary>
		/// Returns the default axis boundaries when no valid data are entered into.
		/// </summary>
		/// <param name="org"></param>
		/// <param name="end"></param>
		public void GetDefaultAxisBoundaries(out double org, out double end)
		{
			org = 0;
			end = 1;
		}

		protected override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
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
				_cachedAxisEnd != end;

			_cachedAxisOrg = org;
			_cachedAxisEnd = end;
			_cachedAxisSpan = end - org;
			_cachedOneByAxisSpan = 1 / _cachedAxisSpan;

			if (changed)
				EhSelfChanged(EventArgs.Empty);
		}
	} // end of class LinearAxis
}