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

namespace Altaxo.Main.Commands
{
	/// <summary>
	/// Commands on project items like DataTables, Graphs and so.
	/// </summary>
	public static class ProjectItemCommands
	{
		#region Clipboard commands

		class ProjectItemList
		{
			/// <summary>Folder from which the items are copied.</summary>
			public string BaseFolder { get; set; }

			/// <summary>
			/// When true, at serialization the internal references are tried to keep internal, i.e. if for instance a table have to be renamed, the plot items in the deserialized graphs
			/// will be relocated to the renamed table.
			/// </summary>
			public bool TryToKeepInternalReferences { get; set; }

			/// <summary>List of project items to serialize/deserialize</summary>
			IList<object> _projectItems;



			private ProjectItemList()
			{
			}

			public ProjectItemList(IList<object> projectItems, string baseFolder)
			{
				BaseFolder = baseFolder;
				_projectItems = projectItems;
			}

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectItemList), 0)]
			class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					ProjectItemList s = (ProjectItemList)obj;

					info.AddValue("BaseFolder", s.BaseFolder);
					info.AddValue("TryToKeepInternalReferences", s.TryToKeepInternalReferences);
					info.CreateArray("Items", s._projectItems.Count);
					foreach (var item in s._projectItems)
						info.AddValue("e", item);
					info.CommitArray();
				}

				public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ProjectItemList s = null != o ? (ProjectItemList)o : new ProjectItemList();

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
			Altaxo.Serialization.ClipboardSerialization.PutObjectToClipboard(ClipboardFormat_ListOfProjectItems, new ProjectItemList(items, baseFolder));
		}


		public static bool CanPasteItemsFromClipboard()
		{
			return Altaxo.Serialization.ClipboardSerialization.IsClipboardFormatAvailable(ClipboardFormat_ListOfProjectItems);
		}

		public static void PasteItemsFromClipboard(string baseFolder)
		{
			var list = Altaxo.Serialization.ClipboardSerialization.GetObjectFromClipboard<ProjectItemList>(ClipboardFormat_ListOfProjectItems);

			foreach (var item in list.ProjectItems)
			{
				if (item is Altaxo.Data.DataTable)
				{
					var table = (Altaxo.Data.DataTable)item;
					table.Name = GetName(table.Name, list.BaseFolder, baseFolder);
					Current.Project.DataTableCollection.Add(table);
				}
				else if (item is Altaxo.Graph.Gdi.GraphDocument)
				{
					var graph = (Altaxo.Graph.Gdi.GraphDocument)item;
					graph.Name = GetName(graph.Name, list.BaseFolder, baseFolder);
					if (Current.Project.GraphDocumentCollection.Contains(graph.Name))
						graph.Name = Current.Project.GraphDocumentCollection.FindNewName(graph.Name);
					Current.Project.GraphDocumentCollection.Add(graph);
				}
			}
		}


		static string GetName(string name, string oldBaseFolder, string newBaseFolder)
		{
			string result = name;

			if ((null!=oldBaseFolder) && name.StartsWith(oldBaseFolder))
			{
				result = name.Substring(oldBaseFolder.Length);
				result = newBaseFolder + name;
			}
			return result;
		}

		#endregion

	}
}
