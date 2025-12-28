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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Provides an <see cref="IInterpolationFunction"/> implementation that evaluates a polynomial regression fitted via SVD.
  /// </summary>
  public class PolynomialRegressionAsInterpolation : IInterpolationFunction
  {
    private Regression.LinearFitBySvd? _fit;
    private double _xMean = 0;
    private double _xScale = 1;

    /// <summary>
    /// Gets the polynomial regression order (degree) used for fitting.
    /// </summary>
    public int RegressionOrder { get; init; }

    /// <summary>
    /// Gets the fit. This property is only valid after the <see cref="Interpolate(IReadOnlyList{double}, IReadOnlyList{double})"/> function has been called.
    /// Note that the x-values of the fit were transformed, using <see cref="XMean"/> and <see cref="XScale"/> in the manner of <c>xt = (x - XMean) * XScale</c>; see <see cref="TransformXToInternalRepresentation(double)"/>.
    /// </summary>
    public Regression.LinearFitBySvd? Fit => _fit;

    /// <summary>
    /// Gets the mean of the set of x-values used for the fit.
    /// </summary>
    public double XMean => _xMean;

    /// <summary>
    /// Gets the inverse of the half span of the set of x-values used for the fit.
    /// </summary>
    public double XScale => _xScale;

    /// <summary>
    /// Transforms the x-value to the internal x-value, that can be used in conjunction with the fit parameters to get the y-value.
    /// </summary>
    /// <param name="x">The (external) x-value.</param>
    /// <returns>The transformed x-value.</returns>
    public double TransformXToInternalRepresentation(double x) => (x - _xMean) * _xScale;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolynomialRegressionAsInterpolation"/> class.
    /// The default regression order is 2.
    /// </summary>
    public PolynomialRegressionAsInterpolation()
    {
      RegressionOrder = 2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolynomialRegressionAsInterpolation"/> class using the specified regression order.
    /// </summary>
    /// <param name="regressionOrder">The polynomial regression order.</param>
    public PolynomialRegressionAsInterpolation(int regressionOrder)
    {
      RegressionOrder = regressionOrder;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public double GetYOfU(double u)
    {
      return GetYOfX(u);
    }

    /// <inheritdoc/>
    public double GetXOfU(double u)
    {
      return u;
    }
  }
}
