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

namespace Altaxo.Science
{
  /// <summary>
  /// Represents available temperature representations for conversion.
  /// </summary>
  public enum TemperatureRepresentation
  {
    /// <summary>Temperature in Kelvin.</summary>
    Kelvin = 0,

    /// <summary>Inverse Kelvin (1/K).</summary>
    AsInverseKelvin,

    /// <summary>Temperature in degrees Celsius.</summary>
    DegreeCelsius,

    /// <summary>Temperature in degrees Fahrenheit.</summary>
    DegreeFahrenheit,

    /// <summary>Energy representation as joules (J) corresponding to kB*T.</summary>
    AsEnergyJoule,

    /// <summary>Energy representation as joules per mole (J/mol) corresponding to R*T.</summary>
    AsEnergyJoulePerMole
  };

  /// <summary>
  /// Provides conversion between different temperature units.
  /// </summary>
  public struct Temperature
  {
    private TemperatureRepresentation _unit;
    private double _value;

    /// <summary>Zero degree Celsius expressed in Kelvin.</summary>
    public const double ZeroDegreeCelsiusAsKelvin = 273.15;

    /// <summary>Zero degree Fahrenheit expressed in Kelvin.</summary>
    public const double ZeroDegreeFahrenheitAsKelvin = 459.67 * 5 / 9.0;

    /// <summary>Factor: degrees Fahrenheit per Kelvin.</summary>
    public const double DegreeFahrenheitPerKelvin = 9.0 / 5.0;

    /// <summary>Factor: Kelvin per degree Fahrenheit.</summary>
    public const double KelvinPerDegreeFahrenheit = 5.0 / 9.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Temperature"/> struct.
    /// </summary>
    /// <param name="value">The numeric temperature value in the specified unit.</param>
    /// <param name="unit">The unit of the temperature value.</param>
    public Temperature(double value, TemperatureRepresentation unit)
    {
      _unit = unit;
      _value = value;
    }

    /// <summary>
    /// Gets the unit of this temperature value.
    /// </summary>
    public TemperatureRepresentation Unit
    {
      get
      {
        return _unit;
      }
    }

    /// <summary>
    /// Gets the numeric temperature value in its <see cref="Unit"/>.
    /// </summary>
    public double Value
    {
      get
      {
        return _value;
      }
    }

    /// <summary>
    /// Gets the temperature converted to SI units (Kelvin).
    /// </summary>
    public double InSIUnits
    {
      get
      {
        return ToKelvin(_value, _unit);
      }
    }

    /// <summary>
    /// Returns the temperature value converted to the specified destination unit.
    /// </summary>
    /// <param name="destUnit">The destination temperature unit.</param>
    /// <returns>The temperature value expressed in <paramref name="destUnit"/>.</returns>
    public double InUnitsOf(TemperatureRepresentation destUnit)
    {
      return FromTo(_value, _unit, destUnit);
    }

    /// <summary>
    /// Returns a new <see cref="Temperature"/> instance converted to the specified unit.
    /// </summary>
    /// <param name="destUnit">The destination unit for the conversion.</param>
    /// <returns>A new <see cref="Temperature"/> with the value converted to <paramref name="destUnit"/>.</returns>
    public Temperature ConvertTo(TemperatureRepresentation destUnit)
    {
      return new Temperature(FromTo(_value, _unit, destUnit), destUnit);
    }

    /// <summary>
    /// Gets the temperature value expressed in Kelvin.
    /// </summary>
    public double InKelvin
    {
      get
      {
        return ToKelvin(_value, _unit);
      }
    }

    /// <summary>
    /// Gets the temperature value expressed as inverse Kelvin (1/K).
    /// </summary>
    public double InInverseKelvin
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.AsInverseKelvin);
      }
    }

    /// <summary>
    /// Gets the temperature value expressed in degrees Celsius.
    /// </summary>
    public double InDegreeCelsius
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.DegreeCelsius);
      }
    }

    /// <summary>
    /// Gets the temperature value expressed in degrees Fahrenheit.
    /// </summary>
    public double InDegreeFahrenheit
    {
      get
      {
        return FromTo(_value, _unit, TemperatureRepresentation.DegreeFahrenheit);
      }
    }

    /// <summary>
    /// Converts the specified temperature value to Kelvin.
    /// </summary>
    /// <param name="srcValue">The source temperature value to convert.</param>
    /// <param name="srcUnit">The source unit of <paramref name="srcValue"/>.</param>
    /// <returns>The temperature value expressed in Kelvin.</returns>
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

    /// <summary>
    /// Converts a temperature value from Kelvin to the specified destination unit.
    /// </summary>
    /// <param name="srcValueInKelvin">The temperature value expressed in Kelvin.</param>
    /// <param name="destinationUnit">The destination unit.</param>
    /// <returns>The converted temperature value expressed in <paramref name="destinationUnit"/>.</returns>
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

    /// <summary>
    /// Converts a temperature value from one unit to another.
    /// </summary>
    /// <param name="srcValue">The source value to convert.</param>
    /// <param name="srcUnit">The source unit of <paramref name="srcValue"/>.</param>
    /// <param name="destUnit">The destination unit to convert to.</param>
    /// <returns>The converted value expressed in <paramref name="destUnit"/>.</returns>
    public static double FromTo(double srcValue, TemperatureRepresentation srcUnit, TemperatureRepresentation destUnit)
    {
      if (srcUnit == destUnit)
        return srcValue;
      else
        return FromKelvinTo(ToKelvin(srcValue, srcUnit), destUnit);
    }

    /// <summary>
    /// Returns the energy corresponding to the provided temperature in Joules.
    /// </summary>
    /// <param name="srcValue">Temperature value.</param>
    /// <param name="srcUnit">Temperature unit.</param>
    /// <returns>Energy kB*T in Joules, where kB is the Boltzmann constant and T is the temperature in Kelvin.</returns>
    public static double ToEnergySI(double srcValue, TemperatureRepresentation srcUnit)
    {
      return ToKelvin(srcValue, srcUnit) * SIConstants.BOLTZMANN;
    }
  }
}
