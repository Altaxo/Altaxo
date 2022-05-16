using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  public class PeakFitFunctions
  {
    public class Gauss : IFitFunction
    {
      int _numberOfTerms;

      public Gauss(int numberOfTerms)
      {
        _numberOfTerms = numberOfTerms; 
      }

      /// <summary>
      /// Gets the width parameter of this function by providing the width at a relative height of the peak.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="relativeHeight">Height of the relative.</param>
      /// <returns></returns>
      /// <exception cref="System.ArgumentException">RelativeHeight should be in the open interval (0,1) - relativeHeight</exception>
      public double GetWidthParameterFromWidthAtRelativeHeight(double width, double relativeHeight)
      {
        if (!(relativeHeight > 0 && relativeHeight < 1))
          throw new ArgumentException("RelativeHeight should be in the open interval (0,1)", nameof(relativeHeight));

        return 0.5*width/Math.Sqrt(-2*Math.Log(relativeHeight));
      }

      public void Evaluate(double[] independent, double[] parameters, double[] FV)
      {
        double result = 0;
        var x = independent[0];
        for(int i=0,j=0;i<_numberOfTerms;++i,j+=3)
        {
          result += parameters[j] * Math.Exp(-0.5 * RMath.Pow2((x - parameters[j + 1]) / parameters[j + 2]));
        }
        FV[0] = result;
      }

      public int NumberOfIndependentVariables => 1;

      public int NumberOfDependentVariables => 1;

      public int NumberOfParameters => 3 * _numberOfTerms;

      public event EventHandler? Changed;

      public double DefaultParameterValue(int i)
      {
        throw new NotImplementedException();
      }

      public IVarianceScaling? DefaultVarianceScaling(int i)
      {
        throw new NotImplementedException();
      }

      public string DependentVariableName(int i)
      {
        throw new NotImplementedException();
      }

     

      public string IndependentVariableName(int i)
      {
        throw new NotImplementedException();
      }

      public string ParameterName(int i)
      {
        throw new NotImplementedException();
      }

      
    }



    public class FunctionWrapper : IScalarFunctionDD
    {
      IFitFunction _f;
      double[] _param;
      double[] _x;
      double[] _y;
    
    public FunctionWrapper(IFitFunction f, double[] param)
    {
      _f = f;
      _param = (double[])param.Clone();
        _x = new double[1];
        _y = new double[1];
    }
public double Evaluate(double x)
      {
        _x[0] = x;
        _f.Evaluate(_x, _param, _y);
        return _y[0];
      }
    }
  }
}
