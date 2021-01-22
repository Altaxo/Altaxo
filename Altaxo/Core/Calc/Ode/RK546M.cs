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

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Runge-Kutta method of 5th order of Dormand and Prince with 6 stages.
  /// Attention: This method can only provide dense output of order 3.
  /// If dense output of high accuracy is needed, use method <see cref="RK547M"/> .
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer, Ordinary differential equations I, 2nd edition, 1993.</para>
  /// <para>[2] Jimenez et al., Locally Linearized Runge Kutta method of Dormand and Prince, arXiv:1209.1415v2, 22 Dec 2013</para>
  /// <para>[3] Engeln-Müllges et al., Numerik-Algorithmen, Springer, 2011 (in German)</para>
  /// </remarks>
  public class RK546M : RungeKuttaExplicitBase
  {
    /// <summary>Scheme coefficients, see [3] p.688</summary>
    private static readonly double[][] _sa = new double[][]
        {
          new double[]{                },
          new double[] { 1 / 5d },
          new double[] { 3 / 40d, 9 / 40d },
          new double[] { 3 / 10d, -9 / 10d, 6 / 5d },
          new double[] { 226 / 729d, -25 / 27d, 880 / 729d, 55 / 729d },
          new double[] { -181 / 270d, 5 / 2d, -266 / 297d, -91 / 27d, 189 / 55d },
         };
    /// <summary>Scheme coefficients 5th order, see [3] p.688 (attention: there is an error in this reference in the denominator of the third element, it must be 2079 instead of 2075)</summary>
    private static readonly double[] _sbh = new double[] { 19 / 216d, 0, 1000 / 2079d, -125 / 216d, 81 / 88d, 5 / 56d };

    /// <summary>Scheme coefficients 4th order, see [3] p.688</summary>
    private static readonly double[] _sbl = new double[] { 31 / 540d, 0, 190 / 297d, -145 / 108d, 351 / 220d, 1 / 20d };

    private static readonly double[] _sbhml = new double[] { 11 / 360d, 0, -10 / 63d, 55 / 72d, -27 / 40d, 11 / 280d };

    /// <summary>Scheme coefficients, see [3] p.688</summary>
    private static readonly double[] _sc = new double[] { 0, 1 / 5d, 3 / 10d, 3 / 5d, 2 / 3d, 1 };

    /// <inheritdoc/>
    public override int Order => 5;

    /// <inheritdoc/>
    public override int NumberOfStages => 6;

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
  }
}
