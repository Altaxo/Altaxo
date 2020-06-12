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

namespace Altaxo.Collections
{
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Collections.ArrayList), 0)]
  public class SystemCollectionsArrayListXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (ArrayList)obj;
      info.CreateArray("List", s.Count);

      for (int i = 0; i < s.Count; ++i)
        info.AddValueOrNull("e", s[i]);

      info.CommitArray();
    }

    public object Deserialize(object? obj, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      int count = info.OpenArray("List");
      var s = (ArrayList?)obj ?? new ArrayList(count);
      s.Clear();
      for (int i = 0; i < count; ++i)
        s.Add(info.GetValueOrNull("e", parent));
      info.CloseArray(count);

      return s;
    }
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Collections.Generic.List<object?>), 0)]
  public class SystemCollectionsListOfObjectListXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (System.Collections.Generic.List<object?>)obj;
      info.CreateArray("List", s.Count);

      for (int i = 0; i < s.Count; ++i)
        info.AddValueOrNull("e", s[i]);

      info.CommitArray();
    }

    public object Deserialize(object? obj, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      int count = info.OpenArray("List");
      var s = (System.Collections.Generic.List<object?>?)obj ?? new System.Collections.Generic.List<object?>(count);
      s.Clear();
      for (int i = 0; i < count; ++i)
        s.Add(info.GetValueOrNull("e", parent));
      info.CloseArray(count);

      return s;
    }
  }
}
