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
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// Describes the outline of a 3D graphical object in order to show the selection markers.
  /// </summary>
  public interface IObjectOutline
  {
    /// <summary>
    /// Describes the object outline as set of lines.
    /// </summary>
    /// <value>
    /// Set of lines that describe the object outline.
    /// </value>
    IEnumerable<LineD3D> AsLines { get; }

    /// <summary>
    /// Determines whether this outline is hitted by the specified hit data.
    /// </summary>
    /// <param name="hitData">The hit data.</param>
    /// <returns>True if the outline is hitted, otherwise false.</returns>
    bool IsHittedBy(HitTestPointData hitData);
  }

  /// <summary>
  /// Designates the outline of a 3D graphical object in order to arrange it horizontally, vertically, or in depth direction.
  /// </summary>
  public interface IObjectOutlineForArrangements
  {
    /// <summary>
    /// Gets the bounds of the object in root layer coordinates.
    /// </summary>
    /// <returns>The bounds of the graphical object.</returns>
    RectangleD3D GetBounds();

    /// <summary>
    /// Gets the bounds of the object in root layer coordinates, but then with an additional transformation.
    /// </summary>
    /// <returns>The bounds of the object, with an additional transformation.</returns>
    RectangleD3D GetBounds(Matrix3x3 additionalTransformation);
  }

  /// <summary>
  /// Provides a simple wrapper that converts from an <see cref="IObjectOutline"/> to an <see cref="IObjectOutlineForArrangements"/>.
  /// This wrapper should be used sparse, since <see cref="IObjectOutline"/> is in most cases a little bit larger than <see cref="IObjectOutlineForArrangements"/>.
  /// </summary>
  /// <seealso cref="Altaxo.Graph.Graph3D.IObjectOutlineForArrangements" />
  public class ObjectOutlineForArrangementsWrapper : IObjectOutlineForArrangements
  {
    private IObjectOutline _outline;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectOutlineForArrangementsWrapper"/> class.
    /// </summary>
    /// <param name="outline">The outline to wrap.</param>
    public ObjectOutlineForArrangementsWrapper(IObjectOutline outline)
    {
      _outline = outline;
    }

    private IEnumerable<PointD3D> AsPoints()
    {
      foreach (var line in _outline.AsLines)
      {
        yield return line.P0;
        yield return line.P1;
      }
    }

    /// <inheritdoc/>
    public RectangleD3D GetBounds(Matrix3x3 transformation)
    {
      return RectangleD3D.NewRectangleIncludingAllPoints(AsPoints().Select(p => transformation.Transform(p)));
    }

    /// <inheritdoc/>
    public RectangleD3D GetBounds()
    {
      return RectangleD3D.NewRectangleIncludingAllPoints(AsPoints());
    }
  }

  /// <summary>
  /// Represents an outline based on a single transformed rectangle.
  /// </summary>
  public class RectangularObjectOutline : IObjectOutline
  {
    private Matrix4x3 _transformation;
    private RectangleD3D _rectangle;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangularObjectOutline"/> class.
    /// </summary>
    /// <param name="rectangle">The rectangle describing the outline.</param>
    /// <param name="transformation">The transformation to world coordinates.</param>
    public RectangularObjectOutline(RectangleD3D rectangle, Matrix4x3 transformation)
    {
      _rectangle = rectangle;
      _transformation = transformation;
    }

    /// <summary>
    /// Returns a new <see cref="RectangularObjectOutline"/> object, at wich the provided transformation is appended.
    /// Thus, when having a <see cref="RectangularObjectOutline"/> in object coordinates, by calling this function with the current
    /// localToWorldTransformation, one gets a <see cref="RectangularObjectOutline"/> in world coordinates.
    /// </summary>
    /// <param name="transformation">The transformation to append.</param>
    /// <returns>New <see cref="RectangularObjectOutline"/> object with the provided transformation appended.</returns>
    public RectangularObjectOutline WithAdditionalTransformation(Matrix4x3 transformation)
    {
      return new RectangularObjectOutline(_rectangle, _transformation.WithAppendedTransformation(transformation));
    }

    /// <inheritdoc/>
    public IEnumerable<LineD3D> AsLines
    {
      get
      {
        foreach (var line in _rectangle.Edges)
        {
          var p0 = _transformation.Transform(line.P0);
          var p1 = _transformation.Transform(line.P1);
          yield return new LineD3D(p0, p1);
        }
      }
    }

    /// <inheritdoc/>
    public bool IsHittedBy(HitTestPointData hitData)
    {
      return hitData.IsHit(_rectangle, _transformation, out var z);
    }
  }

  /// <summary>
  /// Represents multiple rectangular outlines.
  /// </summary>
  public class MultipleRectangularObjectOutlines : IObjectOutline
  {
    private RectangularObjectOutline[] _outlines;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleRectangularObjectOutlines"/> class.
    /// </summary>
    /// <param name="outlines">The outlines.</param>
    /// <param name="localToWorldTransformation">The local-to-world transformation.</param>
    public MultipleRectangularObjectOutlines(IEnumerable<RectangularObjectOutline> outlines, Matrix4x3 localToWorldTransformation)
    {
      _outlines = outlines.ToArray();
      // Replace the original outline object with new one that contain the transformation from local (layer) to world coordinates (root layer).
      for (int i = 0; i < _outlines.Length; ++i)
      {
        if (_outlines[i] is not null)
          _outlines[i] = _outlines[i].WithAdditionalTransformation(localToWorldTransformation);
      }
    }

    /// <inheritdoc/>
    public IEnumerable<LineD3D> AsLines
    {
      get
      {
        foreach (var outline in _outlines)
        {
          foreach (var line in outline.AsLines)
            yield return line;
        }
      }
    }

    /// <inheritdoc/>
    public bool IsHittedBy(HitTestPointData hitData)
    {
      foreach (var outline in _outlines)
        if (outline.IsHittedBy(hitData))
          return true;

      return false;
    }
  }

  /// <summary>
  /// Represents an outline based on multiple transformed rectangles.
  /// </summary>
  public class MultiRectangularObjectOutline : IObjectOutline
  {
    private Matrix4x3 _transformation;
    private RectangleD3D[] _rectangles;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRectangularObjectOutline"/> class.
    /// </summary>
    /// <param name="rectangles">The rectangles.</param>
    /// <param name="transformation">The transformation to world coordinates.</param>
    public MultiRectangularObjectOutline(IEnumerable<RectangleD3D> rectangles, Matrix4x3 transformation)
    {
      if (rectangles is null)
        throw new ArgumentNullException(nameof(rectangles));

      _rectangles = rectangles.ToArray();

      if (_rectangles.Length == 0)
        throw new ArgumentNullException(nameof(rectangles) + " yields no entries");

      _transformation = transformation;
    }

    /// <inheritdoc/>
    public IEnumerable<LineD3D> AsLines
    {
      get
      {
        foreach (var rect in _rectangles)
        {
          foreach (var line in rect.Edges)
          {
            var p0 = _transformation.Transform(line.P0);
            var p1 = _transformation.Transform(line.P1);
            yield return new LineD3D(p0, p1);
          }
        }
      }
    }

    /// <inheritdoc/>
    public bool IsHittedBy(HitTestPointData hitData)
    {
      foreach (var rect in _rectangles)
      {
        if (hitData.IsHit(rect, _transformation, out var z))
          return true;
      }
      return false;
    }
  }

  /// <summary>
  /// Represents an outline based on a polyline with thickness.
  /// </summary>
  public class PolylineObjectOutline : IObjectOutline
  {
    private Matrix4x3 _transformation;

    private PointD3D[] _points;
    private double _thickness1By2;
    private double _thickness2By2;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolylineObjectOutline"/> class.
    /// </summary>
    /// <param name="thickness1">The thickness in west-east direction.</param>
    /// <param name="thickness2">The thickness in north-south direction.</param>
    /// <param name="points">The polyline points.</param>
    /// <param name="localToWorldTransformation">The local-to-world transformation.</param>
    public PolylineObjectOutline(double thickness1, double thickness2, IEnumerable<PointD3D> points, Matrix4x3 localToWorldTransformation)
    {
      _thickness1By2 = thickness1 * 0.55;
      _thickness2By2 = thickness2 * 0.55;
      _points = points.ToArray();
      _transformation = localToWorldTransformation;
    }

    /// <inheritdoc/>
    public IEnumerable<LineD3D> AsLines
    {
      get
      {
        if (_points is null || _points.Length < 2)
          yield break;

        PointD3D prevPoint = PointD3D.Empty;
        bool prevPointIsValid = false;
        foreach (var tp in PolylineMath3D.GetPolylinePointsWithWestAndNorth(_points))
        {
          if (prevPointIsValid)
          {
            var ne = _thickness1By2 * tp.WestVector + _thickness2By2 * tp.NorthVector;
            var se = _thickness1By2 * tp.WestVector - _thickness2By2 * tp.NorthVector;
            var nw = -_thickness1By2 * tp.WestVector + _thickness2By2 * tp.NorthVector;
            var sw = -_thickness1By2 * tp.WestVector - _thickness2By2 * tp.NorthVector;

            yield return new LineD3D(_transformation.Transform(prevPoint + ne), _transformation.Transform(tp.Position + ne));
            yield return new LineD3D(_transformation.Transform(prevPoint + se), _transformation.Transform(tp.Position + se));
            yield return new LineD3D(_transformation.Transform(prevPoint + nw), _transformation.Transform(tp.Position + nw));
            yield return new LineD3D(_transformation.Transform(prevPoint + sw), _transformation.Transform(tp.Position + sw));
          }

          prevPoint = tp.Position;
          prevPointIsValid = true;
        }
      }
    }

    /// <inheritdoc/>
    public bool IsHittedBy(HitTestPointData hitData)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Represents an outline based on multiple individual lines with thickness.
  /// </summary>
  public class MultipleSingleLinesObjectOutline : IObjectOutline
  {
    private Matrix4x3 _transformation;

    private LineD3D[] _lines;
    private double _thickness1By2;
    private double _thickness2By2;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleSingleLinesObjectOutline"/> class.
    /// </summary>
    /// <param name="thickness1">The thickness in west-east direction.</param>
    /// <param name="thickness2">The thickness in north-south direction.</param>
    /// <param name="lines">The lines.</param>
    /// <param name="localToWorldTransformation">The local-to-world transformation.</param>
    public MultipleSingleLinesObjectOutline(double thickness1, double thickness2, IEnumerable<LineD3D> lines, Matrix4x3 localToWorldTransformation)
    {
      _thickness1By2 = thickness1 * 0.55;
      _thickness2By2 = thickness2 * 0.55;
      _lines = lines.ToArray();
      _transformation = localToWorldTransformation;
    }

    /// <inheritdoc/>
    public IEnumerable<LineD3D> AsLines
    {
      get
      {
        if (_lines is null || _lines.Length < 1)
          yield break;

        foreach (var line in _lines)
        {
          var tp = PolylineMath3D.GetWestNorthVectors(line);

          PointD3D prevPoint = line.P0;
          PointD3D currPoint = line.P1;

          var ne = _thickness1By2 * tp.Item1 + _thickness2By2 * tp.Item2;
          var se = _thickness1By2 * tp.Item1 - _thickness2By2 * tp.Item2;
          var nw = -_thickness1By2 * tp.Item1 + _thickness2By2 * tp.Item2;
          var sw = -_thickness1By2 * tp.Item1 - _thickness2By2 * tp.Item2;

          yield return new LineD3D(_transformation.Transform(prevPoint + ne), _transformation.Transform(currPoint + ne));
          yield return new LineD3D(_transformation.Transform(prevPoint + se), _transformation.Transform(currPoint + se));
          yield return new LineD3D(_transformation.Transform(prevPoint + nw), _transformation.Transform(currPoint + nw));
          yield return new LineD3D(_transformation.Transform(prevPoint + sw), _transformation.Transform(currPoint + sw));
        }
      }
    }

    /// <inheritdoc/>
    public bool IsHittedBy(HitTestPointData hitData)
    {
      throw new NotImplementedException();
    }
  }
}
