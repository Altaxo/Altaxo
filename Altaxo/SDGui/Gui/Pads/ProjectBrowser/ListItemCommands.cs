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
	public abstract class ProjectBrowseControllerCommand : ICSharpCode.Core.AbstractCommand
	{
		protected abstract void Run(ProjectBrowseController ctrl);
		public override void Run()
		{
			Run((ProjectBrowseController)Owner);
		}
		protected ProjectBrowseController Ctrl
		{
			get
			{
				return (ProjectBrowseController)Owner;
			}
		}
	}

	public class CmdListItemShow : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ShowSelectedListItem();
		}
	}

	public class CmdListItemHide : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.HideSelectedListItems();
		}
	}


	public class CmdListItemDelete : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.DeleteSelectedListItems();
		}
	}

	public class CmdListItemMoveTo : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.MoveSelectedListItems();
		}
	}

	public class CmdListItemCopyTo : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CopySelectedListItemsToFolder();
		}
	}

	public class CmdListItemClipboardCopy : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CopySelectedListItemsToClipboard();
		}
	}

	public class CmdListItemRename : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.RenameSelectedListItem();
		}
	}

	public class CmdViewOnSelectListNodeOff : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;


		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ViewOnSelectListNodeOn = false;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked
		{
			get
			{
				return !Ctrl.ViewOnSelectListNodeOn;
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
				if (null != IsCheckedChanged)
					IsCheckedChanged(this, EventArgs.Empty);
				return true;
			}
			set
			{
			}
		}

		#endregion
	}

	public class CmdViewOnSelectListNodeOn : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ViewOnSelectListNodeOn = true;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked
		{
			get
			{
				return Ctrl.ViewOnSelectListNodeOn;
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
				if (null != IsCheckedChanged)
					IsCheckedChanged(this, EventArgs.Empty);
				return true;
			}
			set
			{
			}
		}

		#endregion
	}


	public class CmdNewEmptyWorksheet : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CreateNewEmptyWorksheet();
		}
	}

	public class CmdNewStandardWorksheet : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CreateNewStandardWorksheet();
		}
	}

	public class CmdNewGraph : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CreateNewGraph();
		}
	}


	public class CmdPlotCommonColumns : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.PlotCommonColumns();
		}
	}

	public class CmdMultiRenameItems : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems();
			Altaxo.Main.Commands.MultiRenameDocuments.ShowRenameDocumentsDialog(list);
		}
	}

	public class CmdMultiExportGraphs : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.Gdi.GraphDocument>();
			int count = list.Count();

			if (count==0)
				return;
			if (count == 1)
				Altaxo.Graph.Gdi.GraphDocumentExportActions.ShowFileExportSpecificDialog(list.First());
			else
				Altaxo.Graph.Gdi.GraphDocumentExportActions.ShowExportMultipleGraphsDialogAndExportOptions(list);
		}
	}

	public class CmdExchangeTablesForPlotItems : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.Gdi.GraphDocument>();
			int count = list.Count();

			if (count == 0)
			{
				return;
			}
			else
			{
				Altaxo.Graph.Gdi.GraphDocumentOtherActions.ShowExchangeTablesOfPlotItemsDialog(list);
			}
		
		}
	}
		
}
