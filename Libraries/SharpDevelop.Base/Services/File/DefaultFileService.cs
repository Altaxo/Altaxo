// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultFileService : AbstractService, IFileService
	{
		string currentFile;
		RecentOpen       recentOpen = null;
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					recentOpen = (RecentOpen)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen", new RecentOpen());
				}
				return recentOpen;
			}
		}
		
		public string CurrentFile {
			get {
				return currentFile;
			}
			set {
				currentFile = value;
			}
		}
		
		class LoadFileWrapper
		{
			IDisplayBinding binding;
			
			public LoadFileWrapper(IDisplayBinding binding)
			{
				this.binding = binding;
			}
			
			public void Invoke(string fileName)
			{
				IViewContent newContent = binding.CreateContentForFile(fileName);
				WorkbenchSingleton.Workbench.ShowView(newContent);
				DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
				displayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
			}
		}
		public bool IsOpen(string fileName) 
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsUntitled) {
					if (content.UntitledName == fileName) {
 						return true;
					}
				} else if (content.FileName == fileName) {
					return true;
				}
				if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
					continue;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (Path.GetFullPath(viewContent.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
								return true;
							}
						} catch (Exception) {
						}
					}
				}
			}
			return false;
		}
		public void OpenFile(string fileName)
		{
			System.Diagnostics.Debug.Assert(fileUtilityService.IsValidFileName(fileName));
				
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(fileName)) { 
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == fileName) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					}
				} else if (!fileUtilityService.TestFileExists(fileName)) {
					return;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.FileName != null) {
					try {
						if (fileName.StartsWith("http://") ? content.FileName == fileName :
						    Path.GetFullPath(content.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					} catch (Exception) {
					}
				}
				if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
					continue;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (fileName.StartsWith("http://") ? viewContent.FileName == fileName :
							    Path.GetFullPath(viewContent.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
								viewContent.WorkbenchWindow.SelectWindow();
								return;
							}
						} catch (Exception) {
						}
					}
				}
			}
			
			DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			IDisplayBinding binding = displayBindingService.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding).Invoke), fileName) == FileOperationResult.OK) {
					fileService.RecentOpen.AddLastFile(fileName);
				}
			} else {
				throw new ApplicationException("Can't open " + fileName + ", no display codon found.");
			}
		}
		
		public void NewFile(string defaultName, string language, string content)
		{
			DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			IDisplayBinding binding = displayBindingService.GetBindingPerLanguageName(language);
			
			if (binding != null) {
				IViewContent newContent = binding.CreateContentForLanguage(language, content);
				if (newContent == null) {
					throw new ApplicationException(String.Format("Created view content was null{3}DefaultName:{0}{3}Language:{1}{3}Content:{2}", defaultName, language, content, Environment.NewLine));
				}
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
				
				displayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
			} else {
				throw new ApplicationException("Can't create display binding for language " + language);				
			}
		}
		
		public IWorkbenchWindow GetOpenFile(string fileName)
		{
			if (fileName != null && fileName.Length > 0) {
				string normalizedFileName = (fileName.StartsWith("http://") ? fileName : Path.IsPathRooted(fileName) ? Path.GetFullPath(fileName) : fileName).ToLower();
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string normalizedContentName = content.IsUntitled ? content.UntitledName : (content.FileName == null ? "" : (content.FileName.StartsWith("http://") ? content.FileName : Path.GetFullPath(content.FileName)));
					normalizedContentName = normalizedContentName.ToLower();
					Console.WriteLine(normalizedContentName + " -- " + normalizedFileName);

					if (normalizedContentName == normalizedFileName) {
						return content.WorkbenchWindow;
					}
					if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
						continue;
					foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
						IViewContent viewContent = subViewContent as IViewContent;
						if (viewContent != null && viewContent.FileName != null) {
							string normalizedViewContentName = viewContent.IsUntitled ? viewContent.UntitledName : (viewContent.FileName == null ? "" : (viewContent.FileName.StartsWith("http://") ? viewContent.FileName : Path.GetFullPath(viewContent.FileName)));
							normalizedViewContentName = normalizedViewContentName.ToLower();
							Console.WriteLine(normalizedViewContentName + " -- " + normalizedFileName);

							if (normalizedViewContentName == normalizedFileName) {
								return content.WorkbenchWindow;
							}
						}
					}
				}
			}
			return null;
		}
		
		public void RemoveFileFromProject(string fileName)
		{
			if (Directory.Exists(fileName)) {
				OnFileRemovedFromProject(new FileEventArgs(fileName, true));
			} else {
				OnFileRemovedFromProject(new FileEventArgs(fileName, false));
			}
		}
		
		public void RemoveFile(string fileName)
		{
			if (Directory.Exists(fileName)) {
				try {
					Directory.Delete(fileName, true);
				} catch (Exception e) {
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't remove directory " + fileName);
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					if (File.Exists(fileName)) {
						File.Delete(fileName);
					}
				} catch (Exception e) {
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Can't remove file " + fileName);
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, false));
			}
		}
		
		public void RenameFile(string oldName, string newName)
		{
			if (oldName != newName) {
				if (Directory.Exists(oldName)) {
					try {
						Directory.Move(oldName, newName);
					} catch (Exception e) {
						IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, "Can't rename directory " + oldName);
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, "Can't rename file " + oldName);
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, false));
				}
			}
		}
		public void JumpToFilePosition(string fileName, int line, int column)
		{
			if (fileName == null || fileName.Length == 0) {
				return;
			}
			OpenFile(fileName);
			IWorkbenchWindow window = GetOpenFile(fileName);
			if (window == null) {
				return;
			}
			IViewContent content = window.ViewContent;
			if (content.WorkbenchWindow.SubViewContents == null) {
				if (content is IPositionable) {
					window.SwitchView(0);
					((IPositionable)content).JumpTo(Math.Max(0, line), Math.Max(0, column));
				}
			}
			else
			{
				int i = 0;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (Path.GetFullPath(viewContent.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
								if (viewContent is IPositionable) {
									window.SwitchView(i);
									((IPositionable)viewContent).JumpTo(Math.Max(0, line), Math.Max(0, column));
									return;
								}
							}
						} catch (Exception) {
						}
					}
					i++;
				}
			}

		}
		
		protected virtual void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(this, e);
			}
		}

		protected virtual void OnFileRenamed(FileEventArgs e)
		{
			if (FileRenamed != null) {
				FileRenamed(this, e);
			}
		}

		protected virtual void OnFileRemovedFromProject(FileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}
		
		public event FileEventHandler FileRemovedFromProject;
		public event FileEventHandler FileRenamed;
		public event FileEventHandler FileRemoved;
	}
}
