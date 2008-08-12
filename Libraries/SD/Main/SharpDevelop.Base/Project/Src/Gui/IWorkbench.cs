﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	public interface IWorkbench : IMementoCapable
	{
		/// <summary>
		/// The title shown in the title bar.
		/// </summary>
		string Title {
			get;
			set;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		List<IViewContent> ViewContentCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		List<PadDescriptor> PadContentCollection {
			get;
		}
		
		/// <summary>
		/// The active workbench window.
		/// </summary>
		IWorkbenchWindow ActiveWorkbenchWindow {
			get;
		}
		
		object ActiveContent {
			get;
		}
		
		IWorkbenchLayout WorkbenchLayout {
			get;
			set;
		}
		
		/// <summary>
		/// Gets whether SharpDevelop is the active application in Windows.
		/// </summary>
		bool IsActiveWindow {
			get;
		}
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace.
		/// </summary>
		void ShowView(IViewContent content);
		
		/// <summary>
		/// Inserts a new <see cref="IPadContent"/> object in the workspace.
		/// </summary>
		void ShowPad(PadDescriptor content);
		
		/// <summary>
		/// Closes and disposes a <see cref="IPadContent"/>.
		/// </summary>
		void UnloadPad(PadDescriptor content);
		
		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		PadDescriptor GetPad(Type type);
		
		/// <summary>
		/// Closes the IViewContent content when content is open.
		/// </summary>
		void CloseContent(IViewContent content);
		
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		void CloseAllViews();
		
		/// <summary>
		/// Re-initializes all components of the workbench, should be called
		/// when a special property is changed that affects layout stuff.
		/// (like language change)
		/// </summary>
		void RedrawAllComponents();
		
		/// <summary>
		/// Is called, when a workbench view was opened
		/// </summary>
		/// <example>
		/// WorkbenchSingleton.WorkbenchCreated += delegate {
		/// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
		/// };
		/// </example>
		event ViewContentEventHandler ViewOpened;
		
		/// <summary>
		/// Is called, when a workbench view was closed
		/// </summary>
		event ViewContentEventHandler ViewClosed;
		
		/// <summary>
		/// Is called, when the workbench window which the user has into
		/// the foreground (e.g. editable) changed to a new one.
		/// </summary>
		event EventHandler ActiveWorkbenchWindowChanged;
	}
}
