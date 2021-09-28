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
using System.Linq;
using System.Text;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  using Geometry;

  /// <summary>
  ///
  /// </summary>
  public class Ellipsoid : SolidBodyShapeBase
  {
    #region Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">The information.</param>
    protected Ellipsoid(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    : base(info)
    {
    }

    /// <summary>
    /// 2016-03-01 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Ellipsoid), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Ellipsoid)obj;
        info.AddBaseValueEmbedded(s, typeof(Ellipsoid).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (Ellipsoid?)o ?? new Ellipsoid(info);

        info.GetBaseValueEmbedded(s, typeof(Ellipsoid).BaseType!, parent);

        return s;
      }
    }

    #endregion Serialization

    public Ellipsoid()
    {
      Size = new Geometry.VectorD3D(100, 100, 100);
    }

    public Ellipsoid(Ellipsoid from)
      : base(from)
    {
    }

    public override object Clone()
    {
      return new Ellipsoid(this);
    }

    public override void Paint(IGraphicsContext3D g, IPaintContext context)
    {
      var buffers = g.GetPositionNormalIndexedTriangleBuffer(_material);

      if (buffers.PositionNormalIndexedTriangleBuffer is not null)
      {
        var buffer = buffers.PositionNormalIndexedTriangleBuffer;

        var offs = buffer.VertexCount;

        var sphere = new SolidIcoSphere(3); // gives a sphere with radius = 1

        var bounds = Bounds;

        double sx = Bounds.SizeX / 2;
        double sy = Bounds.SizeY / 2;
        double sz = Bounds.SizeZ / 2;

        var dx = Bounds.X + sx;
        var dy = Bounds.Y + sy;
        var dz = Bounds.Z + sz;

        var transformation = Matrix4x3.FromScaleShearRotationDegreeTranslation(sx, sy, sz, 0, 0, 0, 0, 0, 0, dx, dy, dz);
        transformation.AppendTransform(_transformation);

        var normalTransform = transformation.GetTransposedInverseMatrix3x3();

        foreach (var entry in sphere.VerticesAndNormalsForSphere)
        {
          var pt = transformation.Transform(entry.Item1);
          var nm = normalTransform.Transform(entry.Item2).Normalized;
          buffer.AddTriangleVertex(pt.X, pt.Y, pt.Z, nm.X, nm.Y, nm.Z);
        }
        foreach (var idx in sphere.TriangleIndicesForSphere)
        {
          buffer.AddTriangleIndices(idx.Item1 + offs, idx.Item2 + offs, idx.Item3 + offs);
        }
      }
    }

    /// <summary>
    /// Gets the object outline for arrangements.
    /// </summary>
    /// <param name="localToWorldTransformation">The local to world transformation.</param>
    /// <returns></returns>
    public override IObjectOutlineForArrangements GetObjectOutlineForArrangements(Matrix4x3 localToWorldTransformation)
    {
      var bounds = Bounds;

      double sx = Bounds.SizeX / 2;
      double sy = Bounds.SizeY / 2;
      double sz = Bounds.SizeZ / 2;

      var dx = Bounds.X + sx;
      var dy = Bounds.Y + sy;
      var dz = Bounds.Z + sz;

      var transformation = Matrix4x3.FromScaleShearRotationDegreeTranslation(sx, sy, sz, 0, 0, 0, 0, 0, 0, dx, dy, dz); // represents a transformation from a unit sphere to the real sphere

      transformation.AppendTransform(_transformation); // additional transformations of the ellipsoid
      transformation.AppendTransform(localToWorldTransformation); // local to global transformation

      return new SphericalObjectOutline(transformation);
    }

    #region ObjectOutline

    /// <summary>
    /// Represents the outline of an ellipsoid.
    /// </summary>
    /// <seealso cref="Altaxo.Graph.Graph3D.IObjectOutlineForArrangements" />
    /// <remarks>For calculation, see internal document "Eingeschlossenes beliebig transformiertes Ellipsoid"</remarks>
    private class SphericalObjectOutline : IObjectOutlineForArrangements
    {
      private Matrix4x3 _transformation;

      internal SphericalObjectOutline(Matrix4x3 transformation)
      {
        _transformation = transformation;
      }

      public RectangleD3D GetBounds()
      {
        var lx = new VectorD3D(_transformation.M11, _transformation.M21, _transformation.M31).Length;
        var ly = new VectorD3D(_transformation.M12, _transformation.M22, _transformation.M32).Length;
        var lz = new VectorD3D(_transformation.M13, _transformation.M23, _transformation.M33).Length;

        return new RectangleD3D(new PointD3D(_transformation.M41 - lx, _transformation.M42 - ly, _transformation.M43 - lz), new VectorD3D(2 * lx, 2 * ly, 2 * lz));
      }

      public RectangleD3D GetBounds(Matrix3x3 additionalTransformation)
      {
        var t = _transformation.WithAppendedTransformation(additionalTransformation);

        var lx = new VectorD3D(t.M11, t.M21, t.M31).Length;
        var ly = new VectorD3D(t.M12, t.M22, t.M32).Length;
        var lz = new VectorD3D(t.M13, t.M23, t.M33).Length;

        return new RectangleD3D(new PointD3D(t.M41 - lx, t.M42 - ly, t.M43 - lz), new VectorD3D(2 * lx, 2 * ly, 2 * lz));
      }
    }

    #endregion ObjectOutline
  }
}
