using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	/// <summary>
	/// Interface to be used if a controller controls an instance which refers to one or more data column(s).
	/// </summary>
	public interface IColumnDataExternallyControlled
	{
		/// <summary>
		/// Gets the data columns that the controller's document is referring to.
		/// </summary>
		/// <returns>Enumeration of tuples.
		/// Item1 is a label to be shown in the column data dialog to let the user identify the column.
		/// Item2 is the column itself,
		/// Item3 is the column name (last part of the full path to the column), and
		/// Item4 is an action which sets the column (and by the way the supposed data table the column belongs to.</returns>
		IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>> GetDataColumnsExternallyControlled();
	}
}