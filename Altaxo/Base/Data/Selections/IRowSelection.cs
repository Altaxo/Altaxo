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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data.Selections
{
	public interface IRowSelection : Main.IDocumentLeafNode, ICloneable
	{
		/// <summary>
		/// Gets the selected row indices continuously, beginning with no less than the start index and less than the maximum index.
		/// </summary>
		/// <param name="startIndex">The start index. Each row index that is returned has to be equal to or greater than this value.</param>
		/// <param name="maxIndex">The maximum index.  Each row index that is returned has to be less than this value.</param>
		/// <returns>The selected row indices, beginning with no less than the start index and less than the maximum index.</returns>
		IEnumerable<int> GetSelectedRowIndicesFromTo(int startIndex, int maxIndex);
	}

	public interface IRowSelectionCollection : IEnumerable<IRowSelection>, IRowSelection
	{
		IRowSelectionCollection WithAdditionalItem(IRowSelection item);

		IRowSelectionCollection WithChangedItem(int idx, IRowSelection item);

		IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items);
	}
}