#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Interface to a solver for linear equations. The procedure can either be non-destructive (keeping the matrix and the vector b),
  /// or destructive (not keeping matrix m and vector b). The destructive solving process may be faster, since
  /// saving of the matrix and the vector is not required.
  /// </summary>
  /// <typeparam name="T">Type of scalar arithmetic value.</typeparam>
  public interface ILinearEquationSolver<T> where T : struct
  {
    /// <summary>
    /// Solves the equation A*x == b. The matrix m and vector b will be kept. If this is not neccessary,
    /// then use <see cref="SolveDestructive(IMatrix{T}, IVector{T}, IVector{T})"/>, because it might be slightly faster.
    /// </summary>
    /// <param name="A">The matrix.</param>
    /// <param name="b">The right hand vector.</param>
    /// <param name="x">The solution x of the equation A*x==b.</param>
    void Solve(IROMatrix<T> A, IReadOnlyList<T> b, IVector<T> x);

    /// <summary>
    /// Solves the equation A*x == b. The matrix A and right hand vector b will be kept. If this is not neccessary,
    /// then use <see cref="SolveDestructive{VectorT}(IMatrix{double}, IVector{double}, Func{int, VectorT})"/>, because it might be slightly faster.
    /// </summary>
    /// <typeparam name="VectorT"></typeparam>
    /// <param name="A">The matrix.</param>
    /// <param name="b">The right hand vector.</param>
    /// <param name="vectorCreation">Function to create a new vector. Argument is the length of the vector.</param>
    /// <returns>The solution x of the equation A*x==b.</returns>
    VectorT Solve<VectorT>(IROMatrix<double> A, IReadOnlyList<double> b, Func<int, VectorT> vectorCreation) where VectorT : IVector<double>;

    /// <summary>
    /// Solves the equation A*x == b. The matrix A and the right hand vector b might be changed in the process. If this is unwanted,
    /// then better use <see cref="Solve(IROMatrix{T}, IReadOnlyList{T}, IVector{T})"/>.
    /// </summary>
    /// <param name="A">The m.</param>
    /// <param name="b">The b.</param>
    /// <param name="x">The result.</param>
    void SolveDestructive(IMatrix<T> A, IVector<T> b, IVector<T> x);

    /// <summary>
    /// Solves the equation A*x == b. The matrix A and the right hand vector b might be changed in the process. If this is unwanted,
    /// then better use <see cref="Solve{VectorT}(IROMatrix{double}, IReadOnlyList{double}, Func{int, VectorT})"/>.
    /// </summary>
    /// <typeparam name="VectorT"></typeparam>
    /// <param name="A">The matrix. It will be modified during this call!</param>
    /// <param name="b">The right hand vector. Will be modified during this call!</param>
    /// <param name="vectorCreation">Function to create a new vector. Argument is the length of the vector.</param>
    /// <returns>The solution x of the equation A*x==b.</returns>
    VectorT SolveDestructive<VectorT>(IMatrix<double> A, IVector<double> b, Func<int, VectorT> vectorCreation) where VectorT : IVector<double>;
  }
}
