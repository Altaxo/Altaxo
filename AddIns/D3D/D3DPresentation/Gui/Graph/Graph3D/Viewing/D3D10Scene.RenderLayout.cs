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
  using SharpDX.Direct3D10;

  public partial class D3D10Scene
  {
    internal class RenderLayout : IDisposable
    {
      private InputLayout _vertexLayout;
      private EffectTechnique _technique;
      private EffectPass _pass;


      public InputLayout VertexLayout => _vertexLayout;
      public EffectTechnique Technique => _technique;
      public EffectPass Pass => _pass;



      public RenderLayout(EffectTechnique technique, EffectPass pass, InputLayout vertexLayout)
      {
        _technique = technique;
        _pass = pass;
        _vertexLayout = vertexLayout;
      }

      #region IDisposable Support

      private bool _isDisposed = false; // To detect redundant calls

      protected virtual void Dispose(bool disposing)
      {
        if (!_isDisposed)
        {
          Disposer.RemoveAndDispose(ref _vertexLayout!);
          Disposer.RemoveAndDispose(ref _technique!);
          Disposer.RemoveAndDispose(ref _pass!);
          _isDisposed = true;
        }
      }

      ~RenderLayout()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
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
