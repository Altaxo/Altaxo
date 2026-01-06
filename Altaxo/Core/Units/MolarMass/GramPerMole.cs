#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Units.MolarMass
{
  /// <summary>
  /// Represents gram per mole for molar mass.
  /// </summary>
  [UnitDescription("Molar mass", 0, 1, 0, 0, 0, -1, 0)]
  public class GramPerMole : UnitBase, IUnit
  {
    /// <summary>
    /// Gets the singleton instance of <see cref="GramPerMole"/>.
    /// </summary>
    public static GramPerMole Instance { get; } = new();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="GramPerMole"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GramPerMole), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return GramPerMole.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    private GramPerMole()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "GramPerMole"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "g/mol"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x / 1000;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x * 1000;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithAllKnownPrefixes; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return KilogramPerMole.Instance; }
    }
  }
}
