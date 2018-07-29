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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Determines how the plot color influences the colors of a scatter symbol.
  /// </summary>
  [Flags]
  public enum PlotColorInfluence
  {
    /// <summary>
    /// The plot color has no influence on any of the colors in the scatter symbol.
    /// </summary>
    None = 0,

    /// <summary>
    /// The fill color is controlled by the plot color, but the original alpha value of the fill color is preserved.
    /// </summary>
    FillColorPreserveAlpha = 1,

    /// <summary>
    /// The fill color is fully controlled by the plot color.
    /// </summary>
    FillColorFull = 2,

    /// <summary>
    /// The frame color is controlled by the plot color, but the original alpha value of the frame color is preserved.
    /// </summary>
    FrameColorPreserveAlpha = 4,

    /// <summary>
    /// The frame color is controlled by the plot color.
    /// </summary>
    FrameColorFull = 8,

    /// <summary>
    /// The inset color is controlled by the plot color, but the original alpha value of the inset color is preserved.
    /// </summary>
    InsetColorPreserveAlpha = 16,

    /// <summary>
    /// The inset color is controlled by the plot color.
    /// </summary>
    InsetColorFull = 32,
  }
}
