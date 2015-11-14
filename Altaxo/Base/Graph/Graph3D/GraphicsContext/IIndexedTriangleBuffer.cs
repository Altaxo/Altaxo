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

namespace Altaxo.Graph.Graph3D.GraphicsContext
{
	/// <summary>
	/// Interface to a buffer that stores indexed triangle data. This interface is the base interface of all indexed triangle buffers.
	/// </summary>
	public interface IIndexedTriangleBuffer
	{
		/// <summary>
		/// Gets the number of triangles already stored in the buffer.
		/// </summary>
		/// <value>
		/// The index offset, i.e. the number of triangles already stored in the buffer.
		/// </value>
		int VertexCount { get; }

		/// <summary>
		/// Gets the number of triangles (the number of indices is 3 times the number of triangles).
		/// </summary>
		/// <value>
		/// Number of triangles (the number of indices is 3 times the number of triangles).
		/// </value>
		int TriangleCount { get; }

		/// <summary>
		/// Adds the indices for one triangle.
		/// </summary>
		/// <param name="v1">The index of vertex 1.</param>
		/// <param name="v2">The index of vertex 2.</param>
		/// <param name="v3">The index of vertex 3.</param>
		void AddTriangleIndices(int v1, int v2, int v3);
	}
}