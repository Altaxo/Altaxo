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
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Serialization;
namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  /// <summary>
  /// Summary description for ArrowLineDrawingMouseHandler.
  /// </summary>
  public class ArrowLineDrawingMouseHandler : SingleLineDrawingMouseHandler
  {
    public ArrowLineDrawingMouseHandler(GraphController grac)
      : base(grac)
    {
      if(_grac.View!=null)
        _grac.View.SetPanelCursor(Cursors.Arrow);
    }

    protected override void FinishDrawing()
    {
      LineShape go = new LineShape(_Points[0].layerCoord,_Points[1].layerCoord);
      //go.Pen.EndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(2,1,true);
      LineCapEx cap = LineCapEx.FromName("ArrowF10");
      cap.Size = 10;
      go.Pen.EndCap = cap;
     
      // deselect the text tool
      this._grac.CurrentGraphToolType = typeof(GraphControllerMouseHandlers.ObjectPointerMouseHandler);
      _grac.Layers[_grac.CurrentLayerNumber].GraphObjects.Add(go);
      _grac.RefreshGraph();
      
    }
  }
}
