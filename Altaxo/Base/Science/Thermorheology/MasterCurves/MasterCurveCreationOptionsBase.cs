﻿#region Copyright

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
using System.Collections.Immutable;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Contains options for improving the master after it was created.
  /// </summary>
  public abstract record MasterCurveCreationOptionsBase : Main.IImmutable
  {
    /// <summary>
    /// Designates the order with which the curves are shifted to the master curve.
    /// </summary>
    public ShiftOrder.IShiftOrder ShiftOrder { get; init; } = new ShiftOrder.PivotToLastAlternating();

    /// <summary>
    /// Determines the method to best fit the data into the master curve.
    /// </summary>
    public OptimizationMethod OptimizationMethod { get; init; } = OptimizationMethod.OptimizeSquaredDifferenceByBruteForce;

    protected int _numberOfIterations = 40;

    /// <summary>
    /// Gets or sets the number of iterations. Must be greater than or equal to 1.
    /// This number determines how many rounds the master curve is fitted. Increasing this value will in most cases
    /// increase the quality of the fit.
    /// </summary>
    /// <value>
    /// The number of iterations for master curve creation.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">value - Must be a number >= 1</exception>
    public virtual int NumberOfIterations
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

    public MasterCurveGroupOptionsChoice MasterCurveGroupOptionsChoice { get; init; } = MasterCurveGroupOptionsChoice.SameForAllGroups;

    /// <summary>
    /// Get the options for each group. If there is only one <see cref="MasterCurveGroupOptionsWithScalarInterpolation"/>, and multiple groups, the options are applied
    /// to each of the groups. Otherwise, the number of group options must match the number of groups. If there is one <see cref="MasterCurveGroupOptionsWithComplexInterpolation"/>,
    /// two groups are needed.
    /// </summary>
    public ImmutableList<MasterCurveGroupOptions> GroupOptions { get; init; } = new MasterCurveGroupOptions[] { new MasterCurveGroupOptionsWithScalarInterpolation() }.ToImmutableList();
  }
}
