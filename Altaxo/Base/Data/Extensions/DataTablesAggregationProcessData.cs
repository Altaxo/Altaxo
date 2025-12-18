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
using Altaxo.Data.Selections;
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Holds configuration and state for aggregating data from multiple data tables.
  /// </summary>
  public class DataTablesAggregationProcessData : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
    /// <summary>
    /// Gets the list of data table proxies that are used as sources for aggregation.
    /// </summary>
    public List<DataTableProxy> DataTables = new List<DataTableProxy>();

    /// <summary>
    /// Gets or sets the row selection.
    /// </summary>
    /// <exception cref="Markdig.Helpers.ThrowHelper.ArgumentNullException(System.String)">RowSelection</exception>
    IRowSelection RowSelection
    {
      get => field;
      set
      {
        ArgumentNullException.ThrowIfNull(value, nameof(RowSelection));
        field = value;

      }
    } = new AllRows();

    /// <summary>
    /// Gets or sets filters that are used as name patterns to select tables by their name.
    /// </summary>
    /// <remarks>
    /// The alternative possibility is to use a name filter to select tables by their name.
    /// </remarks>
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
    /// Gets or sets a value indicating whether all tables that match the filter are added to the list of tables before execution of the data source.
    /// </summary>
    public bool AddFilterMatchedTablesBeforeExecution { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether all tables that do not match the filter are removed from the list of tables before execution of the data source.
    /// </summary>
    public bool RemoveFilterUnmatchedTablesBeforeExecution { get; set; } = true;



    #region Serialization

    #region Version 0

    /// <summary>
    /// 2025-12-16 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationProcessData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTablesAggregationProcessData)obj;

        info.AddArray("DataTables", s.DataTables, s.DataTables.Count);
        info.AddValue("RowSelection", s.RowSelection);
        info.AddArray("DataTableNameFilter", s.DataTableNameFilter, s.DataTableNameFilter.Count);
        info.AddValue("AddFilterdMatchedTablesBeforeExecution", s.AddFilterMatchedTablesBeforeExecution);
        info.AddValue("RemoveFilterUnmatchedTablesBeforeExecution", s.RemoveFilterUnmatchedTablesBeforeExecution);
      }



      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableProxies = info.GetArrayOfValues<DataTableProxy>("DataTables", parent);
        var rowSelection = info.GetValue<IRowSelection>("RowSelection", parent);
        var filter = info.GetArrayOfStrings("DataTableNameFilter");
        bool addMatched = info.GetBoolean("AddFilterdMatchedTablesBeforeExecution");
        bool removeUnmatched = info.GetBoolean("RemoveFilterUnmatchedTablesBeforeExecution");
        return new DataTablesAggregationProcessData(tableProxies, rowSelection, filter.ToImmutableList(), addMatched, removeUnmatched);
      }
    }

    #endregion Version 0

    #endregion Serialization



    /// <summary>
    /// Initializes a new instance of the <see cref="DataTablesAggregationProcessData"/> class.
    /// </summary>
    /// <param name="tables">The source table proxies. They are used as is, i.e. without cloning, thus make sure to clone them before if they are used elsewhere.</param>
    /// <param name="rowSelection">The row selection to use for all tables. It is used as is, i.e. without cloning, thus make sure to clone it before if it is used elsewhere.</param>
    /// <param name="dataTableNameFilter">The table name filters used to select tables for aggregation.</param>
    /// <param name="addMatched">If set to <c>true</c>, tables matching the filter are added before execution of the data source.</param>
    /// <param name="removeUnmatched">If set to <c>true</c>, tables not matching the filter are removed before execution of the data source.</param>
    public DataTablesAggregationProcessData(IEnumerable<DataTableProxy> tables, IRowSelection rowSelection, ImmutableList<string> dataTableNameFilter, bool addMatched, bool removeUnmatched)
    {
      DataTables = tables.ToList();
      RowSelection = rowSelection;
      DataTableNameFilter = dataTableNameFilter;
      AddFilterMatchedTablesBeforeExecution = addMatched;
      RemoveFilterUnmatchedTablesBeforeExecution = removeUnmatched;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DataTablesAggregationProcessData(
        this.DataTables.Select(tp => { var c = (DataTableProxy)tp.Clone(); c.ParentObject = this; return c; }),
        (IRowSelection)RowSelection.Clone(),
        this.DataTableNameFilter,
        AddFilterMatchedTablesBeforeExecution,
        RemoveFilterUnmatchedTablesBeforeExecution
        );
    }

    /// <summary>
    /// Gets the source tables for the extraction.
    /// </summary>
    public IReadOnlyList<DataTableProxy> TableProxies => DataTables;

    /// <summary>
    /// Determines whether the specified path matches the current table name filter.
    /// </summary>
    /// <param name="path">The table path or name to test.</param>
    /// <returns><c>true</c> if the path matches at least one filter pattern; otherwise, <c>false</c>.</returns>
    public bool IsTableNameMatching(string path)
    {
      return IsTableNameMatching(DataTableNameFilter, path);
    }

    /// <summary>
    /// Determines whether the specified path matches at least one of the provided filter patterns.
    /// </summary>
    /// <param name="filters">The collection of filter patterns.</param>
    /// <param name="path">The table path or name to test.</param>
    /// <returns><c>true</c> if the path matches at least one pattern; otherwise, <c>false</c>.</returns>
    public static bool IsTableNameMatching(IEnumerable<string> filters, string path)
    {
      return filters.Any(pattern => MatchRecursive(pattern, 0, path, 0));
    }

    /// <summary>
    /// Matches a table path against a single filter pattern using recursive wildcard matching.
    /// </summary>
    /// <param name="pattern">The filter pattern that may contain '*', '**', or '?' wildcards.</param>
    /// <param name="pi">The current index in the pattern.</param>
    /// <param name="path">The table path or name to match.</param>
    /// <param name="si">The current index in the path.</param>
    /// <returns><c>true</c> if the pattern matches the path; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Synchronizes the internal table proxies with the current project tables using the filters and returns the resulting source tables.
    /// </summary>
    /// <param name="destinationTable">The destination table, or <c>null</c> to use the global project tables.</param>
    /// <returns>The collection of source tables referenced by the updated proxies.</returns>
    public IEnumerable<DataTable> UpdateTableProxiesAndGetSourceTables(DataTable? destinationTable)
    {
      var allTables = destinationTable?.ParentObject as DataTableCollection ?? Current.Project.DataTableCollection;
      var filteredTables = new HashSet<DataTable>();
      if (RemoveFilterUnmatchedTablesBeforeExecution || AddFilterMatchedTablesBeforeExecution)
      {
        filteredTables = allTables.Where(t => IsTableNameMatching(DataTableNameFilter, t.Name)).ToHashSet();
      }

      if (RemoveFilterUnmatchedTablesBeforeExecution && DataTableNameFilter.Count > 0)
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



    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (DataTables is { } tables)
      {
        for (int i = tables.Count - 1; i >= 0; i--)
          yield return new DocumentNodeAndName(DataTables[i], $"Table[{i}]");
      }
    }

    /// <summary>
    /// Visits all document references so that proxies can be reported or updated.
    /// </summary>
    /// <param name="reportProxies">The callback used to report each proxy instance.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter reportProxies)
    {
      for (int i = DataTables.Count - 1; i >= 0; i--)
        reportProxies(DataTables[i], this, $"Tables[{i}]");
    }
  }
}
