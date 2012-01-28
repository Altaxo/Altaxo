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
			const float _arrowHShaft = 0.15f;
			const float _arrowHWidth = 0.3f;

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


			/// <summary>Draws the grip in the graphics context.</summary>
			/// <param name="g">Graphics context.</param>
			/// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
			public void Show(Graphics g, double pageScale)
			{
				var pts = (PointF[])_outsideArrowPoints.Clone();
				_spanningHalfYRhombus.TransformPoints(pts);
				g.FillPolygon(Brushes.Blue, pts);
			}

			public bool IsGripHitted(PointD2D point)
			{
				point = _spanningHalfYRhombus.InverseTransformPoint(point);
				return Calc.RMath.IsInIntervalCC(point.X, -0.1, 1) && Calc.RMath.IsInIntervalCC(point.Y, -_arrowHWidth, _arrowHWidth);
			}



			#endregion




			static ResizeGripHandle()
			{
				// The arrow has a length of 1 and a maximum witdth of 2*arrowWidth and a shaft width of 2*arrowShaft
				_outsideArrowPoints = new PointF[] {
        new PointF(0,_arrowHShaft),
        new PointF(1-_arrowHWidth,_arrowHShaft),
        new PointF(1-_arrowHWidth,_arrowHWidth),
        new PointF(1,0),
        new PointF(1-_arrowHWidth, -_arrowHWidth),
        new PointF(1-_arrowHWidth, -_arrowHShaft),
        new PointF(0,-_arrowHShaft)
      };
			}


		}


	}
}
