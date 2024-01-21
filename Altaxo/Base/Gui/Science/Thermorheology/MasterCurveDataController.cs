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
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Altaxo.Gui.Data;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveDataView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IMasterCurveDataView))]
  [UserControllerForObject(typeof(MasterCurveData))]
  public partial class MasterCurveDataController : MVCANControllerEditCopyOfDocBase<MasterCurveData, IMasterCurveDataView>
  {
    string _property1;
    string _property2;


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public MasterCurveDataController()
    {
      CommandChangeTableForSelectedItems = new RelayCommand(EhChangeTableForSelectedItems, EhCanChangeTableForSelectedItems);
      CommandChangeColumnsForSelectedItems = new RelayCommand(EhChangeColumnsForSelectedItems, EhCanChangeColumnsForSelectedItems);

      CmdPutDataToPlotItems = new RelayCommand(AvailableItems_PutDataToPlotItems);
      CmdPLotItemsMoveUpSelected = new RelayCommand(PlotItems_MoveUpSelected);
      CmdPLotItemsMoveDownSelected = new RelayCommand(PlotItems_MoveDownSelected);
      CmdPlotItemsDelete = new RelayCommand(PlotItems_Delete);
      CmdPlotItemOpen = new RelayCommand(PlotItem_Open);
      CmdMasterDataDoubleClick = new RelayCommand(PlotItem_Open);
      AvailableItemsDragHandler = new AvailableItems_DragHandler(this);
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

      if (_dataNodes.Count > 1)
      {
        // if we have more than one group,
        // we have to add empty items at the end of the list
        // in order to maintain the number of items
        for (int i = 0; i < itemsDeleted; i++)
        {
          sublist.Add(NewEmptyGuiNode);
        }
      }

    }

    #region Bindings
    public ICommand CommandChangeTableForSelectedItems { get; }
    public ICommand CommandChangeColumnsForSelectedItems { get; }

    public ICommand CmdPutDataToPlotItems { get; }
    public ICommand CmdPLotItemsMoveUpSelected { get; }
    public ICommand CmdPLotItemsMoveDownSelected { get; }
    public ICommand CmdPlotItemsDelete { get; }
    public ICommand CmdPlotItemOpen { get; }
    public ICommand CmdMasterDataDoubleClick { get; }


    private ItemsController<int> _dataGroup;

    public ItemsController<int> DataGroup
    {
      get => _dataGroup;
      set
      {
        if (!(_dataGroup == value))
        {
          _dataGroup = value;
          OnPropertyChanged(nameof(DataGroup));
        }
      }
    }


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

    IEnumerable<NGTreeNode> AvailableItemsSelected
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
        CreateDataGroupTabs(_dataNodes.Count, 0);
      }
    }

    void CreateDataGroupTabs(int numberOfGroups, int indexOfSelectedGroup)
    {
      DataGroup = new ItemsController<int>(new SelectableListNodeList(
          Enumerable.Range(0, numberOfGroups).Select(i => new SelectableListNode($"Group {i}", i, false))
          ), EhSelectedGroupChanged);

      if (numberOfGroups > 0)
      {
        DataGroup.SelectedValue = Math.Min(indexOfSelectedGroup, numberOfGroups - 1);
      }
    }


    private void EhSelectedGroupChanged(int selectedGroup)
    {
      if (selectedGroup >= 0 && selectedGroup < _dataNodes.Count)
      {
        DataItems = new ItemsController<XAndYColumn?>(_dataNodes[selectedGroup]);
      }
      else
      {
        DataItems.Items.Clear();
      }

    }

    /// <summary>
    /// Puts the selected data columns into the plot content.
    /// </summary>
    public void AvailableItems_PutDataToPlotItems()
    {
      var columnsAlreadyProcessed = new HashSet<Altaxo.Data.DataColumn>();

      var selNodes = AvailableItemsSelected;
      var validNodes = NGTreeNode.NodesWithoutSelectedChilds(selNodes);

      // first, put the selected node into the list, even if it is not checked
      foreach (NGTreeNode sn in validNodes)
      {
        var dataCol = sn.Tag as Altaxo.Data.DataColumn;
        if (dataCol is not null && !columnsAlreadyProcessed.Contains(dataCol))
        {
          columnsAlreadyProcessed.Add(dataCol);
          CreatePlotItemNodeAndAddAtEndOfTree(dataCol);
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
              CreatePlotItemNodeAndAddAtEndOfTree(dataCol);
            }
          }
        }
      }

      AvailableItems_ClearSelection();

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    private void CreatePlotItemNodeAndAddAtEndOfTree(DataColumn yCol)
    {
      var node = CreatePlotItem(yCol);
      if (node is not null)
      {
        AppendItemToGuiList(node, DataGroup.SelectedValue);
      }
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
      // see XYPlotLayerContentsController
      return false;
    }

    private void EhChangeTableForSelectedItems()
    {
      // see XYPlotLayerContentsController
    }

    private bool EhCanChangeColumnsForSelectedItems()
    {
      // see XYPlotLayerContentsController
      return false;
    }

    private void EhChangeColumnsForSelectedItems()
    {
      // see XYPlotLayerContentsController
    }

    public void HintOptionValues(int numberOfGroups, string Property1, string Property2)
    {
      if (_dataNodes.Count != numberOfGroups)
      {
        var indexOfSelectedGroup = DataGroup.SelectedValue;
        EnsureNumberOfGroupsInGuiListIsExactly(numberOfGroups);
        CreateDataGroupTabs(numberOfGroups, indexOfSelectedGroup);
      }

      if (_property1 != Property1 || _property2 != Property2)
      {
        _property1 = Property1;
        _property2 = Property2;

        if (_property1 != string.Empty || _property2 != string.Empty)
        {
          Task.Run(() => { UpdateAllGuiNodesWithProperties(); });
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      int numberOfItems = _dataNodes.Max(x => x.IndexOfLast((n, i) => n.Tag is not null));

      var list = new List<XAndYColumn?[]>();
      for (int i = 0; i < _dataNodes.Count; ++i)
      {
        var nodearr = new XAndYColumn?[numberOfItems];
        for (int j = 0; j < numberOfItems; ++j)
          nodearr[j] = _dataNodes[i][j].Tag as XAndYColumn;
        list.Add(nodearr);
      }

      _doc.CurveData = list;

      return ApplyEnd(true, disposeController);
    }
  }
}
