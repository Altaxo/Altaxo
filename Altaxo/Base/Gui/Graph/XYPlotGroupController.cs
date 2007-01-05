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
    /// <param name="bColor">True if the color is changed.</param>
    /// <param name="bLineType">True if the line type is changed.</param>
    /// <param name="bSymbol">True if the symbol shape is changed.</param>
    /// <param name="bConcurrently">True if all styles are changed concurrently.</param>
    /// <param name="bStrict">True if the depending plot styles are enforced to have strictly the same properties than the parent style.</param>
    void InitializePlotGroupConditions(bool bColor, bool bLineType, bool bSymbol, bool bConcurrently, Altaxo.Graph.Plot.Groups.PlotGroupStrictness bStrict);

    /// <summary>
    /// Fired if user requires the full plot group control.
    /// </summary>
    event EventHandler AdvancedPlotGroupControl;

    #region Getter

   

    Altaxo.Graph.Plot.Groups.PlotGroupStrictness PlotGroupStrict { get; }
    bool PlotGroupColor { get; }
    bool PlotGroupLineType { get; }
    bool PlotGroupSymbol { get; }
    bool PlotGroupConcurrently { get; }
    bool PlotGroupUpdate { get; }





    #endregion // Getter
  }

  /// <summary>
  /// This is the controller interface of the LineScatterPlotStyleView
  /// </summary>
  public interface IXYPlotGroupViewEventSink
  {
    
  }
  #endregion

} // end of namespace
