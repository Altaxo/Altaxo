#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  using System;
  using System.Numerics;
  using Altaxo.Drawing.D3D;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Buffer = Vortice.Direct3D12.ID3D12Resource;

  public partial class D3D12Scene
  {
    /// <summary>
    /// Device-side triangle buffer container including material and optional clip/color data.
    /// </summary>
    internal class VertexAndIndexDeviceBuffer
    {
      /// <summary>
      /// Tracks whether this instance has been disposed.
      /// </summary>
      private bool _isDisposed;
      /// <summary>
      /// Material associated with this buffer set.
      /// </summary>
      private IMaterial _material;
      /// <summary>
      /// Vertex buffer resource.
      /// </summary>
      private Buffer _vertexBuffer;
      /// <summary>
      /// Number of vertices in <see cref="_vertexBuffer"/>.
      /// </summary>
      private int _vertexCount;
      /// <summary>
      /// Index buffer resource.
      /// </summary>
      private Buffer _indexBuffer;
      /// <summary>
      /// Number of indices in <see cref="_indexBuffer"/>.
      /// </summary>
      private int _indexCount;
      /// <summary>
      /// Optional clip planes for this geometry.
      /// </summary>
      private Plane[] _clipPlanes;
      /// <summary>
      /// Optional U-color lookup payload for color provider rendering.
      /// </summary>
      private byte[] _uColors;


      /// <summary>
      /// Gets the material.
      /// </summary>
      public IMaterial Material => _material;
      /// <summary>
      /// Gets the vertex buffer resource.
      /// </summary>
      public Buffer VertexBuffer => _vertexBuffer;
      /// <summary>
      /// Gets the vertex count.
      /// </summary>
      public int VertexCount => _vertexCount;
      /// <summary>
      /// Gets the index buffer resource.
      /// </summary>
      public Buffer IndexBuffer => _indexBuffer;
      /// <summary>
      /// Gets the index count.
      /// </summary>
      public int IndexCount => _indexCount;
      /// <summary>
      /// Gets the clip planes.
      /// </summary>
      public Plane[] ClipPlanes => _clipPlanes;
      /// <summary>
      /// Gets the U-color payload.
      /// </summary>
      public byte[] UColors => _uColors;

      /// <summary>
      /// Initializes a new instance of the <see cref="VertexAndIndexDeviceBuffer"/> class.
      /// </summary>
      /// <param name="material">Material for this geometry.</param>
      /// <param name="vertexBuffer">Vertex buffer resource.</param>
      /// <param name="vertexCount">Vertex count.</param>
      /// <param name="indexBuffer">Index buffer resource.</param>
      /// <param name="indexCount">Index count.</param>
      /// <param name="clipPlanes">Optional clip planes.</param>
      /// <param name="uColors">Optional U-color payload.</param>
      public VertexAndIndexDeviceBuffer(
        IMaterial material,
        Buffer vertexBuffer,
        int vertexCount,
        Buffer indexBuffer,
        int indexCount,
        Plane[] clipPlanes,
        byte[] uColors)
      {
        _isDisposed = false;
        _material = material;
        _vertexBuffer = vertexBuffer;
        _vertexCount = vertexCount;
        _indexBuffer = indexBuffer;
        _indexCount = indexCount;
        _clipPlanes = clipPlanes;
        _uColors = uColors;
      }

      #region IDisposable Support



      /// <summary>
      /// Finalizes an instance of the <see cref="VertexAndIndexDeviceBuffer"/> class.
      /// </summary>
      ~VertexAndIndexDeviceBuffer()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
      }

      /// <summary>
      /// Releases unmanaged and optionally managed resources.
      /// </summary>
      /// <param name="disposing">If <see langword="true"/>, dispose managed resources too.</param>
      protected virtual void Dispose(bool disposing)
      {
        if (!_isDisposed)
        {
          Disposer.RemoveAndDispose(ref _material!);
          Disposer.RemoveAndDispose(ref _vertexBuffer!);
          Disposer.RemoveAndDispose(ref _indexBuffer!);
          _vertexCount = 0;
          _indexCount = 0;
          _clipPlanes = null!;
          _uColors = null!;
          _isDisposed = true;
        }
      }

      /// <summary>
      /// Disposes this instance.
      /// </summary>
      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }
      #endregion
    }
  }
}
