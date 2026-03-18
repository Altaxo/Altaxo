#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Altaxo.Science.Spectroscopy.EnsembleProcessing;
using Xunit;

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public class SpikeRemovalByEnsembleStatisticsTests
  {
    private (int numberOfSpectrum, int firstPoint, int lastPoint)[] ExpectedPoints = new[]
    {
      (416, 935, 935),
(417, 935, 935),
(566, 850, 851),
(661, 198, 199),
(701, 214, 217),
(702, 214, 217),
(832, 126, 130),
(833, 126, 130),
(969, 1321, 1323),
(970, 1322, 1323),
(1129, 1064, 1064),
(1194, 1272, 1273),
(1195, 1273, 1273),
/* spectrum 1257 contains a unusual high fluorescence at the end
(1257, 1101, 1101),
(1257, 1122, 1123),
(1257, 1221, 1221),
(1257, 1223, 1223),
(1257, 1225, 1225),
(1257, 1231, 1231),
(1257, 1259, 1260),
(1257, 1264, 1264),
(1257, 1269, 1269),
(1257, 1271, 1273),
(1257, 1287, 1287),
(1257, 1292, 1292),
(1257, 1301, 1302),
(1257, 1311, 1312),
(1257, 1316, 1319),
(1257, 1328, 1328),
(1257, 1333, 1333),
(1257, 1338, 1338),
(1257, 1343, 1343),
(1257, 1346, 1346),
(1257, 1366, 1366),
(1257, 1377, 1378),
(1257, 1384, 1384),
(1257, 1386, 1386),
(1257, 1403, 1403),
(1257, 1409, 1409),
(1257, 1421, 1421),
(1257, 1423, 1423),
(1257, 1452, 1452),
(1257, 1503, 1503),
(1257, 1517, 1517),
(1257, 1532, 1532),
(1257, 1570, 1570),
*/
(1304, 1114, 1114),
(1305, 1114, 1114),
(1440, 1454, 1454),
(1441, 1454, 1455),
(1497, 263, 263),
(1498, 263, 263),
(1506, 298, 298),
(1507, 298, 298),
(1739, 983, 983),
(1740, 983, 983),
(1764, 453, 453),
(1860, 290, 291),
(1860, 327, 328),
(1861, 328, 328),
(1906, 299, 299),
(2007, 939, 939),
(2031, 1210, 1211),
(2032, 1209, 1211),
(2205, 727, 727),
(2441, 1485, 1485),
(2442, 1484, 1486),
(2480, 153, 153),
(2481, 153, 153),
    };


    [Fact]
    public void Test_AllExpectedPlacesArePatched()
    {
      var method = new SpikeRemovalByEnsembleStatistics() { NumberOfSigmas = 10, MaximalWidth = 5, EliminateNegativeSpikes = false };

      var (x, y) = TestFiles.ReadTestFileXYMatrix("EnsembleOf2500Spectra.txt");

      var (xNew, yNew, _, _) = method.Execute(x, y, null);

      Assert.False(object.ReferenceEquals(y, yNew));

      foreach (var (row, firstPoint, lastPoint) in ExpectedPoints)
      {
        for (int i = firstPoint; i <= lastPoint; i++)
        {
          AssertEx.Greater(y[row, i], yNew[row, i], $"point[{row}, {i}] not removed.");
        }
      }
    }
  }
}
