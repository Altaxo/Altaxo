#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds a bunch of <see cref="ParameterSetElement"/>, i.e. a collection of fit parameters together with their values.
  /// </summary>
  public class ParameterSet : ICloneable, IReadOnlyList<ParameterSetElement>
  {
    List<ParameterSetElement> _inner;

    /// <inheritdoc/>
    public int Count => _inner.Count;

    /// <summary>
    /// Event is fired if the main initialization is finished. This event can be fired
    /// multiple times (every time the set has changed basically.
    /// </summary>
    public event EventHandler? InitializationFinished;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSet), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ParameterSet)o;

        info.CreateArray("Parameters", s.Count);
        for (int i = 0; i < s.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ParameterSet s = o is not null ? (ParameterSet)o : new ParameterSet();

        int arraycount = info.OpenArray();
        for (int i = 0; i < arraycount; ++i)
          s.Add((ParameterSetElement)info.GetValue("e", s));
        info.CloseArray(arraycount);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class.
    /// </summary>
    public ParameterSet()
    {
      _inner = new List<ParameterSetElement>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">The parameter set elements.</param>
    public ParameterSet(IEnumerable<ParameterSetElement> elements)
    {
      _inner = new List<ParameterSetElement>(elements);
    }

    /// <summary>
    /// Raises the <see cref="InitializationFinished"/> event.
    /// </summary>
    public void OnInitializationFinished()
    {
      InitializationFinished?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public ParameterSetElement this[int i]
    {
      get
      {
        return _inner[i];
      }
      set
      {
        _inner[i] = value ?? throw new ArgumentNullException(nameof(ParameterSetElement));
      }
    }

    /// <summary>
    /// Adds a parameter element to the set.
    /// </summary>
    /// <param name="ele">The parameter element to add.</param>
    public void Add(ParameterSetElement ele)
    {
      _inner.Add(ele);
    }

    #region ICloneable Members

    /// <inheritdoc/>
    public object Clone()
    {
      var result = new ParameterSet();
      for (int i = 0; i < Count; ++i)
        result.Add(this[i]);

      return result;
    }

    /// <inheritdoc/>
    IEnumerator<ParameterSetElement> IEnumerable<ParameterSetElement>.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator GetEnumerator()
    {
      return _inner.GetEnumerator();
    }

    /// <summary>
    /// Removes all parameter elements from the set.
    /// </summary>
    internal void Clear()
    {
      _inner.Clear();
    }

    #endregion ICloneable Members
  }
}
