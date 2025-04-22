/*
 *  Copyright notice
 *  This code is licensed under GNU General Public License (GPLv3) and
 *  Creative Commons Attribution-ShareAlike 4.0 (CC-BY-SA 4.0).
 *  When using and/or modifying this program for scientific work
 *  and the paper on it has been published, please cite the paper:
 *  M. Schmid, D. Rath and U. Diebold,
 * 'Why and how Savitzky-Golay filters should be replaced',
 *  ACS Measurement Science Au, 2022
 *  Author: Michael Schmid, IAP/TU Wien, 2021.
 *  https://www.iap.tuwien.ac.at/www/surface/group/schmid
 *
 *  Translation to C# by Dr. Dirk Lellinger 2025
 */

using Xunit;

namespace Altaxo.Calc.Regression
{
  public class WhittakerHendersonSmoother_Test
  {

    private static readonly double[] _expectedResult = [
      0.194147534139252, 0.0197920890314065, 0.0218121524184484, 0.190341209009916,
      0.116801424179281, -0.189225146924159, -0.805345956948873, -0.491671916435109,
      1.40168591186754, 5.14957720962258, 7.7558636396397, 7.09847430398126,
      3.95959561060227, 0.692551176647803, -0.114399240831317];

    [Fact]
    public void WeightedSavitzkyGolaySmootherTest1()
    {
      double[] data = [0, 1, -2, 3, -4, 5, -6, 7, -8, 9, 10, 6, 3, 1, 0];   //arbitrary test data

      int degree = 6;
      int m = 7;
      var result = WhittakerHendersonSmoother.SmoothLikeSavitzkyGolay(data, degree, m);
      Assert.Equal(_expectedResult.Length, result.Length);
      for (int i = 0; i < _expectedResult.Length; ++i)
        AssertEx.Equal(_expectedResult[i], result[i], 1e-10);
    }

  }
}
