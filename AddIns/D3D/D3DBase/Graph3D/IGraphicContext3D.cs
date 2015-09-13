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
using System.Drawing;
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

		#region Primitives rendering

		void DrawLine(PenX3D pen, PointD3D p0, PointD3D p1);

		void DrawLine(PenX3D pen, Primitives.ISweepPath3D path);

		VectorD3D MeasureString(string text, FontX3D font, PointD3D pointD3D, StringFormat strfmt);

		void DrawString(string text, FontX3D font, IMaterial3D brush, PointD3D point, StringFormat strfmt);

		#endregion Primitives rendering

		object SaveGraphicsState();

		void RestoreGraphicsState(object graphicsState);

		void MultiplyTransform(MatrixD3D m);

		void TranslateTransform(double x, double y, double z);
	}
}