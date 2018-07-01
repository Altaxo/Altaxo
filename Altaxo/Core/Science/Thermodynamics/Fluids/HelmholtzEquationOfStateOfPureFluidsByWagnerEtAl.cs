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
	/// Base class of state equations based on the papers of the group of W.Wagner / Bochum
	/// </summary>
	/// <remarks>
	/// References:
	/// <para>
	/// [1] R. Span and W. Wagner,
	/// A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa,
	/// J. Phys. Chern. Ref. Data, Vol. 25, No.6, 1996
	/// </para>
	/// <para>
	/// [2] W. Wagner and A.Pruß
	/// The IAPWS Formulation 1995 for the Thermodynamic Properties	of Ordinary Water Substance for General and Scientific Use,
	/// J. Phys. Chem. Ref. Data, Vol. 31, No. 2, 2002
	/// </para>
	/// </remarks>
	public abstract class HelmholtzEquationOfStateOfPureFluidsByWagnerEtAl : HelmholtzEquationOfStateOfPureFluids
	{
		#region Constants

		/// <summary>The full name of the fluid.</summary>
		public abstract string FullName { get; }

		/// <summary>The short name of the fluid.</summary>
		public abstract string ShortName { get; }

		/// <summary>The synonym of the name of the fluid.</summary>
		public abstract string Synonym { get; }

		/// <summary>The chemical formula of the fluid.</summary>
		public abstract string ChemicalFormula { get; }

		/// <summary>The chemical formula of the fluid.</summary>
		public abstract string FluidFamily { get; }

		/// <summary>Gets the CAS number.</summary>
		public abstract string CASNumber { get; }

		/// <summary>The UN number of the fluid.</summary>
		public abstract int UN_Number { get; }

		/// <summary>Gets the dipole moment in Debye.</summary>
		public abstract double DipoleMoment { get; }

		/// <summary>Gets the lower temperature limit of this model in K.</summary>
		public abstract double LowerTemperatureLimit { get; }

		/// <summary>Gets the upper temperature limit of this model in K.</summary>
		public abstract double UpperTemperatureLimit { get; }

		/// <summary>Gets the upper density limit of this model in mol/m³.</summary>
		public abstract double UpperMoleDensityLimit { get; }

		/// <summary>Gets the upper pressure limit of this model in Pa.</summary>
		public abstract double UpperPressureLimit { get; }

		#endregion Constants

		#region Ideal part of dimensionless Helmholtz energy and derivatives

		/// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
		public override double WorkingUniversalGasConstant => UniversalGasConstant;

		/// <summary>The constant term in the equation of the ideal part of the reduced Helmholtz energy.</summary>
		protected double _alpha0_n_const;

		/// <summary>The term with the factor tau in the equation of the ideal part of the reduced Helmholtz energy.</summary>
		protected double _alpha0_n_tau;

		/// <summary>The term with the factor ln(tau) in the equation of the ideal part of the reduced Helmholtz energy.</summary>
		protected double _alpha0_n_lntau;

		// Exponential terms

		/// <summary>
		/// Prefactor and exponent of the exponential terms in the ideal part of the residual Helmholtz energy.
		/// Page 429 Table 6.1 in [2]
		/// </summary>
		protected (double ni, double thetai)[] _alpha0_Exp = _emptyDoubleDoubleArray;

		protected void RescaleAlpha0ExpThetaWithCriticalTemperature()
		{
			for (int i = 0; i < _alpha0_Exp.Length; ++i)
			{
				var (ni, thetai) = _alpha0_Exp[i];
				_alpha0_Exp[i] = (ni, thetai / CriticalPointTemperature);
			}
		}

		/// <summary>The prefactors outside and inside the argument of the  Cosh terms in the equation of the ideal part of the reduced Helmholtz energy.</summary>
		protected (double ni, double thetai)[] _alpha0__Cosh = _emptyDoubleDoubleArray;

		/// <summary>The prefactors outside and inside the argument of the  Sinh terms in the equation of the ideal part of the reduced Helmholtz energy.</summary>
		protected (double ni, double thetai)[] _alpha0__Sinh = _emptyDoubleDoubleArray;

		/// <summary>
		/// Phi0s the of reduced variables. (Page 1541, Table 28 in [2])
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns></returns>
		public override double Phi0_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;

			{
				// Exponential terms
				var alpha0_Exp = _alpha0_Exp;
				for (int i = 0; i < alpha0_Exp.Length; ++i)
				{
					var (n, theta) = alpha0_Exp[i];
					sum += n * Math.Log(1 - Math.Exp(-theta * tau));
				}
			}

			{
				// Cosh terms
				var alpha0_Cosh = _alpha0__Cosh;
				for (int i = 0; i < alpha0_Cosh.Length; ++i)
				{
					var (n, theta) = alpha0_Cosh[i];
					sum += n * Math.Log(Math.Cosh(theta * tau));
				}
			}

			{
				// Sinh terms
				var alpha0_Sinh = _alpha0__Sinh;
				for (int i = 0; i < alpha0_Sinh.Length; ++i)
				{
					var (n, theta) = alpha0_Sinh[i];
					sum += n * Math.Log(Math.Abs(Math.Sinh(theta * tau)));
				}
			}

			return Math.Log(delta) +

				(
				sum +
				_alpha0_n_const +
				_alpha0_n_tau * tau +
				_alpha0_n_lntau * Math.Log(tau)
				);
		}

		/// <summary>
		/// First derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature. (Page 1541, Table 28)
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns>First derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature.</returns>
		public override double Phi0_tau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;

			{
				// Exponential terms
				var alpha0_Exp = _alpha0_Exp;
				for (int i = 0; i < alpha0_Exp.Length; ++i)
				{
					var (n, theta) = alpha0_Exp[i];
					sum += n * theta * (1 / (1 - Math.Exp(-theta * tau)) - 1);
				}
			}
			{
				// Cosh terms
				var alpha0_Cosh = _alpha0__Cosh;
				for (int i = 0; i < alpha0_Cosh.Length; ++i)
				{
					var (n, theta) = alpha0_Cosh[i];
					sum += n * theta * Math.Tanh(theta * tau);
				}
			}
			{
				// Sinh terms
				var alpha0_Sinh = _alpha0__Sinh;
				for (int i = 0; i < alpha0_Sinh.Length; ++i)
				{
					var (n, theta) = alpha0_Sinh[i];
					sum += n * theta * Coth(theta * tau);
				}
			}
			return (sum + _alpha0_n_tau + _alpha0_n_lntau / tau);
		}

		/// <summary>
		/// Second derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature. (Page 1541, Table 28)
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns>Second derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature.</returns>
		public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			{
				// Exponential terms
				var alpha0_Exp = _alpha0_Exp;
				for (int i = 0; i < alpha0_Exp.Length; ++i)
				{
					var (n, theta) = alpha0_Exp[i];
					sum += -n * Pow2(theta) * Math.Exp(-theta * tau) / Pow2(1 - Math.Exp(-theta * tau));
				}
			}
			{
				// Cosh terms
				var alpha0_Cosh = _alpha0__Cosh;
				for (int i = 0; i < alpha0_Cosh.Length; ++i)
				{
					var (n, theta) = alpha0_Cosh[i];
					sum += n * Pow2(theta * Sech(theta * tau));
				}
			}

			{
				// Sinh terms
				var alpha0_Sinh = _alpha0__Sinh;
				for (int i = 0; i < alpha0_Sinh.Length; ++i)
				{
					var (n, theta) = alpha0_Sinh[i];
					sum += -n * Pow2(theta * Csch(theta * tau));
				}
			}
			return (sum - _alpha0_n_lntau / Pow2(tau));
		}

		#endregion Ideal part of dimensionless Helmholtz energy and derivatives

		#region Residual part of dimensionless Helmholtz energy and derivatives

		#region Parameter from Table, e.g. for Water: 6.2, page 430  in [2]

		#region 1st sum term

		/// <summary>
		/// Parameter for the polynomial terms of the reduced Helmholtz energy.
		/// </summary>
		protected (double ni, double ti, int di)[] _pr1;

		#endregion 1st sum term

		#region 2nd sum term

		/// <summary>
		/// Parameter for the exponential terms of the reduced Helmholtz energy.
		/// </summary>
		protected (double ni, double ti, int di, int li)[] _pr2;

		#endregion 2nd sum term

		#region 3rd sum term

		protected (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[] _pr3;

		#endregion 3rd sum term

		#region 4th sum term

		protected (double ni, double b, double beta, double A, double C, double D, double B, double a)[] _pr4;

		#endregion 4th sum term

		#endregion Parameter from Table, e.g. for Water: 6.2, page 430  in [2]

		/// <summary>
		/// Calculates the residual part of the dimensionless Helmholtz energy in dependence on reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = (density / density at the critical point).</param>
		/// <param name="tau">The reduced inverse temperature = (temperature at critical point / temperature).</param>
		/// <returns>The dimensionless Helmholtz energy.</returns>
		public override double PhiR_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * Pow(delta, di) * Math.Pow(tau, ti);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, li) = pr2[i];
				sum2 += ni * Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(-Pow(delta, li));
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai));
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];
				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));
				sum4 += ni * Math.Pow(Delta, bi) * delta * Psi;
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		/// <summary>
		/// Calculates the first derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density delta.
		/// </summary>
		/// <param name="delta">The reduced density.</param>
		/// <param name="tau">The reduced inverse temperature.</param>
		/// <returns>First derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density.</returns>
		public override double PhiR_delta_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * di * Pow(delta, di - 1) * Math.Pow(tau, ti);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, ci) = pr2[i];
				sum2 += ni * Math.Exp(-Pow(delta, ci)) * (Pow(delta, di - 1) * Math.Pow(tau, ti) * (di - ci * Pow(delta, ci)));
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai)) * (di / delta + 2 * alphai * (delta - epsiloni));
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];
				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));
				double Psi_delta = -2 * Ci * (delta - 1) * Psi;

				// Derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) * (Ai * theta * 2 / betai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 1) + 2 * Bi * ai * Math.Pow(Pow2(delta - 1), ai - 1));

				// Derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi * Math.Pow(Delta, bi - 1) * Delta_delta;

				sum4 += ni * (Math.Pow(Delta, bi) * (Psi + delta * Psi_delta) + Deltabi_delta * delta * Psi);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		/// <summary>
		/// Calculates the second derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density delta.
		/// </summary>
		/// <param name="delta">The reduced density.</param>
		/// <param name="tau">The reduced inverse temperature.</param>
		/// <returns>Second derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density.</returns>
		public override double PhiR_deltadelta_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * di * (di - 1) * Pow(delta, di - 2) * Math.Pow(tau, ti);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, li) = pr2[i];
				sum2 += ni * Math.Exp(-Pow(delta, li)) *
								(
								Pow(delta, di - 2) * Math.Pow(tau, ti) *
								((di - li * Pow(delta, li)) * (di - 1 - li * Pow(delta, li)) - Pow2(li) * Pow(delta, li))
								);
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Math.Pow(tau, ti) * Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai)) *
					(
					2 * alphai * Pow(delta, di) +
					4 * Pow2(alphai) * Pow(delta, di) * Pow2(delta - epsiloni) +
					4 * di * alphai * Pow(delta, di - 1) * (delta - epsiloni) +
					di * (di - 1) * Pow(delta, di - 2)
					);
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];

				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));
				double Psi_delta = -2 * Ci * (delta - 1) * Psi;
				double Psi_deltadelta = (2 * Ci * Pow2(delta - 1) - 1) * (2 * Ci * Psi); // 2nd derivative of Psi with respect to delta

				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);

				// 1st derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) *
				 (
				 Ai * theta * 2 / betai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 1) +
				 2 * Bi * ai * Math.Pow(Pow2(delta - 1), ai - 1)
				 );

				// 2nd derivative of Delta with respect to delta
				double Delta_deltadelta = Delta_delta / (delta - 1) + Pow2(delta - 1) *
				 (
					 4 * Bi * ai * (ai - 1) * Math.Pow(Pow2(delta - 1), ai - 2) +
					 2 * Pow2(Ai / betai) * Pow2(Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 1)) +
					 Ai * theta * 4 / betai * (1 / (2 * betai) - 1) * Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 2)
				 );

				// 1st derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi * Math.Pow(Delta, bi - 1) * Delta_delta;

				// 2nd derivative of Delta^bi with respect to delta
				double Deltabi_deltadelta = bi *
				 (
					 Math.Pow(Delta, bi - 1) * Delta_deltadelta +
					 (bi - 1) * Math.Pow(Delta, bi - 2) * Pow2(Delta_delta)
				 );

				sum4 += ni *
							(

								Math.Pow(Delta, bi) * (2 * Psi_delta + delta * Psi_deltadelta) +

								2 * Deltabi_delta * (Psi + delta * Psi_delta) +

								 Deltabi_deltadelta * delta * Psi
								);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		/// <summary>
		/// Calculates the first derivative of the residual part of the dimensionless Helmholtz energy with respect to the inverse reduced temperature.
		/// </summary>
		/// <param name="delta">The reduced density.</param>
		/// <param name="tau">The reduced inverse temperature.</param>
		/// <returns>First derivative of the residual part of the dimensionless Helmholtz energy with respect to the inverse reduced temperature.</returns>
		public override double PhiR_tau_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * ti * Pow(delta, di) * Math.Pow(tau, ti - 1);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, ci) = pr2[i];
				sum2 += ni * ti * Pow(delta, di) * Math.Pow(tau, ti - 1) * Math.Exp(-Pow(delta, ci));
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Pow(delta, di) * Math.Pow(tau, ti) *
								Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai)) *
								(ti / tau + 2 * betai * (tau - gammai));
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];
				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di * (tau - 1) * Psi;

				// Derivative of Delta^bi with respect to delta
				double Deltabi_tau = -2 * theta * bi * Math.Pow(Delta, bi - 1);

				sum4 += ni * delta * (
									Deltabi_tau * Psi +
									Math.Pow(Delta, bi) * Psi_tau
									);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		/// <summary>
		/// Calculates the second derivative of the residual part of the dimensionless Helmholtz energy with respect to the inverse reduced temperature.
		/// </summary>
		/// <param name="delta">The reduced density.</param>
		/// <param name="tau">The reduced inverse temperature.</param>
		/// <returns>Second derivative of the residual part of the dimensionless Helmholtz energy with respect to the inverse reduced temperature.</returns>
		public override double PhiR_tautau_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * ti * (ti - 1) * Pow(delta, di) * Math.Pow(tau, ti - 2);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, ci) = pr2[i];
				sum2 += ni * ti * (ti - 1) * Pow(delta, di) * Math.Pow(tau, ti - 2) *
								Math.Exp(-Pow(delta, ci));
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Pow(delta, di) * Math.Pow(tau, ti) *
								Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai)) *
								(
								Pow2(ti / tau + 2 * betai * (tau - gammai)) -
								ti / Pow2(tau) +
								2 * betai
								);
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];
				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di * (tau - 1) * Psi;

				// 2nd derivative of Psi with respect to tau
				double Psi_tautau = (2 * Di * Pow2(tau - 1) - 1) * 2 * Di * Psi;

				// 1st derivative of Delta^bi with respect to tau
				double Deltabi_tau = -2 * theta * bi * Math.Pow(Delta, bi - 1);

				// 2nd derivative of Delta^bi with respect to tau
				double Deltabi_tautau = 2 * bi * Math.Pow(Delta, bi - 1) + 4 * Pow2(theta) * bi * (bi - 1) * Math.Pow(Delta, bi - 2);

				sum4 += ni * delta *
								(
									Deltabi_tautau * Psi +
									2 * Deltabi_tau * Psi_tau +
									Math.Pow(Delta, bi) * Psi_tautau
								);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		/// <summary>
		/// Calculates the derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density delta and the inverse reduced temperature tau.
		/// </summary>
		/// <param name="delta">The reduced density.</param>
		/// <param name="tau">The reduced inverse temperature.</param>
		/// <returns>First derivative of the residual part of the dimensionless Helmholtz energy with respect to the reduced density delta and the inverse reduced temperature tau.</returns>
		public override double PhiR_deltatau_OfReducedVariables(double delta, double tau)
		{
			// Make local variables to improve speed
			var pr1 = _pr1;
			var pr2 = _pr2;
			var pr3 = _pr3;
			var pr4 = _pr4;

			double sum1 = 0;
			for (int i = 0; i < pr1.Length; ++i)
			{
				var (ni, ti, di) = pr1[i];
				sum1 += ni * di * ti * Pow(delta, di - 1) * Math.Pow(tau, ti - 1);
			}

			double sum2 = 0;
			for (int i = 0; i < pr2.Length; ++i)
			{
				var (ni, ti, di, ci) = pr2[i];
				sum2 += ni * ti * (Pow(delta, di - 1) * Math.Pow(tau, ti - 1) * Math.Exp(-Pow(delta, ci)) *
					(di - ci * Pow(delta, ci)));
			}

			double sum3 = 0;
			for (int i = 0; i < pr3.Length; ++i)
			{
				var (ni, ti, di, alphai, betai, gammai, epsiloni) = pr3[i];
				sum3 += ni * Pow(delta, di) * Math.Pow(tau, ti) *
								Math.Exp(alphai * Pow2(delta - epsiloni) + betai * Pow2(tau - gammai)) *
								(
									di / delta +
									2 * alphai * (delta - epsiloni)
								) *
								(
									ti / tau +
									2 * betai * (tau - gammai)
								);
			}

			double sum4 = 0;
			for (int i = 0; i < pr4.Length; ++i)
			{
				var (ni, bi, betai, Ai, Ci, Di, Bi, ai) = pr4[i];
				double theta = (1 - tau) + Ai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai));
				double Delta = Pow2(theta) + Bi * Math.Pow(Pow2(delta - 1), ai);
				double Psi = Math.Exp(-Ci * Pow2(delta - 1) - Di * Pow2(tau - 1));
				double Psi_delta = -2 * Ci * (delta - 1) * Psi;

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di * (tau - 1) * Psi;

				// derivative of Psi with respect to delta and tau
				double Psi_deltatau = 4 * Ci * Di * (delta - 1) * (tau - 1) * Psi;

				// Derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) * (Ai * theta * 2 / betai * Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 1) + 2 * Bi * ai * Math.Pow(Pow2(delta - 1), ai - 1));

				// 1st derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi * Math.Pow(Delta, bi - 1) * Delta_delta;

				// 1st derivative of Delta^bi with respect to tau
				double Deltabi_tau = -2 * theta * bi * Math.Pow(Delta, bi - 1);

				// derivative of Delta ^ bi with respect to delta and tau
				double Deltabi_deltatau = -Ai * bi * 2 / betai * Math.Pow(Delta, bi - 1) * (delta - 1) *
																	Math.Pow(Pow2(delta - 1), 1 / (2 * betai) - 1) -
																	2 * theta * bi * (bi - 1) * Math.Pow(Delta, bi - 2) * Delta_delta;

				sum4 += ni *
								(
									Math.Pow(Delta, bi) * (Psi_tau + delta * Psi_deltatau) +
									delta * Deltabi_delta * Psi_tau +
									Deltabi_tau * (Psi + delta * Psi_delta) +
									Deltabi_deltatau * delta * Psi
								);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		#endregion Residual part of dimensionless Helmholtz energy and derivatives

		#region Helper functions

		protected static readonly double[] _emptyDoubleArray = new double[0];

		protected static readonly (double, double)[] _emptyDoubleDoubleArray = new(double, double)[] { };

		private static double Coth(double x)
		{
			return 1 + 2 / (Math.Exp(2 * x) - 1);
		}

		private static double Sech(double x)
		{
			return 2 / (Math.Exp(x) + Math.Exp(-x));
		}

		private static double Csch(double x)
		{
			return 2 / (Math.Exp(x) - Math.Exp(-x));
		}

		protected static double Pow(double x, int n)
		{
			double value = 1.0;

			bool inverse = (n < 0);
			if (n < 0)
			{
				n = -n;

				if (!(n > 0)) // if n was so big, that it could not be inverted in sign
					return double.NaN;
			}

			/* repeated squaring method
			 * returns 0.0^0 = 1.0, so continuous in x
			 */
			do
			{
				if (0 != (n & 1))
					value *= x;  /* for n odd */

				n >>= 1;
				x *= x;
			} while (n != 0);

			return inverse ? 1.0 / value : value;
		}

		#endregion Helper functions

		#region Functions to calculate the saturated vapor and liquid density

		protected (double factor, double exponent)[] _saturatedVaporDensity_Coefficients;
		protected int _saturatedVaporDensity_Type;
		protected (double factor, double exponent)[] _saturatedLiquidDensity_Coefficients;
		protected int _saturatedLiquidDensity_Type;

		/// <summary>
		/// Gets an estimate for the saturated vapor mole density in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in K.</param>
		/// <returns>An estimate for the saturated vapor mole density in mol/m³ at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public override double SaturatedVaporMoleDensityEstimate_FromTemperature(double temperature)
		{
			if (!(temperature >= TriplePointTemperature && temperature <= CriticalPointTemperature))
				return double.NaN;

			switch (_saturatedVaporDensity_Type)
			{
				case 0:
					throw new NotImplementedException("This fluid does not contain an equation for the estimated saturated vapor mole density");
				case 1:
					return SaturatedMoleDensity_Type1(temperature, _saturatedVaporDensity_Coefficients);

				case 2:
					return SaturatedMoleDensity_Type2(temperature, _saturatedVaporDensity_Coefficients);

				case 3:
					return SaturatedMoleDensity_Type3(temperature, _saturatedVaporDensity_Coefficients);

				case 4:
					return SaturatedMoleDensity_Type4(temperature, _saturatedVaporDensity_Coefficients);

				case 5:
					return SaturatedMoleDensity_Type5(temperature, _saturatedVaporDensity_Coefficients);

				case 6:
					return SaturatedMoleDensity_Type6(temperature, _saturatedVaporDensity_Coefficients);

				default:
					throw new NotImplementedException(string.Format("Sorry, the saturated vapor mole density equation of type {0} is not implemented yet!", _saturatedVaporDensity_Type));
			}
		}

		/// <summary>
		/// Gets an estimate for the saturated liquid mole density in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in Kelvin.</param>
		/// <returns>An estimate for the saturated liquid mole density in mol/m³ at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public override double SaturatedLiquidMoleDensityEstimate_FromTemperature(double temperature)
		{
			if (!(temperature >= TriplePointTemperature && temperature <= CriticalPointTemperature))
				return double.NaN;

			switch (_saturatedLiquidDensity_Type)
			{
				case 0:
					throw new NotImplementedException("This fluid does not contain an equation for the estimated saturated liquid mole density");
				case 1:
					return SaturatedMoleDensity_Type1(temperature, _saturatedLiquidDensity_Coefficients);

				case 2:
					return SaturatedMoleDensity_Type2(temperature, _saturatedLiquidDensity_Coefficients);

				case 3:
					return SaturatedMoleDensity_Type3(temperature, _saturatedLiquidDensity_Coefficients);

				case 4:
					return SaturatedMoleDensity_Type4(temperature, _saturatedLiquidDensity_Coefficients);

				case 5:
					return SaturatedMoleDensity_Type5(temperature, _saturatedLiquidDensity_Coefficients);

				case 6:
					return SaturatedMoleDensity_Type6(temperature, _saturatedLiquidDensity_Coefficients);

				default:
					throw new NotImplementedException(string.Format("Sorry, the saturated liquid mole density equation of type {0} is not implemented yet!", _saturatedLiquidDensity_Type));
			}
		}

		protected double SaturatedMoleDensity_Type1(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
			}

			return CriticalPointMoleDensity * (sum + 1);
		}

		protected double SaturatedMoleDensity_Type2(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Pow(1 - temperature / CriticalPointTemperature, 1 / 3.0);
			double sum = 0;
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
			}
			return CriticalPointMoleDensity * (sum + 1);
		}

		protected double SaturatedMoleDensity_Type3(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
			}

			return CriticalPointMoleDensity * Math.Exp(sum);
		}

		protected double SaturatedMoleDensity_Type4(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Pow(1 - temperature / CriticalPointTemperature, 1 / 3.0);
			double sum = 0;
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
			}
			return CriticalPointMoleDensity * Math.Exp(sum);
		}

		protected double SaturatedMoleDensity_Type5(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
			}

			return CriticalPointMoleDensity * Math.Exp(sum * CriticalPointTemperature / temperature);
		}

		protected double SaturatedMoleDensity_Type6(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Pow(1 - temperature / CriticalPointTemperature, 1 / 3.0);
			double sum = 0;
			for (int i = 0; i < _saturatedVaporDensity_Coefficients.Length; ++i)
			{
				var (n, e) = _saturatedVaporDensity_Coefficients[i];
				sum += n * Math.Pow(TR, e);
			}
			return CriticalPointMoleDensity * Math.Exp(sum * CriticalPointTemperature / temperature);
		}

		#endregion Functions to calculate the saturated vapor and liquid density

		#region Functions to calculate the saturated vapor pressure

		protected (double factor, double exponent)[] _saturatedVaporPressure_Coefficients;
		protected int _saturatedVaporPressure_Type;

		/// <summary>
		/// Gets an estimate for the saturated vapor pressure in dependence on the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in K.</param>
		/// <returns>An estimate for the saturated vapor pressure in Pa at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], double.NaN is returned.
		/// </returns>
		public override double SaturatedVaporPressureEstimate_FromTemperature(double temperature)
		{
			if (!(temperature >= TriplePointTemperature && temperature <= CriticalPointTemperature))
				return double.NaN;

			switch (_saturatedVaporPressure_Type)
			{
				case 0:
					throw new NotImplementedException("This fluid does not contain an equation for the estimated saturated vapor pressure");
				case 1:
					return SaturatedVaporPressure_Type1(temperature, _saturatedVaporPressure_Coefficients).pressure;

				case 2:
					return SaturatedVaporPressure_Type2(temperature, _saturatedVaporPressure_Coefficients).pressure;

				case 3:
					return SaturatedVaporPressure_Type3(temperature, _saturatedVaporPressure_Coefficients).pressure;

				case 4:
					return SaturatedVaporPressure_Type4(temperature, _saturatedVaporPressure_Coefficients).pressure;

				case 5:
					return SaturatedVaporPressure_Type5(temperature, _saturatedVaporPressure_Coefficients).pressure;

				case 6:
					return SaturatedVaporPressure_Type6(temperature, _saturatedVaporPressure_Coefficients).pressure;

				default:
					throw new NotImplementedException(string.Format("Sorry, the vapor pressure equation of type {0} is not implemented yet!", _saturatedVaporPressure_Type));
			}
		}

		/// <summary>
		/// Gets an estimate for the saturated vapor pressure in dependence on the temperature as well as for the derivative of the saturated vapor pressure with respect to the temperature.
		/// </summary>
		/// <param name="temperature">The temperature in Kelvin.</param>
		/// <returns>An estimate for the saturated vapor pressure in Pa and the derivative w.r.t. temperature in Pa/K at the given temperature.
		/// If the temperature is outside [TriplePointTemperature, CriticalPointTemperature], (double.NaN, double.NaN) is returned.
		/// </returns>
		public override (double pressure, double pressureWrtTemperature) SaturatedVaporPressureEstimateAndDerivativeWrtTemperature_FromTemperature(double temperature)
		{
			if (!(temperature >= TriplePointTemperature && temperature <= CriticalPointTemperature))
				return (double.NaN, double.NaN);

			switch (_saturatedVaporPressure_Type)
			{
				case 0:
					throw new NotImplementedException("This fluid does not contain an equation for the estimated saturated vapor pressure");
				case 1:
					return SaturatedVaporPressure_Type1(temperature, _saturatedVaporPressure_Coefficients);

				case 2:
					return SaturatedVaporPressure_Type2(temperature, _saturatedVaporPressure_Coefficients);

				case 3:
					return SaturatedVaporPressure_Type3(temperature, _saturatedVaporPressure_Coefficients);

				case 4:
					return SaturatedVaporPressure_Type4(temperature, _saturatedVaporPressure_Coefficients);

				case 5:
					return SaturatedVaporPressure_Type5(temperature, _saturatedVaporPressure_Coefficients);

				case 6:
					return SaturatedVaporPressure_Type6(temperature, _saturatedVaporPressure_Coefficients);

				default:
					throw new NotImplementedException(string.Format("Sorry, the vapor pressure equation of type {0} is not implemented yet!", _saturatedVaporPressure_Type));
			}
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type1(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 1);
			}

			var pressure = CriticalPointPressure * (sum + 1);
			var dpdT = (-CriticalPointPressure / CriticalPointTemperature) * sum_dT;
			return (pressure, dpdT);
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type2(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Sqrt(1 - temperature / CriticalPointTemperature);
			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 2);
			}

			var pressure = CriticalPointPressure * (sum + 1);
			var dpdT = (-0.5 * CriticalPointPressure / CriticalPointTemperature) * sum_dT;
			return (pressure, dpdT);
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type3(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 1);
			}

			var pressure = CriticalPointPressure * Math.Exp(sum);
			return (pressure, -pressure * sum_dT / CriticalPointTemperature);
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type4(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Sqrt(1 - temperature / CriticalPointTemperature);
			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 2);
			}
			var pressure = CriticalPointPressure * Math.Exp(sum);
			return (pressure, -pressure * 0.5 * sum_dT / CriticalPointTemperature);
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type5(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = 1 - temperature / CriticalPointTemperature;

			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 1);
			}

			var pressure = CriticalPointPressure * Math.Exp(sum * CriticalPointTemperature / temperature);
			var dpdT = -pressure * (CriticalPointTemperature / temperature) * (sum_dT / CriticalPointTemperature + sum / temperature);
			return (pressure, dpdT);
		}

		protected (double pressure, double dpdT) SaturatedVaporPressure_Type6(double temperature, (double factor, double exponent)[] coefficients)
		{
			double TR = Math.Sqrt(1 - temperature / CriticalPointTemperature);
			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficients.Length; ++i)
			{
				var (n, e) = coefficients[i];
				sum += n * Math.Pow(TR, e);
				sum_dT += n * e * Math.Pow(TR, e - 2);
			}
			var pressure = CriticalPointPressure * Math.Exp(sum * CriticalPointTemperature / temperature);
			var dpdT = -pressure * (CriticalPointTemperature / temperature) * (0.5 * sum_dT / CriticalPointTemperature + sum / temperature);
			return (pressure, dpdT);
		}

		#endregion Functions to calculate the saturated vapor pressure

		#region Functions to calculate sublimation pressure

		protected int _sublimationPressure_Type;
		protected (double factor, double exponent)[] _sublimationPressure_PolynomialCoefficients1;
		protected (double factor, double exponent)[] _sublimationPressure_PolynomialCoefficients2;
		protected (double factor, double exponent)[] _sublimationPressure_PolynomialCoefficients3;

		protected (double pressure, double dpdT) SublimationPressure_Type1(double temperature)
		{
			var coefficientsP = _sublimationPressure_PolynomialCoefficients1;
			var coefficientsQ = _sublimationPressure_PolynomialCoefficients2;
			var coefficientsL = _sublimationPressure_PolynomialCoefficients3;

			double TRP = temperature / TriplePointTemperature;

			double sum = 0;
			double sum_dT = 0; // for derivative of pressure wrt temperature
			for (int i = 0; i < coefficientsP.Length; ++i)
			{
				var (n, e) = coefficientsP[i];
				sum += n * Math.Pow(TRP, e);
				sum_dT += n * e * Math.Pow(TRP, e - 1);
			}

			var TRQ = 1 - TRP;
			for (int i = 0; i < coefficientsQ.Length; ++i)
			{
				var (n, e) = coefficientsQ[i];
				sum += n * Math.Pow(TRQ, e);
				sum_dT += -n * e * Math.Pow(TRQ, e - 1);
			}

			var TRL = Math.Log(TRP);
			for (int i = 0; i < coefficientsL.Length; ++i)
			{
				var (n, e) = coefficientsL[i];
				sum += n * Math.Pow(TRL, e);
				sum_dT += n * e * Math.Pow(TRL, e - 1) / temperature;
			}

			var pressure = CriticalPointPressure * (sum + 1);
			var dpdT = (-CriticalPointPressure / CriticalPointTemperature) * sum_dT;
			return (pressure, dpdT);
		}

		#endregion Functions to calculate sublimation pressure

		#region Functions to calculate melting pressure

		protected int _meltingPressure_Type;
		protected (double factor, double exponent)[][] _meltingPressure_PolynomialCoefficients;

		#endregion Functions to calculate melting pressure
	}
}
