// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.Core.Services
{
	public enum FileErrorPolicy {
		Inform,
		ProvideAlternative
	}
	
	public enum FileOperationResult {
		OK,
		Failed,
		SavedAlternatively
	}
	
	public delegate void FileOperationDelegate();
	
	public delegate void NamedFileOperationDelegate(string fileName);
	
	/// <summary>
	/// A utility class related to file utilities.
	/// </summary>
	public class FileUtilityService : AbstractService
	{
		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.VolumeSeparatorChar };
		string sharpDevelopRootPath;
		
		public string SharpDevelopRootPath {
			get {
				return sharpDevelopRootPath;
			}
		}
		
		public FileUtilityService()
		{
			sharpDevelopRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar + "..";
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
		}
		
		public override void UnloadService()
		{
			base.UnloadService();
		}

		public StringCollection SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			StringCollection collection = new StringCollection();
			SearchDirectory(directory, filemask, collection, searchSubdirectories);
			return collection;
		}
		
		public StringCollection SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true);
		}
		
		/// <summary>
		/// Finds all files which are valid to the mask <code>filemask</code> in the path
		/// <code>directory</code> and all subdirectories (if searchSubdirectories
		/// is true. The found files are added to the StringCollection 
		/// <code>collection</code>.
		/// </summary>
		void SearchDirectory(string directory, string filemask, StringCollection collection, bool searchSubdirectories)
		{
			try {
				string[] file = Directory.GetFiles(directory, filemask);
				foreach (string f in file) {
					collection.Add(f);
				}
				
				if (searchSubdirectories) {
					string[] dir = Directory.GetDirectories(directory);
					foreach (string d in dir) {
						SearchDirectory(d, filemask, collection, searchSubdirectories);
					}
				}
			} catch (Exception e) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e, "Can't access directory " + directory);
			}
		}
		
		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public string AbsoluteToRelativePath(string baseDirectoryPath, string absPath)
		{
			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].Equals(aPath[indx]))
					break;
			}
			
			if (indx == 0) {
				return absPath;
			}
			
			string erg = "";
			
			if(indx == bPath.Length) {
				erg += "." + Path.DirectorySeparatorChar;
			} else {
				for (int i = indx; i < bPath.Length; ++i) {
					erg += ".." + Path.DirectorySeparatorChar;
				}
			}
			erg += String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx);
			
			return erg;
		}
		
		/// <summary>
		/// Converts a given relative path and a given base path to a path that leads
		/// to the relative path absoulte.
		/// </summary>
		public string RelativeToAbsolutePath(string baseDirectoryPath, string relPath)
		{
			if (separators[0] != separators[1] && relPath.IndexOf(separators[1]) != -1) {
				return relPath;
			}
			string[] bPath = baseDirectoryPath.Split(separators[0]);
			string[] rPath = relPath.Split(separators[0]);
			int indx = 0;
			
			for (; indx < rPath.Length; ++indx) {
				if (!rPath[indx].Equals("..")) {
					break;
				}
			}
			
			if (indx == 0) {
				return baseDirectoryPath + separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, 1, rPath.Length-1);
			}
			
			string erg = String.Join(Path.DirectorySeparatorChar.ToString(), bPath, 0, Math.Max(0, bPath.Length - indx));
			
			erg += separators[0] + String.Join(Path.DirectorySeparatorChar.ToString(), rPath, indx, rPath.Length-indx);
			
			return erg;
		}
		
		/// <summary>
		/// This method checks the file fileName if it is valid.
		/// </summary>
		public bool IsValidFileName(string fileName)
		{
			// Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
			//        I can't find a .NET property or method for determining this variable.
			if (fileName == null || fileName.Length == 0 || fileName.Length >= 260) {
				return false;
			}
			
			// platform independend : check for invalid path chars
			foreach (char invalidChar in Path.InvalidPathChars) {
				if (fileName.IndexOf(invalidChar) >= 0) {
					return false;
				}
			}
			
			// platform dependend : Check for invalid file names (DOS)
			// this routine checks for follwing bad file names :
			// CON, PRN, AUX, NUL, COM1-9 and LPT1-9
			
			string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpper();
			}
			
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
		    	
		    	return false;
		    }
			    
		    char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM") ||
			          nameWithoutExtension.StartsWith("LPT")) &&
			          Char.IsDigit(ch));
		}
		
		public bool TestFileExists(string filename)
		{
			if (!File.Exists(filename)) {
				IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowWarning(stringParserService.Parse(resourceService.GetString("Fileutility.CantFindFileError"), new string[,] { {"FILE",  filename} }));
				return false;
			}
			return true;
		}
		
		public bool IsDirectory(string filename)
		{
			if (!Directory.Exists(filename)) {
				return false;
			}
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}
		
		/// <summary>
		/// Returns directoryName + "\\" (Win32) when directoryname doesn't end with
		/// "\\"
		/// </summary>
		public string GetDirectoryNameWithSeparator(string directoryName)
		{
			if (directoryName == null) return "";
			
			if (directoryName.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				return directoryName;
			}
			return directoryName + Path.DirectorySeparatorChar;
		}
		
		// Observe SAVE functions
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
#if !LINUX			
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
								break;
								case DialogResult.Retry:
									return ObservedSave(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
#else
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				Console.WriteLine("Error while saving : " + e.ToString());
			}
	
#endif
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			return ObservedSave(saveFile,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
#if !LINUX
			try {
				saveFileAs(fileName);
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while saving", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						restartlabel:
							using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while saving", e, true)) {
								switch (chooseDialog.ShowDialog()) {
									case DialogResult.OK:
										using (SaveFileDialog fdiag = new SaveFileDialog()) {
											fdiag.OverwritePrompt = true;
											fdiag.AddExtension    = true;
											fdiag.CheckFileExists = false;
											fdiag.CheckPathExists = true;
											fdiag.Title           = "Choose alternate file name";
											fdiag.FileName        = fileName;
											if (fdiag.ShowDialog() == DialogResult.OK) {
												return ObservedSave(saveFileAs, fdiag.FileName, message, policy);
											} else {
												goto restartlabel;
											}
										}
										case DialogResult.Retry:
											return ObservedSave(saveFileAs, fileName, message, policy);
									case DialogResult.Ignore:
										return FileOperationResult.Failed;
								}
							}
							break;
				}
			}
#else
			try {
				saveFileAs(fileName);
				return FileOperationResult.OK;
			} catch (Exception e) {
				Console.WriteLine("Error while saving as : " + e.ToString());
			}
#endif
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			return ObservedSave(saveFileAs,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedSave(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		// Observe LOAD functions
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			Debug.Assert(IsValidFileName(fileName));
#if !LINUX
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "Error while loading", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "Error while loading", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
								break;
								case DialogResult.Retry:
									return ObservedLoad(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
#else
			try {
				saveFile();
				return FileOperationResult.OK;
			} catch (Exception e) {
				Console.WriteLine("Error while loading " + e.ToString());
			}
#endif
			return FileOperationResult.Failed;
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			return ObservedLoad(saveFile,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedLoad(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		class LoadWrapper
		{
			NamedFileOperationDelegate saveFileAs;
			string fileName;
			
			public LoadWrapper(NamedFileOperationDelegate saveFileAs, string fileName)
			{
				this.saveFileAs = saveFileAs;
				this.fileName   = fileName;
			}
			
			public void Invoke()
			{
				saveFileAs(fileName);
			}
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			return ObservedLoad(new FileOperationDelegate(new LoadWrapper(saveFileAs, fileName).Invoke), fileName, message, policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			return ObservedLoad(saveFileAs,
			                    fileName,
			                    resourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedLoad(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
	}
}
