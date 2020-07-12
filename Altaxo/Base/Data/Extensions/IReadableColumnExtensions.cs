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

#nullable enable
using System.Collections.Generic;

namespace Altaxo.Data
{
  public static class IReadableColumnExtensions
  {
    /// <summary>
    /// Try to get a common data table and a group number from all columns (here we don't rely on <see cref="DataTable"/> and GroupNumber of this document).
    /// </summary>
    /// <param name="columns">The columns to consider. <see cref="ITransformedReadableColumn"/> will be stripped to get the underlying data column.</param>
    /// <param name="dataTableIsNotUniform">If the columns gives different result for their underlying data table, the result here will be true.</param>
    /// <param name="commonDataTable">If the previous parameter results in false, this is the common data table of all columns. If the previous parameter is true, this is the first underlying data table that could be deduced from the columns. The result is null if no underlying table could be deduced from the columns.</param>
    /// <param name="groupNumberIsNotUniform">If the columns gives different result for their group number, the result here will be true.</param>
    /// <param name="commonGroupNumber">If the previous parameter results in false, this is the common group number of all columns. If the previous parameter results in true, this is the first group number that could be deduced from the columns. The result is null if no group number could be deduced from the columns.</param>
    public static void GetCommonDataTableAndGroupNumberFromColumns(this IEnumerable<IReadableColumn> columns, out bool dataTableIsNotUniform, out DataTable? commonDataTable, out bool groupNumberIsNotUniform, out int? commonGroupNumber)
    {
      dataTableIsNotUniform = false;
      groupNumberIsNotUniform = false;
      commonDataTable = null;
      commonGroupNumber = null;

      foreach (var col in columns)
      {
        IReadableColumn underlyingColumn = col;

        while (underlyingColumn is ITransformedReadableColumn ucTRC)
        {
          underlyingColumn = ucTRC.UnderlyingReadableColumn;
        }

        if (underlyingColumn is DataColumn ucDC)
        {
          var colColl = DataColumnCollection.GetParentDataColumnCollectionOf(ucDC);
          var dataTable = DataTable.GetParentDataTableOf(colColl);
          int? groupNumber = colColl?.GetColumnGroup(ucDC);

          if (null != dataTable)
          {
            if (null == commonDataTable)
              commonDataTable = dataTable;
            else if (!object.ReferenceEquals(commonDataTable, dataTable))
              dataTableIsNotUniform = true;
          }

          if (null != groupNumber)
          {
            if (null == commonGroupNumber)
              commonGroupNumber = groupNumber;
            else if (!(commonGroupNumber == groupNumber))
              groupNumberIsNotUniform = true;
          }
        }
      }
    }
  }
}
