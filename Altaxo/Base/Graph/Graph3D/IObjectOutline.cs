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

using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	public class RectangularObjectOutline : IObjectOutline
	{
		private Matrix4x3 _transformation;
		private RectangleD3D _rectangle;

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

		public bool IsHittedBy(HitTestPointData hitData)
		{
			double z;
			return hitData.IsHit(_rectangle, _transformation, out z);
		}
	}

	public class MultipleRectangularObjectOutlines : IObjectOutline
	{
		private RectangularObjectOutline[] _outlines;

		public MultipleRectangularObjectOutlines(IEnumerable<RectangularObjectOutline> outlines, Matrix4x3 localToWorldTransformation)
		{
			_outlines = outlines.ToArray();
			// Replace the original outline object with new one that contain the transformation from local (layer) to world coordinates (root layer).
			for (int i = 0; i < _outlines.Length; ++i)
			{
				if (null != _outlines[i])
					_outlines[i] = _outlines[i].WithAdditionalTransformation(localToWorldTransformation);
			}
		}

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

		public bool IsHittedBy(HitTestPointData hitData)
		{
			foreach (var outline in _outlines)
				if (outline.IsHittedBy(hitData))
					return true;

			return false;
		}
	}

	public class MultiRectangularObjectOutline : IObjectOutline
	{
		private Matrix4x3 _transformation;
		private RectangleD3D[] _rectangles;

		public MultiRectangularObjectOutline(IEnumerable<RectangleD3D> rectangles, Matrix4x3 transformation)
		{
			if (null == rectangles)
				throw new ArgumentNullException(nameof(rectangles));

			_rectangles = rectangles.ToArray();

			if (_rectangles.Length == 0)
				throw new ArgumentNullException(nameof(rectangles) + " yields no entries");

			_transformation = transformation;
		}

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

		public bool IsHittedBy(HitTestPointData hitData)
		{
			double z;
			foreach (var rect in _rectangles)
			{
				if (hitData.IsHit(rect, _transformation, out z))
					return true;
			}
			return false;
		}
	}

	public class PolylineObjectOutline : IObjectOutline
	{
		private Matrix4x3 _transformation;

		private PointD3D[] _points;
		private double _thickness1By2;
		private double _thickness2By2;

		public PolylineObjectOutline(double thickness1, double thickness2, IEnumerable<PointD3D> points, Matrix4x3 localToWorldTransformation)
		{
			_thickness1By2 = thickness1 * 0.55;
			_thickness2By2 = thickness2 * 0.55;
			_points = points.ToArray();
			_transformation = localToWorldTransformation;
		}

		public IEnumerable<LineD3D> AsLines
		{
			get
			{
				if (null == _points || _points.Length < 2)
					yield break;

				PointD3D prevPoint = PointD3D.Empty;
				bool prevPointIsValid = false;
				foreach (var tp in Math3D.GetPolylinePointsWithWestAndNorth(_points))
				{
					if (prevPointIsValid)
					{
						var ne = _thickness1By2 * tp.Item2 + _thickness2By2 * tp.Item3;
						var se = _thickness1By2 * tp.Item2 - _thickness2By2 * tp.Item3;
						var nw = -_thickness1By2 * tp.Item2 + _thickness2By2 * tp.Item3;
						var sw = -_thickness1By2 * tp.Item2 - _thickness2By2 * tp.Item3;

						yield return new LineD3D(_transformation.Transform(prevPoint + ne), _transformation.Transform(tp.Item1 + ne));
						yield return new LineD3D(_transformation.Transform(prevPoint + se), _transformation.Transform(tp.Item1 + se));
						yield return new LineD3D(_transformation.Transform(prevPoint + nw), _transformation.Transform(tp.Item1 + nw));
						yield return new LineD3D(_transformation.Transform(prevPoint + sw), _transformation.Transform(tp.Item1 + sw));
					}

					prevPoint = tp.Item1;
					prevPointIsValid = true;
				}
			}
		}

		public bool IsHittedBy(HitTestPointData hitData)
		{
			throw new NotImplementedException();
		}
	}

	public class MultipleSingleLinesObjectOutline : IObjectOutline
	{
		private Matrix4x3 _transformation;

		private LineD3D[] _lines;
		private double _thickness1By2;
		private double _thickness2By2;

		public MultipleSingleLinesObjectOutline(double thickness1, double thickness2, IEnumerable<LineD3D> lines, Matrix4x3 localToWorldTransformation)
		{
			_thickness1By2 = thickness1 * 0.55;
			_thickness2By2 = thickness2 * 0.55;
			_lines = lines.ToArray();
			_transformation = localToWorldTransformation;
		}

		public IEnumerable<LineD3D> AsLines
		{
			get
			{
				if (null == _lines || _lines.Length < 1)
					yield break;

				foreach (var line in _lines)
				{
					var tp = Math3D.GetWestNorthVectors(line);

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

		public bool IsHittedBy(HitTestPointData hitData)
		{
			throw new NotImplementedException();
		}
	}
}