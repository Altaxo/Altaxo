namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  public class PeakEnhancement2ndDerivativeOfSpline : IPeakEnhancement
  {
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var s = new Altaxo.Calc.Interpolation.SmoothingCubicSpline();
      s.Interpolate(x, y);

      int numberOfPoints = (x.Length - 1) * 4 + 1;

      var xx = new double[numberOfPoints];
      var yy = new double[numberOfPoints];
      var zz = new double[numberOfPoints];
      for (int i = 0; i < numberOfPoints; i++)
      {
        double t = i / (numberOfPoints - 1d);
        xx[i] = x[0] * (1 - t) + x[^1] * (t);
        yy[i] = -s.GetY1stDerivativeOfX(xx[i]);
      }
      var sg = new Altaxo.Calc.Regression.SavitzkyGolay(7, 1, 2);
      sg.Apply(yy, zz);
      /*
      var ss = new Altaxo.Calc.Interpolation.AkimaCubicSpline();

      ss.Interpolate(xx, zz);
      var yr = new double[y.Length];
      for (int i = 0; i < yr.Length; ++i)
      {
        yr[i] = ss.GetYOfX(x[i]);
      }
      */

      return (xx, zz, null);
    }
  }
}
