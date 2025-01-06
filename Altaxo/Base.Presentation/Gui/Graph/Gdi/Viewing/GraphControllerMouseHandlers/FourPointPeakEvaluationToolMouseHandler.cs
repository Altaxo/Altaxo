#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Main.Properties;
using Altaxo.Main.Services;
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  public class FourPointPeakEvaluationToolMouseHandler : FourPointsOnCurveMouseHandler
  {
    public static PropertyKey<FourPointPeakEvaluationToolMouseHandlerOptions> DefaultOptionsKey = new PropertyKey<FourPointPeakEvaluationToolMouseHandlerOptions>("380D19F1-2F14-49E8-B4F6-5BFB6CA10A46", "Graph\\PeakEvaluationToolOptions", PropertyLevel.Application, null, () => new FourPointPeakEvaluationToolMouseHandlerOptions());

    protected FourPointPeakEvaluationToolMouseHandlerOptions _options;

    private double[] _xOuter = new double[100];
    private double[] _yOuter = new double[100];
    private double[] _xInner = new double[100];
    private double[] _yInner = new double[100];

    private List<(double x, double y)> _areaPointList = new List<(double x, double y)>();

    private string _errorMessage;
    private bool ShowOuterLine => _lineRegression is not null && _lineRegression.IsValid;
    private bool ShowInnerLine => _lineRegression is not null && _lineRegression.IsValid;

    private QuickLinearRegression? _lineRegression;

    public FourPointPeakEvaluationToolMouseHandler(GraphController grac)
      : base(grac, useFourHandles: true, initAllFourHandles: false)
    {
      _options = grac.Doc.GetPropertyValue(DefaultOptionsKey, () => new FourPointPeakEvaluationToolMouseHandlerOptions());

      if (_options.ShowOptionsWhenToolIsActivated)
      {
        if (Current.Gui.ShowDialog<FourPointPeakEvaluationToolMouseHandlerOptions>(ref _options, "Options for the Step Evaluation Tool", false))
        {
          Current.PropertyService.UserSettings.SetValue(DefaultOptionsKey, _options);
        }
      }
    }

    public override GraphToolType GraphToolType => GraphToolType.FourPointPeakEvaluation;

    protected override void OnHandlesUpdated()
    {
      CalculateLines();
      UpdateDataDisplay();
      _grac.RenderOverlay();
    }

    public override void AfterPaint(Graphics g)
    {
      using (var pen = new Pen(new SolidBrush(_options.LinePen.Color), (float)(2 / _grac.ZoomFactor)))
      {
        using (var brush = new SolidBrush(_options.AreaBrush.Color))
        {
          DrawLines(g, pen, brush);
        }
      }

      base.AfterPaint(g); // draw the handles
    }

    private PointF[] _linePoints = new PointF[100];
    private PointF[] _areaPoints = new PointF[100];
    private byte[] _lineTypes = new byte[100];

    private void DrawLines(Graphics g, Pen pen, Brush brush)
    {
      if (ShowOuterLine)
      {
        for (int i = 0; i < _xOuter.Length; i++)
        {
          var logicalMean = _layer.GetLogical3D(_xOuter[i], _yOuter[i]);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _linePoints[i] = new PointF((float)x, (float)y);
        }
        g.DrawLines(pen, _linePoints);
      }

      if (ShowInnerLine)
      {
        // first, show the inner line
        for (int i = 0; i < _xInner.Length; i++)
        {
          var logicalMean = _layer.GetLogical3D(_xInner[i], _yInner[i]);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _linePoints[i] = new PointF((float)x, (float)y);
        }
        g.DrawLines(pen, _linePoints);

        // then, show the area between the two lines
        if (_areaPoints.Length != _areaPointList.Count)
        {
          _areaPoints = new PointF[_areaPointList.Count];
          _lineTypes = new byte[_areaPointList.Count];
        }

        for (int i = 0; i < _areaPointList.Count; i++)
        {
          var logicalMean = _layer.GetLogical3D(_areaPointList[i].x, _areaPointList[i].y);
          var c = _layer.CoordinateSystem.LogicalToLayerCoordinates(logicalMean, out var x, out var y);
          (x, y) = _layer.TransformCoordinatesFromHereToRoot(new Altaxo.Geometry.PointD2D(x, y));
          _areaPoints[i] = new PointF((float)x, (float)y);
          _lineTypes[i] = i == 0 ? (byte)0 : (byte)1;
        }
        g.FillPath(brush, new System.Drawing.Drawing2D.GraphicsPath(_areaPoints, _lineTypes));
      }

    }
    private void CalculateLines()
    {
      _lineRegression = null;

      if (!IsReadyToBeUsed || PlotItem is null)
      {
        _errorMessage = $"Waiting for all points to be defined...";
        return;
      }

      var (xcol, ycol, rowCount) = PlotItem.XYColumnPlotData.GetResolvedXYData();

      QuickLinearRegression lineReg;

      lineReg = FourPointPeakEvaluationDataSource.GetBaselineRegression(xcol, ycol, _handle[0].PlotIndex, _handle[3].PlotIndex);

      // if either
      if (!lineReg.IsValid)
      {
        _errorMessage = $"Could not define the left line.";
        return;
      }
      else
      {
        _lineRegression = lineReg;
      }

      // outer line is going from the left outer point to the right outer point
      {
        var x0 = xcol[_handle[0].PlotIndex];
        var y0 = ycol[_handle[0].PlotIndex];
        var x1 = xcol[_handle[3].PlotIndex];
        var y1 = ycol[_handle[3].PlotIndex];

        var minx = Math.Min(x0, x1);
        var maxx = Math.Max(x0, x1);

        for (int i = 0; i < _xOuter.Length; i++)
        {
          var r = i / (_xOuter.Length - 1d);
          var x = (1 - r) * minx + r * maxx;
          _xOuter[i] = x;
          _yOuter[i] = lineReg.GetYOfX(x);
        }
      }
      // inner line is going from the left inner point to the right inner point
      {
        var x0 = xcol[_handle[1].PlotIndex];
        var y0 = ycol[_handle[1].PlotIndex];
        var x1 = xcol[_handle[2].PlotIndex];
        var y1 = ycol[_handle[2].PlotIndex];

        var minx = Math.Min(x0, x1);
        var maxx = Math.Max(x0, x1);

        for (int i = 0; i < _xInner.Length; i++)
        {
          var r = i / (_xInner.Length - 1d);
          var x = (1 - r) * minx + r * maxx;

          _xInner[i] = x;
          _yInner[i] = lineReg.GetYOfX(x);
        }
      }

      // the area is defined by the inner line and the curve
      {
        var startIdx = _handle[1].PlotIndex;
        var endIdx = _handle[2].PlotIndex;
        if (startIdx > endIdx)
          (startIdx, endIdx) = (endIdx, startIdx);
        _areaPointList.Clear();
        for (int i = startIdx; i <= endIdx; i++)
        {
          _areaPointList.Add((xcol[i], ycol[i]));
        }
        for (int i = endIdx; i >= startIdx; i--)
        {
          _areaPointList.Add((xcol[i], lineReg.GetYOfX(xcol[i])));
        }
      }
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
      if (PlotItem?.XYColumnPlotData?.XColumn is not { } xcol || PlotItem?.XYColumnPlotData?.YColumn is not { } ycol)
      {
        return;
      }

      var sourceTable = Altaxo.Data.DataTable.GetParentDataTableOf(xcol.GetUnderlyingDataColumnOrDefault());

      var xname = xcol.GetUnderlyingDataColumnOrDefault()?.Name ?? "X";
      var yname = ycol.GetUnderlyingDataColumnOrDefault()?.Name ?? "Y";
      var newTableName = sourceTable.Name + $"_PeakEvaluation_{yname}_Vs_{xname}";
      var newTable = Current.Project.DataTableCollection.EnsureExistence(newTableName);

      var dataSourceOptions = new FourPointPeakEvaluationOptions()
      {
        IndexLeftOuter = _handle[0].PlotIndex,
        IndexLeftInner = _handle[1].PlotIndex,
        IndexRightInner = _handle[2].PlotIndex,
        IndexRightOuter = _handle[3].PlotIndex,
      };

      newTable.DataSource = new FourPointPeakEvaluationDataSource(new XAndYColumn(PlotItem.Data), dataSourceOptions, new DataSourceImportOptions());

      newTable.DataSource.FillData(newTable, DummyProgressReporter.Instance);

      // now add the curves to the plot

      var plotCollection = (PlotItemCollection)TreeNodeExtensions.RootNode<IGPlotItem>(PlotItem);

      var newCollection = new PlotItemCollection();
      plotCollection.Add(newCollection);

      var linePlotStyleTemplate = new LinePlotStyle(_grac.Doc.GetPropertyContext()) { IndependentLineColor = true };
      linePlotStyleTemplate.LinePen = _options.LinePen.WithWidth(linePlotStyleTemplate.LinePen.Width);


      var newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointPeakEvaluationDataSource.ColumnGroupNumberLine, newTable[FourPointPeakEvaluationDataSource.ColumnNameLineX], newTable[FourPointPeakEvaluationDataSource.ColumnNameLineY]),
                          new G2DPlotStyleCollection(new[]
                          {
                              (LinePlotStyle)linePlotStyleTemplate.Clone(),
                          }));

      newCollection.Add(newPlotItem);

      newPlotItem = new XYColumnPlotItem(
                    new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointPeakEvaluationDataSource.ColumnGroupNumberInnerLine, newTable[FourPointPeakEvaluationDataSource.ColumnNameInnerLineX], newTable[FourPointPeakEvaluationDataSource.ColumnNameInnerLineY]),
                    new G2DPlotStyleCollection(new[]
                    {
                               (LinePlotStyle)linePlotStyleTemplate.Clone(),
                    }));

      newCollection.Add(newPlotItem);


      /*

      newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointPeakEvaluationDataSource.ColumnGroupNumberMiddle, newTable[FourPointPeakEvaluationDataSource.ColumnNameMiddleX], newTable[FourPointPeakEvaluationDataSource.ColumnNameMiddleY])
                          {
                            DataRowSelection = RangeOfRowIndices.FromStartAndCount(2, 1)
                          },
                          new G2DPlotStyleCollection(new IG2DPlotStyle[]
                          {
                              new ScatterPlotStyle(_grac.Doc.GetPropertyContext()) { Color = Altaxo.Drawing.NamedColors.Black },
                              new LabelPlotStyle(newTable[FourPointPeakEvaluationDataSource.ColumnNameMiddleX], _grac.Doc.GetPropertyContext())
                              {
                                 AlignmentX = Altaxo.Drawing.Alignment.Near,
                                 AlignmentY = Altaxo.Drawing.Alignment.Far,
                                 LabelFormatString = "x = {0:G5}",
                                 OffsetXEmUnits = 0.5,
                              },
                              new LabelPlotStyle(newTable[FourPointPeakvaluationDataSource.ColumnNameMiddleY], _grac.Doc.GetPropertyContext())
                              {
                                 AlignmentX = Altaxo.Drawing.Alignment.Near,
                                 AlignmentY = Altaxo.Drawing.Alignment.Near,
                                 LabelFormatString = "y = {0:G5}",
                                 OffsetXEmUnits = 0.5,
                              },
                          }));

      newCollection.Add(newPlotItem);
      */
    }
  }
}
