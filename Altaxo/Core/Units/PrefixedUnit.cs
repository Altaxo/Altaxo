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

namespace Altaxo.Units
{
  /// <summary>
  /// Represents a prefixed unit, i.e. a unit together with an SI prefix.
  /// </summary>
  /// <seealso cref="Altaxo.Units.IPrefixedUnit" />
  public struct PrefixedUnit : IPrefixedUnit
  {
    private IUnit _unit;
    private SIPrefix _prefix;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PrefixedUnit), 0)]
    public class SerializationSurrogate0_PrefixUnit : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PrefixedUnit)obj;

        info.AddValue("Prefix", s.Prefix);
        info.AddValue("Unit", s.Unit);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var prefix = (SIPrefix)info.GetValue("Prefix", parent);
        var unit = (IUnit)info.GetValue("Unit", parent);

        return new PrefixedUnit(prefix, unit);
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="PrefixedUnit"/> struct (with <see cref="SIPrefix.None"/> as prefix).
    /// </summary>
    /// <param name="unit">The unit.</param>
    public PrefixedUnit(IUnit unit) : this(SIPrefix.None, unit) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrefixedUnit"/> struct.
    /// </summary>
    /// <param name="prefix">The SI prefix.</param>
    /// <param name="unit">The unit.</param>
    public PrefixedUnit(SIPrefix prefix, IUnit unit)
    {
      prefix ??= SIPrefix.None;
      unit ??= Units.Dimensionless.Unity.Instance;

      if (unit is IPrefixedUnit punit)
      {
        if (punit.Unit is IPrefixedUnit)
          throw new ArgumentException("Multiple nesting of IPrefixedUnit is not supported", nameof(unit));

        (var newPrefix, var factor) = SIPrefix.FromMultiplication(prefix, punit.Prefix);
        if (1 != factor)
          throw new ArgumentException(string.Format("Can not combine prefix {0} with prefix {1} to a new prefix without additional factor", prefix.Name, punit.Prefix.Name));

        _unit = punit.Unit;
        _prefix = newPrefix;
      }
      else
      {
        _prefix = prefix;
        _unit = unit;
      }
    }

    /// <summary>
    /// Gets the unit.
    /// </summary>
    /// <value>
    /// The unit.
    /// </value>
    public IUnit Unit { get { return _unit ?? Units.Dimensionless.Unity.Instance; } }

    /// <summary>
    /// Gets the prefix.
    /// </summary>
    /// <value>
    /// The prefix.
    /// </value>
    public SIPrefix Prefix { get { return _prefix ?? SIPrefix.None; } }

    /// <summary>
    /// Converts to a string consisting of prefix and unit.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Prefix.ShortCut + Unit.ShortCut;
    }
  }
}
