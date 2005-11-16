using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Havriliak-Negami function to fit dielectric spectra.
  /// </summary>
  [FitFunction("HavriliakNegami Complex","Relaxation",1,2,5)]
  public class HavriliakNegamiComplex : IFitFunctionWithGradient
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
      Complex result = P[0] + P[1]/ComplexMath.Pow(1+ComplexMath.Pow(Complex.I*X[0]*P[2],P[3]),P[4]);
      Y[0] = result.Re;
      Y[1] = -result.Im + P[5]/X[0];
    }

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      DY[0][0] = 1;
      DY[1][0] = 0;

      Complex OneByDenom = 1 / ComplexMath.Pow(1 + ComplexMath.Pow(Complex.I * X[0] * P[2], P[3]), P[4]);
      DY[0][1] = OneByDenom.Re;
      DY[1][1] = -OneByDenom.Im;
      Complex IXP2 = Complex.I * X[0] * P[2];
      Complex IXP2PowP3 = ComplexMath.Pow(IXP2, P[3]);
      Complex der2 = OneByDenom * -P[1] * P[2] * P[4] * IXP2PowP3 / (P[2] * (1 + IXP2PowP3));
      DY[0][2] = der2.Re;
      DY[1][2] = -der2.Im;
      Complex der3 = OneByDenom * -P[1] * P[4] * IXP2PowP3 * ComplexMath.Log(IXP2) / (1 + IXP2PowP3);
      DY[0][3] = der3.Re;
      DY[1][3] = -der3.Im;
      Complex der4 = OneByDenom * -P[1] * ComplexMath.Log(1 + IXP2PowP3);
      DY[0][4] = der4.Re;
      DY[1][4] = -der4.Im;

      DY[0][5] = 0;
      DY[1][5] = 1 / X[0];




    }
    #endregion
  }

}
