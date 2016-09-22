#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Graph3D.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// The abstract base class of all MouseStateHandlers.
	/// </summary>
	/// <remarks>The mouse state handler are used to handle the mouse events of the graph view in different contexts,
	/// depending on which GraphTool is choosen by the user.</remarks>
	public abstract class MouseStateHandler
	{
		/// <summary>Stores the mouse position of the last mouse up event. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</summary>
		protected PointD3D _positionLastMouseUpInMouseCoordinates;

		/// <summary>Stores the mouse position of the last mouse down event. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</summary>
		protected PointD3D _positionLastMouseDownInMouseCoordinates;

		/// <summary>Active layer at the time of using the tool.</summary>
		protected HostLayer _cachedActiveLayer;

		/// <summary>Transformation that can be used to transform root layer coordinates into the coordinates of the cached active layer.</summary>
		protected Matrix4x3 _cachedActiveLayerTransformation;

		public abstract GraphToolType GraphToolType { get; }

		/// <summary>
		/// Handles the mouse move event.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseMove(PointD3D position, MouseEventArgs e)
		{
		}

		/// <summary>
		/// Handles the mouse up event. Stores the position of the mouse into <see cref="_positionLastMouseUpInMouseCoordinates"/>.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseUp(PointD3D position, MouseButtonEventArgs e)
		{
			_positionLastMouseUpInMouseCoordinates = position;
		}

		/// <summary>
		/// Handles the mouse down event. Stores the position of the mouse into <see cref="_positionLastMouseDownInMouseCoordinates"/>.
		/// </summary>
		/// <param name="position">Mouse position.  X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnMouseDown(PointD3D position, MouseButtonEventArgs e)
		{
			_positionLastMouseDownInMouseCoordinates = position;
		}

		/// <summary>
		/// Handles the mouse click event.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">EventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnClick(PointD3D position, MouseButtonEventArgs e)
		{
		}

		/// <summary>
		/// Handles the mouse doubleclick event.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">EventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public virtual void OnDoubleClick(PointD3D position, MouseButtonEventArgs e)
		{
		}

		/// <summary>
		/// Is called when the mouse state handler is deselected.
		/// </summary>
		public virtual void OnDeselection()
		{
		}

		/// <summary>
		/// This function is called just after the paint event. The graphic context is in graph coordinates.
		/// </summary>
		/// <param name="g"></param>
		public virtual void AfterPaint(IOverlayContext3D g)
		{
		}

		/// <summary>
		/// Returns true when painting the overlay is currently required; and false if it is not required.
		/// </summary>
		/// <returns>True when painting the overlay is currently required; and false if it is not required.</returns>
		public virtual bool IsOverlayPaintingRequired { get { return true; } }

		/// <summary>
		/// This function is called if a key is pressed.
		/// </summary>
		/// <param name="e">Key event arguments.</param>
		/// <returns></returns>
		public virtual bool ProcessCmdKey(KeyEventArgs e)
		{
			return false; // per default the key is not processed
		}

		#region static helper functions

		/// <summary>
		/// Gets the hit point on that plane of the active layer rectangle, that is facing the camera.
		/// </summary>
		/// <param name="doc">The graph document containing the active layer.</param>
		/// <param name="activeLayer">The active layer of the graph document.</param>
		/// <param name="hitposition">Hit point in relative screen coordinates. The z-component is the aspect ratio of the screen (y/x).</param>
		/// <param name="hitPointOnPlaneInActiveLayerCoordinates">Output: The hit point on the plane of the active layer that faces the camera. The hit point is returned in active layer coordinates.</param>
		/// <param name="rotationsRadian">The rotation angles that can be used e.g. to orient text so that the text is most readable from the current camera setting. Rotation angle around x is the x-component of the returned vector, and so on.</param>
		/// <exception cref="InvalidProgramException">There should always be a plane of a rectangle that can be hit!</exception>
		public static void GetHitPointOnActiveLayerPlaneFacingTheCamera(GraphDocument doc, HostLayer activeLayer, PointD3D hitposition, out PointD3D hitPointOnPlaneInActiveLayerCoordinates, out VectorD3D rotationsRadian)
		{
			var activeLayerTransformation = activeLayer.TransformationFromRootToHere();
			var camera = doc.Camera;
			var hitData = new HitTestPointData(camera.GetHitRayMatrix(hitposition));
			hitData = hitData.NewFromAdditionalTransformation(activeLayerTransformation); // now hitdata are in layer cos

			var targetToEye = hitData.WorldTransformation.Transform(camera.TargetToEyeVectorNormalized); // targetToEye in layer coordinates
			var upEye = hitData.WorldTransformation.Transform(camera.UpVectorPerpendicularToEyeVectorNormalized); // camera up vector in layer coordinates

			// get the face which has the best dot product between the eye vector of the camera and the plane's normal
			var layerRect = new RectangleD3D(PointD3D.Empty, activeLayer.Size);
			double maxval = double.MinValue;
			PlaneD3D maxPlane = PlaneD3D.Empty;
			foreach (var plane in layerRect.Planes)
			{
				double val = VectorD3D.DotProduct(plane.Normal, targetToEye);
				if (val > maxval)
				{
					maxval = val;
					maxPlane = plane;
				}
			}

			bool isHit = hitData.IsPlaneHitByRay(maxPlane, out hitPointOnPlaneInActiveLayerCoordinates); // hitPointOnPlane is in layer coordinates too

			if (!isHit)
				throw new InvalidProgramException("There should always be a plane of a rectangle that can be hit!");

			VectorD3D zaxis = maxPlane.Normal;
			VectorD3D yaxis = upEye;

			// Find y axis perpendicular to zaxis
			maxval = double.MinValue;
			foreach (var plane in layerRect.Planes)
			{
				double val = VectorD3D.DotProduct(plane.Normal, upEye);
				if (val > maxval && 0 == VectorD3D.DotProduct(plane.Normal, zaxis))
				{
					maxval = val;
					yaxis = plane.Normal;
				}
			}
			var xaxis = VectorD3D.CrossProduct(yaxis, zaxis);

			// now we have all information about the spatial position and orientation of the text:
			// hitPointOnPlane is the position of the text
			// maxPlane.Normal is the face orientation of the text
			// maxUpVector is the up orientation of the text

			double cx, sx, cy, sy, cz, sz;

			sy = xaxis.Z;
			if (1 != Math.Abs(sy))
			{
				cy = Math.Sqrt(1 - sy * sy);
				cz = xaxis.X / cy;
				sz = xaxis.Y / cy;
				sx = yaxis.Z / cy;
				cx = zaxis.Z / cy;
			}
			else // sy is +1, thus cy is zero
			{
				// we set x-rotation to zero, i.e. cx==1 and sx==0
				cy = 0;
				cx = 1;
				sx = 0;
				cz = yaxis.Y;
				sz = -yaxis.X;
			}

			rotationsRadian = new VectorD3D(Math.Atan2(sx, cx), Math.Atan2(sy, cy), Math.Atan2(sz, cz));
		}

		#endregion static helper functions
	}
}