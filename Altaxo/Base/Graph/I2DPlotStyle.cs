#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Collections;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
  public interface I2DPlotStyle : Main.IChangedEventSource, System.ICloneable
  {
    /// <summary>
    /// Returns true if the color property get is supported, i.e. the style provides a color.
    /// </summary>
    bool IsColorProvider { get; }

    /// <summary>
    /// Returns true if the color of this plot style can be set.
    /// </summary>
    bool IsColorReceiver { get; }

    /// <summary>
    /// Returns the color of the style. If not supported, returns Color.Black.
    /// Sets the color. If <see>IsColorReceiver</see> is false, this should throw an exception
    /// </summary>
    System.Drawing.Color Color { get; set; }


    /// <summary>
    /// Returns true if this style provides the symbol size.
    /// </summary>
    bool IsSymbolSizeProvider { get; }

    /// <summary>
    ///  Returns true if for this style the <see>SymbolSize</see> property can be set.
    /// </summary>
    bool IsSymbolSizeReceiver { get; }

    /// <summary>
    /// Get / sets the symbol size.
    /// </summary>
    float SymbolSize { get; set; }


    void Paint(Graphics g, IPlotArea layer, PlotRangeList rangeList, PointF[] ptArray);
  }


 

}
