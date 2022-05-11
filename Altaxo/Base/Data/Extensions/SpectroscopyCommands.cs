using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Gui;
using Altaxo.Gui.Analysis.Spectroscopy;

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

      var dict = new Dictionary<DataColumn, DataColumn>();

      foreach(var selColIdx in selectedColumns)
      {
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
      }

      var dstName = srcTable.Name + "_Preprocessed";
      dstTable.Name = Current.Project.DataTableCollection.FindNewItemName(dstName);
      Current.Project.DataTableCollection.Add(dstTable);
      Current.ProjectService.OpenOrCreateWorksheetForTable(dstTable);
    }
  }
}
