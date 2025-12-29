#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Stores the set of parameters necessary to calculate Savitzky-Golay coefficients.
  /// </summary>
  public record SavitzkyGolayParameters
  {
    /// <summary>
    /// Gets the number of points used for Savitzky-Golay coefficients.
    /// Must be a positive odd number.
    /// </summary>
    public int NumberOfPoints { get; init; } = 7;

    /// <summary>
    /// Gets the polynomial order used to calculate Savitzky-Golay coefficients.
    /// Must be a positive number.
    /// </summary>
    public int PolynomialOrder { get; init; } = 2;

    /// <summary>
    /// Gets the derivative order.
    /// Must be zero or positive. A value of zero is used to smooth a function.
    /// </summary>
    public int DerivativeOrder { get; init; } = 0;
  }

  /// <summary>
  /// Implements the calculation of Savitzky-Golay filter coefficients and their application to smooth data,
  /// and to calculate derivatives.
  /// </summary>
  /// <remarks>Ref.: <c>"Numerical Recipes in C"</c>, chapter 14.8.</remarks>
  public class SavitzkyGolay
  {
    /// <summary>
    /// Gets the central coefficients of the Savitzky-Golay method.
    /// </summary>
    /// <param name="parameters">The parameters of the Savitzky-Golay method.</param>
    /// <returns>The central coefficients.</returns>
    public static double[] GetCentralCoefficients(SavitzkyGolayParameters parameters)
    {
      var result = new double[parameters.NumberOfPoints];
      GetCoefficients(parameters.NumberOfPoints / 2, parameters.NumberOfPoints / 2, parameters.DerivativeOrder, parameters.PolynomialOrder, VectorMath.ToVector(result));
      return result;
    }

    /// <summary>
    /// Calculates Savitzky-Golay coefficients.
    /// </summary>
    /// <param name="leftpoints">Points on the left side included in the regression.</param>
    /// <param name="rightpoints">Points on the right side included in the regression.</param>
    /// <param name="derivativeorder">Order of derivative for which the coefficients are calculated.</param>
    /// <param name="polynomialorder">Order of the regression polynomial.</param>
    /// <param name="coefficients">
    /// Output: on return, contains the calculated coefficients.
    /// The length must be at least <c>leftpoints + rightpoints + 1</c>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if any input argument is invalid (e.g., negative point counts, inconsistent derivative/polynomial order,
    /// or an output vector that is <see langword="null"/> or too short).
    /// </exception>
    public static void GetCoefficients(int leftpoints, int rightpoints, int derivativeorder, int polynomialorder, IVector<double> coefficients)
    {
      int totalpoints = leftpoints + rightpoints + 1;
      // Presumtions leftpoints and rightpoints must be >=0
      if (leftpoints < 0)
        throw new ArgumentException("Argument leftpoints must not be <=0!");
      if (rightpoints < 0)
        throw new ArgumentException("Argument rightpoints must not be <=0!");
      if (totalpoints <= 1)
        throw new ArgumentException("Argument leftpoints and rightpoints must not both be zero!");
      if (polynomialorder >= totalpoints)
        throw new ArgumentException("Argument polynomialorder must not be smaller than total number of points");
      if (derivativeorder > polynomialorder)
        throw new ArgumentException("Argument derivativeorder must not be greater than polynomialorder!");
      if (coefficients is null || coefficients.Count < totalpoints)
        throw new ArgumentException("Vector of coefficients is either null or too short");
      // totalpoints must be greater than 1

      // Set up the design matrix
      // this is the matrix of i^j where i ranges from -leftpoints..rightpoints and j from 0 to polynomialorder
      // as usual for regression, we not use the matrix directly, but instead the covariance matrix At*A
      var mat = CreateMatrix.Dense<double>(polynomialorder + 1, polynomialorder + 1);

      double[] val = new double[totalpoints];
      for (int i = 0; i < totalpoints; i++)
        val[i] = 1;

      for (int ord = 0; ord <= polynomialorder; ord++)
      {
        double sum = val.Sum();
        for (int i = 0; i <= ord; i++)
          mat[ord - i, i] = sum;
        for (int i = 0; i < totalpoints; i++)
          val[i] *= (i - leftpoints);
      }

      for (int ord = polynomialorder - 1; ord >= 0; ord--)
      {
        double sum = val.Sum();
        for (int i = 0; i <= ord; i++)
          mat[polynomialorder - i, polynomialorder - ord + i] = sum;
        for (int i = 0; i < totalpoints; i++)
          val[i] *= (i - leftpoints);
      }

      // now solve the equation
      var decompose = mat.LU();
      // ISingularValueDecomposition decompose = mat.GetSingularValueDecomposition();
      var y = CreateMatrix.Dense<double>(polynomialorder + 1, 1);
      y[derivativeorder, 0] = 1;
      var result = decompose.Solve(y);

      // to get the coefficients, the parameter have to be multiplied by i^j and summed up
      for (int i = -leftpoints; i <= rightpoints; i++)
      {
        double sum = 0;
        double x = 1;
        for (int j = 0; j <= polynomialorder; j++, x *= i)
          sum += result[j, 0] * x;
        coefficients[i + leftpoints] = sum;
      }
    }

    /// <summary>
    /// Filters to apply to the left edge of the array.
    /// </summary>
    private double[][] _left;

    /// <summary>
    /// Filters to apply to the right edge of the array.
    /// Note: the rightmost filter is at index 0.
    /// </summary>
    private double[][] _right;

    /// <summary>
    /// Filter to apply to the middle of the array.
    /// </summary>
    private double[] _middle;

    /// <summary>
    /// Initializes a Savitzky-Golay filter.
    /// </summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number; otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative to obtain. Set to 0 for smoothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Typical values are 2 or 4.</param>
    public SavitzkyGolay(int numberOfPoints, int derivativeOrder, int polynomialOrder)
    {
      numberOfPoints = 1 + 2 * (numberOfPoints / 2);
      int numberOfSide = (numberOfPoints - 1) / 2;

      _left = JaggedArrayMath.GetMatrixArray(numberOfSide, numberOfPoints);
      _right = JaggedArrayMath.GetMatrixArray(numberOfSide, numberOfPoints);
      _middle = new double[numberOfPoints];

      GetCoefficients(numberOfSide, numberOfSide, derivativeOrder, polynomialOrder, VectorMath.ToVector(_middle));

      for (int i = 0; i < numberOfSide; i++)
      {
        GetCoefficients(i, 2 * numberOfSide - i, derivativeOrder, polynomialOrder, VectorMath.ToVector(_left[i]));
        GetCoefficients(2 * numberOfSide - i, i, derivativeOrder, polynomialOrder, VectorMath.ToVector(_right[i]));
      }
    }

    /// <summary>
    /// Initializes a Savitzky-Golay filter.
    /// </summary>
    /// <param name="parameters">Set of parameters used for Savitzky-Golay filtering.</param>
    public SavitzkyGolay(SavitzkyGolayParameters parameters)
      : this(parameters.NumberOfPoints, parameters.DerivativeOrder, parameters.PolynomialOrder)
    {
    }

    /// <summary>
    /// Applies the configured filter to an array of numbers.
    /// The left and right sides are treated specially by applying Savitzky-Golay filters with appropriately adjusted
    /// left and right numbers of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to <paramref name="array"/>.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="array"/> and <paramref name="result"/> are identical, or if the input array is shorter than
    /// the filter length.
    /// </exception>
    public void Apply(double[] array, double[] result)
    {
      int filterPoints = _middle.Length;
      int sidePoints = (filterPoints - 1) / 2;

      if (object.ReferenceEquals(array, result))
        throw new ArgumentException("Argument array and result must not be identical!");

      if (array.Length < filterPoints)
        throw new ArgumentException("Input array must have same or greater length than the filter!");

      // left side
      for (int n = 0; n < sidePoints; n++)
      {
        double[] filter = _left[n];
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[i] * filter[i];
        result[n] = sum;
      }

      // middle
      int middleend = array.Length - filterPoints;
      for (int n = 0; n <= middleend; n++)
      {
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[n + i] * _middle[i];
        result[n + sidePoints] = sum;
      }

      // right side
      int arrayOffset = array.Length - filterPoints;
      int resultOffset = array.Length - 1;
      for (int n = 0; n < sidePoints; n++)
      {
        double[] filter = _right[n];
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[arrayOffset + i] * filter[i];
        result[resultOffset - n] = sum;
      }
    }

    /// <summary>
    /// Applies the configured filter to an array of numbers.
    /// The left and right sides are treated specially by applying Savitzky-Golay filters with appropriately adjusted
    /// left and right numbers of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to <paramref name="array"/>.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="array"/> and <paramref name="result"/> are identical, or if the input array is shorter than
    /// the filter length.
    /// </exception>
    public void Apply(IReadOnlyList<double> array, IVector<double> result)
    {
      int filterPoints = _middle.Length;
      int sidePoints = (filterPoints - 1) / 2;

      if (object.ReferenceEquals(array, result))
        throw new ArgumentException("Argument array and result must not be identical!");

      if (array.Count < filterPoints)
        throw new ArgumentException("Input array must have same or greater length than the filter!");

      // left side
      for (int n = 0; n < sidePoints; n++)
      {
        double[] filter = _left[n];
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[i] * filter[i];
        result[n] = sum;
      }

      // middle
      int middleend = array.Count - filterPoints;
      for (int n = 0; n <= middleend; n++)
      {
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[n + i] * _middle[i];
        result[n + sidePoints] = sum;
      }

      // right side
      int arrayOffset = array.Count - filterPoints;
      int resultOffset = array.Count - 1;
      for (int n = 0; n < sidePoints; n++)
      {
        double[] filter = _right[n];
        double sum = 0;
        for (int i = 0; i < filterPoints; i++)
          sum += array[arrayOffset + i] * filter[i];
        result[resultOffset - n] = sum;
      }
    }
  } // end class SavitzkyGolay
}
