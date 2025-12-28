#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright


namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Embedded formula of Runge-Kutta-Fehlberg, with an order of 4.
  /// </summary>
  /// <remarks>
  /// This method supports dense output of order 3.
  /// <para/>
  /// <para>Reference:</para>
  /// <para>[3] Engeln-Müllges et al., Numerik-Algorithmen, Springer, 2011 (in German)</para>
  /// </remarks>
  public class RKF43 : RungeKuttaExplicitBase
  {
    private static readonly double[][] _sa = new double[][]
      {
      new double[] { },
      new double[] { 2/7d  },
      new double[] { 77/900d,   343/900d },
      new double[] { 805/1444d, -77175/ 54872d, 97125/54872d },
      new double[] { 79/490d,   0,              2175/3626d,  2166/9065d }
      };

    private static readonly double[] _sbh = new double[] { 229/1470d, 0, 1125/1813d, 13718/81585d, 1/18d };
    private static readonly double[] _sbl = new double[] { 79/490d, 0, 2175/3626d, 2166/9065d};
    private static readonly double[] _sbhml = new double[] { -4 / 735d, 0, 75 / 3626d, -5776 / 81585d, 1 / 18d };

    private static readonly double[] _sc = new double[] { 0, 2 / 7d, 7 / 5d, 35, 38d, 1 };

    /// <inheritdoc/>
    public override int Order => 4;

    /// <inheritdoc/>
    public override int NumberOfStages => 5;

    /// <inheritdoc/>
    public override int NumberOfAdditionalStagesForDenseOutput => 0;

    /// <inheritdoc/>
    protected override double StiffnessDetectionThresholdValue => 3;


    /// <inheritdoc/>
    protected override double[][] A => _sa;

    /// <inheritdoc/>
    protected override double[] BH => _sbh;

    /// <inheritdoc/>
    protected override double[]? BHML => _sbhml;

    /// <inheritdoc/>
    protected override double[] C => _sc;

    /// <inheritdoc/>
    protected override double[][] InterpolationCoefficients => _emptyJaggedDoubleArray;
  }
}
