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
using System.Diagnostics;

using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class CodeCompletionWindow : AbstractCompletionWindow
	{
		static ICompletionData[] completionData;
		CodeCompletionListView   codeCompletionListView;
		VScrollBar    vScrollBar = new VScrollBar();
		
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
			codeCompletionListView.DoubleClick += new EventHandler(CodeCompletionListViewDoubleClick);
			codeCompletionListView.Click  += new EventHandler(CodeCompletionListViewClick);
			Controls.Add(codeCompletionListView);
			
			if (completionData.Length > 10) {
				vScrollBar.Dock = DockStyle.Right;
				vScrollBar.Minimum = 0;
				vScrollBar.Maximum = completionData.Length - 8;
				vScrollBar.SmallChange = 1;
				vScrollBar.LargeChange = 3;
				codeCompletionListView.FirstItemChanged += new EventHandler(CodeCompletionListViewFirstItemChanged);
				Controls.Add(vScrollBar);
			}
			
			this.drawingSize = new Size(codeCompletionListView.ItemHeight * 10, codeCompletionListView.ItemHeight * Math.Min(10, completionData.Length));
			SetLocation();
			
			declarationViewWindow = new DeclarationViewWindow(parentForm);
			SetDeclarationViewLocation();
			declarationViewWindow.ShowDeclarationViewWindow();
			control.Focus();
			CodeCompletionListViewSelectedItemChanged(this, EventArgs.Empty);
			if (completionDataProvider.PreSelection != null) {
				CaretOffsetChanged(this, EventArgs.Empty);
			}
			
			vScrollBar.Scroll += new ScrollEventHandler(DoScroll);
		}
		
		public void HandleMouseWheel(MouseEventArgs e)
		{
			int MAX_DELTA  = 120; // basically it's constant now, but could be changed later by MS
			int multiplier = Math.Abs(e.Delta) / MAX_DELTA;
			
			int newValue;
			if (System.Windows.Forms.SystemInformation.MouseWheelScrollLines > 0) {
				newValue = this.vScrollBar.Value - (control.TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * System.Windows.Forms.SystemInformation.MouseWheelScrollLines * vScrollBar.SmallChange * multiplier;
			} else {
				newValue = this.vScrollBar.Value - (control.TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * vScrollBar.LargeChange;
			}
			vScrollBar.Value = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum, newValue));
			DoScroll(this, null);
		}
		
		void CodeCompletionListViewFirstItemChanged(object sender, EventArgs e)
		{
			vScrollBar.Value = Math.Min(vScrollBar.Maximum, codeCompletionListView.FirstItem);
		}
		
		void SetDeclarationViewLocation()
		{
			Console.WriteLine("SET DECLARATION VIEW LOCATION.");
			//  This method uses the side with more free space
			int leftSpace = Bounds.Left - workingScreen.Left;
			int rightSpace = workingScreen.Right - Bounds.Right;
			Point pos;
			// The declaration view window has better line break when used on
			// the right side, so prefer the right side to the left.
			if (rightSpace * 2 > leftSpace)
				pos = new Point(Bounds.Right, Bounds.Top);
			else
				pos = new Point(Bounds.Left - declarationViewWindow.Width, Bounds.Top);
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
			if (data != null && data.Description != null) {
				declarationViewWindow.Description = data.Description;
				SetDeclarationViewLocation();
			} else {
				declarationViewWindow.Size = new Size(0, 0);
			}
		}
		
		public override bool ProcessKeyEvent(char ch)
		{
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
			//Console.WriteLine("StartOffset {0} endOffset {1} - Offset {2}", startOffset, endOffset, offset);
			if (offset < startOffset || offset > endOffset) {
				Close();
			} else {
				codeCompletionListView.SelectItemWithStart(control.Document.GetText(startOffset, offset - startOffset));
			}
		}
		
		protected void DoScroll(object sender, ScrollEventArgs sea)
		{
			codeCompletionListView.FirstItem = vScrollBar.Value;
			codeCompletionListView.Refresh();
			control.ActiveTextAreaControl.TextArea.Focus();
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
				case Keys.Home:
					codeCompletionListView.SelectIndex(0);
					return true;
				case Keys.End:
					codeCompletionListView.SelectIndex(completionData.Length-1);
					return true;
				case Keys.PageDown:
					codeCompletionListView.PageDown();
					return true;
				case Keys.PageUp:
					codeCompletionListView.PageUp();
					return true;
				case Keys.Down:
				case Keys.Right:
					codeCompletionListView.SelectNextItem();
					return true;
				case Keys.Up:
				case Keys.Left:
					codeCompletionListView.SelectPrevItem();
					return true;
				case Keys.Tab:
				case Keys.Return:
					InsertSelectedItem();
					return true;
			}
			return base.ProcessTextAreaKey(keyData);
		}
		
		void CodeCompletionListViewDoubleClick(object sender, EventArgs e)
		{
			InsertSelectedItem();
		}
		
		void CodeCompletionListViewClick(object sender, EventArgs e)
		{
			control.ActiveTextAreaControl.TextArea.Focus();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			codeCompletionListView.Close();
			base.OnClosed(e);
			declarationViewWindow.Close();
			declarationViewWindow.Dispose();
		}
		
		void InsertSelectedItem()
		{
			ICompletionData data = codeCompletionListView.SelectedCompletionData;
			if (data != null) {
				control.BeginUpdate();
				
				if (endOffset - startOffset > 0) {
					//Console.WriteLine("start {0} length {1}", startOffset, endOffset - startOffset);
					control.Document.Remove(startOffset, endOffset - startOffset);
					control.ActiveTextAreaControl.Caret.Position = control.Document.OffsetToPosition(startOffset);
				}
				data.InsertAction(control);
				control.EndUpdate();
			}
			Close();
		}
	}
}
