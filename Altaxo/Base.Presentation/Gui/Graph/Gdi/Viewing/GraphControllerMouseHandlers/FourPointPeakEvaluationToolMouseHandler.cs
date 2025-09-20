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
using System.Linq;
using System.Windows.Input;
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Collections;
using Altaxo.Data;
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
  public class FourPointPeakEvaluationToolMouseHandler : FourPointsOnCurveMouseHandler
  {
    public static PropertyKey<FourPointPeakEvaluationToolMouseHandlerOptions> DefaultOptionsKey = new PropertyKey<FourPointPeakEvaluationToolMouseHandlerOptions>("380D19F1-2F14-49E8-B4F6-5BFB6CA10A46", "Graph\\PeakEvaluationToolOptions", PropertyLevel.Application, null, () => new FourPointPeakEvaluationToolMouseHandlerOptions());

    protected FourPointPeakEvaluationToolMouseHandlerOptions _options;

    private double[] _xOuter = new double[100];
    private double[] _yOuter = new double[100];
    private double[] _xInner = new double[100];
    private double[] _yInner = new double[100];

    private double _areaValue;
    private double _height;
    private double _peakX;
    private double _FWHM;

    private List<(double x, double y)> _areaPointList = new List<(double x, double y)>();

    private string _errorMessage;
    private bool ShowOuterLine => _lineRegression is not null && _lineRegression.IsValid;
    private bool ShowInnerLine => _lineRegression is not null && _lineRegression.IsValid;

    private QuickLinearRegression? _lineRegression;

    private bool _isEvaluationSaved;

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointPeakEvaluationToolMouseHandler"/> class.
    /// </summary>
    /// <param name="grac">The graph controller.</param>
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
      Initialize(grac);
    }

    /// <summary>
    /// Initializes the peak evaluation tool. This function searches for existing peak evaluations in the graph
    /// and lets the user choose one of them to edit, or to create a new one.
    /// </summary>
    /// <param name="grac">The graph controller.</param>
    public void Initialize(GraphController grac)
    {
      var graph = grac.Doc;


      // First step: Find plotItems, which comes from a table that contains a FourPointPeakEvaluationDataSource
      // then store the x-y source together with the evaluation table
      var peakDataSources = new Dictionary<XAndYColumn, DataTable>();
      foreach (var layer in graph.RootLayer.EnumerateFromHereToLeaves().OfType<XYPlotLayer>())
      {
        foreach (var plotItem in layer.PlotItems.EnumerateFromHereToLeaves().OfType<XYColumnPlotItem>())
        {
          if (plotItem?.XYColumnPlotData?.DataTable?.DataSource is FourPointPeakEvaluationDataSource ds && ds.ProcessData is { } xyCol)
          {
            peakDataSources[xyCol] = plotItem.XYColumnPlotData.DataTable;
          }
        }
      }

      // now find all plot items here whose data is a x-y-data like that in peakDataSources
      // then store this item together with the evaluation table
      var peakDataPlotItems = new Dictionary<XYColumnPlotItem, DataTable>();
      foreach (var layer in graph.RootLayer.EnumerateFromHereToLeaves().OfType<XYPlotLayer>())
      {
        foreach (var plotItem in layer.PlotItems.EnumerateFromHereToLeaves().OfType<XYColumnPlotItem>())
        {
          if (plotItem.XYColumnPlotData is { } pd && peakDataSources.TryGetValue(pd, out var evaluationTable))
            peakDataPlotItems[plotItem] = evaluationTable;
        }
      }


      // now let the user choose which item he/she wants to edit
      if (peakDataPlotItems.Count >= 1)
      {
        var list = new List<string>();
        list.Add("New evaluation");
        list.AddRange(peakDataPlotItems.Select(kv => $"{kv.Key.GetName(2)} -> {kv.Value.Name}"));
        var controller = new SingleChoiceController(list.ToArray(), 0);
        controller.DescriptionText =
          $"There already exist {peakDataPlotItems.Count} peak evaluations\r\n" +
          $"If you want to reuse an existing item, choose that;\r\n" +
          $"otherwise, choose the option to create a new evaluation\r\n";

        if (Current.Gui.ShowDialog(controller, "Choose new or existing evaluation table"))
        {
          var selectionIndex = (int)controller.ModelObject;
          if (selectionIndex > 0)
          {
            var selected = peakDataPlotItems.Skip(selectionIndex - 1).First();
            OnPlotItemSet(selected.Key, selected.Value);
          }
        }
      }
    }

    public override GraphToolType GraphToolType => GraphToolType.FourPointPeakEvaluation;

    protected override void OnPlotItemSet(XYColumnPlotItem plotItem)
    {
      const string NameOfNewTableEntry = "New evaluation table";

      base.OnPlotItemSet(plotItem);

      var tableList = GetExistingDestinationTables(plotItem.XYColumnPlotData);

      DataTable? selectedTable = null;

      if (tableList.Count == 1)
      {
        if (Current.Gui.YesNoMessageBox(
          $"There is already a peak evaluation in table {tableList[0].Name}\r\n" +
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
          $"There already exist {tableList.Count} peak evaluation tables\r\n" +
          $"If you want to use and overwrite an existing table, choose that\r\n" +
          $"table from the list, otherwise, choose the option to create\r\n" +
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

    /// <summary>
    /// Called when the plot item should be set and there is already a destination table with a <see cref="FourPointPeakEvaluationDataSource"/>.
    /// </summary>
    /// <param name="plotItem">The plot item.</param>
    /// <param name="existingDestinationTable">The existing destination table with a <see cref="FourPointPeakEvaluationDataSource"/>.</param>
    protected override void OnPlotItemSet(XYColumnPlotItem plotItem, DataTable existingDestinationTable)
    {
      base.OnPlotItemSet(plotItem, existingDestinationTable);

      var ds = (FourPointPeakEvaluationDataSource)existingDestinationTable.DataSource;
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
      _errorMessage = string.Empty;

      // outer line is going from the left outer point to the right outer point
      {
        var x0 = RMath.InterpolateLinear(_handle[0].PlotIndex, xcol);
        var y0 = RMath.InterpolateLinear(_handle[0].PlotIndex, ycol);
        var x1 = RMath.InterpolateLinear(_handle[3].PlotIndex, xcol);
        var y1 = RMath.InterpolateLinear(_handle[3].PlotIndex, ycol);

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
        var x0 = RMath.InterpolateLinear(_handle[1].PlotIndex, xcol);
        var y0 = RMath.InterpolateLinear(_handle[1].PlotIndex, ycol);
        var x1 = RMath.InterpolateLinear(_handle[2].PlotIndex, xcol);
        var y1 = RMath.InterpolateLinear(_handle[2].PlotIndex, ycol);

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
        int i = (int)startIdx;
        if (Math.IEEERemainder(startIdx, 1) != 0)
        {
          var x = RMath.InterpolateLinear(startIdx, xcol);
          _areaPointList.Add((x, lineReg.GetYOfX(x)));
          i = (int)Math.Ceiling(startIdx);
        }
        for (; i <= endIdx; i++)
        {
          _areaPointList.Add((xcol[i], ycol[i]));
        }
        if (Math.IEEERemainder(endIdx, 1) != 0)
        {
          var x = RMath.InterpolateLinear(endIdx, xcol);
          _areaPointList.Add((x, lineReg.GetYOfX(x)));
        }

        i = (int)endIdx;
        if (Math.IEEERemainder(endIdx, 1) != 0)
        {
          var x = RMath.InterpolateLinear(endIdx, xcol);
          var y = RMath.InterpolateLinear(endIdx, ycol);
          _areaPointList.Add((x, y));
          endIdx = (int)Math.Floor(endIdx);
        }

        for (; i >= startIdx; i--)
        {
          _areaPointList.Add((xcol[i], lineReg.GetYOfX(xcol[i])));
        }

        if (Math.IEEERemainder(startIdx, 1) != 0)
        {
          var x = RMath.InterpolateLinear(startIdx, xcol);
          var y = RMath.InterpolateLinear(startIdx, ycol);
          _areaPointList.Add((x, y));
        }
      }

      _areaValue = FourPointPeakEvaluationDataSource.CalculateArea(xcol, ycol, lineReg, _handle[1].PlotIndex, _handle[2].PlotIndex);
      (_height, _peakX, _FWHM) = FourPointPeakEvaluationDataSource.CalculatePeakParameters(xcol, ycol, lineReg, _handle[1].PlotIndex, _handle[2].PlotIndex);
    }

    protected override void UpdateDataDisplay()
    {
      if (!string.IsNullOrEmpty(_errorMessage))
      {
        Current.DataDisplay.WriteThreeLines(
          _errorMessage,
          "",
          ""
          );
        return;
      }
      else
      {
        Current.DataDisplay.WriteThreeLines(
          $"OuterLeft: ({_xOuter[0]}, {_yOuter[0]}); OuterRight: ({_xOuter[^1]}, {_yOuter[^1]})",
          $"InnerLeft: ({_xInner[0]}, {_yInner[0]}); InnerRight: ({_xInner[^1]}, {_yInner[^1]})",
          $"Area: {_areaValue}; Height: {_height}; PeakPos: {_peakX}; FWHM: {_FWHM}"
          );
      }
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

        if (t.DataSource is FourPointPeakEvaluationDataSource ds)
        {
          if (ds.ProcessData.Equals(columnPlotData))
            result.Add(t);
        }
      }
      return result;
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

      DataTable? newTable = null;

      if (!string.IsNullOrEmpty(_destinationTableName))
      {
        Current.Project.DataTableCollection.TryGetValue(_destinationTableName, out newTable);
      }

      if (newTable is null)
      {
        var newTableName = sourceTable.Name + $"_PeakEvaluation_{yname}_Vs_{xname}";
        newTableName = Current.Project.DataTableCollection.FindNewItemName(newTableName);
        newTable = Current.Project.DataTableCollection.EnsureExistence(newTableName);
      }

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
                          (XYColumnPlotData)PlotItem.XYColumnPlotData.Clone(),
                          new G2DPlotStyleCollection(new IG2DPlotStyle[]
                          {
                              new LinePlotStyle(_grac.Doc.GetPropertyContext()) { IndependentLineColor = true, Color = Altaxo.Drawing.NamedColors.Transparent },
                          }));
      newCollection.Add(newPlotItem);

      newPlotItem = new XYColumnPlotItem(
                    new Altaxo.Graph.Plot.Data.XYColumnPlotData(newTable, FourPointPeakEvaluationDataSource.ColumnGroupNumberInnerLine, newTable[FourPointPeakEvaluationDataSource.ColumnNameInnerLineX], newTable[FourPointPeakEvaluationDataSource.ColumnNameInnerLineY]),
                    new G2DPlotStyleCollection(new IG2DPlotStyle[]
                    {
                               (LinePlotStyle)linePlotStyleTemplate.Clone(),
                               new FillToCurvePlotStyle(_grac.Doc.GetPropertyContext()) {  FillToNextItem = false, FillToPreviousItem = true, IndependentFillColor = true, FillBrush = _options.AreaBrush },
                    }));

      newCollection.Add(newPlotItem);

      _isEvaluationSaved = true;
    }
  }
}
