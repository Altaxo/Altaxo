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

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.Serialization;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Main.Services.Files;
using Altaxo.Serialization;

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
    protected Altaxo.Text.TextDocumentCollection _textDocuments = null;

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
      _textDocuments = new Text.TextDocumentCollection(this);

      _projectFolderProperties = new Main.Properties.ProjectFolderPropertyDocumentCollection(this);
      _tableLayouts = new Altaxo.Worksheet.WorksheetLayoutCollection(this);
      _fitFunctionScripts = new Altaxo.Scripting.FitFunctionScriptCollection(this);
      _projectFolders = new ProjectFolders(this);
    }

    #region Serialization

    /// <summary>
    /// Saves the document to a project archive.
    /// </summary>
    /// <param name="archiveToSaveTo">The archive to save the document to.</param>
    /// <param name="info">The serialization information.</param>
    /// <param name="originalArchive">The original archive that belongs to the project being saved. Can accelerate the saving of the document by recycling some of the already saved streams.
    /// This parameter can be null.</param>
    /// <returns>A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</returns>
    /// <exception cref="ApplicationException"></exception>
    public Dictionary<string, IProjectItem> SaveToArchive(IProjectArchive archiveToSaveTo, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info, IProjectArchive originalArchive = null, IProjectArchiveManager projectArchiveManager = null)
    {
      var errorText = new System.Text.StringBuilder();
      var dictionary = new Dictionary<string, IProjectItem>();

      // If true, data were stored separately from the table
      bool supportsSeparateDataStorage = archiveToSaveTo.SupportsDeferredLoading;

      // If true, archive entries (of items that have not changed) are copied directly from the original archive to the new archive
      bool supportsStreamRecycling = originalArchive?.GetType() == archiveToSaveTo.GetType() && archiveToSaveTo.SupportsCopyEntryFrom(originalArchive);

      if (supportsSeparateDataStorage)
      {
        info.SetProperty(Altaxo.Data.DataTable.SerializationInfoProperty_SupportsSeparatedData, "true");
      }

      // first, we save all tables into the tables subdirectory
      foreach (Altaxo.Data.DataTable table in _dataTables)
      {
        try
        {
          string entryName = string.Concat("Tables/", table.Name, ".xml");
          var zipEntry = archiveToSaveTo.CreateEntry(entryName);
          using (var zs = zipEntry.OpenForWriting())
          {
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

      if (supportsSeparateDataStorage)
      {
        var orginalValue_StoreDataOnly = info.SaveAndSetProperty(Altaxo.Data.DataColumnCollection.SerialiationInfoProperty_StoreDataOnly, "true");
        // first, we save all tables into the tables subdirectory
        foreach (Altaxo.Data.DataTable table in _dataTables)
        {
          var entryName = string.Concat("TableData/", table.Name, ".xml");
          dictionary.Add(entryName, table);
          try
          {
            if (supportsStreamRecycling &&
                !table.DataColumns.IsDataDirty &&
                table.DataColumns.DeferredDataMemento is IProjectArchiveEntryMemento entryMemento &&
                originalArchive.ContainsEntry(entryMemento.EntryName))
            {
              archiveToSaveTo.CopyEntryFrom(originalArchive, sourceEntryName: entryMemento.EntryName, destinationEntryName: entryName);
            }
            else
            {
              var zipEntry = archiveToSaveTo.CreateEntry(entryName);
              using (var zs = zipEntry.OpenForWriting())
              {
                info.BeginWriting(zs);
                info.AddValue("TableData", table.DataColumns);
                info.EndWriting();
              }
            }
          }
          catch (Exception exc)
          {
            errorText.Append(exc.ToString());
          }
        }
        info.SetProperty(Altaxo.Data.DataColumnCollection.SerialiationInfoProperty_StoreDataOnly, orginalValue_StoreDataOnly);
      }

      // second, we save all graphs into the Graphs subdirectory
      foreach (Graph.Gdi.GraphDocument graph in _graphs)
      {
        try
        {
          var zipEntry = archiveToSaveTo.CreateEntry("Graphs/" + graph.Name + ".xml");
          using (var zs = zipEntry.OpenForWriting())
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
      foreach (Graph.Graph3D.GraphDocument graph in _graphs3D)
      {
        try
        {
          var zipEntry = archiveToSaveTo.CreateEntry("Graphs3D/" + graph.Name + ".xml");
          using (var zs = zipEntry.OpenForWriting())
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
      foreach (var item in _textDocuments)
      {
        try
        {
          var zipEntry = archiveToSaveTo.CreateEntry("Texts/" + item.Name + ".xml");
          using (var zs = zipEntry.OpenForWriting())
          {
            info.BeginWriting(zs);
            info.AddValue("Text", item);
            info.EndWriting();
          }
        }
        catch (Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }

      // third, we save all TableLayouts into the TableLayouts subdirectory
      foreach (Altaxo.Worksheet.WorksheetLayout layout in _tableLayouts)
      {
        if (layout.DataTable == null)
          continue; // dont save orphaned layouts

        try
        {
          var zipEntry = archiveToSaveTo.CreateEntry("TableLayouts/" + layout.Name + ".xml");
          using (var zs = zipEntry.OpenForWriting())
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
      foreach (Altaxo.Scripting.FitFunctionScript fit in _fitFunctionScripts)
      {
        try
        {
          var zipEntry = archiveToSaveTo.CreateEntry("FitFunctionScripts/" + fit.CreationTime.ToString() + ".xml");
          //ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
          //zippedStream.PutNextEntry(ZipEntry);
          //zippedStream.SetLevel(0);
          using (var zs = zipEntry.OpenForWriting())
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
        foreach (var folderProperty in _projectFolderProperties)
        {
          try
          {
            var zipEntry = archiveToSaveTo.CreateEntry("FolderProperties/" + folderProperty.Name + ".xml");
            //ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
            //zippedStream.PutNextEntry(ZipEntry);
            //zippedStream.SetLevel(0);
            using (var zs = zipEntry.OpenForWriting())
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
        var zipEntry = archiveToSaveTo.CreateEntry("DocumentInformation.xml");
        using (var zs = zipEntry.OpenForWriting())
        {
          info.BeginWriting(zs);
          info.AddValue("DocumentInformation", _documentInformation);
          info.EndWriting();
        }
      }
      //  Current.Console.WriteLine("Saving took {0} sec.", (DateTime.UtcNow - time1).TotalSeconds);

      if (errorText.Length != 0)
        throw new ApplicationException(errorText.ToString());

      return dictionary;
    }

    public void RestoreFromZippedFile(IProjectArchive zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info)
    {
      var errorText = new System.Text.StringBuilder();

      foreach (var zipEntry in zipFile.Entries)
      {
        try
        {
          if (zipEntry.FullName.StartsWith("Tables/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              string entryName = "TableData/" + zipEntry.FullName.Substring("Tables/".Length);
              if (zipFile.ContainsEntry(entryName))
              {
                info.PropertyDictionary[Altaxo.Data.DataColumnCollection.DeserialiationInfoProperty_DeferredDataDeserialization] = zipFile.GetEntryMemento(entryName);
              }
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("Table", null);
              if (readedobject is Altaxo.Data.DataTable dataTable)
              {
                _dataTables.Add(dataTable);
              }
              info.EndReading();
              info.PropertyDictionary.Remove(Altaxo.Data.DataColumnCollection.DeserialiationInfoProperty_DeferredDataDeserialization);
            }
          }
          else if (zipEntry.FullName.StartsWith("Graphs/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("Graph", null);
              if (readedobject is Graph.Gdi.GraphDocument)
                _graphs.Add((Graph.Gdi.GraphDocument)readedobject);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("Graphs3D/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("Graph", null);
              if (readedobject is Graph.Graph3D.GraphDocument)
                _graphs3D.Add((Graph.Graph3D.GraphDocument)readedobject);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("Texts/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("Text", null);
              if (readedobject is Text.TextDocument noteDoc)
                _textDocuments.Add(noteDoc);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("TableLayouts/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("WorksheetLayout", null);
              if (readedobject is Altaxo.Worksheet.WorksheetLayout)
                _tableLayouts.Add((Altaxo.Worksheet.WorksheetLayout)readedobject);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("FitFunctionScripts/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("FitFunctionScript", null);
              if (readedobject is Altaxo.Scripting.FitFunctionScript)
                _fitFunctionScripts.Add((Altaxo.Scripting.FitFunctionScript)readedobject);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("FolderProperties/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("FolderProperty", null);
              if (readedobject is Altaxo.Main.Properties.ProjectFolderPropertyDocument)
                _projectFolderProperties.Add((Altaxo.Main.Properties.ProjectFolderPropertyDocument)readedobject);
              info.EndReading();
            }
          }
          else if (zipEntry.FullName == "DocumentInformation.xml")
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("DocumentInformation", null);
              if (readedobject is DocumentInformation)
                _documentInformation = (DocumentInformation)readedobject;
              info.EndReading();
            }
          }
        }
        catch (Exception exc)
        {
          errorText.Append("Error deserializing ");
          errorText.Append(zipEntry.FullName);
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

    public Altaxo.Text.TextDocumentCollection TextDocumentCollection
    {
      get { return _textDocuments; }
    }

    public Altaxo.Worksheet.WorksheetLayoutCollection TableLayouts
    {
      get { return _tableLayouts; }
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
        if (!(_isDirty == value))
        {
          _isDirty = value;
          OnDirtyChanged();

          if (false == _isDirty) // announce that the dirty flag was reset to all childs
            EhSelfTunnelingEventHappened(DirtyResetEventArgs.Empty, true);
        }
      }
    }
    /// <summary>
    /// Clears the <see cref="IsDirty"/> flag in a more advanced manner,
    /// supporting the needs for late loading of data.
    /// It updates the data needed for deferred data loading before clearing the flag.
    /// </summary>
    /// <param name="archiveManager">The archive manager that currently manages the archive in which the project is stored.</param>
    /// <param name="entryNameToItemDictionary">A dictionary where the keys are the archive entry names that where used to store the project items that are the values. The dictionary contains only those project items that need further handling (e.g. late load handling).</param>
    public void ClearIsDirty(IProjectArchiveManager archiveManager, IDictionary<string, IProjectItem> entryNameToItemDictionary)
    {
      if (null != archiveManager && null != entryNameToItemDictionary)
      {
        foreach (var entry in entryNameToItemDictionary)
        {
          if (entry.Value is Altaxo.Data.DataTable table && entry.Key.StartsWith("TableData"))
          {
            table.DataColumns.DeferredDataMemento = new ProjectArchiveEntryMemento(entry.Key, archiveManager, archiveManager.FileOrFolderName);
          }
        }
      }
      IsDirty = false;
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
      var dt1 = new Altaxo.Data.DataTable(worksheetName);

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
      if (!_dataTables.Contains(table))
        _dataTables.Add(table);

      var layout = new Altaxo.Worksheet.WorksheetLayout(table);
      _tableLayouts.Add(layout);
      return layout;
    }

    public override IDocumentLeafNode GetChildObjectNamed(string name)
    {
      switch (name)
      {
        case "Tables":
          return _dataTables;

        case "Graphs":
          return _graphs;

        case "Graphs3D":
          return _graphs3D;

        case "Texts":
          return _textDocuments;

        case "TableLayouts":
          return _tableLayouts;

        case "FitFunctionScripts":
          return _fitFunctionScripts;

        case "FolderProperties":
          return _projectFolderProperties;

        case "ProjectFolders":
          return _projectFolders;
      }
      return null;
    }

    public override string GetNameOfChildObject(IDocumentLeafNode o)
    {
      if (null == o)
        return null;
      else if (object.ReferenceEquals(o, _dataTables))
        return "Tables";
      else if (object.ReferenceEquals(o, _graphs))
        return "Graphs";
      else if (object.ReferenceEquals(o, _graphs3D))
        return "Graphs3D";
      else if (object.ReferenceEquals(o, _textDocuments))
        return "Texts";
      else if (object.ReferenceEquals(o, _tableLayouts))
        return "TableLayouts";
      else if (object.ReferenceEquals(o, _fitFunctionScripts))
        return "FitFunctionScripts";
      else if (object.ReferenceEquals(o, _projectFolderProperties))
        return "FolderProperties";
      else if (object.ReferenceEquals(o, _projectFolders))
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

      if (null != _textDocuments)
        yield return new Main.DocumentNodeAndName(_textDocuments, () => _textDocuments = null, "Text");

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
        yield return typeof(Altaxo.Text.TextDocument);
        yield return typeof(Altaxo.Main.Properties.ProjectFolderPropertyDocument);
      }
    }

    public IEnumerable<IProjectItemCollection> ProjectItemCollections
    {
      get
      {
        yield return DataTableCollection;
        yield return GraphDocumentCollection;
        yield return Graph3DDocumentCollection;
        yield return TextDocumentCollection;
        yield return ProjectFolderProperties;
      }
    }

    /// <summary>
    /// Gets the collection for a certain project item type.
    /// </summary>
    /// <param name="type">The type (must be a type that implements <see cref="Altaxo.Main.IProjectItem"/>).</param>
    /// <returns>The collection in which items of this type are stored.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IProjectItemCollection GetCollectionForProjectItemType(System.Type type)
    {
      if (type == typeof(Altaxo.Data.DataTable))
        return DataTableCollection;
      else if (type == typeof(Altaxo.Graph.Gdi.GraphDocument))
        return GraphDocumentCollection;
      else if (type == typeof(Altaxo.Graph.Graph3D.GraphDocument))
        return Graph3DDocumentCollection;
      else if (type == typeof(Altaxo.Text.TextDocument))
        return TextDocumentCollection;
      else if (type == typeof(Altaxo.Main.Properties.ProjectFolderPropertyDocument))
        return ProjectFolderProperties;
      else
        throw new ArgumentOutOfRangeException(string.Format("Unknown type of project item: {0}, or no project item type", type));
    }

    /// <summary>
    /// Gets the root path for a given project item type.
    /// </summary>
    /// <param name="type">The type of project item.</param>
    /// <returns>The root path of this type of item.</returns>
    public AbsoluteDocumentPath GetRootPathForProjectItemType(System.Type type)
    {
      return AbsoluteDocumentPath.GetAbsolutePath(GetCollectionForProjectItemType(type));
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

      return GetRootPathForProjectItemType(item.GetType()).Append(item.Name);
    }

    /// <summary>
    /// Tests if the project item given in the argument is already contained in this document.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>True if the item is already contained in the document, otherwise false.</returns>
    /// <exception cref="System.ArgumentNullException">item</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">The type of item is not yet considered here.</exception>
    public bool ContainsItem(IProjectItem item)
    {
      if (null == item)
        throw new ArgumentNullException(nameof(item));

      var coll = GetCollectionForProjectItemType(item.GetType());

      return coll.TryGetValue(item.Name, out var foundItem) && object.ReferenceEquals(foundItem, item);
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
        throw new ArgumentNullException(nameof(item));

      if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propDoc)
      {
        if (!ProjectFolderProperties.Contains(propDoc.Name))
        {
          ProjectFolderProperties.Add(propDoc); // if not existing, then add the new property document
        }
        else
        {
          ProjectFolderProperties[propDoc.Name].PropertyBagNotNull.MergePropertiesFrom(propDoc.PropertyBag, true); // if existing, then merge the properties into the existing bag
        }
      }
      else
      {
        var coll = GetCollectionForProjectItemType(item.GetType());
        coll.Add(item);
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

      if (item is Altaxo.Main.Properties.ProjectFolderPropertyDocument propertyDoc)
      {
        if (!ProjectFolderProperties.ContainsAnyName(propertyDoc.Name))
        {
          ProjectFolderProperties.Add(propertyDoc); // if not existing, then add the new property document
        }
        else
        {
          ProjectFolderProperties[propertyDoc.Name].PropertyBagNotNull.MergePropertiesFrom(propertyDoc.PropertyBag, true); // if existing, then merge the properties into the existing bag
        }
      }
      else // normal case
      {
        var coll = GetCollectionForProjectItemType(item.GetType());

        if (item.Name == null || item.Name == string.Empty)
          item.Name = coll.FindNewItemName();
        else if (coll.ContainsAnyName(item.Name))
          item.Name = coll.FindNewItemName(item.Name);

        coll.Add(item);
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

      var coll = GetCollectionForProjectItemType(item.GetType());
      if (coll.Contains(item.Name))
        return coll[item.Name];

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
    public bool RemoveItem(IProjectItem item)
    {
      if (null == item)
        throw new ArgumentNullException(nameof(item));
      var coll = GetCollectionForProjectItemType(item.GetType());
      return coll.Remove(item);
    }

    #endregion Static functions
  }
}
