// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
 
namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class is used for a basic text area control
	/// </summary>
//	[ToolboxBitmap("ICSharpCode.TextEditor.TextEditorControl.bmp")]
	[ToolboxItem(true)]
	public class TextEditorControl : TextEditorControlBase
	{
		TextAreaControl primaryTextArea;
		Splitter        textAreaSplitter  = null;
		TextAreaControl secondaryTextArea = null;
		
		public PrintDocument PrintDocument {
			get {
				PrintDocument printDocument = new PrintDocument();
				printDocument.PrintPage += new PrintPageEventHandler(this.PrintPage);
				return null;
			}
		}
		
		public TextAreaControl ActiveTextAreaControl {
			get {
				return primaryTextArea;
			}
		}
		
		public TextEditorControl()
		{
			Document = (new DocumentFactory()).CreateDocument();
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
			
			primaryTextArea  = new TextAreaControl(this);
			primaryTextArea.Dock = DockStyle.Fill;
			Controls.Add(primaryTextArea);
			InitializeTextAreaControl(primaryTextArea);
			ResizeRedraw = true;
			Document.UpdateCommited += new EventHandler(CommitUpdateRequested);
			OptionsChanged();
		}
		
		protected virtual void InitializeTextAreaControl(TextAreaControl newControl)
		{
		}
		
		public override void OptionsChanged()
		{
			primaryTextArea.OptionsChanged();
			if (secondaryTextArea != null) {
				secondaryTextArea.OptionsChanged();
			}
		}
		
		public void Split()
		{
			if (secondaryTextArea == null) {
				secondaryTextArea = new TextAreaControl(this);
				secondaryTextArea.Dock = DockStyle.Bottom;
				secondaryTextArea.Height = Height / 2;
				textAreaSplitter =  new Splitter();
				textAreaSplitter.BorderStyle = BorderStyle.FixedSingle ;
				textAreaSplitter.Height = 8;
				textAreaSplitter.Dock = DockStyle.Bottom;
				Controls.Add(textAreaSplitter);
				Controls.Add(secondaryTextArea);
				InitializeTextAreaControl(secondaryTextArea);
				secondaryTextArea.OptionsChanged();
			} else {
				Controls.Remove(secondaryTextArea);
				Controls.Remove(textAreaSplitter);
				
				secondaryTextArea.Dispose();
				textAreaSplitter.Dispose();
				secondaryTextArea = null;
				textAreaSplitter  = null;
			}
		}
		
		public void Undo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanUndo) {
				BeginUpdate();
				Document.UndoStack.Undo();
				
				Document.UpdateQueue.Clear();
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public void Redo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanRedo) {
				BeginUpdate();
				Document.UndoStack.Redo();
				
				Document.UpdateQueue.Clear();
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public void SetHighlighting(string name)
		{
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(name);
		}
		
		
#region Update Methods
		public override void EndUpdate()
		{
			base.EndUpdate();
			Document.CommitUpdate();
		}
		
		void CommitUpdateRequested(object sender, EventArgs e)
		{
			if (IsUpdating) {
				return;
			}
			foreach (TextAreaUpdate update in Document.UpdateQueue) {
				switch (update.TextAreaUpdateType) {
					case TextAreaUpdateType.PositionToEnd:
						this.primaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.PositionToLineEnd:
					case TextAreaUpdateType.SingleLine:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.SinglePosition:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						}
						break;
					case TextAreaUpdateType.LinesBetween:
						this.primaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						}
						break;
					case TextAreaUpdateType.WholeTextArea:
						this.primaryTextArea.TextArea.Invalidate();
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.Invalidate();
						}
						break;
				}
			}
			Document.UpdateQueue.Clear();
			this.primaryTextArea.TextArea.Update();
			if (this.secondaryTextArea != null) {
				this.secondaryTextArea.TextArea.Update();
			}
//			Console.WriteLine("-------END");
		}
#endregion
		
#region Printing routines
		void PrintPage(object sender, PrintPageEventArgs ev)
		{
//			float leftMargin = ev.MarginBounds.Left;
//			float topMargin  = ev.MarginBounds.Top;
//			
//			// Calculate the number of lines per page.
//			int linesPerPage = ev.MarginBounds.Height / this.TextEditorProperties.Font.GetHeight(ev.Graphics);
			
			//TODO
		}
#endregion
	}
}
