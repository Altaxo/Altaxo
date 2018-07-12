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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	[TestFixture]
	internal class Test_MixtureCarbondioxideAndWater
	{
		[Test]
		public void TestTable8_1()
		{
			var mix = Altaxo.Science.Thermodynamics.Fluids.MixtureCarbondioxideAndWater.FromMoleFractionCO2(0.95);

			var moleDensity = 250; // mol/m³
			var temperature = 450;

			var massDensity = mix.GetMassDensityFromMolarDensity(moleDensity);
			var moleDensity3 = mix.GetMolarDensityFromMoleFraction1AndTotalMassDensity(0.95, massDensity);

			var delta = massDensity / mix.ReducingMassDensity;
			var tau = mix.ReducingTemperature / temperature;

			Assert_AreEqual(-1.1118412E-02, mix.PhiR_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-1.1280889E-02, delta * mix.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-3.4064003E-02, tau * mix.PhiR_tau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(1.8012251E-04, delta * delta * mix.PhiR_deltadelta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-2.8066041E-02, tau * tau * mix.PhiR_tautau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-3.4262128E-02, delta * tau * mix.PhiR_deltatau_OfReducedVariables(delta, tau), 0, 1E-7);

			Assert_AreEqual(-4.1299125E+00, tau * tau * mix.Phi0_tautau_OfReducedVariables(delta, tau), 0, 1E-4);

			Assert_AreEqual(0.924830E6, mix.Pressure_FromMassDensityAndTemperature(massDensity, temperature), 0, 1E-6);
			Assert_AreEqual(34.5713453, mix.MassSpecificIsochoricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(43.4715971, mix.MassSpecificIsobaricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(328.16, mix.SpeedOfSound_FromMassDensityAndTemperature(massDensity, temperature), 0.01, 1E-6);

			// Tests that depend on the absolute value (offset) of Tau0 are
			// not fullfilled currently, because the zero point is set differently
			// from the papers of CO2 and water
			// Assert.AreEqual(6.8982972E+00, tau * mix.Phi0_tau_OfReducedVariables(delta, tau), 1E-6);
			// Assert.AreEqual(-7.5738019E+00, mix.Phi0_OfReducedVariables(delta, tau), 1E-6);
			// Assert.AreEqual(25682.58, mix.MassSpecificInternalEnergy_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 1E-4);
		}

		[Test]
		public void TestTable8_1a()
		{
			var mix = new MixtureOfFluids(new(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[] { (CarbonDioxide.Instance, 0.95), (Water.Instance, 0.05) });

			var moleDensity = 250; // mol/m³
			var temperature = 450;

			var delta = moleDensity / mix.ReducingMoleDensity;
			var tau = mix.ReducingTemperature / temperature;

			Assert_AreEqual(-1.1118412E-02, mix.PhiR_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-1.1280889E-02, delta * mix.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-3.4064003E-02, tau * mix.PhiR_tau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(1.8012251E-04, delta * delta * mix.PhiR_deltadelta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-2.8066041E-02, tau * tau * mix.PhiR_tautau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-3.4262128E-02, delta * tau * mix.PhiR_deltatau_OfReducedVariables(delta, tau), 0, 1E-7);

			Assert_AreEqual(-4.1299125E+00, tau * tau * mix.Phi0_tautau_OfReducedVariables(delta, tau), 0, 1E-4);

			Assert_AreEqual(0.924830E6, mix.Pressure_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(34.5713453, mix.MoleSpecificIsochoricHeatCapacity_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(43.4715971, mix.MoleSpecificIsobaricHeatCapacity_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(328.16, mix.SpeedOfSound_FromMoleDensityAndTemperature(moleDensity, temperature), 0.01, 1E-6);

			// Tests that depend on the absolute value (offset) of Tau0 are
			// not fullfilled currently, because the zero point is set differently
			// from the papers of CO2 and water
			// Assert.AreEqual(6.8982972E+00, tau * mix.Phi0_tau_OfReducedVariables(delta, tau), 1E-6);
			// Assert.AreEqual(-7.5738019E+00, mix.Phi0_OfReducedVariables(delta, tau), 1E-6);
			// Assert.AreEqual(25682.58, mix.MassSpecificInternalEnergy_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 1E-4);
		}

		[Test]
		public void TestTable8_2()
		{
			var mix = Altaxo.Science.Thermodynamics.Fluids.MixtureCarbondioxideAndWater.FromMoleFractionCO2(0.95);

			var moleDensity = 10000; // mol/m³
			var temperature = 550;

			var massDensity = mix.GetMassDensityFromMolarDensity(moleDensity);
			var moleDensity3 = mix.GetMolarDensityFromMoleFraction1AndTotalMassDensity(0.95, massDensity);

			var delta = massDensity / mix.ReducingMassDensity;
			var tau = mix.ReducingTemperature / temperature;

			Assert_AreEqual(-1.2167537E-01, mix.PhiR_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-1.0102150E-02, delta * mix.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-8.8492294E-01, tau * mix.PhiR_tau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(2.5822994E-01, delta * delta * mix.PhiR_deltadelta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-4.2942636E-01, tau * tau * mix.PhiR_tautau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-8.3978601E-01, delta * tau * mix.PhiR_deltatau_OfReducedVariables(delta, tau), 0, 1E-7);

			Assert_AreEqual(-4.4761904E+00, tau * tau * mix.Phi0_tautau_OfReducedVariables(delta, tau), 0, 1E-4);

			Assert_AreEqual(45.267798E6, mix.Pressure_FromMassDensityAndTemperature(massDensity, temperature), 0, 1E-6);
			Assert_AreEqual(63.2707867, mix.MassSpecificIsobaricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(40.7875533, mix.MassSpecificIsochoricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(453.46, mix.SpeedOfSound_FromMassDensityAndTemperature(massDensity, temperature), 0.01, 1e-6);
		}

		[Test]
		public void TestTable8_2a()
		{
			var mix = new MixtureOfFluids(new(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl, double)[] { (CarbonDioxide.Instance, 0.95), (Water.Instance, 0.05) });

			var moleDensity = 10000; // mol/m³
			var temperature = 550;

			//var massDensity = mix.GetMassDensityFromMolarDensity(moleDensity);
			//var moleDensity3 = mix.GetMolarDensityFromMoleFraction1AndTotalMassDensity(0.95, massDensity);

			var delta = moleDensity / mix.ReducingMoleDensity;
			var tau = mix.ReducingTemperature / temperature;

			Assert_AreEqual(-1.2167537E-01, mix.PhiR_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-1.0102150E-02, delta * mix.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-8.8492294E-01, tau * mix.PhiR_tau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(2.5822994E-01, delta * delta * mix.PhiR_deltadelta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-4.2942636E-01, tau * tau * mix.PhiR_tautau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-8.3978601E-01, delta * tau * mix.PhiR_deltatau_OfReducedVariables(delta, tau), 0, 1E-7);

			Assert_AreEqual(-4.4761904E+00, tau * tau * mix.Phi0_tautau_OfReducedVariables(delta, tau), 0, 1E-4);

			Assert_AreEqual(45.267798E6, mix.Pressure_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(63.2707867, mix.MoleSpecificIsobaricHeatCapacity_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(40.7875533, mix.MoleSpecificIsochoricHeatCapacity_FromMoleDensityAndTemperature(moleDensity, temperature), 0, 1E-6);
			Assert_AreEqual(453.46, mix.SpeedOfSound_FromMoleDensityAndTemperature(moleDensity, temperature), 0.01, 1e-6);
		}

		[Test]
		public void TestGen_1()
		{
			var mix = Altaxo.Science.Thermodynamics.Fluids.MixtureCarbondioxideAndWater.FromMoleFractionCO2(0.02);

			var moleDensity = 53912.169315; // mol/m³
			var temperature = 323.15;

			var massDensity = mix.GetMassDensityFromMolarDensity(moleDensity);
			var moleDensity3 = mix.GetMolarDensityFromMoleFraction1AndTotalMassDensity(0.95, massDensity);

			var delta = massDensity / mix.ReducingMassDensity;
			var tau = mix.ReducingTemperature / temperature;

			Assert_AreEqual(-8.1737533E+00, mix.PhiR_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-8.9644508E-01, delta * mix.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-1.4816374E+01, tau * mix.PhiR_tau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(1.8981217E+01, delta * delta * mix.PhiR_deltadelta_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-5.7131280E+00, tau * tau * mix.PhiR_tautau_OfReducedVariables(delta, tau), 0, 1E-7);
			Assert_AreEqual(-3.4517765E+00, delta * tau * mix.PhiR_deltatau_OfReducedVariables(delta, tau), 0, 1E-7);

			Assert_AreEqual(-3.0656039E+00, tau * tau * mix.Phi0_tautau_OfReducedVariables(delta, tau), 0, 1E-4);

			Assert_AreEqual(15.0E6, mix.Pressure_FromMassDensityAndTemperature(massDensity, temperature), 0, 1E-6);
			Assert_AreEqual(78.7686719, mix.MassSpecificIsobaricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(72.9904132, mix.MassSpecificIsochoricHeatCapacity_FromMassDensityAndTemperature(massDensity, temperature) * mix.MolecularWeight, 0, 1E-6);
			Assert_AreEqual(1686.78, mix.SpeedOfSound_FromMassDensityAndTemperature(massDensity, temperature), 0.01, 1e-6);
		}

		private static void Assert_AreEqual(double expected, double actual, double absError, double relError)
		{
			double delta = Math.Abs(absError) + Math.Abs(relError * expected);
			Assert.AreEqual(expected, actual, delta);
		}

		[Test]
		public void TestDelta()
		{
			double _moleFraction1 = 0.95, CriticalPointDensity1 = 10624.9063;
			double _moleFraction2 = 0.05, CriticalPointDensity2 = 17873.7280;
			double _betaV12 = 1.021392;
			double _gammaV12 = 0.895156;

			double molarMass1 = 0.0440098;
			double molarMass2 = 0.018015268;

			double sum = 0;
			sum += Pow2(_moleFraction1) / CriticalPointDensity1;
			sum += Pow2(_moleFraction2) / CriticalPointDensity2;
			sum += 2 * _moleFraction1 * _moleFraction2 * _betaV12 * _gammaV12 * (_moleFraction1 + _moleFraction2) / (Pow2(_betaV12) * _moleFraction1 + _moleFraction2) * 0.125 * Math.Pow(Math.Pow(CriticalPointDensity1, -1 / 3.0) + Math.Pow(CriticalPointDensity2, -1 / 3.0), 3);
			var reducingDensity = 1 / sum; // mol/m3

			var reducingMassDensity = _moleFraction1 * reducingDensity * molarMass1 + _moleFraction2 * reducingDensity * molarMass2;

			var result = 250 / reducingDensity;

			Assert.AreEqual(0.02280115, result, 0.00001);
		}

		public double Pow2(double x) => x * x;
	}
}
