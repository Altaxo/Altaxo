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

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
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
    private class SuperGrip : IGripManipulationHandle
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

      /// <summary>Draws the grip in the graphics context.</summary>
      /// <param name="g">Graphics context.</param>
      /// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle. Attention: this factor must not be used to transform the path of the handle.</param>
      public void Show(Graphics g, double pageScale)
      {
        foreach (var ele in GripList)
          ele.Show(g, pageScale);
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

      #endregion IGripManipulationHandle Members
    }

    #endregion Internal classes

    #region Members

    /// <summary>If objects where really moved during the moving mode, this value become true</summary>
    protected bool _wereObjectsMoved = false;

    /// <summary>The graph controller this mouse handler belongs to.</summary>
    protected GraphController _grac;

    /// <summary>Locker to suppress changed events during moving of objects.</summary>
    protected Altaxo.Main.ISuspendToken _graphDocumentChangedSuppressor;

    /// <summary>Grips that are displayed on the screen.</summary>
    protected IGripManipulationHandle[] DisplayedGrips;

    /// <summary>Grip that is currently dragged.</summary>
    protected IGripManipulationHandle ActiveGrip;

    /// <summary>
    /// Current displayed grip level;
    /// </summary>
    protected int DisplayedGripLevel;

    /// <summary>List of selected HitTestObjects</summary>
    protected List<IHitTestObject> _selectedObjects;

    /// <summary>
    /// If not null, this is the rectangular selection area drawn by the user.
    /// </summary>
    protected RectangleD2D? _rectangleSelectionArea_GraphCoordinates;

    protected static Brush _blueTransparentBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 255));

    #endregion Members

    public ObjectPointerMouseHandler(GraphController grac)
    {
      _grac = grac;
      if (_grac is not null)
        _grac.SetPanelCursor(Cursors.Arrow);

      _selectedObjects = new List<IHitTestObject>();
    }

    public override GraphToolType GraphToolType
    {
      get { return GraphToolType.ObjectPointer; }
    }

    /// <summary>
    /// Returns the number of objects selected in this graph.
    /// </summary>
    public int NumberOfSelectedObjects
    {
      get { return _selectedObjects.Count; }
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
        if (_selectedObjects.Count != 1)
          return null;

        foreach (IHitTestObject graphObject in _selectedObjects) // foreach is here only for one object!
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
        if (_selectedObjects.Count != 1)
          return null;

        foreach (IHitTestObject graphObject in _selectedObjects) // foreach is here only for one object!
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
      if (DisplayedGrips is null || DisplayedGrips.Length == 0)
        return;

      for (int i = 0; i < DisplayedGrips.Length; i++)
        DisplayedGrips[i].Show(g, _grac.ZoomFactor);
    }

    public void DisplaySelectionRectangle(Graphics g)
    {
      RectangleF r = GuiHelper.ToSysDraw(_rectangleSelectionArea_GraphCoordinates.Value);
      g.FillRectangle(_blueTransparentBrush, r);
    }

    /// <summary>
    /// Tests if a grip from the <see cref="DisplayedGrips"/>  is hitted.
    /// </summary>
    /// <param name="pt">Mouse location.</param>
    /// <returns>The grip which was hitted, or null if no grip was hitted.</returns>
    public IGripManipulationHandle GripHitTest(PointD2D pt)
    {
      if (DisplayedGrips is null || DisplayedGrips.Length == 0)
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
    /// <param name="position">Mouse position.</param>
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

      var mousePixelCoord = position;                         // Mouse pixel coordinates
      var rootLayerCoord = _grac.ConvertMouseToRootLayerCoordinates(mousePixelCoord); // Graph area coordinates

      ActiveGrip = GripHitTest(rootLayerCoord);
      if ((ActiveGrip is SuperGrip) && (bShiftKey || bControlKey))
      {
        var superGrip = ActiveGrip as SuperGrip;
        if (superGrip.GetHittedElement(rootLayerCoord, out var gripHandle, out var hitTestObj))
        {
          _selectedObjects.Remove(hitTestObj);
          superGrip.Remove(gripHandle);
          return;
        }
      }
      else if (ActiveGrip is not null)
      {
        ActiveGrip.Activate(rootLayerCoord, false);
        return;
      }
      _grac.FindGraphObjectAtPixelPosition(mousePixelCoord, false, out var clickedObject, out var clickedLayerNumber);

      if (!bShiftKey && !bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
        ClearSelections();

      if (clickedObject is not null)
        AddSelectedObject(rootLayerCoord, clickedObject);
    } // end of function

    /// <summary>
    /// Adds the selected objects from rectangular selection. Here, we do not activate a grip.
    /// Instead, we only show the selection rectangles.
    /// </summary>
    /// <param name="selObjects">The selected objects to add.</param>
    private void AddSelectedObjectsFromRectangularSelection(IList<IHitTestObject> selObjects)
    {
      _selectedObjects.AddRange(selObjects);
      DisplayedGripLevel = 1;
      DisplayedGrips = GetGripsFromSelectedObjects();
      ActiveGrip = null;
    }

    private void AddSelectedObject(PointD2D graphXY, IHitTestObject clickedObject)
    {
      _selectedObjects.Add(clickedObject);

      DisplayedGripLevel = 1;
      DisplayedGrips = GetGripsFromSelectedObjects();

      if (_selectedObjects.Count == 1) // single object selected
      {
        ActiveGrip = GripHitTest(graphXY);
        if (ActiveGrip is not null)
          ActiveGrip.Activate(graphXY, true);
      }
      else // multiple objects selected
      {
        ActiveGrip = DisplayedGrips[0]; // this is our SuperGrip
        DisplayedGrips[0].Activate(graphXY, true);
      }

      _grac.RenderOverlay();
    }

    private IGripManipulationHandle[] GetGripsFromSelectedObjects()
    {
      if (_selectedObjects.Count == 1) // single object selected
      {
        return _selectedObjects[0].GetGrips(_grac.ZoomFactor, DisplayedGripLevel);
      }
      else // multiple objects selected
      {
        var superGrip = new SuperGrip();
        // now we have multiple selected objects
        // we get the grips of all objects and collect them in one supergrip
        foreach (var sel in _selectedObjects)
        {
          var grips = sel.GetGrips(_grac.ZoomFactor, 0);
          if (grips.Length > 0)
            superGrip.Add(grips[0], sel);
        }

        return new IGripManipulationHandle[] { superGrip };
      }
    }

    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseMove(PointD2D position, MouseEventArgs e)
    {
      base.OnMouseMove(position, e);

      if (ActiveGrip is not null)
      {
        PointD2D graphCoord = _grac.ConvertMouseToRootLayerCoordinates(position);
        ActiveGrip.MoveGrip(graphCoord);
        _wereObjectsMoved = true;
        _grac.RenderOverlay();
      }
      else if (e.LeftButton == MouseButtonState.Pressed)
      {
        var diffPos = position - _positionLastMouseDownInMouseCoordinates;

        var oldRect = _rectangleSelectionArea_GraphCoordinates;

        if (_rectangleSelectionArea_GraphCoordinates is not null ||
            Math.Abs(diffPos.X) >= System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(diffPos.Y) >= System.Windows.SystemParameters.MinimumHorizontalDragDistance)
        {
          if (_rectangleSelectionArea_GraphCoordinates is null)
          {
            (_grac.ViewObject as IGraphView).CaptureMouseOnCanvas();
          }

          var pt1 = _grac.ConvertMouseToRootLayerCoordinates(_positionLastMouseDownInMouseCoordinates);
          var rect = new RectangleD2D(pt1, PointD2D.Empty);
          rect.ExpandToInclude(_grac.ConvertMouseToRootLayerCoordinates(position));
          _rectangleSelectionArea_GraphCoordinates = rect;
        }
        if (_rectangleSelectionArea_GraphCoordinates is not null)
          _grac.RenderOverlay();
      }
    }

    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseUp(position, e);

      if (e.LeftButton == MouseButtonState.Released && _rectangleSelectionArea_GraphCoordinates is not null)
      {
        _grac.FindGraphObjectInRootLayerRectangle(_rectangleSelectionArea_GraphCoordinates.Value, out var foundObjects);
        AddSelectedObjectsFromRectangularSelection(foundObjects);
        (_grac.ViewObject as IGraphView).ReleaseCaptureMouseOnCanvas();
        _rectangleSelectionArea_GraphCoordinates = null;
        _grac.RenderOverlay();
      }
      else if (ActiveGrip is not null)
      {
        bool bRefresh = _wereObjectsMoved; // repaint the graph when objects were really moved
        bool bRepaint = false;
        _wereObjectsMoved = false;
        _grac.Doc.Resume(ref _graphDocumentChangedSuppressor);

        bool chooseNextLevel = ActiveGrip.Deactivate();
        ActiveGrip = null;

        if (chooseNextLevel && SingleSelectedHitTestObject is not null)
        {
          DisplayedGripLevel = SingleSelectedHitTestObject.GetNextGripLevel(DisplayedGripLevel);
          bRepaint = true;
        }

        _grac.RenderOverlay();
      }
    }

    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnDoubleClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnDoubleClick(position, e);

      // if there is exactly one object selected, try to open the corresponding configuration dialog
      if (_selectedObjects.Count == 1)
      {
        IEnumerator graphEnum = _selectedObjects.GetEnumerator(); // get the enumerator
        graphEnum.MoveNext(); // set the enumerator to the first item
        var graphObject = (IHitTestObject)graphEnum.Current;

        // Set the currently active layer to the layer the clicked object is belonging to.
        if (graphObject.ParentLayer is not null && !object.ReferenceEquals(_grac.ActiveLayer, graphObject.ParentLayer))
          _grac.EhView_CurrentLayerChoosen(graphObject.ParentLayer.IndexOf().ToArray(), false); // Sets the current active layer

        if (graphObject.DoubleClick is not null)
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
    /// <param name="position">Mouse position.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnClick(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnClick(position, e);
    }

    /// <summary>
    /// Returns true when painting the overlay is currently required; and false if it is not required.
    /// </summary>
    /// <returns>True when painting the overlay is currently required; and false if it is not required.</returns>
    public override bool IsOverlayPaintingRequired
    {
      get
      {
        return _selectedObjects.Count > 0 || _rectangleSelectionArea_GraphCoordinates is not null;
      }
    }

    public override void AfterPaint(Graphics g)
    {
      if (_rectangleSelectionArea_GraphCoordinates is not null)
      {
        DisplaySelectionRectangle(g);
      }
      else
      {
        DisplayedGrips = GetGripsFromSelectedObjects(); // Update grip positions according to the move
        DisplayGrips(g);
      }

      base.AfterPaint(g);
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

    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="e">Key event args.</param>
    /// <returns></returns>
    public override bool ProcessCmdKey(KeyEventArgs e)
    {
      var keyData = e.Key;

      if (keyData == Key.Delete)
      {
        _grac.RemoveSelectedObjects();
        return true;
      }
      else if (keyData == Key.T)
      {
        _grac.ArrangeTopToTop();
        return true;
      }
      else if (keyData == Key.B)
      {
        _grac.ArrangeBottomToBottom();
        return true;
      }
      else if (keyData == Key.L)
      {
        _grac.ArrangeLeftToLeft();
        return true;
      }
      else if (keyData == Key.R)
      {
        _grac.ArrangeRightToRight();
        return true;
      }
      else if (keyData == Key.Left || keyData == Key.Right || keyData == Key.Up || keyData == Key.Down)
      {
        if (_grac.SelectedObjects.Count > 0)
        {
          PointD2D direction;
          switch (keyData)
          {
            case Key.Right:
              direction = new PointD2D(1, 0);
              break;

            case Key.Left:
              direction = new PointD2D(-1, 0);
              break;

            case Key.Down:
              direction = new PointD2D(0, 1);
              break;

            case Key.Up:
              direction = new PointD2D(0, -1);
              break;

            default:
              direction = new PointD2D(0, 0);
              break;
          }

          _grac.MoveSelectedObjects(direction,
          Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
          Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
          Keyboard.IsKeyDown(Key.CapsLock)
          );
          return true; // by returning true: don't allow navigation between Gui elements using the arrow keys
        }
      }

      return false; // per default the key is not processed
    }
  } // end of class
}
