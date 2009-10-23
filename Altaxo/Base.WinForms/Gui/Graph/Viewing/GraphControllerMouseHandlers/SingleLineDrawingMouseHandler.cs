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
  /// Handles the drawing of a straight single line.
  /// </summary>
  public class SingleLineDrawingMouseHandler : MouseStateHandler
  {
    #region Member variables

    protected GraphView _grac;

    protected PointF _currentMousePrintAreaCoord;

		protected Altaxo.Gui.Graph.Viewing.GraphToolType NextMouseHandlerType = Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer;

    

    protected struct POINT
    {
      public PointF printAreaCoord;
      public PointF layerCoord;
    }

    protected POINT[] _Points = new POINT[2];
    protected int _currentPoint;


    #endregion

    public SingleLineDrawingMouseHandler(GraphView view)
    {
      this._grac = view;

      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.Arrow);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.SingleLineDrawing; }
		}


    /// <summary>
    /// Handles the drawing of a straight single line.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    /// <returns>The mouse state handler for handling the next mouse events.</returns>
    public override void OnClick( System.EventArgs e)
    {
      base.OnClick(e);

      // get the page coordinates (in Point (1/72") units)
      //PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
      PointF printAreaCoord = _currentMousePrintAreaCoord;
      // with knowledge of the current active layer, calculate the layer coordinates from them
      PointF layerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(printAreaCoord);

      _Points[_currentPoint].layerCoord = layerCoord;
      _Points[_currentPoint].printAreaCoord = printAreaCoord;
      _currentPoint++;

      if(2==_currentPoint)
      {
        FinishDrawing();
        _currentPoint=0;
        _grac.SetGraphToolFromInternal( NextMouseHandlerType);
      }
     
    }


    public override void OnMouseMove( MouseEventArgs e)
    {
      base.OnMouseMove ( e);
     
      _currentMousePrintAreaCoord = _grac.WinFormsController.PixelToPrintableAreaCoordinates(new Point(e.X,e.Y));

      ModifyCurrentMousePrintAreaCoordinate();

      _grac.WinFormsController.RepaintGraphArea();
   

   
    }

    protected virtual void ModifyCurrentMousePrintAreaCoordinate()
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
          double d = Math.Atan2(y,x);

          d = Math.Floor(0.5 + 12*d/Math.PI); // lock every 15 degrees
          d = d*Math.PI/12;

          x = r * Math.Cos(d);
          y = r * Math.Sin(d);

          _currentMousePrintAreaCoord.X = (float)(x + _Points[_currentPoint-1].printAreaCoord.X);
          _currentMousePrintAreaCoord.Y = (float)(y + _Points[_currentPoint-1].printAreaCoord.Y);
        }
      }
    }


    /// <summary>
    /// Draws the temporary line(s) from the first point to the mouse.
    /// </summary>
    /// <param name="g"></param>
    public override void AfterPaint(Graphics g)
    {
      base.AfterPaint ( g);

      for(int i=1;i<this._currentPoint;i++)
        g.DrawLine(Pens.Blue,_Points[i-1].printAreaCoord,_Points[i].printAreaCoord);

      if(_currentPoint>0)
        g.DrawLine(Pens.Blue,_Points[_currentPoint-1].printAreaCoord,_currentMousePrintAreaCoord);
      
    }


    protected virtual void FinishDrawing()
    {
      LineShape go = new LineShape(_Points[0].layerCoord,_Points[1].layerCoord);

      // deselect the text tool
			_grac.SetGraphToolFromInternal( Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer);
      _grac.ActiveLayer.GraphObjects.Add(go);
      _grac.WinFormsController.RefreshGraph();
      
    }

  }
}
