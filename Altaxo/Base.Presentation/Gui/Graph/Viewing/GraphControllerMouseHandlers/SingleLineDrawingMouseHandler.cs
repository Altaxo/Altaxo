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

using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the drawing of a straight single line.
	/// </summary>
	public class SingleLineDrawingMouseHandler : MouseStateHandler
	{
		#region Member variables

		protected GraphControllerWpf _grac;

		protected Altaxo.Gui.Graph.Viewing.GraphToolType NextMouseHandlerType = Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer;

		protected POINT[] _Points = new POINT[2];
		protected int _currentPoint;

		#endregion Member variables

		public SingleLineDrawingMouseHandler(GraphControllerWpf view)
		{
			this._grac = view;

			if (_grac != null)
				_grac.SetPanelCursor(Cursors.Pen);
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
		public override void OnClick(PointD2D position, MouseButtonEventArgs e)
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

			_Points[_currentPoint].LayerCoordinates = layerCoord;
			_Points[_currentPoint].RootLayerCoordinates = rootLayerCoord;
			_currentPoint++;

			if (2 == _currentPoint)
			{
				FinishDrawing();
				_currentPoint = 0;
				_grac.SetGraphToolFromInternal(NextMouseHandlerType);
			}
		}

		public override void OnMouseMove(PointD2D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			_positionCurrentMouseInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);

			ModifyCurrentMousePrintAreaCoordinate();

			_grac.RenderOverlay();
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

					_positionCurrentMouseInRootLayerCoordinates = new PointD2D(
						(x + _Points[_currentPoint - 1].RootLayerCoordinates.X),
						(y + _Points[_currentPoint - 1].RootLayerCoordinates.Y)
						);
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

			for (int i = 1; i < this._currentPoint; i++)
				g.DrawLine(Pens.Blue, (PointF)_Points[i - 1].RootLayerCoordinates, (PointF)_Points[i].RootLayerCoordinates);

			if (_currentPoint > 0)
				g.DrawLine(Pens.Blue, (PointF)_Points[_currentPoint - 1].RootLayerCoordinates, (PointF)_positionCurrentMouseInRootLayerCoordinates);
		}

		protected virtual void FinishDrawing()
		{
			LineShape go = new LineShape(_Points[0].LayerCoordinates, _Points[1].LayerCoordinates, _grac.Doc.GetPropertyContext());

			// deselect the text tool
			_grac.SetGraphToolFromInternal(Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer);
			_grac.ActiveLayer.GraphObjects.Add(go);
		}
	}
}