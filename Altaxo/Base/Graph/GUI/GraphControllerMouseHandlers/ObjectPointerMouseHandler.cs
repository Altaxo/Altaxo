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
    protected struct Grip
    {
      public IGripManipulationHandle Handle;
      public int Layer;
      public IGrippableObject Object;
    }
    #endregion

    #region Members
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

    /// <summary>Locker to suppress changed events during moving of objects.</summary>
    IDisposable _graphDocumentChangedSuppressor;

    /// <summary>
    /// This is the structure to store information about an object that currently has its grip shown.
    /// </summary>
    protected Grip _grip;
    /// <summary>
    /// The hashtable of the selected objects. The key is the selected object itself,
    /// the data is a int object, which stores the layer number the object belongs to.
    /// </summary>
    protected System.Collections.Generic.List<IHitTestObject> m_SelectedObjects = new System.Collections.Generic.List<IHitTestObject>();

    #endregion

    public ObjectPointerMouseHandler(GraphController grac)
    {
      _grac = grac;
      if(_grac.View!=null)
        _grac.View.SetPanelCursor(Cursors.Arrow);
    }

    /// <summary>
    /// Returns the number of objects selected in this graph.
    /// </summary>
    public int NumberOfSelectedObjects
    {
      get { return this.m_SelectedObjects.Count; }
    }


    /// <summary>
    /// Returns the hit test object belonging to the selected object if and only if one single object is selected, else null is returned.
    /// </summary>
    public IHitTestObject SingleSelectedHitTestObject
    {
      get
      {
        if(m_SelectedObjects.Count!=1)
          return null;

        foreach(IHitTestObject graphObject in m_SelectedObjects) // foreach is here only for one object!
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
        if(m_SelectedObjects.Count!=1)
          return null;

        foreach(IHitTestObject graphObject in m_SelectedObjects) // foreach is here only for one object!
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
        
      PointF mouseDiff = new PointF(e.X - m_MoveObjectsLastMovePoint.X, e.Y - m_MoveObjectsLastMovePoint.Y);
      
      if(null!=_grip.Handle)
      {
        PointF printAreaCoord = _grac.PixelToPrintableAreaCoordinates(new Point(e.X,e.Y));
        PointF newPosition = _grac.Layers[_grip.Layer].GraphToLayerCoordinates(printAreaCoord);
        _grip.Handle.MoveGrip(newPosition);
        _grac.RepaintGraphArea();
        
      }

      
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
    
        PointF graphDiff = _grac.PixelToPageDifferences(mouseDiff); // calulate the moving distance in page units = graph units

        foreach(IHitTestObject graphObject in m_SelectedObjects)
        {
          // get the layer number the graphObject belongs to
          //int nLayer = (int)grac.m_SelectedObjects[graphObject];
          //PointF layerDiff = grac.Layers[nLayer].GraphToLayerDifferences(graphDiff); // calculate the moving distance in layer units
          graphObject.ShiftPosition(graphDiff.X,graphDiff.Y);
          //System.Diagnostics.Trace.WriteLine(string.Format("Moving mdiff={0}, gdiff={1}", mouseDiff,graphDiff));
        }
        // now paint the objects on the new position
          
         
   
        _grac.RepaintGraphArea(); // rise a normal paint event
          

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
      PointF graphXY = _grac.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates


      // if we have exacly one object selected, and the one object is grippable,
      // we hit test for the grip areas
      if(SingleSelectedObject is IGrippableObject)
      {
        IGrippableObject gripObject = (IGrippableObject)SingleSelectedObject;
        // first switch to the layer graphics context (from printable context)

        PointF layerCoord = SingleSelectedHitTestObject.ParentLayer.GraphToLayerCoordinates(graphXY);

        _grip.Handle = gripObject.GripHitTest(layerCoord);

        if(_grip.Handle!=null)
        {
          _grip.Layer = SingleSelectedHitTestObject.ParentLayer.Number;
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
          m_SelectedObjects.Remove(clickedSelectedObject);
          _grac.RefreshGraph(); // repaint the graph
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
        _grac.FindGraphObjectAtPixelPosition(mouseXY, false, out clickedObject, out clickedLayerNumber);

        if(bShiftKey || bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
        {
          if(null!=clickedObject)
          {
            m_SelectedObjects.Add(clickedObject);
            
            StartMovingObjects(mouseXY);
            _grac.RepaintGraphArea();
          }
        }
        else // no shift or control key pressed
        {
          if(null!=clickedObject)
          {
            ClearSelections();
            m_SelectedObjects.Add(clickedObject);
            
            StartMovingObjects(mouseXY);
            _grac.RepaintGraphArea();
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
        _grac.RefreshGraph();
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
      if(m_SelectedObjects.Count==1)
      {
        IEnumerator graphEnum = m_SelectedObjects.GetEnumerator(); // get the enumerator
        graphEnum.MoveNext(); // set the enumerator to the first item
        IHitTestObject graphObject = (IHitTestObject)graphEnum.Current;

        // Set the currently active layer to the layer the clicked object is belonging to.
        if (graphObject.ParentLayer != null && !object.ReferenceEquals(_grac.ActiveLayer, graphObject.ParentLayer))
          _grac.CurrentLayerNumber = graphObject.ParentLayer.Number; // Sets the current active layer

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
      if(!m_bMoveObjectsOnMouseMove)
      {
        m_bMoveObjectsOnMouseMove=true;
        m_bObjectsWereMoved=false; // up to now no objects were really moved
        m_MoveObjectsLastMovePoint = currentMousePosition;
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
      bool bRepaint = m_bObjectsWereMoved; // repaint the graph when objects were really moved

      m_bMoveObjectsOnMouseMove = false;
      m_bObjectsWereMoved=false;
      m_MoveObjectsLastMovePoint = new Point(0,0); // this is not neccessary, but only for "order"
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
        _grac.RefreshGraph(); // redraw the contents
      }
    }


    public override void AfterPaint(Graphics g)
    {
      g.TranslateTransform(_grac.Doc.PrintableBounds.X,_grac.Doc.PrintableBounds.Y);
      // finally, mark the selected objects
      
      if(SingleSelectedObject is IGrippableObject)
      {
        IGrippableObject gripObject = (IGrippableObject)SingleSelectedObject;
        if(null!=gripObject)
        {
          // first switch to the layer graphics context (from printable context)
         

          SingleSelectedHitTestObject.ParentLayer.GraphToLayerCoordinates(g);

          gripObject.ShowGrips(g);
        }

      }
      else if(m_SelectedObjects.Count>0)
      {
        foreach(IHitTestObject graphObject in m_SelectedObjects)
        {
         
          g.DrawPath(Pens.Blue,graphObject.SelectionPath);
        }
      }


      base.AfterPaint (g);
    }


    /// <summary>
    /// Removes the currently selected objects (the <see cref="IHitTestObject" /> of the selected object(s) must provide
    /// a handler for deleting the object).
    /// </summary>
    public void RemoveSelectedObjects()
    {
      System.Collections.Generic.List<IHitTestObject> removedObjects = new System.Collections.Generic.List<IHitTestObject>();
     
      foreach (IHitTestObject o in this.m_SelectedObjects)
      {
        if (o.Remove!=null)
        {
          if(true==o.Remove(o))
            removedObjects.Add(o);
        }
      }

      if(removedObjects.Count>0)
      {
        foreach(IHitTestObject o in removedObjects)
          this.m_SelectedObjects.Remove(o);

        _grac.RefreshGraph();
      }
    }

    public void CopySelectedObjectsToClipboard()
    {
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();

      ArrayList objectList = new ArrayList();

      foreach (IHitTestObject o in this.m_SelectedObjects)
      {
        if (o.HittedObject is System.Runtime.Serialization.ISerializable)
        {
          objectList.Add(o.HittedObject);
        }
      }

      dao.SetData("Altaxo.Graph.GraphObjectList", objectList);

      // Test code to test if the object list can be serialized
#if false
      {
      System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binform = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        binform.Serialize(stream, objectList);

        stream.Flush();
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        object obj = binform.Deserialize(stream);
        stream.Close();
        stream.Dispose();
      }
#endif

      // now copy the data object to the clipboard
      System.Windows.Forms.Clipboard.SetDataObject(dao, true);
    }

    public void CutSelectedObjectsToClipboard()
    {
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();

      ArrayList objectList = new ArrayList();
      System.Collections.Generic.List<IHitTestObject> notSerialized = new System.Collections.Generic.List<IHitTestObject>();

      foreach (IHitTestObject o in this.m_SelectedObjects)
      {
        if (o.HittedObject is System.Runtime.Serialization.ISerializable)
        {
          objectList.Add(o.HittedObject);
        }
        else
        {
          notSerialized.Add(o);
        }
      }

      dao.SetData("Altaxo.Graph.GraphObjectList", objectList);

      // now copy the data object to the clipboard
      System.Windows.Forms.Clipboard.SetDataObject(dao, true);

      // Remove the not serialized objects from the selection, so they are not removed from the graph..
      foreach (IHitTestObject o in notSerialized)
        m_SelectedObjects.Remove(o);

      this.RemoveSelectedObjects();
    }


    


    public delegate void ArrangeElement(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds);

    /// <summary>
    /// Arranges the objects so they share a common boundary.
    /// </summary>
    /// <param name="arrange">Routine that determines how to arrange the element with respect to the master element.</param>
    public void Arrange(ArrangeElement arrange)
    {
      if (m_SelectedObjects.Count < 2)
        return;


      RectangleF masterbound = m_SelectedObjects[m_SelectedObjects.Count - 1].ObjectPath.GetBounds();

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = m_SelectedObjects.Count - 2; i >= 0; i--)
      {
        IHitTestObject o = m_SelectedObjects[i];
        RectangleF bounds = o.ObjectPath.GetBounds();

        arrange(o, bounds, masterbound);
      }

      _grac.RefreshGraph(); // force a refresh
    }

    /// <summary>
    /// Arranges the objects so they share the top boundary of the last selected object.
    /// </summary>
    public void ArrangeTopToTop()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition(0, masterbounds.Y - bounds.Y); }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the bottom boundary of the last selected object.
    /// </summary>
    public void ArrangeBottomToBottom()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition(0, (masterbounds.Y+masterbounds.Height) - (bounds.Y+bounds.Height)); }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the left boundary of the last selected object.
    /// </summary>
    public void ArrangeLeftToLeft()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition(masterbounds.X - bounds.X, 0); }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the bottom line of the last selected object.
    /// </summary>
    public void ArrangeRightToRight()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition( (masterbounds.X + masterbounds.Width) - (bounds.X + bounds.Width), 0); }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the horizontal middle line of the last selected object.
    /// </summary>
    public void ArrangeVertical()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition((masterbounds.X + masterbounds.Width*0.5f) - (bounds.X + bounds.Width*0.5f), 0); }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the vertical middle line of the last selected object.
    /// </summary>
    public void ArrangeHorizontal()
    {
      Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
      { obj.ShiftPosition(0, (masterbounds.Y + masterbounds.Height*0.5f) - (bounds.Y + bounds.Height*0.5f)); }
      );
    }


    /// <summary>
    /// Arranges the objects so they their vertical middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeHorizontalTable()
    {
      if (m_SelectedObjects.Count < 3)
        return;


      RectangleF firstbound = m_SelectedObjects[0].SelectionPath.GetBounds();
      RectangleF lastbound = m_SelectedObjects[m_SelectedObjects.Count - 1].SelectionPath.GetBounds();
      float step = (lastbound.X + lastbound.Width * 0.5f) - (firstbound.X + firstbound.Width * 0.5f);
      step /= (m_SelectedObjects.Count - 1);

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = m_SelectedObjects.Count - 2; i > 0; i--)
      {
        IHitTestObject o = m_SelectedObjects[i];
        RectangleF bounds = o.SelectionPath.GetBounds();
        o.ShiftPosition((firstbound.X + firstbound.Width * 0.5f) + i * step - (bounds.X + bounds.Width * 0.5f),0);
      }

      _grac.RefreshGraph(); // force a refresh
    }

    /// <summary>
    /// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeVerticalTable()
    {
      if (m_SelectedObjects.Count < 3)
        return;


      RectangleF firstbound = m_SelectedObjects[0].SelectionPath.GetBounds();
      RectangleF lastbound = m_SelectedObjects[m_SelectedObjects.Count - 1].SelectionPath.GetBounds();
      float step = (lastbound.Y + lastbound.Height * 0.5f) - (firstbound.Y + firstbound.Height * 0.5f);
      step /= (m_SelectedObjects.Count - 1);

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = m_SelectedObjects.Count - 2; i > 0; i--)
      {
        IHitTestObject o = m_SelectedObjects[i];
        RectangleF bounds = o.SelectionPath.GetBounds();
        o.ShiftPosition(0,(firstbound.Y + firstbound.Height * 0.5f) + i * step - (bounds.Y + bounds.Height * 0.5f));
      }

      _grac.RefreshGraph(); // force a refresh
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
      foreach(IHitTestObject graphObject in m_SelectedObjects)
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
      bool bRepaint = (m_SelectedObjects.Count>0); // is a repaint neccessary
      m_SelectedObjects.Clear();
      
      EndMovingObjects();
      
      // this.m_MouseState = new ObjectPointerMouseHandler();

      if(bRepaint)
        _grac.RepaintGraphArea();
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
        this.RemoveSelectedObjects();
        return true;
      }
      else if(keyData==Keys.T)
      {
        ArrangeTopToTop();
        return true;
      }
      else if (keyData == Keys.B)
      {
        ArrangeBottomToBottom();
        return true;
      }
      else if (keyData == Keys.L)
      {
        ArrangeLeftToLeft();
        return true;
      }
      else if (keyData == Keys.R)
      {
        ArrangeRightToRight();
        return true;
      }

      return false; // per default the key is not processed
    }


  } // end of class




}
