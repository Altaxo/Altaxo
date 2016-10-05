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

using Altaxo.Data;
using Altaxo.Data.Selections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// Common interface to column plot data as <see cref="XYColumnPlotData"/> or <see cref="XYZColumnPlotData"/>.
	/// </summary>
	/// <seealso cref="System.ICloneable" />
	public interface IColumnPlotData : ICloneable
	{
		/// <summary>Gets the underlying data table. If this property is null (happens with old deserialization), the underlying table must be determined from the data columns.</summary>
		DataTable DataTable { get; set; }

		/// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
		int GroupNumber { get; set; }

		/// <summary>
		/// The selection of data rows to be plotted.
		/// </summary>
		IRowSelection DataRowSelection { get; set; }

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		IEnumerable<Tuple<
	string, // Column label
	IReadableColumn, // the column as it was at the time of this call
	string, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn> // action to set the column during Apply of the controller
	>> GetAdditionallyUsedColumns();
	}
}