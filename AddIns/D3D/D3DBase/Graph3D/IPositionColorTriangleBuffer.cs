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
	public interface IPositionColorTriangleBuffer
	{
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
		void Add(float x, float y, float z, float w, float r, float g, float b, float a);
	}
}