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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Graph3D
{
  public partial class XYZPlotLayerContentsController
  {
    public class PlotItems_DragDropHandler : IMVVMDragDropHandler
    {
      XYZPlotLayerContentsController _parent;

      public PlotItems_DragDropHandler(XYZPlotLayerContentsController parent)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
      }

      public bool CanStartDrag(IEnumerable items)
      {
        return NGTreeNode.AreAllNodesFromSameLevel(items.OfType<NGTreeNode>());
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        data = new List<NGTreeNode>(items.OfType<NGTreeNode>());
        canCopy = true;
        canMove = true;
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
      }

      public void DragCancelled()
      {
      }

      public void DropCanAcceptData(object data, object targetObject, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        if (data is not IEnumerable<NGTreeNode> nodes)
        {
          canCopy = false;
          canMove = false;
          itemIsSwallowingData = false;
          return;
        }

        var targetNode = (targetObject as NGTreeNode) ?? _parent._plotItemsRootNode;

        if (targetNode.Tag is PlotItemCollection)
        {
          foreach (var node in nodes)
          {
            if (object.ReferenceEquals(targetNode, node)) // target item and node should not be identical
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

      public void Drop(object data, object targetObject, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isMove = false;
        isCopy = false;

        var targetNode = (targetObject as NGTreeNode) ?? _parent._plotItemsRootNode;

        bool canTargetSwallowNodes = targetNode is not null && targetNode.Tag is PlotItemCollection;

        Action<NGTreeNode> AddNodeToTree;

        int actualInsertIndex; // is updated every time the following delegate is called
        NGTreeNodeCollection parentNodeCollectionOfTargetNode = null;

        if (canTargetSwallowNodes) // Target is plot item collectio node -> we can simply add the data to it
        {
          AddNodeToTree = node => { targetNode.Nodes.Add(node); ((PlotItemCollection)targetNode.Tag).Add((IGPlotItem)node.Tag); };
        }
        else if (targetNode is null) // no target node -> add data to the end of the colleciton
        {
          AddNodeToTree = node => { _parent._plotItemsRootNode.Nodes.Add(node); ((PlotItemCollection)_parent._plotItemsRootNode.Tag).Add((IGPlotItem)node.Tag); };
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

              var newNode = _parent.CreatePlotItemNode((Altaxo.Data.DataColumn)node.Tag);
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
        if (!TreeNodeExtensions.IsStructuralEquivalentTo<NGTreeNode, IGPlotItem>(_parent._plotItemsRootNode, _parent._doc, (x, y) => object.ReferenceEquals(x.Tag, y)))
          throw new InvalidProgramException("Trees of plot items and model nodes are not structural equivalent");
#endif
      }
    }
  }
}
