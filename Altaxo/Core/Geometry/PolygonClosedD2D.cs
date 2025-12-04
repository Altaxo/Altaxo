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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Implementation of a closed polygon in 2D space.
  /// </summary>
  public class PolygonClosedD2D
  {
    /// <summary>
    /// The points that form the closed polygon.
    /// </summary>
    protected PointD2D[] _points;

    /// <summary>
    /// The points of the polygon which are sharp points.
    /// </summary>
    protected HashSet<PointD2D> _sharpPoints;

    /// <summary>
    /// Indicates whether this polygon is a hole.
    /// </summary>
    protected bool? _isHole;

    /// <summary>
    /// Gets or sets the parent of this polygon.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    public PolygonClosedD2D? Parent { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonClosedD2D"/> class with points and sharp points.
    /// </summary>
    /// <param name="points">The points that form the polygon.</param>
    /// <param name="sharpPoints">The set of sharp points.</param>
    public PolygonClosedD2D(PointD2D[] points, HashSet<PointD2D> sharpPoints)
    {
      _points = points;
      _sharpPoints = sharpPoints;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolygonClosedD2D"/> class by scaling a template polygon.
    /// </summary>
    /// <param name="template">The template polygon.</param>
    /// <param name="scale">The scale factor.</param>
    public PolygonClosedD2D(PolygonClosedD2D template, double scale)
    {
      _points = template._points.Select(p => new PointD2D(p.X * scale, p.Y * scale)).ToArray();
      _sharpPoints = new HashSet<PointD2D>(template._sharpPoints.Select(p => new PointD2D(p.X * scale, p.Y * scale)));
    }

    /// <summary>
    /// Constructs a new closed polygon from the provided points. It is assumed that no point is sharp.
    /// </summary>
    /// <param name="points">The points that construct the polygon.</param>
    /// <returns>The closed polygon.</returns>
    public static PolygonClosedD2D FromPoints(IEnumerable<PointD2D> points)
    {
      return new PolygonClosedD2D(points.ToArray(), new HashSet<PointD2D>());
    }

    /// <summary>
    /// Gets the points that form the closed polygon.
    /// </summary>
    /// <value>
    /// The points.
    /// </value>
    public PointD2D[] Points { get { return _points; } }

    /// <summary>
    /// Gets the points of the polygon which are sharp points. Points of the polygon which are not in this set are considered to be soft points.
    /// </summary>
    /// <value>
    /// The sharp points.
    /// </value>
    public HashSet<PointD2D> SharpPoints { get { return _sharpPoints; } }

    /// <summary>
    /// Gets or sets a value indicating whether this polygon is a hole. In this case it belongs to another, outer, polygon.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is a hole; otherwise, <c>false</c>.
    /// </value>
    /// <exception cref="System.NotImplementedException">Thrown if the value has not been set.</exception>
    public bool IsHole
    {
      get
      {
        if (!_isHole.HasValue)
        {
          throw new NotImplementedException();
          //return PolygonHelper.GetPolygonArea(_points) > 0;
        }
        else
        {
          return _isHole.Value;
        }
      }
      set
      {
        _isHole = value;
      }
    }
  }
}
