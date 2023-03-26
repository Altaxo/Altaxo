using System;
using System.Linq;
using Xunit;

namespace Altaxo.Calc.Interpolation
{
  public class BSpline_Test
  {
    [Fact]
    public void Test_UniformX_LinearY()
    {
      double[] x = { 1, 2, 3, 4, 5, 6, 7 };
      double[] y = { 14, 12, 10, 8, 6, 4, 2 }; // y = 16 - 2*x

      var spl = BSpline1D.createInterpBSpline(x, y, 3);

      for (int i = 2; i <= 14; i++)
      {
        double xx = i / 2.0;
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(16 - 2 * xx, yy, 1e-4, 1e-4);
      }
    }

    [Fact]
    public void Test_UniformX_QuadraticY()
    {
      double[] x = { -3, -2, -1, 0, 1, 2, 3 };
      double[] y = x.Select(xx => xx * xx + xx).ToArray(); // y = x*x +x

      var spl = BSpline1D.createInterpBSpline(x, y, 3);

      for (double xx = -4; xx <= 4; xx += 0.25)
      {
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(xx * xx + xx, yy, 1e-4, 1e-4);
      }
    }

    [Fact]
    public void Test_UniformX_CubicY()
    {
      double[] x = { -3, -2, -1, 0, 1, 2, 3 };
      double[] y = x.Select(xx => xx * xx * xx + xx).ToArray(); // y = x*x*x +x

      var spl = BSpline1D.createInterpBSpline(x, y, 3);

      for (double xx = -4; xx <= 4; xx += 0.25)
      {
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(xx * xx * xx + xx, yy, 1e-4, 1e-4);
      }
    }

    [Fact]
    public void Test_NonuniformX_LinearY()
    {
      var f = new Func<double, double>(x => 16 - 2 * x);
      double[] x = { -3, -1.5, -1, 0, 1, 2.5, 3 };
      double[] y = x.Select(xx => f(xx)).ToArray(); // y = x*x*x +x

      var spl = BSpline1D.createInterpBSpline(x, y, 3);

      for (double xx = -3; xx <= 3; xx += 0.25)
      {
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(f(xx), yy, 1e-4, 1e-4);
      }
    }

    [Fact]
    public void Test_NonuniformX_QuadraticY()
    {
      var f = new Func<double, double>(x => x * x + x);
      double[] x = { -3, -1.5, -1, 0, 1, 2.5, 3 };
      double[] y = x.Select(xx => f(xx)).ToArray(); // y = x*x*x +x

      var spl = BSpline1D.createInterpBSpline(x, y, 3);

      for (double xx = -3; xx <= 3; xx += 0.25)
      {
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(f(xx), yy, 1e-4, 1e-4);
      }
    }


    [Fact]
    public void Test_Approx_UniformX_LinearY()
    {
      var f = new Func<double, double>(x => 16 - 2 * x);
      double[] x = { 1, 2, 3, 4, 5, 6, 7 };
      double[] y = x.Select(xx => f(xx)).ToArray();

      var spl = BSpline1D.createApproxBSpline(x, y, 3, 4);

      for (double xx = x[0]; xx <= x[^1]; xx += 0.25)
      {
        double yy = spl.GetYOfX(xx);
        AssertEx.AreEqual(f(xx), yy, 1e-4, 1e-4);
      }
    }

    /*
    [Fact]
    public void Test_Derivative_NonuniformX_QuadraticY()
    {
      var f = new Func<double, double>(x => x * x + x);
      var df = new Func<double, double>(x => 2 * x + 1);
      double[] x = { -3, -1.5, -1, 0, 1, 2.5, 3 };
      double[] y = x.Select(xx => f(xx)).ToArray();

      var spl = BSpline1D.createInterpBSpline(x, y, 3);
      var dspl = spl.derivativeBSpline(1);

      for (double xx = -3; xx <= 3; xx += 0.25)
      {
        double yy = dspl.GetYOfX(xx);
        AssertEx.AreEqual(df(xx), yy, 1e-4, 1e-4);
      }
    }
    */


  }
}
