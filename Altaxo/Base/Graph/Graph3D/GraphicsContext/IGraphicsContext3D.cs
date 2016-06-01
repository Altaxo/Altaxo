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

using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.GraphicsContext
{
	public interface ITransformationContext
	{
		Matrix4x3 Transformation { get; }
		Matrix3x3 TransposedInverseTransformation { get; }
	}

	public interface IGraphicsContext3D : ITransformationContext
	{
		/// <summary>
		/// Gets an indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.
		/// </summary>
		/// <param name="material">The material to use.</param>
		/// <returns>Indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.</returns>
		PositionIndexedTriangleBuffers GetPositionIndexedTriangleBuffer(IMaterial material);

		/// <summary>
		/// Gets an indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.
		/// </summary>
		/// <param name="material">The material to use.</param>
		/// <param name="clipPlanes">The clip planes to use.</param>
		/// <returns>Indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.</returns>
		PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBufferWithClipping(IMaterial material, PlaneD3D[] clipPlanes);

		/// <summary>
		/// Gets an indexed triangle buffer with a normal, i.e. either <see cref="IPositionNormalIndexedTriangleBuffer"/>, <see cref="IPositionNormalColorIndexedTriangleBuffer"/> or <see cref="IPositionNormalUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.
		/// </summary>
		/// <param name="material">The material to use.</param>
		/// <returns>Indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.</returns>
		PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBuffer(IMaterial material);

		IPositionNormalUIndexedTriangleBuffer GetPositionNormalUIndexedTriangleBuffer(IMaterial material, PlaneD3D[] clipPlanes, Gdi.Plot.IColorProvider colorProvider);

		#region Primitives rendering

		void DrawLine(PenX3D pen, PointD3D p0, PointD3D p1);

		void DrawLine(PenX3D pen, IPolylineD3D path);

		VectorD3D MeasureString(string text, FontX3D font, PointD3D pointD3D, StringFormat strfmt);

		void DrawString(string text, FontX3D font, IMaterial brush, PointD3D point, StringFormat strfmt);

		#endregion Primitives rendering

		object SaveGraphicsState();

		void RestoreGraphicsState(object graphicsState);

		void PrependTransform(Matrix4x3 m);

		void TranslateTransform(double x, double y, double z);

		void TranslateTransform(VectorD3D diff);
	}

	public interface IOverlayContext3D : ITransformationContext
	{
		IPositionColorIndexedTriangleBuffer PositionColorIndexedTriangleBuffers { get; }

		IPositionColorLineListBuffer PositionColorLineListBuffer { get; }
	}
}