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
				openProject = value;
			}
		}



		public override void InitializeService()
		{
			base.InitializeService();
		}

		public void SaveWindowStateToZippedFile(ZipOutputStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
		{
		
		{
			// first, we save our own state 
			ZipEntry ZipEntry = new ZipEntry("Workbench/MainWindow.xml");
			zippedStream.PutNextEntry(ZipEntry);
			zippedStream.SetLevel(0);
			info.BeginWriting(zippedStream);
			//info.AddValue("MainWindow",new MainControllerMemento(this));
			info.EndWriting();
		}

			// second, we save all workbench windows into the Workbench/Views 
			int i=0;
			foreach(Main.GUI.IWorkbenchContentController ctrl in App.Current.Workbench.ViewContentCollection)
			{
				i++;
				ZipEntry ZipEntry = new ZipEntry("Workbench/Views/View"+i.ToString()+".xml");
				zippedStream.PutNextEntry(ZipEntry);
				zippedStream.SetLevel(0);
				info.BeginWriting(zippedStream);
				info.AddValue("WorkbenchViewContent",ctrl);
				info.EndWriting();
			}
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

			foreach(Main.GUI.IWorkbenchContentController ctrl in restoredControllers)
			{
				ctrl.CreateView();
				if(ctrl.WorkbenchContentView != null)
				{
					App.Current.Workbench.ShowView(ctrl);
				}
			}

		}

		public void OpenProject(string filename)
		{
			if (openProject != null) 
			{
				SaveProject();
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
			System.IO.FileStream myStream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
			ZipFile zipFile = new ZipFile(myStream);
			Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
			AltaxoDocument newdocument = new AltaxoDocument();
			newdocument.RestoreFromZippedFile(zipFile,info);

			this.openProject = newdocument;
			App.Current.Workbench.CloseAllViews();
			RestoreWindowStateFromZippedFile(zipFile,info,newdocument);
						
			myStream.Close();
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
			this.openProject.SaveToZippedFile(zippedStream, info);
			SaveWindowStateToZippedFile(zippedStream, info);
			zippedStream.Close();
			myStream.Close();
		}

		public void SaveProject()
		{
			Save(openProjectFileName);
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
			fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.CombineFiles}|*.axoprj|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			
			if (fdiag.ShowDialog() == DialogResult.OK) 
			{
				string filename = fdiag.FileName;
				SaveProject(filename);
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowMessage(filename, resourceService.GetString("Internal.Project.Combine.CombineSavedMessage"));
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
				//if (saveCombinePreferencies)
				//	SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
				
				Altaxo.AltaxoDocument closedProject = CurrentOpenProject;
				//CurrentSelectedProject = null;
				//CurrentOpenCombine = CurrentSelectedCombine = null;
				CurrentOpenProject = null;
				openProjectFileName = null;
				WorkbenchSingleton.Workbench.CloseAllViews();
				OnProjectClosed(new ProjectEventArgs(closedProject));
				//closedProject.Dispose();
			}
		}

		//********* own events
		protected virtual void OnProjectOpened(ProjectEventArgs e)
		{
			if (ProjectOpened != null) 
			{
				ProjectOpened(this, e);
			}
		}
		
		protected virtual void OnProjectClosed(ProjectEventArgs e)
		{
			if (ProjectClosed != null) 
			{
				ProjectClosed(this, e);
			}
		}

		public virtual void OnRenameProject(ProjectRenameEventArgs e)
		{
			if (ProjectRenamed != null) 
			{
				ProjectRenamed(this, e);
			}
		}

		
		
		public event ProjectEventHandler ProjectOpened;
		public event ProjectEventHandler ProjectClosed;
		public event ProjectRenameEventHandler ProjectRenamed;
	}


}
