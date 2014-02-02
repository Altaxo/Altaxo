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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{

  /// <summary>
  /// Handles the zooming into the axes of a plot, i.e changing the axis boundaries.
  /// </summary>
  public class ZoomAxesMouseHandler : AbstractRectangularToolMouseHandler
  {
    public ZoomAxesMouseHandler(GraphControllerWpf grac)
      : base(grac)
    {
			NextMouseHandlerType = this.GraphToolType;

      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.SizeAll);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.ZoomAxes; }
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
      XYPlotLayer layer = this._grac.ActiveLayer as XYPlotLayer;
      layer.CoordinateSystem.LayerToLogicalCoordinates(_Points[0].LayerCoordinates.X, _Points[0].LayerCoordinates.Y, out r0);
      layer.CoordinateSystem.LayerToLogicalCoordinates(_Points[1].LayerCoordinates.X, _Points[1].LayerCoordinates.Y, out r1);

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

			try // trying to set the scales can go wrong..
			{
				layer.XAxis.SetScaleOrgEnd(xo, xe);
				layer.YAxis.SetScaleOrgEnd(yo, ye);
			}
			catch (Exception)
			{
			}

      // deselect the text tool
      // this._grac.CurrentGraphToolType = typeof(GraphControllerMouseHandlers.ObjectPointerMouseHandler);
      _grac.InvalidateCachedGraphImageAndRepaintOffline();
      
    }

  }
}
