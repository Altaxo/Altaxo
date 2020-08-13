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
  /// Runge-Kutta methods of 5th order of Dormand and Prince.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer, Ordinary differential equations I, 2nd edition, 1993.</para>
  /// <para>[2] Jimenez et al., Locally Linearized Runge Kutta method of Dormand and Prince, arXiv:1209.1415v2, 22 Dec 2013</para>
  /// </remarks>
  public class Dopri5 : RungeKuttaExplicitBase
  {
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
    private static readonly double[] _sbh = new double[] { 35 / 384d, 0, 500 / 1113d, 125 / 192d, -2187 / 6784d, 11 / 84d, 0 };
    private static readonly double[] _sbl = new double[] { 5179 / 57600d, 0, 7571 / 16695d, 393 / 640d, -92097 / 339200d, 187 / 2100d, 1 / 40d };
    private static readonly double[] _sc = new double[] { 0, 1 / 5d, 3 / 10d, 4 / 5d, 8 / 9d, 1, 1 };


    /// <summary>
    /// Interpolation coefficients, see [2], Table 2
    /// </summary>
    private static readonly double[][] _sInterpolationAij = new double[][]
        {
       new double[] { 1, -183 / 64d, 37 / 12d, -145 / 128d },
       new double[] { 0, 0, 0, 0 },
       new double[] { 0, 1500 / 371d, -1000 / 159d, 1000 / 371d },
       new double[] { 0, -125 / 32d, 125 / 12d, -375 / 64d },
       new double[] { 0, 9477 / 3392d, -729 / 106d, 25515 / 6784d },
       new double[] { 0, -11 / 7d, 11 / 3d, -55 / 28d },
       new double[] { 0, 3 / 2d, -4, 5 / 2d },
        };


    protected override double[][] A => _sa;
    protected override double[] BH => _sbh;
    protected override double[] BL => _sbl;
    protected override double[] C => _sc;

    protected override double[][]? InterpolationCoefficients => _sInterpolationAij;
  }
}
