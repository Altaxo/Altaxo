﻿#region Copyright

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
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.Shapes
{
  public abstract partial class GraphicBase
  {
    protected class GraphicBaseHitTestObject : HitTestObjectBase
    {
      /// <summary>
      /// Creates a new HitTestObject.
      /// </summary>
      /// <param name="parent">The hitted object.</param>
      /// <param name="localToWorldTransformation">The transformation that transformes from the coordinate space in which the hitted object is embedded to world coordinates. This is usually the transformation from the layer coordinates to the root layer coordinates, but does not include the object's transformation.</param>
      public GraphicBaseHitTestObject(GraphicBase parent, Matrix4x3 localToWorldTransformation)
        : base(parent, localToWorldTransformation)
      {
      }

      /// <summary>
      /// Shifts the position of the object by x and y. Used to arrange objects.
      /// </summary>
      /// <param name="dx">Shift value of x in page coordinates.</param>
      /// <param name="dy">Shift value of y in page coordinates.</param>
      /// <param name="dz">Shift value of z in page coordinates.</param>
      public override void ShiftPosition(double dx, double dy, double dz)
      {
        if (_hitobject is GraphicBase)
        {
          var deltaPos = _matrix.InverseTransform(new VectorD3D(dx, dy, dz)); // Transform to the object's parent coordinates
          ((GraphicBase)_hitobject).X += deltaPos.X;
          ((GraphicBase)_hitobject).Y += deltaPos.Y;
          ((GraphicBase)_hitobject).Z += deltaPos.Z;
        }
      }

      public override void ChangeSize(double? x, double? y, double? z)
      {
        var hit = _hitobject as GraphicBase;

        if (hit is not null)
        {
          var deltaSize = _matrix.InverseTransform(new VectorD3D(x ?? 0, y ?? 0, z ?? 0)); // Transform to the object's parent coordinates
          hit.Size += deltaSize;
        }

        /*
                var currentSizeRootCoord = this.ObjectOutlineForArrangements.GetBounds().Size;
                var destinationSizeRootCoord = currentSizeRootCoord;
                if (x.HasValue)
                    destinationSizeRootCoord = destinationSizeRootCoord.WithX(x.Value);
                if (y.HasValue)
                    destinationSizeRootCoord = destinationSizeRootCoord.WithY(y.Value);
                if (z.HasValue)
                    destinationSizeRootCoord = destinationSizeRootCoord.WithZ(z.Value);

                if (null != hit)
                {
                    if (!hit.AutoSize)
                    {
                        var t = _matrix.WithAppendedTransformation(hit._transformation);
                        // this is the original to be implemented : var innerRect = RectangleD3DExtensions.GetIncludedTransformedRectangle(new RectangleD3D(PointD3D.Empty, destinationSizeRootCoord), t.SX, t.RX, t.RY, t.SY);
                        var innerRect = new RectangleD3D(PointD3D.Empty, destinationSizeRootCoord);
                        hit.Size = innerRect.Size;
                    }
                }
                */
      }

      public override IObjectOutlineForArrangements ObjectOutlineForArrangements
      {
        get
        {
          var ho = (GraphicBase)_hitobject;

          var result = ho.GetObjectOutlineForArrangements(_matrix);
          if (result is not null)
            return result; // if the hitted object provides an outline, it is used

          // the result has to be in root layer coordinates, but we must also take into account the object's own transformation
          var outline = new RectangularObjectOutline(ho.Bounds, _matrix.WithPrependedTransformation(ho._transformation));
          return new ObjectOutlineForArrangementsWrapper(outline);
        }
      }

      public override int GetNextGripLevel(int currentGripLevel)
      {
        int newLevel = 1 + currentGripLevel;
        int maxLevel = ((GraphicBase)_hitobject).AutoSize ? 3 : 4;
        if (newLevel > maxLevel)
          newLevel = 1;
        return newLevel;
      }

      public override IGripManipulationHandle[]? GetGrips(int gripLevel)
      {
        if (((GraphicBase)_hitobject).AutoSize)
        {
          switch (gripLevel)
          {
            case 0:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move);

            case 1:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rotate);

            case 2:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rescale);

            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Shear);
          }
        }
        else // a normal object
        {
          switch (gripLevel)
          {
            case 0:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move);

            case 1:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Resize);

            case 2:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rotate);

            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Shear);

            case 4:
              return ((GraphicBase)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rescale);
          }
        }
        return null;
      }
    }
  }
}
