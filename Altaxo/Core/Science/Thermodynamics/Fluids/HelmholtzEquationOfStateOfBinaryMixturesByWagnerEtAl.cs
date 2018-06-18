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
	/// Equation of state of binary mixtures, based on the paper of the group of Bochum University.
	/// </summary>
	/// <seealso cref="Altaxo.Science.Thermodynamics.Fluids.HelmholtzEquationOfStateOfBinarySystem" />
	/// <remarks>
	/// Reference:
	/// Johannes Gernert, Roland Span,
	/// EOS–CG: A Helmholtz energy mixture model for humid gases and CCS	mixtures,
	/// J. Chem. Thermodynamics 93 (2016) 274–293
	/// </remarks>
	public abstract class HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl : HelmholtzEquationOfStateOfBinarySystem
	{
		protected double _reducingTemperature;
		protected double _reducingMoleDensity;

		protected double _betaT12;
		protected double _gammaT12;
		protected double _betaV12;
		protected double _gammaV12;
		protected double _F12;

		/// <summary>
		/// Parameter for the polynomial terms of the reduced Helmholtz energy.
		/// </summary>
		protected (double ni, double ti, int di)[] _pr1;

		/// <summary>
		/// Parameter for the exponential terms of the reduced Helmholtz energy.
		/// </summary>
		protected (double ni, double ti, int di, int li)[] _pr2;

		/// <summary>
		/// Initializes a new instance of the <see cref="HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl"/> class.
		/// </summary>
		/// <param name="moleFraction1">The mole fraction of component 1.</param>
		/// <param name="component1">The component 1.</param>
		/// <param name="moleFraction2">The mole fraction of component 2.</param>
		/// <param name="component2">The component 2.</param>
		protected HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl(double moleFraction1, HelmholtzEquationOfStateOfPureFluids component1, double moleFraction2, HelmholtzEquationOfStateOfPureFluids component2)
	: base(moleFraction1, component1, moleFraction2, component2)
		{
			InitializeCoefficientArrays();
			_reducingTemperature = CalculateReducingTemperature();
			_reducingMoleDensity = CalculateReducingMoleDensity();
		}

		/// <summary>
		/// Initializes the coefficient arrays with the specific values for this binary mixture.
		/// </summary>
		protected abstract void InitializeCoefficientArrays();

		/// <summary>
		/// Calculates the reducing mole density in dependence of the mole fractions.
		/// </summary>
		/// <returns>Reducing density in mol/m³.</returns>
		protected virtual double CalculateReducingMoleDensity()
		{
			double sum = 0;
			sum += Pow2(_moleFraction1) / _component1.CriticalPointMoleDensity;
			sum += Pow2(_moleFraction2) / _component2.CriticalPointMoleDensity;
			sum += 2 * _moleFraction1 * _moleFraction2 * _betaV12 * _gammaV12 * (_moleFraction1 + _moleFraction2) / (Pow2(_betaV12) * _moleFraction1 + _moleFraction2) * 0.125 * Pow(Math.Pow(_component1.CriticalPointMoleDensity, -1 / 3.0) + Math.Pow(_component2.CriticalPointMoleDensity, -1 / 3.0), 3);
			return 1 / sum;
		}

		/// <summary>
		/// Calculates the reducing temperature in dependence of the mole fractions.
		/// </summary>
		/// <returns>Reducing temperature in K.</returns>
		protected virtual double CalculateReducingTemperature()
		{
			double sum = 0;
			sum += Pow2(_moleFraction1) * _component1.CriticalPointTemperature;
			sum += Pow2(_moleFraction2) * _component2.CriticalPointTemperature;
			sum += 2 * _moleFraction1 * _moleFraction2 * _betaT12 * _gammaT12 * (_moleFraction1 + _moleFraction2) / (Pow2(_betaT12) * _moleFraction1 + _moleFraction2) * Math.Sqrt(_component1.CriticalPointTemperature * _component2.CriticalPointTemperature);
			return sum;
		}

		/// <inheritdoc/>
		public override double ReducingTemperature => _reducingTemperature;

		/// <inheritdoc/>
		public override double ReducingMoleDensity => _reducingMoleDensity;

		/// <inheritdoc/>
		public override double Phi0_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * _component1.Phi0_OfReducedVariables(delta * ReducingMoleDensity / _component1.ReducingMoleDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * _component2.Phi0_OfReducedVariables(delta * ReducingMoleDensity / _component2.ReducingMoleDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			var sum3 = _moleFraction1 * Math.Log(_moleFraction1) + _moleFraction2 * Math.Log(_moleFraction2);

			return double.IsNaN(sum3) ? sum1 + sum2 : sum1 + sum2 + sum3;
		}

		/// <inheritdoc/>
		public override double Phi0_tau_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * (_component1.ReducingTemperature / ReducingTemperature) * _component1.Phi0_tau_OfReducedVariables(delta * ReducingMassDensity / _component1.ReducingMassDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * (_component2.ReducingTemperature / ReducingTemperature) * _component2.Phi0_tau_OfReducedVariables(delta * ReducingMassDensity / _component2.ReducingMassDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			return sum1 + sum2;
		}

		/// <inheritdoc/>
		public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * Pow2(_component1.ReducingTemperature / ReducingTemperature) * _component1.Phi0_tautau_OfReducedVariables(delta * ReducingMassDensity / _component1.ReducingMassDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * Pow2(_component2.ReducingTemperature / ReducingTemperature) * _component2.Phi0_tautau_OfReducedVariables(delta * ReducingMassDensity / _component2.ReducingMassDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			return sum1 + sum2;
		}

		/// <inheritdoc/>
		public override double PhiR_OfReducedVariables(double delta, double tau)
		{
			var sum1 = _moleFraction1 * _component1.PhiR_OfReducedVariables(delta, tau);
			var sum2 = _moleFraction2 * _component2.PhiR_OfReducedVariables(delta, tau);
			var sum3 = _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction(delta, tau);
			return sum1 + sum2 + sum3;
		}

		/// <inheritdoc/>
		public override double PhiR_delta_OfReducedVariables(double delta, double tau)
		{
			var sum1 = _moleFraction1 * _component1.PhiR_delta_OfReducedVariables(delta, tau);
			var sum2 = _moleFraction2 * _component2.PhiR_delta_OfReducedVariables(delta, tau);
			var sum3 = _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction_delta(delta, tau);
			return sum1 + sum2 + sum3;
		}

		/// <inheritdoc/>
		public override double PhiR_deltadelta_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_deltadelta_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_deltadelta_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction_deltadelta(delta, tau);
			return sum;
		}

		/// <inheritdoc/>
		public override double PhiR_tau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_tau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_tau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction_tau(delta, tau);
			return sum;
		}

		/// <inheritdoc/>
		public override double PhiR_tautau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_tautau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_tautau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction_tautau(delta, tau);
			return sum;
		}

		/// <inheritdoc/>
		public override double PhiR_deltatau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_deltatau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_deltatau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * DepartureFunction_deltatau(delta, tau);
			return sum;
		}

		#region Mixture correction function Alpha12R and derivatives

		/// <summary>
		/// The Departure function is a mixture correction function for the residual part of the reduced Helmholtz energy in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Mixture correction function Alpha12R.</returns>
		public double DepartureFunction(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			{
				var ppoly = _pr1;
				for (int i = 0; i < ppoly.Length; ++i)
				{
					var (ni, ti, di) = ppoly[i];
					sum1 += ni * Pow(delta, di) * Math.Pow(tau, ti);
				}
			}

			{
				var pexp = _pr2;
				for (int i = 0; i < pexp.Length; ++i)
				{
					var (ni, ti, di, ci) = pexp[i];
					sum2 += ni * Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(-Pow(delta, ci));
				}
			}

			return sum1 + sum2;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. delta in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. delta.</returns>
		public double DepartureFunction_delta(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _pr1;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * Pow(delta, di - 1) * Math.Pow(tau, ti);
			}

			var pexp = _pr2;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Pow(delta, ci)) *
								(Pow(delta, di - 1) * Math.Pow(tau, ti) * (di - ci * Pow(delta, ci)));
			}

			return sum1 + sum2;
		}

		/// <summary>
		/// 2nd derivative of the mixture correction function Alpha12R w.r.t. delta in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>2nd derivative of the mixture correction function Alpha12R w.r.t. delta.</returns>
		public double DepartureFunction_deltadelta(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _pr1;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * (di - 1) * Pow(delta, di - 2) * Math.Pow(tau, ti);
			}

			var pexp = _pr2;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Pow(delta, ci)) *
								(
								Pow(delta, di - 2) * Math.Pow(tau, ti) *
								((di - ci * Pow(delta, ci)) * (di - 1 - ci * Pow(delta, ci)) - Pow2(ci) * Pow(delta, ci))
								);
			}
			return sum1 + sum2;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. tau.</returns>
		public double DepartureFunction_tau(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _pr1;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * Pow(delta, di) * Math.Pow(tau, ti - 1);
			}

			var pexp = _pr2;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * Pow(delta, di) * Math.Pow(tau, ti - 1) * Math.Exp(-Pow(delta, ci));
			}
			return sum1 + sum2;
		}

		/// <summary>
		/// 2nd derivative of the mixture correction function Alpha12R w.r.t. tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>2nd derivative of the mixture correction function Alpha12R w.r.t. tau.</returns>
		public double DepartureFunction_tautau(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _pr1;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * (ti - 1) * Pow(delta, di) * Math.Pow(tau, ti - 2);
			}

			var pexp = _pr2;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (ti - 1) * Pow(delta, di) * Math.Pow(tau, ti - 2) *
								Math.Exp(-Pow(delta, ci));
			}
			return sum1 + sum2;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. delta and tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. delta and tau.</returns>
		public double DepartureFunction_deltatau(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _pr1;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * ti * Pow(delta, di - 1) * Math.Pow(tau, ti - 1);
			}

			var pexp = _pr2;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (Pow(delta, di - 1) * Math.Pow(tau, ti - 1) * Math.Exp(-Pow(delta, ci)) *
					(di - ci * Pow(delta, ci)));
			}
			return sum1 + sum2;
		}

		#endregion Mixture correction function Alpha12R and derivatives

		#region Helper functions

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
	}
}
