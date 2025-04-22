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
  public class WeightedSavitzkyGolaySmoother_Test
  {
    /* This constructor is only for testing the filter.  It can be removed.
    *  The edge kernel, kernels[0], should be: <br>
    *   [0.9860734984300393,    0.056918568996510654, -0.06931653363045043,   0.001217044253974356,
    *    0.033852366327283966,  0.012208144151913635, -0.013853157267840751, -0.01569839895394231,
    *   -0.0014813956478430153, 0.008387115758517773,  0.005938018439190397, -0.0011816807905541734,
    *   -0.00341113701958563,  -6.743302754188831E-4,  0.0010218772281337806] <br>
    *  The filtered data should be:
    */
    private static readonly double[] _expectedResult = [
      0.2267817407230225,  0.3803379776275339, -0.2542196669759636,  -0.16144537772877116,
      0.16108284817615762, 0.4525943769926024, -0.41288045376351673, -1.0937687611036997,
      0.6198930998271857,  4.862721666447915,   8.535294804032034,    7.223205439511203,
      3.3820361761910283, -0.14330976836859274, 0.3352794400491729];

    [Fact]
    public void WeightedSavitzkyGolaySmootherTest1()
    {
      double[] data = [0, 1, -2, 3, -4, 5, -6, 7, -8, 9, 10, 6, 3, 1, 0];   //arbitrary test data

      var weightType = WeightedSavitzkyGolaySmoother.WeightType.HANNSQR;
      int degree = 6;
      int m = 7;
      //var kernels = WeightedSavitzkyGolaySmoother.MakeKernels(weightType, degree, m);
      var result = WeightedSavitzkyGolaySmoother.Smooth(data, null, weightType, degree, m);
      Assert.Equal(_expectedResult.Length, result.Length);
      for (int i = 0; i < _expectedResult.Length; ++i)
        AssertEx.Equal(_expectedResult[i], result[i], 1e-10);
    }

  }
}
