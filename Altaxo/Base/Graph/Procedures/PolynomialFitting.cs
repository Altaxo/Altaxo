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

namespace Altaxo.Graph.Procedures
{
	/// <summary>
	/// This class handels the procedure of polynomial fitting to the active curve.
	/// </summary>
	public class PolynomialFitting
	{
    public static void Fit(Altaxo.Graph.GUI.GraphController ctrl, int order, double fitCurveXmin, double fitCurveXmax, bool showFormulaOnGraph)
    {

      Altaxo.Graph.PlotItem plotItem = ctrl.ActiveLayer.PlotItems[ctrl.CurrentPlotNumber];

      Altaxo.Graph.XYColumnPlotItem xyPlotItem = plotItem as Altaxo.Graph.XYColumnPlotItem;

      if(xyPlotItem==null)
        return;

   

    }
	}
}
