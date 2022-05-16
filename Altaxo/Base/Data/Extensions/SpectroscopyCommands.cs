using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Analysis.Spectroscopy;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Data
{
  public class SpectroscopyCommands
  {
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

      var doc = new SpectralPreprocessingOptions();
      var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController));

      if (false == Current.Gui.ShowDialog(controller, "Spectral preprocessing"))
        return false;

      options = (SpectralPreprocessingOptions)controller.ModelObject;
      return true;
    }

    public static void SpectralPreprocessingShowDialog(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      if (!ShowDialogGetPreprocessingOptions(ctrl, out var doc))
        return;

      // now process the data
      var srcTable = ctrl.DataTable;
      var dstTable = new DataTable();
      {
        var dstName = srcTable.Name + "_Preprocessed";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        dstTable.Name = dstName;
        Current.Project.DataTableCollection.Add(dstTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(dstTable);
      }


      var peakTable = new DataTable();

      var dictionarySrcXCol_To_DstXCol = new Dictionary<DataColumn, DataColumn>();

      var dictionarySrcYCol_To_DstYCol = new Dictionary<DataColumn, DataColumn>();

      int runningColumnNumber = -1;
      var selectedColumns = ctrl.SelectedDataColumns;
      foreach (var selColIdx in selectedColumns)
      {
        ++runningColumnNumber;
        var yCol = ctrl.DataTable[selColIdx];
        var xCol = ctrl.DataTable.DataColumns.FindXColumnOf(yCol);

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

        // now apply the preprocessing steps

        // 1. Spike removal
        yArr = doc.SpikeRemoval.Execute(yArr);
        // 2. 
        yArr = doc.Smoothing.Execute(yArr);
        // 3. Cropping
        (xArr, yArr) = doc.Cropping.Execute(xArr, yArr);
        // 4. Baseline
        var baseline = doc.BaselineEstimation.Execute(xArr, yArr);
        for (int i = 0; i < len; i++)
        {
          yArr[i] -= baseline[i];
        }
        // 5. Normalization
        yArr = doc.Normalization.Execute(yArr);


        if (!dictionarySrcXCol_To_DstXCol.ContainsKey(xCol))
        {
          var xDst = dstTable.DataColumns.EnsureExistence(
          srcTable.DataColumns.GetColumnName(xCol),
          xCol.GetType(),
          srcTable.DataColumns.GetColumnKind(xCol),
          srcTable.DataColumns.GetColumnGroup(xCol)
          );

          for (int i = 0; i < len; i++)
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

        for (int i = 0; i < len; i++)
          yDst[i] = yArr[i];

        // if peak searching is enabled, make further processing

        {
          var peakResults = doc.PeakSearching.Execute(yArr);
          var cPos = peakTable.DataColumns.EnsureExistence($"Position{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
          var cPro = peakTable.DataColumns.EnsureExistence($"Prominence{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cHei = peakTable.DataColumns.EnsureExistence($"Height{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cWid = peakTable.DataColumns.EnsureExistence($"Width{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);

          var descriptions = peakResults.PeakDescriptions;
          for (int i = 0; i < descriptions.Count; i++)
          {
            cPos[i] = RMath.InterpolateLinear(descriptions[i].PositionIndex, xArr);
            cPro[i] = descriptions[i].Prominence;
            cHei[i] = descriptions[i].Height;
            cWid[i] = Math.Abs(RMath.InterpolateLinear(descriptions[i].PositionIndex - 0.5 * descriptions[i].Width, xArr) -
                      RMath.InterpolateLinear(descriptions[i].PositionIndex + 0.5 * descriptions[i].Width, xArr));
          }

          if (doc.PeakFitting is not PeakFittingNone)
          {
            var fitResults = doc.PeakFitting.Execute(xArr, yArr, peakResults.PeakDescriptions);

            var cFPos = peakTable.DataColumns.EnsureExistence($"FitPosition{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
            var cFPosVar = peakTable.DataColumns.EnsureExistence($"FitPosition.Err{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFHei = peakTable.DataColumns.EnsureExistence($"FitHeight{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFHeiVar = peakTable.DataColumns.EnsureExistence($"FitHeight.Err{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFWid = peakTable.DataColumns.EnsureExistence($"FitWidth{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFWidVar = peakTable.DataColumns.EnsureExistence($"FitWidth.Err{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);
            var cFNot = peakTable.DataColumns.EnsureExistence($"FitNotes{runningColumnNumber}", typeof(TextColumn), ColumnKind.V, runningColumnNumber);


            for (int i = 0; i < descriptions.Count; i++)
            {
              var r = fitResults.PeakDescriptions[i];
              cFPos[i] = r.PositionInXUnits;
              cFPosVar[i] = r.PositionInXUnitsVariance;
              cFHei[i] = r.Height;
              cFHeiVar[i] = r.HeightVariance;
              cFWid[i] = r.Width;
              cFWidVar[i] = r.WidthVariance;
              cFNot[i] = r.Notes;
            }


            // -----------------------------------------------------------------------------
            // Peak plotting
            // -----------------------------------------------------------------------------

            var selColumnsForPeakGraph = new AscendingIntegerCollection();
            selColumnsForPeakGraph.Add(dstTable.DataColumns.GetColumnNumber(yDst));
            string preferredGraphName = "GPeaks";
            var graphController = PlotCommands.PlotLine(dstTable, selectedColumns, bLine: false, bScatter: true, preferredGraphName);
            var graph = graphController.Doc;
            var layer = (XYPlotLayer)graph.RootLayer.Layers[0];
            var pi = Altaxo.Collections.TreeNodeExtensions.TakeFromHereToFirstLeaves<IGPlotItem>(layer.PlotItems).OfType<XYColumnPlotItem>().FirstOrDefault();
            if (pi is not null)
            {
              var scatter = pi.Style.OfType<ScatterPlotStyle>().FirstOrDefault();
              if(scatter is not null)
              {
                scatter.SymbolSize = 3;
              }
            }


            var group = new PlotItemCollection();
            layer.PlotItems.Add(group);
            // add fit function(s)
            var fitFunctionHashTable = new HashSet<IScalarFunctionDD>();
            foreach (var peakDesc in fitResults.PeakDescriptions.Where(pd => pd.FitFunction is not null))
            {
              if (!fitFunctionHashTable.Contains(peakDesc.FitFunction))
              {
                fitFunctionHashTable.Add(peakDesc.FitFunction);
                var data = new XYFunctionPlotData(peakDesc.FitFunction);
                var plotStyle = PlotCommands.PlotStyle_Line(graph.GetPropertyContext());
                var lineStyle = plotStyle.OfType<LinePlotStyle>().FirstOrDefault();
                if(lineStyle is not null && lineStyle.Color.ParentColorSet is { } parentCSet )
                {
                  var idx = parentCSet.IndexOf(lineStyle.Color);
                  if(idx>=0)
                  {
                    lineStyle.Color = parentCSet[(idx+1)%parentCSet.Count];
                  }
                }
                var functionPlotItem = new XYFunctionPlotItem(data, plotStyle);
                group.Add(functionPlotItem);
              }
            }

            // -----------------------------------------------------------------------------
            // End of peak plotting
            // -----------------------------------------------------------------------------
          }
        }

        



       


      } // for each selected column

      

      if (peakTable.DataRowCount > 0)
      {
        var dstName = srcTable.Name + "_Peaks";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        peakTable.Name = dstName;
        Current.Project.DataTableCollection.Add(peakTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(peakTable);
      }

      

    }
  }
}
