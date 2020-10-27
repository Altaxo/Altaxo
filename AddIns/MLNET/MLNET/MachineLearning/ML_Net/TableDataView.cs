#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Microsoft.ML;

namespace Altaxo.MachineLearning.ML_Net
{
  /// <summary>
  /// Wraps all or a set of columns of a <see cref="Altaxo.Data.DataTable"/> in an object that implements <see cref="IDataView"/>,
  /// so that the data in that table can be used as input data of the data pipeline;
  /// </summary>
  /// <seealso cref="Microsoft.ML.IDataView" />
  public partial class TableDataView : IDataView
  {
    Altaxo.Data.DataTable _table;
    int _rowCount;

    // Operational variables
    DataViewSchema _schema;

    #region Construction

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDataView"/> class.
    /// </summary>
    /// <param name="table">The data table containing the data columns.</param>
    /// <param name="selectedColumns">The indices of the columns of the <paramref name="table"/> that should be part of the <see cref="IDataView"/>.</param>
    public TableDataView(Altaxo.Data.DataTable table, Altaxo.Collections.AscendingIntegerCollection selectedColumns, DataViewCreationOptions dataViewCreationOptions = DataViewCreationOptions.None)
    {
      _table = table ?? throw new ArgumentNullException(nameof(table));
      _schema = BuildDataViewSchema(selectedColumns ?? throw new ArgumentNullException(nameof(selectedColumns)), dataViewCreationOptions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDataView"/> class.
    /// </summary>
    /// <param name="table">The data table.</param>
    /// <param name="groupNumber">The group number of the columns of the <paramref name="table"/> that should be part of the <see cref="IDataView"/>.</param>
    /// <exception cref="ArgumentNullException">table</exception>
    public TableDataView(Altaxo.Data.DataTable table, int groupNumber, DataViewCreationOptions dataViewCreationOptions = DataViewCreationOptions.None)
    {
      _table = table ?? throw new ArgumentNullException(nameof(table));
      var selectedColumns = _table.DataColumns.Columns.Where(c => _table.DataColumns.GetColumnGroup(c) == groupNumber).Select(c1 => _table.DataColumns.GetColumnNumber(c1));
      _schema = BuildDataViewSchema(selectedColumns, dataViewCreationOptions);
    }

    #endregion

    public bool CanShuffle => true;

    public DataViewSchema Schema => _schema;

    public long? GetRowCount()
    {
      return _table.DataRowCount;
    }

    public DataViewRowCursor GetRowCursor(IEnumerable<DataViewSchema.Column> columnsNeeded, Random? rand = null)
    {
      return new RowCursor(this, columnsNeeded, rand);
    }

    public DataViewRowCursor[] GetRowCursorSet(IEnumerable<DataViewSchema.Column> columnsNeeded, int n, Random? rand = null)
    {
      return new DataViewRowCursor[] { GetRowCursor(columnsNeeded, rand) };
    }


    #region Internal

    protected DataViewSchema BuildDataViewSchema(IEnumerable<int> selectedColumns, DataViewCreationOptions dataViewCreationOptions)
    {
      var builder = new DataViewSchema.Builder();

      int rowCount = int.MaxValue;

      foreach (var colIndex in selectedColumns)
      {
        var column = _table.DataColumns[colIndex];
        rowCount = Math.Min(rowCount, column.Count);

        Microsoft.ML.Data.DataViewType colType;

        switch (column)
        {
          case Altaxo.Data.DoubleColumn _:
            if (dataViewCreationOptions.HasFlag(DataViewCreationOptions.ConvertDoubleToSingle))
            {
              colType = Microsoft.ML.Data.NumberDataViewType.Single;
            }
            else
            {
              colType = Microsoft.ML.Data.NumberDataViewType.Double;
            }
            break;
          case Altaxo.Data.BooleanColumn _:
            colType = Microsoft.ML.Data.BooleanDataViewType.Instance;
            break;
          case Altaxo.Data.DateTimeColumn _:
            colType = Microsoft.ML.Data.DateTimeDataViewType.Instance;
            break;
          case Altaxo.Data.TextColumn _:
            colType = Microsoft.ML.Data.TextDataViewType.Instance;
            break;
          default:
            throw new NotImplementedException();
        }

        builder.AddColumn(_table.DataColumns.GetColumnName(column), colType);
      }

      _rowCount = rowCount;

      return builder.ToSchema();
    }

    #endregion
  }
}
