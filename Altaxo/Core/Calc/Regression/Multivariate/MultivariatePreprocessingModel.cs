#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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


namespace Altaxo.Calc.Regression.Multivariate
{

  /// <summary>
  /// Contains the basic data that where obtained during preprocessing.
  /// </summary>
  public interface IMultivariatePreprocessingModel
  {
   
    int[] SpectralRegions
    {
      get;
    }

    IROVector XOfX
    {
      get;
    }


    IROVector XMean
    {
      get ; 
    }

    IROVector XScale
    {
      get; 
    }

    IROVector YMean
    {
      get ; 
    }

    IROVector YScale
    {
      get ; 
    }

    SpectralPreprocessingOptions PreprocessOptions
    {
      get;
    }
  }


  public class MultivariatePreprocessingModel : IMultivariatePreprocessingModel
  {
    
    SpectralPreprocessingOptions _preprocessOptions;

    int[]     _spectralRegions;
    IROVector _xOfX;
    IROVector _xMean;
    IROVector _xScale;
    IROVector _yMean;
    IROVector _yScale;



    public SpectralPreprocessingOptions PreprocessOptions
    {
      get { return _preprocessOptions; }
      set { _preprocessOptions = value; }
    }


    public int[] SpectralRegions
    {
      get { return _spectralRegions; }
      set { _spectralRegions = value; }
    }

    public IROVector XOfX
    {
      get { return _xOfX; }
      set { _xOfX = value; }
    }


    public IROVector XMean
    {
      get { return _xMean; }
      set { _xMean = value; }
    }

    public IROVector XScale
    {
      get { return _xScale; }
      set { _xScale = value; }
    }

    public IROVector YMean
    {
      get { return _yMean; }
      set { _yMean = value; }
    }

    public IROVector YScale
    {
      get { return _yScale; }
      set { _yScale = value; }
    }



  }
}
