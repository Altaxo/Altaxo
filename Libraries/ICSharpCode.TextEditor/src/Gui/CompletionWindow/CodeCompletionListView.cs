// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	/// <summary>
	/// Description of CodeCompletionListView.	
	/// </summary>
	public class CodeCompletionListView : System.Windows.Forms.UserControl
	{
		ICompletionData[] completionData;
		int               firstItem    = 0;
		int               selectedItem = 0;
		ImageList         imageList;
		
		public ImageList ImageList {
			get {
				return imageList;
			}
			set {
				imageList = value;
			}
		}
		
		public ICompletionData SelectedCompletionData {
			get {
				if (selectedItem < 0) {
					return null;
				}
				return completionData[selectedItem];
			}
		}
		
		public int ItemHeight {
			get {
				return imageList.ImageSize.Height;
			}
		}
		
		int MaxVisibleItem {
			get {
				return Height / ItemHeight;
			}
		}
		
		public CodeCompletionListView(ICompletionData[] completionData)
		{
			Array.Sort(completionData);
			this.completionData = completionData;
//			SetStyle(ControlStyles.Selectable, false);
//			SetStyle(ControlStyles.UserPaint, true);
//			SetStyle(ControlStyles.DoubleBuffer, false);
		}
		
		public void SelectIndex(int index)
		{
			int oldSelectedItem = selectedItem;
			int oldFirstItem    = firstItem;
			selectedItem = Math.Max(0, Math.Min(completionData.Length - 1, index));
			if (selectedItem < firstItem) {
				firstItem = selectedItem;
			}
			if (firstItem + MaxVisibleItem <= selectedItem) {
				firstItem = selectedItem - MaxVisibleItem + 1;
			}
			if (oldSelectedItem != selectedItem) {
				if (firstItem != oldFirstItem) {
					Invalidate();
				} else {
					int min = Math.Min(selectedItem, oldSelectedItem) - firstItem;
					int max = Math.Max(selectedItem, oldSelectedItem) - firstItem;
					Invalidate(new Rectangle(0, 1 + min * ItemHeight, Width, (max - min + 1) * ItemHeight));
				}
				Update();
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
		
		public void PageDown()
		{
			SelectIndex(selectedItem + MaxVisibleItem);
		}
		
		public void PageUp()
		{
			SelectIndex(selectedItem - MaxVisibleItem);
		}
		
		public void SelectNextItem()
		{
			SelectIndex(selectedItem + 1);
		}
		
		public void SelectPrevItem()
		{
			SelectIndex(selectedItem - 1);
		}
		
		public void SelectItemWithStart(string startText)
		{
			startText = startText.ToLower();
			for (int i = 0; i < completionData.Length; ++i) {
				if (completionData[i].Text[0].ToLower().StartsWith(startText)) {
					SelectIndex(i);
					return;
				}
			}
			selectedItem = -1;
			Refresh();
			OnSelectedItemChanged(EventArgs.Empty);
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			float yPos       = 1;
			float itemHeight = imageList.ImageSize.Height;
			
			int curItem = firstItem;
			Graphics g  = pe.Graphics;
			while (curItem < completionData.Length && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.IntersectsWith(pe.ClipRectangle)) {
					// draw Background
					if (curItem == selectedItem) {
						g.FillRectangle(SystemBrushes.Highlight, drawingBackground);
					} else {
						g.FillRectangle(SystemBrushes.Window, drawingBackground);
					}
					
					// draw Icon
					int   xPos   = 0;
					if (imageList != null && completionData[curItem].ImageIndex < imageList.Images.Count) {
						g.DrawImage(imageList.Images[completionData[curItem].ImageIndex], new RectangleF(1, yPos, imageList.ImageSize.Width, itemHeight));
						xPos = imageList.ImageSize.Width;
					}
					
					// draw text
					if (curItem == selectedItem) {
						g.DrawString(completionData[curItem].Text[0], Font, SystemBrushes.HighlightText, xPos, yPos);
					} else {
						g.DrawString(completionData[curItem].Text[0], Font, SystemBrushes.WindowText, xPos, yPos);
					}
				}
				
				yPos += itemHeight;
				++curItem;
			}
			g.DrawRectangle(SystemPens.Control, new Rectangle(0, 0, Width - 1, Height - 1));
		}
		
		protected override void OnMouseEnter(System.EventArgs e)
		{
			Console.WriteLine("ON MOUSE ENTER!");
		}
		
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			Console.WriteLine("ON MOUSE MOVE!");
		}
		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			Console.WriteLine("ON MOUSE DOWN!");
			base.OnMouseDown(e);
			
			float yPos       = 1;
			int curItem = firstItem;
			float itemHeight = imageList.ImageSize.Height;
			Console.WriteLine(e.X  + " -- " + e.Y);
			while (curItem < completionData.Length && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.Contains(e.X, e.Y)) {
					SelectIndex(curItem);
					break;
				}
				yPos += itemHeight;
				++curItem;
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
		}
		
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (SelectedItemChanged != null) {
				SelectedItemChanged(this, e);
			}
		}
		
		public event EventHandler SelectedItemChanged;
	}
}
