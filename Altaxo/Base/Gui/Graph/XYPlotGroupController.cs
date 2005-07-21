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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  /// <summary>
  /// This view interface is for showing the options of the XYLineScatterPlotStyle
  /// </summary>
  public interface IXYPlotGroupView
  {
    // Get / sets the controller of this view
    IXYPlotGroupViewEventSink Controller { get; set; }
  

    /// <summary>
    /// Initializes the plot group conditions.
    /// </summary>
    /// <param name="bMemberOfPlotGroup">True if this PlotItem is member of a plot group.</param>
    /// <param name="bIndependent">True if all plots independent from each other.</param>
    /// <param name="bColor">True if the color is changed.</param>
    /// <param name="bLineType">True if the line type is changed.</param>
    /// <param name="bSymbol">True if the symbol shape is changed.</param>
    void InitializePlotGroupConditions(bool bMemberOfPlotGroup, bool bIndependent, bool bColor, bool bLineType, bool bSymbol);


    #region Getter

   

    bool PlotGroupIncremental { get; }
    bool PlotGroupColor { get; }
    bool PlotGroupLineType { get; }
    bool PlotGroupSymbol { get; }





    #endregion // Getter
  }

  /// <summary>
  /// This is the controller interface of the LineScatterPlotStyleView
  /// </summary>
  public interface IXYPlotGroupViewEventSink
  {
    void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent);
  }
  #endregion

} // end of namespace
