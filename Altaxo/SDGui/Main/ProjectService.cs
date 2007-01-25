#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpZipLib.Zip;

using Altaxo.Gui;

namespace Altaxo.Main
{



  /// <summary>
  /// Handles administrative tasks concerning an Altaxo project.
  /// </summary>
  /// <remarks>This should be instantiated only once. You can reach the current project service
  /// by calling <see cref="Current.ProjectService" />.</remarks>
  public class ProjectService : IProjectService
  {
    /// <summary>
    /// The currently opened Altaxo project.
    /// </summary>
    Altaxo.AltaxoDocument openProject = null;

    /// <summary>
    /// The file name of the currently opened Altaxo project.
    /// </summary>
    string openProjectFileName = null;


    //FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
    //ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));


    /// <summary>
    /// The currently open Altaxo project.
    /// </summary>
    public Altaxo.AltaxoDocument CurrentOpenProject
    {
      get
      {
        return openProject;
      }
      set
      {
        Altaxo.AltaxoDocument oldProject = openProject;

        if (oldProject != null)
          oldProject.DirtyChanged -= new EventHandler(this.EhProjectDirtyChanged);


        openProject = value;

        if (openProject != null)
          openProject.DirtyChanged += new EventHandler(this.EhProjectDirtyChanged);


        if (!object.Equals(oldProject, value))
          OnProjectChanged();
      }
    }

    /// <summary>
    /// The name of the currently open project. Is either the file name or the untitled name.
    /// </summary>
    public string CurrentProjectFileName
    {
      get { return this.openProjectFileName; }
    }



    /// <summary>
    /// Saves the state of the main window into a zipped file.
    /// </summary>
    /// <param name="zippedStream">The file stream of the zip file.</param>
    /// <param name="info">The serialization info used to serialize the state of the main window.</param>
    public void SaveWindowStateToZippedFile(ICompressedFileContainerStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
    {
      System.Text.StringBuilder errorText = new System.Text.StringBuilder();

      {
        // first, we save our own state 
        zippedStream.StartFile("Workbench/MainWindow.xml", 0);
        try
        {
          info.BeginWriting(zippedStream.Stream);
          info.AddValue("MainWindow", Current.Workbench);
          info.EndWriting();
        }
        catch (Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }

      // second, we save all workbench windows into the Workbench/Views 
      int i = 0;
      foreach (IViewContent ctrl in Current.Workbench.ViewContentCollection)
      {
        if (info.IsSerializable(ctrl))
        {
          i++;
          zippedStream.StartFile("Workbench/Views/View" + i.ToString() + ".xml", 0);
          try
          {
            info.BeginWriting(zippedStream.Stream);
            info.AddValue("WorkbenchViewContent", ctrl);
            info.EndWriting();
          }
          catch (Exception exc)
          {
            errorText.Append(exc.ToString());
          }
        }
      }

      if (errorText.Length != 0)
        throw new ApplicationException(errorText.ToString());
    }


    /// <summary>
    /// Restores the state of the main window from a zipped Altaxo project file.
    /// </summary>
    /// <param name="zipFile">The zip file where the state file can be found into.</param>
    /// <param name="info">The deserialization info used to retrieve the data.</param>
    /// <param name="restoredDoc">The previously (also from the zip file!) restored Altaxo document.</param>
    public void RestoreWindowStateFromZippedFile(ZipFile zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info, AltaxoDocument restoredDoc)
    {
      System.Collections.ArrayList restoredControllers = new System.Collections.ArrayList();
      foreach (ZipEntry zipEntry in zipFile)
      {
        if (!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Workbench/Views/"))
        {
          System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
          info.BeginReading(zipinpstream);
          object readedobject = info.GetValue("Table", this);
          if (readedobject is ICSharpCode.SharpDevelop.Gui.IViewContent)
            restoredControllers.Add(readedobject);
          info.EndReading();
        }
      }

      info.AnnounceDeserializationEnd(restoredDoc);
      info.AnnounceDeserializationEnd(this);

      // now give all restored controllers a view and show them in the Main view

      foreach (IViewContent viewcontent in restoredControllers)
      {
        IMVCControllerWrapper wrapper = viewcontent as IMVCControllerWrapper;
        if (wrapper != null && wrapper.MVCController.ViewObject==null)
          Current.Gui.FindAndAttachControlTo(wrapper.MVCController);

        if (viewcontent.Control != null)
        {
          Current.Workbench.ShowView(viewcontent);
        }
      }

    }

    /// <summary>
    /// Opens a Altaxo project. If the current project is dirty, the user is ask for saving the current project.
    /// </summary>
    /// <param name="filename"></param>
    public void OpenProject(string filename)
    {
      if (CurrentOpenProject != null)
      {
        System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
        if (this.CurrentOpenProject.IsDirty)
          AskForSavingOfProject(e);

        if (e.Cancel == true)
          return;


        CloseProject();
      }

      if (!FileUtility.TestFileExists(filename))
      {
        return;
      }
      StatusBarService.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");

      if (Path.GetExtension(filename).ToUpper() == ".AXOPRJ")
      {
        string validproject = Path.ChangeExtension(filename, ".axoprj");
        if (File.Exists(validproject))
        {
          LoadProject(validproject);
        }

      }
      else
      {
        LoadProject(filename);
      }

      StatusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
    }

    /// <summary>
    /// Loads a existing Altaxo project with the provided name.
    /// </summary>
    /// <param name="filename">The file name of the project to load.</param>
    void LoadProject(string filename)
    {
      if (!FileUtility.TestFileExists(filename))
      {
        return;
      }

      this.Load(filename);
      openProjectFileName = filename;

      //IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
      FileService.RecentOpen.AddLastProject(filename);

      OnProjectOpened(new ProjectEventArgs(openProject));

      // RestoreCombinePreferences(CurrentOpenCombine, openCombineFileName);
    }


    /// <summary>
    /// Opens a Altaxo project from a project file (without asking the user).
    /// </summary>
    /// <param name="filename"></param>
    private void Load(string filename)
    {
      System.Text.StringBuilder errorText = new System.Text.StringBuilder();

      System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
      ZipFile zipFile = new ZipFile(myStream);
      Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
      AltaxoDocument newdocument = new AltaxoDocument();
      ZipFileWrapper zipFileWrapper = new ZipFileWrapper(zipFile);

      try
      {
        newdocument.RestoreFromZippedFile(zipFileWrapper, info);
      }
      catch (Exception exc)
      {
        errorText.Append(exc.ToString());
      }

      try
      {
        Current.Workbench.CloseAllViews();
        this.CurrentOpenProject = newdocument;
        RestoreWindowStateFromZippedFile(zipFile, info, newdocument);
        this.CurrentOpenProject.IsDirty = false;
      }
      catch (Exception exc)
      {
        errorText.Append(exc.ToString());
        System.Windows.Forms.MessageBox.Show(Current.MainWindow, errorText.ToString(), "An error occured");
      }
      finally
      {
        myStream.Close();
      }


      if (errorText.Length != 0)
        throw new ApplicationException(errorText.ToString());
    }

    /// <summary>
    /// Internal routine to save a project under a given name.
    /// </summary>
    /// <param name="filename"></param>
    private void Save(string filename)
    {
      Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();

      bool fileAlreadyExists = System.IO.File.Exists(filename);



      System.IO.Stream myStream;
      string tempFileName = null;
      if (fileAlreadyExists)
      {
        tempFileName = System.IO.Path.GetTempFileName();
        myStream = new System.IO.FileStream(tempFileName, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
      }
      else
        myStream = new System.IO.FileStream(filename, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);

      ZipOutputStream zippedStream = new ZipOutputStream(myStream);
      ZipOutputStreamWrapper zippedStreamWrapper = new ZipOutputStreamWrapper(zippedStream);

      Exception savingException = null;
      try
      {
        this.openProject.SaveToZippedFile(zippedStreamWrapper, info);
        SaveWindowStateToZippedFile(zippedStreamWrapper, info);
      }
      catch (Exception exc)
      {
        savingException = exc;
      }

      zippedStream.Close();
      myStream.Close();

      try
      {
        if (savingException == null)
        {
          // Test the file for integrity
          string testfilename = tempFileName != null ? tempFileName : filename;
          myStream = new System.IO.FileStream(testfilename, System.IO.FileMode.Open, FileAccess.Read, FileShare.None);
          ZipFile zipFile = new ZipFile(myStream);
          foreach (ZipEntry zipEntry in zipFile)
          {
            if (!zipEntry.IsDirectory)
            {
              System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
            }
          }
          zipFile.Close();
          // end test
        }
      }
      catch (Exception exc)
      {
        savingException = exc;
      }

      // now, if no exception happened, copy the temporary file back to the original file
      if (null != tempFileName && null == savingException)
      {
        System.IO.File.Copy(tempFileName, filename, true);
        System.IO.File.Delete(tempFileName);
      }


      if (null != savingException)
        throw savingException;

      this.openProject.IsDirty = false;
    }

    /// <summary>
    /// Saves a project under the current file name.
    /// </summary>
    public void SaveProject()
    {
      SaveProject(openProjectFileName);
    }

    /// <summary>
    /// Saves the current project under a provided file name. If the provided file name differs
    /// from the current file name, a project renaming event is triggered.
    /// </summary>
    /// <param name="filename">The new project file name.</param>
    public void SaveProject(string filename)
    {
      string oldFileName = this.openProjectFileName;
      this.openProjectFileName = filename;
      if (oldFileName != filename)
        this.OnRenameProject(new ProjectRenameEventArgs(this.openProject, oldFileName, filename));


      FileUtility.ObservedSave(new NamedFileOperationDelegate(this.Save),
        filename,
        ResourceService.GetString("Altaxo.Project.CantSaveProjectErrorText"),
        FileErrorPolicy.ProvideAlternative);
    }

    /// <summary>
    /// Asks the user for a file name for the current project, and then saves the project under the given name.
    /// </summary>
    public void SaveProjectAs()
    {
      SaveFileDialog fdiag = new SaveFileDialog();
      fdiag.OverwritePrompt = true;
      fdiag.AddExtension = true;


      fdiag.Filter = StringParser.Parse("${res:Altaxo.FileFilter.ProjectFiles}|*.axoprj|${res:Altaxo.FileFilter.AllFiles}|*.*");

      if (fdiag.ShowDialog() == DialogResult.OK)
      {
        string filename = fdiag.FileName;
        SaveProject(filename);
        FileService.RecentOpen.AddLastProject(filename);
        StatusBarService.SetMessage(filename + ": " + ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
        //MessageService.ShowMessage(filename, ResourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
      }
    }

    /// <summary>
    /// Asks the user whether or not the project should be saved, and saves it in case the user answers with yes.
    /// </summary>
    /// <param name="e">Cancel event args. On return, the e.Cancel property is set to true, if the users cancel the saving.</param>
    public virtual void AskForSavingOfProject(System.ComponentModel.CancelEventArgs e)
    {
      string text = ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Text");
      string caption = ResourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Caption");
      System.Windows.Forms.DialogResult dlgresult = System.Windows.Forms.MessageBox.Show(Current.MainWindow, text, caption, System.Windows.Forms.MessageBoxButtons.YesNoCancel);

      switch (dlgresult)
      {
        case System.Windows.Forms.DialogResult.Yes:
          if (this.CurrentProjectFileName != null)
            this.SaveProject();
          else
            this.SaveProjectAs();

          if (this.CurrentOpenProject.IsDirty)
            e.Cancel = true; // Cancel if the saving was not successfull
          break;

        case System.Windows.Forms.DialogResult.No:
          break;
        case System.Windows.Forms.DialogResult.Cancel:
          e.Cancel = true; // Cancel if the user pressed cancel
          break;
      }

    }


    /// <summary>
    /// Closes a project. If the project is dirty, the user is asked for saving the project.
    /// </summary>
    public void CloseProject()
    {

      if (CurrentOpenProject != null)
      {
        System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
        if (this.CurrentOpenProject.IsDirty)
          AskForSavingOfProject(e);

        if (e.Cancel == false)
        {
          //if (saveCombinePreferencies)
          //  SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);

          Altaxo.AltaxoDocument closedProject = CurrentOpenProject;
          //CurrentSelectedProject = null;
          //CurrentOpenCombine = CurrentSelectedCombine = null;
          openProjectFileName = null;
          WorkbenchSingleton.Workbench.CloseAllViews();
          OnProjectClosed(new ProjectEventArgs(closedProject));
          //closedProject.Dispose();

          // now create a new project
          CurrentOpenProject = new Altaxo.AltaxoDocument();
          OnProjectOpened(new ProjectEventArgs(CurrentOpenProject));

        }
      }
    }


    /// <summary>
    /// Returns all currently open views that show the given document object <code>document</code>.
    /// The IViewContent must implement <see cref="Altaxo.Gui.IMVCControllerWrapper" /> in order to be found by this routine.
    /// </summary>
    /// <param name="document">The document for which views must be found.</param>
    /// <param name="maxNumber">Max number of <see cref="IViewContent" /> to return.</param>
    /// <returns>An array containing all views that show the document table. If no view is found, an empty array is returned.</returns>
    public List<IViewContent> SearchContentForDocument(object document, int maxNumber)
    {
      List<IViewContent> contentList = new List<IViewContent>();
      // first step : look in all views

      foreach (IViewContent content in Current.Workbench.ViewContentCollection)
      {
        if (content is Altaxo.Gui.IMVCControllerWrapper)
        {
          if (object.ReferenceEquals(((Altaxo.Gui.IMVCControllerWrapper)content).MVCController.ModelObject, document))
            contentList.Add(content);
        }
          // TODO can be removed after WorksheetController has also wrapper class
        else if (content is Altaxo.Gui.IMVCController)
        {
          if (object.ReferenceEquals(((Altaxo.Gui.IMVCController)content).ModelObject, document))
            contentList.Add(content);
        }

        if (contentList.Count >= maxNumber)
          break;
      }

      return contentList;
    }

    /// <summary>
    /// Returns true if the given document has at least one open view in the workbench.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>True if there is at least one open view for the document.</returns>
    public bool HasDocumentAnOpenView(object document)
    {
      return SearchContentForDocument(document, 1).Count > 0;
    }

    void SelectFirstAvailableView()
    {
      // the following sequence is related to a bug encountered when closing a tabbed window by the program:
      // the active view content is not updated because the dockpanel lost the focus
      // to circumvent this, we focus on a new viewcontent, in this case the first one
      IViewContent firstView = null;
      foreach (IViewContent v in Current.Workbench.ViewContentCollection)
      {
        firstView = v;
        break;
      }

      if (firstView != null)
        WorkbenchSingleton.Workbench.WorkbenchLayout.ShowView(firstView);
    }

    #region Worksheet functions


    /// <summary>
    /// Creates a table and the view content for that table.
    /// </summary>
    /// <param name="worksheetName">The name of the table to create.</param>
    /// <param name="bCreateDefaultColumns">If true, a default x and a y column is created.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(string worksheetName, bool bCreateDefaultColumns)
    {

      Altaxo.Data.DataTable dt1 = this.CurrentOpenProject.CreateNewTable(worksheetName, bCreateDefaultColumns);
      return CreateNewWorksheet(dt1);
    }

    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="bCreateDefaultColumns">If true, a default x column and a default value column are created in the table.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(bool bCreateDefaultColumns)
    {
      return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableName(), bCreateDefaultColumns);
    }

    /// <summary>
    /// Creates a new table and the view content for that table.
    /// </summary>
    /// <returns>The content controller for that table.</returns>
    public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet()
    {
      return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableName(), false);
    }


    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
    {
      if (table.ParentObject == null)
        this.CurrentOpenProject.DataTableCollection.Add(table);

      return CreateNewWorksheet(table, this.CurrentOpenProject.CreateNewTableLayout(table));
    }


    /// <summary>
    /// Creates a view content for a table.
    /// </summary>
    /// <param name="table">The table which should be viewed.</param>
    /// <param name="layout">The layout for the table.</param>
    /// <returns>The view content for the provided table.</returns>
    public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
    {
      layout.DataTable = table;
      Altaxo.Gui.SharpDevelop.SDWorksheetViewContent ctrl = new Altaxo.Gui.SharpDevelop.SDWorksheetViewContent(layout);
      Altaxo.Worksheet.GUI.WorksheetView view = new Altaxo.Worksheet.GUI.WorksheetView();
      ctrl.Controller.View = view;


      if (null != Current.Workbench)
        Current.Workbench.ShowView(ctrl);

      return ctrl.Controller;
    }

    /// <summary>
    /// Opens a view that shows the table <code>table</code>. If no view for the table can be found,
    /// a new default view is created for the table.
    /// </summary>
    /// <param name="table">The table for which a view must be found.</param>
    /// <returns>The view content for the provided table.</returns>
    public object OpenOrCreateWorksheetForTable(Altaxo.Data.DataTable table)
    {

      // if a content exist that show that table, activate that content
      List<IViewContent> foundContent = SearchContentForDocument(table,1);
      if (foundContent.Count > 0)
      {
        foundContent[0].WorkbenchWindow.SelectWindow();
        return foundContent[0];
      }


      // otherwise create a new Worksheet
      return CreateNewWorksheet(table);
    }

    /// <summary>
    /// This function will delete a data table and close the corresponding views.
    /// </summary>
    /// <param name="table">The data table to delete</param>
    /// <param name="force">If true, the table is deleted without safety question,
    /// if false, the user is ask before the table is deleted.</param>
    public void DeleteTable(Altaxo.Data.DataTable table, bool force)
    {
      if (!force &&
        System.Windows.Forms.DialogResult.No == System.Windows.Forms.MessageBox.Show(Current.MainWindow, "Are you sure to remove the table and the corresponding views?", "Attention", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning))
        return;

      // close all windows
      List<IViewContent> foundContent = SearchContentForDocument(table,int.MaxValue);
      foreach (IViewContent content in foundContent)
      {
        content.WorkbenchWindow.CloseWindow(true);
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
    public void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl)
    {
      foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
      {
        if ((content is Altaxo.Gui.IMVCControllerWrapper) &&
          object.ReferenceEquals(((Altaxo.Gui.IMVCControllerWrapper)content).MVCController, ctrl)) 
          {
            WorkbenchSingleton.Workbench.CloseContent(content);
            break;
          }
        
      }

    }



    #endregion

    #region Graph functions

    /// <summary>
    /// Creates a new graph document and the view content..
    /// </summary>
    /// <returns>The view content for the newly created graph.</returns>
    public Altaxo.Graph.GUI.IGraphController CreateNewGraph()
    {
      return CreateNewGraph(this.CurrentOpenProject.CreateNewGraphDocument());
    }



    /// <summary>
    /// Creates a new view content for a graph document.
    /// </summary>
    /// <param name="graph">The graph document.</param>
    /// <returns>The view content for the provided graph document.</returns>
    public Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.Gdi.GraphDocument graph)
    {
      if (graph == null)
        graph = this.CurrentOpenProject.CreateNewGraphDocument();

      Altaxo.Gui.SharpDevelop.SDGraphViewContent ctrl = new Altaxo.Gui.SharpDevelop.SDGraphViewContent(graph);

      Altaxo.Graph.GUI.GraphView view = new Altaxo.Graph.GUI.GraphView();
      
      ctrl.Controller.View = view;

      if (null != Current.Workbench)
        Current.Workbench.ShowView(ctrl);

      return ctrl.Controller;
    }

    /// <summary>
    /// Opens a view that shows the graph <code>graph</code>. If no view for the graph can be found,
    /// a new default view is created.
    /// </summary>
    /// <param name="graph">The graph for which a view must be found.</param>
    /// <returns>The view content for the provided graph.</returns>
    public object OpenOrCreateGraphForGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph)
    {

      // if a content exist that show that graph, activate that content
      List<IViewContent> foundContent = SearchContentForDocument(graph,1);
      if (foundContent.Count > 0)
      {
        foundContent[0].WorkbenchWindow.SelectWindow();
        return foundContent[0];
      }


      // otherwise create a new graph view
      return CreateNewGraph(graph);
    }


    /// <summary>
    /// This function will delete a graph document and close all corresponding views.
    /// </summary>
    /// <param name="graph">The graph document to delete.</param>
    /// <param name="force">If true, the graph document is deleted without safety question,
    /// if false, the user is ask before the graph document is deleted.</param>
    public void DeleteGraphDocument(Altaxo.Graph.Gdi.GraphDocument graph, bool force)
    {
     
      if (!force &&
        System.Windows.Forms.DialogResult.No == System.Windows.Forms.MessageBox.Show(
        Current.MainWindow,
        "Are you sure to remove the graph document and the corresponding views?", "Attention", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning))
        return;
      
      // close all windows
      List<IViewContent> foundContent = SearchContentForDocument(graph,int.MaxValue);
      foreach (IViewContent content in foundContent)
      {
        content.WorkbenchWindow.CloseWindow(true);
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
    public void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl)
    {
      foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
      {
        if ((content is Altaxo.Gui.IMVCControllerWrapper) &&
            object.ReferenceEquals(((Altaxo.Gui.IMVCControllerWrapper)content).MVCController, ctrl))
        {
          WorkbenchSingleton.Workbench.CloseContent(content);
          break;
        }
      }
    }
    

    #endregion

   

    void EhProjectDirtyChanged(object sender, EventArgs e)
    {
      OnProjectDirtyChanged(new Altaxo.Main.ProjectEventArgs(this.openProject));
    }


    //********* own events

    /// <summary>
    /// Fires the ProjectOpened event.
    /// </summary>
    /// <param name="e">Event args indicating which project was opened.</param>
    protected virtual void OnProjectOpened(ProjectEventArgs e)
    {
      if (ProjectOpened != null)
      {
        ProjectOpened(this, e);
      }

      OnProjectChanged();
    }

    /// <summary>
    /// Fires the project closed event.
    /// </summary>
    /// <param name="e">Indicates which project was closed.</param>
    protected virtual void OnProjectClosed(ProjectEventArgs e)
    {
      if (ProjectClosed != null)
      {
        ProjectClosed(this, e);
      }

      OnProjectChanged();
    }

    /// <summary>
    /// Fires the <see cref="ProjectRenamed" /> event.
    /// </summary>
    /// <param name="e">Indicates which project was renamed, and the old and the new name of the project.</param>
    protected virtual void OnRenameProject(ProjectRenameEventArgs e)
    {
      if (ProjectRenamed != null)
      {
        ProjectRenamed(this, e);
      }

      OnProjectChanged();
    }

    /// <summary>
    /// Fires the <see cref="ProjectDirtyChanged" /> event.
    /// </summary>
    /// <param name="e">Indicats on which project the dirty flag changed.</param>
    protected virtual void OnProjectDirtyChanged(ProjectEventArgs e)
    {
      if (ProjectDirtyChanged != null)
      {
        ProjectDirtyChanged(this, e);
      }

      OnProjectChanged();
    }

    /// <summary>
    /// Fires the <see cref="ProjectChanged" /> event. This occurs <b>after</b> the events <see cref="ProjectOpened" />, 
    /// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" /> event. Usefull if
    /// you not want to subscribe to the above mentioned single events.
    /// </summary>
    protected virtual void OnProjectChanged()
    {
      if (ProjectChanged != null)
        ProjectChanged(this, new ProjectEventArgs(this.CurrentOpenProject));

    }


    /// <summary>
    /// Fired when a project is opened or a new empty project is created.
    /// </summary>
    public event ProjectEventHandler ProjectOpened;

    /// <summary>
    /// Fired when the current open project is closed.
    /// </summary>
    public event ProjectEventHandler ProjectClosed;

    /// <summary>
    /// Fired when the current open project is renamed.
    /// </summary>
    public event ProjectRenameEventHandler ProjectRenamed;
    /// <summary>
    /// Fired when the dirty state of the project changed.
    /// </summary>
    public event ProjectEventHandler ProjectDirtyChanged;

    /// <summary>
    /// Event fired <b>after</b> any of the following other events is fired: <see cref="ProjectOpened" />, 
    /// <see cref="ProjectClosed" />, <see cref="ProjectRenamed" />, and <see cref="ProjectDirtyChanged" />.
    /// </summary>
    public event ProjectEventHandler ProjectChanged;
  }


}
