#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Text;

using Altaxo.Calc.LinearAlgebra;


namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Algorithms to estimate the parameters of a dynamic difference equation.
  /// </summary>
  public class DynamicParameterEstimation
  {
    int _numY;
    int _numX;
    int _backgroundOrderPlus1;
    
    /// <summary>
    /// The singular value composition of our data.
    /// </summary>
    MatrixMath.SingularValueDecomposition _decomposition;

    double[] _parameter;

    public DynamicParameterEstimation Calculate(IROVector x, IROVector y, int numX, int numY, int backgroundOrder)
    {
      _numX = numX;
      _numY = numY;
      _backgroundOrderPlus1 = 1 + Math.Max(-1, backgroundOrder);

      // where to start the calculation (index of first y point that can be used)
      int start = Math.Max(_numX-1, _numY );
      int numberOfData = 0;
      if (numX > 0)
        numberOfData = Math.Min(x.Length, y.Length) - start;
      else
        numberOfData = y.Length - start;

      int numberOfParameter = _numX + _numY + _backgroundOrderPlus1;



      MatrixMath.BEMatrix u = new MatrixMath.BEMatrix(numberOfData, numberOfParameter);

      // Fill the matrix
      for (int i = 0; i < numberOfData; i++)
      {
        int yIdx = i + start;
        // x
        for (int j = 0; j < _numX; j++)
        {
          u[i, j] = x[yIdx - j];
        }
        // y
        for (int j = 0; j < _numY; j++)
        {
          u[i, j + _numX] = y[yIdx - 1 - j];
        }
        // polynomial background component
        double background=1;
        for(int j=0;j<_backgroundOrderPlus1;j++)
        {
          u[i, j + _numX + _numY] = background;
          background *= yIdx;
        }
      }

      // Fill the y
      double[] scaledY = new double[numberOfData];
      for (int i = 0; i < numberOfData; i++)
        scaledY[i] = y[i + start];


      _decomposition = MatrixMath.GetSingularValueDecomposition(u);

      // set singular values < thresholdLevel to zero
      // ChopSingularValues makes only sense if all columns of the x matrix have the same variance
      //decomposition.ChopSingularValues(1E-5);
      // recalculate the parameters with the chopped singular values

      _parameter = new double[numberOfParameter];
      _decomposition.Backsubstitution(scaledY, _parameter);

      return this;
    }

    /// <summary>
    /// Gets the impulse response to a pulse at t=0, i.e. to x[0]==1, x[1]...x[n]==0. The background component is not taken into account.
    /// </summary>
    /// <param name="output">Used to store the output result. Can be of arbitrary size.</param>
    /// <param name="yValueBeforePulse">This is the y-value (not x!) before the pulse. If the <c>NumberOfY</c> is set to zero, this parameter is ignored, since no information about y for t&lt;0 is neccessary.</param>
    public void GetTransferFunction(IVector output, double yValueBeforePulse)
    {
      double[] y = new double[_numY];

      // Initialization
      for (int i = 0; i < _numY; i++)
        y[i] = yValueBeforePulse;

      for (int i = 0; i < output.Length; i++)
      {
        double sum = i<_numX ? _parameter[i] : 0; // this is the contribution of x

        for (int j = 0; j < _numY; j++)
          sum += _parameter[j + _numX] * y[j];

        // right-shift both y 
        for (int j = _numY - 1; j > 0; j--)
          y[j] = y[j - 1];

        // and set the actual values
        if(_numY>0)
          y[0] = sum;
        
        output[i] = sum;
      }


    }

    /// <summary>
    /// Extrapolates y-values until the end of the vector by using linear prediction.
    /// </summary>
    /// <param name="yTraining">Input vector of y values used to calculated the prediction coefficients.
    /// <param name="yPredValues">Input/output vector of y values to extrapolate.
    /// The fields beginning from 0 to <c>len-1</c> must contain valid values used for initialization of the extrapolation.
    /// At the end of the procedure, the upper end (<c>len</c> .. <c>yPredValues.Count-1</c> contain the 
    /// extrapolated data.</param>
    /// </param>
    /// <param name="len">Number of valid input data points for extrapolation (not for the training data!).</param>
    /// <param name="yOrder">Number of history samples used for prediction. Must be greater or equal to 1.</param>
    public static DynamicParameterEstimation Extrapolate(IROVector yTraining, IVector yPredValues, int len, int yOrder)
    {
      if (yOrder < 1)
        throw new ArgumentException("yOrder must be at least 1");
      if (yOrder >= (yTraining.Length - yOrder))
        throw new ArgumentException("Not enough data points for this degree (yOrder must be less than yTraining.Length/2).");

      DynamicParameterEstimation est = new DynamicParameterEstimation();
      est.Calculate(null, yTraining, 0, yOrder, 0);

      // now calculate the extrapolation data
      
      for (int i = len; i < yPredValues.Length; i++)
      {
        double sum = 0;
        for (int j = 0; j < yOrder; j++)
        {
          sum += yPredValues[i - j - 1] * est._parameter[j];
        }
        yPredValues[i] = sum;
      }
      return est;
    }
  }
}
