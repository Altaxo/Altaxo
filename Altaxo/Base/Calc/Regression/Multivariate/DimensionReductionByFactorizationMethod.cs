#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Base class for dimension reduction analyses based on a factorization, for example principal component analysis (PCA)
  /// or non-negative matrix factorization (NMF).
  /// </summary>
  public abstract record DimensionReductionByFactorizationMethod : IDimensionReductionMethod
  {
    /// <inheritdoc/>
    public abstract IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> processData);

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the maximum number of factors to calculate.
    /// </summary>
    /// <value>
    /// The maximum number of factors.
    /// </value>
    public int MaximumNumberOfFactors { get; init; } = 3;
  }
}
