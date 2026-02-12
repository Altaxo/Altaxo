using System;
using System.Collections.Generic;
using Xunit;

namespace Altaxo.Calc.Interpolation
{
  public class RBFSmoothingInterpolation2DTests
  {
    /// <summary>
    /// Verifies that invalid arguments are rejected (length mismatch, invalid smoothing and shape parameters, empty input).
    /// </summary>
    [Fact]
    public void Fit_RejectsInvalidArguments()
    {
      var x = new[] { 0.0, 1.0 };
      var y = new[] { 0.0, 1.0 };
      var z = new[] { 0.0, 1.0 };

      Assert.Throws<ArgumentException>(() =>
        RBFSmoothingInterpolation2D.Fit(x.AsSpan(), y.AsSpan(), new[] { 0.0 }.AsSpan()));

      Assert.Throws<ArgumentOutOfRangeException>(() =>
        RBFSmoothingInterpolation2D.Fit(x, y, z, lambda: -1));

      Assert.Throws<ArgumentOutOfRangeException>(() =>
        RBFSmoothingInterpolation2D.Fit(x, y, z, epsilon: 0));

      Assert.Throws<ArgumentException>(() =>
        RBFSmoothingInterpolation2D.Fit(ReadOnlySpan<double>.Empty, ReadOnlySpan<double>.Empty, ReadOnlySpan<double>.Empty));
    }

    /// <summary>
    /// Verifies that all kernels can be fitted and evaluated without producing non-finite values.
    /// </summary>
    [Theory]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.Gaussian)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.Multiquadric)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.InverseMultiquadric)]
    public void Fit_AllKernels_Test(RBFSmoothingInterpolation2D.KernelKind kernel)
    {
      var x = new[] { -1.0, -0.5, 0.25, 0.75, 1.25 };
      var y = new[] { 0.5, -0.25, 1.0, -1.0, 0.0 };
      var z = new double[x.Length];
      for (int i = 0; i < z.Length; i++)
        z[i] = Math.Sin(x[i]) + 0.25 * Math.Cos(2 * y[i]);

      var smoother = RBFSmoothingInterpolation2D.Fit(
        x,
        y,
        z,
        kernel: kernel,
        lambda: 1e-6,
        epsilon: 0.9,
        tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      var v = smoother.Evaluate(0.1, -0.2);
      Assert.False(double.IsNaN(v));
      Assert.False(double.IsInfinity(v));
    }

    /// <summary>
    /// Verifies that different polynomial tail configurations can be fitted and evaluated.
    /// </summary>
    [Theory]
    [InlineData(RBFSmoothingInterpolation2D.PolynomialTail.None)]
    [InlineData(RBFSmoothingInterpolation2D.PolynomialTail.Constant)]
    [InlineData(RBFSmoothingInterpolation2D.PolynomialTail.Affine)]
    public void Fit_AllTails_Test(RBFSmoothingInterpolation2D.PolynomialTail tail)
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0, 0.5 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0, 0.5 };

      // Non-degenerate values
      var z = new double[x.Length];
      for (int i = 0; i < z.Length; i++)
        z[i] = 0.3 + 0.1 * x[i] - 0.2 * y[i] + Math.Sin(0.5 * x[i] * y[i]);

      var smoother = RBFSmoothingInterpolation2D.Fit(
        x,
        y,
        z,
        kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline,
        lambda: 1e-6,
        epsilon: 1,
        tail: tail);

      var v = smoother.Evaluate(0.25, 0.75);
      Assert.False(double.IsNaN(v));
      Assert.False(double.IsInfinity(v));
    }

    /// <summary>
    /// Verifies that with <c>lambda=0</c> the fitted model interpolates the training data (to numerical precision)
    /// when using the thin-plate spline kernel with an affine polynomial tail.
    /// </summary>
    [Fact]
    public void Fit_WithZeroLambda_InterpolatesTrainingPoints_TpsAffine()
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0 };

      // Plane: z = 1 + 2x - 3y
      var z = new double[x.Length];
      for (int i = 0; i < z.Length; i++)
        z[i] = 1 + 2 * x[i] - 3 * y[i];

      var smoother = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 0, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      for (int i = 0; i < x.Length; i++)
      {
        var zz = smoother.Evaluate(x[i], y[i]);
        Assert.True(Math.Abs(zz - z[i]) < 1e-10);
      }
    }

    /// <summary>
    /// Verifies that for <c>lambda &gt; 0</c> the fitted model typically does not exactly interpolate the training data.
    /// </summary>
    [Fact]
    public void Fit_WithPositiveLambda_DoesNotExactlyInterpolateTrainingPoints()
    {
      // underlying function
      static double f(double x, double y) => Math.Sin(x) + Math.Cos(y);

      var rng = new System.Random(2);
      var n = 40;
      var x = new double[n];
      var y = new double[n];
      var z = new double[n];
      for (int i = 0; i < n; i++)
      {
        x[i] = -1 + 2 * rng.NextDouble();
        y[i] = -1 + 2 * rng.NextDouble();
        z[i] = f(x[i], y[i]);
      }

      var smooth = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 1e-2, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      bool anyDifference = false;
      for (int i = 0; i < n; i++)
      {
        var e = Math.Abs(smooth.Evaluate(x[i], y[i]) - z[i]);
        if (e > 1e-8)
        {
          anyDifference = true;
          break;
        }
      }

      Assert.True(anyDifference);
    }


    /// <summary>
    /// Verifies that the <see cref="RBFSmoothingInterpolation2D.Fit(System.Collections.Generic.IReadOnlyList{double},System.Collections.Generic.IReadOnlyList{double},System.Collections.Generic.IReadOnlyList{double},Altaxo.Calc.Interpolation.RBFSmoothingInterpolation2D.KernelKind,double,double,Altaxo.Calc.Interpolation.RBFSmoothingInterpolation2D.PolynomialTail)" />
    /// overload and the <see cref="RBFSmoothingInterpolation2D.Fit(System.ReadOnlySpan{double},System.ReadOnlySpan{double},System.ReadOnlySpan{double},Altaxo.Calc.Interpolation.RBFSmoothingInterpolation2D.KernelKind,double,double,Altaxo.Calc.Interpolation.RBFSmoothingInterpolation2D.PolynomialTail)" />
    /// overload produce equivalent models (same evaluation results) for the same input data and parameters.
    /// </summary>
    [Fact]
    public void Fit_Overloads_ListAndSpanYieldSameResult()
    {
      var x = new[] { -1.0, -0.5, 0.25, 0.75, 1.25 };
      var y = new[] { 0.5, -0.25, 1.0, -1.0, 0.0 };
      var z = new double[x.Length];
      for (int i = 0; i < z.Length; i++)
        z[i] = Math.Sin(x[i]) + 0.25 * Math.Cos(2 * y[i]);

      IReadOnlyList<double> xl = x;
      IReadOnlyList<double> yl = y;
      IReadOnlyList<double> zl = z;

      var a = RBFSmoothingInterpolation2D.Fit(xl, yl, zl, kernel: RBFSmoothingInterpolation2D.KernelKind.Gaussian, lambda: 1e-6, epsilon: 0.8, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);
      var b = RBFSmoothingInterpolation2D.Fit(x.AsSpan(), y.AsSpan(), z.AsSpan(), kernel: RBFSmoothingInterpolation2D.KernelKind.Gaussian, lambda: 1e-6, epsilon: 0.8, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      var testPoints = new (double x, double y)[]
      {
        (0.0, 0.0),
        (0.1, -0.2),
        (0.9, 0.4),
        (-0.7, 0.8)
      };

      foreach (var p in testPoints)
      {
        var va = a.Evaluate(p.x, p.y);
        var vb = b.Evaluate(p.x, p.y);
        Assert.True(Math.Abs(va - vb) < 1e-10);
      }
    }

    /// <summary>
    /// Verifies that choosing a positive smoothing parameter (<c>lambda &gt; 0</c>) can reduce the sum of squared error
    /// against a known underlying function compared to the interpolating case (<c>lambda=0</c>) for noisy data.
    /// </summary>
    [Fact]
    public void Fit_WithPositiveLambda_SmoothesNoisyData_ReducesSseAgainstUnderlyingFunction()
    {
      // underlying function
      static double f(double x, double y) => Math.Sin(x) + Math.Cos(y);

      // deterministic pseudo-random noise
      var rng = new System.Random(1);
      var n = 60;
      var x = new double[n];
      var y = new double[n];
      var zNoisy = new double[n];
      var zTrue = new double[n];

      for (int i = 0; i < n; i++)
      {
        x[i] = -1 + 2 * rng.NextDouble();
        y[i] = -1 + 2 * rng.NextDouble();
        zTrue[i] = f(x[i], y[i]);
        zNoisy[i] = zTrue[i] + 0.15 * (2 * rng.NextDouble() - 1);
      }

      var interp = RBFSmoothingInterpolation2D.Fit(x, y, zNoisy, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 0, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);
      var smooth = RBFSmoothingInterpolation2D.Fit(x, y, zNoisy, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 1e-2, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      double sseInterp = 0;
      double sseSmooth = 0;
      for (int i = 0; i < n; i++)
      {
        var eI = interp.Evaluate(x[i], y[i]) - zTrue[i];
        var eS = smooth.Evaluate(x[i], y[i]) - zTrue[i];
        sseInterp += eI * eI;
        sseSmooth += eS * eS;
      }

      Assert.True(sseSmooth < sseInterp);
    }

    /// <summary>
    /// Verifies that the delegate returned by <see cref="RBFSmoothingInterpolation2D.AsFunction"/> matches <see cref="RBFSmoothingInterpolation2D.Evaluate"/>.
    /// </summary>
    [Fact]
    public void AsFunction_ReturnsEvaluateDelegate()
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0 };
      var z = new[] { 0.0, 1.0, 1.0, 2.0 };

      var smoother = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.Gaussian, lambda: 1e-6, epsilon: 0.7, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);
      var fn = smoother.AsFunction();

      var v1 = smoother.Evaluate(0.2, 0.3);
      var v2 = fn(0.2, 0.3);
      Assert.True(Math.Abs(v1 - v2) < 1e-12);
    }

    /// <summary>
    /// Verifies that duplicate sites (identical <c>(x,y)</c> but different <c>z</c>) are collapsed by default by averaging
    /// the <c>z</c>-values, so that fitting succeeds and the evaluation at that site matches the average.
    /// </summary>
    [Fact]
    public void Fit_WithDuplicateSites_DefaultCollapsing_AveragesZ()
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0, 1.0 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0, 1.0 };
      var z = new[] { 0.0, 1.0, 1.0, 1.0, 3.0 };

      var smoother = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 0, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Constant);

      var v = smoother.Evaluate(0.0, 0.0);
      Assert.True(Math.Abs(v - 0.0) < 1e-9);

      v = smoother.Evaluate(1.0, 1.0);
      Assert.True(Math.Abs(v - 2.0) < 1e-9);
    }

    /// <summary>
    /// Verifies that disabling duplicate-site collapsing changes the fitted model for inconsistent duplicates.
    /// Depending on the solver and numerical details the fit may or may not throw; this test checks that the result differs
    /// from the default collapsing behavior.
    /// </summary>
    [Fact]
    public void Fit_WithDuplicateSites_DisableCollapsing_ChangesResult()
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0, 1.0 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0, 1.0 };
      var z = new[] { 0.0, 1.0, 1.0, 1.0, 3.0 };

      var collapsed = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 0, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine, collapseDuplicateSites: true);
      var notCollapsed = RBFSmoothingInterpolation2D.Fit(x, y, z, kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline, lambda: 0, epsilon: 1, tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine, collapseDuplicateSites: false);

      var vA = collapsed.Evaluate(0.0, 0.0);
      var vB = notCollapsed.Evaluate(0.0, 0.0);

      Assert.False(Math.Abs(vA - vB) < 1e-4);
    }

    /// <summary>
    /// Verifies that <see cref="RBFSmoothingInterpolation2D.EvaluateGradient"/> matches a central finite-difference
    /// approximation of <see cref="RBFSmoothingInterpolation2D.Evaluate"/> for all supported kernels.
    /// </summary>
    [Theory]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.Gaussian)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.Multiquadric)]
    [InlineData(RBFSmoothingInterpolation2D.KernelKind.InverseMultiquadric)]
    public void EvaluateGradient_MatchesFiniteDifference(RBFSmoothingInterpolation2D.KernelKind kernel)
    {
      var rng = new System.Random(3);
      var n = 35;
      var x = new double[n];
      var y = new double[n];
      var z = new double[n];
      for (int i = 0; i < n; i++)
      {
        x[i] = -1 + 2 * rng.NextDouble();
        y[i] = -1 + 2 * rng.NextDouble();
        z[i] = Math.Sin(1.3 * x[i]) + 0.7 * Math.Cos(0.9 * y[i]);
      }

      var smoother = RBFSmoothingInterpolation2D.Fit(
        x,
        y,
        z,
        kernel: kernel,
        lambda: 1e-3,
        epsilon: 0.9,
        tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      var p = (x: 0.12, y: -0.34);
      var (gx, gy) = smoother.EvaluateGradient(p.x, p.y);

      var h = 1e-6;
      var fpx = smoother.Evaluate(p.x + h, p.y);
      var fmx = smoother.Evaluate(p.x - h, p.y);
      var fpy = smoother.Evaluate(p.x, p.y + h);
      var fmy = smoother.Evaluate(p.x, p.y - h);

      var gxfd = (fpx - fmx) / (2 * h);
      var gyfd = (fpy - fmy) / (2 * h);

      // Use a somewhat relaxed tolerance because the fitted function depends on a numerical solve.
      Assert.True(Math.Abs(gx - gxfd) < 1e-4);
      Assert.True(Math.Abs(gy - gyfd) < 1e-4);
    }

    /// <summary>
    /// Verifies that for planar training data and an affine tail, the fitted model yields a constant gradient equal
    /// to the plane coefficients (here: <c>∂f/∂x = 2</c> and <c>∂f/∂y = -3</c>).
    /// </summary>
    [Fact]
    public void EvaluateGradient_Plane_IsConstantForTpsAffine()
    {
      var x = new[] { 0.0, 1.0, 0.0, 1.0 };
      var y = new[] { 0.0, 0.0, 1.0, 1.0 };

      // Plane: z = 1 + 2x - 3y
      var z = new double[x.Length];
      for (int i = 0; i < z.Length; i++)
        z[i] = 1 + 2 * x[i] - 3 * y[i];

      var smoother = RBFSmoothingInterpolation2D.Fit(
        x,
        y,
        z,
        kernel: RBFSmoothingInterpolation2D.KernelKind.ThinPlateSpline,
        lambda: 0,
        epsilon: 1,
        tail: RBFSmoothingInterpolation2D.PolynomialTail.Affine);

      var (gx, gy) = smoother.EvaluateGradient(0.25, 0.75);
      Assert.True(Math.Abs(gx - 2) < 1e-10);
      Assert.True(Math.Abs(gy + 3) < 1e-10);
    }
  }
}
