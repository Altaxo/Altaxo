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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  /// <summary>
  /// Stores the set of parameters necessary to calculate Savitzky-Golay coefficients.
  /// </summary>
  public class SavitzkyGolayParameters
  {
    /// <summary>
    /// Number of points used for Savitzky Golay Coefficients. Must be a odd positive number.
    /// </summary>
    public int NumberOfPoints=7;
    
    /// <summary>
    /// Polynomial order used to calculate Savitzky-Golay coefficients. Has to be a positive number.
    /// </summary>
    public int PolynomialOrder=2;
    
    /// <summary>
    /// Derivative order. Has to be zero or positive. A value of zero is used to smooth a function.
    /// </summary>
    public int DerivativeOrder=0;
  }

  /// <summary>
  /// SavitzkyGolay implements the calculation of the Savitzky-Golay filter coefficients and their application
  /// to smoth data, and to calculate derivatives.
  /// </summary>
  /// <remarks>Ref.: "Numerical recipes in C", chapter 14.8</remarks>
  public class SavitzkyGolay
  {
    /// <summary>
    /// Calculate Savitzky-Golay coefficients.
    /// </summary>
    /// <param name="leftpoints">Points on the left side included in the regression.</param>
    /// <param name="rightpoints">Points to the right side included in the regression.</param>
    /// <param name="derivativeorder">Order of derivative for which the coefficients are calculated.</param>
    /// <param name="polynomialorder">Order of the regression polynomial.</param>
    /// <param name="coefficients">Output: On return, contains the calculated coefficients.</param>
    public static void GetCoefficients(int leftpoints, int rightpoints, int derivativeorder, int polynomialorder, IVector coefficients)
    {
      int totalpoints = leftpoints+rightpoints+1;
      // Presumtions leftpoints and rightpoints must be >=0
      if(leftpoints<0)
        throw new ArgumentException("Argument leftpoints must not be <=0!");
      if(rightpoints<0)
        throw new ArgumentException("Argument rightpoints must not be <=0!");
      if(totalpoints<=1)
        throw new ArgumentException("Argument leftpoints and rightpoints must not both be zero!");
      if(polynomialorder>=totalpoints)
        throw new ArgumentException("Argument polynomialorder must not be smaller than total number of points");
      if(derivativeorder>polynomialorder)
        throw new ArgumentException("Argument derivativeorder must not be greater than polynomialorder!");
      if(coefficients==null || coefficients.Length<totalpoints)
        throw new ArgumentException("Vector of coefficients is either null or too short");
      // totalpoints must be greater than 1

      // Set up the design matrix
      // this is the matrix of i^j where i ranges from -leftpoints..rightpoints and j from 0 to polynomialorder 
      // as usual for regression, we not use the matrix directly, but instead the covariance matrix At*A
      Matrix mat = new Matrix(polynomialorder+1,polynomialorder+1);
      
      double[] val = new double[totalpoints];
      for(int i=0;i<totalpoints;i++) val[i]=1;

      for(int ord = 0;ord<=polynomialorder;ord++)
      {
        double sum = VectorMath.Sum(val);
        for(int i=0;i<=ord;i++)
          mat[ord-i,i] = sum;
        for(int i=0;i<totalpoints;i++)
          val[i] *= (i-leftpoints);
      }

      for(int ord = polynomialorder-1; ord>=0;ord--)
      {
        double sum = VectorMath.Sum(val);
        for(int i=0;i<=ord;i++)
          mat[polynomialorder-i,polynomialorder-ord+i] = sum;
        for(int i=0;i<totalpoints;i++)
          val[i] *= (i-leftpoints);
      }

      // now solve the equation
      ILuDecomposition decompose = mat.GetLuDecomposition();
      // ISingularValueDecomposition decompose = mat.GetSingularValueDecomposition();
      Matrix y = new Matrix(polynomialorder+1,1);
      y[derivativeorder,0] = 1;
      IMapackMatrix result = decompose.Solve(y);
    
      // to get the coefficients, the parameter have to be multiplied by i^j and summed up
      for(int i= -leftpoints;i<=rightpoints;i++)
      {
        double sum = 0;
        double x=1;
        for (int j=0;j<=polynomialorder;j++,x*=i)
          sum += result[j,0]*x;
        coefficients[i+leftpoints]=sum;
      }
    }


    /// <summary>Filters to apply to the left edge of the array.</summary>
    double[][] _left;
    /// <summary>Filters to apply to the right edge of the array. Note: the rightmost filter is in index 0</summary>
    double[][] _right;
    /// <summary>Filter to apply to the middle of the array.</summary>
    double[]   _middle;

    /// <summary>
    /// This sets up a Savitzky-Golay filter.
    /// </summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
    public SavitzkyGolay(int numberOfPoints, int derivativeOrder, int polynomialOrder)
    {
      numberOfPoints = 1+2*(numberOfPoints/2);
      int numberOfSide = (numberOfPoints-1)/2;

      _left = JaggedArrayMath.GetMatrixArray(numberOfSide,numberOfPoints);
      _right = JaggedArrayMath.GetMatrixArray(numberOfSide,numberOfPoints);
      _middle = new double[numberOfPoints];

      GetCoefficients(numberOfSide,numberOfSide,derivativeOrder,polynomialOrder,VectorMath.ToVector(_middle));

      for(int i=0;i<numberOfSide;i++)
      {
        GetCoefficients(i,2*numberOfSide-i,derivativeOrder,polynomialOrder,VectorMath.ToVector(_left[i]));
        GetCoefficients(2*numberOfSide-i,i,derivativeOrder,polynomialOrder,VectorMath.ToVector(_right[i]));
      }
    }


    /// <summary>
    /// This sets up a Savitzky-Golay filter.
    /// </summary>
    /// <param name="parameters">Set of parameters used for Savitzky-Golay filtering.</param>
    public SavitzkyGolay(SavitzkyGolayParameters parameters)
      : this(parameters.NumberOfPoints,parameters.DerivativeOrder,parameters.PolynomialOrder)
    {
    }


    /// <summary>
    /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
    /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to the input array!</param>
    public void Apply(double[] array, double[] result)
    {
      int filterPoints = _middle.Length;
      int sidePoints = (filterPoints-1)/2;

      if(object.ReferenceEquals(array,result))
        throw new ArgumentException("Argument array and result must not be identical!");

      if(array.Length<filterPoints)
        throw new ArgumentException("Input array must have same or greater length than the filter!");

      // left side
      for(int n=0;n<sidePoints;n++)
      {
        double[] filter = _left[n];
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[i]*filter[i];
        result[n] = sum;
      }

      // middle
      int middleend = array.Length-filterPoints;
      for(int n=0;n<=middleend;n++)
      {
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[n+i]*_middle[i];
        result[n+sidePoints] = sum;
      }

      // right side
      int arrayOffset = array.Length-filterPoints;
      int resultOffset = array.Length-1;
      for(int n=0;n<sidePoints;n++)
      {
        double[] filter = _right[n];
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[arrayOffset+i]*filter[i];
        result[resultOffset-n] = sum;
      }
    }

    /// <summary>
    /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
    /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
    /// </summary>
    /// <param name="array">The array of numbers to filter.</param>
    /// <param name="result">The resulting array. Must not be identical to the input array!</param>
    public void Apply(IROVector array, IVector result)
    {
      int filterPoints = _middle.Length;
      int sidePoints = (filterPoints-1)/2;

      if(object.ReferenceEquals(array,result))
        throw new ArgumentException("Argument array and result must not be identical!");

      if(array.Length<filterPoints)
        throw new ArgumentException("Input array must have same or greater length than the filter!");

      // left side
      for(int n=0;n<sidePoints;n++)
      {
        double[] filter = _left[n];
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[i]*filter[i];
        result[n] = sum;
      }

      // middle
      int middleend = array.Length-filterPoints;
      for(int n=0;n<=middleend;n++)
      {
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[n+i]*_middle[i];
        result[n+sidePoints] = sum;
      }

      // right side
      int arrayOffset = array.Length-filterPoints;
      int resultOffset = array.Length-1;
      for(int n=0;n<sidePoints;n++)
      {
        double[] filter = _right[n];
        double sum = 0;
        for(int i=0;i<filterPoints;i++)
          sum += array[arrayOffset+i]*filter[i];
        result[resultOffset-n] = sum;
      }
    }


  } // end class SavitzkyGolay
} 
