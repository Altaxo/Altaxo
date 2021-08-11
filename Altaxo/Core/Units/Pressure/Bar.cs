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

#nullable enable

namespace Altaxo.Units.Pressure
{
  [UnitDescription("Pressure", -1, 1, -2, 0, 0, 0, 0)]
  public class Bar : UnitBase, IUnit
  {
    public const double OneBarInPascal = 100000;

    public static Bar Instance { get; } = new();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Bar), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Bar.Instance;
      }
    }
    #endregion

    protected Bar()
    {
    }

    public string Name
    {
      get { return "Bar"; }
    }

    public string ShortCut
    {
      get { return "bar"; }
    }

    public double ToSIUnit(double x)
    {
      return x * OneBarInPascal;
    }

    public double FromSIUnit(double x)
    {
      return x / OneBarInPascal;
    }

    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.LisOfPrefixesWithMultipleOf3Exponent; }
    }

    public SIUnit SIUnit
    {
      get { return Pascal.Instance; }
    }
  }
}
