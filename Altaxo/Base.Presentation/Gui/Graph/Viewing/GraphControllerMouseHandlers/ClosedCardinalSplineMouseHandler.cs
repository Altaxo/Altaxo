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
using System.Linq;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the drawing of a straight single line.
	/// </summary>
	public class ClosedCardinalSplineMouseHandler : MouseStateHandler
	{
		#region Member variables

		protected GraphViewWpf _grac;

		protected PointF _currentMousePrintAreaCoord;

		protected Altaxo.Gui.Graph.Viewing.GraphToolType NextMouseHandlerType = Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer;

		protected double _tension;

		protected struct POINT
		{
			public PointF printAreaCoord;
			public PointF layerCoord;
		}

		protected List<POINT> _Points = new List<POINT>();
		protected int _currentPoint;


		#endregion

		public ClosedCardinalSplineMouseHandler(GraphViewWpf view)
		{
			this._grac = view;

			if (_grac != null)
				_grac.SetPanelCursor(Cursors.Pen);

			_tension = OpenCardinalSpline.DefaultTension;
		}

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.ClosedCardinalSplineDrawing; }
		}


		/// <summary>
		/// Handles the drawing of a straight single line.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		/// <returns>The mouse state handler for handling the next mouse events.</returns>
		public override void OnClick(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{
			base.OnClick(position, e);

			// get the page coordinates (in Point (1/72") units)
			//PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
			PointF printAreaCoord = _currentMousePrintAreaCoord;
			// with knowledge of the current active layer, calculate the layer coordinates from them
			PointF layerCoord = _grac.ActiveLayer.GraphToLayerCoordinates(printAreaCoord);

			if (e.ChangedButton == MouseButton.Right)
			{
				FinishDrawing();
			}
			else
			{
				_Points.Add(new POINT() { layerCoord = layerCoord, printAreaCoord = printAreaCoord });
				_currentPoint++;
			}

		}


		public override void OnMouseMove(Altaxo.Graph.PointD2D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			_currentMousePrintAreaCoord = _grac.GuiController.PixelToPrintableAreaCoordinates(position);

			ModifyCurrentMousePrintAreaCoordinate();

			_grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
		}

		public override void OnMouseDown(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{
			base.OnMouseDown(position, e);
			if (e.ChangedButton == MouseButton.Right)
				FinishDrawing();
		}

		public override bool ProcessCmdKey(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					FinishDrawing();
					return true;
				case Key.OemPlus:
					_tension *= 2;
					Current.DataDisplay.WriteOneLine(string.Format("Tension now set to {0}", _tension));
					_grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
					return true;
				case Key.OemMinus:
					_tension /= 2;
					Current.DataDisplay.WriteOneLine(string.Format("Tension now set to {0}", _tension));
					_grac.GuiController.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
					return true;
			}

			return false;
		}

		protected virtual void ModifyCurrentMousePrintAreaCoordinate()
		{
			if (_currentPoint > 0)
			{
				bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
				bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
				// draw a temporary lines of all points to the current mouse position

				if (bShiftKey && _currentPoint > 0)
				{
					double x = _currentMousePrintAreaCoord.X - _Points[_currentPoint - 1].printAreaCoord.X;
					double y = _currentMousePrintAreaCoord.Y - _Points[_currentPoint - 1].printAreaCoord.Y;

					double r = Math.Sqrt(x * x + y * y);
					double d = Math.Atan2(y, x);

					d = Math.Floor(0.5 + 12 * d / Math.PI); // lock every 15 degrees
					d = d * Math.PI / 12;

					x = r * Math.Cos(d);
					y = r * Math.Sin(d);

					_currentMousePrintAreaCoord.X = (float)(x + _Points[_currentPoint - 1].printAreaCoord.X);
					_currentMousePrintAreaCoord.Y = (float)(y + _Points[_currentPoint - 1].printAreaCoord.Y);
				}
			}
		}


		/// <summary>
		/// Draws the temporary line(s) from the first point to the mouse.
		/// </summary>
		/// <param name="g"></param>
		public override void AfterPaint(Graphics g)
		{
			base.AfterPaint(g);


			PointF[] pts = new PointF[_Points.Count + 1];
			for (int i = 0; i < _Points.Count; i++)
				pts[i] = _Points[i].printAreaCoord;
			pts[_Points.Count] = _currentMousePrintAreaCoord;

			if (pts.Length >= 3)
				g.DrawClosedCurve(Pens.Blue, pts, (float)_tension, FillMode.Alternate);
			else if (pts.Length >= 2)
				g.DrawCurve(Pens.Blue, pts, (float)_tension);

		}

		protected virtual void FinishDrawing()
		{
			_currentPoint = 0;
			if (_Points.Count > 2)
			{
				var pts = _Points.Select(x => (Altaxo.Graph.PointD2D)x.layerCoord);
				var go = new ClosedCardinalSpline(pts, _tension);
				_grac.ActiveLayer.GraphObjects.Add(go);
			}

			// deselect the text tool
			_grac.SetGraphToolFromInternal(NextMouseHandlerType);
			_grac.GuiController.InvalidateCachedGraphImageAndRepaintOffline();
		}

	}
}
