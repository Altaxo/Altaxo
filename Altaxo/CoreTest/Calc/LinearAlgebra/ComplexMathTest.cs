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
  public class ComplexMathTest 
  {
    private const float TOLERENCE = 0.001f;   
      
    [Test]
    public void Absolute()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f);
      Assert.AreEqual(ComplexMath.Absolute(cd1),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cd2),2.2,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cd3),1.1,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cd4),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cf1),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cf2),2.2,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cf3),1.1,TOLERENCE);
      Assert.AreEqual(ComplexMath.Absolute(cf4),2.460,TOLERENCE);
    }

    [Test]
    public void Argument()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f);
      Assert.AreEqual(ComplexMath.Argument(cd1),-1.107,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cd2),-1.571,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cd3),0,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cd4),-1.107,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cf1),-1.107,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cf2),-1.571,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cf3),0,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument(cf4),-1.107,TOLERENCE);
    }
    
    [Test]
    public void Argument2()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f);
      Assert.AreEqual(ComplexMath.Argument2(cd1),-1.107,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cd2),-1.571,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cd3),0,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cd4),2.034,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cf1),-1.107,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cf2),-1.571,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cf3),0,TOLERENCE);
      Assert.AreEqual(ComplexMath.Argument2(cf4),2.034,TOLERENCE);
    }

    [Test]
    public void Conjugate()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f);
      Assert.AreEqual(ComplexMath.Conjugate(cd1),new Complex(1.1,2.2));
      Assert.AreEqual(ComplexMath.Conjugate(cd2),new Complex(0,2.2));
      Assert.AreEqual(ComplexMath.Conjugate(cd3),new Complex(1.1,0));
      Assert.AreEqual(ComplexMath.Conjugate(cd4),new Complex(-1.1,-2.2));
      Assert.AreEqual(ComplexMath.Conjugate(cf1),new ComplexFloat(1.1f,2.2f));
      Assert.AreEqual(ComplexMath.Conjugate(cf2),new ComplexFloat(0,2.2f));
      Assert.AreEqual(ComplexMath.Conjugate(cf3),new ComplexFloat(1.1f,0));
      Assert.AreEqual(ComplexMath.Conjugate(cf4),new ComplexFloat(-1.1f,-2.2f));
    }

    [Test]
    public void Cos()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Cos(cd1);
      Assert.AreEqual(cdt.Real, 2.072, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 3.972, TOLERENCE);

      cdt = ComplexMath.Cos(cd2);
      Assert.AreEqual(cdt.Real, 4.568, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    
    
      cdt = ComplexMath.Cos(cd3);
      Assert.AreEqual(cdt.Real, 0.454, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Cos(cd4);
      Assert.AreEqual(cdt.Real, 2.072, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 3.972, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Cos(cf1);
      Assert.AreEqual(cft.Real, 2.072, TOLERENCE);
      Assert.AreEqual(cft.Imag, 3.972, TOLERENCE);

      cft = ComplexMath.Cos(cf2);
      Assert.AreEqual(cft.Real, 4.568, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    
    
      cft = ComplexMath.Cos(cf3);
      Assert.AreEqual(cft.Real, 0.454, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Cos(cf4);
      Assert.AreEqual(cft.Real, 2.072, TOLERENCE);
      Assert.AreEqual(cft.Imag, 3.972, TOLERENCE);    
    }

    [Test]
    public void Cosh()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Cosh(cd1);
      Assert.AreEqual(cdt.Real, -.982, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.08, TOLERENCE);

      cdt = ComplexMath.Cosh(cd2);
      Assert.AreEqual(cdt.Real, -0.589, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    
    
      cdt = ComplexMath.Cosh(cd3);
      Assert.AreEqual(cdt.Real, 1.669, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Cosh(cd4);
      Assert.AreEqual(cdt.Real, -.982, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.08, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Cosh(cf1);
      Assert.AreEqual(cft.Real, -.982, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.08, TOLERENCE);

      cft = ComplexMath.Cosh(cf2);
      Assert.AreEqual(cft.Real, -0.589, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    
    
      cft = ComplexMath.Cosh(cf3);
      Assert.AreEqual(cft.Real, 1.669, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Cosh(cf4);
      Assert.AreEqual(cft.Real, -.982, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.08, TOLERENCE);
    }

    [Test]
    public void Exp()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Exp(cd1);
      Assert.AreEqual(cdt.Real, -1.768, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -2.429, TOLERENCE);

      cdt = ComplexMath.Exp(cd2);
      Assert.AreEqual(cdt.Real,-0.589, TOLERENCE);
      Assert.AreEqual(cdt.Imag,-0.808, TOLERENCE);    
    
      cdt = ComplexMath.Exp(cd3);
      Assert.AreEqual(cdt.Real, 3.004, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Exp(cd4);
      Assert.AreEqual(cdt.Real, -0.196, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0.269, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Exp(cf1);
      Assert.AreEqual(cft.Real, -1.768, TOLERENCE);
      Assert.AreEqual(cft.Imag, -2.429, TOLERENCE);

      cft = ComplexMath.Exp(cf2);
      Assert.AreEqual(cft.Real, -0.589, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.808, TOLERENCE);   
    
      cft = ComplexMath.Exp(cf3);
      Assert.AreEqual(cft.Real, 3.004, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Exp(cf4);
      Assert.AreEqual(cft.Real, -0.196, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0.269, TOLERENCE);
    }

    [Test]
    public void Log()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Log(cd1);
      Assert.AreEqual(cdt.Real, 0.900, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.107, TOLERENCE);

      cdt = ComplexMath.Log(cd2);
      Assert.AreEqual(cdt.Real, 0.788, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.571, TOLERENCE);   
    
      cdt = ComplexMath.Log(cd3);
      Assert.AreEqual(cdt.Real, 0.0953, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Log(cd4);
      Assert.AreEqual(cdt.Real, 0.900, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 2.034, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Log(cf1);
      Assert.AreEqual(cft.Real, 0.900, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.107, TOLERENCE);

      cft = ComplexMath.Log(cf2);
      Assert.AreEqual(cft.Real, 0.788, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.571, TOLERENCE);   
    
      cft = ComplexMath.Log(cf3);
      Assert.AreEqual(cft.Real, 0.095, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Log(cf4);
      Assert.AreEqual(cft.Real, 0.900, TOLERENCE);
      Assert.AreEqual(cft.Imag, 2.034, TOLERENCE);
    }

    [Test]
    public void Max()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);

      Complex cdt = ComplexMath.Max(cd1,cd2);
      Assert.AreEqual(cd1, cdt);

      ComplexFloat cft = ComplexMath.Max(cf1,cf2);
      Assert.AreEqual(cf1, cft);
    }

    [Test]
    public void Norm()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f);
      Assert.AreEqual(ComplexMath.Norm(cd1),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cd2),2.2,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cd3),1.1,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cd4),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cf1),2.460,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cf2),2.2,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cf3),1.1,TOLERENCE);
      Assert.AreEqual(ComplexMath.Norm(cf4),2.460,TOLERENCE);
    }

    [Test]
    public void Polar()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Polar(cd1);
      Assert.AreEqual(cdt.Real, 2.460, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.107, TOLERENCE);

      cdt = ComplexMath.Polar(cd2);
      Assert.AreEqual(cdt.Real, 2.2, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.571, TOLERENCE);   
    
      cdt = ComplexMath.Polar(cd3);
      Assert.AreEqual(cdt.Real, 1.1, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Polar(cd4);
      Assert.AreEqual(cdt.Real, 2.460, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 2.034, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Polar(cf1);
      Assert.AreEqual(cft.Real, 2.460, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.107, TOLERENCE);

      cft = ComplexMath.Polar(cf2);
      Assert.AreEqual(cft.Real, 2.2, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.571, TOLERENCE);   
    
      cft = ComplexMath.Polar(cf3);
      Assert.AreEqual(cft.Real, 1.1, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Polar(cf4);
      Assert.AreEqual(cft.Real, 2.460, TOLERENCE);
      Assert.AreEqual(cft.Imag, 2.034, TOLERENCE);
    }


    [Test]
    public void Sin()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Sin(cd1);
      Assert.AreEqual(cdt.Real, 4.071, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -2.022, TOLERENCE);

      cdt = ComplexMath.Sin(cd2);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -4.457, TOLERENCE);   
    
      cdt = ComplexMath.Sin(cd3);
      Assert.AreEqual(cdt.Real, 0.891, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Sin(cd4);
      Assert.AreEqual(cdt.Real, -4.071, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 2.022, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Sin(cf1);
      Assert.AreEqual(cft.Real, 4.071, TOLERENCE);
      Assert.AreEqual(cft.Imag, -2.022, TOLERENCE);

      cft = ComplexMath.Sin(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, -4.457, TOLERENCE);   
    
      cft = ComplexMath.Sin(cf3);
      Assert.AreEqual(cft.Real, 0.891, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Sin(cf4);
      Assert.AreEqual(cft.Real, -4.071, TOLERENCE);
      Assert.AreEqual(cft.Imag, 2.022, TOLERENCE);
    }

    [Test]
    public void Sinh()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Sinh(cd1);
      Assert.AreEqual(cdt.Real, -0.786, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.349, TOLERENCE);

      cdt = ComplexMath.Sinh(cd2);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.808, TOLERENCE);   
    
      cdt = ComplexMath.Sinh(cd3);
      Assert.AreEqual(cdt.Real, 1.336, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Sinh(cd4);
      Assert.AreEqual(cdt.Real, 0.786, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.349, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Sinh(cf1);
      Assert.AreEqual(cft.Real, -0.786, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.349, TOLERENCE);

      cft = ComplexMath.Sinh(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.808, TOLERENCE);   
    
      cft = ComplexMath.Sinh(cf3);
      Assert.AreEqual(cft.Real, 1.336, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Sinh(cf4);
      Assert.AreEqual(cft.Real, 0.786, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.349, TOLERENCE);
    }

    [Test]
    public void Sqrt()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Sqrt(cd1);
      Assert.AreEqual(cdt.Real, 1.334, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.824, TOLERENCE);

      cdt = ComplexMath.Sqrt(cd2);
      Assert.AreEqual(cdt.Real, 1.049, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.049, TOLERENCE);   
    
      cdt = ComplexMath.Sqrt(cd3);
      Assert.AreEqual(cdt.Real, 1.049, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Sqrt(cd4);
      Assert.AreEqual(cdt.Real, 0.824, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.334, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Sqrt(cf1);
      Assert.AreEqual(cft.Real, 1.334, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.824, TOLERENCE);

      cft = ComplexMath.Sqrt(cf2);
      Assert.AreEqual(cft.Real, 1.0489, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.049, TOLERENCE);   
    
      cft = ComplexMath.Sqrt(cf3);
      Assert.AreEqual(cft.Real, 1.049, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Sqrt(cf4);
      Assert.AreEqual(cft.Real, 0.824, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.334, TOLERENCE);
    }

    [Test]
    public void Tan()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Tan(cd1);
      Assert.AreEqual(cdt.Real, 0.020, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.014, TOLERENCE);

      cdt = ComplexMath.Tan(cd2);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.976, TOLERENCE);   
    
      cdt = ComplexMath.Tan(cd3);
      Assert.AreEqual(cdt.Real, 1.965, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Tan(cd4);
      Assert.AreEqual(cdt.Real, -0.020, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.014, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Tan(cf1);
      Assert.AreEqual(cft.Real, 0.020, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.014, TOLERENCE);

      cft = ComplexMath.Tan(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.976, TOLERENCE);   
    
      cft = ComplexMath.Tan(cf3);
      Assert.AreEqual(cft.Real, 1.965, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Tan(cf4);
      Assert.AreEqual(cft.Real, -0.020, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.014, TOLERENCE);
    }

    [Test]
    public void Tanh()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Tanh(cd1);
      Assert.AreEqual(cdt.Real, 1.046, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0.223, TOLERENCE);

      cdt = ComplexMath.Tanh(cd2);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.374, TOLERENCE);    
    
      cdt = ComplexMath.Tanh(cd3);
      Assert.AreEqual(cdt.Real, 0.800, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Tanh(cd4);
      Assert.AreEqual(cdt.Real, -1.046, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.223, TOLERENCE);   

      ComplexFloat cft = ComplexMath.Tanh(cf1);
      Assert.AreEqual(cft.Real, 1.046, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0.223, TOLERENCE);

      cft = ComplexMath.Tanh(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.374, TOLERENCE);    
    
      cft = ComplexMath.Tanh(cf3);
      Assert.AreEqual(cft.Real, 0.800, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Tanh(cf4);
      Assert.AreEqual(cft.Real, -1.046, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.223, TOLERENCE);
    }

    [Test]
    public void Asin()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Asin(cd1);
      Assert.AreEqual(cdt.Real, 0.433, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.618, TOLERENCE);

      cdt = ComplexMath.Asin(cd2);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.530, TOLERENCE);   
    
      cdt = ComplexMath.Asin(cd3);
      Assert.AreEqual(cdt.Real, 1.571, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.444, TOLERENCE);   

      cdt = ComplexMath.Asin(cd4);
      Assert.AreEqual(cdt.Real, -0.433, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.618, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Asin(cf1);
      Assert.AreEqual(cft.Real, 0.433, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.618, TOLERENCE);

      cft = ComplexMath.Asin(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.530, TOLERENCE);   
    
      cft = ComplexMath.Asin(cf3);
      Assert.AreEqual(cft.Real, 1.571, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.444, TOLERENCE);   

      cft = ComplexMath.Asin(cf4);
      Assert.AreEqual(cft.Real, -0.433, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.618, TOLERENCE);
    }
  
    [Test]
    public void Acos()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Acos(cd1);
      Assert.AreEqual(cdt.Real, 1.1388414556, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.618, TOLERENCE);

      cdt = ComplexMath.Acos(cd2);
      Assert.AreEqual(cdt.Real, 1.571, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.530, TOLERENCE);    
    
      cdt = ComplexMath.Acos(cd3);
      Assert.AreEqual(cdt.Real, 0, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0.444, TOLERENCE);    

      cdt = ComplexMath.Acos(cd4);
      Assert.AreEqual(cdt.Real, 2.004, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.618, TOLERENCE);   

      ComplexFloat cft = ComplexMath.Acos(cf1);
      Assert.AreEqual(cft.Real, 1.138, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.618, TOLERENCE);

      cft = ComplexMath.Acos(cf2);
      Assert.AreEqual(cft.Real, 1.571, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.530, TOLERENCE);    
    
      cft = ComplexMath.Acos(cf3);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0.444, TOLERENCE);    

      cft = ComplexMath.Acos(cf4);
      Assert.AreEqual(cft.Real, 2.004, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.618, TOLERENCE);
    }
    
    [Test]
    public void Atan()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Atan(cd1);
      Assert.AreEqual(cdt.Real, 1.365, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.366, TOLERENCE);

      cdt = ComplexMath.Atan(cd2);
      Assert.AreEqual(cdt.Real, -1.5708, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -0.490415, TOLERENCE);    
    
      cdt = ComplexMath.Atan(cd3);
      Assert.AreEqual(cdt.Real, 0.833, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Atan(cd4);
      Assert.AreEqual(cdt.Real, -1.365, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0.366, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Atan(cf1);
      Assert.AreEqual(cft.Real, 1.365, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.366, TOLERENCE);

      cft = ComplexMath.Atan(cf2);
      Assert.AreEqual(cft.Real, -1.571, TOLERENCE);
      Assert.AreEqual(cft.Imag, -0.490, TOLERENCE);   
    
      cft = ComplexMath.Atan(cf3);
      Assert.AreEqual(cft.Real, 0.833, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Atan(cf4);
      Assert.AreEqual(cft.Real, -1.365, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0.366, TOLERENCE);
    }

    [Test]
    public void Asinh()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Asinh(cd1);
      Assert.AreEqual(cdt.Real, 1.569, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.072, TOLERENCE);

      cdt = ComplexMath.Asinh(cd2);
      Assert.AreEqual(cdt.Real, -1.425, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.571, TOLERENCE);   
    
      cdt = ComplexMath.Asinh(cd3);
      Assert.AreEqual(cdt.Real, 0.950, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Asinh(cd4);
      Assert.AreEqual(cdt.Real, -1.569, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.0716, TOLERENCE);   

      ComplexFloat cft = ComplexMath.Asinh(cf1);
      Assert.AreEqual(cft.Real, 1.569, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.072, TOLERENCE);

      cft = ComplexMath.Asinh(cf2);
      Assert.AreEqual(cft.Real, -1.425, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.571, TOLERENCE);   
    
      cft = ComplexMath.Asinh(cf3);
      Assert.AreEqual(cft.Real, 0.950, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Asinh(cf4);
      Assert.AreEqual(cft.Real, -1.569, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.0716, TOLERENCE);
    }
    
    [Test]
    public void Acosh()
    {
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Acosh(cd1);
      Assert.AreEqual(cdt.Real, 1.618, TOLERENCE);
      Assert.AreEqual(cdt.Imag,  -1.138, TOLERENCE);

      cdt = ComplexMath.Acosh(cd2);
      Assert.AreEqual(cdt.Real, 1.530, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.571, TOLERENCE);   
    
      cdt = ComplexMath.Acosh(cd3);
      Assert.AreEqual(cdt.Real, 0.444, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 0, TOLERENCE);    

      cdt = ComplexMath.Acosh(cd4);
      Assert.AreEqual(cdt.Real, 1.618, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 2.004, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Acosh(cf1);
      Assert.AreEqual(cft.Real, 1.618, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.138, TOLERENCE);

      cft = ComplexMath.Acosh(cf2);
      Assert.AreEqual(cft.Real, 1.530, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.571, TOLERENCE);   
    
      cft = ComplexMath.Acosh(cf3);
      Assert.AreEqual(cft.Real, 0.444, TOLERENCE);
      Assert.AreEqual(cft.Imag, 0, TOLERENCE);    

      cft = ComplexMath.Acosh(cf4);
      Assert.AreEqual(cft.Real, 1.618, TOLERENCE);
      Assert.AreEqual(cft.Imag, 2.004, TOLERENCE);
    }
    
    [Test]
    public void Atanh()
    {      
      Complex cd1 = new Complex(1.1, -2.2);
      Complex cd2 = new Complex(0, -2.2);
      Complex cd3 = new Complex(1.1, 0);
      Complex cd4 = new Complex(-1.1, 2.2);
      ComplexFloat cf1 = new ComplexFloat(1.1f, -2.2f);
      ComplexFloat cf2 = new ComplexFloat(0, -2.2f);
      ComplexFloat cf3 = new ComplexFloat(1.1f, 0);
      ComplexFloat cf4 = new ComplexFloat(-1.1f, 2.2f); 
      
      Complex cdt = ComplexMath.Atanh(cd1);
      Assert.AreEqual(cdt.Real, 0.161, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.212, TOLERENCE);

      cdt = ComplexMath.Atanh(cd2);
      Assert.AreEqual(cdt.Real, 0 , TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.144, TOLERENCE);   
    
      cdt = ComplexMath.Atanh(cd3);
      Assert.AreEqual(cdt.Real, 1.522, TOLERENCE);
      Assert.AreEqual(cdt.Imag, -1.571, TOLERENCE);   

      cdt = ComplexMath.Atanh(cd4);
      Assert.AreEqual(cdt.Real, -0.161, TOLERENCE);
      Assert.AreEqual(cdt.Imag, 1.212, TOLERENCE);    

      ComplexFloat cft = ComplexMath.Atanh(cf1);
      Assert.AreEqual(cft.Real, 0.161, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.212, TOLERENCE);

      cft = ComplexMath.Atanh(cf2);
      Assert.AreEqual(cft.Real, 0, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.144, TOLERENCE);   
    
      cft = ComplexMath.Atanh(cf3);
      Assert.AreEqual(cft.Real, 1.522, TOLERENCE);
      Assert.AreEqual(cft.Imag, -1.571, TOLERENCE);   

      cft = ComplexMath.Atanh(cf4);
      Assert.AreEqual(cft.Real, -0.161, TOLERENCE);
      Assert.AreEqual(cft.Imag, 1.212, TOLERENCE);
    }
  }
}

