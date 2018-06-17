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
	/// State equations and constants of carbon dioxide.
	/// Reference:
	/// R. Span and W. Wagner,
	/// A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa,
	/// J. Phys. Chern. Ref. Data, Vol. 25, No.6, 1996
	/// </summary>
	public class CarbonDioxide : HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl
	{
		/// <summary>
		/// Gets the (only) instance of this class.
		/// </summary>
		public static CarbonDioxide Instance { get; } = new CarbonDioxide();

		public override double WorkingUniversalGasConstant => 8.31451;

		#region Constants for carbon dioxide

		/// <summary>Gets the triple point temperature.</summary>
		public override double TriplePointTemperature { get; } = 216.592;

		/// <summary>Gets the triple point pressure.</summary>
		public override double TriplePointPressure { get; } = 517950;

		/// <summary>Gets the saturated liquid density at the triple point.</summary>
		public override double TriplePointSaturatedLiquidMassDensity { get; } = 1178.53;

		/// <summary>Gets the saturated vapor density at the triple point.</summary>
		public override double TriplePointSaturatedVaporMassDensity { get; } = 13.7614;

		/// <summary>Gets the temperature at the critical point.</summary>
		public override double CriticalPointTemperature { get; } = 304.1282;

		/// <summary>Gets the pressure at the critical point.</summary>
		public override double CriticalPointPressure { get; } = 7.3773E6;

		public override double CriticalPointMoleDensity { get; } = 10624.9063;

		/// <summary>
		/// Gets the molecular weight.
		/// </summary>
		public override double MolecularWeight { get; } = 44.0098E-3; // kg/mol

		#endregion Constants for carbon dioxide

		private CarbonDioxide()
		{
			#region Ideal part of dimensionless Helmholtz energy and derivatives

			_alpha0_n_const = -6.1248710633532329122805;
			_alpha0_n_tau = 5.11559631859617732106;
			_alpha0_n_lntau = 2.50000000;

			/// <summary>
			/// Page 1540 Table 27
			/// </summary>
			_alpha0_n_Exp = new double[]
			{
			1.99427042,
			0.621052475,
			0.411952928,
			1.04028922,
			0.0832767753,
			};

			/// <summary>
			/// Page 1540 Table 27
			/// </summary>
			_alpha0_theta_Exp = new double[]
			{
			958.49956/CriticalPointTemperature,
			1858.80115/CriticalPointTemperature,
			2061.10114/CriticalPointTemperature,
			3443.89908/CriticalPointTemperature,
			8238.20035/CriticalPointTemperature,
			};

			#endregion Ideal part of dimensionless Helmholtz energy and derivatives

			#region Residual part of dimensionless Helmholtz energy and derivatives

			#region Parameter from Table 31, page 1544

			#region Index 1..7 of Table 31, page 1544

			_ni1 = new double[]
			{
			0.388568232032e00 ,
			0.293854759427e01,
			-0.558671885349e01,
			-0.767531995925e00,
			 0.317290055804e00,
			 0.548033158978e00,
			 0.122794112203e00,
			};

			_di1 = new int[]
			{
			1,  // Index 1 in table
			1,
			1,
			1,
			2,
			2,
			3,
			};

			_ti1 = new double[]
			{
			0,  // Index 1 in table
			0.75,
			1,
			2,
			0.75,
			2,
			0.75,
			};

			#endregion Index 1..7 of Table 31, page 1544

			#region Index 8..34 of Table 31, page 1544

			_ni2 = new double[]
			{
			 0.216589615432e01,
			 0.158417351097e01 ,
			-0.231327054055e00 ,
			 0.581169164314e-01 ,
			-0.553691372054e00 ,
			 0.489466159094e00 ,
			-0.242757398435e-01 ,
			 0.624947905017e-01 ,
			-0.121758602252e00 ,
			-0.370556852701e00 ,
			-0.167758797004e-01 ,
			-0.119607366380e00 ,
			-0.456193625088e-01 ,
			 0.356127892703e-01 ,
			-0.744277271321e-02 ,
			-0.173957049024e-02 ,
			-0.218101212895e-01 ,
			 0.243321665592e-01 ,
			-0.374401334235e-01 ,
			 0.143387157569e00 ,
			-0.134919690833e00 ,
			-0.231512250535e-01 ,
			 0.123631254929e-01 ,
			 0.210583219729e-02 ,
			-0.339585190264e-03 ,
			 0.559936517716e-02 ,
			-0.303351180556e-03 ,
			};

			_di2 = new int[]
			{
			1,  // Index 8 in table
			2,
			4,
			5,
			5,
			5,
			6,
			6,
			6,
			1,
			1,
			4,
			4,
			4,
			7,
			8,
			2,
			3,
			3,
			5,
			5,
			6,
			7,
			8,
			10,
			4,
			8,
			};

			_ti2 = new double[]
			{
			1.50,  // Index 8 in table
			1.50,
			2.50,
			0.00,
			1.50,
			2.00,
			0.00,
			1.00,
			2.00,
			3.00,
			6.00,
			3.00,
			6.00, // Index 20
			8.00,
			6.00,
			0.00,
			7.00,
			12.00,
			16.00,
			22.00,
			24.00,
			16.00,
			24.00, // Index 30
			8.00,
			2.00,
			28.00,
			14.00,
			};

			_ci2 = new int[]
			{
			1,  // Index 8 in table
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			2, // Index 17
			2,
			2,
			2,
			2,
			2,
			2,
			3, // Index 24
			3,
			3,
			4,
			4,
			4,
			4,
			4,
			4,
			5,
			6,
			};

			#endregion Index 8..34 of Table 31, page 1544

			#region Index 35..39 of Table 31, page 1544

			_ni3 = new double[]
			{
			-0.213654886883e03 ,
			 0.266415691493e05 ,
			-0.240272122046e05 ,
			-0.283416034240e03 ,
			 0.212472844002e03 ,
			};

			_di3 = new int[]
			{
			2,   // Index 35 in table
			2,
			2,
			3,
			3,
			};

			_ti3 = new double[]
			{
			1.00,   // Index 35 in table
			0.00,
			1.00,
			3.00,
			3.00,
			};

			_alphai3 = new double[]
			{
			25,   // Index 35 in table
			25,
			25,
			15,
			20,
			};

			_betai3 = new double[]
			{
			325,   // Index 35 in table
			300,
			300,
			275,
			275,
			};

			_gammai3 = new double[]
			{
			1.16,  // Index 35 in table
			1.19,
			1.19,
			1.25,
			1.22,
			};

			_epsiloni3 = new double[]
			{
			1,   // Index 35 in table
			1,
			1,
			1,
			1,
			};

			#endregion Index 35..39 of Table 31, page 1544

			#region Index 40..42 of Table 31, page 1544

			_ni4 = new double[]
			{
			-0.666422765408e00 , // Index 40 in table
			 0.726086323499e00 ,
			 0.550686686128e-01 ,
			};

			_ai4 = new double[]
			{
			3.5,  // Index 40 in table
			3.5,
			3,
			};

			_bi4 = new double[]
			{
			0.875,  // Index 40 in table
			0.925,
			0.875,
			};

			_betai4 = new double[]
			{
			0.300, // Index 40 in table
			0.300,
			0.300,
			};

			_Ai4 = new double[]
			{
			0.7, // Index 40 in table
			0.7,
			0.7,
			};

			_Bi4 = new double[]
			{
			0.3, // Index 40 in table
			0.3,
			1,
			};

			_Ci4 = new double[]
			{
			10, // Index 40 in table
			10,
			12.5,
			};

			_Di4 = new double[]
			{
			275, // Index 40 in table
			275,
			275,
			};

			#endregion Index 40..42 of Table 31, page 1544

			#endregion Parameter from Table 31, page 1544

			#endregion Residual part of dimensionless Helmholtz energy and derivatives

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
			const double a1 = 1955.5390;
			const double a2 = 2055.4593;

			var Tr = temperature_Kelvin / TriplePointTemperature - 1;

			var pr = a2 * (Tr + a1) * Tr + 1;
			return pr * TriplePointPressure;
		}

		/// <summary>
		/// Get the sublimation pressure at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The sublimation pressure in Pa.</returns>
		public double SublimationPressureAtTemperature(double temperature_Kelvin)
		{
			const double a1 = -14.740846, a2 = 2.4327015, a3 = -5.3061778;
			var Tr = 1 - temperature_Kelvin / TriplePointTemperature;
			var ln_pr = temperature_Kelvin / TriplePointTemperature * (a1 * Tr + a2 * Math.Pow(Tr, 1.9) + a3 * Math.Pow(Tr, 2.9));
			return Math.Exp(ln_pr) * TriplePointPressure;
		}

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
				a1 = -7.0602087,
				a15 = 1.9391218,
				a2 = -1.6463597,
				a4 = -3.2995634;

			var ln_pr = ((a4 * (Tr) * Tr + a2) * Tr + a1) * Tr + a15 * Tr * Math.Sqrt(Tr);
			ln_pr *= CriticalPointTemperature / temperature_Kelvin;
			double pressure = Math.Exp(ln_pr) * CriticalPointPressure;
			var deriv = (((4 * a4 * Tr) * Tr + 2 * a2) * Tr + a1) + Math.Sqrt(Tr) * (1.5 * a15);

			return (pressure, (-pressure / temperature_Kelvin) * (ln_pr + deriv));
		}

		/// <summary>
		/// Get the saturated liquid density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedLiquidDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = 1.9245108, a2 = -0.62385555, a3 = -0.32731127, a4 = 0.39245142;
			const double t1 = 0.34, t2 = 0.5, t3 = 10 / 6.0, t4 = 11 / 6.0;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;

			var ln_rhoR = a1 * Math.Pow(Tr, t1) + a2 * Math.Pow(Tr, t2) + a3 * Math.Pow(Tr, t3) + a4 * Math.Pow(Tr, t4);
			return Math.Exp(ln_rhoR) * CriticalPointMassDensity;
		}

		/// <summary>
		/// Get the saturated vapor density at a given temperature.
		/// </summary>
		/// <param name="temperature_Kelvin">The temperature in Kelvin.</param>
		/// <returns>The saturated liquid density in kg/m³.</returns>
		public double SaturatedVaporDensityAtTemperature(double temperature_Kelvin)
		{
			const double a1 = -1.7074879,
										a2 = -0.82274670,
										a3 = -4.6008549,
										a4 = -10.111178,
										a5 = -29.742252;
			const double t1 = 0.340,
										t2 = 0.5,
										t3 = 1,
										t4 = 7 / 3.0,
										t5 = 14 / 3.0;

			if (!(temperature_Kelvin <= CriticalPointTemperature))
				return double.NaN;

			var Tr = 1 - temperature_Kelvin / CriticalPointTemperature;
			var ln_rhoR = a1 * Math.Pow(Tr, t1) + a2 * Math.Pow(Tr, t2) + a3 * Math.Pow(Tr, t3) + a4 * Math.Pow(Tr, t4) + a5 * Math.Pow(Tr, t5);
			return Math.Exp(ln_rhoR) * CriticalPointMassDensity;
		}

		#endregion Thermodynamic properties by empirical power laws
	}
}
