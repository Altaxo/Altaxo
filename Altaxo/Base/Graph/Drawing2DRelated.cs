#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
  /// <summary>
  /// Drawing2DRelated contains static methods related to mathematics and helper
  /// functions for classed from the System.Drawing namespace.
  /// </summary>
  public class Drawing2DRelated
  {
    private Drawing2DRelated()
    {
    }

    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="p1">First point.</param>
    /// <param name="p2">Second point.</param>
    /// <returns>The distance between points p1 and p2.</returns>
    public static double Distance(PointF p1, PointF p2)
    {
      double x = p1.X - p2.X;
      double y = p1.Y - p2.Y;
      return Math.Sqrt(x*x+y*y);
    }
  }
}
