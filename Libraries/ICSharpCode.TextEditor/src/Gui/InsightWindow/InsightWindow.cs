// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.InsightWindow
{
	public class InsightWindow : Form
	{
		TextEditorControl control;
		Stack           insightDataProviderStack = new Stack();
		
		EventHandler         focusEventHandler;
		KeyPressEventHandler keyPressEventHandler;
		
		class InsightDataProviderStackElement 
		{
			public int                  currentData;
			public IInsightDataProvider dataProvider;
			
			public InsightDataProviderStackElement(IInsightDataProvider dataProvider)
			{
				this.currentData  = 0;
				this.dataProvider = dataProvider;
			}
		}
		
		public void AddInsightDataProvider(IInsightDataProvider provider)
		{
			provider.SetupDataProvider(fileName, control.ActiveTextAreaControl.TextArea);
			if (provider.InsightDataCount > 0) {
				insightDataProviderStack.Push(new InsightDataProviderStackElement(provider));
			}
		}
		
		int CurrentData {
			get {
				return ((InsightDataProviderStackElement)insightDataProviderStack.Peek()).currentData;
			}
			set {
				((InsightDataProviderStackElement)insightDataProviderStack.Peek()).currentData = value;
			}
		}
		
		IInsightDataProvider DataProvider {
			get {
				if (insightDataProviderStack.Count == 0) {
					return null;
				}
				return ((InsightDataProviderStackElement)insightDataProviderStack.Peek()).dataProvider;
			}
		}
		
		void CloseCurrentDataProvider()
		{
			insightDataProviderStack.Pop();
			if (insightDataProviderStack.Count == 0) {
				Close();
			} else {
				Refresh();
			}
		}
		
		public void ShowInsightWindow()
		{
			if (!Visible) {
				if (insightDataProviderStack.Count > 0) {
//					control.TextAreaPainter.IHaveTheFocusLock = true;
					Show();
//					dialogKeyProcessor = new TextEditorControl.DialogKeyProcessor(ProcessTextAreaKey);
//					control.ProcessDialogKeyProcessor += dialogKeyProcessor;
					control.Focus();
//					control.TextAreaPainter.IHaveTheFocus     = true;
//					control.TextAreaPainter.IHaveTheFocusLock = false;
				}
			} else {
				Refresh();
			}
		}
		string fileName;
		
		public InsightWindow(TextEditorControl control, string fileName)
		{
			this.control             = control;
			this.fileName = fileName;
			Point caretPos  = control.ActiveTextAreaControl.TextArea.Caret.Position;
			Point visualPos = new Point(control.ActiveTextAreaControl.TextArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X) + control.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.X,
			          (int)((1 + caretPos.Y) * control.ActiveTextAreaControl.TextArea.TextView.FontHeight) - control.ActiveTextAreaControl.TextArea.VirtualTop.Y - 1 + control.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Y);
			
			focusEventHandler = new EventHandler(TextEditorLostFocus);
			
			control.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(CaretOffsetChanged);
//			control.TextAreaPainter.IHaveTheFocusChanged += focusEventHandler;
			
//			control.TextAreaPainter.KeyPress += keyPressEventHandler;
			
	 		Location = control.ActiveTextAreaControl.PointToScreen(visualPos);
			
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			Size            = new Size(0, 0);
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}
		
		// Methods that are inserted into the TextArea :
		bool ProcessTextAreaKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				case Keys.Down:
					if (DataProvider != null && DataProvider.InsightDataCount > 0) {
						CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
						Refresh();
					}
					return true;
				case Keys.Up:
					if (DataProvider != null && DataProvider.InsightDataCount > 0) {
						CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
						Refresh();
					}
					return true;
			}
			return false;
		}
		
		void KeyPressEvent(object sender, KeyPressEventArgs e)
		{
			if (DataProvider != null && DataProvider.CharTyped()) {
				CloseCurrentDataProvider();
			}
		}
		
		void CaretOffsetChanged(object sender, EventArgs e)
		{
			// move the window under the caret (don't change the x position)
			Point caretPos  = control.ActiveTextAreaControl.Caret.Position;
			int y = (int)((1 + caretPos.Y) * control.ActiveTextAreaControl.TextArea.TextView.FontHeight) - control.ActiveTextAreaControl.TextArea.VirtualTop.Y - 1 + control.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Y;
			Point p = control.ActiveTextAreaControl.PointToScreen(new Point(0, y));
			p.X = Location.X;
			if (p.Y != Location.Y) {
				Location = p;
			}
			
			while (DataProvider != null && DataProvider.CaretOffsetChanged()) {
				 CloseCurrentDataProvider();
			}
		}
		///// END
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			
			// take out the inserted methods
			control.ActiveTextAreaControl.Caret.PositionChanged -= new EventHandler(CaretOffsetChanged);
//			control.ProcessDialogKeyProcessor            -= dialogKeyProcessor;
//			control.TextAreaPainter.IHaveTheFocusChanged -= focusEventHandler;
//			control.TextAreaPainter.KeyPress             -= keyPressEventHandler;
		}
		
		protected void TextEditorLostFocus(object sender, EventArgs e)
		{
			if (!control.ActiveTextAreaControl.TextArea.Focused) {
				Close();
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			string methodCountMessage = null, description;
			if (DataProvider == null || DataProvider.InsightDataCount < 1) {
				description = "Unknown Method";
			} else {
//				if (DataProvider.InsightDataCount > 1) {
//					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
//					stringParserService.Properties["CurrentMethodNumber"]  = (CurrentData + 1).ToString();
//					stringParserService.Properties["NumberOfTotalMethods"] = DataProvider.InsightDataCount.ToString();
//					methodCountMessage = stringParserService.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.InsightWindow.NumberOfText}");
//				}
				
				description = DataProvider.GetInsightData(CurrentData);
			}
			
			TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics,
				Font, methodCountMessage, description);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
		}
	}
}
