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
  /// Class for doing a quick and dirty regression of order 2 only returning the parameters A0, A1 and A2 as regression parameters.
  /// Can not handle too big or too small input values.
  /// </summary>
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
    /// Adds data points to the statistics.
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
    /// Returns the number of entries added.
    /// </summary>
    public double N
    {
      get
      {
        return _n;
      }
    }

    /// <summary>
    /// Gets the intercept value of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The intercept value or NaN if not enough data points are entered.</returns>
    public double GetA0()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (-_sxxx * _sxxx * _sy + _sxx * _sxxxx * _sy + _sxx * _sxxx * _syx - _sx * _sxxxx * _syx - _sxx * _sxx * _syxx + _sx * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the slope value of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The slope value or NaN if not enough data points are entered.</returns>
    public double GetA1()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (_sxx * _sxxx * _sy - _sx * _sxxxx * _sy - _sxx * _sxx * _syx + _n * _sxxxx * _syx + _sx * _sxx * _syxx - _n * _sxxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the quadratic parameter of the regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The slope value or NaN if not enough data points are entered.</returns>
    public double GetA2()
    {
      if (_n < 3)
      {
        throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least 3 needed.");
      }
      return (-_sxx * _sxx * _sy + _sx * _sxxx * _sy + _sx * _sxx * _syx - _n * _sxxx * _syx - _sx * _sx * _syxx + _n * _sxx * _syxx) / GetDeterminant();
    }

    /// <summary>
    /// Returns the determinant of regression. If zero, not enough data points have been entered.
    /// </summary>
    /// <returns>The determinant of the regression.</returns>
    public double GetDeterminant()
    {
      return _n * _sxx * _sxxxx - _sxx * _sxx * _sxx + 2 * _sx * _sxx * _sxxx - _n * _sxxx * _sxxx - _sx * _sx * _sxxxx;
    }

    /// <summary>
    /// Gets the y value for a given x value. Note that in every call of this function the polynomial coefficients a0, a2, and a1 are calculated again.
    /// For repeated calls, better use <see cref="GetYOfXFunction"/>, but note that this function represents the state of the regression at the time of this call.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The y value at the value x.</returns>
    public double GetYOfX(double x)
    {
      return (GetA2() * x + GetA1()) * x + GetA0();
    }

    /// <summary>
    /// Returns a function to calculate y in dependence of x. Please note note that the returned function represents the state of the regression at the time of the call, i.e. subsequent additions of data does not change the function.
    /// </summary>
    /// <returns>A function to calculate y in dependence of x.</returns>
    public Func<double, double> GetYOfXFunction()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      var a2 = GetA2();
      return new Func<double, double>(x => ((a2 * x) + a1) * x + a0);
    }

    /// <summary>
    /// Get the residual sum of squares.
    /// </summary>
    /// <returns>The residual sum of squares, i.e. the sum of squared differences between original y and predicted y.</returns>
    public double SumChiSquared()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      var a2 = GetA2();
      return _syy - a0 * _sy - a1 * _syx - a2 * _syxx;
    }

    /// <summary>
    /// Returns the squared standard deviation of the regression.
    /// </summary>
    /// <returns>The squared standard deviation (variance) of the regression.</returns>
    public double SigmaSquared()
    {
      return _n <= 3 ? double.NaN : SumChiSquared() / (_n - 3);
    }

    /// <summary>
    /// Returns the standard deviation of the regression.
    /// </summary>
    /// <returns>The standard deviation of the regression.</returns>
    public double Sigma()
    {
      var ss = SigmaSquared();
      return ss < 0 ? 0 : Math.Sqrt(SigmaSquared());
    }

    /// <summary>
    /// Gets the R squared value (square of the correlation coefficient).
    /// </summary>
    /// <returns>the R squared value (square of the correlation coefficient).</returns>
    public double RSquared()
    {
      var SST = _syy - _sy * _sy / _n;
      return 1 - SumChiSquared() / SST;
    }

    /// <summary>
    /// Gets the adjusted R squared value.
    /// </summary>
    /// <returns>The adjusted R squared value.</returns>
    public double AdjustedRSquared()
    {
      return _n <= 3 ? double.NaN : 1 - ((1 - RSquared()) * (_n - 1) / (_n - 3));
    }

    /// <summary>
    /// Gets the covariance matrix.
    /// </summary>
    /// <returns>The covariance matrix.</returns>
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
    /// Gets the prediction variance in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix. For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The prediction variance at the value x.</returns>
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
    /// Gets the error of parameter A0.
    /// </summary>
    /// <param name="covarianceMatrix">The covariance matrix. For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The error of parameter A0.</returns>
    public double GetA0Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[0, 0];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the error of parameter A1.
    /// </summary>
    /// <param name="covarianceMatrix">The covariance matrix.For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The error of parameter A1.</returns>
    public double GetA1Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[1, 1];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the error of parameter A2.
    /// </summary>
    /// <param name="covarianceMatrix">The covariance matrix.For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The error of parameter A2.</returns>
    public double GetA2Error(LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[2, 2];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the mean prediction error of y in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix. For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The mean prediction error of y in dependence on x.</returns>
    public double GetYErrorOfX(double x, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      var variance = GetYVarianceOfX(x, covarianceMatrix);
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the confidence band of the prediction.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix. For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <param name="confidenceLevel">The confidence level.</param>
    /// <returns>The lower value of the confidence band, the mean value of the prediction, and the upper value of the confidence band.</returns>
    public (double yLower, double yMean, double yUpper) GetConfidenceBand(double x, double confidenceLevel, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - 3);
      var et = t * GetYErrorOfX(x, covarianceMatrix);
      var y = GetYOfX(x);
      return (y - et, y, y + et);
    }

    /// <summary>
    /// Gets the confidence band of the prediction for multiple x-values.
    /// </summary>
    /// <param name="xdata">The x values.</param>
    /// <param name="confidenceLevel">The confidence level.</param>
    /// <returns>Enumeration with the x values, the lower value of the confidence band, the mean value of the prediction, and the upper value of the confidence band.</returns>
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
