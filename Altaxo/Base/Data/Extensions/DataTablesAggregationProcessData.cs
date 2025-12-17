#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Main;

namespace Altaxo.Data
{
  public class DataTablesAggregationProcessData : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
    public List<DataTableProxy> DataTables = new List<DataTableProxy>();

    /// <summary>
    /// The alternative possibility is to use a name filter to select tables by their name.
    /// </summary>
    public ImmutableList<string> DataTableNameFilter
    {
      get => field;
      set
      {
        ArgumentNullException.ThrowIfNull(value, nameof(DataTableNameFilter));
        field = value;
      }
    } = ImmutableList<string>.Empty;


    /// <summary>
    /// If true, all tables that match the filter matches are added to the list of tables before execution.
    /// </summary>
    public bool AddFilterMatchedTablesBeforeExecution { get; set; } = true;

    /// <summary>
    /// If true, all tables that do not match the filter are removed from the list of tables before execution.
    public bool RemoveFilterUnmatchedTablesBeforeExecution { get; set; } = true;



    #region Serialization

    #region Version 0

    /// <summary>
    /// 2025-12-16 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationProcessData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTablesAggregationProcessData)obj;

        info.AddArray("DataTables", s.DataTables, s.DataTables.Count);
        info.AddArray("DataTableNameFilter", s.DataTableNameFilter, s.DataTableNameFilter.Count);
        info.AddValue("AddFilterdMatchedTablesBeforeExecution", s.AddFilterMatchedTablesBeforeExecution);
        info.AddValue("RemoveFilterUnmatchedTablesBeforeExecution", s.RemoveFilterUnmatchedTablesBeforeExecution);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableProxies = info.GetArrayOfValues<DataTableProxy>("DataTables", parent);
        var filter = info.GetArrayOfStrings("DataTableNameFilter");
        bool addMatched = info.GetBoolean("AddFilterdMatchedTablesBeforeExecution");
        bool removeUnmatched = info.GetBoolean("RemoveFilterUnmatchedTablesBeforeExecution");
        return new DataTablesAggregationProcessData(tableProxies, filter.ToImmutableList(), addMatched, removeUnmatched);
      }
    }

    #endregion Version 0

    #endregion Serialization



    public DataTablesAggregationProcessData(IEnumerable<DataTableProxy> tables, ImmutableList<string> dataTableNameFilter, bool addMatched, bool removeUnmatched)
    {
      DataTables = new();
      foreach (var entry in tables)
      {
        var tt = (DataTableProxy)entry.Clone();
        tt.ParentObject = this;
        DataTables.Add(tt);
      }
      DataTableNameFilter = dataTableNameFilter;
      AddFilterMatchedTablesBeforeExecution = addMatched;
      RemoveFilterUnmatchedTablesBeforeExecution = removeUnmatched;
    }

    public object Clone()
    {
      return new DataTablesAggregationProcessData(this.DataTables, this.DataTableNameFilter, AddFilterMatchedTablesBeforeExecution, RemoveFilterUnmatchedTablesBeforeExecution);
    }

    /// <summary>
    /// Gets the source tables for the extraction.
    /// </summary>
    public IReadOnlyList<DataTableProxy> TableProxies => DataTables;

    public bool IsTableNameMatching(string path)
    {
      return IsTableNameMatching(DataTableNameFilter, path);
    }

    public static bool IsTableNameMatching(IEnumerable<string> filters, string path)
    {
      return filters.Any(pattern => MatchRecursive(pattern, 0, path, 0));
    }

    private static bool MatchRecursive(string pattern, int pi, string path, int si)
    {
      while (pi < pattern.Length)
      {
        // Handle "**"
        if (pi + 1 < pattern.Length && pattern[pi] == '*' && pattern[pi + 1] == '*')
        {
          // Skip the "**"
          pi += 2;

          // Try to match zero or more characters (including separators)
          for (int skip = si; skip <= path.Length; skip++)
          {
            if (MatchRecursive(pattern, pi, path, skip))
              return true;
          }
          return false;
        }

        // Handle "*"
        if (pattern[pi] == '*')
        {
          pi++;
          // Try to match zero or more non-separator characters
          for (int skip = si; skip <= path.Length; skip++)
          {
            if (skip < path.Length && (path[skip] == '/' || path[skip] == '\\'))
              break; // stop at separator
            if (MatchRecursive(pattern, pi, path, skip))
              return true;
          }
          return false;
        }

        // Handle "?"
        if (pattern[pi] == '?')
        {
          if (si >= path.Length) return false;
          pi++;
          si++;
          continue;
        }

        // Handle literal character
        if (si >= path.Length || pattern[pi] != path[si])
          return false;

        pi++;
        si++;
      }

      // End of pattern: must also be end of path
      return si == path.Length;
    }

    public IEnumerable<DataTable> UpdateTableProxiesAndGetSourceTables(DataTable? destinationTable)
    {
      var allTables = destinationTable?.ParentObject as DataTableCollection ?? Current.Project.DataTableCollection;
      var filteredTables = new HashSet<DataTable>();
      if (RemoveFilterUnmatchedTablesBeforeExecution || AddFilterMatchedTablesBeforeExecution)
      {
        filteredTables = allTables.Where(t => IsTableNameMatching(DataTableNameFilter, t.Name)).ToHashSet();
      }

      if (RemoveFilterUnmatchedTablesBeforeExecution)
      {
        DataTables.RemoveAll(p => p.Document is null || !filteredTables.Contains(p.Document));
      }

      if (AddFilterMatchedTablesBeforeExecution)
      {
        var existingTables = DataTables.Select(p => p.Document).Where(d => d is not null).ToHashSet();
        foreach (var table in filteredTables)
        {
          if (!existingTables.Contains(table))
          {
            var proxy = new DataTableProxy(table) { ParentObject = this };
            DataTables.Add(proxy);
          }
        }
      }
      return DataTables.Select(p => p.Document!).Where(d => d is not null);
    }



    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (DataTables is { } tables)
      {
        for (int i = tables.Count - 1; i >= 0; i--)
          yield return new DocumentNodeAndName(DataTables[i], $"Table[{i}]");
      }
    }

    public void VisitDocumentReferences(DocNodeProxyReporter reportProxies)
    {
      for (int i = DataTables.Count - 1; i >= 0; i--)
        reportProxies(DataTables[i], this, $"Tables[{i}]");
    }
  }
}
