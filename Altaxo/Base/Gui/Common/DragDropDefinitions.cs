#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Designates the position relative to the target item in a drop operation.
  /// </summary>
  [Flags]
  public enum DragDropRelativeInsertPosition
  {
    /// <summary>
    /// The drop should be inserted before the target item.
    /// </summary>
    BeforeTargetItem = 1,

    /// <summary>
    /// The drop should be inserted just after the target item.
    /// </summary>
    AfterTargetItem = 2,

    /// <summary>
    /// The drop should be inserted in the target item.
    /// </summary>
    TargetItemCenter = 4
  }

  public interface IMVVMDragHandler
  {
    bool CanStartDrag(IEnumerable items);

    void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

    void DragEnded(bool isCopy, bool isMove);

    void DragCancelled();
  }

  public interface IMVVMDropHandler
  {
    /// <summary>
    /// Evaluate of a drop operation can accept the data.
    /// </summary>
    /// <param name="data">The data to drop.</param>
    /// <param name="targetItem">The target item. This is the MVVM item that corresponds to the item in the Gui.</param>
    /// <param name="insertPosition">The insert position.</param>
    /// <param name="isCtrlKeyPressed">if set to <c>true</c> [is control key pressed].</param>
    /// <param name="isShiftKeyPressed">if set to <c>true</c> [is shift key pressed].</param>
    /// <param name="canCopy">if set to <c>true</c> [can copy].</param>
    /// <param name="canMove">if set to <c>true</c> [can move].</param>
    /// <param name="itemIsSwallowingData">if set to <c>true</c> [item is swallowing data].</param>
    void DropCanAcceptData(object data, object targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData);

    /// <summary>
    /// Drops the specified data.
    /// </summary>
    /// <param name="data">The data to drop.</param>
    /// <param name="targetItem">The target item. This is the MVVM item that corresponds to the item in the Gui.</param>
    /// <param name="insertPosition">The insert position.</param>
    /// <param name="isCtrlKeyPressed">if set to <c>true</c> [is control key pressed].</param>
    /// <param name="isShiftKeyPressed">if set to <c>true</c> [is shift key pressed].</param>
    /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
    /// <param name="isMove">if set to <c>true</c> [is move].</param>
    void Drop(object data, object targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove);

  }

  public interface IMVVMDragDropHandler : IMVVMDragHandler, IMVVMDropHandler
  {

  }
}
