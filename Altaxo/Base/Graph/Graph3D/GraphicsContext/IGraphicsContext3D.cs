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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.GraphicsContext
{
  /// <summary>
  /// Provides transformation information for a 3D graphics context.
  /// </summary>
  public interface ITransformationContext
  {
    /// <summary>
    /// Gets the current transformation matrix.
    /// </summary>
    Matrix4x3 Transformation { get; }
    /// <summary>
    /// Gets the transposed inverse transformation matrix.
    /// </summary>
    Matrix3x3 TransposedInverseTransformation { get; }
  }

  /// <summary>
  /// Represents a 3D graphics context.
  /// </summary>
  public interface IGraphicsContext3D : ITransformationContext
  {
    /// <summary>
    /// Gets an indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.
    /// </summary>
    /// <param name="material">The material to use.</param>
    /// <returns>Indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.</returns>
    PositionIndexedTriangleBuffers GetPositionIndexedTriangleBuffer(IMaterial material);

    /// <summary>
    /// Gets an indexed triangle buffer with a normal and optional clipping.
    /// </summary>
    /// <param name="material">The material to use.</param>
    /// <param name="clipPlanes">The clip planes to apply.</param>
    /// <returns>An indexed triangle buffer with normals and optional clipping information.</returns>
    PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBufferWithClipping(IMaterial material, PlaneD3D[] clipPlanes);

    /// <summary>
    /// Gets an indexed triangle buffer with a normal, i.e. either <see cref="IPositionNormalIndexedTriangleBuffer"/>, <see cref="IPositionNormalColorIndexedTriangleBuffer"/> or <see cref="IPositionNormalUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.
    /// </summary>
    /// <param name="material">The material to use.</param>
    /// <returns>Indexed triangle buffer without using a normal, i.e. either <see cref="IPositionIndexedTriangleBuffer"/>, <see cref="IPositionColorIndexedTriangleBuffer"/> or <see cref="IPositionUVIndexedTriangleBuffer"/>, depending on wether the material has its own color or texture.</returns>
    PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBuffer(IMaterial material);

    /// <summary>
    /// Gets an indexed triangle buffer with a normal and an additional scalar value.
    /// </summary>
    /// <param name="material">The material to use.</param>
    /// <param name="clipPlanes">The optional clip planes to apply.</param>
    /// <param name="colorProvider">The color provider used to map the additional scalar value to colors.</param>
    /// <returns>An indexed triangle buffer with normals and an additional scalar component.</returns>
    IPositionNormalUIndexedTriangleBuffer GetPositionNormalUIndexedTriangleBuffer(IMaterial material, PlaneD3D[]? clipPlanes, Gdi.Plot.IColorProvider colorProvider);

    #region Primitives rendering

    /// <summary>
    /// Draws a 3D line segment.
    /// </summary>
    /// <param name="pen">The pen used to draw the segment.</param>
    /// <param name="p0">The start point of the segment.</param>
    /// <param name="p1">The end point of the segment.</param>
    void DrawLine(PenX3D pen, PointD3D p0, PointD3D p1);

    /// <summary>
    /// Draws a 3D polyline.
    /// </summary>
    /// <param name="pen">The pen used to draw the polyline.</param>
    /// <param name="path">The polyline path to draw.</param>
    void DrawLine(PenX3D pen, IPolylineD3D path);

    /// <summary>
    /// Measures a text string in 3D space.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font used for the measurement.</param>
    /// <param name="pointD3D">The reference point for the measurement.</param>
    /// <returns>The measured size of the text.</returns>
    VectorD3D MeasureString(string text, FontX3D font, PointD3D pointD3D);

    /// <summary>
    /// Draws a string with explicit alignments.
    /// </summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font used to draw the text.</param>
    /// <param name="brush">The material used to draw the text.</param>
    /// <param name="point">The reference point of the text.</param>
    /// <param name="alignmentX">The horizontal alignment.</param>
    /// <param name="alignmentY">The vertical alignment.</param>
    /// <param name="alignmentZ">The depth alignment.</param>
    void DrawString(string text, FontX3D font, IMaterial brush, PointD3D point, Alignment alignmentX, Alignment alignmentY, Alignment alignmentZ);

    /// <summary>
    /// Draws a string using default alignment.
    /// </summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The font used to draw the text.</param>
    /// <param name="brush">The material used to draw the text.</param>
    /// <param name="point">The reference point of the text.</param>
    void DrawString(string text, FontX3D font, IMaterial brush, PointD3D point);

    #endregion Primitives rendering

    /// <summary>
    /// Saves the current graphics state.
    /// </summary>
    /// <returns>An opaque graphics-state object that can later be passed to <see cref="RestoreGraphicsState(object)"/>.</returns>
    object SaveGraphicsState();

    /// <summary>
    /// Restores a previously saved graphics state.
    /// </summary>
    /// <param name="graphicsState">The graphics state previously returned by <see cref="SaveGraphicsState()"/>.</param>
    void RestoreGraphicsState(object graphicsState);

    /// <summary>
    /// Prepends a transformation matrix.
    /// </summary>
    /// <param name="m">The transformation matrix to prepend.</param>
    void PrependTransform(Matrix4x3 m);

    /// <summary>
    /// Applies a translation.
    /// </summary>
    /// <param name="x">The translation in x-direction.</param>
    /// <param name="y">The translation in y-direction.</param>
    /// <param name="z">The translation in z-direction.</param>
    void TranslateTransform(double x, double y, double z);

    /// <summary>
    /// Applies a translation.
    /// </summary>
    /// <param name="diff">The translation vector.</param>
    void TranslateTransform(VectorD3D diff);

    /// <summary>
    /// Applies a rotation.
    /// </summary>
    /// <param name="degreeX">The rotation around the x-axis, in degrees.</param>
    /// <param name="degreeY">The rotation around the y-axis, in degrees.</param>
    /// <param name="degreeZ">The rotation around the z-axis, in degrees.</param>
    void RotateTransform(double degreeX, double degreeY, double degreeZ);
  }

  /// <summary>
  /// Represents a 3D overlay context.
  /// </summary>
  public interface IOverlayContext3D : ITransformationContext
  {
    /// <summary>
    /// Gets the indexed triangle buffer for colored overlay geometry.
    /// </summary>
    IPositionColorIndexedTriangleBuffer PositionColorIndexedTriangleBuffers { get; }

    /// <summary>
    /// Gets the line-list buffer for colored overlay geometry.
    /// </summary>
    IPositionColorLineListBuffer PositionColorLineListBuffer { get; }
  }
}
