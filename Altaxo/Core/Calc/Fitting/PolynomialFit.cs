#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Fitting
{

  /// <summary>
  /// Evaluates a function base of the variable x.
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
      _numberOfParameter = numberOfParameter;
      _numberOfFreeParameter = numberOfParameter;
      _numberOfData      = numberOfData;
      _parameter = new double[numberOfParameter];

      double[] functionBase = new double[numberOfParameter];
      double[] scaledY      = new double[numberOfData];
      IMatrix u = new MatrixMath.BEMatrix( numberOfData, numberOfParameter);
      

      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for(int i=0;i<numberOfData;i++)
      {
        evaluateFunctionBase(xarr[i], functionBase);
        double scale = 1/stddev[i];

        for(int j=0;j<numberOfParameter;j++)
          u[i,j] = scale*functionBase[j];
        
        scaledY[i] = scale*yarr[i];
      }

      MatrixMath.SingularValueDecomposition decomposition = MatrixMath.GetSingularValueDecomposition(u);

      // set singular values < thresholdLevel to zero
      // ChopSingularValues makes only sense if all columns of the x matrix have the same variance
      //decomposition.ChopSingularValues(1E-5);
      // recalculate the parameters with the chopped singular values
      decomposition.Backsubstitution(scaledY,_parameter);

      _chiSquare = 0;
      for(int i=0;i<numberOfData;i++)
      {
        evaluateFunctionBase(xarr[i],functionBase);
        double ypredicted=0;
        for(int j=0;j<numberOfParameter;j++)
          ypredicted += _parameter[j]*functionBase[j];
        double deviation = yarr[i]-ypredicted;
        _chiSquare += deviation*deviation;
      }
    
      _covarianceMatrix = decomposition.GetCovariances();
 
    }

    /// <summary>
    /// Returns the number of parameter (=Order+1) of the fit.
    /// </summary>
    public int NumberOfParameter { get { return _numberOfParameter; }}

    public int NumberOfData { get { return _numberOfData; }}

    /// <summary>
    /// Get the resulting parameters, so that the model y = SUM(parameter[i]*functionbase[i])
    /// </summary>
    public double[] Parameter { get { return _parameter; }}
    /// <summary>
    /// Gets the sum of ChiSquare for the fit.
    /// </summary>
    public double   SumChiSquare { get { return _chiSquare;}}
    /// <summary>
    /// Get the variance-covariance-matrix for the fit.
    /// </summary>
    public double[][] Covariances { get { return _covarianceMatrix; }}


    /// <summary>Get the estimated residual mean square.</summary>
    /// <remarks>The estimated mean square is defined as SumChiSquare(n-p), where n is the number of data
    /// points and p is the number of (free) parameters.</remarks>
    public double SigmaSquare
    {
      get
      {
        if(_numberOfData>_numberOfFreeParameter)
          return SumChiSquare/(_numberOfData-_numberOfFreeParameter);
        else
          return 0;
      }
    }

    /// <summary>Get the standard error of regression, defined as <c>Sqrt(SigmaSquare)</c>.</summary>
    public double Sigma  {  get  { return Math.Sqrt(SigmaSquare); }}

  }
}
