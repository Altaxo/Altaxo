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
using System.Linq;
using System.Windows.Input;
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Common;
using Altaxo.Main.Properties;
using Altaxo.Main.Services;
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
  public class FourPointStepEvaluationToolMouseHandler : FourPointsOnCurveMouseHandler
  {
    public static PropertyKey<FourPointStepEvaluationToolMouseHandlerOptions> DefaultOptionsKey = new PropertyKey<FourPointStepEvaluationToolMouseHandlerOptions>("F83DAFE6-E522-4930-BA8B-6DC1742DD85C", "Graph\\StepEvaluationToolOptions", PropertyLevel.Application, null, () => new FourPointStepEvaluationToolMouseHandlerOptions());

    protected FourPointStepEvaluationToolMouseHandlerOptions _options;

    private double[] _xleft = new double[100];
    private double[] _yleft = new double[100];
    private double[] _xright = new double[100];
    private double[] _yright = new double[100];
    private double[] _xmiddle = new double[100];
    private double[] _ymiddle = new double[100];
    private string? _errorMessage;
    private bool _showLeftLine => _leftReg is not null && _leftReg.IsValid;
    private bool _showRightLine => _rightReg is not null && _rightReg.IsValid;
    private bool _showMiddleLine => _middleReg is not null && _middleReg.IsValid;

    private (double x, double y)? _middleCrossXY;
    private bool _showMiddleCross => _middleCrossXY.HasValue;

    private QuickLinearRegression? _leftReg, _rightReg, _middleReg;


    private bool _isEvaluationSaved;

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluationToolMouseHandler"/> class.
    /// </summary>
    /// <param name="grac">The graph controller.</param>
    public FourPointStepEvaluationToolMouseHandler(GraphController grac)
      : base(grac, useFourHandles: true, initAllFourHandles: true)
    {
      _options = grac.Doc.GetPropertyValue(DefaultOptionsKey, () => new FourPointStepEvaluationToolMouseHandlerOptions());

      if (_options.ShowOptionsWhenToolIsActivated)
      {
        if (Current.Gui.ShowDialog<FourPointStepEvaluationToolMouseHandlerOptions>(ref _options, "Options for the Step Evaluation Tool", false))
        {
          Current.PropertyService.UserSettings.SetValue(DefaultOptionsKey, _options);
        }
      }
      Initialize(grac);
    }

    /// <summary>
    /// Initializes the step evaluation tool. This function searches for existing step evaluations in the graph
    /// and lets the user choose one of them to edit, or to create a new one.
    /// </summary>
    /// <param name="grac">The graph controller.</param>
    public void Initialize(GraphController grac)
    {
      var graph = grac.Doc;


      // First step: Find plotItems, which comes from a table that contains a FourPointStepEvaluationDataSource
      // then store the x-y source together with the evaluation table
      var stepDataSources = new Dictionary<XAndYColumn, DataTable>();
      foreach (var layer in graph.RootLayer.EnumerateFromHereToLeaves().OfType<XYPlotLayer>())
      {
        foreach (var plotItem in layer.PlotItems.EnumerateFromHereToLeaves().OfType<XYColumnPlotItem>())
        {
          if (plotItem?.XYColumnPlotData?.DataTable?.DataSource is FourPointStepEvaluationDataSource ds && ds.ProcessData is { } xyCol)
          {
            stepDataSources[xyCol] = plotItem.XYColumnPlotData.DataTable;
          }
        }
      }

      // now find all plot items here whose data is a x-y-data like that in stepDataSources
      // then store this item together with the evaluation table
      var stepDataPlotItems = new Dictionary<XYColumnPlotItem, DataTable>();
      foreach (var layer in graph.RootLayer.EnumerateFromHereToLeaves().OfType<XYPlotLayer>())
      {
        foreach (var plotItem in layer.PlotItems.EnumerateFromHereToLeaves().OfType<XYColumnPlotItem>())
        {
          if (plotItem.XYColumnPlotData is { } pd && stepDataSources.TryGetValue(pd, out var evaluationTable))
            stepDataPlotItems[plotItem] = evaluationTable;
        }
      }


      // now let the user choose which item he/she wants to edit
      if (stepDataPlotItems.Count >= 1)
      {
        var list = new List<string>();
        list.Add("New evaluation");
        list.AddRange(stepDataPlotItems.Select(kv => $"{kv.Key.GetName(2)} -> {kv.Value.Name}"));
        var controller = new SingleChoiceController(list.ToArray(), 0);
        controller.DescriptionText =
          $"There already exist {stepDataPlotItems.Count} step evaluations\r\n" +
          $"If you want to reuse an existing item, choose that;\r\n" +
          $"otherwise, choose the option to create a new evaluation\r\n";

        if (Current.Gui.ShowDialog(controller, "Choose new or existing evaluation table"))
        {
          var selectionIndex = (int)controller.ModelObject;
          if (selectionIndex > 0)
          {
            var selected = stepDataPlotItems.Skip(selectionIndex - 1).First();
            OnPlotItemSet(selected.Key, selected.Value);
          }
        }
      }
    }

    public override GraphToolType GraphToolType => GraphToolType.FourPointStepEvaluation;

    protected override void OnPlotItemSet(XYColumnPlotItem plotItem)
    {
      const string NameOfNewTableEntry = "New evaluation table";

      base.OnPlotItemSet(plotItem);

      var tableList = GetExistingDestinationTables(plotItem.XYColumnPlotData);

      DataTable? selectedTable = null;

      if (tableList.Count == 1)
      {
        if (Current.Gui.YesNoMessageBox(
          $"There is already an evaluation in table {tableList[0].Name}\r\n" +
          $"Do you want to use and overwrite the existing evaluation?\r\n" +
          $"'Yes' to use and overwrite the existing evaluation\r\n" +
          "'No' to save the results in a new evaluation table",
          "Use existing evaluation?",
          true)
          )
        {
          selectedTable = tableList[0];
        }
      }
      else if (tableList.Count > 1)
      {
        var list = new List<string>();
        list.Add(NameOfNewTableEntry);
        list.AddRange(tableList.Select(t => t.ShortName));
        var controller = new SingleChoiceController(list.ToArray(), 0);
        controller.DescriptionText =
          $"There already exist {tableList.Count} evaluation tables\r\n" +
          $"If you want to use and overwrite an existing table, choose that\r\n" +
          $"table from the list; otherwise, choose the option to create\r\n" +
          "a new evaluation table.";

        if (Current.Gui.ShowDialog(controller, "Choose new or existing evaluation table"))
        {
          var selection = list[(int)controller.ModelObject];
          selectedTable = tableList.FirstOrDefault(t => t.Name == selection);
        }
      }

      if (selectedTable is not null)
      {
        OnPlotItemSet(plotItem, selectedTable);
      }
    }

    protected override void OnPlotItemSet(XYColumnPlotItem plotItem, DataTable existingDestinationTable)
    {
      base.OnPlotItemSet(plotItem, existingDestinationTable);

      var ds = (FourPointStepEvaluationDataSource)existingDestinationTable.DataSource;
      var maxPlotIndex = plotItem.XYColumnPlotData.GetCommonRowCountFromDataColumns() - 1;
      _handle[0].PlotIndex = Math.Min(ds.ProcessOptions.IndexLeftOuter, maxPlotIndex);
      _handle[1].PlotIndex = Math.Min(ds.ProcessOptions.IndexLeftInner, maxPlotIndex);
      _handle[2].PlotIndex = Math.Min(ds.ProcessOptions.IndexRightInner, maxPlotIndex);
      _handle[3].PlotIndex = Math.Min(ds.ProcessOptions.IndexRightOuter, maxPlotIndex);

      for (int i = 0; i < _handle.Length; ++i)
      {
        (_handle[i].Position, _handle[i].RowIndex) = plotItem.GetPlotPointAt(_handle[i].PlotIndex, this._layer).Value;
        _handle[i].Position = _layer.TransformCoordinatesFromHereToRoot(_handle[i].Position);
      }
      _state = _finalState;

      OnHandlesUpdated();
    }

    public override void OnLeaveTool(MouseStateHandler newTool)
    {
      base.OnLeaveTool(newTool);

      if (!_isEvaluationSaved && IsReadyToBeUsed)
      {
        if (true == Current.Gui.YesNoMessageBox("Your evaluation is not yet saved into an evaluation table\r\nDo you want to save it before leaving this tool?", "Save?", true))
          MakeEvaluationPermanent();
      }
    }

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

      var (xcol, ycol, rowCount) = PlotItem.XYColumnPlotData.GetResolvedXYData();

      QuickLinearRegression leftReg, rightReg, middleReg;

      leftReg = FourPointStepEvaluation.GetLeftRightRegression(xcol, ycol, _handle[0].PlotIndex, _handle[1].PlotIndex, _options.UseRegressionForLeftAndRightLine);

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

      rightReg = FourPointStepEvaluation.GetLeftRightRegression(xcol, ycol, _handle[2].PlotIndex, _handle[3].PlotIndex, _options.UseRegressionForLeftAndRightLine);

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
      if (RMath.IsInIntervalCC(xs, Math.Min(RMath.InterpolateLinear(_handle[1].PlotIndex, xcol), RMath.InterpolateLinear(_handle[2].PlotIndex, xcol)), Math.Max(RMath.InterpolateLinear(_handle[1].PlotIndex, xcol), RMath.InterpolateLinear(_handle[2].PlotIndex, xcol))))
      {
        _errorMessage = $"The left and the right line intersect in the middle zone, thus evaluation is not possible";
        return;
      }


      // left line is going from the left outer point through the left inner point to x if the right inner point
      {
        var x0 = RMath.InterpolateLinear(_handle[0].PlotIndex, xcol);
        var y0 = RMath.InterpolateLinear(_handle[0].PlotIndex, ycol);
        var x1 = RMath.InterpolateLinear(_handle[1].PlotIndex, xcol);
        var y1 = RMath.InterpolateLinear(_handle[1].PlotIndex, ycol);
        var x2 = RMath.InterpolateLinear(_handle[2].PlotIndex, xcol);

        var minx = Math.Min(x0, Math.Min(x1, x2));
        var maxx = Math.Max(x0, Math.Max(x1, x2));

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
        var x0 = RMath.InterpolateLinear(_handle[3].PlotIndex, xcol);
        var y0 = RMath.InterpolateLinear(_handle[3].PlotIndex, ycol);
        var x1 = RMath.InterpolateLinear(_handle[2].PlotIndex, xcol);
        var y1 = RMath.InterpolateLinear(_handle[2].PlotIndex, ycol);
        var x2 = RMath.InterpolateLinear(_handle[1].PlotIndex, xcol);

        var minx = Math.Min(x0, Math.Min(x1, x2));
        var maxx = Math.Max(x0, Math.Max(x1, x2));



        for (int i = 0; i < _xleft.Length; i++)
        {
          var r = i / (_xleft.Length - 1d);
          var x = (1 - r) * minx + r * maxx;

          _xright[i] = x;
          _yright[i] = rightReg.GetYOfX(x);
        }
      }


      // now get the middle regression line

      middleReg = FourPointStepEvaluation.GetMiddleRegression(xcol, ycol, _handle[1].PlotIndex, _handle[2].PlotIndex, leftReg, rightReg, _options.MiddleRegressionLevels.LowerLevel, _options.MiddleRegressionLevels.UpperLevel);

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
          var x = (1 - r) * RMath.InterpolateLinear(_handle[1].PlotIndex, xcol) + r * RMath.InterpolateLinear(_handle[2].PlotIndex, xcol);
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
      else if (e.Key == Key.R)
      {
        _options = _options with { UseRegressionForLeftAndRightLine = !_options.UseRegressionForLeftAndRightLine };
        Current.Gui.InfoMessageBox($"Full regression of the left and right line is now switched {(_options.UseRegressionForLeftAndRightLine ? "on" : "off")}.", "Regression option");
        CalculateLines();
        UpdateDataDisplay();
        _grac.RenderOverlay();
      }

      return false;
    }


    /// <summary>
    /// Gets the existing destination tables. They are searched in the folder in which the data of the plot item is located.
    /// All tables with a <see cref="FourPointPeakEvaluationDataSource"/> are returned, which have the same <see cref="XYColumnPlotData"/> as the current plot item.
    /// </summary>
    /// <param name="columnPlotData">The column plot data.</param>
    /// <returns>A list with all tables with a <see cref="FourPointPeakEvaluationDataSource"/> are returned, which have the same <see cref="XYColumnPlotData"/> as the current plot item.</returns>
    public static List<DataTable> GetExistingDestinationTables(XYColumnPlotData? columnPlotData)
    {
      var result = new List<DataTable>();
      if (columnPlotData?.XColumn is not { } xcol || columnPlotData?.YColumn is not { } ycol)
      {
        return result;
      }
      var sourceTable = Altaxo.Data.DataTable.GetParentDataTableOf(xcol.GetUnderlyingDataColumnOrDefault());
      if (sourceTable is null)
      {
        return result;
      }

      foreach (var t in Current.Project.Folders.GetItemsInFolder(sourceTable.Folder).OfType<DataTable>())
      {
        if (object.ReferenceEquals(t, sourceTable))
          continue;

        if (t.DataSource is FourPointStepEvaluationDataSource ds)
        {
          if (ds.ProcessData.Equals(columnPlotData))
            result.Add(t);
        }
      }
      return result;
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

      DataTable? newTable = null;

      if (!string.IsNullOrEmpty(_destinationTableName))
      {
        Current.Project.DataTableCollection.TryGetValue(_destinationTableName, out newTable);
      }

      var oldDataSource = newTable?.DataSource as FourPointStepEvaluationDataSource;

      var dataSourceOptions = new FourPointStepEvaluationOptions()
      {
        IndexLeftOuter = _handle[0].PlotIndex,
        IndexLeftInner = _handle[1].PlotIndex,
        IndexRightInner = _handle[2].PlotIndex,
        IndexRightOuter = _handle[3].PlotIndex,
        UseRegressionForLeftAndRightLine = oldDataSource?.ProcessOptions?.UseRegressionForLeftAndRightLine ?? _options.UseRegressionForLeftAndRightLine,
        MiddleRegressionLevels = oldDataSource?.ProcessOptions?.MiddleRegressionLevels ?? _options.MiddleRegressionLevels,
        MiddleLineOverlap = oldDataSource?.ProcessOptions?.MiddleLineOverlap ?? _options.MiddleLineOverlap,
      };

      if (newTable is null)
      {
        var newTableName = sourceTable.Name + $"_StepEvaluation_{yname}_Vs_{xname}";
        newTableName = Current.Project.DataTableCollection.FindNewItemName(newTableName);
        newTable = Current.Project.DataTableCollection.EnsureExistence(newTableName);
      }

      newTable.DataSource = new FourPointStepEvaluationDataSource(new XAndYColumn(PlotItem.Data), dataSourceOptions, new DataSourceImportOptions());

      newTable.DataSource.FillData(newTable, DummyProgressReporter.Instance);

      // now add the curves to the plot

      var plotCollection = (PlotItemCollection)TreeNodeExtensions.RootNode<IGPlotItem>(PlotItem);

      var newCollection = new PlotItemCollection();
      plotCollection.Add(newCollection);

      var linePlotStyleTemplate = new LinePlotStyle(_grac.Doc.GetPropertyContext()) { IndependentLineColor = true };
      linePlotStyleTemplate.LinePen = _options.LinePen.WithWidth(linePlotStyleTemplate.LinePen.Width);


      var newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberLeft, newTable[FourPointStepEvaluationDataSource.ColumnNameLeftX], newTable[FourPointStepEvaluationDataSource.ColumnNameLeftY]),
                          new G2DPlotStyleCollection(new[]
                          {
                              (LinePlotStyle)linePlotStyleTemplate.Clone(),
                          }));

      newCollection.Add(newPlotItem);

      newPlotItem = new XYColumnPlotItem(
                    new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberRight, newTable[FourPointStepEvaluationDataSource.ColumnNameRightX], newTable[FourPointStepEvaluationDataSource.ColumnNameRightY]),
                    new G2DPlotStyleCollection(new[]
                    {
                               (LinePlotStyle)linePlotStyleTemplate.Clone(),
                    }));

      newCollection.Add(newPlotItem);


      newPlotItem = new XYColumnPlotItem(
                          new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointStepEvaluationDataSource.ColumnGroupNumberMiddle, newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleX], newTable[FourPointStepEvaluationDataSource.ColumnNameMiddleY]),
                          new G2DPlotStyleCollection(new[]
                          {
                               (LinePlotStyle)linePlotStyleTemplate.Clone(),
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

      _isEvaluationSaved = true;
    }
  }
}
