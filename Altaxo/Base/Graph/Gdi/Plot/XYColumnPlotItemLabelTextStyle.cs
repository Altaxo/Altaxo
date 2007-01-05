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

namespace Altaxo.Graph.Gdi.Plot
{
  /// <summary>
  /// Enumerates the style how a <see cref="XYColumnPlotItem" /> is labeled into the <see cref="Altaxo.Graph.Gdi.Shapes.TextGraphic" />. 
  /// </summary>
  public enum XYColumnPlotItemLabelTextStyle
  {
    /// <summary>Y column name is shown.</summary>
    YS = 0x10,
    /// <summary>Y column name and table name is shown.</summary>
    YM = 0x20,
    /// <summary>Y column name, collection name and table name is shown.</summary>
    YL = 0x30,
    /// <summary>X column name is shown.</summary>
    XS = 0x01,
    /// <summary>X column name and Y column name is shown.</summary>
    XSYS=0x11,
    /// <summary>X column name and Y column name and table name is shown.</summary>
    XSYM=0x21,
    /// <summary>X column name and Y column name, collection name and table name is shown.</summary>
    XSYL=0x31,
    /// <summary>X column name and table name is shown.</summary>
    XM=0x02,
    /// <summary>X column name and table name and Y column name is shown.</summary>
    XMYS=0x12,
    /// <summary>X column name and table name and Y column name and table name is shown.</summary>
    XMYM=0x22,
    /// <summary>X column name and table name and Y column name, collection name and table name is shown.</summary>
    XMYL=0x32,
    /// <summary>X column name, collection name and table name is shown.</summary>
    XL = 0x03,
    /// <summary>X column name, collection name and table name and Y column name is shown.</summary>
    XLXS=0x13,
    /// <summary>X column name, collection name and table name and Y column name and table name is shown.</summary>
    XLYM=0x23,
    /// <summary>X column name, collection name and table name and Y column name, collection name and table name is shown.</summary>
    XLYL=0x33
  }

}
