// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Reflector.UserInterface;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.TextEditor.Gui.InsightWindow;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

using System.Threading;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextAreaControl : TextEditorControl
	{
		readonly static string contextMenuPath       = "/SharpDevelop/ViewContent/DefaultTextEditor/ContextMenu";
		readonly static string editActionsPath       = "/AddIns/DefaultTextEditor/EditActions";
		readonly static string formatingStrategyPath = "/AddIns/DefaultTextEditor/Formater";
		
		QuickClassBrowserPanel quickClassBrowserPanel = null;
		ErrorDrawer errorDrawer;
		
		public QuickClassBrowserPanel QuickClassBrowserPanel {
			get {
				return quickClassBrowserPanel;
			}
		}
		
		public SharpDevelopTextAreaControl()
		{
			errorDrawer = new ErrorDrawer(this);
			Document.FoldingManager.FoldingStrategy = new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ParserFoldingStrategy();
			GenerateEditActions();
			
			TextAreaDragDropHandler dragDropHandler = new TextAreaDragDropHandler();
			TextEditorProperties = new SharpDevelopTextEditorProperties();
		}
		
		public virtual ICompletionDataProvider CreateCodeCompletionDataProvider(bool ctrlSpace)
		{
			//ivoko: please do not touch or discuss with me: we use another CCDP
			return new CodeCompletionDataProvider(ctrlSpace, false);
		}

		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
			newControl.ContextMenu = menuService.CreateContextMenu(this, contextMenuPath);
			newControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(HandleKeyPress);
			newControl.SelectionManager.SelectionChanged += new EventHandler(SelectionChanged);
			newControl.Document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
			newControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
			newControl.TextArea.ClipboardHandler.CopyText += new CopyTextEventHandler(ClipboardHandlerCopyText);
			
			newControl.TextArea.IconBarMargin.Painted   += new MarginPaintEventHandler(PaintIconBarBreakPoints);
			newControl.TextArea.IconBarMargin.MouseDown += new MarginMouseEventHandler(IconBarMouseDown);
			newControl.MouseWheel                       += new MouseEventHandler(TextAreaMouseWheel);
			newControl.DoHandleMousewheel = false;
		}
		void CloseCodeCompletionWindow(object sender, EventArgs e)
		{
			codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
			codeCompletionWindow.Dispose();
			codeCompletionWindow = null;
		}
		void CloseInsightWindow(object sender, EventArgs e)
		{
			insightWindow.Closed -= new EventHandler(CloseInsightWindow);
			insightWindow.Dispose();
			insightWindow = null;
		}
		void TextAreaMouseWheel(object sender, MouseEventArgs e)
		{
			TextAreaControl textAreaControl = (TextAreaControl)sender;
			if (insightWindow != null && !insightWindow.IsDisposed && insightWindow.Visible) {
				insightWindow.HandleMouseWheel(e);
			} else if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed && codeCompletionWindow.Visible) {
				codeCompletionWindow.HandleMouseWheel(e);
			} else {
				textAreaControl.HandleMouseWheel(e);
			}
		}
		
		void ClipboardHandlerCopyText(object sender, CopyTextEventArgs e)
		{
			ICSharpCode.SharpDevelop.Gui.Pads.SideBarView.PutInClipboardRing(e.Text);
		}
		public override void OptionsChanged()
		{
			base.OptionsChanged();
			SharpDevelopTextEditorProperties sdtep = base.TextEditorProperties as SharpDevelopTextEditorProperties;
			
			if (sdtep != null) {
				if (!sdtep.ShowQuickClassBrowserPanel) {
 					RemoveQuickClassBrowserPanel();
				} else {
					ActivateQuickClassBrowserOnDemand();
				}
			}
		}

		protected virtual void IconBarMouseDown(AbstractMargin iconBar, Point mousepos, MouseButtons mouseButtons)
		{
			int realline = iconBar.TextArea.TextView.GetLogicalLine(mousepos);
			if (realline >= 0 && realline < iconBar.TextArea.Document.TotalNumberOfLines) {
				DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
				if (debuggerService != null && debuggerService.CurrentDebugger.SupportsExecutionControl) {
					debuggerService.ToggleBreakpointAt(FileName, realline + 1, 0);
					iconBar.TextArea.Refresh(iconBar);
				}
			}
		}
		
		protected virtual void PaintIconBarBreakPoints(AbstractMargin iconBar, Graphics g, Rectangle rect)
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			if (debuggerService == null)
				return;
			lock (debuggerService.Breakpoints) {
				foreach (Breakpoint breakpoint in debuggerService.Breakpoints) {
					try {
						if (Path.GetFullPath(breakpoint.FileName) == Path.GetFullPath(FileName)) {
							int lineNumber = iconBar.TextArea.Document.GetVisibleLine(breakpoint.LineNumber - 1);
							int yPos = (int)(lineNumber * iconBar.TextArea.TextView.FontHeight) - iconBar.TextArea.VirtualTop.Y;
							if (yPos >= rect.Y && yPos <= rect.Bottom) {
								((IconBarMargin)iconBar).DrawBreakpoint(g, yPos, breakpoint.IsEnabled);
							}
						}
					} catch (Exception) {}
				}
			}
		}
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetCaretPosition(ActiveTextAreaControl.TextArea.TextView.GetVisualColumn(ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column), ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column);
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		void SelectionChanged(object sender, EventArgs e)
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}
		
		void GenerateEditActions()
		{
			try {
				IEditAction[] actions = (IEditAction[])(AddInTreeSingleton.AddInTree.GetTreeNode(editActionsPath).BuildChildItems(this)).ToArray(typeof(IEditAction));
				
				foreach (IEditAction action in actions) {
					foreach (Keys key in action.Keys) {
						editactions[key] = action;
					}
				}
			} catch (TreePathNotFoundException) {
				Console.WriteLine(editActionsPath + " doesn't exists in the AddInTree");
			}
		}
		
		void RemoveQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel != null) {
				Controls.Remove(quickClassBrowserPanel);
				quickClassBrowserPanel.Dispose();
				quickClassBrowserPanel = null;
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			}
		}
		void ShowQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel == null) {
				quickClassBrowserPanel = new QuickClassBrowserPanel(this);
				Controls.Add(quickClassBrowserPanel);
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			}
		}
		public void ActivateQuickClassBrowserOnDemand()
		{
			SharpDevelopTextEditorProperties sdtep = base.TextEditorProperties as SharpDevelopTextEditorProperties;
			if (sdtep != null && sdtep.ShowQuickClassBrowserPanel && FileName != null) {
				IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				bool quickClassPanelActive = parserService.GetParser(FileName) != null;
				if (quickClassPanelActive) {
					ShowQuickClassBrowserPanel();
				} else {
					RemoveQuickClassBrowserPanel();
				}
			}
		}
		
		protected override void OnFileNameChanged(EventArgs e)
		{
			base.OnFileNameChanged(e);
			ActivateQuickClassBrowserOnDemand();
		}
		
//// Alex: routine for pulsing parser thread
//		protected void PulseParser() {
//			lock(DefaultParserService.ParserPulse) {
//				Monitor.Pulse(DefaultParserService.ParserPulse);
//			}
//		}
//// ALex: end of mod
		
		InsightWindow                 insightWindow        = null;
		internal CodeCompletionWindow codeCompletionWindow = null;
		
		// some other languages could support it
		protected virtual bool SupportsNew
		{
			get {
				return false;
			}
		}
		
		// some other languages could support it
		protected virtual bool SupportsDot
		{
			get {
				return false;
			}
		}

		// some other languages could support it
		protected virtual bool SupportsRoundBracket
		{
			get {
				return false;
			}
		}
		
		bool HandleKeyPress(char ch)
		{
			string fileName = FileName;
			if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
				codeCompletionWindow.ProcessKeyEvent(ch);
			}
			string ext = Path.GetExtension(fileName).ToLower();
			bool isCSharpOrVBNet = ext == ".cs" || ext == ".vb";
			bool isBoo = ext == ".boo"; // HACK: Boo wants CC, too...
			
			switch (ch) {
				case ' ':
					//TextEditorProperties.AutoInsertTemplates 
					string word = GetWordBeforeCaret();
					try {
						if ((isCSharpOrVBNet||SupportsNew) && word.ToLower() == "new") {
							if (((SharpDevelopTextEditorProperties)Document.TextEditorProperties).EnableCodeCompletion) {
								IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
								codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(((Form)WorkbenchSingleton.Workbench), this, this.FileName, new CodeCompletionDataProvider(true, true), ch);
								codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
								return false;
							}
						} else {
							if (word != null) {
								CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(FileName);
								if (templateGroup != null) {
									foreach (CodeTemplate template in templateGroup.Templates) {
										if (template.Shortcut == word) {
											if (word.Length > 0) {
												int newCaretOffset = DeleteWordBeforeCaret();
												//// set new position in text area
												ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(newCaretOffset);
											}
											
											InsertTemplate(template);
											return true;
										}
									}
								}
							}
						}
					} catch (Exception) {}
					goto case '.';
				case '<':
					try {
						if (((SharpDevelopTextEditorProperties)Document.TextEditorProperties).EnableCodeCompletion) {
							codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(((Form)WorkbenchSingleton.Workbench), this, fileName, new CommentCompletionDataProvider(), '<');
							codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
						}
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					return false;
				case '(':
					try {
						if (((SharpDevelopTextEditorProperties)Document.TextEditorProperties).EnableCodeCompletion) {
							if (insightWindow == null || insightWindow.IsDisposed) {
								insightWindow = new InsightWindow(((Form)WorkbenchSingleton.Workbench), this, fileName);
								insightWindow.Closed += new EventHandler(CloseInsightWindow);
							}
							insightWindow.AddInsightDataProvider(new MethodInsightDataProvider());
							insightWindow.ShowInsightWindow();
						}
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					return false;
				case '[':
					try {
						if (((SharpDevelopTextEditorProperties)Document.TextEditorProperties).EnableCodeCompletion) {
							if (insightWindow == null || insightWindow.IsDisposed) {
								insightWindow = new InsightWindow(((Form)WorkbenchSingleton.Workbench), this, fileName);
								insightWindow.Closed += new EventHandler(CloseInsightWindow);
							}
							
							insightWindow.AddInsightDataProvider(new IndexerInsightDataProvider());
							insightWindow.ShowInsightWindow();
						}
						
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					return false;
				case '.':
					try {
//						TextAreaPainter.IHaveTheFocusLock = true;
						if (((SharpDevelopTextEditorProperties)Document.TextEditorProperties).EnableCodeCompletion && (isCSharpOrVBNet||isBoo||SupportsDot)) {
							codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(((Form)WorkbenchSingleton.Workbench), this, fileName, CreateCodeCompletionDataProvider(false), ch);
							codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
						}
//						TextAreaPainter.IHaveTheFocusLock = false;
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					return false;
//// Alex: reparse file on ; - end of statement
//				case '}':
//				case ';':
//				case ')':	// reparse on closing bracket for foreach and for definitions
//					PulseParser();
//					return false;
//// Alex: end of mod
			}
			return false;
		}
		
		
		public string GetWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			return Document.GetText(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
		}
		
		public int DeleteWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			Document.Remove(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
			return start;
		}
		
		/// <remarks>
		/// This method inserts a code template at the current caret position
		/// </remarks>
		public void InsertTemplate(CodeTemplate template)
		{
			string selectedText = String.Empty;
			if (base.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected) {
				selectedText = base.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
				ActiveTextAreaControl.TextArea.Caret.Position = ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition;
				base.ActiveTextAreaControl.TextArea.SelectionManager.RemoveSelectedText();
			}
			int newCaretOffset   = ActiveTextAreaControl.TextArea.Caret.Offset;
			int finalCaretOffset = newCaretOffset;
			int firstLine        = Document.GetLineNumberForOffset(newCaretOffset);
			
			// save old properties, these properties cause strange effects, when not
			// be turned off (like insert curly braces or other formatting stuff)
			bool save1         = TextEditorProperties.AutoInsertCurlyBracket;
			IndentStyle save2  = TextEditorProperties.IndentStyle;
			TextEditorProperties.AutoInsertCurlyBracket = false;
			TextEditorProperties.IndentStyle            = IndentStyle.Auto;
			
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			string templateText = stringParserService.Parse(template.Text, new string[,] { { "Selection", selectedText } });
			
			BeginUpdate();
			for (int i =0; i < templateText.Length; ++i) {
				switch (templateText[i]) {
					case '|':
						finalCaretOffset = newCaretOffset;
						break;
					case '\r':
						break;
					case '\t':
//						new Tab().Execute(ActiveTextAreaControl.TextArea);
						break;
					case '\n':
						ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(newCaretOffset);
						new Return().Execute(ActiveTextAreaControl.TextArea);
						newCaretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
						break;
					default:
						ActiveTextAreaControl.TextArea.InsertChar(templateText[i]);
						newCaretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
						break;
				}
			}
			int lastLine = Document.GetLineNumberForOffset(newCaretOffset);
			EndUpdate();
			Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, firstLine, lastLine));
			Document.CommitUpdate();
			ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(finalCaretOffset);
			TextEditorProperties.IndentStyle = IndentStyle.Smart;
			Document.FormattingStrategy.IndentLines(ActiveTextAreaControl.TextArea, firstLine, lastLine);
			
			// restore old property settings
			TextEditorProperties.AutoInsertCurlyBracket = save1;
			TextEditorProperties.IndentStyle            = save2;
		}
		
		public void InitializeFormatter()
		{
			try {
				IFormattingStrategy[] formater = (IFormattingStrategy[])(AddInTreeSingleton.AddInTree.GetTreeNode(formatingStrategyPath).BuildChildItems(this)).ToArray(typeof(IFormattingStrategy));
				if (formater != null && formater.Length > 0) {
//					formater[0].Document = Document;
					Document.FormattingStrategy = formater[0];
				}
			} catch (TreePathNotFoundException) {
				Console.WriteLine(formatingStrategyPath + " doesn't exists in the AddInTree");
			}
		}
		
		public override string GetRangeDescription(int selectedItem, int itemCount)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			stringParserService.Properties["CurrentMethodNumber"]  = selectedItem.ToString("##");
			stringParserService.Properties["NumberOfTotalMethods"] = itemCount.ToString("##");
			return stringParserService.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.InsightWindow.NumberOfText}");
		}
		
//		public override IDeclarationViewWindow CreateDeclarationViewWindow()
//		{
//			return new HtmlDeclarationViewWindow();
//		}
//		
	}
}
