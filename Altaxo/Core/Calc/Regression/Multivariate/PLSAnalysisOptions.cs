#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Determines how to do a Partial Least Squares Analysis.
  /// </summary>
  public struct MultivariateAnalysisOptions
  {
    /// <summary>
    /// Get/sets the maximum number of factors to calculate
    /// </summary>
    public int MaxNumberOfFactors;

    /// <summary>
    /// How to do the calculation of Cross PRESS values.
    /// </summary>
    public CrossPRESSCalculationType CrossPRESSCalculation;

    /// <summary>
    /// Get/sets the class that will handles the analysis.
    /// </summary>
    public System.Type AnalysisMethod;
  }
}
