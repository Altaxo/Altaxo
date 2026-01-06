#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Units.ElectricResistivity
{
  /// <summary>
  /// The SI unit ohm for electric resistivity.
  /// </summary>
  [UnitDescription("Electric resistivity", 2, 1, -3, -2, 0, 0, 0)]
  public class Ohm : SIUnit
  {
    /// <summary>
    /// Gets the singleton instance of <see cref="Ohm"/>.
    /// </summary>
    public static Ohm Instance { get; } = new();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Ohm"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Ohm), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Ohm.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Prevents external instantiation. Use <see cref="Instance"/> to obtain the singleton.
    /// </summary>
    private Ohm()
        : base(2, 1, -3, -2, 0, 0, 0)
    {
    }

    /// <inheritdoc/>
    public override string Name
    {
      get { return "Ohm"; }
    }

    /// <inheritdoc/>
    public override string ShortCut
    {
      get { return "Ω"; }
    }

    /// <inheritdoc/>
    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }
  }
}

