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

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// IMultivariateCalibrationModel contains the basic data for a
  /// multivariate calibration model
  /// </summary>
  public interface IMultivariateCalibrationModel
  {
    int NumberOfX
    {
      get; 
    }

    int NumberOfY
    {
      get; 
    }


    int NumberOfFactors
    {
      get;
    }

    IMultivariatePreprocessingModel PreprocessingModel
    {
      get; 
    }
  }

  public class MultivariateCalibrationModel : IMultivariateCalibrationModel
  {
    protected int _numberOfX;
    protected int _numberOfY;
    protected int _numberOfFactors;

    MultivariatePreprocessingModel _preprocessingData;


    public IMultivariatePreprocessingModel PreprocessingModel
    {
      get { return _preprocessingData; }
    }


    public void SetPreprocessingModel(MultivariatePreprocessingModel val)
    {
      _preprocessingData = val; 
    }

   
    public int NumberOfX
    {
      get { return _numberOfX; }
      set { _numberOfX = value; }
    }

    public virtual int NumberOfY
    {
      get { return _numberOfY; }
      set { _numberOfY = value; }
    }

    public int NumberOfFactors
    {
      get { return _numberOfFactors; }
      set { _numberOfFactors = value; }
    }
  }

}
