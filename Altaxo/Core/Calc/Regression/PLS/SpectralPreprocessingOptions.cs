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


namespace Altaxo.Calc.Regression.PLS
{

  /// <summary>Gives the list of basic processing methods</summary>
  public enum SpectralPreprocessingMethod
  {
    /// <summary>No basic preprocessing.</summary>
    None,
    /// <summary>Multiple scattering correction.</summary>
    MultiplicativeScatteringCorrection,
    /// <summary>Standard normal variate correction.</summary>
    StandardNormalVariate,
    /// <summary>Taking the 1st derivative of each spectrum.</summary>
    FirstDerivative,
    /// <summary>Taking the 2nd derivative of each spectrum.</summary>
    SecondDerivative
  }

  
  /// <summary>
  /// SpectralPreprocessingOptions holds the options applied to all spectra before processed by PLS or PCR.
  /// </summary>
  public class SpectralPreprocessingOptions : System.ICloneable
  {
	
    SpectralPreprocessingMethod _method;
    int  _detrendingOrder;
    bool _ensembleScale;
    double[] _regions;

    /// <summary>
    /// Sets up the main method used for spectral preprocessing.
    /// </summary>
    /// <value>The spectal preprocessing method.</value>
    public SpectralPreprocessingMethod Method
    {
      get { return _method; }
      set { _method = value; }
    }

    /// <summary>
    /// Gets/sets the indices to regions. By default, this array is empty (zero length).
    /// Each element of this array is an index into the spectrum. Each index parts the spectrum in two regions: one before up to the index-1, and a second
    /// beginning from the index (to the next index or to the end).
    /// </summary>
    public double[] Regions
    {
      get { return _regions; }
      set { _regions = value; }
    }

    /// <summary>
    /// Default constructor. Set all options to none.
    /// </summary>
    public SpectralPreprocessingOptions()
    {
      _method = SpectralPreprocessingMethod.None;
      _detrendingOrder = -1;
      _ensembleScale = false;
      _regions = new double[0];
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from"></param>
    public SpectralPreprocessingOptions(SpectralPreprocessingOptions from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies all settings from another instance.
    /// </summary>
    /// <param name="from">The other instance to copy the data from.</param>
    public void CopyFrom(SpectralPreprocessingOptions from)
    {
      this._method = from._method;
      this._detrendingOrder = from._detrendingOrder;
      this._ensembleScale = from._ensembleScale;
      this._regions = (double[])from._regions.Clone();
    }

    /// <summary>
    /// Indicates if Detrending is applied to each spectrum.
    /// </summary>
    /// <value>True if detrending is currently used. If set to true, a default detrending order of 0 will be assumed.</value>
    public bool UseDetrending
    {
      get { return _detrendingOrder>=0; }
      set 
      {
        if(value)
        {
          if(_detrendingOrder<0)
            _detrendingOrder=0;
        }
        else
        {
          _detrendingOrder = -1;
        }
      }
    }

    /// <summary>
    /// Sets up the order used for detrending. Zero order means that from a given spectrum the mean of all spectral slots is subtracted.
    /// One (first order) means that a regression is made over all spectral wavelength (using index as x-value), and that line is then subtracted
    /// from the spectrum.
    /// </summary>
    /// <value>Order of detrending. Available values are: 0, 1 and 2. A negative value indicates that detrending is not used.</value>
    public int DetrendingOrder
    {
      get { return _detrendingOrder; }
      set 
      {
        if(value<0)
          _detrendingOrder = -1;
        else if(value>2)
          _detrendingOrder = 2;
        else
          _detrendingOrder = value;
      }
    }

    /// <summary>
    /// Sets up if the spectral ensemble should be scaled so that each spectral slot (wavelength) has a variance of 1
    /// over the ensemble of spectra.
    /// </summary>
    /// <value>True if ensemble scale is used. False otherwise.</value>
    public bool EnsembleScale
    {
      get { return _ensembleScale; }
      set { _ensembleScale = value; }
    }
    
    /// <summary>
    /// Indicates that the ensemble mean should be taken after the spectral preprocessing. This is the normal case.
    /// Only in case of MultipleScatteringCorrection the ensemble mean is taken by this method itself, so that it is unneccessary
    /// to do it again after the processing.
    /// </summary>
    /// <value>True if ensemble mean should be subtracted after the preprocessing (usually the case).</value>
    public bool EnsembleMeanAfterProcessing
    {
      get { return _method != SpectralPreprocessingMethod.MultiplicativeScatteringCorrection; }
    }


    /// <summary>
    /// Processes the spectra in matrix xMatrix according to the set-up options.
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Will be filled with the spectral mean.</param>
    /// <param name="xScale">Will be filled with the inverse spectral variance.(Or with 1 if the user has not choosen this option).</param>
    public void Process(IMatrix xMatrix, IVector xMean, IVector xScale)
    {
      // before processing, fill xScale with 1
      VectorMath.Fill(xScale,1);

      switch(_method)
      {
        case SpectralPreprocessingMethod.None:
          break;
        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          new MultiplicativeScatterCorrection().Process(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.StandardNormalVariate:
          new StandardNormalVariateCorrection().Process(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.FirstDerivative:
          new SavitzkyGolayCorrection(7,1,2).Process(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.SecondDerivative:
          new SavitzkyGolayCorrection(11,2,3).Process(xMatrix,xMean,xScale);
          break;
      }

      if(UseDetrending)
        new DetrendingCorrection(_detrendingOrder).Process(xMatrix,xMean,xScale);

    
      if(EnsembleMeanAfterProcessing || EnsembleScale)
        new EnsembleMeanAndScaleCorrection(EnsembleMeanAfterProcessing,EnsembleScale).Process(xMatrix,xMean,xScale);
    }

    /// <summary>
    /// Processes the spectra in matrix xMatrix according to the set-up options for prediction.
    /// Since it is prediction, the xMean and xScale vectors must be supplied here!
    /// </summary>
    /// <param name="xMatrix">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="xMean">Vector of spectral mean, must be supplied here.</param>
    /// <param name="xScale">Vector of inverse spectral variance, must be supplied here.</param>
    public void ProcessForPrediction(IMatrix xMatrix, IROVector xMean, IROVector xScale)
    {
      switch(_method)
      {
        case SpectralPreprocessingMethod.None:
          break;
        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          new MultiplicativeScatterCorrection().ProcessForPrediction(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.StandardNormalVariate:
          new StandardNormalVariateCorrection().ProcessForPrediction(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.FirstDerivative:
          new SavitzkyGolayCorrection(7,1,2).ProcessForPrediction(xMatrix,xMean,xScale);
          break;
        case SpectralPreprocessingMethod.SecondDerivative:
          new SavitzkyGolayCorrection(11,2,3).ProcessForPrediction(xMatrix,xMean,xScale);
          break;
      }

      if(UseDetrending)
        new DetrendingCorrection(_detrendingOrder).ProcessForPrediction(xMatrix,xMean,xScale);

    
      if(EnsembleMeanAfterProcessing || EnsembleScale)
        new EnsembleMeanAndScaleCorrection(EnsembleMeanAfterProcessing,EnsembleScale).ProcessForPrediction(xMatrix,xMean,xScale);
    }
    #region ICloneable Members

    public object Clone()
    {
      return new SpectralPreprocessingOptions(this);
    }

    #endregion
  }
}
