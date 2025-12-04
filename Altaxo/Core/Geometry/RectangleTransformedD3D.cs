#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  /// Represents a rectangle in 3D space with an associated transformation matrix.
  /// </summary>
  public class RectangleTransformedD3D
  {
    /// <summary>
    /// The underlying rectangle in 3D space.
    /// </summary>
    private RectangleD3D _rectangle;
    /// <summary>
    /// The transformation matrix applied to the rectangle.
    /// </summary>
    private Matrix4x3 _transformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleTransformedD3D"/> class.
    /// </summary>
    /// <param name="rectangle">The rectangle in 3D space.</param>
    /// <param name="transformation">The transformation matrix to apply.</param>
    public RectangleTransformedD3D(RectangleD3D rectangle, Matrix4x3 transformation)
    {
      _rectangle = rectangle;
      _transformation = transformation;
    }
  }
}
