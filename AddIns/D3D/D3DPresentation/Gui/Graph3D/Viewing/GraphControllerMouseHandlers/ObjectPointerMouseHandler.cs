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

using Altaxo.Collections;
using Altaxo.Graph3D;
using Altaxo.Gui.Graph.Viewing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Graph3D.Viewing.GraphControllerMouseHandlers
{
	public class ObjectPointerMouseHandler : MouseStateHandler
	{
		/// <summary>The graph controller this mouse handler belongs to.</summary>
		private Graph3DControllerWpf _grac;

		/// <summary>List of selected HitTestObjects</summary>
		protected List<IHitTestObject> _selectedObjects;

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

		public override void OnMouseDown(PointD3D position, MouseButtonEventArgs e)
		{
			// first, if we have a mousedown without shift key and the
			// position has changed with respect to the last mousedown
			// we have to deselect all objects
			var keyboardModifiers = System.Windows.Input.Keyboard.Modifiers;
			bool bControlKey = keyboardModifiers.HasFlag(ModifierKeys.Control);
			bool bShiftKey = keyboardModifiers.HasFlag(ModifierKeys.Shift);

			// search for a object first
			IHitTestObject clickedObject;
			int[] clickedLayerNumber = null;
			_grac.FindGraphObjectAtPixelPosition(position, false, out clickedObject, out clickedLayerNumber);

			if (!bShiftKey && !bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
				ClearSelections();

			if (null != clickedObject)
				AddSelectedObject(clickedObject);
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
				var graphEnum = _selectedObjects.GetEnumerator(); // get the enumerator
				graphEnum.MoveNext(); // set the enumerator to the first item
				IHitTestObject graphObject = (IHitTestObject)graphEnum.Current;

				// Set the currently active layer to the layer the clicked object is belonging to.
				if (graphObject.ParentLayer != null && !object.ReferenceEquals(_grac.ActiveLayer, graphObject.ParentLayer))
					_grac.EhView_CurrentLayerChoosen(graphObject.ParentLayer.IndexOf().ToArray(), false); // Sets the current active layer

				if (graphObject.DoubleClick != null)
				{
					//EndMovingObjects(); // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
					ClearSelections();  // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
					graphObject.OnDoubleClick();

					//ClearSelections();
				}
			}
		}

		/// <summary>
		/// Clears the selection list and repaints the graph if neccessary
		/// </summary>
		public void ClearSelections()
		{
			bool bRepaint = (_selectedObjects.Count > 0); // is a repaint neccessary
			_selectedObjects.Clear();
			//	DisplayedGrips = null;
			//	ActiveGrip = null;

			if (bRepaint)
				_grac.RenderOverlay();
		}

		private void AddSelectedObject(IHitTestObject clickedObject)
		{
			_selectedObjects.Add(clickedObject);

			/*
			DisplayedGripLevel = 1;
			DisplayedGrips = GetGripsFromSelectedObjects();

			if (_selectedObjects.Count == 1) // single object selected
			{
				ActiveGrip = GripHitTest(graphXY);
				if (ActiveGrip != null)
					ActiveGrip.Activate(graphXY, true);
			}
			else // multiple objects selected
			{
				ActiveGrip = DisplayedGrips[0]; // this is our SuperGrip
				DisplayedGrips[0].Activate(graphXY, true);
			}
			*/
			_grac.RenderOverlay();
		}
	}
}