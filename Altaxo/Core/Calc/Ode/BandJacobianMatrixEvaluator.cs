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
  /// Approximates the jacobian matrix using finite differences (assuming the jacobian is a band matrix with a fixed lower and upper bandwidth).
  /// </summary>
  public class BandJacobianMatrixEvaluator
  {
    /// <summary>
    /// The rates at the current y values.
    /// </summary>
    private double[] _derivatives_current;

    /// <summary>
    /// The rates at the variated y values.
    /// </summary>
    private double[] _derivatives_variated;

    /// <summary>
    /// The function to calculate the derivatives. First argument is the independent variable (usually designated with x or t),
    /// 2nd argument is the array of y values, and the third array accomodates the calculated derivatives dy/dx.
    /// </summary>
    private Action<double, double[], double[]> _f;
    private int _lowerBandwidth;
    private int _upperBandwidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseJacobianMatrixEvaluator"/> class.
    /// </summary>
    /// <param name="f">
    /// The function to calculate the derivatives. First argument is the independent variable (usually designated with x or t),
    /// 2nd argument is the array of y values, and the third array accomodates the calculated derivatives dy/dx.
    /// </param>
    /// <param name="lowerBandwidth">The lower bandwidth of the band matrix.</param>
    /// <param name="upperBandwidth">The upper bandwidth of the band matrix.</param>
    public BandJacobianMatrixEvaluator(Action<double, double[], double[]> f, int lowerBandwidth, int upperBandwidth)
    {
      _f = f;
      _lowerBandwidth = lowerBandwidth;
      _upperBandwidth = upperBandwidth;
      _derivatives_current = new double[0];
      _derivatives_variated = new double[0];
    }

    /// <summary>
    /// Evaluates the jacobian matrix.
    /// </summary>
    /// <param name="x">The value of the independent variable (usually named x or t).</param>
    /// <param name="y">The array of y values.</param>
    /// <param name="jac">At return, contains the matrix with jacobian values. If you provide null as this parameter, a new matrix is allocated.</param>
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

