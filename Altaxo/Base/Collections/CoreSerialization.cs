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
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  internal class CoreSerialization
  {
    #region Serialization of ContiguousIntegerRange

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Collections.IntegerRangeAsCollection", 0)]
    private class XmlSerializationSurrogate00 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Should not serialize deprecated type");
        /*
                IntegerRangeAsCollection s = (IntegerRangeAsCollection)obj;
                info.AddValue("Start", s._start);
                info.AddValue("Count", s._count);
                */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return ContiguousIntegerRange.FromStartAndCount(start, count);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ContiguousIntegerRange), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ContiguousIntegerRange)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("Count", s.Count);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return ContiguousIntegerRange.FromStartAndCount(start, count);
      }
    }

    #endregion Serialization of ContiguousIntegerRange
  }
}
