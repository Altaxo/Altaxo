using System;
using Altaxo.Calc;
using NUnit.Framework;


namespace Altaxo.Test.Calc
{
	[TestFixture]
	public class TestBesselRelated
	{

		[Test]
		public  void TestBesselK1()
		{
			Assertion.AssertEquals(0.6019072301972345747375400,BesselRelated.BesselK1(1),1E-14);
		}

		[Test]
		public  void TestBesselExpK1()
		{
			Assertion.AssertEquals(1.636153486263258246513311,BesselRelated.BesselExpK1(1),1E-14);
		}
	}
}
