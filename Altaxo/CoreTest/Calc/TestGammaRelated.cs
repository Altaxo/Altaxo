#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc;
using NUnit.Framework;


namespace AltaxoTest.Calc
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

    [Test]
    public void TestBetaIR()
    {
      // N[BetaRegularized[1/2, 3, 6], 25]
      Assertion.AssertEquals(0.85546875,GammaRelated.BetaIR(0.5,3,6));
    }

    [Test]
    public void TestInverseBetaIR()
    {
      // N[BetaRegularized[1/2, 3, 6], 25]
      Assertion.AssertEquals(0.5,GammaRelated.InverseBeta(0.85546875,3,6));
    }

  }
}
