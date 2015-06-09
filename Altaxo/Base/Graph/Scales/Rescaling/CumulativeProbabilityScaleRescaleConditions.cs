#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Rescaling
{
	using Altaxo.Graph.Scales.Boundaries;

	public class CumulativeProbabilityScaleRescaleConditions : NumericScaleRescaleConditions
	{
		public const double DefaultOrgValue = 0.0002;
		public const double DefaultEndValue = 0.9998;

		#region Serialization

		/// <summary>
		/// Initial version 2015-02-10.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CumulativeProbabilityScaleRescaleConditions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CumulativeProbabilityScaleRescaleConditions)obj;

				info.AddBaseValueEmbedded(s, s.GetType().BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (CumulativeProbabilityScaleRescaleConditions)o ?? new CumulativeProbabilityScaleRescaleConditions();

				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		public CumulativeProbabilityScaleRescaleConditions()
		{
			_dataBoundsOrg = _resultingOrg = DefaultOrgValue;
			_dataBoundsEnd = _resultingEnd = DefaultEndValue;
		}

		public CumulativeProbabilityScaleRescaleConditions(CumulativeProbabilityScaleRescaleConditions from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override object Clone()
		{
			return new CumulativeProbabilityScaleRescaleConditions(this);
		}

		public override void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, double orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, double endValue)
		{
			if (orgRelativeTo != BoundariesRelativeTo.Absolute)
				throw new ArgumentException(string.Format("{0} does only support orgRelativeTo={1]", this.GetType().Name, Enum.GetName(typeof(BoundariesRelativeTo), BoundariesRelativeTo.Absolute)));
			if (endRelativeTo != BoundariesRelativeTo.Absolute)
				throw new ArgumentException(string.Format("{0} does only support endRelativeTo={1]", this.GetType().Name, Enum.GetName(typeof(BoundariesRelativeTo), BoundariesRelativeTo.Absolute)));

			if (double.IsNaN(orgValue) || !(0 < orgValue && orgValue < 1))
			{
				if (orgRescaling == BoundaryRescaling.Auto)
					orgValue = DefaultOrgValue;  // ignore this error and set org to 0
				else
					throw new ArgumentOutOfRangeException("orgValue should be a number 0 < orgValue < 1, but is " + orgValue.ToString());
			}

			if (double.IsNaN(endValue) || !(0 < endValue && endValue < 1))
			{
				if (endRescaling == BoundaryRescaling.Auto)
					endValue = DefaultEndValue;  // ignore this error and set end to 1
				else
					throw new ArgumentOutOfRangeException("endValue should be number 0 < endValue < 1, but is " + endValue.ToString());
			}

			base.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, endRescaling, endRelativeTo, endValue);
		}

		/// <summary>
		/// Fixes the data bounds org and end. Here we modify the bounds if org and end are equal.
		/// </summary>
		/// <param name="dataBoundsOrg">The data bounds org.</param>
		/// <param name="dataBoundsEnd">The data bounds end.</param>
		protected override void FixValuesForDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd)
		{
			if (dataBoundsOrg <= 0)
				dataBoundsOrg = DefaultOrgValue;
			else if (dataBoundsOrg >= 1)
				dataBoundsOrg = DefaultEndValue;

			if (dataBoundsEnd <= 0)
				dataBoundsEnd = DefaultOrgValue;
			else if (dataBoundsEnd >= 1)
				dataBoundsEnd = DefaultEndValue;

			if (dataBoundsOrg == dataBoundsEnd)
			{
				double h = dataBoundsOrg / 2;
				dataBoundsOrg -= h;
				dataBoundsEnd += h;
			}
		}

		protected override double GetDataBoundsScaleMean()
		{
			return 0.5 * (_dataBoundsOrg + _dataBoundsEnd);
		}

		#region Resulting Org/End to/fron User Org/End

		protected override double GetResultingOrgFromUserProvidedOrg()
		{
			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _userProvidedOrgValue;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedOrgValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedOrgValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedOrgValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected override double GetUserProvidedOrgFromResultingOrg(double resultingOrg)
		{
			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return resultingOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return resultingOrg - _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return resultingOrg - _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return resultingOrg - GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected override double GetResultingEndFromUserProvidedEnd()
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _userProvidedEndValue;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return _userProvidedEndValue + _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return _userProvidedEndValue + _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return _userProvidedEndValue + GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		protected override double GetUserProvidedEndFromResultingEnd(double resultingEnd)
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return resultingEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsOrg:
					return resultingEnd - _dataBoundsOrg;

				case BoundariesRelativeTo.RelativeToDataBoundsEnd:
					return resultingEnd - _dataBoundsEnd;

				case BoundariesRelativeTo.RelativeToDataBoundsMean:
					return resultingEnd - GetDataBoundsScaleMean();

				default:
					throw new NotImplementedException();
			}
		}

		#endregion Resulting Org/End to/fron User Org/End

		protected override void FixValuesForUserZoomed(ref double zoomOrg, ref double zoomEnd)
		{
			if (zoomOrg > zoomEnd)
			{
				var h = zoomOrg;
				zoomOrg = zoomEnd;
				zoomEnd = h;
			}

			if (zoomOrg == zoomEnd)
			{
				if (null == zoomOrg)
				{
					zoomOrg = -1;
					zoomEnd = 1;
				}
				else
				{
					zoomOrg = zoomOrg - 0.5 * Math.Abs(zoomOrg);
					zoomEnd = zoomEnd + 0.5 * Math.Abs(zoomOrg);
				}
			}
		}

		#region Helper functions for dialog

		public double GetOrgValueToShowInDialog(double currentResultingOrg)
		{
			if (this._orgRescaling == BoundaryRescaling.Auto)
			{
				switch (this._userProvidedOrgRelativeTo)
				{
					case BoundariesRelativeTo.Absolute:
						return currentResultingOrg;

					case BoundariesRelativeTo.RelativeToDataBoundsOrg:
						return currentResultingOrg - _dataBoundsOrg;

					case BoundariesRelativeTo.RelativeToDataBoundsEnd:
						return currentResultingOrg - _dataBoundsEnd;

					case BoundariesRelativeTo.RelativeToDataBoundsMean:
						return currentResultingOrg - 0.5 * (_dataBoundsOrg + _dataBoundsEnd);

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				return this._userProvidedOrgValue;
			}
		}

		public double GetEndValueToShowInDialog(double currentResultingEnd)
		{
			if (this._endRescaling == BoundaryRescaling.Auto)
			{
				switch (this._userProvidedEndRelativeTo)
				{
					case BoundariesRelativeTo.Absolute:
						return currentResultingEnd;

					case BoundariesRelativeTo.RelativeToDataBoundsOrg:
						return currentResultingEnd - _dataBoundsOrg;

					case BoundariesRelativeTo.RelativeToDataBoundsEnd:
						return currentResultingEnd - _dataBoundsEnd;

					case BoundariesRelativeTo.RelativeToDataBoundsMean:
						return currentResultingEnd - 0.5 * (_dataBoundsOrg + _dataBoundsEnd);

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				return this._userProvidedEndValue;
			}
		}

		#endregion Helper functions for dialog
	}
}