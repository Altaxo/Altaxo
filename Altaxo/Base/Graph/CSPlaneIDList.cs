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
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Represents an immutable read-only list of <see cref="CSPlaneID"/> instances.
  /// </summary>
  public class CSPlaneIDList : IReadOnlyList<CSPlaneID>
  {
    private List<CSPlaneID> _innerList = new List<CSPlaneID>();

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CSPlaneIDList), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CSPlaneIDList)obj;
        info.CreateArray("PlaneIDs", s.Count);
        foreach (CSPlaneID plane in s)
          info.AddValue("e", plane);
        info.CommitArray();
      }

      protected virtual CSPlaneIDList SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = new CSPlaneIDList(info);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        CSPlaneIDList s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    private CSPlaneIDList(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _innerList = new List<CSPlaneID>();
      int count = info.OpenArray("PlaneIDs");
      for (int i = count; i > 0; i--)
        _innerList.Add((CSPlaneID)info.GetValue("e", null));
      info.CloseArray(count);
    }

    #endregion Serialization

    #region Constructors

    private CSPlaneIDList()
    {
      _innerList = new List<CSPlaneID>();
    }

    /// <summary>
    /// Gets the empty plane identifier list.
    /// </summary>
    public static CSPlaneIDList Empty { get; private set; }

    static CSPlaneIDList()
    {
      Empty = new CSPlaneIDList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CSPlaneIDList"/> class.
    /// </summary>
    /// <param name="ids">The plane identifiers to include.</param>
    public CSPlaneIDList(IEnumerable<CSPlaneID> ids)
    {
      _innerList = new List<CSPlaneID>(ids);
    }

    #endregion Constructors

    #region IList<CSPlaneID> Members

    /// <summary>
    /// Returns the index of the specified plane identifier.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based index of the item, or -1 if the item was not found.</returns>
    public int IndexOf(CSPlaneID item)
    {
      return _innerList.IndexOf(item);
    }

    /// <inheritdoc />
    public CSPlaneID this[int index]
    {
      get
      {
        return _innerList[index];
      }
    }

    #endregion IList<CSPlaneID> Members

    #region ICollection<CSPlaneID> Members

    /// <summary>
    /// Determines whether the specified plane identifier is contained in the list.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><c>true</c> if the item is contained in the list; otherwise, <c>false</c>.</returns>
    public bool Contains(CSPlaneID item)
    {
      return _innerList.Contains(item);
    }

    /// <summary>
    /// Copies the list contents to an array.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based destination index.</param>
    public void CopyTo(CSPlaneID[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _innerList.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether this list is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return true; }
    }

    #endregion ICollection<CSPlaneID> Members

    #region IEnumerable<CSPlaneID> Members

    /// <inheritdoc />
    public IEnumerator<CSPlaneID> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable<CSPlaneID> Members

    #region IEnumerable Members

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}
