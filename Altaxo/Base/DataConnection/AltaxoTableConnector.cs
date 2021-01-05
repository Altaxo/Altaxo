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
      public Altaxo.Data.DataColumn AltaxoColumn { get; }

      /// <summary>Sets an item in an Altaxo column. 1st arg is the Altaxo data column, 2nd arg is the index in this column, and 3rd arg is the object to set.
      /// The action should set the item at index to the object.</summary>
      public Action<Altaxo.Data.DataColumn, int, object> AltaxoColumnItemSetter { get; }

      public AltaxoColumnMapping(Altaxo.Data.DataColumn altaxoColumn, Action<Altaxo.Data.DataColumn, int, object> altaxoColumnItemSetter)
      {
        AltaxoColumn = altaxoColumn;
        AltaxoColumnItemSetter = altaxoColumnItemSetter;
      }
    }

    private void CreateColumns(System.Data.Common.DbDataReader reader, List<AltaxoColumnMapping> mapping)
    {
      mapping.Clear();

      var schemaTable = reader.GetSchemaTable();
      int currentPosition = -1;

      if (schemaTable is not null)
      {
        foreach (System.Data.DataRow row in schemaTable.Rows)
        {
          ++currentPosition;

          var ct = (Type)row["DataType"];

          var axoColType = GetAltaxoColumnType(ct);
          var axoColName = (string)row["ColumnName"];

          Altaxo.Data.DataColumn axoColumn;
          if (ct == typeof(object)) // Type is not known to OleDb, could be for instance a DateTimeOffset type
          {
            if (_table.DataColumns.Contains(axoColName)) // if there exist already a column with that name, we assume that the user has chosen the right column type - we use that colum
            {
              axoColumn = _table.DataColumns[axoColName];
            }
            else // otherwise, we use a text column
            {
              axoColumn = _table.DataColumns.EnsureExistence(axoColName, axoColType, Data.ColumnKind.V, 0); 
            }
          }
          else
          {
            axoColumn = _table.DataColumns.EnsureExistence(axoColName, axoColType, Data.ColumnKind.V, 0);
          }

          mapping.Add(new AltaxoColumnMapping(axoColumn, GetColumnItemSetter(ct, axoColumn.GetType())));
        }
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

    public static Action<Altaxo.Data.DataColumn, int, object> GetColumnItemSetter(Type oledbType, Type destinationColumnType)
    {
      return (col, idx, obj) =>
        {
          if (obj is null || obj == System.DBNull.Value)
            col.SetElementEmpty(idx);
          else
          {
            if (oledbType == typeof(object) && destinationColumnType == typeof(Altaxo.Data.DateTimeColumn))
            {
              var destCol = (Altaxo.Data.DateTimeColumn)col;
              if (obj is DateTime dt)
                destCol[idx] = dt;
              else if (obj is string sobj && DateTime.TryParse(sobj, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt1))
                destCol[idx] = dt1;
            }
            else
            {
              col[idx] = new Data.AltaxoVariant(obj);
            }
          }
        };
    }
  }
}
