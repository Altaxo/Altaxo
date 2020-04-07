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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Main;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds a collection of <see cref="FitElement" />s and is responsible for parameter bundling.
  /// </summary>
  /// <remarks>The number of parameters in a FitEnsemble is less than or equal to the sum of the number of parameters of all FitElements bundeled in this instance.
  /// (It is less than the sum of parameters if some parameters of different fit elements have equal names).</remarks>
  public class FitEnsemble
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IList<FitElement>,
    ICloneable
  {
    /// <summary>
    /// Current parameter names
    /// </summary>
    private string[] _parameterNames = new string[0];

    /// <summary>
    /// All parameters of the fit ensemble sorted by name. Key is the parameter name. Value is the position of the parameter.
    /// </summary>
    private SortedList<string, int> _parametersSortedByName = new SortedList<string, int>();

    private List<FitElement> _fitElements = new List<FitElement>();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitEnsemble), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitEnsemble)obj;

        info.CreateArray("FitElements", s._fitElements.Count);
        for (int i = 0; i < s._fitElements.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitEnsemble s = o != null ? (FitEnsemble)o : new FitEnsemble();

        int arraycount = info.OpenArray();
        for (int i = 0; i < arraycount; ++i)
          s.Add((FitElement)info.GetValue("e", s));
        info.CloseArray(arraycount);

        s.CollectParameterNames();

        return s;
      }
    }

    #endregion Serialization

    #region Fit parameters

    protected void CollectParameterNames()
    {
      _parametersSortedByName.Clear();

      int nameposition = 0;
      for (int i = 0; i < _fitElements.Count; ++i)
      {
        FitElement ele = this[i];
        if (null != ele.FitFunction)
        {
          for (int k = 0; k < ele.NumberOfParameters; ++k)
          {
            var parameterName = ele.ParameterName(k);

            if (!(_parametersSortedByName.ContainsKey(parameterName)))
            {
              _parametersSortedByName.Add(parameterName, nameposition++);
            }
          }
        }
      }

      // now sort the items in the order of the namepositions
      var sortedbypos = new SortedList<int, string>();
      foreach (KeyValuePair<string, int> en in _parametersSortedByName)
        sortedbypos.Add(en.Value, en.Key);

      _parameterNames = new string[sortedbypos.Count];
      for (int i = 0; i < _parameterNames.Length; i++)
        _parameterNames[i] = sortedbypos[i];
    }

    public string ParameterName(int i)
    {
      return _parameterNames[i];
    }

    public int NumberOfParameters
    {
      get
      {
        return _parameterNames.Length;
      }
    }

    #endregion Fit parameters

    #region ICloneable Members

    public FitEnsemble()
    {
    }

    public FitEnsemble(FitEnsemble from)
    {
      foreach (var ele in from)
      {
        var toadd = (FitElement)ele.Clone();
        toadd.ParentObject = this;
        _fitElements.Add(toadd);
      }

      CollectParameterNames();
    }

    public object Clone()
    {
      return new FitEnsemble(this);
    }

    #endregion ICloneable Members

    #region IList members

    public FitElement this[int i]
    {
      get
      {
        return _fitElements[i];
      }
      set
      {
        if (!object.ReferenceEquals(_fitElements[i], value))
        {
          var tempFitElement = _fitElements[i];
          ChildSetMember(ref tempFitElement, value);
          _fitElements[i] = tempFitElement;

          CollectParameterNames();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public void Add(FitElement e)
    {
      e.ParentObject = this;
      _fitElements.Add(e);

      CollectParameterNames();
      EhSelfChanged(EventArgs.Empty);
    }

    public void Clear()
    {
      if (_fitElements.Count > 0)
      {
        foreach (var ele in _fitElements)
          ele.Dispose();
        _fitElements.Clear();
        CollectParameterNames();
        EhSelfChanged(EventArgs.Empty);
      }
    }

    public bool Contains(FitElement item)
    {
      return _fitElements.Contains(item);
    }

    public void CopyTo(FitElement[] array, int arrayIndex)
    {
      _fitElements.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _fitElements.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(FitElement item)
    {
      var success = _fitElements.Remove(item);
      if (success)
      {
        item?.Dispose();
        CollectParameterNames();
        EhSelfChanged(EventArgs.Empty);
      }

      return success;
    }

    public IEnumerator<FitElement> GetEnumerator()
    {
      return _fitElements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _fitElements.GetEnumerator();
    }

    public int IndexOf(FitElement item)
    {
      return _fitElements.IndexOf(item);
    }

    public void Insert(int index, FitElement item)
    {
      item.ParentObject = this;
      _fitElements.Insert(index, item);
      CollectParameterNames();
      EhSelfChanged(EventArgs.Empty);
    }

    public void RemoveAt(int index)
    {
      var tempFitElement = _fitElements[index];
      _fitElements.RemoveAt(index);
      tempFitElement?.Dispose();
      CollectParameterNames();
      EhSelfChanged(EventArgs.Empty);
    }

    #endregion IList members

    #region Changed handling

    protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
    {
      CollectParameterNames();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed handling

    #region Document node functions

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _fitElements)
      {
        for (int i = 0; i < _fitElements.Count; ++i)
        {
          if (null != _fitElements[i])
            yield return new Main.DocumentNodeAndName(_fitElements[i], "FitElement" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      if (_fitElements is { } fitElements)
      {
        for (int i = 0; i < fitElements.Count; ++i)
        {
          fitElements[i]?.VisitDocumentReferences(Report);
        }
      }
    }

    #endregion Document node functions
  }
}
