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

using System;

#nullable enable

namespace Altaxo.Units.Luminous
{
  /// <summary>
  /// Represents the SI unit lumen used for luminous flux.
  /// </summary>
  [UnitDescription("Luminous flux", 0, 0, 0, 0, 0, 0, 1)]
  public class Lumen : UnitBase, IUnit
  {
    private static readonly SIPrefixList _prefixes = new([SIPrefix.None, SIPrefix.Milli, SIPrefix.Micro]);

    private static readonly Lumen _instance = new();

    /// <summary>
    /// Gets the singleton instance of the <see cref="Lumen"/> unit.
    /// </summary>
    public static Lumen Instance { get { return _instance; } }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Lumen"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Lumen), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Lumen.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Lumen"/> class.
    /// </summary>
    protected Lumen()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Lumen"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "lm"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      // lumen is candela * steradian; SI unit for luminous flux is lumen (dimension: cd·sr) - treat as ratio to candela
      return x / (4 * Math.PI); // Candela = Lumen / Steradian  
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x * 4 * Math.PI; // Candela * Steradian
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return _prefixes; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Candela.Instance; }
    }
  }
}
