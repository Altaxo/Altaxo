﻿#region Copyright

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
	public abstract class HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl : HelmholtzEquationOfStateOfBinarySystem
	{
		protected double _reducingTemperature;
		protected double _reducingDensity;

		protected double _betaT12;
		protected double _gammaT12;
		protected double _betaN12;
		protected double _gammaN12;
		protected double _F12;

		protected double[] _ni1;
		protected double[] _ti1;
		protected int[] _di1;

		protected double[] _ni2;
		protected double[] _ti2;
		protected int[] _di2;
		protected int[] _ci2;

		protected HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl(double moleFraction1, HelmholtzEquationOfStateOfPureFluids component1, double moleFraction2, HelmholtzEquationOfStateOfPureFluids component2)
	: base(moleFraction1, component1, moleFraction2, component2)
		{
			InitializeCoefficientArrays();
			TestArrays();
			_reducingDensity = CalculateReducingDensity();
			_reducingTemperature = CalculateReducingTemperature();
		}

		protected abstract void InitializeCoefficientArrays();

		/// <summary>
		/// Helper function to test the length of the coefficient arrays.
		/// </summary>
		/// <exception cref="InvalidProgramException">
		/// </exception>
		protected virtual void TestArrays()
		{
			if (_ni1.Length != _di1.Length)
				throw new InvalidProgramException();
			if (_ni1.Length != _ti1.Length)
				throw new InvalidProgramException();

			if (_ni2.Length != _di2.Length)
				throw new InvalidProgramException();
			if (_ni2.Length != _ti2.Length)
				throw new InvalidProgramException();
			if (_ni2.Length != _ci2.Length)
				throw new InvalidProgramException();
		}

		protected virtual double CalculateReducingDensity()
		{
			double sum = 0;
			sum += Pow2(_moleFraction1) / _component1.CriticalPointDensity;
			sum += Pow2(_moleFraction2) / _component2.CriticalPointDensity;
			sum += 2 * _moleFraction1 * _moleFraction2 * _betaN12 * _gammaN12 * (_moleFraction1 + _moleFraction2) / (Pow2(_betaN12) * _moleFraction1 + _moleFraction2) * 0.125 * Pow(Math.Pow(_component1.CriticalPointDensity, -1 / 3.0) + Math.Pow(_component2.CriticalPointDensity, -1 / 3.0), 3);
			return 1 / sum;
		}

		protected virtual double CalculateReducingTemperature()
		{
			double sum = 0;
			sum += Pow2(_moleFraction1) * _component1.CriticalPointTemperature;
			sum += Pow2(_moleFraction2) * _component2.CriticalPointTemperature;
			sum += 2 * _moleFraction1 * _moleFraction2 * _betaT12 * _gammaT12 * (_moleFraction1 + _moleFraction2) / (Pow2(_betaT12) * _moleFraction1 + _moleFraction2) * Math.Sqrt(_component1.CriticalPointTemperature * _component2.CriticalPointTemperature);
			return sum;
		}

		public override double ReducingDensity
		{
			get
			{
				return _reducingDensity;
			}
		}

		public override double ReducingTemperature
		{
			get
			{
				return _reducingTemperature;
			}
		}

		public override double Phi0_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * _component1.Phi0_OfReducedVariables(delta * ReducingDensity / _component1.ReducingDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * _component2.Phi0_OfReducedVariables(delta * ReducingDensity / _component2.ReducingDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			var sum3 = _moleFraction1 * Math.Log(_moleFraction1) + _moleFraction2 * Math.Log(_moleFraction2);

			return double.IsNaN(sum3) ? sum1 + sum2 : sum1 + sum2 + sum3;
		}

		public override double Phi0_tau_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * (_component1.ReducingTemperature / ReducingTemperature) * _component1.Phi0_tau_OfReducedVariables(delta * ReducingDensity / _component1.ReducingDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * (_component2.ReducingTemperature / ReducingTemperature) * _component2.Phi0_tau_OfReducedVariables(delta * ReducingDensity / _component2.ReducingDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			return sum1 + sum2;
		}

		public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
		{
			// Note we have to calculate back from the reduced variables of this mixture to the reduced variables of the components
			var sum1 = _moleFraction1 * Pow2(_component1.ReducingTemperature / ReducingTemperature) * _component1.Phi0_tautau_OfReducedVariables(delta * ReducingDensity / _component1.ReducingDensity, tau / ReducingTemperature * _component1.ReducingTemperature);
			var sum2 = _moleFraction2 * Pow2(_component2.ReducingTemperature / ReducingTemperature) * _component2.Phi0_tautau_OfReducedVariables(delta * ReducingDensity / _component2.ReducingDensity, tau / ReducingTemperature * _component2.ReducingTemperature);
			return sum1 + sum2;
		}

		/// <summary>
		/// Residual part of the dimensionless Helmholtz energy as function of density and temperature. (Page 276 eq. 5).
		/// </summary>
		/// <param name="density">The density in kg/m³.</param>
		/// <param name="tau">The temperature in Kelvin.</param>
		/// <returns>Ideal part of the dimensionless Helmholtz energy.</returns>
		public override double PhiR_OfReducedVariables(double delta, double tau)
		{
			var sum1 = _moleFraction1 * _component1.PhiR_OfReducedVariables(delta, tau);
			var sum2 = _moleFraction2 * _component2.PhiR_OfReducedVariables(delta, tau);

			var sum3 = _moleFraction1 * _moleFraction2 * _F12 * Alpha12R(delta, tau);
			return sum1 + sum2 + sum3;
		}

		/// <summary>
		/// Residual part of the dimensionless Helmholtz energy as function of density and temperature. (Page 276 eq. 5).
		/// </summary>
		/// <param name="moleFractionCO2">The molar fraction of CO2 in the mixture.</param>
		/// <param name="density">The density in kg/m³.</param>
		/// <param name="tau">The temperature in Kelvin.</param>
		/// <returns>Ideal part of the dimensionless Helmholtz energy.</returns>
		public override double PhiR_delta_OfReducedVariables(double delta, double tau)
		{
			var sum1 = _moleFraction1 * _component1.PhiR_delta_OfReducedVariables(delta, tau);
			var sum2 = _moleFraction2 * _component2.PhiR_delta_OfReducedVariables(delta, tau);
			var sum3 = _moleFraction1 * _moleFraction2 * _F12 * Alpha12R_delta(delta, tau);
			return sum1 + sum2 + sum3;
		}

		public override double PhiR_deltadelta_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_deltadelta_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_deltadelta_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * Alpha12R_deltadelta(delta, tau);
			return sum;
		}

		public override double PhiR_tau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_tau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_tau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * Alpha12R_tau(delta, tau);
			return sum;
		}

		public override double PhiR_tautau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_tautau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_tautau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * Alpha12R_tautau(delta, tau);
			return sum;
		}

		public override double PhiR_deltatau_OfReducedVariables(double delta, double tau)
		{
			double sum = 0;
			sum += _moleFraction1 * _component1.PhiR_deltatau_OfReducedVariables(delta, tau);
			sum += _moleFraction2 * _component2.PhiR_deltatau_OfReducedVariables(delta, tau);
			sum += _moleFraction1 * _moleFraction2 * _F12 * Alpha12R_deltatau(delta, tau);
			return sum;
		}

		#region Alpha12R and derivatives

		public double Alpha12R(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * Pow(delta, di1[i]) * Math.Pow(tau, ti1[i]);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * Pow(delta, di2[i]) * Math.Pow(tau, ti2[i]) * Math.Exp(-Pow(delta, ci2[i]));
			}

			return sum1 + sum2;
		}

		public double Alpha12R_delta(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * di1[i] * Pow(delta, di1[i] - 1) * Math.Pow(tau, ti1[i]);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * Math.Exp(-Pow(delta, ci2[i])) *
								(Pow(delta, di2[i] - 1) * Math.Pow(tau, ti2[i]) * (di2[i] - ci2[i] * Pow(delta, ci2[i])));
			}
			return sum1 + sum2;
		}

		public double Alpha12R_deltadelta(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * di1[i] * (di1[i] - 1) * Pow(delta, di1[i] - 2) * Math.Pow(tau, ti1[i]);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * Math.Exp(-Pow(delta, ci2[i])) *
								(
								Pow(delta, di2[i] - 2) * Math.Pow(tau, ti2[i]) *
								((di2[i] - ci2[i] * Pow(delta, ci2[i])) * (di2[i] - 1 - ci2[i] * Pow(delta, ci2[i])) - Pow2(ci2[i]) * Pow(delta, ci2[i]))
								);
			}
			return sum1 + sum2;
		}

		public double Alpha12R_tau(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * ti1[i] * Pow(delta, di1[i]) * Math.Pow(tau, ti1[i] - 1);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * ti2[i] * Pow(delta, di2[i]) * Math.Pow(tau, ti2[i] - 1) * Math.Exp(-Pow(delta, ci2[i]));
			}
			return sum1 + sum2;
		}

		public double Alpha12R_tautau(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * ti1[i] * (ti1[i] - 1) * Pow(delta, di1[i]) * Math.Pow(tau, ti1[i] - 2);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * ti2[i] * (ti2[i] - 1) * Pow(delta, di2[i]) * Math.Pow(tau, ti2[i] - 2) *
								Math.Exp(-Pow(delta, ci2[i]));
			}
			return sum1 + sum2;
		}

		public double Alpha12R_deltatau(double delta, double tau)
		{
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;

			var ni2 = _ni2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ci2 = _ci2;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * di1[i] * ti1[i] * Pow(delta, di1[i] - 1) * Math.Pow(tau, ti1[i] - 1);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * ti2[i] * (Pow(delta, di2[i] - 1) * Math.Pow(tau, ti2[i] - 1) * Math.Exp(-Pow(delta, ci2[i])) *
					(di2[i] - ci2[i] * Pow(delta, ci2[i])));
			}
			return sum1 + sum2;
		}

		#endregion Alpha12R and derivatives

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