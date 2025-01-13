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
using Altaxo.Calc;
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


    /// <summary>
    /// Stores information about a handle.
    /// </summary>
    public struct Handle
    {
      /// <summary>
      /// The index of the plot data. This can be different from the row index, e.g. if not all data are plotted.
      /// </summary>
      public double PlotIndex;

      /// <summary>
      /// The row index. Note, that when the plot index is fractional, the row index must be interpolated
      /// between the row index corresponding to the floor of the plot point index and the row index
      /// corresponding to the ceiling of the plot point index.
      /// </summary>
      public double RowIndex;

      /// <summary>
      /// The position of the handle in root layer coordinates
      /// </summary>
      public PointD2D Position;

      /// <summary>
      /// The bounds of the handle rectangle that can be used to drag the handle, in root layer coordinates.
      /// </summary>
      public RectangleF HandleBounds;
    }

    protected class HandleDragState
    {
      public int IndexOfHandle;
      public PointD2D MouseStartCoordinates;

      public double PlotIndex;
      public double RowIndex;
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
    public int PlotItemNumber { get; protected set; }

    /// <summary>
    /// The plot item where the mouse snaps in
    /// </summary>
    public XYColumnPlotItem? PlotItem { get; protected set; }

    protected Handle[] _handle = new Handle[4];
    private HandleDragState? _handleDragState;


    protected GraphController _grac;

    protected enum State { NoPoint, OnePoint, TwoPoints, ThreePoints, FourPoints };

    protected State _state;

    /// <summary>Gets the state that is considered as final state, i.e. the end of the initialization stage.</summary>
    protected State _finalState { get; }

    /// <summary>
    /// A line that is drawn from the current mouse coordinates to the nearest point on the selected curve.
    /// </summary>
    private CatchLine? _catchLine;

    public Handle LeftHandle => _handle[0];

    public Handle RightHandle => _handle[^1];

    /// <summary>
    /// Gets a value that indicated whether the handle positions now can be used for other tools or operations.
    /// </summary>
    public bool IsReadyToBeUsed
    {
      get
      {
        return _state != State.NoPoint && _state == _finalState;
      }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointsOnCurveMouseHandler"/> class.
    /// </summary>
    /// <param name="grac">The current graphics controller.</param>
    /// <param name="useFourHandles">If set to <c>true</c>, 4 handles (2 outer, 2 inner) are used. If set to <c>false</c>, only two handles are used.</param>
    /// <param name="initAllFourHandles">If set to <c>true</c>, and 4 handles are used, the user has to set all 4 handles before the initialization is finished.
    /// If set to <c>false</c>, the user only has to set the outer handles to finish the initialization, and then he has to drag the inner handles.</param>
    /// <exception cref="System.ArgumentNullException">grac</exception>
    public FourPointsOnCurveMouseHandler(GraphController grac, bool useFourHandles, bool initAllFourHandles)
    {
      _grac = grac ?? throw new System.ArgumentNullException(nameof(grac));
      if (_grac is not null)
      {
        _grac.SetPanelCursor(Cursors.Cross);
      }

      _handle = new Handle[useFourHandles ? 4 : 2];
      _finalState = useFourHandles ? (initAllFourHandles ? State.FourPoints : State.TwoPoints) : State.TwoPoints;

      UpdateDataDisplay();
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

      switch ((_state, _finalState))
      {
        case (State.NoPoint, _):
          OnMouseDown_NoPoint(position, graphXY);
          break;
        case (State.OnePoint, State.TwoPoints):
          OnMouseDown_OnePointOfTwo(position, graphXY);
          break;
        case (State.OnePoint, State.FourPoints):
          OnMouseDown_OnePointOfFour(position, graphXY);
          break;
        case (State.TwoPoints, State.FourPoints):
          OnMouseDown_TwoPointsOfFour(position, graphXY);
          break;
        case (State.ThreePoints, State.FourPoints):
          OnMouseDown_ThreePointsOfFour(position, graphXY);
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
      if (clickedObject?.HittedObject is XYColumnPlotItem item)
      {
        PlotItem = item;
        var transXY = clickedObject.Transformation.InverseTransformPoint(graphXY);

        _layer = (XYPlotLayer)(clickedObject.ParentLayer);
        XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, transXY);
        PlotItemNumber = GetPlotItemNumber(_layer, PlotItem);

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

          OnHandlesUpdated();   // show coordinates in the data reader
        }
        OnPlotItemSet(item);
      }
    }

    /// <summary>
    /// Is called when the plot item is set, so that now the curve which is under consideration is fixed.
    /// </summary>
    /// <param name="plotItem">The plot item.</param>
    protected virtual void OnPlotItemSet(XYColumnPlotItem plotItem)
    {
    }

    /// <summary>
    /// Handles the mouse down event after one point is already selected.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_OnePointOfTwo(PointD2D position, PointD2D graphXY)
    {
      var transXY = _layer.TransformCoordinatesFromRootToHere(graphXY);
      XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, transXY);
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

        OnHandlesUpdated();   // show coordinates in the data reader
      }
    }

    /// <summary>
    /// Handles the mouse down event after one point is already selected.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_OnePointOfFour(PointD2D position, PointD2D graphXY)
    {
      var transXY = _layer.TransformCoordinatesFromRootToHere(graphXY);
      XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, transXY);
      if (scatterPoint is not null)
      {
        var plotIndex = scatterPoint.PlotIndex;
        var rowIndex = scatterPoint.RowIndex;
        // convert this layer coordinates first to PrintableAreaCoordinates
        var rootLayerCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());
        _handle[1] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
        _handle[2] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
        _handle[3] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };

        var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(rootLayerCoord);
        _state = State.TwoPoints;
        _catchLine = null;

        OnHandlesUpdated();   // show coordinates in the data reader
      }
    }

    /// <summary>
    /// Handles the mouse down event after one point is already selected.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_TwoPointsOfFour(PointD2D position, PointD2D graphXY)
    {
      var transXY = _layer.TransformCoordinatesFromRootToHere(graphXY);
      XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, transXY);
      if (scatterPoint is not null)
      {
        var plotIndex = scatterPoint.PlotIndex;
        var rowIndex = scatterPoint.RowIndex;
        // convert this layer coordinates first to PrintableAreaCoordinates
        var rootLayerCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());
        _handle[2] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
        _handle[3] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };

        var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(rootLayerCoord);
        _state = State.ThreePoints;
        _catchLine = null;

        OnHandlesUpdated();   // show coordinates in the data reader
      }
    }

    /// <summary>
    /// Handles the mouse down event after one point is already selected.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="graphXY">The graph xy.</param>
    private void OnMouseDown_ThreePointsOfFour(PointD2D position, PointD2D graphXY)
    {
      var transXY = _layer.TransformCoordinatesFromRootToHere(graphXY);
      XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, transXY);
      if (scatterPoint is not null)
      {
        var plotIndex = scatterPoint.PlotIndex;
        var rowIndex = scatterPoint.RowIndex;
        // convert this layer coordinates first to PrintableAreaCoordinates
        var rootLayerCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D());
        _handle[3] = new Handle { PlotIndex = plotIndex, RowIndex = rowIndex, Position = rootLayerCoord };
        Array.Sort(_handle, (a, b) => a.Position.X.CompareTo(b.Position.X));

        _state = State.FourPoints;
        _catchLine = null;

        OnHandlesUpdated();   // show coordinates in the data reader
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
          _handleDragState = new HandleDragState
          {
            IndexOfHandle = i,
            MouseStartCoordinates = graphXY,
            PlotIndex = _handle[i].PlotIndex,
            RowIndex = _handle[i].RowIndex,
            Position = _handle[i].Position,
          };
          break;
        }
      }
    }

    public override void OnMouseMove(PointD2D position, MouseEventArgs e)
    {
      base.OnMouseMove(position, e);
      var mouseRootCoord = _grac.ConvertMouseToRootLayerCoordinates(position);

      if (_state != State.NoPoint && _state != _finalState)
      {
        var mouseLayerCoord = _layer.TransformCoordinatesFromRootToHere(mouseRootCoord);
        if (PlotItem.GetNearestPlotPoint(_layer, mouseLayerCoord) is { } scatterPoint)
        {
          _catchLine = new CatchLine { MouseCoordinates = mouseRootCoord, ScatterPointCoordinates = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates.ToPointD2D()) };
          UpdateDataDisplayDuringDrag(scatterPoint);
          _grac.RenderOverlay();
        }
      }
      if (_handleDragState is { } dragState)
      {
        var mouseLayerCoord = _layer.TransformCoordinatesFromRootToHere(mouseRootCoord);
        XYScatterPointInformation scatterPoint = PlotItem.GetNearestPlotPoint(_layer, mouseLayerCoord);
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
            UpdateDataDisplayDuringDrag(scatterPoint);
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
        OnHandlesUpdated();   // show coordinates in the data reader
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
        return true;
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
        up = PlotItem.GetNextPlotPoint(_layer, (int)h.PlotIndex, 1);
        down = PlotItem.GetNextPlotPoint(_layer, (int)h.PlotIndex, -1);

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
          OnHandlesUpdated();   // show coordinates in the data reader
        }
      }
    }

    /// <summary>
    /// Called every time if one of the handles is updated.
    /// </summary>
    protected virtual void OnHandlesUpdated()
    {
      UpdateDataDisplay();
      _grac.RenderOverlay();
    }

    protected virtual void UpdateDataDisplayDuringDrag(XYScatterPointInformation scatterPoint)
    {
      if (PlotItem?.XYColumnPlotData?.XColumn is { } xcol &&
          PlotItem?.XYColumnPlotData?.YColumn is { } ycol &&
          scatterPoint is not null)
      {
        Current.DataDisplay.WriteOneLine($"X = {xcol[scatterPoint.RowIndex]}; Y = {ycol[scatterPoint.RowIndex]}");
      }
    }

    /// <summary>
    /// Updates the data display. In the basis version, the (x,y) positions of the handles are shown.
    /// </summary>
    protected virtual void UpdateDataDisplay()
    {
      if (_state == State.NoPoint)
      {
        Current.DataDisplay.WriteThreeLines(
          "Click on a curve to set the left outer point.",
          "",
          ""
          );
      }
      else if (_state == State.OnePoint)
      {
        Current.DataDisplay.WriteThreeLines(
          $"Click again to set the {(_finalState == State.FourPoints ? "left inner" : "right outer")} point",
          "",
          ""
          );
      }
      else if (_state == State.TwoPoints && _finalState == State.FourPoints)
      {
        Current.DataDisplay.WriteThreeLines(
          $"Click again to set the right inner point",
          "",
          ""
          );
      }
      else if (_state == State.ThreePoints && _finalState == State.FourPoints)
      {
        Current.DataDisplay.WriteThreeLines(
          $"Click again to set the right outer point",
          "",
          ""
          );
      }
      else if (_handle.Length == 2)
      {
        var (xcol, ycol, rowCount) = PlotItem.XYColumnPlotData.GetResolvedXYData();

        if (xcol is not null && ycol is not null)
        {
          Current.DataDisplay.WriteThreeLines(
            $"{_layer.Name}: {PlotItem}",
            $"XL[{_handle[0].RowIndex}]={RMath.InterpolateLinear(_handle[0].PlotIndex, xcol)}; YL[{_handle[0].RowIndex}]={RMath.InterpolateLinear(_handle[0].PlotIndex, ycol)}; XR[{_handle[1].RowIndex}]={RMath.InterpolateLinear(_handle[1].PlotIndex, xcol)}; YR[{_handle[1].RowIndex}]={RMath.InterpolateLinear(_handle[1].PlotIndex, ycol)}",
            $"DX={RMath.InterpolateLinear(_handle[1].PlotIndex, xcol) - RMath.InterpolateLinear(_handle[0].PlotIndex, xcol)}; DY={RMath.InterpolateLinear(_handle[1].PlotIndex, ycol) - RMath.InterpolateLinear(_handle[0].PlotIndex, ycol)}"
            );
        }
      }
      else if (_handle.Length == 4)
      {
        var (xcol, ycol, rowCount) = PlotItem.XYColumnPlotData.GetResolvedXYData();

        if (xcol is not null && ycol is not null)
        {
          Current.DataDisplay.WriteThreeLines(
            $"{_layer.Name}: {PlotItem}",
            $"XLO[{_handle[0].RowIndex}]={RMath.InterpolateLinear(_handle[0].PlotIndex, xcol)}; YLO[{_handle[0].RowIndex}]={RMath.InterpolateLinear(_handle[0].PlotIndex, ycol)}; XRO[{_handle[3].RowIndex}]={RMath.InterpolateLinear(_handle[3].PlotIndex, xcol)}; YRO[{_handle[3].RowIndex}]={RMath.InterpolateLinear(_handle[3].PlotIndex, ycol)}; DX={RMath.InterpolateLinear(_handle[3].PlotIndex, xcol) - RMath.InterpolateLinear(_handle[0].PlotIndex, xcol)}; DY={RMath.InterpolateLinear(_handle[3].PlotIndex, ycol) - RMath.InterpolateLinear(_handle[0].PlotIndex, ycol)}",
            $"XLI[{_handle[1].RowIndex}]={RMath.InterpolateLinear(_handle[1].PlotIndex, xcol)}; YLI[{_handle[1].RowIndex}]={RMath.InterpolateLinear(_handle[1].PlotIndex, ycol)}; XRI[{_handle[2].RowIndex}]={RMath.InterpolateLinear(_handle[2].PlotIndex, xcol)}; YRI[{_handle[2].RowIndex}]={RMath.InterpolateLinear(_handle[2].PlotIndex, ycol)}; DX={RMath.InterpolateLinear(_handle[2].PlotIndex, xcol) - RMath.InterpolateLinear(_handle[1].PlotIndex, xcol)}; DY={RMath.InterpolateLinear(_handle[2].PlotIndex, ycol) - RMath.InterpolateLinear(_handle[1].PlotIndex, ycol)}"
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
