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
using System.Windows.Input;

using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{

  /// <summary>
  /// Handles the drawing of a rectangle.
  /// </summary>
  public abstract class AbstractRectangularToolMouseHandler : MouseStateHandler
  {
  
    #region Member variables

    protected GraphViewWpf _grac;

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

    public AbstractRectangularToolMouseHandler(GraphViewWpf view)
    {
      _grac = view;

      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.Pen);
    }


		public override void OnMouseDown(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseDown (position, e);

      if(e.ChangedButton== MouseButton.Left)
      {
        _currentPoint = 0;
        // get the page coordinates (in Point (1/72") units)
        //PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
        PointF printAreaCoord = _currentMousePrintAreaCoord;
        // with knowledge of the current active layer, calculate the layer coordinates from them
        PointF layerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(printAreaCoord);

        _Points[_currentPoint].layerCoord = layerCoord;
        _Points[_currentPoint].printAreaCoord = printAreaCoord;
        _currentPoint++;

       
      }
    }

		public override void OnMouseMove(Altaxo.Graph.PointD2D position, MouseEventArgs e)
    {
     
      base.OnMouseMove (position, e);
     
      _currentMousePrintAreaCoord = _grac.GuiController.PixelToPrintableAreaCoordinates(position);

			if (e.LeftButton == MouseButtonState.Pressed)
      {
        ModifyCurrentMousePrintAreaCoordinate();
        _grac.GuiController.RepaintGraphArea();
      }
      else if(_currentPoint!=0)
      {
        _currentPoint=0;
        _grac.GuiController.RepaintGraphArea();
      }

    }

		public override void OnMouseUp(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseUp (position, e);

			if (e.ChangedButton== MouseButton.Left)
      {
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
          _grac.SetGraphToolFromInternal( NextMouseHandlerType );
        }
      }
    }



    protected virtual void  ModifyCurrentMousePrintAreaCoordinate()
    {
      if(_currentPoint>0)
      {
        bool bControlKey=Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
        bool bShiftKey=Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);      
        // draw a temporary lines of all points to the current mouse position

        if(bShiftKey && _currentPoint>0)
        {
          if (_grac.ActiveLayer != null) // with an active layer, we transform to layer coordinates
          {
            PointF currMouseLayerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(_currentMousePrintAreaCoord);
            double x = currMouseLayerCoord.X - _Points[_currentPoint - 1].layerCoord.X;
            double y = currMouseLayerCoord.Y - _Points[_currentPoint - 1].layerCoord.Y;

            double r = Math.Sqrt(x * x + y * y);

            x = r * Math.Sign(x);
            y = r * Math.Sign(y);

            currMouseLayerCoord.X = (float)(x + _Points[_currentPoint - 1].layerCoord.X);
            currMouseLayerCoord.Y = (float)(y + _Points[_currentPoint - 1].layerCoord.Y);
            _currentMousePrintAreaCoord = _grac.ActiveLayer.LayerToGraphCoordinates(currMouseLayerCoord);
          }
          else // without an active layer we use document coordinates
          {
            double x = _currentMousePrintAreaCoord.X - _Points[_currentPoint - 1].printAreaCoord.X;
            double y = _currentMousePrintAreaCoord.Y - _Points[_currentPoint - 1].printAreaCoord.Y;

            double r = Math.Sqrt(x * x + y * y);

            x = r * Math.Sign(x);
            y = r * Math.Sign(y);

            _currentMousePrintAreaCoord.X = (float)(x + _Points[_currentPoint - 1].printAreaCoord.X);
            _currentMousePrintAreaCoord.Y = (float)(y + _Points[_currentPoint - 1].printAreaCoord.Y);
          }
        }
      }
    }

    /// <summary>
    /// Draws the temporary line(s) from the first point to the mouse.
    /// </summary>
    /// <param name="g"></param>
    public override void AfterPaint(Graphics g)
    {
      if (_currentPoint >= 1)
      {
        if (null != _grac.ActiveLayer)
        {
          g.TranslateTransform(_grac.ActiveLayer.Position.X, _grac.ActiveLayer.Position.Y);
          g.RotateTransform((float)-_grac.ActiveLayer.Rotation);
          PointF currLayerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(_currentMousePrintAreaCoord);
          DrawRectangleFromLTRB(g, _Points[0].layerCoord, currLayerCoord);
        }
        else
        {
          DrawRectangleFromLTRB(g, _Points[0].printAreaCoord, _currentMousePrintAreaCoord);
        }
      }
    }


    public RectangleF GetNormalRectangle(PointF a, PointF b)
    {
      float x = Math.Min(a.X,b.X);
      float y = Math.Min(a.Y,b.Y);

      float w = Math.Abs(a.X-b.X);
      float h = Math.Abs(a.Y-b.Y);

      return new RectangleF(x,y,w,h);
    }

    void DrawRectangleFromLTRB(Graphics g,PointF a, PointF b)
    {
      RectangleF rect = RectangleF.FromLTRB(a.X,a.Y,b.X,b.Y);
      Pen pen = Pens.Blue;
      g.DrawLine(pen,a.X,a.Y,b.X,a.Y);
      g.DrawLine(pen,b.X,a.Y,b.X,b.Y);
      g.DrawLine(pen,b.X,b.Y,a.X,b.Y);
      g.DrawLine(pen,a.X,b.Y,a.X,a.Y);
      //      g.DrawRectangle(Pens.Blue,rect.X,rect.Y,rect.Width,rect.Height);      
    }

    protected abstract void FinishDrawing();

  }
}
