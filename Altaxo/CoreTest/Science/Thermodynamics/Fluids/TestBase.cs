#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Altaxo.Science.Thermodynamics.Fluids;
using Xunit;

namespace Altaxo.Science.Thermodynamics.Fluids
{
  public class TestBase
  {
    public static bool IsInToleranceLevel(double expected, double actual, double relativeError, double absoluteError)
    {
      var diff = Math.Abs(expected * relativeError) + Math.Abs(absoluteError);
      return Math.Abs(expected - actual) <= diff;
    }

    public static double GetAllowedError(double expected, double relativeError, double absoluteError)
    {
      return Math.Abs(expected * relativeError) + Math.Abs(absoluteError);
    }

    public static double GetRelativeErrorBetween(double x, double y)
    {
      var min = Math.Min(Math.Abs(x), Math.Abs(y));

      if (double.IsNaN(min) || double.IsInfinity(min))
        return double.PositiveInfinity;
      else if (min == 0)
        return x == y ? 0 : double.PositiveInfinity;
      else
        return Math.Abs(x - y) / min;
    }
  }
}
