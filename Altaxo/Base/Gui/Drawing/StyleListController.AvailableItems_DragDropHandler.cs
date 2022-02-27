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
  public partial class StyleListController<TManager, TList, TItem> where TItem : Altaxo.Main.IImmutable
where TList : IStyleList<TItem>
where TManager : IStyleListManager<TList, TItem>
  {
    public class AvailableItems_DragDropHandler : IMVVMDragDropHandler
    {
      StyleListController<TManager, TList, TItem> _parent;

      public AvailableItems_DragDropHandler(StyleListController<TManager, TList, TItem> parent)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
      }

      #region Drag Available items

      public void DragCancelled()
      {
      }

      
      public void DragEnded(bool isCopy, bool isMove)
      {
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      { 
        var node = items.OfType<SelectableListNode>().FirstOrDefault();


        data = node.Tag;
        canCopy = true;
        canMove = false;
        
      }

      public bool CanStartDrag(IEnumerable items)
      {
        var selNode = items.OfType<SelectableListNode>().FirstOrDefault();
        // to start a drag, at least one item must be selected
        return selNode is not null;
      }

      #endregion Drag Available items

      #region Drop onto available items

      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        // when dropping onto available items, it's only purpose is to remove some items from the current item lists
        // thus the only operation here is move

        canCopy = false;
        canMove = true; // we want the item to be removed from the current item list
        itemIsSwallowingData = false;
      }

      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        // when dropping onto available items, it's only purpose is to remove some items from the item lists
        // thus the only operation here is move

        if (data is TItem)
        {
          isCopy = false;
          isMove = true; // we want the item to be removed from the current item list
        }
        else
        {
          isCopy = false;
          isMove = false;
        }
      }
    
      #endregion Drop onto available items
    }
  }
}
