#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D.GraphicsContext;
  using Drawing.D3D;

  public class D3DGraphicsContext
        : GraphicsContext3DBase, IDisposable
  {
    /// <summary>
    /// Position-only triangle buffers grouped by material key.
    /// </summary>
    protected Dictionary<MaterialKey, PositionIndexedTriangleBuffer> _positionIndexedTriangleBuffers = new Dictionary<MaterialKey, PositionIndexedTriangleBuffer>();

    /// <summary>
    /// Position-normal triangle buffers grouped by material key.
    /// </summary>
    protected Dictionary<MaterialKey, PositionNormalIndexedTriangleBuffer> _positionNormalIndexedTriangleBuffers = new Dictionary<MaterialKey, PositionNormalIndexedTriangleBuffer>();

    //protected Dictionary<IMaterial, PositionColorIndexedTriangleBuffer> _positionColorIndexedTriangleBuffers = new Dictionary<IMaterial, PositionColorIndexedTriangleBuffer>(MaterialComparer.Instance);

    /// <summary>
    /// Position-color triangle buffers grouped by material and clip planes.
    /// </summary>
    protected Dictionary<MaterialPlusClippingKey, PositionColorIndexedTriangleBuffer> _positionColorIndexedTriangleBuffers = new Dictionary<MaterialPlusClippingKey, PositionColorIndexedTriangleBuffer>();

    /// <summary>
    /// Position-normal-color triangle buffers grouped by material and clip planes.
    /// </summary>
    protected Dictionary<MaterialPlusClippingKey, PositionNormalColorIndexedTriangleBuffer> _positionNormalColorIndexedTriangleBuffers = new Dictionary<MaterialPlusClippingKey, PositionNormalColorIndexedTriangleBuffer>();

    /// <summary>
    /// Position-UV triangle buffers grouped by material key.
    /// </summary>
    protected Dictionary<MaterialKey, PositionUVIndexedTriangleBuffer> _positionUVIndexedTriangleBuffers = new Dictionary<MaterialKey, PositionUVIndexedTriangleBuffer>();

    /// <summary>
    /// Position-normal-UV triangle buffers grouped by material key.
    /// </summary>
    protected Dictionary<MaterialKey, PositionNormalUVIndexedTriangleBuffer> _positionNormalUVIndexedTriangleBuffers = new Dictionary<MaterialKey, PositionNormalUVIndexedTriangleBuffer>();

    /// <summary>
    /// Position-normal-U triangle buffers grouped by material, clipping and color provider.
    /// </summary>
    protected Dictionary<MaterialPlusClippingPlusColorProviderKey, PositionNormalUIndexedTriangleBuffer> _positionNormalUIndexedTriangleBuffers = new Dictionary<MaterialPlusClippingPlusColorProviderKey, PositionNormalUIndexedTriangleBuffer>();

    /// <summary>Transformation of positions from local coordinates to global coordinates.</summary>
    private Matrix4x3 _transformation = Matrix4x3.Identity;

    /// <summary>Transformation of normal vectors from local coordinates to global coordinates.</summary>
    private Matrix3x3 _transposedInverseTransformation = Matrix3x3.Identity;

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Gets position-only triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, PositionIndexedTriangleBuffer>> PositionIndexedTriangleBuffers
    {
      get
      {
        return _positionIndexedTriangleBuffers;
      }
    }

    /// <summary>
    /// Gets position-only triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-normal triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, PositionNormalIndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffers
    {
      get
      {
        return _positionNormalIndexedTriangleBuffers;
      }
    }

    /// <summary>
    /// Gets position-normal triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionNormalIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionNormalIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-color triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<IMaterial, PositionColorIndexedTriangleBuffer>> PositionColorIndexedTriangleBuffers
    {
      get
      {
        return _positionColorIndexedTriangleBuffers.Select(kvp => new KeyValuePair<IMaterial, PositionColorIndexedTriangleBuffer>(kvp.Key.Material, kvp.Value));
      }
    }

    /// <summary>
    /// Gets position-color triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionColorIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionColorIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-normal-color triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<IMaterial, PositionNormalColorIndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffers
    {
      get
      {
        return _positionNormalColorIndexedTriangleBuffers.Select(kvp => new KeyValuePair<IMaterial, PositionNormalColorIndexedTriangleBuffer>(kvp.Key.Material, kvp.Value));
      }
    }

    /// <summary>
    /// Gets position-normal-color triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionNormalColorIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionNormalColorIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-UV triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, PositionUVIndexedTriangleBuffer>> PositionUVIndexedTriangleBuffers
    {
      get
      {
        return _positionUVIndexedTriangleBuffers;
      }
    }

    /// <summary>
    /// Gets position-UV triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionUVIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionUVIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-normal-UV triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, PositionNormalUVIndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffers
    {
      get
      {
        return _positionNormalUVIndexedTriangleBuffers;
      }
    }

    /// <summary>
    /// Gets position-normal-UV triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionNormalUVIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionNormalUVIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    /// <summary>
    /// Gets position-normal-U triangle buffers as base indexed triangle buffers.
    /// </summary>
    public IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>> PositionNormalUIndexedTriangleBuffersAsIndexedTriangleBuffers
    {
      get
      {
        foreach (var entry in _positionNormalUIndexedTriangleBuffers)
          yield return new KeyValuePair<MaterialKey, IndexedTriangleBuffer>(entry.Key, entry.Value);
      }
    }

    #region Transformation

    /// <summary>
    /// Gets the current transformation.
    /// </summary>
    public override Matrix4x3 Transformation
    {
      get
      {
        return _transformation;
      }
    }

    /// <summary>
    /// Gets the transposed inverse transformation.
    /// </summary>
    public override Matrix3x3 TransposedInverseTransformation
    {
      get
      {
        return _transposedInverseTransformation;
      }
    }

    /// <summary>
    /// Saves graphics state.
    /// </summary>
    public override object SaveGraphicsState()
    {
      return new GraphicState(_transformation, _transposedInverseTransformation);
    }

    /// <summary>
    /// Restores graphics state.
    /// </summary>
    public override void RestoreGraphicsState(object graphicsState)
    {
      var gs = graphicsState as GraphicState;
      if (gs is not null)
      {
        _transformation = gs.Transformation;
        _transposedInverseTransformation = gs.TransposedInverseTransformation;
      }
      else
        throw new ArgumentException(nameof(graphicsState) + " is not a valid graphic state!");
    }

    /// <summary>
    /// Prepends a transformation.
    /// </summary>
    public override void PrependTransform(Matrix4x3 m)
    {
      _transformation.PrependTransform(m);
      _transposedInverseTransformation = _transformation.GetTransposedInverseMatrix3x3();
    }

    /// <summary>
    /// Prepends a translation transform.
    /// </summary>
    public override void TranslateTransform(double x, double y, double z)
    {
      _transformation.TranslatePrepend(x, y, z);
      // no change to the inverse transform
    }

    /// <summary>
    /// Prepends a translation transform.
    /// </summary>
    public override void TranslateTransform(VectorD3D translation)
    {
      _transformation.TranslatePrepend(translation.X, translation.Y, translation.Z);
      // no change to the inverse transform
    }

    /// <summary>
    /// Prepends rotations around x, y and z axes.
    /// </summary>
    public override void RotateTransform(double degreeX, double degreeY, double degreeZ)
    {
      if (0 != degreeZ)
        _transformation.RotationZDegreePrepend(degreeZ);
      if (0 != degreeY)
        _transformation.RotationYDegreePrepend(degreeY);
      if (0 != degreeX)
        _transformation.RotationXDegreePrepend(degreeX);

      if (0 != degreeZ || 0 != degreeY || 0 != degreeX)
        _transposedInverseTransformation = _transformation.GetTransposedInverseMatrix3x3();
    }

    /// <summary>
    /// Gets a position-normal indexed triangle buffer matching the material.
    /// </summary>
    public override PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBuffer(IMaterial material)
    {
      var result = new PositionNormalIndexedTriangleBuffers();

      if (material.HasTexture)
      {
        result.IndexedTriangleBuffer = result.PositionNormalUVIndexedTriangleBuffer = InternalGetPositionNormalUVIndexedTriangleBuffer(material);
      }
      else if (material.HasColor)
      {
        result.IndexedTriangleBuffer = result.PositionNormalIndexedTriangleBuffer = InternalGetPositionNormalIndexedTriangleBuffer(material);
      }
      else
      {
        result.IndexedTriangleBuffer = result.PositionNormalColorIndexedTriangleBuffer = InternalGetPositionNormalColorIndexedTriangleBuffer(material);
      }
      return result;
    }

    /// <summary>
    /// Gets or creates the position-normal triangle buffer for the material.
    /// </summary>
    private PositionNormalIndexedTriangleBuffer InternalGetPositionNormalIndexedTriangleBuffer(IMaterial material)
    {
      var materialKey = new MaterialKey(material);
      if (!_positionNormalIndexedTriangleBuffers.TryGetValue(materialKey, out var result))
      {
        result = new PositionNormalIndexedTriangleBuffer(this);
        _positionNormalIndexedTriangleBuffers.Add(materialKey, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates the position-normal-color triangle buffer for the material.
    /// </summary>
    private PositionNormalColorIndexedTriangleBuffer InternalGetPositionNormalColorIndexedTriangleBuffer(IMaterial material)
    {
      var key = new MaterialPlusClippingKey(material, clipPlanes: null);
      if (!_positionNormalColorIndexedTriangleBuffers.TryGetValue(key, out var result))
      {
        result = new PositionNormalColorIndexedTriangleBuffer(this);
        _positionNormalColorIndexedTriangleBuffers.Add(key, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates the position-normal-UV triangle buffer for the material.
    /// </summary>
    private PositionNormalUVIndexedTriangleBuffer InternalGetPositionNormalUVIndexedTriangleBuffer(IMaterial material)
    {
      var materialKey = new MaterialKey(material);

      if (!_positionNormalUVIndexedTriangleBuffers.TryGetValue(materialKey, out var result))
      {
        result = new PositionNormalUVIndexedTriangleBuffer(this);
        _positionNormalUVIndexedTriangleBuffers.Add(materialKey, result);
      }

      return result;
    }

    /// <summary>
    /// Gets a position indexed triangle buffer matching the material.
    /// </summary>
    public override PositionIndexedTriangleBuffers GetPositionIndexedTriangleBuffer(IMaterial material)
    {
      var result = new PositionIndexedTriangleBuffers();

      if (material.HasTexture)
      {
        result.IndexedTriangleBuffer = result.PositionUVIndexedTriangleBuffer = InternalGetPositionUVIndexedTriangleBuffer(material);
      }
      else if (material.HasColor)
      {
        result.IndexedTriangleBuffer = result.PositionIndexedTriangleBuffer = InternalGetPositionIndexedTriangleBuffer(material);
      }
      else
      {
        result.IndexedTriangleBuffer = result.PositionColorIndexedTriangleBuffer = InternalGetPositionColorIndexedTriangleBuffer(material);
      }
      return result;
    }

    /// <summary>
    /// Gets position-normal indexed triangle buffers with clipping.
    /// </summary>
    public override PositionNormalIndexedTriangleBuffers GetPositionNormalIndexedTriangleBufferWithClipping(IMaterial material, PlaneD3D[] clipPlanes)
    {
      var result = new PositionNormalIndexedTriangleBuffers();

      if (material.HasTexture)
      {
        throw new NotImplementedException();
      }
      else if (material.HasColor)
      {
        throw new NotImplementedException();
      }
      else
      {
        result.IndexedTriangleBuffer = result.PositionNormalColorIndexedTriangleBuffer = InternalGetPositionNormalColorIndexedTriangleBuffer(material, clipPlanes);
      }
      return result;
    }

    /// <summary>
    /// Gets or creates the position indexed triangle buffer for the material.
    /// </summary>
    private PositionIndexedTriangleBuffer InternalGetPositionIndexedTriangleBuffer(IMaterial material)
    {
      var materialKey = new MaterialKey(material);

      if (!_positionIndexedTriangleBuffers.TryGetValue(materialKey, out var result))
      {
        result = new PositionIndexedTriangleBuffer(this);
        _positionIndexedTriangleBuffers.Add(materialKey, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates the position-color indexed triangle buffer for the material.
    /// </summary>
    private PositionColorIndexedTriangleBuffer InternalGetPositionColorIndexedTriangleBuffer(IMaterial material)
    {
      var key = new MaterialPlusClippingKey(material, null);
      if (!_positionColorIndexedTriangleBuffers.TryGetValue(key, out var result))
      {
        result = new PositionColorIndexedTriangleBuffer(this);
        _positionColorIndexedTriangleBuffers.Add(key, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates the position-normal-color indexed triangle buffer for material and clip planes.
    /// </summary>
    private PositionNormalColorIndexedTriangleBuffer InternalGetPositionNormalColorIndexedTriangleBuffer(IMaterial material, PlaneD3D[] clipPlanes)
    {
      // Transform the clip planes to our coordinate system

      var clipPlanesTransformed = clipPlanes.Select(plane => _transformation.Transform(plane)).ToArray();

      var key = new MaterialPlusClippingKey(material, clipPlanesTransformed);
      if (!_positionNormalColorIndexedTriangleBuffers.TryGetValue(key, out var result))
      {
        result = new PositionNormalColorIndexedTriangleBufferWithClipping(this, clipPlanesTransformed);
        _positionNormalColorIndexedTriangleBuffers.Add(key, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates the position-UV indexed triangle buffer for the material.
    /// </summary>
    private PositionUVIndexedTriangleBuffer InternalGetPositionUVIndexedTriangleBuffer(IMaterial material)
    {
      var materialKey = new MaterialKey(material);

      if (!_positionUVIndexedTriangleBuffers.TryGetValue(materialKey, out var result))
      {
        result = new PositionUVIndexedTriangleBuffer(this);
        _positionUVIndexedTriangleBuffers.Add(materialKey, result);
      }

      return result;
    }

    /// <summary>
    /// Gets or creates a position-normal-U indexed triangle buffer.
    /// </summary>
    public override IPositionNormalUIndexedTriangleBuffer GetPositionNormalUIndexedTriangleBuffer(IMaterial material, PlaneD3D[]? clipPlanes, Gdi.Plot.IColorProvider colorProvider)
    {
      // Transform the clip planes to our coordinate system

      var clipPlanesTransformed = clipPlanes is null ? null : clipPlanes.Select(plane => _transformation.Transform(plane)).ToArray();

      var key = new MaterialPlusClippingPlusColorProviderKey(material, clipPlanesTransformed, colorProvider);
      if (!_positionNormalUIndexedTriangleBuffers.TryGetValue(key, out var result))
      {
        result = new PositionNormalUIndexedTriangleBuffer(this);
        _positionNormalUIndexedTriangleBuffers.Add(key, result);
      }

      return result;
    }

    /// <summary>
    /// Serializable graphics-state snapshot.
    /// </summary>
    internal class GraphicState
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="GraphicState"/> class.
      /// </summary>
      internal GraphicState(Matrix4x3 transformation, Matrix3x3 transposedInverseTransformation)
      {
        Transformation = transformation;
        TransposedInverseTransformation = transposedInverseTransformation;
      }

      /// <summary>
      /// Gets the saved transformation.
      /// </summary>
      internal Matrix4x3 Transformation { get; private set; }
      /// <summary>
      /// Gets the saved transposed inverse transformation.
      /// </summary>
      internal Matrix3x3 TransposedInverseTransformation { get; private set; }
    }

    #endregion Transformation
  }
}
