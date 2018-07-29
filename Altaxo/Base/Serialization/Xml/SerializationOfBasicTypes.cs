#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Contains serialization surrogate classes for serialization of basic types from the System namespace. These serialization surrogates are only needed when the type to serialize is unknown, for instance the entries in an untyped set.
  /// Otherwise, it is preferable to use the special functions of <see cref="IXmlSerializationInfo"/> to serialize and of <see cref="IXmlDeserializationInfo"/> to deserialize those types.
  /// </summary>
  internal static class SerializationOfBasicTypes
  {
    /// <summary>
    /// 2015-06-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(string), 0)]
    private class XmlSerializationSurrogateForSystemString : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (string)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetString("e");
      }
    }

    /// <summary>
    /// 2015-06-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(double), 0)]
    private class XmlSerializationSurrogateForSystemDouble : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (double)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetDouble("e");
      }
    }

    /// <summary>
    /// 2015-06-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Int32), 0)]
    private class XmlSerializationSurrogateForSystemInt32 : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Int32)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetInt32("e");
      }
    }

    /// <summary>
    /// 2015-06-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Int64), 0)]
    private class XmlSerializationSurrogateForSystemInt64 : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Int64)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetInt64("e");
      }
    }

    /// <summary>
    /// 2015-06-30 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTime), 0)]
    private class XmlSerializationSurrogateForSystemDateTime : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DateTime)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetDateTime("e");
      }
    }

    /// <summary>
    /// 2018-04-11 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Boolean), 0)]
    private class XmlSerializationSurrogateForBoolean : IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Boolean)obj;
        info.AddValue("e", s);
      }

      public object Deserialize(object o, IXmlDeserializationInfo info, object parentobject)
      {
        return info.GetBoolean("e");
      }
    }
  }
}
