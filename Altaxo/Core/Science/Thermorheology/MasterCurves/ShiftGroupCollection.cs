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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.RootFinding;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A set of <see cref="IShiftGroup"/>s. The curves in these collections will be shifted with a common set of shift factors.
  /// For example, for complex data to be shifted, there will be a <see cref="ShiftGroupComplexCommonX"/>.
  /// </summary>
  public class ShiftGroupCollection : IReadOnlyList<IShiftGroup>
  {
    IShiftGroup[] _shiftGroups;

    /// <summary>
    /// Resulting list of shift offsets or ln(shiftfactors).
    /// </summary>
    public IReadOnlyList<double?> ResultingShifts => _resultingShifts;

    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; }

    public ShiftOrder.IShiftOrder ShiftOrder { get; init; } = new ShiftOrder.FirstToLast();

    protected int _numberOfIterations = 20;

    /// <summary>
    /// Gets or sets the number of iterations. Must be greater than or equal to 1.
    /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
    /// increase the quality of the fit.
    /// </summary>
    /// <value>
    /// The number of iterations for master curve creation.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
    public int NumberOfIterations
    {
      get { return _numberOfIterations; }
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a number >= 1");

        _numberOfIterations = value;
      }
    }

    protected double _requiredRelativeOverlap = 0;
    /// <summary>
    /// Gets/sets the required relative overlap. The default value is 0, which means that a curve part only needs to touch the rest of the master curve.
    /// Setting this to a value, for instance to 0.1, means that a curve part needs an overlapping of 10% (of its x-range) with the rest of the master curve.
    /// This value can also be set to negative values. For instance, setting it to -1 means that a curve part could be in 100% distance (of its x-range) to the rest of the master curve.
    /// </summary>
    /// <value>
    /// The required overlap.
    /// </value>
    public double RequiredRelativeOverlap
    {
      get => _requiredRelativeOverlap;
      init
      {
        if (double.IsNaN(value))
          throw new ArgumentOutOfRangeException(nameof(value), "Must be a valid number");

        _requiredRelativeOverlap = value;
      }
    }


    /// <summary>
    /// Contains the resulting shift values.
    /// </summary>
    private double?[] _resultingShifts;

    /// <summary>
    /// If the element is true, the curve(s) with that curve index participate in the fit; otherwise, the element is false.
    /// </summary>
    private bool[] _isCurveParticipatingInFit;

    /// <summary>
    /// Contains the indices of the curves that participate in the fit. Contains the same information as <see cref="_isCurveParticipatingInFit"/>, but more convienient for foreach.. statements
    /// </summary>
    private int[] _curvesParticipatingInFit;

    /// <summary>
    /// If the element is true, the group with that group index participate in the fit; otherwise, the element is false.
    /// </summary>
    private bool[] _isGroupParticipatingInFit;

    /// <summary>
    /// Contains the indices of the groups that participate in the fit. Contains the same information as <see cref="_isGroupParticipatingInFit"/>, but more convienient for foreach.. statements
    /// </summary>
    private int[] _groupsParticipatingInFit;

    /// <summary>
    /// If a element is true, the index of that element can be used as curve index for the pivot, i.e. for the starting point of the master curve creation.
    /// </summary>
    private bool[] _isCurveSuitableForPivot;

    /// <summary>
    /// Contains the indices of the groups that participate in the fit. Contains the same information as <see cref="_isGroupParticipatingInFit"/>, but more convienient for foreach.. statements
    /// </summary>
    public IReadOnlyCollection<int> GroupsParticipatingInFit => _groupsParticipatingInFit;

    /// <summary>
    /// Contains the indices of the curves that participate in the fit. Contains the same information as <see cref="_isCurveParticipatingInFit"/>, but more convienient for foreach.. statements
    /// </summary>
    public IReadOnlyCollection<int> CurvesParticipatingInFit => _curvesParticipatingInFit;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupCollection"/> class.
    /// </summary>
    /// <param name="data">The set of <see cref="ShiftGroupDouble"/>s.</param>
    public ShiftGroupCollection(IEnumerable<IShiftGroup> data)
    {
      _shiftGroups = data.ToArray();
      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation
      int maxColumns = _shiftGroups.Max(x => x.Count);
      _resultingShifts = new double?[maxColumns];
      EvaluateParticipatingCurvesAndGroups();
    }

    /// <inheritdoc/>
    public IShiftGroup this[int index] => _shiftGroups[index];

    /// <inheritdoc/>
    public int Count => _shiftGroups.Length;

    public int MaximumNumberOfCurves => _resultingShifts.Length;

    /// <inheritdoc/>
    public IEnumerator<IShiftGroup> GetEnumerator()
    {
      return ((IEnumerable<IShiftGroup>)_shiftGroups).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _shiftGroups.GetEnumerator();
    }

    public void EvaluateParticipatingCurvesAndGroups()
    {
      // First assume that all groups and curves will participate in the fit
      _isGroupParticipatingInFit = new bool[Count];
      _isCurveParticipatingInFit = new bool[MaximumNumberOfCurves];

      var curvesSuitableForFit = new bool[Count, MaximumNumberOfCurves];

      // Exclude groups by fit weight
      for (int idxGroup = 0; idxGroup < Count; ++idxGroup)
      {
        for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
        {
          curvesSuitableForFit[idxGroup, idxCurve] = _shiftGroups[idxGroup].IsCurveSuitableForParticipatingInFit(idxCurve);
        }
      }

      // Exclude groups by fit weight, and number of curves in one group
      for (int idxGroup = 0; idxGroup < Count; ++idxGroup)
      {
        _isGroupParticipatingInFit[idxGroup] = _shiftGroups[idxGroup].ParticipateInFitByFitWeight;
        // The group must have at least two curves to fit, otherwise, we can exclude it
        int sumCurves = 0;
        for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
        {
          sumCurves += curvesSuitableForFit[idxGroup, idxCurve] ? 1 : 0;
        }
        _isGroupParticipatingInFit[idxGroup] &= (sumCurves >= 2);

        // if this group is not participating in the fit, we set all curves in this group to "not participating"
        if (!_isGroupParticipatingInFit[idxGroup])
        {
          for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
          {
            curvesSuitableForFit[idxGroup, idxCurve] = false;
          }
        }
      }

      // now make convenience collections that can be used in foreach statements
      _groupsParticipatingInFit = _isGroupParticipatingInFit.Select((x, i) => (x, i)).Where(e => e.x).Select(e => e.i).ToArray();

      if (_groupsParticipatingInFit.Length == 0)
      {
        throw new InvalidOperationException("No group was found that can participate in master curve creation. Thus master curve creation is not possible");
      }

      // the pivot index can only be used for curve indices for which all participating groups have a suitable curve
      _isCurveSuitableForPivot = new bool[MaximumNumberOfCurves];

      // if in any group no curve at a given index exists, then we can exclude that curve index
      for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
      {
        int sumGroups = 0;
        for (int idxGroup = 0; idxGroup < Count; ++idxGroup)
        {
          sumGroups += (_isGroupParticipatingInFit[idxGroup] && curvesSuitableForFit[idxGroup, idxCurve]) ? 1 : 0;
        }
        _isCurveParticipatingInFit[idxCurve] = (sumGroups >= 1);
        _isCurveSuitableForPivot[idxCurve] = (sumGroups == _groupsParticipatingInFit.Length); // all participating groups must have a suitable curve
      }

      // now make convenience collections that can be used in foreach statements
      _curvesParticipatingInFit = _isCurveParticipatingInFit.Select((x, i) => (x, i)).Where(e => e.x).Select(e => e.i).ToArray();

      if (_curvesParticipatingInFit.Length == 0)
      {
        throw new InvalidOperationException("No curve(s) were found that can participate in master curve creation. Thus master curve creation is not possible");
      }
      else if (_curvesParticipatingInFit.Length == 1)
      {
        throw new InvalidOperationException("Only one curve was found that can participate in master curve creation. Thus master curve creation is not possible");
      }


      if (!_isCurveSuitableForPivot.Any(b => b == true))
        throw new InvalidOperationException("No curve index was found for which all groups contain a valid curve. Thus no starting curve index for master curve creation is available");

    }



    /// <summary>
    /// Creates one or multiple master curve(s).
    /// </summary>
    public void CreateMasterCurve()
    {
      var shiftOrder = ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = _shiftGroups[0].GetCurveIndexWithMostVariation();
        pivotIndexCandidate = ClampPivotIndexCandidateToAvailablePivots(pivotIndexCandidate);
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate.Value);
      }
      var (idxReferenceCurve, shiftOrderIndices) = GetFixedAndShiftedIndices(shiftOrder, MaximumNumberOfCurves);

      if (_isCurveSuitableForPivot[idxReferenceCurve] == false)
      {
        throw new InvalidOperationException($"Index {idxReferenceCurve} is not suitable as a starting index for master curve creation. Please choose another shift order with a variable pivot index.");
      }

      _resultingShifts[idxReferenceCurve] = 0; // set shift value of reference curve to 0
      foreach (var idxGroup in _groupsParticipatingInFit)
      {
        var shiftGroup = _shiftGroups[idxGroup];
        shiftGroup.InitializeInterpolation();
        shiftGroup.AddCurveToInterpolation(idxReferenceCurve, ResultingShifts[idxReferenceCurve].Value);

        // Make the initial interpolation for all column groups, using only the column(s) used as reference (shift 0)
        shiftGroup.Interpolate();
      }

      // now that we have a first interpolation using the reference curve, we can iterate
      // at the first iteration, the other curves will be added to the interpolation
      // and then, the quality of the master curve will be successivly improved
      Iterate(shiftOrderIndices);
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
    /// Uses the pivot index candidate and looks for the next possible pivot index around this candidate index.
    /// </summary>
    /// <param name="pivotIndexCandidate">The pivot index candidate.</param>
    /// <returns>A valid pivot index.</returns>
    /// <exception cref="System.InvalidProgramException">By now, we should have found a pivot index. If not, the {_isCurveSuitableForPivot} does not contain elements of value true, which should be catched in the constructor.</exception>
    public int ClampPivotIndexCandidateToAvailablePivots(int? pivotIndexCandidate)
    {
      var startIndex = pivotIndexCandidate ?? 0;

      for (int i = 0; i < MaximumNumberOfCurves; i++)
      {
        if ((startIndex + i) < _isCurveSuitableForPivot.Length && _isCurveSuitableForPivot[startIndex + i])
          return startIndex + i;
        if ((startIndex - i) >= 0 && _isCurveSuitableForPivot[startIndex - i])
          return startIndex - i;
      }
      throw new InvalidProgramException($"By now, we should have found a pivot index. If not, the {_isCurveSuitableForPivot} does not contain elements of value true, which should be catched in the constructor.");
    }

    /// <summary>
    /// Performs iteration to create or refine the master curve. There must already exist an interpolation for each curve group
    /// (which at the first iteration consist only of the interpolation of the reference curve(s)).
    /// </summary>
    /// <param name="shiftOrder">The order in which the curves are shifted and fitted to the master curve.</param>
    public void Iterate(IReadOnlyList<int> shiftOrder)
    {
      for (int iteration = 0; iteration < NumberOfIterations; ++iteration)
      {
        OneIteration(shiftOrder);
      }
    }

    /// <summary>
    /// Performs one iteration of the shift-and-fit procedure.
    /// </summary>
    /// <param name="shiftCurveOrder">The order in which the curves are shifted and fitted. This list does not contain the index of the fixed curve.</param>
    private void OneIteration(IReadOnlyList<int> shiftCurveOrder)
    {
      foreach (int idxCurve in shiftCurveOrder.Where(idx => _isCurveParticipatingInFit[idx] == true))
      {
        double globalMinShift = double.MaxValue;
        double globalMaxShift = double.MinValue;
        double globalMinRange = double.MaxValue;
        foreach (int idxGroup in _groupsParticipatingInFit)
        {
          var shiftGroup = _shiftGroups[idxGroup];
          var (xmin, xmax) = shiftGroup.GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(idxCurve);

          double localMaxShift;
          double localMinShift;
          double localRange;

          var (xMinOfInterpolation, xMaxOfInterpolation) = shiftGroup.GetXMinimumMaximumOfInterpolationValuesExceptForCurveIndex(idxCurve);

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
        double requiredShiftOverlap = globalMinRange * RequiredRelativeOverlap;
        requiredShiftOverlap = Math.Min(requiredShiftOverlap, 0.5 * (globalMaxShift - globalMinShift));
        // reduce the borders [globalMinShift, globalMaxShift] by the required shift overlap
        globalMinShift += requiredShiftOverlap;
        globalMaxShift -= requiredShiftOverlap;

        double currentShift; // remember: this is either a offset or the natural logarithm of the shift factor
        switch (OptimizationMethod)
        {
          case OptimizationMethod.OptimizeSignedDifference:
            {
              currentShift =
              QuickRootFinding.ByBrentsAlgorithm(
                shift => GetMeanSignedPenalty(idxCurve, shift), globalMinShift, globalMaxShift);
            }
            break;

          case OptimizationMethod.OptimizeSquaredDifference:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanSquaredPenalty(idxCurve, shift);
                //Current.Console.WriteLine("Eval for shift={0}: {1}", shift, res);
                return res;
              };

              var optimizationMethod = new StupidLineSearch(new Simple1DCostFunction(optFunc));
              var vec = CreateVector.Dense<double>(1);
              vec[0] = globalMinShift;

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
                double res = GetMeanSquaredPenalty(idxCurve, shift);
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
            throw new NotImplementedException($"OptimizationMethod not implemented: {OptimizationMethod}");
        }

        if (currentShift.IsFinite())
        {
          _resultingShifts[idxCurve] = currentShift;

          foreach (int idxShiftGroup in _groupsParticipatingInFit)
          {
            // add the data for interpolation again, using the new shift
            _shiftGroups[idxShiftGroup].AddCurveToInterpolation(idxCurve, currentShift);
            // now build up a new interpolation, where the shifted data is taken into account
            _shiftGroups[idxShiftGroup].Interpolate();
          }
        }
      }
    }

    /// <summary>
    /// Reinitializes the result. With that, new options can be used, for instance a new interpolation function.
    /// Typically, after calling this, you can call <see cref="Iterate(ShiftGroupCollectionDouble, List{List{int}}, MasterCurveCreationResultDouble)"/> to iterate
    /// with the new interpolation function again.
    /// </summary>
    public void ReInitializeResult()
    {
      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        var shiftGroup = _shiftGroups[idxGroup];

        shiftGroup.InitializeInterpolation();
        foreach (var idxCurve in _curvesParticipatingInFit)
        {
          if (ResultingShifts[idxCurve] is { } shift)
          {
            shiftGroup.AddCurveToInterpolation(idxCurve, shift);
          }
        }
        shiftGroup.Interpolate();
      }
    }

    /// <summary>
    /// Reinitializes the result (see <see cref="ReInitializeResult(ShiftGroupCollectionDouble, MasterCurveCreationResultDouble)"/>)
    /// and then iterate anew.
    /// </summary>
    /// <param name="previousMasterCurve">A <see cref="ShiftGroupCollection"/> for which the master curve creation was successfully. The results, particularly the shifts,
    /// are used from that previous master curve.</param>
    public void ReIterate(ShiftGroupCollection previousMasterCurve)
    {
      if (_resultingShifts.Length != previousMasterCurve._resultingShifts.Length)
        throw new InvalidOperationException("The number of curves in this shift group collection and in the previos shift group collection should match.");
      // use the shifts from the previous curve, but only then if the curve index is also used here
      for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; idxCurve++)
      {
        if (_isCurveParticipatingInFit[idxCurve])
        {
          _resultingShifts[idxCurve] = previousMasterCurve.ResultingShifts[idxCurve];
        }
        else
        {
          _resultingShifts[idxCurve] = null;
        }
      }
      Array.Copy(previousMasterCurve._resultingShifts, _resultingShifts, _resultingShifts.Length);

      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation

      var shiftOrder = ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = _shiftGroups[0].GetCurveIndexWithMostVariation();
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate ?? 0);
      }

      var (indexOfReferenceColumnInColumnGroup, shiftOrderIndices) = GetFixedAndShiftedIndices(shiftOrder, MaximumNumberOfCurves);

      ReInitializeResult();
      Iterate(shiftOrderIndices);
    }

    /// <summary>
    /// Reinterpolates the master curves in all groups, using the current shift values.
    /// This call will also interpolate the curves in thoses groups, which do not participate in the master curve fitting.
    /// </summary>
    public void ReinterpolateAllGroups()
    {
      for (int idxGroup = 0; idxGroup < Count; idxGroup++)
      {
        var group = _shiftGroups[idxGroup];
        group.InitializeInterpolation();
        for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; idxCurve++)
        {
          if (ResultingShifts[idxCurve] is { } shiftValue)
          {
            group.AddCurveToInterpolation(idxCurve, shiftValue);
          }
        }
        group.Interpolate();
      }
    }

    /// <summary>
    /// Gets the shift offset. After creation of the master curve using the low level interface, the entire curve can be shifted, so that the value shift=0 is at another point.
    /// The point is determined by the options.
    /// </summary>
    /// <param name="referencePropertyValue">If not null, this is the reference value. Most probably in master curve construction, this is the reference temperature.</param>
    /// <param name="useExactReferencePropertyValue">If true, the exact value given by <paramref name="referencePropertyValue"/> is trying to use, even if this value is not the property value of any curve.
    /// In this case, the shift values are interpolated over the property values, and then the shift value is interpolated. If interpolation is not possible, the fallback method is then to not use the exact reference value.</param>
    /// <param name="curveProperties">The properties of the curves. The array must have the same length as the number of curves in this instance. If <paramref name="referencePropertyValue"/> is a reference temperature,
    /// then the values in this array must be temperatures, each temperature associated with a curve.</param>
    /// <param name="indexOfReferenceCurve">If <paramref name="referencePropertyValue"/> is null, then the curve with this index is used as the reference curve. If at this index no shift
    /// information is available, then the curve nextmost to this index is used as the reference curve.</param>
    /// <returns>A tuple containing the shift offset, with which the entire curve should be shifted, and the actually used reference value (e.g. reference temperature).</returns>
    public (double shiftOffset, double? referenceValue) GetShiftOffset(double? referencePropertyValue, bool useExactReferencePropertyValue, double?[] curveProperties, int indexOfReferenceCurve)
    {
      double shiftOffset;
      double? referenceValueUsed = null;
      if (referencePropertyValue.HasValue)
      {
        if (useExactReferencePropertyValue)
        {
          // we have to make an interpolation of the shift values (y) versus the property1 values (x), and then
          // try to get the shift value at the reference property
          var listX = new List<double>();
          var listY = new List<double>();
          for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; idxCurve++)
          {
            if (ResultingShifts[idxCurve] is { } shiftValue)
            {
              if (curveProperties[idxCurve] is { } propertyValue)
              {
                double x = propertyValue;
                double y = shiftValue;

                if (!double.IsNaN(x) && !double.IsNaN(y))
                {
                  listX.Add(x);
                  listY.Add(y);
                }
              }
            }
          }

          if (listX.Count < 2)
          {
            return GetShiftOffset(referencePropertyValue, useExactReferencePropertyValue: false, curveProperties: curveProperties, indexOfReferenceCurve: indexOfReferenceCurve);
          }

          Altaxo.Calc.Interpolation.IInterpolationFunctionOptions interpolation;

          if (listX.Count <= 2)
            interpolation = new Altaxo.Calc.Interpolation.PolynomialRegressionAsInterpolationOptions(order: listX.Count - 1);
          else
            interpolation = new Altaxo.Calc.Interpolation.CrossValidatedCubicSplineOptions();

          var interpolationFunc = interpolation.Interpolate(listX, listY);
          shiftOffset = interpolationFunc.GetYOfX(referencePropertyValue.Value);
          referenceValueUsed = referencePropertyValue.Value;
        }
        else // not using the exact reference value
        {
          // we search for the nearest index that has shift information available
          double minDistance = double.PositiveInfinity;
          shiftOffset = 0;
          for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
          {
            if (ResultingShifts[idxCurve] is { } shiftValue)
            {
              if (curveProperties[idxCurve] is { } propertyValue)
              {
                var distance = Math.Abs(propertyValue - referencePropertyValue.Value);
                if (distance < minDistance)
                {
                  minDistance = distance;
                  shiftOffset = shiftValue;
                  referenceValueUsed = propertyValue;
                }
              }
            }
          }
        }
      }
      else // we use the reference index
      {
        // we search for the nearest index that has shift information available
        double minDistance = double.PositiveInfinity;
        shiftOffset = 0;
        for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
        {
          if (ResultingShifts[idxCurve] is { } shiftValue)
          {
            var distance = Math.Abs(idxCurve - indexOfReferenceCurve);
            if (distance < minDistance)
            {
              minDistance = distance;
              shiftOffset = shiftValue;
              referenceValueUsed = curveProperties[idxCurve];
            }
          }
        }

      }
      return (shiftOffset, referenceValueUsed);
    }


    /// <summary>
    /// Offsets the shift values (<see cref="ResultingShifts"/>) by the provided value.
    /// </summary>
    /// <param name="shiftOffset">The shift offset.</param>
    public void SetShiftOffset(double shiftOffset)
    {
      for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
      {
        if (_resultingShifts[(idxCurve)] is { } shiftValue)
        {
          _resultingShifts[idxCurve] = shiftValue + shiftOffset;
        }
      }
    }

    /// <summary>
    /// Calculates a mean signed penalty value for the current shift factor of the current column. By making the penalty value zero, the current column will fit optimally into the so far created master curve.
    /// </summary>
    /// <param name="idxCurve">Index of the curve.</param>
    /// <param name="shift">Current shift (direct offset or the natural logarithm of the shiftFactor).</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private double GetMeanSignedPenalty(int idxCurve, double shift)
    {
      double penaltySum = 0;
      int penaltyPoints = 0;
      for (int idxShiftGroup = 0; idxShiftGroup < Count; idxShiftGroup++)
      {
        var (penalty, points) = _shiftGroups[idxShiftGroup].GetMeanSignedYDifference(idxCurve, shift);

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
    /// <param name="idxCurve">Index of the curve (in all groups).</param>
    /// <param name="shift">Current shift (direct or log of shiftFactor).</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private double GetMeanSquaredPenalty(int idxCurve, double shift)
    {
      double penaltySum = 0;
      int penaltyPoints = 0;

      for (int idxShiftGroup = 0; idxShiftGroup < Count; idxShiftGroup++)
      {
        var (penalty, points) = _shiftGroups[idxShiftGroup].GetMeanSquaredYDifference(idxCurve, shift);
        if (points > 0)
        {
          penaltySum += penalty;
          penaltyPoints += points;
        }
      }

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanPenalty for shift={0} resulted in {1} ({2} points)", shift, penaltySum, penaltyPoints));

      return penaltyPoints > 0 ? penaltySum / penaltyPoints : float.MaxValue;
    }

  }
}

