#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Styles;

namespace Altaxo.Graph.Graph2D.Plot.Groups
{
  public class ScatterSymbolList : StyleListBase<IScatterSymbol>
  {
    /// <summary>First part of the key that is used during serialization to decide whether the set was already serialized before.</summary>
    private static readonly string _serializationRegistrationKey = typeof(ScatterSymbolList).FullName + " ";

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolList), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScatterSymbolList)obj;

        info.SetProperty(GetSerializationRegistrationKey(s), "True"); // Register a property to note that this color set is already serialized.

        info.AddValue("Name", s._name);
        info.CreateArray("Elements", s._list.Count);
        foreach (var ele in s)
          info.AddValue("e", ele.Clone()); // Trick here: by cloning the value, it is a new instance that is not registered in the list manager, thus it has no parent, so that neither parent set nor parent name are serialized
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        string name = info.GetString("Name");
        int count = info.OpenArray("Elements");
        var list = new List<IScatterSymbol>(count);
        for (int i = 0; i < count; ++i)
          list.Add((IScatterSymbol)info.GetValue("e", null));
        info.CloseArray(count);

        var result = new ScatterSymbolList(name, list);
        return result;
      }
    }

    #endregion Serialization

    public ScatterSymbolList(string name, IEnumerable<IScatterSymbol> symbols)
      : base(name, symbols.Select(instance => (IScatterSymbol)instance.Clone()))
    {
    }

    /// <summary>
    /// Gets a key that is used during serialization to decide whether or not the set was already serialized.
    /// Use the returned key to retrieve a string from the properties of the serialization info. If the returned property string
    /// is null, then the set needs to be serialized; otherwise, it was already serialized before.
    /// </summary>
    /// <param name="set">The set for which the property key should be evaluated.</param>
    /// <returns>The property key to be used to retrieve a property from the serialization info.</returns>
    public static string GetSerializationRegistrationKey(ScatterSymbolList set)
    {
      return _serializationRegistrationKey + set.Name;
    }
  }
}
