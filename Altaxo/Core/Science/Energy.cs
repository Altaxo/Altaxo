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
    /// <summary>Different representations of energy.</summary>
    public enum EnergyRepresentation
    {
        /// <summary>Joule, equivalent to Nm, Ws, VAs.</summary>
        Joule = 0,

        JoulePerMole,
        CalorieInternational,
        CalorieInternationalPerMole,
        ElectronVolt,
        KilowattHours,
        AsTemperatureKelvin
    }

    public struct Energy
    {
        private EnergyRepresentation _unit;
        private double _value;

        public Energy(double value, EnergyRepresentation unit)
        {
            _unit = unit;
            _value = value;
        }

        public EnergyRepresentation Unit
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
                return ToJoule(_value, _unit);
            }
        }

        public double InUnitsOf(EnergyRepresentation destUnit)
        {
            return FromTo(_value, _unit, destUnit);
        }

        public Energy ConvertTo(EnergyRepresentation destUnit)
        {
            return new Energy(FromTo(_value, _unit, destUnit), destUnit);
        }

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
