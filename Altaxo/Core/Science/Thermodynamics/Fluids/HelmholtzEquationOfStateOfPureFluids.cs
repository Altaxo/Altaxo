#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	public abstract class HelmholtzEquationOfStateOfPureFluids : HelmholtzEquationOfState
	{
		#region Constants

		/// <summary>Gets the triple point temperature in K.</summary>
		public abstract double TriplePointTemperature { get; }

		/// <summary>Gets the triple point pressure in Pa.</summary>
		public abstract double TriplePointPressure { get; }

		/// <summary>Gets the saturated liquid density at the triple point in kg/m³.</summary>
		public abstract double TriplePointSaturatedLiquidDensity { get; }

		/// <summary>Gets the saturated vapor density at the triple point in kg/m³.</summary>
		public abstract double TriplePointSaturatedVaporDensity { get; }

		/// <summary>Gets the temperature at the critical point in Kelvin.</summary>
		public abstract double CriticalPointTemperature { get; }

		/// <summary>Gets the pressure at the critical point in Pa.</summary>
		public abstract double CriticalPointPressure { get; }

		/// <summary>Gets the density at the critical point in kg/m³.</summary>
		public abstract double CriticalPointDensity { get; }

		/// <summary>Gets the molecular weight in kg/mol.</summary>
		public abstract double MolecularWeight { get; } // kg/mol

		#endregion Constants

		#region Abstract Functions

		/// <summary>
		/// Gets the vapor pressure and its derivative with respect to temperature (in Pa and Pa/K).
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>Vapor pressure and its derivative with respect to temperature (in Pa and Pa/K)</returns>
		public abstract (double pressure, double pressureWrtTemperature) VaporPressureAndDerivativeWithRespectToTemperatureAtTemperature(double temperature_Kelvin);

		#endregion Abstract Functions

		#region Functions

		/// <inheritdoc/>
		public override double ReducingDensity => CriticalPointDensity;

		/// <inheritdoc/>
		public override double ReducingTemperature => CriticalPointTemperature;

		/// <summary>
		/// Get the vapor pressure at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The vapor pressure in Pa.</returns>
		public double VaporPressureAtTemperature(double temperature_Kelvin)
		{
			return VaporPressureAndDerivativeWithRespectToTemperatureAtTemperature(temperature_Kelvin).pressure;
		}

		/// <summary>
		/// Get the temperature of the liquid/vapor interface for a given pressure.
		/// </summary>
		/// <param name="pressure">The pressure.</param>
		/// <param name="relativeAccuracy">The relative accuracy (of the pressure, that is calculated from the iterated temperature).</param>
		/// <returns>The temperature of the liquid/vapor interface at the given pressure. <see cref="double.NaN"/> is returned for pressures below the <see cref="TriplePointPressure"/> or above the <see cref="CriticalPointPressure"/>.</returns>
		public virtual double VaporTemperatureAtPressure(double pressure, double relativeAccuracy = 1E-5)
		{
			if (!(pressure >= TriplePointPressure && pressure <= CriticalPointPressure))
				return double.NaN;

			// calculate a first guess - the pressure / temperatur curve is almost exponential
			double rel = (Math.Log(pressure) - Math.Log(TriplePointPressure)) / (Math.Log(CriticalPointPressure) - Math.Log(TriplePointPressure));
			double temperature = (1 - rel) * TriplePointTemperature + (rel) * CriticalPointTemperature;

			for (int i = 0; i < 10000; ++i)
			{
				var (currentPressure, currentPressureDeriv) = VaporPressureAndDerivativeWithRespectToTemperatureAtTemperature(temperature);

				if (double.IsNaN(currentPressure))
					return double.NaN;

				if (Math.Abs((currentPressure - pressure) / pressure) < relativeAccuracy)
				{
					return temperature;
				}

				var newTemperature = temperature - (currentPressure - pressure) / currentPressureDeriv;

				if (newTemperature > CriticalPointTemperature)
					newTemperature = 0.5 * (temperature + CriticalPointTemperature);
				else if (newTemperature < TriplePointTemperature)
					newTemperature = 0.5 * (temperature + TriplePointTemperature);

				temperature = newTemperature;
			}

			return double.NaN;
		}

		#endregion Functions
	}
}
