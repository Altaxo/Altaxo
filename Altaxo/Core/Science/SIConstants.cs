#region Copyright

/* const/gsl_const_mks.h
 *
 * Copyright (C) 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005,
 * 2006 Brian Gough
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

// Adopted to Altaxo from GSL 1.10 by D.Lellinger 2008

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
  /// <summary>
  /// Provides physical constants in the SI base units (Metre, Kilogram, Second, Ampere, Kelvin, Candela, Mole).
  /// </summary>
  public static class SIConstants
  {
    /// <summary>Speed of light in vacuum, in metres per second.</summary>
    public const double SPEED_OF_LIGHT = (2.99792458e8); /* m / s */

    /// <summary>Newtonian gravitational constant, in m^3 / kg s^2.</summary>
    public const double GRAVITATIONAL_CONSTANT = (6.673e-11); /* m^3 / kg s^2 */

    /// <summary>Planck's constant h, in kg m^2 / s.</summary>
    public const double PLANCKS_CONSTANT_H = (6.62606876e-34); /* kg m^2 / s */

    /// <summary>Reduced Planck's constant ħ (h-bar), in kg m^2 / s.</summary>
    public const double PLANCKS_CONSTANT_HBAR = (1.05457159642e-34); /* kg m^2 / s */

    /// <summary>Astronomical unit (mean Earth–Sun distance), in metres.</summary>
    public const double ASTRONOMICAL_UNIT = (1.49597870691e11); /* m */

    /// <summary>Light year, in metres.</summary>
    public const double LIGHT_YEAR = (9.46053620707e15); /* m */

    /// <summary>Parsec, in metres.</summary>
    public const double PARSEC = (3.08567758135e16); /* m */

    /// <summary>Standard acceleration of gravity, in metres per second squared.</summary>
    public const double GRAV_ACCEL = (9.80665e0); /* m / s^2 */

    /// <summary>Electron volt in joules (kg m^2 / s^2).</summary>
    public const double ELECTRON_VOLT = (1.6021765314e-19); /* kg m^2 / s^2 */

    /// <summary>Mass of the electron, in kilograms.</summary>
    public const double MASS_ELECTRON = (9.10938188e-31); /* kg */

    /// <summary>Mass of the muon, in kilograms.</summary>
    public const double MASS_MUON = (1.88353109e-28); /* kg */

    /// <summary>Mass of the proton, in kilograms.</summary>
    public const double MASS_PROTON = (1.67262158e-27); /* kg */

    /// <summary>Mass of the neutron, in kilograms.</summary>
    public const double MASS_NEUTRON = (1.67492716e-27); /* kg */

    /// <summary>Rydberg energy, in joules (kg m^2 / s^2).</summary>
    public const double RYDBERG = (2.17987190389e-18); /* kg m^2 / s^2 */

    /// <summary>Boltzmann constant, in kg m^2 / K s^2 (J/K).</summary>
    public const double BOLTZMANN = (1.3806503e-23); /* kg m^2 / K s^2 */

    /// <summary>Bohr magneton, in A m^2.</summary>
    public const double BOHR_MAGNETON = (9.27400899e-24); /* A m^2 */

    /// <summary>Nuclear magneton, in A m^2.</summary>
    public const double NUCLEAR_MAGNETON = (5.05078317e-27); /* A m^2 */

    /// <summary>Electron magnetic moment, in A m^2.</summary>
    public const double ELECTRON_MAGNETIC_MOMENT = (9.28476362e-24); /* A m^2 */

    /// <summary>Proton magnetic moment, in A m^2.</summary>
    public const double PROTON_MAGNETIC_MOMENT = (1.410606633e-26); /* A m^2 */

    /// <summary>Molar gas constant R, in J / (K mol).</summary>
    public const double MOLAR_GAS = (8.314472e0); /* kg m^2 / K mol s^2 */

    /// <summary>Standard molar volume of an ideal gas at STP, in cubic metres per mole.</summary>
    public const double STANDARD_GAS_VOLUME = (2.2710981e-2); /* m^3 / mol */

    /// <summary>One minute in seconds.</summary>
    public const double MINUTE = (6e1); /* s */

    /// <summary>One hour in seconds.</summary>
    public const double HOUR = (3.6e3); /* s */

    /// <summary>One day in seconds.</summary>
    public const double DAY = (8.64e4); /* s */

    /// <summary>One week in seconds.</summary>
    public const double WEEK = (6.048e5); /* s */

    /// <summary>One inch in metres.</summary>
    public const double INCH = (2.54e-2); /* m */

    /// <summary>One foot in metres.</summary>
    public const double FOOT = (3.048e-1); /* m */

    /// <summary>One yard in metres.</summary>
    public const double YARD = (9.144e-1); /* m */

    /// <summary>One mile in metres.</summary>
    public const double MILE = (1.609344e3); /* m */

    /// <summary>Nautical mile in metres.</summary>
    public const double NAUTICAL_MILE = (1.852e3); /* m */

    /// <summary>Fathom in metres.</summary>
    public const double FATHOM = (1.8288e0); /* m */

    /// <summary>Thousandth of an inch (mil) in metres.</summary>
    public const double MIL = (2.54e-5); /* m */

    /// <summary>Typographic point in metres.</summary>
    public const double POINT = (3.52777777778e-4); /* m */

    /// <summary>TeX point in metres.</summary>
    public const double TEXPOINT = (3.51459803515e-4); /* m */

    /// <summary>Micron (micrometer) in metres.</summary>
    public const double MICRON = (1e-6); /* m */

    /// <summary>Angstrom in metres.</summary>
    public const double ANGSTROM = (1e-10); /* m */

    /// <summary>Hectare in square metres.</summary>
    public const double HECTARE = (1e4); /* m^2 */

    /// <summary>Acre in square metres.</summary>
    public const double ACRE = (4.04685642241e3); /* m^2 */

    /// <summary>Barn in square metres.</summary>
    public const double BARN = (1e-28); /* m^2 */

    /// <summary>Liter in cubic metres.</summary>
    public const double LITER = (1e-3); /* m^3 */

    /// <summary>US gallon in cubic metres.</summary>
    public const double US_GALLON = (3.78541178402e-3); /* m^3 */

    /// <summary>Quart in cubic metres.</summary>
    public const double QUART = (9.46352946004e-4); /* m^3 */

    /// <summary>Pint in cubic metres.</summary>
    public const double PINT = (4.73176473002e-4); /* m^3 */

    /// <summary>Cup in cubic metres.</summary>
    public const double CUP = (2.36588236501e-4); /* m^3 */

    /// <summary>Fluid ounce in cubic metres.</summary>
    public const double FLUID_OUNCE = (2.95735295626e-5); /* m^3 */

    /// <summary>Tablespoon in cubic metres.</summary>
    public const double TABLESPOON = (1.47867647813e-5); /* m^3 */

    /// <summary>Teaspoon in cubic metres.</summary>
    public const double TEASPOON = (4.92892159375e-6); /* m^3 */

    /// <summary>Canadian gallon in cubic metres.</summary>
    public const double CANADIAN_GALLON = (4.54609e-3); /* m^3 */

    /// <summary>UK gallon in cubic metres.</summary>
    public const double UK_GALLON = (4.546092e-3); /* m^3 */

    /// <summary>Miles per hour in metres per second.</summary>
    public const double MILES_PER_HOUR = (4.4704e-1); /* m / s */

    /// <summary>Kilometers per hour in metres per second.</summary>
    public const double KILOMETERS_PER_HOUR = (2.77777777778e-1); /* m / s */

    /// <summary>Knot (nautical mile per hour) in metres per second.</summary>
    public const double KNOT = (5.14444444444e-1); /* m / s */

    /// <summary>Pound mass in kilograms.</summary>
    public const double POUND_MASS = (4.5359237e-1); /* kg */

    /// <summary>Ounce (mass) in kilograms.</summary>
    public const double OUNCE_MASS = (2.8349523125e-2); /* kg */

    /// <summary>Ton (short ton) in kilograms.</summary>
    public const double TON = (9.0718474e2); /* kg */

    /// <summary>Metric ton (tonne) in kilograms.</summary>
    public const double METRIC_TON = (1e3); /* kg */

    /// <summary>UK ton (long ton) in kilograms.</summary>
    public const double UK_TON = (1.0160469088e3); /* kg */

    /// <summary>Troy ounce in kilograms.</summary>
    public const double TROY_OUNCE = (3.1103475e-2); /* kg */

    /// <summary>Carat in kilograms.</summary>
    public const double CARAT = (2e-4); /* kg */

    /// <summary>Unified atomic mass unit (Dalton) in kilograms.</summary>
    public const double UNIFIED_ATOMIC_MASS = (1.66053873e-27); /* kg */

    /// <summary>Gram-force in newtons (kg m / s^2).</summary>
    public const double GRAM_FORCE = (9.80665e-3); /* kg m / s^2 */

    /// <summary>Pound-force in newtons (kg m / s^2).</summary>
    public const double POUND_FORCE = (4.44822161526e0); /* kg m / s^2 */

    /// <summary>Kilopound-force in newtons (kg m / s^2).</summary>
    public const double KILOPOUND_FORCE = (4.44822161526e3); /* kg m / s^2 */

    /// <summary>Poundal in newtons (kg m / s^2).</summary>
    public const double POUNDAL = (1.38255e-1); /* kg m / s^2 */

    /// <summary>International calorie in joules.</summary>
    public const double CALORIE = (4.1868e0); /* kg m^2 / s^2 */

    /// <summary>British Thermal Unit (BTU) in joules.</summary>
    public const double BTU = (1.05505585262e3); /* kg m^2 / s^2 */

    /// <summary>Therm in joules.</summary>
    public const double THERM = (1.05506e8); /* kg m^2 / s^2 */

    /// <summary>Horsepower in watts (kg m^2 / s^3).</summary>
    public const double HORSEPOWER = (7.457e2); /* kg m^2 / s^3 */

    /// <summary>Bar in pascals (kg / m s^2).</summary>
    public const double BAR = (1e5); /* kg / m s^2 */

    /// <summary>Standard atmosphere in pascals.</summary>
    public const double STD_ATMOSPHERE = (1.01325e5); /* kg / m s^2 */

    /// <summary>Torr in pascals.</summary>
    public const double TORR = (1.33322368421e2); /* kg / m s^2 */

    /// <summary>Metre of mercury (mm Hg) in pascals.</summary>
    public const double METER_OF_MERCURY = (1.33322368421e5); /* kg / m s^2 */

    /// <summary>Inch of mercury in pascals.</summary>
    public const double INCH_OF_MERCURY = (3.38638815789e3); /* kg / m s^2 */

    /// <summary>Inch of water in pascals.</summary>
    public const double INCH_OF_WATER = (2.490889e2); /* kg / m s^2 */

    /// <summary>Pound per square inch (psi) in pascals.</summary>
    public const double PSI = (6.89475729317e3); /* kg / m s^2 */

    /// <summary>Poise (dynamic viscosity) in kg m^-1 s^-1.</summary>
    public const double POISE = (1e-1); /* kg m^-1 s^-1 */

    /// <summary>Stokes (kinematic viscosity) in square metres per second.</summary>
    public const double STOKES = (1e-4); /* m^2 / s */

    /// <summary>Faraday constant, coulombs per mole (A s / mol).</summary>
    public const double FARADAY = (9.6485341472e4); /* A s / mol */

    /// <summary>Elementary charge in coulombs (A s).</summary>
    public const double ELECTRON_CHARGE = (1.602176462e-19); /* A s */

    /// <summary>Gauss to tesla conversion (1 G = 1e-4 T).</summary>
    public const double GAUSS = (1e-4); /* kg / A s^2 */

    /// <summary>Stilb (candela per square metre) conversion factor.</summary>
    public const double STILB = (1e4); /* cd / m^2 */

    /// <summary>Lumen (candela-steradian) unit factor.</summary>
    public const double LUMEN = (1e0); /* cd sr */

    /// <summary>Lux (lumen per square metre) unit factor.</summary>
    public const double LUX = (1e0); /* cd sr / m^2 */

    /// <summary>Phot (candela-steradian per square metre).</summary>
    public const double PHOT = (1e4); /* cd sr / m^2 */

    /// <summary>Footcandle in lumens per square metre.</summary>
    public const double FOOTCANDLE = (1.076e1); /* cd sr / m^2 */

    /// <summary>Lambert (unit of luminance) conversion factor.</summary>
    public const double LAMBERT = (1e4); /* cd sr / m^2 */

    /// <summary>Footlambert conversion factor.</summary>
    public const double FOOTLAMBERT = (1.07639104e1); /* cd sr / m^2 */

    /// <summary>Curie (radioactivity) in decays per second.</summary>
    public const double CURIE = (3.7e10); /* 1 / s */

    /// <summary>Roentgen in coulombs per kilogram.</summary>
    public const double ROENTGEN = (2.58e-4); /* A s / kg */

    /// <summary>Rad (radiation absorbed dose) in joules per kilogram.</summary>
    public const double RAD = (1e-2); /* m^2 / s^2 */

    /// <summary>Solar mass in kilograms.</summary>
    public const double SOLAR_MASS = (1.98892e30); /* kg */

    /// <summary>Bohr radius in metres.</summary>
    public const double BOHR_RADIUS = (5.291772083e-11); /* m */

    /// <summary>Newton (unit of force) in SI base units (kg m / s^2).</summary>
    public const double NEWTON = (1e0); /* kg m / s^2 */

    /// <summary>Dyne (unit of force) in SI base units (kg m / s^2).</summary>
    public const double DYNE = (1e-5); /* kg m / s^2 */

    /// <summary>Joule (unit of energy) in SI base units (kg m^2 / s^2).</summary>
    public const double JOULE = (1e0); /* kg m^2 / s^2 */

    /// <summary>Erg (unit of energy) in SI base units (kg m^2 / s^2).</summary>
    public const double ERG = (1e-7); /* kg m^2 / s^2 */

    /// <summary>Stefan-Boltzmann constant in kg / K^4 s^3 (W / m^2 K^4).</summary>
    public const double STEFAN_BOLTZMANN_CONSTANT = (5.67039934436e-8); /* kg / K^4 s^3 */

    /// <summary>Thomson scattering cross-section in square metres.</summary>
    public const double THOMSON_CROSS_SECTION = (6.65245853542e-29); /* m^2 */

    /// <summary>Vacuum permittivity (electric constant) in A^2 s^4 / kg m^3.</summary>
    public const double VACUUM_PERMITTIVITY = (8.854187817e-12); /* A^2 s^4 / kg m^3 */

    /// <summary>Vacuum permeability (magnetic constant) in kg m / A^2 s^2.</summary>
    public const double VACUUM_PERMEABILITY = (1.25663706144e-6); /* kg m / A^2 s^2 */

    /// <summary>Debye in coulomb-metre (A s^2 / m^2).</summary>
    public const double DEBYE = (3.33564095198e-30); /* A s^2 / m^2 */

    /// <summary>Avogadro's constant (number of particles per mole).</summary>
    public const double AVOGADROS_CONSTANT = (6.0221417930e23); /* no unit */
  }
}
