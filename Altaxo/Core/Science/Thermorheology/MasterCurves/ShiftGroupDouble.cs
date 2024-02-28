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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A collection of multiple x-y curves (see <see cref="ShiftCurve{Double}"/>) that will finally form one master curve.
  /// </summary>
  public class ShiftGroupDouble : ShiftGroupBase, IShiftGroup
  {
    protected ShiftCurve<double>?[] _curves;

    /// <summary>
    /// Gets the fitting weight, a number number &gt; 0.
    /// </summary>
    public double FittingWeight { get; }

    /// <inheritdoc/>
    public bool ParticipateInFitByFitWeight => FittingWeight > 0;

    /// <inheritdoc/>
    public int NumberOfValueComponents => 1;


    /// <summary>
    /// Creates the fit function. Argument is the tuple consisting of X, Y, and optional YErr. Return value is a function that calculates y for a given x.
    /// </summary>
    public Func<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr), Func<double, double>> CreateInterpolationFunction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupDouble"/> class.
    /// </summary>
    /// <param name="data">Collection of multiple x-y curves that will finally form one master curve.</param>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="fitWeight">The weight with which to participate in the fit. Has to be &gt; 0.</param>
    /// <param name="logarithmizeXForInterpolation">If true, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If true, the y-values are logartihmized prior to participating in the interpolation function.</param>
    /// <param name="createInterpolationFunction">Function that creates the interpolation. Input are the x-array, y-array, and optionally, the array of y-errors. Output is an interpolation function which returns an interpolated y-value for a given x-value.</param>
    public ShiftGroupDouble(IEnumerable<ShiftCurve<double>?> data, ShiftXBy xShiftBy, double fitWeight, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation, Func<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr), Func<double, double>> createInterpolationFunction)
      : base(xShiftBy, logarithmizeXForInterpolation, logarithmizeYForInterpolation)
    {
      _curves = data.ToArray();
      FittingWeight = fitWeight;
      CreateInterpolationFunction = createInterpolationFunction;
    }

    /// <inheritdoc/>
    public int Count => _curves.Length;

    /// <inheritdoc/>
    public bool IsCurveSuitableForParticipatingInFit(int idxCurve)
    {
      var (x, _) = TransformCurveForInterpolationAccordingToGroupOptions(idxCurve);
      return x.Count >= 2 && x.Max() > x.Min();
    }

    public int? GetCurveIndexWithMostVariation()
    {
      double maxAbsoluteSlope = 0;
      int idxMaxAbsoluteSlope = -1;
      for (int idxCurve = 0; idxCurve < Count; idxCurve++)
      {
        var (x, y) = TransformCurveForInterpolationAccordingToGroupOptions(idxCurve);
        if (x.Count < 2)
          continue;
        var reg = new Altaxo.Calc.Regression.QuickLinearRegression();
        for (int i = 0; i < x.Count; i++)
        {
          reg.Add(x[i], y[i]);
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

    public (IReadOnlyList<double> x, IReadOnlyList<double> y) TransformCurveForInterpolationAccordingToGroupOptions(int idx)
    {
      var xarr = new List<double>();
      var yarr = new List<double>();

      if (_curves[idx] is { } curve)
      {

        for (int i = 0; i < curve.Count; ++i)
        {
          var x = curve.X[i];
          var y = curve.Y[i];

          if (LogarithmizeXForInterpolation)
            x = Math.Log(x);
          if (LogarithmizeYForInterpolation)
            y = Math.Log(y);

          if (x.IsFinite() && y.IsFinite())
          {
            xarr.Add(x);
            yarr.Add(y);
          }
        }
      }

      return (xarr, yarr);
    }

    InterpolationInformationDouble? _interpolationInformation;

    /// <inheritdoc/>
    public void InitializeInterpolation()
    {
      _interpolationInformation = new InterpolationInformationDouble();
    }

    /// <inheritdoc/>
    public void AddCurveToInterpolation(int idxCurve, double shift)
    {
      if (_interpolationInformation is null)
        throw new InvalidOperationException($"{nameof(_interpolationInformation)} is not initialized. Call {nameof(InitializeInterpolation)} before.");

      var curve = _curves[idxCurve];
      if (curve is not null)
      {
        _interpolationInformation.AddXYColumn(shift, idxCurve, curve.X, curve.Y, this);
      }
    }

    public void Interpolate()
    {
      if (_interpolationInformation is null)
      {
        throw NewExceptionNoInterpolationInformation;
      }

      var interpol = CreateInterpolationFunction((_interpolationInformation.XValues, _interpolationInformation.YValues, null));
      _interpolationInformation.InterpolationFunction = interpol;
    }

    /// <summary>
    /// Gets the minimum and maximum of the x-values, taking into account different options and whether the y-values are valid.
    /// </summary>
    /// <param name="idxCurve">Index of the curve.</param>
    /// <returns>Minimum and maximum of the x-values, for x and y values appropriate for the conditions given by the parameter.</returns>
    public override (double min, double max) GetXMinimumMaximumOfCurvePointsSuitableForInterpolation(int idxCurve)
    {
      var min = double.PositiveInfinity;
      var max = double.NegativeInfinity;

      if (_curves[idxCurve] is { } curve)
      {
        var x = curve.X;
        var y = curve.Y;
        var len = Math.Min(x.Count, y.Count);

        for (int i = 0; i < len; i++)
        {
          double x1 = XShiftBy == ShiftXBy.Factor ? Math.Log(x[i]) : x[i];
          double xv = LogarithmizeXForInterpolation ? Math.Log(x[i]) : x[i];
          double yv = LogarithmizeYForInterpolation ? Math.Log(y[i]) : y[i];
          if (x1.IsFinite() && xv.IsFinite() && yv.IsFinite())
          {
            min = Math.Min(min, xv);
            max = Math.Max(max, xv);
          }
        }
      }
      return (min, max);
    }

    /// <summary>
    /// Gets the minimum and maximum of the current x-values used for interpolation. Data points that belong
    /// to the curve with the index given in the argument are not taken into account.
    /// </summary>
    /// <param name="indexOfCurve">The index of curve.</param>
    /// <returns>The minimum and maximum of the x-values, except for those points that belong to the curve with index=<paramref name="indexOfCurve"/>.</returns>
    public (double min, double max) GetXMinimumMaximumOfInterpolationValuesExceptForCurveIndex(int indexOfCurve)
    {
      return _interpolationInformation is null
        ? throw NewExceptionNoInterpolationInformation
        : _interpolationInformation.GetMinimumMaximumOfXValuesExceptForCurveIndex(indexOfCurve);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetCurvePoints(int idxCurve, int idxValueComponent)
    {
      if (idxValueComponent != 0)
        throw new ArgumentOutOfRangeException($"{nameof(ShiftGroupDouble)} only has one value component, thus {nameof(idxValueComponent)} has to be 0, but is {idxValueComponent}.");

      var curve = _curves[idxCurve];
      if (curve is not null)
        return (curve.X, curve.Y);
      else
        return (Array.Empty<double>(), Array.Empty<double>());
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetShiftedCurvePoints(int idxCurve, int idxValueComponent, double shiftValue)
    {
      if (idxValueComponent != 0)
        throw new ArgumentOutOfRangeException($"{nameof(ShiftGroupDouble)} only has one value component, thus {nameof(idxValueComponent)} has to be 0, but is {idxValueComponent}.");

      var curve = _curves[idxCurve];
      if (curve is not null)
      {

        double[] xx;
        if (XShiftBy == ShiftXBy.Factor)
        {
          xx = curve.X.Select(x => x * Math.Exp(shiftValue)).ToArray();
        }
        else
        {
          xx = curve.X.Select(x => x + shiftValue).ToArray();
        }

        return (xx, curve.Y);
      }
      else
      {
        return (Array.Empty<double>(), Array.Empty<double>());
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<int> IndexOfCurve) GetMergedCurvePointsUsedForInterpolation(int idxValueComponent)
    {
      if (idxValueComponent != 0)
        throw new ArgumentOutOfRangeException($"{nameof(ShiftGroupDouble)} only has one value component, thus {nameof(idxValueComponent)} has to be 0, but is {idxValueComponent}.");

      if (_interpolationInformation is not null)
      {
        var x = LogarithmizeXForInterpolation ? _interpolationInformation.XValues.Select(xx => Math.Exp(xx)).ToArray() : _interpolationInformation.XValues;
        var y = LogarithmizeYForInterpolation ? _interpolationInformation.YValues.Select(yy => Math.Exp(yy)).ToArray() : _interpolationInformation.YValues;
        return (x, y, _interpolationInformation.IndexOfCurve);
      }
      else
      {
        return (Array.Empty<double>(), Array.Empty<double>(), Array.Empty<int>());
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double> X, IReadOnlyList<double> Y) GetInterpolatedCurvePoints(int idxValueComponent, int numberOfInterpolationPoints = 1001)
    {
      if (idxValueComponent != 0)
        throw new ArgumentOutOfRangeException($"{nameof(ShiftGroupDouble)} only has one value component, thus {nameof(idxValueComponent)} has to be 0, but is {idxValueComponent}.");

      var xcol = new List<double>();
      var ycol = new List<double>();

      if (_interpolationInformation?.InterpolationFunction is { } interpolationResult)
      {
        var minX = _interpolationInformation.InterpolationMinimumX;
        var maxX = _interpolationInformation.InterpolationMaximumX;
        for (int i = 0; i < numberOfInterpolationPoints; ++i)
        {
          var r = i / (numberOfInterpolationPoints + 1d);
          var x = (LogarithmizeXForInterpolation, XShiftBy) switch
          {
            (true, ShiftXBy.Factor) => minX * (1 - r) + maxX * r,// logarithmic spacing, x is logarithmized before and after
            (false, ShiftXBy.Factor) => Math.Exp(Math.Log(minX) * (1 - r) + Math.Log(maxX) * r),// logarithmic spacing, x not logarithmized before and not after
            (true, ShiftXBy.Offset) => Math.Log(Math.Exp(minX) * (1 - r) + Math.Exp(maxX) * r),// linear spacing, x was already logarithmized for interpolation, and has to be afterward again
            (false, ShiftXBy.Offset) => minX * (1 - r) + maxX * r,// linear spacing, x not logarithmized before and after
            _ => throw new NotImplementedException(),
          };
          var y = interpolationResult(x);

          if (LogarithmizeXForInterpolation)
            x = Math.Exp(x);
          if (LogarithmizeYForInterpolation)
            y = Math.Exp(y);

          xcol.Add(x);
          ycol.Add(y);
        }
      }

      return (xcol, ycol);
    }


    /// <summary>
    /// Gets the mean difference between the y column and the interpolation function, provided that the x column is shifted by a factor.
    /// </summary>
    /// <param name="idxCurve">The index of the curve to fit.</param>
    /// <param name="shift">Shift offset (direct offset or natural logarithm of the shiftFactor for the new part of the master curve.</param>
    /// <returns>Returns the calculated penalty value (mean difference between interpolation curve and provided data),
    /// and the number of points (of the new part of the curve) used for calculating the penalty value.</returns>
    /// 
    public (double Penalty, int EvaluatedPoints) GetMeanAbsYDifference(int idxCurve, double shift)
    {
      if (_interpolationInformation is null)
      {
        throw NewExceptionNoInterpolationInformation;
      }


      var curve = _curves[idxCurve] ?? throw new InvalidProgramException($"The curve with index {idxCurve} is null.");
      var x = curve.X;
      var y = curve.Y;
      var interpolation = _interpolationInformation.InterpolationFunction;
      var interpolMin = _interpolationInformation.InterpolationMinimumX;
      var interpolMax = _interpolationInformation.InterpolationMaximumX;
      int len = Math.Min(x.Count, y.Count);
      int validPoints = 0;
      bool doLogX = LogarithmizeXForInterpolation;
      bool doLogY = LogarithmizeYForInterpolation;
      bool shiftXByOffset = XShiftBy == ShiftXBy.Offset;
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
            penaltySum += Math.Abs(diff);
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      var penalty = penaltySum * Math.Abs(FittingWeight);
      var evaluatedPoints = validPoints;

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanYDifference for shift={0} resulted in {1} ({2} points)", shift, penalty, evaluatedPoints));
      return (penalty, evaluatedPoints);
    }

    public (double Penalty, int EvaluatedPoints) GetMeanSquaredYDifference(int idxCurve, double shift)
    {
      if (_interpolationInformation is null) throw NewExceptionNoInterpolationInformation;


      var curve = _curves[idxCurve] ?? throw new InvalidProgramException($"The curve with index {idxCurve} is null.");
      var x = curve.X;
      var y = curve.Y;
      var interpolation = _interpolationInformation.InterpolationFunction;
      var interpolMin = _interpolationInformation.InterpolationMinimumX;
      var interpolMax = _interpolationInformation.InterpolationMaximumX;

      int len = Math.Min(x.Count, y.Count);
      int validPoints = 0;
      bool doLogX = LogarithmizeXForInterpolation;
      bool doLogY = LogarithmizeYForInterpolation;
      bool shiftXByOffset = XShiftBy == ShiftXBy.Offset;
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
            penaltySum += diff * diff;
            validPoints++;
          }
          catch (Exception)
          {
          }
        }
      }
      var penalty = penaltySum * RMath.Pow2(FittingWeight);
      var evaluatedPoints = validPoints;

      //System.Diagnostics.Debug.WriteLine(string.Format("GetMeanYDifference for shift={0} resulted in {1} ({2} points)", shift, penalty, evaluatedPoints));
      return (penalty, evaluatedPoints);
    }
  }
}

