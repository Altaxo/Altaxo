﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2120 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base interface for projects.
	/// Thread-safe members lock on the SyncRoot. Non-thread-safe members may only be called from the main thread.
	/// 
	/// When you implement IProject, you should also implement IProjectItemListProvider and IProjectAllowChangeConfigurations
	/// </summary>
	public interface IProject
		: ISolutionFolder, IDisposable, IMementoCapable, ICanBeDirty
	{
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		ReadOnlyCollection<ProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Gets all items in the project that have the specified item type.
		/// This member is thread-safe.
		/// </summary>
		IEnumerable<ProjectItem> GetItemsOfType(ItemType type);
		
		/// <summary>
		/// Gets the default item type the specified file should have.
		/// </summary>
		/// <param name="fileName">The full path to the file to determine the item type for</param>
		ItemType GetDefaultItemType(string fileName);
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		ICollection<ItemType> AvailableFileItemTypes {
			get;
		}
		
		/// <summary>
		/// Gets a list of project sections stored in the solution file for this project.
		/// </summary>
		List<ProjectSection> ProjectSections {
			get;
		}
		
		/// <summary>
		/// Gets the language properties used for this project. This member is thread-safe.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		/// <summary>
		/// Gets the ambience used for the project. This member is thread-safe.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.IAmbience Ambience {
			get;
		}
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		string FileName {
			get;
			set;
		}
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		string Directory {
			get;
		}
		
		#region MSBuild properties used inside SharpDevelop base
		/// <summary>
		/// Gets/Sets the assembly name of the assembly created when building this project.
		/// Equivalent to MSBuild property "AssemblyName".
		/// </summary>
		string AssemblyName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the root namespace of the project.
		/// </summary>
		string RootNamespace {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		string OutputAssemblyFullPath {
			get;
		}
		
		/// <summary>
		/// Gets the name of the language binding used for the project.
		/// </summary>
		string Language {
			get;
		}
		
		/// <summary>
		/// Gets the name of the directory being the "Properties" folder of the application,
		/// relative to the project directory.
		/// This folder gets a node type in the project browser.
		/// Equivalent to MSBuild property "AppDesignerFolder".
		/// </summary>
		string AppDesignerFolder {
			get;
		}
		#endregion
		
		#region Configuration / Platform management
		/// <summary>
		/// Gets/Sets the active configuration. MSBuild properties
		/// </summary>
		string ActiveConfiguration {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the active platform.
		/// </summary>
		string ActivePlatform {
			get;
			set;
		}
		/// <summary>
		/// Gets the list of available configuration names.
		/// </summary>
		ICollection<string> ConfigurationNames { get; }
		
		/// <summary>
		/// Gets the list of available platform names.
		/// </summary>
		ICollection<string> PlatformNames { get; }
		#endregion
		
		/// <summary>
		/// Saves the project using it's current file name.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name) is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		bool IsFileInProject(string fileName);
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		FileProjectItem FindFile(string fileName);
		
		/// <summary>
		/// Gets if the project can be started.
		/// </summary>
		bool IsStartable { get; }
		
		/// <summary>
		/// Starts the project.
		/// </summary>
		/// <param name="withDebugging">True, if a debugger should be used for the project.</param>
		void Start(bool withDebugging);
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// Return null if you don't want to create any project content.
		/// </summary>
		ParseProjectContent CreateProjectContent();
		
		/// <summary>
		/// Starts building the project using the specified options.
		/// </summary>
		void StartBuild(BuildOptions options);
		
		/// <summary>
		/// Creates a new ProjectItem for the passed MSBuild item.
		/// </summary>
		ProjectItem CreateProjectItem(Microsoft.Build.BuildEngine.BuildItem item);
	}
	
	/// <summary>
	/// Interface for adding and removing items from a project. Not part of the IProject
	/// interface because in nearly all cases, ProjectService.Add/RemoveProjectItem should
	/// be used instead!
	/// So IProject implementors should implement this interface, but only the SharpDevelop methods
	/// ProjectService.AddProjectItem and RemoveProjectItem may call the interface members.
	/// </summary>
	public interface IProjectItemListProvider
	{
		/// <summary>
		/// Gets a list of items in the project.
		/// </summary>
		ReadOnlyCollection<ProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Adds a new entry to the Items-collection
		/// </summary>
		void AddProjectItem(ProjectItem item);
		
		/// <summary>
		/// Removes an entry from the Items-collection
		/// </summary>
		bool RemoveProjectItem(ProjectItem item);
	}
	
	/// <summary>
	/// Interface for changing project or solution configuration.
	/// IProject implementors should implement this interface, but only the SharpDevelop methods
	/// Solution.RenameProjectPlatform etc. may call the interface members.
	/// </summary>
	public interface IProjectAllowChangeConfigurations
	{
		bool RenameProjectConfiguration(string oldName, string newName);
		bool RenameProjectPlatform(string oldName, string newName);
		bool AddProjectConfiguration(string newName, string copyFrom);
		bool AddProjectPlatform(string newName, string copyFrom);
		bool RemoveProjectConfiguration(string name);
		bool RemoveProjectPlatform(string name);
	}
}
