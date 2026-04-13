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
using System.Collections;
using System.Collections.Generic;
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

    private double[] _defaultParameterValues = new double[0];

    /// <summary>
    /// All parameters of the fit ensemble sorted by name. Key is the parameter name. Value is the position of the parameter.
    /// </summary>
    private SortedList<string, (int Position, double DefaultValue)> _parametersSortedByName = new SortedList<string, (int Position, double DefaultValue)>();
    private List<FitElement> _fitElements = new List<FitElement>();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitEnsemble), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitEnsemble)o;

        info.CreateArray("FitElements", s._fitElements.Count);
        for (int i = 0; i < s._fitElements.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        FitEnsemble s = o is not null ? (FitEnsemble)o : new FitEnsemble();

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

    /// <summary>
    /// Rebuilds the bundled parameter list from the contained fit elements.
    /// </summary>
    protected void CollectParameterNames()
    {
      _parametersSortedByName.Clear();

      int nameposition = 0;
      for (int i = 0; i < _fitElements.Count; ++i)
      {
        FitElement ele = this[i];
        if (ele.FitFunction is not null)
        {
          for (int k = 0; k < ele.NumberOfParameters; ++k)
          {
            var parameterName = ele.ParameterName(k);

            if (!string.IsNullOrEmpty(parameterName) && !(_parametersSortedByName.ContainsKey(parameterName)))
            {
              _parametersSortedByName.Add(parameterName, (nameposition++, ele.FitFunction.DefaultParameterValue(k)));
            }
          }
        }
      }

      // now sort the items in the order of the namepositions
      var sortedbypos = new SortedList<int, (string Name, double DefaultValue)>();
      foreach (var en in _parametersSortedByName)
        sortedbypos.Add(en.Value.Position, (en.Key, en.Value.DefaultValue));

      _parameterNames = new string[sortedbypos.Count];
      _defaultParameterValues = new double[sortedbypos.Count];
      for (int i = 0; i < _parameterNames.Length; i++)
      {
        _parameterNames[i] = sortedbypos[i].Name;
        _defaultParameterValues[i] = sortedbypos[i].DefaultValue;
      }
    }

    /// <summary>
    /// Gets the bundled parameter name at the specified index.
    /// </summary>
    /// <param name="i">The parameter index.</param>
    /// <returns>The bundled parameter name at the specified index.</returns>
    public string ParameterName(int i)
    {
      return _parameterNames[i];
    }

    /// <summary>
    /// Gets the default value of the bundled parameter at the specified index.
    /// </summary>
    /// <param name="i">The parameter index.</param>
    /// <returns>The default value of the bundled parameter at the specified index.</returns>
    public double DefaultParameterValue(int i)
    {
      return _defaultParameterValues[i];
    }

    /// <summary>
    /// Gets the number of bundled parameters.
    /// </summary>
    public int NumberOfParameters
    {
      get
      {
        return _parameterNames.Length;
      }
    }

    #endregion Fit parameters

    #region ICloneable Members

    /// <summary>
    /// Initializes a new instance of the <see cref="FitEnsemble"/> class.
    /// </summary>
    public FitEnsemble()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitEnsemble"/> class by cloning another ensemble.
    /// </summary>
    /// <param name="from">The ensemble to clone.</param>
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

    /// <inheritdoc/>
    public object Clone()
    {
      return new FitEnsemble(this);
    }

    #endregion ICloneable Members

    #region IList members

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Add(FitElement item)
    {
      item.ParentObject = this;
      _fitElements.Add(item);

      CollectParameterNames();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool Contains(FitElement item)
    {
      return _fitElements.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(FitElement[] array, int arrayIndex)
    {
      _fitElements.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public int Count
    {
      get { return _fitElements.Count; }
    }

    /// <inheritdoc/>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public IEnumerator<FitElement> GetEnumerator()
    {
      return _fitElements.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _fitElements.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(FitElement item)
    {
      return _fitElements.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, FitElement item)
    {
      item.ParentObject = this;
      _fitElements.Insert(index, item);
      CollectParameterNames();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      CollectParameterNames();

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed handling

    #region Document node functions

    /// <inheritdoc/>
    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_fitElements is not null)
      {
        for (int i = 0; i < _fitElements.Count; ++i)
        {
          if (_fitElements[i] is not null)
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
