// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2072 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// An enumerator which enumerates through a list of project items, returning the
	/// parseable file content of each item.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe in a very limited way:
	/// It can be created from every thread, but may be only used by the thread that created it.
	/// It automatically uses WorkbenchSingleton.SafeThreadCall for reading currently open
	/// files when it is created/accessed from a thread.
	/// </remarks>
	public class ParseableFileContentEnumerator : IEnumerator<KeyValuePair<string, string>>
	{
		void IEnumerator.Reset() {
			throw new NotSupportedException();
		}
		
		KeyValuePair<string, string> current;
		
		object IEnumerator.Current {
			get {
				return current;
			}
		}
		
		public KeyValuePair<string, string> Current {
			get {
				return current;
			}
		}
		
		public string CurrentFileName {
			get {
				return current.Key;
			}
		}
		
		public string CurrentFileContent {
			get {
				return current.Value;
			}
		}
		
		public void Dispose()
		{
		}
		
		IList<ProjectItem> projectItems;
		bool isOnMainThread;
		Encoding defaultEncoding;
		
		public ParseableFileContentEnumerator(IProject project) : this(project.Items) { }
		
		public ParseableFileContentEnumerator(IList<ProjectItem> projectItems)
		{
			isOnMainThread = !WorkbenchSingleton.InvokeRequired;
			this.projectItems = projectItems;
			if (projectItems.Count > 0) {
				nextItem = projectItems[0];
			}
			defaultEncoding = ParserService.DefaultFileEncoding;
		}
		
		string GetParseableFileContent(IProject project, string fileName)
		{
			// Loading the source files is done asynchronously:
			// While one file is parsed, the next is already loaded from disk.
			
			// load file
			Encoding tmp = defaultEncoding;
			return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(fileName, ref tmp, defaultEncoding);
		}
		
		ProjectItem nextItem;
		int index = 0;
		
		public int ItemCount {
			get {
				return projectItems.Count;
			}
		}
		
		public int Index {
			get {
				return index;
			}
		}
		
		public bool MoveNext()
		{
			ProjectItem item = nextItem;
			nextItem = (++index < projectItems.Count) ? projectItems[index] : null;
			if (item == null) return false;
			if (item.ItemType != ItemType.Compile)
				return MoveNext();
			string fileContent;
			try {
				fileContent = GetFileContent(item);
			} catch (FileNotFoundException ex) {
				LoggingService.Warn("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip files that were not found
			} catch (IOException ex) {
				LoggingService.Warn("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip invalid files
			}
			current = new KeyValuePair<string, string>(item.FileName, fileContent);
			return true;
		}
		
		string GetFileContent(ProjectItem item)
		{
			string fileName = item.FileName;
			if (IsFileOpen(fileName)) {
				string content;
				if (isOnMainThread)
					content = GetFileContentFromOpenFile(fileName);
				else
					content = WorkbenchSingleton.SafeThreadFunction<string, string>(GetFileContentFromOpenFile, fileName);
				if (content != null)
					return content;
			}
			return GetParseableFileContent(item.Project, fileName);
		}
		
		IViewContent[] viewContentCollection;
		
		IViewContent[] GetViewContentCollection()
		{
			return WorkbenchSingleton.Workbench.ViewContentCollection.ToArray();
		}
		
		bool IsFileOpen(string fileName)
		{
			if (viewContentCollection == null) {
				viewContentCollection = WorkbenchSingleton.SafeThreadFunction<IViewContent[]>(GetViewContentCollection);
			}
			foreach (IViewContent content in viewContentCollection) {
				string contentName = content.IsUntitled ? content.UntitledName : content.FileName;
				if (contentName != null) {
					if (FileUtility.IsEqualFileName(fileName, contentName))
						return true;
				}
			}
			return false;
		}
		
		string GetFileContentFromOpenFile(string fileName)
		{
			IWorkbenchWindow window = FileService.GetOpenFile(fileName);
			if (window != null) {
				IViewContent viewContent = window.ViewContent;
				IEditable editable = viewContent as IEditable;
				if (editable != null) {
					return editable.Text;
				}
			}
			return null;
		}
	}
}
