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
  /// 'Classical' Runge-Kutta method of order 4. Note that at the moment neither error control (and therefore, no step size control) nor dense output is implemented.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Ode.RungeKuttaExplicitBase" />
  public class RungeKutta4 : RungeKuttaExplicitBase
  {
    private static readonly double[][] _sa = new double[][]
      {
      new double[]{                },
      new double[]{1 / 2d          },
      new double[] { 0, 1 / 2d },
      new double[] { 0, 0, 1 }
      };

    private static readonly double[] _sbh = new double[] { 1 / 6d, 2 / 6d, 2 / 6d, 1 / 6d };

    private static readonly double[] _sc = new double[] { 0, 1 / 2d, 1 / 2d, 1 };

    public override int Order => 4;

    public override int NumberOfStages => 4;
    protected override double StiffnessDetectionThresholdValue => 3;


    protected override double[][] A => _sa;
    protected override double[] BH => _sbh;
    protected override double[]? BHML => null;
    protected override double[] C => _sc;

    protected override double[][]? InterpolationCoefficients => null;
  }
}
