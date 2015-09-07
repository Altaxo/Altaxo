using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public interface IGraphicContext3D
	{
		/// <summary>
		/// Gets a buffer that takes triangle data, consisting of vertex position and vertex color.
		/// </summary>
		/// <param name="numberOfVertices">Number of vertices intended to store.</param>
		/// <returns>A buffer that takes triangle data, consisting of vertex position and vertex color</returns>
		IPositionColorTriangleBuffer GetPositionColorTriangleBuffer(int numberOfVertices);

		/// <summary>
		/// Gets a buffer that takes triangle data, consisting of vertex position and vertex color.
		/// </summary>
		/// <param name="numberOfVertices">Number of vertices intended to store.</param>
		/// <returns>A buffer that takes triangle data, consisting of vertex position and vertex color</returns>
		IPositionColorIndexedTriangleBuffer GetPositionColorIndexedTriangleBuffer(int numberOfVertices);
	}
}