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
using System.Collections.Generic;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds a bunch of <see cref="ParameterSetElement"/>, i.e. a collection of fit parameters together with their values.
  /// </summary>
  public class ParameterSet : System.Collections.CollectionBase, ICloneable, IEnumerable<ParameterSetElement>
  {
    /// <summary>
    /// Event is fired if the main initialization is finished. This event can be fired
    /// multiple times (every time the set has changed basically.
    /// </summary>
    public event EventHandler InitializationFinished;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSet), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ParameterSet)obj;

        info.CreateArray("Parameters", s.Count);
        for (int i = 0; i < s.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ParameterSet s = o != null ? (ParameterSet)o : new ParameterSet();

        int arraycount = info.OpenArray();
        for (int i = 0; i < arraycount; ++i)
          s.Add((ParameterSetElement)info.GetValue("e", s));
        info.CloseArray(arraycount);

        return s;
      }
    }

    #endregion Serialization

    public ParameterSet()
    {
    }

    public void OnInitializationFinished()
    {
      InitializationFinished?.Invoke(this, EventArgs.Empty);
    }

    public ParameterSetElement this[int i]
    {
      get
      {
        return (ParameterSetElement)InnerList[i];
      }
    }

    public void Add(ParameterSetElement ele)
    {
      InnerList.Add(ele);
    }

    #region ICloneable Members

    public object Clone()
    {
      var result = new ParameterSet();
      for (int i = 0; i < Count; ++i)
        result.Add((ParameterSetElement)this[i].Clone());

      return result;
    }

    IEnumerator<ParameterSetElement> IEnumerable<ParameterSetElement>.GetEnumerator()
    {
      foreach (var e in InnerList)
        yield return (ParameterSetElement)e;
    }

    #endregion ICloneable Members
  }
}
