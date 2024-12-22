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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Main.Services;
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  public class StepEvaluationToolMouseHandler : FourPointsOnCurveMouseHandler
  {
    private double _yLowerStepValue = 0.2;
    private double _yUpperStepValue = 0.8;

    private double[] _xleft = new double[100];
    private double[] _yleft = new double[100];
    private double[] _xright = new double[100];
    private double[] _yright = new double[100];
    private double[] _xmiddle = new double[100];
    private double[] _ymiddle = new double[100];
    private string _errorMessage;
    private bool _showLeftLine => _leftReg is not null && _leftReg.IsValid;
    private bool _showRightLine => _rightReg is not null && _rightReg.IsValid;
    private bool _showMiddleLine => _middleReg is not null && _middleReg.IsValid;

    private (double x, double y)? _middleCrossXY;
    private bool _showMiddleCross => _middleCrossXY.HasValue;

    private QuickLinearRegression? _leftReg, _rightReg, _middleReg;

    public StepEvaluationToolMouseHandler(GraphController grac)
      : base(grac)
    {
    }

    public override GraphToolType GraphToolType => GraphToolType.FourPointStepEvaluation;

    protected override void OnHandlesUpdated()
    {
      CalculateLines();
      UpdateDataDisplay();
      _grac.RenderOverlay();
    }

    public override void AfterPaint(Graphics g)
    {
      using (var pen = new Pen(new SolidBrush(Color.LightBlue), (float)(2 / _grac.ZoomFactor)))
      {
        DrawLines(g, pen);
      }

      base.AfterPaint(g); // draw the handles
    }

    private PointF[] _linePoints = new PointF[100];

    private void DrawLines(Graphics g, Pen pen)
    {
      if (_showLeftLine)
      {
        for (int i = 0; i < _xleft.Length; i++)
        {
          var logicalMean = _layer.GetLogical3D(_xleft[i], _yleft[i]);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _linePoints[i] = new PointF((float)x, (float)y);
        }
        g.DrawLines(pen, _linePoints);
      }

      if (_showRightLine)
      {
        for (int i = 0; i < _xright.Length; i++)
        {
          var logicalMean = _layer.GetLogical3D(_xright[i], _yright[i]);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _linePoints[i] = new PointF((float)x, (float)y);
        }
        g.DrawLines(pen, _linePoints);
      }

      if (_showMiddleLine)
      {
        for (int i = 0; i < _xmiddle.Length; i++)
        {
          var logicalMean = _layer.GetLogical3D(_xmiddle[i], _ymiddle[i]);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _linePoints[i] = new PointF((float)x, (float)y);
        }
        g.DrawLines(pen, _linePoints);
      }
    }
    private void CalculateLines()
    {
      _leftReg = _rightReg = _middleReg = null;
      _middleCrossXY = null;

      if (!IsReadyToBeUsed || PlotItem is null)
      {
        _errorMessage = $"Waiting for all points to be defined...";
        return;
      }

      var xcol = PlotItem.XYColumnPlotData.XColumn;
      var ycol = PlotItem.XYColumnPlotData.YColumn;


      QuickLinearRegression leftReg, rightReg, middleReg;

      // left line is going from the left outer point through the left inner point to x if the right inner point
      {
        var x0 = xcol[_handle[0].RowIndex];
        var y0 = ycol[_handle[0].RowIndex];
        var x1 = xcol[_handle[1].RowIndex];
        var y1 = ycol[_handle[1].RowIndex];
        var x2 = xcol[_handle[2].RowIndex];

        var minx = Math.Min(x0, Math.Min(x1, x2));
        var maxx = Math.Max(x0, Math.Max(x1, x2));

        leftReg = new QuickLinearRegression();
        leftReg.Add(x0, y0);
        leftReg.Add(x1, y1);

        for (int i = 0; i < _xleft.Length; i++)
        {
          var r = i / (_xleft.Length - 1d);
          var x = (1 - r) * minx + r * maxx;
          _xleft[i] = x;
          _yleft[i] = leftReg.GetYOfX(x);
        }
      }
      // right line is going from the right outer point through the right inner point to the x of the left inner point
      {
        var x0 = xcol[_handle[3].RowIndex];
        var y0 = ycol[_handle[3].RowIndex];
        var x1 = xcol[_handle[2].RowIndex];
        var y1 = ycol[_handle[2].RowIndex];
        var x2 = xcol[_handle[1].RowIndex];

        var minx = Math.Min(x0, Math.Min(x1, x2));
        var maxx = Math.Max(x0, Math.Max(x1, x2));

        rightReg = new QuickLinearRegression();
        rightReg.Add(x0, y0);
        rightReg.Add(x1, y1);

        for (int i = 0; i < _xleft.Length; i++)
        {
          var r = i / (_xleft.Length - 1d);
          var x = (1 - r) * minx + r * maxx;

          _xright[i] = x;
          _yright[i] = rightReg.GetYOfX(x);
        }
      }

      // if either
      if (!leftReg.IsValid)
      {
        _errorMessage = $"Could not define the left line.";
        return;
      }
      else
      {
        _leftReg = leftReg;
      }

      if (!rightReg.IsValid)
      {
        _errorMessage = $"Could not define the right line.";
        return;
      }
      else
      {
        _rightReg = rightReg;
      }

      // now the middle line
      var (xs, ys) = leftReg.GetIntersectionPoint(rightReg);
      if (RMath.IsInIntervalCC(xs, Math.Min(xcol[_handle[1].RowIndex], xcol[_handle[2].RowIndex]), Math.Max(xcol[_handle[1].RowIndex], xcol[_handle[2].RowIndex])))
      {
        _errorMessage = $"The left and the right line intersect in the middle zone, thus evaluation is not possible";
        return;
      }


      var plotIndexList = new List<Handle>([_handle[1], _handle[2]]);
      plotIndexList.Sort((x, y) => Comparer<int>.Default.Compare(x.PlotIndex, y.PlotIndex));

      var plotPoint = PlotItem.GetNextPlotPoint(_layer, plotIndexList[0].PlotIndex, 0);
      middleReg = new QuickLinearRegression();
      for (; plotPoint is { } p && p.PlotIndex <= plotIndexList[1].PlotIndex; plotPoint = PlotItem.GetNextPlotPoint(_layer, plotPoint.PlotIndex, 1))
      {
        var x = xcol[plotPoint.RowIndex];
        var y = ycol[plotPoint.RowIndex];
        var r = QuickLinearRegression.GetRelativeYBetweenRegressions(leftReg, rightReg, x, y);
        if (RMath.IsInIntervalCC(r, 0.2, 0.8))
        {
          middleReg.Add(x, y);
        }
      }

      if (!middleReg.IsValid)
      {
        _errorMessage = $"The middle line regression is not valid";
        return;
      }
      else
      {
        // calculate 100 points of the middle line.

        for (int i = 0; i < _xmiddle.Length; i++)
        {
          var r = i / (_xmiddle.Length - 1d);
          var x = (1 - r) * xcol[_handle[1].RowIndex] + r * xcol[_handle[2].RowIndex];
          _xmiddle[i] = x;
          _ymiddle[i] = middleReg.GetYOfX(x);
        }

        _middleReg = middleReg;
      }

      // now find the point on the middle regression line where the relative y between left and right regression is 0.5

      var xmiddle = (leftReg.GetA0() + rightReg.GetA0() - 2 * middleReg.GetA0()) / (2 * middleReg.GetA1() - leftReg.GetA1() - rightReg.GetA1());

      if (!RMath.IsFinite(xmiddle))
      {
        _errorMessage = $"The regression of the middle line is not valid";
        return;
      }
      // we are through!
      var ymiddle = middleReg.GetYOfX(xmiddle);
      _middleCrossXY = (xmiddle, ymiddle);
      _errorMessage = $"XM = {xmiddle}; YM = {ymiddle}";
    }

    protected override void UpdateDataDisplay()
    {
      Current.DataDisplay.WriteThreeLines(
          _errorMessage,
          "",
          ""
          );
    }

    public override bool ProcessCmdKey(KeyEventArgs e)
    {
      if (base.ProcessCmdKey(e))
        return true;

      if (e.Key == Key.Enter)
      {
        MakeEvaluationPermanent();
        _grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);
        return true;
      }

      return false;
    }

    public virtual void MakeEvaluationPermanent()
    {
      if (_showMiddleCross == false)
      {
        return;
      }

      if (PlotItem?.XYColumnPlotData?.XColumn is not { } xcol || PlotItem?.XYColumnPlotData?.YColumn is not { } ycol)
      {
        return;
      }

      var sourceTable = Altaxo.Data.DataTable.GetParentDataTableOf(xcol.GetUnderlyingDataColumnOrDefault());

      var xname = xcol.GetUnderlyingDataColumnOrDefault()?.Name ?? "X";
      var yname = ycol.GetUnderlyingDataColumnOrDefault()?.Name ?? "Y";
      var newTableName = sourceTable.Name + $"_StepEvaluation_{yname}_Vs_{xname}";
      var newTable = Current.Project.DataTableCollection.EnsureExistence(newTableName);

      var dataSourceOptions = new FourPointStepEvaluationOptions()
      {
        IndexLeftOuter = _handle[0].PlotIndex,
        IndexLeftInner = _handle[1].PlotIndex,
        IndexRightInner = _handle[2].PlotIndex,
        IndexRightOuter = _handle[3].PlotIndex,
        UseRegressionForLeftAndRightLine = true,
        MiddleRegressionLevels = (0.25, 0.75),
        MiddleLineOverlap = 0.1,
      };

      newTable.DataSource = new FourPointStepEvaluationDataSource(new XAndYColumn(PlotItem.Data), dataSourceOptions, new DataSourceImportOptions());

      newTable.DataSource.FillData(newTable, DummyProgressReporter.Instance);

      // now add the curves to the plot

      var plotCollection = (PlotItemCollection)TreeNodeExtensions.RootNode<IGPlotItem>(PlotItem);

      var newCollection = new PlotItemCollection();
      plotCollection.Add(newCollection);

      var newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberLeft, newTable[FourPointStepEvaluationDataSource.ColumnNameLeftX], newTable[FourPointStepEvaluationDataSource.ColumnNameLeftY]),
                          new G2DPlotStyleCollection(new[]
                          {
                              new LinePlotStyle(_grac.Doc.GetPropertyContext()) { Color = Altaxo.Drawing.NamedColors.Blue },
                          }));

      newCollection.Add(newPlotItem);

      newPlotItem = new XYColumnPlotItem(
                    new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberRight, newTable[FourPointStepEvaluationDataSource.ColumnNameRightX], newTable[FourPointStepEvaluationDataSource.ColumnNameRightY]),
                    new G2DPlotStyleCollection(new[]
                    {
                              new LinePlotStyle(_grac.Doc.GetPropertyContext()) { Color = Altaxo.Drawing.NamedColors.Blue },
                    }));

      newCollection.Add(newPlotItem);


      newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberMiddle, newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleX], newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleY]),
                          new G2DPlotStyleCollection(new[]
                          {
                              new LinePlotStyle(_grac.Doc.GetPropertyContext()) { Color = Altaxo.Drawing.NamedColors.Blue },
                          }));

      newCollection.Add(newPlotItem);

      newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberMiddle, newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleX], newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleY])
                          {
                            DataRowSelection = RangeOfRowIndices.FromStartAndCount(2, 1)
                          },
                          new G2DPlotStyleCollection(new IG2DPlotStyle[]
                          {
                              new ScatterPlotStyle(_grac.Doc.GetPropertyContext()) { Color = Altaxo.Drawing.NamedColors.Black },
                              new LabelPlotStyle(newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleX], _grac.Doc.GetPropertyContext())
                              {
                                 AlignmentX = Altaxo.Drawing.Alignment.Near,
                                 AlignmentY = Altaxo.Drawing.Alignment.Far,
                                 LabelFormatString = "x = {0:G5}",
                                 OffsetXEmUnits = 0.5,
                              },
                              new LabelPlotStyle(newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleY], _grac.Doc.GetPropertyContext())
                              {
                                 AlignmentX = Altaxo.Drawing.Alignment.Near,
                                 AlignmentY = Altaxo.Drawing.Alignment.Near,
                                 LabelFormatString = "y = {0:G5}",
                                 OffsetXEmUnits = 0.5,
                              },
                          }));

      newCollection.Add(newPlotItem);
    }
  }
}
