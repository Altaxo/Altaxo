using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpDevelop.Services;

namespace Altaxo.Main
{
	
	public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);
	
	public class ProjectEventArgs : EventArgs
	{
		Altaxo.AltaxoDocument project;
		
		public Altaxo.AltaxoDocument Project
		{
			get 
			{
				return project;
			}
		}
		
		public ProjectEventArgs(Altaxo.AltaxoDocument combine)
		{
			this.project = project;
		}
	}


	public delegate void ProjectRenameEventHandler(object sender, ProjectRenameEventArgs e);
	
	public class ProjectRenameEventArgs : EventArgs
	{ 
		Altaxo.AltaxoDocument project;
		string   oldName;
		string   newName;
		
		public Altaxo.AltaxoDocument Project 
		{
			get 
			{
				return project;
			}
		}
		
		public string OldName 
		{
			get 
			{
				return oldName;
			}
		}
		
		public string NewName 
		{
			get 
			{
				return newName;
			}
		}
		
		public ProjectRenameEventArgs(Altaxo.AltaxoDocument project, string oldName, string newName)
		{
			this.project = project;
			this.oldName = oldName;
			this.newName = newName;
		}
	}

	public class ProjectService : ICSharpCode.Core.Services.AbstractService
	{
		/// <summary>
		/// The currently opened Altaxo project.
		/// </summary>
		Altaxo.AltaxoDocument openProject = null;
		
		/// <summary>
		/// The file name of the currently opened Altaxo project.
		/// </summary>
		string   openProjectFileName = null;


		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));


		public Altaxo.AltaxoDocument CurrentOpenProject 
		{
			get 
			{
				return openProject;
			}
			set 
			{
				Altaxo.AltaxoDocument oldProject = openProject;

				if(oldProject!=null)
					oldProject.DirtyChanged -= new EventHandler(this.EhProjectDirtyChanged);


				openProject = value;

				if(openProject!=null)
					openProject.DirtyChanged += new EventHandler(this.EhProjectDirtyChanged);


				if(!object.Equals(oldProject,value)	)
					OnProjectChanged();
			}
		}

		public string CurrentProjectFileName
		{
			get { return this.openProjectFileName; }
		}


		public override void InitializeService()
		{
			base.InitializeService();
		}

		public void SaveWindowStateToZippedFile(ZipOutputStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();
		
		{
			// first, we save our own state 
			ZipEntry ZipEntry = new ZipEntry("Workbench/MainWindow.xml");
			zippedStream.PutNextEntry(ZipEntry);
			zippedStream.SetLevel(0);
			try
			{
				info.BeginWriting(zippedStream);
				info.AddValue("MainWindow",Current.Workbench);
				info.EndWriting();
			}
			catch( Exception exc)
			{
				errorText.Append(exc.ToString());
			}
		}

			// second, we save all workbench windows into the Workbench/Views 
			int i=0;
			foreach(IViewContent ctrl in Current.Workbench.ViewContentCollection)
			{
				if(info.IsSerializable(ctrl))
				{
					i++;
					ZipEntry ZipEntry = new ZipEntry("Workbench/Views/View"+i.ToString()+".xml");
					zippedStream.PutNextEntry(ZipEntry);
					zippedStream.SetLevel(0);
					try
					{
						info.BeginWriting(zippedStream);
						info.AddValue("WorkbenchViewContent",ctrl);
						info.EndWriting();
					}
					catch(Exception exc)
					{
						errorText.Append(exc.ToString());
					}
				}
			}

			if(errorText.Length!=0)
				throw new ApplicationException(errorText.ToString());
		}


		public void RestoreWindowStateFromZippedFile(ZipFile zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info, AltaxoDocument restoredDoc)
		{
			System.Collections.ArrayList restoredControllers = new System.Collections.ArrayList();
			foreach(ZipEntry zipEntry in zipFile)
			{
				if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Workbench/Views/"))
				{
					System.IO.Stream zipinpstream = zipFile.GetInputStream(zipEntry);
					info.BeginReading(zipinpstream);
					object readedobject = info.GetValue("Table",this);
					if(readedobject is Main.GUI.IWorkbenchContentController)
						restoredControllers.Add(readedobject);
					info.EndReading();
				}
			}

			info.AnnounceDeserializationEnd(restoredDoc);
			info.AnnounceDeserializationEnd(this);

			// now give all restored controllers a view and show them in the Main view

			foreach(IViewContent viewcontent in restoredControllers)
			{
				Main.GUI.IWorkbenchContentController ctrl = viewcontent as Main.GUI.IWorkbenchContentController;
				if(ctrl!=null)
					ctrl.CreateView();
				
				if(viewcontent.Control != null)
				{
					Current.Workbench.ShowView(ctrl);
				}
			}

		}

		public void OpenProject(string filename)
		{
			if (CurrentOpenProject != null) 
			{
				System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
				if(this.CurrentOpenProject.IsDirty)
					AskForSavingOfProject(e);

				if(e.Cancel==true)
					return;


				CloseProject();
			}
				
			if (!fileUtilityService.TestFileExists(filename)) 
			{
				return;
			}
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");
				
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

			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		/// <summary>
		/// Loads a existing Altaxo project with the provided name.
		/// </summary>
		/// <param name="filename">The file name of the project to load.</param>
		void LoadProject(string filename)
		{
			if (!fileUtilityService.TestFileExists(filename)) 
			{
				return;
			}
			
			this.Load(filename);
			openProjectFileName = filename;
			
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.RecentOpen.AddLastProject(filename);
			
			OnProjectOpened(new ProjectEventArgs(openProject));
			
			// RestoreCombinePreferences(CurrentOpenCombine, openCombineFileName);
		}


		private void Load(string filename)
		{
			System.Text.StringBuilder errorText = new System.Text.StringBuilder();

			System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			ZipFile zipFile = new ZipFile(myStream);
			Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
			AltaxoDocument newdocument = new AltaxoDocument();
			
			try
			{
				newdocument.RestoreFromZippedFile(zipFile,info);
			}
			catch(Exception exc)
			{
				errorText.Append(exc.ToString());
			}

			try
			{
				Current.Workbench.CloseAllViews();
				this.CurrentOpenProject = newdocument;
				RestoreWindowStateFromZippedFile(zipFile,info,newdocument);
				this.CurrentOpenProject.IsDirty = false;
			}
			catch(Exception exc)
			{
				errorText.Append(exc.ToString());
				System.Windows.Forms.MessageBox.Show(Current.MainWindow,errorText.ToString(),"An error occured");
			}
			finally
			{
				myStream.Close();
			}
		}

		/// <summary>
		/// Internal routine to save a project under a given name.
		/// </summary>
		/// <param name="filename"></param>
		private void Save(string filename)
		{
			Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
			System.IO.Stream myStream = new System.IO.FileStream(filename,System.IO.FileMode.OpenOrCreate);
			ZipOutputStream zippedStream = new ZipOutputStream(myStream);
		
			Exception savingException = null;
			try
			{
				this.openProject.SaveToZippedFile(zippedStream, info);
				SaveWindowStateToZippedFile(zippedStream, info);
			}
			catch(Exception exc)
			{
				savingException = exc;
			}

			zippedStream.Close();
			myStream.Close();
			
			if(null!=savingException)
				throw savingException;
			
			this.openProject.IsDirty = false;
		}

		public void SaveProject()
		{
			SaveProject(openProjectFileName);
		}

		public void SaveProject(string filename)
		{
			string oldFileName = this.openProjectFileName;
			this.openProjectFileName = filename;
			if(oldFileName!=filename)
				this.OnRenameProject(new ProjectRenameEventArgs(this.openProject,oldFileName,filename));

			
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(this.Save),
				filename,
				resourceService.GetString("Internal.Project.Combine.CantSaveCombineErrorText"),
				FileErrorPolicy.ProvideAlternative);
		}

		public void SaveProjectAs()
		{
			SaveFileDialog fdiag = new SaveFileDialog();
			fdiag.OverwritePrompt = true;
			fdiag.AddExtension    = true;
			
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			fdiag.Filter = stringParserService.Parse("${res:Altaxo.FileFilter.ProjectFiles}|*.axoprj|${res:Altaxo.FileFilter.AllFiles}|*.*");
			
			if (fdiag.ShowDialog() == DialogResult.OK) 
			{
				string filename = fdiag.FileName;
				SaveProject(filename);
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowMessage(filename, resourceService.GetString("Altaxo.Project.ProjectSavedMessage"));
			}
		}

		public virtual void AskForSavingOfProject(System.ComponentModel.CancelEventArgs e)
		{
			string text = resourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Text");
			string caption = resourceService.GetString("Altaxo.Project.AskForSavingOfProjectDialog.Caption");
			System.Windows.Forms.DialogResult dlgresult = System.Windows.Forms.MessageBox.Show(Current.MainWindow,text,caption,System.Windows.Forms.MessageBoxButtons.YesNoCancel);
		
			switch(dlgresult)
			{
				case System.Windows.Forms.DialogResult.Yes:
					if(this.CurrentProjectFileName!=null)
						this.SaveProject();
					else
						this.SaveProjectAs();

					if(this.CurrentOpenProject.IsDirty)
						e.Cancel = true; // Cancel if the saving was not successfull
					break;

				case System.Windows.Forms.DialogResult.No:
					break;
				case System.Windows.Forms.DialogResult.Cancel:
					e.Cancel = true; // Cancel if the user pressed cancel
					break;
			}
		
		}

		public void CloseProject()
		{
			CloseProject(true);
		}

		public void CloseProject(bool saveCombinePreferencies)
		{

			if (CurrentOpenProject != null) 
			{
				System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
				if(this.CurrentOpenProject.IsDirty)
					AskForSavingOfProject(e);

				if(e.Cancel==false)
				{
					//if (saveCombinePreferencies)
					//	SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
				
					Altaxo.AltaxoDocument closedProject = CurrentOpenProject;
					//CurrentSelectedProject = null;
					//CurrentOpenCombine = CurrentSelectedCombine = null;
					openProjectFileName = null;
					CurrentOpenProject = new Altaxo.AltaxoDocument();
					WorkbenchSingleton.Workbench.CloseAllViews();
					OnProjectClosed(new ProjectEventArgs(closedProject));
					//closedProject.Dispose();
				}
			}
		}


		/// <summary>
		/// Creates a table and the view content for that table.
		/// </summary>
		/// <param name="worksheetName">The name of the table to create.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(string worksheetName, bool bCreateDefaultColumns)
		{
			
			Altaxo.Data.DataTable dt1 = this.CurrentOpenProject.CreateNewTable(worksheetName, bCreateDefaultColumns);
			return CreateNewWorksheet(dt1);
		}
	
		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="table">The table which should be viewed.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(bool bCreateDefaultColumns)
		{
			return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableName(),bCreateDefaultColumns);
		}

		/// <summary>
		/// Creates a new table and the view content for that table.
		/// </summary>
		/// <returns>The content controller for that table.</returns>
		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet()
		{
			return CreateNewWorksheet(this.CurrentOpenProject.DataTableCollection.FindNewTableName(),false);
		}


		/// <summary>
		/// Creates a view content for a table.
		/// </summary>
		/// <param name="table">The table which should be viewed.</param>
		/// <returns>The view content for the provided table.</returns>
		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
		{
			//Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			//Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			//wbv_controller.View = wbvform;

			Altaxo.Worksheet.GUI.WorksheetController ctrl = new Altaxo.Worksheet.GUI.WorksheetController(this.CurrentOpenProject.CreateNewTableLayout(table));
			Altaxo.Worksheet.GUI.WorksheetView view = new Altaxo.Worksheet.GUI.WorksheetView();
			ctrl.View = view;


			if(null!=Current.Workbench)
				Current.Workbench.ShowView(ctrl);

			//wbv_controller.Content = ctrl;
			
			//this.m_WorkbenchViews.Add(wbv_controller);
			//wbvform.Show();
			return ctrl;
		}

	

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
		public Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.GraphDocument graph)
		{
			//Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			//Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			//wbv_controller.View = wbvform;

			if(graph==null)
				graph = this.CurrentOpenProject.CreateNewGraphDocument();

			Altaxo.Graph.GUI.GraphController ctrl = new Altaxo.Graph.GUI.GraphController(graph);
			Altaxo.Graph.GUI.GraphView view = new Altaxo.Graph.GUI.GraphView();
			ctrl.View = view;

			
			//wbv_controller.Content = ctrl;

			//this.m_WorkbenchViews.Add(wbv_controller);
			//wbvform.Show();

			if(null!=Current.Workbench)
				Current.Workbench.ShowView(ctrl);
			return ctrl;
		}


	

		/// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
		/// <param name="ctrl">The GraphController to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl)
		{
			if(null!=Current.Workbench)
				Current.Workbench.CloseContent(ctrl);

			//if(this.m_WorkbenchViews.Contains(ctrl))
			//this.m_WorkbenchViews.Remove(ctrl);
			//else if(ctrl.ParentWorkbenchWindowController !=null && this.m_WorkbenchViews.Contains(ctrl.ParentWorkbenchWindowController))
			//this.m_WorkbenchViews.Remove(ctrl.ParentWorkbenchWindowController);
		}

		/// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
		/// <param name="ctrl">The Worksheet to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
		public void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl)
		{
			if(null!=Current.Workbench)
				Current.Workbench.CloseContent(ctrl);
		}


		void EhProjectDirtyChanged(object sender, EventArgs e)
		{
			OnProjectDirtyChanged(new Altaxo.Main.ProjectEventArgs(this.openProject));
		}


		//********* own events
		protected virtual void OnProjectOpened(ProjectEventArgs e)
		{
			if (ProjectOpened != null) 
			{
				ProjectOpened(this, e);
			}

			OnProjectChanged();
		}
		
		protected virtual void OnProjectClosed(ProjectEventArgs e)
		{
			if (ProjectClosed != null) 
			{
				ProjectClosed(this, e);
			}

			OnProjectChanged();
		}

		protected virtual void OnRenameProject(ProjectRenameEventArgs e)
		{
			if (ProjectRenamed != null) 
			{
				ProjectRenamed(this, e);
			}

			OnProjectChanged();
		}

		protected virtual void OnProjectDirtyChanged(ProjectEventArgs e)
		{
			if (ProjectDirtyChanged != null) 
			{
				ProjectDirtyChanged(this, e);
			}

			OnProjectChanged();
		}
		
		protected virtual void OnProjectChanged()
		{
			if(ProjectChanged != null)
				ProjectChanged(this, new ProjectEventArgs(this.CurrentOpenProject));

		}


		public event ProjectEventHandler ProjectOpened;
		public event ProjectEventHandler ProjectClosed;
		public event ProjectRenameEventHandler ProjectRenamed;
		/// <summary>
		/// Fired when the dirty state of the project changed.
		/// </summary>
		public event ProjectEventHandler ProjectDirtyChanged;
		
		/// <summary>
		/// Event fired when any of the other event is fired: ProjectOpened,
		/// ProjectClosed, ProjectRenamed and ProjectDirty. Firstly, the specific
		/// event is fired, and then, this event is fired.
		/// </summary>
		public event ProjectEventHandler ProjectChanged;
	}


}
