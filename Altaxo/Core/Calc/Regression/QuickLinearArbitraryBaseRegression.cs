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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Class for doing a quick and dirty linear regression with arbitrary base functions provided by the user.
  /// Numerical precision is limited, as it keeps only the XtX matrix, but therefore it is very fast and needs only little memory.
  /// Can not handle too big differences between the base functions.
  /// </summary>
  public class QuickLinearArbitraryBaseRegression
  {
    int _numberOfFreeBaseFunctions;
    int _numberOfFixedBaseFunctions;
    double _n; // number of points added
    Matrix<double> _xTx; // the XTX matrix, during addition of points we only update the upper half.
    Vector<double> _xTy; // the sum y*x vector
    double _sy; // sum of y
    double _syy; // sum of y*y

    /// <summary>Indices of the free parameters. The index into this array is the internal index (for _xTx, _xTy), whereas the value of
    /// each element is the external index (parameter index, base function index, covariance matrix).</summary>
    int[] _freeBaseIndices;

    /// <summary>Indices of the fixed parameters.
    /// The index into this array is the internal index (for _fixedParameters), whereas the value of each element is the external index (parameter index, base function index, covariance matrix).</summary>
    int[] _fixedBaseIndices;

    /// <summary>Temporary storage space for the evaluated base function values.</summary>
    Vector<double> _base;

    /// <summary>Storage of the results (parameters, covariance matrix, inverse of XtX).</summary>
    Results? _results;

    /// <summary>Calculates the base functions at a given x and stores the result in the provided vector. First argument is the x value. The second element is
    /// the vector that will receive the free base functions. The return value is the sum of y values of the fixed parameters multiplied with their respective base functions.</summary>
    private Action<double, Vector<double>> _calculateBase;

    /// <summary>
    /// The fixed parameters. The arrays length is the total number of parameters (fixed + free).
    /// Each element has either a value of null (free parameter, needs to be evaluated), or a double value (fixed parameter).
    /// </summary>
    double[] _fixedParameters;

    private class Results
    {
      public Vector<double>? _parameters;
      public Matrix<double>? _inverseXtX;
      public Matrix<double>? _covarianceMatrix;
    }

    /// <summary> 
    /// Initializes a new instance of the <see cref="QuickLinearArbitraryBaseRegression"/> class.
    /// </summary>
    /// <param name="baseFunctions">The base functions.</param>
    public QuickLinearArbitraryBaseRegression(params Func<double, double>[] baseFunctions)
    {

      _freeBaseIndices = Enumerable.Range(0, baseFunctions.Length).ToArray();
      _numberOfFreeBaseFunctions = _freeBaseIndices.Length;

      _fixedBaseIndices = Array.Empty<int>();
      _numberOfFixedBaseFunctions = _fixedBaseIndices.Length;
      _fixedParameters = Array.Empty<double>();

      _xTx = Matrix<double>.Build.Dense(_numberOfFreeBaseFunctions, _numberOfFreeBaseFunctions);
      _xTy = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
      _base = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);

      var clonedBaseFunctions = (Func<double, double>[])baseFunctions.Clone();
      _calculateBase = (x, vec) =>
      {
        for (int i = 0; i < clonedBaseFunctions.Length; i++)
        {
          vec[i] = clonedBaseFunctions[i](x);
        }
      };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuickLinearArbitraryBaseRegression"/> class.
    /// </summary>
    /// <param name="calculateBase">An action that calculates the base functions. First argument is the x value. The second argument is a vector that will receive the base function values.</param>
    /// <param name="numberOfBaseFunctions">The number of base functions.</param>
    public QuickLinearArbitraryBaseRegression(Action<double, Vector<double>> calculateBase, int numberOfBaseFunctions)
    {
      _freeBaseIndices = Enumerable.Range(0, numberOfBaseFunctions).ToArray();
      _numberOfFreeBaseFunctions = _freeBaseIndices.Length;
      _fixedBaseIndices = Array.Empty<int>();
      _numberOfFixedBaseFunctions = _fixedBaseIndices.Length;
      _fixedParameters = Array.Empty<double>();

      _xTx = Matrix<double>.Build.Dense(_numberOfFreeBaseFunctions, _numberOfFreeBaseFunctions);
      _xTy = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
      _base = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
      _calculateBase = calculateBase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuickLinearArbitraryBaseRegression"/> class.
    /// Here, some parameters can be fixed to given values.
    /// </summary>
    /// <param name="baseFunctions">The base functions.</param>
    /// <param name="parametersFixed">Array of fixed parameters. Must have the same length than <paramref name="baseFunctions"/>. For each element that has a value, the
    /// parameter is considered fixed, and will not participate in the regression evaluation.</param>
    public QuickLinearArbitraryBaseRegression(Func<double, double>[] baseFunctions, double?[] parametersFixed)
    {
      if (baseFunctions.Length != parametersFixed.Length)
      {
        throw new ArgumentException("Length of baseFunctions and parametersFixed must be the same.");
      }

      _freeBaseIndices = parametersFixed.Select((p, idx) => (p, idx)).Where(t => !t.p.HasValue).Select(t => t.idx).ToArray();
      _numberOfFreeBaseFunctions = _freeBaseIndices.Length;

      _fixedBaseIndices = parametersFixed.Select((p, idx) => (p, idx)).Where(t => t.p.HasValue).Select(t => t.idx).ToArray();
      _numberOfFixedBaseFunctions = _fixedBaseIndices.Length;
      _fixedParameters = parametersFixed.Where(p => p.HasValue).Select(p => p!.Value).ToArray();

      _xTx = Matrix<double>.Build.Dense(_numberOfFreeBaseFunctions, _numberOfFreeBaseFunctions);
      _xTy = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
      _base = Vector<double>.Build.Dense(parametersFixed.Length);

      var baseFunctionsClone = (Func<double, double>[])baseFunctions.Clone();
      _calculateBase = (x, vec) =>
      {
        for (int i = 0; i < baseFunctionsClone.Length; i++)
          vec[i] = baseFunctionsClone[i](x);
      };
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="QuickLinearArbitraryBaseRegression"/> class.
    /// Here, some parameters can be fixed to given values (null means not fixed).
    /// </summary>
    /// <param name="calculateBase">An action that calculates the base functions. First argument is the x value. The second argument is a vector that will receive the base function values.</param>
    /// <param name="parametersFixed">Array of fixed parameters. For each element that has a value, the
    /// parameter is considered fixed, and will not participate in the regression evaluation. The length of this array is the number of base functions.</param>
    public QuickLinearArbitraryBaseRegression(Action<double, Vector<double>> calculateBase, double?[] parametersFixed)
    {
      _freeBaseIndices = parametersFixed.Select((p, idx) => (p, idx)).Where(t => !t.p.HasValue).Select(t => t.idx).ToArray();
      _numberOfFreeBaseFunctions = _freeBaseIndices.Length;

      _fixedBaseIndices = parametersFixed.Select((p, idx) => (p, idx)).Where(t => t.p.HasValue).Select(t => t.idx).ToArray();
      _numberOfFixedBaseFunctions = _fixedBaseIndices.Length;
      _fixedParameters = parametersFixed.Where(p => p.HasValue).Select(p => p!.Value).ToArray();

      _xTx = Matrix<double>.Build.Dense(_numberOfFreeBaseFunctions, _numberOfFreeBaseFunctions);
      _xTy = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
      _base = Vector<double>.Build.Dense(parametersFixed.Length);

      var fullVec = Vector<double>.Build.Dense(parametersFixed.Length);
      _calculateBase = calculateBase;
    }

    /// <summary>
    /// Adds a data point to the regression.
    /// </summary>
    /// <param name="x">The x value of the data point.</param>
    /// <param name="y">The y value of the data point.</param>
    public void Add(double x, double y)
    {
      _calculateBase(x, _base);

      double yFixed = 0;
      for (int i = 0; i < _fixedBaseIndices.Length; i++)
      {
        yFixed += _fixedParameters[i] * _base[_fixedBaseIndices[i]];
      }
      y -= yFixed;

      for (int i = 0; i < _freeBaseIndices.Length; i++)
      {
        _xTy[i] += y * _base[_freeBaseIndices[i]];
      }

      for (int r = 0; r < _freeBaseIndices.Length; r++)
      {
        int rr = _freeBaseIndices[r];
        for (int c = r; c < _freeBaseIndices.Length; ++c)
        {
          var cc = _freeBaseIndices[c];
          _xTx[r, c] += _base[rr] * _base[cc];
          _xTx[c, r] = _xTx[r, c];
        }
      }
      _n += 1;
      _sy += y;
      _syy += y * y;
      _results = null;
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
    /// Gets the parameters of the regression. Returns an array with NaNs if not enough data points entered.
    /// </summary>
    /// <returns>The intercept value or NaN if not enough data points are entered.</returns>
    public IReadOnlyList<double> GetParameters()
    {
      if (_results?._parameters is not null)
      {
        return _results._parameters;
      }
      else
      {
        if (_n < _numberOfFreeBaseFunctions)
        {
          throw new InvalidOperationException($"Insufficient number of points for quadratic regression: actual {_n}, but at least {_numberOfFreeBaseFunctions} needed.");
        }

        _results ??= new Results();
        var freeParameters = Vector<double>.Build.Dense(_numberOfFreeBaseFunctions);
        _xTx.Solve(_xTy, freeParameters);

        if (_numberOfFixedBaseFunctions == 0)
        {
          _results._parameters = freeParameters;
        }
        else
        {

          var allParameters = Vector<double>.Build.Dense(_base.Count);
          for (int i = 0; i < _fixedBaseIndices.Length; ++i)
          {
            allParameters[_fixedBaseIndices[i]] = _fixedParameters[i];
          }
          for (int i = 0; i < _freeBaseIndices.Length; ++i)
          {
            allParameters[_freeBaseIndices[i]] = freeParameters[i];
          }
          _results._parameters = allParameters;
        }
        return _results._parameters;
      }
    }

    /// <summary>
    /// Gets the parameter with index idxParameter of the regression.
    /// </summary>
    /// <param name="idxParameter">The index of the parameter.</param>
    /// <returns>The value of the parameter with index idxParameter of the regression.</returns>
    public double GetParameter(int idxParameter)
    {
      var parameters = GetParameters();
      return parameters[idxParameter];
    }


    /// <summary>
    /// Gets the y value for a given x value. Note that in every call of this function the polynomial coefficients a0, a2, and a1 are calculated again.
    /// For repeated calls, better use <see cref="GetYOfXFunction"/>, but note that this function represents the state of the regression at the time of this call.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The y value at the value x.</returns>
    public double GetYOfX(double x)
    {
      var parameters = GetParameters();
      _calculateBase(x, _base);

      double sum = 0;
      for (int i = 0, j = 0; i < parameters.Count; ++i)
      {
        sum += parameters[i] * _base[i];
      }
      return sum;
    }

    /// <summary>
    /// Returns a function to calculate y in dependence of x. Please note note that the returned function represents the state of the regression at the time of the call, i.e. subsequent additions of data does not change the function.
    /// </summary>
    /// <returns>A function to calculate y in dependence of x.</returns>
    public Func<double, double> GetYOfXFunction()
    {
      var parameters = GetParameters().ToArray();
      {
        var len = _numberOfFreeBaseFunctions;
        return new Func<double, double>(x =>
        {
          var baseF = Vector<double>.Build.Dense(parameters.Length);
          double sum = 0;
          for (int i = 0; i < parameters.Length; ++i)
          {
            sum += parameters[i] * _base[i];
          }
          return sum;
        }
        );
      }
    }

    /// <summary>
    /// Gets the înverse of the Xt.X matrix.
    /// </summary>
    /// <returns>Inverse of the Xt.X matrix.</returns>
    public Matrix<double> GetÎnverseXtXMatrix()
    {
      if (_results?._inverseXtX is null)
      {
        _results ??= new Results();
        _results._inverseXtX = _xTx.Inverse();
      }
      return _results._inverseXtX!;
    }

    /// <summary>
    /// Get the residual sum of squares.
    /// </summary>
    /// <returns>The residual sum of squares, i.e. the sum of squared differences between original y and predicted y.</returns>
    public double SumChiSquared()
    {
      var inv = GetÎnverseXtXMatrix();
      return _syy - _xTy.DotProduct(inv.Multiply(_xTy));
    }

    /// <summary>
    /// Returns the squared standard deviation of the regression.
    /// </summary>
    /// <returns>The squared standard deviation (variance) of the regression.</returns>
    public double SigmaSquared()
    {
      return _n <= _numberOfFreeBaseFunctions ? double.NaN : SumChiSquared() / (_n - _numberOfFreeBaseFunctions);
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
      return _n <= _numberOfFreeBaseFunctions ? double.NaN : 1 - ((1 - RSquared()) * (_n - 1) / (_n - _numberOfFreeBaseFunctions));
    }

    /// <summary>
    /// Gets the covariance matrix.
    /// </summary>
    /// <returns>The covariance matrix.</returns>
    public LinearAlgebra.Matrix<double> GetCovarianceMatrix()
    {
      if (_results?._covarianceMatrix is null)
      {
        _results ??= new Results();
        var sq = SigmaSquared();
        var covarianceMatrix = _xTx.Inverse().Multiply(sq);
        if (_numberOfFixedBaseFunctions == 0)
        {
          _results._covarianceMatrix = covarianceMatrix;
        }
        else
        {
          var allCovarianceMatrix = Matrix<double>.Build.Dense(_base.Count, _base.Count);
          for (int r = 0; r < covarianceMatrix.RowCount; ++r)
          {
            var rr = _freeBaseIndices[r];
            for (int c = 0; c < covarianceMatrix.ColumnCount; c++)
            {
              var cc = _freeBaseIndices[c];
              allCovarianceMatrix[rr, cc] = covarianceMatrix[r, c];
            }
          }
          _results._covarianceMatrix = allCovarianceMatrix;
        }
      }
      return _results._covarianceMatrix!;
    }

    /// <summary>
    /// Gets the prediction variance in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The prediction variance at the value x.</returns>
    public double GetYVarianceOfX(double x)
    {
      var covarianceMatrix = GetCovarianceMatrix();
      _calculateBase(x, _base);
      return _base.DotProduct(covarianceMatrix.Multiply(_base));
    }

    /// <summary>
    /// Gets the error of parameter A0.
    /// </summary>
    /// <param name="covarianceMatrix">The covariance matrix. For repeated calls, get it in from <see cref="GetCovarianceMatrix"/>, otherwise, you can provide null.</param>
    /// <returns>The error of parameter A0.</returns>
    public double GetParameterError(int idxParameter, LinearAlgebra.Matrix<double>? covarianceMatrix = null)
    {
      covarianceMatrix ??= GetCovarianceMatrix();
      var variance = covarianceMatrix[idxParameter, idxParameter];
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }


    /// <summary>
    /// Gets the mean prediction error of y in dependence on x.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <returns>The mean prediction error of y in dependence on x.</returns>
    public double GetYErrorOfX(double x)
    {
      var variance = GetYVarianceOfX(x);
      return variance < 0 ? 0 : Math.Sqrt(variance);
    }

    /// <summary>
    /// Gets the confidence band of the prediction.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="confidenceLevel">The confidence level.</param>
    /// <returns>The lower value of the confidence band, the mean value of the prediction, and the upper value of the confidence band.</returns>
    public (double yLower, double yMean, double yUpper) GetConfidenceBand(double x, double confidenceLevel)
    {
      var covarianceMatrix = GetCovarianceMatrix();
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - _numberOfFreeBaseFunctions);
      var et = t * GetYErrorOfX(x);
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
      var t = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - confidenceLevel)), _n - _numberOfFreeBaseFunctions);
      foreach (var x in xdata)
      {
        var et = t * GetYErrorOfX(x);
        var y = GetYOfX(x);
        yield return (x, y - et, y, y + et);
      }
    }
  }
}
