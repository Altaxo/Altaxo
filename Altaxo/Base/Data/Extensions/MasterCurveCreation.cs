#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.Optimization;

namespace Altaxo.Data
{
  using Altaxo.Calc.LinearAlgebra;
  using Altaxo.Calc.RootFinding;

  public static partial class MasterCurveCreation
  {
    #region Helper types

    public class CurveData : List<List<DoubleColumn>>
    {
    }

    /// <summary>
    /// Determines how to shift the x-values.
    /// </summary>
    public enum ShiftXBy
    {
      /// <summary>Shifts by multiplying a constant value to all x values. Use this if your x values are not logarithmized already.</summary>
      Factor,

      /// <summary>Shifts by adding a constant value to all x values. Use this if your x values are already logarithmized.</summary>
      Offset
    }

    /// <summary>
    /// Determines how to best fit the data into the master curve.
    /// </summary>
    public enum OptimizationMethod
    {
      /// <summary>
      /// Evaluates the mean difference (signed) between master curve and new data and tries to make this difference zero (root finding).
      /// </summary>
      OptimizeSignedDifference,

      /// <summary>
      /// Evaluates the mean squared difference (always positive) between master curve and new data and tries to minimize this value (minimization method).
      /// </summary>
      OptimizeSquaredDifference,

      OptimizeSquaredDifferenceByBruteForce
    }


    /// <summary>
    /// Stores the current columns.
    /// </summary>
    private struct CurrentColumnInformation
    {
      /// <summary>Current x column.</summary>
      public DoubleColumn CurrentXCol;

      /// <summary>Current y column.</summary>
      public DoubleColumn CurrentYCol;
    }

    #endregion Helper types

    public static void CreateMasterCurve(Options options)
    {
      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation
      var indexOfReferenceColumnInColumnGroup = options.IndexOfReferenceColumnInColumnGroup;
      var columnGroups = options.ColumnGroups;
      int maxColumns = options.ColumnGroups.Max(x => x.Count);

      options.ResultingShifts.Clear();
      for (int i = 0; i < maxColumns; i++)
        options.ResultingShifts.Add(0);

      var interpolations = new InterpolationInformation[options.ColumnGroups.Count];
      options.ResultingInterpolation = interpolations;
      var currentColumns = new CurrentColumnInformation[options.ColumnGroups.Count];
      for (int nColumnGroup = 0; nColumnGroup < interpolations.Length; nColumnGroup++)
      {
        interpolations[nColumnGroup].Initialize(options.CreateInterpolationFunction);

        var yCol = columnGroups[nColumnGroup][indexOfReferenceColumnInColumnGroup];
        var table = DataColumnCollection.GetParentDataColumnCollectionOf(yCol) ?? throw new InvalidOperationException($"Column {yCol.Name} has no parent data table!");
        var xCol = (DoubleColumn)(table.FindXColumnOf(yCol) ?? throw new InvalidOperationException($"Can't find corresponding x-column for column {yCol.Name}"));

        // Make the initial interpolation for that column group, using only the column used as reference (shift 0)
        interpolations[nColumnGroup].AddXYColumnToInterpolation(0, indexOfReferenceColumnInColumnGroup, xCol, yCol, options);
      }

      // now take the other columns, and shift them with respect to the interpolation
      // in each shift group we have a list of columns, the first of those columns is initially processed with a shift of 1
      // but subsequent columns are then processed with an initial shift factor which is set to the previously calculated shift factor.
      var shiftGroups = new List<List<int>>();
      {
        var shiftGroup = new List<int>();
        for (int i = indexOfReferenceColumnInColumnGroup + 1; i < maxColumns; i++)
          shiftGroup.Add(i);
        shiftGroups.Add(shiftGroup);
        shiftGroup = new List<int>();
        for (int i = indexOfReferenceColumnInColumnGroup - 1; i >= 0; i--)
          shiftGroup.Add(i);
        shiftGroups.Add(shiftGroup);
      }

      // now that we have a first interpolation using the reference curve, we can iterate
      // at the first iteration, the other curves will be added to the interpolation
      // and then, the quality of the master curve will be successivly improved
      Iterate(options.NumberOfIterations, options, columnGroups, interpolations, currentColumns, shiftGroups);
    }

    private static void Iterate(int numberOfIterations, Options options, List<List<DoubleColumn>> columnGroups, InterpolationInformation[] interpolations, CurrentColumnInformation[] currentColumns, List<List<int>> shiftGroups)
    {
      for (int iteration = 0; iteration < numberOfIterations; ++iteration)
      {
        OneIteration(options, columnGroups, interpolations, currentColumns, shiftGroups);
      }
    }

    private static void OneIteration(Options options, List<List<DoubleColumn>> columnGroups, InterpolationInformation[] interpolations, CurrentColumnInformation[] currentColumns, List<List<int>> shiftGroups)
    {
      foreach (var shiftGroup in shiftGroups)
      {
        OneIterationForOneShiftGroup(options, columnGroups, interpolations, currentColumns, shiftGroup);
      }
    }

    private static void OneIterationForOneShiftGroup(Options options, List<List<DoubleColumn>> columnGroups, InterpolationInformation[] interpolations, CurrentColumnInformation[] currentColumns, List<int> shiftGroup)
    {
      double initialShift = 0;

      foreach (int indexOfCurveInShiftGroup in shiftGroup)
      {
        double globalMinShift = double.MaxValue;
        double globalMaxShift = double.MinValue;
        for (int nColumnGroup = 0; nColumnGroup < interpolations.Length; nColumnGroup++)
        {
          var yCol = columnGroups[nColumnGroup][indexOfCurveInShiftGroup];
          var table = DataColumnCollection.GetParentDataColumnCollectionOf(yCol) ?? throw new InvalidOperationException($"Column {yCol.Name} has no parent data table!");
          var xCol = (DoubleColumn)(table.FindXColumnOf(yCol) ?? throw new InvalidOperationException($"Can't find corresponding x-column for column {yCol.Name}"));
          currentColumns[nColumnGroup].CurrentXCol = xCol;
          currentColumns[nColumnGroup].CurrentYCol = yCol;
          GetMinMaxOfFirstColumnForValidSecondColumn(xCol, yCol, options.LogarithmizeXForInterpolation, options.LogarithmizeYForInterpolation, out var xmin, out var xmax);

          double localMaxShift;
          double localMinShift;

          if (options.LogarithmizeXForInterpolation)
          {
            localMaxShift = interpolations[nColumnGroup].InterpolationMaximumX - xmin;
            localMinShift = interpolations[nColumnGroup].InterpolationMinimumX - xmax;
          }
          else
          {
            localMaxShift = Math.Log(interpolations[nColumnGroup].InterpolationMaximumX / xmin);
            localMinShift = Math.Log(interpolations[nColumnGroup].InterpolationMinimumX / xmax);
          }
          globalMinShift = Math.Min(globalMinShift, localMinShift);
          globalMaxShift = Math.Max(globalMaxShift, localMaxShift);
        }

        // we reduce the maximum possible shifts a little in order to get at least one point overlapping
        double diff = (globalMaxShift - globalMinShift) / 100;
        globalMaxShift = globalMaxShift - diff;
        globalMinShift = globalMinShift + diff;

        double currentShift = initialShift; // remember: this is either a offset or the natural logarithm of the shift factor
        switch (options.OptimizationMethod)
        {
          case OptimizationMethod.OptimizeSignedDifference:
            {
              currentShift =
              QuickRootFinding.ByBrentsAlgorithm(
                shift => GetMeanSignedPenalty(interpolations, currentColumns, shift, options), globalMinShift, globalMaxShift);
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifference:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(interpolations, currentColumns, shift, options);
                //Current.Console.WriteLine("Eval for shift={0}: {1}", shift, res);
                return res;
              };

              var optimizationMethod = new StupidLineSearch(new Simple1DCostFunction(optFunc));
              var vec = CreateVector.Dense<double>(1);
              vec[0] = initialShift;

              var dir = CreateVector.Dense<double>(1);
              dir[0] = 1;

              double initialStep = 0.05;
              var result = optimizationMethod.Search(vec, dir, initialStep);
              currentShift = result[0];
              // currentShiftFactor = optimizationMethod.SolutionVector[0];
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifferenceByBruteForce:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(interpolations, currentColumns, shift, options);
                //Current.Console.WriteLine("Eval for shift={0}: {1}", shift, res);
                return res;
              };
              var optimizationMethod = new BruteForceLineSearch(new Simple1DCostFunction(optFunc));
              var vec = CreateVector.Dense<double>(1);
              vec[0] = globalMinShift;

              var dir = CreateVector.Dense<double>(1);
              dir[0] = globalMaxShift - globalMinShift;
              double initialStep = 1;
              var result = optimizationMethod.Search(vec, dir, initialStep);
              currentShift = result[0];
            }
            break;

          default:
            throw new NotImplementedException("OptimizationMethod not implemented: " + options.OptimizationMethod.ToString());
        }

        if (currentShift.IsFinite())
        {
          options.ResultingShifts[indexOfCurveInShiftGroup] = currentShift;

          for (int nColumnGroup = 0; nColumnGroup < interpolations.Length; nColumnGroup++)
          {
            // now build up a new interpolation, where the shifted data is taken into account
            interpolations[nColumnGroup].AddXYColumnToInterpolation(currentShift, indexOfCurveInShiftGroup, currentColumns[nColumnGroup].CurrentXCol, currentColumns[nColumnGroup].CurrentYCol, options);
          }

          initialShift = currentShift;
        }
      }
    }

    public static bool GetMinMaxOfFirstColumnForValidSecondColumn(DoubleColumn x, DoubleColumn y, bool doLogX, bool doLogY, out double min, out double max)
    {
      int len = Math.Min(x.Count, y.Count);
      min = double.PositiveInfinity;
      max = double.NegativeInfinity;

      for (int i = 0; i < len; i++)
      {
        double xv = doLogX ? Math.Log(x[i]) : x[i];
        double yv = doLogY ? Math.Log(y[i]) : y[i];
        if (xv.IsFinite() && yv.IsFinite())
        {
          min = Math.Min(min, xv);
          max = Math.Max(max, xv);
        }
      }
      return max >= min;
    }

    /// <summary>
    /// Calculates a mean signed penalty value for the current shift factor of the current column. By making the penalty value zero, the current column will fit optimally into the so far created master curve.
    /// </summary>
    /// <param name="interpolations">Current interpolation functions for the column groups (e.g. real and imaginary part).</param>
    /// <param name="currentColumns">Current x and y column.</param>
    /// <param name="shift">Current shift (direct offset or the natural logarithm of the shiftFactor).</param>
    /// <param name="options">Options for creating the master curve.</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private static double GetMeanSignedPenalty(InterpolationInformation[] interpolations, CurrentColumnInformation[] currentColumns, double shift, Options options)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int i = 0; i < interpolations.Length; i++)
      {
        GetMeanSignedYDifference(interpolations[i].InterpolationFunction, interpolations[i].InterpolationMinimumX, interpolations[i].InterpolationMaximumX, currentColumns[i].CurrentXCol, currentColumns[i].CurrentYCol, shift, options, out var penalty, out var points);

        if (points > 0)
        {
          penaltySum += penalty;
          penaltyPoints += points;
        }
      }

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanPenalty for shift={0} resulted in {1} ({2} points)", shift, penaltySum, penaltyPoints));

      return penaltyPoints > 0 ? penaltySum / penaltyPoints : float.MaxValue;
    }

    /// <summary>
    /// Calculates a mean squared penalty value for the current shift factor of the current column. By minimizing the penalty value, the current column will fit optimally into the so far created master curve.
    /// </summary>
    /// <param name="interpolations">Current interpolation functions for the column groups (e.g. real and imaginary part).</param>
    /// <param name="currentColumns">Current x and y column.</param>
    /// <param name="shift">Current shift (direct or log of shiftFactor).</param>
    /// <param name="options">Options for creating the master curve.</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private static double GetMeanSquaredPenalty(InterpolationInformation[] interpolations, CurrentColumnInformation[] currentColumns, double shift, Options options)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int i = 0; i < interpolations.Length; i++)
      {
        GetMeanSquaredYDifference(interpolations[i].InterpolationFunction, interpolations[i].InterpolationMinimumX, interpolations[i].InterpolationMaximumX, currentColumns[i].CurrentXCol, currentColumns[i].CurrentYCol, shift, options, out var penalty, out var points);

        if (points > 0)
        {
          penaltySum += penalty;
          penaltyPoints += points;
        }
      }

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanPenalty for shift={0} resulted in {1} ({2} points)", shift, penaltySum, penaltyPoints));

      return penaltyPoints > 0 ? penaltySum / penaltyPoints : float.MaxValue;
    }

    /// <summary>
    /// Gets the mean difference between the y column and the interpolation function, provided that the x column is shifted by a factor.
    /// </summary>
    /// <param name="interpolation">The interpolating function of the master curve created so far.</param>
    /// <param name="interpolMin">Minimum valid x value of the interpolation function.</param>
    /// <param name="interpolMax">Maximum valid x value of the interpolation function.</param>
    /// <param name="x">Column of x values of the new part of the master curve.</param>
    /// <param name="y">Column of y values of the new part of the master curve.</param>
    /// <param name="shift">Shift offset (direct offset or natural logarithm of the shiftFactor for the new part of the master curve.</param>
    /// <param name="options">Information for the master curve creation.</param>
    /// <param name="penalty">Returns the calculated penalty value (mean difference between interpolation curve and provided data).</param>
    /// <param name="evaluatedPoints">Returns the number of points (of the new part of the curve) used for calculating the penalty value.</param>
    public static void GetMeanSignedYDifference(Func<double, double> interpolation, double interpolMin, double interpolMax, DoubleColumn x, DoubleColumn y, double shift, Options options, out double penalty, out int evaluatedPoints)
    {
      int len = Math.Min(x.Count, y.Count);
      int validPoints = 0;
      bool doLogX = options.LogarithmizeXForInterpolation;
      bool doLogY = options.LogarithmizeYForInterpolation;
      bool shiftXByOffset = options.XShiftBy == ShiftXBy.Offset;
      double penaltySum = 0;
      for (int i = 0; i < len; i++)
      {
        double xv;
        if (doLogX)
          xv = shiftXByOffset ? Math.Log(x[i] + shift) : Math.Log(x[i]) + shift;
        else
          xv = shiftXByOffset ? x[i] + shift : x[i] * Math.Exp(shift);

        double yv = y[i];
        if (doLogY)
          yv = Math.Log(yv);

        if (xv.IsFinite() && yv.IsFinite() && xv.IsInIntervalCC(interpolMin, interpolMax))
        {
          try
          {
            double diff = yv - interpolation(xv);
            penaltySum += diff;
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      penalty = penaltySum;
      evaluatedPoints = validPoints;

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanYDifference for shift={0} resulted in {1} ({2} points)", shift, penalty, evaluatedPoints));
    }

    /// <summary>
    /// Gets the mean squared difference between the y column and the interpolation function, provided that the x column is shifted by a factor.
    /// </summary>
    /// <param name="interpolationFunc">The interpolating function of the master curve created so far.</param>
    /// <param name="interpolMin">Minimum valid x value of the interpolation function.</param>
    /// <param name="interpolMax">Maximum valid x value of the interpolation function.</param>
    /// <param name="x">Column of x values of the new part of the master curve.</param>
    /// <param name="y">Column of y values of the new part of the master curve.</param>
    /// <param name="shift">Shift offset (direct offset or natural logarithm of the shiftFactor) for the new part of the master curve.</param>
    /// <param name="options">Information for the master curve creation.</param>
    /// <param name="penalty">Returns the calculated penalty value (mean squared difference between interpolation curve and provided data).</param>
    /// <param name="evaluatedPoints">Returns the number of points (of the new part of the curve) used for calculating the penalty value.</param>
    public static void GetMeanSquaredYDifference(Func<double, double> interpolationFunc, double interpolMin, double interpolMax, DoubleColumn x, DoubleColumn y, double shift, Options options, out double penalty, out int evaluatedPoints)
    {
      int len = Math.Min(x.Count, y.Count);
      int validPoints = 0;
      bool doLogX = options.LogarithmizeXForInterpolation;
      bool doLogY = options.LogarithmizeYForInterpolation;
      bool shiftXByOffset = options.XShiftBy == ShiftXBy.Offset;
      double penaltySum = 0;
      for (int i = 0; i < len; i++)
      {
        double xv;
        if (doLogX)
          xv = shiftXByOffset ? Math.Log(x[i] + shift) : Math.Log(x[i]) + shift;
        else
          xv = shiftXByOffset ? x[i] + shift : x[i] * Math.Exp(shift);

        double yv = y[i];
        if (doLogY)
          yv = Math.Log(yv);

        if (xv.IsFinite() && yv.IsFinite() && xv.IsInIntervalCC(interpolMin, interpolMax))
        {
          try
          {
            double diff = yv - interpolationFunc(xv);
            penaltySum += diff * diff;
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      penalty = penaltySum;
      evaluatedPoints = validPoints;

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanYDifference for shift={0} resulted in {1} ({2} points)", shift, penalty, evaluatedPoints));
    }
  }
}
