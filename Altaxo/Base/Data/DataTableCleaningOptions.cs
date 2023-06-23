#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using System.Collections.Generic;

namespace Altaxo.Data
{
  /// <summary>
  /// Options for clearing one or multiple data tables.
  /// </summary>
  public record DataTableCleaningOptions
  {
    /// <summary>If true, the main data of the table will be cleared.</summary>
    public bool ClearData { get; init; }

    /// <summary>If true, all data columns will be removed.</summary>
    public bool RemoveDataColumns { get; init; }

    /// <summary>If true, the column properties will be cleared.</summary>
    public bool ClearColumnProperties { get; init; }

    /// <summary>If true, all property columns will be removed.</summary>
    public bool RemoveColumnProperties { get; init; }

    /// <summary>If true, the table notes will be cleared.</summary>
    public bool ClearNotes { get; init; }

    /// <summary>If true, the table properties will be cleared.</summary>
    public bool ClearTableProperties { get; init; }

    /// <summary>If true, the table script is removed.</summary>
    public bool ClearTableScript { get; init; }

    /// <summary>If true, the table's data source is removed.</summary>
    public bool ClearDataSource { get; init; }

    /// <summary>
    /// Applies the options to the given tables.
    /// </summary>
    /// <param name="tables">The tables to apply the options to.</param>
    public void ApplyTo(IEnumerable<DataTable> tables)
    {
      using (var token = Current.Project.DataTableCollection.SuspendGetToken())
      {
        foreach (DataTable table in tables)
        {
          ApplyTo(table);
        }
      }
    }

    /// <summary>
    /// Applies the options to a given table.
    /// </summary>
    /// <param name="table">The table to apply the options to.</param>
    public void ApplyTo(DataTable table)
    {
      using (var token = table.SuspendGetToken())
      {
        if (RemoveDataColumns)
        {
          table.DataColumns.RemoveColumnsAll();
        }
        else if (ClearData)
        {
          table.DataColumns.ClearData();
        }

        if (RemoveColumnProperties)
        {
          table.PropCols.RemoveColumnsAll();
        }
        else if (ClearColumnProperties)
        {
          table.PropCols.ClearData();
        }

        if (ClearNotes)
        {
          table.Notes.Clear();
        }

        if (ClearTableProperties)
        {
          if (table.PropertyBag is { } pb)
          {
            pb.Clear();
          }
        }

        if (ClearTableScript)
        {
          table.TableScript = null;
        }

        if (ClearDataSource)
        {
          table.DataSource = null;
        }
      }
    }
  }
}
