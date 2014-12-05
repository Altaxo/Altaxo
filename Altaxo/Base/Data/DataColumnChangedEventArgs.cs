#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
	/// <summary>
	/// Stores the accumulated change data of a column.
	/// </summary>
	public class DataColumnChangedEventArgs : System.EventArgs
	{
		/// <summary>Lower bound of the area of rows, which changed during the data change event off period.</summary>
		protected int _minRowChanged;

		/// <summary>Upper bound (plus one) of the area of rows, which changed during the data change event off period. This in in the (plus one) convention,
		/// i.e. the value of this member is the maximum row number that changed plus one.</summary>
		protected int _maxRowChanged;

		/// <summary>Indicates, if the row count decreased during the data change event off period. In this case it is neccessary
		/// to recalculate the row count of the table, since it is possible that the table row count also decreased in this case.</summary>
		protected bool _hasRowCountDecreased; // true if during event switch of period, the row m_Count  of this column decreases

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minRow">Lower bound of the area of rows, which changed-</param>
		/// <param name="maxRow">Upper bound (plus one) of the area of rows, which changed.</param>
		/// <param name="rowCountDecreased">Indicates, if the row count decreased during the data change.</param>
		public DataColumnChangedEventArgs(int minRow, int maxRow, bool rowCountDecreased)
		{
			_minRowChanged = minRow;
			_maxRowChanged = maxRow;
			_hasRowCountDecreased = rowCountDecreased;
		}

		/// <summary>
		/// Accumulates further data changes of a column into a already created object.
		/// </summary>
		/// <param name="minRow">Lower bound of the area of rows, which changed-</param>
		/// <param name="maxRow">Upper bound (plus one) of the area of rows, which changed.</param>
		/// <param name="rowCountDecreased">Indicates, if the row count decreased during the data change.</param>
		public void Accumulate(int minRow, int maxRow, bool rowCountDecreased)
		{
			if (minRow < _minRowChanged)
				_minRowChanged = minRow;
			if (maxRow > _maxRowChanged)
				_maxRowChanged = maxRow;

			_hasRowCountDecreased |= rowCountDecreased;
		}

		/// <summary>Lower bound of the area of rows, which changed during the data change event off period.</summary>
		public int MinRowChanged
		{
			get { return _minRowChanged; }
		}

		/// <summary>Upper bound (plus one) of the area of rows, which changed during the data change event off period. This in in the (plus one) convention,
		/// i.e. the value of this member is the maximum row number that changed plus one.</summary>
		public int MaxRowChanged
		{
			get { return _maxRowChanged; }
		}

		/// <summary>Indicates, if the row count decreased during the data change event off period. In this case it is neccessary
		/// to recalculate the row count of the table, since it is possible that the table row count also decreased in this case.</summary>
		public bool HasRowCountDecreased
		{
			get { return _hasRowCountDecreased; }
		}
	}
}