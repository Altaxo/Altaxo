using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Gui.Pads.FileBrowser
{
	public interface IFileBrowserView : IFileTreeView, IFileListView
	{
	}

	public class FileBrowserController
	{
		FileSystemTreeController _treeController;
		FileListController _listController;

		IFileBrowserView _view;

		public FileBrowserController(IFileTreeView treeView, IFileListView listView)
		{
			_treeController = new FileSystemTreeController();
			_listController = new FileListController();

			_listController.ViewObject = listView;
			_treeController.ViewObject = treeView;

			_treeController.SelectedPathChanged += EhTreeController_SelectedPathChanged;
		}

		void EhTreeController_SelectedPathChanged(string path)
		{
			_listController.ShowFilesInPath(path);
		}

	}
}
