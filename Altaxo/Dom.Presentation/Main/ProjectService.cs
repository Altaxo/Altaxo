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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using Altaxo.AddInItems;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using Altaxo.Gui.Graph.Gdi.Viewing;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Services;
using Altaxo.Main.Services.Files;

namespace Altaxo.Main
{
  /// <summary>
  /// Handles administrative tasks concerning an Altaxo project.
  /// </summary>
  /// <remarks>This should be instantiated only once. You can reach the current project service
  /// by calling <see cref="Current.IProjectService" />.</remarks>
  public class ProjectService : Altaxo.Dom.ProjectServiceBase, IAltaxoProjectService
  {
    /// <summary>
    /// The currently open Altaxo project.
    /// </summary>
    public Altaxo.AltaxoDocument CurrentOpenProject
    {
      get
      {
        return (Altaxo.AltaxoDocument)_currentProject;
      }
    }

    /// <summary>
    /// Gets all possible file extensions that belong to an Altaxo project file.
    /// </summary>
    /// <value>
    /// The project extensions.
    /// </value>
    public override IEnumerable<string> ProjectFileExtensions
    {
      get
      {
        yield return ".axoprj";
      }
    }

    #region New Project

    /// <inheritdoc/>
    protected override IProject InternalCreateNewProject()
    {
      return new AltaxoDocument();
    }

    #endregion New Project

    #region Project saving

    /// <inheritdoc/>
    public override IDictionary<string, IProjectItem> SaveProjectAndWindowsState(IProjectArchive archiveToSaveTo, IProjectArchive archiveToCopyFrom)
    {
      var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
      var result = CurrentOpenProject.SaveToArchive(archiveToSaveTo, info, archiveToCopyFrom);

      if (!Current.Dispatcher.InvokeRequired)
        SaveWindowStateToArchive(archiveToSaveTo, info);

      return result;
    }

    /// <summary>
    /// Saves the state of the main window into an archive.
    /// </summary>
    /// <param name="zippedStream">The file stream of the zip file.</param>
    /// <param name="info">The serialization info used to serialize the state of the main window.</param>
    public void SaveWindowStateToArchive(Services.IProjectArchive zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
    {
      var exceptions = new List<Exception>();

      {
        // first, we save our own state
        var zipEntry = zippedStream.CreateEntry("Workbench/MainWindow.xml");
        try
        {
          using (var zipEntryStream = zipEntry.OpenForWriting())
          {
            info.BeginWriting(zipEntryStream);
            info.AddValue("MainWindow", Current.Workbench.CreateMemento());
            info.EndWriting();
          }
        }
        catch (Exception exc)
        {
          exceptions.Add(exc);
        }
      }

      // second, we save all workbench windows into the Workbench/Views
      var selectedViewsMemento = new Altaxo.Gui.Workbench.ViewStatesMemento();
      int i = 0;
      foreach (IViewContent ctrl in Current.Workbench.ViewContentCollection)
      {
        if (info.IsSerializable(ctrl.ModelObject))
        {
          i++;
          var entryName = "Workbench/Views/View" + i.ToString() + ".xml";
          if (ctrl.IsSelected)
            selectedViewsMemento.SelectedView_EntryName = entryName;
          var zipEntry = zippedStream.CreateEntry(entryName);
          try
          {
            using (var zipEntryStream = zipEntry.OpenForWriting())
            {
              info.BeginWriting(zipEntryStream);
              info.AddValue("ViewContentModel", ctrl.ModelObject);
              info.EndWriting();
            }
          }
          catch (Exception exc)
          {
            exceptions.Add(exc);
          }
        }
      }

      {
        // Save the states of the views
        var zipEntry = zippedStream.CreateEntry("Workbench/ViewStates.xml");
        try
        {
          using (var zipEntryStream = zipEntry.OpenForWriting())
          {
            info.BeginWriting(zipEntryStream);
            info.AddValue("ViewStates", selectedViewsMemento);
            info.EndWriting();
          }
        }
        catch (Exception exc)
        {
          exceptions.Add(exc);
        }
      }

      if (exceptions.Count > 0)
        throw exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions);
    }

    #endregion Project saving

    #region Project opening

    protected override IFileBasedProjectArchiveManager InternalCreateProjectArchiveManagerFromFileOrFolderLocation(PathName fileOrFolderName)
    {
      if (fileOrFolderName is FileName)
        return new ZipFileProjectArchiveManager();

      return null;
    }

    protected override void InternalLoadProjectAndWindowsStateFromArchive(IProjectArchive projectArchive)
    {
      var exceptions = new List<Exception>();

      var oldProject = CurrentOpenProject;

      if (null != oldProject)
        OnProjectChanged(new ProjectEventArgs(oldProject, oldProject.Name, ProjectEventKind.ProjectClosing));

      try
      {
        Current.Workbench.CloseAllViews();
      }
      catch (Exception exc)
      {
        exceptions.Add(exc);
      }

      try
      {
        SetCurrentProject(null, asUnnamedProject: false);
      }
      catch (Exception exc)
      {
        exceptions.Add(exc);
      }

      // Old project is now closed
      if (null != oldProject)
        OnProjectChanged(new ProjectEventArgs(oldProject, oldProject.Name, ProjectEventKind.ProjectClosed));

      // Now open new project

      OnProjectChanged(new ProjectEventArgs(null, projectArchive.FileName, ProjectEventKind.ProjectOpening));

      AltaxoDocument newdocument;
      Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info;

      try
      {
        newdocument = new AltaxoDocument();
        info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
      }
      catch (Exception exc)
      {
        exceptions.Add(exc);
        throw new AggregateException(exceptions);
      }

      try
      {
        using (var suspendToken = newdocument.SuspendGetToken())
        {
          newdocument.RestoreFromZippedFile(projectArchive, info);
        }
      }
      catch (Exception exc)
      {
        exceptions.Add(exc);
      }

      try
      {
        SetCurrentProject(newdocument, asUnnamedProject: false);

        RestoreWindowStateFromZippedFile(projectArchive, info, newdocument);
        info.AnnounceDeserializationEnd(newdocument, true); // Final call to deserialization end

        CurrentOpenProject.IsDirty = false;

        info.AnnounceDeserializationHasCompletelyFinished(); // Annonce completly finished deserialization, activate data sources of the Altaxo document

        OnProjectChanged(new ProjectEventArgs(CurrentOpenProject, projectArchive.FileName, ProjectEventKind.ProjectOpened));
      }
      catch (Exception exc)
      {
        exceptions.Add(exc);
      }

      if (exceptions.Count > 0)
        throw new AggregateException(exceptions);
    }

    /// <summary>
    /// Restores the state of the main window from a zipped Altaxo project file.
    /// </summary>
    /// <param name="zipFile">The zip file where the state file can be found into.</param>
    /// <param name="info">The deserialization info used to retrieve the data.</param>
    /// <param name="restoredDoc">The previously (also from the zip file!) restored Altaxo document.</param>
    public void RestoreWindowStateFromZippedFile(Services.IProjectArchive zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info, AltaxoDocument restoredDoc)
    {
      var restoredDocModels = new List<(object Document, string ZipEntryName)>();
      var restoredPadModels = new List<object>();
      Altaxo.Gui.Workbench.ViewStatesMemento selectedViewsMemento = null;

      foreach (var zipEntry in zipFile.Entries)
      {
        try
        {
          if (zipEntry.FullName.StartsWith("Workbench/Views/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("ViewContentModel", null);
              restoredDocModels.Add((readedobject, zipEntry.FullName));
              info.EndReading();
            }
          }
          else if (zipEntry.FullName.StartsWith("Workbench/Pads/"))
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              object readedobject = info.GetValue("Model", null);
              if (readedobject != null)
              {
                restoredPadModels.Add(readedobject);
              }
              else
              {
              }
              info.EndReading();
            }
          }
          else if (zipEntry.FullName == "Workbench/ViewStates.xml")
          {
            using (var zipinpstream = zipEntry.OpenForReading())
            {
              info.BeginReading(zipinpstream);
              selectedViewsMemento = info.GetValue("ViewStates", null) as Altaxo.Gui.Workbench.ViewStatesMemento;
              info.EndReading();
            }
          }
        }
        catch (Exception ex)
        {
          Current.Console.WriteLine("Exception during serialization of {0}, message: {1}", zipEntry.FullName, ex.Message);
        }
      }

      info.AnnounceDeserializationEnd(restoredDoc, false);
      info.AnnounceDeserializationEnd(restoredDoc, false);

      // now give all restored controllers a view and show them in the Main view

      foreach ((object doc, string entryName) in restoredDocModels)
      {
        var isSelected = entryName == selectedViewsMemento?.SelectedView_EntryName;
        Current.Workbench.ShowView(doc, isSelected);
      }

      foreach (var o in restoredPadModels)
      {
        var content = (IPadContent)Current.Gui.GetControllerAndControl(new object[] { o }, typeof(IPadContent));
        if (null != content)
          Current.Workbench.ShowPad(content, false);
      }
    }

    public override void ExecuteActionsImmediatelyBeforeRunningApplication(string[] cmdArgs, string[] cmdParameter, string[] cmdFiles)
    {
      foreach (var file in cmdFiles)
      {
        if (System.IO.File.Exists(file))
          LoadProjectFromFileOrFolder(FileName.Create(file));
        else if (System.IO.Directory.Exists(file))
          LoadProjectFromFileOrFolder(DirectoryName.Create(file));
        break;
      }

      for (int i = 0; i < cmdArgs.Length; ++i)
      {
        var lowerarg = cmdArgs[i].ToLowerInvariant();
        if ((lowerarg == "-executetablescript" || lowerarg == "/executetablescript") && i < cmdArgs.Length - 1)
        {
          ExecuteTableScriptOfTable(cmdArgs[i + 1]);
          ++i; // switch over the next argument, since it is then already processed
        }
      }
    }

    private void ExecuteTableScriptOfTable(string tableName)
    {
      if (Current.Project.DataTableCollection.TryGetValue(tableName, out var table))
      {
        if (null != table.TableScript)
        {
          table.TableScript.Execute(table, new DummyBackgroundMonitor());
        }
      }
    }

    #endregion Project opening

    #region Opening of project document files

    private static readonly HashSet<string> ProjectDocumentExtensions = new HashSet<string>()
      {
      ".axowks", // Altaxo worksheet
			".axogrp" // Altaxo graph
			};

    /// <inheritdoc/>
    public override bool TryOpenProjectItemFile(FileName fileName, bool forceTrialRegardlessOfExtension)
    {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      if (!forceTrialRegardlessOfExtension)
      {
        var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

        if (!ProjectDocumentExtensions.Contains(extension))
          return false;
      }

      object deserializedObject;

      using (System.IO.Stream myStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
      {
        using (var info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo())
        {
          try
          {
            info.BeginReading(myStream);
            deserializedObject = info.GetValue("Table", null);
            info.EndReading();
            myStream.Close();
          }
          catch (Exception ex)
          {
            return false;
          }

          if (deserializedObject is IProjectItem projectItem)
          {
            Current.Project.AddItemWithThisOrModifiedName(projectItem);
            info.AnnounceDeserializationEnd(Current.Project, false); // fire the event to resolve path references
            Current.ProjectService.OpenOrCreateViewContentForDocument(projectItem);
          }
          else if (deserializedObject is Altaxo.Worksheet.TablePlusLayout tableAndLayout)
          {
            var table = tableAndLayout.Table;
            Current.Project.AddItemWithThisOrModifiedName(table);

            if (tableAndLayout.Layout != null)
              Current.Project.TableLayouts.Add(tableAndLayout.Layout);

            tableAndLayout.Layout.DataTable = table; // this is the table for the layout now

            info.AnnounceDeserializationEnd(Current.Project, false); // fire the event to resolve path references

            Current.ProjectService.CreateNewWorksheet(table, tableAndLayout.Layout);
          }
          else
          {
            return false;
          }

          info.AnnounceDeserializationEnd(Current.Project, true); // final deserialization end
          return true;
        }
      }
    }

    #endregion Opening of project document files



    #region IProjectItem functions

    /// <summary>
    /// Creates a project item, and add it to the appropriate collection in the current project.
    /// Note that there might exist more specialized function to create a certain project item.
    /// </summary>
    /// <typeparam name="T">The type of project item to create.</typeparam>
    /// <param name="inFolder">The folder into which the project item is created.</param>
    /// <returns>The created project item.</returns>
    public T CreateDocument<T>(string inFolder) where T : IProjectItem
    {
      var collection = CurrentOpenProject.GetCollectionForProjectItemType(typeof(T));
      var itemName = collection.FindNewItemNameInFolder(inFolder);
      if (collection.Contains(itemName))
      {
        return (T)collection[itemName];
      }
      else
      {
        var projectItem = (T)System.Activator.CreateInstance(typeof(T));
        projectItem.Name = itemName;
        collection.Add(projectItem);
        return projectItem;
      }
    }

    /// <summary>
    /// This function will delete a project item and close the corresponding views.
    /// </summary>
    /// <param name="document">The project item to delete</param>
    /// <param name="force">If true, the document is deleted without safety question,
    /// if false, the user is ask before the document is deleted.</param>
    public override void DeleteDocument(Main.IProjectItem document, bool force)
    {
      if (null == document)
        throw new ArgumentNullException(nameof(document));

      Current.Dispatcher.InvokeIfRequired(DeleteDocument_Unsynchronized, document, force);
    }

    private void DeleteDocument_Unsynchronized(Main.IProjectItem document, bool force)
    {
      if (!force &&
        false == Current.Gui.YesNoMessageBox(string.Format("Are you sure to remove the {0} and the corresponding views?", document.GetType().Name), "Attention", false))
        return;

      // close all windows
      var foundContent = Current.Workbench.GetViewModels<IViewContent>(document);
      foreach (IViewContent content in foundContent.ToArray()) // ToArray because we change the collection by closing
      {
        if (content.CloseCommand.CanExecute(true))
          content.CloseCommand.Execute(true);
      }

      Current.Project.RemoveItem(document);

      // the following sequence is related to a bug encountered when closing a tabbed window by the program:
      // the active view content is not updated because the dockpanel lost the focus
      // to circumvent this, we focus on a new viewcontent, in this case the first one
      SelectFirstAvailableView();
    }

    /// <summary>
    /// Opens a view that shows the <paramref name="document"/>. If no view for the document can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="document">The document for which a view must be found. This parameter must not be null.</param>
    /// <returns>The view content for the provided document.</returns>
    public object OpenOrCreateViewContentForDocument(IProjectItem document)
    {
      if (null == document)
        throw new ArgumentNullException(nameof(document));

      return Current.Dispatcher.InvokeIfRequired(OpenOrCreateViewContentForDocument_Unsynchronized, document);
    }

    private object OpenOrCreateViewContentForDocument_Unsynchronized(IProjectItem document)
    {
      // make sure the document is contained in our current project
      if (!CurrentOpenProject.ContainsItem(document))
        CurrentOpenProject.AddItemWithThisOrModifiedName(document);

      // if a content exist that show that graph, activate that content
      var foundContent = GetViewContentsForDocument(document).FirstOrDefault();
      if (foundContent != null)
      {
        foundContent.IsActive = true;
        foundContent.IsSelected = true;
        return foundContent;
      }
      else // not found
      {
        return CreateNewViewContent_Unsynchronized(document);
      }
    }

    private IMVCController CreateNewViewContent_Unsynchronized(IProjectItem document)
    {
      if (document == null)
        throw new ArgumentNullException(nameof(document));

      // make sure that the item is already contained in the project
      if (!Current.Project.ContainsItem(document))
        Current.Project.AddItemWithThisOrModifiedName(document);

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(AbstractViewContent));
      System.Reflection.ConstructorInfo cinfo = null;
      object viewContent = null;
      foreach (Type type in types)
      {
        if (null != (cinfo = type.GetConstructor(new Type[] { document.GetType() })))
        {
          var par = cinfo.GetParameters()[0];
          if (par.ParameterType != typeof(object)) // ignore view content which takes the most generic type
          {
            viewContent = cinfo.Invoke(new object[] { document });
            break;
          }
        }
      }

      if (null == viewContent)
      {
        foreach (IProjectItemDisplayBindingDescriptor descriptor in AddInTree.BuildItems<IProjectItemDisplayBindingDescriptor>("/Altaxo/Workbench/ProjectItemDisplayBindings", this, false))
        {
          if (descriptor.ProjectItemType == document.GetType())
          {
            if (null != (cinfo = descriptor.ViewContentType.GetConstructor(new Type[] { document.GetType() })))
            {
              var par = cinfo.GetParameters()[0];
              if (par.ParameterType != typeof(object)) // ignore view content which takes the most generic type
              {
                viewContent = cinfo.Invoke(new object[] { document });
                break;
              }
            }
          }
        }
      }

      var controller = viewContent as IMVCController;

      if (null != controller && controller.ViewObject == null)
      {
        Current.Gui.FindAndAttachControlTo(controller);
      }

      if (null != Current.Workbench && null != viewContent)
        Current.Workbench.ShowView(viewContent, true);

      return controller;
    }

    #endregion IProjectItem functions

    #region Worksheet functions

    /// <summary>
    /// Creates a table and the view content for that table.
    /// </summary>
    /// <param name="worksheetName">The name of the table to create.</param>
    /// <param name="bCreateDefaultColumns">If true, a default x and a y column is created.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(string worksheetName, bool bCreateDefaultColumns)
    {
      Altaxo.Data.DataTable dt1 = CurrentOpenProject.CreateNewTable(worksheetName, bCreateDefaultColumns);
      return CreateNewWorksheet(dt1);
    }

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="bCreateDefaultColumns">If true, a default x column and a default value column are created in the table.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(bool bCreateDefaultColumns)
    {
      return CreateNewWorksheet(CurrentOpenProject.DataTableCollection.FindNewItemName(), bCreateDefaultColumns);
    }

    /// <summary>
    /// Creates a new table in the root folder and the view content for that table.
    /// </summary>
    /// <returns>The content controller for that table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet()
    {
      return CreateNewWorksheetInFolder(Main.ProjectFolder.RootFolderName);
    }

    /// <summary>
    /// Creates a new table in a specified folder and the view content for that table.
    /// </summary>
    /// <returns>The content controller for that table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheetInFolder(string folder)
    {
      return CreateNewWorksheet(CurrentOpenProject.DataTableCollection.FindNewItemNameInFolder(folder), false);
    }

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewWorksheet_Unsynchronized, table);
    }

    private Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet_Unsynchronized(Altaxo.Data.DataTable table)
    {
      if (table.ParentObject == null)
        CurrentOpenProject.DataTableCollection.Add(table);

      return CreateNewWorksheet(table, CurrentOpenProject.CreateNewTableLayout(table));
    }

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <param name="layout">The layout for the table.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewWorksheet_Unsynchronized, table, layout);
    }

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <param name="layout">The layout for the table.</param>
    /// <returns>The view content for the provided table.</returns>
    private Altaxo.Gui.Worksheet.Viewing.IWorksheetController CreateNewWorksheet_Unsynchronized(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
    {
      layout.DataTable = table;
      var ctrl = new Altaxo.Gui.Worksheet.Viewing.WorksheetController(layout);
      var view = new Altaxo.Gui.Worksheet.Viewing.WorksheetViewWpf();
      ctrl.ViewObject = view;

      if (null != Current.Workbench)
        Current.Workbench.ShowView(ctrl, true);

      return ctrl;
    }

    /// <summary>
    /// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
    /// a new default view is created for the table.
    /// </summary>
    /// <param name="table">The table for which a view must be found.</param>
    /// <returns>The view content for the provided table.</returns>
    public object OpenOrCreateWorksheetForTable(Altaxo.Data.DataTable table)
    {
      return Current.Dispatcher.InvokeIfRequired(OpenOrCreateWorksheetForTable_Unsynchronized, table);
    }

    /// <summary>
    /// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
    /// a new default view is created for the table.
    /// </summary>
    /// <param name="table">The table for which a view must be found.</param>
    /// <returns>The view content for the provided table.</returns>
    private object OpenOrCreateWorksheetForTable_Unsynchronized(Altaxo.Data.DataTable table)
    {
      // if a content exist that show that table, activate that content
      var foundContent = GetViewContentsForDocument(table).FirstOrDefault();
      if (foundContent != null)
      {
        foundContent.IsVisible = true;
        foundContent.IsActive = true;
        return foundContent;
      }

      // otherwise create a new Worksheet
      return CreateNewWorksheet_Unsynchronized(table);
    }

    /// <summary>
    /// This function will delete a data table and close the corresponding views.
    /// </summary>
    /// <param name="table">The data table to delete</param>
    /// <param name="force">If true, the table is deleted without safety question,
    /// if false, the user is ask before the table is deleted.</param>
    public void DeleteTable(Altaxo.Data.DataTable table, bool force)
    {
      Current.Dispatcher.InvokeIfRequired(DeleteTable_Unsynchronized, table, force);
    }

    private void DeleteTable_Unsynchronized(Altaxo.Data.DataTable table, bool force)
    {
      if (!force &&
        false == Current.Gui.YesNoMessageBox("Are you sure to remove the table and the corresponding views?", "Attention", false))
        return;

      // close all windows
      foreach (IViewContent content in GetViewContentsForDocument(table).ToArray())
      {
        Current.Workbench.CloseContent(content);
      }

      Current.Project.DataTableCollection.Remove(table);

      // the following sequence is related to a bug encountered when closing a tabbed window by the program:
      // the active view content is not updated because the dockpanel lost the focus
      // to circumvent this, we focus on a new viewcontent, in this case the first one
      SelectFirstAvailableView();
    }

    /// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
    /// <param name="ctrl">The Worksheet to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
    public void RemoveWorksheet(Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl)
    {
      Current.Dispatcher.InvokeIfRequired(RemoveWorksheet_Unsynchronized, ctrl);
    }

    private void RemoveWorksheet_Unsynchronized(Altaxo.Gui.Worksheet.Viewing.IWorksheetController ctrl)
    {
      if (ctrl is IViewContent content)
        Current.Workbench.CloseContent(content);
    }

    #endregion Worksheet functions

    #region common graph functions

    /// <summary>
    /// Gets an exporter that can be used to export an image of the provided project item.
    /// </summary>
    /// <param name="item">The item to export, for instance an item of type <see cref="Altaxo.Graph.Gdi.GraphDocument"/> or <see cref="Altaxo.Graph.Graph3D.GraphDocument"/>.</param>
    /// <returns>The image exporter class that can be used to export the item in graphical form, or null if no exporter could be found.</returns>
    public IProjectItemImageExporter GetProjectItemImageExporter(IProjectItem item)
    {
      IProjectItemImageExporter result = null;

      foreach (IProjectItemExportBindingDescriptor descriptor in AddInTree.BuildItems<IProjectItemExportBindingDescriptor>("/Altaxo/Workbench/ProjectItemExportBindings", this, false))
      {
        if (descriptor.ProjectItemType == item.GetType())
        {
          System.Reflection.ConstructorInfo cinfo;
          if (null != (cinfo = descriptor.GraphicalExporterType.GetConstructor(new Type[0])))
          {
            result = cinfo.Invoke(new object[0]) as IProjectItemImageExporter;
            if (null != result)
              break;
          }
        }
      }

      return result;
    }

    #endregion common graph functions

    #region Graph functions

    /// <summary>
    /// Creates a new graph document and the view content..
    /// </summary>
    /// <returns>The view content for the newly created graph.</returns>
    public IGraphController CreateNewGraph()
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewGraph_Unsynchronized);
    }

    private IGraphController CreateNewGraph_Unsynchronized()
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
          PropertyExtensions.GetPropertyContextOfProjectFolder(ProjectFolder.RootFolderName), "GRAPH", ProjectFolder.RootFolderName, false);
      return CreateNewGraph_Unsynchronized(graph);
    }

    /// <summary>
    /// Creates a new graph document in a specified folder and the view content.
    /// </summary>
    /// <param name="folderName">The folder name where to create the graph.</param>
    /// <returns>The view content for the newly created graph.</returns>
    public IGraphController CreateNewGraphInFolder(string folderName)
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewGraphInFolder_Unsynchronized, folderName);
    }

    private IGraphController CreateNewGraphInFolder_Unsynchronized(string folderName)
    {
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
        PropertyExtensions.GetPropertyContextOfProjectFolder(folderName), null, folderName, false);

      return CreateNewGraph_Unsynchronized(graph);
    }

    /// <summary>
    /// Creates a new graph document and the view content..
    /// </summary>
    /// <param name="preferredName">The preferred name the new graph document should have.</param>
    /// <returns>The view content for the newly created graph.</returns>
    public IGraphController CreateNewGraph(string preferredName)
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewGraph_Unsynchronized, preferredName);
    }

    private IGraphController CreateNewGraph_Unsynchronized(string preferredName)
    {
      var folderName = Main.ProjectFolder.GetFolderPart(preferredName);
      var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
        PropertyExtensions.GetPropertyContextOfProjectFolder(folderName), preferredName, folderName, false);
      return CreateNewGraph_Unsynchronized(graph);
    }

    /// <summary>
    /// Creates a new view content for a graph document.
    /// </summary>
    /// <param name="graph">The graph document.</param>
    /// <returns>The view content for the provided graph document.</returns>
    public IGraphController CreateNewGraph(Altaxo.Graph.Gdi.GraphDocument graph)
    {
      return Current.Dispatcher.InvokeIfRequired(CreateNewGraph_Unsynchronized, graph);
    }

    private IGraphController CreateNewGraph_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph)
    {
      if (graph == null)
        graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(
          PropertyExtensions.GetPropertyContextOfProjectFolder(ProjectFolder.RootFolderName), null, ProjectFolder.RootFolderName, false);

      return (IGraphController)OpenOrCreateViewContentForDocument_Unsynchronized(graph);
    }

    /// <summary>
    /// Opens a view that shows the graph <code>graph</code>. If no view for the graph can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="graph">The graph for which a view must be found.</param>
    /// <returns>The view content for the provided graph.</returns>
    public object OpenOrCreateGraphForGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph)
    {
      return Current.Dispatcher.InvokeIfRequired(OpenOrCreateGraphForGraphDocument_Unsynchronized, graph);
    }

    private object OpenOrCreateGraphForGraphDocument_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph)
    {
      // if a content exist that show that graph, activate that content
      var foundContent = GetViewContentsForDocument(graph).FirstOrDefault();
      if (foundContent != null)
      {
        foundContent.IsActive = true;
        foundContent.IsSelected = true;
        return foundContent;
      }

      // otherwise create a new graph view
      return CreateNewGraph_Unsynchronized(graph);
    }

    /// <summary>
    /// This function will delete a graph document and close all corresponding views.
    /// </summary>
    /// <param name="graph">The graph document to delete.</param>
    /// <param name="force">If true, the graph document is deleted without safety question,
    /// if false, the user is ask before the graph document is deleted.</param>
    public void DeleteGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph, bool force)
    {
      Current.Dispatcher.InvokeIfRequired(DeleteGraphDocument_Unsynchronized, graph, force);
    }

    private void DeleteGraphDocument_Unsynchronized(Altaxo.Graph.Gdi.GraphDocument graph, bool force)
    {
      if (!force &&
        false == Current.Gui.YesNoMessageBox("Are you sure to remove the graph document and the corresponding views?", "Attention", false))
        return;

      // close all windows
      foreach (IViewContent content in GetViewContentsForDocument(graph).ToArray())
      {
        Current.Workbench.CloseContent(content);
      }

      Current.Project.GraphDocumentCollection.Remove(graph);

      // the following sequence is related to a bug encountered when closing a tabbed window by the program:
      // the active view content is not updated because the dockpanel lost the focus
      // to circumvent this, we focus on a new viewcontent, in this case the first one
      SelectFirstAvailableView();
    }

    /// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
    /// <param name="ctrl">The GraphController to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
    public void RemoveGraph(IGraphController ctrl)
    {
      Current.Dispatcher.InvokeIfRequired(RemoveGraph_Unsynchronized, ctrl);
    }

    private void RemoveGraph_Unsynchronized(IGraphController ctrl)
    {
      if (ctrl is IViewContent content)
        Current.Workbench.CloseContent(content);
    }

    /// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
    /// <param name="ctrl">The GraphController to remove.</param>
    /// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
    public void RemoveGraph3D(Altaxo.Gui.Graph.Graph3D.Viewing.IGraphController ctrl)
    {
      Current.Dispatcher.InvokeIfRequired(RemoveGraph_Unsynchronized, ctrl);
    }

    private void RemoveGraph_Unsynchronized(Altaxo.Gui.Graph.Graph3D.Viewing.IGraphController ctrl)
    {
      if (ctrl is IViewContent content)
        Current.Workbench.CloseContent(content);
    }

    #endregion Graph functions

    #region Graph3D functions

    /// <summary>
    /// Creates a new graph document and the view content..
    /// </summary>
    /// <returns>The view content for the newly created graph.</returns>
    public Altaxo.Gui.Graph.Graph3D.Viewing.IGraphController CreateNewGraph3D(Altaxo.Graph.Graph3D.GraphDocument doc)
    {
      if (null == doc)
      {
        doc = Altaxo.Graph.Graph3D.Templates.TemplateWithXYZPlotLayerWithG3DCartesicCoordinateSystem.CreateGraph(
            PropertyExtensions.GetPropertyContextOfProjectFolder(ProjectFolder.RootFolderName), "GRAPH", ProjectFolder.RootFolderName, false);
      }

      return (Altaxo.Gui.Graph.Graph3D.Viewing.IGraphController)CreateNewViewContent_Unsynchronized(doc);
    }

    #endregion Graph3D functions

    private void EhProjectDirtyChanged(object sender, EventArgs e)
    {
      OnProjectChanged(new Altaxo.Main.ProjectEventArgs(_currentProject, _currentProject?.Name, ProjectEventKind.ProjectDirtyChanged));
    }

    //********* own events

    private void SelectFirstAvailableView()
    {
      var content = Current.Workbench.ViewContentCollection.FirstOrDefault();

      if (null != content)
      {
        content.IsActive = true;
        content.IsSelected = true;
      }
    }
  }
}
