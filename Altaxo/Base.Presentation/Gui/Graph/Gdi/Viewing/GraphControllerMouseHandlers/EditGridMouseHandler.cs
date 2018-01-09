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
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Gdi.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the drawing of a straight single line.
	/// </summary>
	public class EditGridMouseHandler : MouseStateHandler
	{
		#region inner classes

		protected struct LINE
		{
			public POINT Org;
			public POINT End;
		}

		#endregion inner classes

		#region Member variables

		protected GraphToolType NextMouseHandlerType = GraphToolType.ObjectPointer;

		/// <summary>The catch distance in points (1/72 inch) on the screen. (Distance in which the mouse catches the grid line to move).</summary>
		private const double CatchDistance_InPoints = 2;

		/// <summary>The line width of the grid lines in points (1/72 inch) on the screen.</summary>
		private const double GridLineWidth_InPoints = 1;

		protected GraphController _grac;

		/// <summary>The catch distance in root layer units. (Distance in which the mouse catches the grid line to move).</summary>
		protected double _catchDistance_RLC;

		/// <summary>The layer for which to edit the grid.</summary>
		protected HostLayer _layerToEdit;

		/// <summary>The x-grid line positions (org and end) in layer coordinates and root layer coordinates.</summary>
		protected LINE[] _xGridLines;

		/// <summary>The y-grid line positions (org and end) in layer coordinates and root layer coordinates.</summary>
		protected LINE[] _yGridLines;

		/// <summary>If this variable has a value, it designates the index of the  grid line that is currently moved.</summary>
		private int? _movedGridLineIndex;

		/// <summary>If true, the currently moved grid line is an x-grid line, otherwise false.</summary>
		private bool _movedGridLineIsXGrid;

		#endregion Member variables

		public override GraphToolType GraphToolType
		{
			get { return GraphToolType.EditGrid; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditGridMouseHandler"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <exception cref="System.ArgumentNullException">view</exception>
		public EditGridMouseHandler(GraphController view)
		{
			if (null == view)
				throw new ArgumentNullException("view");

			this._grac = view;

			_grac.SetPanelCursor(Cursors.Arrow);

			_catchDistance_RLC = 2 / _grac.ZoomFactor;

			_layerToEdit = _grac.ActiveLayer;

			while (_layerToEdit != null && _layerToEdit.Layers.Count == 0)
				_layerToEdit = _layerToEdit.ParentLayer; // search for a parent layer which has childs

			if (_layerToEdit == null)
				_layerToEdit = _grac.Doc.RootLayer;

			if (_layerToEdit.Grid.XPartitioning.Count == 0 && _layerToEdit.Grid.YPartitioning.Count == 0)
			{
				var result = Current.Gui.YesNoMessageBox(
					string.Format("It seems that layer '{0}' does not define a grid yet.\r\n" +
					"Do you want to define a grid now based on the position of the existing child layer(s)?",
					Altaxo.Main.RelativeDocumentPath.GetRelativePathFromTo(_grac.Doc, _layerToEdit).ToString()),
					"No grid defined!",
					true);

				if (result)
				{
					_layerToEdit.CreateGridIfNullOrEmpty();
				}
				else
				{
					FinishDrawing();
				}
			}

			UpdateCachedGridLinePositions();
		}

		public override void OnMouseDown(PointD2D position, MouseButtonEventArgs e)
		{
			base.OnMouseDown(position, e);

			_positionCurrentMouseInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);

			double minDistanceSquared;
			int minIndex;
			bool minInXGrid;
			GetClosestGridLine(out minDistanceSquared, out minIndex, out minInXGrid);

			if (minDistanceSquared < _catchDistance_RLC * _catchDistance_RLC)
			{
				_movedGridLineIndex = minIndex;
				_movedGridLineIsXGrid = minInXGrid;
			}
			else
			{
				FinishDrawing();
			}
		}

		public override void OnMouseUp(PointD2D position, MouseButtonEventArgs e)
		{
			base.OnMouseUp(position, e);
			_movedGridLineIndex = null;
			_grac.SetPanelCursor(Cursors.Arrow);
		}

		public override void OnMouseMove(PointD2D position, MouseEventArgs e)
		{
			base.OnMouseMove(position, e);

			_positionCurrentMouseInRootLayerCoordinates = _grac.ConvertMouseToRootLayerCoordinates(position);

			if (_movedGridLineIndex.HasValue)
			{
				ModifyCurrentMouseRootLayerCoordinates();
				var positionCurrentMouseInLayerCoordinates = _layerToEdit.TransformCoordinatesFromRootToHere(_positionCurrentMouseInRootLayerCoordinates);

				if (_movedGridLineIsXGrid)
				{
					var part = _layerToEdit.Grid.XPartitioning;
					part.AdjustIndexToMatchPosition(_layerToEdit.Size.X, _movedGridLineIndex.Value, positionCurrentMouseInLayerCoordinates.X);
				}
				else
				{
					var part = _layerToEdit.Grid.YPartitioning;
					part.AdjustIndexToMatchPosition(_layerToEdit.Size.Y, _movedGridLineIndex.Value, positionCurrentMouseInLayerCoordinates.Y);
				}
				_grac.RenderOverlay();
			}
			else // not moving a grid line
			{
				double minDistanceSquared;
				int minIndex;
				bool minInXGrid;
				GetClosestGridLine(out minDistanceSquared, out minIndex, out minInXGrid);
				if (minDistanceSquared < _catchDistance_RLC * _catchDistance_RLC)
				{
					if (_grac != null)
						_grac.SetPanelCursor(Cursors.ScrollAll);
				}
				else
				{
					if (_grac != null)
						_grac.SetPanelCursor(Cursors.Arrow);
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

			UpdateCachedGridLinePositions();

			_catchDistance_RLC = CatchDistance_InPoints / _grac.ZoomFactor; // update catch distance in case user has zoomed in or out

			using (var pen = new Pen(Brushes.Blue, (float)(GridLineWidth_InPoints / _grac.ZoomFactor)))
			{
				for (int i = 0; i < _xGridLines.Length; ++i)
				{
					g.DrawLine(pen, (PointF)_xGridLines[i].Org.RootLayerCoordinates, (PointF)_xGridLines[i].End.RootLayerCoordinates);
				}

				for (int i = 0; i < _yGridLines.Length; ++i)
				{
					g.DrawLine(pen, (PointF)_yGridLines[i].Org.RootLayerCoordinates, (PointF)_yGridLines[i].End.RootLayerCoordinates);
				}
			}
		}

		protected virtual void FinishDrawing()
		{
			_layerToEdit.Grid.XPartitioning.NormalizeRelativeValues();
			_layerToEdit.Grid.YPartitioning.NormalizeRelativeValues();

			// deselect the text tool
			_grac.SetGraphToolFromInternal(GraphToolType.ObjectPointer);
		}

		/// <summary>
		/// Modifies the current mouse root layer coordinates, so that Shift moves in 1% steps of the layer size, Ctrl in 5% steps, and Shift+Ctrl in 10% steps.
		/// </summary>
		protected virtual void ModifyCurrentMouseRootLayerCoordinates()
		{
			bool bControlKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Control); // Control pressed
			bool bShiftKey = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
			// draw a temporary lines of all points to the current mouse position

			int roundToParts = 0;
			if (bControlKey && bShiftKey)
				roundToParts = 10;
			else if (bControlKey)
				roundToParts = 20;
			else if (bShiftKey)
				roundToParts = 100;

			if (roundToParts > 0)
			{
				var layerCoord = _layerToEdit.TransformCoordinatesFromRootToHere(_positionCurrentMouseInRootLayerCoordinates);

				// Round the layer Coordinate to 1/100 of the size

				var layerCoordRounded = new PointD2D(
					_layerToEdit.Size.X * Math.Round(roundToParts * layerCoord.X / _layerToEdit.Size.X) / roundToParts,
					_layerToEdit.Size.Y * Math.Round(roundToParts * layerCoord.Y / _layerToEdit.Size.Y) / roundToParts);

				_positionCurrentMouseInRootLayerCoordinates = _layerToEdit.TransformCoordinatesFromHereToRoot(layerCoordRounded);
			}
		}

		/// <summary>
		/// Gets the grid line closest to our last mouse position.
		/// </summary>
		/// <param name="minDistanceSquared">The minimum distance (squared) between the closest grid line and our mouse position.</param>
		/// <param name="minIndex">The index of the closest grid line.</param>
		/// <param name="minInXGrid">If set to <c>true</c> the closest grid line is an x-grid line; otherwise it is an y-grid line.</param>
		private void GetClosestGridLine(out double minDistanceSquared, out int minIndex, out bool minInXGrid)
		{
			minDistanceSquared = double.PositiveInfinity;
			minIndex = 0;
			minInXGrid = true;
			for (int i = 0; i < _xGridLines.Length - 1; ++i)
			{
				var line = _xGridLines[i];
				var dist = PointD2D.SquareDistanceLineToPoint(_positionCurrentMouseInRootLayerCoordinates, line.Org.RootLayerCoordinates, line.End.RootLayerCoordinates);
				if (minDistanceSquared > dist)
				{
					minDistanceSquared = dist;
					minIndex = i;
					minInXGrid = true;
				}
			}
			for (int i = 0; i < _yGridLines.Length - 1; ++i)
			{
				var line = _yGridLines[i];
				var dist = PointD2D.SquareDistanceLineToPoint(_positionCurrentMouseInRootLayerCoordinates, line.Org.RootLayerCoordinates, line.End.RootLayerCoordinates);
				if (minDistanceSquared > dist)
				{
					minDistanceSquared = dist;
					minIndex = i;
					minInXGrid = false;
				}
			}
		}

		/// <summary>
		/// Updates the grid line positions cached in the member variables _xGridLines and _yGridLines.
		/// </summary>
		protected void UpdateCachedGridLinePositions()
		{
			var grid = _layerToEdit.Grid;
			var xpos = grid.XPartitioning.GetPartitionPositions(_layerToEdit.Size.X);
			var ypos = grid.YPartitioning.GetPartitionPositions(_layerToEdit.Size.Y);

			if (null == _xGridLines || _xGridLines.Length != xpos.Length)
				_xGridLines = new LINE[xpos.Length];

			if (null == _yGridLines || _yGridLines.Length != ypos.Length)
				_yGridLines = new LINE[ypos.Length];

			for (int i = 0; i < xpos.Length; ++i)
			{
				var orgL = new PointD2D(xpos[i], 0);
				var endL = new PointD2D(xpos[i], _layerToEdit.Size.Y);
				_xGridLines[i] = new LINE
				{
					Org = new POINT
					{
						LayerCoordinates = orgL,
						RootLayerCoordinates = _layerToEdit.TransformCoordinatesFromHereToRoot(orgL)
					},
					End = new POINT
					{
						LayerCoordinates = endL,
						RootLayerCoordinates = _layerToEdit.TransformCoordinatesFromHereToRoot(endL)
					}
				};
			}

			for (int i = 0; i < ypos.Length; ++i)
			{
				var orgL = new PointD2D(0, ypos[i]);
				var endL = new PointD2D(_layerToEdit.Size.X, ypos[i]);
				_yGridLines[i] = new LINE
				{
					Org = new POINT
					{
						LayerCoordinates = orgL,
						RootLayerCoordinates = _layerToEdit.TransformCoordinatesFromHereToRoot(orgL)
					},
					End = new POINT
					{
						LayerCoordinates = endL,
						RootLayerCoordinates = _layerToEdit.TransformCoordinatesFromHereToRoot(endL)
					}
				};
			}
		}
	}
}