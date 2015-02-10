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
	}
}