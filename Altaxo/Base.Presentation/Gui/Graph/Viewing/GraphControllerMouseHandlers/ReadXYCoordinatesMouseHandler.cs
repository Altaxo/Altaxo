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
using System.Windows.Input;

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
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
    protected GraphViewWpf _grac;

    protected float _MovementIncrement=4;

    /// <summary>
    /// If true, the tool show the printable coordinates instead of the physical values.
    /// </summary>
    protected bool _showPrintableCoordinates;

    public ReadXYCoordinatesMouseHandler(GraphViewWpf grac)
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
     
    public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
    {
      base.OnMouseDown(position, e);

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) // if M is pressed, we don't move the cross, but instead we display not only the cross coordinates but also the differenz
			{
				var printableCoord = _grac.GuiController.PixelToPrintableAreaCoordinates(position);
				DisplayCrossCoordinatesAndDifference(printableCoord);

			}
			else
			{
				m_Cross = _grac.GuiController.PixelToPrintableAreaCoordinates(position);
				DisplayCrossCoordinates();
			}

      
      _grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
         
     
    } // end of function


		bool CalculateCrossCoordinates(PointF cross, out Altaxo.Data.AltaxoVariant x, out Altaxo.Data.AltaxoVariant y)
		{
			XYPlotLayer layer = _grac.ActiveLayer;
			if (layer == null)
			{
				x = new AltaxoVariant();
				y = new AltaxoVariant();
				return false;
			}

			PointF layerXY = layer.GraphToLayerCoordinates(cross);



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
        if (CalculateCrossCoordinates(m_Cross, out xphys, out  yphys))
          Current.DataDisplay.WriteOneLine(string.Format(
         "Layer({0}) X={1}, Y={2}",
         _grac.ActiveLayer.Number,
         xphys,
         yphys));
      }
    }

		void DisplayCrossCoordinatesAndDifference(PointF printableAreaCoords)
		{
			if (_showPrintableCoordinates)
			{
				Current.DataDisplay.WriteTwoLines(
					string.Format("Layer({0}) XS={1}, YS={2}",_grac.ActiveLayer.Number,	m_Cross.X, m_Cross.Y),
					string.Format("DeltaX={0}, DeltaY={1}, Distance={2}", printableAreaCoords.X-m_Cross.X, printableAreaCoords.Y-m_Cross.Y, Calc.RMath.Hypot(printableAreaCoords.X-m_Cross.X, printableAreaCoords.Y-m_Cross.Y))
					);
			}
			else
			{
				AltaxoVariant xphys, yphys;
				AltaxoVariant xphys2, yphys2;
				if (CalculateCrossCoordinates(m_Cross, out xphys, out  yphys) && CalculateCrossCoordinates(printableAreaCoords, out xphys2, out yphys2))
				{
					double distance=double.NaN;
					AltaxoVariant dx=double.NaN, dy=double.NaN;
					try
					{
						dx = xphys2-xphys;
						dy = yphys2-yphys;
						var r2 = dx*dx+dy*dy;
						distance = Math.Sqrt(r2);
					}
					catch(Exception)
					{
					}

					Current.DataDisplay.WriteTwoLines(
						string.Format("Layer({0}) X={1}, Y={2}", _grac.ActiveLayer.Number, xphys, yphys),
						string.Format("DeltaX={0}, DeltaY={1}, Distance={2}", dx, dy, distance) 
						);
				}
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

      _grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
    }

    /// <summary>
    /// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
    /// </summary>
    /// <param name="increment"></param>
    void MoveUpDown(float increment)
    {
      
      m_Cross.Y += increment;

      DisplayCrossCoordinates();

      _grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross

      
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
		public override bool ProcessCmdKey(KeyEventArgs e)
    {
			var keyData = e.Key;
      if(keyData == Key.Left)
      {
        MoveLeftRight(-_MovementIncrement);
        return true;
      }
      else if(keyData == Key.Right)
      {
        MoveLeftRight(_MovementIncrement);
        return true;
      }
      else if(keyData == Key.Up)
      {
        MoveUpDown(-_MovementIncrement);
        return true;
      }
      else if(keyData == Key.Down)
      {
        MoveUpDown(_MovementIncrement);
        return true;
      }
      else if(keyData == Key.Add || keyData == Key.OemPlus)
      {
        if(_MovementIncrement<1024)
          _MovementIncrement *=2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}",_MovementIncrement));
        return true;
      }
      else if(keyData == Key.Subtract  || keyData == Key.OemMinus)
      {
        if(_MovementIncrement>=(1/1024.0))
          _MovementIncrement /=2;

        Current.DataDisplay.WriteOneLine(string.Format("MoveIncrement: {0}",_MovementIncrement));

        return true;
      }
      else if (keyData == Key.Q)
      {
        _showPrintableCoordinates = !_showPrintableCoordinates;
        Current.DataDisplay.WriteOneLine("Switched to " +  (_showPrintableCoordinates ? "printable coordinates!" : "physical values!"));
      }
      else if (keyData == Key.Enter)
      {
        if (_showPrintableCoordinates)
        {
          Current.Console.WriteLine("{0}\t{1}", m_Cross.X, m_Cross.Y);
        }
        else
        {
          AltaxoVariant xphys, yphys;
          if (CalculateCrossCoordinates(m_Cross, out xphys, out  yphys))
            Current.Console.WriteLine("{0}\t{1}", xphys, yphys);
        }
        return true;
      }

      return false; // per default the key is not processed
    }


    

  } // end of class

}
