#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
  using Buffer = SharpDX.Direct3D10.Buffer;

  public partial class D3D10Scene
  {
    internal class VertexAndIndexDeviceBufferNoMaterialWithUColor : IDisposable
    {
      public Buffer VertexBuffer;
      public Buffer IndexBuffer;
      public int VertexCount;
      public int IndexCount;
      


      #region IDisposable Support

      private bool _isDisposed = false; // To detect redundant calls

      ~VertexAndIndexDeviceBufferNoMaterialWithUColor()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
      }

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



      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }
      #endregion
    }
  }
}
