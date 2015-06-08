#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Collections.Generic;

namespace Altaxo.Graph.Scales
{
	/// <summary>
	/// Represents a scale, whose logical value (0..1) is inverse proportional to the physical value. Note that when both axis origin and end are positive, axis origin is greater than axis end.
	/// </summary>
	/// <remarks>
	/// The logical value l is calculated from the physical value x by the following formula:
	/// l = (1/x - 1/axisOrigin) / (1/axisEnd - 1/axisOrigin);
	/// </remarks>
	/// <example>
	/// If the axis origin is 10 and the axis end is 1, then x=10 is mapped to the logical value l=0 (axis origin), x=1 is mapped to the logical value l=1 (axis end), and x=11/20 is mapped to l=0.5.
	/// </example>
	public class ProbabilityScale : NumericalScale
	{
		static readonly double SquareRootOf2 = Math.Sqrt(2);

		/// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
		protected NumericalBoundaries _dataBounds;

		protected ProbabilityScaleRescaleConditions _rescaling;

		protected Ticks.TickSpacing _tickSpacing;

		// cached values
		/// <summary>Current axis origin (cached value).</summary>
		protected double _cachedAxisOrg = 0.0002;

		/// <summary>Current axis end (cached value).</summary>
		protected double _cachedAxisEnd = 1 - 0.0002;

		protected double _cachedAxisQuantileOrg;
		protected double _cachedAxisQuantileSpan;

		#region Serialization

		/// <summary>
		/// 2015-06-07 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProbabilityScale), 0)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ProbabilityScale)obj;
				info.AddValue("Org", s._cachedAxisOrg);
				info.AddValue("End", s._cachedAxisOrg);
				info.AddValue("Bounds", s._dataBounds);
				info.AddValue("Rescaling", s._rescaling);
				info.AddValue("TickSpacing", s._tickSpacing);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ProbabilityScale)o : new ProbabilityScale(info);

				s._cachedAxisOrg = (double)info.GetDouble("Org");
				s._cachedAxisEnd = (double)info.GetDouble("End");
				s._cachedAxisQuantileOrg = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * s._cachedAxisOrg);
				s._cachedAxisQuantileSpan = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * s._cachedAxisEnd) - s._cachedAxisOrg;

				s._dataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds", s);
				s._dataBounds.ParentObject = s; // restore the event chain

				s._rescaling = (ProbabilityScaleRescaleConditions)info.GetValue("Rescaling", s);
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
		protected ProbabilityScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		/// <summary>
		/// Creates a default linear axis with org=0 and end=1.
		/// </summary>
		public ProbabilityScale()
		{
			_dataBounds = new ProbabilityBoundaries() { ParentObject = this };
			_rescaling = new ProbabilityScaleRescaleConditions() { ParentObject = this };
			_tickSpacing = new Ticks.LinearTickSpacing() { ParentObject = this, EndGrace = 0, OrgGrace = 0, ZeroLever = 0 };
			UpdateTicksAndOrgEndUsingRescalingObject();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">A other linear axis from which to copy from.</param>
		public ProbabilityScale(ProbabilityScale from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as ProbabilityScale;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._cachedAxisOrg = from._cachedAxisOrg;
				this._cachedAxisEnd = from._cachedAxisEnd;
				this._cachedAxisQuantileOrg = from._cachedAxisQuantileOrg;
				this._cachedAxisQuantileSpan = from._cachedAxisQuantileSpan;

				ChildCopyToMemberOrCreateNew(ref _dataBounds, from._dataBounds, () => new FiniteNumericalBoundaries());
				ChildCopyToMemberOrCreateNew(ref _rescaling, from._rescaling, () => new ProbabilityScaleRescaleConditions());
				ChildCopyToMemberOrCreateNew(ref _tickSpacing, from._tickSpacing, () => new Ticks.LinearTickSpacing());

				EhSelfChanged(EventArgs.Empty);
				suspendToken.Resume();
			}

			return true;
		}

		public override object Clone()
		{
			return new ProbabilityScale(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataBounds)
				yield return new Main.DocumentNodeAndName(_dataBounds, () => _dataBounds = null, "DataBounds");
			if (null != _rescaling)
				yield return new Main.DocumentNodeAndName(_rescaling, () => _rescaling = null, "Rescaling");
			if (null != _tickSpacing)
				yield return new Main.DocumentNodeAndName(_tickSpacing, () => _tickSpacing = null, "TickSpacing");
		}

		/// <summary>
		/// Gets the axis origin (physical units).
		/// </summary>
		public override double Org
		{
			get
			{
				return _cachedAxisOrg;
			}
		}

		/// <summary>
		/// Get the axis end (physical units).
		/// </summary>
		public override double End
		{
			get
			{
				return _cachedAxisEnd;
			}
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

				if (ChildSetMember(ref _tickSpacing, (Ticks.NumericTickSpacing)value))
					EhChildChanged(Rescaling, EventArgs.Empty);
			}
		}

		public override void OnUserRescaled()
		{
			_rescaling.OnUserRescaled();
		}

		/// <summary>
		/// Get the internal DataBound object (mostly for merging).
		/// </summary>
		public override NumericalBoundaries DataBounds
		{
			get { return _dataBounds; }
		}

		/// <summary>
		/// Converts a value in axis (physical) units to a "normalized" value, which is 0 for the axis org and 1 for the axis end.
		/// </summary>
		/// <param name="x">Value to convert (physical units).</param>
		/// <returns>Normalized value.</returns>
		public override double PhysicalToNormal(double x)
		{
			// x is presumed to be a probability between 0 and 1 (exactly in the interval (0,1)

			if ((x > 0) && (x < 1))
			{
				double quantile = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * x);
				return (quantile - _cachedAxisQuantileOrg) / _cachedAxisQuantileSpan;
			}
			else
			{
				return double.NaN;
			}
		}

		public override double NormalToPhysical(double x)
		{
			double quantile = _cachedAxisQuantileOrg + x * _cachedAxisQuantileSpan;
			return 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(x / SquareRootOf2));
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

		/// <summary>
		/// Returns the default axis boundaries when no valid data are entered into.
		/// </summary>
		/// <param name="org"></param>
		/// <param name="end"></param>
		public void GetDefaultAxisBoundaries(out double org, out double end)
		{
			org = double.PositiveInfinity;
			end = 1;
		}

		protected override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
		{
			double o = org.ToDouble();
			double e = end.ToDouble();

			if (o <= 0)
				o = 0.01;
			else if (o >= 1)
				o = 0.99;

			if (e <= 0)
				e = 0.01;
			else if (e >= 1)
				e = 0.99;

			if (o == e)
			{
				double h = o / 2;
				o = o - h;
				e = e + h;
			}

			InternalSetOrgEnd(o, e);

			return null;
		}

		private void InternalSetOrgEnd(double org, double end)
		{
			if (org > end)
			{
				var h = org;
				org = end;
				end = h;
			}

			bool changed =
				_cachedAxisOrg != org ||
				_cachedAxisEnd != end
			;

			_cachedAxisQuantileOrg = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * org);
			_cachedAxisQuantileSpan = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * end) - _cachedAxisQuantileOrg;

			if (changed)
				EhSelfChanged(EventArgs.Empty);
		}
	}
}