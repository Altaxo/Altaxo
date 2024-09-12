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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Data
{
  public partial class ListOfXAndYColumnController
  {
    public IMVVMDragHandler AvailableItemsDragHandler { get; }

    public class AvailableItems_DragHandler : IMVVMDragHandler
    {
      private ListOfXAndYColumnController _parent;

      public AvailableItems_DragHandler(ListOfXAndYColumnController parent)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
      }
      public bool CanStartDrag(IEnumerable items)
      {
        var selNodes = items.OfType<NGTreeNode>();
        var selNotAllowedNodes = selNodes.Where(node => !(node.Tag is Altaxo.Data.DataColumn));

        var isAnythingSelected = selNodes.FirstOrDefault() is not null;
        var isAnythingForbiddenSelected = selNotAllowedNodes.FirstOrDefault() is not null;

        // to start a drag, all selected nodes must be on the same level
        return isAnythingSelected && !isAnythingForbiddenSelected;
      }

      public void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove)
      {
        data = new List<NGTreeNode>(items.OfType<NGTreeNode>().Where(node => (node.IsSelected && node.Tag is Altaxo.Data.DataColumn)));
        canCopy = true;
        canMove = false;
      }

      public void DragEnded(bool isCopy, bool isMove)
      {
      }

      public void DragCancelled()
      {
      }
    }

  }
}
