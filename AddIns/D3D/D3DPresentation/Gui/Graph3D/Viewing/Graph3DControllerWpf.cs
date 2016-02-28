#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Graph;
	using Altaxo.Graph.Graph3D;
	using Altaxo.Graph.Graph3D.Camera;
	using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
	using Altaxo.Graph.Graph3D.Plot;
	using Altaxo.Graph.Graph3D.Plot.Groups;
	using Altaxo.Graph.Graph3D.Shapes;
	using Altaxo.Gui.Graph3D.Viewing.GraphControllerMouseHandlers;
	using Altaxo.Main;

	[UserControllerForObject(typeof(GraphDocument))]
	[ExpectedTypeOfView(typeof(IGraph3DView))]
	public class Graph3DControllerWpf : Graph3DController
	{
		/// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
		protected MouseStateHandler _mouseState;

		/// <summary>
		/// The camera as it was when the middle mouse button was pressed. A value != null indicates that the middle button is currently pressed.
		/// This value is used to rotate the camera when the middle mouse button is pressed and the mouse is moved.
		/// </summary>
		private CameraBase _middleButtonPressed_InitialCamera;

		/// <summary>
		/// The position of the mouse as it was when the middle mouse button was pressed. This value is valid only if the field <see cref="_middleButtonPressed_InitialCamera"/> is not null.
		/// </summary>
		private PointD3D _middleButtonPressed_InitialPosition;

		private enum MiddelButtonAction { RotateCamera, MoveCamera, ZoomCamera }

		/// <summary>
		/// The action that is executed if the middle mouse button is pressed and the mouse is moved. This value is only valid if if the field <see cref="_middleButtonPressed_InitialCamera"/> is not null.
		/// </summary>
		private MiddelButtonAction _middleButtonCurrentAction;

		private static IList<IHitTestObject> _emptyReadOnlyList;

		static Graph3DControllerWpf()
		{
			_emptyReadOnlyList = new List<IHitTestObject>().AsReadOnly();

			// register here editor methods
			XYPlotLayerController.RegisterEditHandlers();
			XYZPlotLayer.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
			TextGraphic.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
			TextGraphic.TextGraphicsEditorMethod = new DoubleClickHandler(EhEditTextGraphics);
		}

		public Graph3DControllerWpf()
		{
			_mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(this);
		}

		public Graph3DControllerWpf(GraphDocument graphdoc)
			: base(graphdoc)
		{
			_mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(this);
		}

		internal Graph3DControl ViewWpf
		{
			get
			{
				return _view as Graph3DControl;
			}
		}

		internal void SetPanelCursor(Cursor arrow)
		{
			var view = ViewWpf;

			if (null != view)
				view.SetPanelCursor(arrow);
		}

		public override IList<IHitTestObject> SelectedObjects
		{
			get
			{
				if (_mouseState is ObjectPointerMouseHandler)
					return ((ObjectPointerMouseHandler)_mouseState).SelectedObjects;
				else
					return _emptyReadOnlyList;
			}
		}

		/// <summary>
		/// Handles the mouse down event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseDown(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnMouseDown(position, e);

			bool isSHIFTpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
			bool isCTRLpressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Pressed)
			{
				_middleButtonPressed_InitialPosition = position;
				_middleButtonPressed_InitialCamera = _doc.Camera;
				if (!isSHIFTpressed && !isCTRLpressed)
					_middleButtonCurrentAction = MiddelButtonAction.RotateCamera;
				else if (isSHIFTpressed && !isCTRLpressed)
					_middleButtonCurrentAction = MiddelButtonAction.MoveCamera;
				else if (!isSHIFTpressed && isCTRLpressed)
					_middleButtonCurrentAction = MiddelButtonAction.ZoomCamera;
				else
					_middleButtonPressed_InitialCamera = null; // if inconsistent keys, then no action at all
			}
		}

		/// <summary>
		/// Handles the mouse up event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseUp(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnMouseUp(position, e);

			if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released)
			{
				_middleButtonPressed_InitialCamera = null;
			}
		}

		/// <summary>
		/// Handles the mouse move event onto the graph in the controller class.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseMove(PointD3D position, MouseEventArgs e)
		{
			_mouseState.OnMouseMove(position, e);

			if (e.MiddleButton == MouseButtonState.Released)
			{
				_middleButtonPressed_InitialCamera = null;
			}
			else if (null != _middleButtonPressed_InitialCamera)
			{
				switch (_middleButtonCurrentAction)
				{
					case MiddelButtonAction.RotateCamera:
						{
							double dx = position.X - _middleButtonPressed_InitialPosition.X;
							double dy = position.Y - _middleButtonPressed_InitialPosition.Y;
							//Doc.Camera = CameraRotateDegrees(_middleButtonPressed_InitialCamera, dx * 540, -dy * 540);
							Doc.Camera = ModelRotateDegrees(_middleButtonPressed_InitialCamera, _doc.RootLayer.Size, dx * 540, -dy * 540);
						}
						break;

					case MiddelButtonAction.MoveCamera:
						{
							double dx = position.X - _middleButtonPressed_InitialPosition.X;
							double dy = position.Y - _middleButtonPressed_InitialPosition.Y;
							Doc.Camera = CameraMoveRelative(_middleButtonPressed_InitialCamera, dx, dy);
						}
						break;

					case MiddelButtonAction.ZoomCamera:
						{
							double dy = position.Y - _middleButtonPressed_InitialPosition.Y;
							Doc.Camera = CameraZoomByMouseWheel(_middleButtonPressed_InitialCamera, 0.5, 0.5, position.Z, (dy * 5));
						}
						break;
				}
			}
		}

		/// <summary>
		/// Called if a key is pressed in the view.
		/// </summary>
		/// <param name="e">Key event arguments.</param>
		/// <returns></returns>
		public bool EhView_ProcessCmdKey(KeyEventArgs e)
		{
			bool wasHandled = false;
			if (this._mouseState != null)
			{
				wasHandled = this._mouseState.ProcessCmdKey(e);
				if (wasHandled)
					return wasHandled;
			}

			// handle keys common to all mouse handlers
			/*
			if (e.Key == Key.Up)
				EhMoveOrRoll(0, 1, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Down)
				EhMoveOrRoll(0, -1, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Right)
				EhMoveOrRoll(1, 0, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Left)
				EhMoveOrRoll(-1, 0, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			*/

			return false;
		}

		internal void RenderOverlay()
		{
			var view = _view;

			if (null != view)
			{
				var g = view.GetGraphicContextForOverlay();
				_mouseState.AfterPaint(g);
				view.SetOverlayGeometry(g);
				view.TriggerRendering();
			}
		}

		/// <summary>
		/// Handles the click onto the graph event in the controller class.
		/// </summary>
		/// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelMouseClick(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnClick(position, e);
		}

		/// <summary>
		/// Handles the double click onto the graph event in the controller class.
		/// </summary>
		/// <param name="position">Mouse position.  X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
		/// <param name="e"></param>
		public virtual void EhView_GraphPanelMouseDoubleClick(PointD3D position, MouseButtonEventArgs e)
		{
			_mouseState.OnDoubleClick(position, e);
		}

		/// <summary>
		/// Handles the double click event onto a plot item.
		/// </summary>
		/// <param name="hit">Object containing information about the double clicked object.</param>
		/// <returns>True if the object should be deleted, false otherwise.</returns>
		protected static bool EhEditTextGraphics(IHitTestObject hit)
		{
			var layer = hit.ParentLayer;
			TextGraphic tg = (TextGraphic)hit.HittedObject;

			bool shouldDeleted = false;

			object tgoo = tg;
			if (Current.Gui.ShowDialog(ref tgoo, "Edit text", true))
			{
				tg = (TextGraphic)tgoo;
				if (tg == null || tg.Empty)
				{
					if (null != hit.Remove)
						shouldDeleted = hit.Remove(hit);
					else
						shouldDeleted = false;
				}
				else
				{
					if (tg.ParentObject is IChildChangedEventSink)
						((IChildChangedEventSink)tg.ParentObject).EhChildChanged(tg, EventArgs.Empty);
				}
			}

			return shouldDeleted;
		}

		/// <summary>
		/// Handles the double click event onto a plot item.
		/// </summary>
		/// <param name="hit">Object containing information about the double clicked object.</param>
		/// <returns>True if the object should be deleted, false otherwise.</returns>
		protected static bool EhEditPlotItem(IHitTestObject hit)
		{
			XYZPlotLayer actLayer = hit.ParentLayer as XYZPlotLayer;
			IGPlotItem pa = (IGPlotItem)hit.HittedObject;

			Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()), true);

			return false;
		}
	}
}