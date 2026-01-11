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
  /// Base interface for dimension reduction analyses, for example principal component analysis (PCA), non-negative matrix factorization (NMF), etc.
  /// </summary>
  public interface IDimensionReductionMethod : Main.IImmutable
  {
    /// <summary>
    /// Executes the dimension reduction on the provided process data.
    /// </summary>
    /// <param name="processData">The preprocessed data matrix to be analyzed. Is is assumed that each row of the matrix represents a spectrum.</param>
    /// <returns>The dimension reduction result.</returns>
    IDimensionReductionResult ExecuteDimensionReduction(IROMatrix<double> processData);

    /// <summary>
    /// Gets a user friendly name for the Gui.
    /// </summary>
    string DisplayName { get; }
  }
}
