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
using System.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Serialization;

namespace Altaxo.Graph.Graph3D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Represents the null symbol in a scatter plot, i.e. this symbol is not visible.
  /// </summary>
  /// <seealso cref="Altaxo.Graph.Graph3D.Plot.Styles.IScatterSymbol" />
  public sealed class Sphere : ScatterSymbolShapeBase
  {
    public static Sphere Instance { get; private set; } = new Sphere();

    public static SolidIcoSphere _geometry = new SolidIcoSphere(2);

    #region Serialization

    /// <summary>
    /// 2016-07-01 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Sphere), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SerializeV0((IScatterSymbol)obj, info);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return DeserializeV0(Instance, info, parent);
      }
    }

    #endregion Serialization

    public override void Paint(IGraphicsContext3D g, IMaterial material, PointD3D centerLocation, double symbolSize)
    {
      var radius = symbolSize / 2;
      var buffers = g.GetPositionNormalIndexedTriangleBuffer(material);

      if (null != buffers.PositionNormalIndexedTriangleBuffer)
      {
        var buffer = buffers.PositionNormalIndexedTriangleBuffer;
        var offs = buffer.VertexCount;

        foreach (var entry in _geometry.VerticesAndNormalsForSphere)
        {
          var pt = centerLocation + radius * (VectorD3D)entry.Item1;
          var nm = entry.Item2;
          buffer.AddTriangleVertex(pt.X, pt.Y, pt.Z, nm.X, nm.Y, nm.Z);
        }
        foreach (var idx in _geometry.TriangleIndicesForSphere)
        {
          buffer.AddTriangleIndices(idx.Item1 + offs, idx.Item2 + offs, idx.Item3 + offs);
        }
      }
    }
  }
}
