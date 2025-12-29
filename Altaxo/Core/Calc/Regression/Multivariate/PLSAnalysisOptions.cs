#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Specifies options for running a multivariate analysis (e.g. Partial Least Squares).
  /// </summary>
  public struct MultivariateAnalysisOptions
  {
    /// <summary>
    /// The maximum number of factors to calculate.
    /// </summary>
    public int MaxNumberOfFactors;

    /// <summary>
    /// Strategy that determines how cross-validation groups are formed (used for calculating cross-PRESS values).
    /// </summary>
    public ICrossValidationGroupingStrategy CrossValidationGroupingStrategy;

    /// <summary>
    /// Gets or sets the analysis method type that performs the analysis.
    /// </summary>
    /// <remarks>
    /// The specified type is expected to identify the analysis implementation used by the caller.
    /// </remarks>
    public System.Type AnalysisMethod;
  }
}
