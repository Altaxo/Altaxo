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
using System.Threading;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Science.Spectroscopy.Raman;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Main.Services;
using Altaxo.Science.Spectroscopy.Raman;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Science.Spectroscopy
{
  public class SpectroscopyCommands
  {
    private static PeakSearchingAndFittingOptions? _lastPeakFindingFittingOptions = null;

    private static SpectralPreprocessingOptions? _lastPreprocessOptions = null;

    public static string PeakTable_PreprocessedColumnNameX(int numberOfSpectrum) => $"X_Preprocessed{numberOfSpectrum}";
    public static string PeakTable_PreprocessedColumnNameY(int numberOfSpectrum) => $"Y_Preprocessed{numberOfSpectrum}";
    public static string PeakTable_UsedForFitColumnName(int numberOfSpectrum) => $"UsedForFit{numberOfSpectrum}";

    public static string PeakTable_FitCurveColumnNameX(int numberOfSpectrum) => $"X_FitCurve{numberOfSpectrum}";
    public static string PeakTable_FitCurveColumnNameY(int numberOfSpectrum) => $"Y_FitCurve{numberOfSpectrum}";

    public static string PeakTable_SeparatePeaksColumnNameX(int numberOfSpectrum) => $"X_PeakCurves{numberOfSpectrum}";
    public static string PeakTable_SeparatePeaksColumnNameY(int numberOfSpectrum) => $"Y_PeakCurves{numberOfSpectrum}";
    public static string PeakTable_SeparatePeaksColumnNameID(int numberOfSpectrum) => $"ID_PeakCurves{numberOfSpectrum}";

    /// <summary>
    /// Shows the dialog to get the preprocessing options.
    /// </summary>
    /// <param name="ctrl">The worksheet containing the spectra.</param>
    /// <param name="options">On successfull return, contains the preprocessing options.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    public static bool ShowDialogGetPreprocessingOptions(WorksheetController ctrl, out SpectralPreprocessingOptions? options)
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
    public static bool ShowDialogGetPeakFindingFittingOptions(WorksheetController ctrl, out PeakSearchingAndFittingOptions options)
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

    public const string ColumnX = "X-Column";
    public const string ColumnsV = "V-Columns";


    public static List<(DataColumn xCol, DataColumn yCol, double[] xArray, double[] yArray)> GetColumnsAndArrays(
      DataTableMultipleColumnProxy inputData,
      out DataTable srcTable)
    {
      var resultList = new List<(DataColumn xCol, DataColumn yCol, double[] xArray, double[] yArray)>();
      srcTable = inputData.DataTable;
      int groupNumber = inputData.GroupNumber;
      if (srcTable is null)
        throw new InvalidOperationException($"No source table available for spectral preprocessing");

      var srcYCols = inputData.GetDataColumns(ColumnsV);
      if (srcYCols.Count == 0)
        throw new InvalidOperationException($"No V-columns available for spectral preprocessing");

      DataColumn srcXCol;
      var srcXCols = inputData.ContainsIdentifier(ColumnX) ? inputData.GetDataColumns(ColumnX) : new DataColumn[0];
      if (srcXCols.Count == 0)
      {
        srcXCol = srcTable.DataColumns.FindXColumnOfGroup(groupNumber);
        if (srcXCol is null)
        {
          throw new InvalidOperationException($"There was no x-column contained in the data, and no x-column can be found for group {groupNumber} in table {srcTable.Name}. Please assign an x-column!");
        }
        inputData.EnsureExistenceOfIdentifier(ColumnX, 1);
        inputData.SetDataColumn(ColumnX, srcXCol);
      }
      else if (srcXCols.Count == 1)
      {
        srcXCol = srcXCols[0];
      }
      else
      {
        throw new InvalidOperationException($"There is more than one x-columns available for spectral preprocessing!");
      }
      foreach (var srcYCol in srcYCols)
      {
        var len = Math.Min(srcYCol.Count, srcXCol.Count);

        var xArr = new double[len];
        var yArr = new double[len];
        for (var i = 0; i < len; i++)
        {
          xArr[i] = srcXCol[i];
          yArr[i] = srcYCol[i];
        }

        resultList.Add((srcXCol, srcYCol, xArr, yArr));
      }
      return resultList;
    }

    /// <summary>
    /// Executes the spectral preprocessing for one or more than one spectrum
    /// </summary>
    /// <param name="inputData">The input data. Usually consists of one x-axis and one or more y-arrays (the spectra).</param>
    /// <param name="doc">Spectral preprocessing options. document.</param>
    /// <param name="dstTable">The data table were to write the results to.</param>
    /// <returns>A list.
    /// Each entry consists of:
    /// - the original x-column and original y-column of the spectrum,
    /// - the columns with the preprocessed spectrum (x and y),
    /// - the array with the preprocessed spectrum (x, y, and regions).
    /// </returns>
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
      var runningColumnNumber = -1;

      dstTable.DataColumns.RemoveColumnsAll();
      dstTable.PropCols.RemoveColumnsAll();

      foreach (var entry in GetColumnsAndArrays(inputData, out var srcTable))
      {
        ++runningColumnNumber;
        var groupNumberBase = runningColumnNumber * 10;

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

          for (var i = 0; i < xArr.Length; i++)
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

        for (var i = 0; i < yArr.Length; i++)
          yDst[i] = yArr[i];

        // store the property columns
        for (var i = 0; i < srcTable.PropCols.ColumnCount; ++i)
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
      IReadOnlyList<(IReadOnlyList<PeakFitting.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> fittingResult)>
      ExecutePeakFindingAndFitting(DataTableMultipleColumnProxy inputData,
                                    PeakSearchingAndFittingOptions doc,
                                    DataTable peakTable,
                                    IProgress<string>? progressReporter,
                                    CancellationToken cancellationTokenSoft,
                                    CancellationToken cancellationTokenHard)
    {
      var resultList = new List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      IReadOnlyList<(IReadOnlyList<PeakFitting.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> fittingResult)>();

      var spectralPreprocessingResult = ExecuteSpectralPreprocessing(inputData, doc.Preprocessing, new DataTable());

      peakTable.DataColumns.RemoveColumnsAll();
      peakTable.PropCols.RemoveColumnsAll();
      var runningColumnNumber = -1;
      foreach (var entry in spectralPreprocessingResult)
      {
        ++runningColumnNumber;
        var groupNumberBase = runningColumnNumber * 10;

        if (cancellationTokenSoft.IsCancellationRequested)
        {
          break;
        }
        if (progressReporter is { } pr)
        {
          pr.Report($"Peak search+fit column {inputData.DataTable.DataColumns.GetColumnName(entry.yOrgCol)}");
        }


        var xArr = entry.xArray;
        var yArr = entry.yArray;
        var regions = entry.regions;

        // now apply the steps
        (xArr, yArr, regions, var peakResults) = doc.PeakSearching.Execute(xArr, yArr, regions);
        (xArr, yArr, regions, var fitResults) = doc.PeakFitting.Execute(xArr, yArr, regions, peakResults, cancellationTokenHard);

        // Store the results

        int idxRow;
        if (fitResults.Count > 0) // if we have fit results then we have executed a fit
        {
          // First, the columns for the peak search results
          var cPos = peakTable.DataColumns.EnsureExistence($"Position{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
          var cPro = peakTable.DataColumns.EnsureExistence($"Prominence{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cHei = peakTable.DataColumns.EnsureExistence($"Height{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cWid = peakTable.DataColumns.EnsureExistence($"Width{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);

          // and second, the columns for the peak fit results
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
          var cSumChiSquare = peakTable.DataColumns.EnsureExistence($"SumChiSquare{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cSigmaSquare = peakTable.DataColumns.EnsureExistence($"SigmaSquare{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);

          idxRow = 0;
          for (var ri = 0; ri < fitResults.Count; ri++)
          {
            var descriptions = fitResults[ri].PeakDescriptions;
            for (var pi = 0; pi < descriptions.Count; pi++)
            {
              var r = descriptions[pi];

              if (r.SearchDescription is { } searchDesc)
              {
                cPos[idxRow] = searchDesc.PositionValue;
                cPro[idxRow] = searchDesc.Prominence;
                cHei[idxRow] = searchDesc.Height;
                cWid[idxRow] = searchDesc.WidthValue;
              }

              if (r.FitFunction is { } fitFunction)
              {
                var (pos, posVar, area, areaVar, height, heightVar, fwhm, fwhmVar) = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(r.PeakParameter, r.PeakParameterCovariances);

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
                int numberOfFitPoints = 1 + Math.Abs(r.LastFitPoint - r.FirstFitPoint);
                cNumberOfPoints[idxRow] = numberOfFitPoints;
                cFirstXValue[idxRow] = r.FirstFitPosition;
                cLastXValue[idxRow] = r.LastFitPosition;
                cSumChiSquare[idxRow] = r.SumChiSquare;
                cSigmaSquare[idxRow] = r.SigmaSquare;

                var parameterNames = fitFunction.ParameterNamesForOnePeak;
                for (var j = 0; j < parameterNames.Length; j++)
                {
                  var cParaValue = peakTable.DataColumns.EnsureExistence($"{parameterNames[j]}{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
                  var cParaStdError = peakTable.DataColumns.EnsureExistence($"{parameterNames[j]}{runningColumnNumber}.Err", typeof(DoubleColumn), ColumnKind.Err, runningColumnNumber);

                  cParaValue[idxRow] = r.PeakParameter[j];
                  cParaStdError[idxRow] = r.PeakParameterCovariances is null ? 0 : Math.Sqrt(r.PeakParameterCovariances[j, j]);
                }


              }
              cFNot[idxRow] = r.Notes;
              ++idxRow;
            }
          }
        }


        else if (peakResults.Count > 0) // else if we have peak results, then we have executed a peak search
        {

          var cPos = peakTable.DataColumns.EnsureExistence($"Position{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.X, runningColumnNumber);
          var cPro = peakTable.DataColumns.EnsureExistence($"Prominence{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cHei = peakTable.DataColumns.EnsureExistence($"Height{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);
          var cWid = peakTable.DataColumns.EnsureExistence($"Width{runningColumnNumber}", typeof(DoubleColumn), ColumnKind.V, runningColumnNumber);

          idxRow = 0;
          for (var ri = 0; ri < peakResults.Count; ri++)
          {
            var descriptions = peakResults[ri].PeakDescriptions;
            for (var pi = 0; pi < descriptions.Count; pi++)
            {
              cPos[idxRow] = descriptions[pi].PositionValue;
              cPro[idxRow] = descriptions[pi].Prominence;
              cHei[idxRow] = descriptions[pi].Height;
              cWid[idxRow] = descriptions[pi].WidthValue;

              ++idxRow;
            }
          }
        }

        // Store the preprocessed spectra, if activated in the options
        if (doc.OutputOptions.OutputPreprocessedCurve)
        {
          var cXPre = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_PreprocessedColumnNameX(runningColumnNumber), typeof(DoubleColumn), ColumnKind.X, groupNumberBase + 1);
          var cYPre = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_PreprocessedColumnNameY(runningColumnNumber), typeof(DoubleColumn), ColumnKind.V, groupNumberBase + 1);
          cXPre.Data = xArr;
          cYPre.Data = yArr;
        }

        if (doc.OutputOptions.OutputFitCurve && fitResults.Count > 0)
        {
          // output also a column, which designates, whether a point was used for the fit or was not used.
          // the column group belongs to the preprocessed spectrum
          var cYUsedForFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_UsedForFitColumnName(runningColumnNumber), typeof(DoubleColumn), ColumnKind.V, groupNumberBase + 1);

          // two other columns which accomodate the fit curve
          var cXFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_FitCurveColumnNameX(runningColumnNumber), typeof(DoubleColumn), ColumnKind.X, groupNumberBase + 2);
          var cYFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_FitCurveColumnNameY(runningColumnNumber), typeof(DoubleColumn), ColumnKind.V, groupNumberBase + 2);


          var xUsedForFit = new HashSet<double>(); // x-values that were used for the fit
                                                   // Create a fit curve
                                                   // Note that we process each region individually
          var yFitValues = new double[entry.xArray.Length];
          var idxOutputRow = 0;
          for (var idxRegion = 0; idxRegion < fitResults.Count; idxRegion++)
          {
            var descriptionsOfRegion = fitResults[idxRegion].PeakDescriptions;
            var regionStart = fitResults[idxRegion].StartOfRegion;
            var regionEnd = fitResults[idxRegion].EndOfRegion;
            var regionLength = regionEnd - regionStart;

            double[] localXValues = GetXArrayWithResolution(entry.xArray, regionStart, regionEnd, doc.OutputOptions.OutputFitCurveSamplingFactor);

            var ySumValues = new double[localXValues.Length];
            var yScratch = new double[localXValues.Length];
            for (var pi = 0; pi < descriptionsOfRegion.Count; pi++)
            {
              var peakDescription = descriptionsOfRegion[pi];
              if (peakDescription.FitFunction is IFitFunctionPeak fitFunction)
              {
                fitFunction = fitFunction.WithNumberOfTerms(1).WithOrderOfBaselinePolynomial(-1);
                var para = peakDescription.PeakParameter;
                fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(localXValues), para, VectorMath.ToVector(yScratch), null);
                VectorMath.Add(ySumValues, yScratch, ySumValues);

                for (int j = peakDescription.FirstFitPoint; j <= peakDescription.LastFitPoint; ++j)
                {
                  xUsedForFit.Add(entry.xArray[j]);
                }
              }
            }

            // put the data directly into the column
            for (int i = 0; i < localXValues.Length; ++i, ++idxOutputRow)
            {
              cXFit[idxOutputRow] = localXValues[i];
              cYFit[idxOutputRow] = ySumValues[i];
            }
          }

          for (int i = 0; i < xArr.Length; ++i)
          {
            cYUsedForFit[i] = xUsedForFit.Contains(xArr[i]) ? 1 : 0;
          }
        }

        if (doc.OutputOptions.OutputFitCurveAsSeparatePeaks && fitResults.Count > 0)
        {
          // output each peak separately
          // since we don't want extra columns for every peak, we put all peaks into 3 columns
          // i.e. x-column, y-column, and identifier colum
          // the values of each peak are separated by a blank row, thus a line will not connect
          // the identifier column can be used to color each fit line separately

          var cXFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameX(runningColumnNumber), typeof(DoubleColumn), ColumnKind.X, groupNumberBase + 3);
          var cYFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameY(runningColumnNumber), typeof(DoubleColumn), ColumnKind.V, groupNumberBase + 3);
          var cIDFit = (DoubleColumn)peakTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameID(runningColumnNumber), typeof(DoubleColumn), ColumnKind.V, groupNumberBase + 3);

          idxRow = 0;
          var idxPeak = 0;
          for (var idxRegion = 0; idxRegion < fitResults.Count; idxRegion++)
          {
            var descriptionsOfRegion = fitResults[idxRegion].PeakDescriptions;
            var regionStart = fitResults[idxRegion].StartOfRegion;
            var regionEnd = fitResults[idxRegion].EndOfRegion;
            var regionLength = regionEnd - regionStart;

            var xArrOfRegion = new double[regionLength];
            Array.Copy(xArr, regionStart, xArrOfRegion, 0, regionLength);
            var xMinimumOfRegion = xArrOfRegion.Min();
            var xMaximumOfRegion = xArrOfRegion.Max();

            var ySumValues = new double[regionLength];
            var yScratch = new double[regionLength];
            for (var pi = 0; pi < descriptionsOfRegion.Count; pi++)
            {
              var peakDescription = descriptionsOfRegion[pi];
              if (peakDescription.FitFunction is IFitFunctionPeak fitFunction)
              {
                fitFunction = fitFunction.WithNumberOfTerms(1).WithOrderOfBaselinePolynomial(-1);

                var peakProperties = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(peakDescription.PeakParameter);

                if (peakProperties.Height == 0)
                  continue;

                double widthScale, leftPos, rightPos;
                var centerPos = peakProperties.Position;
                if (peakDescription.FirstFitPosition == xMinimumOfRegion && peakDescription.LastFitPosition == xMaximumOfRegion)
                {
                  // we have a method that uses the entire region for the fit
                  widthScale = 5;
                  leftPos = centerPos - peakProperties.FWHM * widthScale * 0.5;
                  rightPos = centerPos + peakProperties.FWHM * widthScale * 0.5;
                }
                else
                {
                  // we have a method that uses only some points around the center position for the fit
                  widthScale = Math.Abs((peakDescription.LastFitPosition - peakDescription.FirstFitPosition) / peakDescription.SearchDescription.WidthValue);
                  leftPos = Math.Min(centerPos - peakProperties.FWHM * widthScale * 0.5, peakDescription.FirstFitPosition);
                  rightPos = Math.Max(centerPos + peakProperties.FWHM * widthScale * 0.5, peakDescription.LastFitPosition);
                }


                var xValues = GetXValuesForPeak(xArrOfRegion, xMinimumOfRegion, xMaximumOfRegion, leftPos, centerPos, rightPos, doc.OutputOptions.OutputFitCurveAsSeparatePeaksSamplingFactor);
                yScratch = new double[xValues.Length];
                fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(xValues), peakDescription.PeakParameter, VectorMath.ToVector(yScratch), null);

                // output the values of a single peak
                for (int j = 0; j < xValues.Length; j++)
                {
                  cXFit[idxRow] = xValues[j];
                  cYFit[idxRow] = yScratch[j];
                  cIDFit[idxRow] = idxPeak;
                  ++idxRow;
                }

                {
                  // leave one row empty after output of a single peak
                  cXFit[idxRow] = double.NaN;
                  cYFit[idxRow] = double.NaN;
                  cIDFit[idxRow] = double.NaN;
                  ++idxRow;
                }

                ++idxPeak;
              }
            }
          }
          // finally, divide the identifier column by the number of peaks
          for (int i = 0; i < cIDFit.Count; i++)
          {
            if (!cIDFit.IsElementEmpty(i))
            {
              cIDFit[i] = ((((int)cIDFit[i] * (idxPeak + 1)) / 4) % idxPeak) / ((double)idxPeak - 1);
            }
          }
        }


        resultList.Add((entry.xOrgCol, entry.yOrgCol, entry.xPreprocessedCol, entry.yPreprocessedCol, fitResults));
      }


      return resultList;
    }

    private static double[] GetXArrayWithResolution(double[] xArray, int regionStart, int regionEnd, int outputFitCurveSamplingFactor)
    {
      // the region end is exclusive
      int numberOfPoints = (regionEnd - regionStart - 1) * outputFitCurveSamplingFactor + 1;

      var result = new double[numberOfPoints];
      int idxRow = 0;
      for (int i = regionStart + 1; i < regionEnd; ++i)
      {
        for (int j = 0; j < outputFitCurveSamplingFactor; ++j, ++idxRow)
        {
          double r = j / (double)outputFitCurveSamplingFactor;
          var x = (1 - r) * xArray[i - 1] + (r) * xArray[i];
          result[idxRow] = x;
        }
      }
      result[result.Length - 1] = xArray[regionEnd - 1];
      return result;
    }

    /// <summary>
    /// Gets the x values for a single peak.
    /// </summary>
    /// <param name="originalXValues">The original x values.</param>
    /// <param name="minimalX">The minimal x.</param>
    /// <param name="maximalX">The maximal x.</param>
    /// <param name="left">The left.</param>
    /// <param name="center">The center.</param>
    /// <param name="right">The right.</param>
    /// <param name="sampling">The sampling.</param>
    /// <returns></returns>
    private static double[] GetXValuesForPeak(double[] originalXValues, double minimalX, double maximalX, double left, double center, double right, int sampling)
    {
      var xList = new HashSet<double>();
      if (left >= maximalX || right <= minimalX)
        return new double[0];

      // use the sampling of the original x-axis when the peak width is less than the range of the spectrum
      if (Math.Abs(right - left) <= Math.Abs(maximalX - minimalX))
      {
        // Add center and endpoints
        xList.Add(Math.Max(left, minimalX));
        xList.Add(center);
        xList.Add(Math.Min(right, maximalX));
        int centerIdx = originalXValues.IndexOfMin(x => Math.Abs(x - center));
        if (centerIdx < 0)
          return new double[0];


        for (int i = centerIdx; i > 0; --i)
        {
          var x0 = originalXValues[i];
          var x1 = originalXValues[i - 1];
          for (int j = 0; j < sampling; ++j)
          {
            double r = j / (double)(sampling);
            var x = (1 - r) * x0 + (r) * x1;
            if (x >= left)
              xList.Add(x);
            else
              break;
          }
        }
        for (int i = centerIdx; i < originalXValues.Length - 1; ++i)
        {
          var x0 = originalXValues[i];
          var x1 = originalXValues[i + 1];
          for (int j = 0; j < sampling; ++j)
          {
            double r = j / (double)(sampling);
            var x = (1 - r) * x0 + (r) * x1;
            if (x <= right)
              xList.Add(x);
            else
              break;
          }
        }
      }
      else
      {
        // if the peak is broader than the range of the spectrum, we use not more than
        // 40 points per FWHM to sample its behavior
        var leftBound = Math.Max(left, minimalX);
        var rightBound = Math.Min(right, maximalX);
        int length = 40;
        for (int i = 0; i <= length; ++i)
        {
          double r = i / (double)length;
          var x = (1 - r) * leftBound + (r) * rightBound;
          xList.Add(x);
        }
      }

      var result = xList.ToArray();
      Array.Sort(result);
      return result;
    }

    public static void SpectralPeakFindingFittingShowDialog(WorksheetController ctrl)
    {
      if (!TryGetDataProxyForSpectralPreprocessing(ctrl, out var dataProxy))
        return;

      if (!ShowDialogGetPeakFindingFittingOptions(ctrl, out var peakSearchingFittingOptions))
        return;

      // now process the data
      var srcTable = ctrl.DataTable;
      var peakTable = new DataTable();


      List<(
      DataColumn xOrgCol,
      DataColumn yOrgCol,
      DataColumn xPreprocessedCol,
      DataColumn yPreprocessedCol,
      IReadOnlyList<(IReadOnlyList<PeakFitting.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> fittingResult)> result = null;

      var progressMonitor = new ExternalDrivenBackgroundMonitor();

      var fitTask = System.Threading.Tasks.Task.Run(() =>
      {
        try
        {
          result = ExecutePeakFindingAndFitting(dataProxy, peakSearchingFittingOptions, peakTable, progressMonitor, progressMonitor.CancellationToken, progressMonitor.CancellationTokenHard);
        }
        catch (OperationCanceledException)
        {
        }
      }
      );

      // Execute peak searching & fitting
      Current.Gui.ShowTaskCancelDialog(10000, fitTask, progressMonitor);
      if (result is null)
      {
        return;
      }

      {
        // name the peak table, and add it to the project
        var dstName = srcTable.Name + "_Peaks";
        if (Current.Project.DataTableCollection.Contains(dstName))
          dstName = Current.Project.DataTableCollection.FindNewItemName(dstName);
        peakTable.Name = dstName;
        Current.Project.DataTableCollection.Add(peakTable);
        Current.ProjectService.OpenOrCreateWorksheetForTable(peakTable);


        peakTable.DataSource = new PeakSearchingAndFittingDataSource(
          (DataTableMultipleColumnProxy)dataProxy.Clone(),
          peakSearchingFittingOptions,
          new DataSourceImportOptions());
      }

      // -----------------------------------------------------------------------------
      // Peak plotting
      // -----------------------------------------------------------------------------

      int numberOfSpectrum = -1;
      foreach (var resultForOneColumn in result)
      {
        ++numberOfSpectrum;

        if (peakSearchingFittingOptions.OutputOptions.OutputPreprocessedCurve && (peakSearchingFittingOptions.OutputOptions.OutputFitCurve || peakSearchingFittingOptions.OutputOptions.OutputFitCurveAsSeparatePeaks))
        {
          var selColumnsForPeakGraph = new AscendingIntegerCollection
          {
            peakTable.DataColumns.GetColumnNumber(peakTable[PeakTable_PreprocessedColumnNameY(numberOfSpectrum)])
          };
          var preferredGraphName = peakTable.FolderName + "GPeaks";
          var graphController = PlotCommands.PlotLine(peakTable, selColumnsForPeakGraph, bLine: false, bScatter: true, preferredGraphName);
          var graph = graphController.Doc;
          var layer = (XYPlotLayer)graph.RootLayer.Layers[0];
          var pi = layer.PlotItems.TakeFromHereToFirstLeaves<IGPlotItem>().OfType<XYColumnPlotItem>().FirstOrDefault();
          if (pi is not null)
          {
            var scatter = pi.Style.OfType<ScatterPlotStyle>().FirstOrDefault();
            if (scatter is not null)
            {
              scatter.SymbolSize /= 4; // use only a quarter of the symbol size of a usual scatter plot
            }
          }

          PlotItemCollection group;
          if (pi?.ParentCollection is not null)
          {
            group = pi.ParentCollection;
          }
          else
          {
            group = new PlotItemCollection();
            layer.PlotItems.Add(group);
          }

          if (peakSearchingFittingOptions.PeakFitting is not PeakFitting.PeakFittingNone)
          {
            if (peakSearchingFittingOptions.OutputOptions.OutputFitCurveAsSeparatePeaks)
            {
              // plot the separate peaks
              var plotStyle = PlotCommands.PlotStyle_Line(graph.GetPropertyContext());
              var lineStyle = plotStyle.OfType<LinePlotStyle>().FirstOrDefault();
              if (lineStyle is not null)
              {
                var varColorStyle = new ColumnDrivenColorPlotStyle();
                varColorStyle.DataColumn = peakTable[PeakTable_SeparatePeaksColumnNameID(numberOfSpectrum)];
                plotStyle.Insert(0, varColorStyle);

                var xColumn = peakTable.DataColumns[PeakTable_SeparatePeaksColumnNameX(numberOfSpectrum)];
                var yColumn = peakTable.DataColumns[PeakTable_SeparatePeaksColumnNameY(numberOfSpectrum)];
                var groupNumber = peakTable.DataColumns.GetColumnGroup(yColumn);
                var plotData = new XYColumnPlotData(peakTable, groupNumber, xColumn, yColumn);
                var plotItem = new XYColumnPlotItem(plotData, plotStyle);
                group.Add(plotItem);
              }
            }
            else if (peakSearchingFittingOptions.OutputOptions.OutputFitCurve)
            {
              // plot the fit curve
              var plotStyle = PlotCommands.PlotStyle_Line(graph.GetPropertyContext());
              var lineStyle = plotStyle.OfType<LinePlotStyle>().FirstOrDefault();
              if (lineStyle is not null && lineStyle.Color.ParentColorSet is { } parentCSet)
              {
                var xColumn = peakTable.DataColumns[PeakTable_FitCurveColumnNameX(numberOfSpectrum)];
                var yColumn = peakTable.DataColumns[PeakTable_FitCurveColumnNameY(numberOfSpectrum)];
                var groupNumber = peakTable.DataColumns.GetColumnGroup(yColumn);
                var plotData = new XYColumnPlotData(peakTable, groupNumber, xColumn, yColumn);
                var plotItem = new XYColumnPlotItem(plotData, plotStyle);
                group.Add(plotItem);
              }
            }
          }
        }
      }
    }

    public static bool TryGetDataProxyForSpectralPreprocessing(WorksheetController ctrl, out DataTableMultipleColumnProxy? proxy)
    {
      if (ctrl.SelectedDataColumns.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select one or more data columns from one group!");
        proxy = null;
        return false;
      }

      var srcTable = ctrl.DataTable;
      var col = ctrl.DataTable.DataColumns;
      var groupNumber = col.GetColumnGroup(col[ctrl.SelectedDataColumns[0]]);
      var xColumn = col.FindXColumnOfGroup(groupNumber);

      if (xColumn is null)
      {
        Current.Gui.ErrorMessageBox($"Please designate one column of group {groupNumber} to be the x-column!");
        proxy = null;
        return false;
      }

      proxy = new DataTableMultipleColumnProxy(ColumnsV, srcTable, null, ctrl.SelectedDataColumns);
      proxy.EnsureExistenceOfIdentifier(ColumnX, 1);
      proxy.AddDataColumn(ColumnX, xColumn);

      return true;
    }

    public static void SpectralPreprocessingShowDialog(WorksheetController ctrl)
    {
      if (!TryGetDataProxyForSpectralPreprocessing(ctrl, out var dataProxy))
        return;

      if (!ShowDialogGetPreprocessingOptions(ctrl, out var doc))
        return;

      // now process the data
      var srcTable = ctrl.DataTable;
      var preprocessingTable = new DataTable();

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
          doc,
          new DataSourceImportOptions());
      }
    }

    public static void Raman_CalibrateWithNeonSpectrum(WorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select the column containing the intensity of the Neon spectrum");
        return;
      }
      if (ctrl.SelectedDataColumns.Count > 1)
      {
        Current.Gui.ErrorMessageBox("Please select only the one column containing the intensity of the Neon spectrum");
        return;
      }

      var y_column = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]];
      var x_column = ctrl.DataTable.DataColumns.FindXColumnOf(y_column);



      if (x_column is null)
      {
        Current.Gui.ErrorMessageBox("Could not find x-column corresponding to spectrum. Please set the kind of this column to 'X'");
        return;
      }

      var doc = new NeonCalibrationOptionsAndDestinationTable();
      var controller = new OptionsAndDestinationTableController<NeonCalibrationOptions>();
      controller.InitializeDocument(doc);
      if (!Current.Gui.ShowDialog(controller, "Choose options for Neon calibration"))
        return;

      doc = (NeonCalibrationOptionsAndDestinationTable)controller.ModelObject;
      var dstTable = doc.DestinationTable;

      if (dstTable is null)
      {
        dstTable = new DataTable();
        dstTable.Name = ctrl.DataTable.FolderName + "WRamanCalibration";
        dstTable.DataSource = new RamanCalibrationDataSource(new DataSourceImportOptions());
        Current.Project.DataTableCollection.Add(dstTable);
      }

      var dataSource = (RamanCalibrationDataSource)dstTable.DataSource;
      var proxy = new DataTableXYColumnProxy(ctrl.DataTable, x_column, y_column, null);
      if (dataSource.IsNeonCalibration1Empty)
        dataSource.SetNeonCalibration1(doc.Options, proxy);
      else
        dataSource.SetNeonCalibration2(doc.Options, proxy);

      var backgroundMonitor = new ExternalDrivenBackgroundMonitor();
      var task = System.Threading.Tasks.Task.Run(() => dataSource.FillData(dstTable, backgroundMonitor.CancellationTokenHard));
      Current.Gui.ShowTaskCancelDialog(5000, task, backgroundMonitor);
      if (task.IsFaulted || task.IsCanceled)
      {
        Current.Gui.ErrorMessageBox("The Neon calibration task has not completed successfully, thus the calibration table may be corrupted!");
      }

      Current.ProjectService.OpenOrCreateWorksheetForTable(dstTable);

    }

    /// <summary>
    /// Does the relative part of a Raman calibration by utilizing a silicon spectrum.
    /// </summary>
    /// <param name="ctrl">The worksheet controller.</param>
    public static void Raman_CalibrateWithSiliconSpectrum(WorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select the column containing the intensity of the Silicon spectrum");
        return;
      }
      if (ctrl.SelectedDataColumns.Count > 1)
      {
        Current.Gui.ErrorMessageBox("Please select only the one column containing the intensity of the Silicon spectrum");
        return;
      }

      var y_column = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]];
      var x_column = ctrl.DataTable.DataColumns.FindXColumnOf(y_column);

      var len = Math.Min(x_column.Count, y_column.Count);

      if (x_column is null)
      {
        Current.Gui.ErrorMessageBox("Could not find x-column corresponding to spectrum. Please set the kind of this column to 'X'");
        return;
      }


      var doc = new SiliconCalibrationOptionsAndDestinationTable();
      var controller = new OptionsAndDestinationTableController<SiliconCalibrationOptions>();
      controller.InitializeDocument(doc);
      if (!Current.Gui.ShowDialog(controller, "Choose options for Silicon calibration"))
        return;

      doc = (SiliconCalibrationOptionsAndDestinationTable)controller.ModelObject;
      var dstTable = doc.DestinationTable;

      if (dstTable is null)
      {
        dstTable = new DataTable();
        dstTable.Name = ctrl.DataTable.FolderName + "WRamanCalibration";

        dstTable.DataSource = new RamanCalibrationDataSource(new DataSourceImportOptions());
        Current.Project.DataTableCollection.Add(dstTable);
      }

      var dataSource = (RamanCalibrationDataSource)dstTable.DataSource;
      var proxy = new DataTableXYColumnProxy(ctrl.DataTable, x_column, y_column, null);
      dataSource.SetSiliconCalibration(doc.Options, proxy);


      var backgroundMonitor = new ExternalDrivenBackgroundMonitor();
      var task = System.Threading.Tasks.Task.Run(() => dataSource.FillData(dstTable, backgroundMonitor.CancellationTokenHard));
      Current.Gui.ShowTaskCancelDialog(5000, task, backgroundMonitor);
      if (task.IsFaulted || task.IsCanceled)
      {
        Current.Gui.ErrorMessageBox("The Silicon calibration task has not completed successfully, thus the calibration table may be corrupted!");
      }


      Current.ProjectService.OpenOrCreateWorksheetForTable(dstTable);
    }
  }
}
