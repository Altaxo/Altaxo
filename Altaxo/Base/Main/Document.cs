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

using Altaxo.Graph.Gdi;
using Altaxo.Main;
using Altaxo.Main.Properties;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Altaxo
{
	/// <summary>
	/// Summary description for AltaxoDocument.
	/// </summary>
	[SerializationSurrogate(0, typeof(AltaxoDocument.SerializationSurrogate0))]
	[SerializationVersion(0, "Initial version of the main document only contains m_DataSet")]
	public class AltaxoDocument
		:
		IDeserializationCallback,
		Main.INamedObjectCollection,
		Main.IChildChangedEventSink
	{
		/// <summary>Collection of all data tables in this document.</summary>
		protected Altaxo.Data.DataTableCollection _dataTables = null; // The root of all the data

		/// <summary>Collection of all graphs in this document.</summary>
		protected Altaxo.Graph.Gdi.GraphDocumentCollection _graphs = null; // all graphs are stored here

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

		public event EventHandler DirtyChanged;

		[NonSerialized]
		private bool _isDeserializationFinished = false;

		public AltaxoDocument()
		{
			_dataTables = new Altaxo.Data.DataTableCollection(this);
			_graphs = new GraphDocumentCollection(this);
			_projectFolderProperties = new Main.Properties.ProjectFolderPropertyDocumentCollection(this);
			_tableLayouts = new Altaxo.Worksheet.WorksheetLayoutCollection(this);
			_fitFunctionScripts = new Altaxo.Scripting.FitFunctionScriptCollection();
			_projectFolders = new ProjectFolders(this);
		}

		#region Serialization

		private class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				info.AddValue("DataTableCollection", s._dataTables);
				//info.AddValue("Worksheets",s.m_Worksheets);
				//  info.AddValue("GraphForms",s.m_GraphForms);
			}

			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				AltaxoDocument s = (AltaxoDocument)obj;
				s._dataTables = (Altaxo.Data.DataTableCollection)info.GetValue("DataTableCollection", typeof(Altaxo.Data.DataTableCollection));
				// s.tstObj    = (AltaxoTestObject02)info.GetValue("TstObj",typeof(AltaxoTestObject02));
				//s.m_Worksheets = (System.Collections.ArrayList)info.GetValue("Worksheets",typeof(System.Collections.ArrayList));
				//  s.m_GraphForms = (System.Collections.ArrayList)info.GetValue("GraphForms",typeof(System.Collections.ArrayList));
				s._isDirty = false;
				return s;
			}
		}

		public void OnDeserialization(object obj)
		{
			if (!_isDeserializationFinished && obj is DeserializationFinisher)
			{
				_isDeserializationFinished = true;
				DeserializationFinisher finisher = new DeserializationFinisher(this);

				_dataTables.ParentObject = this;
				_dataTables.OnDeserialization(finisher);
			}
		}

		public void RestoreWindowsAfterDeserialization()
		{
		}

		public void SaveToZippedFile(ICompressedFileContainerStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();
			int compressionLevel = 1;
			// DateTime time1 = DateTime.UtcNow;

			// first, we save all tables into the tables subdirectory
			foreach (Altaxo.Data.DataTable table in this._dataTables)
			{
				try
				{
					zippedStream.StartFile("Tables/" + table.Name + ".xml", compressionLevel);
					//ZipEntry ZipEntry = new ZipEntry("Tables/"+table.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("Table", table);
					info.EndWriting();
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// second, we save all graphs into the Graphs subdirectory
			foreach (GraphDocument graph in this._graphs)
			{
				try
				{
					zippedStream.StartFile("Graphs/" + graph.Name + ".xml", compressionLevel);
					//ZipEntry ZipEntry = new ZipEntry("Graphs/"+graph.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("Graph", graph);
					info.EndWriting();
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// third, we save all TableLayouts into the TableLayouts subdirectory
			foreach (Altaxo.Worksheet.WorksheetLayout layout in this._tableLayouts)
			{
				try
				{
					zippedStream.StartFile("TableLayouts/" + layout.Name + ".xml", compressionLevel);
					//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("WorksheetLayout", layout);
					info.EndWriting();
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
					zippedStream.StartFile("FitFunctionScripts/" + fit.CreationTime.ToString() + ".xml", compressionLevel);
					//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("FitFunctionScript", fit);
					info.EndWriting();
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// 5th, we save all folder properties
			foreach (var folderProperty in this._projectFolderProperties)
			{
				try
				{
					zippedStream.StartFile("FolderProperties/" + folderProperty.Name + ".xml", compressionLevel);
					//ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
					//zippedStream.PutNextEntry(ZipEntry);
					//zippedStream.SetLevel(0);
					info.BeginWriting(zippedStream.Stream);
					info.AddValue("FolderProperty", folderProperty);
					info.EndWriting();
				}
				catch (Exception exc)
				{
					errorText.Append(exc.ToString());
				}
			}

			// nun noch den DocumentIdentifier abspeichern
			zippedStream.StartFile("DocumentInformation.xml", compressionLevel);
			info.BeginWriting(zippedStream.Stream);
			info.AddValue("DocumentInformation", _documentInformation);
			info.EndWriting();

			//  Current.Console.WriteLine("Saving took {0} sec.", (DateTime.UtcNow - time1).TotalSeconds);

			if (errorText.Length != 0)
				throw new ApplicationException(errorText.ToString());
		}

		public void RestoreFromZippedFile(ICompressedFileContainer zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();

			foreach (IFileContainerItem zipEntry in zipFile)
			{
				try
				{
					if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Tables/"))
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("Table", this);
						if (readedobject is Altaxo.Data.DataTable)
							this._dataTables.Add((Altaxo.Data.DataTable)readedobject);
						info.EndReading();
					}
					else if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Graphs/"))
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("Graph", this);
						if (readedobject is GraphDocument)
							this._graphs.Add((GraphDocument)readedobject);
						info.EndReading();
					}
					else if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("TableLayouts/"))
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("WorksheetLayout", this);
						if (readedobject is Altaxo.Worksheet.WorksheetLayout)
							this._tableLayouts.Add((Altaxo.Worksheet.WorksheetLayout)readedobject);
						info.EndReading();
					}
					else if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("FitFunctionScripts/"))
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("FitFunctionScript", this);
						if (readedobject is Altaxo.Scripting.FitFunctionScript)
							this._fitFunctionScripts.Add((Altaxo.Scripting.FitFunctionScript)readedobject);
						info.EndReading();
					}
					else if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("FolderProperties/"))
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("FolderProperty", this);
						if (readedobject is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
							this._projectFolderProperties.Add((Altaxo.Main.Properties.ProjectFolderPropertyDocument)readedobject);
						info.EndReading();
					}
					else if (!zipEntry.IsDirectory && zipEntry.Name == "DocumentInformation.xml")
					{
						System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
						info.BeginReading(zipinpstream);
						object readedobject = info.GetValue("DocumentInformation", this);
						if (readedobject is DocumentInformation)
							this._documentInformation = (DocumentInformation)readedobject;
						info.EndReading();
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
			if (null != DirtyChanged)
				DirtyChanged(this, EventArgs.Empty);
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

		public void EhChildChanged(object sender, EventArgs e)
		{
			this.IsDirty = true;
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

		public Altaxo.Graph.Gdi.GraphDocument CreateNewGraphDocument()
		{
			return CreateNewGraphDocument(null);
		}

		public Altaxo.Graph.Gdi.GraphDocument CreateNewGraphDocument(string preferredName)
		{
			GraphDocument doc = new GraphDocument();
			if (!string.IsNullOrEmpty(preferredName))
				doc.Name = preferredName;

			GraphDocumentCollection.Add(doc);

			return doc;
		}

		public Altaxo.Worksheet.WorksheetLayout CreateNewTableLayout(Altaxo.Data.DataTable table)
		{
			if (!this._dataTables.Contains(table))
				this._dataTables.Add(table);

			Altaxo.Worksheet.WorksheetLayout layout = new Altaxo.Worksheet.WorksheetLayout(table);
			this._tableLayouts.Add(layout);
			return layout;
		}

		/// <summary>
		/// Adds an item, for instance a table or a graph, to the project.
		/// </summary>
		/// <param name="item">Item to add.</param>
		public void AddItem(object item)
		{
			if (item == null)
				throw new ArgumentNullException("Can't add a item which is null");

			if (item is Altaxo.Data.DataTable)
			{
				_dataTables.Add(item as Altaxo.Data.DataTable);
			}
			else if (item is Altaxo.Graph.Gdi.GraphDocument)
			{
				_graphs.Add(item as Altaxo.Graph.Gdi.GraphDocument);
			}
			else if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
			{
				_projectFolderProperties.Add(item as Altaxo.Main.Properties.ProjectFolderPropertyDocument);
			}
			else
			{
				throw new NotImplementedException(string.Format("Adding an item of type {0} is currently not implemented", item.GetType()));
			}
		}

		public object GetChildObjectNamed(string name)
		{
			switch (name)
			{
				case "Tables":
					return this._dataTables;

				case "Graphs":
					return this._graphs;

				case "TableLayouts":
					return this._tableLayouts;

				case "FitFunctionScripts":
					return this._fitFunctionScripts;
			}
			return null;
		}

		public string GetNameOfChildObject(object o)
		{
			if (null == o)
				return null;
			else if (object.ReferenceEquals(o, this._dataTables))
				return "Tables";
			else if (object.ReferenceEquals(o, this._graphs))
				return "Graphs";
			else if (object.ReferenceEquals(o, this._fitFunctionScripts))
				return "FitFunctionScripts";
			else
				return null;
		}

		#region Static functions

		/// <summary>
		/// Adds relocation data for tables in a specific project folder.
		/// </summary>
		/// <param name="options">The relocation data to fill in (this object is changed during the operation).</param>
		/// <param name="originalFolder">The original project folder of the data.</param>
		/// <param name="destinationFolder">The new project folder that references should point to.</param>
		public static void AddRelocationDataForTables(DocNodePathReplacementOptions options, string originalFolder, string destinationFolder)
		{
			var srcPath = DocumentPath.GetAbsolutePath(Current.Project.DataTableCollection);
			srcPath.Add(originalFolder);
			var destPath = DocumentPath.GetAbsolutePath(Current.Project.DataTableCollection);
			destPath.Add(destinationFolder);
			options.AddPathReplacement(srcPath, destPath);
		}

		#endregion Static functions
	}
}