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
  /// Handles the drawing of a rectangle.
  /// </summary>
  public class RectangularToolMouseHandler : SingleLineDrawingMouseHandler
  {
  

    public RectangularToolMouseHandler(GraphController grac)
      : base(grac)
    {
    }
   

    protected override void  ModifyCurrentMousePrintAreaCoordinate()
    {
      if(_currentPoint>0)
      {
        bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
        bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));      
        // draw a temporary lines of all points to the current mouse position

        if(bShiftKey && _currentPoint>0)
        {
          double x = _currentMousePrintAreaCoord.X - _Points[_currentPoint-1].printAreaCoord.X;
          double y = _currentMousePrintAreaCoord.Y - _Points[_currentPoint-1].printAreaCoord.Y;

          double r = Math.Sqrt(x*x+y*y);
          
          x = r * Math.Sign(x);
          y = r * Math.Sign(y);

          _currentMousePrintAreaCoord.X = (float)(x + _Points[_currentPoint-1].printAreaCoord.X);
          _currentMousePrintAreaCoord.Y = (float)(y + _Points[_currentPoint-1].printAreaCoord.Y);
        }
      }
    }

    /// <summary>
    /// Draws the temporary line(s) from the first point to the mouse.
    /// </summary>
    /// <param name="grac"></param>
    /// <param name="g"></param>
    public override void AfterPaint(GraphController grac, Graphics g)
    {
      base.AfterPaint (grac, g);

      g.TranslateTransform(_grac.Doc.PrintableBounds.X,_grac.Doc.PrintableBounds.Y);

      if(_currentPoint>=1)
        DrawRectangleFromLTRB(g,_Points[0].printAreaCoord,_currentMousePrintAreaCoord);

    }

    void DrawRectangleFromLTRB(Graphics g,PointF a, PointF b)
    {
      RectangleF rect = RectangleF.FromLTRB(a.X,a.Y,b.X,b.Y);
      g.DrawRectangle(Pens.Blue,rect.X,rect.Y,rect.Width,rect.Height);      
    }


    protected override void FinishDrawing()
    {
      Graph.RectangleGraphic go =  Graph.RectangleGraphic.FromLTRB(_Points[0].layerCoord.X,_Points[0].layerCoord.Y,_Points[1].layerCoord.X,_Points[1].layerCoord.Y);

      // deselect the text tool
      this._grac.CurrentGraphTool = GraphTools.ObjectPointer;
      _grac.Layers[_grac.CurrentLayerNumber].GraphObjects.Add(go);
      _grac.RefreshGraph();
      
    }

  }
}
