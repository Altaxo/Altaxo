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
	/// Represents a binary mixture.
	/// </summary>
	/// <remarks>
	/// Reference:
	/// Johannes Gernert, Roland Span,
	/// EOS–CG: A Helmholtz energy mixture model for humid gases and CCS	mixtures,
	/// J. Chem. Thermodynamics 93 (2016) 274–293
	/// </remarks>
	public class MixtureCarbondioxideAndWater : HelmholtzEquationOfStateOfBinaryMixturesByWagnerEtAl
	{
		public MixtureCarbondioxideAndWater(double moleFractionCarbondioxide)
				: base(moleFractionCarbondioxide, Carbondioxide.Instance, (1 - moleFractionCarbondioxide), Water.Instance)
		{
		}

		public override double WorkingUniversalGasConstant => 8.314472;

		protected override void InitializeCoefficientArrays()
		{
			_betaT12 = 1.030538;
			_gammaT12 = 0.828472;
			_betaV12 = 1.021392;
			_gammaV12 = 0.895156;
			_F12 = 1;

			_pr1 = new(double ni, double ti, int di)[]
			{
( 0.39440467e+00,   0.880,   1),
(-0.17634732e+01,   2.932,   1),
( 0.14620755e+00,   2.433,   3),
			};

			_pr2 = new(double ni, double ti, int di, int ci)[]
{
( 0.87522320e-02,   1.330,   0,   1),
( 0.20349398e+01,   4.416,   2,   1),
(-0.90350250e-01,   5.514,   3,   1),
(-0.21638854e+00,   5.203,   1,   2),
( 0.39612170e-01,   1.000,   5,   2),
};
		}

		public static MixtureCarbondioxideAndWater FromMoleFractionCO2(double moleFractionCO2)
		{
			return new MixtureCarbondioxideAndWater(moleFractionCO2);
		}

		public static MixtureCarbondioxideAndWater FromMassFractionCO2(double massFractionCO2)
		{
			var moleFraction = GetMoleFractionFromMassFraction(massFractionCO2, Carbondioxide.Instance.MolecularWeight, Water.Instance.MolecularWeight);
			return new MixtureCarbondioxideAndWater(moleFraction);
		}
	}
}
