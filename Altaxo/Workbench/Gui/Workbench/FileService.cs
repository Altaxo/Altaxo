// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Linq;
using System.Collections.Generic;
using System;
using Altaxo.Main;
using System.Text;
using Altaxo.Main.Services;
using Altaxo.Workbench;

using System;
using System.Collections.Generic;
//using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

using System.IO;

using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Altaxo.Gui.Workbench
{
	public class FileService : IFileService
	{
		private void ParserServiceLoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			var displayBindingService = Altaxo.Current.GetRequiredService<IDisplayBindingService>();
			foreach (IViewContent content in Altaxo.Current.GetRequiredService<IWorkbench>().ViewContentCollection.ToArray())
			{
				displayBindingService.AttachSubWindows(content, true);
			}
		}

		#region Options

		/// <summary>used for OptionBinding</summary>
		public static FileService Instance
		{
			get { return (FileService)Altaxo.Current.GetRequiredService<IFileService>(); }
		}

		private IRecentOpen recentOpen;

		public IRecentOpen RecentOpen
		{
			get
			{
				return LazyInitializer.EnsureInitialized(ref recentOpen, () => new RecentOpen());
			}
		}

		public bool DeleteToRecycleBin
		{
			get
			{
				return Current.PropertyService.GetValue("Application.DeleteToRecycleBin", true);
			}
			set
			{
				Current.PropertyService.SetValue("Application.DeleteToRecycleBin", value);
			}
		}

		public bool SaveUsingTemporaryFile
		{
			get
			{
				return Current.PropertyService.GetValue("Application.SaveUsingTemporaryFile", true);
			}
			set
			{
				Current.PropertyService.SetValue("Application.SaveUsingTemporaryFile", value);
			}
		}

		#endregion Options

		#region DefaultFileEncoding

		public int DefaultFileEncodingCodePage
		{
			get { return Current.PropertyService.GetValue("Application.DefaultFileEncoding", 65001); }
			set { Current.PropertyService.SetValue("Application.DefaultFileEncoding", value); }
		}

		public Encoding DefaultFileEncoding
		{
			get
			{
				return Encoding.GetEncoding(DefaultFileEncodingCodePage);
			}
		}

		private readonly EncodingInfo[] allEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray();

		public IReadOnlyList<EncodingInfo> AllEncodings
		{
			get { return allEncodings; }
		}

		public EncodingInfo DefaultFileEncodingInfo
		{
			get
			{
				int cp = DefaultFileEncodingCodePage;
				return allEncodings.Single(e => e.CodePage == cp);
			}
			set
			{
				DefaultFileEncodingCodePage = value.CodePage;
			}
		}

		#endregion DefaultFileEncoding

		#region BrowseForFolder

		public string BrowseForFolder(string description, string selectedPath)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				dialog.Description = StringParser.Parse(description);
				if (selectedPath != null && selectedPath.Length > 0 && Directory.Exists(selectedPath))
				{
					dialog.RootFolder = Environment.SpecialFolder.MyComputer;
					dialog.SelectedPath = selectedPath;
				}
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					return dialog.SelectedPath;
				}
				else
				{
					return null;
				}
			}
		}

		#endregion BrowseForFolder

		#region OpenedFile

		private Dictionary<FileName, OpenedFile> openedFileDict = new Dictionary<FileName, OpenedFile>();

		/// <inheritdoc/>
		public IReadOnlyList<OpenedFile> OpenedFiles
		{
			get
			{
				Altaxo.Current.Dispatcher.VerifyAccess();
				return openedFileDict.Values.ToArray();
			}
		}

		/// <inheritdoc/>
		public OpenedFile GetOpenedFile(string fileName)
		{
			return GetOpenedFile(FileName.Create(fileName));
		}

		/// <inheritdoc/>
		public OpenedFile GetOpenedFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			Altaxo.Current.Dispatcher.VerifyAccess();

			OpenedFile file;
			openedFileDict.TryGetValue(fileName, out file);
			return file;
		}

		/// <inheritdoc/>
		public OpenedFile GetOrCreateOpenedFile(string fileName)
		{
			return GetOrCreateOpenedFile(FileName.Create(fileName));
		}

		/// <inheritdoc/>
		public OpenedFile GetOrCreateOpenedFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			OpenedFile file;
			if (!openedFileDict.TryGetValue(fileName, out file))
			{
				openedFileDict[fileName] = file = new FileServiceOpenedFile(this, fileName);
			}
			return file;
		}

		/// <inheritdoc/>
		public OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content)
		{
			if (defaultName == null)
				throw new ArgumentNullException("defaultName");

			OpenedFile file = new FileServiceOpenedFile(this, content);
			file.FileName = new FileName(file.GetHashCode() + "/" + defaultName);
			openedFileDict[file.FileName] = file;
			return file;
		}

		/// <summary>Called by OpenedFile.set_FileName to update the dictionary.</summary>
		internal void OpenedFileFileNameChange(OpenedFile file, FileName oldName, FileName newName)
		{
			if (oldName == null) return; // File just created with NewFile where name is being initialized.

			Current.Log.Debug("OpenedFileFileNameChange: " + oldName + " => " + newName);

			if (openedFileDict[oldName] != file)
				throw new ArgumentException("file must be registered as oldName");
			if (openedFileDict.ContainsKey(newName))
			{
				OpenedFile oldFile = openedFileDict[newName];
				if (oldFile.CurrentView != null)
				{
					oldFile.CurrentView.CloseCommand?.Execute(true);
				}
				else
				{
					throw new ArgumentException("there already is a file with the newName");
				}
			}
			openedFileDict.Remove(oldName);
			openedFileDict[newName] = file;
		}

		/// <summary>Called by OpenedFile.UnregisterView to update the dictionary.</summary>
		internal void OpenedFileClosed(OpenedFile file)
		{
			OpenedFile existing;
			if (openedFileDict.TryGetValue(file.FileName, out existing) && existing != file)
				throw new ArgumentException("file must be registered");

			openedFileDict.Remove(file.FileName);
			Current.Log.Debug("OpenedFileClosed: " + file.FileName);
		}

		#endregion OpenedFile

		#region CheckFileName

		/// <inheritdoc/>
		public bool CheckFileName(string path)
		{
			if (FileUtility.IsValidPath(path))
				return true;
			MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", path)));
			return false;
		}

		/// <inheritdoc/>
		public bool CheckDirectoryEntryName(string name)
		{
			if (FileUtility.IsValidDirectoryEntryName(name))
				return true;
			MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", name)));
			return false;
		}

		#endregion CheckFileName

		#region OpenFile (ViewContent)

		/// <inheritdoc/>
		public bool IsOpen(FileName fileName)
		{
			return GetOpenFile(fileName) != null;
		}

		/// <inheritdoc/>
		public IFileViewContent OpenFile(FileName fileName)
		{
			return OpenFile(fileName, true);
		}

		/// <inheritdoc/>
		public IFileViewContent OpenFile(FileName fileName, bool switchToOpenedView)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			Current.Log.Info("Open file " + fileName);

			IFileViewContent viewContent = GetOpenFile(fileName);
			if (viewContent != null)
			{
				if (switchToOpenedView)
				{
					viewContent.IsSelected = true;
				}
				return viewContent;
			}

			IDisplayBinding binding = Altaxo.Current.GetRequiredService<IDisplayBindingService>().GetBindingPerFileName(fileName);

			if (binding == null)
			{
				binding = new ErrorFallbackBinding("Could not find any display binding for " + Path.GetFileName(fileName));
			}
			if (FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK)
			{
				RecentOpen.AddRecentFile(fileName);
			}
			return GetOpenFile(fileName);
		}

		/// <inheritdoc/>
		public IFileViewContent OpenFileWith(FileName fileName, IDisplayBinding displayBinding, bool switchToOpenedView)
		{
			if (displayBinding == null)
				throw new ArgumentNullException("displayBinding");
			if (FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(displayBinding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK)
			{
				RecentOpen.AddRecentFile(fileName);
			}
			return GetOpenFile(fileName);
		}

		private sealed class LoadFileWrapper
		{
			private readonly IDisplayBinding binding;
			private readonly bool switchToOpenedView;

			public LoadFileWrapper(IDisplayBinding binding, bool switchToOpenedView)
			{
				this.binding = binding;
				this.switchToOpenedView = switchToOpenedView;
			}

			public void Invoke(FileName fileName)
			{
				OpenedFile file = Altaxo.Current.GetRequiredService<IFileService>().GetOrCreateOpenedFile(fileName);
				try
				{
					IViewContent newContent = binding.CreateContentForFile(file);
					if (newContent != null)
					{
						Altaxo.Current.GetRequiredService<IDisplayBindingService>().AttachSubWindows(newContent, false);
						Altaxo.Current.GetRequiredService<IWorkbenchEx>().ShowView(newContent, switchToOpenedView);
					}
				}
				finally
				{
					file.CloseIfAllViewsClosed();
				}
			}
		}

		/// <inheritdoc/>
		public IFileViewContent NewFile(string defaultName, string content)
		{
			return NewFile(defaultName, DefaultFileEncoding.GetBytesWithPreamble(content));
		}

		/// <inheritdoc/>
		public IFileViewContent NewFile(string defaultName, byte[] content)
		{
			if (defaultName == null)
				throw new ArgumentNullException("defaultName");
			if (content == null)
				throw new ArgumentNullException("content");

			var displayBindingService = Altaxo.Current.GetRequiredService<IDisplayBindingService>();
			IDisplayBinding binding = displayBindingService.GetBindingPerFileName(FileName.Create(defaultName));

			if (binding == null)
			{
				binding = new ErrorFallbackBinding("Can't create display binding for file " + defaultName);
			}
			OpenedFile file = CreateUntitledOpenedFile(defaultName, content);

			IFileViewContent newContent = (IFileViewContent)binding.CreateContentForFile(file);
			if (newContent == null)
			{
				Current.Log.Warn("Created view content was null - DefaultName:" + defaultName);
				file.CloseIfAllViewsClosed();
				return null;
			}

			displayBindingService.AttachSubWindows(newContent, false);

			Altaxo.Current.GetRequiredService<IWorkbench>().ShowView(newContent, true);
			return newContent;
		}

		/// <inheritdoc/>
		public IReadOnlyList<FileName> OpenPrimaryFiles
		{
			get
			{
				List<FileName> fileNames = new List<FileName>();
				foreach (var content in Altaxo.Current.GetRequiredService<IWorkbench>().ViewContentCollection.OfType<IFileViewContent>())
				{
					FileName contentName = content.PrimaryFileName;
					if (contentName != null && !fileNames.Contains(contentName))
						fileNames.Add(contentName);
				}
				return fileNames;
			}
		}

		/// <inheritdoc/>
		public IFileViewContent GetOpenFile(FileName fileName)
		{
			if (fileName != null)
			{
				foreach (var content in Altaxo.Current.GetRequiredService<IWorkbench>().ViewContentCollection.OfType<IFileViewContent>())
				{
					string contentName = content.PrimaryFileName;
					if (contentName != null)
					{
						if (FileUtility.IsEqualFileName(fileName, contentName))
							return content;
					}
				}
			}
			return null;
		}

		private sealed class ErrorFallbackBinding : IDisplayBinding
		{
			private string errorMessage;

			public ErrorFallbackBinding(string errorMessage)
			{
				this.errorMessage = errorMessage;
			}

			public bool CanCreateContentForFile(FileName fileName)
			{
				return true;
			}

			public IViewContent CreateContentForFile(OpenedFile file)
			{
				return new SimpleViewContent(errorMessage) { TitleName = Path.GetFileName(file.FileName) };
			}

			public bool IsPreferredBindingForFile(FileName fileName)
			{
				return false;
			}

			public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
			{
				return double.NegativeInfinity;
			}
		}

		/// <inheritdoc/>
		public IFileViewContent JumpToFilePosition(FileName fileName, int line, int column)
		{
			Current.Log.InfoFormatted("FileService\n\tJumping to File Position:  [{0} : {1}x{2}]", fileName, line, column);

			if (fileName == null)
			{
				return null;
			}

			NavigationService.SuspendLogging();
			bool loggingResumed = false;

			try
			{
				var content = OpenFile(fileName);

				NavigationService.ResumeLogging();
				loggingResumed = true;
				NavigationService.Log(content);

				return content;
			}
			finally
			{
				Current.Log.InfoFormatted("FileService\n\tJumped to File Position:  [{0} : {1}x{2}]", fileName, line, column);

				if (!loggingResumed)
				{
					NavigationService.ResumeLogging();
				}
			}
		}

		public IEnumerable<IFileViewContent> ShowOpenWithDialog(IEnumerable<FileName> fileNames, bool switchToOpenedView = true)
		{
			yield break;
		}

		#endregion OpenFile (ViewContent)

		#region Remove/Rename/Copy

		/// <summary>
		/// Removes a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public void RemoveFile(string fileName, bool isDirectory)
		{
			FileCancelEventArgs eargs = new FileCancelEventArgs(fileName, isDirectory);
			OnFileRemoving(eargs);
			if (eargs.Cancel)
				return;
			if (!eargs.OperationAlreadyDone)
			{
				if (isDirectory)
				{
					try
					{
						if (Directory.Exists(fileName))
						{
							if (Altaxo.Current.GetRequiredService<IFileService>().DeleteToRecycleBin)
								NativeMethods.DeleteToRecycleBin(fileName);
							else
								Directory.Delete(fileName, true);
						}
					}
					catch (Exception e)
					{
						MessageService.ShowHandledException(e, "Can't remove directory " + fileName);
					}
				}
				else
				{
					try
					{
						if (File.Exists(fileName))
						{
							if (Altaxo.Current.GetRequiredService<IFileService>().DeleteToRecycleBin)
								NativeMethods.DeleteToRecycleBin(fileName);
							else
								File.Delete(fileName);
						}
					}
					catch (Exception e)
					{
						MessageService.ShowHandledException(e, "Can't remove file " + fileName);
					}
				}
			}
			OnFileRemoved(new FileEventArgs(fileName, isDirectory));
		}

		/// <summary>
		/// Renames or moves a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public bool RenameFile(string oldName, string newName, bool isDirectory)
		{
			if (string.Equals(FileUtility.NormalizePath(oldName),
									FileUtility.NormalizePath(newName),
									StringComparison.Ordinal))
				return false;
			// FileChangeWatcher.DisableAllChangeWatchers();
			try
			{
				FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
				OnFileRenaming(eargs);
				if (eargs.Cancel)
					return false;
				if (!eargs.OperationAlreadyDone)
				{
					try
					{
						if (isDirectory)
						{
							Directory.Move(oldName, newName);
						}
						else
						{
							File.Move(oldName, newName);
						}
					}
					catch (Exception e)
					{
						if (isDirectory)
						{
							MessageService.ShowHandledException(e, "Can't rename directory " + oldName);
						}
						else
						{
							MessageService.ShowHandledException(e, "Can't rename file " + oldName);
						}
						return false;
					}
				}
				OnFileRenamed(new FileRenameEventArgs(oldName, newName, isDirectory));
				return true;
			}
			finally
			{
				// FileChangeWatcher.EnableAllChangeWatchers();
			}
		}

		/// <summary>
		/// Copies a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite)
		{
			if (FileUtility.IsEqualFileName(oldName, newName))
				return false;
			FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
			OnFileCopying(eargs);
			if (eargs.Cancel)
				return false;
			if (!eargs.OperationAlreadyDone)
			{
				try
				{
					if (FileHelpers.CheckRenameOrReplacePossible(eargs, overwrite))
					{
						if (isDirectory)
						{
							FileUtility.DeepCopy(oldName, newName, overwrite);
						}
						else
						{
							File.Copy(oldName, newName, overwrite);
						}
					}
				}
				catch (Exception e)
				{
					if (isDirectory)
					{
						MessageService.ShowHandledException(e, "Can't copy directory " + oldName);
					}
					else
					{
						MessageService.ShowHandledException(e, "Can't copy file " + oldName);
					}
					return false;
				}
			}
			OnFileCopied(new FileRenameEventArgs(oldName, newName, isDirectory));
			return true;
		}

		private void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null)
			{
				FileRemoved(this, e);
			}
		}

		private void OnFileRemoving(FileCancelEventArgs e)
		{
			if (FileRemoving != null)
			{
				FileRemoving(this, e);
			}
		}

		private void OnFileRenamed(FileRenameEventArgs e)
		{
			if (FileRenamed != null)
			{
				FileRenamed(this, e);
			}
		}

		private void OnFileRenaming(FileRenamingEventArgs e)
		{
			if (FileRenaming != null)
			{
				FileRenaming(this, e);
			}
		}

		private void OnFileCopied(FileRenameEventArgs e)
		{
			if (FileCopied != null)
			{
				FileCopied(this, e);
			}
		}

		private void OnFileCopying(FileRenamingEventArgs e)
		{
			if (FileCopying != null)
			{
				FileCopying(this, e);
			}
		}

		public event EventHandler<FileRenamingEventArgs> FileRenaming;

		public event EventHandler<FileRenameEventArgs> FileRenamed;

		public event EventHandler<FileRenamingEventArgs> FileCopying;

		public event EventHandler<FileRenameEventArgs> FileCopied;

		public event EventHandler<FileCancelEventArgs> FileRemoving;

		public event EventHandler<FileEventArgs> FileRemoved;

		#endregion Remove/Rename/Copy

		#region FileCreated/Replaced

		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		/// <returns>True if the operation can proceed, false if an event handler cancelled the operation.</returns>
		public bool FireFileReplacing(string fileName, bool isDirectory)
		{
			FileCancelEventArgs e = new FileCancelEventArgs(fileName, isDirectory);
			if (FileReplacing != null)
			{
				FileReplacing(this, e);
			}
			return !e.Cancel;
		}

		/// <summary>
		/// Fires the event handlers for a file being replaced.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public void FireFileReplaced(string fileName, bool isDirectory)
		{
			if (FileReplaced != null)
			{
				FileReplaced(this, new FileEventArgs(fileName, isDirectory));
			}
		}

		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public void FireFileCreated(string fileName, bool isDirectory)
		{
			if (FileCreated != null)
			{
				FileCreated(this, new FileEventArgs(fileName, isDirectory));
			}
		}

		public event EventHandler<FileEventArgs> FileCreated;

		public event EventHandler<FileCancelEventArgs> FileReplacing;

		public event EventHandler<FileEventArgs> FileReplaced;

		#endregion FileCreated/Replaced
	}
}
