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

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  using Altaxo.Calc.LinearAlgebra;
  using Altaxo.Calc.RootFinding;

  /// <summary>
  /// Methods for creating a master curve.
  /// </summary>
  public static class MasterCurveCreation
  {
    /// <summary>
    /// Creates one or multiple master curve(s).
    /// </summary>
    /// <param name="options">The options for creating a master curve.</param>
    /// <param name="shiftCurveCollections">Raw data for master curve creation.</param>
    /// <returns>The result of the master curve creation.</returns>
    public static MasterCurveCreationResult CreateMasterCurve(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections)
    {
      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation
      var indexOfReferenceColumnInColumnGroup = options.IndexOfReferenceColumnInColumnGroup;
      int maxColumns = shiftCurveCollections.Max(x => x.Count);

      var result = new MasterCurveCreationResult(shiftCurveCollections.Count);
      result.ResultingShifts.Clear();
      for (int i = 0; i < maxColumns; i++)
        result.ResultingShifts.Add(0);

      for (int nColumnGroup = 0; nColumnGroup < result.ResultingInterpolation.Length; nColumnGroup++)
      {
        result.ResultingInterpolation[nColumnGroup] = new InterpolationInformation();
        var referenceShiftCurve = shiftCurveCollections[nColumnGroup][options.IndexOfReferenceColumnInColumnGroup];
        // To each column group, add initially only the column used as reference (shift 0)
        result.ResultingInterpolation[nColumnGroup].AddXYColumn(0, indexOfReferenceColumnInColumnGroup, referenceShiftCurve.X, referenceShiftCurve.Y, options);
      }

      // Make the initial interpolation for all column groups, using only the column(s) used as reference (shift 0)
      InterpolateAllColumnGroups(options, result);

      CreateShiftGroups(indexOfReferenceColumnInColumnGroup, maxColumns, result);

      // now that we have a first interpolation using the reference curve, we can iterate
      // at the first iteration, the other curves will be added to the interpolation
      // and then, the quality of the master curve will be successivly improved
      Iterate(options, shiftCurveCollections, result);

      return result;
    }

    private static void InterpolateAllColumnGroups(MasterCurveCreationOptions options, MasterCurveCreationResult result)
    {
      var setOfCurvesToInterpolate = result.ResultingInterpolation.Select(
              ri => ((IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr))(ri.XValues, ri.YValues, null)).ToArray();
      var interpolations = options.CreateInterpolationFunction(setOfCurvesToInterpolate);
      for (int nColumnGroup = 0; nColumnGroup < result.ResultingInterpolation.Length; nColumnGroup++)
      {
        result.ResultingInterpolation[nColumnGroup].InterpolationFunction = interpolations[nColumnGroup];
      }
    }

    private static void CreateShiftGroups(int indexOfReferenceColumnInColumnGroup, int maxColumns, MasterCurveCreationResult result)
    {
      // now take the other columns, and shift them with respect to the interpolation
      // in each shift group we have a list of columns, the first of those columns is initially processed with a shift of 1
      // but subsequent columns are then processed with an initial shift factor which is set to the previously calculated shift factor.
      var shiftGroups = result.ShiftGroups;
      shiftGroups.Clear();
      var shiftGroup = new List<int>();
      for (int i = indexOfReferenceColumnInColumnGroup + 1; i < maxColumns; i++)
        shiftGroup.Add(i);
      shiftGroups.Add(shiftGroup);
      shiftGroup = new List<int>();
      for (int i = indexOfReferenceColumnInColumnGroup - 1; i >= 0; i--)
        shiftGroup.Add(i);
      shiftGroups.Add(shiftGroup);
    }

    /// <summary>
    /// Performs iteration to create or refine the master curve. There must already exist an interpolation for each curve group
    /// (which at the first iteration consist only of the interpolation of the reference curve(s)).
    /// </summary>
    /// <param name="options">The master curve creation options.</param>
    /// <param name="shiftCurveCollections">The data to construct the master curve(s).</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void Iterate(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections, MasterCurveCreationResult result)
    {
      for (int iteration = 0; iteration < options.NumberOfIterations; ++iteration)
      {
        OneIteration(options, shiftCurveCollections, result);
      }
    }

    private static void OneIteration(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections, MasterCurveCreationResult result)
    {
      foreach (var shiftGroup in result.ShiftGroups)
      {
        OneIterationForOneShiftGroup(options, shiftCurveCollections, shiftGroup, result);
      }
    }

    private static void OneIterationForOneShiftGroup(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections, List<int> shiftGroup, MasterCurveCreationResult result)
    {
      double initialShift = 0;
      var oneShiftDataAcrossCurveGroups = new ShiftCurve[shiftCurveCollections.Count]; // contains the shift data for the same curve index, but for all curve groups
      var interpolations = result.ResultingInterpolation ?? throw new InvalidOperationException($"{nameof(result)} must already contain valid interpolation(s).");

      foreach (int indexOfCurveInShiftGroup in shiftGroup)
      {
        double globalMinShift = double.MaxValue;
        double globalMaxShift = double.MinValue;
        for (int nColumnGroup = 0; nColumnGroup < interpolations.Length; nColumnGroup++)
        {
          var shiftCurve = shiftCurveCollections[nColumnGroup][indexOfCurveInShiftGroup];
          oneShiftDataAcrossCurveGroups[nColumnGroup] = shiftCurve;
          var (xmin, xmax) = GetMinMaxOfFirstColumnForValidSecondColumn(shiftCurve.X, shiftCurve.Y, options.XShiftBy, options.LogarithmizeXForInterpolation, options.LogarithmizeYForInterpolation);

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
                shift => GetMeanSignedPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, options), globalMinShift, globalMaxShift);
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifference:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, options);
                //Current.Console.WriteLine("Eval for shift={0}: {1}", shift, res);
                return res;
              };

              var optimizationMethod = new StupidLineSearch(new Simple1DCostFunction(optFunc));
              var vec = CreateVector.Dense<double>(1);
              vec[0] = initialShift;

              var dir = CreateVector.Dense<double>(1);
              dir[0] = 1;

              double initialStep = 0.05;
              var optresult = optimizationMethod.Search(vec, dir, initialStep);
              currentShift = optresult[0];
              // currentShiftFactor = optimizationMethod.SolutionVector[0];
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifferenceByBruteForce:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, options);
                //Current.Console.WriteLine("Eval for shift={0}: {1}", shift, res);
                return res;
              };
              var optimizationMethod = new BruteForceLineSearch(new Simple1DCostFunction(optFunc));
              var vec = CreateVector.Dense<double>(1);
              vec[0] = globalMinShift;

              var dir = CreateVector.Dense<double>(1);
              dir[0] = globalMaxShift - globalMinShift;
              double initialStep = 1;
              var optresult = optimizationMethod.Search(vec, dir, initialStep);
              currentShift = optresult[0];
            }
            break;

          default:
            throw new NotImplementedException("OptimizationMethod not implemented: " + options.OptimizationMethod.ToString());
        }

        if (currentShift.IsFinite())
        {
          result.ResultingShifts[indexOfCurveInShiftGroup] = currentShift;

          for (int nColumnGroup = 0; nColumnGroup < interpolations.Length; nColumnGroup++)
          {
            // add the data for interpolation again, using the new shift
            interpolations[nColumnGroup].AddXYColumn(currentShift, indexOfCurveInShiftGroup, oneShiftDataAcrossCurveGroups[nColumnGroup].X, oneShiftDataAcrossCurveGroups[nColumnGroup].Y, options);
          }
          // now build up a new interpolation, where the shifted data is taken into account
          InterpolateAllColumnGroups(options, result);

          initialShift = currentShift;
        }
      }
    }


    /// <summary>
    /// Reinitializes the result. With that, new options can be used, for instance a new interpolation function.
    /// Typically, after calling this, you can call <see cref="Iterate(MasterCurveCreationOptions, ShiftCurveCollections, List{List{int}}, MasterCurveCreationResult)"/> to iterate
    /// with the new interpolation function again.
    /// </summary>
    /// <param name="options">The master curve creation options.</param>
    /// <param name="shiftCurveCollections">The data to construct the master curve(s).</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void ReInitializeResult(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections, MasterCurveCreationResult result)
    {
      for (int idxCurveCollection = 0; idxCurveCollection < shiftCurveCollections.Count; idxCurveCollection++)
      {
        var interpolation = result.ResultingInterpolation[idxCurveCollection];
        var shiftCurves = shiftCurveCollections[idxCurveCollection];
        interpolation.Clear();

        for (int idxCurve = 0; idxCurve < shiftCurves.Count; idxCurve++)
        {
          interpolation.AddXYColumn(result.ResultingShifts[idxCurve], idxCurve, shiftCurves[idxCurve].X, shiftCurves[idxCurve].Y, options);
        }
      }
      InterpolateAllColumnGroups(options, result);
    }

    /// <summary>
    /// Reinitializes the result (see <see cref="ReInitializeResult(MasterCurveCreationOptions, ShiftCurveCollections, MasterCurveCreationResult)"/>)
    /// and then iterate anew.
    /// </summary>
    /// <param name="options">The master curve creation options.</param>
    /// <param name="shiftCurveCollections">The data to construct the master curve(s).</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void ReIterate(MasterCurveCreationOptions options, ShiftCurveCollections shiftCurveCollections, MasterCurveCreationResult result)
    {
      ReInitializeResult(options, shiftCurveCollections, result);
      Iterate(options, shiftCurveCollections, result);
    }

    /// <summary>
    /// Gets the minimum and maximum of the x-values, taking into account different options and whether the y-values are valid.
    /// </summary>
    /// <param name="x">The x-values.</param>
    /// <param name="y">The y-values.</param>
    /// <param name="shiftBy">The method to shift the data.</param>
    /// <param name="doLogX">True if the x-values are logarithmized for the interpolation.</param>
    /// <param name="doLogY">True if the y-values are logarithmized for the interpolation.</param>
    /// <returns>Minimum and maximum of the x-values, for x and y values appropriate for the conditions given by the parameter.</returns>
    public static (double min, double max) GetMinMaxOfFirstColumnForValidSecondColumn(IReadOnlyList<double> x, IReadOnlyList<double> y, ShiftXBy shiftBy, bool doLogX, bool doLogY)
    {
      var len = Math.Min(x.Count, y.Count);
      var min = double.PositiveInfinity;
      var max = double.NegativeInfinity;

      for (int i = 0; i < len; i++)
      {
        double x1 = shiftBy == ShiftXBy.Factor ? Math.Log(x[i]) : x[i];
        double xv = doLogX ? Math.Log(x[i]) : x[i];
        double yv = doLogY ? Math.Log(y[i]) : y[i];
        if (x1.IsFinite() && xv.IsFinite() && yv.IsFinite())
        {
          min = Math.Min(min, xv);
          max = Math.Max(max, xv);
        }
      }
      return (min, max);
    }

    /// <summary>
    /// Calculates a mean signed penalty value for the current shift factor of the current column. By making the penalty value zero, the current column will fit optimally into the so far created master curve.
    /// </summary>
    /// <param name="interpolations">Current interpolation functions for the column groups (e.g. real and imaginary part).</param>
    /// <param name="oneShiftDataAcrossGroups">Shift data for a given curve index, for all curve groups.</param>
    /// <param name="shift">Current shift (direct offset or the natural logarithm of the shiftFactor).</param>
    /// <param name="options">Options for creating the master curve.</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private static double GetMeanSignedPenalty(InterpolationInformation[] interpolations, IReadOnlyList<ShiftCurve> oneShiftDataAcrossGroups, double shift, MasterCurveCreationOptions options)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int i = 0; i < oneShiftDataAcrossGroups.Count; i++)
      {
        var shiftData = oneShiftDataAcrossGroups[i];
        GetMeanSignedYDifference(interpolations[i].InterpolationFunction, interpolations[i].InterpolationMinimumX, interpolations[i].InterpolationMaximumX, shiftData.X, shiftData.Y, shift, options, out var penalty, out var points);

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
    /// <param name="oneShiftDataAcrossGroups">Shift data for a given curve index, for all curve groups.</param>
    /// <param name="shift">Current shift (direct or log of shiftFactor).</param>
    /// <param name="options">Options for creating the master curve.</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private static double GetMeanSquaredPenalty(InterpolationInformation[] interpolations, IReadOnlyList<ShiftCurve> oneShiftDataAcrossGroups, double shift, MasterCurveCreationOptions options)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int i = 0; i < oneShiftDataAcrossGroups.Count; i++)
      {
        var shiftData = oneShiftDataAcrossGroups[i];
        GetMeanSquaredYDifference(interpolations[i].InterpolationFunction, interpolations[i].InterpolationMinimumX, interpolations[i].InterpolationMaximumX, shiftData.X, shiftData.Y, shift, options, out var penalty, out var points);

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
    public static void GetMeanSignedYDifference(Func<double, double> interpolation, double interpolMin, double interpolMax, IReadOnlyList<double> x, IReadOnlyList<double> y, double shift, MasterCurveCreationOptions options, out double penalty, out int evaluatedPoints)
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
    public static void GetMeanSquaredYDifference(Func<double, double> interpolationFunc, double interpolMin, double interpolMax, IReadOnlyList<double> x, IReadOnlyList<double> y, double shift, MasterCurveCreationOptions options, out double penalty, out int evaluatedPoints)
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
