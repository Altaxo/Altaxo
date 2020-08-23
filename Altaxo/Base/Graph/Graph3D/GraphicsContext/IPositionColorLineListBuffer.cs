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
  public interface IPositionColorLineListBuffer
  {
    /// <summary>
    /// Adds the specified vertex.
    /// </summary>
    /// <param name="x0">The x position of line start point.</param>
    /// <param name="y0">The y position of line start point.</param>
    /// <param name="z0">The z position of line start point.</param>
    /// <param name="x1">The x position of line end point.</param>
    /// <param name="y1">The y position of line end point.</param>
    /// <param name="z1">The z position of line end point.</param>
    /// <param name="r">The r color component.</param>
    /// <param name="g">The g color component.</param>
    /// <param name="b">The b color component.</param>
    /// <param name="a">The a color component.</param>
    void AddLine(double x0, double y0, double z0, double x1, double y1, double z1, float r, float g, float b, float a);
  }
}
