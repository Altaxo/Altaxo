#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Helper class to add regularization terms to linear algebra problems.
  /// Regularization is used to stabilize the solution of ill-posed problems by adding additional constraints that enforce smoothness or other desired properties on the solution.
  /// For this, the parameters of the linear equation system must have the same meaning and order of magnitude.
  /// Regularization is typically applied in least squares problems to prevent overfitting and improve the generalization of the solution.
  /// Examples are Inverse Laplace transformations, deconvolution problems.
  /// </summary>
  public static class Regularization
  {
    /// <summary>
    /// Adds regularization coefficients to a matrix representing a linear equation system matrix*parameters == 0 in order to enforce smoothness on the solution parameters.
    /// </summary>
    /// <param name="matrix">The matrix of the linear equation system <c>matrix*parameter==0</c>, which should be solved in the least squares sense. The RowCount of the matrix has to be <c>numberOfRegularRows + matrix.ColumnCount</c>.</param>
    /// <param name="parameter">The parameter vector of the linear equation system. If not <c>null</c>, this vector will be filled with zeros for the regularization terms. The length of the vector has to be <c>numberOfRegularRows + matrix.ColumnCount</c>.</param>
    /// <param name="numberOfRegularRows">The number of regular rows of the matrix, i.e. the number rows of the linear equation system. The matrix must have more rows to adapt the regularization coefficients, namely <c>numberOfRegularRows + matrix.ColumnCount</c>.</param>
    /// <param name="lambda">The regularization parameter lambda. The higher this parameter is, the smoother the parameters.</param>
    /// <param name="numberOfPoints">The number of points. Must be 1 (regularization order 0), 2 (regularization order 1), or an odd number.</param>
    /// <param name="derivativeOrder">The derivative order, usually, this is set to 2. This parameter is ignored if <paramref name="numberOfPoints"/> is 1 or 2.</param>
    /// <param name="polynomialOrder">The polynomial order, usually this is set to 2.  This parameter is ignored if <paramref name="numberOfPoints"/> is 1 or 2.</param>
    /// <param name="isCircular">Usually, this parameter is false. If set to <c>true</c>, the regularization parameters will be set circular, which means that there is also some smoothness enforced between the last parameter and the first parameter.</param>
    /// <exception cref="System.ArgumentException">Matrix does not have enough rows to add regularization terms. - matrix</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">numberOfPoints - Number of points for regularization must be 1, 2, or an odd integer.</exception>
    public static void AddRegularization(this IMatrix<double> matrix, IVector<double>? parameter, int numberOfRegularRows, double lambda, int numberOfPoints, int derivativeOrder, int polynomialOrder, bool isCircular)
    {
      // ensure that the matrix has enough rows: numberOfRegularRows + numberOfColumns
      if (matrix.RowCount != numberOfRegularRows + matrix.ColumnCount)
        throw new ArgumentException($"Matrix should have {numberOfRegularRows + matrix.ColumnCount} rows to add regularization coefficients, but has {matrix.RowCount} rows.", nameof(matrix));
      if (parameter is not null && parameter.Count != numberOfRegularRows + matrix.ColumnCount)
        throw new ArgumentException($"Parameters should have {numberOfRegularRows + matrix.ColumnCount} elements to add zeros, but has {parameter.Count} elements.", nameof(parameter));
      if (numberOfPoints <= 0 || (numberOfPoints > 3 && numberOfPoints % 2 == 0))
        throw new ArgumentOutOfRangeException(nameof(numberOfPoints), "Number of points for regularization must be 1, 2, or an odd integer.");
      if (!(numberOfPoints > polynomialOrder))
        throw new ArgumentOutOfRangeException(nameof(polynomialOrder), "Polynomial order must be less than number of points.");
      if (!(derivativeOrder <= polynomialOrder))
        throw new ArgumentOutOfRangeException(nameof(derivativeOrder), "Derivative order must be less than or equal to polynomial order.");

      int columnCount = matrix.ColumnCount;

      // at first, extend the parameter vector with zeros for the regularization terms
      if (parameter is not null)
      {
        for (int r = numberOfRegularRows; r < parameter.Count; r++)
        {
          parameter[r] = 0;
        }
      }

      if (numberOfPoints == 1) // zero order regularization, using lambda to create penalty on the parameters
      {
        for (int r = 0; r < columnCount; r++)
        {
          for (int c = 0; c < columnCount; c++)
          {
            matrix[r + numberOfRegularRows, c] = (r == c) ? lambda : 0;
          }
        }
      }
      else if (numberOfPoints == 2) // first order regularization, using +lambda and -lambda to create 1st derivative of the parameters
      {
        var lambdaHalf = lambda / 2;
        if (isCircular)
        {
          // first order regularization: using +lambda and -lambda to create difference
          for (int r = 0; r < columnCount; r++)
          {
            for (int c = 0; c < columnCount; c++)
            {
              var idx = (c - r + 1 + columnCount) % columnCount;
              matrix[r + numberOfRegularRows, c] = idx switch { 0 => -lambdaHalf, 1 => lambdaHalf, _ => 0 };
            }
          }
        }
        else // not circular
        {
          // first order regularization: using -lambda and +lambda to create difference

          // first row
          for (int c = 0; c < columnCount; c++)
          {
            matrix[numberOfRegularRows, c] = c switch { 0 => -lambdaHalf, 1 => lambdaHalf, _ => 0 };
          }
          // other rows
          for (int r = 1; r < columnCount; r++)
          {
            for (int c = 0; c < columnCount; c++)
            {
              matrix[r + numberOfRegularRows, c] = (c + 1 - r) switch { 0 => -lambdaHalf, 1 => lambdaHalf, _ => 0 };
            }
          }
        }
      }
      else
      {
        // higher order regularization: we use Savitzky-Golay style coefficients
        var coefficients = new double[numberOfPoints];
        var halfPoints = (numberOfPoints - 1) / 2;
        if (isCircular)
        {
          // get the Savitzky-Golay coefficients for circular data
          Altaxo.Calc.Regression.SavitzkyGolay.GetCoefficients(halfPoints, halfPoints, derivativeOrder, polynomialOrder, coefficients.ToVector());
          for (int i = 0; i < numberOfPoints; ++i)
          {
            coefficients[i] *= lambda; // premultiply with lambda
          }
          for (int r = 0; r < columnCount; r++)
          {
            for (int c = 0; c < columnCount; c++)
            {
              var idx = (c - r + columnCount + halfPoints) % columnCount;
              matrix[r + numberOfRegularRows, c] = idx >= 0 && idx < numberOfPoints ? coefficients[idx] : 0.0;
            }
          }
        }
        else // not circular
        {
          // left edge
          for (int r = 0; r < halfPoints; r++)
          {
            Altaxo.Calc.Regression.SavitzkyGolay.GetCoefficients(r, numberOfPoints - 1 - r, derivativeOrder, polynomialOrder, coefficients.ToVector());
            for (int c = 0; c < numberOfPoints; c++)
            {
              matrix[r + numberOfRegularRows, c] = coefficients[c] * lambda;
            }
            for (int c = numberOfPoints; c < columnCount; c++)
            {
              matrix[r + numberOfRegularRows, c] = 0;
            }
          }
          // central filling
          {
            Altaxo.Calc.Regression.SavitzkyGolay.GetCoefficients(halfPoints, halfPoints, derivativeOrder, polynomialOrder, coefficients.ToVector());
            for (int i = 0; i < numberOfPoints; ++i)
            {
              coefficients[i] *= lambda;
            }
            for (int r = halfPoints; r < columnCount - halfPoints; r++)
            {
              for (int c = 0; c < columnCount; c++)
              {
                var idx = c - r + halfPoints;
                matrix[r + numberOfRegularRows, c] = (idx >= 0 && idx < numberOfPoints) ? coefficients[idx] : 0;
              }
            }
          }
          // right edge
          for (int i = 0, r = columnCount - halfPoints; r < columnCount; i++, r++)
          {
            Altaxo.Calc.Regression.SavitzkyGolay.GetCoefficients(numberOfPoints - 1 - i, i, derivativeOrder, polynomialOrder, coefficients.ToVector());
            for (int c = 0; c < columnCount - numberOfPoints; c++)
            {
              matrix[r + numberOfRegularRows, c] = 0;
            }
            for (int c = columnCount - numberOfPoints, j = 0; c < columnCount; c++, j++)
            {
              matrix[r + numberOfRegularRows, c] = coefficients[j] * lambda;
            }
          }
        }
      }
    }
  }
}
