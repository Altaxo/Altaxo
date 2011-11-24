#region Copyright
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Data;

namespace Altaxo.Main.Commands
{
	/// <summary>
	/// Commands on project items like DataTables, Graphs and so.
	/// </summary>
	public static class ProjectItemCommands
	{
		#region Clipboard commands

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// There are two groups of possible options:
		/// <para>Group1: where are the items included?</para>
		/// <para>Possibilities: (i) with absolute names, i.e. the item name remains as is. (ii) relative to the project folder where it is included and where it was copied from.</para>
		/// <para>Group2: what should be happen with the references (for instance in plot items to the table columns)</para>
		/// <para>Possibilities: (i) nothing, they remain as is (ii) they will be relocated taking the source folder and destination folder into account, or (iii) same as (ii) but also
		/// taking the renaming of the copied tables into account, if there is a name conflict with an already existing worksheet for instance.</para>
		/// <para>The first group is controlled by the <c>baseFolder</c> parameters during creation of the object (source base folder) and during pasting of the items (destination base folder).
		/// If source base folder or destination base folder is null, the items will be included with absolute names. This is usually the case when copying or pasting from/into the AllGraph, AllWorksheet or
		/// AllProject items view. If both source base folder and destination base folder have a value, then the items are included relative to both folders.</para>
		/// <para>
		/// </para>
		/// </remarks>
		class ProjectItemClipboardList
		{
			/// <summary>Folder from which the items are copied.</summary>
			public string BaseFolder { get; set; }


			/// <summary>If true, references will be relocated in the same way as the project items will be relocated.</summary>
			/// <value><c>true</c> if references should be relocated, <c>false</c> otherwise</value>
			public bool RelocateReferences { get; set; }

			/// <summary>
			/// When true, at serialization the internal references are tried to keep internal, i.e. if for instance a table have to be renamed, the plot items in the deserialized graphs
			/// will be relocated to the renamed table.
			/// </summary>
			public bool TryToKeepInternalReferences { get; set; }

			/// <summary>List of project items to serialize/deserialize</summary>
			IList<object> _projectItems;



			private ProjectItemClipboardList()
			{
			}

			public ProjectItemClipboardList(IList<object> projectItems, string baseFolder)
			{
				BaseFolder = baseFolder;
				_projectItems = projectItems;
			}

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectItemClipboardList), 0)]
			class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					ProjectItemClipboardList s = (ProjectItemClipboardList)obj;

					info.AddValue("BaseFolder", s.BaseFolder);
					info.AddValue("TryToKeepInternalReferences", s.TryToKeepInternalReferences);
					info.CreateArray("Items", s._projectItems.Count);
					foreach (var item in s._projectItems)
						info.AddValue("e", item);
					info.CommitArray();
				}

				public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ProjectItemClipboardList s = null != o ? (ProjectItemClipboardList)o : new ProjectItemClipboardList();

					s.BaseFolder = info.GetString("BaseFolder");
					s.TryToKeepInternalReferences = info.GetBoolean("TryToKeepInternalReferences");

					int count = info.OpenArray("Items");
					s._projectItems = new List<object>();
					for (int i = 0; i < count; ++i)
					{
						s._projectItems.Add(info.GetValue("e"));
					}
					info.CloseArray(count);


					return s;
				}
			}

			public IList<object> ProjectItems
			{
				get { return _projectItems; }
			}

		}

		/// <summary>
		/// Identifier used for putting a list of project items on the clipboard.
		/// </summary>
		public const string ClipboardFormat_ListOfProjectItems = "Altaxo.Main.ProjectItems.AsXml";

		/// <summary>Copies the items to the clipboard. In principle, this can be all items that have Altaxo's XML serialization support. But the corresponding clipboard paste operation
		/// is only supported for main project items (DataTables, Graphs).</summary>
		/// <param name="items">The items.</param>
		public static void CopyItemsToClipboard(List<object> items, string baseFolder)
		{
			Altaxo.Serialization.ClipboardSerialization.PutObjectToClipboard(ClipboardFormat_ListOfProjectItems, new ProjectItemClipboardList(items, baseFolder));
		}


		public static bool CanPasteItemsFromClipboard()
		{
			return Altaxo.Serialization.ClipboardSerialization.IsClipboardFormatAvailable(ClipboardFormat_ListOfProjectItems);
		}

		public static void PasteItemsFromClipboard(string baseFolder)
		{
			var list = Altaxo.Serialization.ClipboardSerialization.GetObjectFromClipboard<ProjectItemClipboardList>(ClipboardFormat_ListOfProjectItems);

			// Dictionary wich has as key the type of project item and as value a dictionary with the old names and with the relocated names.
			var renameDictionary = new Dictionary<System.Type, Dictionary<string, string>>();
			renameDictionary.Add(typeof(Altaxo.Data.DataTable), new Dictionary<string, string>());
			renameDictionary.Add(typeof(Altaxo.Graph.Gdi.GraphDocument), new Dictionary<string, string>());


			string oldName, newName;

			foreach (var item in list.ProjectItems)
			{
				if (item is Altaxo.Data.DataTable)
				{
					var table = (Altaxo.Data.DataTable)item;
					oldName = table.Name;
					table.Name = newName = GetRelocatedName(oldName, list.BaseFolder, baseFolder);
					Current.Project.DataTableCollection.Add(table);
					if (list.TryToKeepInternalReferences) newName = table.Name; // when trying to keep the references, we use the name the table gets after added to the collection (it can have changed during this operation).
					renameDictionary[typeof(Altaxo.Data.DataTable)].Add(oldName, newName);
				}
				else if (item is Altaxo.Graph.Gdi.GraphDocument)
				{
					var graph = (Altaxo.Graph.Gdi.GraphDocument)item;
					oldName = graph.Name;
					graph.Name = newName = GetRelocatedName(oldName, list.BaseFolder, baseFolder);
					if (Current.Project.GraphDocumentCollection.Contains(graph.Name))
						graph.Name = Current.Project.GraphDocumentCollection.FindNewName(graph.Name);
					Current.Project.GraphDocumentCollection.Add(graph);
					if (list.TryToKeepInternalReferences) newName = graph.Name; // when trying to keep the references, we use the name the graph gets after added to the collection (it can have changed during this operation).
					renameDictionary[typeof(Altaxo.Graph.Gdi.GraphDocument)].Add(oldName, newName);
				}
			}

			if (list.RelocateReferences)
			{

			}
		}





		static string GetRelocatedName(string name, string oldBaseFolder, string newBaseFolder)
		{
			string result = name;

			if ((null != oldBaseFolder) && name.StartsWith(oldBaseFolder))
			{
				result = name.Substring(oldBaseFolder.Length);
				result = newBaseFolder + result;
			}
			return result;
		}


		static void RelocateReferences(ProjectItemClipboardList pastedItems, Dictionary<System.Type, Dictionary<string, string>> renameDictionary)
		{
			foreach (var graph in pastedItems.ProjectItems.OfType<Altaxo.Graph.Gdi.GraphDocument>())
			{
				graph.VisitDocumentReferences((proxy, owner, propertyName) => CollectDataColumnFromProxyVisit(proxy, owner, propertyName, renameDictionary));
			}
		}


		static void CollectDataColumnFromProxyVisit(DocNodeProxy proxy, object owner, string propertyName, Dictionary<System.Type, Dictionary<string, string>> renameDictionary)
		{
			string newName;
			if (proxy.IsEmpty)
			{
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumn)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)proxy.DocumentObject);
				if (table != null && renameDictionary[typeof(Altaxo.Data.DataTable)].TryGetValue(table.Name, out newName))
					proxy.ReplacePathParts(DocumentPath.GetAbsolutePath(table), DocumentPath.FromDocumentCollectionNodeAndName(Current.Project.DataTableCollection, newName));
			}
			else if (proxy.DocumentObject is Altaxo.Data.DataColumnCollection)
			{
				var table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumnCollection)proxy.DocumentObject);
				if (table != null && renameDictionary[typeof(Altaxo.Data.DataTable)].TryGetValue(table.Name, out newName))
					proxy.ReplacePathParts(DocumentPath.GetAbsolutePath(table), DocumentPath.FromDocumentCollectionNodeAndName(Current.Project.DataTableCollection, newName));

			}
			else if (proxy.DocumentObject is DataTable)
			{
				var table = proxy.DocumentObject as DataTable;
				if (table != null && renameDictionary[typeof(Altaxo.Data.DataTable)].TryGetValue(table.Name, out newName))
					proxy.ReplacePathParts(DocumentPath.GetAbsolutePath(table), DocumentPath.FromDocumentCollectionNodeAndName(Current.Project.DataTableCollection, newName));
			}
			else if ((proxy is Altaxo.Data.NumericColumnProxy) || (proxy is Altaxo.Data.ReadableColumnProxy))
			{
				var path = proxy.DocumentPath;
				if (path.Count >= 2 && path.StartsWith(DocumentPath.GetPath(Current.Project.DataTableCollection, int.MaxValue)))
				{
					if (renameDictionary[typeof(Altaxo.Data.DataTable)].TryGetValue(path[1], out newName))
						proxy.ReplacePathParts(path.SubPath(2), DocumentPath.FromDocumentCollectionNodeAndName(Current.Project.DataTableCollection, newName));
				}
			}
		}

		#endregion

	}
}
