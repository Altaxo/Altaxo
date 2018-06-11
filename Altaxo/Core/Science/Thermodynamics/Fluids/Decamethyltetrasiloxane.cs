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

		/// <summary>Gets the triple point temperature.</summary>
		public override double TriplePointTemperature { get; } = 205.2; // Table 8 page 16

		/// <summary>Gets the triple point pressure.</summary>
		public override double TriplePointPressure { get; } = double.NaN;

		/// <summary>Gets the saturated liquid density at the triple point.</summary>
		public override double TriplePointSaturatedLiquidDensity { get; } = 941.9981328;

		/// <summary>Gets the saturated vapor density at the triple point.</summary>
		public override double TriplePointSaturatedVaporDensity { get; } = double.NaN;

		/// <summary>Gets the temperature at the critical point.</summary>
		public override double CriticalPointTemperature { get; } = 599.4;

		/// <summary>Gets the pressure at the critical point.</summary>
		public override double CriticalPointPressure { get; } = 1.144E6;

		/// <summary>Gets the density at the critical point.</summary>
		public override double CriticalPointMassDensity { get; } = 268.4321856;

		/// <summary>Gets the density at the critical point.</summary>
		public override double CriticalPointMoleDensity { get; } = 268.4321856 / 310.6854E-3;

		/// <summary>
		/// Gets the molecular weight.
		/// </summary>
		public override double MolecularWeight { get; } = 310.6854E-3; // kg/mol

		/// <summary>
		/// Gets the specific gas constant.
		/// </summary>
		public override double SpecificGasConstant { get; } = 26.761669032403841; // J/(kg K)

		#endregion Constants

		private Decamethyltetrasiloxane()
		{
			#region Ideal part of dimensionless Helmholtz energy and derivatives

			/// <summary>
			/// Page 1540 Table 27
			/// </summary>
			_ai0 = new double[]
			{
			double.NaN,
			131.089725,
		 -26.3839138,
			3,
			28.59,
			56.42,
			50.12,
			};

			/// <summary>
			/// Page 1540 Table 27
			/// </summary>
			_thetai0 = new double[]
			{
			double.NaN,
			double.NaN,
			double.NaN,
			double.NaN,
			20/599.4,
			1180/599.4,
			4240/599.4,
			};

			#endregion Ideal part of dimensionless Helmholtz energy and derivatives

			#region Residual part of dimensionless Helmholtz energy and derivatives

			#region Parameter from Table 6

			#region Index 1..5 of Table 6

			_ni1 = new double[]
			{
				1.458333E-2,
				3.227554,
				-3.503565,
				-2.017391,
				8.606129E-1,
				};

			_di1 = new int[]
			{
				4,
				1,
				1,
				2,
				3,
			};

			_ti1 = new double[]
			{
			1.000,
			0.319,
			0.829,
			0.780,
			0.687,
			};

			#endregion Index 1..5 of Table 6

			#region Index 7..10 of Table 6

			_ni2 = new double[]
			{
				-2.196015,
				-9.289014E-1,
				2.027740,
				-9.168439E-1,
				-6.383507E-2,
			};

			_di2 = new int[]
			{
			1,
			3,
			2,
			2,
			7,
			};

			_ti2 = new double[]
			{
			1.290,
			3.910,
			0.770,
			3.055,
			1.013,
			};

			_ci2 = new int[]
			{
			2,
			2,
			1,
			2,
			1,
			};

			#endregion Index 7..10 of Table 6

			#region Index 11..15 of Table 6

			_ni3 = new double[]
			{
			2.674255,
			4.662529E-2,
			3.835361E-1,
			4.273462E-1,
			1.148009,
			};

			_di3 = new int[]
			{
			1,
			1,
			3,
			2,
			2,
			};

			_ti3 = new double[]
			{
			1.070,
			1.890,
			1.133,
			0.826,
			0.830,
			};

			_alphai3 = new double[]
			{
			0.982,
			2.700,
			1.347,
			0.864,
			1.149,
			};

			_betai3 = new double[]
			{
			0.7323,
			0.5430,
			1.2600,
			0.8780,
			2.2200,
			};

			_gammai3 = new double[]
			{
			1.0420,
			1.1000,
			1.1460,
			1.0850,
			0.6844,
			};

			_epsiloni3 = new double[]
			{
			0.874,
			1.430,
			0.855,
			0.815,
			0.491,
			};

			#endregion Index 11..15 of Table 6

			#region Empty term 4

			_ni4 = new double[]
			{
			};

			_ai4 = new double[]
			{
			};

			_bi4 = new double[]
			{
			};

			_betai4 = new double[]
			{
			};

			_Ai4 = new double[]
			{
			};

			_Bi4 = new double[]
			{
			};

			_Ci4 = new double[]
			{
			};

			_Di4 = new double[]
			{
			};

			#endregion Empty term 4

			#endregion Parameter from Table 6

			#endregion Residual part of dimensionless Helmholtz energy and derivatives

			TestArrays();
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
