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


namespace Altaxo.Calc.Regression.Multivariate
{
  public class PLS1CalibrationModel : IMultivariateCalibrationModel
  {
    IROVector _xOfX;
    IROVector _xMean;
    IROVector _xScale;
    IROVector _yMean;
    IROVector _yScale;

    IROMatrix[] _xWeights;
    IROMatrix[] _xLoads;
    IROMatrix[] _yLoads;
    IROMatrix[] _crossProduct;

    int _numberOfX;
    int _numberOfY;
    int _numberOfFactors;

    public PLS1CalibrationModel(int numberOfX, int numberOfY, int numberOfFactors)
    {
      _numberOfX = numberOfX;
      _numberOfY = numberOfY;
      _numberOfFactors = numberOfFactors;


      _xWeights = new IROMatrix[_numberOfY] ;
      _xLoads = new IROMatrix[_numberOfY];
      _yLoads = new IROMatrix[_numberOfY];
      _crossProduct = new IROMatrix[_numberOfY];
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

    public IROMatrix[] XWeights
    {
      get { return _xWeights; }
    }

    public IROMatrix[] XLoads
    {
      get { return _xLoads; }
    }

    public IROMatrix[] YLoads
    {
      get { return _yLoads; }
    }

    public IROMatrix[] CrossProduct
    {
      get { return _crossProduct; }
    }

    public int NumberOfX
    {
      get { return _numberOfX; }
      set { _numberOfX = value; }
    }

    public int NumberOfY
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
