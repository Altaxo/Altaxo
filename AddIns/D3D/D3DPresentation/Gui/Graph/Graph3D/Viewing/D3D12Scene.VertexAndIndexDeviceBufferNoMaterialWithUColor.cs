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
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Buffer = Vortice.Direct3D12.ID3D12Resource;

  public partial class D3D12Scene
  {
    /// <summary>
    /// Device-side triangle buffer container without material but with optional U-color payload.
    /// </summary>
    internal class VertexAndIndexDeviceBufferNoMaterialWithUColor : IDisposable
    {
      /// <summary>
      /// Vertex buffer resource.
      /// </summary>
      public Buffer? VertexBuffer;
      /// <summary>
      /// Index buffer resource.
      /// </summary>
      public Buffer? IndexBuffer;
      /// <summary>
      /// Number of vertices in <see cref="VertexBuffer"/>.
      /// </summary>
      public int VertexCount;
      /// <summary>
      /// Number of indices in <see cref="IndexBuffer"/>.
      /// </summary>
      public int IndexCount;



      #region IDisposable Support

      /// <summary>
      /// Tracks whether this instance has been disposed.
      /// </summary>
      private bool _isDisposed = false; // To detect redundant calls

      /// <summary>
      /// Finalizes an instance of the <see cref="VertexAndIndexDeviceBufferNoMaterialWithUColor"/> class.
      /// </summary>
      ~VertexAndIndexDeviceBufferNoMaterialWithUColor()
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
          Disposer.RemoveAndDispose(ref VertexBuffer);
          Disposer.RemoveAndDispose(ref IndexBuffer);
          VertexCount = 0;
          IndexCount = 0;

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
