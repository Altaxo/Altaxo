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
using Altaxo.Graph;
using Altaxo.Serialization;
namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
 

  /// <summary>
  /// This class handles the mouse events in case the text tool is selected.
  /// </summary>
  public class TextToolMouseHandler : MouseStateHandler
  {
    /// <summary>
    /// Handles the click event by opening the text tool dialog.
    /// </summary>
    /// <param name="grac">The graph control.</param>
    /// <param name="e">EventArgs.</param>
    /// <returns>The mouse state handler for handling the next mouse events.</returns>
    public override MouseStateHandler OnClick(GraphController grac, System.EventArgs e)
    {
      base.OnClick(grac,e);

      // get the page coordinates (in Point (1/72") units)
      PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
      // with knowledge of the current active layer, calculate the layer coordinates from them
      PointF layerCoord = grac.Layers[grac.CurrentLayerNumber].GraphToLayerCoordinates(printAreaCoord);

      TextGraphics tgo = new TextGraphics();
      tgo.Position = layerCoord;

      // deselect the text tool
      grac.CurrentGraphTool = GraphTools.ObjectPointer;

      TextControlDialog dlg = new TextControlDialog(grac.Layers[grac.CurrentLayerNumber],tgo);
      if(DialogResult.OK==dlg.ShowDialog(grac.View.Window))
      {
        // add the resulting textgraphobject to the layer
        if(!dlg.SimpleTextGraphics.Empty)
        {
          grac.Layers[grac.CurrentLayerNumber].GraphObjects.Add(dlg.SimpleTextGraphics);
          grac.RefreshGraph();
        }
      }
      return new ObjectPointerMouseHandler(grac);
    }
  }
}
