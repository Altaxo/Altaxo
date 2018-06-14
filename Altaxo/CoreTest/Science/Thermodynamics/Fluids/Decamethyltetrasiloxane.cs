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

namespace Science.Thermodynamics.Fluids
{
	[TestFixture]
	internal class Decamethyltetrasiloxane
	{
		[Test]
		public void Test_350K_2900molm3()
		{
			var material = Altaxo.Science.Thermodynamics.Fluids.Decamethyltetrasiloxane.Instance;

			double temperature = 350;
			double moleDensity = 2900;
			var massDensity = moleDensity * material.MolecularWeight;

			var delta = moleDensity / material.CriticalPointMoleDensity;
			var tau = material.CriticalPointTemperature / temperature;

			AssertHelper.AreEqual(-7.0351025E+00, material.PhiR_OfReducedVariables(delta, tau), 0, 1E-6);
			AssertHelper.AreEqual(1.0570249E+01, delta * material.PhiR_delta_OfReducedVariables(delta, tau), 0, 1E-6);

			AssertHelper.AreEqual(9.764338E7, material.Pressure_FromMassDensityAndTemperature(massDensity, temperature), 0, 1E-6);
			// Assert.AreEqual(24555.74, material.MolecularWeight * material.MassSpecificInternalEnergy_FromMassDensityAndTemperature(massDensity, temperature), 1E-1);
		}
	}
}
