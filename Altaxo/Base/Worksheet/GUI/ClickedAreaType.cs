#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion
using System;

namespace Altaxo.Worksheet.GUI
{
  

  /// <summary>The type of area we have clicked into, used by ClickedCellInfo.</summary>
  public enum ClickedAreaType 
  { 
    /// <summary>Outside of all relevant areas.</summary>
    OutsideAll,
    /// <summary>On the table header (top left corner of the data grid).</summary>
    TableHeader,
    /// <summary>Inside a data cell.</summary>
    DataCell,
    /// <summary>Inside a property cell.</summary>
    PropertyCell,
    /// <summary>On the column header.</summary>
    DataColumnHeader,
    /// <summary>On the row header.</summary>
    DataRowHeader,
    /// <summary>On the property column header.</summary>
    PropertyColumnHeader
  }
}
