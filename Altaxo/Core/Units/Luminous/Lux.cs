#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Units.Luminous
{
  /// <summary>
  /// Represents the SI illuminance unit lux.
  /// </summary>
  [UnitDescription("Illuminance", -2, 0, 0, 0, 0, 0, 1)]
  public class Lux : SIUnit
  {
    /// <summary>
    /// Gets the singleton instance of the <see cref="Lux"/> unit.
    /// </summary>
    public static Lux Instance { get; } = new();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Lux"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Lux), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Lux.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Lux"/> class.
    /// </summary>
    private Lux()
      : base(-2, 0, 0, 0, 0, 0, 1)
    {
    }

    /// <inheritdoc/>
    public override string Name
    {
      get { return "Lux"; }
    }

    /// <inheritdoc/>
    public override string ShortCut
    {
      get { return "lx"; }
    }

    /// <inheritdoc/>
    public override ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }
  }
}
