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
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	/// <summary>
	/// This interface describes the basic functions of the 
	/// SharpDevelop file service.
	/// </summary>
	public interface IFileService
	{
		/// <remarks>
		/// gets the RecentOpen object.
		/// </remarks>
		RecentOpen RecentOpen {
			get;
		}
		
		/// <remarks>
		/// Opens the file fileName in SharpDevelop (shows the file in
		/// the workbench window)
		/// </remarks>
		void OpenFile(string fileName);
		
		/// <remarks>
		/// Opens a new file with a given name, language and file content
		/// in the workbench window.
		/// </remarks>
		void NewFile(string defaultName, string language, string content);
		
		/// <remarks>
		/// Gets an opened file by name, returns null, if the file is not open.
		/// </remarks>
		IWorkbenchWindow GetOpenFile(string fileName);
		
		/// <remarks>
		/// Removes a file physically
		/// CAUTION : Use only this file for a remove operation, because it is important
		/// to know for other parts of the IDE when a file is removed.
		/// </remarks>
		void RemoveFile(string fileName);
		
		/// <remarks>
		/// Renames a file physically
		/// CAUTION : Use only this file for a rename operation, because it is important
		/// to know for other parts of the IDE when a file is renamed.
		/// </remarks>
		void RenameFile(string oldName, string newName);
		
		/// <remarks>
		/// Is called, when a file is renamed.
		/// </remarks>
		event FileEventHandler FileRenamed;
		
		/// <remarks>
		/// Is called, when a file is removed.
		/// </remarks>
		event FileEventHandler FileRemoved;
	}
}
