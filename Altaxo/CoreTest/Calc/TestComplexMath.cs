#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
  public class TestComplexMath
  {
    [Test]
    public void TestExp()
    {
      Complex result;
      
      result = ComplexMath.Exp(new Complex(0.5,0.5));
      Assert.AreEqual(1.446889036584169158051583, result.Re,1e-15);
      Assert.AreEqual(0.7904390832136149118432626, result.Im,1e-15);
    }

    [Test]
    public void TestLog()
    {
      Complex arg;
      Complex result;
      
      arg = new Complex(1/2.0, 1/3.0);

      
      Assert.AreEqual(0.6009252125773315488532035, arg.GetModulus(), 1e-15);

      result = ComplexMath.Log(arg);
      Assert.AreEqual(-0.5092847904972866327857336, result.Re, 1e-15);
      Assert.AreEqual(0.5880026035475675512456111 , result.Im, 1e-15);
    }

  
  }
}
