#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Shapes;

namespace Altaxo.Gui.Graph.Graph3D.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the drawing of a straight single line.
  /// </summary>
  public class SingleLineDrawingMouseHandler : MouseStateHandler
  {
    #region Member variables

    protected Graph3DController _grac;

    protected GraphToolType NextMouseHandlerType = GraphToolType.ObjectPointer;

    protected PointD3D[] _Points = new PointD3D[2];
    protected int _currentPoint;
    protected PointD3D _positionCurrentMouseInActiveLayerCoordinates;

    #endregion Member variables

    public SingleLineDrawingMouseHandler(Graph3DController grac)
    {
      _grac = grac;

      _grac?.View?.SetPanelCursor(Cursors.Pen);
    }

    public override GraphToolType GraphToolType
    {
      get { return GraphToolType.SingleLineDrawing; }
    }

    /// <summary>
    /// Handles the drawing of a straight single line.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">EventArgs.</param>
    /// <returns>The mouse state handler for handling the next mouse events.</returns>
    public override void OnClick(PointD3D position, MouseButtonEventArgs e)
    {
      base.OnClick(position, e);

      if (0 == _currentPoint)
      {
        _cachedActiveLayer = _grac.ActiveLayer;
        _cachedActiveLayerTransformation = _cachedActiveLayer.TransformationFromRootToHere();
      }
      GetHitPointOnActiveLayerPlaneFacingTheCamera(_grac.Doc, _grac.ActiveLayer, position, out var hitPointOnLayerPlaneInLayerCoordinates, out var rotationsRadian);
      _positionCurrentMouseInActiveLayerCoordinates = hitPointOnLayerPlaneInLayerCoordinates;

      _Points[_currentPoint] = _positionCurrentMouseInActiveLayerCoordinates;
      _currentPoint++;

      if (2 == _currentPoint)
      {
        FinishDrawing();
        _currentPoint = 0;
        _grac.CurrentGraphTool = NextMouseHandlerType;
      }
    }

    public override void OnMouseMove(PointD3D position, MouseEventArgs e)
    {
      base.OnMouseMove(position, e);

      if (null != _cachedActiveLayer)
      {
        GetHitPointOnActiveLayerPlaneFacingTheCamera(_grac.Doc, _cachedActiveLayer, position, out var hitPointOnLayerPlaneInLayerCoordinates, out var rotationsRadian);
        _positionCurrentMouseInActiveLayerCoordinates = hitPointOnLayerPlaneInLayerCoordinates;
        ModifyCurrentMousePrintAreaCoordinate();

        _grac.View?.RenderOverlay();
      }
    }

    protected virtual void ModifyCurrentMousePrintAreaCoordinate()
    {
      /*
			if (_currentPoint > 0)
			{
				bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
				bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
				// draw a temporary lines of all points to the current mouse position

				if (bShiftKey && _currentPoint > 0)
				{
					double x = _positionCurrentMouseInRootLayerCoordinates.X - _Points[_currentPoint - 1].RootLayerCoordinates.X;
					double y = _positionCurrentMouseInRootLayerCoordinates.Y - _Points[_currentPoint - 1].RootLayerCoordinates.Y;

					double r = Math.Sqrt(x * x + y * y);
					double d = Math.Atan2(y, x);

					d = Math.Floor(0.5 + 12 * d / Math.PI); // lock every 15 degrees
					d = d * Math.PI / 12;

					x = r * Math.Cos(d);
					y = r * Math.Sin(d);

					_positionCurrentMouseInRootLayerCoordinates = new PointD2D(
						(x + _Points[_currentPoint - 1].RootLayerCoordinates.X),
						(y + _Points[_currentPoint - 1].RootLayerCoordinates.Y)
						);
				}
			}
			*/
    }

    /// <summary>
    /// Draws the temporary line(s) from the first point to the mouse.
    /// </summary>
    /// <param name="g"></param>
    public override void AfterPaint(IOverlayContext3D g)
    {
      base.AfterPaint(g);

      PointD3D p0, p1;

      for (int i = 1; i < _currentPoint; i++)
      {
        // first transform this points to root layer coordinates
        p0 = _cachedActiveLayerTransformation.Transform(_Points[i - 1]);
        p1 = _cachedActiveLayerTransformation.Transform(_Points[i]);
        g.PositionColorLineListBuffer.AddLine(p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z, 0, 0, 1, 1);
      }

      if (_currentPoint > 0)
      {
        p0 = _cachedActiveLayerTransformation.Transform(_Points[_currentPoint - 1]);
        p1 = _cachedActiveLayerTransformation.Transform(_positionCurrentMouseInActiveLayerCoordinates);
        g.PositionColorLineListBuffer.AddLine(p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z, 0, 0, 1, 1);
      }
    }

    protected virtual void FinishDrawing()
    {
      var go = new LineShape(_Points[0], _Points[1], _grac.Doc.GetPropertyContext());

      // deselect the text tool
      _grac.CurrentGraphTool = GraphToolType.ObjectPointer;
      _grac.ActiveLayer.GraphObjects.Add(go);
    }
  }
}
