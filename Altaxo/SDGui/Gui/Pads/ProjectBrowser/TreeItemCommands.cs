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
