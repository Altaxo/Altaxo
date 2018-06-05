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
	/// Equation of state of a binary system, either a mixture, or a segregated system of two components.
	/// </summary>
	/// <seealso cref="Altaxo.Science.Thermodynamics.Fluids.HelmholtzEquationOfState" />
	public abstract class HelmholtzEquationOfStateOfBinarySystem : HelmholtzEquationOfState
	{
		protected double _moleFraction1;
		protected HelmholtzEquationOfStateOfPureFluids _component1;

		protected double _moleFraction2;
		protected HelmholtzEquationOfStateOfPureFluids _component2;

		protected HelmholtzEquationOfStateOfBinarySystem(double moleFraction1, HelmholtzEquationOfStateOfPureFluids component1, double moleFraction2, HelmholtzEquationOfStateOfPureFluids component2)
		{
			if (!(moleFraction1 >= 0 && moleFraction1 <= 1))
				throw new ArgumentOutOfRangeException(nameof(moleFraction1));
			if (!(moleFraction2 >= 0 && moleFraction2 <= 1))
				throw new ArgumentOutOfRangeException(nameof(moleFraction2));
			if (!((moleFraction1 + moleFraction2) <= 1))
				throw new ArgumentOutOfRangeException(nameof(moleFraction2), "both mole fractions must sum up to a value <=1");

			_moleFraction1 = moleFraction1;
			_component1 = component1 ?? throw new ArgumentNullException(nameof(component1));
			_moleFraction2 = moleFraction2;
			_component2 = component2 ?? throw new ArgumentNullException(nameof(component2));
		}

		public virtual double MolecularWeight
		{
			get
			{
				return _moleFraction1 * _component1.MolecularWeight + _moleFraction2 * _component2.MolecularWeight;
			}
		}

		public override double SpecificGasConstant
		{
			get
			{
				double inv = _moleFraction1 / _component1.SpecificGasConstant + _moleFraction2 / _component2.SpecificGasConstant;
				return 1 / inv;
			}
		}

		public double GetMoleFraction1FromMassFraction1(double massFraction1)
		{
			return GetMoleFractionFromMassFraction(massFraction1, _component1.MolecularWeight, _component2.MolecularWeight);
		}

		public double GetMassFraction1FromMoleFraction1(double moleFraction1)
		{
			return GetMassFractionFromMoleFraction(moleFraction1, _component1.MolecularWeight, _component2.MolecularWeight);
		}

		public double GetMassDensityFromMolarDensity(double molarDensity)
		{
			return GetMassDensityFromMolarDensity(_moleFraction1, _component1.MolecularWeight, _moleFraction2, _component2.MolecularWeight, molarDensity);
		}

		public double GetMolarDensityFromMoleFraction1AndTotalMassDensity(double moleFraction1, double massDensity)
		{
			return massDensity / (moleFraction1 * _component1.MolecularWeight + (1 - moleFraction1) * _component2.MolecularWeight);
		}

		public override double Phi0_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double Phi0_tautau_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double Phi0_tau_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_deltadelta_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_deltatau_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_delta_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_tautau_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		public override double PhiR_tau_OfReducedVariables(double delta, double tau)
		{
			throw new NotImplementedException();
		}

		#region Static molar / mass conversions

		public static double GetMassFractionFromMoleFraction(double moleFraction1, double molarMass1, double molarMass2)
		{
			return moleFraction1 * molarMass1 / (moleFraction1 * molarMass1 + (1 - moleFraction1) * molarMass2);
		}

		public static double GetMoleFractionFromMassFraction(double massFraction1, double molarMass1, double molarMass2)
		{
			double mol1 = massFraction1 / molarMass1;
			double mol2 = (1 - massFraction1) / molarMass2;

			return mol1 / (mol1 + mol2);
		}

		public double GetMassDensityFromMolarDensity(double moleFraction1, double molarMass1, double moleFraction2, double molarMass2, double molarDensity)
		{
			double massDensity1 = molarDensity * moleFraction1 * molarMass1;
			double massDensity2 = molarDensity * moleFraction2 * molarMass2;
			return massDensity1 + massDensity2;
		}

		#endregion Static molar / mass conversions
	}
}
