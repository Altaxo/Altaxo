#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo
{
  /// <summary>
  /// Fix for TimeSpan: TimeSpan.FromSeconds is rounding the resulting time span to integer milliseconds.
  /// </summary>
  public static class TimeSpanExtensions
  {
    private readonly static double TimeSpanMaxInSeconds = TimeSpan.MaxValue.Ticks / (double)TimeSpan.TicksPerSecond;
    private readonly static double TimeSpanMinInSeconds = TimeSpan.MinValue.Ticks / (double)TimeSpan.TicksPerSecond;

    /// <summary>
    /// Constructs a TimeSpan from the provided seconds exactly (without rounding to milliseconds).
    /// </summary>
    /// <param name="seconds">The seconds.</param>
    /// <returns></returns>
    public static TimeSpan FromSecondsAccurate(double seconds)
    {
      if (double.IsNaN(seconds))
        throw new ArgumentOutOfRangeException("Argument 'seconds' is Not a Number (NaN)");
      else if (seconds >= TimeSpanMaxInSeconds)
        return TimeSpan.MaxValue;
      else if (seconds <= TimeSpanMinInSeconds)
        return TimeSpan.MinValue;
      else
        return TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * seconds));
    }

    public static TimeSpan SafeAddition(this TimeSpan x, TimeSpan y)
    {
      double result = x.Ticks + (double)y.Ticks;

      if (result >= TimeSpan.MaxValue.Ticks)
        return TimeSpan.MaxValue;
      else if (result <= TimeSpan.MinValue.Ticks)
        return TimeSpan.MinValue;
      else
        return x + y;
    }

    public static TimeSpan SafeAddition(this TimeSpan x, double y)
    {
      double result = (double)x.Ticks + y * TimeSpan.TicksPerSecond;

      if (result >= TimeSpan.MaxValue.Ticks)
        return TimeSpan.MaxValue;
      else if (result <= TimeSpan.MinValue.Ticks)
        return TimeSpan.MinValue;
      else
        return TimeSpan.FromTicks((long)result);
    }

    public static TimeSpan Max(TimeSpan x, TimeSpan y)
    {
      return x > y ? x : y;
    }

    public static TimeSpan Min(TimeSpan x, TimeSpan y)
    {
      return x < y ? x : y;
    }
  }
}
