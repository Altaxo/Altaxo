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
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.GraphicsContext.D3D
{
  public class PositionColorLineListBuffer : IPositionColorLineListBuffer
  {
    private const int FloatsPerLine = 16;
    private const int BytesPerLine = FloatsPerLine * 4;
    protected ITransformationContext _parent;
    protected float[] _vertexStream;
    protected int _numberOfLines;

    public PositionColorLineListBuffer(ITransformationContext parent)
    {
      _parent = parent;
      _vertexStream = new float[8 + 32 * FloatsPerLine];
    }

    public int NumberOfLines { get { return _numberOfLines; } }

    public int VertexStreamLength { get { return _numberOfLines * BytesPerLine; } }

    public int VertexCount { get { return _numberOfLines * 2; } }

    public float[] VertexStream
    {
      get { return _vertexStream; }
    }

    public void AddLine(double x0, double y0, double z0, double x1, double y1, double z1, float r, float g, float b, float a)
    {
      var pt0 = _parent.Transformation.Transform(new PointD3D(x0, y0, z0));
      var pt1 = _parent.Transformation.Transform(new PointD3D(x1, y1, z1));

      int offs = _numberOfLines * FloatsPerLine;

      if (offs + 8 >= _vertexStream.Length)
        Array.Resize(ref _vertexStream, _vertexStream.Length * 2);

      _vertexStream[offs + 0] = (float)pt0.X;
      _vertexStream[offs + 1] = (float)pt0.Y;
      _vertexStream[offs + 2] = (float)pt0.Z;
      _vertexStream[offs + 3] = 1.0f;
      _vertexStream[offs + 4] = r;
      _vertexStream[offs + 5] = g;
      _vertexStream[offs + 6] = b;
      _vertexStream[offs + 7] = a;

      _vertexStream[offs + 8] = (float)pt1.X;
      _vertexStream[offs + 9] = (float)pt1.Y;
      _vertexStream[offs + 10] = (float)pt1.Z;
      _vertexStream[offs + 11] = 1.0f;
      _vertexStream[offs + 12] = r;
      _vertexStream[offs + 13] = g;
      _vertexStream[offs + 14] = b;
      _vertexStream[offs + 15] = a;
      ++_numberOfLines;
    }
  }
}
