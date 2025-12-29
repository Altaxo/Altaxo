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
using System.Collections.Generic;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Provides a fast, lightweight quadratic (order 2) regression that returns the parameters A0, A1, and A2.
  /// </summary>
  /// <remarks>
  /// This implementation is intended for quick evaluations and uses running sums.
  /// Numerical precision is limited; it may not handle very large or very small input values well.
  /// </remarks>
  public class QuickQuadraticRegression
  {
    private double _n;
    private double _sx;
    private double _sxx;
    private double _sxxx;
    private double _sxxxx;
    private double _sy;
    private double _syx;
    private double _syxx;
    private double _syy;

    /// <summary>
    /// Adds a data point to the regression.
    /// </summary>
    /// <param name="x">The x value of the data point.</param>
    /// <param name="y">The y value of the data point.</param>
    public void Add(double x, double y)
    {
      _n += 1;
      _sx += x;
      _sxx += x * x;
      _sxxx += (x * x) * x;
      _sxxxx += (x * x) * (x * x);
      _sy += y;
      _syy += y * y;
      _syx += y * x;
      _syxx += y * x * x;
    }

    /// <summary>
    /// Adds a sequence of data points to the regression.
    /// </summary>
    /// <param name="values">The data points to add.</param>
    public void AddRange(IEnumerable<(double x, double y)> values)
    {
      foreach (var (x, y) in values)
      {
        Add(x, y);
      }
    }

    /// <summary>
    /// Gets the number of entries added.
    /// </summary>
    public double N
    {
      get
      {
        return _n;
      }
    }

    /// <summary>
    /// Gets the intercept parameter (A0) of the quadratic regression.
    /// </summary>
    /// <returns>The intercept parameter (A0).</returns>
    /// <exception cref="InvalidOperationException">Thrown if fewer than three data points were added.</exception>
    public double GetA0()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (-_sxxx * _sxxx * _sy + _sxx * _sxxxx * _sy + _sxx * _sxxx * _syx - _sx * _sxxxx * _syx - _sxx * _sxx * _syxx + _sx * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the linear parameter (A1) of the quadratic regression.
    /// </summary>
    /// <returns>The linear parameter (A1).</returns>
    /// <exception cref="InvalidOperationException">Thrown if fewer than three data points were added.</exception>
    public double GetA1()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (_sxx * _sxxx * _sy - _sx * _sxxxx * _sy - _sxx * _sxx * _syx + _n * _sxxxx * _syx + _sx * _sxx * _syxx - _n * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the quadratic parameter (A2) of the quadratic regression.
    /// </summary>
    /// <returns>The quadratic parameter (A2).</returns>
    /// <exception cref="InvalidOperationException">Thrown if fewer than three data points were added.</exception>
    public double GetA2()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (-_sxx * _sxx * _sy + _sx * _sxxx * _sy + _sx * _sxx * _syx - _n * _sxxx * _syx - _sx * _sx * _syxx + _n * _sxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the determinant of the normal-equation system.
    /// </summary>
    /// <returns>The determinant of the regression system.</returns>
    public double GetDeterminant()
    {
      return _n * _sxx * _sxxxx - _sxx * _sxx * _sxx + 2 * _sx * _sxx * _sxxx - _n * _sxxx * _sxxx - _sx * _sx * _sxxxx;
    }

    /// <summary>
    /// Evaluates the fitted quadratic at the specified x value.
    /// </summary>
    /// <remarks>
    /// The polynomial coefficients A0, A1, and A2 are recomputed on each call.
    /// For repeated evaluations, consider using <see cref="GetYOfXFunction"/>.
    /// Note that the returned function represents the state of the regression at the time of the call.
    /// </remarks>
    /// <param name="x">The x value.</param>
    /// <returns>The predicted y value at <paramref name="x"/>.</returns>
    public double GetYOfX(double x)
    {
      return (GetA2() * x + GetA1()) * x + GetA0();
    }

    /// <summary>
    /// Creates a function that evaluates the fitted quadratic y(x).
    /// </summary>
    /// <remarks>
    /// The returned function captures the regression parameters at the time this method is called.
    /// Subsequent additions of data points do not change the returned function.
    /// </remarks>
    /// <returns>A function that computes y as a function of x.</returns>
    public Func<double, double> GetYOfXFunction()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      var a2 = GetA2();
      return new Func<double, double>(x => ((a2 * x) + a1) * x + a0);
    }

    /// <summary>
    /// Gets the residual sum of squares.
    /// </summary>
    /// <returns>
    /// The residual sum of squares, i.e. the sum of squared differences between the original y values and the predicted y values.
    /// </returns>
    public double SumChiSquared()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      var a2 = GetA2();
      return _syy - a0 * _sy - a1 * _syx - a2 * _syxx;
    }

    /// <summary>
    /// Gets the estimated variance (squared standard deviation) of the regression.
    /// </summary>
    /// <returns>The estimated variance of the regression.</returns>
    public double SigmaSquared()
    {
      return _n <= 3 ? double.NaN : SumChiSquared() / (_n - 3);
    }

    /// <summary>
    /// Gets the estimated standard deviation of the regression.
    /// </summary>
    /// <returns>The estimated standard deviation of the regression.</returns>
    public double Sigma()
    {
      var ss = SigmaSquared();
      return ss < 0 ? 0 : Math.Sqrt(SigmaSquared());
    }

    /// <summary>
    /// Gets the coefficient of determination (R²).
    /// </summary>
    /// <returns>The coefficient of determination (R²).</returns>
    public double RSquared()
    {
      var SST = _syy - _sy * _sy / _n;
      return 1 - SumChiSquared() / SST;
    }

    /// <summary>
    /// Gets the adjusted coefficient of determination (adjusted R²).
    /// </summary>
    /// <returns>The adjusted coefficient of determination (adjusted R²).</returns>
    public double AdjustedRSquared()
    {
      return _n <= 3 ? double.NaN : 1 - ((1 - RSquared()) * (_n - 1) / (_n - 3));
    }

    /// <summary>
    /// Gets the covariance matrix of the fit parameters.
    /// </summary>
    /// <returns>The covariance matrix of the parameters (A0, A1, A2).</returns>
    public LinearAlgebra.Matrix<double> GetCovarianceMatrix()
    {
      var sq = SigmaSquared();
      var cov = LinearAlgebra.Matrix<double>.Build.Dense(3, 3);

      cov[0, 0] = _n;
      cov[0, 1] = _sx;
      cov[1, 0] = _sx;
      cov[0, 2] = _sxx;
      cov[2, 0] = _sxx;
      cov[1, 1] = _sxx;
      cov[1, 2] = _sxxx;
      cov[2, 1] = _sxxx;
      cov[2, 2] = _sxxxx;
      cov = cov.Inverse();
      cov[0, 0] *= sq;
      cov[0, 1] *= sq;
      cov[1, 0] *= sq;
      cov[0, 2] *= sq;
      cov[2, 0] *= sq;
      cov[1, 1] *= sq;
      cov[1, 2] *= sq;
      cov[2, 1] *= sq;
      cov[2, 2] *= sq;
      return cov;
    }

    /// <summary>
    /// Gets the prediction variance of y at the specified x value.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <returns>The prediction variance at the value <paramref name="x"/>.</returns>
    public double GetYVarianceOfX(double x, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      return
        covarianceMatrix[0, 0] +
        x * (covarianceMatrix[0, 1] + covarianceMatrix[1, 0]) +
        x * x * (covarianceMatrix[0, 2] + covarianceMatrix[2, 0]) +
        x * x * covarianceMatrix[1, 1] +
        x * x * x * (covarianceMatrix[1, 2] + covarianceMatrix[2, 1]) +
        x * x * x * x * covarianceMatrix[2, 2];
    }

    /// <summary>
    /// Gets the standard error of parameter A0.
    /// </summary>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <returns>The standard error of parameter A0.</returns>
    public double GetA0Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[0, 0];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the standard error of parameter A1.
    /// </summary>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <returns>The standard error of parameter A1.</returns>
    public double GetA1Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[1, 1];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the standard error of parameter A2.
    /// </summary>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <returns>The standard error of parameter A2.</returns>
    public double GetA2Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[2, 2];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the mean prediction error of y at the specified x value.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <returns>The mean prediction error of y at <paramref name="x"/>.</returns>
    public double GetYErrorOfX(double x, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      var variance = GetYVarianceOfX(x, covarianceMatrix);
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the confidence band of the prediction at the specified x value.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">
    /// The covariance matrix. For repeated calls, obtain it once via <see cref="GetCovarianceMatrix"/>; otherwise, pass <see langword="null"/>.
    /// </param>
    /// <param name="confidenceLevel">The confidence level (e.g. 0.95 for a 95% confidence band).</param>
    /// <returns>
    /// The lower value of the confidence band, the mean value of the prediction, and the upper value of the confidence band.
    /// </returns>
    public (double yLower, double yMean, double yUpper) GetConfidenceBand(double x, double confidenceLevel, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - 3);
      var et = t * GetYErrorOfX(x, covarianceMatrix);
      var y = GetYOfX(x);
      return (y - et, y, y + et);
    }

    /// <summary>
    /// Gets the confidence band of the prediction for multiple x values.
    /// </summary>
    /// <param name="xdata">The x values.</param>
    /// <param name="confidenceLevel">The confidence level (e.g. 0.95 for a 95% confidence band).</param>
    /// <returns>
    /// An enumeration of tuples containing the x value, the lower confidence bound, the mean prediction, and the upper confidence bound.
    /// </returns>
    public IEnumerable<(double x, double yLower, double yMean, double yUpper)> GetConfidenceBand(IEnumerable<double> xdata, double confidenceLevel)
    {
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - 3);
      var cov = GetCovarianceMatrix();
      foreach (var x in xdata)
      {
        var et = t * GetYErrorOfX(x, cov);
        var y = GetYOfX(x);
        yield return (x, y - et, y, y + et);
      }
    }
  }
}
