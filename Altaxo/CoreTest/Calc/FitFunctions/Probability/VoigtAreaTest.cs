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

using Xunit;

namespace Altaxo.Calc.FitFunctions.Probability
{
  public class VoigtAreaTest
  {
    [Fact]
    public void TestHeight()
    {
      var v = new VoigtArea();

      const double areaG = 13;
      const double positionG = 77;
      var parameters = new double[] { areaG, positionG, 0, 0 };

      double sigma = 1;
      double gamma;
      parameters[2] = sigma;
      int i;
      double d;
      for (i = 0, d = -1; d < 1; ++i, d += 1 / 64.0)
      {
        gamma = (1 + d) / (1 - d);
        parameters[3] = gamma;

        var (pos, _, area, _, height, _, fwhm, _) = v.GetPositionAreaHeightFwhmFromSinglePeakParameters(parameters, null);

        AssertEx.AreEqual(areaG, area, 0, 1E-14);
        AssertEx.AreEqual(positionG, pos, 0, 1E-14);

        var heightExpected = Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, sigma, gamma) * areaG;
        AssertEx.AreEqual(heightExpected, height, 0, 1E-7);

        var fwhmExpected = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma);
        AssertEx.AreEqual(fwhmExpected, fwhm, 0, Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximumApproximationMaximalRelativeError);
      }

      gamma = 1;
      parameters[3] = gamma;
      for (i = 0, d = -1; d < 1; ++i, d += 1 / 64.0)
      {
        sigma = (1 + d) / (1 - d);
        parameters[2] = sigma;

        var (pos, _, area, _, height, _, fwhm, _) = v.GetPositionAreaHeightFwhmFromSinglePeakParameters(parameters, null);

        AssertEx.AreEqual(areaG, area, 0, 1E-14);
        AssertEx.AreEqual(positionG, pos, 0, 1E-14);

        var heightExpected = Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, sigma, gamma) * areaG;
        AssertEx.AreEqual(heightExpected, height, 0, 1E-7);

        var fwhmExpected = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma);
        AssertEx.AreEqual(fwhmExpected, fwhm, 0, Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximumApproximationMaximalRelativeError);
      }

    }
  }
}
