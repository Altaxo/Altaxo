#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
  public class TestKohlrausch
  {

    [Test]
    public void TestIm1()
    {
      double result = Kohlrausch.Im1(1.0+4/32.0, 1);
    }

    [Test]
    public void TestIm2()
    {
      double beta = 63/32.0;
      double result = Kohlrausch.Im2(beta, Math.Exp(2.65625 / beta));
    }

    [Test]
    public void TestIm()
    {
      double beta = 1 / 32.0;
      double result = Kohlrausch.Im(beta, Math.Exp(-5/beta));
    }

    [Test]
    public void TestIm2SmallBeta()
    {
      double beta = 1 / 32.0;
      double result = Kohlrausch.Im2SmallBeta(beta, Math.Exp(-5 / beta));
    }

    [Test]
    public void TestRe1()
    {
      double result = Kohlrausch.Re1(0.5, 1024);
    }

    [Test]
    public void TestRe2()
    {
      double result = Kohlrausch.Re(0.03125, Math.Exp(-5/0.03125));
    }
  }
}