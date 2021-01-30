using System;

namespace Altaxo.Calc.Ode
{
  public partial class AdamsExplicit
  {
    public class Core
    {
      /// <summary>
      /// The interpolation coefficents. These are used to calculate the coefficients for the backward differences, but not
      /// for a full step, but for a partial step. See eq. (1.4) and (1.5) on page 357 in [1].
      /// </summary>
      /// <remarks>The integral of the binomial from 0 to p is a series of p, but these coefficients are not used here directly,
      /// but recalculated for a p-q polynomial (q = 1-p). The intercept of the polynomial is always zero and not included here.</remarks>
      private static readonly double[][] _interpolationCoeff =
      {
        new double[]{ 1},
        new double[]{ 1/2d, -1/2d },
        new double[]{ 5/12d, -5/12d, -1/6d },
        new double[]{ 3/8d, -3/8d, -1/4d, 1/24d },
        new double[]{ 251/720d, -251/720d, -109/360d, 19/240d, 1/120d },
        new double[]{ 95/288d, -95/288d, -49/144d, 53/480d, 1/48d, -1/720d},
        new double[]{ 19087/60480d, -19087/60480d, -11153/30240d, 2753/20160d, 71/2016d, -41/10080d, -1/5040d},
        new double[]{ 5257/17280d, -5257/17280d, -3383/8640d, 6401/40320d, 29/576d, -467/60480d, -1/1440d,  1/40320d},
        new double[]{ 1070017/3628800d, -1070017/3628800d, -744383/1814400d, 215183/1209600d,  5671/86400d, -4381/362880d, -55/36288d, 71/725760d, 1/362880d},
      };

      private static readonly double[] _coeff = new double[] { 1, 1 / 2d, 5 / 12d, 3 / 8d, 251 / 720d, 95 / 288d, 19087 / 60480d, 5257 / 17280d, 1070017 / 3628800d, 25713 / 89600d };


      private static readonly double[][] _directCoeff = new double[][]
      {
        new double[]{},
        new double[]{1},
        new double[]{3/2d, -1/2d},
        new double[]{23/12d, -4/3d, 5/12d },
        new double[]{55/24d, -59/24d, 37/24d, -3/8d},
        new double[]{1901/720d, -1387/360d, 109/30d, -637/360d, 251/720d},
        new double[]{4277/1440d, -2641/480d, 4991/720d, -3649/720d, 959/480d, -95/288d},
        new double[]{198721/60480d, -18637/2520d, 235183/20160d, -10754/945d, 135713/20160d, -5603/2520d, 19087/60480d},
        new double[]{16083/4480d, -1152169/120960d, 242653/13440d, -296053/13440d, 2102243/120960d, -115747/13440d, 32863/13440d, -5257/17280d},
        new double[]{14097247/3628800d, -21562603/1814400d, 47738393/1814400d, -69927631/1814400d, 862303/22680d, -45586321/1814400d, 19416743/1814400d, -4832053/1814400d, 1070017/3628800d}
      };

      private int _order = 4;
      private double _stepSize;
      private long _stepNumber;
      private double _x_initial;


      // <summary>
      /// The differential equation to integrate. First arg is the independent variable (x), second arg are the current y-values, and the 3rd arg adopts the
      /// derivatives y' as calculated by this function.
      /// </summary>
      protected Action<double, double[], double[]> _f;
      private double _x_current;
      private double[] _y_current;
      private double[][] _k;

      public double X => _x_current;
      public double[] Y_volatile => _y_current;

      // helper variables
      private double[] _backwardDifferences;

      private double[] _coefficientsForDenseOutput;
      private double[] _y_interpolated;

      public Core(int order, double stepSize, double x, double[] y, Action<double, double[], double[]> f, double[][] stages)
      {
        _order = order;
        _stepSize = stepSize;
        _stepNumber = 0;
        _x_initial = x;
        _f = f;

        _x_current = x;
        _y_current = (double[])y.Clone();
        _k = stages;
        _backwardDifferences = new double[_order];
        _coefficientsForDenseOutput = new double[_order];
        _y_interpolated = new double[y.Length];
      }



      public virtual void EvaluateNextSolutionPoint()
      {
        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = _k[0][ni]; // Zero-th backward difference

          // Initialize already the 1st stage of backward differences
          for (int si = 1; si < _order; ++si)
          {
            _backwardDifferences[si - 1] = _k[si - 1][ni] - _k[si][ni];
          }
          for (int j = 1; j < _order; ++j)
          {
            sum += _backwardDifferences[0] * _coeff[j];

            // Update backward differences
            for (int si = 1; si < _order - j; ++si)
            {
              _backwardDifferences[si - 1] -= _backwardDifferences[si];
            }
          }
          _y_current[ni] += sum * _stepSize;
        }
        ++_stepNumber;
        _x_current = _x_initial + _stepNumber * _stepSize; // advanced formula to avoid accumulating errors

        var hlp = _k[_k.Length - 1];
        for (int j = _k.Length - 1; j > 0; --j)
          _k[j] = _k[j - 1];
        _k[0] = hlp;

        _f(_x_current, _y_current, _k[0]);
      }

      public virtual void EvaluateNextSolutionPointDir()
      {
        var coeff = _directCoeff[_order];
        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = coeff[0] * _k[0][ni];
          for (int j = 1; j < _order; ++j)
          {
            sum += coeff[j] * _k[j][ni];
          }
          _y_current[ni] += sum * _stepSize;
        }
        _x_current += _stepSize;

        var hlp = _k[_k.Length - 1];
        for (int j = _k.Length - 1; j > 0; --j)
          _k[j] = _k[j - 1];
        _k[0] = hlp;

        _f(_x_current, _y_current, _k[0]);
      }

      public virtual double[] EvaluateInterpolatedPoint(double p)
      {
        double q = 1 - p;

        // calculate coefficients for p
        // for p==1, the result should be identical to the coefficients in _coeff;
        for (int i = 0; i < _order; ++i)
        {
          _coefficientsForDenseOutput[i] = PQPolynomWithoutOffset(p, q, _interpolationCoeff[i]);
        }


        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = _coefficientsForDenseOutput[0] * _k[0][ni]; // _k[0][ni] is the zero-th backward difference

          // Initialize the 1st stage of backward differences
          for (int si = 1; si < _order; ++si)
          {
            _backwardDifferences[si - 1] = _k[si - 1][ni] - _k[si][ni];
          }
          for (int j = 1; j < _order; ++j)
          {
            sum += _coefficientsForDenseOutput[j] * _backwardDifferences[0];

            // Evaluate the next stage of backward differences
            for (int si = 1; si < _order - j; ++si)
            {
              _backwardDifferences[si - 1] -= _backwardDifferences[si];
            }
          }
          _y_interpolated[ni] = _y_current[ni] + sum * _stepSize;
        }
        return _y_interpolated;
      }

      private static double PQPolynomWithoutOffset(double p, double q, double[] a)
      {
        return a.Length switch
        {
          1 => p * (a[0]),
          2 => p * (a[0] + q * (a[1])),
          3 => p * (a[0] + q * (a[1] + p * (a[2]))),
          4 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3])))),
          5 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3] + p * (a[4]))))),
          6 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3] + p * (a[4] + q * (a[5])))))),
          7 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3] + p * (a[4] + q * (a[5] + p * (a[6]))))))),
          8 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3] + p * (a[4] + q * (a[5] + p * (a[6] + q * (a[7])))))))),
          9 => p * (a[0] + q * (a[1] + p * (a[2] + q * (a[3] + p * (a[4] + q * (a[5] + p * (a[6] + q * (a[7] + p * (a[8]))))))))),
          _ => throw new NotImplementedException($"Not implemented for a length of {a.Length}")
        };
      }
    }

   public class InitializationData
    {
      public double? StepSize { get; set; }
      public double X0 { get; set; }
      public double[] Y0 { get; set; }

      public Action<double, double[], double[]> F { get; set; }
    }
  }
}
