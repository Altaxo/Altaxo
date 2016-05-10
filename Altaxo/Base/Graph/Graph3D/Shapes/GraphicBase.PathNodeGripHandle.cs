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

using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Graph3D.Shapes
{
	public abstract partial class GraphicBase
	{
		/// <summary>
		/// Shows a single round grip, which can be customized to a move action.
		/// </summary>
		protected class PathNodeGripHandle : IGripManipulationHandle
		{
			/// <summary>The location of the grip center in world coordinates (root layer coordinates).</summary>
			protected PointD3D _gripCenter;

			/// <summary>The radius of the grip to show.</summary>
			protected double _gripRadius;

			protected IHitTestObject _parent;

			/// <summary>The relative position of the node that should be moved. For instance, (0,0,0) is at the origin of the object, (1,1,1) on the other side of the object, (0.5, 0.5, 0.5) at the center of the object.</summary>
			protected VectorD3D _movePointRelativePosition;

			/// <summary>The relative position of the object's edge or vertex, which is held fixed during the operation.</summary>
			protected VectorD3D _fixPointRelativePosition;

			/// <summary>The variable <see cref="_fixPointRelativePosition"/>, converted to parent's (layer) coordinates.</summary>
			protected PointD3D _fixPointAbsolutePosition;

			/// <summary>The initial size of the object that was hit.</summary>
			protected VectorD3D _initialObjectSize;

			/// <summary>Initial mouse position at the time of activation of this handle.</summary>
			protected HitTestPointData _initialMousePosition;

			/// <summary>When set, this is a custom move action.</summary>
			private Action<HitTestPointData> _moveAction;

			/// <summary>True if this grip has moved.</summary>
			private bool _hasMoved;

			/// <summary>
			/// Initializes a new instance of the <see cref="PathNodeGripHandle"/> class.
			/// </summary>
			/// <param name="parent">The object that was hit.</param>
			/// <param name="movePointRelativePosition">The relative position of the node that should be moved. For instance, (0,0,0) is at the origin of the object, (1,1,1) on the other side of the object, (0.5, 0.5, 0.5) at the center of the object.</param>
			/// <param name="gripCenter">The grip center in local (layer) coordinates.</param>
			/// <param name="gripRadius">The grip radius.</param>
			public PathNodeGripHandle(IHitTestObject parent, VectorD3D movePointRelativePosition, PointD3D gripCenter, double gripRadius)
			{
				_parent = parent;
				_initialObjectSize = GraphObject.Size;
				_movePointRelativePosition = movePointRelativePosition;
				_fixPointRelativePosition = new VectorD3D(movePointRelativePosition.X == 0 ? 1 : 0, movePointRelativePosition.Y == 0 ? 1 : 0, movePointRelativePosition.Z == 0 ? 1 : 0);
				_fixPointAbsolutePosition = GraphObject.RelativeLocalToAbsoluteParentCoordinates(_fixPointRelativePosition);

				_gripCenter = gripCenter;
				_gripRadius = gripRadius;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="PathNodeGripHandle"/> class.
			/// </summary>
			/// <param name="parent">The object that was hit.</param>
			/// <param name="relPos">The relative position of the node that should be moved. For instance, (0,0,0) is at the origin of the object, (1,1,1) on the other side of the object, (0.5, 0.5, 0.5) at the center of the object.</param>
			/// <param name="gripCenter">The grip center in local (layer) coordinates.</param>
			/// <param name="gripRadius">The grip radius.</param>
			/// <param name="moveAction">Action to carry out when moving the handle.</param>
			public PathNodeGripHandle(IHitTestObject parent, VectorD3D relPos, PointD3D gripCenter, double gripRadius, Action<HitTestPointData> moveAction)
				: this(parent, relPos, gripCenter, gripRadius)
			{
				_moveAction = moveAction;
			}

			/// <summary>
			/// Gets the graph object that is to be manipulated.
			/// </summary>
			/// <value>
			/// The graph object.
			/// </value>
			protected GraphicBase GraphObject { get { return (GraphicBase)_parent.HittedObject; } }

			#region IGripManipulationHandle Members

			/// <summary>
			/// Activates this grip, providing the initial position of the mouse.
			/// </summary>
			/// <param name="initialPosition">Initial position of the mouse.</param>
			/// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
			/// thie activation is due to a regular mouse click in this grip.</param>
			public void Activate(HitTestPointData initialPosition, bool isActivatedUponCreation)
			{
				_initialMousePosition = initialPosition;
				_hasMoved = false;
			}

			/// <summary>
			/// Announces the deactivation of this grip.
			/// </summary>
			/// <returns>The grip level, that should be displayed next, or -1 when the level should not change.</returns>
			public virtual bool Deactivate()
			{
				if (_hasMoved)
					GraphObject.EhSelfChanged(EventArgs.Empty);

				return false;
			}

			/// <summary>
			/// Moves the grip to the new position.
			/// </summary>
			/// <param name="newPosition">The new position (of the mouse).</param>
			public virtual void MoveGrip(HitTestPointData newPosition)
			{
				if (_moveAction != null)
				{
					_moveAction(newPosition);
				}
				else
				{
					var diffWC = MovementGripHandle.GetMoveVectorInWorldCoordinates(_initialMousePosition, newPosition, _gripCenter); // in World coordinates
					var diffLC = _parent.Transformation.InverseTransform(diffWC); // now in local (layer) coordinates
					var diffOC = GraphObject._transformation.InverseTransform(diffLC); // now in object coordinates
					GraphObject.SetBoundsFrom(_fixPointRelativePosition, _fixPointAbsolutePosition, _movePointRelativePosition, diffOC, _initialObjectSize, Main.EventFiring.Suppressed);
					_hasMoved = true;
				}
			}

			/// <summary>Draws the grip in the graphics context.</summary>
			/// <param name="g">Graphics context.</param>
			public void Show(IOverlayContext3D g)
			{
				var buf = g.PositionColorLineListBuffer;

				var vec = new VectorD3D(_gripRadius, _gripRadius, _gripRadius);
				var rect = new RectangleD3D(_gripCenter - vec, 2 * vec);

				foreach (var line in rect.Edges)
					buf.AddLine(line.P0.X, line.P0.Y, line.P0.Z, line.P1.X, line.P1.Y, line.P1.Z, 1, 0, 0, 1);
			}

			/// <summary>
			/// Determines whether the grip is hit by the current mouse position.
			/// </summary>
			/// <param name="mousePosition">The mouse position (hit ray).</param>
			/// <returns></returns>
			public bool IsGripHit(HitTestPointData mousePosition)
			{
				var vec = new VectorD3D(_gripRadius, _gripRadius, _gripRadius);
				var rect = new RectangleD3D(_gripCenter - vec, 2 * vec);

				double z;
				return mousePosition.IsHit(rect, out z);
			}

			#endregion IGripManipulationHandle Members
		}
	}
}