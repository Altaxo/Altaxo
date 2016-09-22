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

using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Gui.Graph.Viewing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Graph3D.Viewing.GraphControllerMouseHandlers
{
	public class ObjectPointerMouseHandler : MouseStateHandler
	{
		#region Internal classes

		/// <summary>
		/// Supergrip collects multiple active grips so that they can be moved simultaneously.
		/// </summary>
		protected class SuperGrip : IGripManipulationHandle
		{
			/// <summary>List of all collected grips.</summary>
			private List<IGripManipulationHandle> GripList;

			private List<IHitTestObject> HittedList;

			public SuperGrip()
			{
				GripList = new List<IGripManipulationHandle>();
				HittedList = new List<IHitTestObject>();
			}

			public void Add(IGripManipulationHandle gripHandle, IHitTestObject hitTestObject)
			{
				GripList.Add(gripHandle);
				HittedList.Add(hitTestObject);
			}

			public void Remove(IGripManipulationHandle gripHandle)
			{
				for (int i = GripList.Count - 1; i >= 0; i--)
				{
					if (object.ReferenceEquals(gripHandle, GripList[i]))
					{
						GripList.RemoveAt(i);
						HittedList.RemoveAt(i);
						break;
					}
				}
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
				foreach (var ele in GripList)
					ele.Activate(initialPosition, isActivatedUponCreation);
			}

			public bool Deactivate()
			{
				foreach (var ele in GripList)
					ele.Deactivate();

				return false;
			}

			public void MoveGrip(HitTestPointData newPosition)
			{
				foreach (var ele in GripList)
					ele.MoveGrip(newPosition);
			}

			/// <summary>Draws the grip in the graphics context.</summary>
			/// <param name="g">Graphics context.</param>
			/// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
			public void Show(IOverlayContext3D g)
			{
				foreach (var ele in GripList)
					ele.Show(g);
			}

			public bool IsGripHit(HitTestPointData point)
			{
				foreach (var ele in GripList)
					if (ele.IsGripHit(point))
						return true;
				return false;
			}

			public bool GetHittedElement(HitTestPointData point, out IGripManipulationHandle gripHandle, out IHitTestObject hitObject)
			{
				for (int i = GripList.Count - 1; i >= 0; i--)
				{
					if (GripList[i].IsGripHit(point))
					{
						gripHandle = GripList[i];
						hitObject = HittedList[i];
						return true;
					}
				}

				gripHandle = null;
				hitObject = null;
				return false;
			}

			#endregion IGripManipulationHandle Members
		}

		#endregion Internal classes

		/// <summary>The graph controller this mouse handler belongs to.</summary>
		private Graph3DControllerWpf _grac;

		/// <summary>List of selected HitTestObjects</summary>
		protected List<IHitTestObject> _selectedObjects;

		/// <summary>If objects where really moved during the moving mode, this value become true</summary>
		protected bool _wereObjectsMoved = false;

		/// <summary>
		/// Current displayed grip level;
		/// </summary>
		protected int DisplayedGripLevel;

		/// <summary>Grips that are displayed on the screen.</summary>
		protected IGripManipulationHandle[] DisplayedGrips;

		/// <summary>Grip that is currently dragged.</summary>
		protected IGripManipulationHandle ActiveGrip;

		/// <summary>Locker to suppress changed events during moving of objects.</summary>
		private Altaxo.Main.ISuspendToken _graphDocumentChangedSuppressor;

		public ObjectPointerMouseHandler(Graph3DControllerWpf grac)
		{
			_grac = grac;

			if (_grac != null)
				_grac.SetPanelCursor(Cursors.Arrow);

			_selectedObjects = new List<IHitTestObject>();
		}

		public override GraphToolType GraphToolType
		{
			get
			{
				return GraphToolType.ObjectPointer;
			}
		}

		public IList<IHitTestObject> SelectedObjects { get { return _selectedObjects; } }

		/// <summary>
		/// Returns the hit test object belonging to the selected object if and only if one single object is selected, else null is returned.
		/// </summary>
		public IHitTestObject SingleSelectedHitTestObject
		{
			get
			{
				if (_selectedObjects.Count != 1)
					return null;

				foreach (IHitTestObject graphObject in _selectedObjects) // foreach is here only for one object!
				{
					return graphObject;
				}
				return null;
			}
		}

		public override void OnMouseDown(PointD3D position, MouseButtonEventArgs e)
		{
			base.OnMouseDown(position, e);

			if (e.ChangedButton == MouseButton.Left)
			{
				var hitData = new HitTestPointData(_grac.Doc.Camera.GetHitRayMatrix(position));

				// first, if we have a mousedown without shift key and the
				// position has changed with respect to the last mousedown
				// we have to deselect all objects
				var keyboardModifiers = System.Windows.Input.Keyboard.Modifiers;
				bool bControlKey = keyboardModifiers.HasFlag(ModifierKeys.Control);
				bool bShiftKey = keyboardModifiers.HasFlag(ModifierKeys.Shift);

				ActiveGrip = GripHitTest(hitData);
				if ((ActiveGrip is SuperGrip) && (bShiftKey || bControlKey))
				{
					var superGrip = ActiveGrip as SuperGrip;
					IHitTestObject hitTestObj;
					IGripManipulationHandle gripHandle;
					if (superGrip.GetHittedElement(hitData, out gripHandle, out hitTestObj))
					{
						_selectedObjects.Remove(hitTestObj);
						superGrip.Remove(gripHandle);
						return;
					}
				}
				else if (ActiveGrip != null)
				{
					ActiveGrip.Activate(hitData, false);
					return;
				}

				// search for a object first
				IHitTestObject clickedObject;
				int[] clickedLayerNumber = null;
				_grac.FindGraphObjectAtPixelPosition(hitData, false, out clickedObject, out clickedLayerNumber);

				if (!bShiftKey && !bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
					ClearSelections();

				if (null != clickedObject)
					AddSelectedObject(hitData, clickedObject);
			}
		}

		/// <summary>
		/// Handles the mouse up event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public override void OnMouseUp(PointD3D position, MouseButtonEventArgs e)
		{
			base.OnMouseUp(position, e);

			/*
			if (e.LeftButton == MouseButtonState.Released && null != _rectangleSelectionArea_GraphCoordinates)
			{
				List<IHitTestObject> foundObjects;
				_grac.FindGraphObjectInRootLayerRectangle(_rectangleSelectionArea_GraphCoordinates.Value, out foundObjects);
				AddSelectedObjectsFromRectangularSelection(foundObjects);
				_grac.ReleaseMouseCapture();
				_rectangleSelectionArea_GraphCoordinates = null;
				_grac.RenderOverlay();
				return;
			}
			*/

			if (e.ChangedButton == MouseButton.Left)
			{
				if (ActiveGrip != null)
				{
					bool bRefresh = _wereObjectsMoved; // repaint the graph when objects were really moved
					bool bRepaint = false;
					_wereObjectsMoved = false;
					_grac.Doc.Resume(ref _graphDocumentChangedSuppressor);

					bool chooseNextLevel = ActiveGrip.Deactivate();
					ActiveGrip = null;

					if (chooseNextLevel && null != SingleSelectedHitTestObject)
					{
						DisplayedGripLevel = SingleSelectedHitTestObject.GetNextGripLevel(DisplayedGripLevel);
						bRepaint = true;
					}

					_grac.RenderOverlay();
				}
			}
		}

		/// <summary>
		/// Handles the mouse move event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public override void OnMouseMove(PointD3D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			if (null != ActiveGrip)
			{
				var graphCoord = new HitTestPointData(_grac.Doc.Camera.GetHitRayMatrix(position));
				ActiveGrip.MoveGrip(graphCoord);
				_wereObjectsMoved = true;
				_grac.RenderOverlay();
			}
			/*
			else if (e.LeftButton == MouseButtonState.Pressed)
			{
				var diffPos = position - _positionLastMouseDownInMouseCoordinates;

				var oldRect = _rectangleSelectionArea_GraphCoordinates;

				if (null != _rectangleSelectionArea_GraphCoordinates ||
						Math.Abs(diffPos.X) >= System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
						Math.Abs(diffPos.Y) >= System.Windows.SystemParameters.MinimumHorizontalDragDistance)
				{
					if (null == _rectangleSelectionArea_GraphCoordinates)
					{
						_grac.CaptureMouse();
					}

					var pt1 = _grac.ConvertMouseToRootLayerCoordinates(_positionLastMouseDownInMouseCoordinates);
					var rect = new RectangleD2D(pt1, PointD2D.Empty);
					rect.ExpandToInclude(_grac.ConvertMouseToRootLayerCoordinates(position));
					_rectangleSelectionArea_GraphCoordinates = rect;
				}
				if (null != _rectangleSelectionArea_GraphCoordinates)
					_grac.RenderOverlay();
			}
			*/
		}

		/// <summary>
		/// Handles the mouse doubleclick event.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">EventArgs as provided by the view.</param>
		/// <returns>The next mouse state handler that should handle mouse events.</returns>
		public override void OnDoubleClick(PointD3D position, MouseButtonEventArgs e)
		{
			base.OnDoubleClick(position, e);

			// if there is exactly one object selected, try to open the corresponding configuration dialog
			if (_selectedObjects.Count == 1)
			{
				IHitTestObject graphObject = (IHitTestObject)SelectedObjects[0];

				// Set the currently active layer to the layer the clicked object is belonging to.
				if (graphObject.ParentLayer != null && !object.ReferenceEquals(_grac.ActiveLayer, graphObject.ParentLayer))
					_grac.EhView_CurrentLayerChoosen(graphObject.ParentLayer.IndexOf().ToArray(), false); // Sets the current active layer

				if (graphObject.DoubleClick != null)
				{
					//EndMovingObjects(); // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
					ClearSelections();  // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
					graphObject.OnDoubleClick();
				}
			}
		}

		public override bool ProcessCmdKey(KeyEventArgs e)
		{
			// Note: a return value of true indicates that the key was processed, thus the key will not trigger further actions
			var keyData = e.Key;

			if (keyData == Key.Delete)
			{
				_grac.RemoveSelectedObjects();
				_grac.RenderOverlay();
				return true;
			}

			return base.ProcessCmdKey(e);
		}

		/// <summary>
		/// Clears the selection list and repaints the graph if neccessary
		/// </summary>
		public void ClearSelections()
		{
			bool bRepaint = (_selectedObjects.Count > 0); // is a repaint neccessary
			_selectedObjects.Clear();
			DisplayedGrips = null;
			ActiveGrip = null;

			if (bRepaint)
				_grac.RenderOverlay();
		}

		private void AddSelectedObject(HitTestPointData hitPoint, IHitTestObject clickedObject)
		{
			_selectedObjects.Add(clickedObject);

			DisplayedGripLevel = 1;
			DisplayedGrips = GetGripsFromSelectedObjects();

			if (_selectedObjects.Count == 1) // single object selected
			{
				ActiveGrip = GripHitTest(hitPoint);
				if (ActiveGrip != null)
					ActiveGrip.Activate(hitPoint, true);
			}
			else // multiple objects selected
			{
				ActiveGrip = DisplayedGrips[0]; // this is our SuperGrip
				DisplayedGrips[0].Activate(hitPoint, true);
			}

			_grac.RenderOverlay();
		}

		private IGripManipulationHandle[] GetGripsFromSelectedObjects()
		{
			if (_selectedObjects.Count == 1) // single object selected
			{
				return _selectedObjects[0].GetGrips(DisplayedGripLevel);
			}
			else // multiple objects selected
			{
				var superGrip = new SuperGrip();
				// now we have multiple selected objects
				// we get the grips of all objects and collect them in one supergrip
				foreach (var sel in _selectedObjects)
				{
					var grips = sel.GetGrips(0);
					if (grips.Length > 0)
						superGrip.Add(grips[0], sel);
				}

				return new IGripManipulationHandle[] { superGrip };
			}
		}

		/// <summary>
		/// Tests if a grip from the <see cref="DisplayedGrips"/>  is hitted.
		/// </summary>
		/// <param name="pt">Mouse location.</param>
		/// <returns>The grip which was hitted, or null if no grip was hitted.</returns>
		public IGripManipulationHandle GripHitTest(HitTestPointData pt)
		{
			if (null == DisplayedGrips || DisplayedGrips.Length == 0)
				return null;

			for (int i = 0; i < DisplayedGrips.Length; i++)
			{
				if (DisplayedGrips[i].IsGripHit(pt))
					return DisplayedGrips[i];
			}
			return null;
		}

		/// <summary>
		/// Draws the <see cref="DisplayedGrips"/> on the graphics context.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		public void DisplayGrips(IOverlayContext3D g)
		{
			if (null == DisplayedGrips || DisplayedGrips.Length == 0)
				return;

			for (int i = 0; i < DisplayedGrips.Length; i++)
				DisplayedGrips[i].Show(g);
		}

		/// <summary>
		/// This function is called just after the paint event. The graphic context is in graph coordinates.
		/// </summary>
		/// <param name="g"></param>
		public override void AfterPaint(IOverlayContext3D g)
		{
			{
				DisplayedGrips = GetGripsFromSelectedObjects(); // Update grip positions according to the move
				DisplayGrips(g);
			}

			base.AfterPaint(g);
		}
	}
}