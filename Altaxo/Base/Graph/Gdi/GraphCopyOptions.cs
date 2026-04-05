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

#nullable enable
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
    /// <summary>
    /// No options are selected.
    /// </summary>
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
    /// <summary>
    /// The size and position of the layers will be copied.
    /// </summary>
    CopyLayerSizePosition = 0x20,

    /// <summary>
    /// The layer background will be copied.
    /// </summary>
    CopyLayerBackground = 0x40,

    /// <summary>
    /// The layer grid will be copied.
    /// </summary>
    CopyLayerGrid = 0x80,

    /// <summary>
    /// The layer scales will be copied.
    /// </summary>
    CopyLayerScales = 0x100,

    /// <summary>
    /// The layer axes will be copied.
    /// </summary>
    CopyLayerAxes = 0x200,

    /// <summary>
    /// The layer axis labels will be copied.
    /// </summary>
    CopyLayerAxesLabels = 0x400,

    /// <summary>
    /// The layer legends will be copied.
    /// </summary>
    CopyLayerLegends = 0x800,

    /// <summary>
    /// The graph items of the layer will be copied.
    /// </summary>
    CopyLayerGraphItems = 0x1000,

    /// <summary>Plotitems inclusive all styles will be cloned.</summary>
    CopyLayerPlotItems = 0x2000,

    /// <summary>Only the plot styles, but not the plot items, will be copied</summary>
    CopyLayerPlotStyles = 0x4000,

    /// <summary>
    /// The links of the layer will be copied.
    /// </summary>
    CopyLayerLinks = 0x8000,

    /// <summary>The layers will be copied from the other layers.</summary>
    CopyLayerAll = 0xFFE0, // all layer copy options into one variable

    /// <summary>
    /// All copy options are selected.
    /// </summary>
    All = -1
  }
}
