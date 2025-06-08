#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2025 Dr. Dirk Lellinger
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

namespace Altaxo.Units.MassSpecificEntropy
{
  [UnitDescription("Mass specific entropy", 2, 0, -2, 0, -1, 0, 0)]
  public class JoulePerKilogramKelvin : SIUnit
  {
    public static JoulePerKilogramKelvin Instance { get; } = new();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(JoulePerKilogramKelvin), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return JoulePerKilogramKelvin.Instance;
      }
    }
    #endregion

    private JoulePerKilogramKelvin()
        : base(2, 0, -2, 0, -1, 0, 0)
    {
    }

    public override string Name
    {
      get { return "Joule per Kilogram and Kelvin"; }
    }

    public override string ShortCut
    {
      get { return "J/(kg·K)"; }
    }

    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }
  }
}
