#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Viewing
{
	using Altaxo.Collections;
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi;
	using Altaxo.Graph.Gdi.Shapes;
	using Altaxo.Main;
	using Altaxo.Serialization.Clipboard;

	[ExpectedTypeOfView(typeof(IGraphView))]
	public abstract class GraphController : IGraphController, IGraphViewEventSink, IDisposable
	{
		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const double PtPerInch = 72;

		/// <summary>Holds the Graph document (the place were the layers, plots, graph elements... are stored).</summary>
		protected GraphDocument _doc;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected IGraphView _view;

		/// <summary>Number of the currently selected layer (or null if no layer is present).</summary>
		protected IList<int> _currentLayerNumber = new List<int>();

		/// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
		protected int _currentPlotNumber = -1;

		/// <summary>Current zoom factor. If AutoZoom is on, this factor is calculated automatically.</summary>
		protected double _zoomFactor;

		/// <summary>If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
		protected bool _isAutoZoomActive = true; // if true, the sheet is zoomed as big as possible to fit into window

		/// <summary>
		/// Ratio of view port dimension to the dimension of the graph.
		/// Example: a values of 2 means that the view port size is two times the size of the graph.
		/// </summary>
		protected double _areaFillingFactor = 1.2;

		protected PointD2D _positionOfViewportsUpperLeftCornerInRootLayerCoordinates;

		private NGTreeNode _layerStructure;

		#region Constructors

		protected GraphController()
		{
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		protected GraphController(GraphDocument graphdoc)
		{
			if (null == graphdoc)
				throw new ArgumentNullException("Leaving the graphdoc null in constructor is not supported here");

			InternalInitializeGraphDocument(graphdoc); // Using DataTable here wires the event chain also
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0)
				return false;
			if (args[0] is GraphDocument)
			{
				InternalInitializeGraphDocument(args[0] as GraphDocument);
			}
			else if (args[0] is GraphViewLayout)
			{
				var o = (GraphViewLayout)args[0];
				_isAutoZoomActive = o.IsAutoZoomActive;
				_zoomFactor = o.ZoomFactor;
				_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = o.PositionOfViewportsUpperLeftCornerInRootLayerCoordinates;
				InternalInitializeGraphDocument(o.GraphDocument);
			}
			else
			{
				return false;
			}
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion Constructors

		protected void Initialize(bool initData)
		{
			if (initData)
			{
				InitLayerStructure();
			}

			if (null != _view)
			{
				InitLayerStructure();
				_view.SetLayerStructure(_layerStructure, this.CurrentLayerNumber.ToArray()); // tell the view how many layers we have

				// Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
				this.EhView_GraphPanelSizeChanged();
			}
		}

		private void InitLayerStructure()
		{
			_layerStructure = Altaxo.Collections.TreeNodeExtensions.ProjectTreeToNewTree(
				_doc.RootLayer,
				new List<int>(),
				(sn, indices) => new NGTreeNode { Tag = indices.ToArray(), Text = HostLayer.GetDefaultNameOfLayer(indices) }, (parent, child) => parent.Nodes.Add(child));
		}

		#region Properties

		/// <summary>
		/// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
		/// </summary>
		public HostLayer RootLayer
		{
			get { return _doc.RootLayer; }
		}

		#region Size

		/// <summary>Gets the size (in points = 1/72 inch) of the graph with margin (without zoom).</summary>
		/// <value>The size of the graph with margin arount it.</value>
		public PointD2D SizeOfGraphWithMargin
		{
			get
			{
				return new PointD2D(_zoomFactor * _doc.Size.X * _areaFillingFactor, _zoomFactor * _doc.Size.Y * _areaFillingFactor);
			}
		}

		/// <summary>Gets the size (in points = 1/72 inch) of the graph with margin with taking into account the current zoom factor. This can be much greater than the actual viewport size, if the zoom factor exceeds the auto zoom factor.</summary>
		/// <value>The size of the zoomed graph with padding.</value>
		public PointD2D SizeOfGraphWithMarginZoomed
		{
			get
			{
				return new PointD2D(_zoomFactor * _doc.Size.X * _areaFillingFactor, _zoomFactor * _doc.Size.Y * _areaFillingFactor);
			}
		}

		/// <summary>
		/// Size of the viewport window in points (1/72 inch). This is the physical size of the visible window in the graph panel.
		/// </summary>
		private PointD2D SizeOfViewport
		{
			get
			{
				return _view.ViewportSizeInPoints;
			}
		}

		/// <summary>
		/// Size of the viewport in graph coordinates, taking the current zoom value into account (differences of the positions of the lower right corner and upper left corner of the view port in graph coordinates).
		/// </summary>
		private PointD2D SizeOfViewportInGraphCoordinates
		{
			get
			{
				return _view.ViewportSizeInPoints / _zoomFactor;
			}
		}

		#endregion Size

		#region Position

		/// <summary>Gets or sets the position of the view port window's upper left corner in graph coordinates.</summary>
		/// <value>
		/// The position of view port window's upper left corner in graph coordinates.
		/// </value>
		public PointD2D PositionOfViewportsUpperLeftCornerInGraphCoordinates
		{
			get { return _positionOfViewportsUpperLeftCornerInRootLayerCoordinates; }
			set
			{
				var oldVal = _positionOfViewportsUpperLeftCornerInRootLayerCoordinates;
				_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = value;
				if (oldVal != value && null != _view)
				{
					SetViewsScrollbarParameter();
					_view.InvalidateCachedGraphBitmapAndRepaint();
				}
			}
		}

		/// <summary>
		/// Calculates the position of the upper left corner of the graph's margin in graph coordinates. This depends only from the margin set. Does not change any member variables.
		/// </summary>
		private PointD2D PositionOfMarginsUpperLeftCornerInGraphCoordinates
		{
			get
			{
				return new PointD2D(
					(-_doc.Size.X * (_areaFillingFactor - 1) / 2),
					(-_doc.Size.Y * (_areaFillingFactor - 1) / 2)
					);
			}
		}

		/// <summary>
		/// Position of the lower right corner of the graph's margin in graph coordinates. This depends only from the margin set.
		/// </summary>
		private PointD2D PositionOfMarginsLowerRightCornerInGraphCoordinates
		{
			get
			{
				return new PointD2D(
					(_doc.Size.X * (_areaFillingFactor + 1) / 2),
					(_doc.Size.Y * (_areaFillingFactor + 1) / 2)
					);
			}
		}

		#endregion Position

		#region Margin

		/// <summary>Gets or sets the margin. A value of 0 indicates that if autozoom is active, there is no margin around the graph.
		/// A value of 1 means that there is a right and left margin of 100 percent of the graph width, and a top and bottom
		/// margin of 100 percent of the graph heigth.</summary>
		/// <value>The margin value. Must be a non-negative value, otherwise a <see cref="ArgumentOutOfRangeException"/> is thrown.</value>
		public double Margin
		{
			get
			{
				return (_areaFillingFactor - 1) / 2;
			}
			set
			{
				if (!(value >= 0))
					throw new ArgumentOutOfRangeException("Margin is not greater than or equal to zero");

				var oldValue = _areaFillingFactor;
				_areaFillingFactor = 1 + 2 * value;
				if (_areaFillingFactor != oldValue)
				{
					if (_isAutoZoomActive)
						RefreshAutoZoom(true);
					else
						RefreshManualZoom();
				}
			}
		}

		#endregion Margin

		#region Zoom

		/// <summary>Gets the current zoom factor that would be used for auto zoom, but does not set it. Does not change any member variables.</summary>
		/// <returns>The zoom factor that would be used for autozoom.</returns>
		public double AutoZoomFactor
		{
			get
			{
				var rlsize = _doc.Size;
				if (0 == _areaFillingFactor)
					throw new InvalidOperationException("AreaFillingFactor is 0, thus AutoZoomFactor can not be calculated");
				if (0 == rlsize.X || 0 == rlsize.Y)
					throw new InvalidOperationException("Root layer size x or y is 0, thus AutoZoomFactor can not be calculated");

				double zoomh = (_view.ViewportSizeInPoints.X) / (_doc.Size.X * _areaFillingFactor);
				double zoomv = (_view.ViewportSizeInPoints.Y) / (_doc.Size.Y * _areaFillingFactor);
				var zoomFactor = System.Math.Min(zoomh, zoomv);
				if (zoomFactor <= 0)
					zoomFactor = 1;

				return zoomFactor;
			}
		}

		/// <summary>
		/// Enables / disable the autozoom feature.
		/// </summary>
		/// <remarks>If autozoom is enables, the zoom factor is calculated depending on the size of the
		/// graph view so that the graph fits best possible inside the view.</remarks>
		public bool IsAutoZoomActive
		{
			get
			{
				return this._isAutoZoomActive;
			}
			set
			{
				this._isAutoZoomActive = value;
				if (this._isAutoZoomActive)
				{
					RefreshAutoZoom(true);
				}
				else
				{
					RefreshManualZoom();
				}
			}
		}

		/// <summary>
		/// Zoom value of the graph view. If this property is set, AutoZoom will be set to false.
		/// </summary>
		public double ZoomFactor
		{
			get
			{
				return _zoomFactor;
			}
			set
			{
				_isAutoZoomActive = false;
				var oldValue = _zoomFactor;

				if (value > 0 && value < double.MaxValue)
				{
					if (!(value >= 0.0009765625))
						value = 0.0009765625;
					else if (!(value <= 1024))
						value = 1024;
					_zoomFactor = value;
				}
				else
				{
					_zoomFactor = 1;
				}

				if (!(oldValue == _zoomFactor))
					RefreshManualZoom();
			}
		}

		/// <summary>Zooms around a pivot point. The pivot point is the point in graph coordinates that does not change the location in the viewport window when zooming.</summary>
		/// <param name="newZoomValue">The new zoom value.</param>
		/// <param name="graphCoordinate">The location of the pivot point in graph coordinates (points = 1/72 inch).</param>
		public void ZoomAroundPivotPoint(double newZoomValue, PointD2D graphCoordinate)
		{
			var oldViewportCoord = (graphCoordinate - PositionOfViewportsUpperLeftCornerInGraphCoordinates) * _zoomFactor;
			_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = graphCoordinate - oldViewportCoord / newZoomValue;
			var newPos = (graphCoordinate - PositionOfViewportsUpperLeftCornerInGraphCoordinates) * newZoomValue;
			ZoomFactor = newZoomValue;
		}

		/// <summary>
		/// Recalculates and sets the value of m_Zoom so the whole page is visible
		/// </summary>
		/// <param name="triggerRepaintIfChanged">If true, and either the zoom factor or the view port offset has changed, a refresh of the graph is triggered.</param>
		protected void RefreshAutoZoom(bool triggerRepaintIfChanged)
		{
			var hasChanged = CalculateAutoZoom();
			_view.ShowGraphScrollBars = false;

			if (hasChanged && triggerRepaintIfChanged)
				RefreshGraph();
		}

		/// <summary>
		/// Calculates the automatic zoom factor and the position of the view ports upper left corner (in Auto zoom mode).
		/// </summary>
		/// <returns>True if either the zoom factor or the view port origin has changed; otherweise <c>false</c>.</returns>
		private bool CalculateAutoZoom()
		{
			var oldZoomFactor = _zoomFactor;
			_zoomFactor = AutoZoomFactor;

			var oldPositionOfViewportsUpperLeftCornerInRootLayerCoordinates = _positionOfViewportsUpperLeftCornerInRootLayerCoordinates;
			_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = PositionOfMarginsUpperLeftCornerInGraphCoordinates;

			return oldZoomFactor != _zoomFactor || oldPositionOfViewportsUpperLeftCornerInRootLayerCoordinates != _positionOfViewportsUpperLeftCornerInRootLayerCoordinates;
		}

		protected void RefreshManualZoom()
		{
			var virtualSize = SizeOfViewportInGraphCoordinates;
			var xratio = virtualSize.X / _doc.Size.X;
			var yratio = virtualSize.Y / _doc.Size.Y;

			bool showScrollbars = xratio < 1 || yratio < 1;
			if (showScrollbars)
			{
			}
			else
			{
				// we center the graph in the viewport
				var gz = _doc.Size;
				var vz = SizeOfViewportInGraphCoordinates;
				_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = new PointD2D((gz.X - vz.X) / 2, (gz.Y - vz.Y) / 2);
			}

			if (null != _view)
			{
				SetViewsScrollbarParameter();
				_view.InvalidateCachedGraphBitmapAndRepaint();
			}
		}

		#endregion Zoom

		#region Scrolling

		/// <summary>Converts the scrollbar values to the corresponding graph coordinate.</summary>
		/// <param name="scrollbarValue">The scrollbar value.</param>
		/// <returns>The graph coordinate corresponding to the upper left corner of the viewport window.</returns>
		/// <remarks>
		/// <para>The scroll bars are set as following:</para>
		/// <list type="Bullet">
		/// <item><description>The viewport value of the scrollbar is set to the physical viewport size of the window (all units in points = 1/72 inch)</description></item>
		/// <item><description>The minimum value of the scrollbar is set to 0. This corresponds to the graph coordinate: PositionOfMarginsUpperLeftCornerInGraphCoordinates</description></item>
		/// <item><description>The maximum value of the scrollbar is set to SizeOfZoomedGraphWithMargin</description></item>
		/// <item><description>The value of the scrollbar in dependence on the visible upper left corner of the graph in graph coordinates is then calculated as:
		///                                    ScrollBarValue = (GraphCoordinate - PositionOfMarginsUpperLeftCornerInGraphCoordinates)*zoom</description></item>
		/// <item><description>or vice versa:  GraphCoordinate = ScrollBarValue/zoom + PositionOfMarginsUpperLeftCornerInGraphCoordinates;</description></item>
		/// </list>
		/// </remarks>
		public PointD2D ConvertScrollbarValueToGraphCoordinate(PointD2D scrollbarValue)
		{
			return scrollbarValue / _zoomFactor + PositionOfMarginsUpperLeftCornerInGraphCoordinates;
		}

		/// <summary>Converts a graph coordinate to scrollbar values. See the remarks in <see cref="ConvertScrollbarValueToGraphCoordinate"/> to learn how the scroll bar parameters are set.</summary>
		/// <param name="graphCoordinate">The graph coordinate that corresponds to the upper left corner of the view port window.</param>
		/// <returns>The scroll bar values that should be set.</returns>
		public PointD2D ConvertGraphCoordinateToScrollbarValue(PointD2D graphCoordinate)
		{
			return (graphCoordinate - PositionOfMarginsUpperLeftCornerInGraphCoordinates) * _zoomFactor;
		}

		/// <summary>Sets the views scrollbar parameter according to the current settings for zoom, offset, and viewport size.</summary>
		private void SetViewsScrollbarParameter()
		{
			if (_isAutoZoomActive || _zoomFactor < AutoZoomFactor)
			{
				_view.SetHorizontalScrollbarParameter(false, 0, 1, 1000000, 0, 0);
				_view.SetVerticalScrollbarParameter(false, 0, 1, 1000000, 0, 0);
			}
			else
			{
				var scrollMaxima = SizeOfGraphWithMarginZoomed;
				var scrollValues = ConvertGraphCoordinateToScrollbarValue(PositionOfViewportsUpperLeftCornerInGraphCoordinates);
				var portSize = SizeOfViewport;
				_view.SetHorizontalScrollbarParameter(true, scrollValues.X, scrollMaxima.X, portSize.X, portSize.X / 2, portSize.X / 25);
				_view.SetVerticalScrollbarParameter(true, scrollValues.Y, scrollMaxima.Y, portSize.Y, portSize.Y / 2, portSize.Y / 25);
			}
		}

		#endregion Scrolling

		#region Graph tools

		/// <summary>
		/// This event will be fired if the current graph tool has changed, either by the user
		/// or by the program.
		/// </summary>
		public event EventHandler CurrentGraphToolChanged;

		public abstract GraphToolType CurrentGraphTool { get; set; }

		#endregion Graph tools

		#endregion Properties

		#region IGraphController Members

		private void InternalInitializeGraphDocument(GraphDocument doc)
		{
			if (_doc != null)
				throw new ApplicationException("_doc is already initialized");
			if (doc == null)
				throw new ArgumentNullException("doc");

			_doc = doc;

			// we are using weak events here, to avoid that _doc will maintain strong references to the controller
			_doc.Changed += new WeakEventHandler(this.EhGraph_Changed, x => _doc.Changed -= x);
			_doc.RootLayer.LayerCollectionChanged += new WeakEventHandler(this.EhGraph_LayerCollectionChanged, x => _doc.RootLayer.LayerCollectionChanged -= x);
			_doc.SizeChanged += new WeakEventHandler(this.EhGraph_SizeChanged, x => _doc.SizeChanged -= x);

			// if the host layer has at least one child, we set the active layer to the first child of the host layer
			if (_doc.RootLayer.Layers.Count >= 1)
				_currentLayerNumber = new List<int>() { 0 };

			// Ensure the current layer and plot numbers are valid
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();

			OnTitleNameChanged(EventArgs.Empty);
		}

		public Altaxo.Graph.Gdi.GraphDocument Doc
		{
			get { return _doc; }
		}

		public object ModelObject
		{
			get
			{
				return new GraphViewLayout(_isAutoZoomActive, _zoomFactor, _doc, _positionOfViewportsUpperLeftCornerInRootLayerCoordinates);
			}
		}

		/// <summary>
		/// Returns the currently active layer. There is always an active layer.
		/// </summary>
		public HostLayer ActiveLayer
		{
			get
			{
				return _doc.RootLayer.ElementAt(this._currentLayerNumber);
			}
			set
			{
				CurrentLayerNumber = value.IndexOf();
			}
		}

		/// <summary>
		/// Get / sets the currently active layer by number.
		/// </summary>
		public IList<int> CurrentLayerNumber
		{
			get
			{
				EnsureValidityOfCurrentLayerNumber();
				return new List<int>(_currentLayerNumber);
			}
			set
			{
				// negative values are only accepted if there is no layer
				if (value == null)
					throw new ArgumentNullException("CurrentLayerNumber");

				if (!_doc.RootLayer.IsValidIndex(value))
					throw new ArgumentOutOfRangeException("CurrentLayerNumber", value, "The provided layer number was invalid");

				bool isDifferent = !System.Linq.Enumerable.SequenceEqual(value, _currentLayerNumber);

				_currentLayerNumber = new List<int>(value);

				// if something changed
				if (isDifferent)
				{
					// reflect the change in layer number in the layer tool bar
					if (_view != null)
						_view.CurrentLayer = this._currentLayerNumber.ToArray();
				}
			}
		}

		/// <summary>
		/// Get / sets the currently active plot by number.
		/// </summary>
		public int CurrentPlotNumber
		{
			get
			{
				return _currentPlotNumber;
			}
			set
			{
				var layer = ActiveLayer as XYPlotLayer;

				if (null != layer && 0 != layer.PlotItems.Flattened.Length && value < 0)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to be greater or equal than zero");

				if (null != layer && value >= layer.PlotItems.Flattened.Length)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to  be lesser than actual count: " + layer.PlotItems.Flattened.Length.ToString());

				_currentPlotNumber = value < 0 ? -1 : value;
			}
		}

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		public HostLayer EnsureValidityOfCurrentLayerNumber()
		{
			_doc.RootLayer.EnsureValidityOfNodeIndex(_currentLayerNumber);
			return _doc.RootLayer.ElementAt(_currentLayerNumber);
		}

		/// <summary>
		/// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
		/// the current plot number is set to -1.
		/// </summary>
		public void EnsureValidityOfCurrentPlotNumber()
		{
			var layer = EnsureValidityOfCurrentLayerNumber() as XYPlotLayer;

			// if XYPlotLayer don't exist anymore, correct CurrentLayerNumber and ActualPlotAssocitation
			if (null != layer) // if the ActiveLayer exists
			{
				// if the XYColumnPlotData don't exist anymore, correct it
				if (layer.PlotItems.Flattened.Length > 0) // if at least one plotitem exists
				{
					if (_currentPlotNumber < 0)
						CurrentPlotNumber = 0;
					else if (_currentPlotNumber > layer.PlotItems.Flattened.Length)
						CurrentPlotNumber = 0;
				}
				else
				{
					if (-1 != _currentPlotNumber)
						CurrentPlotNumber = -1;
				}
			}
			else // if no layer anymore
			{
				if (-1 != _currentPlotNumber)
					CurrentPlotNumber = -1;
			}
		}

		public void RefreshGraph()
		{
			if (null != _view)
				_view.InvalidateCachedGraphBitmapAndRepaint();
		}

		public abstract object ViewObject { get; set; }

		#endregion IGraphController Members

		#region Event handlers from GraphDocument

		/// <summary>
		/// Called if something in the <see cref="GraphDocument"/> changed.
		/// </summary>
		/// <param name="sender">Not used (always the GraphDocument).</param>
		/// <param name="e">The EventArgs.</param>
		protected void EhGraph_Changed(object sender, System.EventArgs e)
		{
			var eAsNOC = e as Altaxo.Main.NamedObjectCollectionChangedEventArgs;
			if (null != eAsNOC && eAsNOC.WasItemRenamed)
			{
				Current.Gui.Execute(EhGraphDocumentNameChanged_Unsynchronized, (GraphDocument)sender, eAsNOC.OldName);
				return;
			}

			// if something changed on the graph, make sure that the layer and plot number reflect this changed
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();

			if (null != _view)
				_view.InvalidateCachedGraphBitmapAndRepaint(); // this function is non-Gui thread safe
		}

		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraph_SizeChanged(object sender, System.EventArgs e)
		{
			Current.Gui.BeginExecute(EhGraph_BoundsChanged_Unsynchronized);
		}

		protected void EhGraph_BoundsChanged_Unsynchronized()
		{
			if (_view != null)
			{
				if (this._isAutoZoomActive)
					this.RefreshAutoZoom(false);
				_view.InvalidateCachedGraphBitmapAndRepaint();
			}
		}

		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraph_LayerCollectionChanged(object sender, System.EventArgs e)
		{
			Current.Gui.BeginExecute(EhGraph_LayerCollectionChanged_Unsynchronized);
		}

		protected void EhGraph_LayerCollectionChanged_Unsynchronized()
		{
			var oldActiveLayer = new List<int>(this._currentLayerNumber);

			// Ensure that the current layer and current plot are valid anymore
			var newActiveLayer = EnsureValidityOfCurrentLayerNumber();

			// even if the active layer number not changed, it can be that the layer itself has changed from
			// one to another, so make sure that the current plot number is valid also
			EnsureValidityOfCurrentPlotNumber();

			// make sure the view knows about when the number of layers changed
			InitLayerStructure();
			if (_view != null)
			{
				_view.SetLayerStructure(_layerStructure, _currentLayerNumber.ToArray());
			}
		}

		private void EhGraphDocumentNameChanged_Unsynchronized(INameOwner sender, string oldName)
		{
			if (null != _view)
				_view.GraphViewTitle = Doc.Name;

			this.TitleName = Doc.Name;
		}

		/// <summary>
		/// This is the whole name of the content, e.g. the file name or
		/// the url depending on the type of the content.
		/// </summary>
		public string TitleName
		{
			get
			{
				return this.Doc.Name;
			}
			set
			{
				OnTitleNameChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Is called each time the name for the content has changed.
		/// </summary>
		public event EventHandler TitleNameChanged;

		protected virtual void OnTitleNameChanged(System.EventArgs e)
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this, e);
		}

		#endregion Event handlers from GraphDocument

		#region IGraphViewEventSink

		/// <summary>
		/// The controller should show a data context menu (contains all plots of the currentLayer).
		/// </summary>
		/// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
		/// <param name="parent">The parent control which is the parent of the context menu.</param>
		/// <param name="pt">The location where the context menu should be shown.</param>
		public virtual void EhView_ShowDataContextMenu(int[] currLayer, object parent, Point pt)
		{
			int oldCurrLayer = this.ActiveLayer.Number;

			this.CurrentLayerNumber = new List<int>(currLayer);

			if (null != this.ActiveLayer)
			{
				Current.Gui.ShowContextMenu(parent, parent, "/Altaxo/Views/Graph/LayerButton/ContextMenu", pt.X, pt.Y);
			}
		}

		/// <summary>
		/// This function is called if the user changed the GraphTool.
		/// </summary>
		/// <param name="currGraphToolType">The type of the new selected GraphTool.</param>
		public virtual void EhView_CurrentGraphToolChoosen(System.Type currGraphToolType)
		{
			// this.CurrentGraphToolType = currGraphToolType;
		}

		public virtual void EhView_CurrentGraphToolChanged()
		{
			if (null != CurrentGraphToolChanged)
				CurrentGraphToolChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Handles the selection of the current layer by the <b>user</b>.
		/// </summary>
		/// <param name="currLayer">The current layer number as selected by the user.</param>
		/// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
		public virtual void EhView_CurrentLayerChoosen(int[] currLayer, bool bAlternative)
		{
			var oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = new List<int>(currLayer);

			// if we have clicked the button already down then open the layer dialog
			if (null != ActiveLayer && System.Linq.Enumerable.SequenceEqual(_currentLayerNumber, oldCurrLayer) && false == bAlternative)
			{
				var activeLayer = ActiveLayer;
				if (activeLayer is XYPlotLayer)
					XYPlotLayerController.ShowDialog((XYPlotLayer)activeLayer);
				else
					HostLayerController.ShowDialog(activeLayer);
			}
		}

		/// <summary>
		/// Handles the event when the size of the graph area is changed.
		/// </summary>
		public virtual void EhView_GraphPanelSizeChanged()
		{
			if (_isAutoZoomActive)
			{
				RefreshAutoZoom(false);
			}
			else
			{
				if (null != _view && 0 != _view.ViewportSizeInPoints.X && 0 != _view.ViewportSizeInPoints.Y)
					SetViewsScrollbarParameter();
			}
		}

		public void EhView_Scroll()
		{
			_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = ConvertScrollbarValueToGraphCoordinate(_view.GraphScrollPosition);
			_view.InvalidateCachedGraphBitmapAndRepaint();
		}

		#endregion IGraphViewEventSink

		#region Arrange

		public abstract IList<IHitTestObject> SelectedObjects { get; }

		public delegate void ArrangeElement(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds);

		/// <summary>
		/// Arranges the objects so they share a common boundary.
		/// </summary>
		/// <param name="arrange">Routine that determines how to arrange the element with respect to the master element.</param>
		public void Arrange(ArrangeElement arrange)
		{
			if (SelectedObjects.Count < 2)
				return;

			RectangleF masterbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements.GetBounds();

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i >= 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				RectangleF bounds = o.ObjectOutlineForArrangements.GetBounds();

				arrange(o, bounds, masterbound);
			}

			_view.InvalidateCachedGraphBitmapAndRepaint(); // force a refresh
		}

		/// <summary>
		/// Arranges the objects so they share the top boundary with the top boundary of the master element.
		/// </summary>
		public void ArrangeTopToTop()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(0, masterbounds.Y - bounds.Y); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the bottom boundary with the top boundary of the master element.
		/// </summary>
		public void ArrangeBottomToTop()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(0, (masterbounds.Y) - (bounds.Y + bounds.Height)); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the top boundary with the bottom boundary of the master element.
		/// </summary>
		public void ArrangeTopToBottom()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(0, (masterbounds.Y + masterbounds.Height) - (bounds.Y)); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the bottom boundary with the bottom boundary of the master element.
		/// </summary>
		public void ArrangeBottomToBottom()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(0, (masterbounds.Y + masterbounds.Height) - (bounds.Y + bounds.Height)); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the left boundary with the left boundary of the master element.
		/// </summary>
		public void ArrangeLeftToLeft()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(masterbounds.X - bounds.X, 0); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the left boundary with the right boundary of the master element.
		/// </summary>
		public void ArrangeLeftToRight()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition((masterbounds.X + masterbounds.Width) - bounds.X, 0); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the right boundary with the left boundary of the master element.
		/// </summary>
		public void ArrangeRightToLeft()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition((masterbounds.X) - (bounds.X + bounds.Width), 0); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the right boundary with the right boundary of the master element.
		/// </summary>
		public void ArrangeRightToRight()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition((masterbounds.X + masterbounds.Width) - (bounds.X + bounds.Width), 0); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the horizontal middle line of the last selected object.
		/// </summary>
		public void ArrangeVertical()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition((masterbounds.X + masterbounds.Width * 0.5f) - (bounds.X + bounds.Width * 0.5f), 0); }
			);
		}

		/// <summary>
		/// Arranges the objects so they share the vertical middle line of the last selected object.
		/// </summary>
		public void ArrangeHorizontal()
		{
			Arrange(delegate(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds)
			{ obj.ShiftPosition(0, (masterbounds.Y + masterbounds.Height * 0.5f) - (bounds.Y + bounds.Height * 0.5f)); }
			);
		}

		/// <summary>
		/// Arranges the objects so they their vertical middle line is uniform spaced between the first and the last selected object.
		/// </summary>
		public void ArrangeHorizontalTable()
		{
			if (SelectedObjects.Count < 3)
				return;

			var firstbound = SelectedObjects[0].ObjectOutlineForArrangements.GetBounds();
			var lastbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements.GetBounds();
			var step = (lastbound.X + lastbound.Width * 0.5) - (firstbound.X + firstbound.Width * 0.5);
			step /= (SelectedObjects.Count - 1);

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i > 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				var bounds = o.ObjectOutlineForArrangements.GetBounds();
				o.ShiftPosition((firstbound.X + firstbound.Width * 0.5) + i * step - (bounds.X + bounds.Width * 0.5), 0);
			}

			_view.InvalidateCachedGraphBitmapAndRepaint(); // force a refresh
		}

		/// <summary>
		/// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
		/// </summary>
		public void ArrangeVerticalTable()
		{
			if (SelectedObjects.Count < 3)
				return;

			var firstbound = SelectedObjects[0].ObjectOutlineForArrangements.GetBounds();
			var lastbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements.GetBounds();
			var step = (lastbound.Y + lastbound.Height * 0.5) - (firstbound.Y + firstbound.Height * 0.5);
			step /= (SelectedObjects.Count - 1);

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i > 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				var bounds = o.ObjectOutlineForArrangements.GetBounds();
				o.ShiftPosition(0, (firstbound.Y + firstbound.Height * 0.5) + i * step - (bounds.Y + bounds.Height * 0.5));
			}

			_view.InvalidateCachedGraphBitmapAndRepaint(); // force a refresh
		}

		public void MoveSelectedGraphItemsUp()
		{
			var selectedItems = new HashSet<object>();
			foreach (var hittestobject in SelectedObjects)
				selectedItems.Add(hittestobject.HittedObject);

			foreach (var layer in _doc.RootLayer.TakeFromHereToFirstLeaves())
			{
				Altaxo.Collections.ListExtensions.MoveSelectedItemsTowardsHigherIndices(layer.GraphObjects, i => selectedItems.Contains(layer.GraphObjects[i]), 1);
			}
		}

		public void MoveSelectedGraphItemsDown()
		{
			var selectedItems = new HashSet<object>();
			foreach (var hittestobject in SelectedObjects)
				selectedItems.Add(hittestobject.HittedObject);

			foreach (var layer in _doc.RootLayer.TakeFromHereToFirstLeaves())
			{
				Altaxo.Collections.ListExtensions.MoveSelectedItemsTowardsLowerIndices(layer.GraphObjects, i => selectedItems.Contains(layer.GraphObjects[i]), 1);
			}
		}

		public void MoveSelectedGraphItemsToTop()
		{
			var selectedItems = new HashSet<object>();
			foreach (var hittestobject in SelectedObjects)
				selectedItems.Add(hittestobject.HittedObject);

			foreach (var layer in _doc.RootLayer.TakeFromHereToFirstLeaves())
			{
				Altaxo.Collections.ListExtensions.MoveSelectedItemsToMaximumIndex(layer.GraphObjects, i => selectedItems.Contains(layer.GraphObjects[i]));
			}
		}

		public void MoveSelectedGraphItemsToBottom()
		{
			var selectedItems = new HashSet<object>();
			foreach (var hittestobject in SelectedObjects)
				selectedItems.Add(hittestobject.HittedObject);

			foreach (var layer in _doc.RootLayer.TakeFromHereToFirstLeaves())
			{
				Altaxo.Collections.ListExtensions.MoveSelectedItemsToMinimumIndex(layer.GraphObjects, i => selectedItems.Contains(layer.GraphObjects[i]));
			}
		}

		public bool IsCmdDeleteEnabled()
		{
			return true;
		}

		public void CmdDelete()
		{
			if (SelectedObjects.Count > 0)
			{
				RemoveSelectedObjects();
			}
			else
			{
				// nothing is selected, we assume that the user wants to delete the worksheet itself
				Current.ProjectService.DeleteGraphDocument(Doc, false);
			}
		}

		/// <summary>
		/// Removes the currently selected objects (the <see cref="IHitTestObject" /> of the selected object(s) must provide
		/// a handler for deleting the object).
		/// </summary>
		public void RemoveSelectedObjects()
		{
			System.Collections.Generic.List<IHitTestObject> removedObjects = new System.Collections.Generic.List<IHitTestObject>();

			foreach (IHitTestObject o in SelectedObjects)
			{
				if (o.Remove != null)
				{
					if (true == o.Remove(o))
						removedObjects.Add(o);
				}
			}

			if (removedObjects.Count > 0)
			{
				foreach (IHitTestObject o in removedObjects)
					SelectedObjects.Remove(o);

				_view.InvalidateCachedGraphBitmapAndRepaint();
			}
		}

		public bool IsCmdCopyEnabled()
		{
			return true;
		}

		/// <summary>
		/// Copy the selected objects of this graph to the clipboard.
		/// </summary>
		public void CopySelectedObjectsToClipboard()
		{
			if (0 == SelectedObjects.Count)
			{
				// we copy the whole graph as xml
				this.Doc.CopyToClipboardAsNative();
			}
			else
			{
				var objectList = new ArrayList();
				foreach (IHitTestObject o in SelectedObjects)
				{
					objectList.Add(o.HittedObject);
				}
				ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphObjectListAsXml", objectList);
			}
		}

		public bool IsCmdCutEnabled()
		{
			return 0 != SelectedObjects.Count;
		}

		public void CutSelectedObjectsToClipboard()
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			var objectList = new ArrayList();
			var notSerialized = new List<IHitTestObject>();

			foreach (IHitTestObject o in SelectedObjects)
			{
				objectList.Add(o.HittedObject);
			}

			ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.GraphObjectListAsXml", objectList);

			// Remove the not serialized objects from the selection, so they are not removed from the graph..
			foreach (IHitTestObject o in notSerialized)
				SelectedObjects.Remove(o);

			this.RemoveSelectedObjects();
		}

		public bool IsCmdPasteEnabled()
		{
			return true;
		}

		public void PasteObjectsFromClipboard()
		{
			GraphDocument gd = this.Doc;
			var dao = Current.Gui.OpenClipboardDataObject();

			if (dao.GetDataPresent("Altaxo.Graph.GraphObjectListAsXml"))
			{
				object obj = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.GraphObjectListAsXml");
				if (obj is ICollection)
				{
					ICollection list = (ICollection)obj;
					foreach (object item in list)
					{
						if (item is GraphicBase)
							this.ActiveLayer.GraphObjects.Add(item as GraphicBase);
						else if (item is Altaxo.Graph.Gdi.Plot.IGPlotItem && ActiveLayer is XYPlotLayer)
							((XYPlotLayer)ActiveLayer).PlotItems.Add((Altaxo.Graph.Gdi.Plot.IGPlotItem)item);
					}
				}
				return;
			}
			if (dao.GetDataPresent("Altaxo.Graph.GraphDocumentAsXml"))
			{
				Doc.PasteFromClipboardAsGraphStyle(true);
				return;
			}
			if (dao.GetDataPresent("Altaxo.Graph.GraphLayerAsXml"))
			{
				Doc.PasteFromClipboardAsNewLayer();
				return;
			}

			if (dao.ContainsFileDropList())
			{
				bool bSuccess = false;
				System.Collections.Specialized.StringCollection coll = dao.GetFileDropList();
				foreach (string filename in coll)
				{
					ImageProxy img;
					try
					{
						img = ImageProxy.FromFile(filename);
						if (img != null)
						{
							var size = this.ActiveLayer.Size;
							size.X /= 2;
							size.Y /= 2;

							PointD2D imgSize = img.GetImage().PhysicalDimension;

							double scale = Math.Min(size.X / imgSize.X, size.Y / imgSize.Y);
							imgSize.X *= scale;
							imgSize.Y *= scale;

							EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointD2D.Empty, imgSize, img);
							this.ActiveLayer.GraphObjects.Add(item);
							bSuccess = true;
							continue;
						}
					}
					catch (Exception)
					{
					}
				}
				if (bSuccess)
					return;
			}

			if (dao.GetDataPresent(typeof(System.Drawing.Imaging.Metafile)))
			{
				System.Drawing.Imaging.Metafile img = dao.GetData(typeof(System.Drawing.Imaging.Metafile)) as System.Drawing.Imaging.Metafile;
				if (img != null)
				{
					var size = 0.5 * this.ActiveLayer.Size;
					EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointD2D.Empty, size, ImageProxy.FromImage(img));
					this.ActiveLayer.GraphObjects.Add(item);
					return;
				}
			}
			if (dao.ContainsImage())
			{
				Image img = dao.GetImage();
				if (img != null)
				{
					var size = 0.5 * this.ActiveLayer.Size;
					EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointD2D.Empty, size, ImageProxy.FromImage(img));
					this.ActiveLayer.GraphObjects.Add(item);
					return;
				}
			}
		}

		/// <summary>
		/// Groups the selected objects to form a ShapeGroup.
		/// </summary>
		public void GroupSelectedObjects()
		{
			var objectsToGroup = new List<IHitTestObject>();
			do
			{
				objectsToGroup.Clear();
				HostLayer currentLayer = null;
				foreach (IHitTestObject o in SelectedObjects)
				{
					var graphObject = o.HittedObject as GraphicBase;
					if (null == graphObject)
						continue;
					var layer = graphObject.ParentObject as HostLayer;
					if (null == layer)
						continue;

					if (null == currentLayer)
					{
						currentLayer = layer;
						objectsToGroup.Add(o);
					}
					else if (object.ReferenceEquals(currentLayer, layer))
					{
						objectsToGroup.Add(o);
					}
				}

				// if objectsToGroup contains at least two items, we can group them together, using the position of the first item
				// if objectsToGroup contains only one item, we ignore it, but remove it from selected objects.
				// if objectsToGroup contains no item, we are done

				if (objectsToGroup.Count >= 2)
				{
					// note that: we must take care of the visibility: more visible items must be added later to the list (and elements from layers with higher index)
					// the group element must be added to the position at which the element with the best visibility was before

					var elements = new List<GraphicBase>();
					foreach (var hit in objectsToGroup)
						elements.Add(hit.HittedObject as GraphicBase);
					var group = new Altaxo.Graph.Gdi.Shapes.ShapeGroup(elements);
					int index = currentLayer.GraphObjects.IndexOf(elements[0]);
					currentLayer.GraphObjects.Insert(index, group);

					foreach (var ele in objectsToGroup)
					{
						SelectedObjects.Remove(ele);
						ele.Remove(ele);
					}
				}
				else if (objectsToGroup.Count == 1)
				{
					SelectedObjects.Remove(objectsToGroup[0]);
				}
			}
			while (objectsToGroup.Count > 0);

			SelectedObjects.Clear();
			_view.InvalidateCachedGraphBitmapAndRepaint();
		}

		/// <summary>
		/// Ungroups the selected objects (if they are ShapeGroup objects).
		/// </summary>
		public void UngroupSelectedObjects()
		{
			foreach (IHitTestObject o in SelectedObjects)
			{
				var shapeGroup = o.HittedObject as Altaxo.Graph.Gdi.Shapes.ShapeGroup;
				if (null != shapeGroup)
				{
					var parentLayer = shapeGroup.ParentObject as HostLayer;
					if (null != parentLayer)
					{
						int idx = parentLayer.GraphObjects.IndexOf(shapeGroup);
						parentLayer.GraphObjects.RemoveAt(idx);
						var separateObjects = shapeGroup.Ungroup();
						for (int i = separateObjects.Length - 1; i >= 0; i--)
							parentLayer.GraphObjects.Insert(idx, separateObjects[i]);
					}
				}
			}
			SelectedObjects.Clear();
		}

		public void SetSelectedObjectsProperty(IRoutedSetterProperty property)
		{
			foreach (IHitTestObject o in this.SelectedObjects)
			{
				if (o.HittedObject is IRoutedPropertyReceiver)
				{
					((IRoutedPropertyReceiver)(o.HittedObject)).SetRoutedProperty(property);
				}
			}
		}

		public void GetSelectedObjectsProperty(IRoutedGetterProperty property)
		{
			foreach (IHitTestObject o in this.SelectedObjects)
			{
				if (o.HittedObject is IRoutedPropertyReceiver)
				{
					((IRoutedPropertyReceiver)(o.HittedObject)).GetRoutedProperty(property);
				}
			}
		}

		#endregion Arrange

		public void Dispose()
		{
			if (_view is IDisposable)
				((IDisposable)_view).Dispose();

			_view = null;
		}

		public bool Apply()
		{
			return true;
		}
	}
}