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
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using Altaxo.Data;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{

  /// <summary>
  /// Handles the zooming into the axes of a plot, i.e changing the axis boundaries.
  /// </summary>
  public class ZoomAxesMouseHandler : AbstractRectangularToolMouseHandler
  {
    public ZoomAxesMouseHandler(GraphController grac)
      : base(grac)
    {
      NextMouseHandlerType = this.GetType();

      if(_grac.View!=null)
        _grac.View.SetPanelCursor(Cursors.Arrow);
    }
   


    void Swap(ref double a, ref double b)
    {
      double h = a;
      a = b;
      b = h;
    }
    protected override void FinishDrawing()
    {

      // we must deduce from layer coordinates back to physical coordinates
      Logical3D r0, r1;
      XYPlotLayer layer = this._grac.ActiveLayer;
      layer.CoordinateSystem.LayerToLogicalCoordinates(_Points[0].layerCoord.X, _Points[0].layerCoord.Y, out r0);
      layer.CoordinateSystem.LayerToLogicalCoordinates(_Points[1].layerCoord.X, _Points[1].layerCoord.Y, out r1);

      double xr0 = r0.RX;
      double yr0 = r0.RY;

      double xr1 = r1.RX;
      double yr1 = r1.RY;

      if(xr0>xr1)
        Swap(ref xr0, ref xr1);
      if(yr0>yr1)
        Swap(ref yr0, ref yr1);

      // now with the help of the axes, calculate the new boundaries
      AltaxoVariant xo = layer.XAxis.NormalToPhysicalVariant(xr0);
      AltaxoVariant xe = layer.XAxis.NormalToPhysicalVariant(xr1);
      AltaxoVariant yo = layer.YAxis.NormalToPhysicalVariant(yr0);
      AltaxoVariant ye = layer.YAxis.NormalToPhysicalVariant(yr1);

     

      layer.XAxis.ProcessDataBounds(xo,true,xe,true);
      layer.YAxis.ProcessDataBounds(yo,true,ye,true);

      // deselect the text tool
      // this._grac.CurrentGraphToolType = typeof(GraphControllerMouseHandlers.ObjectPointerMouseHandler);
      _grac.RefreshGraph();
      
    }

  }
}
