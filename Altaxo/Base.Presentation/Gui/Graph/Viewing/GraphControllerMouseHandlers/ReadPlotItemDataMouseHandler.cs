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

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Plot.Data;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Viewing.GraphControllerMouseHandlers
{
	/// <summary>
	/// Handles the mouse events when the read plot item tool is selected.
	/// </summary>
	public class ReadPlotItemDataMouseHandler : MouseStateHandler
	{
		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int X, int Y);

		/// <summary>
		/// Layer, in which the plot item resides which is currently selected.
		/// </summary>
		protected XYPlotLayer _layer;

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
		protected PointD2D _positionOfCrossInRootLayerCoordinates;

		protected GraphControllerWpf _grac;

		public ReadPlotItemDataMouseHandler(GraphControllerWpf grac)
		{
			_grac = grac;
			if (_grac != null)
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

			var graphXY = _grac.ConvertMouseToRootLayerCoordinates(position);

			// search for a object first
			IHitTestObject clickedObject;
			int[] clickedLayerNumber = null;
			_grac.FindGraphObjectAtPixelPosition(position, true, out clickedObject, out clickedLayerNumber);
			if (null != clickedObject && clickedObject.HittedObject is XYColumnPlotItem)
			{
				_PlotItem = (XYColumnPlotItem)clickedObject.HittedObject;
				var transXY = clickedObject.Transformation.InverseTransformPoint(graphXY);

				this._layer = (XYPlotLayer)(clickedObject.ParentLayer);
				XYScatterPointInformation scatterPoint = _PlotItem.GetNearestPlotPoint(_layer, transXY);
				this._PlotItemNumber = GetPlotItemNumber(_layer, _PlotItem);

				if (null != scatterPoint)
				{
					this._PlotIndex = scatterPoint.PlotIndex;
					this._RowIndex = scatterPoint.RowIndex;
					// convert this layer coordinates first to PrintableAreaCoordinates
					var rootLayerCoord = clickedObject.ParentLayer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates);
					_positionOfCrossInRootLayerCoordinates = rootLayerCoord;
					// m_Cross.X -= _grac.GraphViewOffset.X;
					// m_Cross.Y -= _grac.GraphViewOffset.Y;

					var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(rootLayerCoord);

					// TODO (Wpf)
					//var newCursorPosition = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
					//SetCursorPos(newCursorPosition.X, newCursorPosition.Y);

					this.DisplayData(_PlotItem, scatterPoint.RowIndex,
						_PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
						_PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);

					// here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
					// of the grac
					_grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
				}
			}
		} // end of function

		private void ShowCross(XYScatterPointInformation scatterPoint)
		{
			this._PlotIndex = scatterPoint.PlotIndex;
			this._RowIndex = scatterPoint.RowIndex;
			// convert this layer coordinates first to PrintableAreaCoordinates
			var rootLayerCoord = _layer.TransformCoordinatesFromHereToRoot(scatterPoint.LayerCoordinates);
			_positionOfCrossInRootLayerCoordinates = rootLayerCoord;
			// m_Cross.X -= _grac.GraphViewOffset.X;
			// m_Cross.Y -= _grac.GraphViewOffset.Y;

			var newPixelCoord = _grac.ConvertGraphToMouseCoordinates(rootLayerCoord);
			//Cursor.Position = new Point((int)(Cursor.Position.X + newPixelCoord.X - mouseXY.X),(int)(Cursor.Position.Y + newPixelCoord.Y - mouseXY.Y));
			//Cursor.Position = ((Control)_grac.View).PointToScreen(newPixelCoord);

			this.DisplayData(_PlotItem, scatterPoint.RowIndex,
				_PlotItem.XYColumnPlotData.XColumn[scatterPoint.RowIndex],
				_PlotItem.XYColumnPlotData.YColumn[scatterPoint.RowIndex]);

			// here we shoud switch the bitmap cache mode on and link us with the AfterPaint event
			// of the grac
			_grac.RepaintGraphAreaImmediatlyIfCachedBitmapValidElseOffline(); // no refresh necessary, only invalidate to show the cross
		}

		private void DisplayData(object plotItem, int rowIndex, AltaxoVariant x, AltaxoVariant y)
		{
			Current.DataDisplay.WriteOneLine(string.Format(
				"{0}: {1}[{2}] X={3}, Y={4}",
				_layer.Name,
				plotItem.ToString(),
				rowIndex,
				x,
				y));
		}

		/// <summary>
		/// Tests presumtions for a move of the cross.
		/// </summary>
		/// <returns>True if the cross can be moved, false if one of the presumtions does not hold.</returns>
		private bool TestMovementPresumtions()
		{
			if (_PlotItem == null)
				return false;
			if (_grac == null || _grac.Doc == null || _grac.Doc.RootLayer == null)
				return false;
			if (null == this._layer)
				return false;

			return true;
		}

		/// <summary>
		/// Moves the cross along the plot.
		/// </summary>
		/// <param name="increment"></param>
		private void MoveLeftRight(int increment)
		{
			if (!TestMovementPresumtions())
				return;

			XYScatterPointInformation scatterPoint = _PlotItem.GetNextPlotPoint(_layer, this._PlotIndex, increment);

			if (null != scatterPoint)
				ShowCross(scatterPoint);
		}

		/// <summary>
		/// Moves the cross to the next plot item. If no plot item is found in this layer, it moves the cross to the next layer.
		/// </summary>
		/// <param name="increment"></param>
		private void MoveUpDown(int increment)
		{
			if (!TestMovementPresumtions())
				return;

			var layerList = _layer.SiblingLayers;
			int numlayers = layerList.Count;
			var nextlayer = _layer as XYPlotLayer;
			int indexOfNextLayer = layerList.IndexOf(_layer);
			int nextplotitemnumber = this._PlotItemNumber;

			XYScatterPointInformation scatterPoint = null;
			XYColumnPlotItem plotitem = null;
			do
			{
				nextplotitemnumber = nextplotitemnumber + Math.Sign(increment);
				if (nextplotitemnumber < 0) // then try to use the previous layer
				{
					--indexOfNextLayer;
					nextlayer = indexOfNextLayer >= 0 ? layerList[indexOfNextLayer] as XYPlotLayer : null;
					nextplotitemnumber = nextlayer == null ? int.MaxValue : nextlayer.PlotItems.Flattened.Length - 1;
				}
				else if (nextplotitemnumber >= nextlayer.PlotItems.Flattened.Length)
				{
					++indexOfNextLayer;
					nextlayer = indexOfNextLayer < layerList.Count ? layerList[indexOfNextLayer] as XYPlotLayer : null;
					nextplotitemnumber = 0;
				}
				// check if this results in a valid information
				if (indexOfNextLayer < 0 || indexOfNextLayer >= numlayers)
					break; // no more layers available

				if (nextlayer == null)
					continue; // this is not an XYPlotLayer

				if (nextplotitemnumber < 0 || nextplotitemnumber >= nextlayer.PlotItems.Flattened.Length)
					continue;

				plotitem = nextlayer.PlotItems.Flattened[nextplotitemnumber] as XYColumnPlotItem;
				if (null == plotitem)
					continue;

				scatterPoint = plotitem.GetNextPlotPoint(nextlayer, this._PlotIndex, 0);
			} while (scatterPoint == null);

			if (null != scatterPoint)
			{
				this._PlotItem = plotitem;
				this._layer = nextlayer;
				this._PlotItemNumber = nextplotitemnumber;
				this._PlotIndex = scatterPoint.PlotIndex;
				this._RowIndex = scatterPoint.RowIndex;

				ShowCross(scatterPoint);
			}
		}

		public override void AfterPaint(Graphics g)
		{
			// draw a red cross onto the selected data point
			double startLine = 1 / _grac.ZoomFactor;
			double endLine = 10 / _grac.ZoomFactor;
			using (HatchBrush brush = new HatchBrush(HatchStyle.Percent50, Color.Red, Color.Yellow))
			{
				using (Pen pen = new Pen(brush, (float)(2 / _grac.ZoomFactor)))
				{
					g.DrawLine(pen, (float)(_positionOfCrossInRootLayerCoordinates.X + startLine), (float)_positionOfCrossInRootLayerCoordinates.Y, (float)(_positionOfCrossInRootLayerCoordinates.X + endLine), (float)_positionOfCrossInRootLayerCoordinates.Y);
					g.DrawLine(pen, (float)(_positionOfCrossInRootLayerCoordinates.X - startLine), (float)_positionOfCrossInRootLayerCoordinates.Y, (float)(_positionOfCrossInRootLayerCoordinates.X - endLine), (float)_positionOfCrossInRootLayerCoordinates.Y);
					g.DrawLine(pen, (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y + startLine), (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y + endLine));
					g.DrawLine(pen, (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y - startLine), (float)_positionOfCrossInRootLayerCoordinates.X, (float)(_positionOfCrossInRootLayerCoordinates.Y - endLine));
				}
			}
			base.AfterPaint(g);
		}

		/// <summary>
		/// This function is called if a key is pressed.
		/// </summary>
		/// <param name="e">Key event arguments.</param>
		/// <returns></returns>
		public override bool ProcessCmdKey(KeyEventArgs e)
		{
			var keyData = e.Key;
			if (keyData == Key.Left)
			{
				System.Diagnostics.Trace.WriteLine("Read tool key handler, left key pressed!");
				MoveLeftRight(-1);
				return true;
			}
			else if (keyData == Key.Right)
			{
				System.Diagnostics.Trace.WriteLine("Read tool key handler, right key pressed!");
				MoveLeftRight(1);
				return true;
			}
			else if (keyData == Key.Up)
			{
				MoveUpDown(1);
				return true;
			}
			else if (keyData == Key.Down)
			{
				MoveUpDown(-1);
				return true;
			}
			else if (keyData == Key.Enter)
			{
				Current.Console.WriteLine("{0}", this._RowIndex);
				return true;
			}

			return false; // per default the key is not processed
		}

		/// <summary>
		/// Find the plot item number of a given plot item.
		/// </summary>
		/// <param name="layer">The layer in which this plot item resides.</param>
		/// <param name="plotitem">The plot item for which the number should be retrieved.</param>
		/// <returns></returns>
		private int GetPlotItemNumber(XYPlotLayer layer, XYColumnPlotItem plotitem)
		{
			if (null != layer)
			{
				for (int i = 0; i < layer.PlotItems.Flattened.Length; i++)
					if (object.ReferenceEquals(layer.PlotItems.Flattened[i], plotitem))
						return i;
			}
			return -1;
		}
	} // end of class
}