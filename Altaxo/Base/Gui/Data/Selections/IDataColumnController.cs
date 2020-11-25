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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Gui.Data.Selections
{
  /// <summary>
  /// Signifies that this controller is controlling a row selection with one data column.
  /// </summary>
  public interface IDataColumnController
  {
    /// <summary>
    /// Sets the index of this controller. Can be used in the view to show the index of the data column. The same index is shown in the
    /// data column view again. In this way the column in the row selection can be matched with the column in the data column view.
    /// </summary>
    /// <param name="idx">The index.</param>
    void SetIndex(int idx);

    void SetDataColumn(IReadableColumn column, DataTable supposedParentDataTable, int supposedGroupNumber);

    IReadableColumn Column { get; }

    string ColumnName { get; }
  }
}
