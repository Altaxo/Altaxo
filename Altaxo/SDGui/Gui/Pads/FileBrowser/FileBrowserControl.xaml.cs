using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Pads.FileBrowser
{
	/// <summary>
	/// Interaction logic for FileScoutControl.xaml
	/// </summary>
	public partial class FileBrowserControl : UserControl, ICSharpCode.SharpDevelop.Gui.IPadContent, IFileTreeView, IFileListView
	{
		FileBrowserController _controller;

		/// <summary>
		/// Occurs when the user activates the selected items (either by double-clicking on it, or by pressing Enter).
		/// </summary>
		public event Action SelectedItemsActivated;

		public FileBrowserControl()
		{
			InitializeComponent();

			_controller = new FileBrowserController(this, this);
		}


		#region ICSharpCode.SharpDevelop.Gui.IPadContent

		public new object Control
		{
			get { return this; }
		}

		public object InitiallyFocusedControl
		{
			get { return _treeView; }
		}

		public void Dispose()
		{
			
		}

		#endregion

		#region IFileTreeView

		public void Initialize_FolderTree(Collections.NGTreeNodeCollection nodes)
		{
			_treeView.ItemsSource = nodes;
		}

		public event Action<Collections.NGTreeNode> FolderTreeNodeSelected;

		#endregion


		#region IFileListView

		

		public void Initialize_FileListColumnNames(ICollection<string> names)
		{
			int i = 0;
			foreach(var name in names)
			{
				/*
				var gvc = new GridViewColumn() { Header = name };
				gvc.DisplayMemberBinding = new Binding(i == 0 ? "Text " : "Text" + i.ToString());

				grid.Columns.Add(gvc);
				*/
				++i;
			}
		}

		public void Initialize_FileList(Collections.SelectableListNodeList files)
		{
			_listView.ItemsSource = files;
		}

		#endregion

		private void EhTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (null != FolderTreeNodeSelected)
				FolderTreeNodeSelected((Collections.NGTreeNode)_treeView.SelectedItem);
		}

		private void EhListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (null != SelectedItemsActivated)
				SelectedItemsActivated();
			e.Handled = true;
		}

		private void EhListViewItem_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				if (null != SelectedItemsActivated)
				{
					SelectedItemsActivated();
				}
			}
		}
	}
}
