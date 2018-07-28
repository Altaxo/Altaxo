#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  public static class RectangleD2DExtensions
  {
    /// <summary>
    /// Calculates the dimensions of the greatest (by area) rectangle included in an outer rectangle, where the inner rectangle is rotated/sheared/scaled.
    /// </summary>
    /// <param name="outerRectangle">The outer rectangle.</param>
    /// <param name="sx">SX component of the transformation matrix that is applied to the inner rectangle.</param>
    /// <param name="rx">RX component of the transformation matrix that is applied to the inner rectangle.</param>
    /// <param name="ry">RY component of the transformation matrix that is applied to the inner rectangle.</param>
    /// <param name="sy">SY component of the transformation matrix that is applied to the inner rectangle.</param>
    /// <returns>The inner rectangle with the greatest area that fits (when transformed with the transformation elements) into the outer rectangle.
    /// The position of the returned rectangle is calculated so that it centers into the outer rectangle.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// X-Size of outer rectangle must be > 0
    /// or
    /// Y-Size of outer rectangle must be > 0
    /// </exception>
    public static RectangleD2D GetIncludedTransformedRectangle(this RectangleD2D outerRectangle, double sx, double rx, double ry, double sy)
    {
      PointD2D outerRectangleSize = outerRectangle.Size;

      if (!(outerRectangleSize.X > 0))
        throw new ArgumentOutOfRangeException("X-Size of outer rectangle must be > 0");
      if (!(outerRectangleSize.Y > 0))
        throw new ArgumentOutOfRangeException("Y-Size of outer rectangle must be > 0");

      double a = Math.Abs(sx);
      double b = Math.Abs(rx);
      double c = Math.Abs(ry);
      double d = Math.Abs(sy);

      double maxArea = 0;
      double sizeX = 0, sizeY = 0;
      double x, y, area;

      {
        // solution 1, which touches all walls
        double bcMad = b * c - a * d;
        if (bcMad != 0)
        {
          x = (b * outerRectangleSize.Y - d * outerRectangleSize.X) / bcMad;
          y = (c * outerRectangleSize.X - a * outerRectangleSize.Y) / bcMad;
          area = x * y;
          if (maxArea < area)
          {
            maxArea = area;
            sizeX = x;
            sizeY = y;
          }
        }
      }

      {
        // solution2 (which does not touch the left and right walls of the outer retangle
        var eps2 = outerRectangleSize.X - outerRectangleSize.Y * (b * c + a * d) / (2 * c * d);
        if (eps2 >= 0 && eps2 < outerRectangleSize.X)
        {
          area = outerRectangleSize.Y * outerRectangleSize.Y / (4 * c * d);
          x = outerRectangleSize.Y / (2 * c);
          y = outerRectangleSize.Y / (2 * d);
          if (maxArea < area)
          {
            maxArea = area;
            sizeX = x;
            sizeY = y;
          }
        }
      }

      {
        // solution3 (which does not touch the top and bottom walls of the outer rectangle
        var eps3 = outerRectangleSize.Y - outerRectangleSize.X * (b * c + a * d) / (2 * a * b);
        if (eps3 >= 0 && eps3 < outerRectangleSize.Y)
        {
          area = outerRectangleSize.X * outerRectangleSize.X / (4 * a * b);
          x = outerRectangleSize.X / (2 * a);
          y = outerRectangleSize.X / (2 * b);
          if (maxArea < area)
          {
            maxArea = area;
            sizeX = x;
            sizeY = y;
          }
        }
      }

      RectangleD2D innerRect = new RectangleD2D();
      innerRect.ExpandToInclude(new PointD2D(sx * sizeX + rx * sizeY, ry * sizeX + sy * sizeY));
      innerRect.ExpandToInclude(new PointD2D(sx * sizeX, ry * sizeX));
      innerRect.ExpandToInclude(new PointD2D(rx * sizeY, sy * sizeY));

      var outerMiddle = outerRectangle.CenterCenter;
      var innerMiddle = innerRect.CenterCenter;

      return new RectangleD2D((outerMiddle.X - innerMiddle.X), (outerMiddle.Y - innerMiddle.Y), sizeX, sizeY);
    }
  }
}
