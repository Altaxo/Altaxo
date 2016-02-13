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

namespace Altaxo.Graph.Graph3D.GraphicsContext.D3D
{
	public abstract class IndexedTriangleBuffer : IIndexedTriangleBuffer
	{
		protected ITransformationContext _parent;
		protected float[] _vertexStream;
		protected int[] _indexStream;
		protected int _numberOfVertices;
		protected int _numberOfTriangles;

		protected IndexedTriangleBuffer(ITransformationContext parent)
		{
			_parent = parent;

			_vertexStream = new float[8 * 65536];
			_indexStream = new int[3 * 65536];
		}

		protected abstract int BytesPerVertex { get; }

		public int TriangleCount
		{
			get
			{
				return _numberOfTriangles;
			}
		}

		public int VertexCount
		{
			get
			{
				return _numberOfVertices;
			}
		}

		public float[] VertexStream
		{
			get
			{
				return _vertexStream;
			}
		}

		public int VertexStreamLength
		{
			get
			{
				return _numberOfVertices * BytesPerVertex;
			}
		}

		public int[] IndexStream
		{
			get
			{
				return _indexStream;
			}
		}

		public int IndexStreamLength
		{
			get
			{
				return _numberOfTriangles * (3 * 4);
			}
		}

		public void AddTriangleIndices(int v1, int v2, int v3)
		{
			int offs = _numberOfTriangles * 3;

			if (offs + 3 >= _indexStream.Length)
				Array.Resize(ref _indexStream, _indexStream.Length * 2);

			_indexStream[offs + 0] = v1;
			_indexStream[offs + 1] = v3;
			_indexStream[offs + 2] = v2;
			++_numberOfTriangles;
		}
	}
}