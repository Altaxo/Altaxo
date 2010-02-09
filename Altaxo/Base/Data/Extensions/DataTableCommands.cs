using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
  public static class DataTableCommands
  {
    public static void AddStandardColumns(this DataTable table)
    {
      table.DataColumns.Add(new DoubleColumn(), "A", ColumnKind.X, 0);
      table.DataColumns.Add(new DoubleColumn(), "B", ColumnKind.V, 0);
    }

		/// <summary>
		/// Copies the structure of the source table into the destination table.
		/// All existing property and data columns will be destroyed.
		/// </summary>
		/// <param name="destinationTable"></param>
		/// <param name="srcTable"></param>
		public static void CopyTableStructureFrom(this DataTable destinationTable, DataTable srcTable)
		{
		}
  }
}
