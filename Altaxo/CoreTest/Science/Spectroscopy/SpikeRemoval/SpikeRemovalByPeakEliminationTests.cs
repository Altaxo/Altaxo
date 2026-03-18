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

using Xunit;

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public class SpikeRemovalByPeakEliminationTests
  {

    [Fact]
    public void Test1()
    {
      var method = new SpikeRemovalByPeakElimination() { EliminateNegativeSpikes = false, MaximalWidth = 3 };

      foreach (var (name, firstPoint, lastPoint, x, y) in TestFiles.GetTestFiles())
      {
        var (xNew, yNew, _) = method.Execute(x, y, null);

        for (int i = firstPoint; i <= lastPoint; i++)
          AssertEx.Greater(y[i], yNew[i], $"{name}: point[{i}] not removed.");
      }
    }
  }
}
