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
using System.Collections.Generic;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Class for doing a quick and dirty regression of order 1 only returning intercept and slope.
  /// Can not handle too big or too input values.
  /// </summary>
  public class QuickLinearRegression
  {
    private double _n;
    private double _sx;
    private double _sxx;
    private double _sy;
    private double _syy;
    private double _syx;

    /// <summary>
    /// Gets an invalid regression.
    /// </summary>
    public static QuickLinearRegression Invalid => new QuickLinearRegression() { _n = double.NaN };

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
      _sy += y;
      _syy += y * y;
      _syx += y * x;
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
    /// Gets the intercept value of the linear regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The intercept value or NaN if not enough data points are entered.</returns>
    public double GetA0()
    {
      return (_sy * _sxx - _syx * _sx) / GetDeterminant();
    }

    /// <summary>
    /// Gets the slope value of the linear regression. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The slope value or NaN if not enough data points are entered.</returns>
    public double GetA1()
    {
      return (_n * _syx - _sx * _sy) / GetDeterminant();
    }

    /// <summary>
    /// Gets the intercept of the linear regression with the X-axis. Returns NaN if not enough data points entered.
    /// </summary>
    /// <returns>The intercept value with the X-Axis, i.e. the point where the regression value is zero. Returns NaN if not enough data points are entered.</returns>
    public double GetX0()
    {
      return -(_sy * _sxx - _syx * _sx) / (_n * _syx - _sx * _sy);
    }

    /// <summary>
    /// Gets the y value for a given x value. Note that in every call of this function the coefficients a0 and a1 are calculated again.
    /// For repeated calls, better use <see cref="GetYOfXFunction"/>, but note that this function represents the state of the regression at the time of this call.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The y value at the value x.</returns>
    public double GetYOfX(double x)
    {
      var a0 = GetA0();
      var a1 = GetA1();
      return a0 + a1 * x;
    }

    /// <summary>
    /// Returns a function to calculate y in dependence of x. Please note note that the returned function represents the state of the regression at the time of the call, i.e. subsequent additions of data does not change the function.
    /// </summary>
    /// <returns>A function to calculate y in dependence of x.</returns>
    public Func<double, double> GetYOfXFunction()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      return new Func<double, double>(x => a0 + a1 * x);
    }

    /// <summary>
    /// Returns the determinant of regression. If zero, not enough data points have been entered.
    /// </summary>
    /// <returns>The determinant of the regression.</returns>
    public double GetDeterminant()
    {
      return _n * _sxx - _sx * _sx;
    }

    /// <summary>
    /// Returns true if this regression spans a valid line, i.e. both coefficients a0 and a1 are finite.
    /// </summary>
    /// <value>
    /// <c>true</c> if this regression is valid; otherwise, <c>false</c>.
    /// </value>
    public bool IsValid
    {
      get
      {
        return _n >= 2 && RMath.IsFinite(GetA0()) && RMath.IsFinite(GetA1());
      }
    }

    public double MeanX
    {
      get
      {
        return _sx / _n;
      }
    }

    public double MeanY
    {
      get
      {
        return _sy / _n;
      }
    }

    public double PearsonCorrelationCoefficient
    {
      get
      {
        return (_n * _syx - _sx * _sy) / Math.Sqrt((_n * _sxx - _sx * _sx) * (_n * _syy - _sy * _sy));
      }
    }

    /// <summary>
    /// Returns the squared (Pearson) correlation coefficient R².
    /// </summary>
    /// <returns>The squared correlation coefficient R²</returns>
    public double RSquared()
    {
      var nom = _n * _syx - _sx * _sy;
      var denom = (_n * _sxx - _sx * _sx) * (_n * _syy - _sy * _sy);
      return (nom * nom) / denom;
    }

    public double AdjustedRSquared()
    {
      var r2 = RSquared();
      return 1 - (1 - r2) * (_n - 1) / (_n - 2);
    }

    public double ChiSquared()
    {
      var a0 = GetA0();
      var a1 = GetA1();
      return _syy + a0 * a0 * _n + a1 * a1 * _sxx + 2 * a0 * a1 * _sx - 2 * a0 * _sy - 2 * a1 * _syx;
    }

    public LinearAlgebra.Matrix<double> GetCovarianceMatrix()
    {
      var sigma2 = SigmaSquared();
      var det = GetDeterminant();
      var varA0 = sigma2 * _sxx / det;
      var varA1 = sigma2 * _n / det;
      var covA0A1 = -sigma2 * _sx / det;
      var matrix = LinearAlgebra.Matrix<double>.Build.Dense(2, 2);
      matrix[0, 0] = varA0;
      matrix[1, 1] = varA1;
      matrix[0, 1] = covA0A1;
      matrix[1, 0] = covA0A1;
      return matrix;
    }

    /// <summary>
    /// Gets the prediction variance in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix.  Get it from <see cref="GetCovarianceMatrix"/>.</param>
    /// <returns>The prediction variance at the value x.</returns>
    public double GetYVarianceOfX(double x, LinearAlgebra.Matrix<double> covarianceMatrix)
    {
      return covarianceMatrix[0, 0] + x * covarianceMatrix[0, 1] + x * covarianceMatrix[1, 0] + x * x * covarianceMatrix[1, 1];
    }

    /// <summary>
    /// Gets the mean prediction error of y in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix. Get it from <see cref="GetCovarianceMatrix"/>.</param>
    /// <returns>The mean prediction error of y in dependence on x.</returns>
    public double GetYErrorOfX(double x, LinearAlgebra.Matrix<double> covarianceMatrix)
    {
      var variance = covarianceMatrix[0, 0] + x * covarianceMatrix[0, 1] + x * covarianceMatrix[1, 0] + x * x * covarianceMatrix[1, 1];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the confidence band of the prediction.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="covarianceMatrix">The covariance matrix. Get it from <see cref="GetCovarianceMatrix"/>.</param>
    /// <param name="confidenceLevel">The confidence level.</param>
    /// <returns>The lower value of the confidence band, the mean value of the prediction, and the upper value of the confidence band.</returns>
    public (double yLower, double yMean, double yUpper) GetConfidenceBand(double x, LinearAlgebra.Matrix<double> covarianceMatrix, double confidenceLevel)
    {
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - 2);
      var et = t * GetYErrorOfX(x, covarianceMatrix);
      var y = GetYOfX(x);
      return (y - et, y, y + et);
    }

    /// <summary>
    /// Returns the squared standard deviation of the regression.
    /// </summary>
    /// <returns>The squared standard deviation (variance) of the regression.</returns>
    public double SigmaSquared()
    {
      var a = GetA0();
      var b = GetA1();
      //      return (_syy + a * a * _n + b * b * _sxx + 2 * a * b * _sx - 2 * a * _sy - 2 * b * _syx) / (_n - 2);
      return (_syy - a * _sy - b * _syx) / (_n - 2);
    }

    /// <summary>
    /// Returns the standard deviation of the regression.
    /// </summary>
    /// <returns>The standard deviation of the regression.</returns>
    public double Sigma()
    {
      var ss = SigmaSquared();
      return ss < 0 ? 0 : Math.Sqrt(ss);
    }

    /// <summary>
    /// Gets the intersection point of two linear regressions
    /// </summary>
    /// <param name="reg2">The reg2.</param>
    /// <returns></returns>
    public (double x, double y) GetIntersectionPoint(QuickLinearRegression reg2)
    {
      var x = (GetA0() - reg2.GetA0()) / (reg2.GetA1() - GetA1());
      var y = 0.5 * (this.GetYOfX(x) + reg2.GetYOfX(x)); // we use the average, although both regression should give the same result
      return (x, y);
    }

    /// <summary>
    /// Gets the relative distance of a point (x,y) between two regression lines.
    /// </summary>
    /// <param name="a">The first regression line.</param>
    /// <param name="b">The other regression line.</param>
    /// <param name="x">The x value of the point.</param>
    /// <param name="y">The y value of the point.</param>
    /// <returns>The relative y value. If the point (x,y) is located on the regression line a, then 0 is returned.
    /// If the point (x,y) is located on the regression line b, then 1 is returned. Likewise, if the point is located between
    /// the two regression lines, then a value inbetween 0 and 1 is returned.
    /// The value is not clamped to the interval [0, 1]. Thus if the point is outside the two regression lines,
    /// then a value less than 0 or greater than 1 could be returned.
    /// </returns>
    public static double GetRelativeYBetweenRegressions(QuickLinearRegression a, QuickLinearRegression b, double x, double y)
    {
      var y0 = a.GetYOfX(x);
      var y1 = b.GetYOfX(x);
      return (y - y0) / (y1 - y0);
    }




  }
}
