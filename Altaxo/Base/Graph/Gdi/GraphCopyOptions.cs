#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Options to copy styles or items from one graph document to the other.
  /// </summary>
  [Flags]
  public enum GraphCopyOptions
  {
    None = 0x00,

    /// <summary>The notes will be copied.</summary>
    CloneNotes = 0x01,

    /// <summary>The graph document properties will be copied.</summary>
    CloneProperties = 0x02,

    /// <summary>The page properties (format and printable size) will be copied.</summary>
    CopySize = 0x04,

    /// <summary>The graph's design size will be copied.</summary>
    CopyChildLayers = 0x08,

    // now layer properties
    CopyLayerSizePosition = 0x20,

    CopyLayerBackground = 0x40,

    CopyLayerGrid = 0x80,

    CopyLayerScales = 0x100,

    CopyLayerAxes = 0x200,

    CopyLayerAxesLabels = 0x400,

    CopyLayerLegends = 0x800,

    CopyLayerGraphItems = 0x1000,

    /// <summary>Plotitems inclusive all styles will be cloned.</summary>
    CopyLayerPlotItems = 0x2000,

    /// <summary>Only the plot styles, but not the plot items, will be copied</summary>
    CopyLayerPlotStyles = 0x4000,

    CopyLayerLinks = 0x8000,

    /// <summary>The layers will be copied from the other layers.</summary>
    CopyLayerAll = 0xFFE0, // all layer copy options into one variable

    All = -1
  }
}
