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
using System.Runtime.InteropServices;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	/// <summary>
	/// Description of AbstractCompletionWindow.	
	/// </summary>
	public abstract class AbstractCompletionWindow : System.Windows.Forms.Form
	{
		protected TextEditorControl control;
		protected string            fileName;
		protected Size              drawingSize;
		Form parentForm;
		Rectangle workingScreen;
		
		protected AbstractCompletionWindow(Form parentForm, TextEditorControl control, string fileName)
		{
			workingScreen = Screen.GetWorkingArea(parentForm);
			this.parentForm = parentForm;
			this.control  = control;
			this.fileName = fileName;
			
			SetLocation();
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar   = false;
			Size            = new Size(1, 1);
		}
		
		protected virtual void SetLocation()
		{
			TextArea textArea = control.ActiveTextAreaControl.TextArea;
			Point caretPos  = textArea.Caret.Position;
			
			int xpos = textArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X);
			int rulerHeight = textArea.TextEditorProperties.ShowHorizontalRuler ? textArea.TextView.FontHeight : 0;
			Point pos = new Point(textArea.TextView.DrawingPosition.X + xpos,
			                      textArea.TextView.DrawingPosition.Y + (textArea.Document.GetVisibleLine(caretPos.Y)) * textArea.TextView.FontHeight - textArea.TextView.TextArea.VirtualTop.Y + textArea.TextView.FontHeight + rulerHeight);
			
			Point location = control.ActiveTextAreaControl.PointToScreen(pos);
			
			// set bounds
			Rectangle bounds = new Rectangle(location, drawingSize);
			
			if (!workingScreen.Contains(bounds)) {
				if (bounds.Right > workingScreen.Right) {
					bounds.X = workingScreen.Right - bounds.Width;
				}
				if (bounds.Left < workingScreen.Left) {
					bounds.X = workingScreen.Left;
				}
				if (bounds.Top < workingScreen.Top) {
					bounds.Y = workingScreen.Top;
				}
				if (bounds.Bottom > workingScreen.Bottom) {
					bounds.Y = bounds.Y - bounds.Height - control.ActiveTextAreaControl.TextArea.TextView.FontHeight;
					if (bounds.Bottom > workingScreen.Bottom) {
						bounds.Y = workingScreen.Bottom - bounds.Height;
					}
				}
			}
			Bounds = bounds;
		}
		
		public readonly static int SW_SHOWNA = 8;
		
		[DllImport("user32")]
		public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
		
		protected void ShowCompletionWindow()
		{
			Owner = parentForm;
			
			Enabled = true;
			AbstractCompletionWindow.ShowWindow(base.Handle, AbstractCompletionWindow.SW_SHOWNA);
			
			control.Focus();
			
			if (parentForm != null) {
				parentForm.LocationChanged += new EventHandler(this.ParentFormLocationChanged);
			}
			
			control.ActiveTextAreaControl.VScrollBar.ValueChanged     += new EventHandler(ParentFormLocationChanged);
			control.ActiveTextAreaControl.HScrollBar.ValueChanged     += new EventHandler(ParentFormLocationChanged);
			control.ActiveTextAreaControl.TextArea.DoProcessDialogKey += new DialogKeyProcessor(ProcessTextAreaKey);
			control.ActiveTextAreaControl.Caret.PositionChanged       += new EventHandler(CaretOffsetChanged);
			control.ActiveTextAreaControl.TextArea.LostFocus          += new EventHandler(this.TextEditorLostFocus);
			control.Resize += new EventHandler(ParentFormLocationChanged);
		}
		
		void ParentFormLocationChanged(object sender, EventArgs e)
		{
			SetLocation();
		}
		
		public virtual bool ProcessKeyEvent(char ch)
		{
			return false;
		}
		
		protected virtual bool ProcessTextAreaKey(Keys keyData)
		{
			if (!Visible) {
				return false;
			}
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
			}
			return false;
		}
		
		protected virtual void CaretOffsetChanged(object sender, EventArgs e)
		{
		}
		
		protected void TextEditorLostFocus(object sender, EventArgs e)
		{
			if (!control.ActiveTextAreaControl.TextArea.Focused && !this.ContainsFocus) {
				Close();
			}
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			
			// take out the inserted methods
			parentForm.LocationChanged -= new EventHandler(this.ParentFormLocationChanged);
			
			control.ActiveTextAreaControl.VScrollBar.ValueChanged     -= new EventHandler(ParentFormLocationChanged);
			control.ActiveTextAreaControl.HScrollBar.ValueChanged     -= new EventHandler(ParentFormLocationChanged);
			
			control.ActiveTextAreaControl.TextArea.LostFocus          -= new EventHandler(this.TextEditorLostFocus);
			control.ActiveTextAreaControl.Caret.PositionChanged       -= new EventHandler(CaretOffsetChanged);
			control.ActiveTextAreaControl.TextArea.DoProcessDialogKey -= new DialogKeyProcessor(ProcessTextAreaKey);
			control.Resize -= new EventHandler(ParentFormLocationChanged);
			Dispose();
		}
	}
}
