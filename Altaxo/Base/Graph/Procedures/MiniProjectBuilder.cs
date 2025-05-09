﻿#region Copyright

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Main;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Given a single GraphDocumentBase, this class retrieves all the objects that are neccessary for this particular GraphDocumentBase (tables, functions etc.)
  /// and builds a new <see cref="Altaxo.AltaxoDocument"/> that contains these objects.
  /// </summary>
  public class MiniProjectBuilder
  {
    private GraphDocumentBase _graph;

    /// <summary>
    /// The _document to build.
    /// </summary>
    private AltaxoDocument _document;

    private Dictionary<AbsoluteDocumentPath, DataTable?> _tablesToChange;
    private Dictionary<DataColumn, DataTable?> _columnsToChange;

    protected MiniProjectBuilder(GraphDocumentBase graph, bool ensureEmbeddedObjectRenderingOptionsStoredInGraph)
    {
      Initialize();

      _graph = graph;
      CollectAllDataColumnReferences();
      CopyReferencedColumnsToNewProject();
      CopyGraphToNewDocument(_graph, ensureEmbeddedObjectRenderingOptionsStoredInGraph);
      CopyFolderPropertiesOf(_graph);
      CopyDocumentInformation(_graph);
    }

    /// <summary>
    /// Gets a graph document as mini project. The mini project is an <see cref="AltaxoDocument"/> consisting of the provided graph document <see cref="AltaxoDocument"/> and all data that is neccessary
    /// to plot that graph.
    /// </summary>
    /// <param name="graph">The existing graph (this graph is cloned before added to the mini project).</param>
    /// <param name="ensureEmbeddedObjectRenderingOptionsStoredInGraph">If set to <c>true</c>, the current embedded rendering options are stored as property in the graph document of the mini project.
    /// This ensures that later on the graph is rendered in the client document exactly as it was chosen to be in the current project. If the mini project is not used for COM, leave that flag to <c>false.</c></param>
    /// <returns>The mini project containing the cloned graph and all related data.</returns>
    public static AltaxoDocument CreateMiniProject(GraphDocumentBase graph, bool ensureEmbeddedObjectRenderingOptionsStoredInGraph)
    {
      var pb = new MiniProjectBuilder(graph, ensureEmbeddedObjectRenderingOptionsStoredInGraph);
      return pb._document;
    }

    [MemberNotNull(nameof(_document), nameof(_tablesToChange), nameof(_columnsToChange))]
    protected void Initialize()
    {
      _document = new AltaxoDocument();
      _tablesToChange = new Dictionary<AbsoluteDocumentPath, DataTable?>();
      _columnsToChange = new Dictionary<DataColumn, DataTable?>();
    }

    protected void CopyGraphToNewDocument(GraphDocumentBase oldGraph, bool ensureEmbeddedObjectRenderingOptionsStoredInGraph)
    {
      var newGraph = (GraphDocumentBase)oldGraph.Clone();
      newGraph.Name = oldGraph.Name;
      _document.AddItem(newGraph);

      if (ensureEmbeddedObjectRenderingOptionsStoredInGraph)
      {
        var clipboardRenderingOptions = Altaxo.PropertyExtensions.GetPropertyValue(oldGraph, ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, () => new ClipboardRenderingOptions());
        var embeddedRenderingOptions = Altaxo.PropertyExtensions.GetPropertyValue(oldGraph, EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, null);

        // if embeddedRenderingOptions exists explicitely, they should be used. Else the clipboard options can be used.
        var clonedOptions = embeddedRenderingOptions is not null ? embeddedRenderingOptions.Clone() : new EmbeddedObjectRenderingOptions(clipboardRenderingOptions);
        newGraph.PropertyBagNotNull.SetValue(EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, clonedOptions);
      }
    }

    protected void CopyFolderPropertiesOf(GraphDocumentBase oldGraph)
    {
      foreach (var doc in PropertyExtensions.GetProjectFolderPropertyDocuments(oldGraph))
      {
        if (doc.PropertyBag is not null && doc.PropertyBag.Count > 0)
        {
          var bagclone = doc.Clone();
          _document.ProjectFolderProperties.Add(bagclone);
        }
      }
    }

    private void CopyDocumentInformation(GraphDocumentBase graph)
    {
      var sourceDocument = (AltaxoDocument?)Main.AbsoluteDocumentPath.GetRootNodeImplementing(graph, typeof(AltaxoDocument));

      if (sourceDocument is not null)
      {
        _document.DocumentIdentifier = sourceDocument.DocumentIdentifier;
      }
    }

    protected void CollectAllDataColumnReferences()
    {
      _graph.VisitDocumentReferences(CollectDataColumnsFromProxyVisit);
    }

    protected void CopyReferencedColumnsToNewProject()
    {
      while (_columnsToChange.Count > 0)
      {
        var entry = _columnsToChange.First();
        var table = entry.Value;
        var columnList = new HashSet<DataColumn>(_columnsToChange.Where(x => object.ReferenceEquals(x.Value, table)).Select(x => x.Key));

        foreach (var col in columnList)
          _columnsToChange.Remove(col);
        if (table is not null)
        {
          BuildNewTableWithColumns(table, columnList);
        }
      }
    }

    private void BuildNewTableWithColumns(DataTable oldTable, HashSet<DataColumn> columnList)
    {
      var newToOldDataColIndex = new Dictionary<int, int>();

      var oldDataCollection = oldTable.DataColumns;
      var newTable = new DataTable();
      for (int i = 0; i < oldTable.DataColumnCount; ++i)
      {
        if (columnList.Contains(oldTable.DataColumns[i]))
        {
          newToOldDataColIndex.Add(newTable.DataColumnCount, i);
          newTable.DataColumns.Add((DataColumn)(oldDataCollection[i].Clone()), oldDataCollection.GetColumnName(i), oldDataCollection.GetColumnKind(i), oldDataCollection.GetColumnGroup(i));
        }
      }
      // now the property colums
      var oldPropColleciton = oldTable.PropCols;

      for (int i = 0; i < oldPropColleciton.ColumnCount; ++i)
      {
        var oldPropCol = oldPropColleciton[i];
        var newPropCol = (DataColumn)oldPropCol.Clone();
        newPropCol.Clear();
        newTable.PropCols.Add(newPropCol, oldPropColleciton.GetColumnName(i), oldPropColleciton.GetColumnKind(i), oldPropColleciton.GetColumnGroup(i));

        for (int k = 0; k < newTable.DataColumns.ColumnCount; ++k)
          newPropCol[k] = oldPropCol[newToOldDataColIndex[k]];
      }

      newTable.Notes.Text = oldTable.Notes.Text;
      newTable.Name = oldTable.Name;

      _document.DataTableCollection.Add(newTable);
    }

    /// <summary>Collects a underlying data table from a proxy.</summary>
    /// <param name="proxy">The proxy.</param>
    /// <param name="owner">The owner of the proxy.</param>
    /// <param name="propertyName">Name of the property in the owner class that will return the proxy.</param>
    private void CollectDataColumnsFromProxyVisit(IProxy proxy, object owner, string propertyName)
    {
      if (proxy is null || proxy.IsEmpty)
      {
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumn dataColumn)
      {
        if (!_columnsToChange.ContainsKey(dataColumn))
        {
          var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumn);
          _columnsToChange.Add(dataColumn, table);
        }
      }
      else if (proxy.DocumentObject() is Altaxo.Data.DataColumnCollection dataColumnCollection)
      {
        var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumnCollection);
        if (table is not null)
        {
          var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
          if (!_tablesToChange.ContainsKey(tablePath))
            _tablesToChange.Add(tablePath, null);
        }
      }
      else if (proxy.DocumentObject() is DataTable table)
      {
        var tablePath = AbsoluteDocumentPath.GetAbsolutePath(table);
        if (!_tablesToChange.ContainsKey(tablePath))
          _tablesToChange.Add(tablePath, null);
      }
      else if ((proxy is Altaxo.Data.INumericColumnProxy) || (proxy is Altaxo.Data.IReadableColumnProxy))
      {
        // seems to be an indexer column or something alike. No need to do something
      }
    }
  }
}
