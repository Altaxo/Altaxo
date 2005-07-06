using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Summary description for FitFunctionToScalarFunctionDDWrapper.
	/// </summary>
	public class FitFunctionToScalarFunctionDDWrapper : IScalarFunctionDD
	{
    IFitFunction _fitFunction;

    double[] _y;
    double[] _x;
    double[] _parameter;
    int _independentVariable;
    int _dependentVariable;

		public FitFunctionToScalarFunctionDDWrapper(IFitFunction fitFunction, int dependentVariable, double[] parameter)
		{
      Initialize(fitFunction,dependentVariable,parameter);
    }

    public void Initialize(IFitFunction fitFunction, int dependentVariable, double[] parameter)
    {
      _fitFunction = fitFunction;

      _x = new double[_fitFunction.NumberOfIndependentVariables];
      _y = new double[_fitFunction.NumberOfDependentVariables];
      _parameter = new double[_fitFunction.NumberOfParameters];
      _dependentVariable = dependentVariable;

      int len = Math.Min(_parameter.Length,parameter.Length);
      for(int i=0;i<len;i++)
        _parameter[i]=parameter[i];

    }

    public double[] Parameter
    {
      get
      {
        return _parameter;
      }
    }

    public double[] X
    {
      get
      {
        return _x;
      }
    }

    public int DependentVariable
    {
      get
      {
        return _dependentVariable;
      }
      set
      {
        _dependentVariable = value;
      }
    }

    public int IndependentVariable
    {
      get
      {
        return _independentVariable;
      }
      set
      {
        _independentVariable = value;
      }
    }


    #region IScalarFunctionDD Members

    public double Evaluate(double x)
    {
      if(_fitFunction!=null)
      {
        _x[_independentVariable] = x;     
        _fitFunction.Evaluate(_x,_parameter,_y);
        return _y[_dependentVariable];
      }
      else
      {
        return double.NaN;
      }
    }

    #endregion
  }
}
