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

      public void Evaluate(double[] independent, double[] parameters, double[] FV)
      {
        double result = 0;
        var x = independent[0];
        for(int i=0,j=0;i<_numberOfTerms;++i,j+=3)
        {
          result += parameters[j] * Math.Exp(-RMath.Pow2((x - parameters[j + 1]) / parameters[j + 2]));
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
  }
}
