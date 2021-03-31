#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class OdeGearsBDFTest1
  {
    private const double lambda1 = -1;
    private const double lambda2 = -1000;
    private const double lambda1PlusLambda2By2 = (lambda1 + lambda2) / 2;
    private const double lambda1MinusLambda2By2 = (lambda1 - lambda2) / 2;

    private void RateEquations(double t, double[] y, double[] dydt)
    {
      dydt[0] = lambda1PlusLambda2By2 * y[0] + lambda1MinusLambda2By2 * y[1];
      dydt[1] = lambda1MinusLambda2By2 * y[0] + lambda1PlusLambda2By2 * y[1];
    }

    private void JacRateEquations(double t, double[] y, double[,] jacobian)
    {
      jacobian[0, 0] = lambda1PlusLambda2By2;
      jacobian[0, 1] = lambda1MinusLambda2By2;
      jacobian[1, 0] = lambda1MinusLambda2By2;
      jacobian[1, 1] = lambda1PlusLambda2By2;
    }

    [Fact]
    public void Test1_WithoutJacobian()
    {
      const double C1 = 1;
      const double C2 = 1;

      double[] y = new double[2];
      y[0] = C1 + C2;
      y[1] = C1 - C2;

      var YDot = new OdeFunction(RateEquations);
      var bdf = new OdeGearsBDF(YDot, 2);
      bdf.SetInitialValues(0, y);

      bdf.RelTolArray[0] = 1.0E-4;
      bdf.RelTolArray[1] = 1.0E-4;

      bdf.AbsTolArray[0] = 1.0E-8;
      bdf.AbsTolArray[1] = 1.0E-8;

      for (int i = -3; i < 5; i++)
      {
        double time = Altaxo.Calc.RMath.Pow(10, i);
        y = bdf.Solve(time);
        var y0_expected = C1 * Math.Exp(lambda1 * time) + C2 * Math.Exp(lambda2 * time);
        var y1_expected = C1 * Math.Exp(lambda1 * time) - C2 * Math.Exp(lambda2 * time);

        AssertEx.Equal(y0_expected, y[0], 1E-3 * y0_expected + 1E-4);
        AssertEx.Equal(y1_expected, y[1], 1E-3 * y1_expected + 1E-4);
      }
    }

    [Fact]
    public void Test2_WithJacobian()
    {
      const double C1 = 1;
      const double C2 = 1;

      double[] y = new double[2];
      y[0] = C1 + C2;
      y[1] = C1 - C2;

      var YDot = new OdeFunction(RateEquations);
      var bdf = new OdeGearsBDF(YDot, 2);
      bdf.SetInitialValues(0, y);

      bdf.RelTolArray[0] = 1.0E-5;
      bdf.RelTolArray[1] = 1.0E-5;

      bdf.AbsTolArray[0] = 1.0E-8;
      bdf.AbsTolArray[1] = 1.0E-8;

      for (int i = -4; i < 5; i++)
      {
        double time = Altaxo.Calc.RMath.Pow(10, i);
        y = bdf.Solve(time);
        var y0_expected = C1 * Math.Exp(lambda1 * time) + C2 * Math.Exp(lambda2 * time);
        var y1_expected = C1 * Math.Exp(lambda1 * time) - C2 * Math.Exp(lambda2 * time);

        AssertEx.Equal(y0_expected, y[0], 1E-4 * y0_expected + 1E-7);
        AssertEx.Equal(y1_expected, y[1], 1E-4 * y1_expected + 1E-7);
      }
    }
  }
}
