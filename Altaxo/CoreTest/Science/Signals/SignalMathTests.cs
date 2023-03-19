#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Xunit;

namespace Altaxo.Science.Signals
{
  public class SignalMathTests
  {

    [Fact]
    public void TestGetIndicesOfZeroCrossings()
    {
      var signal = new double[5];
      List<int> zeroCrossings;

      // Going to zero and back again should not count as zero crossing 
      signal[0] = 1;
      signal[1] = 1;
      signal[2] = 0;
      signal[3] = 1;
      signal[4] = 0;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Empty(zeroCrossings);


      // Going to zero and back again should not count as zero crossing 
      signal[0] = 1;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = 1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Empty(zeroCrossings);

      // Zero at the beginning counts as zero crossing
      signal[0] = 0;
      signal[1] = 1;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = 0;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Single(zeroCrossings);

      // Zero at the counts as zero crossing
      signal[0] = 0;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 1;
      signal[4] = 0;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Single(zeroCrossings);

      // Going to zero and then to negative should count as zero crossing 
      signal[0] = 1;
      signal[1] = 0;
      signal[2] = -1;
      signal[3] = -2;
      signal[4] = -3;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Single(zeroCrossings);
      Assert.True(zeroCrossings[0] == 1);

      // Going to zero and then to negative should count as zero crossing 
      signal[0] = 1;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = -1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Single(zeroCrossings);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 4);

      signal[0] = -1;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = 1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Single(zeroCrossings);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 4);

      // Next signal has two zero crossings
      signal[0] = -1;
      signal[1] = 1;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = -1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Equal(2, zeroCrossings.Count);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 1);
      Assert.True(zeroCrossings[1] >= 1);
      Assert.True(zeroCrossings[1] < 4);


      signal[0] = 1;
      signal[1] = -1;
      signal[2] = 0;
      signal[3] = 0;
      signal[4] = 1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Equal(2, zeroCrossings.Count);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 1);
      Assert.True(zeroCrossings[1] >= 1);
      Assert.True(zeroCrossings[1] < 4);


      signal[0] = 1;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = -1;
      signal[4] = 1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Equal(2, zeroCrossings.Count);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 3);
      Assert.True(zeroCrossings[1] >= 3);
      Assert.True(zeroCrossings[1] < 4);


      signal[0] = -1;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 1;
      signal[4] = -1;
      zeroCrossings = SignalMath.GetIndicesOfZeroCrossings(signal);
      Assert.Equal(2, zeroCrossings.Count);
      Assert.True(zeroCrossings[0] >= 0);
      Assert.True(zeroCrossings[0] < 3);
      Assert.True(zeroCrossings[1] >= 3);
      Assert.True(zeroCrossings[1] < 4);

    }

    [Fact]
    public void TestGetIndicesOfExtrema()
    {
      var signal = new double[5];
      // Extrema at the start or end should not be included in the list
      signal[0] = 0;
      signal[1] = 1;
      signal[2] = 2;
      signal[3] = 3;
      signal[4] = 4;
      var (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Empty(min);
      Assert.Empty(max);

      signal[0] = 7;
      signal[1] = 6;
      signal[2] = 5;
      signal[3] = 4;
      signal[4] = 3;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Empty(min);
      Assert.Empty(max);

      signal[0] = 7;
      signal[1] = 7;
      signal[2] = 7;
      signal[3] = 7;
      signal[4] = 7;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Empty(min);
      Assert.Empty(max);

      signal[0] = -8;
      signal[1] = -7;
      signal[2] = -7;
      signal[3] = -7;
      signal[4] = -7;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Empty(min);
      Assert.Empty(max);

      signal[0] = -8;
      signal[1] = -7;
      signal[2] = -7;
      signal[3] = -7;
      signal[4] = -9;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Empty(min);
      Assert.Single(max);
      Assert.True(max[0] == 2);

      signal[0] = 7;
      signal[1] = 0;
      signal[2] = 1;
      signal[3] = 1;
      signal[4] = 0;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Single(min);
      Assert.Single(max);
      Assert.True(min[0] == 1);
      Assert.True(max[0] == 2);

      signal[0] = 7;
      signal[1] = 0;
      signal[2] = 0;
      signal[3] = 1;
      signal[4] = -9;
      (min, max) = SignalMath.GetIndicesOfExtrema(signal);
      Assert.Single(min);
      Assert.Single(max);
      Assert.True(min[0] == 1);
      Assert.True(max[0] == 3);
    }
  }
}
