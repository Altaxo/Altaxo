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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Calibration model for principal component regression (PCR).
  /// </summary>
  public class PCRCalibrationModel : MultivariateCalibrationModel
  {
#nullable disable
    private IROMatrix<double> _xScores;
    private IROMatrix<double> _xLoads;
    private IROMatrix<double> _yLoads;
    private IReadOnlyList<double> _crossProduct;
#nullable enable

    /// <summary>
    /// Gets or sets the x score matrix.
    /// </summary>
    public IROMatrix<double> XScores
    {
      get { return _xScores; }
      set { _xScores = value; }
    }

    /// <summary>
    /// Gets or sets the x loading matrix.
    /// </summary>
    public IROMatrix<double> XLoads
    {
      get { return _xLoads; }
      set { _xLoads = value; }
    }

    /// <summary>
    /// Gets or sets the y load matrix.
    /// </summary>
    public IROMatrix<double> YLoads
    {
      get { return _yLoads; }
      set { _yLoads = value; }
    }

    /// <summary>
    /// Gets or sets the cross-product vector (singular values) used by the PCR model.
    /// </summary>
    public IReadOnlyList<double> CrossProduct
    {
      get { return _crossProduct; }
      set { _crossProduct = value; }
    }
  }
}
