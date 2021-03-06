﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for Altaxo.Data.DataTableCollection.
  /// </summary>
  public class DataTableCollection
    :
    Altaxo.Main.ProjectItemCollectionBase<DataTable>
  {
    public DataTableCollection(AltaxoDocument parent)
      : base(parent)
    {
    }

    public override string ItemBaseName { get { return "WKS"; } }

    public string[] GetSortedTableNames()
    {
      string[] arr = new string[_itemsByName.Count];
      _itemsByName.Keys.CopyTo(arr, 0);
      return arr;
    }

    /// <summary>
    /// Ensures the existence of a DataTable with the given name. Returns the table with the given name if it exists,
    /// otherwise a table with that name will be created and returned.
    /// </summary>
    /// <param name="tableName">Table name.</param>
    /// <returns>The data table with the provided name.</returns>
    public DataTable EnsureExistence(string tableName)
    {
      if (Contains(tableName))
      {
        return this[tableName];
      }
      else
      {
        var newTable = new DataTable(tableName);
        Add(newTable);
        return newTable;
      }
    }

    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (_itemsByName.TryGetValue(name, out var result))
        return result;

      return null;
    }

    public override string? GetNameOfChildObject(Main.IDocumentLeafNode o)
    {
      if (o is DataTable table)
      {
        if (_itemsByName.TryGetValue(table.Name, out var item))
        {
          if(object.ReferenceEquals(o, item))
            return table.Name;
          else
            throw new InvalidProgramException($"Names out of sync: the entry with key {table.Name} contains a table with the name {item.Name}");
        }

        // just make sure that the item is out of sync with the name
        foreach (var entry in _itemsByName)
        {
          if (object.ReferenceEquals(entry.Value, o))
          {
            throw new InvalidProgramException($"Names out of sync: in collection the name is {entry.Key}, but in item the name is {table.Name}");
          }
        }
      }

      return null;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _itemsByName)
        yield return new Main.DocumentNodeAndName(entry.Value, entry.Key);
    }

    /// <summary>
    /// Gets the parent DataTableCollection of a child table, a child ColumnCollection, or a child column.
    /// </summary>
    /// <param name="child">Can be a DataTable, a DataColumnCollection, or a DataColumn for which the parent table collection is searched.</param>
    /// <returns>The parent DataTableCollection, if it exists, or null otherwise.</returns>
    public static Altaxo.Data.DataTableCollection? GetParentDataTableCollectionOf(Main.IDocumentLeafNode child)
    {
      return (DataTableCollection?)Main.AbsoluteDocumentPath.GetRootNodeImplementing(child, typeof(DataTableCollection));
    }
  }
}
