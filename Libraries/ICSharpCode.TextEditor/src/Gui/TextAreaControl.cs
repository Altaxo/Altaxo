// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class paints the textarea.
	/// </summary>
	[ToolboxItem(false)]
	public class TextAreaControl : Panel
	{
		TextEditorControl         motherTextEditorControl;
		
		VScrollBar vScrollBar = new VScrollBar();
		HScrollBar hScrollBar = new HScrollBar();
		TextArea   textArea;
		
		public TextArea TextArea {
			get {
				return textArea;
			}
		}
		public SelectionManager SelectionManager {
			get {
				return textArea.SelectionManager;
			}
		}
		public Caret Caret {
			get {
				return textArea.Caret;
			}
		}

		
		[Browsable(false)]
		public IDocument Document {
			get {
				return motherTextEditorControl.Document;
			}
		}
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return motherTextEditorControl.TextEditorProperties;
			}
		}
		
		public TextAreaControl(TextEditorControl motherTextEditorControl)
		{
			this.motherTextEditorControl = motherTextEditorControl;
			
			this.textArea                = new TextArea(motherTextEditorControl, this);
			Controls.Add(textArea);
			
			vScrollBar.ValueChanged += new EventHandler(VScrollBarValueChanged);
			Controls.Add(this.vScrollBar);
			
			hScrollBar.ValueChanged += new EventHandler(HScrollBarValueChanged);
			Controls.Add(this.hScrollBar);
			ResizeRedraw = true;
			
			Document.DocumentChanged += new DocumentEventHandler(AdjustScrollBars);
		}
		
		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);
			textArea.Bounds = new Rectangle(0, 0,
			                                Width - SystemInformation.HorizontalScrollBarArrowWidth,
			                                Height - SystemInformation.VerticalScrollBarArrowHeight);
			SetScrollBarBounds();
		}
		
		public void SetScrollBarBounds()
		{
			vScrollBar.Bounds = new Rectangle(textArea.Bounds.Right, 0, SystemInformation.HorizontalScrollBarArrowWidth, Height - SystemInformation.VerticalScrollBarArrowHeight);
			hScrollBar.Bounds = new Rectangle(0, Height - SystemInformation.VerticalScrollBarArrowHeight, 
			                                  Width - SystemInformation.HorizontalScrollBarArrowWidth, 
			                                  SystemInformation.VerticalScrollBarArrowHeight);
		}
		
		public void AdjustScrollBars(object sender, DocumentEventArgs e)
		{
			vScrollBar.Minimum = 0;
			vScrollBar.Maximum = (Document.TotalNumberOfLines + textArea.TextView.VisibleLineCount - 2) * Document.TextEditorProperties.Font.Height;
			
//			int max = 0;
//			foreach (ISegment lineSegment in Document.ArrayList) {
//				max = Math.Max(lineSegment.Length, max);
//			}
			hScrollBar.Minimum = 0;
			hScrollBar.Maximum = (int)(1000 * textArea.TextView.GetWidth(' ')) ;//Math.Max(0, max + textArea.TextView.VisibleColumnCount - 1);
			
			vScrollBar.LargeChange = Math.Max(0, textArea.TextView.DrawingPosition.Height);
			vScrollBar.SmallChange = Math.Max(0, textArea.TextView.FontHeight);
			
			hScrollBar.LargeChange = Math.Max(0, textArea.TextView.DrawingPosition.Width);
			hScrollBar.SmallChange = Math.Max(0, (int)textArea.TextView.GetWidth(' '));
		}
		
		public void OptionsChanged()
		{
			textArea.OptionsChanged();
			
			AdjustScrollBars(null, null);
		}
		
		void VScrollBarValueChanged(object sender, EventArgs e)
		{
			textArea.VirtualTop = new Point(textArea.VirtualTop.X, vScrollBar.Value);
			textArea.Invalidate();
		}
		
		void HScrollBarValueChanged(object sender, EventArgs e)
		{
			textArea.VirtualTop = new Point(hScrollBar.Value, textArea.VirtualTop.Y);
			textArea.Invalidate();
		}
		
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			int MAX_DELTA  = 120; // basically it's constant now, but could be changed later by MS
			int multiplier = Math.Abs(e.Delta) / MAX_DELTA;
			
			int newValue;
			if (System.Windows.Forms.SystemInformation.MouseWheelScrollLines > 0) {
				newValue = this.vScrollBar.Value - (TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * System.Windows.Forms.SystemInformation.MouseWheelScrollLines * vScrollBar.SmallChange * multiplier ;
			} else {
				newValue = this.vScrollBar.Value - (TextEditorProperties.MouseWheelScrollDown ? 1 : -1) * Math.Sign(e.Delta) * vScrollBar.LargeChange;
			}
			vScrollBar.Value = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum, newValue));
		}
		
		public void ScrollToCaret()
		{
			int curCharMin  = (int)(this.hScrollBar.Value - this.hScrollBar.Minimum);
			int curCharMax  = curCharMin + textArea.TextView.VisibleColumnCount;
			
			int pos         = textArea.TextView.GetVisualColumn(textArea.Caret.Line, textArea.Caret.Column);
			
			if (textArea.TextView.VisibleColumnCount < 0) {
				hScrollBar.Value = 0;
			} else {
				if (pos < curCharMin) {
					hScrollBar.Value = (int)(Math.Max(0, pos - scrollMarginHeight));
				} else {
					if (pos > curCharMax) {
						hScrollBar.Value = (int)Math.Max(0, Math.Min(hScrollBar.Maximum, (pos - textArea.TextView.VisibleColumnCount + scrollMarginHeight)));
					}
				}
			}
			ScrollTo(textArea.Caret.Line);
		}
		
		int scrollMarginHeight  = 3;
		
		public void ScrollTo(int line)
		{
			line = Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line));
			line = Document.GetLogicalLine(line);
			
			int curLineMin = textArea.TextView.FirstVisibleLine;
			if (line - scrollMarginHeight < curLineMin) {
				this.vScrollBar.Value =  Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line - scrollMarginHeight)) * textArea.TextView.FontHeight;
			} else {
				int curLineMax = curLineMin + this.textArea.TextView.VisibleLineCount;
				if (line + scrollMarginHeight > curLineMax) {
					this.vScrollBar.Value = Math.Min(Document.TotalNumberOfLines - 1, 
					                                 line - this.textArea.TextView.VisibleLineCount + scrollMarginHeight) * textArea.TextView.FontHeight;
				}
			}
		}
		
		public void JumpTo(int line, int column)
		{
			textArea.SelectionManager.ClearSelection();
			textArea.Caret.Position = new Point(column, line);
			textArea.SetDesiredColumn();
			ScrollToCaret();
			textArea.Focus();
		}
		
	}
}
