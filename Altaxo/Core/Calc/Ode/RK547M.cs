#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Runge-Kutta method of 5th order of Dormand and Prince with 7 stages.
  /// This method can provide dense output of order 4.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Ode.RungeKuttaExplicitBase" />
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer, Ordinary differential equations I, 2nd edition, 1993.</para>
  /// <para>[2] Jimenez et al., Locally Linearized Runge Kutta method of Dormand and Prince, arXiv:1209.1415v2, 22 Dec 2013</para>
  /// <para>[3] Engeln-Müllges et al., Numerik-Algorithmen, Springer, 2011 (in German)</para></remarks>
  public partial class RK547M : RungeKuttaExplicitBase
  {
    #region Core

    protected class CoreRK547M : Core
    {
      public CoreRK547M(int order, int stages, double[][] a, double[] b, double[]? bl, double[] c, double x0, double[] y, Action<double, double[], double[]> f)
        : base(order, stages, a, b, bl, c, x0, y, f)
      {
      }
      /*

      /// <inheritdoc/> // overridden because we can provide output of 4th order instead of 3rd order
      public override double[] GetInterpolatedY_volatile(double theta)
      {
        var y = _y_previous;
        var ys = _y_stages;
        int n = y.Length;
        var h = _stepSize_current;
        var k0 = _k[0];
        var k2 = _k[2];
        var k3 = _k[3];
        var k4 = _k[4];
        var k5 = _k[5];
        var k6 = _k[6];

        if (_rcont is null)
        {
          _rcont = new double[5][];
          for (int i = 0; i < _rcont.Length; ++i)
            _rcont[i] = new double[n];
        }
        var rcont0 = _rcont[0];
        var rcont1 = _rcont[1];
        var rcont2 = _rcont[2];
        var rcont3 = _rcont[3];
        var rcont4 = _rcont[4];

        if (!_isDenseOutputPrepared)
        {
          _isDenseOutputPrepared = true;


          // now calculate the polynomial coefficients for dense interpolation
          var interpolation4 = _interpolation_aij[0];
          double valcont1, valcont2;
          for (int ni = 0; ni < n; ++ni)
          {
            rcont0[ni] = y[ni]; // values at begin of step
            rcont1[ni] = valcont1 = _y_current[ni] - y[ni]; // values at end of step minus values at begin of step
            rcont2[ni] = valcont2 = h * k0[ni] - valcont1;
            rcont3[ni] = valcont1 - h * k6[ni] - valcont2;
            rcont4[ni] = h * (interpolation4[0] * k0[ni] + interpolation4[2] * k2[ni] + interpolation4[3] * k3[ni] + interpolation4[4] * k4[ni] + interpolation4[5] * k5[ni] + interpolation4[6] * k6[ni]);
          }
        }

        var theta1 = 1 - theta;
        for (int ni = 0; ni < n; ++ni)
        {
          ys[ni] = rcont0[ni] + theta * (rcont1[ni] + theta1 * (rcont2[ni] + theta * (rcont3[ni] + theta1 * rcont4[ni])));
        }

        return ys;
      }

      */
    }



    #endregion

    /// <summary>
    /// Initializes the Runge-Kutta method.
    /// </summary>
    /// <param name="x">The initial x value.</param>
    /// <param name="y">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    public override RungeKuttaExplicitBase Initialize(double x, double[] y, Action<double, double[], double[]> f)
    {
      _core = new CoreRK547M(Order, NumberOfStages, A, BH, BHML, C, x, y, f);
      if (InterpolationCoefficients is not null)
        _core.InterpolationCoefficients = InterpolationCoefficients;

      return this;
    }


    /// <summary>Scheme coefficients, see [3] p.689</summary>
    private static readonly double[][] _sa = new double[][]
        {
          new double[]{                },
          new double[] { 1 / 5d },
          new double[] { 3 / 40d, 9 / 40d },
          new double[] { 44 / 45d, -56 / 15d, 32 / 9d },
          new double[] { 19372 / 6561d, -25360 / 2187d, 64448 / 6561d, -212 / 729d },
          new double[] { 9017 / 3168d, -355 / 33d, 46732 / 5247d, 49 / 176d, -5103 / 18656d },
          new double[] { 35 / 384d, 0, 500 / 1113d, 125 / 192d, -2187 / 6784d, 11 / 84d }
         };
    /// <summary>Scheme coefficients 5th order, see [3] p.689</summary>
    private static readonly double[] _sbh = new double[] { 35 / 384d, 0, 500 / 1113d, 125 / 192d, -2187 / 6784d, 11 / 84d, 0 };

    /// <summary>Scheme coefficients 4th order, see [3] p.689</summary>
    private static readonly double[] _sbl = new double[] { 5179 / 57600d, 0, 7571 / 16695d, 393 / 640d, -92097 / 339200d, 187 / 2100d, 1 / 40d };

    /// <summary>Scheme coefficients 5th order minus scheme coefficients 4th order, see [3] p.689</summary>
    private static readonly double[] _sbhml = new double[] { 71 / 57600d, 0, -71 / 16695d, 71 / 1920d, -17253 / 339200d, 22 / 525d, -1 / 40d };

    /// <summary>Scheme coefficients, see [3] p.689</summary>
    private static readonly double[] _sc = new double[] { 0, 1 / 5d, 3 / 10d, 4 / 5d, 8 / 9d, 1, 1 };

    /// <summary>
    /// Interpolation coefficients (for the 4th order)
    /// </summary>
    private static readonly double[][] _sInterpolationAij = new double[][]
        {
         new double[] { -12715105075 / 11282082432d, 0, 87487479700 / 32700410799d, -10690763975 / 1880347072d, 701980252875 / 199316789632d, -1453857185 / 822651844d, 69997945 / 29380423d },
        };

    /// <inheritdoc/>
    public override int Order => 5;

    public override int NumberOfStages => 7;

    /// <summary>
    /// Sets the stiffness detection threshold value.
    /// </summary>
    protected override double StiffnessDetectionThresholdValue => 3.25;


    /// <inheritdoc/>
    protected override double[][] A => _sa;

    /// <inheritdoc/>
    protected override double[] BH => _sbh;

    /// <inheritdoc/>
    protected override double[] BHML => _sbhml;

    /// <inheritdoc/>
    protected override double[] C => _sc;


    /// <inheritdoc/>
    protected override double[][]? InterpolationCoefficients => _sInterpolationAij;


  }
}
