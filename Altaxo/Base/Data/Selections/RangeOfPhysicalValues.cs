#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Data.Selections
{
	public class RangeOfPhysicalValues : IRowSelection
	{
		private AltaxoVariant _lowerValue;
		private bool _isLowerInclusive;

		private AltaxoVariant _upperValue;
		private bool _isUpperInclusive;

		private NumericColumnProxy _columnProxy;

		#region Serialization

		/// <summary>
		/// 2016-09-25 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RangeOfPhysicalValues), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (RangeOfPhysicalValues)obj;

				info.AddValue("LowerValue", (object)s._lowerValue);
				info.AddValue("LowerIsInclusive", s._isLowerInclusive);

				info.AddValue("UpperValue", (object)s._upperValue);
				info.AddValue("UpperIsInclusive", s._isUpperInclusive);

				info.AddValue("Column", s._columnProxy);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var lower = (AltaxoVariant)info.GetValue("LowerValue", parent);
				var isLowerInclusive = info.GetBoolean("LowerIsInclusive");

				var upper = (AltaxoVariant)info.GetValue("UpperValue", parent);
				var isUpperInclusive = info.GetBoolean("UppperIsInclusive");

				var columnProxy = (NumericColumnProxy)info.GetValue("Column", parent);

				return new RangeOfPhysicalValues(info, lower, isLowerInclusive, upper, isUpperInclusive, columnProxy);
			}
		}

		#endregion Serialization

		public RangeOfPhysicalValues()
		{
			_lowerValue = 0;
			_isLowerInclusive = true;
			_upperValue = 1;
			_isUpperInclusive = true;
			_columnProxy = null;
		}

		/// <summary>
		/// Deserialization constructor. Initializes a new instance of the <see cref="RangeOfPhysicalValues"/> class.
		/// </summary>
		/// <param name="info">The deserialization information.</param>
		/// <param name="lower">The lower.</param>
		/// <param name="isLowerInclusive">if set to <c>true</c> [is lower inclusive].</param>
		/// <param name="upper">The upper.</param>
		/// <param name="isUpperInclusive">if set to <c>true</c> [is upper inclusive].</param>
		/// <param name="column">The column.</param>
		protected RangeOfPhysicalValues(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, AltaxoVariant lower, bool isLowerInclusive, AltaxoVariant upper, bool isUpperInclusive, NumericColumnProxy column)
		{
			_lowerValue = lower;
			_isLowerInclusive = isLowerInclusive;
			_upperValue = upper;
			_isUpperInclusive = isUpperInclusive;
			_columnProxy = column;
		}

		public RangeOfPhysicalValues(AltaxoVariant lower, bool isLowerInclusive, AltaxoVariant upper, bool isUpperInclusive, INumericColumn column)
		{
			_lowerValue = lower;
			_isLowerInclusive = isLowerInclusive;
			_upperValue = upper;
			_isUpperInclusive = isUpperInclusive;
			_columnProxy = NumericColumnProxy.FromColumn(column);
		}

		/// <inheritdoc/>
		public IEnumerable<int> GetSelectedRowIndicesFromTo(int startIndex, int maxIndex)
		{
			var column = _columnProxy?.Document;

			if (null == column)
				yield break;

			var maxIdx = column.Count.HasValue ? Math.Min(column.Count.Value, maxIndex) : maxIndex;

			for (int i = startIndex; i < maxIdx; ++i)
			{
				if (column.IsElementEmpty(i))
					continue;

				var x = column[i];

				if (_isLowerInclusive)
				{
					if (!(x >= _lowerValue))
						continue;
				}
				else
				{
					if (!(x > _lowerValue))
						continue;
				}
				if (_isUpperInclusive)
				{
					if (!(x <= _upperValue))
						continue;
				}
				else
				{
					if (!(x < _upperValue))
						continue;
				}

				yield return i;
			}
		}
	}
}