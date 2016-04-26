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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.LineCaps
{
#if false
	public class Round : ILineCap
	{
		public double BaseInset
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Adds the specified add position and normal.
		/// </summary>
		/// <param name="AddPositionAndNormal">The procedure to add a vertex specifying position and normal.</param>
		/// <param name="AddIndices">The procedure to add a triangle by specifying vertex indices.</param>
		/// <param name="vertexIndexOffset">The vertex index offset.</param>
		/// <param name="lineReferencePoint">The line reference point. This is either the start or the end of the line to be capped.</param>
		/// <param name="forward">The forward vector. Always points in direction of the line (start to end).</param>
		/// <param name="e">The east vector.</param>
		/// <param name="n">The north vector.</param>
		/// <param name="crossSection">The cross section of the line.</param>
		/// <param name="contourPositions">The contour positions at the lineReferencePoint, if known. If providing null, this points will be calculated.</param>
		public void Add(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int> AddIndices,
			ref int vertexIndexOffset,
			bool isStartCap,
			PointD3D lineReferencePoint,
			VectorD3D forward,
			VectorD3D e,
			VectorD3D n,
			ICrossSectionOfLine crossSection,
			PointD3D[] contourPositions)
		{
			var crossSectionVertexCount = crossSection.NumberOfVertices;
			var crossSectionNormalCount = crossSection.NumberOfNormals;

			if (null == contourPositions)
			{
				var matrix = Math3D.Get2DProjectionToPlane(e, n, lineReferencePoint);
				contourPositions = new PointD3D[crossSectionVertexCount];
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					contourPositions[i] = matrix.Transform(crossSection.Vertices(i));
				}
			}

			// and now the cap

			if (isStartCap)
				forward = -forward;

			int currIndex = vertexIndexOffset;

			// Add the midpoint
			// add the middle point of the end cap and the normal of the end cap
			AddPositionAndNormal(lineReferencePoint, forward);
			++currIndex;

			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				AddPositionAndNormal(contourPositions[i], forward);
				if (crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(contourPositions[i], forward);
				}

				if (isStartCap)
				{
					AddIndices(
						currIndex - 1, // mid point of the end cap
						currIndex + j,
						currIndex + (1 + j) % crossSectionNormalCount);
				}
				else
				{
					AddIndices(
					currIndex + j,
					currIndex - 1, // mid point of the end cap
					currIndex + (1 + j) % crossSectionNormalCount);
				}
			}

			currIndex += crossSectionNormalCount;

			vertexIndexOffset = currIndex;
		}
	}
#endif
}