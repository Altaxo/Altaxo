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

namespace Altaxo.Graph3D.Primitives
{
	public class StraightLineSweepPath3D : ISweepPath3D
	{
		private PointD3D _p0, _p1;

		public StraightLineSweepPath3D(PointD3D p0, PointD3D p1)
		{
			_p0 = p0;
			_p1 = p1;
		}

		public int Count
		{
			get
			{
				return 2;
			}
		}

		public PointD3D GetPoint(int idx)
		{
			switch (idx)
			{
				case 0:
					return _p0;
					break;

				case 1:
					return _p1;

				default:
					throw new ArgumentOutOfRangeException(nameof(idx));
			}
		}

		public bool IsTransitionFromIdxToNextIdxSharp(int idx)
		{
			return true;
		}
	}
}