using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Summary description for NonlinearFitDocument.
	/// </summary>
	public class NonlinearFitDocument
	{
    FitEnsemble _fitEnsemble;
    ParameterSet _currentParameters;
    object _fitContext; 


		public NonlinearFitDocument()
		{
      _fitEnsemble = new FitEnsemble();
      _currentParameters = new ParameterSet();
		}

    public object FitContext
    {
      get 
      {
        return _fitContext; 
      }
      set
      {
        _fitContext = value;
      }
    }

    public FitEnsemble FitEnsemble
    {
      get
      {
        return _fitEnsemble;
      }
    }

    public ParameterSet CurrentParameters
    {
      get
      {
        return _currentParameters;
      }
    }
	}
}
