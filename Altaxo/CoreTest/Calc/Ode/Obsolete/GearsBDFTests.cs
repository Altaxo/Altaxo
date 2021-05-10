#region Copyright

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class GearsBDFTest
  {
    
    [Fact]
    public void GearTest3a()
    {
      Vector fuu(double t, Vector y)
      {
        return new Vector(y[0] / t);
      }

      var pulse = Altaxo.Calc.Ode.Obsolete.Ode.GearBDF(
      1,
      new Vector(1),
      fuu,
      new Altaxo.Calc.Ode.Obsolete.Options { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var ode = new GearsBDF();

      ode.Initialize(
      1,
      new double[] { 1 },
      (t, y, dydt) => { dydt[0] = y[0] / t; },
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];
      foreach (var spulse in pulse.SolveTo(100000))
      {
        double t = spulse.T;
        ode.Evaluate(out var tsp, sp);

        AssertEx.Equal(t, tsp, 1e-4);

        var y0_expected = t;

        AssertEx.Equal(y0_expected, sp[0], 1E-3 * y0_expected + 1E-4);
      }
    }

    [Fact]
    public void GearTest4a()
    {
      // evaluating y' = 2 y/ t  solution y = t²
      var ode = new GearsBDF();

      ode.Initialize(
      1,
      new double[] { 1 },
      (t, y, dydt) => { dydt[0] = 2 * y[0] / t; },
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];
      double tres;
      do
      {
        ode.Evaluate(out tres, sp);
        var y0_expected = tres * tres;

        AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
      } while (tres < 1e6);
    }

    [Fact]
    public void GearTest5a()
    {
      // evaluating y' = 3 y/ t  solution y = t³
      var ode = new GearsBDF();

      ode.Initialize(
      1,
      new double[] { 1 },
      (t, y, dydt) => { dydt[0] = 3 * y[0] / t; },
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];
      double tres;
      do
      {
        ode.Evaluate(out tres, sp);
        var y0_expected = tres * tres * tres;

        AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
      } while (tres < 1e6);
    }

    /// <summary>
    /// Tests the equations y' = exponent * y/ t, which should give the solution y = t^exponent,
    /// with step size evaluated by the ODE solver.
    /// </summary>
    [Fact]
    public void GearTest6a()
    {
      for (int exponent = 2; exponent <= 5; exponent++)
      {
        // evaluating y' = 3 y/ t  solution y = t^exponent
        var ode = new GearsBDF();

        ode.Initialize(
        1,
        new double[] { 1 },
        (t, y, dydt) => { dydt[0] = exponent * y[0] / t; },
        new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

        var sp = new double[1];
        double tres;
        do
        {
          ode.Evaluate(out tres, sp);
          var y0_expected = RMath.Pow(tres, exponent);

          AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
        } while (tres < 1e6);
      }
    }

    /// <summary>
    /// Tests the equations y' = exponent * y/ t, which should give the solution y = t^exponent,
    /// with time points provided externally.
    /// </summary>
    [Fact]
    public void GearTest6b()
    {
      for (int exponent = 2; exponent <= 5; exponent++)
      {
        // evaluating y' = 3 y/ t  solution y = t^exponent
        var ode = new GearsBDF();

        ode.Initialize(
        1,
        new double[] { 1 },
        (t, y, dydt) => { dydt[0] = exponent * y[0] / t; },
        new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

        var sp = new double[1];
        for (double time = 1; time < 1000; time += 1)
        {
          ode.Evaluate(time, sp);

          var y0_expected = RMath.Pow(time, exponent);
          AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
        }
        for (double time = 1000; time <= 1000000; time += 1000)
        {
          ode.Evaluate(time, sp);
          AssertEx.Equal(time, time, 1e-8);

          var y0_expected = RMath.Pow(time, exponent);
          AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
        }
      }
    }

    /// <summary>
    /// Tests the equations y' = exponent * y/ t, which should give the solution y = t^exponent,
    /// with time points provided externally, with logarithmic spacing from 1 to 1e6.
    /// </summary>
    [Fact]
    public void GearTest6c()
    {
      for (int exponent = 2; exponent <= 5; exponent++)
      {
        // evaluating y' = 3 y/ t  solution y = t^exponent
        var ode = new GearsBDF();

        ode.Initialize(
        1,
        new double[] { 1 },
        (t, y, dydt) => { dydt[0] = exponent * y[0] / t; },
        new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

        var sp = new double[1];
        for (double ii = 0; ii <= 6000; ii += 1)
        {
          double time = Math.Pow(10, ii / 1000.0);
          ode.Evaluate(time, sp);

          var y0_expected = RMath.Pow(time, exponent);
          AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
        }
      }
    }

    /// <summary>
    /// Tests the equations y' = 1/y, y[t=0]==1/64, which should give the solution y=Sqrt(1+8192*t)/64
    /// with step size evaluated by the ODE solver.
    /// </summary>
    [Fact]
    public void GearTest7a()
    {
      // evaluating y' = 1/y, y[0]=1/64
      var ode = new GearsBDF();

      ode.Initialize(
      0, // t0=0
      new double[] { 1.0 / 64 }, // y0=1/64
      (t, y, dydt) => { dydt[0] = 1 / y[0]; },
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];
      double tres;
      do
      {
        ode.Evaluate(out tres, sp);
        var y0_expected = Math.Sqrt(1 + 8192 * tres) / 64;

        AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
      } while (tres < 1e6);
    }

    /// <summary>
    /// Tests the equations y' = y², y[t=0]==1/64, which should give the solution y=1/(64-t)
    /// with step size evaluated by the ODE solver.
    /// </summary>
    [Fact]
    public void GearTest7b()
    {
      Vector fuu(double t, Vector y)
      {
        return new Vector(y[0] * y[0]);
      }

      var pulse = Altaxo.Calc.Ode.Obsolete.Ode.GearBDF(
      0,
      new Vector(1 / 64.0),
      fuu,
      new Altaxo.Calc.Ode.Obsolete.Options { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      // evaluating y' = y², y[0]=1/64
      var ode = new GearsBDF();

      ode.Initialize(
      0, // t0=0
      new double[] { 1.0 / 64 }, // y0=1/64
      (t, y, dydt) => { dydt[0] = y[0] * y[0]; },
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];

      foreach (var spulse in pulse.SolveTo(32))
      {
        ode.Evaluate(out var tres, sp);
        Assert.Equal(spulse.T, tres);
        Assert.Equal(spulse.X[0], sp[0]);

        var y0_expected = 1 / (64 - tres);

        AssertEx.Equal(y0_expected, spulse.X[0], 1E-6 * y0_expected + 1E-7);
        AssertEx.Equal(y0_expected, sp[0], 1E-6 * y0_expected + 1E-7);
      };
    }

    /// <summary>
    /// Tests the equations y' = -y, y[t=0]==1, which should give the solution y=Exp(-x)
    /// with step size evaluated by the ODE solver.
    /// </summary>
    [Fact]
    public void GearTest7d()
    {
      // Evaluate y' = -y, y0=1, which should give y=Exp(-x)
      var ode = new GearsBDF();

      ode.Initialize(
      0, // t0=0
      new double[] { 1.0 }, // y0=1
      (t, y, dydt) => { dydt[0] = -y[0]; },
      new GearsBDFOptions { RelativeTolerance = 1e-8, AbsoluteTolerance = 1E-8 });

      var sp = new double[1];

      for (int i=0;i<100000;++i )
      {
        ode.Evaluate(out var tres, sp);
        AssertEx.AreEqual(Math.Exp(-tres), sp[0], 1E-6, 1E-6);
        if (tres > 6)
          break;
      }
    }

    /// <summary>
    /// Same test as above, but now with a sparse matrix
    /// </summary>
    [Fact]
    public void GearTestSparse01()
    {
      const double lambda1 = -1;
      const double lambda2 = -1000;
      const double lambda1PlusLambda2By2 = (lambda1 + lambda2) / 2;
      const double lambda1MinusLambda2By2 = (lambda1 - lambda2) / 2;

      const double C1 = 1;
      const double C2 = 1;

      var ode = new GearsBDF();

      ode.InitializeSparse(
      0,
      new double[] { C1 + C2, C1 - C2 },
      (t, y, dydt) => { dydt[0] = lambda1PlusLambda2By2 * y[0] + lambda1MinusLambda2By2 * y[1]; dydt[1] = lambda1MinusLambda2By2 * y[0] + lambda1PlusLambda2By2 * y[1]; },
      null,
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      int nCounter = 0;
      var sp = new double[2];

      double tres = 0;
      for (; tres <= 20;)
      {
        ode.Evaluate(out tres, sp);

        double t = tres;
        var y0_expected = C1 * Math.Exp(lambda1 * t) + C2 * Math.Exp(lambda2 * t);
        var y1_expected = C1 * Math.Exp(lambda1 * t) - C2 * Math.Exp(lambda2 * t);
        AssertEx.Equal(y0_expected, sp[0], 1E-7 * y0_expected + 1E-8);
        AssertEx.Equal(y1_expected, sp[1], 6E-7 * y1_expected + 1E-8);

        ++nCounter;
      }
    }

    /// <summary>
    /// Same test as above, but now with 100 same and independent systems
    /// </summary>
    [Fact]
    public void GearTestSparse02()
    {
      const double lambda1 = -1;
      const double lambda2 = -1000;
      const double lambda1PlusLambda2By2 = (lambda1 + lambda2) / 2;
      const double lambda1MinusLambda2By2 = (lambda1 - lambda2) / 2;

      const double C1 = 1;
      const double C2 = 1;

      var ode = new GearsBDF();

      var numberOfCells = 100;

      var initY = new double[numberOfCells * 2];

      for (int i = 0; i < numberOfCells; ++i)
      {
        initY[i * 2 + 0] = C1 + C2;
        initY[i * 2 + 1] = C1 - C2;
      }

      var f = new Action<double, double[], double[]>((t, y, dydt) =>
        {
          for (int i = 0; i < y.Length; i += 2)
          {
            dydt[i] = lambda1PlusLambda2By2 * y[i] + lambda1MinusLambda2By2 * y[i + 1];
            dydt[i + 1] = lambda1MinusLambda2By2 * y[i] + lambda1PlusLambda2By2 * y[i + 1];
          }
        }
        );

      ode.InitializeSparse(
      0,
      initY,
      f,
      null,
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      int nCounter = 0;
      var sp = new double[numberOfCells * 2];

      double tres = 0;
      for (; tres <= 20;)
      {
        ode.Evaluate(out tres, sp);

        double t = tres;
        var y0_expected = C1 * Math.Exp(lambda1 * t) + C2 * Math.Exp(lambda2 * t);
        var y1_expected = C1 * Math.Exp(lambda1 * t) - C2 * Math.Exp(lambda2 * t);

        for (int i = 0; i < numberOfCells; ++i)
        {
          AssertEx.Equal(y0_expected, sp[i * 2 + 0], 1E-7 * y0_expected + 1E-8);
          AssertEx.Equal(y1_expected, sp[i * 2 + 1], 6E-7 * y1_expected + 1E-8);
        }

        ++nCounter;
      }
    }

    /// <summary>
    /// Solve a simple diffusion problem with 100 cells
    /// </summary>
    [Fact]
    public void GearTestSparse03()
    {
      var ode = new GearsBDF();

      var numberOfCells = 100;

      var initY = new double[numberOfCells];

      for (int i = 0; i < numberOfCells; ++i)
      {
        initY[i] = 0; // initally zero concentration
      }

      // Symmetrical problem, diffusion from both sides of a plate
      var f = new Action<double, double[], double[]>((t, y, dydt) =>
        {
          double yim1 = 1; // outer concentration at lower index
          double yip1 = 1; // outer concentration at upper index
          int lend = y.Length - 1;

          int i = 0;
          dydt[0] = y[i + 1] - 2 * y[i] + yim1;

          for (i = 1; i < lend; ++i)
          {
            dydt[i] = y[i + 1] - 2 * y[i] + y[i - 1];
          }
          dydt[i] = yip1 - 2 * y[i] + y[i - 1];
        }
        );

      ode.InitializeSparse(
      0,
      initY,
      f,
      null,
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      int nCounter = 0;
      var sp = new double[numberOfCells];

      double tres = 0;
      for (; tres <= 10000;)
      {
        ode.Evaluate(out tres, sp);

        double t = tres;
        for (int i = 0; i < numberOfCells; ++i)
        {
          AssertEx.Equal(sp[i], sp[numberOfCells - 1 - i], 1E-7 * sp[i] + 1E-8); // test for symmetry of the solution
        }

        ++nCounter;
      }
    }

    /// <summary>
    /// Solve a simple diffusion problem with 100 cells
    /// </summary>
    [Fact]
    public void GearTestSparse03b()
    {
      var ode = new GearsBDF();

      var numberOfCells = 100;

      var initY = new double[numberOfCells];

      for (int i = 0; i < numberOfCells; ++i)
      {
        initY[i] = 0; // initally zero concentration
      }

      // Symmetrical problem, diffusion from both sides of a plate
      var f = new Action<double, double[], double[]>((t, y, dydt) =>
      {
        double yim1 = 1; // outer concentration at lower index
        double yip1 = 1; // outer concentration at upper index
        int lend = y.Length - 1;

        var yy = VectorMath.ToROVectorStructAmendedUnshifted(y, yim1, yip1);

        for (int i = 0; i < y.Length; ++i)
        {
          dydt[i] = yy[i + 1] - 2 * yy[i] + yy[i - 1];
        }
      }
        );

      ode.InitializeSparse(
      0,
      initY,
      f,
      null,
      new GearsBDFOptions { RelativeTolerance = 1e-7, AbsoluteTolerance = 1E-8 });

      int nCounter = 0;
      var sp = new double[numberOfCells];

      double tres = 0;
      for (; tres <= 10000;)
      {
        ode.Evaluate(out tres, sp);

        double t = tres;
        for (int i = 0; i < numberOfCells; ++i)
        {
          AssertEx.Equal(sp[i], sp[numberOfCells - 1 - i], 1E-7 * sp[i] + 1E-8); // test for symmetry of the solution
        }

        ++nCounter;
      }
    }
  }
}
