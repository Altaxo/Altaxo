using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	public abstract class BinaryMixtureDefinitionBase
	{
		/// <summary>Gets the CAS registry number of component 1.</summary>
		public abstract string CASRegistryNumber1 { get; }

		/// <summary>Gets the CAS registry number of component 2.</summary>
		public abstract string CASRegistryNumber2 { get; }

		protected double _beta_T;
		protected double _gamma_T;
		protected double _beta_v;
		protected double _gamma_v;
		protected double _F;

		public double Beta_T { get { return _beta_T; } }
		public double Gamma_T { get { return _gamma_T; } }
		public double Beta_v { get { return _beta_v; } }
		public double Gamma_v { get { return _gamma_v; } }
		public double F { get { return _F; } }

		protected static readonly (double, double, double)[] _emptyArrayOfThreeDoubles = new(double, double, double)[0];
		protected static readonly (double, double, double, double)[] _emptyArrayOfFourDoubles = new(double, double, double, double)[0];
		protected static readonly (double, double, double, double, double, double, double)[] _emptyArrayOfSevenDoubles = new(double, double, double, double, double, double, double)[0];

		protected (double ai, double ti, double di)[] _departureCoefficients_Polynomial = _emptyArrayOfThreeDoubles;
		protected (double ai, double ti, double di, double ci)[] _departureCoefficients_Exponential = _emptyArrayOfFourDoubles;
		protected (double n, double t, double d, double eta, double epsilon, double beta, double gamma)[] _departureCoefficients_Special = _emptyArrayOfSevenDoubles;

		/// <summary>
		/// The Departure function is a mixture correction function for the residual part of the reduced Helmholtz energy in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Mixture correction function Alpha12R.</returns>
		public double DepartureFunction_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			{
				var ppoly = _departureCoefficients_Polynomial;
				for (int i = 0; i < ppoly.Length; ++i)
				{
					var (ni, ti, di) = ppoly[i];
					sum1 += ni * Math.Pow(delta, di) * Math.Pow(tau, ti);
				}
			}

			{
				var pexp = _departureCoefficients_Exponential;
				for (int i = 0; i < pexp.Length; ++i)
				{
					var (ni, ti, di, ci) = pexp[i];
					sum2 += ni * Math.Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(-Math.Pow(delta, ci));
				}
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d) * Math.Pow(tau, t) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma));
				}
			}

			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. delta in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. delta.</returns>
		public double DepartureFunction_delta_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			var ppoly = _departureCoefficients_Polynomial;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * Math.Pow(delta, di - 1) * Math.Pow(tau, ti);
			}

			var pexp = _departureCoefficients_Exponential;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Math.Pow(delta, ci)) *
								(Math.Pow(delta, di - 1) * Math.Pow(tau, ti) * (di - ci * Math.Pow(delta, ci)));
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d - 1) * Math.Pow(tau, t) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma)) *
						(d + delta * (beta + 2 * (delta - epsilon) * eta));
				}
			}

			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// 2nd derivative of the mixture correction function Alpha12R w.r.t. delta in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>2nd derivative of the mixture correction function Alpha12R w.r.t. delta.</returns>
		public double DepartureFunction_deltadelta_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			var ppoly = _departureCoefficients_Polynomial;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * (di - 1) * Math.Pow(delta, di - 2) * Math.Pow(tau, ti);
			}

			var pexp = _departureCoefficients_Exponential;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Math.Pow(delta, ci)) *
								(
								Math.Pow(delta, di - 2) * Math.Pow(tau, ti) *
								((di - ci * Math.Pow(delta, ci)) * (di - 1 - ci * Math.Pow(delta, ci)) - ci * ci * Math.Pow(delta, ci))
								);
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d - 2) * Math.Pow(tau, t) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma)) *
						(Pow2(d) + d * (-1 + 2 * beta * delta + 4 * delta * (delta - epsilon) * eta) +
						Pow2(delta) * (2 * eta + Pow2(beta + 2 * (delta - epsilon) * eta)));
				}
			}

			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. tau.</returns>
		public double DepartureFunction_tau_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			var ppoly = _departureCoefficients_Polynomial;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * Math.Pow(delta, di) * Math.Pow(tau, ti - 1);
			}

			var pexp = _departureCoefficients_Exponential;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * Math.Pow(delta, di) * Math.Pow(tau, ti - 1) * Math.Exp(-Math.Pow(delta, ci));
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d) * Math.Pow(tau, t - 1) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma)) *
									(t);
				}
			}

			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// 2nd derivative of the mixture correction function Alpha12R w.r.t. tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>2nd derivative of the mixture correction function Alpha12R w.r.t. tau.</returns>
		public double DepartureFunction_tautau_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			var ppoly = _departureCoefficients_Polynomial;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * (ti - 1) * Math.Pow(delta, di) * Math.Pow(tau, ti - 2);
			}

			var pexp = _departureCoefficients_Exponential;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (ti - 1) * Math.Pow(delta, di) * Math.Pow(tau, ti - 2) *
								Math.Exp(-Math.Pow(delta, ci));
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d) * Math.Pow(tau, t - 2) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma)) *
									(t * (t - 1));
				}
			}

			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// Derivative of the mixture correction function Alpha12R w.r.t. delta and tau in dependence of the reduced density and reduced inverse temperature.
		/// </summary>
		/// <param name="delta">The reduced density = mole density / <see cref="ReducingMoleDensity"/>.</param>
		/// <param name="tau">The reduced inverse temperature = <see cref="ReducingTemperature"/> / temperature.</param>
		/// <returns>Derivative of the mixture correction function Alpha12R w.r.t. delta and tau.</returns>
		public double DepartureFunction_deltatau_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;
			double sum3 = 0;

			var ppoly = _departureCoefficients_Polynomial;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * ti * Math.Pow(delta, di - 1) * Math.Pow(tau, ti - 1);
			}

			var pexp = _departureCoefficients_Exponential;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (Math.Pow(delta, di - 1) * Math.Pow(tau, ti - 1) * Math.Exp(-Math.Pow(delta, ci)) *
					(di - ci * Math.Pow(delta, ci)));
			}

			{
				var pspec = _departureCoefficients_Special;
				for (int i = 0; i < pspec.Length; ++i)
				{
					var (n, t, d, eta, epsilon, beta, gamma) = pspec[i];
					// Note that we work with changed sign of the prefactors in the Exp function
					sum3 += n * Math.Pow(delta, d - 1) * Math.Pow(tau, t - 1) * Math.Exp(eta * Pow2(delta - epsilon) + beta * (delta - gamma)) *
									((d + delta * (beta + 2 * (delta - epsilon) * eta)) * t);
				}
			}

			return sum1 + sum2 + sum3;
		}

		private static double Pow2(double x) => x * x;
	}
}
