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
using System.Windows.Forms;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the object pointer tool is selected.
  /// </summary>
  public class ObjectPointerMouseHandler : MouseStateHandler
  {
    #region Inner structures
    protected struct GripManipulationHandles
    {
			public IGripManipulationHandle Handle;
      public IGripManipulationHandle[] Handles;
      public IGrippableObject Object;

			public void ShowGrips(Graphics g)
			{
				if (null == Handles || Handles.Length == 0)
					return;

				var gstate = g.Save();
				g.PageScale = 1;
				g.ResetTransform();

				for (int i = 0; i < Handles.Length; i++)
					Handles[i].Show(g);


				g.Restore(gstate);
			}

			public IGripManipulationHandle HitTest(PointF pt)
			{
				if (null == Handles || Handles.Length == 0)
					return null;

				for (int i = 0; i < Handles.Length; i++)
				{
					if (Handles[i].IsGripHitted(pt))
						return Handles[i];
				}
				return null;
			}
		}
    #endregion

    #region Members
    /// <summary>
    /// If true, the selected objects where moved when a MouseMove event is fired
    /// </summary>
    protected bool _moveObjectsOnMouseMove=false;
    /// <summary>Stores the mouse position of the last point to where the selected objects where moved</summary>
    protected PointF _movedObjectsLastMovePoint;
    /// <summary>If objects where really moved during the moving mode, this value become true</summary>
    protected bool _wereObjectsMoved=false;

    /// <summary>The graph controller this mouse handler belongs to.</summary>
    protected GraphView _grac;

    /// <summary>Locker to suppress changed events during moving of objects.</summary>
    IDisposable _graphDocumentChangedSuppressor;

    /// <summary>
    /// This is the structure to store information about an object that currently has its grip shown.
    /// </summary>
    protected GripManipulationHandles _grip;
    /// <summary>
    /// The hashtable of the selected objects. The key is the selected object itself,
    /// the data is a int object, which stores the layer number the object belongs to.
    /// </summary>
    protected List<IHitTestObject> _selectedObjects = new List<IHitTestObject>();

    #endregion

    public ObjectPointerMouseHandler(GraphView grac)
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
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseMove(e);
        
      PointF mouseDiff = new PointF(e.X - _movedObjectsLastMovePoint.X, e.Y - _movedObjectsLastMovePoint.Y);
      
      if(null!=_grip.Handle)
      {
        PointF printAreaCoord = _grac.WinFormsController.PixelToPrintableAreaCoordinates(new Point(e.X,e.Y));
        //PointF newPosition = _grac.GC.Layers[_grip.Layer].GraphToLayerCoordinates(printAreaCoord);
        var pts = new PointF[] { printAreaCoord };
        SingleSelectedHitTestObject.InverseTransformation.TransformPoints(pts);
        PointF newPosition = pts[0];
        _grip.Handle.MoveGrip(newPosition);
        _grac.WinFormsController.RepaintGraphArea();
        
      }

      
      if(_moveObjectsOnMouseMove && 0!= _selectedObjects.Count && (mouseDiff.X!=0 || mouseDiff.Y!=0))
      {
        // move all the selected objects to the new position
        // first update the position of the selected objects to reflect the new position
        _movedObjectsLastMovePoint.X = e.X;
        _movedObjectsLastMovePoint.Y = e.Y;

        // indicate the objects has moved now
        _wereObjectsMoved = true;


        // this difference, which is in mouse coordinates, must first be 
        // converted to Graph coordinates (1/72"), and then transformed for
        // each object to the layer coordinate differences of the layer
    
        PointF graphDiff = _grac.WinFormsController.PixelToPageDifferences(mouseDiff); // calulate the moving distance in page units = graph units

        foreach(IHitTestObject graphObject in _selectedObjects)
        {
          // get the layer number the graphObject belongs to
          //int nLayer = (int)grac.m_SelectedObjects[graphObject];
          //PointF layerDiff = grac.Layers[nLayer].GraphToLayerDifferences(graphDiff); // calculate the moving distance in layer units
          graphObject.ShiftPosition(graphDiff.X,graphDiff.Y);
          //System.Diagnostics.Trace.WriteLine(string.Format("Moving mdiff={0}, gdiff={1}", mouseDiff,graphDiff));
        }
        // now paint the objects on the new position
          
         
   
        _grac.WinFormsController.RepaintGraphArea(); // rise a normal paint event
          

      }
      
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
    public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseDown( e);

      if(e.Button != MouseButtons.Left)
        return ; // then there is nothing to do here

      // first, if we have a mousedown without shift key and the
      // position has changed with respect to the last mousedown
      // we have to deselect all objects
      bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
      bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));

      PointF mouseXY = new PointF(e.X,e.Y);                           // Mouse pixel coordinates
      PointF graphXY = _grac.WinFormsController.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates


      // if we have exacly one object selected, and the one object is grippable,
      // we hit test for the grip areas
      if(SingleSelectedObject is IGrippableObject)
      {
        IGrippableObject gripObject = (IGrippableObject)SingleSelectedObject;
        // first switch to the layer graphics context (from printable context)

        //PointF layerCoord = SingleSelectedHitTestObject.ParentLayer.GraphToLayerCoordinates(graphXY);
        var pts = new PointF[]{graphXY};
        SingleSelectedHitTestObject.InverseTransformation.TransformPoints(pts);
        var layerCoord = pts[0];

        //_grip.Handle = gripObject.GripHitTest(layerCoord);

				// Calculate unscaled page coordinates
				pts[0] = _grac.WinFormsController.PixelToUnscaledPageCoordinates(mouseXY); // Graph area coordinates
				_grip.Handle = _grip.HitTest(pts[0]);


        if(_grip.Handle!=null)
        {
//          _grip.Layer = SingleSelectedHitTestObject.ParentLayer.Number;
          _grip.Object = gripObject;
          return; // 
        }

			



      }
     

  
      // have we clicked on one of the already selected objects
      IHitTestObject clickedSelectedObject=null;
      bool bClickedOnAlreadySelectedObjects = IsPixelPositionOnAlreadySelectedObject(mouseXY, out clickedSelectedObject);

      if(bClickedOnAlreadySelectedObjects)
      {
        if(bShiftKey || bControlKey) // if shift or control is pressed, remove the selection
        {
          _selectedObjects.Remove(clickedSelectedObject);
          _grac.WinFormsController.RefreshGraph(); // repaint the graph
        }
        else // not shift or control pressed -> so activate the object moving mode
        {
          StartMovingObjects(mouseXY);
        }
      } // end if bClickedOnAlreadySelectedObjects
      else // not clicked on a already selected object
      {
        // search for a object first
        IHitTestObject clickedObject;
        int clickedLayerNumber=0;
        _grac.WinFormsController.FindGraphObjectAtPixelPosition(mouseXY, false, out clickedObject, out clickedLayerNumber);

        if(bShiftKey || bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
        {
          if(null!=clickedObject)
          {
            _selectedObjects.Add(clickedObject);
            
            StartMovingObjects(mouseXY);
            _grac.WinFormsController.RepaintGraphArea();
          }
        }
        else // no shift or control key pressed
        {
          if(null!=clickedObject)
          {
            ClearSelections();
            _selectedObjects.Add(clickedObject);
            
            StartMovingObjects(mouseXY);
            _grac.WinFormsController.RepaintGraphArea();
          }
          else // if clicked to nothing 
          {
            ClearSelections(); // clear the selection list
          }
        } // end else no shift or control

      } // end else (not cklicked on already selected object)
      
    } // end of function

    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseUp(e);


      if(_grip.Handle!=null)
      {
        _grip.Handle=null;
        _grac.WinFormsController.RefreshGraph();
        return;
      }

      System.Console.WriteLine("MouseUp {0},{1}",e.X,e.Y);
      EndMovingObjects();
      
    }

    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override void OnDoubleClick(System.EventArgs e)
    {
      base.OnDoubleClick(e);

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
    public override void OnClick(System.EventArgs e)
    {
      base.OnClick(e);

      System.Console.WriteLine("Click");

     
    }


    /// <summary>
    /// Actions neccessary to start the dragging of graph objects
    /// </summary>
    /// <param name="currentMousePosition">the current mouse position in pixel</param>
    protected void StartMovingObjects(PointF currentMousePosition)
    {
      if(!_moveObjectsOnMouseMove)
      {
        _moveObjectsOnMouseMove=true;
        _wereObjectsMoved=false; // up to now no objects were really moved
        _movedObjectsLastMovePoint = currentMousePosition;
        _graphDocumentChangedSuppressor = _grac.Doc.BeginUpdate();

        // create a frozen bitmap of the graph
        /*
          Graphics g = grac.m_View.CreateGraphGraphics(); // do not translate the graphics here!
          grac.m_FrozenGraph = new Bitmap(grac.m_View.GraphSize.Width,grac.m_View.GraphSize.Height,g);
          Graphics gbmp = Graphics.FromImage(grac.m_FrozenGraph);
          grac.DoPaint(gbmp,false);
          gbmp.Dispose();
          */
      }
    }

    /// <summary>
    /// Actions neccessary to end the dragging of graph objects
    /// </summary>
    protected void EndMovingObjects()
    {
      bool bRepaint = _wereObjectsMoved; // repaint the graph when objects were really moved

      _moveObjectsOnMouseMove = false;
      _wereObjectsMoved=false;
      _movedObjectsLastMovePoint = new Point(0,0); // this is not neccessary, but only for "order"
      _grac.Doc.EndUpdate(ref _graphDocumentChangedSuppressor);        
        
      /*
        if(null!=grac.m_FrozenGraph) 
        {
          grac.m_FrozenGraph.Dispose(); grac.m_FrozenGraph=null;
        }
        */
      
      if(bRepaint)
      {
        //grac.m_FrozenGraphIsDirty = true;
        _grac.WinFormsController.RefreshGraph(); // redraw the contents
      }
    }


    public override void AfterPaint(Graphics g)
    {
			// finally, mark the selected objects
			if (SingleSelectedObject is IGrippableObject)
			{
				var gripObject = (IGrippableObject)SingleSelectedObject;

				// first switch to the layer graphics context (from printable context)


				//SingleSelectedHitTestObject.ParentLayer.GraphToLayerCoordinates(g);

				g.MultiplyTransform(SingleSelectedHitTestObject.Transformation);

				_grip.Handles = gripObject.ShowGrips(g);
				_grip.ShowGrips(g);


			}
			else if (_selectedObjects.Count > 0)
			{
				foreach (IHitTestObject graphObject in _selectedObjects)
				{

					g.DrawPath(Pens.Blue, graphObject.SelectionPath);
				}
			}


      base.AfterPaint (g);
    }

    /// <summary>
    /// Determines whether or not the pixel position in <paramref name="pixelPos"/> is on a already selected object
    /// </summary>
    /// <param name="pixelPos">The pixel position to test (on the graph panel)</param>
    /// <param name="foundObject">Returns the object the position <paramref name="pixelPos"/> is pointing to, else null</param>
    /// <returns>True when the pixel position is upon a selected object, else false</returns>
    public bool IsPixelPositionOnAlreadySelectedObject(PointF pixelPos, out IHitTestObject foundObject)
    {
      PointF graphXY = _grac.WinFormsController.PixelToPrintableAreaCoordinates(pixelPos); // Graph area coordinates

      // have we clicked on one of the already selected objects
      foreach(IHitTestObject graphObject in _selectedObjects)
      {
       
        
        if(graphObject.SelectionPath.IsVisible(graphXY))
        {
          foundObject = graphObject;
          return true;
        }
      }
      foundObject = null;
      return false;
    }

    /// <summary>
    /// Clears the selection list and repaints the graph if neccessary
    /// </summary>
    public void ClearSelections()
    {
      bool bRepaint = (_selectedObjects.Count>0); // is a repaint neccessary
      _selectedObjects.Clear();
      
      EndMovingObjects();
      
      // this.m_MouseState = new ObjectPointerMouseHandler();

      if(bRepaint)
        _grac.WinFormsController.RepaintGraphArea();
    }


    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Delete)
      {
        _grac.GC.RemoveSelectedObjects();
        return true;
      }
      else if(keyData==Keys.T)
      {
				_grac.GC.ArrangeTopToTop();
        return true;
      }
      else if (keyData == Keys.B)
      {
				_grac.GC.ArrangeBottomToBottom();
        return true;
      }
      else if (keyData == Keys.L)
      {
				_grac.GC.ArrangeLeftToLeft();
        return true;
      }
      else if (keyData == Keys.R)
      {
				_grac.GC.ArrangeRightToRight();
        return true;
      }

      return false; // per default the key is not processed
    }


  } // end of class




}
