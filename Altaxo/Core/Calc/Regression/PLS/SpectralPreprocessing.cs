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
    /// <param name="xScale">Not used.</param>
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

        double intercept = regression.GetA0();
        double slope = regression.GetA1();

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
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
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

        // 3.) Get the standard deviation
        double dev = 0;
        for(int i=0;i<xMatrix.Columns;i++)
          dev += xMatrix[n,i]*xMatrix[n,i];
        dev = Math.Sqrt(dev/(xMatrix.Columns-1));

        // 4. Divide by standard deviation
        for(int i=0;i<xMatrix.Columns;i++)
          xMatrix[n,i] /= dev;
      }
    }
  }
  #endregion  

  #region SavitzkyGolayCorrection
  /// <summary>
  /// This class processes the spectra for influence of scattering.
  /// </summary>
  public class SavitzkyGolayCorrection
  {
    SavitzkyGolay _filter = null;

    /// <summary>This sets up a Savitzky-Golay-Filtering for all spectra.</summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
    public SavitzkyGolayCorrection(int numberOfPoints, int derivativeOrder, int polynomialOrder)
    {
      _filter = new SavitzkyGolay(numberOfPoints,derivativeOrder,polynomialOrder);
    }
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {
      IVector helpervector = VectorMath.ToVector(new double[xMatrix.Columns]);
      for(int n=0;n<xMatrix.Rows;n++)
      {
        IVector vector = MatrixMath.RowToVector(xMatrix,n);
        _filter.Apply(vector,helpervector);
        MatrixMath.SetRow(helpervector,xMatrix,n);
      }
    }
  }
  #endregion  

  #region Detrending
  /// <summary>
  /// This class detrends all spectra. This is done by fitting a polynomial to the spectrum (x value is simply the index of data point), and then
  /// subtracting the fit curve from the spectrum.
  /// The degree of the polynomial can be choosen between 0 (the mean is subtracted), 1 (a fitted straight line is subtracted).
  /// </summary>
  public class DetrendingCorrection
  {
    int _order=0;
  
    public DetrendingCorrection(int order)
    {
      if(order<0)
        throw new ArgumentOutOfRangeException("Order must be 0 or positive");
      if(order>2)
        throw new ArgumentOutOfRangeException("Order must be in the range between 0 and 2");
      _order = order;
    }
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {
      switch(_order)
      {
        case 0: // Detrending of order 0 - subtract mean
          for(int n=0;n<xMatrix.Rows;n++)
          {
            // 1.) Get the mean response of a spectrum
            double mean = 0;
            for(int i=0;i<xMatrix.Columns;i++)
              mean += xMatrix[n,i];
            mean /= xMatrix.Columns;

            for(int i=0;i<xMatrix.Columns;i++)
              xMatrix[n,i] -= mean;
          }
          break;
        case 1: // Detrending of order 1 - subtract linear regression line
          for(int n=0;n<xMatrix.Rows;n++)
          {
            QuickLinearRegression regression = new QuickLinearRegression();
            for(int i=0;i<xMatrix.Columns;i++)
              regression.Add(i,xMatrix[n,i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();

            for(int i=0;i<xMatrix.Columns;i++)
              xMatrix[n,i] -= (a1*i+a0);
          }
          break;
        case 2: // Detrending of order 2 - subtract quadratic regression line
          for(int n=0;n<xMatrix.Rows;n++)
          {
            QuickQuadraticRegression regression = new QuickQuadraticRegression();
            for(int i=0;i<xMatrix.Columns;i++)
              regression.Add(i,xMatrix[n,i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();
            double a2 = regression.GetA2();

            for(int i=0;i<xMatrix.Columns;i++)
              xMatrix[n,i] -= (((a2*i)+a1)*i+a0);
          }
          break;

        default:
          throw new NotImplementedException(string.Format("Detrending of order {0} is not implemented yet",_order));
      }
    }
  }
  #endregion  


  #region EnsembleMeanAndScaleCorrection
  /// <summary>
  /// This class takes the ensemble mean of all spectra and then subtracts the mean from all spectra.
  /// It then takes the variance of each wavelength slot and divides all spectral slots by their ensemble variance.
  /// </summary>
  public class EnsembleMeanAndScaleCorrection
  {
   bool _ensembleMean;
    bool _ensembleScale;
  
    public EnsembleMeanAndScaleCorrection(bool ensembleMean, bool ensembleScale)
    {
      _ensembleMean = ensembleMean;
      _ensembleScale = ensembleScale;
    }
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {
      if(_ensembleMean)
      {
        MatrixMath.ColumnsToZeroMean(xMatrix, xMean);
      }
      if(_ensembleScale)
      {
        MatrixMath.ColumnsToZeroMeanAndUnitVariance(xMatrix,null,xScale);
      }
    }
  }
  #endregion  
}
