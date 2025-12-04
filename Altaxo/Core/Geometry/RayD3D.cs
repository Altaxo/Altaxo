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

#nullable enable
namespace Altaxo.Geometry
{
  /// <summary>
  /// An infinite line in 3D space.
  /// </summary>
  public class Ray3D
  {
    /// <summary>
    /// The origin of the line (one arbitrary point at the line).
    /// </summary>
    private PointD3D _origin;

    /// <summary>
    /// The direction of the line.
    /// </summary>
    private VectorD3D _direction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ray3D"/> class.
    /// </summary>
    /// <param name="origin">The origin of the line (one arbitrary point at the line).</param>
    /// <param name="direction">The direction of the line.</param>
    public Ray3D(PointD3D origin, VectorD3D direction)
    {
      _origin = origin;
      _direction = direction;
    }

    /// <summary>
    /// Gets the origin, i.e. one point at the line.
    /// </summary>
    public PointD3D Origin { get { return _origin; } }

    /// <summary>
    /// Gets the direction of the line.
    /// </summary>
    public VectorD3D Direction { get { return _direction; } }
  }
}
