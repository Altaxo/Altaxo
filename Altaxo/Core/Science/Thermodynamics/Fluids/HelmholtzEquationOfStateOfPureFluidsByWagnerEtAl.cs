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
		#region Reduced density and pressure

		/// <inheritdoc/>
		public override double ReducingDensity { get { return CriticalPointDensity; } }

		/// <inheritdoc/>
		public override double ReducingTemperature { get { return CriticalPointTemperature; } }

		#endregion Reduced density and pressure

		/// <summary>
		/// Helper function to test the length of the coefficient arrays.
		/// </summary>
		/// <exception cref="InvalidProgramException">
		/// </exception>
		protected void TestArrays()
		{
			if (_ni1.Length != _di1.Length)
				throw new InvalidProgramException();
			if (_ni1.Length != _ti1.Length)
				throw new InvalidProgramException();

			if (_ni2.Length != _ci2.Length)
				throw new InvalidProgramException();
			if (_ni2.Length != _di2.Length)
				throw new InvalidProgramException();
			if (_ni2.Length != _ti2.Length)
				throw new InvalidProgramException();

			if (_ni3.Length != _di3.Length)
				throw new InvalidProgramException();
			if (_ni3.Length != _ti3.Length)
				throw new InvalidProgramException();
			if (_ni3.Length != _alphai3.Length)
				throw new InvalidProgramException();
			if (_ni3.Length != _betai3.Length)
				throw new InvalidProgramException();
			if (_ni3.Length != _gammai3.Length)
				throw new InvalidProgramException();
			if (_ni3.Length != _epsiloni3.Length)
				throw new InvalidProgramException();

			if (_ni4.Length != _ai4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _bi4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _Bi4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _Ci4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _Di4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _Ai4.Length)
				throw new InvalidProgramException();
			if (_ni4.Length != _betai4.Length)
				throw new InvalidProgramException();
		}

		#region Ideal part of dimensionless Helmholtz energy and derivatives

		/// <summary>
		/// Page 429 Table 6.1 in [2] (n_i there is ai0 here)
		/// </summary>
		protected double[] _ai0;

		/// <summary>
		/// Page 429 Table 6.1 in [2] (gamma_i there is thetai0 here)
		/// </summary>
		protected double[] _thetai0;

		/// <summary>
		/// Phi0s the of reduced variables. (Page 1541, Table 28 in [2])
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns></returns>
		public override double Phi0_OfReducedVariables(double delta, double tau)
		{
			var ai0 = _ai0;
			var thetai0 = _thetai0;

			double sum = 0;
			for (int i = 4; i <= 8; ++i)
				sum += ai0[i] * Math.Log(1 - Math.Exp(-thetai0[i] * tau));

			return sum + Math.Log(delta) + ai0[1] + ai0[2] * tau + ai0[3] * Math.Log(tau);
		}

		/// <summary>
		/// First derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature. (Page 1541, Table 28)
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns>First derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature.</returns>
		public override double Phi0_tau_OfReducedVariables(double delta, double tau)
		{
			var ai0 = _ai0;
			var thetai0 = _thetai0;

			double sum = 0;
			for (int i = 4; i <= 8; ++i)
			{
				sum += ai0[i] * thetai0[i] * (1 / (1 - Math.Exp(-thetai0[i] * tau)) - 1);
			}

			return sum + ai0[2] + ai0[3] / tau;
		}

		/// <summary>
		/// Second derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature. (Page 1541, Table 28)
		/// </summary>
		/// <param name="delta">The delta.</param>
		/// <param name="tau">The tau.</param>
		/// <returns>Second derivative of Phi0 the of reduced variables with respect to the inverse reduced temperature.</returns>
		public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
		{
			var ai0 = _ai0;
			var thetai0 = _thetai0;

			double sum = 0;
			for (int i = 4; i <= 8; ++i)
			{
				sum += ai0[i] * Pow2(thetai0[i]) * Math.Exp(-thetai0[i] * tau) / Pow2(1 - Math.Exp(-thetai0[i] * tau));
			}

			return -sum - ai0[3] / Pow2(tau);
		}

		#endregion Ideal part of dimensionless Helmholtz energy and derivatives

		#region Residual part of dimensionless Helmholtz energy and derivatives

		#region Parameter from Table, e.g. for Water: 6.2, page 430  in [2]

		#region 1st sum term

		protected double[] _ni1;
		protected int[] _di1;
		protected double[] _ti1;

		#endregion 1st sum term

		#region 2nd sum term

		protected double[] _ni2;
		protected int[] _di2;
		protected double[] _ti2;

		protected int[] _ci2;

		#endregion 2nd sum term

		#region 3rd sum term

		protected double[] _ni3;

		protected int[] _di3;

		protected double[] _ti3;

		protected int[] _alphai3;

		protected int[] _betai3;

		protected double[] _gammai3;

		protected int[] _epsiloni3;

		#endregion 3rd sum term

		#region 4th sum term

		protected double[] _ni4;

		protected double[] _ai4;

		protected double[] _bi4;

		protected double[] _betai4;

		protected double[] _Ai4;

		protected double[] _Bi4;

		protected double[] _Ci4;

		protected double[] _Di4;

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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

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

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Pow(delta, di3[i]) * Math.Pow(tau, ti3[i]) * Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i]));
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));
				sum4 += ni4[i] * Math.Pow(Delta, bi4[i]) * delta * Psi;
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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

			double sum1 = 0;
			for (int i = 0; i < ni1.Length; ++i)
			{
				sum1 += ni1[i] * di1[i] * Pow(delta, di1[i] - 1) * Math.Pow(tau, ti1[i]);
			}

			double sum2 = 0;
			for (int i = 0; i < ni2.Length; ++i)
			{
				sum2 += ni2[i] * Math.Exp(-Pow(delta, ci2[i])) * (Pow(delta, di2[i] - 1) * Math.Pow(tau, ti2[i]) * (di2[i] - ci2[i] * Pow(delta, ci2[i])));
			}

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Pow(delta, di3[i]) * Math.Pow(tau, ti3[i]) * Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i])) * (di3[i] / delta - 2 * alphai3[i] * (delta - epsiloni3[i]));
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));
				double Psi_delta = -2 * Ci4[i] * (delta - 1) * Psi;

				// Derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) * (Ai4[i] * theta * 2 / betai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 1) + 2 * Bi4[i] * ai4[i] * Math.Pow(Pow2(delta - 1), ai4[i] - 1));

				// Derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi4[i] * Math.Pow(Delta, bi4[i] - 1) * Delta_delta;

				sum4 += ni4[i] * (Math.Pow(Delta, bi4[i]) * (Psi + delta * Psi_delta) + Deltabi_delta * delta * Psi);
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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

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

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Math.Pow(tau, ti3[i]) * Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i])) *
					(
					-2 * alphai3[i] * Pow(delta, di3[i]) +
					4 * Pow2(alphai3[i]) * Pow(delta, di3[i]) * Pow2(delta - epsiloni3[i]) -
					4 * di3[i] * alphai3[i] * Pow(delta, di3[i] - 1) * (delta - epsiloni3[i]) +
					di3[i] * (di3[i] - 1) * Pow(delta, di3[i] - 2)
					);
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));
				double Psi_delta = -2 * Ci4[i] * (delta - 1) * Psi;
				double Psi_deltadelta = (2 * Ci4[i] * Pow2(delta - 1) - 1) * (2 * Ci4[i] * Psi); // 2nd derivative of Psi with respect to delta

				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);

				// 1st derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) *
					(
					Ai4[i] * theta * 2 / betai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 1) +
					2 * Bi4[i] * ai4[i] * Math.Pow(Pow2(delta - 1), ai4[i] - 1)
					);

				// 2nd derivative of Delta with respect to delta
				double Delta_deltadelta = Delta_delta / (delta - 1) + Pow2(delta - 1) *
					(
						4 * Bi4[i] * ai4[i] * (ai4[i] - 1) * Math.Pow(Pow2(delta - 1), ai4[i] - 2) +
						2 * Pow2(Ai4[i] / betai4[i]) * Pow2(Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 1)) +
						Ai4[i] * theta * 4 / betai4[i] * (1 / (2 * betai4[i]) - 1) * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 2)
					);

				// 1st derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi4[i] * Math.Pow(Delta, bi4[i] - 1) * Delta_delta;

				// 2nd derivative of Delta^bi with respect to delta
				double Deltabi_deltadelta = bi4[i] *
					(
						Math.Pow(Delta, bi4[i] - 1) * Delta_deltadelta +
						(bi4[i] - 1) * Math.Pow(Delta, bi4[i] - 2) * Pow2(Delta_delta)
					);

				sum4 += ni4[i] *
					(
						Math.Pow(Delta, bi4[i]) * (2 * Psi_delta + delta * Psi_deltadelta) +
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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

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

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Pow(delta, di3[i]) * Math.Pow(tau, ti3[i]) *
								Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i])) *
								(ti3[i] / tau - 2 * betai3[i] * (tau - gammai3[i]));
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di4[i] * (tau - 1) * Psi;

				// Derivative of Delta^bi with respect to delta
				double Deltabi_tau = -2 * theta * bi4[i] * Math.Pow(Delta, bi4[i] - 1);

				sum4 += ni4[i] * delta * (
									Deltabi_tau * Psi +
									Math.Pow(Delta, bi4[i]) * Psi_tau
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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

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

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Pow(delta, di3[i]) * Math.Pow(tau, ti3[i]) *
								Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i])) *
								(
								Pow2(ti3[i] / tau - 2 * betai3[i] * (tau - gammai3[i])) -
								ti3[i] / Pow2(tau) -
								2 * betai3[i]
								);
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di4[i] * (tau - 1) * Psi;

				// 2nd derivative of Psi with respect to tau
				double Psi_tautau = (2 * Di4[i] * Pow2(tau - 1) - 1) * 2 * Di4[i] * Psi;

				// 1st derivative of Delta^bi with respect to tau
				double Deltabi_tau = -2 * theta * bi4[i] * Math.Pow(Delta, bi4[i] - 1);

				// 2nd derivative of Delta^bi with respect to tau
				double Deltabi_tautau = 2 * bi4[i] * Math.Pow(Delta, bi4[i] - 1) + 4 * Pow2(theta) * bi4[i] * (bi4[i] - 1) * Math.Pow(Delta, bi4[i] - 2);

				sum4 += ni4[i] * delta *
								(
									Deltabi_tautau * Psi +
									2 * Deltabi_tau * Psi_tau +
									Math.Pow(Delta, bi4[i]) * Psi_tautau
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
			var ni1 = _ni1;
			var di1 = _di1;
			var ti1 = _ti1;
			var ni2 = _ni2;
			var ci2 = _ci2;
			var di2 = _di2;
			var ti2 = _ti2;
			var ni3 = _ni3;
			var di3 = _di3;
			var ti3 = _ti3;
			var alphai3 = _alphai3;
			var betai3 = _betai3;
			var gammai3 = _gammai3;
			var epsiloni3 = _epsiloni3;
			var ni4 = _ni4;
			var ai4 = _ai4;
			var Ai4 = _Ai4;
			var bi4 = _bi4;
			var Bi4 = _Bi4;
			var Ci4 = _Ci4;
			var Di4 = _Di4;
			var betai4 = _betai4;

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

			double sum3 = 0;
			for (int i = 0; i < ni3.Length; ++i)
			{
				sum3 += ni3[i] * Pow(delta, di3[i]) * Math.Pow(tau, ti3[i]) *
								Math.Exp(-alphai3[i] * Pow2(delta - epsiloni3[i]) - betai3[i] * Pow2(tau - gammai3[i])) *
								(
									di3[i] / delta -
									2 * alphai3[i] * (delta - epsiloni3[i])
								) *
								(
									ti3[i] / tau -
									2 * betai3[i] * (tau - gammai3[i])
								);
			}

			double sum4 = 0;
			for (int i = 0; i < ni4.Length; ++i)
			{
				double theta = (1 - tau) + Ai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]));
				double Delta = Pow2(theta) + Bi4[i] * Math.Pow(Pow2(delta - 1), ai4[i]);
				double Psi = Math.Exp(-Ci4[i] * Pow2(delta - 1) - Di4[i] * Pow2(tau - 1));
				double Psi_delta = -2 * Ci4[i] * (delta - 1) * Psi;

				// 1st derivative of Psi with respect to tau
				double Psi_tau = -2 * Di4[i] * (tau - 1) * Psi;

				// derivative of Psi with respect to delta and tau
				double Psi_deltatau = 4 * Ci4[i] * Di4[i] * (delta - 1) * (tau - 1) * Psi;

				// Derivative of Delta with respect to delta
				double Delta_delta = (delta - 1) * (Ai4[i] * theta * 2 / betai4[i] * Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 1) + 2 * Bi4[i] * ai4[i] * Math.Pow(Pow2(delta - 1), ai4[i] - 1));

				// 1st derivative of Delta^bi with respect to delta
				double Deltabi_delta = bi4[i] * Math.Pow(Delta, bi4[i] - 1) * Delta_delta;

				// 1st derivative of Delta^bi with respect to tau
				double Deltabi_tau = -2 * theta * bi4[i] * Math.Pow(Delta, bi4[i] - 1);

				// derivative of Delta ^ bi with respect to delta and tau
				double Deltabi_deltatau = -Ai4[i] * bi4[i] * 2 / betai4[i] * Math.Pow(Delta, bi4[i] - 1) * (delta - 1) *
																	Math.Pow(Pow2(delta - 1), 1 / (2 * betai4[i]) - 1) -
																	2 * theta * bi4[i] * (bi4[i] - 1) * Math.Pow(Delta, bi4[i] - 2) * Delta_delta;

				sum4 += ni4[i] *
								(
									Math.Pow(Delta, bi4[i]) * (Psi_tau + delta * Psi_deltatau) +
									delta * Deltabi_delta * Psi_tau +
									Deltabi_tau * (Psi + delta * Psi_delta) +
									Deltabi_deltatau * delta * Psi
								);
			}

			return sum1 + sum2 + sum3 + sum4;
		}

		#endregion Residual part of dimensionless Helmholtz energy and derivatives

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
