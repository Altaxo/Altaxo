using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Summary description for NonlinearFitDocument.
  /// </summary>
  public class NonlinearFitDocument : ICloneable
  {
    FitEnsemble _fitEnsemble;
    ParameterSet _currentParameters;
    object _fitContext; 

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonlinearFitDocument),0)]
    public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        NonlinearFitDocument s = (NonlinearFitDocument)obj;

        info.AddValue("FitEnsemble",s._fitEnsemble);
        info.AddValue("Parameters",s._currentParameters);

      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        NonlinearFitDocument s = o!=null ? (NonlinearFitDocument)o : new NonlinearFitDocument();

        s._fitEnsemble = (FitEnsemble)info.GetValue("FitEnsemble",s);
        s._fitEnsemble.Changed += new EventHandler(s.EhFitEnsemble_Changed);
        s._currentParameters = (ParameterSet)info.GetValue("Parameters",s);

        return s;
      }
    }

    #endregion


    public NonlinearFitDocument()
    {
      _fitEnsemble = new FitEnsemble();
      _currentParameters = new ParameterSet();
      _fitEnsemble.Changed += new EventHandler(EhFitEnsemble_Changed);
    }

    public NonlinearFitDocument(NonlinearFitDocument from)
    {
      _fitEnsemble = null == from._fitEnsemble ? null : (FitEnsemble)from._fitEnsemble.Clone();
      _fitEnsemble.Changed += new EventHandler(EhFitEnsemble_Changed);
      _currentParameters = null == from._currentParameters ? null : (ParameterSet)from._currentParameters.Clone();
      // Note that the fit context is not cloned here.
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

    public double[] GetParametersForFitElement(int idx)
    {
      FitElement fitele = _fitEnsemble[idx];

      System.Collections.Hashtable byName = new System.Collections.Hashtable();
      for(int i=0;i<_currentParameters.Count;i++)
        byName.Add(_currentParameters[i].Name,_currentParameters[i].Parameter);
      
      double[] result = new double[fitele.NumberOfParameters];
      for(int i=0;i<result.Length;++i)
      {
        result[i] = (double)byName[fitele.ParameterName(i)];
      }

      return result;
    }

    private void RecalculateParameterSet()
    {
      // save old values
      System.Collections.Hashtable byName = new System.Collections.Hashtable();
      for(int i=0;i<_currentParameters.Count;i++)
        byName.Add(_currentParameters[i].Name,_currentParameters[i]);

      // now restore the values
      _currentParameters.Clear();

      for(int i=0;i<_fitEnsemble.NumberOfParameters;i++)
      {
        string name = _fitEnsemble.ParameterName(i);
        if(byName.ContainsKey(name))
          _currentParameters.Add((ParameterSetElement)byName[name]);
        else
          _currentParameters.Add(new ParameterSetElement(name));

      }

      _currentParameters.OnInitializationFinished();
    }


    private void EhFitEnsemble_Changed(object sender, EventArgs e)
    {
      
      RecalculateParameterSet();
    }

    #region ICloneable Members

    public object Clone()
    {
      return new NonlinearFitDocument(this);
    }

    #endregion
}
}
