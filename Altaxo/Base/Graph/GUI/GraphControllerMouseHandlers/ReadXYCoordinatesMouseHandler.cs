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
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  /// <summary>
  /// Handles the mouse events when the read coordinate tools is selected.
  /// </summary>
  public class ReadXYCoordinatesMouseHandler : MouseStateHandler
  {
    /// <summary>
    /// Number of the current layer.
    /// </summary>
    protected int _LayerNumber;

    /// <summary>
    /// Coordinates of the red data reader cross (in printable coordinates)
    /// </summary>
    protected PointF m_Cross;

    /// <summary>
    /// The parent graph controller.
    /// </summary>
    protected GraphController _grac;

    protected float _MovementIncrement=4;

    public ReadXYCoordinatesMouseHandler(GraphController grac)
    {
      _grac = grac;

      if(_grac.View!=null)
        _grac.View.SetPanelCursor(Cursors.Cross);
    }

  
    /// <summary>
    /// Handles the MouseDown event when the plot point tool is selected
    /// </summary>
    /// <param name="e">The mouse event args</param>
     
    public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseDown(e);

      PointF mouseXY = new PointF(e.X,e.Y);
      m_Cross = _grac.PixelToPrintableAreaCoordinates(mouseXY);

      DisplayCrossCoordinates();
      
      _grac.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross
         
     
    } // end of function


    void DisplayCrossCoordinates()
    {
     

      XYPlotLayer layer = _grac.ActiveLayer;
      if(layer==null)
        return;

      PointF layerXY = layer.GraphToLayerCoordinates(m_Cross);

     

      Logical3D r;
      layer.CoordinateSystem.LayerToLogicalCoordinates(layerXY.X,layerXY.Y, out r);
      Altaxo.Data.AltaxoVariant xphys = layer.XAxis.NormalToPhysicalVariant(r.RX);
      Altaxo.Data.AltaxoVariant yphys = layer.YAxis.NormalToPhysicalVariant(r.RY);

      this.DisplayData(_grac.CurrentLayerNumber,xphys,yphys);
    }


    void DisplayData(int layerNumber, Altaxo.Data.AltaxoVariant x, Altaxo.Data.AltaxoVariant y)
    {
      Current.DataDisplay.WriteOneLine(string.Format(
        "Layer({0}) X={1}, Y={2}",
        layerNumber,
        x,
        y));
    }


    /// <summary>
    /// Moves the cross along the plot.
    /// </summary>
    /// <param name="increment"></param>
    void MoveLeftRight(float increment)
    {
      
      m_Cross.X += increment;

      DisplayCrossCoordinates();

      _grac.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross
    }

    /// <summary>
    /// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
    /// </summary>
    /// <param name="increment"></param>
    void MoveUpDown(float increment)
    {
      
      m_Cross.Y += increment;

      DisplayCrossCoordinates();

      _grac.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross

      
    }






    public override void AfterPaint( Graphics g)
    {
      base.AfterPaint (g);
      g.TranslateTransform(_grac.Doc.PrintableBounds.X,_grac.Doc.PrintableBounds.Y);

      // draw a red cross onto the selected data point

      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X+1,m_Cross.Y,m_Cross.X+10,m_Cross.Y);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X-1,m_Cross.Y,m_Cross.X-10,m_Cross.Y);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X,m_Cross.Y+1,m_Cross.X,m_Cross.Y+10);
      g.DrawLine(System.Drawing.Pens.Red,m_Cross.X,m_Cross.Y-1,m_Cross.X,m_Cross.Y-10);
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
        MoveLeftRight(-_MovementIncrement);
        return true;
      }
      else if(keyData == Keys.Right)
      {
        System.Diagnostics.Trace.WriteLine("Read tool key handler, right key pressed!");
        MoveLeftRight(_MovementIncrement);
        return true;
      }
      else if(keyData == Keys.Up)
      {
        MoveUpDown(-_MovementIncrement);
        return true;
      }
      else if(keyData == Keys.Down)
      {
        MoveUpDown(_MovementIncrement);
        return true;
      }
      else if(keyData == Keys.Add || keyData == Keys.Oemplus)
      {
        if(_MovementIncrement<1024)
          _MovementIncrement *=2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}",_MovementIncrement));
        return true;
      }
      else if(keyData == Keys.Subtract  || keyData == Keys.OemMinus)
      {
        if(_MovementIncrement>=(1/1024.0))
          _MovementIncrement /=2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}",_MovementIncrement));

        return true;
      }


      return false; // per default the key is not processed
    }


    

  } // end of class

}
