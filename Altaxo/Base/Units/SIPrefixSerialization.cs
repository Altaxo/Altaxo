#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Units
{
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SIPrefix), 0)]
  public class SerializationSurrogate0_SIPrefix : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (SIPrefix)obj;

      info.AddValue("Exponent", s.Exponent);
      info.AddValue("Name", s.Name);
      info.AddValue("Shortcut", s.ShortCut);
    }

    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      var exponent = info.GetInt32("Exponent");
      var name = info.GetString("Name");
      var shortcut = info.GetString("Shortcut");

      if (SIPrefix.TryGetPrefixFromExponent(exponent, out var prefix))
        return prefix;
      else
        return new SIPrefix(name, shortcut, exponent);
    }
  }
}
