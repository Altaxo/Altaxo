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
				: base(moleFractionCarbondioxide, CarbonDioxide.Instance, (1 - moleFractionCarbondioxide), Water.Instance)
		{
		}

		public override double WorkingUniversalGasConstant => UniversalGasConstant;

		protected override void InitializeCoefficientArrays()
		{
			_betaT12 = 1.030538;
			_gammaT12 = 0.828472;
			_betaV12 = 1.021392;
			_gammaV12 = 0.895156;
			_F12 = 1;

			_ni1 = new double[]
			{
				3.9440467E-1,
			-1.7634732,
			1.4620755E-1,
			};

			_di1 = new int[]
			{
				1 ,
			1 ,
			3 ,
			};

			_ti1 = new double[]
			{
				0.880,
			2.932,
			2.433,
			};

			_ni2 = new double[]
			{
			8.7522320E-3,
			2.0349398,
			-9.0350250E-2,
			-2.1638854E-1,
			3.9612170E-2,
			};

			_ti2 = new double[]
			{
			1.330,
			4.416,
			5.514,
 			5.203,
 			1.000,
			};

			_di2 = new int[]
			{
			0 ,
			2 ,
			3,
			1 ,
			5 ,
			};

			_ci2 = new int[]
			{
			1 ,
			1 ,
			1,
			2 ,
			2 ,
			};
		}

		public static MixtureCarbondioxideAndWater FromMoleFractionCO2(double moleFractionCO2)
		{
			return new MixtureCarbondioxideAndWater(moleFractionCO2);
		}

		public static MixtureCarbondioxideAndWater FromMassFractionCO2(double massFractionCO2)
		{
			var moleFraction = GetMoleFractionFromMassFraction(massFractionCO2, CarbonDioxide.Instance.MolecularWeight, Water.Instance.MolecularWeight);
			return new MixtureCarbondioxideAndWater(moleFraction);
		}
	}
}
