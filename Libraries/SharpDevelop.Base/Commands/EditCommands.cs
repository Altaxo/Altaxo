// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class Undo : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
			if (editable != null) {
				editable.Undo();
			}
		}
	}
	
	public class Redo : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
			if (editable != null) {
				editable.Redo();
			}
		}
	}

	public class Cut : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					return editable.ClipboardHandler.EnableCut;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			if (IsEnabled) {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					editable.ClipboardHandler.Cut(null, null);
				}
			}
		}
	}
	
	public class Copy : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					return editable.ClipboardHandler.EnableCopy;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			if (IsEnabled) {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					editable.ClipboardHandler.Copy(null, null);
				}
			}
		}
	}
	
	public class Paste : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					return editable.ClipboardHandler.EnablePaste;
				}
				return false;
			}
		}
		public override void Run()
		{
			if (IsEnabled) {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					editable.ClipboardHandler.Paste(null, null);
				}
			}
		}
	}
	
	public class Delete : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					return editable.ClipboardHandler.EnableDelete;
				}
				return false;
			}
		}
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent is IEditable) {
				if (((IEditable)window.ViewContent).ClipboardHandler != null) {
					((IEditable)window.ViewContent).ClipboardHandler.Delete(null, null);
				}
			}
		}
	}
	
	public class SelectAll : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					return editable.ClipboardHandler.EnableSelectAll;
				}
				return false;
			}
		}
		public override void Run()
		{
			if (IsEnabled) {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IEditable        editable = window != null ? window.ActiveViewContent as IEditable : null;
				if (editable != null) {
					editable.ClipboardHandler.SelectAll(null, null);
				}
			}
		}
	}

	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.Owner = (Form)WorkbenchSingleton.Workbench;
				wcd.ShowDialog();
			}
		}
	}
}
