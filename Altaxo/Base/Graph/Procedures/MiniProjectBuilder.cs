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

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Procedures
{
	/// <summary>
	/// Given a single GraphDocument, this class retrieves all the objects that are neccessary for this particular GraphDocument (tables, functions etc.)
	/// and builds a new <see cref="Altaxo.AltaxoDocument"/> that contains these objects.
	/// </summary>
	public class MiniProjectBuilder
	{
		private GraphDocument _graph;

		/// <summary>
		/// The _document to build.
		/// </summary>
		private AltaxoDocument _document;

		private Dictionary<DocumentPath, DataTable> _tablesToChange;
		private Dictionary<DataColumn, DataTable> _columnsToChange;

		public MiniProjectBuilder()
		{
		}

		/// <summary>
		/// Gets a graph document as mini project. The mini project is an <see cref="AltaxoDocument"/> consisting of the provided graph document <see cref="AltaxoDocument"/> and all data that is neccessary
		/// to plot that graph.
		/// </summary>
		/// <param name="graph">The existing graph (this graph is cloned before added to the mini project).</param>
		/// <param name="ensureEmbeddedObjectRenderingOptionsStoredInGraph">If set to <c>true</c>, the current embedded rendering options are stored as property in the graph document of the mini project.
		/// This ensures that later on the graph is rendered in the client document exactly as it was chosen to be in the current project. If the mini project is not used for COM, leave that flag to <c>false.</c></param>
		/// <returns>The mini project containing the cloned graph and all related data.</returns>
		public AltaxoDocument GetMiniProject(GraphDocument graph, bool ensureEmbeddedObjectRenderingOptionsStoredInGraph)
		{
			Initialize();

			_graph = graph;
			CollectAllDataColumnReferences();
			CopyReferencedColumnsToNewProject();
			CopyGraphToNewDocument(_graph, ensureEmbeddedObjectRenderingOptionsStoredInGraph);
			CopyFolderPropertiesOf(_graph);
			CopyDocumentInformation(_graph);

			return _document;
		}

		protected void Initialize()
		{
			_document = new AltaxoDocument();
			_tablesToChange = new Dictionary<DocumentPath, DataTable>();
			_columnsToChange = new Dictionary<DataColumn, DataTable>();
		}

		protected void CopyGraphToNewDocument(GraphDocument oldGraph, bool ensureEmbeddedObjectRenderingOptionsStoredInGraph)
		{
			var newGraph = (GraphDocument)oldGraph.Clone();
			newGraph.Name = oldGraph.Name;
			_document.GraphDocumentCollection.Add(newGraph);

			if (ensureEmbeddedObjectRenderingOptionsStoredInGraph)
			{
				var clipboardRenderingOptions = Altaxo.PropertyExtensions.GetPropertyValue(oldGraph, ClipboardRenderingOptions.PropertyKeyClipboardRenderingOptions, () => new ClipboardRenderingOptions());
				var embeddedRenderingOptions = Altaxo.PropertyExtensions.GetPropertyValue(oldGraph, EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, () => null);

				// if embeddedRenderingOptions exists explicitely, they should be used. Else the clipboard options can be used.
				var clonedOptions = null != embeddedRenderingOptions ? embeddedRenderingOptions.Clone() : new EmbeddedObjectRenderingOptions(clipboardRenderingOptions);
				newGraph.PropertyBagNotNull.SetValue(EmbeddedObjectRenderingOptions.PropertyKeyEmbeddedObjectRenderingOptions, clonedOptions);
			}
		}

		protected void CopyFolderPropertiesOf(GraphDocument oldGraph)
		{
			foreach (var doc in PropertyExtensions.GetProjectFolderPropertyDocuments(oldGraph))
			{
				if (doc.PropertyBag != null && doc.PropertyBag.Count > 0)
				{
					var bagclone = doc.Clone();
					_document.ProjectFolderProperties.Add(bagclone);
				}
			}
		}

		private void CopyDocumentInformation(GraphDocument graph)
		{
			var sourceDocument = (AltaxoDocument)Main.DocumentPath.GetRootNodeImplementing(graph, typeof(AltaxoDocument));

			if (null != sourceDocument)
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

				BuildNewTableWithColumns(table, columnList);
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
		private void CollectDataColumnsFromProxyVisit(DocNodeProxy proxy, object owner, string propertyName)
		{
			if (proxy.IsEmpty)
			{
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var dataColumn = (DataColumn)proxy.DocumentObject;
				if (!_columnsToChange.ContainsKey(dataColumn))
				{
					var table = Altaxo.Data.DataTable.GetParentDataTableOf(dataColumn);
					_columnsToChange.Add(dataColumn, table);
				}
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumnCollection)proxy.DocumentObject);
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}
			else if (proxy.DocumentObject is DataTable)
			{
				var table = proxy.DocumentObject as DataTable;
				if (table != null)
				{
					var tablePath = DocumentPath.GetAbsolutePath(table);
					if (!_tablesToChange.ContainsKey(tablePath))
						_tablesToChange.Add(tablePath, null);
				}
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				// seems to be an indexer column or something alike. No need to do something
			}
		}
	}
}