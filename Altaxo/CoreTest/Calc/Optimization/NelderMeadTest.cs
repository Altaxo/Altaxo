﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Xunit;

namespace AltaxoTest.Calc.Optimization
{
  public sealed class Poly : CostFunction
  {
    public override double Value(Vector<double> x)
    {
      return System.Math.Pow(x[0], 2);
    }
  }


  public class NelderMeadTest
  {
    //Test Create Simplex
    [Fact]
    public void TestInitializeMethod()
    {
      var cf = new Rosenbrock();
      var optim = new NelderMead(cf);
      var x0 = CreateVector.DenseOfArray(new double[4] { 0, 1, 2, 3 });

      optim.SimplexDelta = 0.1;
      optim.SimplexZeroDelta = 0.0001;

      optim.InitializeMethod(x0);

      Assert.Equal(5, optim.Simplex.Length);
      for (int i = 0; i < optim.Simplex.Length; i++)
      {
        AssertEx.Equal(optim.Simplex[i][0], x0[0], optim.SimplexZeroDelta);
        AssertEx.Equal(optim.Simplex[i][1], x0[1], optim.SimplexDelta * x0[1] + 0.001);
        AssertEx.Equal(optim.Simplex[i][2], x0[2], optim.SimplexDelta * x0[2] + 0.001);
        AssertEx.Equal(optim.Simplex[i][3], x0[3], optim.SimplexDelta * x0[3] + 0.001);
      }
      for (int i = 1; i < optim.Simplex.Length; i++)
      {
        Assert.True(cf.Value(optim.Simplex[i - 1]) < cf.Value(optim.Simplex[i]));
      }
    }

    //Test Reflection
    [Fact]
    public void TestReflection()
    {
      var cf = new Poly();
      var optim = new NelderMead(cf);

      var simplex = new Vector<double>[3];
      simplex[0] = CreateVector.DenseOfArray(new double[2] { 1, 1 });
      simplex[1] = CreateVector.DenseOfArray(new double[2] { 1, -1 });
      simplex[2] = CreateVector.DenseOfArray(new double[2] { 2, 0 });

      optim.Rho = 1.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();

      var xr = (1 + optim.Rho) * (CreateVector.DenseOfArray(new double[2] { 1, 0 })) - optim.Rho * simplex[2];

      Assert.True(optim.LastStep == NelderMead.Step.Reflection);
      Assert.Equal(optim.Simplex[0][0], xr[0]);
      Assert.Equal(optim.Simplex[0][1], xr[1]);
    }

    //Test Expansion
    [Fact]
    public void TestExpansion()
    {
      var cf = new Poly();
      var optim = new NelderMead(cf);

      var simplex = new Vector<double>[3];
      simplex[0] = CreateVector.DenseOfArray(new double[2] { 1, 1 });
      simplex[1] = CreateVector.DenseOfArray(new double[2] { 1, -1 });
      simplex[2] = CreateVector.DenseOfArray(new double[2] { 2, 0 });

      optim.InitializeMethod(simplex);
      optim.Rho = 1.5;
      optim.Chi = 1 / 1.5;
      optim.IterateMethod();

      var xr = (1 + optim.Rho * optim.Chi) * (CreateVector.DenseOfArray(new double[2] { 1, 0 })) - optim.Rho * optim.Chi * simplex[2];

      Assert.True(optim.LastStep == NelderMead.Step.Expansion);
      Assert.Equal(optim.Simplex[0][0], xr[0]);
      Assert.Equal(optim.Simplex[0][1], xr[1]);
    }

    //Test Outside Contraction
    [Fact]
    public void TestOutsideContraction()
    {
      var cf = new Poly();
      var optim = new NelderMead(cf);

      var simplex = new Vector<double>[3];
      simplex[0] = CreateVector.DenseOfArray(new double[2] { 1, 1 });
      simplex[1] = CreateVector.DenseOfArray(new double[2] { 1, -1 });
      simplex[2] = CreateVector.DenseOfArray(new double[2] { 2, 0 });

      optim.Rho = 2.25;
      optim.Psi = 1 / 2.25;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();

      var xr = (1 + optim.Rho * optim.Psi) * (CreateVector.DenseOfArray(new double[2] { 1, 0 })) - optim.Rho * optim.Psi * simplex[2];

      Assert.True(optim.LastStep == NelderMead.Step.OutsideContraction);
      Assert.Equal(optim.Simplex[0][0], xr[0]);
      Assert.Equal(optim.Simplex[0][1], xr[1]);
    }

    //Test Inside Contraction
    [Fact]
    public void TestInsideContraction()
    {
      var cf = new Poly();
      var optim = new NelderMead(cf);

      var simplex = new Vector<double>[3];
      simplex[0] = CreateVector.DenseOfArray(new double[2] { 1, 1 });
      simplex[1] = CreateVector.DenseOfArray(new double[2] { 1, -1 });
      simplex[2] = CreateVector.DenseOfArray(new double[2] { 2, 0 });

      optim.Rho = 10;
      optim.Psi = 0.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();

      var xr = (1 - optim.Psi) * (CreateVector.DenseOfArray(new double[2] { 1, 0 })) + optim.Psi * simplex[2];

      Assert.True(optim.LastStep == NelderMead.Step.InsideContraction);
      Assert.Equal(optim.Simplex[2][0], xr[0]);
      Assert.Equal(optim.Simplex[2][1], xr[1]);
    }

    //Test Shrink
    [Fact]
    public void TestShrink()
    {
      var cf = new Poly();
      var optim = new NelderMead(cf);

      var simplex = new Vector<double>[3];
      simplex[0] = CreateVector.DenseOfArray(new double[2] { 1, 1 });
      simplex[1] = CreateVector.DenseOfArray(new double[2] { 1, -1 });
      simplex[2] = CreateVector.DenseOfArray(new double[2] { 2, 0 });

      optim.Rho = 10;
      optim.Psi = 1.5;
      optim.InitializeMethod(simplex);
      optim.IterateMethod();

      Assert.True(optim.LastStep == NelderMead.Step.Shrink);
    }

    //Test Rosenbrock
    [Fact]
    public void TestRosenbrock()
    {
      var cf = new Rosenbrock();
      var optim = new NelderMead(cf);

      var x0 = CreateVector.DenseOfArray(new double[5] { 1.3, 0.7, 0.8, 1.9, 1.2 });

      optim.Minimize(x0);

      AssertEx.Equal(optim.SolutionValue, 0.0, 0.0001);
      AssertEx.Equal(optim.SolutionVector[0], 1.0, 0.0001);
      AssertEx.Equal(optim.SolutionVector[1], 1.0, 0.0001);
      AssertEx.Equal(optim.SolutionVector[2], 1.0, 0.0001);
      AssertEx.Equal(optim.SolutionVector[3], 1.0, 0.0001);
      AssertEx.Equal(optim.SolutionVector[4], 1.0, 0.0001);
    }
  }
}
