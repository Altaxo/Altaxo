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
		protected class RescaleGripHandle : IGripManipulationHandle
		{
			static readonly PointF[] _shapePoints;

			IHitTestObject _parent;
			PointD2D _drawrPosition;
			PointD2D _drawaPosition;
			PointD2D _fixrPosition;
			PointD2D _fixaPosition;
			PointD2D _initialMousePosition;
			double _initialScaleX, _initialScaleY;
			TransformationMatrix2D _spanningHalfYRhombus;


			private GraphicBase GraphObject { get { return (GraphicBase)_parent.HittedObject; } }

			public RescaleGripHandle(IHitTestObject parent, PointD2D relPos, TransformationMatrix2D spanningHalfYRhombus)
			{
				_parent = parent;
				_drawrPosition = relPos;
				_drawaPosition = GraphObject.RelativeToAbsolutePosition(_drawrPosition, true);

				_fixrPosition = new PointD2D(1 - relPos.X, 1 - relPos.Y);
				_fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);

				_spanningHalfYRhombus = spanningHalfYRhombus;
			}


			#region IGripManipulationHandle Members

			public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
			{
				initialPosition = _parent.Transformation.InverseTransformPoint(initialPosition);
				_initialMousePosition = GraphObject.ToUnrotatedDifference(_fixaPosition, initialPosition);

				_fixaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixrPosition);
				_drawaPosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_drawrPosition);

				_initialScaleX = GraphObject.ScaleX;
				_initialScaleY = GraphObject.ScaleY;
			}

			public bool Deactivate()
			{
				return false;
			}

			public void MoveGrip(PointD2D newPosition)
			{
				newPosition = _parent.Transformation.InverseTransformPoint(newPosition);
				var diff = GraphObject.ToUnrotatedDifference(_fixaPosition, newPosition);
				diff -= _initialMousePosition;


				GraphObject.SetScalesFrom(_fixrPosition, _fixaPosition, _drawrPosition, diff, _initialScaleX, _initialScaleY);
			}

			/// <summary>Draws the grip in the graphics context.</summary>
			/// <param name="g">Graphics context.</param>
			/// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
			public void Show(Graphics g, double pageScale)
			{
				var pts = (PointF[])_shapePoints.Clone();
				_spanningHalfYRhombus.TransformPoints(pts);
				g.FillPolygon(Brushes.Blue, pts);
			}

			public bool IsGripHitted(PointD2D point)
			{
				point = _spanningHalfYRhombus.InverseTransformPoint(point);
				return Calc.RMath.IsInIntervalCC(point.X, 0, 2 * barX + stegX) && Calc.RMath.IsInIntervalCC(point.Y, -bigY, bigY);
			}

			#endregion



			const float bigY = 0.5f; // half heigth of the bar
			const float smallY = 0.125f; // half height of the steg
			const float barX = 0.33f; // width of the bar
			const float stegX = 0.2f; // width of the steg between the bars


			static RescaleGripHandle()
			{
				_shapePoints = new PointF[] {
        new PointF(0,-bigY),
        new PointF(barX, -bigY),
        new PointF(barX, -smallY),
        new PointF(barX+stegX,-smallY),
        new PointF(barX+stegX, -bigY),
        new PointF(2*barX+stegX, -bigY),
        new PointF(2*barX+stegX, bigY),
        new PointF(barX+stegX, bigY),
        new PointF(barX+stegX, smallY),
        new PointF(barX, smallY),
        new PointF(barX, bigY),
        new PointF(0, bigY),
          };
			}
		}
	}
}
