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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;

namespace Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  /// <summary>
  /// Table data source for applying the <see cref="PeakSearchingAndFittingOptions"/> to columns of a table.
  /// </summary>
  /// <seealso cref="Altaxo.Data.TableDataSourceBase" />
  /// <seealso cref="Altaxo.Data.IAltaxoTableDataSource" />
  public class PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode _processOptions;
    private ListOfXAndYColumn _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization



    #region Version 0

    /// <summary>
    /// 2024-09-11 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (ListOfXAndYColumn)info.GetValue("ProcessData", this));
      ChildSetMember(ref _processOptions, (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode)info.GetValue("ProcessOptions", this));
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 0

    protected PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertXYVToMatrixDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
    /// <param name="dataSourceOptions">The Fourier transformation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="System.ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource(ListOfXAndYColumn inputData, PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _processOptions, new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode(dataSourceOptions));
      ChildSetMember(ref _processData, inputData);
      ChildSetMember(ref _importOptions, importOptions);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    public void CopyFrom(PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null SpectralPreprocessingDataSource when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var token = SuspendGetToken())
      {
        ListOfXAndYColumn? processData = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        CopyHelper.Copy(ref processData, from._processData);

        ProcessOptions = from.ProcessOptions;
        ImportOptions = importOptions;
        ProcessData = processData;
      }
    }

    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>True</c> if anything could be copied from the object, otherwise <c>false</c>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource from)
      {
        CopyFrom(from);

        return true;

      }
      return false;
    }



    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      var peakFindingAndFittingOptions = _processOptions.GetPeakSearchingAndFittingOptions();

      var listXY = _processData.CurveData;

      // preprocess the data
      var preprocessedSpectra = new List<(double[] X, double[] Y)>();
      foreach (var curve in listXY)
      {
        var (x, y, rowCount) = curve.GetResolvedXYData();
        var (xpre, ypre, regions) = peakFindingAndFittingOptions.Preprocessing.Execute(x, y, null);
        preprocessedSpectra.Add((xpre, ypre));
      }

      // now do the peak fitting
      var peakResults = peakFindingAndFittingOptions.PeakFitting.Execute(preprocessedSpectra, reporter.CancellationToken, reporter, reporter);

      // and now the output
      {
        destinationTable.DataColumns.RemoveColumnsAll();
        destinationTable.PropCols.RemoveColumnsAll();

        int groupNumber = 0;
        var outputOptions = peakFindingAndFittingOptions.OutputOptions;
        var numberOfSpectra = peakResults.NumberOfSpectra;


        // ***************************************
        // Retrieve the properties
        // ***************************************

        var columnProperties = new AltaxoVariant[outputOptions.PropertyNames.Count][];
        var columnPropertyColumns = new DataColumn[outputOptions.PropertyNames.Count];
        for (int idxProperty = 0; idxProperty < outputOptions.PropertyNames.Count; idxProperty++)
        {
          var propertyName = outputOptions.PropertyNames[idxProperty];
          columnProperties[idxProperty] = new AltaxoVariant[numberOfSpectra];
          bool isNumeric = true;
          for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; ++idxSpectrum)
          {
            var pvalue = IndependentAndDependentColumns.GetPropertyValueOfCurve(_processData.CurveData[idxSpectrum], propertyName);
            columnProperties[idxProperty][idxSpectrum] = pvalue;
            isNumeric &= pvalue.CanConvertedToDouble;
          }
          columnPropertyColumns[idxProperty] = destinationTable.PropCols.EnsureExistence(propertyName, isNumeric ? typeof(DoubleColumn) : typeof(TextColumn), ColumnKind.V, 0);
        }

        // ***************************************
        //     output the peaks
        // ***************************************

        // and second, the columns for the peak fit results
        var cFNot = destinationTable.DataColumns.EnsureExistence($"FitNotes", typeof(TextColumn), ColumnKind.V, groupNumber);
        var cFPos = destinationTable.DataColumns.EnsureExistence($"FitPosition", typeof(DoubleColumn), ColumnKind.X, groupNumber);
        var cFPosVar = destinationTable.DataColumns.EnsureExistence($"FitPosition.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
        var cFWid = destinationTable.DataColumns.EnsureExistence($"FitFwhm", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        var cFWidVar = destinationTable.DataColumns.EnsureExistence($"FitFwhm.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);

        var cFArea = new DataColumn[numberOfSpectra];
        var cFAreaVar = new DataColumn[numberOfSpectra];
        var cFHei = new DataColumn[numberOfSpectra];
        var cFHeiVar = new DataColumn[numberOfSpectra];
        for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; idxSpectrum++)
        {
          cFArea[idxSpectrum] = destinationTable.DataColumns.EnsureExistence($"FitArea{idxSpectrum}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          cFAreaVar[idxSpectrum] = destinationTable.DataColumns.EnsureExistence($"FitArea{idxSpectrum}.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
          cFHei[idxSpectrum] = destinationTable.DataColumns.EnsureExistence($"FitHeight{idxSpectrum}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
          cFHeiVar[idxSpectrum] = destinationTable.DataColumns.EnsureExistence($"FitHeight{idxSpectrum}.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
        }
        //var cFirstXValue = destinationTable.DataColumns.EnsureExistence($"FitFirstXValue", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        //var cLastXValue = destinationTable.DataColumns.EnsureExistence($"FitLastXValue", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        var cSumChiSquare = destinationTable.DataColumns.EnsureExistence($"SumChiSquare", typeof(DoubleColumn), ColumnKind.V, groupNumber);
        var cSigmaSquare = destinationTable.DataColumns.EnsureExistence($"SigmaSquare", typeof(DoubleColumn), ColumnKind.V, groupNumber);

        // set the properties for the columns which are separate for each spectrum
        for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
        {
          var pcol = columnPropertyColumns[idxProperty];
          for (int idxSpectrum = 0; idxSpectrum < numberOfSpectra; idxSpectrum++)
          {
            pcol[destinationTable.DataColumns.GetColumnNumber(cFArea[idxSpectrum])] = columnProperties[idxProperty][idxSpectrum];
            pcol[destinationTable.DataColumns.GetColumnNumber(cFAreaVar[idxSpectrum])] = columnProperties[idxProperty][idxSpectrum];
            pcol[destinationTable.DataColumns.GetColumnNumber(cFHei[idxSpectrum])] = columnProperties[idxProperty][idxSpectrum];
            pcol[destinationTable.DataColumns.GetColumnNumber(cFHeiVar[idxSpectrum])] = columnProperties[idxProperty][idxSpectrum];
          }
        }

        double[] localParam = null;
        IROMatrix<double> localCovariances = null;

        var descriptions = peakResults.PeakDescriptions;
        for (var idxPeak = 0; idxPeak < descriptions.Count; idxPeak++)
        {
          var r = descriptions[idxPeak];
          if (r.FitFunction is { } fitFunction)
          {
            for (int idxSpectrum = 0; idxSpectrum < peakResults.NumberOfSpectra; idxSpectrum++)
            {
              localParam = r.GetPeakParametersOfSpectrum(idxSpectrum);
              localCovariances = r.GetPeakParameterCovariancesForSpectrum(idxSpectrum);
              var (pos, posVar, area, areaVar, height, heightVar, fwhm, fwhmVar) = fitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(localParam, localCovariances);

              cFPos[idxPeak] = pos;
              cFPosVar[idxPeak] = posVar;
              cFArea[idxSpectrum][idxPeak] = area;
              cFAreaVar[idxSpectrum][idxPeak] = areaVar;
              cFHei[idxSpectrum][idxPeak] = height;
              cFHeiVar[idxSpectrum][idxPeak] = heightVar;
              cFWid[idxPeak] = fwhm;
              cFWidVar[idxPeak] = fwhmVar;
              //cNumberOfPoints[idxPeak] = numberOfFitPoints;
              //cFirstXValue[idxPeak] = r.FirstFitPosition;
              //cLastXValue[idxPeak] = r.LastFitPosition;
              cSumChiSquare[idxPeak] = r.SumChiSquare;
              cSigmaSquare[idxPeak] = r.SigmaSquare;

            }

            // now output the raw parameters
            var parameterNames = fitFunction.ParameterNamesForOnePeak;



            // at first, output the numberOfSpectra amplitudes
            for (int idxSpectrum = 0; idxSpectrum < peakResults.NumberOfSpectra; idxSpectrum++)
            {
              localParam = r.GetPeakParametersOfSpectrum(idxSpectrum);
              localCovariances = r.GetPeakParameterCovariancesForSpectrum(idxSpectrum);

              var cParaValue = destinationTable.DataColumns.EnsureExistence($"{parameterNames[0]}{idxSpectrum}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
              var cParaStdError = destinationTable.DataColumns.EnsureExistence($"{parameterNames[0]}{idxSpectrum}.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);
              cParaValue[idxPeak] = localParam[0];
              cParaStdError[idxPeak] = localCovariances is null ? 0 : Math.Sqrt(localCovariances[0, 0]);

              for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
              {
                columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(cParaValue)] = columnProperties[idxProperty][idxSpectrum];
                columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(cParaStdError)] = columnProperties[idxProperty][idxSpectrum];
              }
            }

            // now the rest of the parameters
            for (var j = 1; j < parameterNames.Length; j++)
            {
              var cParaValue = destinationTable.DataColumns.EnsureExistence($"{parameterNames[j]}", typeof(DoubleColumn), ColumnKind.V, groupNumber);
              var cParaStdError = destinationTable.DataColumns.EnsureExistence($"{parameterNames[j]}.Err", typeof(DoubleColumn), ColumnKind.Err, groupNumber);

              cParaValue[idxPeak] = localParam[j];
              cParaStdError[idxPeak] = localCovariances is null ? 0 : Math.Sqrt(localCovariances[j, j]);
            }
          }
        }


        // ***************************************
        //     output the preprocessed curves
        // ***************************************
        if (outputOptions.OutputPreprocessedCurve)
        {
          groupNumber = (int)(1000 * Math.Ceiling((Math.Max(0, groupNumber) + 1) / 1000d) - 1);
          for (int idxSpectrum = 0; idxSpectrum < preprocessedSpectra.Count; idxSpectrum++)
          {
            ++groupNumber;
            var destX = destinationTable.DataColumns.EnsureExistence(PeakTable_PreprocessedColumnNameX(idxSpectrum), typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var destY = destinationTable.DataColumns.EnsureExistence(PeakTable_PreprocessedColumnNameY(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);

            destX.Data = preprocessedSpectra[idxSpectrum].X;
            destY.Data = preprocessedSpectra[idxSpectrum].Y;

            for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
            {
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destX)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destY)] = columnProperties[idxProperty][idxSpectrum];
            }
          }
        }

        // ***************************************
        //     output the fit curves
        // ***************************************
        if (outputOptions.OutputFitCurve)
        {
          groupNumber = (int)(1000 * Math.Ceiling((Math.Max(0, groupNumber) + 1) / 1000d) - 1);

          for (int idxSpectrum = 0; idxSpectrum < preprocessedSpectra.Count; idxSpectrum++)
          {
            ++groupNumber;
            var destX = destinationTable.DataColumns.EnsureExistence(PeakTable_FitCurveColumnNameX(idxSpectrum), typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var destY = destinationTable.DataColumns.EnsureExistence(PeakTable_FitCurveColumnNameY(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);

            var parameter = peakResults.GetFullParameterSetForSpectrum(idxSpectrum);
            var y = Vector<double>.Build.Dense(preprocessedSpectra[idxSpectrum].Y.Length);
            peakResults.FitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(preprocessedSpectra[idxSpectrum].X), parameter, y, null);

            destX.Data = preprocessedSpectra[idxSpectrum].X;
            destY.Data = y;

            for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
            {
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destX)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destY)] = columnProperties[idxProperty][idxSpectrum];
            }
          }
        }

        // ********************************************
        //     output the fit curves as separate peaks
        // ********************************************
        if (outputOptions.OutputFitCurveAsSeparatePeaks)
        {
          groupNumber = (int)(1000 * Math.Ceiling((Math.Max(0, groupNumber) + 1) / 1000d) - 1);
          var fitWidthScalingFactor = _processOptions.GetPeakSearchingAndFittingOptions().PeakFitting.FitWidthScalingFactor;
          var numberOfPeaks = peakResults.NumberOfPeaks;
          var fitFunc = peakResults.FitFunction.WithNumberOfTerms(1);
          for (int idxSpectrum = 0; idxSpectrum < preprocessedSpectra.Count; idxSpectrum++)
          {
            ++groupNumber;
            var destX = destinationTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameX(idxSpectrum), typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var destY = destinationTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameY(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);
            var destZ = destinationTable.DataColumns.EnsureExistence(PeakTable_SeparatePeaksColumnNameID(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);

            for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
            {
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destX)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destY)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destZ)] = columnProperties[idxProperty][idxSpectrum];
            }

            int idxRow = 0;
            for (int idxPeak = 0; idxPeak < peakResults.NumberOfPeaks; ++idxPeak)
            {
              var pahf = peakResults.PeakDescriptions[idxPeak].GetPositionAreaHeightFWHMOfSpectrum(idxSpectrum);
              var parameter = peakResults.GetParametersForOnePeakInclusiveBaselineForSpectrum(idxSpectrum, idxPeak);
              var x = preprocessedSpectra[idxSpectrum].X;
              var y = Vector<double>.Build.Dense(preprocessedSpectra[idxSpectrum].Y.Length);
              fitFunc.Evaluate(MatrixMath.ToROMatrixWithOneColumn(preprocessedSpectra[idxSpectrum].X), parameter, y, null);

              var lowerBound = pahf.Position - pahf.FWHM * (fitWidthScalingFactor ?? 1000);
              var upperBound = pahf.Position + pahf.FWHM * (fitWidthScalingFactor ?? 1000);

              for (int i = 0; i < x.Length; ++i)
              {
                if (RMath.IsInIntervalCC(x[i], lowerBound, upperBound))
                {
                  destX[idxRow] = x[i];
                  destY[idxRow] = y[i];
                  destZ[idxRow] = numberOfPeaks <= 1 ? 0 : (((idxPeak * (numberOfPeaks + 1)) / 4) % numberOfPeaks) / ((double)numberOfPeaks - 1);
                  ++idxRow;
                }
              }
              ++idxRow; // one empty row between each peak
            }
          }
        }

        // ***************************************
        //     output the baseline curves
        // ***************************************
        if (outputOptions.OutputBaselineCurve)
        {
          groupNumber = (int)(1000 * Math.Ceiling((Math.Max(0, groupNumber) + 1) / 1000d) - 1);
          var fitFunction = peakResults.FitFunction.WithNumberOfTerms(0); // only baseline
          for (int idxSpectrum = 0; idxSpectrum < preprocessedSpectra.Count; idxSpectrum++)
          {
            ++groupNumber;
            var destX = destinationTable.DataColumns.EnsureExistence(PeakTable_BaselineCurveColumnNameX(idxSpectrum), typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var destY = destinationTable.DataColumns.EnsureExistence(PeakTable_BaselineCurveColumnNameY(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);

            var parameter = peakResults.GetBaselineParametersForSpectrum(idxSpectrum);
            var y = Vector<double>.Build.Dense(preprocessedSpectra[idxSpectrum].Y.Length);
            fitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(preprocessedSpectra[idxSpectrum].X), parameter, y, null);

            destX.Data = preprocessedSpectra[idxSpectrum].X;
            destY.Data = y;

            for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
            {
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destX)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destY)] = columnProperties[idxProperty][idxSpectrum];
            }
          }
        }

        // ***************************************
        //     output the residual curves
        // ***************************************
        if (outputOptions.OutputFitResidualCurve)
        {
          groupNumber = (int)(1000 * Math.Ceiling((Math.Max(0, groupNumber) + 1) / 1000d) - 1);

          for (int idxSpectrum = 0; idxSpectrum < preprocessedSpectra.Count; idxSpectrum++)
          {
            ++groupNumber;
            var destX = destinationTable.DataColumns.EnsureExistence(PeakTable_ResidualCurveColumnNameX(idxSpectrum), typeof(DoubleColumn), ColumnKind.X, groupNumber);
            var destY = destinationTable.DataColumns.EnsureExistence(PeakTable_ResidualCurveColumnNameY(idxSpectrum), typeof(DoubleColumn), ColumnKind.V, groupNumber);

            var parameter = peakResults.GetFullParameterSetForSpectrum(idxSpectrum);
            var y = Vector<double>.Build.Dense(preprocessedSpectra[idxSpectrum].Y.Length);
            peakResults.FitFunction.Evaluate(MatrixMath.ToROMatrixWithOneColumn(preprocessedSpectra[idxSpectrum].X), parameter, y, null);

            destX.Data = preprocessedSpectra[idxSpectrum].X;
            for (int i = 0; i < y.Count; i++)
            {
              destY[i] = preprocessedSpectra[idxSpectrum].Y[i] - y[i];
            }

            for (int idxProperty = 0; idxProperty < columnPropertyColumns.Length; ++idxProperty)
            {
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destX)] = columnProperties[idxProperty][idxSpectrum];
              columnPropertyColumns[idxProperty][destinationTable.DataColumns.GetColumnNumber(destY)] = columnProperties[idxProperty][idxSpectrum];
            }
          }
        }
      }
    }

    #region Peak table output

    public static string PeakTable_PreprocessedColumnNameX(int numberOfSpectrum) => $"X_Preprocessed{numberOfSpectrum}";
    public static string PeakTable_PreprocessedColumnNameY(int numberOfSpectrum) => $"Y_Preprocessed{numberOfSpectrum}";
    public static string PeakTable_UsedForFitColumnName(int numberOfSpectrum) => $"UsedForFit{numberOfSpectrum}";

    public static string PeakTable_FitCurveColumnNameX(int numberOfSpectrum) => $"X_FitCurve{numberOfSpectrum}";
    public static string PeakTable_FitCurveColumnNameY(int numberOfSpectrum) => $"Y_FitCurve{numberOfSpectrum}";

    public static string PeakTable_BaselineCurveColumnNameX(int numberOfSpectrum) => $"X_BaselineCurve{numberOfSpectrum}";
    public static string PeakTable_BaselineCurveColumnNameY(int numberOfSpectrum) => $"Y_BaselineCurve{numberOfSpectrum}";

    public static string PeakTable_ResidualCurveColumnNameX(int numberOfSpectrum) => $"X_ResidualCurve{numberOfSpectrum}";
    public static string PeakTable_ResidualCurveColumnNameY(int numberOfSpectrum) => $"Y_ResidualCurve{numberOfSpectrum}";

    public static string PeakTable_SeparatePeaksColumnNameX(int numberOfSpectrum) => $"X_PeakCurves{numberOfSpectrum}";
    public static string PeakTable_SeparatePeaksColumnNameY(int numberOfSpectrum) => $"Y_PeakCurves{numberOfSpectrum}";
    public static string PeakTable_SeparatePeaksColumnNameID(int numberOfSpectrum) => $"ID_PeakCurves{numberOfSpectrum}";


    #endregion

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
    /// </summary>
    public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
    {
      add
      {
        bool isFirst = _dataSourceChanged is null;
        _dataSourceChanged += value;
        if (isFirst)
        {
          //EhInputDataChanged(this, EventArgs.Empty);
        }
      }
      remove
      {
        _dataSourceChanged -= value;
        bool isLast = _dataSourceChanged is null;
        if (isLast)
        {
        }
      }
    }

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    /// <value>
    /// The input data.
    /// </value>
    public ListOfXAndYColumn ProcessData
    {
      get
      {
        return _processData;
      }
      [MemberNotNull(nameof(_processData))]
      set
      {
        if (ChildSetMember(ref _processData, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_processData, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the data source import options.
    /// </summary>
    /// <value>
    /// The import options.
    /// </value>
    /// <exception cref="System.ArgumentNullException">ImportOptions</exception>
    public override Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (ChildSetMember(ref _importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the options for this data source.
    /// </summary>
    /// <value>
    /// The options for this data source.
    /// </value>
    public PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions ProcessOptions
    {
      get
      {
        return _processOptions.GetPeakSearchingAndFittingOptions();
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ProcessOptions));

        if (_processOptions is null || !object.Equals(_processOptions.GetPeakSearchingAndFittingOptions(), value))
        {
          ChildSetMember(ref _processOptions, new PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptionsDocNode(value));
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => ProcessOptions;
      set => ProcessOptions = (PeakFittingOfMultipleSpectraByIncrementalPeakAdditionOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (ListOfXAndYColumn)value;
    }

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (sender is not null && object.ReferenceEquals(_processData, sender)) // incoming call from data proxy
      {
        if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
        {
          e = TableDataSourceChangedEventArgs.Empty;
        }
        else
        {
          return true; // if option is not DataSourceChanged, absorb this event
        }
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Change event handling

    #region Document Node functions

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processOptions is not null)
        yield return new Main.DocumentNodeAndName(_processOptions, "ProcessOptions");

      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");

      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, "ImportOptions");
    }

    #endregion Document Node functions

    /// <summary>
    /// Called after deserization of a data source instance, when it is already associated with a data table.
    /// </summary>
    public void OnAfterDeserialization()
    {
    }

    /// <summary>
    /// Visits all document references.
    /// </summary>
    /// <param name="ReportProxies">The report proxies.</param>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
      _processData?.VisitDocumentReferences(ReportProxies);
      _processOptions?.VisitDocumentReferences(ReportProxies);
    }

    #endregion IAltaxoTableDataSource

  }
}
