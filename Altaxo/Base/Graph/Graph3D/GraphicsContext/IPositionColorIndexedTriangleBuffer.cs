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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.GraphicsContext
{
  /// <summary>
  /// Interface to a buffer that stores indexed triangle data consisting of position and color.
  /// </summary>
  public interface IPositionColorIndexedTriangleBuffer : IIndexedTriangleBuffer
  {
    /// <summary>
    /// Adds the specified vertex.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="r">The r color component.</param>
    /// <param name="g">The g color component.</param>
    /// <param name="b">The b color component.</param>
    /// <param name="a">The a color component.</param>
    void AddTriangleVertex(double x, double y, double z, float r, float g, float b, float a);
  }
}
