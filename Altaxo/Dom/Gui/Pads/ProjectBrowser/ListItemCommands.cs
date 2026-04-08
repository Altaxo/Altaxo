#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Linq;
using Altaxo.AddInItems;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Shows the selected project browser list item.
  /// </summary>
  public class CmdListItemShow : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ShowSelectedListItem();
    }
  }

  /// <summary>
  /// Hides the selected project browser list items.
  /// </summary>
  public class CmdListItemHide : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.HideSelectedListItems();
    }
  }

  /// <summary>
  /// Deletes the selected project browser list items.
  /// </summary>
  public class CmdListItemDelete : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.DeleteSelectedListItems();
    }
  }

  /// <summary>
  /// Moves the selected project browser list items.
  /// </summary>
  public class CmdListItemMoveTo : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.MoveSelectedListItems();
    }
  }

  /// <summary>
  /// Copies the selected project browser list items to another folder.
  /// </summary>
  public class CmdListItemCopyTo : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CopySelectedListItemsToFolder();
    }
  }

  /// <summary>
  /// Copies the selected project browser list items to multiple folders.
  /// </summary>
  public class CmdListItemCopyToMultipleFolders : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CopySelectedListItemsToMultipleFolders();
    }
  }

  /// <summary>
  /// Copies the selected project browser list items to the clipboard.
  /// </summary>
  public class CmdListItemClipboardCopy : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CopySelectedListItemsToClipboard();
    }
  }

  /// <summary>
  /// Renames the selected project browser list item.
  /// </summary>
  public class CmdListItemRename : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.RenameSelectedListItem();
    }
  }

  /// <summary>
  /// Disables the action executed when selecting a list item.
  /// </summary>
  public class CmdViewOnSelectListNodeOff : ProjectBrowseControllerCommand, ICheckableMenuCommand
  {
    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler IsCheckedChanged;

    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ViewOnSelectListNodeOn = false;
      if (IsCheckedChanged is not null)
        IsCheckedChanged(this, EventArgs.Empty);
    }

    #region ICheckableMenuCommand Members

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      return ((ProjectBrowseController)parameter).ViewOnSelectListNodeOn;
    }

    #endregion ICheckableMenuCommand Members

    #region IMenuCommand Members

    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get
      {
        if (IsCheckedChanged is not null)
          IsCheckedChanged(this, EventArgs.Empty);
        return true;
      }
      set
      {
      }
    }

    #endregion IMenuCommand Members
  }

  /// <summary>
  /// Enables the action executed when selecting a list item.
  /// </summary>
  public class CmdViewOnSelectListNodeOn : ProjectBrowseControllerCommand, ICheckableMenuCommand
  {
    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler IsCheckedChanged;

    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ViewOnSelectListNodeOn = true;
      if (IsCheckedChanged is not null)
        IsCheckedChanged(this, EventArgs.Empty);
    }

    #region ICheckableMenuCommand Members

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      return ((ProjectBrowseController)parameter).ViewOnSelectListNodeOn;
    }

    #endregion ICheckableMenuCommand Members

    #region IMenuCommand Members

    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get
      {
        if (IsCheckedChanged is not null)
          IsCheckedChanged(this, EventArgs.Empty);
        return true;
      }
      set
      {
      }
    }

    #endregion IMenuCommand Members
  }

  /// <summary>
  /// Creates a new empty worksheet.
  /// </summary>
  public class CmdNewEmptyWorksheet : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewEmptyWorksheet();
    }
  }

  /// <summary>
  /// Creates a new standard worksheet.
  /// </summary>
  public class CmdNewStandardWorksheet : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewStandardWorksheet();
    }
  }

  /// <summary>
  /// Creates a new graph.
  /// </summary>
  public class CmdNewGraph : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewGraph();
    }
  }

  /// <summary>
  /// Creates a new property bag.
  /// </summary>
  public class CmdNewPropertyBag : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewPropertyBag();
    }
  }

  /// <summary>
  /// Creates a new text document.
  /// </summary>
  public class CmdNewTextDocument : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewTextDocument();
    }
  }

  /// <summary>
  /// Creates a new folder text document.
  /// </summary>
  public class CmdNewFolderTextDocument : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewFolderTextDocument();
    }
  }

  /// <summary>
  /// Plots common columns of the selected tables.
  /// </summary>
  public class CmdPlotCommonColumns : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.PlotCommonColumns();
    }
  }

  /// <summary>
  /// Extracts common columns of the selected tables.
  /// </summary>
  public class CmdExtractCommonColumns : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ExtractCommonColumns();
    }
  }

  /// <summary>
  /// Executes all data sources of the selected tables.
  /// </summary>
  public class CmdExecuteAllDataSources : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      Current.Gui.ExecuteAsUserCancellable(1000, ctrl.ExecuteAllDataSources);
    }
  }

  /// <summary>
  /// Opens the dialog for clearing selected data tables.
  /// </summary>
  public class CmdClearDataTablesShowDialog : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      object optionsO = new DataTableCleaningOptions();
      if (true == Current.Gui.ShowDialog(ref optionsO, "Cleaning options"))
      {
        var options = (DataTableCleaningOptions)optionsO;

        // find all selected tables with data sources in it
        var dataTables = ctrl.GetSelectedListItems().OfType<DataTable>();
        options.ApplyTo(dataTables);
      }

    }
  }

  /// <summary>
  /// Opens the multi-rename dialog for the selected items.
  /// </summary>
  public class CmdMultiRenameItems : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = ctrl.GetSelectedListItems();
      Altaxo.Main.Commands.MultiRenameDocuments.ShowRenameDocumentsDialog(list);
    }
  }

  /// <summary>
  /// Exports the selected graphs.
  /// </summary>
  public class CmdMultiExportGraphs : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Exports the selected graphs as mini projects.
  /// </summary>
  public class CmdMultiExportGraphsAsMiniProject : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = ctrl.GetSelectedListItems().OfType<Altaxo.Graph.GraphDocumentBase>();
      int count = list.Count();

      if (count == 0)
        return;
      else if (count == 1)
        Altaxo.Graph.Commands.SaveAsMiniProjectBase.Run(list.First());
      else
        Altaxo.Graph.GraphDocumentBaseExportActions.ShowExportMultipleGraphsAsMiniProjectDialog(list);
    }
  }

  /// <summary>
  /// Imports data into the selected tables.
  /// </summary>
  public class CmdMultiImportTables : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
    protected override void Run(ProjectBrowseController ctrl)
    {
      var list = ctrl.GetSelectedListItems().OfType<Altaxo.Data.DataTable>();
      int count = list.Count();

      if (count == 0)
        return;
      else
        Altaxo.Data.ImportIntoMultipleTablesAction.ShowImportIntoMultipleTablesDialog(list);
    }
  }

  /// <summary>
  /// This command will rescale all axes in all layers of all selected graph documents.
  /// </summary>
  public class CmdMultiRescaleGraphs : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// This command will rescale all axes in all layers of all selected graph documents.
  /// </summary>
  public class CmdMultiResizeGraphs : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
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

  /// <summary>
  /// Exchanges tables of plot items in the selected graphs.
  /// </summary>
  public class CmdExchangeTablesForPlotItems : ProjectBrowseControllerCommand
  {
    /// <inheritdoc/>
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
