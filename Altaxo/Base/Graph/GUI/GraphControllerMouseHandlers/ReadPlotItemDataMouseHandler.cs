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
using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the <see cref="GraphTools.ObjectPointer"/> tools is selected.
  /// </summary>
  public class ReadPlotItemDataMouseHandler : MouseStateHandler
  {
    /// <summary>
    /// Number of the layer, in which the plot item resides which is currently selected.
    /// </summary>
    protected int _LayerNumber;
     
    /// <summary>
    /// The number of the plot item where the cross is currently.
    /// </summary>
    protected int _PlotItemNumber;


    /// <summary>
    /// Number of the plot point which has currently the cross onto.
    /// </summary>
    protected int _PlotIndex;

    /// <summary>
    /// The number of the data point (index into the data row) where the cross is currently.
    /// </summary>
    protected int _RowIndex;

    /// <summary>
    /// The plot item where the mouse snaps in
    /// </summary>
    protected XYColumnPlotItem m_PlotItem;

    /// <summary>
    /// Coordinates of the red data reader cross (in printable coordinates)
    /// </summary>
    protected PointF m_Cross;

    protected GraphController _grac;

    public ReadPlotItemDataMouseHandler(GraphController grac)
    {
      _grac = grac;
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
        
      return this;
    }

    /// <summary>
    /// Handles the MouseDown event when the plot point tool is selected
    /// </summary>
    /// <param name="grac">The sender of the event.</param>
    /// <param name="e">The mouse event args</param>
     
    public override MouseStateHandler OnMouseDown(GraphController grac, System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseDown(grac, e);

      PointF mouseXY = new PointF(e.X,e.Y);
      PointF graphXY = grac.PixelToPrintableAreaCoordinates(mouseXY);
       
      // search for a object first
      IHitTestObject clickedObject;
      int clickedLayerNumber=0;
      grac.FindGraphObjectAtPixelPosition(mouseXY, out clickedObject, out clickedLayerNumber);
      if(null!=clickedObject && clickedObject.HittedObject is XYColumnPlotItem)
      {
        m_PlotItem = (XYColumnPlotItem)clickedObject.HittedObject;
        PointF[] transXY = new PointF[]{graphXY};
        Matrix inv = clickedObject.Transformation.Clone();
        inv.Invert();
        inv.TransformPoints(transXY);
        XYScatterPointInformation scatterPoint = m_PlotItem.GetNearestPlotPoint(clickedObject.ParentLayer,transXY[0]);

        this._PlotItemNumber = GetPlotItemNumber(clickedLayerNumber,m_PlotItem);
        this._LayerNumber = clickedLayerNumber;


        if(null!=scatterPoint)
        {
          this._PlotIndex = scatterPoint.PlotIndex;
          this._RowIndex = scatterPoint.RowIndex;
          // convert this layer coordinates first to PrintableAreaCoordinates
          PointF printableCoord = clickedObject.ParentLayer.LayerToGraphCoordinates(scatterPoint.LayerCoordinates);
          m_Cross = printableCoord;
          m_Cross.X+=_grac.Doc.PrintableBounds.X;
          m_Cross.Y+=_grac.Doc.PrintableBounds.Y;
           
          PointF newPixelCoord = _grac.PrintableAreaToPixelCoordinates(printableCoord);
          Cursor.Position = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
            

          this.DisplayData(m_PlotItem,scatterPoint.RowIndex,
            m_PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
            m_PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);


          
          
          // here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
          // of the grac
          _grac.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross
        }
      }
       
         
      return this;
    } // end of function


      


    void ShowCross(XYScatterPointInformation scatterPoint)
    {
      
      this._PlotIndex = scatterPoint.PlotIndex;
      this._RowIndex = scatterPoint.RowIndex;
      // convert this layer coordinates first to PrintableAreaCoordinates
      PointF printableCoord = _grac.Layers[this._LayerNumber].LayerToGraphCoordinates(scatterPoint.LayerCoordinates);
      m_Cross = printableCoord;
      m_Cross.X+=_grac.Doc.PrintableBounds.X;
      m_Cross.Y+=_grac.Doc.PrintableBounds.Y;
           
      PointF newPixelCoord = _grac.PrintableAreaToPixelCoordinates(printableCoord);
      //Cursor.Position = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
      //Cursor.Position = ((Control)_grac.View).PointToScreen(newPixelCoord);
          
   

      this.DisplayData(m_PlotItem,scatterPoint.RowIndex,
        m_PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
        m_PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);


          
      // here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
      // of the grac
      _grac.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross
       
    }


    void DisplayData(object plotItem, int rowIndex, AltaxoVariant x, AltaxoVariant y)
  {
      Current.DataDisplay.WriteOneLine(string.Format(
        "{0}[{1}] X={2}, Y={3}",
        plotItem.ToString(),
        rowIndex,
        x,
        y));
  }


    /// <summary>
    /// Tests presumtions for a move of the cross.
    /// </summary>
    /// <returns>True if the cross can be moved, false if one of the presumtions does not hold.</returns>
    bool TestMovementPresumtions()
    {
      if(m_PlotItem==null)
        return false;
      if(_grac==null || _grac.Doc==null || _grac.Doc.Layers==null)
        return false;
      if(this._LayerNumber<0 || this._LayerNumber>=_grac.Doc.Layers.Count)
        return false;

      return true;
    }

    /// <summary>
    /// Moves the cross along the plot.
    /// </summary>
    /// <param name="increment"></param>
    void MoveLeftRight(int increment)
    {
      if(!TestMovementPresumtions())
        return;

      XYScatterPointInformation scatterPoint = m_PlotItem.GetNextPlotPoint(_grac.Doc.Layers[this._LayerNumber],this._PlotIndex,increment);
        
      if(null!=scatterPoint)
        ShowCross(scatterPoint);
    }

    /// <summary>
    /// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
    /// </summary>
    /// <param name="increment"></param>
    void MoveUpDown(int increment)
    {
      if(!TestMovementPresumtions())
        return;

      int numlayers = _grac.Layers.Count;
      int nextlayer = _LayerNumber;
      int nextplotitemnumber = this._PlotItemNumber;

      XYScatterPointInformation scatterPoint=null;
      XYColumnPlotItem plotitem = null;
      do
      {
        nextplotitemnumber = this._PlotItemNumber + Math.Sign(increment);
        if(nextplotitemnumber<0)
        {
          nextlayer-=1;
          nextplotitemnumber = nextlayer<0 ? int.MaxValue : _grac.Layers[nextlayer].PlotItems.Count-1;
        }
        else if(nextplotitemnumber>=_grac.Layers[nextlayer].PlotItems.Count)
        {
          nextlayer+=1;
          nextplotitemnumber=0;
        }
        // check if this results in a valid information
        if(nextlayer<0 || nextlayer>=numlayers)
          break;
          
        if(nextplotitemnumber<0 || nextplotitemnumber>=_grac.Layers[nextlayer].PlotItems.Count)
          continue;
  
        plotitem =  _grac.Layers[nextlayer].PlotItems[nextplotitemnumber] as XYColumnPlotItem;
        if(null==plotitem)
          continue;
  
        scatterPoint = plotitem.GetNextPlotPoint(_grac.Layers[nextlayer],this._PlotIndex,0);
      } while(scatterPoint==null);
      
      if(null!=scatterPoint)
      {
        this.m_PlotItem = plotitem;
        this._LayerNumber = nextlayer;
        this._PlotItemNumber = nextplotitemnumber;
        this._PlotIndex = scatterPoint.PlotIndex;
        this._RowIndex = scatterPoint.RowIndex;

        ShowCross(scatterPoint);
      }
    }


    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="grac">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public override MouseStateHandler OnMouseUp(GraphController grac, System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseUp(grac,e);

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

      

      return this;
    }


    public override void AfterPaint(GraphController grac, Graphics g)
    {
      // draw a red cross onto the selected data point

      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X+1,m_Cross.Y,m_Cross.X+10,m_Cross.Y);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X-1,m_Cross.Y,m_Cross.X-10,m_Cross.Y);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X,m_Cross.Y+1,m_Cross.X,m_Cross.Y+10);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X,m_Cross.Y-1,m_Cross.X,m_Cross.Y-10);
        
      base.AfterPaint (grac,g);
    }


    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if(keyData == Keys.Left)
      {
        System.Diagnostics.Trace.WriteLine("Read tool key handler, left key pressed!");
        MoveLeftRight(-1);
        return true;
      }
      else if(keyData == Keys.Right)
      {
        System.Diagnostics.Trace.WriteLine("Read tool key handler, right key pressed!");
        MoveLeftRight(1);
        return true;
      }
      else if(keyData == Keys.Up)
      {
        MoveUpDown(1);
        return true;
      }
      else if(keyData == Keys.Down)
      {
        MoveUpDown(-1);
        return true;
      }


      return false; // per default the key is not processed
    }


    /// <summary>
    /// Find the plot item number of a given plot item.
    /// </summary>
    /// <param name="grac"></param>
    /// <param name="layernumber"></param>
    /// <param name="plotitem"></param>
    /// <returns></returns>
    int GetPlotItemNumber(int layernumber, XYColumnPlotItem plotitem)
    {
      if(layernumber<_grac.Doc.Layers.Count)
      {
        for(int i=0;i<_grac.Doc.Layers[layernumber].PlotItems.Count;i++)
          if(object.ReferenceEquals(_grac.Doc.Layers[layernumber].PlotItems[i],plotitem))
            return i;
      }
      return -1;
    }

  } // end of class

  
}
