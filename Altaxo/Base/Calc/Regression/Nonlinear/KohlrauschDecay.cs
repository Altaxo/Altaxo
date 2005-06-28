using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Summary description for KohlrauschDecay.
	/// </summary>
	[FitFunction("KohlrauschDecay","Relaxation",1,1,4)]
	public class KohlrauschDecay : IFitFunction
	{
		public KohlrauschDecay()
		{
			//
			// TODO: Add constructor logic here
			//
    }
    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return 4;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return (new string[]{"offset","amplitude","tau","beta"})[i];
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[0] + P[1]*Math.Exp(-Math.Pow(X[0]/P[2],P[3]));
    }

    #endregion
  }
}
