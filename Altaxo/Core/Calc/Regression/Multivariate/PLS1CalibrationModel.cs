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
  public class PLS1CalibrationModel : MultivariateCalibrationModel
  {
   

    IROMatrix[] _xWeights;
    IROMatrix[] _xLoads;
    IROMatrix[] _yLoads;
    IROMatrix[] _crossProduct;

   

    public override int NumberOfY
    {
      get { return _numberOfY; }
      set 
      {
        _numberOfY = value;
        Allocate(value);
      }
    }

   
    protected void Allocate(int numberOfY)
    {
      _xWeights = new IROMatrix[numberOfY] ;
      _xLoads = new IROMatrix[numberOfY];
      _yLoads = new IROMatrix[numberOfY];
      _crossProduct = new IROMatrix[numberOfY];
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

  
  
  }


}
