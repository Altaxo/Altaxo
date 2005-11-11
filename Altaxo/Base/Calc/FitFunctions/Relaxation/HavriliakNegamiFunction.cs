using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunction("HavriliakNegami Complex","Relaxation",1,2,5)]
  public class HavriliakNegamiComplex : IFitFunction
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HavriliakNegamiComplex),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return new HavriliakNegamiComplex();;
      }
    }

    #endregion

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
    string[] _parameterName = new string[]{"offset","amplitude","tau","alpha","gamma","conduct"};
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
     // double b = P[3];
     // double g = P[4];
     // double tau = P[2] * Math.Pow(Math.Sin(b * Math.PI / (2 + 2 * g)), (1 / b)) * Math.Pow(Math.Sin(b * g * Math.PI / (2 + 2 * g)), (-1 / b));

      Complex result = P[0] + P[1]/ComplexMath.Pow(1+ComplexMath.Pow(Complex.I*X[0]*P[2],P[3]),P[4]);
      Y[0] = result.Re;
      Y[1] = -result.Im + P[5]/X[0];
    }

    #endregion
  }

}
