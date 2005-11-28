#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{

  /// <summary>
  /// Evaluates a function base of the variable x. The function base is returned in
  /// array <c>functionbase</c>.
  /// </summary>
  public delegate void FunctionBaseEvaluator(double x, double[] functionbase);


 

  /// <summary>
  /// Performs a linear fit to a given function base using singular value decomposition.
  /// </summary>
  public class LinearFitBySvd
  {
    double[] _parameter;
    double  _chiSquare;
    double[][] _covarianceMatrix;
    int _numberOfParameter;
    int _numberOfFreeParameter;
    int _numberOfData;
    /// <summary>Mean value of y.</summary>
    double _yMean;
    /// <summary>Sum (yi-ymean)^2.</summary>
    double _yCorrectedSumOfSquares;

    /// <summary>
    /// Vector of residuals.
    /// </summary>
    double[] _residual;

    /// <summary>
    /// Vector of predicted values.
    /// </summary>
    double[] _predicted;

    /// <summary>
    /// The reduced variance of prediction at each index. Is calculated from x' (X'X)^(-1) x.
    /// To get the real prediction variance, the values have to be multiplicated with sigma².
    /// </summary>
    double[] _reducedPredictionVariance;

    /// <summary>
    /// The singular value composition of our data.
    /// </summary>
    MatrixMath.SingularValueDecomposition _decomposition;

    /// <summary>
    /// Fits a data set linear to a given function base.
    /// </summary>
    /// <param name="xarr">The array of x values of the data set.</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit == size of the function base.</param>
    /// <param name="evaluateFunctionBase">The function base used to fit.</param>
    /// <param name="threshold">A treshold value (usually 1E-5) used to chop the unimportant singular values away.</param>
    public LinearFitBySvd(
      double[] xarr, 
      double[] yarr,
      double[] stddev,
      int numberOfData,
      int numberOfParameter,
      FunctionBaseEvaluator evaluateFunctionBase,
      double threshold)
    {
      IMatrix u = new MatrixMath.BEMatrix( numberOfData, numberOfParameter);

      double[] functionBase = new double[numberOfParameter];

      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for(int i=0;i<numberOfData;i++)
      {
        evaluateFunctionBase(xarr[i], functionBase);
        for(int j=0;j<numberOfParameter;j++)
          u[i,j] = functionBase[j];
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
    /// Fits a data set linear to a given x base.
    /// </summary>
    /// <param name="xbase">The matrix of x values of the data set. Dimensions: numberOfData x numberOfParameters. The matrix is changed during calculation!</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit == size of the function base.</param>
    /// <param name="threshold">A treshold value (usually 1E-5) used to chop the unimportant singular values away.</param>
    public LinearFitBySvd(
      IROMatrix xbase, // NumberOfData, NumberOfParameters 
      double[] yarr,
      double[] stddev,
      int numberOfData,
      int numberOfParameter,
      double threshold)
    {
      Calculate(xbase,yarr,stddev,numberOfData,numberOfParameter,threshold);
    }

    /// <summary>
    /// Fits a data set linear to a given x base.
    /// </summary>
    /// <param name="xbase">The matrix of x values of the data set. Dimensions: numberOfData x numberOfParameters. The matrix is changed during calculation!</param>
    /// <param name="yarr">The array of y values of the data set.</param>
    /// <param name="stddev">The array of y standard deviations of the data set.</param>
    /// <param name="numberOfData">The number of data points (may be smaller than the array sizes of the data arrays).</param>
    /// <param name="numberOfParameter">The number of parameters to fit == size of the function base.</param>
    /// <param name="threshold">A treshold value (usually 1E-5) used to chop the unimportant singular values away.</param>
    public LinearFitBySvd Calculate(
      IROMatrix xbase, // NumberOfData, NumberOfParameters 
      double[] yarr,
      double[] stddev,
      int numberOfData,
      int numberOfParameter,
      double threshold)
    {
      _numberOfParameter = numberOfParameter;
      _numberOfFreeParameter = numberOfParameter;
      _numberOfData      = numberOfData;
      _parameter = new double[numberOfParameter];
      _residual = new double[numberOfData];
      _predicted = new double[numberOfData];
      _reducedPredictionVariance = new double[numberOfData];

      double[] scaledY      = new double[numberOfData];
      
      // Calculated some useful values
      _yMean = Mean(yarr,0,_numberOfData);
      _yCorrectedSumOfSquares = CorrectedSumOfSquares(yarr,_yMean,0,_numberOfData);

      MatrixMath.BEMatrix u = new MatrixMath.BEMatrix(numberOfData,numberOfParameter);
      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for(int i=0;i<numberOfData;i++)
      {
        double scale = 1/stddev[i];

        for(int j=0;j<numberOfParameter;j++)
          u[i,j] = scale*xbase[i,j];
        
        scaledY[i] = scale*yarr[i];
      }

      _decomposition = MatrixMath.GetSingularValueDecomposition(u);

      // set singular values < thresholdLevel to zero
      // ChopSingularValues makes only sense if all columns of the x matrix have the same variance
      //decomposition.ChopSingularValues(1E-5);
      // recalculate the parameters with the chopped singular values
      _decomposition.Backsubstitution(scaledY,_parameter);

      _chiSquare = 0;
      for(int i=0;i<numberOfData;i++)
      {
        double ypredicted=0;
        for(int j=0;j<numberOfParameter;j++)
          ypredicted += _parameter[j]*xbase[i,j];
        double deviation = yarr[i]-ypredicted;
        _predicted[i] = ypredicted;
        _residual[i] = deviation;
        _chiSquare += deviation*deviation;
      }
    
      _covarianceMatrix = _decomposition.GetCovariances();


      //calculate the reduced prediction variance x'(X'X)^(-1)x
      for(int i=0;i<numberOfData;i++)
      {
        double total = 0;
        for(int j=0;j<numberOfParameter;j++)
        {
          double sum=0;
          for(int k=0;k<numberOfParameter;k++)
            sum += _covarianceMatrix[j][k]*u[i,k];

          total += u[i,j]*sum;
        }
        _reducedPredictionVariance[i] = total;
      }
 
      return this;
    }

    /// <summary>
    /// Calculates the mean value of <c>length</c> elements in array x starting from index <c>start</c>.
    /// </summary>
    /// <param name="x">The array of values.</param>
    /// <param name="start">First element.</param>
    /// <param name="length">Number of elements used for calculation.</param>
    /// <returns></returns>
    public static double Mean(double[] x, int start, int length)
    {
      double sum=0;
      int end = start+length;
      for(int i=start;i<end;i++)
        sum += x[i];

      return sum/length;
    }

    /// <summary>
    /// Calculates the corrected sum of squares of <c>length</c> elements of array x starting from index <c>start</c>. The corrected sum
    /// of squares is defined as sum of squares of the elements minus their mean value.
    /// </summary>
    /// <param name="x">Array of values.</param>
    /// <param name="mean">Mean value of the values.</param>
    /// <param name="start">Starting index.</param>
    /// <param name="length">Number of elements used for calculation.</param>
    /// <returns></returns>
    public static double CorrectedSumOfSquares(double[] x, double mean, int start, int length)
    {
      int end = start + length;
      double sum=0;
      double r;
      for(int i=start;i<end;i++)
      {
        r=x[i]-mean;
        sum+=r*r;
      }
      return sum;
    }

    /// <summary>
    /// Returns the number of parameter (=Order+1) of the fit.
    /// </summary>
    public int NumberOfParameter { get { return _numberOfParameter; }}

    /// <summary>
    /// Returns the number of data value.
    /// </summary>
    public int NumberOfData { get { return _numberOfData; }}

    /// <summary>
    /// Get the resulting parameters, so that the model y = SUM(parameter[i]*functionbase[i])
    /// </summary>
    public double[] Parameter { get { return _parameter; }}
    /// <summary>
    /// Gets the sum of ChiSquare for the fit. This is SUM(yi-yi`)^2, where yi is the ith y value and yi` is the ith predicted y.
    /// </summary>
    public double   ResidualSumOfSquares { get { return _chiSquare;}}

    /// <summary>
    /// Gets the regression sum of squares, i.e. SUM(yi`-ymean), where yi` is the predicted ith y value and y mean is the mean value of all y values.
    /// </summary>
    public double   RegressionCorrectedSumOfSquares { get { return _yCorrectedSumOfSquares - _chiSquare; }}

    /// <summary>
    /// Gives the corrected sum of squares of y, i.e. SUM(yi-ymean), where yi is the ith y value and ymean is the mean of all y values.
    /// </summary>
    public double TotalCorrectedSumOfSquares { get { return this._yCorrectedSumOfSquares; }}
  

    /// <summary>
    /// Gives the coefficient of determination, also called R^2, squared correlation coefficient. It is a measure, how  much
    /// of the variability of the y data is accounted for by the regression model.
    /// </summary>
    public double RSquared { get { return 1 - _chiSquare/_yCorrectedSumOfSquares; }}
  
    /// <summary>Gives the adjusted coefficient of determination.</summary>
    /// <remarks>Ref. "Introduction to linear regression analysis", Wiley, p.90.</remarks>
    public double AdjustedRSquared 
    {
      get
      {
        if(_numberOfFreeParameter>=_numberOfData)
          return double.NaN;
        else
          return 1 - (_chiSquare*(_numberOfData-1))/(_yCorrectedSumOfSquares*(_numberOfData-_numberOfFreeParameter)); 
      }
    }

    /// <summary>
    /// Gets the estimated standard error of parameter <c>i</c>.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>The estimated standard error of parameter <c>i</c>.</returns>
    public double StandardErrorOfParameter(int i)
    {
      return Math.Sqrt(EstimatedVariance*_covarianceMatrix[i][i]); 
    }
    

    public double TofParameter(int i)
    {
      return Math.Abs(Parameter[i])/StandardErrorOfParameter(i);
    }

    /// <summary>
    /// Gets the array of residual values defined as the difference y[i]-ypredicted[i].
    /// </summary>
    public double[] ResidualValues
    {
      get { return _residual; }
    }

    /// <summary>
    /// Gets the predicted dependent values
    /// </summary>
    public double[] PredictedValues
    {
      get { return _predicted; }
    }


    /// <summary>
    /// Gives the ith PRESS residual.
    /// </summary>
    /// <param name="i">The index of the PRESS residual.</param>
    /// <returns>The ith PRESS residual.</returns>
    /// <remarks>The PRESS residual is the prediction error of the ith value, if the ith value itself
    /// is not used in the prediction model.
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.135</para></remarks>
    
    public double PRESSResidual(int i)
    {
      return _residual[i]/(1-_decomposition.HatDiagonal[i]);
    }


    /// <summary>
    /// Gives the ith studentized residual.
    /// </summary>
    /// <param name="i">The index of the residual.</param>
    /// <returns>The ith studentized residual.</returns>
    /// <remarks>The studentized residual has constant variance of 1, regardless of the location of xi. 
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.134</para></remarks>
    public double StudentizedResidual(int i)
    {
      return _residual[i]/Math.Sqrt((1-_decomposition.HatDiagonal[i])*this.EstimatedVariance);
    }

    /// <summary>
    /// Gives the ith studentized residual, with the ith observation removed from the model.
    /// </summary>
    /// <param name="i">The index to the residual.</param>
    /// <returns>The ith externally studentized residual.</returns>
    /// <remarks>
    /// As with the studentized residual, the expected variance of this residual is 1. Since the ith
    /// observation is excluded from the model, the externally studentized residual is better suited
    /// for outlier detection than the (normal) studentized residual.
    /// <para>Ref: Introduction to linear regression analysis, 3rd ed., Wiley, p.136</para>
    /// </remarks>
    public double ExternallyStudentizedResidual(int i)
    {
      double ssi = this._chiSquare - square(_residual[i])/(1-_decomposition.HatDiagonal[i]);
      ssi /= (this._numberOfData - this._numberOfFreeParameter - 1);

      return _residual[i]/Math.Sqrt(ssi*(1-_decomposition.HatDiagonal[i]));
    }


    /// <summary>
    /// Get the variance-covariance-matrix for the fit.
    /// </summary>
    public double[][] Covariances { get { return _covarianceMatrix; }}


    /// <summary>Get the estimated residual mean square, also called SigmaSquare..</summary>
    /// <remarks>The estimated mean square is defined as SumChiSquare(n-p), where n is the number of data
    /// points and p is the number of (free) parameters.</remarks>
    public double EstimatedVariance
    {
      get
      {
        if(_numberOfData>_numberOfFreeParameter)
          return this._chiSquare/(_numberOfData-_numberOfFreeParameter);
        else
          return 0;
      }
    }

    /// <summary>
    /// Gives the variance of the prediction of the ith y-value.
    /// </summary>
    /// <param name="i">The index to the ith observation.</param>
    /// <returns>The variance of the ith prediction value.</returns>
    public double PredictionVariance(int i)
    {
      return this.EstimatedVariance*this._reducedPredictionVariance[i];
    }



    /// <summary>Get the standard error of regression, defined as <c>Sqrt(SigmaSquare)</c>.</summary>
    public double Sigma  {  get  { return Math.Sqrt(ResidualSumOfSquares); }}

    #region Helper
    double square(double x) { return x*x; }
    #endregion

  }
}
