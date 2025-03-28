﻿#region Copyright

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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Geometry;
using Altaxo.Main;

namespace Altaxo.Graph.Gdi.Shapes
{
  public abstract partial class GraphicBase
  {
    protected class RotationGripHandle : IGripManipulationHandle
    {
      private static GraphicsPath _rotationGripShape;

      private IHitTestObject _parent;
      private PointD2D _drawrPosition;
      private PointD2D _fixrPosition;
      private PointD2D _fixaPosition;
      private MatrixD2D _spanningHalfYRhombus;
      private bool _hasMoved;

      public RotationGripHandle(IHitTestObject parent, PointD2D relPos, MatrixD2D spanningHalfYRhombus)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointD2D(0.5, 0.5);
        _fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);
        _spanningHalfYRhombus = spanningHalfYRhombus;
      }

      private GraphicBase GraphObject { get { return (GraphicBase)_parent.HittedObject; } }

      /// <summary>
      /// Activates this grip, providing the initial position of the mouse.
      /// </summary>
      /// <param name="initialPosition">Initial position of the mouse.</param>
      /// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
      /// thie activation is due to a regular mouse click in this grip.</param>
      public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
      {
        _fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);
      }

      /// <summary>
      /// Announces the deactivation of this grip.
      /// </summary>
      /// <returns>The grip level, that should be displayed next, or -1 when the level should not change.</returns>
      public bool Deactivate()
      {
        if (_hasMoved)
        {
          GraphObject.EhSelfChanged(EventArgs.Empty);
        }

        return false;
      }

      public void MoveGrip(PointD2D newPosition)
      {
        newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
        var diff = new PointD2D(newPosition.X - _fixaPosition.X, newPosition.Y - _fixaPosition.Y);
        GraphObject.SetRotationFrom(_fixrPosition, _fixaPosition, _drawrPosition, diff, Main.EventFiring.Suppressed);
        _hasMoved = true;
      }

      /// <summary>Draws the grip in the graphics context.</summary>
      /// <param name="g">Graphics context.</param>
      /// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
      public void Show(Graphics g, double pageScale)
      {
        var shape = (GraphicsPath)_rotationGripShape.Clone();
        shape.Transform(_spanningHalfYRhombus.ToGdi());
        g.FillPath(Brushes.Blue, shape);
      }

      public bool IsGripHitted(PointD2D point)
      {
        point = _spanningHalfYRhombus.InverseTransformPoint(point);
        return Calc.RMath.IsInIntervalCC(point.X, 0, 1) && Calc.RMath.IsInIntervalCC(point.Y, -1, 1);
      }

      public bool IsGrippedObjectDisposed
      {
        get
        {
          return _parent?.HittedObject switch
          {
            IDocumentLeafNode dln => dln.IsDisposed,
            _ => false,
          };
        }
      }

      static RotationGripHandle()
      {
        const float ra = 2.1f / 2;
        const float ri = 1.5f / 2;
        const float rotArrowWidth = 0.6f;
        const float rii = (ra + ri - rotArrowWidth) / 2;
        const float raa = (ra + ri + rotArrowWidth) / 2;
        float cos45 = (float)Math.Sqrt(0.5);
        float sin45 = cos45;
        _rotationGripShape = new GraphicsPath();

        _rotationGripShape.AddArc(-ri, -ri, 2 * ri, 2 * ri, -45, 90); // mit Innenradius beginnen

        _rotationGripShape.AddLines(new PointF[]
      {
        new PointF(rii*cos45, rii*sin45),
        new PointF(rii*cos45, rii*sin45 + rotArrowWidth*cos45),
        new PointF(raa*cos45, raa*sin45)
      });

        _rotationGripShape.AddArc(-ra, -ra, 2 * ra, 2 * ra, 45, -90); // Außenradius

        _rotationGripShape.AddLines(new PointF[]
      {
        new PointF(raa*cos45, -raa*sin45),
        new PointF(rii*cos45, -rii*sin45 - rotArrowWidth*cos45),
        new PointF(rii*cos45, -rii*sin45),
      });

        _rotationGripShape.CloseFigure();
      }
    }
  }
}
