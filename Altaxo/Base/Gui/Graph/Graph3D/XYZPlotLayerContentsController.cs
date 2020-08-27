#define VERIFY_TREESYNCHRONIZATION

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Main;
using Altaxo.Serialization.Clipboard;

namespace Altaxo.Gui.Graph.Graph3D
{
  #region Interfaces

  public interface IXYZPlotLayerContentsViewEventSink
  {
    void EhView_DataAvailableBeforeExpand(NGTreeNode node);

    void EhView_ContentsDoubleClick(NGTreeNode selNode);

    void AvailableItems_PutDataToPlotItems();

    void PlotItems_MoveUpSelected();

    void PlotItems_MoveDownSelected();

    void PlotItems_GroupClick();

    void PlotItems_UngroupClick();

    void PlotItems_EditRangeClick();

    void PlotItem_Open();

    void PlotItems_Copy();

    void PlotItems_Cut();

    bool PlotItems_CanPaste();

    void PlotItems_Paste();

    bool PlotItems_CanDelete();

    void PlotItems_Delete();

    void PlotItems_ShowRangeChanged(bool showRange);

    bool PlotItems_CanStartDrag(IEnumerable items);

    void PlotItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

    void PlotItems_DragEnded(bool isCopy, bool isMove);

    void PlotItems_DragCancelled();

    void PlotItems_DropCanAcceptData(object data, NGTreeNode targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData);

    void PlotItems_Drop(object data, NGTreeNode targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove);

    bool AvailableItems_CanStartDrag(IEnumerable items);

    void AvailableItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

    void AvailableItems_DragEnded(bool isCopy, bool isMove);

    void AvailableItems_DragCancelled();
  }

  public interface IXYZPlotLayerContentsView
  {
    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYZPlotLayerContentsViewEventSink Controller { get; set; }

    IEnumerable<object> PlotItemsSelected { get; }

    IEnumerable<object> AvailableItemsSelected { get; }

    /// <summary>
    /// Initializes the treeview of available data with content.
    /// </summary>
    /// <param name="nodes"></param>
    void InitializeAvailableItems(NGTreeNodeCollection nodes);

    /// <summary>
    /// Initializes the content list box by setting the items.
    /// </summary>
    /// <param name="items">Collection of items.</param>
    void InitializePlotItems(NGTreeNodeCollection items);

    /// <summary>
    /// Initializes the data clipping choices. The view should show a appropriate control only after this function has been called.
    /// </summary>
    /// <param name="list">The list.</param>
    void InitializeDataClipping(SelectableListNodeList list);

    bool ShowRange { set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controls the content of a <see cref="PlotItemCollection" />
  /// </summary>
  [UserControllerForObject(typeof(PlotItemCollection))]
  [ExpectedTypeOfView(typeof(IXYZPlotLayerContentsView))]
  public class XYZPlotLayerContentsController
    :
    MVCANControllerEditOriginalDocBase<PlotItemCollection, IXYZPlotLayerContentsView>,
    IXYZPlotLayerContentsViewEventSink, IMVCANController
  {
    private NGTreeNode _plotItemsRootNode;
    private NGTreeNodeCollection _plotItemsTree;
    private NGTreeNode _availableItemsRootNode;
    private SelectableListNodeList _dataClippingChoices;
    public ICommand CommandChangeTableForSelectedItems { get; protected set; }
    public ICommand CommandChangeColumnsForSelectedItems { get; protected set; }

    private bool _showRange = false;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _plotItemsRootNode = null;
      _plotItemsTree = null;
      _availableItemsRootNode = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      // now fill the tree view  with all plot associations currently inside
      if (initData)
      {
        CommandChangeTableForSelectedItems = new RelayCommand(EhChangeTableForSelectedItems, EhCanChangeTableForSelectedItems);
        CommandChangeColumnsForSelectedItems = new RelayCommand(EhChangeColumnsForSelectedItems, EhCanChangeColumnsForSelectedItems);

        _plotItemsRootNode = new NGTreeNode() { IsExpanded = true };
        _plotItemsTree = _plotItemsRootNode.Nodes;
        _availableItemsRootNode = new NGTreeNode();

        PlotItemsToTree(_plotItemsRootNode, _doc);

#if VERIFY_TREESYNCHRONIZATION
        if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
          throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif

        // available items
        Altaxo.Gui.Graph.SingleColumnChoiceController.AddAllTableNodes(_availableItemsRootNode);

        // Data clipping
        var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(_doc);
        if (null == layer)
        {
          _dataClippingChoices = null;
        }
        else
        {
          _dataClippingChoices = new SelectableListNodeList();
          foreach (var value in new[] { Altaxo.Graph.LayerDataClipping.None, Altaxo.Graph.LayerDataClipping.StrictToCS })
            _dataClippingChoices.Add(new SelectableListNode(value.ToString(), value, value.ToString() == layer.ClipDataToFrame.ToString()));
        }
      }

      // Available Items
      if (null != _view)
      {
        _view.InitializePlotItems(_plotItemsTree);

        _view.InitializeAvailableItems(_availableItemsRootNode.Nodes);

        _view.ShowRange = _showRange;

        if (null != _dataClippingChoices)
          _view.InitializeDataClipping(_dataClippingChoices);
      }
    }

    public override bool Apply(bool disposeController)
    {
#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif

      TreeNodeExtensions.FixAndTestParentChildRelations<IGPlotItem>(_doc, (x, y) => x.ParentObject = (Altaxo.Main.IDocumentNode)y);

      if (null != _dataClippingChoices)
      {
        var selNode = _dataClippingChoices.FirstSelectedNode;
        var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(_doc);
        if (null != layer && null != selNode)
          layer.ClipDataToFrame = (Altaxo.Graph.LayerDataClipping)selNode.Tag;
      }

      if (!disposeController)
        Initialize(true); // Reload the applied contents to make sure it is synchronized

      return ApplyEnd(true, disposeController); // all ok
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.Controller = this;
    }

    protected override void DetachView()
    {
      _view.Controller = null;
      base.DetachView();
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
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(_doc);

      if (null != layer)
        return layer.SuspendGetToken();
      else
        return base.GetSuspendTokenForControllerDocument();
    }

    private NGTreeNode[] PlotItemsSelected
    {
      get
      {
        if (null != _view)
          return _view.PlotItemsSelected.OfType<NGTreeNode>().ToArray();
        else
          return new NGTreeNode[0];
      }
    }

    private void AvailableItems_ClearSelection()
    {
      _availableItemsRootNode.ClearSelectionRecursively();
    }

    public void PlotItems_ShowRangeChanged(bool showRange)
    {
      var oldValue = _showRange;
      _showRange = showRange;

      if (oldValue != _showRange)
      {
        _plotItemsTree.Clear();
        PlotItemsToTree(_plotItemsRootNode, _doc); // rebuild the tree with the changed names
      }
    }

    private string GetNameOfItem(IGPlotItem item)
    {
      if (item is PlotItemCollection)
      {
        return "PlotGroup";
      }
      else if (item != null && item is PlotItem)
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

      if (null != picoll) // Plot item collection
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

    private static XYZColumnPlotItem FindFirstXYColumnPlotItem(PlotItemCollection coll)
    {
      // search in our document for the first plot item that is XYColumnPlotItem,
      // we need this as template style
      foreach (IGPlotItem pi in coll)
      {
        if (pi is PlotItemCollection)
        {
          XYZColumnPlotItem result = FindFirstXYColumnPlotItem(pi as PlotItemCollection);
          if (result != null)
            return result;
        }
        else if (pi is XYZColumnPlotItem)
        {
          return pi as XYZColumnPlotItem;
        }
      }
      return null;
    }

    private IGPlotItem CreatePlotItem(string tablename, string columnname)
    {
      throw new NotImplementedException();

      /*

            if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(columnname))
                return null;

            // create a new plotassociation from the column
            // first, get the y column from table and name
            DataTable tab = Current.Project.DataTableCollection[tablename];
            if (null != tab)
            {
                DataColumn ycol = tab[columnname];
                if (null != ycol)
                {
                    DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

                    // search in our document for the first plot item that is an XYColumnPlotItem,
                    // we need this as template style
                    XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
                    G2DPlotStyleCollection templatePlotStyle;
                    if (null != templatePlotItem)
                    {
                        templatePlotStyle = templatePlotItem.Style.Clone();
                    }
                    else // there is no item that can be used as template
                    {
                        int numRows = ycol.Count;
                        if (null != xcol)
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
                    if (null == xcol)
                        result = new XYColumnPlotItem(new XYColumnPlotData(new Altaxo.Data.IndexerColumn(), ycol), templatePlotStyle);
                    else
                        result = new XYColumnPlotItem(new XYColumnPlotData(xcol, ycol), templatePlotStyle);

                    return result;
                }
            }
            return null;

    */
    }

    private IGPlotItem CreatePlotItem(Altaxo.Data.DataColumn zcol)
    {
      if (null == zcol)
        return null;

      var tab = DataTable.GetParentDataTableOf(zcol);
      if (null == tab)
        return null;

      int groupNumber = tab.DataColumns.GetColumnGroup(zcol);

      DataColumn xcol = tab.DataColumns.FindXColumnOf(zcol);
      DataColumn ycol = tab.DataColumns.FindYColumnOf(zcol);

      // search in our document for the first plot item that is an XYColumnPlotItem,
      // we need this as template style
      XYZColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
      G3DPlotStyleCollection templatePlotStyle;
      if (null != templatePlotItem)
      {
        templatePlotStyle = templatePlotItem.Style.Clone();
      }
      else // there is no item that can be used as template
      {
        int numRows = zcol.Count;
        if (null != xcol)
          numRows = Math.Min(numRows, xcol.Count);
        if (numRows < 100)
        {
          // line and scatter
          templatePlotStyle = new G3DPlotStyleCollection
          {
            new ScatterPlotStyle(_doc.GetPropertyContext()),
            new LinePlotStyle(_doc.GetPropertyContext())
          };
        }
        else
        {
          // only line
          templatePlotStyle = new G3DPlotStyleCollection
          {
            new LinePlotStyle(_doc.GetPropertyContext()) // TODO change this to line style if it is implemented
          };
        }
      }

      var xyzPlotData = new XYZColumnPlotData(
        tab,
        groupNumber,
        (IReadableColumn)xcol ?? new IndexerColumn(),
        (IReadableColumn)ycol ?? new ConstantDoubleColumn(0),
        zcol);

      return new XYZColumnPlotItem(xyzPlotData, templatePlotStyle);
    }

    #region ILineScatterLayerContentsController Members

    public void EhView_DataAvailableBeforeExpand(NGTreeNode node)
    {
      DataTable dt = Current.Project.DataTableCollection[node.Text];
      if (null != dt)
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

      var selNodes = _view.AvailableItemsSelected.OfType<NGTreeNode>();
      var validNodes = NGTreeNode.NodesWithoutSelectedChilds(selNodes);

      // first, put the selected node into the list, even if it is not checked
      foreach (NGTreeNode sn in validNodes)
      {
        var dataCol = sn.Tag as Altaxo.Data.DataColumn;
        if (null != dataCol && !columnsAlreadyProcessed.Contains(dataCol))
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

    private NGTreeNode CreatePlotItemNode(IGPlotItem plotItem)
    {
      if (null == plotItem)
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
      if (null != newItem)
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
      if (null != node)
      {
        _plotItemsTree.Add(node);
        _doc.Add((IGPlotItem)node.Tag);
      }
    }

    public void PlotItems_MoveUpSelected()
    {
      var selNodes = PlotItemsSelected;
      if (selNodes.Length != 0)
      {
        // move the selected items upwards in the list
        ContentsListBox_MoveUpDown(-1, selNodes);
      }
    }

    public void PlotItems_MoveDownSelected()
    {
      var selNodes = PlotItemsSelected;
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
    /// Group the selected nodes.
    /// </summary>
    public void PlotItems_GroupClick()
    {
      var selNodes = PlotItemsSelected;

      // retrieve the selected items
      if (selNodes.Length < 2)
        return; // we cannot group anything if no or only one item is selected

      // look, if one of the selected items is a plot group
      // if found, use this group and add the remaining items to this
      int foundindex = -1;
      for (int i = 0; i < selNodes.Length; i++)
      {
        if (selNodes[i].Tag is PlotItemCollection)
        {
          foundindex = i;
          break;
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
      var selNodes = PlotItemsSelected;

      // retrieve the selected items
      if (selNodes.Length < 1)
        return; // we cannot ungroup anything if nothing selected

      selNodes = NGTreeNode.FilterIndependentNodes(selNodes);

      for (int i = 0; i < selNodes.Length; i++)
      {
        var selNode = selNodes[i];

        if (selNode.Nodes.Count == 0 && selNode.ParentNode != null && selNode.ParentNode.ParentNode != null)
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
        else if (selNode.Nodes.Count > 0 && selNode.ParentNode != null)
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

    public void EhView_ContentsDoubleClick(NGTreeNode selNode)
    {
      var pi = selNode.Tag as IGPlotItem;
      if (null != pi)
      {
        if (pi is PlotItemCollection)
        {
          // show not the dialog for PlotItemCollection, but only those for the group styles into that collection
          throw new NotImplementedException();
          //Current.Gui.ShowDialog(new object[] { ((PlotItemCollection)pi).GroupStyles }, pi.Name);
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

      if (selNodes.Length == 0)
        return;
      if (!GetMinimumMaximumPlotRange(selNodes, out var minRange, out var maxRange))
        return;
      var range = new Altaxo.Collections.ContiguousNonNegativeIntegerRange(minRange, maxRange - minRange);

      if (!Current.Gui.ShowDialog(ref range, "Edit plot range for selected items", false))
        return;

      foreach (NGTreeNode node in selNodes)
      {
        var pi = node.Tag as XYZColumnPlotItem;
        if (pi == null)
          continue;

        pi.Data.DataRowSelection = Altaxo.Data.Selections.RangeOfRowIndices.FromStartAndCount(range.Start, range.Count);
      }
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

    private bool GetMinimumMaximumPlotRange(IEnumerable<NGTreeNode> selNodes, out int minInclusive, out int maxExclusive)
    {
      minInclusive = 0;
      maxExclusive = int.MaxValue;
      bool result = false;

      foreach (NGTreeNode node in selNodes)
      {
        var pi = node.Tag as XYZColumnPlotItem;
        if (pi == null)
          continue;

        if (pi.Data.DataRowSelection.GetSelectedRowIndexSegmentsFromTo(minInclusive, maxExclusive, pi.Data.DataTable?.DataColumns, pi.Data.GetMaximumRowIndexFromDataColumns()).TryGetFirstAndLast(out var firstSeg, out var lastSeg))
        {
          minInclusive = Math.Max(minInclusive, firstSeg.start);
          maxExclusive = Math.Min(maxExclusive, lastSeg.endExclusive);
        }
        result = true;
      }
      return result;
    }

    public void PlotItem_Open()
    {
      var selNodes = PlotItemsSelected;

      if (selNodes.Length == 1)
        EhView_ContentsDoubleClick(selNodes[0]);
    }

    public bool PlotItems_CanDelete()
    {
      var anySelected = null != _plotItemsRootNode.TakeFromHereToFirstLeaves(false).Where(node => node.IsSelected).FirstOrDefault();
      return anySelected;
    }

    public void PlotItems_Delete()
    {
      var selNodes = PlotItemsSelected;
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

    public void PlotItems_Copy()
    {
      var selNodes = NGTreeNode.FilterIndependentNodes(PlotItemsSelected);

      PlotItemCollection coll = PutSelectedPlotItemsToTemporaryDocumentForClipboard(selNodes);

      ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml", coll);
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
      return null != coll;
    }

    public void PlotItems_Paste()
    {
      object o = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Gdi.Plot.PlotItemCollection.AsXml");
      var coll = o as PlotItemCollection;
      // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
      if (null != coll)
      {
        foreach (IGPlotItem item in coll) // it is neccessary to add the items to the doc first, because otherwise they don't have names
        {
          var clonedItem = (IGPlotItem)item.Clone();
          _doc.Add(clonedItem); // cloning neccessary because coll will be disposed afterwards, which would destroy all items
          var newNode = new NGTreeNode();
          PlotItemsToTree(newNode, clonedItem);
          _plotItemsTree.Add(newNode);
        }
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    #endregion ILineScatterLayerContentsController Members

    #region Drag/drop support

    #region Plot items

    public bool PlotItems_CanStartDrag(IEnumerable items)
    {
      return NGTreeNode.AreAllNodesFromSameLevel(items.OfType<NGTreeNode>());
    }

    public void PlotItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
    {
      data = new List<NGTreeNode>(items.OfType<NGTreeNode>());
      canCopy = true;
      canMove = true;
    }

    public void PlotItems_DragEnded(bool isCopy, bool isMove)
    {
    }

    public void PlotItems_DragCancelled()
    {
    }

    public void PlotItems_DropCanAcceptData(object data, NGTreeNode targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
    {
      var nodes = data as IEnumerable<NGTreeNode>;
      if (null == nodes)
      {
        canCopy = false;
        canMove = false;
        itemIsSwallowingData = false;
        return;
      }

      if (targetItem != null && targetItem.Tag is PlotItemCollection)
      {
        foreach (var node in nodes)
        {
          if (object.ReferenceEquals(targetItem, node)) // target item and node should not be identical
          {
            canCopy = false;
            canMove = false;
            itemIsSwallowingData = true;
            return;
          }
        }

        canCopy = true;
        canMove = true;
        itemIsSwallowingData = true;
      }
      else
      {
        canCopy = true;
        canMove = true;
        itemIsSwallowingData = false;
      }
    }

    public void PlotItems_Drop(object data, NGTreeNode targetNode, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
    {
      isMove = false;
      isCopy = false;

      bool canTargetSwallowNodes = null != targetNode && targetNode.Tag is PlotItemCollection;

      Action<NGTreeNode> AddNodeToTree;

      int actualInsertIndex; // is updated every time the following delegate is called
      NGTreeNodeCollection parentNodeCollectionOfTargetNode = null;

      if (canTargetSwallowNodes) // Target is plot item collectio node -> we can simply add the data to it
      {
        AddNodeToTree = node => { targetNode.Nodes.Add(node); ((PlotItemCollection)targetNode.Tag).Add((IGPlotItem)node.Tag); };
      }
      else if (targetNode == null) // no target node -> add data to the end of the colleciton
      {
        AddNodeToTree = node => { _plotItemsRootNode.Nodes.Add(node); ((PlotItemCollection)_plotItemsRootNode.Tag).Add((IGPlotItem)node.Tag); };
      }
      else // target node is plot item only --> Add as sibling of the target node
      {
        int idx = targetNode.Index;
        if (idx < 0) // target node has no parent node -> should not happen
        {
          throw new InvalidProgramException("Please report this exception to the forum");
          // AddNodeToTree = node => { targetNode.Nodes.Add(node); ((PlotItemCollection)targetNode.Tag).Add((IGPlotItem)node.Tag); };
        }
        else
        {
          if (insertPosition.HasFlag(Gui.Common.DragDropRelativeInsertPosition.AfterTargetItem))
            idx = targetNode.Index + 1;

          actualInsertIndex = idx;
          parentNodeCollectionOfTargetNode = targetNode.ParentNode.Nodes;
          AddNodeToTree = node =>
          {
            parentNodeCollectionOfTargetNode.Insert(actualInsertIndex, node); // the incrementation is to support dropping of multiple items, they must be dropped at increasing indices
            ((ITreeListNode<IGPlotItem>)targetNode.ParentNode.Tag).ChildNodes.Insert(actualInsertIndex, (IGPlotItem)node.Tag);
            ((IGPlotItem)node.Tag).ParentObject = (Altaxo.Main.IDocumentNode)(targetNode.ParentNode.Tag); // fix parent child relation

            ++actualInsertIndex;
          };
        }
      }

      if (data is IEnumerable<NGTreeNode>)
      {
        var dummyNodes = new List<NGTreeNode>();
        foreach (var node in (IEnumerable<NGTreeNode>)data)
        {
          if (node.Tag is Altaxo.Data.DataColumn)
          {
            isMove = false;
            isCopy = true;

            var newNode = CreatePlotItemNode((Altaxo.Data.DataColumn)node.Tag);
            AddNodeToTree(newNode);
          }
          else if (node.Tag is IGPlotItem)
          {
            isMove = true;
            isCopy = false;

            var index = node.Index;
            var parentNode = node.ParentNode;

            var dummyItem = new DummyPlotItem() { ParentObject = (PlotItemCollection)parentNode.Tag };
            var dummyNode = new NGTreeNode() { Tag = dummyItem };

            parentNode.Nodes[index] = dummyNode; // instead of removing the old node from the tree, we replace it by a dummy. In this way we retain the position of all nodes in the tree, so that the insert index is valid during the whole drop operation
            ((ITreeListNode<IGPlotItem>)parentNode.Tag).ChildNodes[index] = dummyItem;

            AddNodeToTree(node);

            dummyNodes.Add(dummyNode); // for deletion of dummy nodes afterwards
          }
        }
        // now, after the drop is complete, we can remove the dummy nodes
        foreach (var dummy in dummyNodes)
        {
          ((IGPlotItem)dummy.Tag).Remove();
          dummy.Remove();
        }
      }

#if VERIFY_TREESYNCHRONIZATION
      if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_plotItemsRootNode, _doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
        throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
    }

    #endregion Plot items

    #region Available items

    public bool AvailableItems_CanStartDrag(IEnumerable items)
    {
      var selNodes = items.OfType<NGTreeNode>();
      var selNotAllowedNodes = selNodes.Where(node => !(node.Tag is Altaxo.Data.DataColumn));

      var isAnythingSelected = selNodes.FirstOrDefault() != null;
      var isAnythingForbiddenSelected = selNotAllowedNodes.FirstOrDefault() != null;

      // to start a drag, all selected nodes must be on the same level
      return isAnythingSelected && !isAnythingForbiddenSelected;
    }

    public void AvailableItems_StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
    {
      data = new List<NGTreeNode>(items.OfType<NGTreeNode>().Where(node => (node.IsSelected && node.Tag is Altaxo.Data.DataColumn)));
      canCopy = true;
      canMove = false;
    }

    public void AvailableItems_DragEnded(bool isCopy, bool isMove)
    {
    }

    public void AvailableItems_DragCancelled()
    {
    }

    #endregion Available items

    #endregion Drag/drop support

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

      public void PaintPreprocessing(Altaxo.Graph.IPaintContext context)
      {
        throw new NotImplementedException();
      }

      public void Paint(Graphics g, Altaxo.Graph.IPaintContext context, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem)
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

      public IHitTestObject HitTest(IPlotArea layer, HitTestPointData hitpoint)
      {
        throw new NotImplementedException();
      }

      public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
      {
        throw new NotImplementedException();
      }

      public bool CopyFrom(object obj)
      {
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

      public void CollectStyles(PlotGroupStyleCollection styles)
      {
        throw new NotImplementedException();
      }

      public void PrepareGroupStyles(PlotGroupStyleCollection styles, IPlotArea layer)
      {
        throw new NotImplementedException();
      }

      public void ApplyGroupStyles(PlotGroupStyleCollection styles)
      {
        throw new NotImplementedException();
      }

      public void Paint(IGraphicsContext3D g, IPaintContext context, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem)
      {
        throw new NotImplementedException();
      }

      public void PaintSymbol(IGraphicsContext3D g, RectangleD3D location)
      {
        throw new NotImplementedException();
      }

      public IHitTestObject HitTest(IPlotArea layer, Ray3D hitpoint)
      {
        throw new NotImplementedException();
      }

      public IDocumentLeafNode DataObject { get { return null; } }

      public IDocumentLeafNode StyleObject { get { return null; } }
    }

    #endregion Inner classes
  }
}
