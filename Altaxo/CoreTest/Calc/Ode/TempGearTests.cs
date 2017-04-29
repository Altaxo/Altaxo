#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Ode.Temp
{
	[TestFixture]
	public class TempGearsTest
	{
		[Test]
		public void GearTest2a()
		{
			const double lambda1 = -1;
			const double lambda2 = -1000;
			const double lambda1PlusLambda2By2 = (lambda1 + lambda2) / 2;
			const double lambda1MinusLambda2By2 = (lambda1 - lambda2) / 2;

			const double C1 = 1;
			const double C2 = 1;

			Vector fuu(double t, Vector y)
			{
				return new Vector(lambda1PlusLambda2By2 * y[0] + lambda1MinusLambda2By2 * y[1], lambda1MinusLambda2By2 * y[0] + lambda1PlusLambda2By2 * y[1]);
			}

			var pulse = Altaxo.Calc.Ode.Ode.GearBDF(
			0,
			new Vector(C1 + C2, C1 - C2),
			fuu,
			new Altaxo.Calc.Ode.Options { RelativeTolerance = 1e-4, AbsoluteTolerance = 1E-8 });

			var ode = new GearsBDF();

			ode.Initialize(
			0,
			new double[] { C1 + C2, C1 - C2 },
			(t, y, dydt) => { dydt[0] = lambda1PlusLambda2By2 * y[0] + lambda1MinusLambda2By2 * y[1]; dydt[1] = lambda1MinusLambda2By2 * y[0] + lambda1PlusLambda2By2 * y[1]; },
			new Options { RelativeTolerance = 1e-4, AbsoluteTolerance = 1E-8 });

			var sp = new double[2];
			foreach (var spulse in pulse.SolveTo(100000))
			{
				double t = spulse.T;
				ode.Evaluate(t, sp);

				var y0_expected = C1 * Math.Exp(lambda1 * t) + C2 * Math.Exp(lambda2 * t);
				var y1_expected = C1 * Math.Exp(lambda1 * t) - C2 * Math.Exp(lambda2 * t);

				Assert.AreEqual(y0_expected, sp[0], 1E-3 * y0_expected + 1E-4);
				Assert.AreEqual(y1_expected, sp[1], 1E-3 * y1_expected + 1E-4);
			}
		}
	}
}