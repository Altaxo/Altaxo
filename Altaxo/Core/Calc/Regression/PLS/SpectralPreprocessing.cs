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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.PLS
{
  /// <summary>
  /// Common interface for all methods for preprocessing the spectra before their usage in calibration and prediction.
  /// </summary>
  interface ISpectralPreprocessor
  {
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Output: On return, contains the scale of the spectral slots.</param>
    void Process(IMatrix xMatrix, IVector xMean, IVector xScale);
  }

  #region MultiplicativeScatterCorrection (MSC)
	/// <summary>
	/// This class processes the spectra for influence of multiplicative scattering.
	/// </summary>
  public class MultiplicativeScatterCorrection
  {
  
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Output: On return, contains the scale of the spectral slots.</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {
      // 1.) Get the mean spectrum
      MatrixMath.ColumnsToZeroMean(xMatrix,xMean);

      for(int n=0;n<xMatrix.Rows;n++)
      {
        // 2.) Do linear regression of the current spectrum versus the mean spectrum
        QuickLinearRegression regression = new QuickLinearRegression();
        for(int i=0;i<xMatrix.Columns;i++)
          regression.Add(xMean[i],xMatrix[n,i]);

        double intercept = regression.GetIntercept();
        double slope = regression.GetSlope();

        // 3.) Subtract intercept and divide by slope
        for(int i=0;i<xMatrix.Columns;i++)
          xMatrix[n,i] = (xMatrix[n,i]-intercept)/slope;
      }
    }
  }
  #endregion  



  #region StandardNormalVariate (SNV)
  /// <summary>
  /// This class processes the spectra for influence of scattering.
  /// </summary>
  public class StandardNormalVariateCorrection
  {
  
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Output: On return, contains the scale of the spectral slots.</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {

      for(int n=0;n<xMatrix.Rows;n++)
      {
        // 1.) Get the mean response of a spectrum
        double mean = 0;
        for(int i=0;i<xMatrix.Columns;i++)
          mean += xMatrix[n,i];
        mean /= xMatrix.Columns;

        // 2.) Subtract mean response
        for(int i=0;i<xMatrix.Columns;i++)
          xMatrix[n,i] -= mean;


        QuickLinearRegression regression = new QuickLinearRegression();
        for(int i=0;i<xMatrix.Columns;i++)
          regression.Add(xMean[i],xMatrix[n,i]);

        double intercept = regression.GetIntercept();
        double slope = regression.GetSlope();

        // 3.) Subtract intercept and divide by slope
        for(int i=0;i<xMatrix.Columns;i++)
          xMatrix[n,i] = (xMatrix[n,i]-intercept)/slope;
      }
    }
  }
  #endregion  


}
