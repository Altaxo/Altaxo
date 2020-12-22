#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

#nullable enable

namespace Altaxo.Units.Mass
{
  [UnitDescription("Mass", 0, 1, 0, 0, 0, 0, 0)]
  public class Kilogram : SIUnit, IPrefixedUnit
  {
    private static readonly Kilogram _instance = new Kilogram();

    public static Kilogram Instance { get { return _instance; } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Kilogram), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Kilogram.Instance;
      }
    }
    #endregion



    private Kilogram()
        : base(0, 1, 0, 0, 0, 0, 0)
    {
    }

    public override string Name
    {
      get { return "Kilogram"; }
    }

    public override string ShortCut
    {
      get { return "kg"; }
    }

    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    #region IPrefixedUnit explicit implementation

    SIPrefix IPrefixedUnit.Prefix => SIPrefix.Kilo;

    IUnit IPrefixedUnit.Unit => Gram.Instance;

    #endregion IPrefixedUnit explicit implementation
  }
}
