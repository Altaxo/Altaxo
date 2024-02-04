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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.Optimization;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  using Altaxo.Calc.LinearAlgebra;
  using Altaxo.Calc.RootFinding;

  /// <summary>
  /// Methods for creating a master curve.
  /// </summary>
  public class MasterCurveCreationComplexCommonX
  {
    /// <summary>
    /// Creates one or multiple master curve(s).
    /// </summary>
    /// <param name="shiftGroupCollection">Raw data for master curve creation.</param>
    /// <returns>The result of the master curve creation.</returns>
    public static MasterCurveCreationResultComplexCommonX CreateMasterCurve(ShiftGroupCollectionComplexCommonX shiftGroupCollection)
    {
      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation
      int maxColumns = shiftGroupCollection.Max(x => x.Count);

      var shiftOrder = shiftGroupCollection.ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = GetCurveIndexWithMostVariation(shiftGroupCollection[0]); // we get the curve with most variation from the real part
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate ?? 0);
      }

      var (indexOfReferenceColumnInColumnGroup, shiftOrderIndices) = GetFixedAndShiftedIndices(shiftOrder, maxColumns);

      var result = new MasterCurveCreationResultComplexCommonX(shiftGroupCollection.Count);
      result.ResultingShifts.Clear();
      result.ResultingShifts.AddRange(Enumerable.Range(0, maxColumns).Select(_ => 0d));

      for (int idxGroup = 0; idxGroup < shiftGroupCollection.Count; idxGroup++)
      {
        var shiftGroup = shiftGroupCollection[idxGroup];
        result.ResultingInterpolation[idxGroup] = new InterpolationInformationComplexCommonX();
        var referenceShiftCurve = shiftGroupCollection[idxGroup][indexOfReferenceColumnInColumnGroup];
        // To each column group, add initially only the column used as reference (shift 0)
        result.ResultingInterpolation[idxGroup].AddXYColumn(0, indexOfReferenceColumnInColumnGroup, referenceShiftCurve.X, referenceShiftCurve.Y, null, idxGroup, shiftGroup);
      }

      // Make the initial interpolation for all column groups, using only the column(s) used as reference (shift 0)
      InterpolateAllColumnGroups(shiftGroupCollection, result);

      // now that we have a first interpolation using the reference curve, we can iterate
      // at the first iteration, the other curves will be added to the interpolation
      // and then, the quality of the master curve will be successivly improved
      Iterate(shiftGroupCollection, shiftOrderIndices, result);

      return result;
    }

    private static void InterpolateAllColumnGroups(ShiftGroupCollectionComplexCommonX shiftGroupCollection, MasterCurveCreationResultComplexCommonX result)
    {
      for (int idxGroup = 0; idxGroup < shiftGroupCollection.Count; idxGroup++)
      {
        var columnGroup = shiftGroupCollection[idxGroup];
        var ri = result.ResultingInterpolation[idxGroup];
        ri.InterpolationFunction = columnGroup.CreateInterpolationFunction((ri.XValues, ri.YValues, null));
      }
    }

    /// <summary>
    /// Gets the index of the initially fixed curve, and the indices of the curve that are shifted and fitted towards the master curve.
    /// </summary>
    /// <param name="order">An instance of <see cref="ShiftOrder.IShiftOrder"/> that determines the order.</param>
    /// <param name="numberOfItems">The number of items. This is the maximal number of curves in a group, e.g. if there is one shift group with 20 curves and another with 30 curves, then the argument should be 30.</param>
    /// <returns>A tuple of the initially fixed index and the indices that should be fitted then.</returns>
    /// <remarks>This function does not ensure that at the fixed index all groups have a valid curve, which is absolutely neccessary to start the shift procedure.</remarks>
    public static (int fixedIndex, IReadOnlyList<int> shiftOrderIndices) GetFixedAndShiftedIndices(ShiftOrder.IShiftOrder order, int numberOfItems)
    {
      var e = order.GetShiftOrderIndices(numberOfItems);
      return (e.First(), e.Skip(1).ToArray());
    }

    /// <summary>
    /// Gets the index of the curve in the given group with the most variation. The variation is determined by calculating the absolute slope,
    /// with applying the logarithmic transformations according to the interpolation settings in that group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <returns>The index of the curve with most variation. If no suitable curve was found, then the return value is null.</returns>
    public static int? GetCurveIndexWithMostVariation(ShiftGroupComplex group)
    {
      double maxAbsoluteSlope = 0;
      int idxMaxAbsoluteSlope = -1;
      for (int idxCurve = 0; idxCurve < group.Count; idxCurve++)
      {
        var (x, y) = TransformCurveForInterpolationAccordingToGroupOptions(group[idxCurve], group);
        if (x.Count < 2)
          continue;
        var reg = new Altaxo.Calc.Regression.QuickLinearRegression();
        for (int i = 0; i < x.Count; i++)
        {
          reg.Add(x[i], y[i].Real);
        }

        var absSlope = Math.Abs(reg.GetA1());
        if (absSlope > maxAbsoluteSlope)
        {
          maxAbsoluteSlope = absSlope;
          idxMaxAbsoluteSlope = idxCurve;
        }
      }

      return idxMaxAbsoluteSlope >= 0 ? idxMaxAbsoluteSlope : null;
    }

    public static (IReadOnlyList<double> x, IReadOnlyList<Complex64> y) TransformCurveForInterpolationAccordingToGroupOptions(ShiftCurve<Complex64> curve, ShiftGroupBase<Complex64> group)
    {
      var xarr = new List<double>();
      var yarr = new List<Complex64>();

      for (int i = 0; i < curve.Count; ++i)
      {
        var x = curve.X[i];
        var y = curve.Y[i];

        if (group.LogarithmizeXForInterpolation)
          x = Math.Log(x);
        if (group.LogarithmizeYForInterpolation)
          y = new Complex64(Math.Log(y.Real), Math.Log(y.Imaginary));

        if (x.IsFinite() && y.Real.IsFinite() && y.Imaginary.IsFinite())
        {
          xarr.Add(x);
          yarr.Add(y);
        }
      }

      return (xarr, yarr);
    }

    /// <summary>
    /// Performs iteration to create or refine the master curve. There must already exist an interpolation for each curve group
    /// (which at the first iteration consist only of the interpolation of the reference curve(s)).
    /// </summary>
    /// <param name="shiftGroupCollection">The data to construct the master curve(s).</param>
    /// <param name="shiftOrder">The order in which the curves are shifted and fitted to the master curve.</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void Iterate(ShiftGroupCollectionComplexCommonX shiftGroupCollection, IReadOnlyList<int> shiftOrder, MasterCurveCreationResultComplexCommonX result)
    {
      for (int iteration = 0; iteration < shiftGroupCollection.NumberOfIterations; ++iteration)
      {
        OneIteration(shiftGroupCollection, shiftOrder, result);
      }
    }

    /// <summary>
    /// Performs one iteration of the shift-and-fit procedure.
    /// </summary>
    /// <param name="shiftGroupCollection">The shift group collection.</param>
    /// <param name="shiftCurveOrder">The order in which the curves are shifted and fitted. This list does not contain the index of the fixed curve.</param>
    /// <param name="result">The result that accomodates the actual shift values.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    /// <exception cref="System.NotImplementedException">OptimizationMethod not implemented: " + shiftGroupCollection.OptimizationMethod.ToString()</exception>
    private static void OneIteration(ShiftGroupCollectionComplexCommonX shiftGroupCollection, IReadOnlyList<int> shiftCurveOrder, MasterCurveCreationResultComplexCommonX result)
    {
      double initialShift = 0;
      var oneShiftDataAcrossCurveGroups = new ShiftCurve<Complex64>[shiftGroupCollection.Count]; // contains the shift data for the same curve index, but for all curve groups
      var interpolations = result.ResultingInterpolation ?? throw new InvalidOperationException($"{nameof(result)} must already contain valid interpolation(s).");

      foreach (int indexOfCurveInShiftGroup in shiftCurveOrder)
      {
        double globalMinShift = double.MaxValue;
        double globalMaxShift = double.MinValue;
        double globalMinRange = double.MaxValue;
        for (int idxShiftGroup = 0; idxShiftGroup < shiftGroupCollection.Count; idxShiftGroup++)
        {
          var shiftGroup = shiftGroupCollection[idxShiftGroup];
          var shiftCurve = shiftGroup[indexOfCurveInShiftGroup];
          oneShiftDataAcrossCurveGroups[idxShiftGroup] = shiftCurve;
          var (xmin, xmax) = GetMinMaxOfFirstColumnForValidSecondColumn(shiftCurve.X, shiftCurve.Y, shiftGroup.XShiftBy, shiftGroup.LogarithmizeXForInterpolation, shiftGroup.LogarithmizeYForInterpolation);

          double localMaxShift;
          double localMinShift;
          double localRange;

          var (xMinOfInterpolation, xMaxOfInterpolation) = interpolations[idxShiftGroup].GetMinimumMaximumOfXValuesExceptForCurveIndex(indexOfCurveInShiftGroup);

          if (shiftGroup.LogarithmizeXForInterpolation)
          {
            localMaxShift = xMaxOfInterpolation - xmin;
            localMinShift = xMinOfInterpolation - xmax;
            localRange = xmax - xmin;
          }
          else
          {
            localMaxShift = Math.Log(xMaxOfInterpolation / xmin);
            localMinShift = Math.Log(xMinOfInterpolation / xmax);
            localRange = Math.Log(xmax / xmin);
          }
          globalMinShift = Math.Min(globalMinShift, localMinShift);
          globalMaxShift = Math.Max(globalMaxShift, localMaxShift);
          globalMinRange = Math.Min(globalMinRange, localRange);
        }

        // we reduce the maximum possible shifts a little in order to get at least one point overlapping
        double requiredShiftOverlap = globalMinRange * shiftGroupCollection.RequiredRelativeOverlap;
        requiredShiftOverlap = Math.Min(requiredShiftOverlap, 0.5 * (globalMaxShift - globalMinShift));
        // reduce the borders [globalMinShift, globalMaxShift] by the required shift overlap
        globalMinShift += requiredShiftOverlap;
        globalMaxShift -= requiredShiftOverlap;

        double currentShift = initialShift; // remember: this is either a offset or the natural logarithm of the shift factor
        switch (shiftGroupCollection.OptimizationMethod)
        {
          case OptimizationMethod.OptimizeSignedDifference:
            {
              currentShift =
              QuickRootFinding.ByBrentsAlgorithm(
                shift => GetMeanSignedPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, shiftGroupCollection), globalMinShift, globalMaxShift);
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifference:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, shiftGroupCollection);
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
                double res = GetMeanSquaredPenalty(interpolations, oneShiftDataAcrossCurveGroups, shift, shiftGroupCollection);
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
            throw new NotImplementedException("OptimizationMethod not implemented: " + shiftGroupCollection.OptimizationMethod.ToString());
        }

        if (currentShift.IsFinite())
        {
          result.ResultingShifts[indexOfCurveInShiftGroup] = currentShift;

          for (int idxShiftGroup = 0; idxShiftGroup < interpolations.Length; idxShiftGroup++)
          {
            // add the data for interpolation again, using the new shift
            interpolations[idxShiftGroup].AddXYColumn(currentShift, indexOfCurveInShiftGroup, oneShiftDataAcrossCurveGroups[idxShiftGroup].X, oneShiftDataAcrossCurveGroups[idxShiftGroup].Y, null, idxShiftGroup, shiftGroupCollection[idxShiftGroup]);
          }
          // now build up a new interpolation, where the shifted data is taken into account
          InterpolateAllColumnGroups(shiftGroupCollection, result);

          initialShift = currentShift;
        }
      }
    }


    /// <summary>
    /// Reinitializes the result. With that, new options can be used, for instance a new interpolation function.
    /// Typically, after calling this, you can call <see cref="Iterate(ShiftGroupCollection, List{List{int}}, MasterCurveCreationResult)"/> to iterate
    /// with the new interpolation function again.
    /// </summary>
    /// <param name="shiftGroupCollection">The data to construct the master curve(s).</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void ReInitializeResult(ShiftGroupCollectionComplexCommonX shiftGroupCollection, MasterCurveCreationResultComplexCommonX result)
    {
      for (int idxColumnGroup = 0; idxColumnGroup < shiftGroupCollection.Count; idxColumnGroup++)
      {
        var interpolation = result.ResultingInterpolation[idxColumnGroup];
        var shiftCurves = shiftGroupCollection[idxColumnGroup];
        interpolation.Clear();

        for (int idxCurve = 0; idxCurve < shiftCurves.Count; idxCurve++)
        {
          interpolation.AddXYColumn(result.ResultingShifts[idxCurve], idxCurve, shiftCurves[idxCurve].X, shiftCurves[idxCurve].Y, null, idxColumnGroup, shiftCurves);
        }
      }
      InterpolateAllColumnGroups(shiftGroupCollection, result);
    }

    /// <summary>
    /// Reinitializes the result (see <see cref="ReInitializeResult(ShiftGroupCollection, MasterCurveCreationResult)"/>)
    /// and then iterate anew.
    /// </summary>
    /// <param name="shiftGroupCollection">The data to construct the master curve(s).</param>
    /// <param name="result">The result of the master curve construction.</param>
    public static void ReIterate(ShiftGroupCollectionComplexCommonX shiftGroupCollection, MasterCurveCreationResultComplexCommonX result)
    {
      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation
      int maxColumns = shiftGroupCollection.Max(x => x.Count);

      var shiftOrder = shiftGroupCollection.ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = GetCurveIndexWithMostVariation(shiftGroupCollection[0]);
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate ?? 0);
      }

      var (indexOfReferenceColumnInColumnGroup, shiftOrderIndices) = GetFixedAndShiftedIndices(shiftOrder, maxColumns);

      ReInitializeResult(shiftGroupCollection, result);
      Iterate(shiftGroupCollection, shiftOrderIndices, result);
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
    public static (double min, double max) GetMinMaxOfFirstColumnForValidSecondColumn(IReadOnlyList<double> x, IReadOnlyList<Complex64> y, ShiftXBy shiftBy, bool doLogX, bool doLogY)
    {
      var len = Math.Min(x.Count, y.Count);
      var min = double.PositiveInfinity;
      var max = double.NegativeInfinity;

      for (int i = 0; i < len; i++)
      {
        var x1 = shiftBy == ShiftXBy.Factor ? Math.Log(x[i]) : x[i];
        var xv = doLogX ? Math.Log(x[i]) : x[i];
        var yv = doLogY ? new Complex64(Math.Log(y[i].Real), Math.Log(y[i].Imaginary)) : y[i];
        if (x1.IsFinite() && xv.IsFinite() && yv.Real.IsFinite() && yv.Imaginary.IsFinite())
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
    private static double GetMeanSignedPenalty(InterpolationInformationComplexCommonX[] interpolations, IReadOnlyList<ShiftCurve<Complex64>> oneShiftDataAcrossGroups, double shift, ShiftGroupCollectionComplexCommonX options)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int idxShiftGroup = 0; idxShiftGroup < oneShiftDataAcrossGroups.Count; idxShiftGroup++)
      {
        var shiftData = oneShiftDataAcrossGroups[idxShiftGroup];
        GetMeanSignedYDifference(interpolations[idxShiftGroup].InterpolationFunction, interpolations[idxShiftGroup].InterpolationMinimumX, interpolations[idxShiftGroup].InterpolationMaximumX, shiftData.X, shiftData.Y, shift, options[idxShiftGroup], out var penalty, out var points);

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
    private static double GetMeanSquaredPenalty(InterpolationInformationComplexCommonX[] interpolations, IReadOnlyList<ShiftCurve<Complex64>> oneShiftDataAcrossGroups, double shift, ShiftGroupCollectionComplexCommonX shiftGroupCollection)
    {

      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int idxShiftGroup = 0; idxShiftGroup < oneShiftDataAcrossGroups.Count; idxShiftGroup++)
      {
        var shiftData = oneShiftDataAcrossGroups[idxShiftGroup];
        GetMeanSquaredYDifference(interpolations[idxShiftGroup].InterpolationFunction, interpolations[idxShiftGroup].InterpolationMinimumX, interpolations[idxShiftGroup].InterpolationMaximumX, shiftData.X, shiftData.Y, shift, shiftGroupCollection[idxShiftGroup], out var penalty, out var points);

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
    public static void GetMeanSignedYDifference(Func<double, Complex64> interpolation, double interpolMin, double interpolMax, IReadOnlyList<double> x, IReadOnlyList<Complex64> y, double shift, ShiftGroupComplex options, out double penalty, out int evaluatedPoints)
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

        var yv = y[i];
        if (doLogY)
          yv = new Complex64(Math.Log(yv.Real), Math.Log(yv.Imaginary));

        if (xv.IsFinite() && yv.Real.IsFinite() && yv.Imaginary.IsFinite() && xv.IsInIntervalCC(interpolMin, interpolMax))
        {
          try
          {
            var yinterpol = interpolation(xv);
            double diff = yv.Real - yinterpol.Real + yv.Imaginary - yinterpol.Imaginary;
            penaltySum += diff;
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      penalty = penaltySum * Math.Abs(options.FittingWeight);
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
    public static void GetMeanSquaredYDifference(Func<double, Complex64> interpolationFunc, double interpolMin, double interpolMax, IReadOnlyList<double> x, IReadOnlyList<Complex64> y, double shift, ShiftGroupComplex options, out double penalty, out int evaluatedPoints)
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

        var yv = y[i];
        if (doLogY)
          yv = new Complex64(Math.Log(yv.Real), Math.Log(yv.Imaginary));

        if (xv.IsFinite() && yv.Real.IsFinite() && yv.Imaginary.IsFinite() && xv.IsInIntervalCC(interpolMin, interpolMax))
        {
          try
          {
            var yinterpol = interpolationFunc(xv);
            var diff = yv - yinterpol;
            penaltySum += diff.MagnitudeSquared();
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      penalty = penaltySum * RMath.Pow2(options.FittingWeight);
      evaluatedPoints = validPoints;

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanYDifference for shift={0} resulted in {1} ({2} points)", shift, penalty, evaluatedPoints));
    }
  }
}
