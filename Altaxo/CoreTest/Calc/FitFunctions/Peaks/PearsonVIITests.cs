#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  public class PearsonVIITests
  {
    [Fact]
    public void TestDerivatives()
    {
      double amp = 3;
      double pos = 7;
      double w = 5;
      double m = 11;

      var areaDerivs = new double[] { 10.852370479261061516, 0, 6.5114222875566369095, -0.060225693099807316307 };

      double[] pars = new double[] { amp, pos, w, m };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new PearsonVII();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = new DoubleMatrix(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaVariance, 1E-13, 1E-7);
      }
    }
  }
}
