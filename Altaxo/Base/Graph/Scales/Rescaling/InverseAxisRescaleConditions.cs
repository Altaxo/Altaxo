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

namespace Altaxo.Graph.Scales.Rescaling
{
	/// <summary>
	/// Summary description for LogarithmicAxisRescaleConditions.
	/// </summary>
	[Serializable]
	public class InverseAxisRescaleConditions : NumericAxisRescaleConditions
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InverseAxisRescaleConditions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (InverseAxisRescaleConditions)obj;

				info.AddBaseValueEmbedded(s, s.GetType().BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (InverseAxisRescaleConditions)o : new InverseAxisRescaleConditions();

				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

				return s;
			}
		}

		#endregion Serialization

		public InverseAxisRescaleConditions()
		{
		}

		public InverseAxisRescaleConditions(InverseAxisRescaleConditions from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override object Clone()
		{
			return new InverseAxisRescaleConditions(this);
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
					var offs = 0.5 * Math.Abs(dataBoundsOrg);
					dataBoundsOrg = dataBoundsOrg - offs;
					dataBoundsEnd = dataBoundsEnd + offs;
				}
			}
		}

		protected override double GetDataBoundsScaleMean()
		{
			return 0.5 * _dataBoundsOrg * _dataBoundsEnd / (_dataBoundsOrg + _dataBoundsEnd);
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
	}
}