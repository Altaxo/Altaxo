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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using Altaxo.Data;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the read coordinate tools is selected.
  /// </summary>
  public class ReadXYCoordinatesMouseHandler : MouseStateHandler
  {
    /// <summary>
    /// Coordinates of the red data reader cross (in printable coordinates)
    /// </summary>
    protected PointD2D _positionOfCrossInRootLayerCoordinates;

    /// <summary>
    /// The parent graph controller.
    /// </summary>
    protected GraphController _grac;

    protected double _MovementIncrement = 4;

    /// <summary>
    /// If true, the tool show the printable coordinates instead of the physical values.
    /// </summary>
    protected bool _showRootLayerPrintCoordinates;

    public ReadXYCoordinatesMouseHandler(GraphController grac)
    {
      _grac = grac;

      if (_grac != null)
        _grac.SetPanelCursor(Cursors.Cross);
    }

    public override GraphToolType GraphToolType
    {
      get { return GraphToolType.ReadXYCoordinates; }
    }

    /// <summary>
    /// Handles the MouseDown event when the plot point tool is selected
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">The mouse event args</param>
    public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseDown(position, e);

      if (null != _cachedActiveLayer && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) // if M is pressed, we don't move the cross, but instead we display not only the cross coordinates but also the difference
      {
        var printableCoord = _grac.ConvertMouseToRootLayerCoordinates(position);
        DisplayCrossCoordinatesAndDifference(printableCoord);
      }
      else
      {
        _cachedActiveLayer = _grac.ActiveLayer;
        _cachedActiveLayerTransformation = _cachedActiveLayer.TransformationFromRootToHere();
        _cachedActiveLayerTransformationGdi = _cachedActiveLayerTransformation;

        _positionOfCrossInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);
        DisplayCrossCoordinates();
      }

      _grac.RenderOverlay(); // no refresh necessary, only invalidate to show the cross
    } // end of function

    private bool CalculateCrossCoordinates(PointD2D crossRootLayerCoord, out Altaxo.Data.AltaxoVariant x, out Altaxo.Data.AltaxoVariant y)
    {
      var layer = _cachedActiveLayer as XYPlotLayer;
      if (layer == null)
      {
        x = new AltaxoVariant();
        y = new AltaxoVariant();
        return false;
      }

      var layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(crossRootLayerCoord);

      layer.CoordinateSystem.LayerToLogicalCoordinates(layerCoord.X, layerCoord.Y, out var r);
      x = layer.XAxis.NormalToPhysicalVariant(r.RX);
      y = layer.YAxis.NormalToPhysicalVariant(r.RY);
      return true;
    }

    private void DisplayCrossCoordinates()
    {
      if (_showRootLayerPrintCoordinates)
      {
        var crossLayerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(_positionOfCrossInRootLayerCoordinates);
        Current.DataDisplay.WriteOneLine(string.Format(
        "Layer({0}) XS={1} pt, YS={2} pt",
        _cachedActiveLayer.Name,
        crossLayerCoord.X,
        crossLayerCoord.Y));
      }
      else
      {
        if (CalculateCrossCoordinates(_positionOfCrossInRootLayerCoordinates, out var xphys, out var yphys))
          Current.DataDisplay.WriteOneLine(string.Format(
         "Layer({0}) X={1}, Y={2}",
         _cachedActiveLayer.Name,
         xphys,
         yphys));
      }
    }

    private void DisplayCrossCoordinatesAndDifference(PointD2D rootLayerCoord)
    {
      if (_showRootLayerPrintCoordinates)
      {
        var crossLayerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(_positionOfCrossInRootLayerCoordinates);
        var layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(rootLayerCoord);
        Current.DataDisplay.WriteTwoLines(
          string.Format("Layer({0}) XS={1} pt, YS={2} pt", _cachedActiveLayer.Name, crossLayerCoord.X, crossLayerCoord.Y),
          string.Format("DeltaXS={0} pt, DeltaYS={1} pt, Distance={2} pt", layerCoord.X - crossLayerCoord.X, layerCoord.Y - crossLayerCoord.Y, Calc.RMath.Hypot(layerCoord.X - crossLayerCoord.X, layerCoord.Y - crossLayerCoord.Y))
          );
      }
      else
      {
        if (CalculateCrossCoordinates(_positionOfCrossInRootLayerCoordinates, out var xphys, out var yphys) && CalculateCrossCoordinates(rootLayerCoord, out var xphys2, out var yphys2))
        {
          double distance = double.NaN;
          AltaxoVariant dx = double.NaN, dy = double.NaN;
          try
          {
            dx = xphys2 - xphys;
            dy = yphys2 - yphys;
            var r2 = dx * dx + dy * dy;
            distance = Math.Sqrt(r2);
          }
          catch (Exception)
          {
          }

          Current.DataDisplay.WriteTwoLines(
            string.Format("Layer({0}) X={1}, Y={2}", _cachedActiveLayer.Name, xphys, yphys),
            string.Format("DeltaX={0}, DeltaY={1}, Distance={2}", dx, dy, distance)
            );
        }
      }
    }

    /// <summary>
    /// Moves the cross along the plot.
    /// </summary>
    /// <param name="increment"></param>
    private void MoveLeftRight(double increment)
    {
      _positionOfCrossInRootLayerCoordinates = _positionOfCrossInRootLayerCoordinates.WithXPlus(increment);

      DisplayCrossCoordinates();

      _grac.RenderOverlay(); // no refresh necessary, only invalidate to show the cross
    }

    /// <summary>
    /// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
    /// </summary>
    /// <param name="increment"></param>
    private void MoveUpDown(double increment)
    {
      _positionOfCrossInRootLayerCoordinates = _positionOfCrossInRootLayerCoordinates.WithYPlus(increment);

      DisplayCrossCoordinates();

      _grac.RenderOverlay(); // no refresh necessary, only invalidate to show the cross
    }

    public override void AfterPaint(Graphics g)
    {
      base.AfterPaint(g);
      // draw a red cross onto the selected data point

      // draw a red cross onto the selected data point
      double startLine = 1 / _grac.ZoomFactor;
      double endLine = 10 / _grac.ZoomFactor;
      using (var brush = new HatchBrush(HatchStyle.Percent50, Color.Red, Color.Yellow))
      {
        using (var pen = new Pen(brush, (float)(2 / _grac.ZoomFactor)))
        {
          g.DrawLine(pen, (float)(_positionOfCrossInRootLayerCoordinates.X + startLine), (float)_positionOfCrossInRootLayerCoordinates.Y, (float)(_positionOfCrossInRootLayerCoordinates.X + endLine), (float)_positionOfCrossInRootLayerCoordinates.Y);
          g.DrawLine(pen, (float)(_positionOfCrossInRootLayerCoordinates.X - startLine), (float)_positionOfCrossInRootLayerCoordinates.Y, (float)(_positionOfCrossInRootLayerCoordinates.X - endLine), (float)_positionOfCrossInRootLayerCoordinates.Y);
          g.DrawLine(pen, (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y + startLine), (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y + endLine));
          g.DrawLine(pen, (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y - startLine), (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y - endLine));
        }
      }
    }

    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="e">Key event arguments.</param>
    /// <returns></returns>
    public override bool ProcessCmdKey(KeyEventArgs e)
    {
      var keyData = e.Key;
      if (keyData == Key.Left)
      {
        MoveLeftRight(-_MovementIncrement);
        return true;
      }
      else if (keyData == Key.Right)
      {
        MoveLeftRight(_MovementIncrement);
        return true;
      }
      else if (keyData == Key.Up)
      {
        MoveUpDown(-_MovementIncrement);
        return true;
      }
      else if (keyData == Key.Down)
      {
        MoveUpDown(_MovementIncrement);
        return true;
      }
      else if (keyData == Key.Add || keyData == Key.OemPlus)
      {
        if (_MovementIncrement < 1024)
          _MovementIncrement *= 2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}", _MovementIncrement));
        return true;
      }
      else if (keyData == Key.Subtract || keyData == Key.OemMinus)
      {
        if (_MovementIncrement >= (1 / 1024.0))
          _MovementIncrement /= 2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}", _MovementIncrement));

        return true;
      }
      else if (keyData == Key.Q)
      {
        _showRootLayerPrintCoordinates = !_showRootLayerPrintCoordinates;
        Current.DataDisplay.WriteOneLine("Switched to " + (_showRootLayerPrintCoordinates ? "print coordinates (pt)!" : "physical values!"));
      }
      else if (keyData == Key.Enter)
      {
        if (_showRootLayerPrintCoordinates)
        {
          Current.Console.WriteLine("{0}\t{1}", _positionOfCrossInRootLayerCoordinates.X, _positionOfCrossInRootLayerCoordinates.Y);
        }
        else
        {
          if (CalculateCrossCoordinates(_positionOfCrossInRootLayerCoordinates, out var xphys, out var yphys))
            Current.Console.WriteLine("{0}\t{1}", xphys, yphys);
        }
        return true;
      }

      return false; // per default the key is not processed
    }
  } // end of class
}
