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

namespace Altaxo.Science
{
  public enum TemperatureRepresentation
  {
    Kelvin = 0,
    AsInverseKelvin,
    DegreeCelsius,
    DegreeFahrenheit,
    AsEnergyJoule,
    AsEnergyJoulePerMole
  };

  /// <summary>
  /// Provides conversion between different temperature units.
  /// </summary>
  public struct Temperature
  {
    private TemperatureRepresentation _unit;
    private double _value;

    public const double ZeroDegreeCelsiusAsKelvin = 273.15;
    public const double ZeroDegreeFahrenheitAsKelvin = 459.67 * 5 / 9.0;
    public const double DegreeFahrenheitPerKelvin = 9.0 / 5.0;
    public const double KelvinPerDegreeFahrenheit = 5.0 / 9.0;

    public Temperature(double value, TemperatureRepresentation unit)
    {
      _unit = unit;
      _value = value;
    }

    public TemperatureRepresentation Unit
    {
      get
      {
        return _unit;
      }
    }

    public double Value
    {
      get
      {
        return _value;
      }
    }

    public double InSIUnits
    {
      get
      {
        return ToKelvin(_value, _unit);
      }
    }

    public double InUnitsOf(TemperatureRepresentation destUnit)
    {
      return FromTo(_value, _unit, destUnit);
    }

    public Temperature ConvertTo(TemperatureRepresentation destUnit)
    {
      return new Temperature(FromTo(_value, _unit, destUnit), destUnit);
    }

    public double InKelvin
    {
      get
      {
        return ToKelvin(_value, _unit);
      }
    }

    public double InInverseKelvin
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.AsInverseKelvin);
      }
    }

    public double InDegreeCelsius
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.DegreeCelsius);
      }
    }

    public double InDegreeFahrenheit
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.DegreeFahrenheit);
      }
    }

    public static double ToKelvin(double srcValue, TemperatureRepresentation srcUnit)
    {
      switch (srcUnit)
      {
        case TemperatureRepresentation.DegreeCelsius:
          return 273.15 + srcValue;

        case TemperatureRepresentation.DegreeFahrenheit:
          return ZeroDegreeFahrenheitAsKelvin + srcValue * KelvinPerDegreeFahrenheit;

        case TemperatureRepresentation.AsInverseKelvin:
          return 1 / srcValue;

        case TemperatureRepresentation.Kelvin:
          return srcValue;

        case TemperatureRepresentation.AsEnergyJoule:
          return srcValue / SIConstants.BOLTZMANN;

        case TemperatureRepresentation.AsEnergyJoulePerMole:
          return srcValue / SIConstants.MOLAR_GAS;

        default:
          throw new ArgumentOutOfRangeException("TemperatureUnit is unknown: " + srcUnit.ToString());
      }
    }

    public static double FromKelvinTo(double srcValueInKelvin, TemperatureRepresentation destinationUnit)
    {
      switch (destinationUnit)
      {
        case TemperatureRepresentation.DegreeCelsius:
          return srcValueInKelvin - ZeroDegreeCelsiusAsKelvin;

        case TemperatureRepresentation.DegreeFahrenheit:
          return (srcValueInKelvin - ZeroDegreeFahrenheitAsKelvin) * DegreeFahrenheitPerKelvin;

        case TemperatureRepresentation.AsInverseKelvin:
          return 1 / srcValueInKelvin;

        case TemperatureRepresentation.Kelvin:
          return srcValueInKelvin;

        case TemperatureRepresentation.AsEnergyJoule:
          return srcValueInKelvin * SIConstants.BOLTZMANN;

        case TemperatureRepresentation.AsEnergyJoulePerMole:
          return srcValueInKelvin * SIConstants.MOLAR_GAS;

        default:
          throw new ArgumentOutOfRangeException("TemperatureUnit is unknown: " + destinationUnit.ToString());
      }
    }

    public static double FromTo(double srcValue, TemperatureRepresentation srcUnit, TemperatureRepresentation destUnit)
    {
      if (srcUnit == destUnit)
        return srcValue;
      else
        return FromKelvinTo(ToKelvin(srcValue, srcUnit), destUnit);
    }

    /// <summary>
    /// Returns the energy corresponding to the provided temperature in Joule.
    /// </summary>
    /// <param name="srcValue">Temperature value.</param>
    /// <param name="srcUnit">Temperature unit.</param>
    /// <returns>Energy kB*T in Joule, where kB is the BOLTZMANN constant and T the temperature in Kelvin.</returns>
    public static double ToEnergySI(double srcValue, TemperatureRepresentation srcUnit)
    {
      return ToKelvin(srcValue, srcUnit) * SIConstants.BOLTZMANN;
    }
  }
}
