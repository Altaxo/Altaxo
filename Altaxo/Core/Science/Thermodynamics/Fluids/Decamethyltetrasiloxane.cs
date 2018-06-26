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
	/// State equations and constants for Decamethyltetrasiloxane
	/// Reference:
	/// Monika Thol, Frithjof H. Dubberke, Elmar Baumhögger, Jadran Vrabec, Roland Span,
	/// Speed of Sound Measurements and Fundamental Equations of State for Octamethyltrisiloxane and Decamethyltetrasiloxane,
	/// J. Chem. Eng. Data, 2017, 62 (9), pp 2633–2648
	/// </summary>
	public class Decamethyltetrasiloxane : HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl
	{
		/// <summary>
		/// Gets the (only) instance of this class.
		/// </summary>
		public static Decamethyltetrasiloxane Instance { get; } = new Decamethyltetrasiloxane();

		#region Constants

		/// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
		public override double WorkingUniversalGasConstant => 8.3144621;

		/// <summary>Gets the molecular weight in kg/mol.</summary>
		public override double MolecularWeight { get; } = 310.6854E-3; // kg/mol

		/// <summary>Gets the triple point temperature.</summary>
		public override double TriplePointTemperature { get; } = 205.2; // Table 8 page 16

		/// <summary>Gets the triple point pressure.</summary>
		public override double TriplePointPressure { get; } = 0.0003127;

		/// <summary>Gets the saturated liquid density at the triple point.</summary>
		public override double TriplePointSaturatedLiquidMassDensity { get; } = 941.9981328;

		/// <summary>Gets the saturated vapor density at the triple point.</summary>
		public override double TriplePointSaturatedVaporMassDensity { get; } = double.NaN;

		/// <summary>Gets the temperature at the critical point.</summary>
		public override double CriticalPointTemperature { get; } = 599.4;

		/// <summary>Gets the pressure at the critical point.</summary>
		public override double CriticalPointPressure { get; } = 1.144E6;

		/// <summary>Gets the mole density at the critical point in mol/m³.</summary>
		public override double CriticalPointMoleDensity { get; } = 864;

		#endregion Constants

		private Decamethyltetrasiloxane()
		{
			#region Ideal part of dimensionless Helmholtz energy and derivatives

			// <summary>The constant term in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_const = 131.089725009572;

			// <summary>The term with the factor tau in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_tau = -26.3839137983442;

			// <summary>The term with the factor ln(tau) in the equation of the ideal part of the reduced Helmholtz energy.</summary>
			_alpha0_n_lntau = 3;

			_alpha0_Exp = new(double ni, double thetai)[]
			{
					(               28.59,                   20),
					(               56.42,                 1180),
					(               50.12,                 4240),
			};
			RescaleAlpha0ExpThetaWithCriticalTemperature();

			#endregion Ideal part of dimensionless Helmholtz energy and derivatives

			#region Residual part of dimensionless Helmholtz energy and derivatives

			#region Parameter from Table 6

			#region Index 1..5 of Table 6

			_pr1 = new(double ni, double ti, int di)[]
			{
					(          0.01458333,                    1,                    4),
					(            3.227554,                0.319,                    1),
					(           -3.503565,                0.829,                    1),
					(           -2.017391,                 0.78,                    2),
					(           0.8606129,                0.687,                    3),
			};

			#endregion Index 1..5 of Table 6

			#region Index 7..10 of Table 6

			_pr2 = new(double ni, double ti, int di, int li)[]
			{
					(           -2.196015,                 1.29,                    1,                    2),
					(          -0.9289014,                 3.91,                    3,                    2),
					(             2.02774,                 0.77,                    2,                    1),
					(          -0.9168439,                3.055,                    2,                    2),
					(         -0.06383507,                1.013,                    7,                    1),
			};

			#endregion Index 7..10 of Table 6

			#region Index 11..15 of Table 6

			_pr3 = new(double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
			{
					(            2.674255,                 1.07,                    1,               -0.982,              -0.7323,                1.042,                0.874),
					(          0.04662529,                 1.89,                    1,                 -2.7,               -0.543,                  1.1,                 1.43),
					(          -0.3835361,                1.133,                    3,               -1.347,                -1.26,                1.146,                0.855),
					(          -0.4273462,                0.826,                    2,               -0.864,               -0.878,                1.085,                0.815),
					(           -1.148009,                 0.83,                    2,               -1.149,                -2.22,               0.6844,                0.491),
			};

			#endregion Index 11..15 of Table 6

			#region Empty term 4

			_pr4 = new(double ni, double b, double beta, double A, double C, double D, double B, double a)[]
				{
				};

			#endregion Empty term 4

			#endregion Parameter from Table 6

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
				a1 = -10.174, a2 = 9.607, a3 = -10.08, a4 = -7.242, a5 = -30.56;
			const double
				t2 = 1.50, t3 = 1.83, t4 = 4.15, t5 = 17.8;
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
			const double a1 = 8.215, a2 = -24.65, a3 = 47.23, a4 = -42.44, a5 = 15.18;
			const double t1 = 0.498, t2 = 0.855, t3 = 1.220, t4 = 1.600, t5 = 2.040;

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
			const double a1 = -4.5483, a2 = -101.989, a3 = 224.06, a4 = -182.79, a5 = -110.45, a6 = -330.87;
			const double t1 = 0.428, t2 = 2.320, t3 = 2.800, t4 = 3.300, t5 = 8.500, t6 = 17.50;

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
