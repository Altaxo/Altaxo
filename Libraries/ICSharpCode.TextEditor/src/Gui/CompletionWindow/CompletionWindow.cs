// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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
	public class CompletionWindow : Form
	{
		const  int  DeclarationIndent  = 1;
		static Size ListViewBorderSize = SystemInformation.Border3DSize;
		
		ICompletionDataProvider completionDataProvider;
		TextEditorControl       control;
		ListView                listView              = new MyListView();
		DeclarationViewWindow   declarationviewwindow = new DeclarationViewWindow();
		
		int    insertLength = 0;
		
		class MyListView : ListView
		{	
			protected override bool ProcessDialogKey(Keys keyData)
			{
				if (keyData == Keys.Tab) {
					OnItemActivate(null);
				}
				return base.ProcessDialogKey(keyData);
			}
		}
		
		string GetTypedString()
		{
			return control.Document.GetText(control.ActiveTextAreaControl.Caret.Offset - insertLength, insertLength);
		}
		
		void DeleteInsertion()
		{
			if (insertLength > 0) {
				int startOffset = control.ActiveTextAreaControl.Caret.Offset - insertLength;
				control.Document.Remove(startOffset, insertLength);
				control.ActiveTextAreaControl.Caret.Position = control.Document.OffsetToPosition(startOffset);
				control.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, new Point(0, control.Document.GetLineNumberForOffset(control.ActiveTextAreaControl.Caret.Offset))));
				control.Document.CommitUpdate();
			}
		}
				
		void ListKeypressEvent(object sender, KeyPressEventArgs ex)
		{
			switch (ex.KeyChar) {
				case (char)27: // Escape
					LostFocusListView(null, null);
					return;
				case '\b': //Backspace
					new ICSharpCode.TextEditor.Actions.Backspace().Execute(control.ActiveTextAreaControl.TextArea);
					if (insertLength > 0) {
						--insertLength;
					} else {
						// no need to delete here (insertLength <= 0)
						LostFocusListView(null, null);
					}
					break;
				default:
					if (ex.KeyChar != '_' && !Char.IsLetterOrDigit(ex.KeyChar)) {
						if (listView.SelectedItems.Count > 0) {
							ActivateItem(null, null);
						} else {
							LostFocusListView(null, null);
						}
						
						control.ActiveTextAreaControl.TextArea.SimulateKeyPress(ex.KeyChar);
						return;
					} else {
						control.ActiveTextAreaControl.TextArea.InsertChar(ex.KeyChar);
						++insertLength;
					}
					break;
			}
			
			// select the current typed word
			int lastSelected = -1;
			int capitalizationIndex = -1;
			
			string typedString = GetTypedString();
			for (int i = 0; i < listView.Items.Count; ++i) {
				
				if (listView.Items[i].Text.ToUpper().StartsWith(typedString.ToUpper())) {
					int currentCapitalizationIndex = 0;
					for (int j = 0; j < typedString.Length && j < listView.Items[i].Text.Length; ++j) {
						if (typedString[j] == listView.Items[i].Text[j]) {
							++currentCapitalizationIndex;
						}
					}
					
					if (currentCapitalizationIndex > capitalizationIndex) {
						lastSelected = i;
						capitalizationIndex = currentCapitalizationIndex;
					}
				}
			}
			
			listView.SelectedItems.Clear();
			if (lastSelected != -1) {
				listView.Items[lastSelected].Focused  = true;
				listView.Items[lastSelected].Selected = true;
				listView.EnsureVisible(lastSelected);
			}
			ex.Handled = true;
		}
		
		void InitializeControls()
		{
			Width    = 340;
			Height   = 210 - 85;

			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			
			listView.Dock        = DockStyle.Fill;
			listView.View        = View.Details;
			listView.AutoArrange = true;
			listView.Alignment   = ListViewAlignment.Left;
			listView.HeaderStyle = ColumnHeaderStyle.None;
			listView.Sorting     = SortOrder.Ascending;
			listView.MultiSelect = false;
			listView.FullRowSelect  = true;
			listView.HideSelection  = false;
//			listView.Font           = ICSharpCode.TextEditor.Document.FontContainer.DefaultFont;
			listView.SmallImageList = completionDataProvider.ImageList;
			listView.KeyPress += new KeyPressEventHandler(ListKeypressEvent);
			
			listView.LostFocus            += new EventHandler(LostFocusListView);
			listView.ItemActivate         += new EventHandler(ActivateItem);
			listView.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			this.Controls.Add(listView);			

			/*
			Panel buttonPanel = new Panel();
			buttonPanel.Dock = DockStyle.Bottom;
			buttonPanel.Size = new Size(100, 30);
			
			this.Controls.Add(buttonPanel);
			*/
		}
	
		/// <remarks>
		/// Shows the filled completion window, if it has no items it isn't shown.
		/// </remarks>
		public void ShowCompletionWindow(char firstChar)
		{
			FillList(true, firstChar);
			
			if (listView.Items.Count > 0) {
				Rectangle size = listView.Items[0].GetBounds(ItemBoundsPortion.Entire);
				int clientHeight = size.Height * Math.Min(10, listView.Items.Count);
				int clientWidth = size.Width;
				
				if (listView.Items.Count > 10) {
					clientWidth += SystemInformation.VerticalScrollBarWidth;
					listView.Scrollable = true;
				}
				else {
					listView.Scrollable = false;
				}
				
				// ListView.ClientSize does not work properly. Thus...
				Size nonClientArea = new Size(ListViewBorderSize.Width * 2,
				                              ListViewBorderSize.Height * 2);
				Size totalSize = new Size(clientWidth, clientHeight) +
					nonClientArea;
				
				listView.Size = totalSize; ClientSize = totalSize;

				declarationviewwindow.Show();
				Show();
				
				listView.Select();
				listView.Focus();
				
				listView.Items[0].Focused = listView.Items[0].Selected = true;				
//				control.TextAreaPainter.IHaveTheFocus = true;
			} else {
				control.Focus();
			}
		}
		string fileName;
		
		/// <remarks>
		/// Creates a new Completion window and puts it location under the caret
		/// </remarks>
		public CompletionWindow(TextEditorControl control, string fileName, ICompletionDataProvider completionDataProvider)
		{
			this.fileName = fileName;
			this.completionDataProvider = completionDataProvider;
			this.control                = control;
			
			Point caretPos  = control.ActiveTextAreaControl.Caret.Position;
			Point visualPos = new Point(control.ActiveTextAreaControl.TextArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X) + control.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.X,
			          (int)((1 + caretPos.Y) * control.ActiveTextAreaControl.TextArea.TextView.FontHeight) - control.ActiveTextAreaControl.TextArea.VirtualTop.Y - 1 + control.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Y);
			
	 		Location = control.ActiveTextAreaControl.PointToScreen(visualPos);
			InitializeControls();
		}
		
		/// <remarks>
		/// Creates a new Completion window at a given location
		/// </remarks>
		CompletionWindow(TextEditorControl control, Point location, ICompletionDataProvider completionDataProvider)
		{
			this.completionDataProvider = completionDataProvider;
			this.control                = control;
			Location = location;
			InitializeControls();
		}
		
		void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0) {
				ICompletionData data = (ICompletionData)listView.SelectedItems[0].Tag;				
				
				if (data.Description != null) {
					listView.EnsureVisible(listView.SelectedIndices[0]);
					Point pos = new Point(Bounds.Right + DeclarationIndent,
						Bounds.Top + listView.GetItemRect(listView.SelectedIndices[0]).Y +
						ListViewBorderSize.Height);
					declarationviewwindow.Location    = pos;
					declarationviewwindow.Description = data.Description;
				} else {
					declarationviewwindow.Size = new Size(0, 0);
				}
			}
		}
		
		void ActivateItem(object sender, EventArgs e)
		{
//			control.TextAreaPainter.IHaveTheFocusLock = true;
			if (listView.SelectedItems.Count > 0) {
				ICompletionData data = (ICompletionData)listView.SelectedItems[0].Tag;
				control.BeginUpdate();
				DeleteInsertion();
				data.InsertAction(control);
				LostFocusListView(sender, e);
				control.EndUpdate();
			}
//			control.TextAreaPainter.IHaveTheFocusLock = false;
		}
		
		void LostFocusListView(object sender, EventArgs e)
		{
			control.Focus();
			declarationviewwindow.Close();
			Close();
		}
		
		void FillList(bool firstTime, char ch)
		{
			ICompletionData[] completionData = completionDataProvider.GenerateCompletionData(fileName, control.ActiveTextAreaControl.TextArea, ch);
			if (completionData == null || completionData.Length == 0) {
				return;
			}
			if (firstTime && completionData.Length > 0) {
				int columnHeaders = completionData[0].Text.Length;
				for (int i = 0; i < columnHeaders; ++i) {
					ColumnHeader header = new ColumnHeader();
					header.Width = -1;
					listView.Columns.Add(header);
				}
			}

			listView.BeginUpdate();
			foreach (ICompletionData data in completionData) {
				ListViewItem newItem = new ListViewItem(data.Text, data.ImageIndex);
				newItem.Tag = data;
			
				listView.Items.Add(newItem);
			}
			listView.EndUpdate();
		}
	}
}
