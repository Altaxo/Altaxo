#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi.Shapes
{
	public abstract partial class GraphicBase 
	{
		protected class ResizeGripHandle : IGripManipulationHandle
		{
			static readonly PointF[] _outsideArrowPoints;

			IHitTestObject _parent;
			PointD2D _drawrPosition;
			PointD2D _fixrPosition;
			PointD2D _fixaPosition;
      PointD2D _initialMousePosition;
      PointD2D _initialSize;
			TransformationMatrix2D _spanningHalfYRhombus;


			private GraphicBase GraphObject { get { return (GraphicBase)_parent.HittedObject; } }

			public ResizeGripHandle(IHitTestObject parent, PointD2D relPos, TransformationMatrix2D spanningHalfYRhombus)
			{
				_parent = parent;
				_drawrPosition = relPos;
				_fixrPosition = new PointD2D(relPos.X == 0 ? 1 : 0, relPos.Y == 0 ? 1 : 0);
				_fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);
				_spanningHalfYRhombus = spanningHalfYRhombus;
			}

			/// <summary>
			/// Activates this grip, providing the initial position of the mouse.
			/// </summary>
			/// <param name="initialPosition">Initial position of the mouse.</param>
			/// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
			/// thie activation is due to a regular mouse click in this grip.</param>
			public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
			{
				_fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);
				
				initialPosition = _parent.Transformation.InverseTransformPoint(initialPosition);
        _initialMousePosition = GraphObject.ParentCoordinatesToLocalDifference(_fixaPosition, initialPosition);


        _initialSize = GraphObject.Size;
			}

			public bool Deactivate()
			{
				return false;
			}

			public void MoveGrip(PointD2D newPosition)
			{
        newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
        var diff = GraphObject.ParentCoordinatesToLocalDifference(_fixaPosition, newPosition);
        diff -= _initialMousePosition;

        GraphObject.SetBoundsFrom(_fixrPosition, _fixaPosition, _drawrPosition, diff, _initialSize);
			}

			#region IGripManipulationHandle Members


			public void Show(Graphics g)
			{
				var pts = (PointF[])_outsideArrowPoints.Clone();
				_spanningHalfYRhombus.TransformPoints(pts);
				g.FillPolygon(Brushes.Blue, pts);
			}

			public bool IsGripHitted(PointD2D point)
			{
				point = _spanningHalfYRhombus.InverseTransformPoint(point);
				return Calc.RMath.IsInIntervalCC(point.X, -0.5, 1) && Calc.RMath.IsInIntervalCC(point.Y, -1, 1);
			}



			#endregion




			static ResizeGripHandle()
			{
				// The arrow has a length of 1 and a maximum witdth of 2*arrowWidth and a shaft width of 2*arrowShaft
				const float arrowShaft = 0.15f;
				const float arrowWidth = 0.3f;
				_outsideArrowPoints = new PointF[] {
        new PointF(0,arrowShaft),
        new PointF(1-arrowWidth,arrowShaft),
        new PointF(1-arrowWidth,arrowWidth),
        new PointF(1,0),
        new PointF(1-arrowWidth, -arrowWidth),
        new PointF(1-arrowWidth, -arrowShaft),
        new PointF(0,-arrowShaft)
      };
			}


		}


	}
}
