#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Analysis.Statistics.Histograms
{
  public static class HistogramCreation
  {
    #region Internal classes

    private class NumericTableRegionEnumerator : IEnumerator<double>
    {
      private DataTable _srctable;
      private IAscendingIntegerCollection _selectedColumns;
      private IAscendingIntegerCollection _selectedRows;

      private int _nCol = -1;
      private int _nRow = 0;
      private INumericColumn? _col;

      public NumericTableRegionEnumerator(DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows)
      {
        _srctable = srctable;
        _selectedColumns = selectedColumns;
        _selectedRows = selectedRows;

        if (selectedColumns.Count == 0)
        {
          _selectedColumns = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, srctable.DataColumns.ColumnCount);
        }
        if (selectedRows.Count == 0)
        {
          _selectedRows = Altaxo.Collections.ContiguousIntegerRange.FromStartAndCount(0, srctable.DataColumns.RowCount);
        }
      }

      public double Current
      {
        get
        {
          if (_col is not null)
            return _col[_selectedRows[_nRow]];
          else
            throw new InvalidOperationException("Before or behind enumeration");
        }
      }

      public void Dispose()
      {
        _col = null;
      }

      object System.Collections.IEnumerator.Current
      {
        get
        {
          if (_col is not null)
            return _col[_selectedRows[_nRow]];
          else
            throw new InvalidOperationException("Before or behind enumeration");
        }
      }

      public bool MoveNext()
      {
        if (_col is not null)
        {
          if (_nRow < _selectedRows.Count - 1)
          {
            ++_nRow;
            return true;
          }
          else
          {
            ++_nCol;
            if (_nCol < _selectedColumns.Count)
            {
              _nRow = 0;
              _col = (INumericColumn)_srctable.DataColumns[_selectedColumns[_nCol]];
              return true;
            }
            else
            {
              _col = null;
              return false;
            }
          }
        }
        else // _col is null
        {
          if (_nCol < 0)
          {
            _nCol = 0;
            _nRow = 0;
            _col = (INumericColumn)_srctable.DataColumns[_selectedColumns[_nCol]];
            return true;
          }
        }
        return false;
      }

      public void Reset()
      {
        _col = null;
        _nCol = -1;
        _nRow = 0;
      }
    }

    #endregion Internal classes

    /// <summary>
    /// Calculates statistics of selected columns. Returns a new table where the statistical data will be written to.
    /// </summary>
    /// <param name="srctable">Source table.</param>
    /// <param name="selectedColumns">Selected data columns in the source table.</param>
    /// <param name="selectedRows">Selected rows in the source table.</param>
    /// <param name="userInteractionLevel">Determines the level of user interaction.</param>
    public static DataTable? CreateHistogramOnColumns(
      this DataTable srctable,
      IAscendingIntegerCollection selectedColumns,
      IAscendingIntegerCollection selectedRows,
      Gui.UserInteractionLevel userInteractionLevel
      )
    {
      if (srctable is null)
        throw new ArgumentNullException(nameof(srctable));

      var histInfo = new HistogramCreationInformation() { UserInteractionLevel = userInteractionLevel, OriginalDataEnsemble = new NumericTableRegionEnumerator(srctable, selectedColumns, selectedRows) };

      return CreateHistogram(srctable.Name + "-HistogramData", histInfo);
    }

    public static void CreateHistogram(ref DataTable destinationTable, HistogramCreationInformation histInfo)
    {
      if (histInfo is null)
        throw new ArgumentNullException(nameof(histInfo));

      var dataEnumerator = histInfo.OriginalDataEnsemble;
      var userInteractionLevel = histInfo.UserInteractionLevel;

      if (dataEnumerator is null)
        throw new ArgumentNullException("histInfo's DataEnsemble is null.");

      bool showDialog = PopulateHistogramCreationInformation(histInfo);

      if (showDialog)
      {
        if (!Current.Gui.ShowDialog(ref histInfo, "Binning options", true))
          return;
      }

      if (histInfo.FilteredAndSortedDataEnsemble is null || histInfo.CreationOptions.Binning is null)
        throw new InvalidProgramException();

      CreateHistogramTable(ref destinationTable, null, histInfo.FilteredAndSortedDataEnsemble, histInfo.CreationOptions.Binning);
    }

    public static DataTable? CreateHistogram(string proposedTableName, HistogramCreationInformation histInfo)
    {
      if (proposedTableName is null)
        throw new ArgumentNullException(nameof(proposedTableName));

      if (histInfo is null)
        throw new ArgumentNullException(nameof(histInfo));

      DataTable? destinationTable = null;

      bool showDialog = PopulateHistogramCreationInformation(histInfo);

      if (showDialog)
      {
        if (!Current.Gui.ShowDialog(ref histInfo, "Binning options", true))
          return null;
      }

      if (histInfo.FilteredAndSortedDataEnsemble is null || histInfo.CreationOptions.Binning is null)
        throw new InvalidProgramException();

      CreateHistogramTable(ref destinationTable, proposedTableName, histInfo.FilteredAndSortedDataEnsemble, histInfo.CreationOptions.Binning);

      return destinationTable;
    }

    public static bool PopulateHistogramCreationInformation(HistogramCreationInformation histInfo)
    {
      if (histInfo is null)
        throw new ArgumentNullException(nameof(histInfo));

      if (histInfo.OriginalDataEnsemble is null)
        throw new ArgumentNullException($"{nameof(histInfo)}.{nameof(histInfo.OriginalDataEnsemble)} is null, but is required for this action.");

      histInfo.Errors.Clear();
      histInfo.Warnings.Clear();

      var list = new List<double>();
      histInfo.FilteredAndSortedDataEnsemble = list;

      Func<double, bool> IsExcluded = histInfo.CreationOptions.GetValueExcludingFunction();

      var options = histInfo.CreationOptions;

      int numberOfNaNValues = 0;
      int numberOfInfiniteValues = 0;
      int numberOfValuesOriginal = 0;

      var dataEnumerator = histInfo.OriginalDataEnsemble;
      dataEnumerator.Reset();
      while (dataEnumerator.MoveNext())
      {
        ++numberOfValuesOriginal;
        double x = dataEnumerator.Current;

        if (double.IsNaN(x))
        {
          ++numberOfNaNValues;
        }
        else if (!Altaxo.Calc.RMath.IsFinite(x))
        {
          ++numberOfInfiniteValues;
        }
        else if (!IsExcluded(x))
        {
          list.Add(x);
        }
      }

      if (numberOfNaNValues > 0 && !histInfo.CreationOptions.IgnoreNaN)
        histInfo.Errors.Add("Data ensemble contains NaN values. Set the option 'Ignore NaN values' to ignore those values.");

      if (numberOfInfiniteValues > 0 && !histInfo.CreationOptions.IgnoreInfinity)
        histInfo.Errors.Add("Data ensemble contains infinite values. Set the option 'Ignore infinite values' to ignore those values.");

      list.Sort();

      int numberOfValues = list.Count;
      histInfo.NumberOfValuesOriginal = numberOfValuesOriginal;
      histInfo.NumberOfValuesFiltered = list.Count;
      histInfo.NumberOfNaNValues = numberOfNaNValues;
      histInfo.NumberOfInfiniteValues = numberOfInfiniteValues;

      double minValue = double.NaN;
      double maxValue = double.NaN;

      if (list.Count == 0)
      {
        histInfo.Errors.Add("No values available (number of selected values is zero).");
      }
      else if (list.Count < 10)
      {
        histInfo.Warnings.Add("Number of selected values is less than 10");
      }

      if (list.Count > 0)
      {
        minValue = list[0];
        maxValue = list[numberOfValues - 1];

        histInfo.MinimumValue = minValue;
        histInfo.MaximumValue = maxValue;

        // here we must decide between linear binning and logarithmic binning
        if (!histInfo.CreationOptions.IsUserDefinedBinningType)
        {
          Type binningType;

          if (minValue > 0 && maxValue > 0 && (Math.Log10(maxValue) - Math.Log10(minValue)) > 3)
            binningType = typeof(LogarithmicBinning);
          else
            binningType = typeof(LinearBinning);

          if (binningType != histInfo.CreationOptions.Binning?.GetType())
            histInfo.CreationOptions.Binning = (IBinning)(Activator.CreateInstance(binningType) ?? throw new InvalidProgramException($"Unable to create IBinning class {binningType}. Maybe a public parameterless constructor is missing?"));
        }
      }

      // OK, up here we have either LinearBinning or LogarithmicBinning

      var binning = histInfo.CreationOptions.Binning;

      if (binning is null)
        throw new InvalidProgramException($"Here, {nameof(binning)} should be either {nameof(LinearBinning)} or {nameof(LogarithmicBinning)}.");

      try
      {
        // calculate number of resulting bins
        binning.CalculateBinPositionsFromSortedList(list);
      }
      catch (Exception ex)
      {
        histInfo.Errors.Add(ex.Message);
      }

      bool showDialog = ShouldShowDialog(histInfo);

      if (!showDialog && histInfo.Errors.Count > 0)
      {
        throw new InvalidOperationException(string.Join("\r\n", histInfo.Errors));
      }

      return showDialog;
    }

    private static bool ShouldShowDialog(HistogramCreationInformation histInfo)
    {
      bool showDialog;
      switch (histInfo.UserInteractionLevel)
      {
        case Gui.UserInteractionLevel.None:
          showDialog = false;
          break;

        case Gui.UserInteractionLevel.InteractOnErrors:
          showDialog = histInfo.Errors.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractOnWarningsAndErrors:
          showDialog = histInfo.Errors.Count > 0 || histInfo.Warnings.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractAlways:
          showDialog = true;
          break;

        default:
          throw new NotImplementedException("userInteractionLevel");
      }
      return showDialog;
    }

    private static void CreateHistogramTable([AllowNull][NotNull] ref DataTable destinationTable, string? proposedTableName, IReadOnlyList<double> sortedListOfData, IBinning binning)
    {
      if (destinationTable is null)
      {
        destinationTable = Current.Project.CreateNewTable(proposedTableName, false);
        Current.ProjectService.CreateNewWorksheet(destinationTable);
      }

      var colBinPosition = destinationTable.DataColumns.EnsureExistence("BinPosition", typeof(DoubleColumn), ColumnKind.X, 0);
      var colBinLowerBound = destinationTable.DataColumns.EnsureExistence("BinLowerBoundary", typeof(DoubleColumn), ColumnKind.V, 0);
      var colBinUpperBound = destinationTable.DataColumns.EnsureExistence("BinUpperBoundary", typeof(DoubleColumn), ColumnKind.V, 0);
      var colBinCounts = destinationTable.DataColumns.EnsureExistence("BinCounts", typeof(DoubleColumn), ColumnKind.V, 0);
      var colProbabilityDensity = destinationTable.DataColumns.EnsureExistence("ProbabilityDensity", typeof(DoubleColumn), ColumnKind.V, 0);

      binning.CalculateBinsFromSortedList(sortedListOfData);

      using (var suspendToken = destinationTable.SuspendGetToken())
      {
        for (int idx = 0; idx < binning.Bins.Count; ++idx)
        {
          var bin = binning.Bins[idx];
          colBinPosition[idx] = bin.CenterPosition;
          colBinLowerBound[idx] = bin.LowerBound;
          colBinUpperBound[idx] = bin.UpperBound;
          colBinCounts[idx] = bin.ValueCount;
          colProbabilityDensity[idx] = bin.ValueCount / (sortedListOfData.Count * bin.Width);
        }

        suspendToken.Resume();
      }
    }

    public static string TestAgainstStandardDistribution(IList<double> sortedListOfData, LinearBinning binning, INumericColumn colBinPosition, INumericColumn colBinCounts)
    {
      var stb = new StringBuilder();

      var stat = new Calc.Regression.QuickStatistics();
      foreach (var v in sortedListOfData)
        stat.Add(v);

      // Test the probability against the normal distribution

      // First make a fit of the normal distribution to get mu and sigma
      // guess of mu
      double guessedMu = stat.Mean;
      double guessedSigma = stat.SampleStandardDeviation;

      /*
                Altaxo.Calc.Regression.Nonlinear.SimpleNonlinearFit fit = new Altaxo.Calc.Regression.Nonlinear.SimpleNonlinearFit(
                    delegate (double[] indep, double[] p, double[] res)
                    {
                        // 3 Parameter:  mu and sigma
                        res[0] = Calc.Probability.NormalDistribution.PDF(indep[0], p[0], p[1]);
                    },
                    new double[] { guessedMu, guessedSigma },
                    colBinPosition,
                    colProbabilityDensity,
                    0, // Start (first point)
                    binning.Count // point count
                    );

                fit.Fit();

                guessedMu = fit.GetParameter(0);
                guessedSigma = fit.GetParameter(1);

    */

      // Test hypothesis that we have a normal distribution

      double chiSquare = 0;

      int degreesOfFreedom = binning.NumberOfBins - 1 - 2; // -2 because we lost two degrees of freedom in the previous fitting

      for (int nBinIndex = 0; nBinIndex < binning.NumberOfBins; ++nBinIndex)
      {
        var bin = binning.Bins[nBinIndex];
        double lowerBound = bin.LowerBound;
        double upperBound = bin.UpperBound;

        var probability = Calc.Probability.NormalDistribution.CDF(upperBound, guessedMu, guessedSigma) - Calc.Probability.NormalDistribution.CDF(lowerBound, guessedMu, guessedSigma);
        double n0 = probability * sortedListOfData.Count;

        chiSquare += RMath.Pow2(n0 - colBinCounts[nBinIndex]) / n0;
      }

      double chiSquareThreshold = Calc.Probability.ChiSquareDistribution.Quantile(0.95, degreesOfFreedom);

      if (chiSquare <= chiSquareThreshold)
      {
        stb.AppendFormat("The hypothesis that this is a normal distribution with mu={0} and sigma={1} can not be rejected. ChiSquare ({2}) is less than ChiSquare needed to reject hypothesis ({3})", guessedMu, guessedSigma, chiSquare, chiSquareThreshold);
      }
      else
      {
        stb.AppendFormat("The hypothesis that this is a normal distribution with mu={0} and sigma={1} must be rejected with a confidence of {2}%. ChiSquare ({3}) is greater than ChiSquare needed to reject hypothesis ({4})", guessedMu, guessedSigma, 100 * Altaxo.Calc.Probability.ChiSquareDistribution.CDF(chiSquare, degreesOfFreedom), chiSquare, chiSquareThreshold);
      }

      return stb.ToString();
    }

    private static double StrictCeiling(double x)
    {
      double result = Math.Ceiling(x);
      if (result == x)
        result += 1;

      return result;
    }
  }
}
