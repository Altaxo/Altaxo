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

namespace Altaxo.Data
{

  /// <summary>
  /// The indexer column is a simple readable numeric column. The value of an element is 
  /// it's index in the column, i.e. GetDoubleAt(i) simply returns the value i.
  /// </summary>
  [Serializable]
  public class IndexerColumn : INumericColumn, IReadableColumn, ICloneable
  {

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IndexerColumn),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        IndexerColumn s = null!=o ? (IndexerColumn)o : new IndexerColumn();
        return s;
      }
    }
    #endregion
    /// <summary>
    /// Creates a cloned instance of this object.
    /// </summary>
    /// <returns>The cloned instance of this object.</returns>
    public object Clone()
    {
      return new IndexerColumn();
    }
    /// <summary>
    /// Simply returns the value i.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>The index i.</returns>
    public double this[int i]
    {
      get
      {
        return i;
      }
    }

    /// <summary>
    /// This returns always true.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>Always true.</returns>
    public bool IsElementEmpty(int i)
    {
      return false;
    }

    /// <summary>
    /// Returns the index i as AltaxoVariant.
    /// </summary>
    AltaxoVariant IReadableColumn.this[int i] 
    {
      get 
      {
        return new AltaxoVariant((double)i);
      }
    } 

    /// <summary>
    /// The full name of a indexer column is "IndexerColumn".
    /// </summary>
    public string FullName
    {
      get { return "IndexerColumn"; }
    }

  }


}
