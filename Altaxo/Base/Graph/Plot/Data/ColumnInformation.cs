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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;

namespace Altaxo.Graph.Plot.Data
{
  /// <summary>
  /// Gui information about a required column, for instance for selection of rows.
  /// </summary>
  public struct ColumnInformationSimple
  {
    /// <summary>
    /// The label of this column for user convenience. An example would be 'X' to label an X-column.
    /// </summary>
    public string ColumnLabel; // Column label

    /// <summary>
    /// The column as it was at the time of this call. Can be null if the column currently could not be resolved.
    /// </summary>
    public IReadableColumn? Column;

    /// <summary>
    /// The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.
    /// Can be null if the user has no column attached currently.
    /// </summary>
    public string? ColumnName;

    /// <summary>
    /// Designates the action to set the column.
    /// First argument are the column, 2nd argument are the underlying table of the column, and 3rd argument is the group number of the column.
    /// </summary>
    public Action<IReadableColumn> SetColumnAction;

    /// <summary>
    /// Creates an instance of <see cref="ColumnInformationSimple"/>.
    /// </summary>
    /// <param name="columnLabel">The label of this column for user convenience.</param>
    /// <param name="column">The column.</param>
    /// <param name="columnName">The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.</param>
    /// <param name="setColumnAction">Designates the action to set the column. Argument is the value of the column that should be set.</param>
    public ColumnInformationSimple(string columnLabel, IReadableColumn? column, string? columnName, Action<IReadableColumn> setColumnAction)
    {
      ColumnLabel = columnLabel;
      Column = column;
      ColumnName = columnName;
      SetColumnAction = setColumnAction;
    }

    /// <summary>
    /// Deconstructs this instance into its parts.
    /// </summary>
    /// <param name="columnLabel">The label of this column for user convenience.</param>
    /// <param name="column">The column.</param>
    /// <param name="columnName">The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.</param>
    /// <param name="setColumnAction">Designates the action to set the column. Argument is the value of the column that should be set.</param>
    public void Deconstruct(out string columnLabel, out IReadableColumn? column, out string? columnName, out Action<IReadableColumn> setColumnAction)
    {
      columnLabel = ColumnLabel;
      column = Column;
      columnName = ColumnName;
      setColumnAction = SetColumnAction;
    }
  }



  /// <summary>
  /// Gui information about a required column, for instance for selection of columns for plot items.
  /// </summary>
  public struct ColumnInformation
  {
    /// <summary>
    /// The label of this column for user convenience. An example would be 'X' to label an X-column.
    /// </summary>
    public string ColumnLabel; // Column label

    /// <summary>
    /// The column as it was at the time of this call. Can be null if the column currently could not be resolved.
    /// </summary>
    public IReadableColumn? Column;

    /// <summary>
    /// The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.
    /// Can be null if the user has no column attached currently.
    /// </summary>
    public string? ColumnName;

    /// <summary>
    /// Designates the action to set the column.
    /// First argument are the column, 2nd argument are the underlying table of the column, and 3rd argument is the group number of the column.
    /// </summary>
    public Action<IReadableColumn?, DataTable, int> SetColumnAction;


    /// <summary>
    /// Creates an instance of <see cref="ColumnInformation"/>.
    /// </summary>
    /// <param name="columnLabel">The label of this column for user convenience.</param>
    /// <param name="column">The column.</param>
    /// <param name="columnName">The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.</param>
    /// <param name="setColumnAction">Designates the action to set the column. 1st argument is the value of the column that should be set. 2nd argument are the underlying table of the column, and 3rd argument is the group number of the column.</param>
    public ColumnInformation(string columnLabel, IReadableColumn? column, string? columnName, Action<IReadableColumn?, DataTable, int> setColumnAction)
    {
      ColumnLabel = columnLabel;
      Column = column;
      ColumnName = columnName;
      SetColumnAction = setColumnAction;
    }

    /// <summary>
    /// Deconstructs this instance into its parts.
    /// </summary>
    /// <param name="columnLabel">The label of this column for user convenience.</param>
    /// <param name="column">The column.</param>
    /// <param name="columnName">The name of the column (e.g. the column name in the underlying table), last part of the column proxies document path.</param>
    /// <param name="setColumnAction">Designates the action to set the column. Argument is the value of the column that should be set.</param>
    public void Deconstruct(out string columnLabel, out IReadableColumn? column, out string? columnName, out Action<IReadableColumn?, DataTable, int> setColumnAction)
    {
      columnLabel = ColumnLabel;
      column = Column;
      columnName = ColumnName;
      setColumnAction = SetColumnAction;
    }
  }

  /// <summary>
  /// Bundles a set of column informations of columns belonging to each other. For instance, in a 3D-Plot, the columns 'X', 'Y' and 'Z' belonging to each other.
  /// Other examples of column groups are the columns needed for a vector style, or for an error style.
  /// </summary>
  public struct GroupOfColumnsInformation
  {
    /// <summary>
    /// Name of the column group, e.g. 'X-Y-Data'.
    /// </summary>
    public string NameOfColumnGroup;

    /// <summary>
    /// The information about the columns in this column group.
    /// </summary>
    public IEnumerable<ColumnInformation> ColumnInfos;

    /// <summary>
    /// Creates an instance of <see cref="GroupOfColumnsInformation"/>.
    /// </summary>
    /// <param name="groupName">The Gui name of the column group, e.g. 'X-Y Data'.</param>
    /// <param name="columnInfos">The infos of the columns belonging to this group.</param>
    public GroupOfColumnsInformation(string groupName, IEnumerable<ColumnInformation> columnInfos)
    {
      NameOfColumnGroup = groupName;
      ColumnInfos = columnInfos;
    }

    /// <summary>
    /// Deconstruct this instance into its parts.
    /// </summary>
    /// <param name="groupName">The Gui name of the column group, e.g. 'X-Y Data'.</param>
    /// <param name="columnInfos">The infos of the columns belonging to this group.</param>
    public void Deconstruct(out string groupName, out IEnumerable<ColumnInformation> columnInfos)
    {
      groupName = NameOfColumnGroup;
      columnInfos = ColumnInfos;
    }
  }
}
