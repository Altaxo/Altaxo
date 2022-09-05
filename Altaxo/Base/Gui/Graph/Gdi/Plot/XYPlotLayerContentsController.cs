#define VERIFY_TREESYNCHRONIZATION

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;
using Altaxo.Main;
using Altaxo.Serialization.Clipboard;

namespace Altaxo.Gui.Graph.Gdi.Plot
{
  public interface IXYPlotLayerContentsView : IDataContextAwareView
  {

  }


  /// <summary>
  /// Controls the content of a <see cref="PlotItemCollection" />
  /// </summary>
  [UserControllerForObject(typeof(PlotItemCollection))]
  [ExpectedTypeOfView(typeof(IXYPlotLayerContentsView))]
  public partial class XYPlotLayerContentsController
    :
    MVCANControllerEditOriginalDocBase<PlotItemCollection, IXYPlotLayerContentsView>
  {
    public XYPlotLayerContentsController()
    {
      CommandChangeTableForSelectedItems = new RelayCommand(EhChangeTableForSelectedItems, EhCanChangeTableForSelectedItems);
      CommandChangeColumnsForSelectedItems = new RelayCommand(EhChangeColumnsForSelectedItems, EhCanChangeColumnsForSelectedItems);
      CmdPutDataToPlotItems = new RelayCommand(AvailableItems_PutDataToPlotItems);
      CmdPLotItemsMoveUpSelected = new RelayCommand(PlotItems_MoveUpSelected);
      CmdPLotItemsMoveDownSelected = new RelayCommand(PlotItems_MoveDownSelected);
      CmdPlotItemOpen = new RelayCommand(PlotItem_Open);
      CmdPlotItemsGroupIntoExistent = new RelayCommand(PlotItems_GroupIntoExistentGroup);
      CmdPlotItemsGroupIntoNew = new RelayCommand(PlotItems_GroupIntoNewGroup);
      CmdPlotItemsUngroup = new RelayCommand(PlotItems_UngroupClick);
      CmdPlotItemsEditRange = new RelayCommand(PlotItems_EditRangeClick);

      CmdPlotItemsCopy = new RelayCommand(PlotItems_Copy, PlotItems_CanCopy);
      CmdPlotItemsCut = new RelayCommand(PlotItems_Cut, PlotItems_CanCut);
      CmdPlotItemsPaste = new RelayCommand(PlotItems_Paste, PlotItems_CanPaste);
      CmdPlotItemsDelete = new RelayCommand(PlotItems_Delete, PlotItems_CanDelete);
      CmdPlotItemDoubleClick = new RelayCommand(PlotItem_DoubleClick);
      PlotItemsDragDropHandler = new PlotItems_DragDropHandler(this);
      AvailableItemsDragHandler = new AvailableItems_DragHandler(this);
    }


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public ICommand CommandChangeTableForSelectedItems { get; }
    public ICommand CommandChangeColumnsForSelectedItems { get; }

    public ICommand CmdPutDataToPlotItems { get; }
    public ICommand CmdPLotItemsMoveUpSelected { get; }
    public ICommand CmdPLotItemsMoveDownSelected { get; }
    public ICommand CmdPlotItemOpen { get; }
    public ICommand CmdPlotItemsGroupIntoExistent { get; }
    public ICommand CmdPlotItemsGroupIntoNew { get; }
    public ICommand CmdPlotItemsUngroup { get; }
    public ICommand CmdPlotItemsEditRange { get; }
    public ICommand CmdPlotItemsCopy { get; }
    public ICommand CmdPlotItemsCut { get; }
    public ICommand CmdPlotItemsPaste { get; }
    public ICommand CmdPlotItemsDelete { get; }

    public ICommand CmdPlotItemDoubleClick { get; }

    public IMVVMDragDropHandler PlotItemsDragDropHandler { get; }

    public IMVVMDragHandler AvailableItemsDragHandler { get; }


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


    private NGTreeNode _plotItemsRootNode;

    /// <summary>
    /// Initializes the content list box by setting the items.
    /// </summary>
    public NGTreeNode PlotItems
    {
      get => _plotItemsRootNode;
      set
      {
        if (!(_plotItemsRootNode == value))
        {
          _plotItemsRootNode = value;
          OnPropertyChanged(nameof(PlotItems));
        }
      }
    }


    IEnumerable<NGTreeNode> PlotItemsSelected
    {
      get
      {
        return TreeNodeExtensions.TakeFromHereToFirstLeaves(_plotItemsRootNode, false).Where(n => n.IsSelected);
      }
    }

    private bool _showRange;

    public bool ShowRange
    {
      get => _showRange;
      set
      {
        if (!(_showRange == value))
        {
          _showRange = value;
          OnPropertyChanged(nameof(ShowRange));
          EhShowRangeChanged(value);
        }
      }
    }

    public void EhShowRangeChanged(bool value)
    {
      _plotItemsRootNode.Nodes.Clear();
      PlotItemsToTree(_plotItemsRootNode, _doc); // rebuild the tree with the changed names
    }

    private ItemsController<LayerDataClipping> _dataClipping;

    /// <summary>
    /// The data clipping choices. 
    /// </summary>
    public ItemsController<LayerDataClipping> DataClipping
    {
      get => _dataClipping;
      set
      {
        if (!(_dataClipping == value))
        {
          _dataClipping = value;
          OnPropertyChanged(nameof(DataClipping));
        }
      }
    }

    #endregion

    public override void Dispose(bool isDisposing)
    {
      _plotItemsRootNode = null;
      _availableItemsRootNode = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      // now fill the tree view  with all plot associations currently inside
      if (initData)
      {

        _plotItemsRootNode = new NGTreeNode() { IsExpanded = true };
        _availableItemsRootNode = new NGTreeNode();

        PlotItemsToTree(_plotItemsRootNode, _doc);

#if VERIFY_TREESYNCHRONIZATION
        if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
          throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif

        // available items
        SingleColumnChoiceController.AddAllTableNodes(_availableItemsRootNode);

        // Data clipping
        var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(_doc);
        if (layer is null)
        {
          DataClipping = null;
        }
        else
        {
          var dataClippingChoices = new SelectableListNodeList();
          foreach (var value in new[] { Altaxo.Graph.LayerDataClipping.None, Altaxo.Graph.LayerDataClipping.StrictToCS })
            dataClippingChoices.Add(new SelectableListNode(value.ToString(), value, value.ToString() == layer.ClipDataToFrame.ToString()));
          DataClipping = new ItemsController<LayerDataClipping>(dataClippingChoices);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif

      TreeNodeExtensions.FixAndTestParentChildRelations<IGPlotItem>(_doc, (x, y) => x.ParentObject = (Altaxo.Main.IDocumentNode)y);

      if (DataClipping is not null)
      {
        var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(_doc);
        if (layer is not null)
        {
          layer.ClipDataToFrame = DataClipping.SelectedValue;
        }
      }

      if (!disposeController)
        Initialize(true); // Reload the applied contents to make sure it is synchronized

      return ApplyEnd(true, disposeController); // all ok
    }


    /// <summary>
    /// Gets the suspend token for the controller document. Here we try to suspend the parent XYPlotLayer instead of the PlotItemCollection, because we want to modify the
    /// clipping property of the layer too.
    /// </summary>
    /// <returns>Suspend token of the parent XYPlotLayer; if possible; otherwise suspend token for the PlotItemCollection itself.</returns>
    protected override Altaxo.Main.ISuspendToken GetSuspendTokenForControllerDocument()
    {
      // in this override, we try to suspend the XYPlotLayer this collection belongs to
      // this is because we want to modify properties of the layer, like the clip property
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(_doc);

      if (layer is not null)
        return layer.SuspendGetToken();
      else
        return base.GetSuspendTokenForControllerDocument();
    }

    private void AvailableItems_ClearSelection()
    {
      _availableItemsRootNode.ClearSelectionRecursively();
    }



    private string GetNameOfItem(IGPlotItem item)
    {
      if (item is PlotItemCollection)
      {
        return "PlotGroup";
      }
      else if (item is not null && item is PlotItem)
      {
        string name = item.GetName(2);
        return name;
      }
      else
      {
        return string.Empty;
      }
    }

    private void PlotItemsToTree(NGTreeNode node, IGPlotItem plotItem)
    {
      var picoll = plotItem as PlotItemCollection;

      node.Tag = plotItem;
      node.Text = GetNameOfItem(plotItem);
      node.IsExpanded = true;

      if (picoll is not null) // Plot item collection
      {
        node.Tag = plotItem;
        node.Text = GetNameOfItem(plotItem);
        node.IsExpanded = true;

        foreach (var childPlotItem in picoll)
        {
          var childNode = new NGTreeNode();
          PlotItemsToTree(childNode, childPlotItem);
          node.Nodes.Add(childNode);
        }
      }
    }

    private PlotItemCollection PutSelectedPlotItemsToTemporaryDocumentForClipboard(NGTreeNode[] selNodes)
    {
      var picoll = new PlotItemCollection();

      foreach (NGTreeNode node in selNodes)
      {
        var item = (IGPlotItem)node.Tag;
        var clonedItem = (IGPlotItem)item.Clone(); // clone necessary to maintain parent-child relationship and to prevent disposing of original items
        picoll.Add(clonedItem);
      }

      return picoll;
    }

    private static XYColumnPlotItem FindFirstXYColumnPlotItem(PlotItemCollection coll)
    {
      // search in our document for the first plot item that is XYColumnPlotItem,
      // we need this as template style
      foreach (IGPlotItem pi in coll)
      {
        if (pi is PlotItemCollection)
        {
          XYColumnPlotItem result = FindFirstXYColumnPlotItem(pi as PlotItemCollection);
          if (result is not null)
            return result;
        }
        else if (pi is XYColumnPlotItem)
        {
          return pi as XYColumnPlotItem;
        }
      }
      return null;
    }

    private IGPlotItem CreatePlotItem(string tablename, string columnname)
    {
      if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(columnname))
        return null;

      // create a new plotassociation from the column
      // first, get the y column from table and name
      DataTable tab = Current.Project.DataTableCollection[tablename];
      if (tab is not null)
      {
        DataColumn ycol = tab[columnname];
        if (ycol is not null)
        {
          int groupNumber = tab.DataColumns.GetColumnGroup(ycol);
          DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

          // search in our document for the first plot item that is an XYColumnPlotItem,
          // we need this as template style
          XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
          G2DPlotStyleCollection templatePlotStyle;
          if (templatePlotItem is not null)
          {
            templatePlotStyle = templatePlotItem.Style.Clone();
          }
          else // there is no item that can be used as template
          {
            int numRows = ycol.Count;
            if (xcol is not null)
              numRows = Math.Min(numRows, xcol.Count);
            if (numRows < 100)
            {
              templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, _doc.GetPropertyContext());
            }
            else
            {
              templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, _doc.GetPropertyContext());
            }
          }

          XYColumnPlotItem result;
          if (xcol is null)
            result = new XYColumnPlotItem(new XYColumnPlotData(tab, groupNumber, new Altaxo.Data.IndexerColumn(), ycol), templatePlotStyle);
          else
            result = new XYColumnPlotItem(new XYColumnPlotData(tab, groupNumber, xcol, ycol), templatePlotStyle);

          return result;
        }
      }
      return null;
    }

    private IGPlotItem CreatePlotItem(Altaxo.Data.DataColumn ycol)
    {
      if (ycol is null)
        return null;

      var tab = DataTable.GetParentDataTableOf(ycol);
      if (tab is null)
        return null;

      var groupNumber = tab.DataColumns.GetColumnGroup(ycol);
      DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

      // search in our document for the first plot item that is an XYColumnPlotItem,
      // we need this as template style
      XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
      G2DPlotStyleCollection templatePlotStyle;
      if (templatePlotItem is not null)
      {
        templatePlotStyle = templatePlotItem.Style.Clone();
      }
      else // there is no item that can be used as template
      {
        int numRows = ycol.Count;
        if (xcol is not null)
          numRows = Math.Min(numRows, xcol.Count);
        if (numRows < 100)
        {
          templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter, _doc.GetPropertyContext());
        }
        else
        {
          templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, _doc.GetPropertyContext());
        }
      }

      XYColumnPlotItem result;
      if (xcol is null)
        result = new XYColumnPlotItem(new XYColumnPlotData(tab, groupNumber, new Altaxo.Data.IndexerColumn(), ycol), templatePlotStyle);
      else
        result = new XYColumnPlotItem(new XYColumnPlotData(tab, groupNumber, xcol, ycol), templatePlotStyle);

      return result;
    }

    #region ILineScatterLayerContentsController Members

    public void EhView_DataAvailableBeforeExpand(NGTreeNode node)
    {
      DataTable dt = Current.Project.DataTableCollection[node.Text];
      if (dt is not null)
      {
        node.Nodes.Clear();
        var toadd = new NGTreeNode[dt.DataColumns.ColumnCount];
        for (int i = 0; i < toadd.Length; i++)
        {
          toadd[i] = new NGTreeNode(dt[i].Name);
        }
        node.Nodes.AddRange(toadd);
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
        else if (sn is SingleColumnChoiceController.TableNode)
        {
          var rangeNode = sn as SingleColumnChoiceController.TableNode;
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

    private NGTreeNode CreatePlotItemNode(IGPlotItem plotItem)
    {
      if (plotItem is null)
        throw new ArgumentNullException();

      var newNode = new NGTreeNode
      {
        Text = plotItem.GetName(2),
        Tag = plotItem
      };
      return newNode;
    }

    private NGTreeNode CreatePlotItemNode(DataColumn dataCol)
    {
      IGPlotItem newItem = CreatePlotItem(dataCol);
      if (newItem is not null)
      {
        return CreatePlotItemNode(newItem);
      }
      else
      {
        return null;
      }
    }

    private void CreatePlotItemNodeAndAddAtEndOfTree(DataColumn dataCol)
    {
      var node = CreatePlotItemNode(dataCol);
      if (node is not null)
      {
        _plotItemsRootNode.Nodes.Add(node);
        _doc.Add((IGPlotItem)node.Tag);
      }
    }

    public void PlotItems_MoveUpSelected()
    {
      var selNodes = PlotItemsSelected.ToArray();
      if (selNodes.Length != 0)
      {
        // move the selected items upwards in the list
        ContentsListBox_MoveUpDown(-1, selNodes);
      }
    }

    public void PlotItems_MoveDownSelected()
    {
      var selNodes = PlotItemsSelected.ToArray();
      if (selNodes.Length != 0)
      {
        // move the selected items downwards in the list
        ContentsListBox_MoveUpDown(1, selNodes);
      }
    }

    public void ContentsListBox_MoveUpDown(int iDelta, NGTreeNode[] selNodes)
    {
      if (NGTreeNode.HaveSameParent(selNodes))
      {
        NGTreeNode.MoveUpDown(iDelta, selNodes);
        var plotItems = new HashSet<IGPlotItem>(selNodes.Select(x => (IGPlotItem)x.Tag));
        TreeNodeExtensions.MoveNodesUpDown(iDelta, plotItems);
        // _view.InitializePlotItems(this._plotItemsTree);
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    /// <summary>
    /// Group the selected nodes into a new group.
    /// </summary>
    public void PlotItems_GroupIntoNewGroup() => PlotItems_GroupClick(true);

    /// <summary>
    /// Group the selected nodes. The items were grouped into the first group that was selected.
    /// If no group is selected, the items are grouped into a new group.
    /// </summary>
    public void PlotItems_GroupIntoExistentGroup() => PlotItems_GroupClick(false);

    /// <summary>
    /// Group the selected nodes.
    /// </summary>
    /// <param name="groupIntoNewGroup">If true, all items where grouped into a new group.
    /// If false, the items were grouped into the first group that was selected. If no group is selected,
    /// the behavior is the same as if this parameter is true.</param>
    public void PlotItems_GroupClick(bool groupIntoNewGroup)
    {
      var selNodes = PlotItemsSelected.ToArray();

      // retrieve the selected items
      if (selNodes.Length < 2)
        return; // we cannot group anything if no or only one item is selected

      // look, if one of the selected items is a plot group
      // if found, use this group and add the remaining items to this
      int foundindex = -1;
      if (!groupIntoNewGroup)
      {
        for (int i = 0; i < selNodes.Length; i++)
        {
          if (selNodes[i].Tag is PlotItemCollection)
          {
            foundindex = i;
            break;
          }
        }
      }

      // if a group was found use this to add the remaining items
      if (foundindex >= 0)
      {
        for (int i = 0; i < selNodes.Length; i++)
          if (i != foundindex)
          {
            var selNode = selNodes[i];
            var selPlotItem = (IGPlotItem)selNodes[i].Tag;
            selNode.Remove();
            selPlotItem.Remove();

            var newParentNode = selNodes[foundindex];
            var newParentPlotColl = (PlotItemCollection)newParentNode.Tag;
            newParentNode.Nodes.Add(selNode);
            newParentPlotColl.Add(selPlotItem);
          }
      }
      else // if we found no group to add to, we have to create a new group
      {
        var newParentNode = new NGTreeNode();
        var newParentPlotColl = new PlotItemCollection();
        newParentNode.Tag = newParentPlotColl;
        newParentNode.Text = "PlotGroup";
        newParentNode.IsExpanded = true;

        // now add the remaining selected items to the found group
        for (int i = 0; i < selNodes.Length; i++)
        {
          var selNode = selNodes[i];
          var selPlotItem = (IGPlotItem)selNode.Tag;
          /*
          if (selNode.Nodes.Count > 0) // if it is a group, add the members of the group to avoid more than one recursion
          {
            while (selNode.Nodes.Count > 0)
            {
              var addnode = selNode.Nodes[0];
              var addPlotItem = (IGPlotItem)addnode.Tag;

              addnode.Remove();
              addPlotItem.Remove();

              newParentNode.Nodes.Add(addnode);
              newParentPlotColl.Add(addPlotItem);
            }
          }
          else // item to add is not a group
          */
          {
            selNode.Remove();
            selPlotItem.Remove();
            newParentNode.Nodes.Add(selNode);
            newParentPlotColl.Add(selPlotItem);
          }
        } // end for
        _plotItemsRootNode.Nodes.Add(newParentNode);
        _doc.Add(newParentPlotColl);
      }
      // now all items are in the new group

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    public void PlotItems_UngroupClick()
    {
      var selNodes = PlotItemsSelected.ToArray();

      // retrieve the selected items
      if (selNodes.Length < 1)
        return; // we cannot ungroup anything if nothing selected

      selNodes = NGTreeNode.FilterIndependentNodes(selNodes);

      for (int i = 0; i < selNodes.Length; i++)
      {
        var selNode = selNodes[i];

        if (selNode.Nodes.Count == 0 && selNode.ParentNode is not null && selNode.ParentNode.ParentNode is not null)
        {
          NGTreeNode parent = selNode.ParentNode;
          NGTreeNode grandParent = parent.ParentNode;

          selNode.Remove();
          ((IGPlotItem)selNode.Tag).Remove();
          grandParent.Nodes.Add(selNode);
          ((PlotItemCollection)grandParent.Tag).Add((IGPlotItem)selNode.Tag);

          if (parent.Nodes.Count == 0)
          {
            parent.Remove();
            ((IGPlotItem)parent.Tag).Remove();
          }
        }
        else if (selNode.Nodes.Count > 0 && selNode.ParentNode is not null)
        {
          NGTreeNode parent = selNode.ParentNode;
          while (selNode.Nodes.Count > 0)
          {
            NGTreeNode firstNode = selNode.Nodes[0];

            firstNode.Remove();
            ((IGPlotItem)firstNode.Tag).Remove();
            parent.Nodes.Add(firstNode);
            ((PlotItemCollection)parent.Tag).Add((IGPlotItem)firstNode.Tag);
          }
          selNode.Remove();
          ((IGPlotItem)selNode.Tag).Remove();
        }
      } // end for

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    public void PlotItem_DoubleClick()
    {
      if (!PlotItemsSelected.TryGetSingleElement(out var selNode))
        return;

      var pi = selNode.Tag as IGPlotItem;
      if (pi is not null)
      {
        if (pi is PlotItemCollection)
        {
          // show not the dialog for PlotItemCollection, but only those for the group styles into that collection
          Current.Gui.ShowDialog(new object[] { ((PlotItemCollection)pi).GroupStyles }, pi.Name);
        }
        else
        {
          object piAsObject = pi;
          Current.Gui.ShowDialog(ref piAsObject, pi.GetName(2), true);
          pi = (IGPlotItem)piAsObject;
        }
      }

      // now set a new name of this node
      selNode.Text = GetNameOfItem(pi);
      selNode.Tag = pi;
    }

    public void PlotItems_EditRangeClick()
    {
      var selNodes = PlotItemsSelected;
      if (!selNodes.Any())
        return;

      int maxRange = -1;


      if (!GetMinimumMaximumPlotRange(selNodes, out var minRange, out maxRange))
        return;

      var range = Altaxo.Data.Selections.RangeOfRowIndices.FromStartAndEndInclusive(minRange, maxRange);

      if (!Current.Gui.ShowDialog(ref range, "Edit plot range for selected items", false))
        return;

      foreach (NGTreeNode node in selNodes)
      {
        var pi = node.Tag as XYColumnPlotItem;
        if (pi is null)
          continue;
        pi.Data.DataRowSelection = (Altaxo.Data.Selections.RangeOfRowIndices)range.Clone();
      }
    }

    private bool GetMinimumMaximumPlotRange(IEnumerable<NGTreeNode> selNodes, out int minInclusive, out int maxInclusive)
    {
      int lowerBoundNegMax = int.MinValue;
      int lowerBoundPosMax = int.MinValue;

      int upperBoundNegMin = int.MaxValue;
      int upperBoundPosMin = int.MaxValue;

      bool result = false;

      foreach (NGTreeNode node in selNodes)
      {
        if (node.Tag is XYColumnPlotItem pi)
        {
          // we ask for the same boundaries, first with int.Max and second with int.Max-1 as upper boundary
          // if the two values (lower, lower1) are different, then the boundary was given as negative value
          // the same holds if (upper, upper1) are different
          if (
            pi.Data.DataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, int.MaxValue, pi.Data.DataTable?.DataColumns, int.MaxValue).TryGetFirstAndLast(out var firstSeg, out var lastSeg) &&
            pi.Data.DataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, int.MaxValue - 1, pi.Data.DataTable?.DataColumns, int.MaxValue - 1).TryGetFirstAndLast(out var firstSeg1, out var lastSeg1))
          {
            int lower = firstSeg.start;
            int upper = lastSeg.endExclusive;
            int lower1 = firstSeg1.start;
            int upper1 = lastSeg1.endExclusive;

            if (lower != lower1)
              lower = (lower - int.MaxValue) - 1; // convert to negative boundary

            // upper is exclusive bound here,
            // thus we convert it to inclusive bound
            if (upper != upper1)
              upper = (upper - int.MaxValue) - 1; // convert to negative inclusive boundary
            else
              upper = upper - 1; // convert to inclusive boundary


            if (lower < 0)
              lowerBoundNegMax = Math.Max(lower, lowerBoundNegMax);
            else
              lowerBoundPosMax = Math.Max(lower, lowerBoundPosMax);

            if (upper < 0)
              upperBoundNegMin = Math.Min(upper, upperBoundNegMin);
            else
              upperBoundPosMin = Math.Min(upper, upperBoundPosMin);
          }
          result = true;
        }
      }

      if (lowerBoundPosMax >= 0 && lowerBoundNegMax == int.MinValue)
        minInclusive = lowerBoundPosMax;
      else if (lowerBoundNegMax < 0 && lowerBoundPosMax == int.MinValue)
        minInclusive = lowerBoundNegMax;
      else // we have both positive and negative values for lowerBound - we set it to 0
        minInclusive = 0;

      if (upperBoundNegMin < 0 && upperBoundPosMin == int.MaxValue)
        maxInclusive = upperBoundNegMin;
      else if (upperBoundPosMin >= 0 && upperBoundNegMin == int.MaxValue)
        maxInclusive = upperBoundPosMin;
      else // we have both positive and negative values for upperBound - we set it to -1
        maxInclusive = -1;

      return result;
    }

    private bool EhCanChangeTableForSelectedItems()
    {
      return ColumnPlotDataExchangeTableData.CanChangeTableForPlotItems(

      PlotItemsSelected.Where(n => n.Tag is Altaxo.Graph.Plot.IGPlotItem item && item.DataObject is IColumnPlotData)
        .Select(n => (Altaxo.Graph.Plot.IGPlotItem)(n.Tag)));
    }

    private void EhChangeTableForSelectedItems()
    {
      // get all selected plot items with IColumnPlotData
      var selectedNodes = PlotItemsSelected.Where(n => n.Tag is Altaxo.Graph.Plot.IGPlotItem item && item.DataObject is IColumnPlotData);
      var selectedPlotItems = selectedNodes.Select(n => (Altaxo.Graph.Plot.IGPlotItem)(n.Tag));

      ColumnPlotDataExchangeTableData.ShowChangeTableForSelectedItemsDialog(selectedPlotItems);

      // update the text for the items here

      foreach (var selNode in selectedNodes)
      {
        selNode.Text = GetNameOfItem((IGPlotItem)selNode.Tag);
      }
    }

    private bool EhCanChangeColumnsForSelectedItems()
    {
      return ColumnPlotDataExchangeColumnsData.CanChangeCommonColumnsForPlotItems(

      PlotItemsSelected.Where(n => n.Tag is Altaxo.Graph.Plot.IGPlotItem item && item.DataObject is IColumnPlotData)
        .Select(n => (Altaxo.Graph.Plot.IGPlotItem)(n.Tag)));
    }

    private void EhChangeColumnsForSelectedItems()
    {
      // get all selected plot items with IColumnPlotData
      var selectedNodes = PlotItemsSelected.Where(n => n.Tag is Altaxo.Graph.Plot.IGPlotItem item && item.DataObject is IColumnPlotData);
      var selectedPlotItems = selectedNodes.Select(n => (Altaxo.Graph.Plot.IGPlotItem)(n.Tag));

      ColumnPlotDataExchangeColumnsData.ShowChangeColumnsForSelectedItemsDialog(selectedPlotItems);

      // update the text for the items here

      foreach (var selNode in selectedNodes)
      {
        selNode.Text = GetNameOfItem((IGPlotItem)selNode.Tag);
      }
    }

    public void PlotItem_Open()
    {
      PlotItem_DoubleClick();
    }

    public bool PlotItems_CanDelete()
    {
      return true;
    }

    public void PlotItems_Delete()
    {
      var selNodes = PlotItemsSelected.ToArray();
      foreach (var node in selNodes)
      {
        TreeNodeExtensions.Remove((IGPlotItem)node.Tag);
        node.Remove();
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    public bool PlotItems_CanCopy()
    {
      return PlotItemsSelected.Any();
    }
    public void PlotItems_Copy()
    {
      var selNodes = NGTreeNode.FilterIndependentNodes(PlotItemsSelected.ToArray());

      PlotItemCollection coll = PutSelectedPlotItemsToTemporaryDocumentForClipboard(selNodes);

      ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml", coll);
    }

    public bool PlotItems_CanCut()
    {
      return PlotItemsSelected.Any();
    }
    public void PlotItems_Cut()
    {
      var selNodes = PlotItemsSelected;
      PlotItems_Copy();
      foreach (var node in selNodes)
      {
        ((IGPlotItem)node.Tag).Remove();
        node.Remove();
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    public bool PlotItems_CanPaste()
    {
      object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
      var coll = o as PlotItemCollection;
      return coll is not null;
    }

    public void PlotItems_Paste()
    {
      object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
      var coll = o as PlotItemCollection;
      // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
      if (coll is not null)
      {
        foreach (IGPlotItem item in coll) // it is neccessary to add the items to the doc first, because otherwise they don't have names
        {
          var clonedItem = (IGPlotItem)item.Clone();
          _doc.Add(clonedItem); // cloning neccessary because coll will be disposed afterwards, which would destroy all items
          var newNode = new NGTreeNode();
          PlotItemsToTree(newNode, clonedItem);
          _plotItemsRootNode.Nodes.Add(newNode);
        }
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    #endregion ILineScatterLayerContentsController Members


    #region Inner classes

    private class DummyPlotItem : Altaxo.Main.SuspendableDocumentLeafNode, IGPlotItem
    {
      public string GetName(int level)
      {
        throw new NotImplementedException();
      }

      public string GetName(string style)
      {
        throw new NotImplementedException();
      }

      public PlotItemCollection ParentCollection
      {
        get { throw new NotImplementedException(); }
      }

      public void CollectStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection styles)
      {
        throw new NotImplementedException();
      }

      public void PrepareGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection styles, IPlotArea layer)
      {
        throw new NotImplementedException();
      }

      public void ApplyGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection styles)
      {
        throw new NotImplementedException();
      }

      public void SetPlotStyleFromTemplate(IGPlotItem template, Altaxo.Graph.Plot.Groups.PlotGroupStrictness strictness)
      {
        throw new NotImplementedException();
      }

      public void PrepareScales(IPlotArea layer)
      {
        throw new NotImplementedException();
      }

      public void PaintPreprocessing(IPaintContext context)
      {
        throw new NotImplementedException();
      }

      public void Paint(Graphics g, IPaintContext context, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem)
      {
        throw new NotImplementedException();
      }

      public void PaintPostprocessing()
      {
        throw new NotImplementedException();
      }

      public void PaintSymbol(Graphics g, RectangleF location)
      {
        throw new NotImplementedException();
      }

      public IHitTestObject HitTest(IPlotArea layer, PointD2D hitpoint)
      {
        throw new NotImplementedException();
      }

      public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
      {
        throw new NotImplementedException();
      }

      public IDocumentLeafNode DataObject { get { return null; } }

      public IDocumentLeafNode StyleObject { get { return null; } }

      public bool CopyFrom(object obj)
      {
        if (ReferenceEquals(this, obj))
          return true;

        throw new NotImplementedException();
      }

      public object Clone()
      {
        throw new NotImplementedException();
      }

      public IEnumerable<Altaxo.Main.IDocumentLeafNode> ChildNodes
      {
        get { throw new NotImplementedException(); }
      }

      public Altaxo.Main.IDocumentLeafNode ParentNode
      {
        get { throw new NotImplementedException(); }
      }

      IList<IGPlotItem> ITreeListNode<IGPlotItem>.ChildNodes
      {
        get { throw new NotImplementedException(); }
      }

      IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
      {
        get { throw new NotImplementedException(); }
      }

      IGPlotItem INodeWithParentNode<IGPlotItem>.ParentNode
      {
        get { return _parent as IGPlotItem; }
      }

      protected override bool AccumulatedEventData_HasZeroOrOneEventArg(out EventArgs singleEventArg)
      {
        throw new NotImplementedException();
      }

      protected override IEnumerable<EventArgs> AccumulatedEventData
      {
        get { throw new NotImplementedException(); }
      }

      protected override void AccumulatedEventData_Clear()
      {
        throw new NotImplementedException();
      }

      protected override void AccumulateChangeData(object sender, EventArgs e)
      {
        throw new NotImplementedException();
      }

      protected override void AccumulatedChangeData_SetBackAfterResumeAndSuspend(params EventArgs[] e)
      {
        throw new NotImplementedException();
      }
    }

    #endregion Inner classes
  }
}
