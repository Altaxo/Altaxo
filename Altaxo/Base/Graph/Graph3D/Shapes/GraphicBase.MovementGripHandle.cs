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
using System.Drawing.Drawing2D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  public abstract partial class GraphicBase
  {
    protected class MovementGripHandle : IGripManipulationHandle
    {
      private IHitTestObject _parent;

      /// <summary>Path active for selection of this grip.</summary>
      private IObjectOutline _gripPath;

      /// <summary>Path that is shown on the display.</summary>
      private IObjectOutline _displayedPath;

      private bool _hasMoved;
      private bool _wasActivatedUponCreation;
      private HitTestPointData? _initialMousePosition;
      private PointD3D _initialObjectPosition;

      /// <summary>
      /// Initializes a new instance of the <see cref="MovementGripHandle"/> class.
      /// </summary>
      /// <param name="parent">The parent hit test object.</param>
      /// <param name="gripPath">The grip path, i.e. the outline that is used to test whether this grip is hitted with the mouse.</param>
      /// <param name="displayPath">The display path, i.e. the outline that is displayed on the plot.</param>
      public MovementGripHandle(IHitTestObject parent, IObjectOutline gripPath, IObjectOutline? displayPath)
      {
        if (parent is null)
          throw new ArgumentNullException(nameof(parent));
        if (gripPath is null)
          throw new ArgumentNullException(nameof(gripPath));

        _parent = parent;
        _gripPath = gripPath;
        _displayedPath = displayPath ?? gripPath;
      }

      #region IGripManipulationHandle Members

      /// <summary>
      /// Activates this grip, providing the initial position of the mouse.
      /// </summary>
      /// <param name="initialPosition">Initial position of the mouse.</param>
      /// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
      /// thie activation is due to a regular mouse click in this grip.</param>
      public void Activate(HitTestPointData initialPosition, bool isActivatedUponCreation)
      {
        if (null == initialPosition)
          throw new ArgumentNullException(nameof(initialPosition));

        _wasActivatedUponCreation = isActivatedUponCreation;
        _initialMousePosition = initialPosition;
        _initialObjectPosition = ((GraphicBase)_parent.HittedObject).Position;
        _hasMoved = false;
      }

      /// <summary>
      /// Announces the deactivation of this grip.
      /// </summary>
      /// <returns>The grip level, that should be displayed next, or -1 when the level should not change.</returns>
      public bool Deactivate()
      {
        if (_hasMoved)
          ((GraphicBase)_parent.HittedObject).EhSelfChanged(EventArgs.Empty);

        bool isGraphics = _parent.HittedObject is GraphicBase;
        if (isGraphics && !_hasMoved && !_wasActivatedUponCreation)
          return true;
        else
          return false;
      }

      public void MoveGrip(HitTestPointData newPosition)
      {
        if (_initialMousePosition is null)
          throw new InvalidProgramException($"{nameof(_initialMousePosition)} is null, please call {nameof(Activate)} before!");

        var objectToMove = ((GraphicBase)_parent.HittedObject);
        VectorD3D diff = GetMoveVectorInWorldCoordinates(_initialMousePosition, newPosition, _initialObjectPosition);

        if (!diff.IsEmpty)
          _hasMoved = true;

        diff = _parent.Transformation.InverseTransform(diff); // Transform from world to local coordinates

        objectToMove.SilentSetPosition(_initialObjectPosition + diff);
      }

      /// <summary>
      /// Calculates a difference vector for moving a handle or an object.
      /// </summary>
      /// <param name="initialMousePosition">The initial mouse position at begin of the move operation.</param>
      /// <param name="currentMousePosition">The current mouse position.</param>
      /// <param name="initialObjectHitPositionLocalCoordinates">The initial position in local coordinates where the object or the handle was hit.</param>
      /// <returns>A difference vector (in world coordinates) that can be used to move the object or handle around.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException"></exception>
      public static VectorD3D GetMoveVectorInWorldCoordinates(HitTestPointData initialMousePosition, HitTestPointData currentMousePosition, PointD3D initialObjectHitPositionLocalCoordinates)
      {
        var m = initialMousePosition.HitTransformation; // initial ray position
        var n = currentMousePosition.HitTransformation;           // current ray position

        double x = initialObjectHitPositionLocalCoordinates.X;
        double y = initialObjectHitPositionLocalCoordinates.Y;
        double z = initialObjectHitPositionLocalCoordinates.Z;

        // For the mathematics behind the following, see internal document "3D_MoveObjectByMovingRay"

        double denom = m.M33 * n.M12 * n.M21 - m.M33 * n.M11 * n.M22 - m.M23 * n.M12 * n.M31 +
   m.M13 * n.M22 * n.M31 + m.M23 * n.M11 * n.M32 - m.M13 * n.M21 * n.M32;

        if (0 == denom)
          throw new ArgumentOutOfRangeException();

        double dx = m.M23 * (-(m.M42 * n.M31) +
      n.M32 * (m.M41 - n.M41 + m.M11 * x - n.M11 * x + m.M21 * y -
         n.M21 * y + m.M31 * z) +
      n.M31 * (n.M42 - m.M12 * x + n.M12 * x - m.M22 * y +
         n.M22 * y - m.M32 * z)) +
   m.M33 * (m.M42 * n.M21 -
      n.M22 * (m.M41 - n.M41 + m.M11 * x - n.M11 * x + m.M21 * y) -
      (m.M31 * n.M22 - n.M22 * n.M31 + n.M21 * n.M32) * z +
      n.M21 * (-n.M42 + m.M12 * x - n.M12 * x + m.M22 * y + m.M32 * z));

        double dy = m.M13 * (m.M42 * n.M31 - n.M32 *
       (m.M41 - n.M41 + m.M11 * x - n.M11 * x + m.M21 * y -
         n.M21 * y + m.M31 * z) +
      n.M31 * (-n.M42 + m.M12 * x - n.M12 * x + m.M22 * y -
         n.M22 * y + m.M32 * z)) +
   m.M33 * (-(m.M42 * n.M11) + n.M12 * (m.M41 - n.M41) +
      n.M12 * (m.M11 * x + m.M21 * y - n.M21 * y + m.M31 * z -
         n.M31 * z) + n.M11 *
       (n.M42 - m.M12 * x - m.M22 * y + n.M22 * y - m.M32 * z +
         n.M32 * z));

        double dz = m.M13 * (-(m.M42 * n.M21) +
      n.M22 * (m.M41 - n.M41 + m.M11 * x - n.M11 * x + m.M21 * y) +
      (m.M31 * n.M22 - n.M22 * n.M31 + n.M21 * n.M32) * z +
      n.M21 * (n.M42 - m.M12 * x + n.M12 * x - m.M22 * y - m.M32 * z))
     + m.M23 * (m.M42 * n.M11 + n.M12 * (-m.M41 + n.M41) -
      n.M12 * (m.M11 * x + m.M21 * y - n.M21 * y + m.M31 * z -
         n.M31 * z) + n.M11 *
       (-n.M42 + m.M12 * x + m.M22 * y - n.M22 * y + m.M32 * z -
         n.M32 * z));

        var diff = new VectorD3D(dx / denom, dy / denom, dz / denom);
        return diff;
      }

      /// <summary>Draws the grip in the graphics context.</summary>
      /// <param name="g">Graphics context.</param>
      public void Show(IOverlayContext3D g)
      {
        var buf = g.PositionColorLineListBuffer;
        foreach (var line in _displayedPath.AsLines)
        {
          buf.AddLine(line.P0.X, line.P0.Y, line.P0.Z, line.P1.X, line.P1.Y, line.P1.Z, 0, 0, 1, 1);
        }
      }

      public bool IsGripHit(HitTestPointData point)
      {
        return _gripPath.IsHittedBy(point);
      }

      #endregion IGripManipulationHandle Members
    }
  }
}
