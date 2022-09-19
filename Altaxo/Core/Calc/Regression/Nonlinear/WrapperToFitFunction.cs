using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Nonlinear
{
  class WrapperToFitFunction
  {
    FitEvaluationFunction _f;
    double[] _x = new double[1];
    double[] _y = new double[1];

    public WrapperToFitFunction(FitEvaluationFunction f)
    {
      _f = f;
    }

    public Vector<double> Evaluate(Vector<double> parameter, Vector<double> x)
    {
      var _yR = Vector<double>.Build.Dense(x.Count);
      var p = parameter.ToArray();

      for (int i = 0; i < x.Count; i++)
      {
        _x[0] = x[i];
        _f(_x, p, _y);
        _yR[i] = _y[0];
      }
      return _yR;
    }
  }

  class WrapperToFitFunction2
  {
    Action<double[], IReadOnlyList<double>, double[]> _f;
    double[] _x = new double[1];
    double[] _y = new double[1];

    public WrapperToFitFunction2(Action<double[], IReadOnlyList<double>, double[]> f)
    {
      _f = f;
    }

    public Vector<double> Evaluate(Vector<double> parameter, Vector<double> x)
    {
      var _yR = Vector<double>.Build.Dense(x.Count);

      for (int i = 0; i < x.Count; i++)
      {
        _x[0] = x[i];
        _f(_x, parameter, _y);
        _yR[i] = _y[0];
      }
      return _yR;
    }
  }
}
