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
  /// Approximates the jacobian matrix using finite differences (assuming the jacobian is a dense matrix).
  /// </summary>
  public class DenseJacobianMatrixEvaluator
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

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseJacobianMatrixEvaluator"/> class.
    /// </summary>
    /// <param name="f">
    /// The function to calculate the derivatives. First argument is the independent variable (usually designated with x or t),
    /// 2nd argument is the array of y values, and the third array accomodates the calculated derivatives dy/dx.
    /// </param>
    public DenseJacobianMatrixEvaluator(Action<double, double[], double[]> f)
    {
      _f = f;
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
        jac = new DoubleMatrix(y.Length, y.Length);
      }

      _f(x, y, _derivatives_current); // evaluate rates at old point x

      double variation;
      var jarray = ((DoubleMatrix)jac).data;
      for (int i = 0; i < N; ++i)
      {
        var yi_saved = y[i]; // save state
        y[i] += (variation = Math.Sqrt(1e-6 * Math.Max(1e-5, Math.Abs(y[i]))));
        _f(x, y, _derivatives_variated); // calculate rates at x variated at index i
        y[i] = yi_saved; // restore old state

        for (int c = 0; c < N; ++c)
        {
          jarray[c][i] = (_derivatives_variated[c] - _derivatives_current[c]) / (variation);
        }
      }
    }
  }
}

