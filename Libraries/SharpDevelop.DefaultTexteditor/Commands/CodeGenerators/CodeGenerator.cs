// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Collections;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class CodeGenerator
	{
		ArrayList content = new ArrayList();
		protected int       numOps  = 0;
		protected IAmbience csa;
		protected IAmbience vba;
		protected IClass    currentClass = null;
		protected TextArea editActionHandler;
		
		public CodeGenerator(IClass currentClass)
		{	
			try {
				csa = (IAmbience)AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences").BuildChildItem("CSharp", this);
				vba = (IAmbience)AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences").BuildChildItem("VBNET", this);
			} catch {
				Console.WriteLine("CSharpAmbience not found -- is the C# backend binding loaded???");
				return;
			}
			
			this.currentClass = currentClass;
			csa.ConversionFlags = ConversionFlags.All;
		}
		
		public abstract string CategoryName {
			get;
		}
		
		public abstract string Hint {
			get;
		}
		
		public abstract int ImageIndex {
			get;
		}
		
		public virtual bool IsActive {
			get {
				return content.Count > 0;
			}
		}
		
		public ArrayList Content {
			get {
				return content;
			}
		}
		
		protected bool StartCodeBlockInSameLine {
			get {
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				IProperties p = (IProperties)propertyService.GetProperty("SharpDevelop.UI.CodeGenerationOptions", new DefaultProperties());
				return p.GetProperty("StartBlockOnSameLine", true);
			}
		}
		
		public void GenerateCode(TextArea editActionHandler, IList items)
		{
			numOps = 0;
			this.editActionHandler = editActionHandler;
			editActionHandler.BeginUpdate();
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool save1         = editActionHandler.TextEditorProperties.AutoInsertCurlyBracket;
			IndentStyle save2  = editActionHandler.TextEditorProperties.IndentStyle;
			bool save3         = propertyService.GetProperty("VBBinding.TextEditor.EnableEndConstructs", true);
			propertyService.SetProperty("VBBinding.TextEditor.EnableEndConstructs", false);
			editActionHandler.TextEditorProperties.AutoInsertCurlyBracket = false;
			editActionHandler.TextEditorProperties.IndentStyle            = IndentStyle.Smart;
						
			string extension = Path.GetExtension(editActionHandler.MotherTextEditorControl.FileName);
			StartGeneration(items, extension);
			
			if (numOps > 0) {
				editActionHandler.Document.UndoStack.UndoLast(numOps);
			}
			// restore old property settings
			editActionHandler.TextEditorProperties.AutoInsertCurlyBracket = save1;
			editActionHandler.TextEditorProperties.IndentStyle            = save2;
			propertyService.SetProperty("VBBinding.TextEditor.EnableEndConstructs", save3);
			editActionHandler.EndUpdate();
			
			editActionHandler.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			editActionHandler.Document.CommitUpdate();
		}
		
		protected abstract void StartGeneration(IList items, string fileExtension);
		
		protected void Indent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			IProperties p = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			
			bool tabsToSpaces = p.GetProperty("TabsToSpaces", false);
			
			int  tabSize      = p.GetProperty("TabIndent", 4);
			int  indentSize   = p.GetProperty("IndentationSize", 4);
			
			if (tabsToSpaces) {
				editActionHandler.InsertString(new String(' ', indentSize));
			} else {
				editActionHandler.InsertString(new String('\t', indentSize / tabSize));
				int trailingSpaces = indentSize % tabSize;
				if (trailingSpaces > 0) {
					editActionHandler.InsertString(new String(' ', trailingSpaces));
					++numOps;
				}
			}
			++numOps;
		}
		
		protected void Return()
		{
			IndentLine();
			new Return().Execute(editActionHandler);++numOps;
		}
		
		protected void IndentLine()
		{
			int delta = editActionHandler.Document.FormattingStrategy.IndentLine(editActionHandler, editActionHandler.Document.GetLineNumberForOffset(editActionHandler.Caret.Offset));
			if (delta != 0) {
				++numOps;
				LineSegment caretLine = editActionHandler.Document.GetLineSegmentForOffset(editActionHandler.Caret.Offset);
				editActionHandler.Caret.Position = editActionHandler.Document.OffsetToPosition(Math.Min(editActionHandler.Caret.Offset + delta, caretLine.Offset + caretLine.Length));
			}
		}
	}
}
