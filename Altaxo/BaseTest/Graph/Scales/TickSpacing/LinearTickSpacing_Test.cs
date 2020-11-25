#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Main;
using Xunit;

namespace AltaxoTest.Graph.Scales.Ticks
{
  /// <summary>
  /// Summary description for DoubleColumn_Test.
  /// </summary>
  
  public class LinearTickSpacing_Test
  {
    [Fact]
    public void T001_MajorTicksSpacedOne()
    {
      var scale = new LinearScale();
      var s = (LinearTickSpacing)scale.TickSpacing;

      s.OrgGrace = 0;
      s.EndGrace = 0;
      s.ZeroLever = 0;

      for (int tmt = 4; tmt <= 16; ++tmt)
      {
        s.TargetNumberOfMajorTicks = tmt;
        s.TargetNumberOfMinorTicks = 1;

        s.FinalProcessScaleBoundaries(0, tmt - 1, scale);

        var majorTicks = s.GetMajorTicks();

        Assert.NotNull(majorTicks);
        Assert.Equal(tmt, majorTicks.Length);

        for (int i = 0; i <= tmt - 1; ++i)
          Assert.Equal(majorTicks[i], i);
      }
    }

    [Fact]
    public void T002_TestZeroLever()
    {
      var scale = new LinearScale();
      var s = (LinearTickSpacing)scale.TickSpacing;

      s.OrgGrace = 0;
      s.EndGrace = 0;
      s.ZeroLever = 0;

      Altaxo.Data.AltaxoVariant org, end;

      s.ZeroLever = 0.25;

      for (int i = 0; i < 50; ++i)
      {
        org = i;
        end = i + 100; // Span now is 100

        s.PreProcessScaleBoundaries(ref org, ref end, true, true);
        Assert.Equal(i <= 25 ? 0 : i, org.ToDouble());

        Assert.Equal(i + 100, end.ToDouble());
      }
    }
  }
}
