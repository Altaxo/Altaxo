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
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Altaxo.Graph
{
	/// <summary>
	/// Interface for two dimensional plot styles.
	/// </summary>
	public interface I2DPlotStyle
	{
    /// <summary>
    /// Returns true if the color property is supported.
    /// </summary>
    bool IsColorSupported { get ; }
    
    /// <summary>
    /// Returns the color of the style. If not supported, returns Color.Black.
    /// </summary>
    System.Drawing.Color Color { get; }


    /// <summary>
    /// Returns true if the line style property is supported.
    /// </summary>
    bool IsXYLineStyleSupported { get; }

    /// <summary>
    /// Returns the line style property. If not supported, returns null.
    /// </summary>
    XYPlotLineStyle XYLineStyle { get; }
     

    
    /// <summary>
    /// Returns true if the scatter style property is supported.
    /// </summary>
    bool IsXYScatterStyleSupported { get; }
    
    /// <summary>
    /// Returns the scatter style property. If not supported, returns null.
    /// </summary>
    XYPlotScatterStyle XYScatterStyle { get; }


	}
}
