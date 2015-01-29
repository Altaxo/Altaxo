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

using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the drawing of a straight single line.
	/// </summary>
	public class OpenCardinalSplineMouseHandler : MouseStateHandler
	{
		#region Member variables

		protected GraphControllerWpf _grac;

		protected Altaxo.Gui.Graph.Viewing.GraphToolType NextMouseHandlerType = Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer;

		protected double _tension;

		protected List<POINT> _Points = new List<POINT>();
		protected int _currentPoint;

		#endregion Member variables

		public OpenCardinalSplineMouseHandler(GraphControllerWpf grac)
		{
			this._grac = grac;

			if (_grac != null)
				_grac.SetPanelCursor(Cursors.Pen);

			_tension = OpenCardinalSpline.DefaultTension;
		}

		public override Altaxo.Gui.Graph.Viewing.GraphToolType GraphToolType
		{
			get { return Altaxo.Gui.Graph.Viewing.GraphToolType.SingleLineDrawing; }
		}

		/// <summary>
		/// Handles the drawing of a straight single line.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">EventArgs.</param>
		/// <returns>The mouse state handler for handling the next mouse events.</returns>
		public override void OnClick(Altaxo.Graph.PointD2D position, MouseButtonEventArgs e)
		{
			base.OnClick(position, e);

			if (0 == _currentPoint)
			{
				_cachedActiveLayer = _grac.ActiveLayer;
				_cachedActiveLayerTransformation = _cachedActiveLayer.TransformationFromRootToHere();
				_cachedActiveLayerTransformationGdi = (Matrix)_cachedActiveLayerTransformation;
			}

			// get the page coordinates (in Point (1/72") units)
			var rootLayerCoord = _positionCurrentMouseInRootLayerCoordinates;
			// with knowledge of the current active layer, calculate the layer coordinates from them
			var layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(rootLayerCoord);

			if (e.ChangedButton == MouseButton.Right)
			{
				FinishDrawing();
			}
			else
			{
				_Points.Add(new POINT() { LayerCoordinates = layerCoord, RootLayerCoordinates = rootLayerCoord });
				_currentPoint++;
			}
		}

		public override void OnMouseMove(Altaxo.Graph.PointD2D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			_positionCurrentMouseInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);

			ModifyCurrentMousePrintAreaCoordinate();

			_grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
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
					_grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
					return true;

				case Key.OemMinus:
					_tension /= 2;
					Current.DataDisplay.WriteOneLine(string.Format("Tension now set to {0}", _tension));
					_grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline();
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
					double x = _positionCurrentMouseInRootLayerCoordinates.X - _Points[_currentPoint - 1].RootLayerCoordinates.X;
					double y = _positionCurrentMouseInRootLayerCoordinates.Y - _Points[_currentPoint - 1].RootLayerCoordinates.Y;

					double r = Math.Sqrt(x * x + y * y);
					double d = Math.Atan2(y, x);

					d = Math.Floor(0.5 + 12 * d / Math.PI); // lock every 15 degrees
					d = d * Math.PI / 12;

					x = r * Math.Cos(d);
					y = r * Math.Sin(d);

					_positionCurrentMouseInRootLayerCoordinates.X = (x + _Points[_currentPoint - 1].RootLayerCoordinates.X);
					_positionCurrentMouseInRootLayerCoordinates.Y = (y + _Points[_currentPoint - 1].RootLayerCoordinates.Y);
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
				pts[i] = (PointF)_Points[i].RootLayerCoordinates;
			pts[_Points.Count] = (PointF)_positionCurrentMouseInRootLayerCoordinates;

			if (pts.Length >= 2)
				g.DrawCurve(Pens.Blue, pts, (float)_tension);

			/*

			for (int i = 1; i < this._currentPoint; i++)
				g.DrawLine(Pens.Blue, _Points[i - 1].printAreaCoord, _Points[i].printAreaCoord);

			if (_currentPoint > 0)
				g.DrawLine(Pens.Blue, _Points[_currentPoint - 1].printAreaCoord, _currentMousePrintAreaCoord);
			*/
		}

		protected virtual void FinishDrawing()
		{
			_currentPoint = 0;

			// deselect the text tool
			_grac.SetGraphToolFromInternal(NextMouseHandlerType);

			if (_Points.Count >= 2)
			{
				var pts = _Points.Select(x => (Altaxo.Graph.PointD2D)x.LayerCoordinates);
				var go = new OpenCardinalSpline(pts, _tension, _grac.Doc.GetPropertyContext());
				_grac.ActiveLayer.GraphObjects.Add(go);
			}

			_grac.InvalidateCachedGraphImageAndRepaintOffline();
		}
	}
}