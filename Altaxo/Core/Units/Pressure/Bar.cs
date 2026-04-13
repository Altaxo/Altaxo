#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Units.Pressure
{
  /// <summary>
  /// Represents the bar unit of pressure.
  /// </summary>
  [UnitDescription("Pressure", -1, 1, -2, 0, 0, 0, 0)]
  public class Bar : UnitBase, IUnit
  {
    /// <summary>
    /// The number of pascals in one bar.
    /// </summary>
    public const double OneBarInPascal = 100000;

    /// <summary>
    /// Gets the singleton instance of the <see cref="Bar"/> unit.
    /// </summary>
    public static Bar Instance { get; } = new();

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Bar"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Bar), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Bar.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Bar"/> class.
    /// </summary>
    protected Bar()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Bar"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "bar"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x * OneBarInPascal;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x / OneBarInPascal;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.LisOfPrefixesWithMultipleOf3Exponent; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Pascal.Instance; }
    }
  }
}
