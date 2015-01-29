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
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Wrapper struct for a row of a data column collection.
	/// </summary>
	public struct DataRow : IEnumerable<AltaxoVariant>
	{
		private DataColumnCollection _col;
		private int _rowIdx;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="col">The underlying <see cref="DataColumnCollection"/>.</param>
		/// <param name="rowIndex">Index of the row of the data column collection.</param>
		public DataRow(DataColumnCollection col, int rowIndex)
		{
			_col = col;
			_rowIdx = rowIndex;
		}

		/// <summary>
		/// Gets the index of the wrapped row.
		/// </summary>
		public int RowIndex
		{
			get { return _rowIdx; }
		}

		/// <summary>
		/// Gets the underlying data column collection.
		/// </summary>
		public DataColumnCollection ColumnCollection
		{
			get { return _col; }
		}

		/// <summary>
		/// Accesses one field of the row.
		/// </summary>
		/// <param name="i">Column index.</param>
		/// <returns>Element at column[i] and the wrapped row index.</returns>
		public AltaxoVariant this[int i]
		{
			get { return _col[i][_rowIdx]; }
			set { _col[i][_rowIdx] = value; }
		}

		/// <summary>
		/// Accesses one field of the row.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>Element at column[name] and the wrapped row index.</returns>
		public AltaxoVariant this[string name]
		{
			get { return _col[name][_rowIdx]; }
			set { _col[name][_rowIdx] = value; }
		}

		#region IEnumerable<AltaxoVariant> Members

		/// <summary>
		/// Gets the enumeratur that iterates through all elements of this row.
		/// </summary>
		/// <returns>Enumeratur.</returns>
		public IEnumerator<AltaxoVariant> GetEnumerator()
		{
			for (int i = 0; i < _col.ColumnCount; i++)
				yield return _col[i][_rowIdx];
		}

		#endregion IEnumerable<AltaxoVariant> Members

		#region IEnumerable Members

		/// <summary>
		/// Gets the enumeratur that iterates through all elements of this row.
		/// </summary>
		/// <returns>Enumeratur.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _col.ColumnCount; i++)
				yield return _col[i][_rowIdx];
		}

		#endregion IEnumerable Members
	}
}