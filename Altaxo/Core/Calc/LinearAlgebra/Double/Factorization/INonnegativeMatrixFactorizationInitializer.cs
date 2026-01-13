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

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  /// <summary>
  /// Provides initialization factors for non-negative matrix factorization (NMF).
  /// </summary>
  public interface INonnegativeMatrixFactorizationInitializer
  {
    /// <summary>
    /// Gets initial non-negative factor matrices for an NMF of the input matrix <paramref name="X"/>.
    /// </summary>
    /// <param name="X">The input matrix to factorize.</param>
    /// <param name="rank">The target rank of the factorization.</param>
    /// <returns>
    /// A tuple of factor matrices <c>(W, H)</c> such that <c>X ≈ W * H</c>.
    /// </returns>
    (Matrix<double> W, Matrix<double> H) GetInitialFactors(Matrix<double> X, int rank);
  }
}
