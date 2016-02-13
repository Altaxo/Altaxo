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
		/// Appends an additional transformation to the object outline.
		/// </summary>
		/// <param name="transformation">The additional transformation.</param>
		void AppendTransformation(Matrix4x3 transformation);

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

		public void AppendTransformation(Matrix4x3 transformation)
		{
			_transformation.AppendTransform(transformation);
		}

		public bool IsHittedBy(HitTestPointData hitData)
		{
			double z;
			return hitData.IsHit(_rectangle, _transformation, out z);
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

		public void AppendTransformation(Matrix4x3 transformation)
		{
			_transformation.AppendTransform(transformation);
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
}