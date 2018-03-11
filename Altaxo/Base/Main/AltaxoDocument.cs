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

using Altaxo.Main;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace Altaxo
{
	/// <summary>
	/// Summary description for AltaxoDocument.
	/// </summary>
	public class AltaxoDocument
		:
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
		Main.INamedObjectCollection,
		Main.IProject
	{
		/// <summary>Collection of all data tables in this document.</summary>
		protected Altaxo.Data.DataTableCollection _dataTables = null; // The root of all the data

		/// <summary>Collection of all graphs in this document.</summary>
		protected Altaxo.Graph.Gdi.GraphDocumentCollection _graphs = null; // all graphs are stored here

		/// <summary>Collection of all graphs in this document.</summary>
		protected Altaxo.Graph.Graph3D.GraphDocumentCollection _graphs3D = null; // all graphs are stored here

		/// <summary>Collection of all notes documents in this document.</summary>
		protected Altaxo.Notes.NotesDocumentCollection _notesDocuments = null;

		/// <summary>
		/// The properties associated with the project folders. Please note that the properties of the project are also stored inside this collection, with the name being an empty string (root folder node).
		/// </summary>
		protected Altaxo.Main.Properties.ProjectFolderPropertyDocumentCollection _projectFolderProperties;

		/// <summary>Collection of all data tables layouts in this document.</summary>
		protected Altaxo.Worksheet.WorksheetLayoutCollection _tableLayouts = null;

		/// <summary>Collection of all fit function scripts in this document.</summary>
		private Altaxo.Scripting.FitFunctionScriptCollection _fitFunctionScripts;

		/// <summary>Keeps track of the name of all project items, and admisters them in virtual folders.</summary>
		protected ProjectFolders _projectFolders;

		/// <summary>A short string to identify the document. This string can be shown for instance in the graph windows.</summary>
		private DocumentInformation _documentInformation = new DocumentInformation();

		[NonSerialized]
		protected bool _isDirty = false;

		public event EventHandler IsDirtyChanged;

		public AltaxoDocument()
		{
			_dataTables = new Altaxo.Data.DataTableCollection(this);
			var commonDictionaryForGraphs = new SortedDictionary<string, IProjectItem>();

			_graphs = new Graph.Gdi.GraphDocumentCollection(this, commonDictionaryForGraphs);
			_graphs3D = new Graph.Graph3D.GraphDocumentCollection(this, commonDictionaryForGraphs);
			_notesDocuments = new Notes.NotesDocumentCollection(this, commonDictionaryForGraphs);

			_projectFolderProperties = new Main.Properties.ProjectFolderPropertyDocumentCollection(this);
			_tableLayouts = new Altaxo.Worksheet.WorksheetLayoutCollection(this);
			_fitFunctionScripts = new Altaxo.Scripting.FitFunctionScriptCollection(this);
			_projectFolders = new ProjectFolders(this);
		}

		#region Serialization

		public void SaveToZippedFile(ZipArchive zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();
			int compressionLevel = 1;
			// DateTime time1 = DateTime.UtcNow;

			// first, we save all tables into the tables subdirectory
			foreach (Altaxo.Data.DataTable table in this._dataTables)
			{
				try
				{
					var zipEntry = zippedStream.CreateEntry("Tables/" + table.Name + ".xml");
					using (var zs = zipEntry.Open())
					{
						//ZipEntry ZipEntry = new ZipEntry("Tables/"+table.Name+".xml");
						//zippedStream.PutNextEntry(ZipEntry);
						//zippedStream.SetLevel(0);
						info.BeginWriting(zs);
						info.AddValue("Table", table);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// second, we save all graphs into the Graphs subdirectory
			foreach (Graph.Gdi.GraphDocument graph in this._graphs)
			{
				try
				{
					var zipEntry = zippedStream.CreateEntry("Graphs/" + graph.Name + ".xml");
					using (var zs = zipEntry.Open())
					{
						//ZipEntry ZipEntry = new ZipEntry("Graphs/"+graph.Name+".xml");
						//zippedStream.PutNextEntry(ZipEntry);
						//zippedStream.SetLevel(0);
						info.BeginWriting(zs);
						info.AddValue("Graph", graph);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// second, we save all graphs into the Graphs3D subdirectory
			foreach (Graph.Graph3D.GraphDocument graph in this._graphs3D)
			{
				try
				{
					var zipEntry = zippedStream.CreateEntry("Graphs3D/" + graph.Name + ".xml");
					using (var zs = zipEntry.Open())
					{
						//ZipEntry ZipEntry = new ZipEntry("Graphs/"+graph.Name+".xml");
						//zippedStream.PutNextEntry(ZipEntry);
						//zippedStream.SetLevel(0);
						info.BeginWriting(zs);
						info.AddValue("Graph", graph);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// next, we save all notes documents
			foreach (var item in this._notesDocuments)
			{
				try
				{
					var zipEntry = zippedStream.CreateEntry("Notes/" + item.Name + ".xml");
					using (var zs = zipEntry.Open())
					{
						info.BeginWriting(zs);
						info.AddValue("Note", item);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// third, we save all TableLayouts into the TableLayouts subdirectory
			foreach (Altaxo.Worksheet.WorksheetLayout layout in this._tableLayouts)
			{
				if (layout.DataTable == null)
					continue; // dont save orphaned layouts

				try
				{
					var zipEntry = zippedStream.CreateEntry("TableLayouts/" + layout.Name + ".xml");
					using (var zs = zipEntry.Open())
					{
						//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
						//zippedStream.PutNextEntry(ZipEntry);
						//zippedStream.SetLevel(0);
						info.BeginWriting(zs);
						info.AddValue("WorksheetLayout", layout);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// 4th, we save all FitFunctions into the FitFunctions subdirectory
			foreach (Altaxo.Scripting.FitFunctionScript fit in this._fitFunctionScripts)
			{
				try
				{
					var zipEntry = zippedStream.CreateEntry("FitFunctionScripts/" + fit.CreationTime.ToString() + ".xml");
					//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					using (var zs = zipEntry.Open())
					{
						info.BeginWriting(zs);
						info.AddValue("FitFunctionScript", fit);
						info.EndWriting();
					}
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}
			{
				// 5th, we save all folder properties
				foreach (var folderProperty in this._projectFolderProperties)
				{
					try
					{
						var zipEntry = zippedStream.CreateEntry("FolderProperties/" + folderProperty.Name + ".xml");
						//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
						//zippedStream.PutNextEntry(ZipEntry);
						//zippedStream.SetLevel(0);
						using (var zs = zipEntry.Open())
						{
							info.BeginWriting(zs);
							info.AddValue("FolderProperty", folderProperty);
							info.EndWriting();
						}
					}
					catch (Exception exc)
					{
						errorText.Append(exc.ToString());
					}
				}
			}

			{
				// nun noch den DocumentIdentifier abspeichern
				var zipEntry = zippedStream.CreateEntry("DocumentInformation.xml");
				using (var zs = zipEntry.Open())
				{
					info.BeginWriting(zs);
					info.AddValue("DocumentInformation", _documentInformation);
					info.EndWriting();
				}
			}
			//  Current.Console.WriteLine("Saving took {0} sec.", (DateTime.UtcNow - time1).TotalSeconds);

			if (errorText.Length != 0)
				throw new ApplicationException(errorText.ToString());
		}

		public void RestoreFromZippedFile(ZipArchive zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();

			foreach (var zipEntry in zipFile.Entries)
			{
				try
				{
					if (zipEntry.FullName.StartsWith("Tables/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("Table", null);
							if (readedobject is Altaxo.Data.DataTable)
								this._dataTables.Add((Altaxo.Data.DataTable)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("Graphs/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("Graph", null);
							if (readedobject is Graph.Gdi.GraphDocument)
								this._graphs.Add((Graph.Gdi.GraphDocument)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("Graphs3D/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("Graph", null);
							if (readedobject is Graph.Graph3D.GraphDocument)
								this._graphs3D.Add((Graph.Graph3D.GraphDocument)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("Notes/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("Note", null);
							if (readedobject is Notes.NotesDocument noteDoc)
								this._notesDocuments.Add(noteDoc);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("TableLayouts/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("WorksheetLayout", null);
							if (readedobject is Altaxo.Worksheet.WorksheetLayout)
								this._tableLayouts.Add((Altaxo.Worksheet.WorksheetLayout)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("FitFunctionScripts/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("FitFunctionScript", null);
							if (readedobject is Altaxo.Scripting.FitFunctionScript)
								this._fitFunctionScripts.Add((Altaxo.Scripting.FitFunctionScript)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName.StartsWith("FolderProperties/"))
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("FolderProperty", null);
							if (readedobject is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
								this._projectFolderProperties.Add((Altaxo.Main.Properties.ProjectFolderPropertyDocument)readedobject);
							info.EndReading();
						}
					}
					else if (zipEntry.FullName == "DocumentInformation.xml")
					{
						using (var zipinpstream = zipEntry.Open())
						{
							info.BeginReading(zipinpstream);
							object readedobject = info.GetValue("DocumentInformation", null);
							if (readedobject is DocumentInformation)
								this._documentInformation = (DocumentInformation)readedobject;
							info.EndReading();
						}
					}
				}
				catch (Exception exc)
				{
					errorText.Append("Error deserializing ");
					errorText.Append(zipEntry.Name);
					errorText.Append(", ");
					errorText.Append(exc.ToString());
				}
			}

			try
			{
				info.AnnounceDeserializationEnd(this, false);
			}
			catch (Exception exc)
			{
				errorText.Append(exc.ToString());
			}

			if (errorText.Length != 0)
				throw new ApplicationException(errorText.ToString());
		}

		#endregion Serialization

		public Altaxo.Data.DataTableCollection DataTableCollection
		{
			get { return _dataTables; }
		}

		public Altaxo.Graph.Gdi.GraphDocumentCollection GraphDocumentCollection
		{
			get { return _graphs; }
		}

		public Altaxo.Graph.Graph3D.GraphDocumentCollection Graph3DDocumentCollection
		{
			get { return _graphs3D; }
		}

		public Altaxo.Notes.NotesDocumentCollection NotesDocumentCollection
		{
			get { return _notesDocuments; }
		}

		public Altaxo.Worksheet.WorksheetLayoutCollection TableLayouts
		{
			get { return this._tableLayouts; }
		}

		public Altaxo.Scripting.FitFunctionScriptCollection FitFunctionScripts
		{
			get { return _fitFunctionScripts; }
		}

		/// <summary>
		/// The properties associated with the project folders. Please note that the properties of the project are also stored inside this collection, with the name being an empty string (root folder node).
		/// </summary>
		public Altaxo.Main.Properties.ProjectFolderPropertyDocumentCollection ProjectFolderProperties
		{
			get { return _projectFolderProperties; }
		}

		/// <summary>
		/// Get information about the folders in this project.
		/// </summary>
		public ProjectFolders Folders
		{
			get { return _projectFolders; }
		}

		public string DocumentIdentifier
		{
			get
			{
				return _documentInformation.DocumentIdentifier;
			}
			set
			{
				_documentInformation.DocumentIdentifier = value;
			}
		}

		protected virtual void OnDirtyChanged()
		{
			if (null != IsDirtyChanged)
				IsDirtyChanged(this, EventArgs.Empty);
		}

		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				bool oldValue = _isDirty;
				_isDirty = value;
				if (oldValue != _isDirty)
				{
					OnDirtyChanged();
				}
			}
		}

		protected override bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			IsDirty = true;
			return base.HandleLowPriorityChildChangeCases(sender, ref e);
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData = e ?? EventArgs.Empty;
		}

		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return null;
			}
			set
			{
				if (null != value)
					throw new InvalidOperationException("The parent object of AltaxoDocument can not be set and is always null");
			}
		}

		public override string Name
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException("The name of AltaxoDocument can not be set and is always an empty string");
			}
		}

		public Altaxo.Data.DataTable CreateNewTable(string worksheetName, bool bCreateDefaultColumns)
		{
			Altaxo.Data.DataTable dt1 = new Altaxo.Data.DataTable(worksheetName);

			if (bCreateDefaultColumns)
			{
				dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(), "A", Altaxo.Data.ColumnKind.X);
				dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(), "B");
			}

			DataTableCollection.Add(dt1);

			return dt1;
		}

		public Altaxo.Worksheet.WorksheetLayout CreateNewTableLayout(Altaxo.Data.DataTable table)
		{
			if (!this._dataTables.Contains(table))
				this._dataTables.Add(table);

			Altaxo.Worksheet.WorksheetLayout layout = new Altaxo.Worksheet.WorksheetLayout(table);
			this._tableLayouts.Add(layout);
			return layout;
		}

		public override IDocumentLeafNode GetChildObjectNamed(string name)
		{
			switch (name)
			{
				case "Tables":
					return this._dataTables;

				case "Graphs":
					return this._graphs;

				case "Graphs3D":
					return this._graphs3D;

				case "Notes":
					return this._notesDocuments;

				case "TableLayouts":
					return this._tableLayouts;

				case "FitFunctionScripts":
					return this._fitFunctionScripts;

				case "FolderProperties":
					return this._projectFolderProperties;

				case "ProjectFolders":
					return this._projectFolders;
			}
			return null;
		}

		public override string GetNameOfChildObject(IDocumentLeafNode o)
		{
			if (null == o)
				return null;
			else if (object.ReferenceEquals(o, this._dataTables))
				return "Tables";
			else if (object.ReferenceEquals(o, this._graphs))
				return "Graphs";
			else if (object.ReferenceEquals(o, this._graphs3D))
				return "Graphs3D";
			else if (object.ReferenceEquals(o, this._notesDocuments))
				return "Notes";
			else if (object.ReferenceEquals(o, this._tableLayouts))
				return "TableLayouts";
			else if (object.ReferenceEquals(o, this._fitFunctionScripts))
				return "FitFunctionScripts";
			else if (object.ReferenceEquals(o, this._projectFolderProperties))
				return "FolderProperties";
			else if (object.ReferenceEquals(o, this._projectFolders))
				return "ProjectFolders";
			else
				return null;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataTables)
				yield return new Main.DocumentNodeAndName(_dataTables, () => _dataTables = null, "Tables");

			if (null != _graphs)
				yield return new Main.DocumentNodeAndName(_graphs, () => _graphs = null, "Graphs");

			if (null != _graphs3D)
				yield return new Main.DocumentNodeAndName(_graphs3D, () => _graphs3D = null, "Graphs3D");

			if (null != _notesDocuments)
				yield return new Main.DocumentNodeAndName(_notesDocuments, () => _notesDocuments = null, "Notes");

			if (null != _tableLayouts)
				yield return new Main.DocumentNodeAndName(_tableLayouts, () => _tableLayouts = null, "TableLayouts");

			if (null != _fitFunctionScripts)
				yield return new Main.DocumentNodeAndName(_fitFunctionScripts, () => _fitFunctionScripts = null, "FitFunctionScripts");

			if (null != _projectFolderProperties)
				yield return new Main.DocumentNodeAndName(_projectFolderProperties, () => _projectFolderProperties = null, "FolderProperties");

			if (null != _projectFolders)
				yield return new Main.DocumentNodeAndName(_projectFolders, () => _projectFolders = null, "ProjectFolders");
		}

		#region Static functions

		/// <summary>
		/// Gets the types of project item currently supported in the document.
		/// </summary>
		/// <value>
		/// The project item types.
		/// </value>
		public IEnumerable<System.Type> ProjectItemTypes
		{
			get
			{
				yield return typeof(Altaxo.Data.DataTable);
				yield return typeof(Altaxo.Graph.Gdi.GraphDocument);
				yield return typeof(Altaxo.Graph.Graph3D.GraphDocument);
				yield return typeof(Altaxo.Notes.NotesDocument);
				yield return typeof(Altaxo.Main.Properties.ProjectFolderPropertyDocument);
			}
		}

		/// <summary>
		/// Gets the root path for a given project item type.
		/// </summary>
		/// <param name="type">The type of project item.</param>
		/// <returns>The root path of this type of item.</returns>
		public AbsoluteDocumentPath GetRootPathForProjectItemType(System.Type type)
		{
			if (type == typeof(Altaxo.Data.DataTable))
				return AbsoluteDocumentPath.GetAbsolutePath(Current.Project.DataTableCollection);
			else if (type == typeof(Altaxo.Graph.Gdi.GraphDocument))
				return AbsoluteDocumentPath.GetAbsolutePath(Current.Project.GraphDocumentCollection);
			else if (type == typeof(Altaxo.Graph.Graph3D.GraphDocument))
				return AbsoluteDocumentPath.GetAbsolutePath(Current.Project.Graph3DDocumentCollection);
			else if (type == typeof(Altaxo.Notes.NotesDocument))
				return AbsoluteDocumentPath.GetAbsolutePath(Current.Project.NotesDocumentCollection);
			else if (type == typeof(Altaxo.Main.Properties.ProjectFolderPropertyDocument))
				return AbsoluteDocumentPath.GetAbsolutePath(Current.Project.ProjectFolderProperties);
			else
				throw new ArgumentOutOfRangeException(string.Format("Unknown type of project item: {0}", type));
		}

		/// <summary>
		/// Gets the document path for project item, using its type and name. It is not neccessary that the item is part of the project yet.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The document part for the project item, deduces from its type and its name.</returns>
		/// <exception cref="System.ArgumentNullException">item</exception>
		public AbsoluteDocumentPath GetDocumentPathForProjectItem(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException("item");

			return GetRootPathForProjectItemType(item.GetType()).Append(((INameOwner)item).Name);
		}

		/// <summary>
		/// Adds the provided project item to the Altaxo project, for instance a table or a graph, to the project. For <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>s,
		/// if a document with the same name is already present, the properties are merged.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
		public bool ContainsItem(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException(nameof(item));

			if (item is Altaxo.Data.DataTable table)
			{
				return this.DataTableCollection.Contains(table);
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument graphGdiDoc)
			{
				return this.GraphDocumentCollection.Contains(graphGdiDoc);
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument graph3DDoc)
			{
				return this.Graph3DDocumentCollection.Contains(graph3DDoc);
			}
			else if (item is Altaxo.Notes.NotesDocument notesDoc)
			{
				return this.NotesDocumentCollection.Contains(notesDoc);
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
			{
				return this.ProjectFolderProperties.Contains(item.Name);
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("Processing an item of type {0} is currently not implemented", item.GetType()));
			}
		}

		/// <summary>
		/// Adds the provided project item to the Altaxo project, for instance a table or a graph, to the project. For <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>s,
		/// if a document with the same name is already present, the properties are merged.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
		public void AddItem(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException("item");

			if (item is Altaxo.Data.DataTable table)
			{
				this.DataTableCollection.Add(table);
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument graphGdiDoc)
			{
				this.GraphDocumentCollection.Add(graphGdiDoc);
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument graph3DDoc)
			{
				this.Graph3DDocumentCollection.Add(graph3DDoc);
			}
			else if (item is Altaxo.Notes.NotesDocument notesDoc)
			{
				this.NotesDocumentCollection.Add(notesDoc);
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
			{
				if (!this.ProjectFolderProperties.Contains(propDoc.Name))
				{
					Current.Project.ProjectFolderProperties.Add(propDoc); // if not existing, then add the new property document
				}
				else
				{
					Current.Project.ProjectFolderProperties[propDoc.Name].PropertyBagNotNull.MergePropertiesFrom(propDoc.PropertyBag, true); // if existing, then merge the properties into the existing bag
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("Adding an item of type {0} is currently not implemented", item.GetType()));
			}
		}

		/// <summary>
		/// Adds the provided project item to the Altaxo project, for instance a table or a graph, to the project. If another project item with the same name already exists,
		/// a new unique name for the item is found, based on the given name.
		/// For <see cref="T:Altaxo.Main.Properties.ProjectFolderPropertyDocument"/>s, if a document with the same name is already present, the properties are merged.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
		public void AddItemWithThisOrModifiedName(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException(nameof(item));

			if (item is Altaxo.Data.DataTable table)
			{
				if (table.Name == null || table.Name == string.Empty)
					table.Name = Current.Project.DataTableCollection.FindNewTableName();
				else if (Current.Project.DataTableCollection.Contains(table.Name))
					table.Name = Current.Project.DataTableCollection.FindNewTableName(table.Name);

				this.DataTableCollection.Add(table);
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument graphGdi)
			{
				if (graphGdi.Name == null || graphGdi.Name == string.Empty)
					graphGdi.Name = Current.Project.GraphDocumentCollection.FindNewItemName();
				else if (Current.Project.GraphDocumentCollection.Contains(graphGdi.Name))
					graphGdi.Name = Current.Project.GraphDocumentCollection.FindNewItemName(graphGdi.Name);

				this.GraphDocumentCollection.Add((Altaxo.Graph.Gdi.GraphDocument)item);
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument graph3D)
			{
				if (graph3D.Name == null || graph3D.Name == string.Empty)
					graph3D.Name = Current.Project.Graph3DDocumentCollection.FindNewItemName();
				else if (Current.Project.Graph3DDocumentCollection.Contains(graph3D.Name))
					graph3D.Name = Current.Project.Graph3DDocumentCollection.FindNewItemName(graph3D.Name);

				this.Graph3DDocumentCollection.Add((Altaxo.Graph.Graph3D.GraphDocument)item);
			}
			else if (item is Altaxo.Notes.NotesDocument notesDoc)
			{
				if (notesDoc.Name == null || notesDoc.Name == string.Empty)
					notesDoc.Name = Current.Project.NotesDocumentCollection.FindNewItemName();
				else if (Current.Project.NotesDocumentCollection.Contains(notesDoc.Name))
					notesDoc.Name = Current.Project.NotesDocumentCollection.FindNewItemName(notesDoc.Name);

				this.NotesDocumentCollection.Add((Altaxo.Notes.NotesDocument)item);
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
			{
				var doc = (Altaxo.Main.Properties.ProjectFolderPropertyDocument)item;
				if (!this.ProjectFolderProperties.Contains(doc.Name))
				{
					Current.Project.ProjectFolderProperties.Add(doc); // if not existing, then add the new property document
				}
				else
				{
					Current.Project.ProjectFolderProperties[doc.Name].PropertyBagNotNull.MergePropertiesFrom(doc.PropertyBag, true); // if existing, then merge the properties into the existing bag
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("Adding an item of type {0} is currently not implemented", item.GetType()));
			}
		}

		/// <summary>
		/// Tries to get an existring project item with the same type and name as the provided item.
		/// </summary>
		/// <param name="item">The item to test for.</param>
		/// <returns>True an item with the same type and name as the provided item exists in the project, that existing item is returned; otherwise, the return value is null.</returns>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public IProjectItem TryGetExistingItemWithSameTypeAndName(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException(nameof(item));

			string name = item.Name;

			if (item is Altaxo.Data.DataTable)
			{
				if (this.DataTableCollection.Contains(name))
					return this.DataTableCollection[name];
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument)
			{
				if (this.GraphDocumentCollection.Contains(item.Name))
					return this.GraphDocumentCollection[item.Name];
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument)
			{
				if (this.Graph3DDocumentCollection.Contains(item.Name))
					return this.Graph3DDocumentCollection[item.Name];
			}
			else if (item is Altaxo.Notes.NotesDocument)
			{
				if (this.NotesDocumentCollection.Contains(item.Name))
					return this.NotesDocumentCollection[item.Name];
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
			{
				if (this.ProjectFolderProperties.Contains(item.Name))
					return this.ProjectFolderProperties[item.Name];
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("Processing an item of type {0} is currently not implemented", item.GetType()));
			}

			return null;
		}

		/// <summary>
		/// Tests whether an item with the same type and name is already present in the project.
		/// </summary>
		/// <param name="item">The item to test.</param>
		/// <returns>True if an item with the same type and same name is already present in the project.</returns>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public bool ExistsItemWithSameTypeAndName(IProjectItem item)
		{
			return null != TryGetExistingItemWithSameTypeAndName(item);
		}

		/// <summary>
		/// Removes the provided project item to the Altaxo project, for instance a table or a graph, to the project.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <exception cref="System.ArgumentNullException">item</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
		public void RemoveItem(IProjectItem item)
		{
			if (null == item)
				throw new ArgumentNullException("item");

			if (item is Altaxo.Data.DataTable table)
			{
				this.DataTableCollection.Remove(table);
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument graphGdiDoc)
			{
				this.GraphDocumentCollection.Remove(graphGdiDoc);
			}
			else if (item is Altaxo.Graph.Graph3D.GraphDocument graph3DDoc)
			{
				this.Graph3DDocumentCollection.Remove(graph3DDoc);
			}
			else if (item is Altaxo.Notes.NotesDocument notesDoc)
			{
				this.NotesDocumentCollection.Remove(notesDoc);
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
			{
				this.ProjectFolderProperties.Remove(propDoc);
			}
			else
			{
				throw new ArgumentOutOfRangeException(string.Format("Removing an item of type {0} is currently not implemented", item.GetType()));
			}
		}

		#endregion Static functions
	}
}
