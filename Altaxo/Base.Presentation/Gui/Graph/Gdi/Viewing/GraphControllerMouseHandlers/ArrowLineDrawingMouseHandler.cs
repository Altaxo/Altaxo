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

using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// Summary description for ArrowLineDrawingMouseHandler.
  /// </summary>
  public class ArrowLineDrawingMouseHandler : SingleLineDrawingMouseHandler
  {
    public ArrowLineDrawingMouseHandler(GraphController grac)
      : base(grac)
    {
      if (_grac != null)
        _grac.SetPanelCursor(Cursors.Pen);
    }

    public override GraphToolType GraphToolType
    {
      get { return GraphToolType.ArrowLineDrawing; }
    }

    protected override void FinishDrawing()
    {
      var context = _grac.Doc.GetPropertyContext();
      LineShape go = new LineShape(_Points[0].LayerCoordinates, _Points[1].LayerCoordinates, context);

      var absArrowSize = go.Pen.Width * 8;
      go.Pen.EndCap = new Altaxo.Graph.Gdi.LineCaps.ArrowF10LineCap(absArrowSize, 4);

      // deselect the text tool
      _grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);
      _grac.ActiveLayer.GraphObjects.Add(go);
    }
  }
}
