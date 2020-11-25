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

#nullable enable
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
  public sealed class TetrahedronUp : ScatterSymbolShapeBase
  {
    public static TetrahedronUp Instance { get; private set; } = new TetrahedronUp();

    private static readonly double SinArcCos1By3 = Math.Sin(Math.Acos(1 / 3.0)); // Radius of an arm if projected to the x-y-plane
    private const double MinusOneBy3 = -1 / 3.0;
    private const double Cos120 = -0.5;
    private static readonly double Sin120 = 0.5 * Math.Sqrt(3);

    #region Serialization

    /// <summary>
    /// 2016-07-01 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TetrahedronUp), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SerializeV0((IScatterSymbol)obj, info);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return DeserializeV0(Instance, info, parent);
      }
    }

    #endregion Serialization

    public override void Paint(IGraphicsContext3D g, IMaterial material, PointD3D centerLocation, double symbolSize)
    {
      var radius = symbolSize / 2;
      var buffers = g.GetPositionNormalIndexedTriangleBuffer(material);

      if (buffers.PositionNormalIndexedTriangleBuffer is not null)
      {
        var buffer = buffers.PositionNormalIndexedTriangleBuffer;
        var offs = buffer.VertexCount;

        foreach (var entry in GetVerticesAndNormals())
        {
          var pt = centerLocation + radius * entry.Item1;
          var nm = entry.Item2;
          buffer.AddTriangleVertex(pt.X, pt.Y, pt.Z, nm.X, nm.Y, nm.Z);
        }
        foreach (var idx in GetTriangleIndices())
        {
          buffer.AddTriangleIndices(idx.Item1 + offs, idx.Item2 + offs, idx.Item3 + offs);
        }
      }
    }

    public static IEnumerable<Tuple<VectorD3D, VectorD3D>> GetVerticesAndNormals()
    {
      var pt0 = new VectorD3D(0, -SinArcCos1By3, MinusOneBy3);
      var pt1 = new VectorD3D(SinArcCos1By3 * Sin120, -SinArcCos1By3 * Cos120, MinusOneBy3);
      var pt2 = new VectorD3D(-SinArcCos1By3 * Sin120, -SinArcCos1By3 * Cos120, MinusOneBy3);
      var pt3 = new VectorD3D(0, 0, 1);

      // note: the normals are the negative vectors of the points

      // Bottom
      yield return new Tuple<VectorD3D, VectorD3D>(pt0, -pt3);
      yield return new Tuple<VectorD3D, VectorD3D>(pt2, -pt3);
      yield return new Tuple<VectorD3D, VectorD3D>(pt1, -pt3);

      yield return new Tuple<VectorD3D, VectorD3D>(pt0, -pt2);
      yield return new Tuple<VectorD3D, VectorD3D>(pt1, -pt2);
      yield return new Tuple<VectorD3D, VectorD3D>(pt3, -pt2);

      yield return new Tuple<VectorD3D, VectorD3D>(pt1, -pt0);
      yield return new Tuple<VectorD3D, VectorD3D>(pt2, -pt0);
      yield return new Tuple<VectorD3D, VectorD3D>(pt3, -pt0);

      yield return new Tuple<VectorD3D, VectorD3D>(pt2, -pt1);
      yield return new Tuple<VectorD3D, VectorD3D>(pt0, -pt1);
      yield return new Tuple<VectorD3D, VectorD3D>(pt3, -pt1);
    }

    public static IEnumerable<Tuple<int, int, int>> GetTriangleIndices()
    {
      yield return new Tuple<int, int, int>(0, 1, 2);
      yield return new Tuple<int, int, int>(3, 4, 5);
      yield return new Tuple<int, int, int>(6, 7, 8);
      yield return new Tuple<int, int, int>(9, 10, 11);
    }
  }
}
