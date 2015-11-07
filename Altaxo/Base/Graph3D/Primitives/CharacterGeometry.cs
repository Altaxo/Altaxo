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

namespace Altaxo.Graph3D.Primitives
{
	public class CharacterGeometry
	{
		private List<PolygonWithNormalsD2D> _characterContour;

		private IndexedTriangles _frontFace;

		public IList<PolygonWithNormalsD2D> CharacterContour { get { return _characterContour; } }

		public IndexedTriangles FrontFace { get { return _frontFace; } }

		public double FontSize { get; protected set; }
		public double LineSpacing { get; protected set; }
		public double BaseLine { get; protected set; }
		public double AdvanceWidth { get; protected set; }
		public double LeftSideBearing { get; protected set; }
		public double RightSideBearing { get; protected set; }

		public CharacterGeometry(List<PolygonWithNormalsD2D> characterContour, IndexedTriangles frontFace,
			double fontSize, double lineSpacing, double baseLine,
			double advanceWidth, double leftSideBearing, double rightSideBearing)
		{
			_characterContour = characterContour;
			_frontFace = frontFace;

			FontSize = fontSize;
			LineSpacing = lineSpacing;
			BaseLine = baseLine;
			AdvanceWidth = advanceWidth;
			LeftSideBearing = leftSideBearing;
			RightSideBearing = rightSideBearing;
		}
	}
}