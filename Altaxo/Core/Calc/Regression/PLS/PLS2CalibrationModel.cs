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
  public class PLS2CalibrationModel
  {
    IROVector _xOfX;
    IROVector _xMean;
    IROVector _xScale;
    IROMatrix _yMean;
    IROMatrix _yScale;

    IROMatrix _xWeights;
    IROMatrix _xLoads;
    IROMatrix _yLoads;
    IROMatrix _crossProduct;

    int _numberOfX;
    int _numberOfY;
    int _numberOfFactors;

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

    public IROMatrix YMean
    {
      get { return _yMean; }
      set { _yMean = value; }
    }

    public IROMatrix YScale
    {
      get { return _yScale; }
      set { _yScale = value; }
    }

    public IROMatrix XWeights
    {
      get { return _xWeights; }
      set { _xWeights = value; }
    }

    public IROMatrix XLoads
    {
      get { return _xLoads; }
      set { _xLoads = value; }
    }

    public IROMatrix YLoads
    {
      get { return _yLoads; }
      set { _yLoads = value; }
    }

    public IROMatrix CrossProduct
    {
      get { return _crossProduct; }
      set { _crossProduct = value; }
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
