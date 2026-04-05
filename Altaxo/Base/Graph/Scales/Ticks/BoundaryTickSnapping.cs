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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Specifies how axis boundaries are snapped to tick positions.
  /// </summary>
  public enum BoundaryTickSnapping
  {
    /// <summary>
    /// Do not snap the boundary to any tick.
    /// </summary>
    [Description("${res:ClassNames.Altaxo.Graph.Scales.Ticks.BoundaryTickSnapping.SnapToNothing}")]
    SnapToNothing,

    /// <summary>
    /// Snap the boundary to the nearest minor or major tick.
    /// </summary>
    [Description("${res:ClassNames.Altaxo.Graph.Scales.Ticks.BoundaryTickSnapping.SnapToMinorOrMajor}")]
    SnapToMinorOrMajor,

    /// <summary>
    /// Snap the boundary only to major ticks.
    /// </summary>
    [Description("${res:ClassNames.Altaxo.Graph.Scales.Ticks.BoundaryTickSnapping.SnapToMajorOnly}")]
    SnapToMajorOnly,

    /// <summary>
    /// Snap the boundary only to minor ticks.
    /// </summary>
    [Description("${res:ClassNames.Altaxo.Graph.Scales.Ticks.BoundaryTickSnapping.SnapToMinorOnly}")]
    SnapToMinorOnly,
  }
}
