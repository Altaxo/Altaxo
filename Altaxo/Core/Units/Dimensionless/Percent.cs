#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Units.Dimensionless
{
  /// <summary>
  /// Represents percent as a dimensionless unit (ratio).
  /// </summary>
  [UnitDescription("Relation", 0, 0, 0, 0, 0, 0, 0)]
  public class Percent : UnitBase, IUnit
  {
    private static readonly Percent _instance = new Percent();

    /// <summary>
    /// Gets the singleton instance of <see cref="Percent"/>.
    /// </summary>
    public static Percent Instance { get { return _instance; } }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Percent"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Percent), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Percent.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    protected Percent()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Percent"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "%"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x / 100;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x * 100;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Unity.Instance; }
    }
  }
}
