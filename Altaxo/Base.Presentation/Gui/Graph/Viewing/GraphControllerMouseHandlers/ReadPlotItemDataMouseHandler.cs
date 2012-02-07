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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.InteropServices;

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the read plot item tool is selected.
  /// </summary>
  public class ReadPlotItemDataMouseHandler : MouseStateHandler
  {
		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int X, int Y);

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
    protected XYColumnPlotItem _PlotItem;

    /// <summary>
    /// Coordinates of the red data reader cross (in printable coordinates)
    /// </summary>
    protected PointD2D _positionOfCrossInGraphCoordinates;

    protected PresentationGraphController _grac;

    public ReadPlotItemDataMouseHandler(PresentationGraphController grac)
    {
      _grac = grac;
      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.Cross);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.ReadPlotItemData; }
		}


    /// <summary>
    /// Handles the MouseDown event when the plot point tool is selected
    /// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">The mouse event args</param>
    public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseDown(position, e);

			var graphXY = _grac.ConvertMouseToGraphCoordinates(position);
       
      // search for a object first
      IHitTestObject clickedObject;
      int clickedLayerNumber=0;
      _grac.FindGraphObjectAtPixelPosition(position, true, out clickedObject, out clickedLayerNumber);
      if(null!=clickedObject && clickedObject.HittedObject is XYColumnPlotItem)
      {
        _PlotItem = (XYColumnPlotItem)clickedObject.HittedObject;
        var transXY = clickedObject.Transformation.InverseTransformPoint(graphXY);
        XYScatterPointInformation scatterPoint = _PlotItem.GetNearestPlotPoint(clickedObject.ParentLayer,transXY);

        this._PlotItemNumber = GetPlotItemNumber(clickedLayerNumber,_PlotItem);
        this._LayerNumber = clickedLayerNumber;


        if(null!=scatterPoint)
        {
          this._PlotIndex = scatterPoint.PlotIndex;
          this._RowIndex = scatterPoint.RowIndex;
          // convert this layer coordinates first to PrintableAreaCoordinates
          var printableCoord = clickedObject.ParentLayer.LayerToGraphCoordinates(scatterPoint.LayerCoordinates);
          _positionOfCrossInGraphCoordinates = printableCoord;
					// m_Cross.X -= _grac.GraphViewOffset.X;
					// m_Cross.Y -= _grac.GraphViewOffset.Y;

					var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(printableCoord);

					// TODO (Wpf)
          //var newCursorPosition = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
					//SetCursorPos(newCursorPosition.X, newCursorPosition.Y);
            
					
          this.DisplayData(_PlotItem,scatterPoint.RowIndex,
            _PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
            _PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);


          
          
          // here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
          // of the grac
          _grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
        }
      }
       
         
     
    } // end of function


      


    void ShowCross(XYScatterPointInformation scatterPoint)
    {
      
      this._PlotIndex = scatterPoint.PlotIndex;
      this._RowIndex = scatterPoint.RowIndex;
      // convert this layer coordinates first to PrintableAreaCoordinates
      var printableCoord = _grac.Layers[this._LayerNumber].LayerToGraphCoordinates(scatterPoint.LayerCoordinates);
      _positionOfCrossInGraphCoordinates = printableCoord;
			// m_Cross.X -= _grac.GraphViewOffset.X;
			// m_Cross.Y -= _grac.GraphViewOffset.Y;

			var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(printableCoord);
      //Cursor.Position = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
      //Cursor.Position = ((Control)_grac.View).PointToScreen(newPixelCoord);
          
   

      this.DisplayData(_PlotItem,scatterPoint.RowIndex,
        _PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
        _PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);


          
      // here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
      // of the grac
      _grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
       
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
      if(_PlotItem==null)
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

      XYScatterPointInformation scatterPoint = _PlotItem.GetNextPlotPoint(_grac.Doc.Layers[this._LayerNumber],this._PlotIndex,increment);
        
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
        nextplotitemnumber = nextplotitemnumber + Math.Sign(increment);
        if(nextplotitemnumber<0)
        {
          nextlayer-=1;
          nextplotitemnumber = nextlayer<0 ? int.MaxValue : _grac.Layers[nextlayer].PlotItems.Flattened.Length-1;
        }
        else if(nextplotitemnumber>=_grac.Layers[nextlayer].PlotItems.Flattened.Length)
        {
          nextlayer+=1;
          nextplotitemnumber=0;
        }
        // check if this results in a valid information
        if(nextlayer<0 || nextlayer>=numlayers)
          break;
          
        if(nextplotitemnumber<0 || nextplotitemnumber>=_grac.Layers[nextlayer].PlotItems.Flattened.Length)
          continue;
  
        plotitem =  _grac.Layers[nextlayer].PlotItems.Flattened[nextplotitemnumber] as XYColumnPlotItem;
        if(null==plotitem)
          continue;
  
        scatterPoint = plotitem.GetNextPlotPoint(_grac.Layers[nextlayer],this._PlotIndex,0);
      } while(scatterPoint==null);
      
      if(null!=scatterPoint)
      {
        this._PlotItem = plotitem;
        this._LayerNumber = nextlayer;
        this._PlotItemNumber = nextplotitemnumber;
        this._PlotIndex = scatterPoint.PlotIndex;
        this._RowIndex = scatterPoint.RowIndex;

        ShowCross(scatterPoint);
      }
    }



    public override void AfterPaint( Graphics g)
    {
      // draw a red cross onto the selected data point
			double startLine = 1 / _grac.ZoomFactor;
			double endLine = 10 / _grac.ZoomFactor;
			using (HatchBrush brush = new HatchBrush(HatchStyle.Percent50, Color.Red, Color.Yellow))
			{
				using (Pen pen = new Pen(brush, (float)(2 / _grac.ZoomFactor)))
				{
					g.DrawLine(pen, (float)(_positionOfCrossInGraphCoordinates.X + startLine), (float)_positionOfCrossInGraphCoordinates.Y, (float)(_positionOfCrossInGraphCoordinates.X + endLine), (float)_positionOfCrossInGraphCoordinates.Y);
					g.DrawLine(pen, (float)(_positionOfCrossInGraphCoordinates.X - startLine), (float)_positionOfCrossInGraphCoordinates.Y, (float)(_positionOfCrossInGraphCoordinates.X - endLine), (float)_positionOfCrossInGraphCoordinates.Y);
					g.DrawLine(pen, (float)_positionOfCrossInGraphCoordinates.X, (float)(_positionOfCrossInGraphCoordinates.Y + startLine), (float)_positionOfCrossInGraphCoordinates.X, (float)(_positionOfCrossInGraphCoordinates.Y + endLine));
					g.DrawLine(pen, (float)_positionOfCrossInGraphCoordinates.X, (float)(_positionOfCrossInGraphCoordinates.Y - startLine), (float)_positionOfCrossInGraphCoordinates.X, (float)(_positionOfCrossInGraphCoordinates.Y - endLine));
				}
			}
      base.AfterPaint (g);
    }


    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
		/// <param name="e">Key event arguments.</param>
    /// <returns></returns>
		public override bool ProcessCmdKey(KeyEventArgs e)
    {
			var keyData = e.Key;
      if(keyData == Key.Left)
      {
        System.Diagnostics.Trace.WriteLine("Read tool key handler, left key pressed!");
        MoveLeftRight(-1);
        return true;
      }
      else if(keyData == Key.Right)
      {
        System.Diagnostics.Trace.WriteLine("Read tool key handler, right key pressed!");
        MoveLeftRight(1);
        return true;
      }
      else if(keyData == Key.Up)
      {
        MoveUpDown(1);
        return true;
      }
      else if(keyData == Key.Down)
      {
        MoveUpDown(-1);
        return true;
      }
			else if (keyData ==Key.Enter)
			{
				Current.Console.WriteLine("{0}",this._RowIndex);
				return true;
			}

      return false; // per default the key is not processed
    }


    /// <summary>
    /// Find the plot item number of a given plot item.
    /// </summary>
    /// <param name="layernumber"></param>
    /// <param name="plotitem"></param>
    /// <returns></returns>
    int GetPlotItemNumber(int layernumber, XYColumnPlotItem plotitem)
    {
      if(layernumber<_grac.Doc.Layers.Count)
      {
        for(int i=0;i<_grac.Doc.Layers[layernumber].PlotItems.Flattened.Length;i++)
          if(object.ReferenceEquals(_grac.Doc.Layers[layernumber].PlotItems.Flattened[i],plotitem))
            return i;
      }
      return -1;
    }

  } // end of class

  
}
