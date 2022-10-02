#region Copyright

/////////////////////////////////////////////////////////////////////////////
// Altaxo:  a data processing and data plotting program
// Copyright (C) 2002-2022 Dr. Dirk Lellinger
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Nonlinear
{
  [Obsolete("Please use QuickNonlinearRegression instead.")]
  public class QuickNonlinearRegressionOld
  {
    protected IFitFunction _fitFunction;

    protected double _sumChiSquare;
    protected double _sigmaSquare;

    protected double[] _parameters = new double[0];
    protected double[] _parameterVariances = new double[0];
    protected IROMatrix<double>? _covariances;

    protected bool _isExecuted;

    private void CheckExecuted()
    {
      if (!_isExecuted)
        throw new InvalidOperationException("Please execute the fit before accessing the results");
    }

    public double SumChiSquare
    {
      get
      {
        CheckExecuted();
        return _sumChiSquare;
      }
    }

    public double SigmaSquare
    {
      get
      {
        CheckExecuted();
        return _sigmaSquare;
      }
    }

    public IReadOnlyList<double> ParameterVariances
    {
      get
      {
        CheckExecuted();
        return _parameterVariances;
      }
    }

    public IReadOnlyList<double> Parameters
    {
      get
      {
        CheckExecuted();
        return _parameters;
      }
    }

    /// <summary>
    /// Gets the resulting covariances of the fit.
    /// </summary>
    /// <value>
    /// The covariances.
    /// </value>
    public IROMatrix<double> Covariances
    {
      get
      {
        CheckExecuted();
        return _covariances;
      }
    }


    public QuickNonlinearRegressionOld(IFitFunction fitFunction)
    {
      _fitFunction = fitFunction ?? throw new ArgumentNullException(nameof(fitFunction));
    }

    public double[] Fit(double[] xValues, double[] yValues, double[] initialGuess, CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Length], cancellationToken);
    }

    public virtual double[] Fit(double[] xValues, double[] yValues, double[] initialGuess, bool[] isFixed, CancellationToken cancellationToken)
    {
      _isExecuted = false;
      if (xValues.Length != yValues.Length)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Length != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");

      var adapter = new Adapter(xValues, yValues, initialGuess, _fitFunction, isFixed);
      double[] param = Enumerable.Zip(initialGuess, isFixed, (param, isfix) => (param, isfix)).Where(e => !e.isfix).Select(e => e.param).ToArray();
      double[] ys = new double[yValues.Length];
      int info = 0;

      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(adapter.EvaluateFunctionDifferences), param, ys, 1E-10, cancellationToken, ref info);
      var resultingCovariances = new double[param.Length * param.Length];
      _covariances = MatrixMath.ToROMatrixFromColumnMajorLinearArray(resultingCovariances, param.Length);
      NLFit.ComputeCovariances(new NLFit.LMFunction(adapter.EvaluateFunctionDifferences), param, ys.Length, param.Length, resultingCovariances, out _sumChiSquare, out _sigmaSquare);

      _parameters = (double[])initialGuess.Clone();
      _parameterVariances = new double[_parameters.Length];
      for (int i = 0; i < param.Length; ++i)
      {
        _parameters[adapter.ParameterMapping[i]] = param[i];
        _parameterVariances[adapter.ParameterMapping[i]] = resultingCovariances[i + i * param.Length];
      }

      _isExecuted = true;
      return _parameters;
    }

    private class Adapter
    {
      private double[] _xValues;
      private double[] _yValues;
      private IFitFunction _fitFunction;
      private double[] _parameters;
      private int[] _parameterMapping;
      private double[] _xx = new double[1];
      private double[] _yy = new double[1];

      public int FreeParameterCount => _parameterMapping.Length;

      public int[] ParameterMapping => _parameterMapping;

      public Adapter(double[] x, double[] y, double[] initialGuess, IFitFunction fitFunc, bool[] isFixed)
      {
        _xValues = x;
        _yValues = y;
        _fitFunction = fitFunc;
        _parameters = (double[])initialGuess.Clone();

        var l = new List<int>();
        for (int i = 0; i < initialGuess.Length; i++)
        {
          if (!isFixed[i])
          {
            l.Add(i);
          }
        }
        _parameterMapping = l.ToArray();
      }

      public void EvaluateFunctionDifferences(int numberOfYs, int numberOfParameter, double[] param, double[] ys, ref int info)
      {
        for (int i = 0; i < param.Length; ++i)
        {
          _parameters[_parameterMapping[i]] = param[i];
        }

        for (int i = 0; i < ys.Length; ++i) // TODO make this more efficient by using special fit functions that accept
        {                                   // an entire array of x values instead of only one
          _xx[0] = _xValues[i];
          _fitFunction.Evaluate(_xx, _parameters, _yy);
          ys[i] = _yy[0];

        }
        for (int i = 0; i < numberOfYs; i++)
          ys[i] -= _yValues[i];
      }
    }
  }
}
