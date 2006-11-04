﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public static class MenuService
	{
		public static void AddItemsToMenu(ToolStripItemCollection collection, object owner, string addInTreePath)
		{
			ArrayList buildItems = AddInTree.GetTreeNode(addInTreePath).BuildChildItems(owner);
			foreach (object item in buildItems) {
				if (item is ToolStripItem) {
					collection.Add((ToolStripItem)item);
					if (item is IStatusUpdate)
						((IStatusUpdate)item).UpdateStatus();
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					collection.AddRange(submenuBuilder.BuildSubmenu(null, owner));
				}
			}
		}
		
		public static ContextMenuStrip CreateContextMenu(object owner, string addInTreePath)
		{
			if (addInTreePath == null) {
				return null;
			}
			try {
				ArrayList buildItems = AddInTree.GetTreeNode(addInTreePath).BuildChildItems(owner);
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				contextMenu.Items.Add(new ToolStripMenuItem("dummy"));
				contextMenu.Opening += delegate {
					contextMenu.Items.Clear();
					foreach (object item in buildItems) {
						if (item is ToolStripItem) {
							contextMenu.Items.Add((ToolStripItem)item);
						} else {
							ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
							contextMenu.Items.AddRange(submenuBuilder.BuildSubmenu(null, owner));
						}
					}
				};
				contextMenu.Opened += ContextMenuOpened;
				contextMenu.Closed += ContextMenuClosed;
				return contextMenu;
			} catch (TreePathNotFoundException) {
				MessageService.ShowError("Warning tree path '" + addInTreePath +"' not found.");
				return null;
			}
		}
		
		static bool isContextMenuOpen;
		
		public static bool IsContextMenuOpen {
			get {
				return isContextMenuOpen;
			}
		}
		
		static void ContextMenuOpened(object sender, EventArgs e)
		{
			isContextMenuOpen = true;
			ContextMenuStrip contextMenu = (ContextMenuStrip)sender;
			foreach (object o in contextMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		static void ContextMenuClosed(object sender, EventArgs e)
		{
			isContextMenuOpen = false;
		}
		
		public static void ShowContextMenu(object owner, string addInTreePath, Control parent, int x, int y)
		{
			ContextMenuStrip menu = CreateContextMenu(owner, addInTreePath);
			if (menu != null) {
				menu.Show(parent, new Point(x, y));
			}
		}
		
		class QuickInsertMenuHandler
		{
			TextBoxBase targetControl;
			string      text;
			
			public QuickInsertMenuHandler(TextBoxBase targetControl, string text)
			{
				this.targetControl = targetControl;
				this.text          = text;
			}
			
			public EventHandler EventHandler {
				get {
					return new EventHandler(PopupMenuHandler);
				}
			}
			void PopupMenuHandler(object sender, EventArgs e)
			{
				targetControl.SelectedText += text;
			}
		}
		
		class QuickInsertHandler
		{
			Control               popupControl;
			ContextMenuStrip      quickInsertMenu;
			
			public QuickInsertHandler(Control popupControl, ContextMenuStrip quickInsertMenu)
			{
				this.popupControl    = popupControl;
				this.quickInsertMenu = quickInsertMenu;
				
				popupControl.Click += new EventHandler(showQuickInsertMenu);
			}
			
			void showQuickInsertMenu(object sender, EventArgs e)
			{
				Point cords = new Point(popupControl.Width, 0);
				quickInsertMenu.Show(popupControl, cords);
			}
		}
		
		public static void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems)
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			for (int i = 0; i < quickInsertMenuItems.GetLength(0); ++i) {
				if (quickInsertMenuItems[i, 0] == "-") {
					contextMenu.Items.Add(new MenuSeparator());
				} else {
					MenuCommand cmd = new MenuCommand(quickInsertMenuItems[i, 0],
					                                  new QuickInsertMenuHandler(targetControl, quickInsertMenuItems[i, 1]).EventHandler);
					contextMenu.Items.Add(cmd);
				}
			}
			new QuickInsertHandler(popupControl, contextMenu);
		}
	}
}
