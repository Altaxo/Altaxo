#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public class CmdViewOnSelectOff : ProjectBrowseControllerCommand, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		private ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.Off;
			Ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked(object parameter)
		{
			return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.Off;
		}

		#endregion ICheckableMenuCommand Members

		#region IMenuCommand Members

		public bool IsEnabled
		{
			get
			{
				if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && null != IsCheckedChanged)
				{
					IsCheckedChanged(this, EventArgs.Empty);
					_lastKnownValue = Ctrl.ViewOnSelectTreeNode;
				}

				return true;
			}
			set
			{
			}
		}

		#endregion IMenuCommand Members
	}

	public class CmdViewOnSelectFolderItems : ProjectBrowseControllerCommand, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		private ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.ItemsInFolder;
			ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			IsCheckedChanged?.Invoke(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked(object parameter)
		{
			return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolder;
		}

		#endregion ICheckableMenuCommand Members

		#region IMenuCommand Members

		public bool IsEnabled
		{
			get
			{
				if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && null != IsCheckedChanged)
				{
					IsCheckedChanged(this, EventArgs.Empty);
					_lastKnownValue = Ctrl.ViewOnSelectTreeNode;
				}

				return true;
			}
			set
			{
			}
		}

		#endregion IMenuCommand Members
	}

	public class CmdViewOnSelectFolderAndSubfolderItems : ProjectBrowseControllerCommand, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		private ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.ItemsInFolderAndSubfolders;
			ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			IsCheckedChanged?.Invoke(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked(object parameter)
		{
			return ((ProjectBrowseController)parameter).ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolderAndSubfolders;
		}

		#endregion ICheckableMenuCommand Members

		#region IMenuCommand Members

		public bool IsEnabled
		{
			get
			{
				if (Ctrl.ViewOnSelectTreeNode != _lastKnownValue && null != IsCheckedChanged)
				{
					IsCheckedChanged(this, EventArgs.Empty);
					_lastKnownValue = Ctrl.ViewOnSelectTreeNode;
				}

				return true;
			}
			set
			{
			}
		}

		#endregion IMenuCommand Members
	}

	public class CmdTreeNodeFolderDelete : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var items = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems());
			Altaxo.Main.ProjectFolders.DeleteDocuments(items);
		}
	}

	public class CmdTreeNodeFolderRename : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.RenameTreeNode();
		}
	}

	public class CmdTreeNodeShowWindows : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ProjectBrowserExtensions.ShowDocumentsExclusively(ctrl.GetAllListItems());
		}
	}

	public class CmdTreeNodeShowWindowsRecursively : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ProjectBrowserExtensions.ShowDocumentsExclusively(Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems()));
		}
	}

	public class CmdTreeNodeHideWindows : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetAllListItems();
			foreach (var item in list)
				Current.IProjectService.CloseDocumentViews(item);
		}
	}

	public class CmdTreeNodeHideWindowsRecursively : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = Current.Project.Folders.GetExpandedProjectItemSet(ctrl.GetAllListItems());
			foreach (var item in list)
				Current.IProjectService.CloseDocumentViews(item);
		}
	}
}