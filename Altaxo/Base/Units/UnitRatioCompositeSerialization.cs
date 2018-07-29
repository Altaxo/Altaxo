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
using System.Threading.Tasks;

namespace Altaxo.Units
{
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UnitRatioComposite), 0)]
  public class SerializationSurrogate0_UnitRatioComposite : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (UnitRatioComposite)obj;

      info.AddValue("NominatorPrefix", s.NominatorPrefix);
      info.AddValue("NominatorUnit", s.NominatorUnit);
      info.AddValue("DenominatorPrefix", s.DenominatorPrefix);
      info.AddValue("DenominatorUnit", s.DenominatorUnit);
    }

    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      var nomPrefix = (SIPrefix)info.GetValue("NominatorPrefix", null);
      var nomUnit = (IUnit)info.GetValue("NominatorUnit", null);
      var denomPrefix = (SIPrefix)info.GetValue("DenominatorPrefix", null);
      var denomUnit = (IUnit)info.GetValue("DenominatorUnit", null);

      return new UnitRatioComposite(nomPrefix, nomUnit, denomPrefix, denomUnit);
    }
  }
}
