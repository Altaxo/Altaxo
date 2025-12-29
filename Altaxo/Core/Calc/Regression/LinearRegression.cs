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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Evaluates a function base at a given value of <paramref name="x"/>.
  /// The evaluated base function values are written into <paramref name="functionbase"/>.
  /// </summary>
  /// <param name="x">The value of the independent variable.</param>
  /// <param name="functionbase">The destination array that receives the function base values.</param>
  public delegate void FunctionBaseEvaluator(double x, double[] functionbase);

  /// <summary>
  /// Performs a linear least-squares fit to a given function base using singular value decomposition (SVD).
  /// </summary>
  public class LinearFitBySvd
  {
    private const string ErrorNoExecutionYet = "No results available yet - please execute the fit first!";

    private double[]? _parameter;
    private double _chiSquare;
    private double[][]? _covarianceMatrix;
    private int _numberOfParameter;
    private int _numberOfFreeParameter;
    private int _numberOfData;

    /// <summary>Mean value of y.</summary>
    private double _yMean;

    /// <summary>Sum (yi-ymean)^2.</summary>
    private double _yCorrectedSumOfSquares;

    /// <summary>
    /// Vector of residuals.
    /// </summary>
    private double[]? _residual;

    /// <summary>
    /// Vector of predicted values.
    /// </summary>
    private double[]? _predicted;

    /// <summary>
    /// The reduced variance of prediction at each index. Is calculated from x' (X'X)^(-1) x.
    /// To get the real prediction variance, the values have to be multiplicated with sigma².
    /// </summary>
    private double[]? _reducedPredictionVariance;

    /// <summary>
    /// The singular value composition of our data.
    /// </summary>
    private MatrixMath.SingularValueDecomposition? _decomposition;

    /// <summary>
    /// Fits a data set linearly to a given function base.
    /// </summary>
    /// <param name="xarr">The array of x values of the data set.</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set. Can be <see langword="null"/> if the standard deviations are unknown.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit (equals the size of the function base).</param>
    /// <param name="evaluateFunctionBase">The function base used for fitting.</param>
    /// <param name="threshold">A threshold value (usually 1E-5) used to discard unimportant singular values.</param>
    public LinearFitBySvd(
        double[] xarr,
        double[] yarr,
        double[]? stddev,
        int numberOfData,
        int numberOfParameter,
        FunctionBaseEvaluator evaluateFunctionBase,
        double threshold)
    {
      var u = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfData, numberOfParameter);

      double[] functionBase = new double[numberOfParameter];

      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for (int i = 0; i < numberOfData; i++)
      {
        evaluateFunctionBase(xarr[i], functionBase);
        for (int j = 0; j < numberOfParameter; j++)
          u[i, j] = functionBase[j];
      }

      Calculate(
          u,
          VectorMath.ToROVector(yarr),
          VectorMath.ToROVector(stddev),
          numberOfData,
          numberOfParameter,
          threshold);
    }

    /// <summary>
    /// Fits a data set linearly to a given function base.
    /// </summary>
    /// <param name="xarr">The array of x values of the data set.</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set. Can be <see langword="null"/> if the standard deviations are unknown.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit (equals the size of the function base).</param>
    /// <param name="evaluateFunctionBase">The function base used for fitting.</param>
    /// <param name="threshold">A threshold value (usually 1E-5) used to discard unimportant singular values.</param>
    public LinearFitBySvd(
        IReadOnlyList<double> xarr,
        IReadOnlyList<double> yarr,
        IReadOnlyList<double>? stddev,
        int numberOfData,
        int numberOfParameter,
        FunctionBaseEvaluator evaluateFunctionBase,
        double threshold)
    {
      var u = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfData, numberOfParameter);

      double[] functionBase = new double[numberOfParameter];

      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for (int i = 0; i < numberOfData; i++)
      {
        evaluateFunctionBase(xarr[i], functionBase);
        for (int j = 0; j < numberOfParameter; j++)
          u[i, j] = functionBase[j];
      }

      Calculate(
          u,
          yarr,
          stddev,
          numberOfData,
          numberOfParameter,
          threshold);
    }

    /// <summary>
    /// Fits a data set linearly to a given design matrix.
    /// </summary>
    /// <param name="xbase">The matrix of x base values of the data set. Dimensions: numberOfData x numberOfParameters. The matrix is changed during the calculation.</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set. Can be <see langword="null"/> if the standard deviations are unknown.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit (equals the number of columns of <paramref name="xbase"/>).</param>
    /// <param name="threshold">A threshold value (usually 1E-5) used to discard unimportant singular values.</param>
    public LinearFitBySvd(
        IROMatrix<double> xbase, // NumberOfData, NumberOfParameters
        double[] yarr,
        double[]? stddev,
        int numberOfData,
        int numberOfParameter,
        double threshold)
    {
      Calculate(xbase, VectorMath.ToROVector(yarr), stddev is null ? null : VectorMath.ToROVector(stddev), numberOfData, numberOfParameter, threshold);
    }

    /// <summary>
    /// Fits a data set linearly to a given design matrix.
    /// </summary>
    /// <param name="xbase">The matrix of x base values of the data set. Dimensions: numberOfData x numberOfParameters. The matrix is changed during the calculation!</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set. Can be null if the standard deviation is unkown.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit == size of the function base.</param>
    /// <param name="threshold">A treshold value (usually 1E-5) used to chop the unimportant singular values away.</param>
    public LinearFitBySvd Calculate(
        IROMatrix<double> xbase, // NumberOfData, NumberOfParameters
        IReadOnlyList<double> yarr,
        IReadOnlyList<double>? stddev,
        int numberOfData,
        int numberOfParameter,
        double threshold)
    {
      _numberOfParameter = numberOfParameter;
      _numberOfFreeParameter = numberOfParameter;
      _numberOfData = numberOfData;
      _parameter = new double[numberOfParameter];
      _residual = new double[numberOfData];
      _predicted = new double[numberOfData];
      _reducedPredictionVariance = new double[numberOfData];

      double[] scaledY = new double[numberOfData];

      // Calculated some useful values
      _yMean = Mean(yarr, 0, _numberOfData);
      _yCorrectedSumOfSquares = CorrectedSumOfSquares(yarr, _yMean, 0, _numberOfData);

      var u = new MatrixMath.LeftSpineJaggedArrayMatrix<double>(numberOfData, numberOfParameter);
      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      if (stddev is null)
      {
        for (int i = 0; i < numberOfData; i++)
        {
          for (int j = 0; j < numberOfParameter; j++)
            u[i, j] = xbase[i, j];

          scaledY[i] = yarr[i];
        }
      }
      else
      {
        for (int i = 0; i < numberOfData; i++)
        {
          double scale = 1 / stddev[i];

          for (int j = 0; j < numberOfParameter; j++)
            u[i, j] = scale * xbase[i, j];

          scaledY[i] = scale * yarr[i];
        }
      }
      _decomposition = MatrixMath.GetSingularValueDecomposition(u);

      // set singular values < thresholdLevel to zero
      // ChopSingularValues makes only sense if all columns of the x matrix have the same variance
      //decomposition.ChopSingularValues(1E-5);
      // recalculate the parameters with the chopped singular values
      _decomposition.Backsubstitution(scaledY, _parameter);

      _chiSquare = 0;
      for (int i = 0; i < numberOfData; i++)
      {
        double ypredicted = 0;
        for (int j = 0; j < numberOfParameter; j++)
          ypredicted += _parameter[j] * xbase[i, j];
        double deviation = yarr[i] - ypredicted;
        _predicted[i] = ypredicted;
        _residual[i] = deviation;
        _chiSquare += deviation * deviation;
      }

      _covarianceMatrix = _decomposition.GetCovariances();

      //calculate the reduced prediction variance x'(X'X)^(-1)x
      for (int i = 0; i < numberOfData; i++)
      {
        double total = 0;
        for (int j = 0; j < numberOfParameter; j++)
        {
          double sum = 0;
          for (int k = 0; k < numberOfParameter; k++)
            sum += _covarianceMatrix[j][k] * u[i, k];

          total += u[i, j] * sum;
        }
        _reducedPredictionVariance[i] = total;
      }

      return this;
    }

    /// <summary>
    /// Calculates the mean value of <paramref name="length"/> elements in <paramref name="x"/>, starting at index <paramref name="start"/>.
    /// </summary>
    /// <param name="x">The sequence of values.</param>
    /// <param name="start">The index of the first element.</param>
    /// <param name="length">The number of elements used for the calculation.</param>
    /// <returns>The mean value.</returns>
    public static double Mean(IReadOnlyList<double> x, int start, int length)
    {
      double sum = 0;
      int end = start + length;
      for (int i = start; i < end; i++)
        sum += x[i];

      return sum / length;
    }

    /// <summary>
    /// Calculates the corrected sum of squares of <paramref name="length"/> elements in <paramref name="x"/>, starting at index <paramref name="start"/>.
    /// </summary>
    /// <param name="x">The sequence of values.</param>
    /// <param name="mean">The mean value of the sequence.</param>
    /// <param name="start">The starting index.</param>
    /// <param name="length">The number of elements used for the calculation.</param>
    /// <returns>The corrected sum of squares (sum of squared deviations from <paramref name="mean"/>).</returns>
    public static double CorrectedSumOfSquares(IReadOnlyList<double> x, double mean, int start, int length)
    {
      int end = start + length;
      double sum = 0;
      double r;
      for (int i = start; i < end; i++)
      {
        r = x[i] - mean;
        sum += r * r;
      }
      return sum;
    }

    /// <summary>
    /// Gets the number of parameters of the fit.
    /// </summary>
    public int NumberOfParameter { get { return _numberOfParameter; } }

    /// <summary>
    /// Gets the number of data points used by the fit.
    /// </summary>
    public int NumberOfData { get { return _numberOfData; } }

    /// <summary>
    /// Gets the fitted parameters such that <c>y = Σ(parameter[i] * functionBase[i])</c>.
    /// </summary>
    public double[] Parameter { get { return _parameter ?? throw new InvalidOperationException(ErrorNoExecutionYet); } }

    /// <summary>
    /// Gets the residual sum of squares (RSS) of the fit, i.e. <c>Σ(yi - ŷi)^2</c>.
    /// </summary>
    public double ResidualSumOfSquares { get { return _chiSquare; } }

    /// <summary>
    /// Gets the regression corrected sum of squares, i.e. <c>Σ(ŷi - ȳ)^2</c>.
    /// </summary>
    public double RegressionCorrectedSumOfSquares { get { return _yCorrectedSumOfSquares - _chiSquare; } }

    /// <summary>
    /// Gets the total corrected sum of squares of <c>y</c>, i.e. <c>Σ(yi - ȳ)^2</c>.
    /// </summary>
    public double TotalCorrectedSumOfSquares { get { return _yCorrectedSumOfSquares; } }

    /// <summary>
    /// Gets the coefficient of determination (R²).
    /// </summary>
    public double RSquared { get { return 1 - _chiSquare / _yCorrectedSumOfSquares; } }

    /// <summary>Gives the adjusted coefficient of determination.</summary>
    /// <remarks>Ref. "Introduction to linear regression analysis", Wiley, p.90.</remarks>
    public double AdjustedRSquared
    {
      get
      {
        if (_numberOfFreeParameter >= _numberOfData)
          return double.NaN;
        else
          return 1 - (_chiSquare * (_numberOfData - 1)) / (_yCorrectedSumOfSquares * (_numberOfData - _numberOfFreeParameter));
      }
    }

    /// <summary>
    /// Gets the condition number.
    /// </summary>
    /// <remarks>
    /// The decadic logarithm of the condition number roughly estimates the loss of precision (in digits) during the calculation.
    /// </remarks>
    public double ConditionNumber
    {
      get
      {
        return _decomposition?.Condition ?? throw new InvalidOperationException(ErrorNoExecutionYet);
      }
    }

    /// <summary>
    /// Gets the estimated standard error of parameter <paramref name="i"/>.
    /// </summary>
    /// <param name="i">The index of the parameter.</param>
    /// <returns>The estimated standard error of parameter <paramref name="i"/>.</returns>
    public double StandardErrorOfParameter(int i)
    {
      var covarianceMatrix = _covarianceMatrix ?? throw new InvalidOperationException(ErrorNoExecutionYet);
      return Math.Sqrt(EstimatedVariance * covarianceMatrix[i][i]);
    }

    /// <summary>
    /// Gets the absolute t-statistic of parameter <paramref name="i"/>.
    /// </summary>
    /// <param name="i">The index of the parameter.</param>
    /// <returns>The absolute t-statistic of parameter <paramref name="i"/>.</returns>
    public double TofParameter(int i)
    {
      return Math.Abs(Parameter[i]) / StandardErrorOfParameter(i);
    }

    /// <summary>
    /// Gets the residual values defined as <c>y[i] - ŷ[i]</c>.
    /// </summary>
    public double[] ResidualValues
    {
      get { return _residual ?? throw new InvalidOperationException(ErrorNoExecutionYet); }
    }

    /// <summary>
    /// Gets the predicted dependent values <c>ŷ[i]</c>.
    /// </summary>
    public double[] PredictedValues
    {
      get { return _predicted ?? throw new InvalidOperationException(ErrorNoExecutionYet); }
    }

    /// <summary>
    /// Returns the i-th PRESS residual.
    /// </summary>
    /// <param name="i">The index of the PRESS residual.</param>
    /// <returns>The i-th PRESS residual.</returns>
    /// <remarks>
    /// The PRESS residual is the prediction error of the i-th value, if the i-th value itself is not used in the prediction model.
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.135</para>
    /// </remarks>
    public double PRESSResidual(int i)
    {
      if (_residual is null || _decomposition is null)
        throw new InvalidOperationException(ErrorNoExecutionYet);

      return _residual[i] / (1 - _decomposition.HatDiagonal[i]);
    }

    /// <summary>
    /// Returns the i-th studentized residual.
    /// </summary>
    /// <param name="i">The index of the residual.</param>
    /// <returns>The i-th studentized residual.</returns>
    /// <remarks>
    /// The studentized residual has constant variance of 1, regardless of the location of xi.
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.134</para>
    /// </remarks>
    public double StudentizedResidual(int i)
    {
      if (_residual is null || _decomposition is null)
        throw new InvalidOperationException(ErrorNoExecutionYet);

      return _residual[i] / Math.Sqrt((1 - _decomposition.HatDiagonal[i]) * EstimatedVariance);
    }

    /// <summary>
    /// Returns the i-th studentized residual, with the i-th observation removed from the model.
    /// </summary>
    /// <param name="i">The index of the residual.</param>
    /// <returns>The i-th externally studentized residual.</returns>
    /// <remarks>
    /// As with the studentized residual, the expected variance of this residual is 1. Since the i-th observation is excluded from the model,
    /// the externally studentized residual is better suited for outlier detection than the (normal) studentized residual.
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.136</para>
    /// </remarks>
    public double ExternallyStudentizedResidual(int i)
    {
      if (_residual is null || _decomposition is null)
        throw new InvalidOperationException(ErrorNoExecutionYet);

      double ssi = _chiSquare - square(_residual[i]) / (1 - _decomposition.HatDiagonal[i]);
      ssi /= (_numberOfData - _numberOfFreeParameter - 1);

      return _residual[i] / Math.Sqrt(ssi * (1 - _decomposition.HatDiagonal[i]));
    }

    /// <summary>
    /// Gets the variance-covariance matrix of the fitted parameters.
    /// </summary>
    public double[][] Covariances { get { return _covarianceMatrix ?? throw new InvalidOperationException(ErrorNoExecutionYet); } }

    /// <summary>
    /// Gets the estimated residual mean square (also called sigma square).
    /// </summary>
    /// <remarks>
    /// The estimated mean square is defined as <c>RSS / (n - p)</c>, where <c>n</c> is the number of data points and <c>p</c> is the number of (free) parameters.
    /// </remarks>
    public double EstimatedVariance
    {
      get
      {
        if (_numberOfData > _numberOfFreeParameter)
          return _chiSquare / (_numberOfData - _numberOfFreeParameter);
        else
          return 0;
      }
    }

    /// <summary>
    /// Returns the variance of the prediction of the i-th y-value.
    /// </summary>
    /// <param name="i">The index of the i-th observation.</param>
    /// <returns>The variance of the i-th predicted value.</returns>
    public double PredictionVariance(int i)
    {
      if (_reducedPredictionVariance is null)
        throw new InvalidOperationException(ErrorNoExecutionYet);

      return EstimatedVariance * _reducedPredictionVariance[i];
    }

    /// <summary>
    /// Gets the standard error of regression.
    /// </summary>
    /// <remarks>
    /// This value is currently computed as <c>Sqrt(RSS)</c>.
    /// </remarks>
    public double Sigma { get { return Math.Sqrt(ResidualSumOfSquares); } }

    #region Helper

    /// <summary>
    /// Returns <paramref name="x"/> squared.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns><paramref name="x"/> squared.</returns>
    private double square(double x)
    {
      return x * x;
    }

    #endregion Helper

    #region Default function bases

    /// <summary>
    /// Polynomial function base factory.
    /// </summary>
    private class PolynomialFunction
    {
      private int _order;

      /// <summary>
      /// Initializes a new instance of the <see cref="PolynomialFunction"/> class.
      /// </summary>
      /// <param name="order">The polynomial order.</param>
      public PolynomialFunction(int order)
      {
        _order = order;
      }

      /// <summary>
      /// Evaluates the polynomial function base for the specified <paramref name="x"/>.
      /// </summary>
      /// <param name="x">The value of the independent variable.</param>
      /// <param name="result">The destination array that receives the function base values.</param>
      public void Evaluate(double x, double[] result)
      {
        double sum = 1;
        for (int i = 0; i <= _order; i++)
        {
          result[i] = sum;
          sum *= x;
        }
      }
    }

    /// <summary>
    /// Gets a default polynomial function base with intercept, i.e. <c>y = a + b*x + c*x*x + ...</c>.
    /// </summary>
    /// <param name="order">The order of the polynomial (0: intercept only, 1: linear, 2: quadratic, ...).</param>
    /// <returns>The function base to use with this fit.</returns>
    public static FunctionBaseEvaluator GetPolynomialFunctionBase(int order)
    {
      if (order < 0)
        throw new ArgumentOutOfRangeException("Order must be > 0");

      return new PolynomialFunction(order).Evaluate;
    }

    /// <summary>
    /// Fits data provided as x and y sequences with a polynomial base.
    /// </summary>
    /// <remarks>
    /// Special measures are taken (scaling of the x-variable) in order to keep precision high.
    /// </remarks>
    /// <param name="order">The order of the fit (1: linear, 2: quadratic, etc.).</param>
    /// <param name="xValues">The sequence of x-values. Only those values are used that are not NaN.</param>
    /// <param name="yValues">The sequence of y-values.</param>
    /// <param name="start">Index of the first data point to use.</param>
    /// <param name="count">Number of data points to use.</param>
    /// <param name="doRemoveNaNValues">If <see langword="true"/>, value pairs containing NaN are removed before calculation of the fit.</param>
    /// <returns>The fitted model.</returns>
    public static LinearFitBySvd FitPolymomial(int order, IReadOnlyList<double> xValues, IReadOnlyList<double> yValues, int start, int count, bool doRemoveNaNValues)
    {
      if (xValues is null)
        throw new ArgumentNullException(nameof(xValues));
      if (yValues is null)
        throw new ArgumentNullException(nameof(yValues));
      if (!(start >= 0))
        throw new ArgumentOutOfRangeException(nameof(count), "must be >=0");
      if (!(count > 0))
        throw new ArgumentOutOfRangeException(nameof(count), "must be >0");
      int end = start + count;
      if (!(end <= xValues.Count))
        throw new ArgumentOutOfRangeException(nameof(count), "exceeds capacity of array " + nameof(xValues));
      if (!(end <= yValues.Count))
        throw new ArgumentOutOfRangeException(nameof(count), "exceeds capacity of array " + nameof(yValues));

      double[] xarr = new double[count];
      double[] yarr = new double[count];
      double[] earr = new double[count];

      int numberOfDataPoints = 0;
      if (doRemoveNaNValues)
      {
        for (int i = start; i < end; ++i)
        {
          double x = xValues[i];
          double y = yValues[i];
          if (double.IsNaN(x) || double.IsNaN(y))
            continue;

          xarr[numberOfDataPoints] = x;
          yarr[numberOfDataPoints] = y;
          earr[numberOfDataPoints] = 1;
          numberOfDataPoints++;
        }
      }
      else
      {
        for (int i = start; i < end; ++i)
        {
          xarr[numberOfDataPoints] = xValues[i];
          yarr[numberOfDataPoints] = yValues[i];
          earr[numberOfDataPoints] = 1;
          numberOfDataPoints++;
        }
      }

      return FitPolymomialDestructive(order, xarr, yarr, earr, numberOfDataPoints);
    }

    /// <summary>
    /// Fits data provided as x and y arrays with a polynomial base.
    /// </summary>
    /// <remarks>
    /// Special measures are taken (scaling of the x-variable) in order to keep precision high.
    /// </remarks>
    /// <param name="order">The order of the fit (1: linear, 2: quadratic, etc.).</param>
    /// <param name="xValues">The array of x-values. The values of the array are modified during evaluation.</param>
    /// <param name="yValues">The array of y-values.</param>
    /// <param name="errorValues">The array of error values. If <see langword="null"/>, error values are set to 1 for each element.</param>
    /// <param name="count">Number of values to use (<c>array[0]</c> ... <c>array[count-1]</c>).</param>
    /// <returns>The fitted model.</returns>
    public static LinearFitBySvd FitPolymomialDestructive(int order, double[] xValues, double[] yValues, double[]? errorValues, int count)
    {
      if (xValues is null)
        throw new ArgumentNullException(nameof(xValues));
      if (yValues is null)
        throw new ArgumentNullException(nameof(yValues));
      if (!(count > 0))
        throw new ArgumentOutOfRangeException(nameof(count), "must be >0");
      if (!(count <= xValues.Length))
        throw new ArgumentOutOfRangeException(nameof(count), "exceeds capacity of array " + nameof(xValues));
      if (!(count <= yValues.Length))
        throw new ArgumentOutOfRangeException(nameof(count), "exceeds capacity of array " + nameof(yValues));
      if (errorValues is not null && !(count <= errorValues.Length))
        throw new ArgumentOutOfRangeException(nameof(count), "exceeds capacity of array " + nameof(errorValues));

      var xarr = xValues;
      var yarr = yValues;
      var earr = errorValues;

      if (earr is null)
      {
        earr = new double[count];
        VectorMath.FillWith(earr, 1);
      }

      int numberOfDataPoints = count;

      // we scale the x-values in order to keep the Condition number reasonable

      var xmin = Altaxo.Calc.LinearAlgebra.VectorMath.Min(xarr, 0, numberOfDataPoints);
      var xmax = Altaxo.Calc.LinearAlgebra.VectorMath.Max(xarr, 0, numberOfDataPoints);

      double xscale = Math.Max(-xmin, xmax);
      double xinvscale = 1 / xscale;

      if (0 == xscale)
      {
        xscale = xinvscale = 1;
      }

      for (int i = 0; i < numberOfDataPoints; ++i)
        xarr[i] *= xinvscale;

      var fit =
          new LinearFitBySvd(
          xarr, yarr, earr, numberOfDataPoints, order + 1, new FunctionBaseEvaluator(GetPolynomialFunctionBase(order)), 1E-15);

      // rescale parameter of fit in order to account for rescaled x variable
      for (int i = 0; i <= order; ++i)
      {
        fit._parameter![i] *= RMath.Pow(xinvscale, i);
        for (int j = 0; j <= order; ++j)
          fit._covarianceMatrix![i][j] *= RMath.Pow(xinvscale, i + j);
      }

      return fit;
    }

    #endregion Default function bases
  }
}
