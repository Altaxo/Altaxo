using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  public class PeakFitFunctions
  {
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
