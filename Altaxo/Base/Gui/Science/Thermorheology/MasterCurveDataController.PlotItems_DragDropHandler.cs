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
using Altaxo.Data;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Science.Thermorheology
{
  public partial class MasterCurveDataController
  {
    public IMVVMDragDropHandler PlotItemsDragDropHandler { get; }

    public class PlotItems_DragDropHandler : IMVVMDragDropHandler
    {
      MasterCurveDataController _parent;

      public PlotItems_DragDropHandler(MasterCurveDataController parent)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
      }

      public bool CanStartDrag(IEnumerable items)
      {
        return items.OfType<SelectableListNode>().Any();
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        data = new List<SelectableListNode>(items.OfType<SelectableListNode>());
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
        if (data is IEnumerable<NGTreeNode> nodes1)
        {
          canCopy = true;
          canMove = true;
          itemIsSwallowingData = false;
        }
        if (data is IEnumerable<SelectableListNode> nodes2)
        {
          canCopy = false;
          canMove = true;
          itemIsSwallowingData = false;
        }
        else
        {
          canCopy = false;
          canMove = false;
          itemIsSwallowingData = false;
          return;
        }
      }

      public void Drop(object data, object targetObject, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isMove = false;
        isCopy = false;

        // the data came from the list of available items
        int insertPositionIndex = _parent.DataItems.Items.Count;
        // determine the insert position
        if (targetObject is MyNode myNode)
        {
          var targetIndex = _parent.DataItems.Items.IndexOfFirst((n, i) => object.ReferenceEquals(n, myNode));
          if (targetIndex != -1)
          {
            insertPositionIndex = insertPosition switch
            {
              DragDropRelativeInsertPosition.BeforeTargetItem => targetIndex,
              DragDropRelativeInsertPosition.AfterTargetItem => targetIndex + 1,
              _ => targetIndex + 1
            };
          }
        }

        if (data is IEnumerable<NGTreeNode> nodes)
        {
          var xyColumns = new List<XAndYColumn>();

          foreach (var rcol in nodes.Where(n => n.Tag is DataColumn).Select(n => (DataColumn)(n.Tag)))
          {
            var node = _parent.CreatePlotItem(rcol);
            if (node is not null)
            {
              xyColumns.Add(node);
            }
          }
          _parent.AddItemsToGuiList(xyColumns, _parent.DataGroup.SelectedValue, insertPositionIndex);
          isMove = false;
          isCopy = true;
        }
        else if (data is IEnumerable<SelectableListNode> nodes2)
        {
          var mynodes = nodes2.OfType<MyNode>().ToArray();
          var xyColumns = new List<XAndYColumn?>(mynodes.Select(x => x.Curve));
          _parent.AddItemsToGuiList(xyColumns, _parent.DataGroup.SelectedValue, insertPositionIndex);

          // now remove the nodes
          foreach (var node in mynodes)
          {
            _parent.DataItems.Items.Remove(node);
          }

          isMove = true;
          isCopy = false;
        }
      }
    }
  }
}
