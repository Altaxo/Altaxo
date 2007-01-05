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
using System.Xml;
using Altaxo.Calc.LinearAlgebra;


namespace Altaxo.Calc.Regression.Multivariate
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
    int[] _regions;

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
    public int[] Regions
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
      _regions = new int[0];
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
      this._regions = (int[])from._regions.Clone();
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
      get { return true; }
    }

    /// <summary>
    /// Trys to identify spectral regions by supplying the spectral x values.
    /// A end_of_region is recognized when the gap between two x-values is ten times higher
    /// than the previous gap, or if the sign of the gap value changes.
    /// This method fails if a spectral region contains only a single point (since no gap value can be obtained then).
    /// (But in this case almost all spectral correction methods also fails).
    /// </summary>
    /// <param name="xvalues">The vector of x values for the spectra (wavelength, frequencies...).</param>
    public void SetRegionsByIdentification(IROVector xvalues)
    {   
      _regions = IdentifyRegions(xvalues);
    }

    /// <summary>
    /// Set the regions by providing an array of indices. These indices are the starting indices of the
    /// different regions.
    /// </summary>
    /// <param name="regions">Starting indices of the regions. Must be ascending. You can provide null as an argument.</param>
    public void SetRegions(int[] regions)
    {
      if(regions==null)
        _regions = new int[0];
      else
        _regions = (int[])regions.Clone();
    }

    /// <summary>
    /// Trys to identify spectral regions by supplying the spectral x values.
    /// A end_of_region is recognized when the gap between two x-values is ten times higher
    /// than the previous gap, or if the sign of the gap value changes.
    /// This method fails if a spectral region contains only a single point (since no gap value can be obtained then).
    /// (But in this case almost all spectral correction methods also fails).
    /// </summary>
    /// <param name="xvalues">The vector of x values for the spectra (wavelength, frequencies...).</param>
    /// <returns>The array of regions. Each element in the array is the starting index of a new region into the vector xvalues.</returns>
    public static int[] IdentifyRegions(IROVector xvalues)
    {
      System.Collections.ArrayList list = new System.Collections.ArrayList();

      int len = xvalues.Length;

      for(int i=0;i<len-2;i++)
      {
        double gap = Math.Abs(xvalues[i+1]-xvalues[i]);
        double nextgap = Math.Abs(xvalues[i+2]-xvalues[i+1]);
        if(gap!=0 && (Math.Sign(gap) == -Math.Sign(nextgap) || Math.Abs(nextgap)>10*Math.Abs(gap)))
        {
          list.Add(i+2);
          i++;
        }
      }
    
      return (int[])list.ToArray(typeof(int));
    }

    /// <summary>
    /// Gets the preprocessing method choosen.
    /// </summary>
    /// <returns>The preprocessing method.</returns>
    ISpectralPreprocessor GetPreprocessingMethod()
    {
      ISpectralPreprocessor result;
      switch(_method)
      {
        default:
        case SpectralPreprocessingMethod.None:
          result = new NoSpectralCorrection();
          break;
        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          result = new MultiplicativeScatterCorrection();
          break;
        case SpectralPreprocessingMethod.StandardNormalVariate:
          result = new StandardNormalVariateCorrection();
          break;
        case SpectralPreprocessingMethod.FirstDerivative:
          result = new SavitzkyGolayCorrection(7,1,2);
          break;
        case SpectralPreprocessingMethod.SecondDerivative:
          result = new SavitzkyGolayCorrection(11,2,3);
          break;
      }
      return result;
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

      GetPreprocessingMethod().Process(xMatrix,xMean,xScale,_regions);

      if(UseDetrending)
        new DetrendingCorrection(_detrendingOrder).Process(xMatrix,xMean,xScale,_regions);
    
      if(EnsembleMeanAfterProcessing || EnsembleScale)
        new EnsembleMeanAndScaleCorrection(EnsembleMeanAfterProcessing,EnsembleScale).Process(xMatrix,xMean,xScale,_regions);
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
      GetPreprocessingMethod().ProcessForPrediction(xMatrix,xMean,xScale,_regions);

      if(UseDetrending)
        new DetrendingCorrection(_detrendingOrder).ProcessForPrediction(xMatrix,xMean,xScale,_regions);
    
      if(EnsembleMeanAfterProcessing || EnsembleScale)
        new EnsembleMeanAndScaleCorrection(EnsembleMeanAfterProcessing,EnsembleScale).ProcessForPrediction(xMatrix,xMean,xScale,_regions);
    }

    public void Export(XmlWriter writer)
    {
      writer.WriteStartElement("SpectralRegions");
      for(int i=0;i<_regions.Length;i++)
        writer.WriteElementString("e",XmlConvert.ToString(_regions[i]));
      writer.WriteEndElement();

      GetPreprocessingMethod().Export(writer);

      if(UseDetrending)
        new DetrendingCorrection(_detrendingOrder).Export(writer);
    }

    #region ICloneable Members

    public object Clone()
    {
      return new SpectralPreprocessingOptions(this);
    }

    #endregion
  }
}
