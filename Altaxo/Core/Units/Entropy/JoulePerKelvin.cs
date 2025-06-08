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

namespace Altaxo.Units.Entropy
{
  [UnitDescription("Entropy", 2, 1, -2, 0, -1, 0, 0)]
  public class JoulePerKelvin : SIUnit
  {
    public static JoulePerKelvin Instance { get; } = new();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(JoulePerKelvin), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return JoulePerKelvin.Instance;
      }
    }
    #endregion

    private JoulePerKelvin()
        : base(2, 1, -2, 0, -1, 0, 0)
    {
    }

    public override string Name
    {
      get { return "JoulePerKelvin"; }
    }

    public override string ShortCut
    {
      get { return "J/K"; }
    }

    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }
  }
}
