using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	/// <summary>
	/// Interface to a buffer that stores non-indexed triangle data consisting of position and color.
	/// </summary>
	public interface IPositionColorIndexedTriangleBuffer
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
		/// Adds the specified vertex.
		/// </summary>
		/// <param name="x">The x position.</param>
		/// <param name="y">The y position.</param>
		/// <param name="z">The z position.</param>
		/// <param name="w">The w position.</param>
		/// <param name="r">The r color component.</param>
		/// <param name="g">The g color component.</param>
		/// <param name="b">The b color component.</param>
		/// <param name="a">The a color component.</param>
		void AddTriangleVertex(float x, float y, float z, float w, float r, float g, float b, float a);

		/// <summary>
		/// Adds the indices for one triangle.
		/// </summary>
		/// <param name="v1">The index of vertex 1.</param>
		/// <param name="v2">The index of vertex 2.</param>
		/// <param name="v3">The index of vertex 3.</param>
		void AddTriangleIndices(int v1, int v2, int v3);
	}
}