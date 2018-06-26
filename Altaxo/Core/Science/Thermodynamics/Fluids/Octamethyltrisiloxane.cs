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
	/// State equations and constants for Octamethyltrisiloxane
	/// Reference:
	/// Monika Thol, Frithjof H. Dubberke, Elmar Baumhögger, Jadran Vrabec, Roland Span,
	/// Speed of Sound Measurements and Fundamental Equations of State for Octamethyltrisiloxane and Decamethyltetrasiloxane,
	/// J. Chem. Eng. Data, 2017, 62 (9), pp 2633–2648
	/// </summary>
	public class Octamethyltrisiloxane : HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl
	{
		/// <summary>
		/// Gets the (only) instance of this class.
		/// </summary>
		public static Octamethyltrisiloxane Instance { get; } = new Octamethyltrisiloxane();

		#region Constants

		public override double WorkingUniversalGasConstant => 8.3144621;

		/// <summary>
		/// Gets the molecular weight.
		/// </summary>
		public override double MolecularWeight { get; } = 236.53146E-3; // kg/mol

		/// <summary>Gets the triple point temperature.</summary>
		public override double TriplePointTemperature { get; } = 187.2; // Table 8

		/// <summary>Gets the triple point pressure.</summary>
		public override double TriplePointPressure { get; } = 0.0010815;

		/// <summary>Gets the saturated liquid density at the triple point.</summary>
		public override double TriplePointSaturatedLiquidMassDensity { get; } = 924.1285705;

		/// <summary>Gets the saturated vapor density at the triple point.</summary>
		public override double TriplePointSaturatedVaporMassDensity { get; } = double.NaN;

		/// <summary>Gets the temperature at the critical point.</summary>
		public override double CriticalPointTemperature { get; } = 565.3609;

		/// <summary>Gets the pressure at the critical point.</summary>
		public override double CriticalPointPressure { get; } = 1.4375E6;

		/// <summary>Gets the density at the critical point.</summary>
		public override double CriticalPointMoleDensity { get; } = 1.134E3;

		#endregion Constants

		private Octamethyltrisiloxane()
		{
			#region Ideal part of dimensionless Helmholtz energy and derivatives

			// <summary>The constant term in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_const = 117.994606421838;

			// <summary>The term with the factor tau in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_tau = -19.6600754237831;

			// <summary>The term with the factor ln(tau) in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_lntau = 3;

			_alpha0_Exp = new(double ni, double thetai)[]
							{
 (28.817,     20.0),
 (46.951,   1570.0),
 (31.054,   4700.0),
							};
			RescaleAlpha0ExpThetaWithCriticalTemperature();

			#endregion Ideal part of dimensionless Helmholtz energy and derivatives

			#region Residual part of dimensionless Helmholtz energy and derivatives

			#region Parameter from Table 5

			#region Index 1..5 of Table 5

			_pr1 = new(double ni, double ti, int di)[]
			{
					(          0.05039724,                    1,                    4),
					(            1.189992,                0.188,                    1),
					(           -2.468723,                 1.03,                    1),
					(           -0.743856,                  0.7,                    2),
					(           0.4434056,                0.464,                    3),
			};

			#endregion Index 1..5 of Table 5

			#region Index 6..10 of Table 5

			_pr2 = new(double ni, double ti, int di, int li)[]
			{
					(           -1.371359,                2.105,                    1,                    2),
					(           -1.529621,                1.376,                    3,                    2),
					(           0.4445898,                  0.8,                    2,                    1),
					(           -1.009921,                  1.8,                    2,                    2),
					(         -0.05903694,                1.005,                    7,                    1),
			};

			#endregion Index 6..10 of Table 5

			#region Index 11..15 of Table 5

			_pr3 = new(double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
			{
					(            3.515188,                  0.7,                    1,               -0.986,               -0.966,                 1.25,                0.928),
					(          0.08367608,                 0.66,                    1,               -1.715,               -0.237,                1.438,                2.081),
					(            1.646856,                1.138,                    3,               -0.837,               -0.954,                0.894,                0.282),
					(          -0.2851917,                 1.56,                    2,               -1.312,               -0.861,                  0.9,                1.496),
					(           -2.457571,                 1.31,                    2,               -1.191,               -0.909,                0.899,                0.805),
			};

			#endregion Index 11..15 of Table 5

			#region Empty term 4

			_pr4 = new(double ni, double b, double beta, double A, double C, double D, double B, double a)[]
			{
			};

			#endregion Empty term 4

			#endregion Parameter from Table 5

			#endregion Residual part of dimensionless Helmholtz energy and derivatives
		}

		#region Thermodynamic properties by empirical power laws

		/// <summary>
		/// Gets the vapor pressure and its derivative with respect to temperature (in Pa and Pa/K).
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>Vapor pressure and its derivative with respect to temperature (in Pa and Pa/K)</returns>
		public override (double pressure, double pressureWrtTemperature) VaporPressureAndDerivativeWithRespectToTemperatureAtTemperature(double temperature_Kelvin)
		{
			if (!(TriplePointTemperature <= temperature_Kelvin && temperature_Kelvin <= CriticalPointTemperature))
				return (double.NaN, double.NaN);

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;

			const double
			a1 = -8.8192, a2 = 4.0952, a3 = -4.062, a4 = -6.208, a5 = -3.212;
			const double
				t2 = 1.50, t3 = 1.90, t4 = 3.71, t5 = 14.6;

			var ln_pr = a1 * Tr +
									a2 * Math.Pow(Tr, t2) +
									a3 * Math.Pow(Tr, t3) +
									a4 * Math.Pow(Tr, t4) +
									a5 * Math.Pow(Tr, t5);
			ln_pr *= CriticalPointTemperature / temperature_Kelvin;
			double pressure = Math.Exp(ln_pr) * CriticalPointPressure;
			var deriv = a1 +
									a2 * t2 * Math.Pow(Tr, t2 - 1) +
									a3 * t3 * Math.Pow(Tr, t3 - 1) +
									a4 * t4 * Math.Pow(Tr, t4 - 1) +
									a5 * t5 * Math.Pow(Tr, t5 - 1);

			return (pressure, (-pressure / temperature_Kelvin) * (ln_pr + deriv));
		}

		/// <summary>
		/// Get the saturated liquid density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedLiquidDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = 87.016, a2 = 13.924, a3 = 20.84, a4 = -16.64, a5 = 5.906;
			const double t1 = 0.54, t2 = 0.90, t3 = 1.30, t4 = 1.73, t5 = 2.20;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;

			var rhoR = a1 * Math.Pow(Tr, t1) +
										a2 * Math.Pow(Tr, t2) +
										a3 * Math.Pow(Tr, t3) +
										a4 * Math.Pow(Tr, t4) +
										a5 * Math.Pow(Tr, t5);
			return CriticalPointMassDensity * (rhoR + 1);
		}

		/// <summary>
		/// Get the saturated vapor density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedVaporDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = -5.3686, a2 = -11.85, a3 = -16.64, a4 = -52.26, a5 = -125.6, a6 = -235.7;
			const double t1 = 0.515, t2 = 4.580, t3 = 2.060, t4 = 5.250, t5 = 11.30, t6 = 21.60;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;
			var ln_rhoR = a1 * Math.Pow(Tr, t1) +
										a2 * Math.Pow(Tr, t2) +
										a3 * Math.Pow(Tr, t3) +
										a4 * Math.Pow(Tr, t4) +
										a5 * Math.Pow(Tr, t5) +
										a6 * Math.Pow(Tr, t6);
			return Math.Exp(ln_rhoR) * CriticalPointMassDensity;
		}

		#endregion Thermodynamic properties by empirical power laws
	}
}
