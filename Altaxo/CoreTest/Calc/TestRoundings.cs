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

using Altaxo.Calc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AltaxoTest.Calc
{
	[TestFixture]
	public class TestRoundings
	{
		[Test]
		public void TestRoundUpDownMod1()
		{
			var rnd = new Random();

			for (int i = 0; i < 1000; ++i)
			{
				var n = rnd.Next(int.MinValue, int.MaxValue);

				Assert.AreEqual(n, Rounding.RoundUp(n, 1));
				Assert.AreEqual(n, Rounding.RoundDown(n, 1));
				Assert.AreEqual(n, Rounding.RoundUp((long)n, 1));
				Assert.AreEqual(n, Rounding.RoundDown((long)n, 1));
			}
		}

		[Test]
		public void TestRoundUpIntMod5()
		{
			for (int i = -500; i < 500; ++i)
			{
				var r = Rounding.RoundUp(i, 5);
				Assert.GreaterOrEqual(r, i, "n=" + i.ToString());
				Assert.LessOrEqual(r - i, 4, "n=" + i.ToString());
			}
		}

		[Test]
		public void TestRoundDownIntMod5()
		{
			for (int i = -500; i < 500; ++i)
			{
				var r = Rounding.RoundDown(i, 5);
				Assert.LessOrEqual(r, i, "n=" + i.ToString());
				Assert.LessOrEqual(i - r, 4, "n=" + i.ToString());
			}
		}

		[Test]
		public void TestRoundUpLongMod5()
		{
			for (long i = -500; i < 500; ++i)
			{
				var r = Rounding.RoundUp(i, 5);
				Assert.GreaterOrEqual(r, i, "n=" + i.ToString());
				Assert.LessOrEqual(r - i, 4, "n=" + i.ToString());
			}
		}

		[Test]
		public void TestRoundDownLongMod5()
		{
			for (long i = -500; i < 500; ++i)
			{
				var r = Rounding.RoundDown(i, 5);
				Assert.LessOrEqual(r, i, "n=" + i.ToString());
				Assert.LessOrEqual(i - r, 4, "n=" + i.ToString());
			}
		}

		[Test]
		public void TestRoundToSignificantDigits()
		{
			Assert.AreEqual(0.01, Rounding.RoundToNumberOfSignificantDigits(0.01, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(0.1, Rounding.RoundToNumberOfSignificantDigits(0.1, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(1, Rounding.RoundToNumberOfSignificantDigits(1, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(0, Rounding.RoundToNumberOfSignificantDigits(0, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(10, Rounding.RoundToNumberOfSignificantDigits(10, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(100, Rounding.RoundToNumberOfSignificantDigits(100, 2, MidpointRounding.ToEven), 1E-6);

			Assert.AreEqual(0.012, Rounding.RoundToNumberOfSignificantDigits(0.012345678, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(1.2, Rounding.RoundToNumberOfSignificantDigits(1.2345678, 2, MidpointRounding.ToEven), 1E-6);
			Assert.AreEqual(120, Rounding.RoundToNumberOfSignificantDigits(123.45678, 2, MidpointRounding.ToEven), 1E-6);
		}
	}
}