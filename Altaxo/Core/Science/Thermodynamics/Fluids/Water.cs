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
	/// State equations and constants of pure water.
	/// Reference:
	/// W. Wagner and A.Pruß
	/// The IAPWS Formulation 1995 for the Thermodynamic Properties	of Ordinary Water Substance for General and Scientific Use,
	/// J. Phys. Chem. Ref. Data, Vol. 31, No. 2, 2002
	/// </summary>
	public class Water : HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl
	{
		/// <summary>
		/// Gets the (only) instance of this class.
		/// </summary>
		public static Water Instance { get; } = new Water();

		#region Constants for water

		/// <summary>Gets the triple point temperature.</summary>
		public override double TriplePointTemperature { get; } = 273.16;

		/// <summary>Gets the triple point pressure.</summary>
		public override double TriplePointPressure { get; } = 611.657;

		/// <summary>Gets the saturated liquid density at the triple point.</summary>
		public override double TriplePointSaturatedLiquidDensity { get; } = 999.793;

		/// <summary>Gets the saturated vapor density at the triple point.</summary>
		public override double TriplePointSaturatedVaporDensity { get; } = 0.00485458;

		/// <summary>Gets the temperature at the critical point.</summary>
		public override double CriticalPointTemperature { get; } = 647.096;

		/// <summary>Gets the pressure at the critical point.</summary>
		public override double CriticalPointPressure { get; } = 22.064E6;

		/// <summary>Gets the density at the critical point.</summary>
		public override double CriticalPointDensity { get; } = 322;

		/// <summary>
		/// Gets the molecular weight.
		/// </summary>
		public override double MolecularWeight { get; } = 18.015268E-3; // kg/mol

		/// <summary>
		/// Gets the specific gas constant.
		/// </summary>
		public override double SpecificGasConstant { get; } = 461.51805; // J/(kg K)

		#endregion Constants for water

		private Water()
		{
			#region Ideal part of dimensionless Helmholtz energy and derivatives

			/// <summary>
			/// Page 429 Table 6.1 (n_i there is ai0 here)
			/// </summary>
			_ai0 = new double[]
			{
			double.NaN,
		 -8.32044648201,
			6.6832105268,
			3.00632,
			0.012436,
			0.97315,
			1.27950,
			0.96956,
			0.24873
			};

			/// <summary>
			/// Page 429 Table 6.1 (gamma_i there is thetai0 here)
			/// </summary>
			_thetai0 = new double[]
			{
			double.NaN,
			double.NaN,
			double.NaN,
			double.NaN,
			1.28728967,
			3.53734222,
			7.74073708,
			9.24437796,
			27.5075105,
			};

			#endregion Ideal part of dimensionless Helmholtz energy and derivatives

			#region Parameter from Table 6.2, page 430

			#region Index 1..7 of Table 6.2, page 430

			_ni1 = new double[]
			{
			0.12533547935523E-1,
			0.78957634722828E1,
		 -0.87803203303561E1,
			0.31802509345418,
		 -0.26145533859358,
		 -0.78199751687981E-2,
			0.88089493102134E-2,
			};

			_di1 = new int[]
			{
			1,
			1,
			1,
			2,
			2,
			3,
			4,
			};

			_ti1 = new double[]
			{
			-0.5,
			0.875,
			1,
			0.5,
			0.75,
			0.375,
			1,
			};

			#endregion Index 1..7 of Table 6.2, page 430

			#region Index 8..34 of Table 6.2, page 430

			_ni2 = new double[]
			{
			-0.66856572307965, // Index 8 in table
			0.20433810950965,
			-0.66212605039687E-4,
			-0.19232721156002,
			-0.25709043003438,
			0.16074868486251,
			-0.40092828925807E-1,
			0.39343422603254E-6,
			-0.75941377088144E-5,
			0.56250979351888E-3,
			-0.15608652257135E-4,
			0.11537996422951E-8,
			0.36582165144204E-6, // Index 20
			-0.13251180074668E-11,
			-0.62639586912454E-9,
			-0.10793600908932,
			0.17611491008752E-1,
			0.22132295167546,
			-0.40247669763528,
			0.58083399985759,
			0.49969146990806E-2,
			-0.31358700712549E-1,
			-0.74315929710341, // Index 30
			0.47807329915480,
			0.20527940895948E-1,
			-0.13636435110343,
			0.14180634400617E-1,
			0.83326504880713E-2,
			-0.29052336009585E-1,
			0.38615085574206E-1,
			-0.20393486513704E-1,
			-0.16554050063734E-2,
			0.19955571979541E-2, // Index 40
			0.15870308324157E-3,
			-0.16388568342530E-4,
			0.43613615723811E-1,
			0.34994005463765E-1,
			-0.76788197844621E-1,
			0.22446277332006E-1,
			-0.62689710414685E-4,
			-0.55711118565645E-9,
			-0.19905718354408,
			0.31777497330738, // Index 50
			-0.11841182425981,
			};

			_di2 = new int[]
			{
			1, // Index 8 in table
			1,
			1,
			2,
			2,
			3,
			4,
			4,
			5,
			7,
			9,
			10,
			11,
			13,
			15,
			1,
			2,
			2,
			2,
			3,
			4,
			4,
			4,
			5,
			6,
			6,
			7,
			9,
			9,
			9,
			9,
			9,
			10,
			10,
			12,
			3,
			4,
			4,
			5,
			14,
			3,
			6,
			6,
			6,
			};

			_ti2 = new double[]
			{
			4, // Index 8 in table
			6,
			12,
			1,
			5,
			4,
			2,
			13,
			9,
			3,
			4,
			11,
			4,
			13,
			1,
			7,
			1,
			9,
			10,
			10,
			3,
			7,
			10,
			10,
			6,
			10,
			10,
			1,
			2,
			3,
			4,
			8,
			6,
			9,
			8,
			16,
			22,
			23,
			23,
			10,
			50,
			44,
			46,
			50,
			};

			_ci2 = new int[]
			{
			1, // Index 8 in table
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			6,
			6,
			6,
			6,
			};

			#endregion Index 8..34 of Table 6.2, page 430

			#region Index 52..54 of Table 6.2, page 430

			_ni3 = new double[]
			{
			 -0.31306260323435E2, // Index 52 in table
			 0.31546140237781E2,
			 -0.25213154341695E4,
			};

			_di3 = new int[]
			{
			3, // Index 52 in table
			3,
			3,
			};

			_ti3 = new double[]
			{
			0, // Index 52 in table
			1,
			4,
			};

			_alphai3 = new int[]
			{
			20, // Index 52 in table
			20,
			20,
			};

			_betai3 = new int[]
			{
			150, // Index 52 in table
			150,
			250,
			};

			_gammai3 = new double[]
			{
			1.21, // Index 52 in table
			1.21,
			1.25,
			};

			_epsiloni3 = new int[]
			{
			1, // Index 52 in table
			1,
			1,
			};

			#endregion Index 52..54 of Table 6.2, page 430

			#region Index 55..56 of Table 6.2, page 340

			_ni4 = new double[]
			{
		 -0.14874640856724, // Index 55 in table
			0.31806110878444,
			};

			_ai4 = new double[]
			{
			3.5, // Index 55 in table
			3.5,
			};

			_bi4 = new double[]
			{
			0.85, // Index 55 in table
			0.95,
			};

			_betai4 = new double[]
			{
			0.3, // Index 55 in table
			0.3,
			};

			_Ai4 = new double[]
			{
			0.32, // Index 55 in table
			0.32,
			};

			_Bi4 = new double[]
			{
			0.2, // Index 55 in table
			0.2,
				};

			_Ci4 = new double[]
			{
			28, // Index 55 in table
			32,
			};

			_Di4 = new double[]
			{
			700, // Index 55 in table
			800,
			};

			#endregion Index 55..56 of Table 6.2, page 340

			#endregion Parameter from Table 6.2, page 430

			TestArrays();
		}

		#region Thermodynamic properties by empirical power laws

		/// <summary>
		/// Get the melting pressure at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The melting pressure in Pa.</returns>
		public double MeltingPressureAtTemperature(double temperature_Kelvin)
		{
			double Tr;
			double? pr1 = null, pr3 = null, pr5 = null, pr6 = null, pr7 = null;
			if (251.165 <= temperature_Kelvin && temperature_Kelvin <= 273.16) // Ice I
			{
				Tr = temperature_Kelvin / TriplePointTemperature;
				pr1 = 1 - 0.626E6 * (1 - Pow(Tr, -3)) + 0.197135E6 * (1 - Math.Pow(Tr, 21.2));
				pr1 *= TriplePointPressure;
			}

			if (251.165 <= temperature_Kelvin && temperature_Kelvin <= 256.164) // Ice III
			{
				Tr = temperature_Kelvin / 251.165;
				pr3 = 1 - 0.2952521 * (1 - Pow(Tr, 60));
				pr3 *= 209.9E6;
			}

			if (256.164 <= temperature_Kelvin && temperature_Kelvin <= 273.31) // Ice V
			{
				Tr = temperature_Kelvin / 256.164;
				pr5 = 1 - 1.18721 * (1 - Pow(Tr, 8));
				pr5 *= 350.1E6;
			}

			if (273.31 <= temperature_Kelvin && temperature_Kelvin <= 355) // Ice VI
			{
				Tr = temperature_Kelvin / 273.31;
				pr6 = 1 - 1.07476 * (1 - Math.Pow(Tr, 4.6));
				pr6 *= 632.4E6;
			}

			if (355 <= temperature_Kelvin && temperature_Kelvin <= 715) // Ice VII
			{
				Tr = temperature_Kelvin / 355;
				pr5 = 1.73683 * (1 - 1 / Tr) - 0.0544606 * (1 - Pow(Tr, 5)) + 0.806106E-7 * (1 - Pow(Tr, 22));
				pr5 *= 2216E6;
			}

			return pr1 ?? pr3 ?? pr5 ?? pr6 ?? pr7 ?? double.NaN;
		}

		/// <summary>
		/// Get the sublimation pressure at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The sublimation pressure in Pa.</returns>
		public double SublimationPressureAtTemperature(double temperature_Kelvin)
		{
			var Tr = temperature_Kelvin / TriplePointTemperature;
			var ln_pr = -13.928169 * (1 - Math.Pow(Tr, -1.5)) + 34.7078238 * (1 - Math.Pow(Tr, -1.25));
			return Math.Exp(ln_pr) * TriplePointPressure;
		}

		/// <summary>
		/// Get the vapor pressure at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The vapor pressure in Pa.</returns>
		public double VaporPressureAtTemperature(double temperature_Kelvin)
		{
			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;

			const double
				a1 = 7.85951783,
				a15 = 1.84408259,
				a3 = -11.7866497,
				a35 = 22.6807411,
				a4 = -15.9618719,
				a75 = 1.80122502;

			var ln_pr = (((a4 * Tr + a3) * Tr + 0) * Tr + a1) * Tr + Math.Sqrt(Tr) * (a15 * Tr + a35 * Pow(Tr, 3) + a75 * Pow(Tr, 7));
			ln_pr *= CriticalPointTemperature / temperature_Kelvin;
			return Math.Exp(ln_pr) * CriticalPointPressure;
		}

		/// <summary>
		/// Get the saturated liquid density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedLiquidDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = 1.99274064, a2 = 1.09965342, a3 = 0.510839303, a4 = -1.75493479, a5 = -45.5170352, a6 = -6.74694450E5;
			const double t1 = 1 / 3.0, t2 = 2 / 3.0, t3 = 5 / 3.0, t4 = 16 / 3.0, t5 = 43 / 3.0, t6 = 110 / 3.0;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;

			var ln_rhoR = a1 * Math.Pow(Tr, t1) + a2 * Math.Pow(Tr, t2) + a3 * Math.Pow(Tr, t3) + a4 * Math.Pow(Tr, t4) + a5 * Math.Pow(Tr, t5) + a6 * Math.Pow(Tr, t6);
			return Math.Exp(ln_rhoR) * CriticalPointDensity;
		}

		/// <summary>
		/// Get the saturated vapor density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedVaporDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = -2.03150240, a2 = -2.68302940, a3 = -5.38626492, a4 = -17.299, a5 = -44.7586581, a6 = -63.9201063;
			const double t1 = 2 / 6.0,
										t2 = 4 / 6.0,
										t3 = 8 / 6.0,
										t4 = 18 / 6.0,
										t5 = 37 / 6.0,
										t6 = 71 / 6.0;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;
			var ln_rhoR = a1 * Math.Pow(Tr, t1) + a2 * Math.Pow(Tr, t2) + a3 * Math.Pow(Tr, t3) + a4 * Math.Pow(Tr, t4) + a5 * Math.Pow(Tr, t5) + a6 * Math.Pow(Tr, t6);
			return Math.Exp(ln_rhoR) * CriticalPointDensity;
		}

		#endregion Thermodynamic properties by empirical power laws
	}
}
