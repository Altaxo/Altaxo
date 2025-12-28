#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Approximates the Jacobian matrix using finite differences (assuming the Jacobian is a band matrix with a fixed lower and upper bandwidth).
  /// </summary>
  public class BandJacobianMatrixEvaluator
  {
    /// <summary>
    /// The derivatives at the current <c>y</c> values.
    /// </summary>
    private double[] _derivatives_current;

    /// <summary>
    /// The derivatives at the variated <c>y</c> values.
    /// </summary>
    private double[] _derivatives_variated;

    /// <summary>
    /// The function to calculate the derivatives.
    /// </summary>
    /// <remarks>
    /// The first argument is the independent variable (usually designated with <c>x</c> or <c>t</c>),
    /// the second argument is the array of <c>y</c> values, and the third argument is the array that receives
    /// the calculated derivatives <c>dy/dx</c>.
    /// </remarks>
    private Action<double, double[], double[]> _f;

    /// <summary>
    /// The lower bandwidth of the band matrix.
    /// </summary>
    private int _lowerBandwidth;

    /// <summary>
    /// The upper bandwidth of the band matrix.
    /// </summary>
    private int _upperBandwidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="BandJacobianMatrixEvaluator"/> class.
    /// </summary>
    /// <param name="f">
    /// The function to calculate the derivatives.
    /// </param>
    /// <param name="lowerBandwidth">The lower bandwidth of the band matrix.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the band matrix.</param>
    /// <remarks>
    /// <paramref name="f"/> is called with the independent variable (usually designated with <c>x</c> or <c>t</c>),
    /// an array of state values <c>y</c>, and an output array that receives the derivatives.
    /// </remarks>
    public BandJacobianMatrixEvaluator(Action<double, double[], double[]> f, int lowerBandwidth, int upperBandwidth)
    {
      _f = f;
      _lowerBandwidth = lowerBandwidth;
      _upperBandwidth = upperBandwidth;
      _derivatives_current = new double[0];
      _derivatives_variated = new double[0];
    }

    /// <summary>
    /// Evaluates the Jacobian matrix.
    /// </summary>
    /// <param name="x">The value of the independent variable (usually designated with <c>x</c> or <c>t</c>).</param>
    /// <param name="y">The array of state values <c>y</c>.</param>
    /// <param name="jac">
    /// On return, contains the matrix with Jacobian values.
    /// If <see langword="null"/> is provided, a new matrix is allocated.
    /// </param>
    /// <exception cref="InvalidCastException">
    /// Thrown when <paramref name="jac"/> is not <see langword="null"/> and is not a <see cref="BandDoubleMatrix"/> instance.
    /// </exception>
    public void EvaluateJacobian(double x, double[] y, [AllowNull][NotNull] ref IMatrix<double> jac)
    {
      int N = y.Length;
      if (_derivatives_current.Length != N)
      {
        _derivatives_current = new double[y.Length];
        _derivatives_variated = new double[y.Length];
      }

      if (jac is null)
      {
        jac = new BandDoubleMatrix(y.Length, y.Length, _lowerBandwidth, _upperBandwidth);
      }
      var jacnative = (BandDoubleMatrix)jac;
      jacnative.Clear();

      _f(x, y, _derivatives_current); // evaluate rates at old point x

      double variation;
      for (int ri = 0; ri < N; ++ri)
      {
        var yi_saved = y[ri]; // save state
        y[ri] += (variation = Math.Sqrt(1e-6 * Math.Max(1e-5, Math.Abs(y[ri]))));
        _f(x, y, _derivatives_variated); // calculate rates at x variated at index i
        y[ri] = yi_saved; // restore old state

        int c_start = Math.Max(0, ri - _lowerBandwidth);
        int c_end = Math.Min(N, ri + _upperBandwidth + 1);

        for (int ci = c_start; ci < c_end; ++ci)
        {
          jacnative[ri, ci] = (_derivatives_variated[ci] - _derivatives_current[ci]) / (variation);
        }
      }
    }
  }
}

