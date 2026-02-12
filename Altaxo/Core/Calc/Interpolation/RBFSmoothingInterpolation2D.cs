using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Provides a two-dimensional radial basis function (RBF) smoother for scattered <c>(x,y,z)</c> data.
  /// </summary>
  /// <remarks>
  /// This implementation fits a model of the form
  /// <c>f(x,y) = p(x,y) + Σ wᵢ φ(‖(x,y)-(xᵢ,yᵢ)‖)</c> using a Tikhonov/"ridge" smoothing term controlled by <see cref="Lambda" />.
  /// If the input data contains multiple points with identical <c>(x,y)</c> coordinates, the default behavior is to
  /// collapse such duplicate sites by averaging their <c>z</c>-values before fitting.
  ///
  /// Literature:
  /// <list type="bullet">
  /// <item>
  /// <description>
  /// M. D. Buhmann, <i>Radial Basis Functions: Theory and Implementations</i>, Cambridge University Press, 2003.
  /// </description>
  /// </item>
  /// <item>
  /// <description>
  /// H. Wendland, <i>Scattered Data Approximation</i>, Cambridge University Press, 2005.
  /// </description>
  /// </item>
  /// <item>
  /// <description>
  /// G. Wahba, <i>Spline Models for Observational Data</i>, SIAM, 1990 (thin-plate smoothing splines).
  /// </description>
  /// </item>
  /// </list>
  /// </remarks>
  public class RBFSmoothingInterpolation2D
  {
    /// <summary>
    /// Enumerates the supported radial basis function kernels.
    /// </summary>
    public enum KernelKind
    {
      /// <summary>Thin-plate spline kernel <c>φ(r)=r² log(r)</c> (with <c>φ(0)=0</c>).</summary>
      ThinPlateSpline,
      /// <summary>Multiquadric kernel <c>φ(r)=sqrt(1+(εr)²)</c>.</summary>
      Multiquadric,
      /// <summary>Inverse multiquadric kernel <c>φ(r)=1/sqrt(1+(εr)²)</c>.</summary>
      InverseMultiquadric,
      /// <summary>Gaussian kernel <c>φ(r)=exp(-(εr)²)</c>.</summary>
      Gaussian
    }

    /// <summary>
    /// Specifies the polynomial tail <c>p(x,y)</c> appended to the RBF sum.
    /// </summary>
    public enum PolynomialTail
    {
      /// <summary>No polynomial tail.</summary>
      None,
      /// <summary>Constant tail <c>p(x,y)=a</c>.</summary>
      Constant,
      /// <summary>Affine tail <c>p(x,y)=a + b·x + c·y</c>.</summary>
      Affine
    }

    private readonly double[] _x;
    private readonly double[] _y;
    private readonly KernelKind _kernel;
    private readonly PolynomialTail _tail;
    private readonly double _epsilon;
    private readonly double _lambda;

    private readonly Vector<double> _weights;
    private readonly Vector<double> _poly;

    /// <summary>Gets the number of support points used by this model.</summary>
    public int Count => _x.Length;

    /// <summary>Gets the kernel used by this model.</summary>
    public KernelKind Kernel => _kernel;

    /// <summary>Gets the polynomial tail type used by this model.</summary>
    public PolynomialTail Tail => _tail;

    /// <summary>Gets the kernel shape parameter (used by some kernels).</summary>
    public double Epsilon => _epsilon;

    /// <summary>Gets the smoothing parameter used during fitting.</summary>
    public double Lambda => _lambda;

    private RBFSmoothingInterpolation2D(
      double[] x,
      double[] y,
      KernelKind kernel,
      PolynomialTail tail,
      double epsilon,
      double lambda,
      Vector<double> weights,
      Vector<double> poly)
    {
      _x = x;
      _y = y;
      _kernel = kernel;
      _tail = tail;
      _epsilon = epsilon;
      _lambda = lambda;
      _weights = weights;
      _poly = poly;
    }

    /// <summary>
    /// Fits an RBF smoother to scattered <c>(x,y,z)</c> data.
    /// </summary>
    /// <param name="x">The x-coordinates of the data points.</param>
    /// <param name="y">The y-coordinates of the data points.</param>
    /// <param name="z">The z-values of the data points.</param>
    /// <param name="kernel">The radial basis function kernel.</param>
    /// <param name="lambda">Smoothing parameter (Tikhonov regularization). Use 0 for interpolation; use &gt; 0 for smoothing.</param>
    /// <param name="epsilon">Kernel shape parameter (used by some kernels, e.g. Gaussian/multiquadric).</param>
    /// <param name="tail">Optional polynomial tail.</param>
    /// <returns>A fitted smoother that can be evaluated at arbitrary <c>(x,y)</c>.</returns>
    public static RBFSmoothingInterpolation2D Fit(
      IReadOnlyList<double> x,
      IReadOnlyList<double> y,
      IReadOnlyList<double> z,
      KernelKind kernel = KernelKind.ThinPlateSpline,
      double lambda = 0,
      double epsilon = 1,
      PolynomialTail tail = PolynomialTail.Affine,
      bool collapseDuplicateSites = true)
    {
      if (x is null)
        throw new ArgumentNullException(nameof(x));
      if (y is null)
        throw new ArgumentNullException(nameof(y));
      if (z is null)
        throw new ArgumentNullException(nameof(z));
      if (x.Count != y.Count || x.Count != z.Count)
        throw new ArgumentException("x, y and z must have the same length.");

      var n = x.Count;
      var xa = new double[n];
      var ya = new double[n];
      var za = new double[n];

      for (int i = 0; i < n; i++)
      {
        xa[i] = x[i];
        ya[i] = y[i];
        za[i] = z[i];
      }

      return Fit(xa, ya, za, kernel, lambda, epsilon, tail, collapseDuplicateSites);
    }

    /// <summary>
    /// Fits an RBF smoother to scattered <c>(x,y,z)</c> data.
    /// </summary>
    /// <param name="x">The x-coordinates of the data points.</param>
    /// <param name="y">The y-coordinates of the data points.</param>
    /// <param name="z">The z-values of the data points.</param>
    /// <param name="kernel">The radial basis function kernel.</param>
    /// <param name="lambda">Smoothing parameter (Tikhonov regularization). Use 0 for interpolation; use &gt; 0 for smoothing.</param>
    /// <param name="epsilon">Kernel shape parameter (used by some kernels, e.g. Gaussian/multiquadric).</param>
    /// <param name="tail">Optional polynomial tail.</param>
    /// <returns>A fitted smoother that can be evaluated at arbitrary <c>(x,y)</c>.</returns>
    public static RBFSmoothingInterpolation2D Fit(
      ReadOnlySpan<double> x,
      ReadOnlySpan<double> y,
      ReadOnlySpan<double> z,
      KernelKind kernel = KernelKind.ThinPlateSpline,
      double lambda = 0,
      double epsilon = 1,
      PolynomialTail tail = PolynomialTail.Affine,
      bool collapseDuplicateSites = true)
    {
      if (x.Length != y.Length || x.Length != z.Length)
        throw new ArgumentException("x, y and z must have the same length.");

      return FitCore(x, y, z, kernel, lambda, epsilon, tail, collapseDuplicateSites);
    }

    /// <summary>
    /// Core implementation of fitting that operates on spans.
    /// </summary>
    /// <param name="x">The x-coordinates of the data points.</param>
    /// <param name="y">The y-coordinates of the data points.</param>
    /// <param name="z">The z-values of the data points.</param>
    /// <param name="kernel">The radial basis function kernel.</param>
    /// <param name="lambda">Smoothing parameter (Tikhonov regularization).</param>
    /// <param name="epsilon">Kernel shape parameter (used by some kernels).</param>
    /// <param name="tail">Optional polynomial tail.</param>
    /// <returns>A fitted smoother instance.</returns>
    private static RBFSmoothingInterpolation2D FitCore(
      ReadOnlySpan<double> x,
      ReadOnlySpan<double> y,
      ReadOnlySpan<double> z,
      KernelKind kernel,
      double lambda,
      double epsilon,
      PolynomialTail tail,
      bool collapseDuplicateSites)
    {
      if (x.Length == 0)
        throw new ArgumentException("At least one point is required.", nameof(x));
      if (!(lambda >= 0))
        throw new ArgumentOutOfRangeException(nameof(lambda), "lambda must be >= 0.");
      if (!(epsilon > 0))
        throw new ArgumentOutOfRangeException(nameof(epsilon), "epsilon must be > 0.");

      // We store point coordinates in arrays because the returned smoother must keep them.
      // Optionally collapse duplicate (x,y) sites by averaging their z values.
      double[] xa;
      double[] ya;
      double[] za;
      if (collapseDuplicateSites)
      {
        (xa, ya, za) = CollapseDuplicateSitesExact(x, y, z);
      }
      else
      {
        xa = x.ToArray();
        ya = y.ToArray();
        za = z.ToArray();
      }

      var n = xa.Length;

      var p = TailDimension(tail);
      var a = CreateMatrix.Dense<double>(n + p, n + p, 0);
      var b = CreateVector.Dense<double>(n + p, 0);

      for (int i = 0; i < n; i++)
        b[i] = za[i];

      for (int i = 0; i < n; i++)
      {
        a[i, i] = lambda; // Tikhonov smoothing

        for (int j = 0; j < i; j++)
        {
          var v = RbfValue(kernel, xa[i] - xa[j], ya[i] - ya[j], epsilon);
          a[i, j] = v;
          a[j, i] = v;
        }

        if (p > 0)
        {
          FillTailRow(a, i, n, tail, xa[i], ya[i]);
          FillTailColumn(a, i, n, tail, xa[i], ya[i]);
        }
      }

      Vector<double> solution;
      try
      {
        solution = a.Solve(b);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Failed to solve RBF system. Consider increasing lambda or using a different tail/kernel.", ex);
      }

      var weights = solution.SubVector(0, n);
      var poly = p > 0 ? solution.SubVector(n, p) : CreateVector.Dense<double>(0, 0);

      return new RBFSmoothingInterpolation2D(xa, ya, kernel, tail, epsilon, lambda, weights, poly);
    }

    private static (double[] x, double[] y, double[] z) CollapseDuplicateSitesExact(
      ReadOnlySpan<double> x,
      ReadOnlySpan<double> y,
      ReadOnlySpan<double> z)
    {
      var dict = new Dictionary<(double x, double y), (double sum, int count)>(x.Length);
      for (int i = 0; i < x.Length; i++)
      {
        var key = (x[i], y[i]);
        if (dict.TryGetValue(key, out var acc))
          dict[key] = (acc.sum + z[i], acc.count + 1);
        else
          dict.Add(key, (z[i], 1));
      }

      var n = dict.Count;
      var xa = new double[n];
      var ya = new double[n];
      var za = new double[n];

      int idx = 0;
      foreach (var kvp in dict)
      {
        xa[idx] = kvp.Key.x;
        ya[idx] = kvp.Key.y;
        za[idx] = kvp.Value.sum / kvp.Value.count;
        idx++;
      }

      return (xa, ya, za);
    }

    /// <summary>
    /// Evaluates the fitted smoother at the provided point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>The smoothed/interpolated value at <c>(x,y)</c>.</returns>
    public double Evaluate(double x, double y)
    {
      double sum = 0;
      for (int i = 0; i < _x.Length; i++)
        sum += _weights[i] * RbfValue(_kernel, x - _x[i], y - _y[i], _epsilon);

      return sum + EvaluateTail(_tail, _poly, x, y);
    }

    /// <summary>
    /// Evaluates the gradient of the fitted smoother at the provided point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>
    /// A tuple <c>(dx, dy)</c> containing the partial derivatives <c>∂f/∂x</c> and <c>∂f/∂y</c>.
    /// </returns>
    public (double dx, double dy) EvaluateGradient(double x, double y)
    {
      double gx = 0;
      double gy = 0;
      for (int i = 0; i < _x.Length; i++)
      {
        var dx = x - _x[i];
        var dy = y - _y[i];
        var (kdx, kdy) = RbfGradient(_kernel, dx, dy, _epsilon);
        gx += _weights[i] * kdx;
        gy += _weights[i] * kdy;
      }

      var (tx, ty) = EvaluateTailGradient(_tail, _poly);
      return (gx + tx, gy + ty);
    }

    /// <summary>
    /// Returns a delegate that evaluates this smoother.
    /// </summary>
    public Func<double, double, double> AsFunction() => Evaluate;

    private static int TailDimension(PolynomialTail tail)
    {
      return tail switch
      {
        PolynomialTail.None => 0,
        PolynomialTail.Constant => 1,
        PolynomialTail.Affine => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(tail))
      };
    }

    /// <summary>
    /// Fills the <c>P</c> block entries for one data point (row direction).
    /// </summary>
    private static void FillTailRow(Matrix<double> a, int row, int colOffset, PolynomialTail tail, double x, double y)
    {
      switch (tail)
      {
        case PolynomialTail.Constant:
          a[row, colOffset + 0] = 1;
          break;
        case PolynomialTail.Affine:
          a[row, colOffset + 0] = 1;
          a[row, colOffset + 1] = x;
          a[row, colOffset + 2] = y;
          break;
      }
    }

    /// <summary>
    /// Fills the <c>Pᵀ</c> block entries for one data point (column direction).
    /// </summary>
    private static void FillTailColumn(Matrix<double> a, int row, int rowOffset, PolynomialTail tail, double x, double y)
    {
      switch (tail)
      {
        case PolynomialTail.Constant:
          a[rowOffset + 0, row] = 1;
          break;
        case PolynomialTail.Affine:
          a[rowOffset + 0, row] = 1;
          a[rowOffset + 1, row] = x;
          a[rowOffset + 2, row] = y;
          break;
      }
    }

    /// <summary>
    /// Evaluates the polynomial tail <c>p(x,y)</c> for the provided point.
    /// </summary>
    private static double EvaluateTail(PolynomialTail tail, Vector<double> poly, double x, double y)
    {
      return tail switch
      {
        PolynomialTail.None => 0,
        PolynomialTail.Constant => poly[0],
        PolynomialTail.Affine => poly[0] + poly[1] * x + poly[2] * y,
        _ => 0
      };
    }

    /// <summary>
    /// Evaluates the gradient of the polynomial tail <c>p(x,y)</c>.
    /// </summary>
    /// <param name="tail">The tail kind.</param>
    /// <param name="poly">Polynomial coefficients as stored in this model.</param>
    /// <returns>
    /// A tuple <c>(dx, dy)</c> containing the partial derivatives <c>∂p/∂x</c> and <c>∂p/∂y</c>.
    /// </returns>
    private static (double dx, double dy) EvaluateTailGradient(PolynomialTail tail, Vector<double> poly)
    {
      return tail switch
      {
        PolynomialTail.None => (0, 0),
        PolynomialTail.Constant => (0, 0),
        PolynomialTail.Affine => (poly[1], poly[2]),
        _ => (0, 0)
      };
    }

    /// <summary>
    /// Evaluates the selected kernel function for the distance between two points.
    /// </summary>
    /// <param name="kernel">The kernel kind.</param>
    /// <param name="dx">Difference in x.</param>
    /// <param name="dy">Difference in y.</param>
    /// <param name="epsilon">Kernel shape parameter (used by some kernels).</param>
    private static double RbfValue(KernelKind kernel, double dx, double dy, double epsilon)
    {
      var r2 = dx * dx + dy * dy;
      if (r2 == 0)
      {
        return kernel switch
        {
          KernelKind.ThinPlateSpline => 0,
          _ => kernel switch
          {
            KernelKind.Multiquadric => 1,
            KernelKind.InverseMultiquadric => 1,
            KernelKind.Gaussian => 1,
            _ => 0
          }
        };
      }

      switch (kernel)
      {
        case KernelKind.ThinPlateSpline:
          // U(r) = r^2 log(r), with U(0)=0
          var r = Math.Sqrt(r2);
          return r2 * Math.Log(r);

        case KernelKind.Multiquadric:
          {
            var e2 = epsilon * epsilon;
            return Math.Sqrt(1 + e2 * r2);
          }

        case KernelKind.InverseMultiquadric:
          {
            var e2 = epsilon * epsilon;
            return 1.0 / Math.Sqrt(1 + e2 * r2);
          }

        case KernelKind.Gaussian:
          {
            var e2 = epsilon * epsilon;
            return Math.Exp(-(e2 * r2));
          }

        default:
          throw new ArgumentOutOfRangeException(nameof(kernel));
      }
    }

    /// <summary>
    /// Evaluates the gradient of the selected kernel function for the distance between two points.
    /// </summary>
    /// <param name="kernel">The kernel kind.</param>
    /// <param name="dx">Difference in x (<c>x - xᵢ</c>).</param>
    /// <param name="dy">Difference in y (<c>y - yᵢ</c>).</param>
    /// <param name="epsilon">Kernel shape parameter (used by some kernels).</param>
    /// <returns>
    /// A tuple <c>(dx, dy)</c> containing the partial derivatives <c>∂φ/∂x</c> and <c>∂φ/∂y</c>.
    /// </returns>
    private static (double dx, double dy) RbfGradient(KernelKind kernel, double dx, double dy, double epsilon)
    {
      var r2 = dx * dx + dy * dy;
      if (r2 == 0)
        return (0, 0);

      switch (kernel)
      {
        case KernelKind.ThinPlateSpline:
          {
            // φ(r)=r^2 log(r) = r2 * log(sqrt(r2)) = 0.5 * r2 * log(r2)
            // dφ/dx = x * (log(r2) + 1)
            var s = Math.Log(r2) + 1;
            return (dx * s, dy * s);
          }

        case KernelKind.Multiquadric:
          {
            // φ = sqrt(1 + (ε^2) r2)
            // ∂φ/∂x = (ε^2 x) / sqrt(1 + ε^2 r2)
            var e2 = epsilon * epsilon;
            var denom = Math.Sqrt(1 + e2 * r2);
            var f = e2 / denom;
            return (f * dx, f * dy);
          }

        case KernelKind.InverseMultiquadric:
          {
            // φ = (1 + ε^2 r2)^(-1/2)
            // ∂φ/∂x = -ε^2 x * (1 + ε^2 r2)^(-3/2)
            var e2 = epsilon * epsilon;
            var t = 1 + e2 * r2;
            var denom = t * Math.Sqrt(t);
            var f = -e2 / denom;
            return (f * dx, f * dy);
          }

        case KernelKind.Gaussian:
          {
            // φ = exp(-(ε^2) r2)
            // ∂φ/∂x = -2 ε^2 x exp(-(ε^2) r2)
            var e2 = epsilon * epsilon;
            var v = Math.Exp(-(e2 * r2));
            var f = -2 * e2 * v;
            return (f * dx, f * dy);
          }

        default:
          throw new ArgumentOutOfRangeException(nameof(kernel));
      }
    }
  }
}
