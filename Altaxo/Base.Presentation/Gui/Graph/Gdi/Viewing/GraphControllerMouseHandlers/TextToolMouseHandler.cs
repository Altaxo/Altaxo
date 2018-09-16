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

#endregion Copyright

using System;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// This class handles the mouse events in case the text tool is selected.
  /// </summary>
  public class TextToolMouseHandler : MouseStateHandler
  {
    private GraphController _grac;

    public TextToolMouseHandler(GraphController grac)
    {
      _grac = grac;
      if (_grac != null)
        _grac.SetPanelCursor(Cursors.IBeam);
    }

    public override GraphToolType GraphToolType
    {
      get { return GraphToolType.TextDrawing; }
    }

    /// <summary>
    /// Handles the click event by opening the text tool dialog.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    /// <param name="position">Mouse position.</param>
    /// <returns>The mouse state handler for handling the next mouse events.</returns>
    public override void OnClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnClick(position, e);

      _cachedActiveLayer = _grac.ActiveLayer;
      _cachedActiveLayerTransformation = _cachedActiveLayer.TransformationFromRootToHere();
      _cachedActiveLayerTransformationGdi = _cachedActiveLayerTransformation;

      // get the page coordinates (in Point (1/72") units)
      var rootLayerCoord = _grac.ConvertMouseToRootLayerCoordinates(_positionLastMouseDownInMouseCoordinates);
      // with knowledge of the current active layer, calculate the layer coordinates from them
      var layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(rootLayerCoord);

      var tgo = new TextGraphic(_grac.Doc.GetPropertyContext());
      tgo.SetParentSize(_grac.ActiveLayer.Size, false);
      tgo.Position = layerCoord;
      tgo.ParentObject = _grac.ActiveLayer;

      // deselect the text tool
      _grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);

      object tgoo = tgo;
      if (Current.Gui.ShowDialog(ref tgoo, "Text", false))
      {
        tgo = (TextGraphic)tgoo;
        if (tgo != null && !tgo.Empty)
        {
          _grac.ActiveLayer.GraphObjects.Add(tgo);
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
      _grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);
    }
  }
}
