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
using System.DirectoryServices; // for SortDirection
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class RemoveLeadingWS : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.RemoveLeadingWS();
			}
		}
	}
	
	public class RemoveTrailingWS : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.RemoveTrailingWS();
			}
		}
	}
	
	
	public class ToUpperCase : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ToUpperCase();
			}
		}
	}
	
	public class ToLowerCase : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ToLowerCase();
			}
		}
	}
	
	public class InvertCaseAction : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.InvertCaseAction();
			}
		}
	}
	
	public class CapitalizeAction : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.CapitalizeAction();
			}
		}
	}
	
	public class ConvertTabsToSpaces : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertTabsToSpaces();
			}
		}
	}
	
	public class ConvertSpacesToTabs : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertSpacesToTabs();
			}
		}
	}
	
	public class ConvertLeadingTabsToSpaces : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertLeadingTabsToSpaces();
			}
		}
	}
	
	public class ConvertLeadingSpacesToTabs : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertLeadingSpacesToTabs();
			}
		}
	}
	
	/// <summary>
	/// This is a sample editaction plugin, it indents the selected area.
	/// </summary>
	public class IndentSelection : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.FormatBuffer();
			}
		}
	}
	
	/// <summary>
	/// This is a sample editaction plugin, it indents the selected area.
	/// </summary>
	public class SortSelection : AbstractMenuCommand
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		class SortComparer : IComparer
		{
			
			SortDirection sortDirection;
			bool isCaseSensitive;
			bool ignoreWhitespaces;
			
			public SortComparer()
			{
				isCaseSensitive   = propertyService.GetProperty(SortOptionsDialog.caseSensitiveOption, true);
				ignoreWhitespaces = propertyService.GetProperty(SortOptionsDialog.ignoreWhiteSpacesOption, true);
				sortDirection     = (SortDirection)propertyService.GetProperty(SortOptionsDialog.sortDirectionOption, SortDirection.Ascending);
			}
			
			public int Compare(object x, object y)
			{
				if (x == null || y == null) {
					return -1;
				}
				string str1;
				string str2;
				
				if (sortDirection == SortDirection.Ascending) {
					str1 = x.ToString();
					str2 = y.ToString();
				} else {
					str1 = y.ToString();
					str2 = x.ToString();
				}
				
				if (ignoreWhitespaces) {
					str1 = str1.Trim();
					str2 = str2.Trim();
				}
				
				if (!isCaseSensitive) {
					str1 = str1.ToUpper();
					str2 = str2.ToUpper();
				}
				
				return str1.CompareTo(str2);
			}
		}
		
		public void SortLines(IDocument document, int startLine, int endLine)
		{
			ArrayList lines = new ArrayList();
			for (int i = startLine; i <= endLine; ++i) {
				LineSegment line = document.GetLineSegment(i);
				lines.Add(document.GetText(line.Offset, line.Length));
			}
			
			lines.Sort(new SortComparer());
			
			bool removeDupes = propertyService.GetProperty(SortOptionsDialog.removeDupesOption, false);
			if (removeDupes) {
				for (int i = 0; i < lines.Count - 1; ++i) {
					if (lines[i].Equals(lines[i + 1])) {
						lines.RemoveAt(i);
						--i;
					}
				}
			}
			
			for (int i = 0; i < lines.Count; ++i) {
				LineSegment line = document.GetLineSegment(startLine + i);
				document.Replace(line.Offset, line.Length, lines[i].ToString());
			}
			
			// remove removed duplicate lines
			for (int i = startLine + lines.Count; i <= endLine; ++i) {
				LineSegment line = document.GetLineSegment(startLine + lines.Count);
				document.Remove(line.Offset, line.TotalLength);
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			
			using (SortOptionsDialog sortOptionsDialog = new SortOptionsDialog()) {
				sortOptionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				if (sortOptionsDialog.ShowDialog() == DialogResult.OK) {
					TextArea textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl.ActiveTextAreaControl.TextArea;
					textarea.BeginUpdate();
					if (textarea.SelectionManager.HasSomethingSelected) {
						foreach (ISelection selection in textarea.SelectionManager.SelectionCollection) {
							SortLines(textarea.Document, selection.StartPosition.Y, selection.EndPosition.Y);
						}
					} else { 
						SortLines(textarea.Document, 0, textarea.Document.TotalNumberOfLines - 1);
					}
					textarea.Caret.ValidateCaretPos();
					textarea.EndUpdate();
					textarea.Refresh();
				}
			}
		}
	}
	
}
