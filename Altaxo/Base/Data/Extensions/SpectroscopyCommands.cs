using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;
using Altaxo.Gui;
using Altaxo.Gui.Analysis.Spectroscopy;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Data
{
  public class SpectroscopyCommands
  {
      public static void SpectralPreprocessingShowDialog(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
    {
      var selectedColumns = ctrl.SelectedDataColumns;

      if(selectedColumns is null || selectedColumns.Count == 0)
      {
        Current.Gui.InfoMessageBox("Please select one or more columns with spectral data");
        return;
      }
    
      var doc = new SpectralPreprocessingOptions();
      var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { doc }, typeof(IMVCANController));

      if (false == Current.Gui.ShowDialog(controller, "Spectral preprocessing"))
        return;

      doc = (SpectralPreprocessingOptions)controller.ModelObject;

      // now process the data

      var srcTable = ctrl.DataTable;
      var dstTable = new DataTable();

      var peakTable = new DataTable();

      var dict = new Dictionary<DataColumn, DataColumn>();

      int runningColumnNumber = -1;
      foreach(var selColIdx in selectedColumns)
      {
        ++runningColumnNumber;
        var yCol = ctrl.DataTable[selColIdx];
        var xCol = ctrl.DataTable.DataColumns.FindXColumnOf(yCol);

        if(xCol is null)
        {
          continue;
        }
        var len = Math.Min(yCol.Count, xCol.Count);

        var xArr = new double[len];
        var yArr = new double[len];
        for(int i = 0; i < len; i++)
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
        (xArr,yArr) = doc.Cropping.Execute(xArr,yArr);
        // 4. Baseline
        var baseline = doc.BaselineEstimation.Execute(xArr, yArr);
        for (int i = 0; i < len; i++)
        {
          yArr[i] -= baseline[i];
        }
        // 5. Normalization
        yArr = doc.Normalization.Execute(yArr);


        if(!dict.ContainsKey(xCol))
        {
          var xDst = dstTable.DataColumns.EnsureExistence(
          srcTable.DataColumns.GetColumnName(xCol),
          xCol.GetType(),
          srcTable.DataColumns.GetColumnKind(xCol),
          srcTable.DataColumns.GetColumnGroup(xCol)
          );

          for (int i = 0; i < len; i++)
            xDst[i] = xArr[i];

          dict.Add(xCol, xDst);
        }

        var yDst = dstTable.DataColumns.EnsureExistence(
          srcTable.DataColumns.GetColumnName(yCol),
          yCol.GetType(),
          srcTable.DataColumns.GetColumnKind(yCol),
          srcTable.DataColumns.GetColumnGroup(yCol)
          );

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
          for(int i = 0; i < descriptions.Count; i++)
          {
            cPos[i] = RMath.InterpolateLinear(descriptions[i].PositionIndex, xArr);
            cPro[i] = descriptions[i].Prominence;
            cHei[i] = descriptions[i].Height;
            cWid[i] = Math.Abs(RMath.InterpolateLinear(descriptions[i].PositionIndex - 0.5*descriptions[i].Width, xArr) -
                      RMath.InterpolateLinear(descriptions[i].PositionIndex + 0.5 * descriptions[i].Width, xArr));
          }

          if (doc.PeakFitting is not PeakFittingNone)
          {
            var fitResults = doc.PeakFitting.Execute(xArr, yArr, peakResults.PeakDescriptions);

            var cFPos = peakTable.DataColumns.EnsureExistence($"FitPosition{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
            var cFHei = peakTable.DataColumns.EnsureExistence($"FitHeight{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFWid = peakTable.DataColumns.EnsureExistence($"FitWidth{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
            var cFNot = peakTable.DataColumns.EnsureExistence($"FitNotes{runningColumnNumber}", typeof(TextColumn), ColumnKind.V, runningColumnNumber);


            for (int i = 0; i < descriptions.Count; i++)
            {
              var r = fitResults.PeakDescriptions[i];
              cFPos[i] = r.PositionInXUnits;
              cFHei[i] = r.Height;
              cFWid[i] = r.Width;
              cFNot[i] = r.Notes;
            }
          }
        }
      }
      {
        var dstName = srcTable.Name + "_Preprocessed";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        dstTable.Name = dstName;
        Current.Project.DataTableCollection.Add(dstTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(dstTable);
      }

      if(peakTable.DataRowCount > 0)
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
