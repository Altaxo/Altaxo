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
  /// <summary>
  /// Base class for indexed triangle buffers with shared vertex and index storage.
  /// </summary>
  public abstract class IndexedTriangleBuffer : IIndexedTriangleBuffer
  {
    /// <summary>
    /// Transformation context used to transform incoming geometry.
    /// </summary>
    protected ITransformationContext _parent;
    /// <summary>
    /// Vertex stream backing array.
    /// </summary>
    protected float[] _vertexStream;
    /// <summary>
    /// Index stream backing array.
    /// </summary>
    protected int[] _indexStream;
    /// <summary>
    /// Current number of vertices.
    /// </summary>
    protected int _numberOfVertices;
    /// <summary>
    /// Current number of triangles.
    /// </summary>
    protected int _numberOfTriangles;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedTriangleBuffer"/> class.
    /// </summary>
    /// <param name="parent">Transformation context.</param>
    protected IndexedTriangleBuffer(ITransformationContext parent)
    {
      _parent = parent;

      _vertexStream = new float[8 * 65536];
      _indexStream = new int[3 * 65536];
    }

    /// <summary>
    /// Gets the number of bytes per vertex.
    /// </summary>
    protected abstract int BytesPerVertex { get; }

    /// <summary>
    /// Gets the number of triangles currently in the buffer.
    /// </summary>
    public int TriangleCount
    {
      get
      {
        return _numberOfTriangles;
      }
    }

    /// <summary>
    /// Gets the number of vertices currently in the buffer.
    /// </summary>
    public int VertexCount
    {
      get
      {
        return _numberOfVertices;
      }
    }

    /// <summary>
    /// Gets the vertex stream backing array.
    /// </summary>
    public float[] VertexStream
    {
      get
      {
        return _vertexStream;
      }
    }

    /// <summary>
    /// Gets the used length of the vertex stream in bytes.
    /// </summary>
    public int VertexStreamLength
    {
      get
      {
        return _numberOfVertices * BytesPerVertex;
      }
    }

    /// <summary>
    /// Gets the index stream backing array.
    /// </summary>
    public int[] IndexStream
    {
      get
      {
        return _indexStream;
      }
    }

    /// <summary>
    /// Gets the used length of the index stream in bytes.
    /// </summary>
    public int IndexStreamLength
    {
      get
      {
        return _numberOfTriangles * (3 * 4);
      }
    }

    /// <summary>
    /// Adds one triangle index triplet in default winding order.
    /// </summary>
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

    /// <summary>
    /// Adds one triangle index triplet with winding defined by the coordinate-system handedness.
    /// </summary>
    public void AddTriangleIndices(int v1, int v2, int v3, bool isLeftHandedCOS)
    {
      int offs = _numberOfTriangles * 3;

      if (offs + 3 >= _indexStream.Length)
        Array.Resize(ref _indexStream, _indexStream.Length * 2);

      _indexStream[offs + 0] = v1;
      if (isLeftHandedCOS)
      {
        _indexStream[offs + 1] = v2;
        _indexStream[offs + 2] = v3;
      }
      else
      {
        _indexStream[offs + 1] = v3;
        _indexStream[offs + 2] = v2;
      }
      ++_numberOfTriangles;
    }
  }
}
