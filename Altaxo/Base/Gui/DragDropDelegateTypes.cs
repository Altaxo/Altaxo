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
	public delegate bool CanStartDragDelegate(IEnumerable items);

	public struct StartDragData
	{
		public object Data;
		public bool CanCopy;
		public bool CanMove;
	}

	public delegate StartDragData StartDragDelegate(IEnumerable items);

	public delegate void DragEndedDelegate(bool isCopy, bool isMove);

	public delegate void DragCancelledDelegate();

	public struct DropCanAcceptDataReturnData
	{
		public bool CanCopy;
		public bool CanMove;
		public bool ItemIsSwallowingData;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="data">The data to accept.</param>
	/// <param name="nonGuiTargetItem">Object that can identify the drop target, for instance a non gui tree node or list node, or a tag.</param>
	/// <param name="insertPosition">The insert position.</param>
	/// <param name="isCtrlKeyPressed">if set to <c>true</c> [is control key pressed].</param>
	/// <param name="isShiftKeyPressed">if set to <c>true</c> [is shift key pressed].</param>
	/// <returns></returns>
	public delegate DropCanAcceptDataReturnData DropCanAcceptDataDelegate(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed);

	public struct DropReturnData
	{
		public bool IsCopy;
		public bool IsMove;
	}

	public delegate DropReturnData DropDelegate(object data, object nonGuiTargetItem, Gui.Common.DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed);
}