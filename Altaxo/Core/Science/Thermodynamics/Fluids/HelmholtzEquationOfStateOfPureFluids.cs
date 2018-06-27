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
	/// <summary>
	/// Equation of state based on the dimensionless Helmholtz energy, for pure fluids.
	/// </summary>
	public abstract class HelmholtzEquationOfStateOfPureFluids : HelmholtzEquationOfState
	{
		#region Constants

		/// <summary>Gets the triple point temperature in K.</summary>
		public abstract double TriplePointTemperature { get; }

		/// <summary>Gets the triple point pressure in Pa.</summary>
		public abstract double TriplePointPressure { get; }

		/// <summary>Gets the saturated liquid density at the triple point in mol/m³.</summary>
		public abstract double TriplePointSaturatedLiquidMoleDensity { get; }

		/// <summary>Gets the saturated vapor density at the triple point in mol/m³.</summary>
		public abstract double TriplePointSaturatedVaporMoleDensity { get; }

		/// <summary>Gets the temperature at the critical point in Kelvin.</summary>
		public abstract double CriticalPointTemperature { get; }

		/// <summary>Gets the pressure at the critical point in Pa.</summary>
		public abstract double CriticalPointPressure { get; }

		/// <summary>Gets the mole density at the critical point in mol/m³.</summary>
		public abstract double CriticalPointMoleDensity { get; }

		/// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K.</summary>
		public abstract double NormalBoilingPointTemperature { get; }

		/// <summary>Gets the acentric factor.</summary>
		public abstract double AcentricFactor { get; }

		#endregion Constants

		#region Derived constants

		/// <summary>Gets the saturated liquid density at the triple point in kg/m³.</summary>
		public virtual double TriplePointSaturatedLiquidMassDensity { get { return TriplePointSaturatedLiquidMoleDensity * MolecularWeight; } }

		/// <summary>Gets the saturated vapor density at the triple point in kg/m³.</summary>
		public virtual double TriplePointSaturatedVaporMassDensity { get { return TriplePointSaturatedVaporMoleDensity * MolecularWeight; } }

		/// <summary>Gets the mass density at the critical point in kg/m³.</summary>
		public double CriticalPointMassDensity => CriticalPointMoleDensity * MolecularWeight;

		#endregion Derived constants

		#region Abstract Functions

		/// <summary>
		/// Gets an estimate for the saturated vapor pressure in dependence on the temperature as well as for the derivative of the saturated vapor pressure with respect to the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in Kelvin.</param>
		/// <returns>An estimate for the saturated vapor pressure in Pa and the derivative w.r.t. temperature in Pa/K at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], (double.NaN, double.NaN) is returned.
		/// </returns>
		public abstract (double pressure, double pressureWrtTemperature) SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(double temperature);

		#endregion Abstract Functions

		#region Functions

		/// <inheritdoc/>
		public override double ReducingMoleDensity => CriticalPointMoleDensity;

		/// <inheritdoc/>
		public override double ReducingTemperature => CriticalPointTemperature;

		/// <summary>
		/// Gets an estimate for the saturated vapor pressure in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in K.</param>
		/// <returns>An estimate for the saturated vapor pressure in Pa at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public virtual double SaturatedVaporPressureEstimate_FromTemperature(double temperature)
		{
			if (!(temperature >= TriplePointTemperature && temperature <= CriticalPointTemperature))
				return double.NaN;

			return SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature).pressure;
		}

		/// <summary>
		/// Get the temperature at the liquid/vapor interface for a given pressure by iteration (Newton-Raphson).
		/// </summary>
		/// <param name="pressure">The pressure in Pa.</param>
		/// <param name="relativeAccuracy">The relative accuracy (of the pressure, that is calculated back from the iterated temperature).</param>
		/// <returns>The temperature in Kelvin of the liquid/vapor interface at the given pressure in Pa. See <see cref="double.NaN"/> is returned for pressures below the <see cref="TriplePointPressure"/> or above the <see cref="CriticalPointPressure"/>.</returns>
		public virtual double SaturatedVaporTemperature_FromPressure(double pressure, double relativeAccuracy = 1E-6)
		{
			if (!(pressure >= TriplePointPressure && pressure <= CriticalPointPressure))
				return double.NaN;

			// calculate a first guess - the pressure / temperature curve is almost exponential
			double rel = (Math.Log(pressure) - Math.Log(TriplePointPressure)) / (Math.Log(CriticalPointPressure) - Math.Log(TriplePointPressure));
			double temperature = (1 - rel) * TriplePointTemperature + (rel) * CriticalPointTemperature;

			for (int i = 0; i < 100; ++i)
			{
				var (currentPressure, currentPressureDeriv) = SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(temperature);

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
