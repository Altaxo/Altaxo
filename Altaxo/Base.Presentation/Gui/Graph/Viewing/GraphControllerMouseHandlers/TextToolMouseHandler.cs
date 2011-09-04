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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
 

  /// <summary>
  /// This class handles the mouse events in case the text tool is selected.
  /// </summary>
  public class TextToolMouseHandler : MouseStateHandler
  {
    GraphViewWpf _grac;

    public TextToolMouseHandler(GraphViewWpf grac)
    {
      _grac = grac;
      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.IBeam);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.TextDrawing; }
		}


    /// <summary>
    /// Handles the click event by opening the text tool dialog.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    /// <returns>The mouse state handler for handling the next mouse events.</returns>
    public override void OnClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnClick(position, e);

      // get the page coordinates (in Point (1/72") units)
      PointF printAreaCoord = _grac.GuiController.PixelToPrintableAreaCoordinates(m_LastMouseDown);
      // with knowledge of the current active layer, calculate the layer coordinates from them
      PointF layerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(printAreaCoord);

      TextGraphic tgo = new TextGraphic();
      tgo.Position = layerCoord;
      tgo.ParentObject = _grac.ActiveLayer.GraphObjects;

      // deselect the text tool
			_grac.SetGraphToolFromInternal( Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer);

      object tgoo = tgo;
      if (Current.Gui.ShowDialog(ref tgoo, "Text", false))
      {
        tgo = (TextGraphic)tgoo;
        if (tgo!=null && !tgo.Empty)
        {
          _grac.ActiveLayer.GraphObjects.Add(tgo);
          _grac.GuiController.InvalidateCachedGraphImageAndRepaintOffline();
        }
      }

      /*
      TextControlDialog dlg = new TextControlDialog(_grac.Layers[_grac.CurrentLayerNumber],tgo);
      if(DialogResult.OK==dlg.ShowDialog(_grac.View.Window))
      {
        // add the resulting textgraphobject to the layer
        if(!dlg.SimpleTextGraphics.Empty)
        {
          _grac.Layers[_grac.CurrentLayerNumber].GraphObjects.Add(dlg.SimpleTextGraphics);
          _grac.RefreshGraph();
        }
      }
      */
			_grac.SetGraphToolFromInternal( Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer);
    }
  }
}
