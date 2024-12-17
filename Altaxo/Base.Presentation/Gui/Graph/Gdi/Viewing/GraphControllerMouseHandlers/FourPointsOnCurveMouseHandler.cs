#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Plot.Data;


namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// A mouse handler that creates either 2 or 4 points on a curve, that can be used to evaluate the area under the line that is
  /// created by the points, or to evaluate step positions.
  /// </summary>
  public class FourPointsOnCurveMouseHandler : MouseStateHandler
  {
    // Working thesis:
    // 1. The user uses the cross cursor to set the first mark on a curve.
    // 2. With that, the curve is fixed, and can be changed only the next time this tool is used
    // 3. The user chooses another position on the curve and click to set the 2nd position.
    // 4. Now we have two positions on the curve, with handles upward and downward. The upward handles will move the outer points,
    //    and the downward flags will handle the inner points.
    // Keyboard shortcuts: ESC: Pointer back to normal, Left/Right: moves all points on a curve
    // 1 + Left/Right : moves the most-left point on the curve ... 4 + Left/Right: moves the most-right point on the curve.


    private struct Handle
    {
      public int PlotIndex;
      public int RowIndex;
      public PointD2D Position;
      public RectangleF HandleBounds;
    }

    private class HandleDragState
    {
      public int IndexOfHandle;
      public PointD2D MouseStartCoordinates;

      public int PlotIndex;
      public int RowIndex;
      public PointD2D Position;
    }

    private class CatchLine
    {
      public PointD2D MouseCoordinates;
      public PointD2D ScatterPointCoordinates;
    }

    /// <summary>
    /// Layer, in which the plot item resides which is currently selected.
    /// </summary>
    protected XYPlotLayer _layer;

    /// <summary>
    /// The number of the plot item where the cross is currently.
    /// </summary>
    protected int _PlotItemNumber;

    /// <summary>
    /// The plot item where the mouse snaps in
    /// </summary>
    protected XYColumnPlotItem? _plotItem;

    private Handle[] _handle = new Handle[4];
    private HandleDragState? _handleDragState;


    protected GraphController _grac;

    private enum State { NoPoint, OnePoint, TwoPoints };

    private State _state;

    private bool _drawLineBetweenOuterPoints = true;

    /// <summary>
    /// A line that is drawn from the current mouse coordinates to the nearest point on the selected curve.
    /// </summary>
    private CatchLine? _catchLine;


    public FourPointsOnCurveMouseHandler(GraphController grac)
    {
      _grac = grac ?? throw new System.ArgumentNullException(nameof(grac));
      if (_grac is not null)
        _grac.SetPanelCursor(Cursors.Cross);
    }

    public override GraphToolType GraphToolType => GraphToolType.FourPointsOnCurve;


    /// <summary>
    /// Handles the MouseDown event when the tool is selected
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">The mouse event args</param>
    public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseDown(position, e);
      var graphXY = _grac.ConvertMouseToRootLayerCoordinates(position);

      switch (_state)
      {
        case State.NoPoint:
          OnMouseDown_NoPoint(position, graphXY);
          break;
        case State.OnePoint:
          OnMouseDown_OnePoint(position, graphXY);
          break;
        default:
          OnMouseDown_FullState(position, graphXY);
          break;
      }
    } // end of function

    /// <summary>
    /// Handles the mouse down event when the tool was freshly selected, so that no curve is selected in the moment.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_NoPoint(PointD2D position, PointD2D graphXY)
    {
      _grac.FindGraphObjectAtPixelPosition(position, true, out var clickedObject, out var clickedLayerNumber);
      if (clickedObject is not null && clickedObject.HittedObject is XYColumnPlotItem)
      {
        _plotItem = (XYColumnPlotItem)clickedObject.HittedObject;
        var transXY = clickedObject.Transformation.InverseTransformPoint(graphXY);

        _layer = (XYPlotLayer)(clickedObject.ParentLayer);
        XYScatterPointInformation scatterPoint = _plotItem.GetNearestPlotPoint(_layer, transXY);
        _PlotItemNumber = GetPlotItemNumber(_layer, _plotItem);

        if (scatterPoint is not null)
        {
          var plotIndex = scatterPoint.PlotIndex;
          var rowIndex = scatterPoint.RowIndex;
          // convert this layer coordinates first to PrintableAreaCoordinates
          var rootLayerCoord = clickedObject.ParentLayer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());
          _handle[0] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
          _handle[1] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
          _handle[2] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
          _handle[3] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
          _state = State.OnePoint;

          UpdateDataDisplay();   // show coordinates in the data reader
          _grac.RenderOverlay(); // invalidate the overlay
        }
      }
    }

    /// <summary>
    /// Handles the mouse down event after one point is already selected.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_OnePoint(PointD2D position, PointD2D graphXY)
    {
      var transXY = _layer.TransformCoordinatesFromRootToHere(graphXY);
      XYScatterPointInformation scatterPoint = _plotItem.GetNearestPlotPoint(_layer, transXY);
      if (scatterPoint is not null)
      {
        var plotIndex = scatterPoint.PlotIndex;
        var rowIndex = scatterPoint.RowIndex;
        // convert this layer coordinates first to PrintableAreaCoordinates
        var rootLayerCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());
        _handle[2] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
        _handle[3] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };

        if (_handle[3].Position.X < _handle[0].Position.X)
        {
          (_handle[0], _handle[3]) = (_handle[3], _handle[0]);
          (_handle[1], _handle[2]) = (_handle[2], _handle[1]);
        }

        var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(rootLayerCoord);
        _state = State.TwoPoints;
        _catchLine = null;

        UpdateDataDisplay();   // show coordinates in the data reader

        // here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
        // of the grac
        _grac.RenderOverlay(); // no refresh necessary, only invalidate to show the cross

      }
    }

    /// <summary>
    /// Handles the mouse down event after all points are specified. Thus, now it is checked if the user grabbed a handle.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_FullState(PointD2D position, PointD2D graphXY)
    {
      for (int i = 0; i < _handle.Length; i++)
      {
        if (_handle[i].HandleBounds.Contains((float)graphXY.X, (float)graphXY.Y))
        {
          _handleDragState = new HandleDragState { IndexOfHandle = i, MouseStartCoordinates = graphXY };
          break;
        }
      }
    }

    public override void OnMouseMove(PointD2D position, MouseEventArgs e)
    {
      base.OnMouseMove(position, e);
      var mouseRootCoord = _grac.ConvertMouseToRootLayerCoordinates(position);

      if (_state == State.OnePoint)
      {
        var mouseLayerCoord = _layer.TransformCoordinatesFromRootToHere(mouseRootCoord);
        if (_plotItem.GetNearestPlotPoint(_layer, mouseLayerCoord) is { } scatterPoint)
        {
          _catchLine = new CatchLine { MouseCoordinates = mouseRootCoord, ScatterPointCoordinates = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D()) };
          _grac.RenderOverlay();
        }
      }
      if (_handleDragState is { } dragState)
      {
        var mouseLayerCoord = _layer.TransformCoordinatesFromRootToHere(mouseRootCoord);
        XYScatterPointInformation scatterPoint = _plotItem.GetNearestPlotPoint(_layer, mouseLayerCoord);
        if (scatterPoint is not null)
        {
          var plotIndex = scatterPoint.PlotIndex;
          var rowIndex = scatterPoint.RowIndex;
          var scatterRootCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());

          // only accept this point, if the x-order of the handles is maintained
          if (IsPointAcceptable(scatterRootCoord, dragState.IndexOfHandle))
          {
            dragState.Position = scatterRootCoord;
            dragState.PlotIndex = plotIndex;
            dragState.RowIndex = rowIndex;
            _catchLine = new CatchLine { MouseCoordinates = mouseRootCoord, ScatterPointCoordinates = scatterRootCoord };
            _grac.RenderOverlay();
          }
        }
      }
    }

    private bool IsPointAcceptable(PointD2D point, int indexOfHandle)
    {
      for (int i = 0; i < indexOfHandle; i++)
      {
        if (_handle[i].Position.X > point.X)
          return false;
      }
      for (int i = indexOfHandle + 1; i < _handle.Length; i++)
      {
        if (_handle[i].Position.X < point.X)
          return false;
      }
      return true;
    }

    public override void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseUp(position, e);

      bool renderOverlay = false;

      if (_handleDragState is { } dragState)
      {
        _handle[dragState.IndexOfHandle].PlotIndex = dragState.PlotIndex;
        _handle[dragState.IndexOfHandle].RowIndex = dragState.RowIndex;
        _handle[dragState.IndexOfHandle].Position = dragState.Position;
        _handleDragState = null;
        renderOverlay = true;
      }

      if (_catchLine is not null)
      {
        _catchLine = null;
        renderOverlay = true;
      }

      if (renderOverlay)
      {
        UpdateDataDisplay();   // show coordinates in the data reader
        _grac.RenderOverlay();
      }
    }


    /// <summary>
    /// This function is called just after the paint event. The graphic context is in graph coordinates.
    /// </summary>
    /// <param name="g">Graphics context</param>
    public override void AfterPaint(Graphics g)
    {
      // draw a red cross onto the selected data point
      //  

      if (_state == State.NoPoint)
        return;

      if (_catchLine is { } catchLine)
      {
        using var catchPen = new Pen(Color.Orange, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        g.DrawLine(catchPen, (float)catchLine.MouseCoordinates.X, (float)catchLine.MouseCoordinates.Y, (float)catchLine.ScatterPointCoordinates.X, (float)catchLine.ScatterPointCoordinates.Y);
      }


      using var brush = new SolidBrush(Color.Blue);
      using var pen = new Pen(brush, (float)(2 / _grac.ZoomFactor));


      for (int i = 0; i < _handle.Length; ++i)
      {
        if (_handleDragState is not null && _handleDragState.IndexOfHandle == i)
        {
          DrawHandle(g, brush, pen, _handleDragState.Position, i);
        }
        else
        {
          _handle[i].HandleBounds = DrawHandle(g, brush, pen, _handle[i].Position, i);
        }
      }

      if (_drawLineBetweenOuterPoints)
      {
        g.DrawLine(pen, (float)_handle[0].Position.X, (float)_handle[0].Position.Y, (float)_handle[^1].Position.X, (float)_handle[^1].Position.Y);
      }


      base.AfterPaint(g);
    }

    private RectangleF DrawHandle(Graphics g, Brush brush, Pen pen, PointD2D position, int indexOfHandle)
    {
      var downFactor = indexOfHandle == 1 || indexOfHandle == 2 ? 1 : -1;
      var leftFactor = indexOfHandle == 0 || indexOfHandle == 1 ? -1 : 1;

      var stemLength = 10 / _grac.ZoomFactor;
      var squareLength = stemLength / 2d;
      // draw the stem
      g.DrawLine(pen, (float)position.X, (float)position.Y, (float)position.X, (float)(position.Y + downFactor * stemLength));

      var xr = position.X + Math.Min(leftFactor, 0) * squareLength;
      var yr = position.Y + downFactor * stemLength + Math.Min(downFactor, 0) * squareLength;

      var rect = new RectangleF((float)xr, (float)yr, (float)squareLength, (float)squareLength);
      g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
      return rect;
    }

    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="e">Key event arguments.</param>
    /// <returns></returns>
    public override bool ProcessCmdKey(KeyEventArgs e)
    {
      int? indexOfHandle = null;
      if (e.KeyboardDevice.IsKeyDown(Key.D1) || e.KeyboardDevice.IsKeyDown(Key.NumPad1))
        indexOfHandle = 0;
      else if (e.KeyboardDevice.IsKeyDown(Key.D2) || e.KeyboardDevice.IsKeyDown(Key.NumPad2))
        indexOfHandle = 1;
      else if (e.KeyboardDevice.IsKeyDown(Key.D3) || e.KeyboardDevice.IsKeyDown(Key.NumPad3))
        indexOfHandle = 2;
      else if (e.KeyboardDevice.IsKeyDown(Key.D4) || e.KeyboardDevice.IsKeyDown(Key.NumPad4))
        indexOfHandle = 3;

      if (indexOfHandle.HasValue)
      {
        var keyData = e.Key;
        if (keyData == Key.Left)
        {
          MoveHandleLeftRight(indexOfHandle.Value, -1);
          return true;
        }
        else if (keyData == Key.Right)
        {
          MoveHandleLeftRight(indexOfHandle.Value, 1);
          return true;
        }
      }

      if (e.Key == Key.Escape)
      {
        _grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);
      }

      return false; // per default the key is not processed
    }

    private void MoveHandleLeftRight(int indexOfHandle, int direction)
    {
      var h = _handle[indexOfHandle];

      // test one point index up and one point index down, and find out if that is the intended direction
      XYScatterPointInformation up = null, down = null, next = null;

      var prevPlotIndexUp = h.PlotIndex;
      var prevPlotIndexDown = h.PlotIndex;
      for (; ; )
      {
        up = _plotItem.GetNextPlotPoint(_layer, h.PlotIndex, 1);
        down = _plotItem.GetNextPlotPoint(_layer, h.PlotIndex, -1);

        if (up is not null)
        {
          var rootCoord = _layer.TransformCoordinatesFromHereToRoot(up.LayerCoordinates.ToPointD2D());
          if (Math.Sign(rootCoord.X - h.Position.X) == direction)
          {
            next = up;
            break;
          }
        }
        if (down is not null)
        {
          var rootCoord = _layer.TransformCoordinatesFromHereToRoot(down.LayerCoordinates.ToPointD2D());
          if (Math.Sign(rootCoord.X - h.Position.X) == direction)
          {
            next = down;
            break;
          }
        }

        if ((up is null || up.PlotIndex <= prevPlotIndexUp) && (down is null || down.PlotIndex >= prevPlotIndexDown))
        {
          break;
        }

        if (up is not null)
          prevPlotIndexUp = up.PlotIndex;
        if (down is not null)
          prevPlotIndexDown = down.PlotIndex;
      }

      if (next is not null)
      {
        // is this point acceptable?
        var rootCoord = _layer.TransformCoordinatesFromHereToRoot(next.LayerCoordinates.ToPointD2D());
        if (IsPointAcceptable(rootCoord, indexOfHandle))
        {
          _handle[indexOfHandle].PlotIndex = next.PlotIndex;
          _handle[indexOfHandle].RowIndex = next.RowIndex;
          _handle[indexOfHandle].Position = rootCoord;
          UpdateDataDisplay();   // show coordinates in the data reader
          _grac.RenderOverlay();
        }
      }
    }

    private void UpdateDataDisplay()
    {
      if (_state == State.NoPoint)
      {
        Current.DataDisplay.WriteThreeLines(
          "",
          "",
          ""
          );
      }
      else if (_handle.Length == 2)
      {
        var xcol = _plotItem.XYColumnPlotData.XColumn;
        var ycol = _plotItem.XYColumnPlotData.YColumn;

        if (xcol is not null && ycol is not null)
        {
          Current.DataDisplay.WriteThreeLines(
            $"{_layer.Name}: {_plotItem}",
            $"XL[{_handle[0].RowIndex}]={xcol[_handle[0].RowIndex]}; YL[{_handle[0].RowIndex}]={ycol[_handle[0].RowIndex]}; XR[{_handle[1].RowIndex}]={xcol[_handle[1].RowIndex]}; YR[{_handle[1].RowIndex}]={ycol[_handle[1].RowIndex]}",
            $"DX={xcol[_handle[1].RowIndex] - xcol[_handle[0].RowIndex]}; DY={ycol[_handle[1].RowIndex] - ycol[_handle[0].RowIndex]}"
            );
        }
      }
      else if (_handle.Length == 4)
      {
        var xcol = _plotItem.XYColumnPlotData.XColumn;
        var ycol = _plotItem.XYColumnPlotData.YColumn;

        if (xcol is not null && ycol is not null)
        {
          Current.DataDisplay.WriteThreeLines(
            $"{_layer.Name}: {_plotItem}",
            $"XLO[{_handle[0].RowIndex}]={xcol[_handle[0].RowIndex]}; YLO[{_handle[0].RowIndex}]={ycol[_handle[0].RowIndex]}; XRO[{_handle[3].RowIndex}]={xcol[_handle[3].RowIndex]}; YRO[{_handle[3].RowIndex}]={ycol[_handle[3].RowIndex]}; DX={xcol[_handle[3].RowIndex] - xcol[_handle[0].RowIndex]}; DY={ycol[_handle[3].RowIndex] - ycol[_handle[0].RowIndex]}",
            $"XLI[{_handle[1].RowIndex}]={xcol[_handle[1].RowIndex]}; YLI[{_handle[1].RowIndex}]={ycol[_handle[1].RowIndex]}; XRI[{_handle[2].RowIndex}]={xcol[_handle[2].RowIndex]}; YRI[{_handle[2].RowIndex}]={ycol[_handle[2].RowIndex]}; DX={xcol[_handle[2].RowIndex] - xcol[_handle[1].RowIndex]}; DY={ycol[_handle[2].RowIndex] - ycol[_handle[1].RowIndex]}"
            );
        }
      }
    }

    /// <summary>
    /// Find the plot item number of a given plot item.
    /// </summary>
    /// <param name="layer">The layer in which this plot item resides.</param>
    /// <param name="plotitem">The plot item for which the number should be retrieved.</param>
    /// <returns></returns>
    private int GetPlotItemNumber(XYPlotLayer layer, XYColumnPlotItem plotitem)
    {
      if (layer is not null)
      {
        for (int i = 0; i < layer.PlotItems.Flattened.Length; i++)
          if (object.ReferenceEquals(layer.PlotItems.Flattened[i], plotitem))
            return i;
      }
      return -1;
    }
  }
}
