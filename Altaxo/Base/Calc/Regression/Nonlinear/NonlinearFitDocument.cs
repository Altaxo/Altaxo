#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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


    /// <summary>
    /// This will set all parameters in the ensembly with the same name than that of the parameter names
    /// of fit function at index <c>idx</c> to their default values (those are provided by the fit function).
    /// </summary>
    /// <param name="idx">Index of the fit element.</param>
    public void SetDefaultParametersForFitElement(int idx)
    {
      FitElement fitele = _fitEnsemble[idx];
      if (fitele.FitFunction == null)
        return;


      System.Collections.Hashtable byName = new System.Collections.Hashtable();
      for (int i = 0; i < _currentParameters.Count; i++)
        byName.Add(_currentParameters[i].Name, i);
      
      for (int i = 0; i < fitele.NumberOfParameters; ++i)
      {
        int k = (int)byName[fitele.ParameterName(i)];
          _currentParameters[k].Parameter = fitele.FitFunction.DefaultParameterValue(i);
      }

      _currentParameters.OnInitializationFinished();
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
