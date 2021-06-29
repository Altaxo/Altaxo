#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Main;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Bundles a <see cref="FitEnsemble"/>, i.e. a set of fit functions, together with the current parameters.
  /// </summary>
  public class NonlinearFitDocument
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ICloneable
  {
    private FitEnsemble _fitEnsemble;
    private ParameterSet _currentParameters;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonlinearFitDocument), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonlinearFitDocument)obj;

        info.AddValue("FitEnsemble", s._fitEnsemble);
        info.AddValue("Parameters", s._currentParameters);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        NonlinearFitDocument s = o is not null ? (NonlinearFitDocument)o : new NonlinearFitDocument();

        s._fitEnsemble = (FitEnsemble)info.GetValue("FitEnsemble", s);
        s._fitEnsemble.ParentObject = s;
        s._currentParameters = (ParameterSet)info.GetValue("Parameters", s);

        return s;
      }
    }

    #endregion Serialization

    public NonlinearFitDocument()
    {
      _fitEnsemble = new FitEnsemble() { ParentObject = this };
      _currentParameters = new ParameterSet();
    }

    public NonlinearFitDocument(NonlinearFitDocument from)
    {
      _fitEnsemble = ChildCloneFrom(from._fitEnsemble);
      _currentParameters = (ParameterSet)from._currentParameters.Clone();
      // Note that the fit context is not cloned here.
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

      var byName = new Dictionary<string, double>();
      for (int i = 0; i < _currentParameters.Count; i++)
        byName.Add(_currentParameters[i].Name, _currentParameters[i].Parameter);

      double[] result = new double[fitele.NumberOfParameters];
      for (int i = 0; i < result.Length; ++i)
      {
        result[i] = byName[fitele.ParameterName(i)];
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
      if (fitele.FitFunction is null)
        return;

      var byName = new Dictionary<string, int>();
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
      var byName = new Dictionary<string, ParameterSetElement>();
      for (int i = 0; i < _currentParameters.Count; i++)
        byName.Add(_currentParameters[i].Name, _currentParameters[i]);

      // now restore the values
      _currentParameters.Clear();

      for (int i = 0; i < _fitEnsemble.NumberOfParameters; i++)
      {
        string name = _fitEnsemble.ParameterName(i);
        if (byName.ContainsKey(name))
        {
          _currentParameters.Add((ParameterSetElement)byName[name]);
        }
        else
        {
          var newParameterSet = new ParameterSetElement(name)
          {
            Parameter = _fitEnsemble.DefaultParameterValue(i)
          };
          _currentParameters.Add(newParameterSet);
        }
      }

      _currentParameters.OnInitializationFinished();
    }

    #region ICloneable Members

    public object Clone()
    {
      return new NonlinearFitDocument(this);
    }

    #endregion ICloneable Members

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      RecalculateParameterSet();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling

    #region Document node functions

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_fitEnsemble is not null)
      {
        yield return new Main.DocumentNodeAndName(_fitEnsemble, "FitEnsemble");
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      _fitEnsemble?.VisitDocumentReferences(Report);
    }

    #endregion Document node functions
  }
}
