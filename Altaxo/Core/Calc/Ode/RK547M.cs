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

    /// <inheritdoc/>
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
    protected override double[][] InterpolationCoefficients => _sInterpolationAij;


  }
}
