using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Common
{
	using Altaxo.Graph3D;

	using SharpDX;

	public class D3D10GraphicContext : IGraphicContext3D, IDisposable
	{
		protected PositionColorTriangleBuffer _positionColorTriangleBuffer;

		protected PositionColorIndexedTriangleBuffer _positionColorIndexedTriangleBuffer;

		public IPositionColorTriangleBuffer GetPositionColorTriangleBuffer(int numberOfVertices)
		{
			if (null == _positionColorTriangleBuffer)
				_positionColorTriangleBuffer = new PositionColorTriangleBuffer(this);

			return _positionColorTriangleBuffer;
		}

		public IPositionColorIndexedTriangleBuffer GetPositionColorIndexedTriangleBuffer(int numberOfVertices)
		{
			if (null == _positionColorIndexedTriangleBuffer)
				_positionColorIndexedTriangleBuffer = new PositionColorIndexedTriangleBuffer(this);

			return _positionColorIndexedTriangleBuffer;
		}

		public void Dispose()
		{
			if (null != _positionColorTriangleBuffer)
				_positionColorTriangleBuffer.Dispose();
		}

		#region Rendering

		public PositionColorTriangleBuffer BuffersNonindexedTrianglesPositionColor { get { return _positionColorTriangleBuffer; } }
		public PositionColorIndexedTriangleBuffer BuffersIndexTrianglesPositionColor { get { return _positionColorIndexedTriangleBuffer; } }

		#endregion Rendering
	}

	public class PositionColorTriangleBuffer : IPositionColorTriangleBuffer, IDisposable
	{
		private D3D10GraphicContext _parent;
		protected DataStream _vertexStream;
		protected int _vertexCount;

		public PositionColorTriangleBuffer(D3D10GraphicContext parent)
		{
			_parent = parent;
			_vertexStream = new DataStream(1024 * 3 * 32, true, true);
		}

		public DataStream VertexStream { get { return _vertexStream; } }
		public long VertexStreamLength { get { return _vertexCount * 32; } }
		public int VertexCount { get { return _vertexCount; } }

		public void Add(float x, float y, float z, float w, float r, float g, float b, float a)
		{
			_vertexStream.Write(new Vector4(x, y, z, w));
			_vertexStream.Write(new Vector4(r, g, b, a));
			++_vertexCount;
		}

		public void Dispose()
		{
			Disposer.RemoveAndDispose(ref _vertexStream);
			_vertexCount = 0;
		}
	}

	public class PositionColorIndexedTriangleBuffer : IPositionColorIndexedTriangleBuffer, IDisposable
	{
		private D3D10GraphicContext _parent;
		protected DataStream _vertexStream;
		protected DataStream _indexStream;
		protected int _numberOfVertices;
		protected int _numberOfTriangles;

		public PositionColorIndexedTriangleBuffer(D3D10GraphicContext parent)
		{
			_parent = parent;
			_vertexStream = new DataStream(1024 * 3 * 32, true, true);
			_indexStream = new DataStream(1024 * 12, true, true);
		}

		public DataStream VertexStream { get { return _vertexStream; } }

		public DataStream IndexStream { get { return _indexStream; } }

		public int VertexCount
		{
			get
			{
				return _numberOfVertices;
			}
		}

		public int VertexStreamLength
		{
			get
			{
				return _numberOfVertices * 32;
			}
		}

		public int TriangleCount
		{
			get
			{
				return _numberOfTriangles;
			}
		}

		public int IndexStreamLength
		{
			get
			{
				return _numberOfTriangles * 12;
			}
		}

		public void AddTriangleVertex(float x, float y, float z, float w, float r, float g, float b, float a)
		{
			_vertexStream.Write(new Vector4(x, y, z, w));
			_vertexStream.Write(new Vector4(r, g, b, a));
			++_numberOfVertices;
		}

		public void Dispose()
		{
			Disposer.RemoveAndDispose(ref _vertexStream);
			Disposer.RemoveAndDispose(ref _indexStream);
			_numberOfTriangles = 0;
			_numberOfVertices = 0;
		}

		public void AddTriangleIndices(int v1, int v2, int v3)
		{
			_indexStream.Write(v1);
			_indexStream.Write(v2);
			_indexStream.Write(v3);
			++_numberOfTriangles;
		}
	}
}