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

using Altaxo.Calc.Integration;
using NUnit.Framework;
using System;

namespace AltaxoTest.Calc.Integration
{
	[TestFixture]
	public class QAGPTests
	{
		[Test]
		public void TestSin()
		{
			double result, abserr;
			QagpIntegration.Integration(Math.Sin, new double[] { 0, Math.PI }, 2, 0, 1E-6, 100, out result, out abserr);

			NUnit.Framework.Assert.AreEqual(2.0, result, 2.0 * 1E-6);
		}

		[Test]
		public void TestOneBySqrtX()
		{
			double result, abserr;
			QagpIntegration.Integration(delegate (double x) { return 1 / Math.Sqrt(Math.Abs(x)); }, new double[] { -1, 0, 1 }, 3, 0, 1E-6, 100, out result, out abserr);

			NUnit.Framework.Assert.AreEqual(4.0, result, 4.0 * 1E-6);
		}
	}
}
