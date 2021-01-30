#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.Ode
{
  public partial class AdamsExplicit
  {
    /// <summary>
    /// Core of the explicit Adams method.
    /// </summary>
    public class Core
    {
      /// <summary>
      /// The interpolation coefficents. These are used to calculate the coefficients for the backward differences, but not
      /// for a full step, but for a partial step. See eq. (1.4) and (1.5) on page 357 in [1].
      /// </summary>
      /// <remarks>The integral of the binomial from 0 to p is a series of p, but these coefficients are not used here directly,
      /// but recalculated for a p-q polynomial (q = 1-p). The intercept of the polynomial is always zero and not included here.</remarks>
      private static readonly double[][] _interpolationCoefficients =
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

      /// <summary>
      /// The coefficients used to calculate the y values of the next step, using backward differences of the derivatives.
      /// </summary>
      private static readonly double[] _coefficientsForBackwardDifferences = new double[] { 1, 1 / 2d, 5 / 12d, 3 / 8d, 251 / 720d, 95 / 288d, 19087 / 60480d, 5257 / 17280d, 1070017 / 3628800d, 25713 / 89600d };


      /// <summary>
      /// The coefficients used to calculate the y values of the next step, using the derivatives directly (only for <see cref="NumberOfStages"/> &lt;=5 
      /// </summary>
      private static readonly double[][] _coefficientsForDirectEvaluation = new double[][]
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

      /// <summary>
      /// Gets the number of stages = number of points used for extrapolation of the slope to the next point.
      /// The value may range from 1 .. 9.
      /// </summary>
      private int _numberOfStages;

      /// <summary>Fixed step size.</summary>
      private double _stepSize;

      /// <summary>The current step number. Is used to calculate the current x value
      /// by using <c>_x_initial +  _stepNumber * _stepSize</c> in order to avoid numerical inaccuracies.</summary>
      private long _stepNumber;


      /// <summary>The initial x value (at initialization of the core). Is used to calculate the current x value
      /// by using <c>_x_initial +  _stepNumber * _stepSize</c> in order to avoid numerical inaccuracies.</summary>
      private double _x_initial;


      /// <summary>
      /// The differential equation to integrate. First arg is the independent variable (x), second arg are the current y-values, and the 3rd arg adopts the
      /// derivatives of y as calculated by this function.
      /// </summary> 
      protected Action<double, double[], double[]> _f;

      /// <summary>The current x value</summary>
      private double _x_current;

      /// <summary>The current y values</summary>
      private double[] _y_current;

      /// <summary>The y derivates. The y derivative at the current position is stored in _k[0], the derivatives of the previous step are stored in _k[1] and so on.</summary>
      private double[][] _k;

      /// <summary>
      /// Gets the current x value;
      /// </summary>
      public double X => _x_current;

      /// <summary>
      /// Gets the current y values. The elements of this array must not be changed, and are intended for immediate use only.
      /// </summary>
      public double[] Y_volatile => _y_current;

      // helper variables
      /// <summary>Helper array to evaluate the backward differences (length = NumberOfStages).</summary>
      private double[] _backwardDifferences;

      /// <summary>
      /// Helper array for temporary accomodation of the coefficients for dense output (calculated for a partial step).
      /// </summary>
      private double[] _coefficientsForDenseOutput;

      /// <summary>Helper array to accodomate the interpolated y values (for dense output).</summary>
      private double[] _y_interpolated;

      /// <summary>
      /// Initializes a new instance of the <see cref="Core"/> class.
      /// </summary>
      /// <param name="order">The number of stages used to extrapolate to the next step.</param>
      /// <param name="stepSize">The fixed step size.</param>
      /// <param name="x">The initial x value.</param>
      /// <param name="y">The initial y values.</param>
      /// <param name="f">The function used to calculate the derivatives. 1st arg is the current x value, 2nd arg are the current y values, and the third arg accomodates the derivatives calculated by this function.</param>
      /// <param name="stages">The y derivatives of the current x, the previous x and so on. The element stages[0] contains the derivatives at <paramref name="x"/>,
      /// the element stages[1] contains the derivatives at the previous point (<paramref name="stages"/>-<paramref name="stepSize"/></param>), and so on.
      public Core(int order, double stepSize, double x, double[] y, Action<double, double[], double[]> f, double[][] stages)
      {
        _numberOfStages = order;
        _stepSize = stepSize;
        _stepNumber = 0;
        _x_initial = x;
        _f = f;

        _x_current = x;
        _y_current = (double[])y.Clone();
        _k = stages;
        _backwardDifferences = new double[_numberOfStages];
        _coefficientsForDenseOutput = new double[_numberOfStages];
        _y_interpolated = new double[y.Length];
      }

      /// <summary>
      /// Evaluates the next solution point. For NumberOfStages &lt;=5, the <see cref="EvaluateNextSolutionPointDirectly"/> /> method is used, because it is slightly faster.
      /// For NumberOfStages &gt;5, the <see cref="EvaluateNextSolutionPointUsingBackwardDifferences"/> is used, because it is somewhat more accurate.
      /// </summary>
      /// <remarks>At the end of the call, <see cref="_x_current"/> contains the current x value, <see cref="_y_current"/> contains
      /// the current y values, and _k[0] contains the current y derivatives.</remarks>
      public virtual void EvaluateNextSolutionPoint()
      {
        if (_numberOfStages <= 5)
          EvaluateNextSolutionPointDirectly(); // for low order, we can evaluate the next point without backward differences
        else
          EvaluateNextSolutionPointUsingBackwardDifferences(); // for higher order, this method is more accurate
      }

      /// <summary>
      /// Evaluates the next solution point using backward differences. This is more accurate than using the derivatives directly,
      /// especially for higher number of stages.
      /// </summary>
      /// <remarks>At the end of the call, <see cref="_x_current"/> contains the current x value, <see cref="_y_current"/> contains
      /// the current y values, and _k[0] contains the current y derivatives.</remarks>
      public virtual void EvaluateNextSolutionPointUsingBackwardDifferences()
      {
        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = _k[0][ni]; // Zero-th backward difference

          // Initialize already the 1st stage of backward differences
          for (int si = 1; si < _numberOfStages; ++si)
          {
            _backwardDifferences[si - 1] = _k[si - 1][ni] - _k[si][ni];
          }
          for (int j = 1; j < _numberOfStages; ++j)
          {
            sum += _backwardDifferences[0] * _coefficientsForBackwardDifferences[j];

            // Update backward differences
            for (int si = 1; si < _numberOfStages - j; ++si)
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

      /// <summary>
      /// Evaluates the next solution point, using the derivatives of the current and previous points directly 
      /// For  NumberOfStages &gt;5, this method is not accurate enough, and <see cref="EvaluateNextSolutionPointUsingBackwardDifferences"/> should be used instead.
      /// </summary>
      /// <remarks>At the end of the call, <see cref="_x_current"/> contains the current x value, <see cref="_y_current"/> contains
      /// the current y values, and _k[0] contains the current y derivatives.</remarks>
      public virtual void EvaluateNextSolutionPointDirectly()
      {
        var coeff = _coefficientsForDirectEvaluation[_numberOfStages];
        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = coeff[0] * _k[0][ni];
          for (int j = 1; j < _numberOfStages; ++j)
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

      /// <summary>
      /// Gets an interpolated point in the interval between x_current and (x_current+stepSize).
      /// </summary>
      /// <param name="p">The relative value (0..1) of the interval [x_current, (x_current+stepSize)]</param>
      /// <returns>The interpolated y values. The array must not be changed, and is intended for immediate use only.</returns>
      public virtual double[] GetInterpolatedPoint(double p)
      {
        double q = 1 - p;

        // calculate coefficients for p
        // for p==1, the result should be identical to the coefficients in _coeff;
        for (int i = 0; i < _numberOfStages; ++i)
        {
          _coefficientsForDenseOutput[i] = PQPolynomWithoutOffset(p, q, _interpolationCoefficients[i]);
        }


        for (int ni = 0; ni < _y_current.Length; ++ni)
        {
          double sum = _coefficientsForDenseOutput[0] * _k[0][ni]; // _k[0][ni] is the zero-th backward difference

          // Initialize the 1st stage of backward differences
          for (int si = 1; si < _numberOfStages; ++si)
          {
            _backwardDifferences[si - 1] = _k[si - 1][ni] - _k[si][ni];
          }
          for (int j = 1; j < _numberOfStages; ++j)
          {
            sum += _coefficientsForDenseOutput[j] * _backwardDifferences[0];

            // Evaluate the next stage of backward differences
            for (int si = 1; si < _numberOfStages - j; ++si)
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

    /// <summary>
    /// Stores the initialization data.
    /// </summary>
    public class InitializationData
    {
      /// <summary>
      /// Gets or sets the size of the step.
      /// </summary>
      public double? StepSize { get; set; }

      /// <summary>
      /// Gets or sets the initial x value.
      /// </summary>
      public double X0 { get; set; }

      /// <summary>
      /// Gets or sets the initial y values.
      /// </summary>
      public double[] Y0 { get; set; }

      /// <summary>
      /// Gets or sets the function used to calculate the derivatives.
      /// </summary>
      public Action<double, double[], double[]> F { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="InitializationData"/> class.
      /// </summary>
      /// <param name="x0">The initial x value.</param>
      /// <param name="y0">The initial y values.</param>
      /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
      public InitializationData(double x0, double[] y0, Action<double, double[], double[]> f)
      {
        X0 = x0;
        Y0 = y0;
        F = f;
      }
    }
  }
}
