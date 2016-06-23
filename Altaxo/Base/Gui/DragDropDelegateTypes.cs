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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Is called at the start of a drag/drop operation on the drag source side. Determines whether or not a drag operation can start.
	/// </summary>
	/// <param name="items">The items that are included in this drag operation.</param>
	/// <returns>True if the drag operation can start and proceed, otherwise false.</returns>
	public delegate bool CanStartDragDelegate(IEnumerable items);

	/// <summary>
	/// Return data for the <see cref="StartDragDelegate"/> delegate.
	/// </summary>
	public struct StartDragData
	{
		/// <summary>The data that are dragged, packed in a serializable format.</summary>
		public object Data;

		/// <summary>True if the drag/drop operation can be a copy operation.</summary>
		public bool CanCopy;

		/// <summary>True if the drag/drop operation can be a move operation.</summary>
		public bool CanMove;
	}

	/// <summary>
	/// Executes the drag operation.
	/// </summary>
	/// <param name="items">The items that are included in this drag operation.</param>
	/// <returns>The data of this drag operation. See <see cref="StartDragData"/>.</returns>
	public delegate StartDragData StartDragDelegate(IEnumerable items);

	/// <summary>
	/// Indicates the successful end of a drag operation.
	/// </summary>
	/// <param name="isCopy">Is set to <c>true</c> if the drag/drop operation was a copy operation.</param>
	/// <param name="isMove">Is set to <c>true</c> if the drag/drop operation was a move operation (in this case e.g. the source item should be removed).</param>
	public delegate void DragEndedDelegate(bool isCopy, bool isMove);

	/// <summary>Called when the drag operation was cancelled. Can be used to return the state of your items to the state before the start of the drag operation.</summary>
	public delegate void DragCancelledDelegate();

	/// <summary>
	/// Return data for the <see cref="DropCanAcceptDataDelegate"/> delegate.
	/// </summary>
	public struct DropCanAcceptDataReturnData
	{
		/// <summary>Is set to true if the drag/drop operation can be a copy operation.</summary>
		public bool CanCopy;

		/// <summary>Is set to true if the drag/drop operation can be a move operation.</summary>
		public bool CanMove;

		/// <summary>
		/// Is set to true if the item is swallowing the data, i.e. the data must be removed on the source side.
		/// </summary>
		public bool ItemIsSwallowingData;
	}

	/// <summary>
	/// Tests if a drop target can accept the data that are dragged.
	/// </summary>
	/// <param name="data">The data to accept.</param>
	/// <param name="nonGuiTargetItem">Object that can identify the drop target, for instance a non gui tree node or a list node, or a tag of a gui item.</param>
	/// <param name="insertPosition">The insert position. Applies for lists and trees only.</param>
	/// <param name="isCtrlKeyPressed">If set to <c>true</c>, the control key is pressed.</param>
	/// <param name="isShiftKeyPressed">If set to <c>true</c>, the shift key is pressed.</param>
	/// <returns>Data that indicate if the target can accept the data. In this case, at least one of <see cref="DropCanAcceptDataReturnData.CanCopy"/> or <see cref="DropCanAcceptDataReturnData.CanMove"/> have to be set to true.</returns>
	public delegate DropCanAcceptDataReturnData DropCanAcceptDataDelegate(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed);

	/// <summary>
	/// Return data of the <see cref="DropDelegate"/> delegate.
	/// </summary>
	public struct DropReturnData
	{
		/// <summary>
		/// If true, the drop operation was a copy operation.
		/// </summary>
		public bool IsCopy;

		/// <summary>
		/// If true, the drop operation was a move operation.
		/// </summary>
		public bool IsMove;
	}

	/// <summary>
	/// Executes the drop operation.
	/// </summary>
	/// <param name="data">The data to accept.</param>
	/// <param name="nonGuiTargetItem">The target item of this drop operation. Can be either a non gui list or tree node, or a tag of a Gui item.</param>
	/// <param name="insertPosition">The insert position (applies for lists and trees only).</param>
	/// <param name="isCtrlKeyPressed">If set to <c>true</c>, the control key is pressed.</param>
	/// <param name="isShiftKeyPressed">If set to <c>true</c>, the shift key is pressed.</param>
	/// <returns>Data that indicate whether the drop operation was successful, and whether the items were copyied or moved. See <see cref="DropReturnData"/>.</returns>
	public delegate DropReturnData DropDelegate(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed);
}