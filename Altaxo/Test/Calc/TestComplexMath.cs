using System;
using Altaxo.Calc;
using NUnit.Framework;

namespace Altaxo.Test.Calc
{
	[TestFixture]
	public class TestComplexMath
	{
		[Test]
		public void TestExp()
		{
			Complex result;
			
			result = ComplexMath.Exp(new Complex(0.5,0.5));
			Assertion.AssertEquals(1.446889036584169158051583, result.Re,1e-15);
			Assertion.AssertEquals(0.7904390832136149118432626, result.Im,1e-15);
		}

		[Test]
		public void TestLog()
		{
			Complex arg;
			Complex result;
			
			arg = new Complex(1/2.0, 1/3.0);

			
			Assertion.AssertEquals(0.6009252125773315488532035, arg.GetModulus(), 1e-15);

			result = ComplexMath.Log(arg);
			Assertion.AssertEquals(-0.5092847904972866327857336, result.Re, 1e-15);
			Assertion.AssertEquals(0.5880026035475675512456111 , result.Im, 1e-15);
		}

	
	}
}
