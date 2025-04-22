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
  public class ModifiedSincSmoother_Test
  {
    private static readonly double[] _expectedResult = [0.1583588453161306,    0.11657466389491726, -0.09224721042380793, 0.031656885544917315,
      -0.054814729808335835, -0.054362188355910813, 0.5105482655952578, -0.5906786605713916,
      -1.2192869459451745,    5.286105202110525,   10.461619519603234,   6.82674246410578,
       2.4923674303784833,    1.0422038091960153,   0.032646599192913656];

    [Fact]
    public void TestModifiedSincSmoother()
    {
      bool isMS1 = false;
      int degree = 6;
      int m = 7;
      double[] data = [0, 1, -2, 3, -4, 5, -6, 7, -8, 9, 10, 6, 3, 1, 0]; //arbitrary test data

      var result = ModifiedSincSmoother.Smooth(data, isMS1, degree, m);

      Assert.Equal(_expectedResult.Length, result.Length);
      for (int i = 0; i < _expectedResult.Length; ++i)
        AssertEx.Equal(_expectedResult[i], result[i], 1e-10);
    }
  }
}
