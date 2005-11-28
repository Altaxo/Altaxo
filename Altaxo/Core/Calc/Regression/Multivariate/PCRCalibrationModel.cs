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
  public class PCRCalibrationModel : MultivariateCalibrationModel
  {

    IROMatrix _xScores;
    IROMatrix _xLoads;
    IROMatrix _yLoads;
    IROVector _crossProduct;

    public IROMatrix XScores
    {
      get { return _xScores; }
      set { _xScores = value; }
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

    public IROVector CrossProduct
    {
      get { return _crossProduct; }
      set { _crossProduct = value; }
    }

  
   
  }


}
