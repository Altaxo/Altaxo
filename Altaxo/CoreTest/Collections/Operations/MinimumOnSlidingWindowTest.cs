#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

namespace AltaxoTest.Collections.Operations
{
	[TestFixture]
	public class TestMinimumOnSlidingWindow
	{
		private System.Random _rnd = new System.Random(8542221);
		private double[] vals;

		private void Initialize()
		{
			vals = new double[10000];
			for (int i = 0; i < vals.Length; ++i)
			{
				vals[i] = Math.Floor(short.MaxValue * _rnd.NextDouble());
			}
		}

		private double GetMinimumOnSlidingWindowNaiveVersion(int start, int length, double[] vals)
		{
			int end = start + length;
			start = Math.Max(0, start);
			end = Math.Min(vals.Length, end);

			double result = double.PositiveInfinity;
			for (int i = start; i < end; ++i)
			{
				result = Math.Min(result, vals[i]);
			}
			return result;
		}

		public void TestSingleAddsWithWindowWidth(int windowWidth)
		{
			if (null == vals)
				Initialize();

			var min = new Altaxo.Collections.Operations.MinimumOnSlidingWindow<double>(windowWidth, double.PositiveInfinity);
			for (int i = 0; i < vals.Length - 1; ++i)
			{
				min.Add(vals[i]);
				double naiveResult = GetMinimumOnSlidingWindowNaiveVersion(1 + i - windowWidth, windowWidth, vals);

				Assert.AreEqual(naiveResult, min.MinimumValue, string.Format("Difference between naive version and Altaxo's implementation at i={0}, windowWidth={1}", i, windowWidth));
			}
		}

		[Test]
		public void TestSingleAdds()
		{
			TestSingleAddsWithWindowWidth(5);
			TestSingleAddsWithWindowWidth(73);
		}
	}
}