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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Defines the calibration model data required by a PLS2 regression.
  /// </summary>
  public interface IPLS2CalibrationModel : IMultivariateCalibrationModel
  {
    /// <summary>
    /// Gets the x weight matrix.
    /// </summary>
    IROMatrix<double> XWeights
    {
      get;
    }

    /// <summary>
    /// Gets the x loading matrix.
    /// </summary>
    IROMatrix<double> XLoads
    {
      get;
    }

    /// <summary>
    /// Gets the y loading matrix.
    /// </summary>
    IROMatrix<double> YLoads
    {
      get;
    }

    /// <summary>
    /// Gets the cross-product matrix (typically a 1-by-factors matrix).
    /// </summary>
    IROMatrix<double> CrossProduct
    {
      get;
    }
  }

  /// <summary>
  /// Calibration model for PLS2 regression.
  /// </summary>
  public class PLS2CalibrationModel : MultivariateCalibrationModel, IPLS2CalibrationModel
  {
#nullable disable
    private IROMatrix<double> _xWeights;
    private IROMatrix<double> _xLoads;
    private IROMatrix<double> _yLoads;
    private IROMatrix<double> _crossProduct;
#nullable enable

    /// <inheritdoc/>
    public IROMatrix<double> XWeights
    {
      get { return _xWeights; }
      set { _xWeights = value; }
    }

    /// <inheritdoc/>
    public IROMatrix<double> XLoads
    {
      get { return _xLoads; }
      set { _xLoads = value; }
    }

    /// <inheritdoc/>
    public IROMatrix<double> YLoads
    {
      get { return _yLoads; }
      set { _yLoads = value; }
    }

    /// <inheritdoc/>
    public IROMatrix<double> CrossProduct
    {
      get { return _crossProduct; }
      set { _crossProduct = value; }
    }
  }
}
