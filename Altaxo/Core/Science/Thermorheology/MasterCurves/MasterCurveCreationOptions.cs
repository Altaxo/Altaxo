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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Calc.Interpolation;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public abstract record MasterCurveGroupOptions : Main.IImmutable
  {
    /// <summary>Logarithmize x values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeXForInterpolation { get; init; }

    /// <summary>Logarithmize y values before adding to the interpolation curve. (Only for interpolation).</summary>
    public bool LogarithmizeYForInterpolation { get; init; }

    /// <summary>
    /// Determines how to shift the x values: either by factor or by offset. Use offset if the original data are already logarithmized.
    /// </summary>
    public ShiftXBy XShiftBy { get; init; }

    public double FittingWeight { get; init; } = 1;
  }

  public record MasterCurveGroupOptionsWithScalarInterpolation : MasterCurveGroupOptions
  {
    public Altaxo.Calc.Interpolation.IInterpolationFunctionOptions InterpolationFunction { get; init; } = new PolynomialRegressionAsInterpolationOptions { Order = 3 };
  }

  public record MasterCurveGroupOptionsWithComplexInterpolation : MasterCurveGroupOptions
  {
    public Altaxo.Calc.Interpolation.IInterpolationCurveOptions InterpolationFunction { get; init; }
  }

  /// <summary>
  /// Choices for the interpolation function used for master curve interpolation.
  /// </summary>
  public enum MasterCurveGroupOptionsChoice
  {
    /// <summary>Use the same interpolation function for all curve groups.</summary>
    SameForAllGroups,

    /// <summary>For each curve group, use a separate interpolation function.</summary>
    SeparateForEachGroup,

    /// <summary>When there are exactly two groups, use a complex interpolation function.</summary>
    ForComplex,
  }

  /// <summary>
  /// Contains options for master curve creation.
  /// </summary>
  public record MasterCurveCreationOptions : Main.IImmutable
  {
    /// <summary>Index of the reference curve.</summary>
    public int IndexOfReferenceColumnInColumnGroup { get; init; }

    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; }

    public MasterCurveGroupOptionsChoice MasterCurveGroupOptionsChoice { get; init; }

    /// <summary>
    /// Get the options for each group. If there is only one <see cref="MasterCurveGroupOptionsWithScalarInterpolation"/>, and multiple groups, the options are applied
    /// to each of the groups. Otherwise, the number of group options must match the number of groups. If there is one <see cref="MasterCurveGroupOptionsWithComplexInterpolation"/>,
    /// two groups are needed.
    /// </summary>
    public ImmutableList<MasterCurveGroupOptions> GroupOptions { get; init; } = new MasterCurveGroupOptions[] { new MasterCurveGroupOptionsWithScalarInterpolation() }.ToImmutableList();

    protected Func<IReadOnlyList<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr)>, IReadOnlyList<Func<double, double>>> _createInterpolationFunctionCreation = (ds) => throw new NotImplementedException();

    /// <summary>
    /// Gets or sets a function that creates the interpolation function. The input is a set of datapoint collections,
    /// the output is one or multiple interpolation functions (for each datapoint collection, a separate interpolation function).
    /// </summary>
    /// <value>
    /// A function, that inputs sets of curve data, each set consisting of arrays of x, y, and yerr data.
    /// The return value of this function is a set of interpolation functions, one interpolation function for each set of curve data.
    /// </value>
    public Func<IReadOnlyList<(IReadOnlyList<double> X, IReadOnlyList<double> Y, IReadOnlyList<double>? YErr)>, IReadOnlyList<Func<double, double>>> CreateInterpolationFunction
    {
      get { return _createInterpolationFunctionCreation; }
      init { _createInterpolationFunctionCreation = value ?? throw new ArgumentNullException(nameof(value)); }
    }

    /// <summary>
    /// Sets a creator for interpolation functions that creates real values. For each set of curve data, an interpolation
    /// function of the same type will be created.
    /// </summary>
    /// <value>
    /// A function, that inputs the x, y, and yerr values, and creates with that values an interpolation function that interpolates these values.
    /// </value>
    public Func<IReadOnlyList<double>, IReadOnlyList<double>, IReadOnlyList<double>?, Func<double, double>> CreateInterpolationFunctionDDForAllGroups
    {
      init
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        _createInterpolationFunctionCreation = (setOfCurves) => setOfCurves.Select(curveData => value(curveData.X, curveData.Y, curveData.YErr)).ToArray();
      }
    }

    /// <summary>
    /// Sets a creator for an interpolation function that creates complex values.
    /// It is assumed that the master curve is constructed
    /// with exactly two sets of curves, the first set containing the real parts, and the second containing the imaginary parts. Both curve sets
    /// must have exactly the same x-values (which of course is the case if the data originate from complex data).
    /// </summary>
    /// <value>
    /// A function, that inputs the x, y, and yerr values, and creates with that values an interpolation function that interpolates these values. y and yerr are sets of complex values.
    /// </value>
    public Func<IReadOnlyList<double>, IReadOnlyList<Complex64>, IReadOnlyList<Complex64>?, Func<double, Complex64>> CreateInterpolationFunctionDComplex
    {
      init
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        _createInterpolationFunctionCreation = (setOfCurves) =>
        {
          if (setOfCurves.Count != 2)
            throw new InvalidOperationException("Two sets of curve data expected (one for the real and one for the imaginary part).");
          if (setOfCurves[0].X.Count != setOfCurves[1].X.Count)
            throw new InvalidOperationException("In the two sets of curve data expected (one for the real and one for the imaginary part), the same number of points are expected.");

          var YC = new Complex64[setOfCurves[0].X.Count];
          var x0 = setOfCurves[0].X;
          var yre = setOfCurves[0].Y;
          var yim = setOfCurves[1].Y;
          for (int i = 0; i < YC.Length; ++i)
          {
            YC[i] = new Complex64(yre[i], yim[i]);
          }


          var interp = value(x0, YC, null);

          return new Func<double, double>[2] { x => interp(x).Real, x => interp(x).Imaginary };
        };
      }
    }


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

    /// <summary>
    /// Gets/sets the required relative overlap. The default value is 0, which means that a curve part only needs to touch the rest of the master curve.
    /// Setting this to a value, for instance to 0.1, means that a curve part needs an overlapping of 10% (of its x-range) with the rest of the master curve.
    /// This value can also be set to negative values. For instance, setting it to -1 means that a curve part could be in 100% distance (of its x-range) to the rest of the master curve.
    /// </summary>
    /// <value>
    /// The required overlap.
    /// </value>
    public double RequiredRelativeOverlap { get; init; }
  }
}
