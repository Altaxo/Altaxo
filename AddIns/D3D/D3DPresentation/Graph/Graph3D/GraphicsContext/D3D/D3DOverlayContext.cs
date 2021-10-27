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

  public class D3DOverlayContext : GraphicsContextD3DPrimitivesBase, IOverlayContext3D, IDisposable
  {
    protected PositionColorIndexedTriangleBuffer _positionColorIndexedTriangleBuffer;
    protected PositionColorLineListBuffer? _positionColorLineListBuffer;

    private Matrix4x3 _transformation = Matrix4x3.Identity;

    public D3DOverlayContext()
    {
      _positionColorIndexedTriangleBuffer = new PositionColorIndexedTriangleBuffer(this);
    }

    public void Dispose()
    {
    }

    public IPositionColorIndexedTriangleBuffer PositionColorIndexedTriangleBuffers
    {
      get
      {
        return _positionColorIndexedTriangleBuffer;
      }
    }

    public IPositionColorLineListBuffer PositionColorLineListBuffer
    {
      get
      {
        if (_positionColorLineListBuffer is null)
          _positionColorLineListBuffer = new D3D.PositionColorLineListBuffer(this);

        return _positionColorLineListBuffer;
      }
    }

    #region Transformation

    public Matrix4x3 Transformation
    {
      get
      {
        return _transformation;
      }
    }

    public Matrix3x3 TransposedInverseTransformation
    {
      get
      {
        throw new NotImplementedException("TransposedInverseTransformation should not be needed in this overlay context, because we don't use normals here");
      }
    }

    public object SaveGraphicsState()
    {
      return new GraphicState { Transformation = _transformation };
    }

    public void RestoreGraphicsState(object graphicsState)
    {
      var gs = graphicsState as GraphicState;
      if (gs is not null)
      {
        _transformation = gs.Transformation;
      }
      else
        throw new ArgumentException(nameof(graphicsState) + " is not a valid graphic state!");
    }

    public void PrependTransform(Matrix4x3 m)
    {
      _transformation.PrependTransform(m);
    }

    public void TranslateTransform(double x, double y, double z)
    {
      _transformation.TranslatePrepend(x, y, z);
    }

    private class GraphicState
    {
      internal Matrix4x3 Transformation;
    }

    #endregion Transformation
  }
}
