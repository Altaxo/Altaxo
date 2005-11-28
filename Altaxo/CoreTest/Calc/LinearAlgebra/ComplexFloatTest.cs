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
using NUnit.Framework;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class ComplexFloatTest 
  {
    private const float TOLERENCE = 0.001f;   
    [Test]
    public void EqualsTest()
    {
      ComplexFloat cf1 = new ComplexFloat(-1.1f, 2.2f);
      ComplexFloat cf2 = new ComplexFloat(-1.1f, 2.2f);
      Assert.IsTrue(cf1 == cf2);
      Assert.IsTrue(cf1.Equals(cf2));
    }

    [Test]
    public void ConversionTest()
    {
      ComplexFloat cf1 = 2.2f;
      Assert.AreEqual(cf1.Real,2.2);
      Assert.AreEqual(cf1.Imag,0);
    }   

    [Test]
    public void OperatorsTest()
    {
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(-3.3f, 4.4f);
      ComplexFloat test = cf1 * cf2;
      Assert.AreEqual(test.Real,6.05);
      Assert.AreEqual(test.Imag,12.1);
      
      test = cf1 / cf2;
      Assert.AreEqual(test.Real,-0.44);
      Assert.AreEqual(test.Imag,0.08,TOLERENCE);

      test = cf1 + cf2;
      Assert.AreEqual(test.Real,-2.2);
      Assert.AreEqual(test.Imag,2.2);
    
      test = cf1 - cf2;
      Assert.AreEqual(test.Real,4.4);
      Assert.AreEqual(test.Imag,-6.6);

      //test = cf1 ^ cf2;
      //Assert.AreEqual(test.Real,1.593,TOLERENCE);
      //Assert.AreEqual(test.Imag,6.503,TOLERENCE);

    }

    [Test]
    public void NaNTest()
    {
      ComplexFloat cf = new ComplexFloat(Single.NaN, 1.1f);
      Assert.IsTrue(cf.IsNaN());
      cf = new ComplexFloat(1.1f, Single.NaN);
      Assert.IsTrue(cf.IsNaN());
      cf = new ComplexFloat(1.1f,2.2f);
      Assert.IsFalse(cf.IsNaN());
    }

    [Test]
    public void InfinityTest()
    {
      ComplexFloat cf = new ComplexFloat(Single.NegativeInfinity, 1.1f);
      Assert.IsTrue(cf.IsInfinity());
      cf = new ComplexFloat(1.1f, Single.NegativeInfinity);
      Assert.IsTrue(cf.IsInfinity());
      cf = new ComplexFloat(Single.PositiveInfinity, 1.1f);
      Assert.IsTrue(cf.IsInfinity());
      cf = new ComplexFloat(1.1f, Single.PositiveInfinity);
      Assert.IsTrue(cf.IsInfinity());
      cf = new ComplexFloat(1.1f,2.2f);
      Assert.IsFalse(cf.IsInfinity());
    }

    [Test]
    public void CloneTest()
    {
      ComplexFloat cf1 = new ComplexFloat(1.1f, 2.2f);
      ComplexFloat cf2 = (ComplexFloat)((ICloneable)cf1).Clone();
      Assert.AreEqual(cf1, cf2);
    }

    [Test]
    public void HashTest()
    {
      ComplexFloat cd1 = new ComplexFloat(1.1f, 2.2f);
      ComplexFloat cd2 = new ComplexFloat(1.1f, 3.3f);
      ComplexFloat cd3 = new ComplexFloat(0.1f, 2.2f);
      Assert.AreNotEqual(cd1.GetHashCode(), cd2.GetHashCode());
      Assert.AreNotEqual(cd1.GetHashCode(), cd3.GetHashCode());
      Assert.AreNotEqual(cd2.GetHashCode(), cd3.GetHashCode());
    }
    
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullString()
    {
      string s = null;
      ComplexFloat cf = new ComplexFloat(s);
    }

    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest1()
    {
      string s = "";
      ComplexFloat cf = new ComplexFloat(s);
    }

    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest2()
    {
      string s = "+";
      ComplexFloat cf = new ComplexFloat(s);
    }
  
    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest3()
    {
      string s = "1i+2";
      ComplexFloat cf = new ComplexFloat(s);
    }

    [Test]
    public void ParseTest()
    {
      string s = "1";
      ComplexFloat cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 0);

      s = "i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 0);
      Assert.AreEqual(cf.Imag, 1);

      s = "2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 0);
      Assert.AreEqual(cf.Imag, 2);

      s = "1 + 2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);

      s = "1+2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);

      s = "1 - 2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, -2);

      s = "1-2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, -2);

      s = "1+-2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, -2);
      
      s = "1 - 2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, -2);

      s = "1,2";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);

      s = "1 , 2 ";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);
      
      s = "1,2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);

      s = "-1, -2i";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, -1);
      Assert.AreEqual(cf.Imag, -2);

      s = "(+1,2i)";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 1);
      Assert.AreEqual(cf.Imag, 2);

      s = "(-1 , -2)";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, -1);
      Assert.AreEqual(cf.Imag, -2);

      s = "(-1 , -2i)";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, -1);
      Assert.AreEqual(cf.Imag, -2);

    
      s = "(+1e1 , -2e-2i)";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, 10);
      Assert.AreEqual(cf.Imag, -.02);   
    
      s = "(-1e1 + -2e2i)";
      cf = new ComplexFloat(s);
      Assert.AreEqual(cf.Real, -10);
      Assert.AreEqual(cf.Imag, -200);   

    }
  }
}

