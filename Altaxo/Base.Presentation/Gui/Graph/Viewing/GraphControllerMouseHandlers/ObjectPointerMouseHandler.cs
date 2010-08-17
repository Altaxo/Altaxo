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
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the object pointer tool is selected.
  /// </summary>
  public class ObjectPointerMouseHandler : MouseStateHandler
  {
    #region Internal classes

    /// <summary>
    /// Supergrip collects multiple active grips so that they can be moved simultaneously.
    /// </summary>
    class SuperGrip : IGripManipulationHandle
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
			public void Activate(PointD2D initialPosition, bool isActivatedUponCreation)
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

      public void MoveGrip(PointD2D newPosition)
      {
        foreach (var ele in GripList)
          ele.MoveGrip(newPosition);
      }

      public void Show(Graphics g)
      {
        foreach (var ele in GripList)
          ele.Show(g);
      }

      public bool IsGripHitted(PointD2D point)
      {
        foreach (var ele in GripList)
          if (ele.IsGripHitted(point))
            return true;
        return false;
      }

			public bool GetHittedElement(PointD2D point, out IGripManipulationHandle gripHandle, out IHitTestObject hitObject)
			{
				for (int i = GripList.Count - 1; i >= 0; i--)
				{
					if (GripList[i].IsGripHitted(point))
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

      #endregion
    }

    #endregion


    #region Members

    /// <summary>If objects where really moved during the moving mode, this value become true</summary>
    protected bool _wereObjectsMoved=false;

    /// <summary>The graph controller this mouse handler belongs to.</summary>
    protected GraphViewWpf _grac;

    /// <summary>Locker to suppress changed events during moving of objects.</summary>
    IDisposable _graphDocumentChangedSuppressor;

    /// <summary>Grips that are displayed on the screen.</summary>
    protected IGripManipulationHandle[] DisplayedGrips;

    /// <summary>Grip that is currently dragged.</summary>
    protected IGripManipulationHandle ActiveGrip;

		/// <summary>
		/// Current displayed grip level;
		/// </summary>
		protected int DisplayedGripLevel;

    /// <summary>List of selected HitTestObjects</summary>
    protected List<IHitTestObject> _selectedObjects = new List<IHitTestObject>();

    #endregion

    public ObjectPointerMouseHandler(GraphViewWpf grac)
    {
      _grac = grac;
      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.Arrow);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer; }
		}

    /// <summary>
    /// Returns the number of objects selected in this graph.
    /// </summary>
    public int NumberOfSelectedObjects
    {
      get { return this._selectedObjects.Count; }
    }

		public IList<IHitTestObject> SelectedObjects
		{
			get
			{
				return _selectedObjects;
			}
		}

    /// <summary>
    /// Returns the hit test object belonging to the selected object if and only if one single object is selected, else null is returned.
    /// </summary>
    public IHitTestObject SingleSelectedHitTestObject
    {
      get
      {
        if(_selectedObjects.Count!=1)
          return null;

        foreach(IHitTestObject graphObject in _selectedObjects) // foreach is here only for one object!
        {
          return graphObject;
        }
        return null;
      }
    }

    /// <summary>
    /// Returns the selected object if and only if one single object is selected, else null is returned.
    /// </summary>
    public object SingleSelectedObject
    {
      get
      {
        if(_selectedObjects.Count!=1)
          return null;

        foreach(IHitTestObject graphObject in _selectedObjects) // foreach is here only for one object!
        {
          return graphObject.HittedObject;
        }
        return null;
      }
    }

    /// <summary>
    /// Draws the <see cref="DisplayedGrips"/> on the graphics context.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    public void DisplayGrips(Graphics g)
			{
				if (null == DisplayedGrips || DisplayedGrips.Length == 0)
					return;

				for (int i = 0; i < DisplayedGrips.Length; i++)
					DisplayedGrips[i].Show(g);
			}


    /// <summary>
    /// Tests if a grip from the <see cref="DisplayedGrips"/>  is hitted.
    /// </summary>
    /// <param name="pt">Mouse location.</param>
    /// <returns>The grip which was hitted, or null if no grip was hitted.</returns>
			public IGripManipulationHandle GripHitTest(PointF pt)
			{
				if (null == DisplayedGrips || DisplayedGrips.Length == 0)
					return null;

				for (int i = 0; i < DisplayedGrips.Length; i++)
				{
					if (DisplayedGrips[i].IsGripHitted(pt))
						return DisplayedGrips[i];
				}
				return null;
			}
		

   



    /// <summary>
    /// Handles the MouseDown event when the object pointer tool is selected
    /// </summary>
    /// <param name="e">The mouse event args</param>
    /// <remarks>
    /// The strategy to handle the mousedown event is as following:
    /// 
    /// Have we clicked on already selected objects?
    ///   if yes (we have clicked on already selected objects) and the shift or control key was pressed -> deselect the object and repaint
    ///   if yes (we have clicked on already selected objects) and none shift nor control key was pressed-> activate the object moving  mode
    ///   if no (we have not clicked on already selected objects) and shift or control key was pressed -> search for the object and add it to the selected objects, then aktivate moving mode
    ///   if no (we have not clicked on already selected objects) and no shift or control key pressed -> if a object was found add it to the selected objects and activate moving mode
    ///                                                                                                  if no object was found clear the selection list, deactivate moving mode
    /// </remarks>
			public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
			{
				base.OnMouseDown(position, e);

				if (e.LeftButton != MouseButtonState.Pressed)
					return; // then there is nothing to do here

				// first, if we have a mousedown without shift key and the
				// position has changed with respect to the last mousedown
				// we have to deselect all objects
				var keyboardModifiers = System.Windows.Input.Keyboard.Modifiers;

				bool bControlKey = keyboardModifiers.HasFlag(ModifierKeys.Control);
				bool bShiftKey = keyboardModifiers.HasFlag(ModifierKeys.Shift);

				PointF mouseXY = (PointF)position;                         // Mouse pixel coordinates
				PointF graphXY = _grac.GuiController.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates


				ActiveGrip = GripHitTest(graphXY);
				if ((ActiveGrip is SuperGrip) && (bShiftKey || bControlKey))
				{
					var superGrip = ActiveGrip as SuperGrip;
					IHitTestObject hitTestObj;
					IGripManipulationHandle gripHandle;
					if (superGrip.GetHittedElement(graphXY, out gripHandle, out hitTestObj))
					{
						_selectedObjects.Remove(hitTestObj);
						superGrip.Remove(gripHandle);
						_grac.GuiController.RefreshGraph(); // repaint the graph
						return;
					}
				}
				else if (ActiveGrip != null)
				{
					ActiveGrip.Activate(graphXY, false);
					return;
				}

				// search for a object first
				IHitTestObject clickedObject;
				int clickedLayerNumber = 0;
				_grac.GuiController.FindGraphObjectAtPixelPosition(mouseXY, false, out clickedObject, out clickedLayerNumber);

				if (!bShiftKey && !bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
					ClearSelections();

				if (null != clickedObject)
					AddSelectedObject(graphXY, clickedObject);

			} // end of function

    private void AddSelectedObject(PointF graphXY, IHitTestObject clickedObject)
    {
      _selectedObjects.Add(clickedObject);

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

      _grac.GuiController.RepaintGraphArea();
    }

    private IGripManipulationHandle[] GetGripsFromSelectedObjects()
    {
      if (_selectedObjects.Count == 1) // single object selected
      {
        return _selectedObjects[0].GetGrips(_grac.GC.ZoomFactor, DisplayedGripLevel);
      }
      else // multiple objects selected
      {
        var superGrip = new SuperGrip();
        // now we have multiple selected objects
        // we get the grips of all objects and collect them in one supergrip
				foreach (var sel in _selectedObjects)
				{
					var grips = sel.GetGrips(_grac.GC.ZoomFactor, 0);
					if(grips.Length>0)
						superGrip.Add(grips[0], sel);
				}

        return new IGripManipulationHandle[] { superGrip };
      }
    }

    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseMove(PointD2D position, MouseEventArgs e)
    {
      base.OnMouseMove(position, e);

      if (null != ActiveGrip)
      {
        PointF printAreaCoord = _grac.GuiController.PixelToPrintableAreaCoordinates((PointF)position);
        ActiveGrip.MoveGrip(printAreaCoord);
        _wereObjectsMoved = true;
        _grac.GuiController.RepaintGraphArea();
      }
    }

    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseUp(position, e);

      if(ActiveGrip!=null)
      {
        bool bRefresh = _wereObjectsMoved; // repaint the graph when objects were really moved
				bool bRepaint = false;
        _wereObjectsMoved = false;
        _grac.Doc.EndUpdate(ref _graphDocumentChangedSuppressor);

				bool chooseNextLevel = ActiveGrip.Deactivate();
				ActiveGrip = null;

				if (chooseNextLevel && null!=SingleSelectedHitTestObject)
				{
					DisplayedGripLevel = SingleSelectedHitTestObject.GetNextGripLevel(DisplayedGripLevel);
					bRepaint = true;
				}
					

        if (bRefresh)
          _grac.GuiController.RefreshGraph(); // redraw the contents
				else if(bRepaint)
					_grac.GuiController.RepaintGraphArea();
      }
    }

    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
		public override void OnDoubleClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnDoubleClick(position, e);

      // if there is exactly one object selected, try to open the corresponding configuration dialog
      if(_selectedObjects.Count==1)
      {
        IEnumerator graphEnum = _selectedObjects.GetEnumerator(); // get the enumerator
        graphEnum.MoveNext(); // set the enumerator to the first item
        IHitTestObject graphObject = (IHitTestObject)graphEnum.Current;

        // Set the currently active layer to the layer the clicked object is belonging to.
        if (graphObject.ParentLayer != null && !object.ReferenceEquals(_grac.ActiveLayer, graphObject.ParentLayer))
          _grac.SetActiveLayerFromInternal( graphObject.ParentLayer.Number ); // Sets the current active layer

        if(graphObject.DoubleClick!=null)
        {
          //EndMovingObjects(); // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
          ClearSelections();  // this will resume the suspended graph so that pressing the "Apply" button in a dialog will result in a visible change
          graphObject.OnDoubleClick();
         
          //ClearSelections();
        }
      }
      
    }


    /// <summary>
    /// Handles the mouse click event.
    /// </summary>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
		public override void OnClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnClick(position, e);

      System.Console.WriteLine("Click");

     
    }

    public override void AfterPaint(Graphics g)
    {
			DisplayedGrips = GetGripsFromSelectedObjects(); // Update grip positions according to the move
      DisplayGrips(g);
      base.AfterPaint (g);
    }

    /// <summary>
    /// Clears the selection list and repaints the graph if neccessary
    /// </summary>
    public void ClearSelections()
    {
      bool bRepaint = (_selectedObjects.Count>0); // is a repaint neccessary
      _selectedObjects.Clear();
      DisplayedGrips = null;
      ActiveGrip = null;

      if(bRepaint)
        _grac.GuiController.RepaintGraphArea();
    }


    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public override bool ProcessCmdKey(KeyEventArgs e)
    {
			var keyData = e.Key;

      if (keyData == Key.Delete)
      {
        _grac.GC.RemoveSelectedObjects();
        return true;
      }
      else if(keyData==Key.T)
      {
				_grac.GC.ArrangeTopToTop();
        return true;
      }
      else if (keyData == Key.B)
      {
				_grac.GC.ArrangeBottomToBottom();
        return true;
      }
      else if (keyData == Key.L)
      {
				_grac.GC.ArrangeLeftToLeft();
        return true;
      }
      else if (keyData == Key.R)
      {
				_grac.GC.ArrangeRightToRight();
        return true;
      }

      return false; // per default the key is not processed
    }


  } // end of class




}
