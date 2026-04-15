#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Holds a triple of logical values to designate a location into a 3D coordinate system. Can
  /// also be used for 2D (with RZ=0).
  /// </summary>
  public struct Logical3D
  {
    /// <summary>
    /// The logical x-coordinate.
    /// </summary>
    public double RX;
    /// <summary>
    /// The logical y-coordinate.
    /// </summary>
    public double RY;
    /// <summary>
    /// The logical z-coordinate.
    /// </summary>
    public double RZ;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logical3D"/> struct.
    /// </summary>
    /// <param name="rx">The logical x-coordinate.</param>
    /// <param name="ry">The logical y-coordinate.</param>
    /// <param name="rz">The logical z-coordinate.</param>
    public Logical3D(double rx, double ry, double rz)
    {
      RX = rx;
      RY = ry;
      RZ = rz;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Logical3D"/> struct for a 2D coordinate system.
    /// </summary>
    /// <param name="rx">The logical x-coordinate.</param>
    /// <param name="ry">The logical y-coordinate.</param>
    public Logical3D(double rx, double ry)
    {
      RX = rx;
      RY = ry;
      RZ = 0;
    }

    /// <summary>
    /// Returns a copy with one coordinate replaced.
    /// </summary>
    /// <param name="axisNumber">The axis number.</param>
    /// <param name="r">The new coordinate value.</param>
    /// <returns>A copy with the specified coordinate replaced.</returns>
    public Logical3D WithR(int axisNumber, double r)
    {
      var result = this;
      result.SetR(axisNumber, r);
      return result;
    }

    /// <summary>
    /// Gets the coordinate for the specified axis number.
    /// </summary>
    /// <param name="axisNumber">The axis number.</param>
    /// <returns>The coordinate value.</returns>
    public double GetR(int axisNumber)
    {
      switch (axisNumber)
      {
        case 0:
          return RX;

        case 1:
          return RY;

        case 2:
          return RZ;

        default:
          throw new ArgumentOutOfRangeException(nameof(axisNumber));
      }
    }

    /// <summary>
    /// Sets the coordinate for the specified axis number.
    /// </summary>
    /// <param name="axisNumber">The axis number.</param>
    /// <param name="r">The coordinate value.</param>
    public void SetR(int axisNumber, double r)
    {
      switch (axisNumber)
      {
        case 0:
          RX = r;
          break;

        case 1:
          RY = r;
          break;

        case 2:
          RZ = r;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(axisNumber));
      }
    }

    /// <summary>
    /// Gets the perpendicular axis number. Given a first and a second axis number, this gives the third axis number that is unequal to the first and the second.
    /// </summary>
    /// <param name="axisNumber1">The axis number1.</param>
    /// <param name="axisNumber2">The axis number2.</param>
    /// <returns>The third axis number.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Out of range [0,2]
    /// or
    /// Out of range [0,2]
    /// or
    /// Second axis number is equal to first axis number
    /// </exception>
    public static int GetPerpendicularAxisNumber(int axisNumber1, int axisNumber2)
    {
      if (axisNumber1 < 0 || axisNumber1 > 2)
        throw new ArgumentOutOfRangeException(nameof(axisNumber1), "Out of range [0,2]");
      if (axisNumber2 < 0 || axisNumber2 > 2)
        throw new ArgumentOutOfRangeException(nameof(axisNumber2), "Out of range [0,2]");
      if (axisNumber1 == axisNumber2)
        throw new ArgumentOutOfRangeException(nameof(axisNumber2), "Second axis number is equal to first axis number");

      switch (axisNumber1)
      {
        case 0:
          return axisNumber2 == 1 ? 2 : 1;

        case 1:
          return axisNumber2 == 0 ? 2 : 0;

        case 2:
          return axisNumber2 == 0 ? 1 : 0;

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns the coordinate with index idx.
    /// </summary>
    /// <param name="idx">Number of the coordinate.</param>
    /// <returns>If idx is 0, 1, or 2, then RX, RY, or RZ is returned, respectively. Otherwise an <see cref="ArgumentOutOfRangeException"/> is thrown.</returns>
    public double GetCoordinate(int idx)
    {
      switch (idx)
      {
        case 0:
          return RX;

        case 1:
          return RY;

        case 2:
          return RZ;

        default:
          throw new ArgumentOutOfRangeException("idx out of range, it should be in the range [0,2]");
      }
    }

    /// <summary>
    /// Sets the coordinate with index idx.
    /// </summary>
    /// <param name="idx">Number of the coordinate.</param>
    /// <param name="value">Value of the coordinate to set.</param>
    public void SetCoordinate(int idx, double value)
    {
      switch (idx)
      {
        case 0:
          RX = value;
          break;

        case 1:
          RY = value;
          break;

        case 2:
          RZ = value;
          break;

        default:
          throw new ArgumentOutOfRangeException("idx out of range, it should be in the range [0,2]");
      }
    }

    /// <summary>
    /// Gets/sets the coordinate with index idx.
    /// </summary>
    /// <param name="idx">Number of the coordinate.</param>
    /// <returns>If idx is 0, 1, or 2, then RX, RY, or RZ is accessed, respectively. Otherwise an <see cref="ArgumentOutOfRangeException"/> is thrown.</returns>
    public double this[int idx]
    {
      get
      {
        return GetCoordinate(idx);
      }
      set
      {
        SetCoordinate(idx, value);
      }
    }

    /// <summary>
    /// Interpolates from this value to another logical coordinate.
    /// </summary>
    /// <param name="to">The target coordinate.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <returns>The interpolated coordinate.</returns>
    public Logical3D InterpolateTo(Logical3D to, double t)
    {
      return new Logical3D
        (
        RX + t * (to.RX - RX),
        RY + t * (to.RY - RY),
        RZ + t * (to.RZ - RZ)
        );
    }

    /// <summary>
    /// Returns true if one of the three member variables RX, RY, or RZ has the value NaN.
    /// </summary>
    public bool IsNaN
    {
      get { return double.IsNaN(RX) || double.IsNaN(RY) || double.IsNaN(RZ); }
    }

    /// <summary>
    /// Interpolates between two logical coordinates.
    /// </summary>
    /// <param name="from">The start coordinate.</param>
    /// <param name="to">The target coordinate.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <returns>The interpolated coordinate.</returns>
    public static Logical3D Interpolate(Logical3D from, Logical3D to, double t)
    {
      return new Logical3D
        (
        from.RX + t * (to.RX - from.RX),
        from.RY + t * (to.RY - from.RY),
        from.RZ + t * (to.RZ - from.RZ)
        );
    }

    /// <summary>
    /// Adds two logical coordinates.
    /// </summary>
    /// <param name="r">The first logical coordinate.</param>
    /// <param name="s">The second logical coordinate.</param>
    /// <returns>The component-wise sum of <paramref name="r"/> and <paramref name="s"/>.</returns>
    public static Logical3D operator +(Logical3D r, Logical3D s)
    {
      return new Logical3D(r.RX + s.RX, r.RY + s.RY, r.RZ + s.RZ);
    }
  }
}
