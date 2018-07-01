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

		/// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
		public abstract double? NormalBoilingPointTemperature { get; }

		/// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
		public abstract double? NormalSublimationPointTemperature { get; }

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

		/// <summary>
		/// Gets an estimate for the saturated vapor mole density in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in K.</param>
		/// <returns>An estimate for the saturated vapor mole density in mol/m³ at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public abstract double SaturatedVaporMoleDensityEstimate_FromTemperature(double temperature);

		/// <summary>
		/// Gets an estimate for the saturated liquid mole density in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in Kelvin.</param>
		/// <returns>An estimate for the saturated liquid mole density in mol/m³ at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public abstract double SaturatedLiquidMoleDensityEstimate_FromTemperature(double temperature);

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

		#region Equilibrium calculations

		/// <summary>
		/// Gets the saturated liquid mole density, the saturated vapor mole density, and the temperature for a given pressure.
		/// This is done by iteration, using multivariate Newton-Raphson.
		/// </summary>
		/// <param name="pressure">The pressure in Pa. Must be greater than or equal the triple point pressure and less than or equal to the critical point pressure.</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public (double liquidMoleDensity, double vaporMoleDensity, double temperature) SaturatedLiquidAndVaporMoleDensitiesAndTemperature_FromPressure(double pressure)
		{
			double temperatureGuess = SaturatedVaporTemperature_FromPressure(pressure);
			double liquidMoleDensity = SaturatedLiquidMoleDensityEstimate_FromTemperature(temperatureGuess);
			double vaporMoleDensity = SaturatedVaporMoleDensityEstimate_FromTemperature(temperatureGuess);

			// now we have some initial values, we can begin to iterate ...
			// three equations to solve simultaneously (variables rho_liquid, rho_vapor, T):
			// p(rho_liquid, T) - p_given = 0 (equality of liquid pressure and given pressure)
			// p(rho_vapor, T) - p_given = 0  (equality of vapor pressure and given pressure)
			// fugacitycoeff(rho_liquid, T)*p(rho_liquid, T) - fugacitycoeff(rho_vapor, T)*p(rho_vapor, T) = 0 (equality of fugacities)

			// for convenience, we work with reduced variables:

			double tau = ReducingTemperature / temperatureGuess; // reduced temperature
			double deltaL = liquidMoleDensity / ReducingMoleDensity; // reduced liquid density
			double deltaV = vaporMoleDensity / ReducingMoleDensity; // reduced vapor density

			for (int i = 0; i < 100; ++i)
			{
				var PhiRL = PhiR_OfReducedVariables(deltaL, tau);
				var deltaLPhiR_deltaL = deltaL * PhiR_delta_OfReducedVariables(deltaL, tau);
				var deltaLdeltaLPhiR_deltaLdeltaL = deltaL * deltaL * PhiR_deltadelta_OfReducedVariables(deltaL, tau);
				var tauPhiRL_tau = tau * PhiR_tau_OfReducedVariables(deltaL, tau);
				var deltaLtauPhiR_deltaLtau = deltaL * tau * PhiR_deltatau_OfReducedVariables(deltaL, tau);

				var PhiRV = PhiR_OfReducedVariables(deltaV, tau);
				var deltaVPhiR_deltaV = deltaV * PhiR_delta_OfReducedVariables(deltaV, tau);
				var deltaVdeltaVPhiR_deltaVdeltaV = deltaV * deltaV * PhiR_deltadelta_OfReducedVariables(deltaV, tau);
				var tauPhiRV_tau = tau * PhiR_tau_OfReducedVariables(deltaV, tau);
				var deltaVtauPhiR_deltaVtau = deltaV * tau * PhiR_deltatau_OfReducedVariables(deltaV, tau);

				// we also work with a reduced pressure pr = pressure/(R*TCrit*RhoCrit)
				double prL = (deltaL / tau) * (1 + deltaLPhiR_deltaL); // Reduced liquid pressure
				double prV = (deltaV / tau) * (1 + deltaVPhiR_deltaV); // Reduced vapor pressure
				double prGiven = pressure / (WorkingUniversalGasConstant * ReducingTemperature * ReducingMoleDensity);

				// Derivatives of reduced pressure with respect to delta and tau
				double prL_deltaL = (1 + 2 * deltaLPhiR_deltaL + deltaLdeltaLPhiR_deltaLdeltaL) / tau;
				double prL_tau = deltaL * (-1 - deltaLPhiR_deltaL + deltaLtauPhiR_deltaLtau) / (tau * tau);

				double prV_deltaV = (1 + 2 * deltaVPhiR_deltaV + deltaVdeltaVPhiR_deltaVdeltaV) / tau;
				double prV_tau = deltaV * (-1 - deltaVPhiR_deltaV + deltaVtauPhiR_deltaVtau) / (tau * tau);

				// we also don't use the Fugacity directly (fucacityCoefficent*pressure), but instead
				// we use the reduced fugacity fugr = (fugacityCoefficient*reducedPressure)
				// now derivatives of reduced fugacity w.r.t. delta and tau

				double fugrL = (deltaL / tau) * Math.Exp(PhiRL + deltaLPhiR_deltaL);
				double fugrL_deltaL = (fugrL / deltaL) * (1 + 2 * deltaLPhiR_deltaL + deltaLdeltaLPhiR_deltaLdeltaL);
				double fugrL_tau = (fugrL / tau) * (-1 + tauPhiRL_tau + deltaLtauPhiR_deltaLtau);

				double fugrV = (deltaV / tau) * Math.Exp(PhiRV + deltaVPhiR_deltaV);
				double fugrV_deltaV = (fugrV / deltaV) * (1 + 2 * deltaVPhiR_deltaV + deltaVdeltaVPhiR_deltaVdeltaV);
				double fugrV_tau = (fugrV / tau) * (-1 + tauPhiRV_tau + deltaVtauPhiR_deltaVtau);

				// now the equations
				var f1 = prL - prGiven; // equality of liquid pressure with given pressure
				var f2 = prV - prGiven; // equality of vapor pressure with given pressure
				var f3 = fugrL - fugrV; // equality of fugacities

				var a11 = prL_deltaL; // derivative of prL w.r.t. deltaL
															// var a12 = 0.0; // derivative of prL w.r.t. deltaV is 0
				var a13 = prL_tau; // derivative of prL w.r.t. tau
													 // var a21 = 0.0; // derivative of prV w.r.t. deltaL is 0
				var a22 = prV_deltaV; // derivative of prV w.r.t. deltaV
				var a23 = prV_tau; // derivative of prV w.r.t. tau
				var a31 = fugrL_deltaL; // derivative of f3 w.r.t. deltaL
				var a32 = -fugrV_deltaV; // derivative of f3 w.r.t deltaV
				var a33 = fugrL_tau - fugrV_tau; // derivative of f2 w.r.t. tau

				// determinant, taking a12=0 and a21=0 into account
				var determinant = a11 * a22 * a33 - a11 * a23 * a32 - a13 * a22 * a31;

				var i11 = (a22 * a33 - a23 * a32) / determinant;
				var i12 = (a13 * a32) / determinant;
				var i13 = (-a13 * a22) / determinant;
				var i21 = (a23 * a31) / determinant;
				var i22 = (-a13 * a31 + a11 * a33) / determinant;
				var i23 = (-a11 * a23) / determinant;
				var i31 = (-a22 * a31) / determinant;
				var i32 = (-a11 * a32) / determinant;
				var i33 = (a11 * a22) / determinant;

				// now calculate a new guess of (deltaL, deltaV, tau)

				deltaL -= a11 * f1 + a13 * f3;
				deltaV -= a22 * f2 + a23 * f3;
				tau -= a31 * f1 + a32 * f2 + a33 * f3;
			} // iteration

			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the saturated liquid mole density, the saturated vapor mole density, and the pressure for a given temperature.
		/// This is done by iteration, using multivariate Newton-Raphson.
		/// </summary>
		/// <param name="temperature">The temperature in Kelvin. Must be greater than or equal the triple point temperature and less than or equal to the critical point temperature.</param>
		/// <returns>A tuple consisting of the liquid mole density (mol/m³), the vapor mole density (mol/m³), and the pressure in Pa.</returns>
		/// <exception cref="NotImplementedException"></exception>
		public (double liquidMoleDensity, double vaporMoleDensity, double pressure) SaturatedLiquidAndVaporMoleDensitiesAndPressure_FromTemerature(double temperature)
		{
			double pressureGuess = SaturatedVaporPressureEstimate_FromTemperature(temperature);
			double liquidMoleDensity = SaturatedLiquidMoleDensityEstimate_FromTemperature(temperature);
			double vaporMoleDensity = SaturatedVaporMoleDensityEstimate_FromTemperature(temperature);

			// now we have some initial values, we can begin to iterate ...
			// two equations to solve simultaneously (variables rho_liquid, rho_vapor, T):
			// p(rho_liquid, T) - p(rho_vapor, T) = 0 (equality of liquid pressure and vapor pressure)
			// fugacitycoeff(rho_liquid, T)*p(rho_liquid, T) - fugacitycoeff(rho_vapor, T)*p(rho_vapor, T) = 0 (equality of fugacities)

			// for convenience, we work with reduced variables:

			double tau = ReducingTemperature / temperature; // reduced temperature
			double deltaL = liquidMoleDensity / ReducingMoleDensity; // reduced liquid density
			double deltaV = vaporMoleDensity / ReducingMoleDensity; // reduced vapor density
			double reducingPressure = (WorkingUniversalGasConstant * ReducingTemperature * ReducingMoleDensity); // we calculate everything with reduced pressure prL and prV. This is the reducing pressure.
			double prL = 0, prV = 0; // reduced pressures for liquid and vapor.

			double dampingFactor = 1;

			double deltaL_prev = deltaL, deltaV_prev = deltaV, f1_prev = 0, f2_prev = 0;

			for (int i = 0; i < 1000; ++i)
			{
				var PhiRL = PhiR_OfReducedVariables(deltaL, tau);
				var deltaLPhiR_deltaL = deltaL * PhiR_delta_OfReducedVariables(deltaL, tau);
				var deltaLdeltaLPhiR_deltaLdeltaL = deltaL * deltaL * PhiR_deltadelta_OfReducedVariables(deltaL, tau);

				var PhiRV = PhiR_OfReducedVariables(deltaV, tau);
				var deltaVPhiR_deltaV = deltaV * PhiR_delta_OfReducedVariables(deltaV, tau);
				var deltaVdeltaVPhiR_deltaVdeltaV = deltaV * deltaV * PhiR_deltadelta_OfReducedVariables(deltaV, tau);

				// we also work with a reduced pressure pr = pressure/(R*TCrit*RhoCrit)
				prL = (deltaL / tau) * (1 + deltaLPhiR_deltaL); // Reduced liquid pressure
				prV = (deltaV / tau) * (1 + deltaVPhiR_deltaV); // Reduced vapor pressure

				// Derivatives of reduced pressure with respect to delta and tau
				double prL_deltaL = (1 + 2 * deltaLPhiR_deltaL + deltaLdeltaLPhiR_deltaLdeltaL) / tau;

				double prV_deltaV = (1 + 2 * deltaVPhiR_deltaV + deltaVdeltaVPhiR_deltaVdeltaV) / tau;

				// we also don't use the Fugacity directly (fucacityCoefficent*pressure), but instead
				// we use the reduced fugacity fugr = (fugacityCoefficient*reducedPressure)
				// now derivatives of reduced fugacity w.r.t. delta and tau

				double fugrL = (deltaL / tau) * Math.Exp(PhiRL + deltaLPhiR_deltaL);
				double fugrL_deltaL = (fugrL / deltaL) * (1 + 2 * deltaLPhiR_deltaL + deltaLdeltaLPhiR_deltaLdeltaL);

				double fugrV = (deltaV / tau) * Math.Exp(PhiRV + deltaVPhiR_deltaV);
				double fugrV_deltaV = (fugrV / deltaV) * (1 + 2 * deltaVPhiR_deltaV + deltaVdeltaVPhiR_deltaVdeltaV);

				// now the equations
				var f1 = prL - prV; // equality of liquid pressure with given pressure
				var f2 = fugrL - fugrV; // equality of fugacities

				// compare this with previous values: if greater, decrease dampingFactor and return to previous values

				if (i > 0)
				{
					if (Math.Abs(f1) > Math.Abs(f1_prev) || Math.Abs(f2) > Math.Abs(f2_prev))
					{
						deltaL = deltaL_prev;
						deltaV = deltaV_prev;
						dampingFactor = dampingFactor / 4;
						continue;
					}

					if (Math.Abs(f1) < Math.Abs(f1_prev) && Math.Abs(f2) < Math.Abs(f2_prev))
					{
						dampingFactor = Math.Min(1, dampingFactor * Math.Sqrt(2));

						continue;
					}
				}

				deltaL_prev = deltaL;
				deltaV_prev = deltaV;
				f1_prev = f1;
				f2_prev = f2;

				var a11 = prL_deltaL; // derivative of f1 w.r.t. deltaL
				var a12 = -prV_deltaV; // derivative of f1 w.r.t. deltaV
				var a21 = fugrL_deltaL; // derivative of f2 w.r.t. deltaL
				var a22 = -fugrV_deltaV; // derivative of f2 w.r.t deltaV

				// determinant, taking a12=0 and a21=0 into account
				var determinant = a11 * a22 - a12 * a21;

				var i11 = a22 / determinant;
				var i12 = -a12 / determinant;
				var i21 = -a21 / determinant;
				var i22 = a11 / determinant;

				// now calculate a new guess of (deltaL, deltaV, tau)

				var deltaLDiff = a11 * f1 + a12 * f2;
				var deltaVDiff = a21 * f1 + a22 * f2;

				deltaL -= dampingFactor * deltaLDiff;
				deltaV -= dampingFactor * deltaVDiff;
			} // iteration

			return (deltaL * ReducingMoleDensity, deltaV * ReducingMoleDensity, 0.5 * (prL + prV) * reducingPressure);
		}

		#endregion Equilibrium calculations
	}
}
