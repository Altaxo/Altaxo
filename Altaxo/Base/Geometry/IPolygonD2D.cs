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

using Altaxo.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a open or closed polygon in 2D space.
  /// </summary>
  public interface IPolygonD2D
  {
    /// <summary>
    /// Gets the points that form the closed polygon.
    /// </summary>
    /// <value>
    /// The points.
    /// </value>
    IList<PointD2D> Points { get; }

    /// <summary>
    /// Gets the points of the polygon which are sharp points. Points of the polygon which are not in this set are considered to be soft points.
    /// </summary>
    /// <value>
    /// The sharp points.
    /// </value>
    ISet<PointD2D> SharpPoints { get; }
  }
}
