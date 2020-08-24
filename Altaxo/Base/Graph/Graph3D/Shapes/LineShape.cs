#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  [Serializable]
  public class LineShape : OpenPathShapeBase
  {
    #region Serialization

    /// <summary>
    /// 2016-04-19 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineShape), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LineShape)obj;
        info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LineShape?)o ?? new LineShape(info);
        info.GetBaseValueEmbedded(s, typeof(LineShape).BaseType!, parent);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    protected LineShape(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    public LineShape(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirect(), context)
    {
    }

    public LineShape(LineShape from)
      : base(from)
    {
    }

    public LineShape(PointD3D startPosition, Altaxo.Main.Properties.IReadOnlyPropertyBag? context)
      : base(new ItemLocationDirect(), context)
    {
      Position = startPosition;
    }

    public LineShape(PointD3D startPosition, PointD3D endPosition, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      :
      this(startPosition, context)
    {
      _location.SizeX = RADouble.NewAbs(endPosition.X - startPosition.X);
      _location.SizeY = RADouble.NewAbs(endPosition.Y - startPosition.Y);
      _location.SizeZ = RADouble.NewAbs(endPosition.Z - startPosition.Z);
    }

    public LineShape(PointD3D startPosition, PointD3D endPosition, double lineWidth, NamedColor lineColor)
      :
      this(startPosition, null)
    {
      _location.SizeX = RADouble.NewAbs(endPosition.X - startPosition.X);
      _location.SizeY = RADouble.NewAbs(endPosition.Y - startPosition.Y);
      _location.SizeZ = RADouble.NewAbs(endPosition.Z - startPosition.Z);
      _linePen = _linePen.WithUniformThickness(lineWidth).WithColor(lineColor);
    }

    #endregion Constructors

    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    public override object Clone()
    {
      return new LineShape(this);
    }

    /// <summary>
    /// Gets the path of the object in object world coordinates.
    /// </summary>
    /// <returns></returns>
    public override IObjectOutlineForArrangements GetObjectOutlineForArrangements(Matrix4x3 localToWorldTransformation)
    {
      return new LineShapeObjectOutline(_transformation.WithAppendedTransformation(localToWorldTransformation), Bounds);
    }

    public override IHitTestObject? HitTest(HitTestPointData parentHitData)
    {
      IHitTestObject? result = null;
      var localHitData = parentHitData.NewFromAdditionalTransformation(_transformation);
      if (localHitData.IsHit(new LineD3D(Bounds.Location, Bounds.LocationPlusSize), _linePen.Thickness1, _linePen.Thickness2))
      {
        result = GetNewHitTestObject(parentHitData.WorldTransformation);
      }

      if (result != null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    protected override IHitTestObject GetNewHitTestObject(Matrix4x3 localToWorldTransformation)
    {
      return new LineShapeHitTestObject(this, localToWorldTransformation);
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties", true);
      ((LineShape)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    public override void Paint(IGraphicsContext3D g, IPaintContext context)
    {
      var gs = g.SaveGraphicsState();
      g.PrependTransform(_transformation);
      g.DrawLine(_linePen, Bounds.Location, Bounds.LocationPlusSize);
      g.RestoreGraphicsState(gs);
    }

    protected override IGripManipulationHandle[] GetGrips(IHitTestObject hitTest, GripKind gripKind)
    {
      var list = new List<IGripManipulationHandle>();

      /*

const double gripNominalSize = 10; // 10 Points nominal size on the screen
            if ((GripKind.Resize & gripKind) != 0)
            {
                double gripSize = gripNominalSize / pageScale; // 10 Points, but we have to consider the current pageScale
                for (int i = 1; i < _gripRelPositions.Length; i++)
                {
                    PointD2D outVec, pos;
                    if (1 == i % 2)
                        GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);
                    else
                        GetMiddleRayOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

                    outVec *= (gripSize / outVec.VectorLength);
                    PointD2D altVec = outVec.Get90DegreeRotated();
                    PointD2D ptStart = pos;
                    list.Add(new ResizeGripHandle(hitTest, _gripRelPositions[i], new MatrixD2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
                }
            }
            */

      /*
            if ((GripKind.Rotate & gripKind) != 0)
            {
                double gripSize = 10 / pageScale;
                // Rotation grips
                for (int i = 1; i < _gripRelPositions.Length; i += 2)
                {
                    PointD2D outVec, pos;
                    GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

                    outVec *= (gripSize / outVec.VectorLength);
                    PointD2D altVec = outVec.Get90DegreeRotated();
                    PointD2D ptStart = pos;
                    list.Add(new RotationGripHandle(hitTest, _gripRelPositions[i], new MatrixD2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
                }
            }
            */

      /*
            if ((GripKind.Rescale & gripKind) != 0)
            {
                double gripSize = 10 / pageScale; // 10 Points, but we have to consider the current pageScale
                for (int i = 1; i < _gripRelPositions.Length; i++)
                {
                    PointD2D outVec, pos;
                    if (1 == i % 2)
                        GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);
                    else
                        GetMiddleRayOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

                    outVec *= (gripSize / outVec.VectorLength);
                    PointD2D altVec = outVec.Get90DegreeRotated();
                    PointD2D ptStart = pos;
                    list.Add(new RescaleGripHandle(hitTest, _gripRelPositions[i], new MatrixD2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
                }
            }
            */

      /*
            if ((GripKind.Shear & gripKind) != 0)
            {
                double gripSize = 10 / pageScale; // 10 Points, but we have to consider the current pageScale
                for (int i = 2; i < _gripRelPositions.Length; i += 2)
                {
                    PointD2D outVec, pos;
                    GetEdgeOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

                    outVec *= (gripSize / outVec.VectorLength);
                    PointD2D altVec = outVec.Get90DegreeRotated();
                    PointD2D ptStart = pos;
                    list.Add(new ShearGripHandle(hitTest, _gripRelPositions[i], new MatrixD2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
                }
            }
            */

      if ((GripKind.Move & gripKind) != 0)
      {
        var bounds = Bounds;
        var wn = PolylineMath3D.GetWestNorthVectors(bounds.Size);
        var transformation = Matrix4x3.NewFromBasisVectorsAndLocation(wn.Item1, wn.Item2, bounds.Size.Normalized, PointD3D.Empty);

        transformation.AppendTransform(_transformation);
        transformation.AppendTransform(hitTest.Transformation);

        double t1 = 0.55 * _linePen.Thickness1;
        double t2 = 0.55 * _linePen.Thickness2;
        var rect = new RectangleD3D(-t1, -t2, 0, 2 * t1, 2 * t2, bounds.Size.Length);
        var objectOutline = new RectangularObjectOutline(rect, transformation);
        list.Add(new MovementGripHandle(hitTest, objectOutline, null));
      }

      return list.ToArray();
    }

    protected class LineShapeHitTestObject : GraphicBaseHitTestObject
    {
      public LineShapeHitTestObject(LineShape parent, Matrix4x3 localToWorldTransformation)
        : base(parent, localToWorldTransformation)
      {
      }

      public override IGripManipulationHandle[]? GetGrips(int gripLevel)
      {
        if (gripLevel <= 1)
        {
          var ls = (LineShape)_hitobject;
          var pts = new PointD3D[] { PointD3D.Empty, (PointD3D)ls.Size };
          for (int i = 0; i < pts.Length; i++)
          {
            var pt = ls._transformation.Transform(pts[i]);
            pt = Transformation.Transform(pt);
            pts[i] = pt;
          }

          var grips = new IGripManipulationHandle[gripLevel == 0 ? 1 : 3];

          // Translation grips
          var bounds = ls.Bounds;
          var wn = PolylineMath3D.GetWestNorthVectors(bounds.Size);
          var transformation = Matrix4x3.NewFromBasisVectorsAndLocation(wn.Item1, wn.Item2, bounds.Size.Normalized, PointD3D.Empty);

          transformation.AppendTransform(ls._transformation);
          transformation.AppendTransform(Transformation);

          double t1 = 0.55 * ls._linePen.Thickness1;
          double t2 = 0.55 * ls._linePen.Thickness2;
          var rect = new RectangleD3D(-t1, -t2, 0, 2 * t1, 2 * t2, bounds.Size.Length);
          var objectOutline = new RectangularObjectOutline(rect, transformation);
          grips[0] = new MovementGripHandle(this, objectOutline, null);

          // PathNode grips
          if (gripLevel == 1)
          {
            grips[2] = grips[0]; // put the movement grip to the background, the two NodeGrips need more priority
            var gripRadius = Math.Max(t1, t2);
            grips[0] = new PathNodeGripHandle(this, new VectorD3D(0, 0, 0), pts[0], gripRadius);
            grips[1] = new PathNodeGripHandle(this, new VectorD3D(1, 1, 1), pts[1], gripRadius);
          }
          return grips;
        }
        else
        {
          return base.GetGrips(gripLevel);
        }
      }
    }

    private class LineShapeObjectOutline : IObjectOutlineForArrangements
    {
      private Matrix4x3 _transformation;
      private RectangleD3D _bounds;

      internal LineShapeObjectOutline(Matrix4x3 transformation, RectangleD3D bounds)
      {
        _transformation = transformation;
        _bounds = bounds;
      }

      private IEnumerable<PointD3D> Points
      {
        get
        {
          yield return _bounds.Location;
          yield return _bounds.LocationPlusSize;
        }
      }

      public RectangleD3D GetBounds()
      {
        return RectangleD3D.NewRectangleIncludingAllPoints(Points.Select(p => _transformation.Transform(p)));
      }

      public RectangleD3D GetBounds(Matrix3x3 additionalTransformation)
      {
        return RectangleD3D.NewRectangleIncludingAllPoints(Points.Select(p => additionalTransformation.Transform(_transformation.Transform(p))));
      }
    }
  } // End Class
} // end Namespace
