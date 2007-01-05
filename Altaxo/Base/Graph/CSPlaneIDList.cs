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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public class CSPlaneIDList : IList<CSPlaneID>
  {
    List<CSPlaneID> _innerList = new List<CSPlaneID>();

    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CSPlaneIDList), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        CSPlaneIDList s = (CSPlaneIDList)obj;
        info.CreateArray("PlaneIDs", s.Count);
        foreach (CSPlaneID plane in s)
          info.AddValue("e", plane);
        info.CommitArray();
      }
      protected virtual CSPlaneIDList SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        CSPlaneIDList s = (o == null ? new CSPlaneIDList() : (CSPlaneIDList)o);

        int count = info.OpenArray("PlaneIDs");
        for(int i=count;i>0;i--)
          s.Add((CSPlaneID)info.GetValue("e",s));
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        CSPlaneIDList s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion

    public void AddClonedRange(IEnumerable<CSPlaneID> list)
    {
      foreach (CSPlaneID id in list)
        Add(id.Clone());
    }

    #region IList<CSPlaneID> Members

    public int IndexOf(CSPlaneID item)
    {
      return _innerList.IndexOf(item);
    }

    public void Insert(int index, CSPlaneID item)
    {
      _innerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      _innerList.RemoveAt(index);
    }

    public CSPlaneID this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        _innerList[index] = value;
      }
    }

    #endregion

    #region ICollection<CSPlaneID> Members

    public void Add(CSPlaneID item)
    {
      _innerList.Add(item);
    }

    public void Clear()
    {
      _innerList.Clear();
    }

    public bool Contains(CSPlaneID item)
    {
      return _innerList.Contains(item);
    }

    public void CopyTo(CSPlaneID[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _innerList.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(CSPlaneID item)
    {
      return _innerList.Remove(item);
    }

    #endregion

    #region IEnumerable<CSPlaneID> Members

    public IEnumerator<CSPlaneID> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();

    }

    #endregion
  }
}
