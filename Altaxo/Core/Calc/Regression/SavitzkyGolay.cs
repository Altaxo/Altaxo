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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
	/// <summary>
	/// SavitzkyGolay implements the calculation of the Savitzky-Golay filter coefficients and their application
	/// to smoth data, and to calculate derivatives.
	/// </summary>
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
      // Set up the design matrix
      // this is the matrix of i^j where i ranges from -leftpoints..rightpoints and j from 0 to polynomialorder 
    
      Matrix mat = new Matrix(leftpoints+rightpoints+1,polynomialorder+1);
      for(int i=-leftpoints;i<=rightpoints;i++)
      {
        double x=1;
        for(int j=1;j<=polynomialorder;j++,x*=i)
          mat[i+leftpoints,j] = x;
      }

      ILuDecomposition decompose = mat.GetLuDecomposition();
      Matrix y = new Matrix(leftpoints+rightpoints+1,1);
      y[derivativeorder,0] = 1;
      IMapackMatrix result = decompose.Solve(y);
    
      for(int i= -leftpoints;i<=rightpoints;i++)
      {
        double sum = result[0,0];
        double x=1;
        for (int j=0;j<polynomialorder;j++,x*=i)
          sum += result[j,0]*x;
        coefficients[i+leftpoints]=sum;
      }
    }
  }
}
