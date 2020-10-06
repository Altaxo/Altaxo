#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Linq;
using System.Net;
using Xunit;

namespace Altaxo.Calc.Ode
{
  
  public class RK547MTests
  {
    /// <summary>Solves dx/dt = exp(-x) equation with x(0) = 1 initial condition</summary>
    [Fact]
    public void ExponentSolveToRKTest()
    {
      foreach (var sp in Ode.RK547M(0,
          1,
          (t, x) => -x,
          new Options { RelativeTolerance = 1e-3 }).SolveTo(1000))
        Assert.True(Math.Abs(sp.X[0] - Math.Exp(-sp.T)) < 1e-2);
    }

    /// <summary>Solves dx/dt = exp(-x) equation an stores results in array</summary>
    [Fact]
    public void ExponentSolveToArrayTest()
    {
      var arr = Ode.RK547M(0,
          1,
          (t, x) => -x,
          new Options { RelativeTolerance = 1e-3 }).SolveTo(1000).ToArray();

      foreach (var sp in arr)
        Assert.True(Math.Abs(sp.X[0] - Math.Exp(-sp.T)) < 1e-2); // AbsTol instead of 1e-4
    }
  }
}
