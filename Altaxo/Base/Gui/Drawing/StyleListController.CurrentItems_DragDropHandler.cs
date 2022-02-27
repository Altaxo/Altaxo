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
using System.Linq;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Drawing
{
  public partial class StyleListController<TManager, TList, TItem> 
    where TItem : Altaxo.Main.IImmutable
    where TList : IStyleList<TItem>
    where TManager : IStyleListManager<TList, TItem>
  {
    public class CurrentItems_DragDropHandler : IMVVMDragDropHandler
    {
      StyleListController<TManager, TList, TItem> _parent;

      public CurrentItems_DragDropHandler(StyleListController<TManager, TList, TItem> parent)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
      }

      #region Drag current items

      public void DragCancelled()
      {
        _draggedNode = null;
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
        if (isMove && _draggedNode is not null)
        {
          _parent.CurrentItems.Remove(_draggedNode);
          _parent.SetListDirty();
        }

        _draggedNode = null;
      }

      private SelectableListNode _draggedNode;

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
       
        _draggedNode = items.OfType<SelectableListNode>().FirstOrDefault();


        data = _draggedNode.Tag;
        canCopy = true;
        canMove = true;
        
      }

      public bool CanStartDrag(IEnumerable items)
      {
        var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
        // to start a drag, at least one item must be selected
        return selNode is not null;
      }

      #endregion Drag current items

      #region Drop onto current items

      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
       
        // investigate data

        if (data is TItem)
        {

          canCopy = true;
          canMove = false;
          itemIsSwallowingData = false;
        }
        else if (data is Type)
        {
          canCopy = true;
            canMove = false;
            itemIsSwallowingData = false;
        }
        else
        {

          canCopy = false;
            canMove = false;
            itemIsSwallowingData = false;
          
        }
      }

      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
       
        var droppedItem = default(TItem);
        if (data is Type)
        {
          object createdObj = null;
          try
          {
            createdObj = System.Activator.CreateInstance((Type)data);
          }
          catch (Exception ex)
          {
            Current.Gui.ErrorMessageBox("This object could not be dropped because it could not be created, message: " + ex.ToString(), "Error");
            isCopy = false;
            isMove = false;
          }

          if (!(createdObj is TItem))
          {
            isCopy = false;
            isMove = false;
          }

          droppedItem = (TItem)createdObj;
        } // end if data is type
        else if (data is TItem)
        {
          droppedItem = (TItem)data;
        } // end if data is TItem
        else
        {
          isCopy = false;
          isMove = false;
        }

        int targetIndex = int.MaxValue;
        if (targetItem is SelectableListNode)
        {
          int idx = _parent.CurrentItems.IndexOf((SelectableListNode)targetItem);
          if (idx >= 0 && insertPosition.HasFlag(DragDropRelativeInsertPosition.AfterTargetItem))
            ++idx;
          targetIndex = idx;
        }

        var newNode = new SelectableListNode(droppedItem.ToString(), droppedItem, false);
        if (targetIndex >= _parent.CurrentItems.Count)
          _parent.CurrentItems.Add(newNode);
        else
          _parent.CurrentItems.Insert(targetIndex, newNode);

        _parent.SetListDirty();


        isCopy = isCtrlKeyPressed;
        isMove = !isCtrlKeyPressed;
      }

     

      

     

      #endregion Drop onto current items
    }
  }
}
