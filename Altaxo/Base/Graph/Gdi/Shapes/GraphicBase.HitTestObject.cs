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
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  public abstract partial class GraphicBase
  {
    protected class GraphicBaseHitTestObject : HitTestObjectBase
    {
      /// <summary>
      /// Creates a new HitTestObject.
      /// </summary>
      /// <param name="parent">The hitted object.</param>
      public GraphicBaseHitTestObject(GraphicBase parent)
        : base(parent)
      {
      }

      /// <summary>
      /// Shifts the position of the object by x and y. Used to arrange objects.
      /// </summary>
      /// <param name="dx">Shift value of x in page coordinates.</param>
      /// <param name="dy">Shift value of y in page coordinates.</param>
      public override void ShiftPosition(double dx, double dy)
      {
        if (_hitobject is GraphicBase)
        {
          var deltaPos = _matrix.InverseTransformVector(new PointD2D(dx, dy)); // Transform to the object's parent coordinates
          ((GraphicBase)_hitobject).X += deltaPos.X;
          ((GraphicBase)_hitobject).Y += deltaPos.Y;
        }
      }

      public override void ChangeSize(double? x, double? y)
      {
        var hit = _hitobject as GraphicBase;

        PointD2D currentSizeRootCoord = ObjectOutlineForArrangements.GetBounds().Size.ToPointD2D();
        PointD2D destinationSizeRootCoord = currentSizeRootCoord;
        if (x.HasValue)
          destinationSizeRootCoord = destinationSizeRootCoord.WithX(x.Value);
        if (y.HasValue)
          destinationSizeRootCoord = destinationSizeRootCoord.WithY(y.Value);

        if (hit is not null)
        {
          if (!hit.AutoSize)
          {
            var t = _matrix.Clone();
            t.AppendTransform(hit._transformation);
            var innerRect = RectangleD2DExtensions.GetIncludedTransformedRectangle(new RectangleD2D(PointD2D.Empty, destinationSizeRootCoord), t.SX, t.RX, t.RY, t.SY);
            hit.Width = innerRect.Width;
            hit.Height = innerRect.Height;
          }
        }
      }

      public override GraphicsPath ObjectOutlineForArrangements
      {
        get
        {
          var objPath = ((GraphicBase)_hitobject).GetObjectOutlineForArrangements();
          objPath.Transform(((GraphicBase)_hitobject)._transformation.ToGdi());
          objPath.Transform(Transformation.ToGdi());
          return objPath;
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

      public override IGripManipulationHandle[]? GetGrips(double pageScale, int gripLevel)
      {
        if (((GraphicBase)_hitobject).AutoSize)
        {
          switch (gripLevel)
          {
            case 0:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move);

            case 1:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rotate);

            case 2:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rescale);

            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Shear);
          }
        }
        else // a normal object
        {
          switch (gripLevel)
          {
            case 0:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move);

            case 1:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Resize);

            case 2:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rotate);

            case 3:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Shear);

            case 4:
              return ((GraphicBase)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rescale);
          }
        }
        return null;
      }
    }
  }
}
