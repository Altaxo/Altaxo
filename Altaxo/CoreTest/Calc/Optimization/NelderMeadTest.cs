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
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace AltaxoTest.Calc.Optimization 
{
  public sealed class Poly : CostFunction
  {
    public override double Value (DoubleVector x) 
    {
      return System.Math.Pow(x[0],2);
    }

  } 

  [TestFixture]
  public class NelderMeadTest
  {

    //Test Create Simplex
    [Test]
    public void TestInitializeMethod()
    {
      Rosenbrock cf = new Rosenbrock();
      NelderMead optim = new NelderMead(cf);
      DoubleVector x0 = new DoubleVector(new double[4]{0,1,2,3});
      
      optim.SimplexDelta = 0.1;
      optim.SimplexZeroDelta = 0.0001;
      
      optim.InitializeMethod(x0);
      
      Assert.AreEqual(optim.Simplex.Length,5);
      for (int i=0; i<optim.Simplex.Length; i++) 
      {
        Assert.AreEqual(optim.Simplex[i][0],x0[0],optim.SimplexZeroDelta);
        Assert.AreEqual(optim.Simplex[i][1],x0[1],optim.SimplexDelta*x0[1]+0.001);
        Assert.AreEqual(optim.Simplex[i][2],x0[2],optim.SimplexDelta*x0[2]+0.001);
        Assert.AreEqual(optim.Simplex[i][3],x0[3],optim.SimplexDelta*x0[3]+0.001);
      }
      for (int i=1; i<optim.Simplex.Length; i++) 
      {
        Assert.IsTrue(cf.Value(optim.Simplex[i-1])<cf.Value(optim.Simplex[i]));
      }
    }
    
    //Test Reflection
    [Test]
    public void TestReflection() 
    {
      Poly cf = new Poly();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector[] simplex = new DoubleVector[3];
      simplex[0] = new DoubleVector(new double[2]{1,1});
      simplex[1] = new DoubleVector(new double[2]{1,-1});
      simplex[2] = new DoubleVector(new double[2]{2,0});
      
      optim.Rho = 1.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();
      
      DoubleVector xr = (1+optim.Rho)*(new DoubleVector(new double[2]{1,0})) - optim.Rho*simplex[2];
      
      Assert.IsTrue(optim.LastStep == NelderMead.Step.Reflection);
      Assert.AreEqual(optim.Simplex[0][0],xr[0]);
      Assert.AreEqual(optim.Simplex[0][1],xr[1]);
    }
    
    //Test Expansion
    [Test]
    public void TestExpansion() 
    {
      Poly cf = new Poly();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector[] simplex = new DoubleVector[3];
      simplex[0] = new DoubleVector(new double[2]{1,1});
      simplex[1] = new DoubleVector(new double[2]{1,-1});
      simplex[2] = new DoubleVector(new double[2]{2,0});
      
      
      optim.InitializeMethod(simplex);
      optim.Rho = 1.5;
      optim.Chi = 1/1.5;
      optim.IterateMethod();
      
      DoubleVector xr = (1+optim.Rho*optim.Chi)*(new DoubleVector(new double[2]{1,0})) - optim.Rho*optim.Chi*simplex[2];
      
      Assert.IsTrue(optim.LastStep == NelderMead.Step.Expansion);
      Assert.AreEqual(optim.Simplex[0][0],xr[0]);
      Assert.AreEqual(optim.Simplex[0][1],xr[1]);
    }
    
    //Test Outside Contraction
    [Test]
    public void TestOutsideContraction() 
    {
      Poly cf = new Poly();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector[] simplex = new DoubleVector[3];
      simplex[0] = new DoubleVector(new double[2]{1,1});
      simplex[1] = new DoubleVector(new double[2]{1,-1});
      simplex[2] = new DoubleVector(new double[2]{2,0});
      
      optim.Rho = 2.25;
      optim.Psi = 1/2.25;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();
      
      DoubleVector xr = (1+optim.Rho*optim.Psi)*(new DoubleVector(new double[2]{1,0})) - optim.Rho*optim.Psi*simplex[2];
      
      Assert.IsTrue(optim.LastStep == NelderMead.Step.OutsideContraction);
      Assert.AreEqual(optim.Simplex[0][0],xr[0]);
      Assert.AreEqual(optim.Simplex[0][1],xr[1]);
    }
    
    //Test Inside Contraction
    [Test]
    public void TestInsideContraction() 
    {
      Poly cf = new Poly();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector[] simplex = new DoubleVector[3];
      simplex[0] = new DoubleVector(new double[2]{1,1});
      simplex[1] = new DoubleVector(new double[2]{1,-1});
      simplex[2] = new DoubleVector(new double[2]{2,0});
      
      optim.Rho = 10;
      optim.Psi = 0.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();
      
      DoubleVector xr = (1-optim.Psi)*(new DoubleVector(new double[2]{1,0})) + optim.Psi*simplex[2];

      Assert.IsTrue(optim.LastStep == NelderMead.Step.InsideContraction);
      Assert.AreEqual(optim.Simplex[2][0],xr[0]);
      Assert.AreEqual(optim.Simplex[2][1],xr[1]);
    }
    
    //Test Shrink
    [Test]
    public void TestShrink() 
    {
      Poly cf = new Poly();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector[] simplex = new DoubleVector[3];
      simplex[0] = new DoubleVector(new double[2]{1,1});
      simplex[1] = new DoubleVector(new double[2]{1,-1});
      simplex[2] = new DoubleVector(new double[2]{2,0});
      
      optim.Rho = 10;
      optim.Psi = 1.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();

      Assert.IsTrue(optim.LastStep == NelderMead.Step.Shrink);
    }
    
    //Test Rosenbrock
    [Test]
    public void TestRosenbrock() 
    {
      Rosenbrock cf = new Rosenbrock();
      NelderMead optim = new NelderMead(cf);
      
      DoubleVector x0 = new DoubleVector(new double[5]{1.3,0.7,0.8,1.9,1.2});
      
      optim.Minimize(x0);

      Assert.AreEqual(optim.SolutionValue,0.0,0.0001);
      Assert.AreEqual(optim.SolutionVector[0],1.0,0.0001);
      Assert.AreEqual(optim.SolutionVector[1],1.0,0.0001);
      Assert.AreEqual(optim.SolutionVector[2],1.0,0.0001);
      Assert.AreEqual(optim.SolutionVector[3],1.0,0.0001);
      Assert.AreEqual(optim.SolutionVector[4],1.0,0.0001);
    }
  }
}
