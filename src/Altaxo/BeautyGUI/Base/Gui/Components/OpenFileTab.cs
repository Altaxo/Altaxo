// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using Crownwood.Magic.Menus;
using Crownwood.Magic.Controls;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;


namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class OpenFileTab : Crownwood.Magic.Controls.TabControl, IOwnerState
	{
		readonly static string contextMenuPath = "/SharpDevelop/Workbench/OpenFileTab/ContextMenu";
		
		[Flags]
		public enum OpenFileTabState {
			Nothing             = 0,
			FileDirty           = 1,
			ClickedWindowIsForm = 2,
			FileUntitled        = 4
		}
		
		OpenFileTabState internalState = OpenFileTabState.Nothing;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		int clickedTabIndex = -1;
		
		public IWorkbenchWindow ClickedWindow {
			get {
				if (clickedTabIndex == -1) {
					return null;
				}
				return ((MyTabPage)TabPages[clickedTabIndex]).Window;
			}
		}
		
		public int ClickedTabIndex {
			get {
				return clickedTabIndex;
			}
			set {
				clickedTabIndex = value;
			}
		}
		
		public OpenFileTab()
		{
			Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			ShowArrows = true;
			ShowClose  = true;
			
			ClosePressed     += new EventHandler(MyClosePressed);
			SelectionChanged += new EventHandler(MySelectionChanged);
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
		}
		
		public class MyTabPage : Crownwood.Magic.Controls.TabPage
		{
			IWorkbenchWindow window;
			OpenFileTab tab;
			
			public IWorkbenchWindow Window {
				get {
					return window;
				}
			}
			
			public MyTabPage(OpenFileTab tab, IWorkbenchWindow window)
			{
				this.tab    = tab;
				this.window = window;
				
				window.TitleChanged   += new EventHandler(TitleChanged);
				window.CloseEvent     += new EventHandler(CloseEvent);
				window.WindowSelected += new EventHandler(SelectEvent);
				TitleChanged(null, null);
			}
			
			public void SelectPage()
			{
				for (int i = 0; i < tab.TabPages.Count; ++i) {
					if (tab.TabPages[i] == this) {
						tab.SelectedIndex = i;
						break;
					}
				}
			}
			
			void CloseEvent(object sender, EventArgs e)
			{
				if (tab.TabPages.IndexOf(this) >= 0) {
					tab.TabPages.Remove(this);
				}
			}
			
			void SelectEvent(object sender, EventArgs e)
			{
				SelectPage();
			}
			
			void TitleChanged(object sender, EventArgs e)
			{
				Title = Path.GetFileName(window.Title);
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				MenuCommand[] contextMenu = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(contextMenuPath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
				
				if (contextMenu.Length > 0 && TabPages.Count > 0 && clickedTabIndex >= 0) {
					PopupMenu popup = new PopupMenu();
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					popup.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
					popup.MenuCommands.AddRange(contextMenu);
					popup.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
				}
			} else {
				base.OnMouseUp(e);
			}
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
//			if (e.Button == MouseButtons.Left) {
				base.OnMouseDown(e);
//			}
			clickedTabIndex = -1;
			
			for(int i=0; i<_tabPages.Count; i++) {
				Rectangle rect = (Rectangle)_tabRects[i];
				if (rect.Contains(e.X, e.Y)) {
					clickedTabIndex = i;
					break;
				}
			}
			
			internalState = OpenFileTabState.Nothing;
			if (clickedTabIndex != -1) {
				if (ClickedWindow.ViewContent.ContentName == null) {
					internalState |= OpenFileTabState.FileUntitled;
				}
				if (ClickedWindow.ViewContent.IsDirty) {
					internalState |= OpenFileTabState.FileDirty;
				}
				if (ClickedWindow is Form) {
					internalState |= OpenFileTabState.ClickedWindowIsForm;
				}
// KSL Start, Fix for loosing focus when clicking the tab control to select another tab
				((MyTabPage)TabPages[SelectedIndex]).Window.SelectWindow();
// KSL End
			}
		}
		
		void MyClosePressed(object sender, EventArgs e)
		{
			if (SelectedIndex >= 0 && SelectedIndex < TabPages.Count) {
				MyTabPage page = (MyTabPage)TabPages[SelectedIndex];
				page.Window.CloseWindow(false);
			}
		}
		void MySelectionChanged(object sender, EventArgs e)
		{
			if (SelectedIndex >= 0 && SelectedIndex < TabPages.Count && TabPages[SelectedIndex] != null) {
				((MyTabPage)TabPages[SelectedIndex]).Window.SelectWindow();
			}
		}
		
		public Crownwood.Magic.Controls.TabPage AddWindow(IWorkbenchWindow window)
		{
			Crownwood.Magic.Controls.TabPage tabPage = new MyTabPage(this, window);
			tabPage.Selected = true;
			tabPage.Tag = window;
			try
			{
				TabPages.Add(tabPage);
      }
      catch {}
			return tabPage;
		}
	}
}
