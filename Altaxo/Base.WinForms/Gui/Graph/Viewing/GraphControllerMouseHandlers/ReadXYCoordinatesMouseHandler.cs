﻿#region Copyright
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
    protected GraphView _grac;

    protected float _MovementIncrement=4;

    /// <summary>
    /// If true, the tool show the printable coordinates instead of the physical values.
    /// </summary>
    protected bool _showPrintableCoordinates;

    public ReadXYCoordinatesMouseHandler(GraphView grac)
    {
      _grac = grac;

      if(_grac!=null)
        _grac.SetPanelCursor(Cursors.Cross);
    }

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.ReadXYCoordinates; }
		}


    /// <summary>
    /// Handles the MouseDown event when the plot point tool is selected
    /// </summary>
    /// <param name="e">The mouse event args</param>

    public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      base.OnMouseDown(e);

      PointF mouseXY = new PointF(e.X,e.Y);
      m_Cross = _grac.WinFormsController.PixelToPrintableAreaCoordinates(mouseXY);

      DisplayCrossCoordinates();

      _grac.WinFormsController.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross


    } // end of function


		bool CalculateCrossCoordinates(out Altaxo.Data.AltaxoVariant x, out Altaxo.Data.AltaxoVariant y)
		{
			XYPlotLayer layer = _grac.ActiveLayer;
			if (layer == null)
			{
				x = new AltaxoVariant();
				y = new AltaxoVariant();
				return false;
			}

			PointF layerXY = layer.GraphToLayerCoordinates(m_Cross);



			Logical3D r;
			layer.CoordinateSystem.LayerToLogicalCoordinates(layerXY.X, layerXY.Y, out r);
			x = layer.XAxis.NormalToPhysicalVariant(r.RX);
			y = layer.YAxis.NormalToPhysicalVariant(r.RY);
			return true;
		}

    void DisplayCrossCoordinates()
    {
      if (_showPrintableCoordinates)
      {
        Current.DataDisplay.WriteOneLine(string.Format(
        "Layer({0}) XS={1}, YS={2}",
        _grac.ActiveLayer.Number,
        m_Cross.X,
        m_Cross.Y));
      }
      else
      {
        AltaxoVariant xphys, yphys;
        if (CalculateCrossCoordinates(out xphys, out  yphys))
          Current.DataDisplay.WriteOneLine(string.Format(
         "Layer({0}) X={1}, Y={2}",
         _grac.ActiveLayer.Number,
         xphys,
         yphys));
      }
    }




    /// <summary>
    /// Moves the cross along the plot.
    /// </summary>
    /// <param name="increment"></param>
    void MoveLeftRight(float increment)
    {

      m_Cross.X += increment;

      DisplayCrossCoordinates();

      _grac.WinFormsController.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross
    }

    /// <summary>
    /// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
    /// </summary>
    /// <param name="increment"></param>
    void MoveUpDown(float increment)
    {

      m_Cross.Y += increment;

      DisplayCrossCoordinates();

      _grac.WinFormsController.RepaintGraphArea(); // no refresh necessary, only invalidate to show the cross


    }






    public override void AfterPaint( Graphics g)
    {
      base.AfterPaint (g);
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
        MoveLeftRight(-_MovementIncrement);
        return true;
      }
      else if(keyData == Keys.Right)
      {
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
      else if (keyData == Keys.Q)
      {
        _showPrintableCoordinates = !_showPrintableCoordinates;
        Current.DataDisplay.WriteOneLine("Switched to " +  (_showPrintableCoordinates ? "printable coordinates!" : "physical values!"));
      }
      else if (keyData == Keys.Enter)
      {
        if (_showPrintableCoordinates)
        {
          Current.Console.WriteLine("{0}\t{1}", m_Cross.X, m_Cross.Y);
        }
        else
        {
          AltaxoVariant xphys, yphys;
          if (CalculateCrossCoordinates(out xphys, out  yphys))
            Current.Console.WriteLine("{0}\t{1}", xphys, yphys);
        }
        return true;
      }

      return false; // per default the key is not processed
    }




  } // end of class

}
