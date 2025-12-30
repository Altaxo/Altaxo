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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A set of <see cref="IShiftGroup"/>s. The curves in these collections will be shifted with a common set of shift factors.
  /// For example, for complex data to be shifted, there will be a <see cref="ShiftGroupComplexCommonX"/>.
  /// </summary>
  public class ShiftGroupCollection : IReadOnlyList<IShiftGroup>
  {
    private IShiftGroup[] _shiftGroups;

    /// <summary>
    /// Contains the resulting shift values.
    /// </summary>
    private double?[] _shiftValues;

    /// <summary>
    /// Contains the shift errors.
    /// </summary>
    private double?[] _shiftErrors;

    /// <summary>
    /// Element is true if the shift value of the respective curve is not at its optimum, but
    /// is restricted by the boundaries given by the <see cref="RequiredRelativeOverlap"/>.
    /// </summary>
    private bool[] _isShiftValueRestrictedByBoundaries;

    /// <summary>
    /// If the element is true, the curve(s) with that curve index participate in the fit; otherwise, the element is false.
    /// </summary>
    private bool[] _isCurveParticipatingInFit;

    /// <summary>
    /// Contains the indices of the curves that participate in the fit. Contains the same information as <see cref="_isCurveParticipatingInFit"/>, but more convenient for foreach.. statements
    /// </summary>
    private int[] _curvesParticipatingInFit;

    /// <summary>
    /// If the element is true, the group with that group index participate in the fit; otherwise, the element is false.
    /// </summary>
    private bool[] _isGroupParticipatingInFit;

    /// <summary>
    /// Contains the indices of the groups that participate in the fit. Contains the same information as <see cref="_isGroupParticipatingInFit"/>, but more convenient for foreach.. statements
    /// </summary>
    private int[] _groupsParticipatingInFit;

    /// <summary>
    /// If a element is true, the index of that element can be used as curve index for the pivot, i.e. for the starting point of the master curve creation.
    /// </summary>
    private bool[] _isCurveSuitableForPivot;

    /// <summary>
    /// Resulting list of shift offsets or ln(shiftfactors).
    /// </summary>
    public IReadOnlyList<double?> ShiftValues => _shiftValues;

    /// <summary>
    /// Gets the shift errors.
    /// </summary>
    public IReadOnlyList<double?> ShiftErrors => _shiftErrors;

    /// <summary>
    /// Element is true if the shift value of the respective curve is not at its optimum, but
    /// is restricted by the boundaries given by the <see cref="RequiredRelativeOverlap"/>.
    /// </summary>
    public IReadOnlyList<bool> IsShiftValueRestrictedByBoundaries => _isShiftValueRestrictedByBoundaries;

    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; }

    /// <summary>
    /// Gets or sets the shift order strategy used to determine the order in which curves are fixed and fitted.
    /// </summary>
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
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 0</exception>
    public int NumberOfIterations
    {
      get { return _numberOfIterations; }
      init
      {
        if (!(value >= 0))
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
    /// Contains the indices of the groups that participate in the fit. Contains the same information as <see cref="_isGroupParticipatingInFit"/>, but more convenient for foreach.. statements
    /// </summary>
    public IReadOnlyList<int> GroupsParticipatingInFit => _groupsParticipatingInFit;

    /// <summary>
    /// Contains the indices of the curves that participate in the fit. Contains the same information as <see cref="_isCurveParticipatingInFit"/>, but more convenient for foreach.. statements
    /// </summary>
    public IReadOnlyList<int> CurvesParticipatingInFit => _curvesParticipatingInFit;

    /// <summary>
    /// If the element is true, the curve(s) with that curve index participate in the fit; otherwise, the element is false.
    /// </summary>
    public IReadOnlyList<bool> IsCurveParticipatingInFit => _isCurveParticipatingInFit;

    /// <summary>
    /// Gets the maximum number of curves across all groups (i.e. the maximum number of columns available).
    /// </summary>
    public int MaximumNumberOfCurves => _shiftValues.Length;

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
      _shiftValues = new double?[maxColumns];
      _shiftErrors = new double?[maxColumns];
      _isShiftValueRestrictedByBoundaries = new bool[maxColumns];
      EvaluateParticipatingCurvesAndGroups();
    }

    /// <inheritdoc/>
    public IShiftGroup this[int index] => _shiftGroups[index];

    /// <inheritdoc/>
    public int Count => _shiftGroups.Length;

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

    /// <summary>
    /// Evaluates and determines which curves and groups are participating in the fitting process.
    /// </summary>
    [MemberNotNull(nameof(_isGroupParticipatingInFit))]
    [MemberNotNull(nameof(_isCurveParticipatingInFit))]
    [MemberNotNull(nameof(_groupsParticipatingInFit))]
    [MemberNotNull(nameof(_curvesParticipatingInFit))]
    [MemberNotNull(nameof(_isCurveSuitableForPivot))]
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
    public void CreateMasterCurve(CancellationToken cancellationToken, IProgress<double>? progress)
    {
      if (NumberOfIterations == 0)
      {
        throw new InvalidOperationException($"If calling {nameof(CreateMasterCurve)}, the {nameof(NumberOfIterations)} must be >= 1, but currently it is 0.");
      }

      var shiftOrder = ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = _shiftGroups[0].GetCurveIndexWithMostVariation();
        pivotIndexCandidate = ClampPivotIndexCandidateToAvailablePivots(pivotIndexCandidate);
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate.Value);
      }

      var shiftOrderIndices = shiftOrder.GetShiftOrderIndices(MaximumNumberOfCurves).ToArray();
      var idxReferenceCurve = shiftOrderIndices[0];
      if (_isCurveSuitableForPivot[idxReferenceCurve] == false)
      {
        throw new InvalidOperationException($"Index {idxReferenceCurve} is not suitable as a starting index for master curve creation. Please choose another shift order with a variable pivot index.");
      }

      double initialShiftOfReferenceCurve = 0;
      _shiftValues[idxReferenceCurve] = initialShiftOfReferenceCurve; // set shift value of reference curve to 0
      foreach (var idxGroup in _groupsParticipatingInFit)
      {
        var shiftGroup = _shiftGroups[idxGroup];
        shiftGroup.InitializeInterpolation();
        shiftGroup.AddCurveToInterpolation(idxReferenceCurve, initialShiftOfReferenceCurve);

        // Make the initial interpolation for all column groups, using only the column(s) used as reference (shift 0)
        shiftGroup.Interpolate();
      }

      // now that we have a first interpolation using the reference curve, we can iterate
      // at the first iteration, the other curves will be added to the interpolation
      // and then, the quality of the master curve will be successivly improved
      Iterate(shiftOrderIndices, cancellationToken, progress);
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
    /// <param name="cancellationToken">The token to cancel the iteration.</param>
    /// <param name="progress">Progress result.</param>
    public void Iterate(IReadOnlyList<int> shiftOrder, CancellationToken cancellationToken, IProgress<double>? progress)
    {
      progress?.Report(0);
      for (int idxIteration = 0; idxIteration < NumberOfIterations && !cancellationToken.IsCancellationRequested; ++idxIteration)
      {
        OneIteration(shiftOrder, idxIteration, cancellationToken);
        progress?.Report((idxIteration + 1) / (double)NumberOfIterations);
      }

      CalculateShiftErrors();

      progress?.Report(1);
    }

    /// <summary>
    /// Performs one iteration of the shift-and-fit procedure.
    /// </summary>
    /// <param name="shiftCurveOrder">The order in which the curves are shifted and fitted. This first index in this list is the index of the fixed curve.</param>
    /// <param name="idxIteration">Index of the current iteration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private void OneIteration(IReadOnlyList<int> shiftCurveOrder, int idxIteration, CancellationToken cancellationToken)
    {
      // start the tracking of the x-minimum and x-maximum by adding the fixed curve
      TrackXMinimumMaximumOfMasterCurvePoints(shiftCurveOrder[0], _shiftValues[shiftCurveOrder[0]]!.Value, startNewTracking: true);

      foreach (int idxCurve in shiftCurveOrder.Skip(1).Where(idx => _isCurveParticipatingInFit[idx] == true))
      {
        if (cancellationToken.IsCancellationRequested)
        {
          break;
        }

        var (globalMinShift, globalMaxShift) = GetMinimumMaximumGlobalShiftAlt(idxCurve);

        double currentShift; // remember: this is either a offset or the natural logarithm of the shift factor
        switch (OptimizationMethod)
        {
          case OptimizationMethod.OptimizeAbsoluteDifference:
            {
              Func<double, double> optFunc = delegate (double shift)
              {
                double res = GetMeanAbsolutePenalty(idxCurve, shift);
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
          if (!RMath.IsInIntervalCC(currentShift, globalMinShift, globalMaxShift))
          {
          }

          _shiftValues[idxCurve] = currentShift;
          _isShiftValueRestrictedByBoundaries[idxCurve] = currentShift == globalMinShift || currentShift == globalMaxShift;

          foreach (int idxGroup in _groupsParticipatingInFit)
          {
            // Continue tracking of x-minimum and x-maximum by considering the curve
            _shiftGroups[idxGroup].TrackXMinimumMaximumOfMasterCurvePoints(idxCurve, currentShift, startNewTracking: false);
            // add the data for interpolation again, using the new shift
            _shiftGroups[idxGroup].AddCurveToInterpolation(idxCurve, currentShift);
            // now build up a new interpolation, where the shifted data is taken into account
            _shiftGroups[idxGroup].Interpolate();
          }
        }
        else
        {
          throw new InvalidOperationException($"For curve[{idxCurve}], no valid shift value could be evaluated by fitting. Consider using another optimization method.");
        }
      }
    }

    /// <summary>
    /// Tracks the x minimum and x maximum of the master curve points (independently of the interpolation points, for each iteration only those curves which were already considered).
    /// </summary>
    /// <param name="idxCurve">The index of the curve to consider.</param>
    /// <param name="shift">The shift value for this curve.</param>
    /// <param name="startNewTracking">If set to true, a new tracking will be started, i.e. the xmin and xmax of the curve (under consideration of the shift value) is
    /// set as the new tracked xminimum and xmaximum. If false, the xmin and xmax of the curve (under consideration) of the shift value is calculated, and then merged
    /// into the tracked xminimum and xmaximum.</param>
    public void TrackXMinimumMaximumOfMasterCurvePoints(int idxCurve, double shift, bool startNewTracking)
    {
      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        _shiftGroups[idxGroup].TrackXMinimumMaximumOfMasterCurvePoints(idxCurve, shift, startNewTracking);
      }
    }

    /// <summary>
    /// Gets the tracked x minimum and x maximum values. For explanation, see <see cref="TrackXMinimumMaximumOfMasterCurvePoints(int, double, bool)"/>.
    /// </summary>
    /// <returns>The tracked x-minimum and x-maximum values.</returns>
    public (double xmin, double xmax) GetTrackedXMinimumMaximum()
    {
      double xmin = double.PositiveInfinity;
      double xmax = double.NegativeInfinity;
      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        var (min, max) = this[idxGroup].GetTrackedXMinimumMaximum();
        xmin = Math.Min(xmin, min);
        xmax = Math.Max(xmax, max);
      }
      return (xmin, xmax);
    }

    /// <summary>
    /// Gets the minimum and the maximum of the possible shift value for the designated curve. Here, we use the points that form
    /// the master curve so far (in the current iteration) to calculate the values.
    /// </summary>
    /// <param name="idxCurve">The index of curve.</param>
    /// <returns>The minimum and maximum shift values by which the curve can be shifted.</returns>
    private (double globalMinShift, double globalMaxShift) GetMinimumMaximumGlobalShiftAlt(int idxCurve)
    {
      var globalMinShift = double.MaxValue;
      var globalMaxShift = double.MinValue;
      double globalMinRange = double.MaxValue;
      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        var shiftGroup = _shiftGroups[idxGroup];

        var (xMinOfMasterCurveSoFar, xMaxOfMasterCurveSoFar) = shiftGroup.GetTrackedXMinimumMaximum(); // if ShiftedByFactor, the values are already logarithmized!

        var (xmin, xmax) = shiftGroup.GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(idxCurve);
        (xmin, xmax) = (shiftGroup.XShiftBy, shiftGroup.LogarithmizeXForInterpolation) switch
        {
          (ShiftXBy.Offset, false) => (xmin, xmax),
          (ShiftXBy.Offset, true) => (Math.Exp(xmin), Math.Exp(xmax)),
          (ShiftXBy.Factor, false) => (Math.Log(xmin), Math.Log(xmax)),
          (ShiftXBy.Factor, true) => (xmin, xmax),
          _ => throw new NotImplementedException(),
        };


        var localMaxShift = xMaxOfMasterCurveSoFar - xmin;
        var localMinShift = xMinOfMasterCurveSoFar - xmax;
        var localRange = xmax - xmin;

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

      return (globalMinShift, globalMaxShift);
    }

    /// <summary>
    /// Calculates the errors of the shift values. The function assumes that the shift values
    /// were calculated before, and that the current interpolation is using all curves.
    /// </summary>
    private void CalculateShiftErrors()
    {
      for (int i = 0; i < _shiftErrors.Length; i++)
      {
        _shiftErrors[i] = null;
      }

      foreach (var idxCurve in _curvesParticipatingInFit)
      {
        if (!_isShiftValueRestrictedByBoundaries[idxCurve])
        {
          _shiftErrors[idxCurve] = CalculateShiftError(idxCurve);
        }
      }
    }


    /// <summary>
    /// Calculates the errors of the shift values. This function assumes that the shift values were already before.
    /// </summary>
    /// <param name="idxCurve">The number of the curve for which to calculate the shift error.</param>
    /// <returns>The shift error, or null if the shift error could not be calculated.</returns>
    private double? CalculateShiftError(int idxCurve)
    {
      if (!(ShiftValues[idxCurve] is { } shift))
      {
        return null;
      }

      var (globalMinShift, globalMaxShift) = GetMinimumMaximumGlobalShiftAlt(idxCurve);
      var penaltyMiddle = GetMeanSquaredPenalty(idxCurve, shift);

      double? shiftErr = null;
      for (int iExp = -8; iExp <= -4; ++iExp)
      {
        var delta = Math.Abs(globalMaxShift - globalMinShift) * RMath.Pow(10, iExp);
        var penaltyLeft = GetMeanSquaredPenalty(idxCurve, shift - delta);
        var penaltyRight = GetMeanSquaredPenalty(idxCurve, shift + delta);
        var deriv2nd = ((penaltyRight - penaltyMiddle) - (penaltyMiddle - penaltyLeft)) / (delta * delta);

        if (penaltyLeft > penaltyMiddle && penaltyRight > penaltyMiddle)
        {
          shiftErr = Math.Sqrt(penaltyMiddle * 2 / deriv2nd);
          break;
        }
        else
          shiftErr = null;
      }

      return shiftErr;
    }

    /// <summary>
    /// Reinitializes the result. With that, new options can be used, for instance a new interpolation function.
    /// Typically, after calling this, you can call <see cref="Iterate(IReadOnlyList{int}, CancellationToken, IProgress{double}?)"/> to iterate
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
          if (ShiftValues[idxCurve] is { } shift)
          {
            shiftGroup.AddCurveToInterpolation(idxCurve, shift);
          }
        }
        shiftGroup.Interpolate();
      }

      for (int i = 0; i < _shiftErrors.Length; i++)
      {
        _shiftErrors[i] = null;
      }
    }

    /// <summary>
    /// Reinitializes the result (see <see cref="ReInitializeResult"/>) 
    /// and then iterate anew with <see cref="Iterate(IReadOnlyList{int}, CancellationToken, IProgress{double}?)"/>.
    /// </summary>
    /// <param name="previousMasterCurve">A <see cref="ShiftGroupCollection"/> for which the master curve creation was successfully. The results, particularly the shifts,
    /// are used from that previous master curve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="progress">Progress reporter.</param>
    public void ReIterate(ShiftGroupCollection previousMasterCurve, CancellationToken cancellationToken, IProgress<double>? progress)
    {
      if (_shiftValues.Length != previousMasterCurve._shiftValues.Length)
        throw new InvalidOperationException("The number of curves in this shift group collection and in the previos shift group collection should match.");
      // use the shifts from the previous curve, but only then if the curve index is also used here
      for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; idxCurve++)
      {
        if (_isCurveParticipatingInFit[idxCurve])
        {
          _shiftValues[idxCurve] = previousMasterCurve.ShiftValues[idxCurve];
        }
        else
        {
          _shiftValues[idxCurve] = null;
        }
      }

      // First we create the initial interpolation of the master column
      // then we successively add columns by shifting the x and merge them with the interpolation

      var shiftOrder = ShiftOrder;
      if (shiftOrder.IsPivotIndexRequired && !shiftOrder.PivotIndex.HasValue)
      {
        var pivotIndexCandidate = _shiftGroups[0].GetCurveIndexWithMostVariation();
        shiftOrder = shiftOrder.WithPivotIndex(pivotIndexCandidate ?? 0);
      }

      var shiftOrderIndices = shiftOrder.GetShiftOrderIndices(MaximumNumberOfCurves).ToArray();
      var idxReferenceCurve = shiftOrderIndices[0];

      ReInitializeResult();
      Iterate(shiftOrderIndices, cancellationToken, progress);
    }

    /// <summary>
    /// Reinterpolates the master curves in all groups, using the current shift values.
    /// This call will also interpolate the curves in those groups, which do not participate in the master curve fitting.
    /// </summary>
    public void ReinterpolateAllGroups()
    {
      for (int idxGroup = 0; idxGroup < Count; idxGroup++)
      {
        var group = _shiftGroups[idxGroup];
        group.InitializeInterpolation();
        for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; idxCurve++)
        {
          if (ShiftValues[idxCurve] is { } shiftValue)
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
            if (ShiftValues[idxCurve] is { } shiftValue)
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
            if (ShiftValues[idxCurve] is { } shiftValue)
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
          if (ShiftValues[idxCurve] is { } shiftValue)
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
    /// Offsets the shift values (<see cref="ShiftValues"/>) by the provided value.
    /// </summary>
    /// <param name="shiftOffset">The shift offset.</param>
    public void SetShiftOffset(double shiftOffset)
    {
      for (int idxCurve = 0; idxCurve < MaximumNumberOfCurves; ++idxCurve)
      {
        if (_shiftValues[(idxCurve)] is { } shiftValue)
        {
          _shiftValues[idxCurve] = shiftValue + shiftOffset;
        }
      }
    }

    /// <summary>
    /// Calculates a mean signed penalty value for the current shift factor of the current column. By making the penalty value zero, the current column will fit optimally into the so far created master curve.
    /// </summary>
    /// <param name="idxCurve">Index of the curve.</param>
    /// <param name="shift">Current shift (direct offset or the natural logarithm of the shiftFactor).</param>
    /// <returns>The mean penalty value for the current shift factor of the current column.</returns>
    private double GetMeanAbsolutePenalty(int idxCurve, double shift)
    {
      double penaltySum = 0;
      int penaltyPoints = 0;
      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        var (penalty, points) = _shiftGroups[idxGroup].GetMeanAbsYDifference(idxCurve, shift);

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

      foreach (int idxGroup in _groupsParticipatingInFit)
      {
        var (penalty, points) = _shiftGroups[idxGroup].GetMeanSquaredYDifference(idxCurve, shift);
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

