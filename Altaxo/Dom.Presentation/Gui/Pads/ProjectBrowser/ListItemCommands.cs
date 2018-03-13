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
using Altaxo.Collections;
using Altaxo.Gui.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public abstract class ProjectBrowseControllerCommand : SimpleCommand
	{
		private ProjectBrowseController _controller;

		protected abstract void Run(ProjectBrowseController ctrl);

		public override void Execute(object parameter)
		{
			_controller = parameter as ProjectBrowseController;

			Run((ProjectBrowseController)parameter);
		}

		public override bool CanExecute(object parameter)
		{
			_controller = parameter as ProjectBrowseController;
			return base.CanExecute(parameter);
		}

		protected ProjectBrowseController Ctrl
		{
			get
			{
				return _controller;
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

	public class CmdListItemCopyToMultipleFolders : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CopySelectedListItemsToMultipleFolders();
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

	public class CmdViewOnSelectListNodeOff : ProjectBrowseControllerCommand, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ViewOnSelectListNodeOn = false;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked(object parameter)
		{
			return ((ProjectBrowseController)parameter).ViewOnSelectListNodeOn;
		}

		#endregion ICheckableMenuCommand Members

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

		#endregion IMenuCommand Members
	}

	public class CmdViewOnSelectListNodeOn : ProjectBrowseControllerCommand, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged;

		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ViewOnSelectListNodeOn = true;
			if (null != IsCheckedChanged)
				IsCheckedChanged(this, EventArgs.Empty);
		}

		#region ICheckableMenuCommand Members

		public bool IsChecked(object parameter)
		{
			return ((ProjectBrowseController)parameter).ViewOnSelectListNodeOn;
		}

		#endregion ICheckableMenuCommand Members

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

		#endregion IMenuCommand Members
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

	public class CmdNewPropertyBag : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CreateNewPropertyBag();
		}
	}

	public class CmdNewNotesDocument : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.CreateNewTextDocument();
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
			var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.GraphDocumentBase>();
			int count = list.Count();

			if (count == 0)
				return;
			if (count == 1)
				Altaxo.Graph.GraphDocumentBaseExportActions.ShowFileExportSpecificDialog(list.First());
			else
				Altaxo.Graph.GraphDocumentBaseExportActions.ShowExportMultipleGraphsDialogAndExportOptions(list);
		}
	}

	/// 	/// <summary>
	/// This command will rescale all axes in all layers of all selected graph documents.
	/// </summary>
	public class CmdMultiRescaleGraphs : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.GraphDocumentBase>();
			int count = list.Count();

			if (count == 0)
			{
				Current.Gui.ErrorMessageBox("There were no graph documents selected for rescaling!", "No graph documents selected");
				return;
			}
			else
			{
				foreach (var graph in list)
				{
					if (graph is Altaxo.Graph.Gdi.GraphDocument gdiDoc)
						RescaleAllLayers(gdiDoc);
					else if (graph is Altaxo.Graph.Graph3D.GraphDocument graph3DDoc)
						RescaleAllLayers(graph3DDoc);
					else
						throw new NotImplementedException("This type of graph document is not known here.");
				}

				Current.Gui.InfoMessageBox(string.Format("Axes of {0} graph document(s) rescaled.", list.Count()), "Success");
			}
		}

		private void RescaleAllLayers(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			var layers = TreeNodeExtensions.TakeFromFirstLeavesToHere(doc.RootLayer).OfType<Altaxo.Graph.Gdi.XYPlotLayer>();
			foreach (var layer in layers)
				layer.OnUserRescaledAxes();
		}

		private void RescaleAllLayers(Altaxo.Graph.Graph3D.GraphDocument doc)
		{
			var layers = TreeNodeExtensions.TakeFromFirstLeavesToHere(doc.RootLayer).OfType<Altaxo.Graph.Graph3D.XYZPlotLayer>();
			foreach (var layer in layers)
				layer.OnUserRescaledAxes();
		}
	}

	/// 	/// <summary>
	/// This command will rescale all axes in all layers of all selected graph documents.
	/// </summary>
	public class CmdMultiResizeGraphs : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.Gdi.GraphDocument>();
			int count = list.Count();

			if (count == 0)
			{
				Current.Gui.ErrorMessageBox("There were no graph documents selected for resizing!", "No graph documents selected");
				return;
			}
			else
			{
				if (Altaxo.Gui.Graph.Graph2D.ResizeGraphController.ShowResizeGraphDialog(list))
					Current.Gui.InfoMessageBox(string.Format("{0} graph(s) resized.", list.Count()), "Success");
			}
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
