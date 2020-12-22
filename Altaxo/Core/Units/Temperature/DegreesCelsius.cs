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
  [UnitDescription("Temperature", 0, 0, 0, 0, 1, 0, 0)]
  public class DegreesCelsius : UnitBase, IUnit, IBiasedUnit
  {
    private const double KelvinOffset = 273.15;

    private static readonly DegreesCelsius _instance = new DegreesCelsius();

    public static DegreesCelsius Instance { get { return _instance; } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DegreesCelsius), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return DegreesCelsius.Instance;
      }
    }
    #endregion

    protected DegreesCelsius()
    {
    }

    public string Name
    {
      get { return "DegreesCelsius"; }
    }

    public string ShortCut
    {
      get { return "°C"; }
    }

    public double ToSIUnit(double x)
    {
      return x + KelvinOffset;
    }

    public double FromSIUnit(double x)
    {
      return x - KelvinOffset;
    }

    public double ToSIUnitIfTreatedAsDifference(double differenceValue)
    {
      return differenceValue;
    }

    public double AddBiasedValueOfThisUnitAndSIValue(double biasedValueOfThisUnit, double siValue)
    {
      return biasedValueOfThisUnit + siValue;
    }

    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    public SIUnit SIUnit
    {
      get { return Kelvin.Instance; }
    }
  }
}
