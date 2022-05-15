using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Regression.Nonlinear
{
  public class QuickNonlinearRegression
  {
    IFitFunction _fitFunction;


    public QuickNonlinearRegression(IFitFunction fitFunction)
    {
      _fitFunction = fitFunction ?? throw new ArgumentNullException(nameof(fitFunction));
    }

    public double[] Fit(double[] xValues, double[] yValues, double[] initialGuess)
    {
      return Fit(xValues, yValues, initialGuess, new bool[initialGuess.Length]);
    }

      public double[] Fit(double[] xValues, double[] yValues, double[] initialGuess, bool[] isFixed)
    {
      if (xValues.Length != yValues.Length)
        throw new ArgumentException("Length of x array is unequal length of y array");

      if (initialGuess.Length != _fitFunction.NumberOfParameters)
        throw new ArgumentException("Number of provided parameters is unequal number of parameters of fit function");

      var adapter = new Adapter(xValues, yValues, initialGuess, _fitFunction, isFixed);
      double[] param = Enumerable.Zip(initialGuess, isFixed, (param, isfix) => (param, isfix)).Where(e => !e.isfix).Select(e => e.param).ToArray();
      double[] ys = new double[yValues.Length];
      int info = 0;

      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(adapter.EvaluateFunctionDifferences), param, ys, 1E-10, ref info);

      var result = (double[])initialGuess.Clone();
      for (int i = 0; i < param.Length; ++i)
      {
        result[adapter.ParameterMapping[i]] = param[i];
      }

      return result;
    }

   private class Adapter
    {
      double[] _xValues;
      double[] _yValues;
      IFitFunction _fitFunction;
      double[] _parameters;
      int[] _parameterMapping;
      double[] _xx = new double[1];
      double[] _yy = new double[1];

      public int FreeParameterCount => _parameterMapping.Length;

      public int[] ParameterMapping => _parameterMapping; 

      public Adapter(double[] x, double[] y, double[] initialGuess, IFitFunction fitFunc, bool[] isFixed)
      {
        _xValues = x;
        _yValues = y;
        _fitFunction = fitFunc;
        _parameters = (double[])initialGuess.Clone();

        var l = new List<int>();
        for(int i=0;i<initialGuess.Length;i++)
        {
          if(!isFixed[i])
          {
            l.Add(i);
          }
        }
        _parameterMapping = l.ToArray();
      }

      public void EvaluateFunctionDifferences(int numberOfYs, int numberOfParameter, double[] param, double[] ys, ref int info)
      {
        for(int i=0; i < param.Length; ++i)
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
