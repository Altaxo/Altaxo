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

namespace Altaxo.Units.MolarEntropy
{
  [UnitDescription("Molar entropy", 2, 1, -2, 0, -1, -1, 0)]
  public class JoulePerMoleKelvin : SIUnit
  {
    public static JoulePerMoleKelvin Instance { get; } = new();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(JoulePerMoleKelvin), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return JoulePerMoleKelvin.Instance;
      }
    }
    #endregion

    private JoulePerMoleKelvin()
        : base(2, 1, -2, 0, -1, -1, 0)
    {
    }

    public override string Name
    {
      get { return "JoulePerMoleKelvin"; }
    }

    public override string ShortCut
    {
      get { return "J/(mol·K)"; }
    }

    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }
  }
}
