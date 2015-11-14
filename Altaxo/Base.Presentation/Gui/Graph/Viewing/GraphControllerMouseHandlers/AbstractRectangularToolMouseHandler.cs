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
using Altaxo.Graph;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the drawing of a rectangle.
	/// </summary>
	public abstract class AbstractRectangularToolMouseHandler : MouseStateHandler
	{
		#region Member variables

		protected GraphControllerWpf _grac;

		protected Altaxo.Gui.Graph.Viewing.GraphToolType NextMouseHandlerType = Altaxo.Gui.Graph.Viewing.GraphToolType.ObjectPointer;

		protected POINT[] _Points = new POINT[2];
		protected int _currentPoint;

		#endregion Member variables

		public AbstractRectangularToolMouseHandler(GraphControllerWpf ctrl)
		{
			_grac = ctrl;

			if (_grac != null)
				_grac.SetPanelCursor(Cursors.Pen);
		}

		public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
		{
			base.OnMouseDown(position, e);

			if (e.ChangedButton == MouseButton.Left)
			{
				_cachedActiveLayer = _grac.ActiveLayer;
				_cachedActiveLayerTransformation = _cachedActiveLayer.TransformationFromRootToHere();
				_cachedActiveLayerTransformationGdi = (Matrix)_cachedActiveLayerTransformation;

				_currentPoint = 0;
				// get the page coordinates (in Point (1/72") units)
				PointD2D rootLayerCoord = _positionCurrentMouseInRootLayerCoordinates;
				// with knowledge of the current active layer, calculate the layer coordinates from them
				PointD2D layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(rootLayerCoord);

				_Points[_currentPoint].LayerCoordinates = layerCoord;
				_Points[_currentPoint].RootLayerCoordinates = rootLayerCoord;
				_currentPoint++;
			}
		}

		public override void OnMouseMove(PointD2D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			_positionCurrentMouseInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ModifyCurrentMousePrintAreaCoordinate();
				_grac.RenderOverlay();
			}
			else if (_currentPoint != 0)
			{
				_currentPoint = 0;
				_grac.RenderOverlay();
			}
		}

		public override void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
		{
			base.OnMouseUp(position, e);

			if (e.ChangedButton == MouseButton.Left)
			{
				// get the page coordinates (in Point (1/72") units)
				PointD2D rootLayerCoord = _positionCurrentMouseInRootLayerCoordinates;
				// with knowledge of the current active layer, calculate the layer coordinates from them
				PointD2D layerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(rootLayerCoord);

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
					if (_grac.ActiveLayer != null) // with an active layer, we transform to layer coordinates
					{
						var currMouseLayerCoord = _grac.ActiveLayer.TransformCoordinatesFromParentToHere(_positionCurrentMouseInRootLayerCoordinates);
						double x = currMouseLayerCoord.X - _Points[_currentPoint - 1].LayerCoordinates.X;
						double y = currMouseLayerCoord.Y - _Points[_currentPoint - 1].LayerCoordinates.Y;

						double r = Math.Sqrt(x * x + y * y);

						x = r * Math.Sign(x);
						y = r * Math.Sign(y);

						currMouseLayerCoord.X = (x + _Points[_currentPoint - 1].LayerCoordinates.X);
						currMouseLayerCoord.Y = (y + _Points[_currentPoint - 1].LayerCoordinates.Y);
						_positionCurrentMouseInRootLayerCoordinates = _grac.ActiveLayer.TransformCoordinatesFromHereToParent(currMouseLayerCoord);
					}
					else // without an active layer we use document coordinates
					{
						double x = _positionCurrentMouseInRootLayerCoordinates.X - _Points[_currentPoint - 1].RootLayerCoordinates.X;
						double y = _positionCurrentMouseInRootLayerCoordinates.Y - _Points[_currentPoint - 1].RootLayerCoordinates.Y;

						double r = Math.Sqrt(x * x + y * y);

						x = r * Math.Sign(x);
						y = r * Math.Sign(y);

						_positionCurrentMouseInRootLayerCoordinates.X = (x + _Points[_currentPoint - 1].RootLayerCoordinates.X);
						_positionCurrentMouseInRootLayerCoordinates.Y = (y + _Points[_currentPoint - 1].RootLayerCoordinates.Y);
					}
				}
			}
		}

		/// <summary>
		/// Draws the temporary line(s) from the first point to the mouse.
		/// </summary>
		/// <param name="g"></param>
		public override void AfterPaint(Graphics g)
		{
			if (_currentPoint >= 1)
			{
				if (null != _grac.ActiveLayer)
				{
					g.MultiplyTransform(_cachedActiveLayerTransformationGdi);
					//					g.TranslateTransform((float)_grac.ActiveLayer.Position.X, (float)_grac.ActiveLayer.Position.Y);
					//				g.RotateTransform((float)-_grac.ActiveLayer.Rotation);
					var currLayerCoord = _cachedActiveLayerTransformation.InverseTransformPoint(_positionCurrentMouseInRootLayerCoordinates);
					DrawRectangleFromLTRB(g, _Points[0].LayerCoordinates, currLayerCoord);
				}
				else
				{
					DrawRectangleFromLTRB(g, _Points[0].RootLayerCoordinates, _positionCurrentMouseInRootLayerCoordinates);
				}
			}
		}

		public RectangleD GetNormalRectangle(PointD2D a, PointD2D b)
		{
			var x = Math.Min(a.X, b.X);
			var y = Math.Min(a.Y, b.Y);

			var w = Math.Abs(a.X - b.X);
			var h = Math.Abs(a.Y - b.Y);

			return new RectangleD(x, y, w, h);
		}

		private void DrawRectangleFromLTRB(Graphics g, PointD2D a, PointD2D b)
		{
			var rect = RectangleD.FromLTRB(a.X, a.Y, b.X, b.Y);
			Pen pen = Pens.Blue;
			g.DrawLine(pen, (float)a.X, (float)a.Y, (float)b.X, (float)a.Y);
			g.DrawLine(pen, (float)b.X, (float)a.Y, (float)b.X, (float)b.Y);
			g.DrawLine(pen, (float)b.X, (float)b.Y, (float)a.X, (float)b.Y);
			g.DrawLine(pen, (float)a.X, (float)b.Y, (float)a.X, (float)a.Y);
			//      g.DrawRectangle(Pens.Blue,rect.X,rect.Y,rect.Width,rect.Height);
		}

		protected abstract void FinishDrawing();
	}
}