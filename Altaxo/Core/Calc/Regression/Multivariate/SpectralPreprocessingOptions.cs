#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Lists the available basic preprocessing methods.
  /// </summary>
  public enum SpectralPreprocessingMethod
  {
    /// <summary>
    /// No basic preprocessing.
    /// </summary>
    None,

    /// <summary>
    /// Multiplicative scattering correction.
    /// </summary>
    MultiplicativeScatteringCorrection,

    /// <summary>
    /// Standard normal variate correction.
    /// </summary>
    StandardNormalVariate,

    /// <summary>
    /// Takes the first derivative of each spectrum.
    /// </summary>
    FirstDerivative,

    /// <summary>
    /// Takes the second derivative of each spectrum.
    /// </summary>
    SecondDerivative
  }

  /// <summary>
  /// Holds options that are applied to all spectra before they are processed by PLS or PCR.
  /// </summary>
  public class SpectralPreprocessingOptions : System.ICloneable
  {
    private SpectralPreprocessingMethod _method;
    private int _detrendingOrder;
    private bool _ensembleScale;
    private int[] _regions;

    /// <summary>
    /// Gets or sets the main method used for spectral preprocessing.
    /// </summary>
    /// <value>The spectral preprocessing method.</value>
    public SpectralPreprocessingMethod Method
    {
      get { return _method; }
      set { _method = value; }
    }

    /// <summary>
    /// Gets or sets the indices delimiting regions.
    /// </summary>
    /// <remarks>
    /// By default, this array is empty (length 0). Each element of this array is an index into the spectrum.
    /// Each index divides the spectrum into two regions: one before (up to index - 1) and a second starting at the index
    /// (up to the next index or to the end).
    /// </remarks>
    public int[] Regions
    {
      get { return _regions; }
      set { _regions = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingOptions"/> class.
    /// All options are set to their defaults.
    /// </summary>
    public SpectralPreprocessingOptions()
    {
      _method = SpectralPreprocessingMethod.None;
      _detrendingOrder = -1;
      _ensembleScale = false;
      _regions = new int[0];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingOptions"/> class by copying values from another instance.
    /// </summary>
    /// <param name="from">The instance to copy options from.</param>
    public SpectralPreprocessingOptions(SpectralPreprocessingOptions from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies all settings from another instance.
    /// </summary>
    /// <param name="from">The other instance to copy the data from.</param>
    [MemberNotNull(nameof(_regions))]
    public void CopyFrom(SpectralPreprocessingOptions from)
    {
      if (object.ReferenceEquals(this, from) && !(_regions is null))
        return;

      _method = from._method;
      _detrendingOrder = from._detrendingOrder;
      _ensembleScale = from._ensembleScale;
      _regions = (int[])from._regions.Clone();
    }

    /// <summary>
    /// Gets or sets a value indicating whether detrending is applied to each spectrum.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if detrending is currently used; otherwise, <see langword="false"/>.
    /// If set to <see langword="true"/>, a default detrending order of 0 will be assumed.
    /// </value>
    public bool UseDetrending
    {
      get { return _detrendingOrder >= 0; }
      set
      {
        if (value)
        {
          if (_detrendingOrder < 0)
            _detrendingOrder = 0;
        }
        else
        {
          _detrendingOrder = -1;
        }
      }
    }

    /// <summary>
    /// Gets or sets the order used for detrending.
    /// </summary>
    /// <remarks>
    /// Zero order means that, from a given spectrum, the mean of all spectral slots is subtracted.
    /// First order means that a regression is fitted over all spectral wavelengths (using the index as the <c>X</c> value),
    /// and that line is then subtracted from the spectrum.
    /// </remarks>
    /// <value>
    /// Order of detrending. Available values are 0, 1, and 2. A negative value indicates that detrending is not used.
    /// </value>
    public int DetrendingOrder
    {
      get { return _detrendingOrder; }
      set
      {
        if (value < 0)
          _detrendingOrder = -1;
        else if (value > 2)
          _detrendingOrder = 2;
        else
          _detrendingOrder = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the spectral ensemble should be scaled so that each spectral slot (wavelength)
    /// has a variance of 1 over the ensemble of spectra.
    /// </summary>
    /// <value><see langword="true"/> if ensemble scaling is used; otherwise, <see langword="false"/>.</value>
    public bool EnsembleScale
    {
      get { return _ensembleScale; }
      set { _ensembleScale = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the ensemble mean should be taken after spectral preprocessing.
    /// </summary>
    /// <remarks>
    /// This is the normal case. Only in case of <see cref="SpectralPreprocessingMethod.MultiplicativeScatteringCorrection"/>
    /// the ensemble mean is taken by the preprocessing method itself, so that it is unnecessary to do it again after processing.
    /// </remarks>
    /// <value><see langword="true"/> if the ensemble mean should be subtracted after preprocessing (usually the case).</value>
    public bool EnsembleMeanAfterProcessing
    {
      get { return true; }
    }

    /// <summary>
    /// Tries to identify spectral regions by analyzing the spectral <c>X</c> values.
    /// </summary>
    /// <remarks>
    /// An end of a region is recognized when the gap between two consecutive <c>X</c> values is ten times larger than the previous gap,
    /// or if the sign of the gap changes.
    /// This method fails if a spectral region contains only a single point (since no gap value can be obtained then).
    /// (But in this case almost all spectral correction methods also fail.)
    /// </remarks>
    /// <param name="xvalues">The vector of <c>X</c> values for the spectra (wavelengths, frequencies, ...).</param>
    public void SetRegionsByIdentification(IReadOnlyList<double> xvalues)
    {
      _regions = IdentifyRegions(xvalues);
    }

    /// <summary>
    /// Sets the regions by providing an array of starting indices.
    /// </summary>
    /// <param name="regions">Starting indices of the regions. Must be ascending. You can provide <see langword="null"/> as an argument.</param>
    public void SetRegions(int[] regions)
    {
      if (regions is null)
        _regions = new int[0];
      else
        _regions = (int[])regions.Clone();
    }

    /// <summary>
    /// Tries to identify spectral regions by analyzing the spectral <c>X</c> values.
    /// </summary>
    /// <remarks>
    /// An end of a region is recognized when the gap between two consecutive <c>X</c> values is ten times larger than the previous gap,
    /// or if the sign of the gap changes.
    /// This method fails if a spectral region contains only a single point (since no gap value can be obtained then).
    /// (But in this case almost all spectral correction methods also fail.)
    /// </remarks>
    /// <param name="xvalues">The vector of <c>X</c> values for the spectra (wavelengths, frequencies, ...).</param>
    /// <returns>The array of regions. Each element in the array is the starting index of a new region into <paramref name="xvalues"/>.</returns>
    public static int[] IdentifyRegions(IReadOnlyList<double> xvalues)
    {
      var list = new List<int>();

      int len = xvalues.Count;

      for (int i = 0; i < len - 2; i++)
      {
        double gap = Math.Abs(xvalues[i + 1] - xvalues[i]);
        double nextgap = Math.Abs(xvalues[i + 2] - xvalues[i + 1]);
        if (gap != 0 && (Math.Sign(gap) == -Math.Sign(nextgap) || Math.Abs(nextgap) > 10 * Math.Abs(gap)))
        {
          list.Add(i + 2);
          i++;
        }
      }

      return list.ToArray();
    }

    

    


    

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>A copy of this instance.</returns>
    public object Clone()
    {
      return new SpectralPreprocessingOptions(this);
    }

    #endregion ICloneable Members
  }
}
