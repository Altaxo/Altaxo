#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.ColorManagement
{
  /// <summary>
  /// Immutable identifier that stores the <see cref="Altaxo.Main.ItemDefinitionLevel"/> and name of a color set.
  /// It is used as a key value in internal dictionaries.
  /// </summary>
  [System.ComponentModel.ImmutableObject(true)]
  public class ColorSetIdentifier : IEquatable<ColorSetIdentifier>, IComparable<ColorSetIdentifier>, Main.IImmutable
  {
    private Altaxo.Main.ItemDefinitionLevel _level;
    private string _name;

    #region Serialization

    /// <summary>
    /// 2015-11-14 Version 1 moved to Altaxo.Drawing.ColorManagement.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ColorManagement.ColorSetIdentifier", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorSetIdentifier), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColorSetIdentifier)o;
        info.AddEnum("Level", s.Level);
        info.AddValue("Name", s.Name);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var colorSetLevel = (Altaxo.Main.ItemDefinitionLevel)info.GetEnum("Level", typeof(Altaxo.Main.ItemDefinitionLevel));
        string colorSetName = info.GetString("Name");
        return new ColorSetIdentifier(colorSetLevel, colorSetName);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorSetIdentifier"/> class.
    /// </summary>
    /// <param name="colorSetLevel">The definition level of the color set.</param>
    /// <param name="colorSetName">The color set name.</param>
    public ColorSetIdentifier(Altaxo.Main.ItemDefinitionLevel colorSetLevel, string colorSetName)
    {
      if (string.IsNullOrEmpty(colorSetName))
        throw new ArgumentOutOfRangeException("colorSetName is null or is empty");

      _level = colorSetLevel;
      _name = colorSetName;
    }

    /// <summary>
    /// Gets the color set level.
    /// </summary>
    /// <value>
    /// The level of the color set.
    /// </value>
    public Altaxo.Main.ItemDefinitionLevel Level { get { return _level; } }

    /// <summary>
    /// Gets the name of the color set.
    /// </summary>
    /// <value>
    /// The name of the color set.
    /// </value>
    public string Name { get { return _name; } }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _level.GetHashCode() * 29 + _name.GetHashCode() * 31;
    }

    /// <inheritdoc/>
    public bool Equals(ColorSetIdentifier? other)
    {
      return other is not null && _level == other._level && 0 == string.Compare(_name, other._name);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is ColorSetIdentifier other ? _level == other._level && 0 == string.Compare(_name, other._name) : false;
    }

    /// <inheritdoc/>
    public int CompareTo(ColorSetIdentifier? other)
    {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      int result;
      result = Comparer<int>.Default.Compare((int)_level, (int)other._level);
      return 0 != result ? result : string.Compare(_name, other._name);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("{0} ({1})", _name, _level);
    }
  }
}
