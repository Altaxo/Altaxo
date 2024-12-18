﻿#region Copyright

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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  public class PolynomialRegressionAsInterpolation : IInterpolationFunction
  {
    private Regression.LinearFitBySvd? _fit;
    private double _xMean = 0;
    private double _xScale = 1;

    public int RegressionOrder { get; set; }

    public PolynomialRegressionAsInterpolation()
    {
      RegressionOrder = 2;
    }

    public PolynomialRegressionAsInterpolation(int regressionOrder)
    {
      RegressionOrder = regressionOrder;
    }

    public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
    {
      // Center and scale x in order
      // to avoid numeric errors at high orders
      var xmin = xvec.Min();
      var xmax = xvec.Max();
      _xMean = 0.5 * (xmin + xmax);
      _xScale = 1 / (0.5 * (xmax - xmin));

      var err = VectorMath.GetConstantVector(1.0, yvec.Count);
      var xScaled = xvec.Select(x => (x - _xMean) * _xScale).ToArray();
      _fit = new Regression.LinearFitBySvd(xScaled, yvec, err, xvec.Count, RegressionOrder + 1, Regression.LinearFitBySvd.GetPolynomialFunctionBase(RegressionOrder), 1E-6);
    }

    public double GetYOfX(double x)
    {
      if (_fit is null)
        throw new InvalidOperationException($"Results not available yet - please execute an interpolation first");

      var xcs = (x - _xMean) * _xScale;
      double[] paras = _fit.Parameter;

      double result = 0;
      for (int i = paras.Length - 1; i >= 0; i--)
      {
        result *= xcs;
        result += paras[i];
      }
      return result;
    }

    public double GetYOfU(double u)
    {
      return GetYOfX(u);
    }

    public double GetXOfU(double u)
    {
      return u;
    }
  }
}
