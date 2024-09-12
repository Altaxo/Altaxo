#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;
using Altaxo.Serialization.Clipboard;

namespace Altaxo.Gui.Data
{
  public interface IListOfXAndYColumnView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IListOfXAndYColumnView))]
  [UserControllerForObject(typeof(ListOfXAndYColumn))]
  public partial class ListOfXAndYColumnController : MVCANControllerEditCopyOfDocBase<ListOfXAndYColumn, IListOfXAndYColumnView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public ListOfXAndYColumnController()
    {
      CommandChangeTableForSelectedItems = new RelayCommand(EhChangeTableForSelectedItems, EhCanChangeTableForSelectedItems);
      CommandChangeColumnsForSelectedItems = new RelayCommand(EhChangeColumnsForSelectedItems, EhCanChangeColumnsForSelectedItems);

      CmdPutDataToPlotItemsUp = new RelayCommand(AvailableItems_PutDataToPlotItemsUp);
      CmdPutDataToPlotItemsDown = new RelayCommand(AvailableItems_PutDataToPlotItemsDown);
      CmdPLotItemsMoveUpSelected = new RelayCommand(PlotItems_MoveUpSelected);
      CmdPLotItemsMoveDownSelected = new RelayCommand(PlotItems_MoveDownSelected);
      CmdPlotItemOpen = new RelayCommand(PlotItem_Open);
      CmdMasterDataDoubleClick = new RelayCommand(PlotItem_Open);
      AvailableItemsDragHandler = new AvailableItems_DragHandler(this);
      PlotItemsDragDropHandler = new PlotItems_DragDropHandler(this);

      CmdPlotItemsCopy = new RelayCommand(PlotItems_Copy, PlotItems_CanCopy);
      CmdPlotItemsCut = new RelayCommand(PlotItems_Cut, PlotItems_CanCut);
      CmdPlotItemsPaste = new RelayCommand(PlotItems_Paste, PlotItems_CanPaste);
      CmdPlotItemsDelete = new RelayCommand(PlotItems_Delete);
    }



    private void PlotItem_Open()
    {
      var item = DataItems.Items.FirstSelectedNode;
      if (item is MyNode myitem && myitem.Curve is { } curve)
      {
        var ctrl = new XAndYColumnController();
        ctrl.InitializeDocument(curve);
        Current.Gui.FindAndAttachControlTo(ctrl);
        if (Current.Gui.ShowDialog(ctrl, curve.GetName(0x21)))
        {
          myitem.Tag = ctrl.ModelObject;
          myitem.Text = myitem.Curve?.GetName(0x21) ?? "---";
        }
      }
    }

    public bool PlotItems_CanCopy()
    {
      return DataItems.Items.Any(node => node.IsSelected);
    }
    public void PlotItems_Copy()
    {
      var selNodes = DataItems.Items.Where(node => node.IsSelected).Select(node => node.Tag).ToList();
      ClipboardSerialization.PutObjectToClipboard("Altaxo.Data.ListOfXAndYColumn.AsXml", selNodes);
    }

    public bool PlotItems_CanCut()
    {
      return DataItems.Items.Any(node => node.IsSelected);
    }
    public void PlotItems_Cut()
    {
      var selNodes = DataItems.Items.Where(node => node.IsSelected).ToArray();
      PlotItems_Copy();
      foreach (var node in DataItems.Items.Where(node => node.IsSelected))
      {
        (node.Tag as IDisposable)?.Dispose();
        DataItems.Items.Remove(node);
      }
    }

    public bool PlotItems_CanPaste()
    {
      object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
      var coll = o as Altaxo.Graph.Gdi.Plot.PlotItemCollection;
      if (coll is not null)
        return true;

      o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Data.ListOfXAndYColumn.AsXml");
      if (o is List<object> list && list.Any(x => x is XAndYColumn))
        return true;

      return false;
    }

    public void PlotItems_Paste()
    {
      var itemsToPast = new List<XAndYColumn>();
      object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
      if (o is Altaxo.Graph.Gdi.Plot.PlotItemCollection coll)
      {
        // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
        foreach (Altaxo.Graph.Gdi.Plot.IGPlotItem item in coll) // it is neccessary to add the items to the doc first, because otherwise they don't have names
        {
          if (item is XYColumnPlotData xyPlotData)
          {
            var xyitem = new XAndYColumn(xyPlotData.DataTable, xyPlotData.GroupNumber) { XColumn = xyPlotData.XColumn, YColumn = xyPlotData.YColumn };
            itemsToPast.Add(xyitem);
          }
        }
      }
      o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Data.ListOfXAndYColumn.AsXml");
      if (o is List<object> list && list.Any(x => x is XAndYColumn))
      {
        itemsToPast.AddRange(list.Where(l => l is XAndYColumn).Select(l => (XAndYColumn)l));
      }

      if (itemsToPast.Count > 0)
      {
        AddItemsToGuiList(itemsToPast, toLast: true);
      }
    }

    private void PlotItems_Delete()
    {
      var sublist = DataItems.Items;

      int itemsDeleted = 0;
      for (int i = sublist.Count - 1; i >= 0; i--)
      {
        if (sublist[i].IsSelected)
        {
          sublist.RemoveAt(i);
          itemsDeleted++;
        }
      }
    }

    #region Bindings
    public ICommand CommandChangeTableForSelectedItems { get; }
    public ICommand CommandChangeColumnsForSelectedItems { get; }

    public ICommand CmdPutDataToPlotItemsUp { get; }
    public ICommand CmdPutDataToPlotItemsDown { get; }
    public ICommand CmdPLotItemsMoveUpSelected { get; }
    public ICommand CmdPLotItemsMoveDownSelected { get; }
    public ICommand CmdPlotItemsCopy { get; }
    public ICommand CmdPlotItemsCut { get; }
    public ICommand CmdPlotItemsPaste { get; }
    public ICommand CmdPlotItemsDelete { get; }
    public ICommand CmdPlotItemOpen { get; }
    public ICommand CmdMasterDataDoubleClick { get; }

    private ItemsController<XAndYColumn?> _dataItems;

    public ItemsController<XAndYColumn?> DataItems
    {
      get => _dataItems;
      set
      {
        if (!(_dataItems == value))
        {
          _dataItems?.Dispose();
          _dataItems = value;
          OnPropertyChanged(nameof(DataItems));
        }
      }
    }



    private NGTreeNode _availableItemsRootNode;

    /// <summary>
    /// Initializes the treeview of available data with content.
    /// </summary>
    public NGTreeNode AvailableContent
    {
      get => _availableItemsRootNode;
      set
      {
        if (!(_availableItemsRootNode == value))
        {
          _availableItemsRootNode = value;
          OnPropertyChanged(nameof(AvailableContent));
        }
      }
    }

    private IEnumerable<NGTreeNode> AvailableItemsSelected
    {
      get
      {
        return TreeNodeExtensions.TakeFromHereToFirstLeaves(_availableItemsRootNode, false).Where(n => n.IsSelected);
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        _availableItemsRootNode = new NGTreeNode();
        // available items
        Altaxo.Gui.Graph.SingleColumnChoiceController.AddAllTableNodes(_availableItemsRootNode);

        InitializeGuiNodesFromDocument(_doc);
        DataItems = new ItemsController<XAndYColumn?>(_dataNodes);
      }
    }


    /// <summary>
    /// Puts the selected data columns into the plot content, either at the first position(s), or immediately before the first selected item.
    /// </summary>
    public void AvailableItems_PutDataToPlotItemsUp()
    {
      AvailableItems_PutDataToPlotItems(false);
    }

    /// <summary>
    /// Puts the selected data columns into the plot content, either at the first position(s), or immediately before the first selected item.
    /// </summary>
    public void AvailableItems_PutDataToPlotItemsDown()
    {
      AvailableItems_PutDataToPlotItems(true);
    }

    /// <summary>
    /// Puts the selected data columns into the plot content, either at the first position(s), or immediately before the first selected item.
    /// </summary>
    public void AvailableItems_PutDataToPlotItems(bool toLast)
    {
      var columnsAlreadyProcessed = new HashSet<Altaxo.Data.DataColumn>();

      var selNodes = AvailableItemsSelected;
      var validNodes = NGTreeNode.NodesWithoutSelectedChilds(selNodes);

      var xyColumns = new List<XAndYColumn>();

      // first, put the selected node into the list, even if it is not checked
      foreach (NGTreeNode sn in validNodes)
      {
        var dataCol = sn.Tag as Altaxo.Data.DataColumn;
        if (dataCol is not null && !columnsAlreadyProcessed.Contains(dataCol))
        {
          columnsAlreadyProcessed.Add(dataCol);
          var node = CreatePlotItem(dataCol);
          if (node is not null)
          {
            xyColumns.Add(node);
          }
        }
        else if (sn is Altaxo.Gui.Graph.SingleColumnChoiceController.TableNode)
        {
          var rangeNode = sn as Altaxo.Gui.Graph.SingleColumnChoiceController.TableNode;
          var coll = rangeNode.Collection;
          var firstCol = rangeNode.FirstColumn;
          var columnCount = rangeNode.ColumnCount;

          for (int i = firstCol; i < firstCol + columnCount; ++i)
          {
            dataCol = coll[i];
            if (coll.GetColumnKind(i) == ColumnKind.V && !columnsAlreadyProcessed.Contains(dataCol)) // add only value columns as plot items
            {
              columnsAlreadyProcessed.Add(dataCol);
              var node = CreatePlotItem(dataCol);
              if (node is not null)
              {
                xyColumns.Add(node);
              }
            }
          }
        }
      }

      AddItemsToGuiList(xyColumns, toLast);

      AvailableItems_ClearSelection();

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    private XAndYColumn? CreatePlotItem(DataColumn ycol)
    {
      if (ycol is null)
        return null;

      var table = DataTable.GetParentDataTableOf(ycol);
      if (table is null)
        return null;

      var groupNumber = table.DataColumns.GetColumnGroup(ycol);
      var xcol = table.DataColumns.FindXColumnOf(ycol);

      if (xcol is not null)
        return new XAndYColumn(table, groupNumber) { XColumn = xcol, YColumn = ycol };
      else
        return new XAndYColumn(table, groupNumber) { YColumn = ycol };
    }


    private void AvailableItems_ClearSelection()
    {
      _availableItemsRootNode.ClearSelectionRecursively();
    }


    private bool EhCanChangeTableForSelectedItems()
    {
      return ColumnPlotDataExchangeTableData.CanChangeTableForIColumnPlotDataItems(
        DataItems.Items.Where(n => n.IsSelected && n.Tag is IColumnPlotData).Select(n => (IColumnPlotData)n.Tag));
    }

    private void EhChangeTableForSelectedItems()
    {
      // get all selected plot items with IColumnPlotData
      var selectedNodes = DataItems.Items.Where(n => n.IsSelected && n.Tag is IColumnPlotData);
      var selectedPlotItems = selectedNodes.Select(n => (IColumnPlotData)n.Tag);

      ColumnPlotDataExchangeTableData.ShowChangeTableForSelectedItemsDialog(selectedPlotItems);

      // update the text for the items here
      foreach (var selNode in selectedNodes)
      {
        ((MyNode)selNode).UpdateName();
      }
    }

    private bool EhCanChangeColumnsForSelectedItems()
    {
      return ColumnPlotDataExchangeColumnsData.CanChangeCommonColumnsForItems(
      DataItems.Items.Where(n => n.IsSelected && n.Tag is IColumnPlotData).Select(n => (IColumnPlotData)(n.Tag)));
    }

    private void EhChangeColumnsForSelectedItems()
    {
      // get all selected plot items with IColumnPlotData
      var selectedNodes = DataItems.Items.Where(n => n.IsSelected && n.Tag is IColumnPlotData);
      var selectedPlotItems = selectedNodes.Select(n => (IColumnPlotData)(n.Tag));

      ColumnPlotDataExchangeColumnsData.ShowChangeColumnsForSelectedItemsDialog(selectedPlotItems);

      // update the text for the items here
      foreach (var selNode in selectedNodes)
      {
        ((MyNode)selNode).UpdateName();
      }
    }





    public override bool Apply(bool disposeController)
    {
      int numberOfItems = _dataNodes.Count;

      var list = new List<XAndYColumn>();

      for (int j = 0; j < numberOfItems; ++j)
      {
        list.Add(_dataNodes[j].Tag as XAndYColumn);
      }

      _doc.CurveData = list;

      return ApplyEnd(true, disposeController);
    }
  }
}
