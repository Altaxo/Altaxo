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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.Primitives
{
	public class SweepPath3D : ISweepPath3D
	{
		private List<PointD3D> _points = new List<PointD3D>();
		private List<bool> _isSharpTransition = new List<bool>();

		public SweepPath3D(PointD3D p1, PointD3D p2)
		{
			_points.Add(p1);
			_points.Add(p2);
		}

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

		public PointD3D GetPoint(int idx)
		{
			return _points[idx];
		}

		public IList<PointD3D> Points
		{
			get
			{
				return _points;
			}
		}

		public int Count
		{
			get
			{
				return _points.Count;
			}
		}

		public bool IsTransitionFromIdxToNextIdxSharp(int idx)
		{
			return idx < _isSharpTransition.Count ? _isSharpTransition[idx] : false;
		}
	}
}