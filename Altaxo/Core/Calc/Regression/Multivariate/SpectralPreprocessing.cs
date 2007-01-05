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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using System.Xml;


namespace Altaxo.Calc.Regression.Multivariate
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
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions);


    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction. Thats why it is absolutely neccessary to
    /// provide the right xMean and xScale vectors!
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Contains the scale of the spectral slots.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions);


    /// <summary>
    /// Exports the processing to an xml node.
    /// </summary>
    /// <param name="writer">The writer to export to</param>
    void Export(XmlWriter writer);
  }

  #region NoSpectralCorrection
  /// <summary>
  /// This class does nothing. It is only intended for export, and to hold static methods common to all.
  /// </summary>
  public class NoSpectralCorrection : ISpectralPreprocessor
  {
  
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public virtual void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
    {
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public virtual void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
    }

    /// <summary>
    /// Exports the processing to an xml node.
    /// </summary>
    /// <param name="writer">The writer to export to</param>
    public virtual void Export(XmlWriter writer)
    {
      writer.WriteElementString("NoSpectralCorrection",string.Empty);
    }

 
    /// <summary>
    /// Calculates the start index of region i.
    /// </summary>
    /// <param name="i">The number of region whose starting index is calculated.</param>
    /// <param name="regions">The array of region start indices.</param>
    /// <returns>The starting index of the region.</returns>
    public static int RegionStart(int i, int[] regions)
    {
      return i==0 ? 0 : regions[i+1];
    }
    /// <summary>
    /// Calculates the end index of region i (one above the last element).
    /// </summary>
    /// <param name="i">The number of region whose end index is calculated.</param>
    /// <param name="regions">The array of region start indices.</param>
    /// <param name="totalNumberOfPoints">The total number of points of the spectra.</param>
    /// <returns>The end index of the region (one above the last element).</returns>
    public static int RegionEnd(int i, int[] regions, int totalNumberOfPoints)
    {
      return i<regions.Length ? regions[i] : totalNumberOfPoints;
    }


  }
  #endregion

  #region MultiplicativeScatterCorrection (MSC)
  /// <summary>
  /// This class processes the spectra for influence of multiplicative scattering.
  /// </summary>
  public class MultiplicativeScatterCorrection : NoSpectralCorrection
  {
  
    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
    {
      // note: we have a light deviation here to the literature:
      // we repeat the multiple scattering correction until the xMean vector is self consistent,
      // in detail: after each MSC correction, we calculate the new xMean and compare with the xMean
      // of the step before. We repeat until the deviation of the xMean to the xMean_before is
      // reasonable small.
      // The reason for this deviation is that we don't want to store two separate xMean vectors: one used
      // for MSC (the x in linear regression) and another to center the MSC corrected spectra

      IVector xMeanBefore=null;
      double threshold = 1E-14*MatrixMath.SumOfSquares(xMatrix)/xMatrix.Rows;
      for(int cycle=0;cycle<50;cycle++)
      {
        // 1.) Get the mean spectrum
        // we want to have the mean of each matrix column, but not center the matrix now, since this
        // is done later on
        int cols = xMatrix.Columns;
        int rows = xMatrix.Rows;
        for(int n=0;n<cols;n++)
        {
          double sum = 0;
          for(int i=0;i<rows;i++)
            sum += xMatrix[i,n];
          xMean[n] = sum/rows;
        }

        // 2.) Process the spectras
        ProcessForPrediction(xMatrix,xMean,xScale, regions);

        // 3. Compare the xMean with the xMean_before
        if(xMeanBefore==null)
        {
          xMeanBefore = VectorMath.CreateExtensibleVector(xMean.Length);
          VectorMath.Copy(xMean,xMeanBefore);
        }
        else
        {
          double sumdiffsquare = VectorMath.SumOfSquaredDifferences(xMean,xMeanBefore);
          if(sumdiffsquare<threshold)
            break;
          else
            VectorMath.Copy(xMean,xMeanBefore);
        }
      }
    }


    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
      for(int i=0;i<=regions.Length;i++)
      {
        ProcessForPrediction(xMatrix,xMean,xScale,RegionStart(i,regions),RegionEnd(i,regions,xMatrix.Columns));
      }
    }


    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Output: On return, contains the ensemble mean of the spectra.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regionstart">Starting index of the region to process.</param>
    /// <param name="regionend">End index of the region to process.</param>
    void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int regionstart, int regionend)
    {
      int regionlength = regionend - regionstart;

      for(int n=0;n<xMatrix.Rows;n++)
      {
        // 2.) Do linear regression of the current spectrum versus the mean spectrum
        QuickLinearRegression regression = new QuickLinearRegression();
        for(int i=regionstart;i<regionend;i++)
          regression.Add(xMean[i],xMatrix[n,i]);

        double intercept = regression.GetA0();
        double slope = regression.GetA1();

        // 3.) Subtract intercept and divide by slope
        for(int i=regionstart;i<regionend;i++)
          xMatrix[n,i] = (xMatrix[n,i]-intercept)/slope;
      }
    }

    /// <summary>
    /// Exports the processing to an xml node.
    /// </summary>
    /// <param name="writer">The writer to export to</param>
    public override void Export(XmlWriter writer)
    {
      writer.WriteElementString("MultiplicativeScatterCorrection",string.Empty);
    }

  }
  #endregion  

  #region StandardNormalVariate (SNV)
  /// <summary>
  /// This class processes the spectra for influence of scattering.
  /// </summary>
  public class StandardNormalVariateCorrection : NoSpectralCorrection
  {

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
    {
      ProcessForPrediction(xMatrix,xMean,xScale, regions);
    }


    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
      for(int i=0;i<=regions.Length;i++)
      {
        ProcessForPrediction(xMatrix,xMean,xScale,RegionStart(i,regions),RegionEnd(i,regions,xMatrix.Columns));
      }
    }
    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regionstart">Starting index of the region to process.</param>
    /// <param name="regionend">End index of the region to process.</param>
    public void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int regionstart, int regionend)
    {
      int regionlength = regionend - regionstart;

      for(int n=0;n<xMatrix.Rows;n++)
      {
        // 1.) Get the mean response of a spectrum
        double mean = 0;
        for(int i=regionstart;i<regionend;i++)
          mean += xMatrix[n,i];
        mean /= regionlength;

        // 2.) Subtract mean response
        for(int i=regionstart;i<regionend;i++)
          xMatrix[n,i] -= mean;

        // 3.) Get the standard deviation
        double dev = 0;
        for(int i=regionstart;i<regionend;i++)
          dev += xMatrix[n,i]*xMatrix[n,i];
        dev = Math.Sqrt(dev/(regionlength-1));

        // 4. Divide by standard deviation
        for(int i=regionstart;i<regionend;i++)
          xMatrix[n,i] /= dev;
      }
    }

    public override void Export(XmlWriter writer)
    {
      writer.WriteElementString("StandardNormalVariateCorrection",string.Empty);
    }

  
  }

  #endregion  

  #region SavitzkyGolayCorrection
  /// <summary>
  /// This class processes the spectra for influence of scattering.
  /// </summary>
  public class SavitzkyGolayCorrection : NoSpectralCorrection
  {
    SavitzkyGolay _filter = null;

    int _numberOfPoints,_derivativeOrder,_polynomialOrder;

    /// <summary>This sets up a Savitzky-Golay-Filtering for all spectra.</summary>
    /// <param name="numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
    /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
    /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
    public SavitzkyGolayCorrection(int numberOfPoints, int derivativeOrder, int polynomialOrder)
    {
      _numberOfPoints = numberOfPoints;
      _derivativeOrder = derivativeOrder;
      _polynomialOrder = polynomialOrder;
      _filter = new SavitzkyGolay(numberOfPoints,derivativeOrder,polynomialOrder);
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used, since this processing sets xMean by itself (to zero).</param>
    /// <param name="xScale">Not used, since the processing sets xScale by itself.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
    {
      ProcessForPrediction(xMatrix,xMean,xScale, regions);
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
      for(int i=0;i<=regions.Length;i++)
      {
        ProcessForPrediction(xMatrix,xMean,xScale,RegionStart(i,regions),RegionEnd(i,regions,xMatrix.Columns));
      }
    }
    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regionstart">Starting index of the region to process.</param>
    /// <param name="regionend">End index of the region to process.</param>
    public void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int regionstart, int regionend)
    {
      int regionlength = regionend - regionstart;

      IVector helpervector = VectorMath.ToVector(new double[regionlength]);
      for(int n=0;n<xMatrix.Rows;n++)
      {
        IVector vector = MatrixMath.RowToVector(xMatrix,n,regionstart,regionlength);
        _filter.Apply(vector,helpervector);
        VectorMath.Copy(helpervector,vector);
      }
    }

    public override void Export(XmlWriter writer)
    {
      writer.WriteStartElement("SavitzkyGolayCorrection");
      writer.WriteElementString("NumberOfPoints",XmlConvert.ToString(_numberOfPoints));
      writer.WriteElementString("DerivativeOrder",XmlConvert.ToString(_derivativeOrder));
      writer.WriteElementString("PolynomialOrder",XmlConvert.ToString(_polynomialOrder));
      writer.WriteEndElement();
    }

  }
  #endregion  

  #region Detrending
  /// <summary>
  /// This class detrends all spectra. This is done by fitting a polynomial to the spectrum (x value is simply the index of data point), and then
  /// subtracting the fit curve from the spectrum.
  /// The degree of the polynomial can be choosen between 0 (the mean is subtracted), 1 (a fitted straight line is subtracted).
  /// </summary>
  public class DetrendingCorrection : NoSpectralCorrection
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
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
    {
      ProcessForPrediction(xMatrix,xMean,xScale,regions);
    }

    

    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
      for(int i=0;i<=regions.Length;i++)
        Process(xMatrix,xMean,xScale,RegionStart(i,regions),RegionEnd(i,regions,xMatrix.Columns));
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Not used.</param>
    /// <param name="xScale">Not used.</param>
    /// <param name="regionstart">Starting index of the region.</param>
    /// <param name="regionend">End index of the region (one behind the last region element).</param>
    public void Process(IMatrix xMatrix, IROVector xMean, IROVector xScale, int regionstart, int regionend)
    {
      int regionlength = regionend-regionstart;
      int currentorder = Math.Min(_order,regionlength);

      switch(currentorder)
      {
        case 0: // Detrending of order 0 - subtract mean
          for(int n=0;n<xMatrix.Rows;n++)
          {
            // 1.) Get the mean response of a spectrum
            double mean = 0;
            for(int i=regionstart;i<regionend;i++)
              mean += xMatrix[n,i];
            mean /= regionlength;

            for(int i=regionstart;i<regionend;i++)
              xMatrix[n,i] -= mean;
          }
          break;
        case 1: // Detrending of order 1 - subtract linear regression line
          for(int n=0;n<xMatrix.Rows;n++)
          {
            QuickLinearRegression regression = new QuickLinearRegression();
            for(int i=regionstart;i<regionend;i++)
              regression.Add(i,xMatrix[n,i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();

            for(int i=regionstart;i<regionend;i++)
              xMatrix[n,i] -= (a1*i+a0);
          }
          break;
        case 2: // Detrending of order 2 - subtract quadratic regression line
          for(int n=0;n<xMatrix.Rows;n++)
          {
            QuickQuadraticRegression regression = new QuickQuadraticRegression();
            for(int i=regionstart;i<regionend;i++)
              regression.Add(i,xMatrix[n,i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();
            double a2 = regression.GetA2();

            for(int i=regionstart;i<regionend;i++)
              xMatrix[n,i] -= (((a2*i)+a1)*i+a0);
          }
          break;

        default:
          throw new NotImplementedException(string.Format("Detrending of order {0} is not implemented yet",_order));
      }
    }

    public override void Export(XmlWriter writer)
    {
      writer.WriteStartElement("DetrendingCorrection");
      writer.WriteElementString("Order",XmlConvert.ToString(_order));
      writer.WriteEndElement();
    }
  }
  #endregion  


  #region EnsembleMeanAndScaleCorrection
  /// <summary>
  /// This class takes the ensemble mean of all spectra and then subtracts the mean from all spectra.
  /// It then takes the variance of each wavelength slot and divides all spectral slots by their ensemble variance.
  /// </summary>
  public class EnsembleMeanAndScaleCorrection : NoSpectralCorrection
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
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void Process(IMatrix xMatrix, IVector xMean, IVector xScale, int[] regions)
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

    /// <summary>
    /// Processes the spectra in matrix xMatrix for prediction.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Must be supplied, and will be subtracted from all spectra (if option set).</param>
    /// <param name="xScale">Must be supplied, and will be multiplied to all spectra (if option set).</param>
    /// <param name="regions">Vector of spectal regions. Each element is the index of the start of a new region.</param>
    public override void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale, int[] regions)
    {
      if(_ensembleMean)
      {
        MatrixMath.SubtractRow(xMatrix, xMean,xMatrix);
      }
      if(_ensembleScale)
      {
        MatrixMath.MultiplyRow(xMatrix,xScale,xMatrix);
      }
    }

    public override void Export(XmlWriter writer)
    {
    }

  }
  #endregion  
}
