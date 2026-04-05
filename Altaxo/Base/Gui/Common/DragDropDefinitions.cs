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

  /// <summary>
  /// Provides drag-handling functionality for MVVM items.
  /// </summary>
  public interface IMVVMDragHandler
  {
    /// <summary>
    /// Determines whether a drag operation can be started.
    /// </summary>
    /// <param name="items">The items to drag.</param>
    /// <returns><see langword="true"/> if dragging can start; otherwise, <see langword="false"/>.</returns>
    bool CanStartDrag(IEnumerable items);

    /// <summary>
    /// Starts a drag operation.
    /// </summary>
    /// <param name="items">The items to drag.</param>
    /// <param name="data">The drag data object.</param>
    /// <param name="canCopy">Set to <see langword="true"/> if copying is allowed.</param>
    /// <param name="canMove">Set to <see langword="true"/> if moving is allowed.</param>
    void StartDrag(IEnumerable items, out object data, out bool canCopy, out bool canMove);

    /// <summary>
    /// Notifies the handler that the drag operation ended.
    /// </summary>
    /// <param name="isCopy">Whether the operation ended as a copy.</param>
    /// <param name="isMove">Whether the operation ended as a move.</param>
    void DragEnded(bool isCopy, bool isMove);

    /// <summary>
    /// Notifies the handler that the drag operation was cancelled.
    /// </summary>
    void DragCancelled();
  }

  /// <summary>
  /// Provides drop-handling functionality for MVVM items.
  /// </summary>
  public interface IMVVMDropHandler
  {
    /// <summary>
    /// Evaluates whether a drop operation can accept the data.
    /// </summary>
    /// <param name="data">The data to drop.</param>
    /// <param name="targetItem">The target item. This is the MVVM item that corresponds to the item in the Gui.</param>
    /// <param name="insertPosition">The insert position.</param>
    /// <param name="isCtrlKeyPressed">If set to <see langword="true"/>, the Control key is pressed.</param>
    /// <param name="isShiftKeyPressed">If set to <see langword="true"/>, the Shift key is pressed.</param>
    /// <param name="canCopy">Set to <see langword="true"/> if copying is allowed.</param>
    /// <param name="canMove">Set to <see langword="true"/> if moving is allowed.</param>
    /// <param name="itemIsSwallowingData">Set to <see langword="true"/> if the target item consumes the dropped data.</param>
    void DropCanAcceptData(object data, object targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData);

    /// <summary>
    /// Drops the specified data.
    /// </summary>
    /// <param name="data">The data to drop.</param>
    /// <param name="targetItem">The target item. This is the MVVM item that corresponds to the item in the Gui.</param>
    /// <param name="insertPosition">The insert position.</param>
    /// <param name="isCtrlKeyPressed">If set to <see langword="true"/>, the Control key is pressed.</param>
    /// <param name="isShiftKeyPressed">If set to <see langword="true"/>, the Shift key is pressed.</param>
    /// <param name="isCopy">Set to <see langword="true"/> if the operation was a copy.</param>
    /// <param name="isMove">Set to <see langword="true"/> if the operation was a move.</param>
    void Drop(object data, object targetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove);

  }

  /// <summary>
  /// Combines drag and drop handling for MVVM items.
  /// </summary>
  public interface IMVVMDragDropHandler : IMVVMDragHandler, IMVVMDropHandler
  {

  }
}
