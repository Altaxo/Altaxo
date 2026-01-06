#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

namespace Altaxo.Units.Temperature
{
  /// <summary>
  /// Represents degrees Fahrenheit temperature unit.
  /// </summary>
  [UnitDescription("Temperature", 0, 0, 0, 0, 1, 0, 0)]
  public class DegreesFahrenheit : UnitBase, IUnit, IBiasedUnit
  {
    /// <summary>
    /// Offset used for Fahrenheit to Kelvin conversion.
    /// </summary>
    private const double KelvinOffset = 459.67;

    private static readonly DegreesFahrenheit _instance = new DegreesFahrenheit();

    /// <summary>
    /// Gets the singleton instance of <see cref="DegreesFahrenheit"/>.
    /// </summary>
    public static DegreesFahrenheit Instance { get { return _instance; } }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="DegreesFahrenheit"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DegreesFahrenheit), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return DegreesFahrenheit.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    protected DegreesFahrenheit()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "DegreesFahrenheit"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "°F"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return (x + KelvinOffset) * 5 / 9.0;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x * 9 / 5.0 - KelvinOffset;
    }

    /// <inheritdoc/>
    public double ToSIUnitIfTreatedAsDifference(double differenceValue)
    {
      return differenceValue * 5 / 9.0;
    }

    /// <inheritdoc/>
    public double AddBiasedValueOfThisUnitAndSIValue(double biasedValueOfThisUnit, double siValue)
    {
      return biasedValueOfThisUnit + siValue * 9 / 5.0;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Kelvin.Instance; }
    }
  }
}
