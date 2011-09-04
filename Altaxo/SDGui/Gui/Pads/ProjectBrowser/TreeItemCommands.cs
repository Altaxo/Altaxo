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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public class CmdViewOnSelectOff : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;
		ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.Off;
			Ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked
		{
			get
			{
				return Ctrl.ViewOnSelectTreeNode == ViewOnSelect.Off;
			}
			set
			{
			}
		}

		#endregion

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

		#endregion
	}

	public class CmdViewOnSelectFolderItems : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;
		ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.ItemsInFolder;
			Ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked
		{
			get
			{
				return Ctrl.ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolder;
			}
			set
			{
			}
		}

		#endregion

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

		#endregion
	}

	public class CmdViewOnSelectFolderAndSubfolderItems : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;
		ViewOnSelect _lastKnownValue;

		protected override void Run(ProjectBrowseController ctrl)
		{
			_lastKnownValue = ViewOnSelect.ItemsInFolderAndSubfolders;
			Ctrl.ViewOnSelectTreeNode = _lastKnownValue;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked
		{
			get
			{
				return Ctrl.ViewOnSelectTreeNode == ViewOnSelect.ItemsInFolderAndSubfolders;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

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

		#endregion
	}

	public class CmdTreeNodeFolderDelete : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ProjectBrowserExtensions.DeleteDocuments(ctrl.ExpandItemListToSubfolderItems(ctrl.GetAllListItems()));
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
			ProjectBrowserExtensions.ShowDocumentsExclusively(ctrl.ExpandItemListToSubfolderItems(ctrl.GetAllListItems()));
		}
	}

	public class CmdTreeNodeHideWindows : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetAllListItems();
			foreach (var item in list)
				Current.ProjectService.CloseDocumentViews(item);
		}
	}

	public class CmdTreeNodeHideWindowsRecursively : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetAllListItems();
			list = ctrl.ExpandItemListToSubfolderItems(list);

			foreach (var item in list)
				Current.ProjectService.CloseDocumentViews(item);
		}
	}

}
