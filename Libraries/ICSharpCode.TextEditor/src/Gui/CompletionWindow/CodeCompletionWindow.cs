// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class CodeCompletionWindow : AbstractCompletionWindow
	{
		static ICompletionData[] completionData;
		CodeCompletionListView   codeCompletionListView;
		
		int                      startOffset;
		int                      endOffset;
		DeclarationViewWindow    declarationViewWindow = null;
		Rectangle workingScreen;
			
		public static CodeCompletionWindow ShowCompletionWindow(Form parent, TextEditorControl control, string fileName, ICompletionDataProvider completionDataProvider, char firstChar)
		{
			completionData = completionDataProvider.GenerateCompletionData(fileName, control.ActiveTextAreaControl.TextArea, firstChar);
			if (completionData == null || completionData.Length == 0) {
				return null;
			}
			CodeCompletionWindow codeCompletionWindow = new CodeCompletionWindow(completionDataProvider, parent, control, fileName);
			codeCompletionWindow.ShowCompletionWindow();
			return codeCompletionWindow;
		}
		
		CodeCompletionWindow(ICompletionDataProvider completionDataProvider, Form parentForm, TextEditorControl control, string fileName) : base(parentForm, control, fileName)
		{
			workingScreen = Screen.GetWorkingArea(Location);
			startOffset = control.ActiveTextAreaControl.Caret.Offset + 1;
			endOffset   = startOffset;
			if (completionDataProvider.PreSelection != null) {
				startOffset -= completionDataProvider.PreSelection.Length + 1;
				endOffset--;
			} 
			
			codeCompletionListView = new CodeCompletionListView(completionData);
			codeCompletionListView.ImageList = completionDataProvider.ImageList;
			codeCompletionListView.Dock = DockStyle.Fill;
			codeCompletionListView.SelectedItemChanged += new EventHandler(CodeCompletionListViewSelectedItemChanged);
			Controls.Add(codeCompletionListView);
			
//			VScrollBar vScrollBar = new VScrollBar();
//			vScrollBar.Dock = DockStyle.Right;
//			vScrollBar.Minimum = 0;
//			vScrollBar.Maximum = completionData.Length;
//			vScrollBar.SmallChange = 1;
//			vScrollBar.LargeChange = 3;
//			vScrollBar.Enabled = true;
//			vScrollBar.Visible = true;
//			Controls.Add(vScrollBar);
//			Console.WriteLine(vScrollBar);
			
			this.drawingSize = new Size(180, codeCompletionListView.ItemHeight * Math.Min(10, completionData.Length));
			SetLocation();
			
			declarationViewWindow = new DeclarationViewWindow(parentForm);
			SetDeclarationViewLocation();
			declarationViewWindow.ShowDeclarationViewWindow();
			control.Focus();
			CodeCompletionListViewSelectedItemChanged(this, EventArgs.Empty);
			if (completionDataProvider.PreSelection != null) {
				CaretOffsetChanged(this, EventArgs.Empty);
			}
		}
		
		void SetDeclarationViewLocation()
		{
			Point pos = new Point(Bounds.Right, Bounds.Top);
			if (pos.X + declarationViewWindow.Width >= workingScreen.Right) {
				pos = new Point(Bounds.Left - declarationViewWindow.Width, pos.Y);
			}
			if (declarationViewWindow.Location != pos) {
				declarationViewWindow.Location = pos;
			}
		}
		
		protected override void SetLocation()
		{
			base.SetLocation();
			if (declarationViewWindow != null) {
				SetDeclarationViewLocation();
			}
		}
		
		void CodeCompletionListViewSelectedItemChanged(object sender, EventArgs e)
		{
			ICompletionData data = codeCompletionListView.SelectedCompletionData;
			if (data != null) {
				declarationViewWindow.Description = data.Description;
				SetDeclarationViewLocation();
			} else {
				declarationViewWindow.Size = new Size(0, 0);
			}
		}
		
		public override bool ProcessKeyEvent(char ch)
		{
			Console.WriteLine("PRocesS: " + ch);
			if (!Char.IsLetterOrDigit(ch) && ch != '_') {
				InsertSelectedItem();
				return false;
			}
			++endOffset;
			return base.ProcessKeyEvent(ch);
		}
		
		protected override void CaretOffsetChanged(object sender, EventArgs e)
		{
			int offset = control.ActiveTextAreaControl.Caret.Offset;
			if (offset < startOffset || offset > endOffset) {
				Close();
			} else {
				codeCompletionListView.SelectItemWithStart(control.Document.GetText(startOffset, offset - startOffset));
			}
		}
		
		protected override bool ProcessTextAreaKey(Keys keyData)
		{
			if (!Visible) {
				return false;
			}
			
			switch (keyData) {
				case Keys.Back:
					--endOffset;
					if (endOffset < startOffset) {
						Close();
					}
					return false;
				case Keys.Delete:
					if (control.ActiveTextAreaControl.Caret.Offset <= endOffset) {
						--endOffset;
					}
					if (endOffset < startOffset) {
						Close();
					}
					return false;
				case Keys.PageDown:
					codeCompletionListView.PageDown();
					return true;
				case Keys.PageUp:
					codeCompletionListView.PageUp();
					return true;
				case Keys.Down:
					codeCompletionListView.SelectNextItem();
					return true;
				case Keys.Up:
					codeCompletionListView.SelectPrevItem();
					return true;
				case Keys.Tab:
				case Keys.Return:
					InsertSelectedItem();
					return true;
			}
			return base.ProcessTextAreaKey(keyData);
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			declarationViewWindow.Close();
			declarationViewWindow.Dispose();
		}
		
		void InsertSelectedItem()
		{
			ICompletionData data = codeCompletionListView.SelectedCompletionData;
			if (data != null) {
				control.BeginUpdate();
				control.Document.Remove(startOffset, endOffset - startOffset);
				control.ActiveTextAreaControl.Caret.Position = control.Document.OffsetToPosition(startOffset);
				data.InsertAction(control);
				control.EndUpdate();
			}
			Close();
		}
	}
}
