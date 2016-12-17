#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using NUnit.Framework;
using System;

namespace Altaxo.Calc.Ode
{
	[TestFixture]
	public class SparseVectorTests
	{
		private const double Eps = 1e-10;

		[Test]
		public void ElementAccessorTest()
		{
			SparseVector sv = new SparseVector(1000);
			for (var i = 0; i < sv.Length / 2; i++)
			{
				sv[i] = i * i;
				sv[sv.Length - i - 1] = sv.Length - i - 1;
				for (var j = 0; j <= i; j++)
				{
					Assert.AreEqual(sv[i], i * i, Eps);
					Assert.AreEqual(sv[sv.Length - i - 1], sv.Length - i - 1);
				}
			}
		}
	}
}