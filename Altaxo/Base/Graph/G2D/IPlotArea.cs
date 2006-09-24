#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Graph.Scales;


namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Interface used for all plot items and styles to get information for plotting their data.
  /// </summary>
  public interface IPlotArea
  {
  

    /// <summary>
    /// Gets the axis of the independent variable.
    /// </summary>
    Scale XAxis { get; }
    
    /// <summary>
    /// Gets the axis of the dependent variable.
    /// </summary>
    Scale YAxis { get; }


    G2DCoordinateSystem CoordinateSystem { get; }

  
    /// <summary>
    /// Returns the size of the rectangular layer area.
    /// </summary>
    SizeF Size { get; }
  }
}
