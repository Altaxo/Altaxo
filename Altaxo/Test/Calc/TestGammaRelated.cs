using System;
using Altaxo.Calc;
using NUnit.Framework;


namespace Altaxo.Test.Calc
{
	[TestFixture]
	public class TestGammaRelated
	{

		[Test]
		public  void TestGamma()
		{
			Assertion.AssertEquals(1.0,GammaRelated.Gamma(1.0),0);
			Assertion.AssertEquals(0.8862269254527580136490837,GammaRelated.Gamma(1.5),1e-15);
			Assertion.AssertEquals(1.0,GammaRelated.Gamma(2.0),0);
			Assertion.AssertEquals(2.0,GammaRelated.Gamma(3.0),0);
			Assertion.AssertEquals(2.2880377953400324180,GammaRelated.Gamma(Math.PI),1e-15);
			Assertion.AssertEquals(99999.42279422556767349323,GammaRelated.Gamma(1e-5),1e5*1e-10);

			Assertion.AssertEquals(-4.062353818279201250,GammaRelated.Gamma(-1.0/3.0),1e-15);
			Assertion.AssertEquals(0.0002238493288596894971637404,GammaRelated.Gamma(-7.5),1e-15);
			Assertion.AssertEquals(-100000.57722555555224,GammaRelated.Gamma(-1.0/100000),100000*1e-10);
		}



		[Test]
		public void TestComplexGamma()
		{
			Complex result;
			result = GammaRelated.Gamma(new Complex(0.5,0.5));
			Assertion.AssertEquals(0.8181639995417473940777489, result.Re, 1e-14);
			Assertion.AssertEquals(-0.7633138287139826166702968 , result.Im,1e-14);
		}



		[Test]
		public void TestFac()
		{
			Assertion.AssertEquals(1,GammaRelated.Fac(1),0);
			Assertion.AssertEquals(2,GammaRelated.Fac(2),0);
			Assertion.AssertEquals(6,GammaRelated.Fac(3),0);
			Assertion.AssertEquals(3628800,GammaRelated.Fac(10),0);
		}

		[Test]
		public void TestBetaI()
		{
			Assertion.AssertEquals(0.0109375,GammaRelated.BetaI(0.5,3,4));
			Assertion.AssertEquals(0.016666666666666666666666666,GammaRelated.BetaI(1,3,4));
		}


	}
}
