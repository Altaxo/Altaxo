#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
    public double RX;
    public double RY;
    public double RZ;

    public Logical3D(double rx, double ry, double rz)
    {
      RX = rx;
      RY = ry;
      RZ = rz;
    }
    public Logical3D(double rx, double ry)
    {
      RX = rx;
      RY = ry;
      RZ = 0;
    }


    public Logical3D InterpolateTo(Logical3D to, double t)
    {
      return new Logical3D
        (
        this.RX+t*(to.RX-this.RX),
        this.RY+t*(to.RY-this.RY),
        this.RZ+t*(to.RZ-this.RZ)
        );
    }

    /// <summary>
    /// Returns true if one of the three member variables RX, RY, or RZ has the value NaN.
    /// </summary>
    public bool IsNaN
    {
      get { return double.IsNaN(RX) || double.IsNaN(RY) || double.IsNaN(RZ); }
    }

    public static Logical3D Interpolate(Logical3D from, Logical3D to, double t)
    {
      return new Logical3D
        (
        from.RX + t * (to.RX - from.RX),
        from.RY + t * (to.RY - from.RY),
        from.RZ + t * (to.RZ - from.RZ)
        );
    }

    public static Logical3D operator +(Logical3D r, Logical3D s)
    {
      return new Logical3D(r.RX + s.RX, r.RY + s.RY, r.RZ + s.RZ);
    }

  }
}
