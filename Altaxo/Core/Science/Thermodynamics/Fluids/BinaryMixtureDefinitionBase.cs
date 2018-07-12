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

		protected (double ai, double ti, double di)[] _departure_Poly;
		protected (double ai, double ti, double di, double ci)[] _departure_Exp;

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

			{
				var ppoly = _departure_Poly;
				for (int i = 0; i < ppoly.Length; ++i)
				{
					var (ni, ti, di) = ppoly[i];
					sum1 += ni * Math.Pow(delta, di) * Math.Pow(tau, ti);
				}
			}

			{
				var pexp = _departure_Exp;
				for (int i = 0; i < pexp.Length; ++i)
				{
					var (ni, ti, di, ci) = pexp[i];
					sum2 += ni * Math.Pow(delta, di) * Math.Pow(tau, ti) * Math.Exp(-Math.Pow(delta, ci));
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
		public double DepartureFunction_delta_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _departure_Poly;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * Math.Pow(delta, di - 1) * Math.Pow(tau, ti);
			}

			var pexp = _departure_Exp;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Math.Pow(delta, ci)) *
								(Math.Pow(delta, di - 1) * Math.Pow(tau, ti) * (di - ci * Math.Pow(delta, ci)));
			}

			return sum1 + sum2;
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

			var ppoly = _departure_Poly;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * (di - 1) * Math.Pow(delta, di - 2) * Math.Pow(tau, ti);
			}

			var pexp = _departure_Exp;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * Math.Exp(-Math.Pow(delta, ci)) *
								(
								Math.Pow(delta, di - 2) * Math.Pow(tau, ti) *
								((di - ci * Math.Pow(delta, ci)) * (di - 1 - ci * Math.Pow(delta, ci)) - ci * ci * Math.Pow(delta, ci))
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
		public double DepartureFunction_tau_OfReducedVariables(double delta, double tau)
		{
			double sum1 = 0;
			double sum2 = 0;

			var ppoly = _departure_Poly;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * Math.Pow(delta, di) * Math.Pow(tau, ti - 1);
			}

			var pexp = _departure_Exp;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * Math.Pow(delta, di) * Math.Pow(tau, ti - 1) * Math.Exp(-Math.Pow(delta, ci));
			}
			return sum1 + sum2;
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

			var ppoly = _departure_Poly;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * ti * (ti - 1) * Math.Pow(delta, di) * Math.Pow(tau, ti - 2);
			}

			var pexp = _departure_Exp;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (ti - 1) * Math.Pow(delta, di) * Math.Pow(tau, ti - 2) *
								Math.Exp(-Math.Pow(delta, ci));
			}
			return sum1 + sum2;
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

			var ppoly = _departure_Poly;
			for (int i = 0; i < ppoly.Length; ++i)
			{
				var (ni, ti, di) = ppoly[i];
				sum1 += ni * di * ti * Math.Pow(delta, di - 1) * Math.Pow(tau, ti - 1);
			}

			var pexp = _departure_Exp;
			for (int i = 0; i < pexp.Length; ++i)
			{
				var (ni, ti, di, ci) = pexp[i];
				sum2 += ni * ti * (Math.Pow(delta, di - 1) * Math.Pow(tau, ti - 1) * Math.Exp(-Math.Pow(delta, ci)) *
					(di - ci * Math.Pow(delta, ci)));
			}
			return sum1 + sum2;
		}
	}
}
