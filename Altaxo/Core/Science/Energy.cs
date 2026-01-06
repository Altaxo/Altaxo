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
  /// Different representations of energy.
  /// </summary>
  public enum EnergyRepresentation
  {
    /// <summary>Joule, equivalent to Nm, Ws, VAs.</summary>
    Joule = 0,

    /// <summary>Joule per mole (J mol^-1).</summary>
    JoulePerMole,

    /// <summary>International calorie (cal).</summary>
    CalorieInternational,

    /// <summary>International calorie per mole (cal mol^-1).</summary>
    CalorieInternationalPerMole,

    /// <summary>Electronvolt (eV).</summary>
    ElectronVolt,

    /// <summary>Kilowatt hours (kWh).</summary>
    KilowattHours,

    /// <summary>Interpret energy as temperature in Kelvin via Boltzmann's constant.</summary>
    AsTemperatureKelvin
  }

  /// <summary>
  /// Represents an energy value together with its unit.
  /// </summary>
  public struct Energy
  {
    private EnergyRepresentation _unit;
    private double _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Energy"/> struct.
    /// </summary>
    /// <param name="value">The numeric energy value in the specified unit.</param>
    /// <param name="unit">The unit of the energy value.</param>
    public Energy(double value, EnergyRepresentation unit)
    {
      _unit = unit;
      _value = value;
    }

    /// <summary>
    /// Gets the unit of this energy value.
    /// </summary>
    public EnergyRepresentation Unit
    {
      get
      {
        return _unit;
      }
    }

    /// <summary>
    /// Gets the raw numeric value of this energy in its <see cref="Unit"/>.
    /// </summary>
    public double Value
    {
      get
      {
        return _value;
      }
    }

    /// <summary>
    /// Gets the energy converted to SI units (Joule).
    /// </summary>
    public double InSIUnits
    {
      get
      {
        return ToJoule(_value, _unit);
      }
    }

    /// <summary>
    /// Returns the energy value converted to the specified destination unit.
    /// </summary>
    /// <param name="destUnit">The destination energy unit.</param>
    /// <returns>The energy value expressed in <paramref name="destUnit"/>.</returns>
    public double InUnitsOf(EnergyRepresentation destUnit)
    {
      return FromTo(_value, _unit, destUnit);
    }

    /// <summary>
    /// Returns a new <see cref="Energy"/> instance converted to the specified unit.
    /// </summary>
    /// <param name="destUnit">The destination unit for the conversion.</param>
    /// <returns>A new <see cref="Energy"/> with the value converted to <paramref name="destUnit"/>.</returns>
    public Energy ConvertTo(EnergyRepresentation destUnit)
    {
      return new Energy(FromTo(_value, _unit, destUnit), destUnit);
    }

    /// <summary>
    /// Converts a value from the specified unit to Joule (SI unit of energy).
    /// </summary>
    /// <param name="srcValue">The source value to convert.</param>
    /// <param name="srcUnit">The source unit of <paramref name="srcValue"/>.</param>
    /// <returns>The converted value expressed in Joule.</returns>
    public static double ToJoule(double srcValue, EnergyRepresentation srcUnit)
    {
      switch (srcUnit)
      {
        case EnergyRepresentation.Joule:
          return srcValue;

        case EnergyRepresentation.JoulePerMole:
          return srcValue / SIConstants.AVOGADROS_CONSTANT;

        case EnergyRepresentation.CalorieInternational:
          return srcValue * SIConstants.CALORIE;

        case EnergyRepresentation.CalorieInternationalPerMole:
          return srcValue * SIConstants.CALORIE / SIConstants.AVOGADROS_CONSTANT;

        case EnergyRepresentation.ElectronVolt:
          return srcValue * SIConstants.ELECTRON_VOLT;

        case EnergyRepresentation.KilowattHours:
          return srcValue * 3600000;

        case EnergyRepresentation.AsTemperatureKelvin:
          return srcValue * SIConstants.BOLTZMANN;

        default:
          throw new ArgumentOutOfRangeException("EnergyUnit unknown: " + srcUnit.ToString());
      }
    }

    /// <summary>
    /// Gets the factor neccessary to convert a value from the specified unit to Joule (SI unit of energy).
    /// </summary>
    /// <param name="srcUnit">The source unit.</param>
    /// <returns>The factor that is neccessary to converted a value in the given <paramref name="srcUnit"/> to a value expressed in Joule.</returns>
    public static double ToJouleFactor(EnergyRepresentation srcUnit)
    {
      switch (srcUnit)
      {
        case EnergyRepresentation.Joule:
          return 1;

        case EnergyRepresentation.JoulePerMole:
          return 1 / SIConstants.AVOGADROS_CONSTANT;

        case EnergyRepresentation.CalorieInternational:
          return SIConstants.CALORIE;

        case EnergyRepresentation.CalorieInternationalPerMole:
          return SIConstants.CALORIE / SIConstants.AVOGADROS_CONSTANT;

        case EnergyRepresentation.ElectronVolt:
          return SIConstants.ELECTRON_VOLT;

        case EnergyRepresentation.KilowattHours:
          return 3600000;

        case EnergyRepresentation.AsTemperatureKelvin:
          return SIConstants.BOLTZMANN;

        default:
          throw new ArgumentOutOfRangeException("EnergyUnit unknown: " + srcUnit.ToString());
      }
    }

    /// <summary>
    /// Converts a value from Joule to the specified destination unit.
    /// </summary>
    /// <param name="srcValueInJoule">The source value expressed in Joule.</param>
    /// <param name="destUnit">The destination unit.</param>
    /// <returns>The value expressed in <paramref name="destUnit"/>.</returns>
    public static double FromJouleTo(double srcValueInJoule, EnergyRepresentation destUnit)
    {
      switch (destUnit)
      {
        case EnergyRepresentation.Joule:
          return srcValueInJoule;

        case EnergyRepresentation.JoulePerMole:
          return srcValueInJoule * SIConstants.AVOGADROS_CONSTANT;

        case EnergyRepresentation.CalorieInternational:
          return srcValueInJoule / SIConstants.CALORIE;

        case EnergyRepresentation.CalorieInternationalPerMole:
          return srcValueInJoule * SIConstants.AVOGADROS_CONSTANT / SIConstants.CALORIE;

        case EnergyRepresentation.ElectronVolt:
          return srcValueInJoule / SIConstants.ELECTRON_VOLT;

        case EnergyRepresentation.KilowattHours:
          return srcValueInJoule / 3600000;

        case EnergyRepresentation.AsTemperatureKelvin:
          return srcValueInJoule / SIConstants.BOLTZMANN;

        default:
          throw new ArgumentOutOfRangeException("EnergyUnit unknown: " + destUnit.ToString());
      }
    }

    /// <summary>
    /// Converts a value from one energy unit to another.
    /// </summary>
    /// <param name="srcValue">The source value to convert.</param>
    /// <param name="srcUnit">The source unit of <paramref name="srcValue"/>.</param>
    /// <param name="destUnit">The destination unit to convert to.</param>
    /// <returns>The converted value expressed in <paramref name="destUnit"/>.</returns>
    public static double FromTo(double srcValue, EnergyRepresentation srcUnit, EnergyRepresentation destUnit)
    {
      if (srcUnit == destUnit)
        return srcValue;
      else
        return FromJouleTo(ToJoule(srcValue, srcUnit), destUnit);
    }

    /// <summary>
    /// Returns the temperature (in Kelvin) corresponding to the provided energy.
    /// </summary>
    /// <param name="srcValue">Energy value.</param>
    /// <param name="srcUnit">Energy unit.</param>
    /// <returns>Temperature E/kB in Kelvin, where kB is the BOLTZMANN constant and E the energy in Joule.</returns>
    public static double ToTemperatureSI(double srcValue, EnergyRepresentation srcUnit)
    {
      return ToJoule(srcValue, srcUnit) / SIConstants.BOLTZMANN;
    }
  }
}
