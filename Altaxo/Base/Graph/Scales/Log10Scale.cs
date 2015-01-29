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
	/// Represents a logarithmic axis, i.e. the physical values v correspond to logical values l by v=a*10^(b*l).
	/// </summary>
	[Serializable]
	public class Log10Scale : NumericalScale, System.Runtime.Serialization.IDeserializationCallback
	{
		/// <summary>Decimal logarithm of axis org. Should always been set together with <see cref="_cachedOrg"/>.</summary>
		private double _log10Org = 0; // Log10 of physical axis org

		/// <summary>Origin of the axis. This value is used to maintain numeric precision. Should always been set together with <see cref="_log10Org"/>.</summary>
		private double _cachedOrg = 1;

		/// <summary>Decimal logarithm of axis end.  Should always been set together with <see cref="_cachedEnd"/>.</summary>
		private double _log10End = 1; // Log10 of physical axis end

		/// <summary>Value of the end of the axis. This value is used to maintain numeric precision. Should always been set together with <see cref="_log10End"/>.</summary>
		private double _cachedEnd = 10;

		/// <summary>True if org is allowed to be extended to smaller values.</summary>
		protected bool _isOrgExtendable;

		/// <summary>True if end is allowed to be extended to higher values.</summary>
		protected bool _isEndExtendable;

		/// <summary>The boundary object. It collectes only positive values for the axis is logarithmic.</summary>
		protected NumericalBoundaries _dataBounds;

		protected LogarithmicAxisRescaleConditions _rescaling;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10Scale), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Log10Scale s = (Log10Scale)obj;
				info.AddValue("Log10Org", s._log10Org);
				info.AddValue("Log10End", s._log10End);

				info.AddValue("Rescaling", s._rescaling);

				info.AddValue("Bounds", s._dataBounds);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				Log10Scale s = null != o ? (Log10Scale)o : new Log10Scale();

				s._log10Org = (double)info.GetDouble("Log10Org");
				s._cachedOrg = Math.Pow(10, s._log10Org);

				s._log10End = (double)info.GetDouble("Log10End");
				s._cachedEnd = Math.Pow(10, s._log10End);
				s._rescaling = (LogarithmicAxisRescaleConditions)info.GetValue("Rescaling", s);
				s._rescaling.ParentObject = s;

				s._dataBounds = (PositiveFiniteNumericalBoundaries)info.GetValue("Bounds", s);
				s._dataBounds.ParentObject = s;

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization of the linear axis.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// restore the event chain
			_dataBounds.ParentObject = this;
		}

		#endregion Serialization

		/// <summary>
		/// Creates a default logarithmic axis with org=1 and end=10.
		/// </summary>
		public Log10Scale()
		{
			_rescaling = new LogarithmicAxisRescaleConditions() { ParentObject = this };
			_dataBounds = new PositiveFiniteNumericalBoundaries() { ParentObject = this };
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The axis to copy from.</param>
		public Log10Scale(Log10Scale from)
		{
			this._dataBounds = null == from._dataBounds ? new PositiveFiniteNumericalBoundaries() : (NumericalBoundaries)from._dataBounds.Clone();
			this._dataBounds.ParentObject = this;
			this._log10Org = from._log10Org;
			this._cachedOrg = from._cachedOrg;
			this._log10End = from._log10End;
			this._cachedEnd = from._cachedEnd;

			this._rescaling = null == from.Rescaling ? new LogarithmicAxisRescaleConditions() : (LogarithmicAxisRescaleConditions)from.Rescaling.Clone();
			this._rescaling.ParentObject = this;
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataBounds)
				yield return new Main.DocumentNodeAndName(_dataBounds, "DataBounds");

			if (null != _rescaling)
				yield return new Main.DocumentNodeAndName(_rescaling, "Rescaling");
		}

		/// <summary>
		/// Creates a clone copy of this axis.
		/// </summary>
		/// <returns>The cloned copy.</returns>
		public override object Clone()
		{
			return new Log10Scale(this);
		}

		/// <summary>
		/// Returns the rescaling conditions for this axis
		/// </summary>
		public override NumericAxisRescaleConditions Rescaling
		{
			get
			{
				return _rescaling;
			}
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
		public override double PhysicalToNormal(double x)
		{
			if (x <= 0)
				return Double.NaN;

			double log10x = Math.Log10(x);
			return (log10x - _log10Org) / (_log10End - _log10Org);
		}

		/// <summary>
		/// NormalToPhysical is the inverse function to PhysicalToNormal
		/// It translates a normalized value (0 for the axis origin, 1 for the axis end)
		/// into the physical value
		/// </summary>
		/// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
		/// <returns>the corresponding physical value</returns>
		public override double NormalToPhysical(double x)
		{
			double log10x = _log10Org + (_log10End - _log10Org) * x;
			return Math.Pow(10, log10x);
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

		public override NumericalBoundaries DataBounds
		{
			get { return this._dataBounds; }
		} // return a PhysicalBoundarie object that is associated with that axis

		public override double Org
		{
			get { return _cachedOrg; }
		}

		public override double End
		{
			get { return _cachedEnd; }
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

		private void HandleInvalidOrgOrEnd(ref double org, ref double end)
		{
			if (org > 0)
			{
				end = org * 10;
			}
			else if (end > 0)
			{
				org = end / 10;
			}
			else
			{
				org = 1;
				end = 10;
			}
		}

		private void InternalSetOrgEnd(double org, double end, bool isOrgExtendable, bool isEndExtendable)
		{
			double lgorg = Math.Log10(org);
			double lgend = Math.Log10(end);

			_cachedOrg = org;
			_cachedEnd = end;

			bool changed = _log10Org != lgorg ||
				_log10End != lgend ||
				_isOrgExtendable != isOrgExtendable ||
				_isEndExtendable != isEndExtendable;

			_log10Org = lgorg;
			_log10End = lgend;

			_isOrgExtendable = isOrgExtendable;
			_isEndExtendable = isEndExtendable;

			if (changed)
				EhSelfChanged(EventArgs.Empty);
		}

		public override void Rescale()
		{
			double xorg = double.NaN;
			double xend = double.NaN;

			if (null != _dataBounds && !_dataBounds.IsEmpty)
			{
				xorg = _dataBounds.LowerBound;
				xend = _dataBounds.UpperBound;
			}

			bool isAutoOrg, isAutoEnd;
			_rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);

			if (!(xorg > 0) || !(xend > 0))
				HandleInvalidOrgOrEnd(ref xorg, ref xend);

			InternalSetOrgEnd(xorg, xend, isAutoOrg, isAutoEnd);
		}

		public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
			double o = org.ToDouble();
			double e = end.ToDouble();

			if (!(o < e))
				return "org is not less than end";
			if (!(o > 0))
				return "org is not positive";
			if (!(e > 0))
				return "end is not positive";

			InternalSetOrgEnd(o, e, false, false);
			return null;
		}

		#region Changed event handling

		protected override void OnChanged(EventArgs e)
		{
			if (e is BoundariesChangedEventArgs)
			{
				Rescale(); // calculate new bounds and fire AxisChanged event
			}

			base.OnChanged(e);
		}

		#endregion Changed event handling
	} // end of class Log10Axis
}