#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  #region Object Pointer Mouse Handler
  /// <summary>
  /// Handles the mouse events when the <see cref="GraphTools.ObjectPointer"/> tools is selected.
  /// </summary>
  public class ObjectPointerMouseHandler : MouseStateHandler
  {
    /// <summary>
    /// If true, the selected objects where moved when a MouseMove event is fired
    /// </summary>
    protected bool m_bMoveObjectsOnMouseMove=false;
    /// <summary>Stores the mouse position of the last point to where the selected objects where moved</summary>
    protected PointF m_MoveObjectsLastMovePoint;
    /// <summary>If objects where really moved during the moving mode, this value become true</summary>
    protected bool m_bObjectsWereMoved=false;

    /// <summary>The graph controller this mouse handler belongs to.</summary>
    protected GraphController _grac;

    /// <summary>
    /// The hashtable of the selected objects. The key is the selected object itself,
    /// the data is a int object, which stores the layer number the object belongs to.
    /// </summary>
    protected System.Collections.Hashtable m_SelectedObjects = new System.Collections.Hashtable();

    public ObjectPointerMouseHandler(GraphController grac)
    {
      _grac = grac;
    }

    /// <summary>
    /// Returns the number of objects selected in this graph.
    /// </summary>
    public int NumberOfSelectedObjects
    {
      get { return this.m_SelectedObjects.Count; }
    }

    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="grac">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override MouseStateHandler OnMouseMove(GraphController grac, System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseMove(grac,e);
        
      PointF mouseDiff = new PointF(e.X - m_MoveObjectsLastMovePoint.X, e.Y - m_MoveObjectsLastMovePoint.Y);
      if(m_bMoveObjectsOnMouseMove && 0!= m_SelectedObjects.Count && (mouseDiff.X!=0 || mouseDiff.Y!=0))
      {
        // move all the selected objects to the new position
        // first update the position of the selected objects to reflect the new position
        m_MoveObjectsLastMovePoint.X = e.X;
        m_MoveObjectsLastMovePoint.Y = e.Y;

        // indicate the objects has moved now
        m_bObjectsWereMoved = true;


        // this difference, which is in mouse coordinates, must first be 
        // converted to Graph coordinates (1/72"), and then transformed for
        // each object to the layer coordinate differences of the layer
    
        PointF graphDiff = grac.PixelToPageDifferences(mouseDiff); // calulate the moving distance in page units = graph units

        foreach(IHitTestObject graphObject in m_SelectedObjects.Keys)
        {
          // get the layer number the graphObject belongs to
          //int nLayer = (int)grac.m_SelectedObjects[graphObject];
          //PointF layerDiff = grac.Layers[nLayer].GraphToLayerDifferences(graphDiff); // calculate the moving distance in layer units
          graphObject.ShiftPosition(graphDiff.X,graphDiff.Y);
          // Console.WriteLine("Moving mdiff={0}, gdiff={1}, ldiff={2}", mouseDiff,graphDiff,layerDiff);
        }
        // now paint the objects on the new position
          
         
   
          grac.RepaintGraphArea(); // rise a normal paint event
          

      }
      return this;
    }

    /// <summary>
    /// Handles the MouseDown event when the object pointer tool is selected
    /// </summary>
    /// <param name="grac">The sender of the event.</param>
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
    public override MouseStateHandler OnMouseDown(GraphController grac, System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseDown(grac, e);

      if(e.Button != MouseButtons.Left)
        return this; // then there is nothing to do here

      // first, if we have a mousedown without shift key and the
      // position has changed with respect to the last mousedown
      // we have to deselect all objects
      PointF mouseXY = new PointF(e.X,e.Y);
      bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
      bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
      PointF graphXY = grac.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates

  
      // have we clicked on one of the already selected objects
      IHitTestObject clickedSelectedObject=null;
      bool bClickedOnAlreadySelectedObjects = IsPixelPositionOnAlreadySelectedObject(mouseXY, out clickedSelectedObject);

      if(bClickedOnAlreadySelectedObjects)
      {
        if(bShiftKey || bControlKey) // if shift or control is pressed, remove the selection
        {
          m_SelectedObjects.Remove(clickedSelectedObject);
          grac.RefreshGraph(); // repaint the graph
        }
        else // not shift or control pressed -> so activate the object moving mode
        {
          StartMovingObjects(grac,mouseXY);
        }
      } // end if bClickedOnAlreadySelectedObjects
      else // not clicked on a already selected object
      {
        // search for a object first
        IHitTestObject clickedObject;
        int clickedLayerNumber=0;
        grac.FindGraphObjectAtPixelPosition(mouseXY, out clickedObject, out clickedLayerNumber);

        if(bShiftKey || bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
        {
          if(null!=clickedObject)
          {
            m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
            grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
            StartMovingObjects(grac,mouseXY);
          }
        }
        else // no shift or control key pressed
        {
          if(null!=clickedObject)
          {
            ClearSelections(grac);
            m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
            grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
            StartMovingObjects(grac,mouseXY);
          }
          else // if clicked to nothing 
          {
            ClearSelections(grac); // clear the selection list
          }
        } // end else no shift or control

      } // end else (not cklicked on already selected object)
      return this;
    } // end of function

    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="grac">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override MouseStateHandler OnMouseUp(GraphController grac, System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseUp(grac,e);

      System.Console.WriteLine("MouseUp {0},{1}",e.X,e.Y);
      EndMovingObjects(grac);
      return this;
    }

    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="grac">The GraphController that sends this event.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override MouseStateHandler OnDoubleClick(GraphController grac, System.EventArgs e)
    {
      base.OnDoubleClick(grac,e);

      // if there is exactly one object selected, try to open the corresponding configuration dialog
      if(m_SelectedObjects.Count==1)
      {
        IEnumerator graphEnum = m_SelectedObjects.Keys.GetEnumerator(); // get the enumerator
        graphEnum.MoveNext(); // set the enumerator to the first item
        IHitTestObject graphObject = (IHitTestObject)graphEnum.Current;
        int nLayer = (int)m_SelectedObjects[graphObject];
        if(graphObject.DoubleClick!=null)
        {
          if(true==graphObject.OnDoubleClick())
          {
            ClearSelections();
          }
              
        }
      }
      return this;
    }


    /// <summary>
    /// Handles the mouse click event.
    /// </summary>
    /// <param name="grac">The GraphController that sends this event.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override MouseStateHandler OnClick(GraphController grac, System.EventArgs e)
    {
      base.OnClick(grac,e);

      System.Console.WriteLine("Click");

      return this;
    }


    /// <summary>
    /// Actions neccessary to start the dragging of graph objects
    /// </summary>
    /// <param name="grac">The GraphController for which these actions should apply.</param>
    /// <param name="currentMousePosition">the current mouse position in pixel</param>
    protected void StartMovingObjects(GraphController grac, PointF currentMousePosition)
    {
      if(!m_bMoveObjectsOnMouseMove)
      {
        m_bMoveObjectsOnMouseMove=true;
        m_bObjectsWereMoved=false; // up to now no objects were really moved
        m_MoveObjectsLastMovePoint = currentMousePosition;

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
    protected void EndMovingObjects(GraphController grac)
    {
      bool bRepaint = m_bObjectsWereMoved; // repaint the graph when objects were really moved

      m_bMoveObjectsOnMouseMove = false;
      m_bObjectsWereMoved=false;
      m_MoveObjectsLastMovePoint = new Point(0,0); // this is not neccessary, but only for "order"
        
        
      /*
        if(null!=grac.m_FrozenGraph) 
        {
          grac.m_FrozenGraph.Dispose(); grac.m_FrozenGraph=null;
        }
        */
      
      if(bRepaint)
      {
        //grac.m_FrozenGraphIsDirty = true;
        grac.RefreshGraph(); // redraw the contents
      }
    }

    /// <summary>
    /// Clears the selection list and repaints the graph if neccessary
    /// </summary>
    public void ClearSelections(GraphController grac)
    {
      bool bRepaint = (m_SelectedObjects.Count>0); // is a repaint neccessary
      m_SelectedObjects.Clear();
      EndMovingObjects(grac);

      if(bRepaint)
        grac.RefreshGraph(); 
    }

    public override void AfterPaint(GraphController grac, Graphics g)
    {
      g.TranslateTransform(grac.Doc.PrintableBounds.X,grac.Doc.PrintableBounds.Y);
      // finally, mark the selected objects
      if(m_SelectedObjects.Count>0)
      {
        foreach(IHitTestObject graphObject in m_SelectedObjects.Keys)
        {
          int nLayer = (int)m_SelectedObjects[graphObject];
          g.DrawPath(Pens.Blue,graphObject.SelectionPath);
        }
      }
      base.AfterPaint (grac,g);
    }


    /// <summary>
    /// Removes the currently selected objects (currently only GraphicsObjects are removed)
    /// </summary>
    public void RemoveSelectedObjects()
    {
      System.Collections.ArrayList removedObjects = new System.Collections.ArrayList();
      foreach(object o in this.m_SelectedObjects.Keys)
      {
        if(o is GraphicsObject && ((GraphicsObject)o).Container!=null)
        {
          GraphicsObjectCollection coll = ((GraphicsObject)o).Container;
          coll.Remove((GraphicsObject)o);
          removedObjects.Add(o);
        }
      }

      if(removedObjects.Count>0)
      {

        foreach(object o in removedObjects)
          this.m_SelectedObjects.Remove(o);

        _grac.RefreshGraph();
      }

    }


    /// <summary>
    /// Determines whether or not the pixel position in <paramref name="pixelPos"/> is on a already selected object
    /// </summary>
    /// <param name="pixelPos">The pixel position to test (on the graph panel)</param>
    /// <param name="foundObject">Returns the object the position <paramref name="pixelPos"/> is pointing to, else null</param>
    /// <returns>True when the pixel position is upon a selected object, else false</returns>
    public bool IsPixelPositionOnAlreadySelectedObject(PointF pixelPos, out IHitTestObject foundObject)
    {
      PointF graphXY = _grac.PixelToPrintableAreaCoordinates(pixelPos); // Graph area coordinates

      // have we clicked on one of the already selected objects
      foreach(IHitTestObject graphObject in m_SelectedObjects.Keys)
      {
        int nLayer = (int)m_SelectedObjects[graphObject];
        // if(null!=graphObject.HitTest(Layers[nLayer].GraphToLayerCoordinates(graphXY)))
        if(graphObject.SelectionPath.IsVisible(pixelPos))
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
      bool bRepaint = (m_SelectedObjects.Count>0); // is a repaint neccessary
      m_SelectedObjects.Clear();
      
      
      // this.m_MouseState = new ObjectPointerMouseHandler();

      if(bRepaint)
        _grac.RepaintGraphArea();
    }



  } // end of class

  #endregion // object pointer mouse handler


}
