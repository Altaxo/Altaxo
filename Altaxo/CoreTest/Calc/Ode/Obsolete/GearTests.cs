#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class GearTests
  {
    /// <summary>Solves dx/dt = exp(-x) equation with x(0) = 1 initial condition</summary>
    [Fact]
    public void ExponentSolveToGearTest()
    {
      foreach (var sp in Ode.GearBDF(0,
              1,
              (t, x) => -x,
              new Options { RelativeTolerance = 1e-4 }).SolveTo(10))
        Assert.True(Math.Abs(sp.X[0] - Math.Exp(-sp.T)) < 1e-3);
    }

    /// <summary>Solves dx/dt = A*x equation with x(0) = {1,1} initial condition</summary>
    [Fact]
    public void JacobianGearTest()
    {
      var A = new Matrix(new double[,] { { -1, 0.5 }, { 0, -1 } });

      var sol = new List<double>();
      foreach (var sp in Ode.GearBDF(0,
              new Vector(1, 1),
              (t, x) => A * x).SolveFromToStep(0, 10, 0.1))
        sol.Add(sp.X[0]);

      var solJ = new List<double>();
      foreach (var sp in Ode.GearBDF(0,
              new Vector(1, 1),
              (t, x) => A * x).SolveFromToStep(0, 10, 0.1))
        solJ.Add(sp.X[0]);

      for (int i = 0; i < sol.Count; i++)
        Assert.True(Math.Abs(sol[i] - solJ[i]) < 1e-3);
    }

    [Fact]
    public void GearTest2()
    {
      const double lambda1 = -1;
      const double lambda2 = -1000;
      const double lambda1PlusLambda2By2 = (lambda1 + lambda2) / 2;
      const double lambda1MinusLambda2By2 = (lambda1 - lambda2) / 2;

      const double C1 = 1;
      const double C2 = 1;

      var ode = Ode.GearBDF(
      0,
      new Vector(C1 + C2, C1 - C2),
      (t, y) => new Vector(lambda1PlusLambda2By2 * y[0] + lambda1MinusLambda2By2 * y[1], lambda1MinusLambda2By2 * y[0] + lambda1PlusLambda2By2 * y[1]),
      new Options { RelativeTolerance = 1e-4, AbsoluteTolerance = 1E-8 });

      foreach (var sp in ode.SolveTo(100000))
      {
        var y0_expected = C1 * Math.Exp(lambda1 * sp.T) + C2 * Math.Exp(lambda2 * sp.T);
        var y1_expected = C1 * Math.Exp(lambda1 * sp.T) - C2 * Math.Exp(lambda2 * sp.T);

        AssertEx.Equal(y0_expected, sp.X[0], 1E-3 * y0_expected + 1E-4);
        AssertEx.Equal(y1_expected, sp.X[1], 1E-3 * y1_expected + 1E-4);
      }
    }
  }
}
