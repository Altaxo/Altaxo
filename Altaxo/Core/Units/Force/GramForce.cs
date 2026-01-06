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

namespace Altaxo.Units.Force
{
  /// <summary>
  /// Represents the unit gram-force.
  /// </summary>
  [UnitDescription("Force", 1, 1, -2, 0, 0, 0, 0)]
  public class GramForce : UnitBase, IUnit
  {
    private static readonly GramForce _instance = new GramForce();

    /// <summary>
    /// Gets the singleton instance of <see cref="GramForce"/>.
    /// </summary>
    public static GramForce Instance { get { return _instance; } }

    private const double _factorToSI = Science.SIConstants.GRAV_ACCEL / 1000d;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="GramForce"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GramForce), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return GramForce.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    protected GramForce()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "GramForce"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "gf"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x * _factorToSI;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x / _factorToSI;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Newton.Instance; }
    }
  }
}
