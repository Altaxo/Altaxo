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
using System.Linq;
using Altaxo.Calc;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// A collection of multiple x-y curves (see <see cref="ShiftCurve{Double}"/>) that will finally form one master curve.
  /// </summary>
  public class ShiftGroupComplexSeparateX : ShiftGroupBase<double>, IShiftGroup
  {
    protected ShiftCurve<double>[] _dataIm;

    /// <summary>
    /// Gets the fitting weight, a number number &gt; 0.
    /// </summary>
    public double FittingWeightIm { get; }

    /// <summary>
    /// Creates the fit function. Argument is the tuple consisting of X, Y, and optional YErr. Return value is a function that calculates y for a given x.
    /// </summary>
    public Func<(IReadOnlyList<double> XRe, IReadOnlyList<double> YRe, IReadOnlyList<double> XIm, IReadOnlyList<double> YIm), Func<double, Complex64>>? CreateInterpolationFunction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftGroupDouble"/> class.
    /// </summary>
    /// <param name="dataRe">Collection of multiple x-y curves that will finally form one master curve.</param>
    /// <param name="xShiftBy">Shift method, either additive or multiplicative.</param>
    /// <param name="fitWeightRe">The weight with which to participate in the fit. Has to be &gt; 0.</param>
    /// <param name="logarithmizeXForInterpolation">If true, the x-values are logarithmized prior to participating in the interpolation function.</param>
    /// <param name="logarithmizeYForInterpolation">If true, the y-values are logartihmized prior to participating in the interpolation function.</param>
    /// <param name="createInterpolationFunction">Function that creates the interpolation. Input are the x-array, y-array, and optionally, the array of y-errors. Output is an interpolation function which returns an interpolated y-value for a given x-value.</param>
    public ShiftGroupComplexSeparateX(IEnumerable<ShiftCurve<double>> dataRe, IEnumerable<ShiftCurve<double>> dataIm, ShiftXBy xShiftBy, double fitWeightRe, double fitWeightIm, bool logarithmizeXForInterpolation, bool logarithmizeYForInterpolation, Func<(IReadOnlyList<double> XRe, IReadOnlyList<double> YRe, IReadOnlyList<double> XIm, IReadOnlyList<double> YIm), Func<double, Complex64>>? createInterpolationFunction = null)
      : base(dataRe, xShiftBy, fitWeightRe, logarithmizeXForInterpolation, logarithmizeYForInterpolation)
    {
      _dataIm = dataIm.ToArray();
      FittingWeightIm = fitWeightIm;
      CreateInterpolationFunction = createInterpolationFunction;
    }

    /// <summary>
    /// Gets the index of the curve in the given group with the most variation. The variation is determined by calculating the absolute slope,
    /// with applying the logarithmic transformations according to the interpolation settings in that group.
    /// </summary>
    /// <returns>The index of the curve with most variation. If no suitable curve was found, then the return value is null.</returns>
    public int? GetCurveIndexWithMostVariation()
    {
      double maxAbsoluteSlope = 0;
      int idxMaxAbsoluteSlope = -1;
      for (int idxCurve = 0; idxCurve < _inner.Length; idxCurve++)
      {
        var (x, y) = TransformCurveRealForInterpolationAccordingToGroupOptions(idxCurve);
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

    public (IReadOnlyList<double> x, IReadOnlyList<double> y) TransformCurveRealForInterpolationAccordingToGroupOptions(int idx)
    {
      var xarr = new List<double>();
      var yarr = new List<double>();

      var curve = _inner[idx];
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

      return (xarr, yarr);
    }

    InterpolationInformationComplexSeparateX? _interpolationInformation;

    /// <inheritdoc/>
    public void InitializeInterpolation()
    {
      _interpolationInformation = new InterpolationInformationComplexSeparateX();
    }

    /// <inheritdoc/>
    public void AddCurveToInterpolation(int idxCurve, double shift)
    {
      if (_interpolationInformation is null)
        throw new InvalidOperationException($"{nameof(_interpolationInformation)} is not initialized. Call {nameof(InitializeInterpolation)} before.");

      _interpolationInformation.AddXYColumn(shift, idxCurve, _inner[idxCurve].X, _inner[idxCurve].Y, 0, this);
      _interpolationInformation.AddXYColumn(shift, idxCurve, _dataIm[idxCurve].X, _dataIm[idxCurve].Y, 1, this);
    }

  }
}

