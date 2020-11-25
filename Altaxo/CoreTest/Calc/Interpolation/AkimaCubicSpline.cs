using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Calc.Interpolation
{

  public class AkimaCubicSpline_Test
  {
    /// <summary>
    /// Splines a strongly monotonic increasing function and tests whether the derivatives are (i) not zero and ii) positive and iii) are decreasing
    /// </summary>
    [Fact]
    public void Test1()
    {
      var x = new double[100];
      var y = new double[100];
      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = 2 * i;
        y[i] = 7 * i;
      }

      var spline = new AkimaCubicSpline();
      spline.Interpolate(x, y);

      // make sure that all derivatives are positive

      for (int i = 0; i < x.Length; ++i)
      {
        var deriv = spline.GetY1stDerivativeOfX(x[i]);
        AssertEx.Greater(deriv, 0, $"Derivative is 0 at x={x[i]}");
        Assert.Equal(7 / 2.0, deriv, 0);
      }
    }


    /// <summary>
    /// Splines a strongly monotonic increasing function and tests whether the derivatives are (i) not zero and ii) positive and iii) are decreasing
    /// </summary>
    [Fact]
    public void Test2()
    {
      double offset = 0.0164719554599108;
      double amplitude = -29.2707513927763;
      double tau = 6.24647551966886E-23;
      double beta = 0.0401179333360172;

      var x = new double[5000];
      var y = new double[5000];
      for (int i = 0; i < x.Length; ++i)
      {
        var xx = (i + 1) * 2000;
        x[i] = xx;
        y[i] = offset + amplitude * Math.Exp(-Math.Pow(xx / tau, beta));
      }

      var spline = new AkimaCubicSpline();
      spline.Interpolate(x, y);

      // make sure that all derivatives are positive

      for (int i = 0; i < x.Length; ++i)
      {
        var deriv = spline.GetY1stDerivativeOfX(x[i]);
        AssertEx.Greater(deriv, 0, $"Derivative is 0 at x={x[i]}");
      }

    }


  }
}
