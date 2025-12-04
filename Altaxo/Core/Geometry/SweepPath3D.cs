#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Represents a sweep path in 3D space, consisting of multiple points and transitions (sharp or soft) between them.
  /// </summary>
  public class SweepPath3D : IPolylineD3D
  {
    /// <summary>
    /// The points of the sweep path.
    /// </summary>
    private List<PointD3D> _points = new List<PointD3D>();
    /// <summary>
    /// Indicates whether the transition to the next point is sharp.
    /// </summary>
    private List<bool> _isSharpTransition = new List<bool>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SweepPath3D"/> class with two points.
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    public SweepPath3D(PointD3D p1, PointD3D p2)
    {
      _points.Add(p1);
      _points.Add(p2);
    }

    /// <summary>
    /// Adds a point to the sweep path and sets whether the transition to the next point is sharp.
    /// </summary>
    /// <param name="point">The point to add.</param>
    /// <param name="isNextTransitionSharp">True if the transition to the next point is sharp; otherwise, false.</param>
    public void AddPoint(PointD3D point, bool isNextTransitionSharp)
    {
      var idx = _points.Count;

      _points.Add(point);

      var count = _isSharpTransition.Count;
      if (idx == count)
      {
        _isSharpTransition.Add(isNextTransitionSharp);
      }
      else if (idx < count)
      {
        _isSharpTransition[idx] = isNextTransitionSharp;
      }
      else
      {
        for (int i = count; i < idx; ++i)
          _isSharpTransition.Add(false);
        _isSharpTransition[idx] = isNextTransitionSharp;
      }
    }

    /// <inheritdoc/>
    public PointD3D GetPoint(int idx)
    {
      return _points[idx];
    }

    /// <inheritdoc/>
    public IList<PointD3D> Points
    {
      get
      {
        return _points;
      }
    }

    /// <inheritdoc/>
    IList<PointD3D> IPolylineD3D.Points
    {
      get
      {
        return _points;
      }
    }

    /// <inheritdoc/>
    public int Count
    {
      get
      {
        return _points.Count;
      }
    }

    /// <inheritdoc/>
    public bool IsTransitionFromIdxToNextIdxSharp(int idx)
    {
      return idx < _isSharpTransition.Count ? _isSharpTransition[idx] : false;
    }

    /// <inheritdoc/>
    public double TotalLineLength
    {
      get
      {
        double sum = 0;
        for (int i = 1; i < _points.Count; ++i)
          sum += (_points[i] - _points[i - 1]).Length;

        return sum;
      }
    }

    /// <inheritdoc/>
    public IPolylineD3D ShortenedBy(RADouble marginAtStart, RADouble marginAtEnd)
    {
      throw new NotImplementedException();
    }
  }
}
