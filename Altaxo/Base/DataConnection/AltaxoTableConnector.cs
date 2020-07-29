#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
  public class AltaxoTableConnector
  {
    private Altaxo.Data.DataTable _table;

    public AltaxoTableConnector(Altaxo.Data.DataTable table)
    {
      _table = table;
    }

    public void ReadAction(System.Data.Common.DbDataReader reader)
    {
      var columnMapping = new List<AltaxoColumnMapping>();
      CreateColumns(reader, columnMapping);
      FillAltaxoTable(reader, columnMapping);
    }

    private void FillAltaxoTable(System.Data.Common.DbDataReader reader, List<AltaxoColumnMapping> columnMapping)
    {
      int j = -1;
      while (reader.Read())
      {
        ++j;

        for (int i = 0; i < reader.FieldCount; ++i)
        {
          var mapping = columnMapping[i];
          var axoColumn = mapping.AltaxoColumn;
          mapping.AltaxoColumnItemSetter(axoColumn, j, reader[i]);
        }
      }
    }

    private struct AltaxoColumnMapping
    {
      public Altaxo.Data.DataColumn AltaxoColumn;

      /// <summary>Sets an item in an Altaxo column. 1st arg is the Altaxo data column, 2nd arg is the index in this column, and 3rd arg is the object to set.
      /// The action should set the item at index to the object.</summary>
      public Action<Altaxo.Data.DataColumn, int, object> AltaxoColumnItemSetter;
    }

    private void CreateColumns(System.Data.Common.DbDataReader reader, List<AltaxoColumnMapping> mapping)
    {
      mapping.Clear();

      var schemaTable = reader.GetSchemaTable();
      int currentPosition = -1;

      foreach (System.Data.DataRow row in schemaTable.Rows)
      {
        ++currentPosition;

        var ct = (Type)row["DataType"];

        var axoColType = GetAltaxoColumnType(ct);
        var axoColName = (string)row["ColumnName"];

        var axoColumn = _table.DataColumns.EnsureExistence(axoColName, axoColType, Data.ColumnKind.V, 0);

        mapping.Add(new AltaxoColumnMapping() { AltaxoColumn = axoColumn, AltaxoColumnItemSetter = GetColumnItemSetter(ct) });
      }

      // clear all mapped columns
      foreach (var item in mapping)
        item.AltaxoColumn.Clear();
    }

    public static Type GetAltaxoColumnType(Type oledbType)
    {
      if (OleDbSchema.IsNumeric(oledbType))
        return typeof(Altaxo.Data.DoubleColumn);
      else if (OleDbSchema.IsDateTime(oledbType))
        return typeof(Altaxo.Data.DateTimeColumn);
      else
        return typeof(Altaxo.Data.TextColumn);
    }

    public static Action<Altaxo.Data.DataColumn, int, object> GetColumnItemSetter(Type oledbType)
    {
      return (col, idx, obj) =>
        {
          if (null == obj || obj == System.DBNull.Value)
            col.SetElementEmpty(idx);
          else
            col[idx] = new Data.AltaxoVariant(obj);
        };
    }
  }
}
