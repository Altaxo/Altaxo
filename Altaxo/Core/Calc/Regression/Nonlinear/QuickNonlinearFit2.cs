using System;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace Altaxo.Calc.Regression.Nonlinear
{
  public class QuickNonlinearRegression2 : QuickNonlinearRegression
  {
    WrapperToFitFunction _wrapperToFitFunction;


    public QuickNonlinearRegression2(IFitFunction fitFunction) : base(fitFunction)
    {
      _wrapperToFitFunction = new WrapperToFitFunction(fitFunction.Evaluate);
    }

    public double[] Fit(double[] xValues, double[] yValues, double[] initialGuess, CancellationToken cancellationToken)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Length], cancellationToken);
    }

    public override double[] Fit(double[] xValues, double[] yValues, double[] initialGuess, bool[] isFixed, CancellationToken cancellationToken)
    {
      _isExecuted = false;
      if (xValues.Length != yValues.Length)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Length != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");


      var xVect = CreateVector.DenseOfArray(xValues);
      var yVect = CreateVector.DenseOfArray(yValues);

      var obj = ObjectiveFunction.NonlinearModel(_wrapperToFitFunction.Evaluate, xVect, yVect);

      var levmar = new LevenbergMarquardtMinimizer();
      var result = levmar.FindMinimum(obj, initialGuess, null, null, null, isFixed);

      _parameters = (double[])initialGuess.Clone();
      _parameterVariances = new double[_parameters.Length];
      for (int i = 0; i < _parameters.Length; ++i)
      {

        _parameters[i] = result.MinimizingPoint[i];
        _parameterVariances[i] = result.StandardErrors[i];
      }

      _isExecuted = true;
      return _parameters;
    }
  }
}
