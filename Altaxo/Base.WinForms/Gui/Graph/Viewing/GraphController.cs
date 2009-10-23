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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Serialization;
using Altaxo.Graph.GUI.GraphControllerMouseHandlers;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph;

using Altaxo.Gui.Graph.Viewing;

namespace Altaxo.Graph.GUI
{
	/// <summary>
	/// GraphController is our default implementation to control a graph view.
	/// </summary>
	public class WinFormsGraphController
	{

		#region Member variables

		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float UnitPerInch = 72;

		private static IList<IHitTestObject> _emptyReadOnlyList;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected GraphView _view;

		/// <summary>
		/// Color for the area of the view, where there is no page.
		/// </summary>
		protected Color _nonPageAreaColor;

		/// <summary>
		/// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
		/// this brush fills only the non printable margins of the page. 
		/// </summary>
		protected BrushX _pageGroundBrush;

		/// <summary>
		/// Brush to fill the printable area of the graph.
		/// </summary>
		protected BrushX _graphAreaBrush;

		/// <summary>Current horizontal resolution of the paint method.</summary>
		protected float _horizontalResolution;

		/// <summary>Current vertical resolution of the paint method.</summary>
		protected float _verticalResolution;

		/// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
		protected MouseStateHandler _mouseState;

		

		/// <summary>
		/// This holds a frozen image of the graph during the moving time
		/// </summary>
		protected Bitmap _cachedGraphImage;

		protected bool _isCachedGraphImageDirty;

		#endregion Member variables

		#region Constructors


		/// <summary>
		/// Set the member variables to default values. Intended only for use in constructors and deserialization code.
		/// </summary>
		protected virtual void SetMemberVariablesToDefault()
		{
			_nonPageAreaColor = Color.Gray;

			_pageGroundBrush = new BrushX(Color.LightGray);

			_graphAreaBrush = new BrushX(Color.Snow);

			_horizontalResolution = 300;

			_verticalResolution = 300;

			// A instance of a mouse handler class that currently handles the mouse events..</summary>
			_mouseState = new ObjectPointerMouseHandler(this._view);



			// This holds a frozen image of the graph during the moving time
			_cachedGraphImage = null;
		}


		public WinFormsGraphController(GraphView view)
		{
			_view = view;
			SetMemberVariablesToDefault();
		}

		static WinFormsGraphController()
		{
			_emptyReadOnlyList = new List<IHitTestObject>().AsReadOnly();

			// register here editor methods
			LayerController.RegisterEditHandlers();
			XYPlotLayer.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
			TextGraphic.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
			TextGraphic.TextGraphicsEditorMethod = new DoubleClickHandler(EhEditTextGraphics);
		}

		#endregion // Constructors

		#region IGraphController interface definitions

		/// <summary>
		/// This returns the GraphDocument that is managed by this controller.
		/// </summary>
		public GraphDocument Doc
		{
			get { return _view.Doc; }
		}
		/// <summary>
		/// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
		/// </summary>
		public XYPlotLayerCollection Layers
		{
			get { return _view.Doc.Layers; }
		}

		public XYPlotLayer ActiveLayer
		{
			get { return _view.ActiveLayer; }
		}

		public void SetActiveLayer(int layerNumber)
		{
			_view.SetActiveLayerFromInternal(layerNumber);
		}

		public GraphToolType GraphTool
		{
			get
			{
				return null == _mouseState ? GraphToolType.None : _mouseState.GraphToolType;
			}
			set
			{
				switch (value)
				{
					case GraphToolType.None:
						_mouseState = null;
						break;
					case GraphToolType.ArrowLineDrawing:
						_mouseState = new GraphControllerMouseHandlers.ArrowLineDrawingMouseHandler(_view);
						break;
					case GraphToolType.CurlyBraceDrawing:
						_mouseState = new GraphControllerMouseHandlers.CurlyBraceDrawingMouseHandler(_view);
						break;
					case GraphToolType.EllipseDrawing:
						_mouseState = new GraphControllerMouseHandlers.EllipseDrawingMouseHandler(_view);
						break;
					case GraphToolType.ObjectPointer:
						_mouseState = new GraphControllerMouseHandlers.ObjectPointerMouseHandler(_view);
						break;
					case GraphToolType.ReadPlotItemData:
						_mouseState = new GraphControllerMouseHandlers.ReadPlotItemDataMouseHandler(_view);
						break;
					case GraphToolType.ReadXYCoordinates:
						_mouseState = new GraphControllerMouseHandlers.ReadXYCoordinatesMouseHandler(_view);
						break;
					case GraphToolType.RectangleDrawing:
						_mouseState = new GraphControllerMouseHandlers.RectangleDrawingMouseHandler(_view);
						break;
					case GraphToolType.SingleLineDrawing:
						_mouseState = new GraphControllerMouseHandlers.SingleLineDrawingMouseHandler(_view);
						break;
					case GraphToolType.TextDrawing:
						_mouseState = new GraphControllerMouseHandlers.TextToolMouseHandler(_view);
						break;
					case GraphToolType.ZoomAxes:
						_mouseState = new GraphControllerMouseHandlers.ZoomAxesMouseHandler(_view);
						break;
					default:
						throw new NotImplementedException("Type not implemented: " + value.ToString());
				}
			}
		}


		private double ZoomFactor
		{
			get
			{
				return _view.GC.ZoomFactor;
			}
		}

		private PointF GraphViewOffset
		{
			get
			{
				return _view.GC.GraphViewOffset;
			}
		}





		/// <summary>
		/// Called if a key is pressed in the view.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		public bool EhView_ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this._mouseState != null)
				return this._mouseState.ProcessCmdKey(ref msg, keyData);
			else
				return false;
		}


		/// <summary>
		/// Called if the host window is about to be closed.
		/// </summary>
		/// <returns>True if the closing should be canceled, false otherwise.</returns>
		public bool HostWindowClosing()
		{
			if (!Current.ApplicationIsClosing)
			{

				if (false == Current.Gui.YesNoMessageBox("Do you really want to close this graph?", "Attention", false))
				{
					return true; // cancel the closing
				}
			}
			return false;
		}

		/*
    /// <summary>
    /// Called by the host window after the host window was closed.
    /// </summary>
    public void HostWindowClosed()
    {
      Current.ProjectService.RemoveGraph(this);
    }
		*/





		/// <summary>
		/// Handles the mouse up event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			_mouseState.OnMouseUp(e);
		}

		/// <summary>
		/// Handles the mouse down event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			_mouseState.OnMouseDown(e);
		}

		/// <summary>
		/// Handles the mouse move event onto the graph in the controller class.
		/// </summary>
		/// <param name="e">MouseEventArgs.</param>
		public virtual void EhView_GraphPanelMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			_mouseState.OnMouseMove(e);
		}

		/// <summary>
		/// Handles the click onto the graph event in the controller class.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelMouseClick(System.EventArgs e)
		{
			_mouseState.OnClick(e);
		}

		/// <summary>
		/// Handles the double click onto the graph event in the controller class.
		/// </summary>
		/// <param name="e"></param>
		public virtual void EhView_GraphPanelMouseDoubleClick(System.EventArgs e)
		{
			_mouseState.OnDoubleClick(e);
		}

		/// <summary>
		/// Handles the paint event of that area, where the graph is shown.
		/// </summary>
		/// <param name="e">The paint event args.</param>
		public virtual void EhView_GraphPanelPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (!e.ClipRectangle.IsEmpty)
				this.DoPaint(e.Graphics, false);
		}

		#endregion // IGraphView interface definitions


		#region Other event handlers




		/// <summary>
		/// This is called if the host window is selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void EhParentWindowSelected(object sender, EventArgs e)
		{
			if (_view != null)
				_view.OnViewSelection();
		}

		/// <summary>
		/// This is called if the host window is deselected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void EhParentWindowDeselected(object sender, EventArgs e)
		{
			if (_view != null)
				_view.OnViewDeselection();
		}


		/// <summary>
		/// Handles the double click event onto a plot item.
		/// </summary>
		/// <param name="hit">Object containing information about the double clicked object.</param>
		/// <returns>True if the object should be deleted, false otherwise.</returns>
		protected static bool EhEditPlotItem(IHitTestObject hit)
		{
			XYPlotLayer actLayer = hit.ParentLayer;
			IGPlotItem pa = (IGPlotItem)hit.HittedObject;


			// get plot group
			PlotGroupStyleCollection plotGroup = pa.ParentCollection.GroupStyles;

			Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()), true);

			return false;
		}

		/// <summary>
		/// Handles the double click event onto a plot item.
		/// </summary>
		/// <param name="hit">Object containing information about the double clicked object.</param>
		/// <returns>True if the object should be deleted, false otherwise.</returns>
		protected static bool EhEditTextGraphics(IHitTestObject hit)
		{
			XYPlotLayer layer = hit.ParentLayer;
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
					if (layer.ParentLayerList != null)
						layer.ParentLayerList.EhChildChanged(layer, EventArgs.Empty);
				}
			}



			return shouldDeleted;
		}

		#endregion

		#region Methods






		private void DoPaint(Graphics g, bool bForPrinting)
		{
			if (bForPrinting)
			{
				DoPaintUnbuffered(g, bForPrinting);
			}
			else
			{

				if (_cachedGraphImage == null || _cachedGraphImage.Width != _view.GraphSize.Width || _cachedGraphImage.Height != _view.GraphSize.Height)
				{
					if (_cachedGraphImage != null)
					{
						_cachedGraphImage.Dispose();
						_cachedGraphImage = null;
					}

					// create a frozen bitmap of the graph
					// using(Graphics g = m_View.CreateGraphGraphics())

					_cachedGraphImage = new Bitmap(_view.GraphSize.Width, _view.GraphSize.Height, g);
					_isCachedGraphImageDirty = true;
				}

				if (_cachedGraphImage == null)
				{
					DoPaintUnbuffered(g, bForPrinting);
				}
				else if (_isCachedGraphImageDirty)
				{
					using (Graphics gbmp = Graphics.FromImage(_cachedGraphImage))
					{
						DoPaintUnbuffered(gbmp, false);
						_isCachedGraphImageDirty = false;
					}

					g.DrawImageUnscaled(_cachedGraphImage, 0, 0, _view.GraphSize.Width, _view.GraphSize.Height);
					ScaleForPaint(g, bForPrinting);
				}
				else
				{
					g.DrawImageUnscaled(_cachedGraphImage, 0, 0, _view.GraphSize.Width, _view.GraphSize.Height);
					ScaleForPaint(g, bForPrinting); // to be in the same state as when drawing unbuffered
				}

				// special painting depending on current selected tool
				g.TranslateTransform(-GraphViewOffset.X, -GraphViewOffset.Y);
				this._mouseState.AfterPaint(g);
			}
		}

		/// <summary>
		/// This functions scales the graphics context to be ready for painting.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="bForPrinting">Indicates if the contexts is to be scaled
		/// for printing purposed (true) or for painting to the screen (false).</param>
		private void ScaleForPaint(Graphics g, bool bForPrinting)
		{
			// g.SmoothingMode = SmoothingMode.AntiAlias;
			// get the dpi settings of the graphics context,
			// for example; 96dpi on screen, 600dpi for the printer
			// used to adjust grid and margin sizing.
			this._horizontalResolution = g.DpiX;
			this._verticalResolution = g.DpiY;

			g.PageUnit = GraphicsUnit.Point;

			if (bForPrinting)
			{
				g.PageScale = 1;
			}
			else
			{
				g.PageScale = (float)this.ZoomFactor;
				float pointsh = (float)(UnitPerInch * _view.GraphScrollPosition.X / (this._horizontalResolution * this.ZoomFactor));
				float pointsv = (float)(UnitPerInch * _view.GraphScrollPosition.Y / (this._verticalResolution * this.ZoomFactor));
				g.TranslateTransform(pointsh, pointsv);
			}
		}

		/// <summary>
		/// Central routine for painting the graph. The painting can either be on the screen (bForPrinting=false), or
		/// on a printer or file (bForPrinting=true).
		/// </summary>
		/// <param name="g">The graphics context painting to.</param>
		/// <param name="bForPrinting">If true, margins and background are not painted, as is usefull for printing.
		/// Also, if true, the scale is temporarely set to 1.</param>
		private void DoPaintUnbuffered(Graphics g, bool bForPrinting)
		{
			try
			{
				ScaleForPaint(g, bForPrinting);

				if (!bForPrinting)
				{
					g.Clear(this._nonPageAreaColor);
					// Fill the page with its own color
					//g.FillRectangle(_pageGroundBrush,_doc.PageBounds);
					//g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
					g.FillRectangle(_graphAreaBrush, -GraphViewOffset.X, -GraphViewOffset.Y, Doc.Layers.GraphSize.Width, Doc.Layers.GraphSize.Height);
					// DrawMargins(g);
				}

				// Paint the graph now
				//g.TranslateTransform(m_Graph.PrintableBounds.X,m_Graph.PrintableBounds.Y); // translate the painting to the printable area
				g.TranslateTransform(-GraphViewOffset.X, -GraphViewOffset.Y);
				Doc.DoPaint(g, bForPrinting);




			}
			catch (System.Exception ex)
			{
				g.PageUnit = GraphicsUnit.Point;
				g.PageScale = 1;

				g.DrawString(ex.ToString(),
					new System.Drawing.Font("Arial", 10),
					System.Drawing.Brushes.Black,
					Doc.PrintableBounds);


			}

		}


		#endregion // Methods


		#region Editing selected objects

		public IList<IHitTestObject> SelectedObjects
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
		/// Returns the number of selected objects into this graph.
		/// </summary>
		public int NumberOfSelectedObjects
		{
			get
			{
				if (_mouseState is ObjectPointerMouseHandler)
					return ((ObjectPointerMouseHandler)_mouseState).NumberOfSelectedObjects;
				else
					return 0;
			}
		}















		#endregion


		#region Scaling and Positioning





		/// <summary>
		/// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
		/// </summary>
		public void RefreshGraph()
		{
			this._isCachedGraphImageDirty = true;
				_view.InvalidateGraph();
		}

		/// <summary>
		/// If the graph is cached, this causes an immediate redraw of the client area using the cached bitmap.
		/// If not cached, this simply invalidates the client area.
		/// </summary>
		public void RepaintGraphArea()
		{
			if (_view == null)
				return;

			if (this._cachedGraphImage != null && !this._isCachedGraphImageDirty)
			{
				using (Graphics g = this._view.CreateGraphGraphics())
				{
					this.DoPaint(g, false);
				}
			}
			else
			{
				_view.InvalidateGraph();
			}

		}





		/// <summary>
		/// Factor for horizontal conversion of page units (points=1/72 inch) to pixel.
		/// The resolution used for this is <see cref="m_HorizRes"/>.
		/// </summary>
		/// <returns>The factor described above.</returns>
		public float HorizFactorPageToPixel()
		{
			return (float)(this._horizontalResolution * this.ZoomFactor / UnitPerInch);
		}

		/// <summary>
		/// Factor for vertical conversion of page units (points=1/72 inch) to pixel.
		/// The resolution used for this is <see cref="m_VertRes"/>.
		/// </summary>
		/// <returns>The factor described above.</returns>
		public float VertFactorPageToPixel()
		{
			return (float)(this._verticalResolution * this.ZoomFactor / UnitPerInch);
		}

		/// <summary>
		/// Converts page coordinates (in points=1/72 inch) to pixel units. Uses the resolutions <see cref="m_HorizRes"/>
		/// and <see cref="m_VertRes"/> for calculation-
		/// </summary>
		/// <param name="pagec">The page coordinates to convert.</param>
		/// <returns>The coordinates as pixel coordinates.</returns>
		public PointF PageCoordinatesToPixel(PointF pagec)
		{
			return new PointF(pagec.X * HorizFactorPageToPixel(), pagec.Y * VertFactorPageToPixel());
		}

		/// <summary>
		/// Converts pixel coordinates to page coordinates (in points=1/72 inch). Uses the resolutions <see cref="m_HorizRes"/>
		/// and <see cref="m_VertRes"/> for calculation-
		/// </summary>
		/// <param name="pixelc">The pixel coordinates to convert.</param>
		/// <returns>The coordinates as page coordinates (points=1/72 inch).</returns>
		public PointF PixelToPageCoordinates(PointF pixelc)
		{
			return new PointF(pixelc.X / HorizFactorPageToPixel(), pixelc.Y / VertFactorPageToPixel());
		}
		public SizeF PixelToPageCoordinates(SizeF pixelc)
		{
			return new SizeF(PixelToPageCoordinates(new PointF(pixelc.Width, pixelc.Height)));
		}

		/// <summary>
		/// Converts page coordinates (in points=1/72 inch) to pixel coordinates . Uses the resolutions <see cref="m_HorizRes"/>
		/// and <see cref="m_VertRes"/> for calculation-
		/// </summary>
		/// <param name="pagec">The page coordinates to convert (points=1/72 inch).</param>
		/// <returns>The coordinates as pixel coordinates.</returns>
		public PointF PageToPixelCoordinates(PointF pagec)
		{
			return new PointF(pagec.X * HorizFactorPageToPixel(), pagec.Y * VertFactorPageToPixel());
		}

		/// <summary>
		/// Converts x,y differences in pixels to the corresponding
		/// differences in page coordinates
		/// </summary>
		/// <param name="pixeldiff">X,Y differences in pixel units</param>
		/// <returns>X,Y differences in page coordinates</returns>
		public PointF PixelToPageDifferences(PointF pixeldiff)
		{
			return new PointF(pixeldiff.X / HorizFactorPageToPixel(), pixeldiff.Y / VertFactorPageToPixel());
		}

		/// <summary>
		/// converts from pixel to printable area coordinates
		/// </summary>
		/// <param name="pixelc">pixel coordinates as returned by MouseEvents</param>
		/// <returns>coordinates of the printable area in 1/72 inch</returns>
		public PointF PixelToPrintableAreaCoordinates(PointF pixelc)
		{
			PointF r = PixelToPageCoordinates(pixelc);
			r.X += GraphViewOffset.X;
			r.Y += GraphViewOffset.Y;
			return r;
		}

		/// <summary>
		/// converts printable area  to pixel coordinates
		/// </summary>
		/// <param name="printc">Printable area coordinates.</param>
		/// <returns>Pixel coordinates as returned by MouseEvents</returns>
		public PointF PrintableAreaToPixelCoordinates(PointF printc)
		{
			printc.X -= GraphViewOffset.X;
			printc.Y -= GraphViewOffset.Y;
			return PageToPixelCoordinates(printc);
		}




		#endregion // Scaling, Converting






		/// <summary>
		/// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
		/// </summary>
		/// <param name="pixelPos">The pixel coordinates (graph panel coordinates)</param>
		/// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
		/// <param name="foundObject">Found object if there is one found, else null</param>
		/// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
		/// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
		public bool FindGraphObjectAtPixelPosition(PointF pixelPos, bool plotItemsOnly, out IHitTestObject foundObject, out int foundInLayerNumber)
		{
			// search for a object first
			PointF mousePT = PixelToPrintableAreaCoordinates(pixelPos);

			for (int nLayer = 0; nLayer < Layers.Count; nLayer++)
			{
				XYPlotLayer layer = Layers[nLayer];
				foundObject = layer.HitTest(mousePT, plotItemsOnly);
				if (null != foundObject)
				{
					foundInLayerNumber = nLayer;
					return true;
				}
			}
			foundObject = null;
			foundInLayerNumber = 0;
			return false;
		}









	}
}
