using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Summary description for KohlrauschDecay.
  /// </summary>
  [FitFunction("HavriliakNegami Complex","Relaxation",1,2,4)]
  public class HavriliakNegamiComplex : IFitFunction
  {
    public HavriliakNegamiComplex()
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #region IFitFunction Members
  
    #region independent variable definition
    protected string[] _independentVariableName = new string[]{"w"};
    public int NumberOfIndependentVariables
    {
      get
      {
        return _independentVariableName.Length;
      }
    }
    public string IndependentVariableName(int i)
    {
      return _independentVariableName[i];
    }
    #endregion

    #region dependent variable definition
    private string[] _dependentVariableName = new string[]{"re","im"};
    public int NumberOfDependentVariables
    {
      get
      {
        return _dependentVariableName.Length;
      }
    }
    public string DependentVariableName(int i)
    {
      return _dependentVariableName[i];
    }
    #endregion

    #region parameter definition
    string[] _parameterName = new string[]{"offset","amplitude","tau","alpha","gamma"};
    public int NumberOfParameters
    {
      get
      {
        return _parameterName.Length;
      }
    }
    public string ParameterName(int i)
    {
      return _parameterName[i];
    }
    #endregion

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Complex result = P[0] + P[1]/ComplexMath.Pow(1+ComplexMath.Pow(Complex.I*X[0]*P[2],P[3]),P[4]);
      Y[0] = result.Re;
      Y[1] = -result.Im;
    }

    #endregion
  }

}
