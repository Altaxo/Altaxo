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
using NUnit.Framework;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra 
{
  [TestFixture]
  public class ComplexDoubleTest 
  {
    private const double TOLERENCE = 0.001;   
    private const double DBL_EPSILON = DoubleConstants.DBL_EPSILON;
    [Test]
    public void EqualsTest()
    {
      Complex cd1 = new Complex(-1.1, 2.2);
      Complex cd2 = new Complex(-1.1, 2.2);
      Complex cd3 = new Complex(-1, 2);
      ComplexFloat cf = new ComplexFloat(-1, 2);
      Assert.IsTrue(cd1 == cd2);
      Assert.IsTrue(cd1.Equals(cd2));
      Assert.IsTrue(cd3 == cf);
      Assert.IsTrue(cd3.Equals(cf));
    }

    [Test]
    public void ConversionTest()
    {
      Complex cd1 = 2.2;
      ComplexFloat cf = new ComplexFloat(-1.1f, 2.2f);
      Complex cd2 = cf;
      Assert.AreEqual(cd1.Real,2.2);
      Assert.AreEqual(cd1.Imag,0);
      Assert.AreEqual(cd2.Real,-1.1,TOLERENCE);
      Assert.AreEqual(cd2.Imag,2.2,TOLERENCE);
    }   

    [Test]
    public void OperatorsTest()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(-3.3, 4.4);
      Complex test = cd1 * cd2;
      Assert.AreEqual(test.Real,6.05, 20*DBL_EPSILON);
      Assert.AreEqual(test.Imag,12.1, 25*DBL_EPSILON);
      
      test = cd1 / cd2;
      Assert.AreEqual(test.Real,-0.44, 2*DBL_EPSILON);
      Assert.AreEqual(test.Imag,0.08, 2*DBL_EPSILON);

      test = cd1 + cd2;
      Assert.AreEqual(test.Real,-2.2, 10*DBL_EPSILON);
      Assert.AreEqual(test.Imag,2.2, 10*DBL_EPSILON);
    
      test = cd1 - cd2;
      Assert.AreEqual(test.Real,4.4, 10*DBL_EPSILON);
      Assert.AreEqual(test.Imag,-6.6, 10*DBL_EPSILON);

      //test = cd1 ^ cd2;
      //Assert.AreEqual(test.Real,1.593,TOLERENCE);
      //Assert.AreEqual(test.Imag,6.503,TOLERENCE);

    }
    
    [Test]
    public void NaNTest()
    {
      Complex cd = new Complex(Double.NaN, 1.1);
      Assert.IsTrue(cd.IsNaN());
      cd = new Complex(1.1, Double.NaN);
      Assert.IsTrue(cd.IsNaN());
      cd = new Complex(1.1,2.2);
      Assert.IsFalse(cd.IsNaN());
    }

    [Test]
    public void InfinityTest()
    {
      Complex cd = new Complex(Double.NegativeInfinity, 1.1);
      Assert.IsTrue(cd.IsInfinity());
      cd = new Complex(1.1, Double.NegativeInfinity);
      Assert.IsTrue(cd.IsInfinity());
      cd = new Complex(Double.PositiveInfinity, 1.1);
      Assert.IsTrue(cd.IsInfinity());
      cd = new Complex(1.1, Double.PositiveInfinity);
      Assert.IsTrue(cd.IsInfinity());
      cd = new Complex(1.1,2.2);
      Assert.IsFalse(cd.IsInfinity());
    }

    [Test]
    public void CloneTest()
    {
      Complex cd1 = new Complex(1.1, 2.2);
      Complex cd2 = (Complex)((ICloneable)cd1).Clone();
      Assert.AreEqual(cd1, cd2);
    }

    
    [Test]
    public void HashTest()
    {
      Complex cd1 = new Complex(1.1, 2.2);
      Complex cd2 = new Complex(1.1, 3.3);
      Complex cd3 = new Complex(0.1, 2.2);
      Assert.AreNotEqual(cd1.GetHashCode(), cd2.GetHashCode());
      Assert.AreNotEqual(cd1.GetHashCode(), cd3.GetHashCode());
      Assert.AreNotEqual(cd2.GetHashCode(), cd3.GetHashCode());
    }
    

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullString()
    {
      string s = null;
      Complex cd = new Complex(s);
    }

    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest1()
    {
      string s = "";
      Complex cd = new Complex(s);
    }

    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest2()
    {
      string s = "+";
      Complex cd = new Complex(s);
    }
  
    [Test]
    [ExpectedException(typeof(FormatException))]
    public void FormatExceptionTest3()
    {
      string s = "1i+2";
      Complex cd = new Complex(s);
    }

    [Test]
    public void ParseTest()
    {
      string s = "1";
      Complex cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 0);

      s = "i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 0);
      Assert.AreEqual(cd.Imag, 1);

      s = "2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 0);
      Assert.AreEqual(cd.Imag, 2);

      s = "1 + 2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);

      s = "1+2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);

      s = "1 - 2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, -2);

      s = "1-2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, -2);

      s = "1+-2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, -2);
      
      s = "1 - 2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, -2);

      s = "1,2";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);

      s = "1 , 2 ";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);
      
      s = "1,2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);

      s = "-1, -2i";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, -1);
      Assert.AreEqual(cd.Imag, -2);

      s = "(+1,2i)";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 1);
      Assert.AreEqual(cd.Imag, 2);

      s = "(-1 , -2)";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, -1);
      Assert.AreEqual(cd.Imag, -2);

      s = "(-1 , -2i)";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, -1);
      Assert.AreEqual(cd.Imag, -2);

    
      s = "(+1e1 , -2e-2i)";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, 10);
      Assert.AreEqual(cd.Imag, -.02);   
    
      s = "(-1e1 + -2e2i)";
      cd = new Complex(s);
      Assert.AreEqual(cd.Real, -10);
      Assert.AreEqual(cd.Imag, -200);   

    }
  }
}

