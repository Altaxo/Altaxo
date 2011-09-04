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
		protected class MovementGripHandle : IGripManipulationHandle
		{
			IHitTestObject _parent;
			/// <summary>Path active for selection of this grip.</summary>
			GraphicsPath _gripPath;
			/// <summary>Path that is shown on the display.</summary>
			GraphicsPath _displayedPath;
			bool _hasMoved;
			bool _wasActivatedUponCreation;
			PointD2D _initialMousePosition;
			PointD2D _initialObjectPosition;

			public MovementGripHandle(IHitTestObject parent, GraphicsPath gripPath, GraphicsPath objectPath)
			{
				_parent = parent;
				_gripPath = gripPath;
				_displayedPath = objectPath;
			}


			#region IGripManipulationHandle Members

			/// <summary>
			/// Activates this grip, providing the initial position of the mouse.
			/// </summary>
			/// <param name="initialPosition">Initial position of the mouse.</param>
			/// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
			/// thie activation is due to a regular mouse click in this grip.</param>
			public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
			{
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
				var ht = _parent as GraphicBaseHitTestObject;
				if (null != ht && !_hasMoved && !_wasActivatedUponCreation)
					return true;
				else
					return false;
			}

			public void MoveGrip(PointD2D newPosition)
			{
				var diff = newPosition - _initialMousePosition;

				if (!diff.IsEmpty)
					_hasMoved = true;

				diff = _parent.Transformation.InverseTransformVector(diff);
				((GraphicBase)_parent.HittedObject).SilentSetPosition(_initialObjectPosition + diff);


			}

			public void Show(Graphics g)
			{
				g.DrawPath(Pens.Blue, _displayedPath ?? _gripPath);
			}

			public bool IsGripHitted(PointD2D point)
			{
				return _gripPath.IsVisible((PointF)point);
			}

			#endregion
		}

	}
}
