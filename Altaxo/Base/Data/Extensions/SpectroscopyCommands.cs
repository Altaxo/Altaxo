#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Data
{
  public class SpectroscopyCommands
  {
    private static PeakSearchingAndFittingOptions? _lastPeakFindingFittingOptions = null;

    private static SpectralPreprocessingOptions? _lastPreprocessOptions = null;

    /// <summary>
    /// Shows the dialog to get the preprocessing options.
    /// </summary>
    /// <param name="ctrl">The worksheet containing the spectra.</param>
    /// <param name="options">On successfull return, contains the preprocessing options.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    public static bool ShowDialogGetPreprocessingOptions(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl, out SpectralPreprocessingOptions options)
    {
      options = null;
      var selectedColumns = ctrl.SelectedDataColumns;

      if (selectedColumns is null || selectedColumns.Count == 0)
      {
        Current.Gui.InfoMessageBox("Please select one or more columns with spectral data");
        return false;
      }

      var doc = _lastPreprocessOptions ?? new SpectralPreprocessingOptions();
      var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController));

      if (false == Current.Gui.ShowDialog(controller, "Spectral preprocessing"))
        return false;

      options = (SpectralPreprocessingOptions)controller.ModelObject;
      _lastPreprocessOptions = options;
      return true;
    }

    /// <summary>
    /// Shows the dialog to get the preprocessing options.
    /// </summary>
    /// <param name="ctrl">The worksheet containing the spectra.</param>
    /// <param name="options">On successfull return, contains the preprocessing options.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    public static bool ShowDialogGetPeakFindingFittingOptions(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl, out PeakSearchingAndFittingOptions options)
    {
      options = null;
      var selectedColumns = ctrl.SelectedDataColumns;

      if (selectedColumns is null || selectedColumns.Count == 0)
      {
        Current.Gui.InfoMessageBox("Please select one or more columns with spectral data");
        return false;
      }

      var doc = _lastPeakFindingFittingOptions ?? new PeakSearchingAndFittingOptions();
      var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController));

      if (false == Current.Gui.ShowDialog(controller, "Spectral preprocessing"))
        return false;

      options = (PeakSearchingAndFittingOptions)controller.ModelObject;
      _lastPeakFindingFittingOptions = options;
      return true;
    }

    public const string ColumnsV = "V-Columns";


    public static List<(DataColumn xCol, DataColumn yCol, double[] xArray, double[] yArray)> GetColumnsAndArrays(DataTableMultipleColumnProxy inputData, out DataTable srcTable)
    {
      var resultList = new List<(DataColumn xCol, DataColumn yCol, double[] xArray, double[] yArray)>();
      srcTable = inputData.DataTable;
      if (srcTable is null)
        throw new InvalidOperationException($"No source table available for spectral preprocessing");

      var srcYCols = inputData.GetDataColumns(ColumnsV);
      if (srcYCols.Count == 0)
        throw new InvalidOperationException($"No V-columns available for spectral preprocessing");


      foreach (var yCol in srcYCols)
      {


        var xCol = srcTable.DataColumns.FindXColumnOf(yCol);

        if (xCol is null)
        {
          continue;
        }


        var len = Math.Min(yCol.Count, xCol.Count);

        var xArr = new double[len];
        var yArr = new double[len];
        for (int i = 0; i < len; i++)
        {
          xArr[i] = xCol[i];
          yArr[i] = yCol[i];
        }

        resultList.Add((xCol, yCol, xArr, yArr));
      }
      return resultList;
    }

    public static List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      double[] xArray,
      double[] yArray,
      int[]? regions)>
      ExecuteSpectralPreprocessing(DataTableMultipleColumnProxy inputData, SpectralPreprocessingOptions doc, DataTable dstTable)
    {
      var resultList = new List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      double[] xArray,
      double[] yArray,
      int[]? regions)>();

      var dictionarySrcXCol_To_DstXCol = new Dictionary<DataColumn, DataColumn>();
      var dictionarySrcYCol_To_DstYCol = new Dictionary<DataColumn, DataColumn>();
      int runningColumnNumber = -1;

      dstTable.DataColumns.RemoveColumnsAll();
      dstTable.PropCols.RemoveColumnsAll();

      foreach (var entry in GetColumnsAndArrays(inputData, out var srcTable))
      {
        ++runningColumnNumber;
        var xCol = entry.xCol;
        var yCol = entry.yCol;
        var xArr = entry.xArray;
        var yArr = entry.yArray;

        // now apply the preprocessing steps
        int[]? regions = null;
        (xArr, yArr, regions) = doc.Execute(xArr, yArr, regions);

        // Store result

        if (!dictionarySrcXCol_To_DstXCol.ContainsKey(xCol))
        {
          var xDst = dstTable.DataColumns.EnsureExistence(
          srcTable.DataColumns.GetColumnName(xCol),
          xCol.GetType(),
          srcTable.DataColumns.GetColumnKind(xCol),
          srcTable.DataColumns.GetColumnGroup(xCol)
          );

          for (int i = 0; i < xArr.Length; i++)
            xDst[i] = xArr[i];

          dictionarySrcXCol_To_DstXCol.Add(xCol, xDst);
        }


        var yDst = dstTable.DataColumns.EnsureExistence(
          srcTable.DataColumns.GetColumnName(yCol),
          yCol.GetType(),
          srcTable.DataColumns.GetColumnKind(yCol),
          srcTable.DataColumns.GetColumnGroup(yCol)
          );
        dictionarySrcYCol_To_DstYCol[yCol] = yDst;

        for (int i = 0; i < yArr.Length; i++)
          yDst[i] = yArr[i];

        // store the property columns
        for (int i = 0; i < srcTable.PropCols.ColumnCount; ++i)
        {
          var pCol = dstTable.PropCols.EnsureExistence(
            srcTable.PropCols.GetColumnName(i),
            srcTable.PropCols[i].GetType(),
            srcTable.PropCols.GetColumnKind(i),
            srcTable.PropCols.GetColumnGroup(i));

          var idxSrc = srcTable.DataColumns.GetColumnNumber(yCol);
          var idxDst = dstTable.DataColumns.GetColumnNumber(yDst);
          pCol[idxDst] = srcTable.PropCols[i][idxSrc];
        }

        resultList.Add((xCol, yCol, dictionarySrcXCol_To_DstXCol[xCol], yDst, xArr, yArr, regions));
      }

      return resultList;
    }

    public static List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      IReadOnlyList<(IReadOnlyList<Science.Spectroscopy.PeakFitting.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> fittingResult)>
      ExecutePeakFindingAndFitting(DataTableMultipleColumnProxy inputData, PeakSearchingAndFittingOptions doc, DataTable peakTable, DataTable? preprocessedSpectraTable = null)
    {
      var resultList = new List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      IReadOnlyList<(IReadOnlyList<Science.Spectroscopy.PeakFitting.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> fittingResult)>();

      preprocessedSpectraTable ??= new DataTable();

      var spectralPreprocessingResult = ExecuteSpectralPreprocessing(inputData, doc.Preprocessing, preprocessedSpectraTable);

      peakTable.DataColumns.RemoveColumnsAll();
      peakTable.PropCols.RemoveColumnsAll();
      int runningColumnNumber = -1;
      foreach (var entry in spectralPreprocessingResult)
      {
        ++runningColumnNumber;

        var xArr = entry.xArray;
        var yArr = entry.yArray;

        // now apply the steps
        var peakResults = doc.PeakSearching.Execute(yArr, entry.regions);
        var fitResults = doc.PeakFitting.Execute(xArr, yArr, peakResults);

        // Store the results

        if (doc.PeakSearching is not PeakSearchingNone)
        {

          var cPos = peakTable.DataColumns.EnsureExistence($"Position{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
          var cPro = peakTable.DataColumns.EnsureExistence($"Prominence{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cHei = peakTable.DataColumns.EnsureExistence($"Height{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cWid = peakTable.DataColumns.EnsureExistence($"Width{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);

          int idxRow = 0;
          for (int ri = 0; ri < peakResults.Count; ri++)
          {
            var descriptions = peakResults[ri].PeakDescriptions;
            for (int pi = 0; pi < descriptions.Count; pi++)
            {
              cPos[idxRow] = RMath.InterpolateLinear(descriptions[pi].PositionIndex, xArr);
              cPro[idxRow] = descriptions[pi].Prominence;
              cHei[idxRow] = descriptions[pi].Height;
              cWid[idxRow] = Math.Abs(RMath.InterpolateLinear(descriptions[pi].PositionIndex - 0.5 * descriptions[pi].Width, xArr) -
                        RMath.InterpolateLinear(descriptions[pi].PositionIndex + 0.5 * descriptions[pi].Width, xArr));

              ++idxRow;
            }
          }

          if (doc.PeakFitting is not PeakFittingNone)
          {
            var cFNot = peakTable.DataColumns.EnsureExistence($"FitNotes{runningColumnNumber}", typeof(TextColumn), ColumnKind.V, runningColumnNumber);
            var cFPos = peakTable.DataColumns.EnsureExistence($"FitPosition{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
            var cFPosVar = peakTable.DataColumns.EnsureExistence($"FitPosition{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFArea = peakTable.DataColumns.EnsureExistence($"FitArea{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFAreaVar = peakTable.DataColumns.EnsureExistence($"FitArea{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFHei = peakTable.DataColumns.EnsureExistence($"FitHeight{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFHeiVar = peakTable.DataColumns.EnsureExistence($"FitHeight{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFWid = peakTable.DataColumns.EnsureExistence($"FitFwhm{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFWidVar = peakTable.DataColumns.EnsureExistence($"FitFwhm{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFirstPoint = peakTable.DataColumns.EnsureExistence($"FitFirstPoint{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cLastPoint = peakTable.DataColumns.EnsureExistence($"FitLastPoint{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cNumberOfPoints = peakTable.DataColumns.EnsureExistence($"FitNumberOfPoints{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFirstXValue = peakTable.DataColumns.EnsureExistence($"FitFirstXValue{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cLastXValue = peakTable.DataColumns.EnsureExistence($"FitLastXValue{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);




            idxRow = 0;
            for (int ri = 0; ri < peakResults.Count; ri++)
            {
              var descriptions = fitResults[ri].PeakDescriptions;
              for (int pi = 0; pi < descriptions.Count; pi++)
              {
                var r = descriptions[pi];

                if (r.FitFunction is { } fitFunction)
                {
                  var (pos, posVar, area, areaVar, height, heightVar, fwhm, fwhmVar) = fitFunction.GetPositionAreaHeightFwhmFromSinglePeakParameters(r.PeakParameter, r.PeakParameterCovariances);

                  cFPos[idxRow] = pos;
                  cFPosVar[idxRow] = posVar;
                  cFArea[idxRow] = area;
                  cFAreaVar[idxRow] = areaVar;
                  cFHei[idxRow] = height;
                  cFHeiVar[idxRow] = heightVar;
                  cFWid[idxRow] = fwhm;
                  cFWidVar[idxRow] = fwhmVar;
                  cFirstPoint[idxRow] = r.FirstFitPoint;
                  cLastPoint[idxRow] = r.LastFitPoint;
                  cNumberOfPoints[idxRow] = Math.Abs(r.LastFitPoint - r.FirstFitPoint);
                  cFirstXValue[idxRow] = r.FirstFitPosition;
                  cLastXValue[idxRow] = r.LastFitPosition;

                  var parameterNames = fitFunction.ParameterNamesForOnePeak;
                  for (int j = 0; j < parameterNames.Length; j++)
                  {
                    var cParaValue = peakTable.DataColumns.EnsureExistence($"{parameterNames[j]}{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
                    var cParaVariance = peakTable.DataColumns.EnsureExistence($"{parameterNames[j]}{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);

                    cParaValue[idxRow] = r.PeakParameter[j];
                    cParaVariance[idxRow] = Math.Sqrt(r.PeakParameterCovariances[j, j]);
                  }


                }
                cFNot[idxRow] = r.Notes;
                ++idxRow;
              }
            }
          }
        }

        resultList.Add((entry.xOrgCol, entry.yOrgCol, entry.xPreprocessedCol, entry.yPreprocessedCol, fitResults));
      }


      return resultList;
    }

    public static void SpectralPeakFindingFittingShowDialog(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (!ShowDialogGetPeakFindingFittingOptions(ctrl, out var doc))
        return;

      // now process the data
      var srcTable = ctrl.DataTable;
      var preprocessingTable = new DataTable();

      var peakTable = new DataTable();


      var dataProxy = new DataTableMultipleColumnProxy(ColumnsV, srcTable, null, ctrl.SelectedDataColumns);
      var result = ExecutePeakFindingAndFitting(dataProxy, doc, peakTable, preprocessingTable);

      {
        var dstName = srcTable.Name + "_Preprocessed";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        preprocessingTable.Name = dstName;
        Current.Project.DataTableCollection.Add(preprocessingTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(preprocessingTable);


        preprocessingTable.DataSource = new SpectralPreprocessingDataSource(
          dataProxy,
          new SpectralPreprocessingOptions(doc.Preprocessing), // downcast to SpectralPreprocessingOptions
          new DataSourceImportOptions());
      }

      {
        var dstName = srcTable.Name + "_Peaks";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        peakTable.Name = dstName;
        Current.Project.DataTableCollection.Add(peakTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(peakTable);


        peakTable.DataSource = new PeakFindingAndFittingDataSource(
          dataProxy,
          new PeakSearchingAndFittingOptions(doc), // downcast to PeakFindingAndFittingOptions
          new DataSourceImportOptions());
      }



      // -----------------------------------------------------------------------------
      // Peak plotting
      // -----------------------------------------------------------------------------

      foreach (var resultForOneColumn in result)
      {
        var selColumnsForPeakGraph = new AscendingIntegerCollection();
        selColumnsForPeakGraph.Add(preprocessingTable.DataColumns.GetColumnNumber(resultForOneColumn.yPreprocessedCol));
        string preferredGraphName = preprocessingTable.FolderName + "GPeaks";
        var graphController = PlotCommands.PlotLine(preprocessingTable, selColumnsForPeakGraph, bLine: false, bScatter: true, preferredGraphName);
        var graph = graphController.Doc;
        var layer = (XYPlotLayer)graph.RootLayer.Layers[0];
        var pi = Altaxo.Collections.TreeNodeExtensions.TakeFromHereToFirstLeaves<IGPlotItem>(layer.PlotItems).OfType<XYColumnPlotItem>().FirstOrDefault();
        if (pi is not null)
        {
          var scatter = pi.Style.OfType<ScatterPlotStyle>().FirstOrDefault();
          if (scatter is not null)
          {
            scatter.SymbolSize = 3;
          }
        }


        var group = new PlotItemCollection();
        layer.PlotItems.Add(group);
        // add fit function(s)
        var fitFunctionHashTable = new HashSet<(IFitFunction FitFunction, double[] Parameter)>();
        foreach (var resultForOneRegion in resultForOneColumn.fittingResult)
        {
          foreach (var peakDesc in resultForOneRegion.PeakDescriptions.Where(pd => pd.FitFunction is not null))
          {
            if (!fitFunctionHashTable.Contains((peakDesc.FitFunction, peakDesc.FitFunctionParameter)))
            {
              fitFunctionHashTable.Add((peakDesc.FitFunction, peakDesc.FitFunctionParameter));
              // var data = new XYFunctionPlotData(peakDesc.FitFunction);
              var fitElement = new FitElement(peakDesc.FitFunction);
              var fitDocument = new NonlinearFitDocument();
              fitDocument.FitEnsemble.Add(fitElement);
              fitDocument.SetDefaultParametersForFitElement(0);
              for (int i = 0; i < peakDesc.FitFunctionParameter.Length; i++)
                fitDocument.CurrentParameters[i].Parameter = peakDesc.FitFunctionParameter[i];
              var data = new XYNonlinearFitFunctionPlotData(System.Guid.NewGuid().ToString(), fitDocument, 0, 0, null, 0, null);

              var plotStyle = PlotCommands.PlotStyle_Line(graph.GetPropertyContext());
              var lineStyle = plotStyle.OfType<LinePlotStyle>().FirstOrDefault();
              if (lineStyle is not null && lineStyle.Color.ParentColorSet is { } parentCSet)
              {
                var idx = parentCSet.IndexOf(lineStyle.Color);
                if (idx >= 0)
                {
                  lineStyle.Color = parentCSet[(idx + 1) % parentCSet.Count];
                }
              }
              var functionPlotItem = new XYFunctionPlotItem(data, plotStyle);
              group.Add(functionPlotItem);
            }
          }
        }
      }

    }

    public static void SpectralPreprocessingShowDialog(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (!ShowDialogGetPreprocessingOptions(ctrl, out var doc))
        return;

      // now process the data
      var srcTable = ctrl.DataTable;
      var preprocessingTable = new DataTable();


      var dataProxy = new DataTableMultipleColumnProxy(ColumnsV, srcTable, null, ctrl.SelectedDataColumns);
      var result = ExecuteSpectralPreprocessing(dataProxy, doc, preprocessingTable);

      {
        var dstName = srcTable.Name + "_Preprocessed";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        preprocessingTable.Name = dstName;
        Current.Project.DataTableCollection.Add(preprocessingTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(preprocessingTable);


        preprocessingTable.DataSource = new SpectralPreprocessingDataSource(
          dataProxy,
          new SpectralPreprocessingOptions(doc), // downcast to SpectralPreprocessingOptions
          new DataSourceImportOptions());
      }
    }

  }
}
