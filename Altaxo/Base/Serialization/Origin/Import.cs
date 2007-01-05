#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.Origin
{
  /// <summary>
  /// Static methods for importing Origin OPJ files.
  /// </summary>
  public static class Importer
  {
    /// <summary>
    /// Imports a Origin OPJ file (tables only) into corresponding new tables in Altaxo.
    /// </summary>
    /// <param name="filename">The file name of the origin OPJ file.</param>
    /// <returns>Null if the import was successfull, or a error message.</returns>
    public static string Import(string filename)
    {
      OpjFile opj = new OpjFile(filename);
      opj.Parse();

      // now create corresponding tables in Altaxo

      for (int nspread = 0; nspread < opj.numSpreads(); nspread++)
      {
        // Create a new table
        string tablename = Current.Project.DataTableCollection.FindNewTableName(opj.spreadName(nspread));
        DataTable table = new DataTable(tablename);

        int numberOfColumns = opj.numCols(nspread);
        for (int ncol = 0; ncol < numberOfColumns; ncol++)
        {
          string colname = opj.colName(nspread, ncol);
          string coltype = opj.colType(nspread, ncol);
          int numberOfRows = opj.numRows(nspread, ncol);
          ColumnKind kind = coltype == "X" ? ColumnKind.X : ColumnKind.V;

          DoubleColumn column = new DoubleColumn(numberOfRows);
          column.CopyDataFrom(opj.Data(nspread, ncol), numberOfRows);


          colname = table.DataColumns.FindUniqueColumnName(colname);
          table.DataColumns.Add(column, colname, kind, 0);
        }


        table.Name = tablename;
        Current.Project.DataTableCollection.Add(table);
        Current.ProjectService.CreateNewWorksheet(table);
      }
      return null;
    }
  }
}
