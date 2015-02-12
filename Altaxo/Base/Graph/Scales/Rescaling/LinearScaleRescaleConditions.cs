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

	public class LinearScaleRescaleConditions : NumericAxisRescaleConditions
	{
		#region Serialization

		/// <summary>
		/// Initial version 2015-02-10.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearScaleRescaleConditions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (LinearScaleRescaleConditions)obj;

				info.AddBaseValueEmbedded(s, s.GetType().BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (LinearScaleRescaleConditions)o ?? new LinearScaleRescaleConditions();

				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		public LinearScaleRescaleConditions()
		{
			_dataBoundsOrg = _resultingOrg = 0;
			_dataBoundsEnd = _resultingEnd = 1;
		}

		public LinearScaleRescaleConditions(LinearScaleRescaleConditions from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override object Clone()
		{
			return new LinearScaleRescaleConditions(this);
		}

		public override void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, double orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, double endValue)
		{
			if (double.IsNaN(orgValue) || !Altaxo.Calc.RMath.IsFinite(orgValue))
				throw new ArgumentOutOfRangeException("orgValue should be a finite number but is " + orgValue.ToString());

			if (double.IsNaN(endValue) || !Altaxo.Calc.RMath.IsFinite(endValue))
				throw new ArgumentOutOfRangeException("endValue should be a finite number but is " + endValue.ToString());

			base.SetUserParameters(orgRescaling, orgRelativeTo, orgValue, endRescaling, endRelativeTo, endValue);
		}

		/// <summary>
		/// Fixes the data bounds org and end. Here we modify the bounds if org and end are equal.
		/// </summary>
		/// <param name="dataBoundsOrg">The data bounds org.</param>
		/// <param name="dataBoundsEnd">The data bounds end.</param>
		protected override void FixDataBoundsOrgAndEnd(ref double dataBoundsOrg, ref double dataBoundsEnd)
		{
			// ensure that data bounds always have some distance
			if (dataBoundsOrg == dataBoundsEnd)
			{
				if (0 == dataBoundsOrg)
				{
					dataBoundsOrg = -1;
					dataBoundsEnd = 1;
				}
				else
				{
					dataBoundsOrg = dataBoundsOrg - Math.Abs(dataBoundsOrg);
					dataBoundsEnd = dataBoundsEnd + Math.Abs(dataBoundsEnd);
				}
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

		/*
		protected override double GetResultingOrgFromDataBoundsOrg()
		{
			if (_orgRescaling == BoundaryRescaling.Auto)
				return _dataBoundsOrg; // TODO ERROR

			switch (_userProvidedOrgRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _dataBoundsOrg;

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
		*/

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

		/*
		protected double GetResultingEndFromDataBoundsEnd()
		{
			switch (_userProvidedEndRelativeTo)
			{
				case BoundariesRelativeTo.Absolute:
					return _dataBoundsEnd;

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
		*/

		#endregion Resulting Org/End to/fron User Org/End
	}
}