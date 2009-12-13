using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Gui.Graph.Viewing
{
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi;
	using Altaxo.Graph.Gdi.Shapes;


	[ExpectedTypeOfView(typeof(IGraphView))]
	public class GraphController : IGraphController, IGraphViewEventSink
	{
		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float PtPerInch = 72;

		/// <summary>Holds the Graph document (the place were the layers, plots, graph elements... are stored).</summary>
		protected GraphDocument _doc;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected IGraphView _view;



		/// <summary>Number of the currently selected layer (or -1 if no layer is present).</summary>
		protected int _currentLayerNumber=-1;

		/// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
		protected int _currentPlotNumber=-1;

		/// <summary>Current zoom factor. If AutoZoom is on, this factor is calculated automatically.</summary>
		protected double _zoomFactor;

		/// <summary>If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
		protected bool _isAutoZoomActive=true; // if true, the sheet is zoomed as big as possible to fit into window

		/// <summary>
		/// Ratio of view port dimension to the dimension of the graph. 
		/// Example: a values of 2 means that the view port size is two times the size of the graph. 
		/// </summary>
		protected double _areaFillingFactor = 1.2f;

		protected PointF _graphViewOffset;


		/// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
		protected object _mouseState;



		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GUI.GraphController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Graph.GUI.SDGraphController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDGraphViewContent", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphController), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			Main.DocumentPath _PathToGraph;
			GraphController _GraphController;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphController s = (GraphController)obj;
				info.AddValue("AutoZoom", s._isAutoZoomActive);
				info.AddValue("Zoom", s._zoomFactor);
				info.AddValue("Graph", Main.DocumentPath.GetAbsolutePath(s.Doc));
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				GraphController s = null != o ? (GraphController)o : new GraphController();
				s._isAutoZoomActive = info.GetBoolean("AutoZoom");
				s._zoomFactor = info.GetSingle("Zoom");

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr._GraphController = s;
				surr._PathToGraph = (Main.DocumentPath)info.GetValue("Graph", s);
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
			{
				object o = Main.DocumentPath.GetObject(_PathToGraph, documentRoot, _GraphController);
				if (o is GraphDocument)
				{
					_GraphController.InternalInitializeGraphDocument(o as GraphDocument);
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion


		#region Constructors

		private GraphController()
		{
		}


		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		public GraphController(GraphDocument graphdoc)
		{
			if (null == graphdoc)
				throw new ArgumentNullException("Leaving the graphdoc null in constructor is not supported here");

			InternalInitializeGraphDocument(graphdoc); // Using DataTable here wires the event chain also
		}

		#endregion

		void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.NumberOfLayers = _doc.Layers.Count; // tell the view how many layers we have
				_view.CurrentLayer = this.CurrentLayerNumber;

				// Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
				this.EhView_GraphPanelSizeChanged();
			}
		}

		#region Properties


		/// <summary>
		/// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
		/// </summary>
		public XYPlotLayerCollection Layers
		{
			get { return _doc.Layers; }
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
					RefreshAutoZoom();
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

				if (value > 0.05)
					_zoomFactor = value;
				else
					_zoomFactor = 0.05f;

				RefreshManualZoom();
			}
		}

		public PointF GraphViewOffset
		{
			get { return _graphViewOffset; }
		}

		/// <summary>
		/// Location of the upper left point of the viewport in graph coordinates.
		/// </summary>
		PointF GraphPaddingOffset
		{
			get
			{
				return new PointF(
					(float)(-_doc.Layers.GraphSize.Width * (_areaFillingFactor - 1) / 2),
					(float)(-_doc.Layers.GraphSize.Height * (_areaFillingFactor - 1) / 2)
					);
			}
		}

		/// <summary>
		/// Size of the viewport (i.e. of the visible window in the graph panel) in units of point (1/72 inch).
		/// </summary>
		SizeF ViewportSizeInPt
		{
			get
			{
				SizeF inch = _view.ViewportSizeInInch;
				return new SizeF(inch.Width*PtPerInch, inch.Height*PtPerInch);
			}
		}

		/// <summary>
		/// Virtual size of the viewport (= size of the graph that can be shown fully), taking into account the ZoomFactor, in units of point (1/72 inch).
		/// </summary>
		SizeF VirtualViewportSizeInPt
		{
			get
			{
				SizeF inch = _view.ViewportSizeInInch;
				return new SizeF((float)(inch.Width * PtPerInch/_zoomFactor), (float)(inch.Height * PtPerInch/_zoomFactor));
			}
		}

		PointF ScrollPositionToGraphViewOffset(PointF scrollPos)
		{
			SizeF virtualSize = VirtualViewportSizeInPt;
			PointF po = GraphPaddingOffset;

			return new PointF(
				(float)(scrollPos.X * (_doc.Layers.GraphSize.Width * _areaFillingFactor - virtualSize.Width) + po.X),
				(float)(scrollPos.Y * (_doc.Layers.GraphSize.Height * _areaFillingFactor - virtualSize.Height) + po.Y)
				);
		}

		PointF GraphViewOffsetToScrollPosition(PointF viewOffset)
		{
			SizeF virtualSize = VirtualViewportSizeInPt;
			PointF po = GraphPaddingOffset;

			double x = (viewOffset.X - po.X) / (_doc.Layers.GraphSize.Width * _areaFillingFactor - virtualSize.Width);
			double y = (viewOffset.Y - po.Y) / (_doc.Layers.GraphSize.Height * _areaFillingFactor - virtualSize.Height);

			if (!(x >= 0))
				x = 0;
			if (!(x <= 1))
				x = 1;
			if (!(y >= 0))
				y = 0;
			if (!(y <= 1))
				y = 1;
			return new PointF((float)x, (float)y);
		}


		#region Properties
		/// <summary>
		/// This event will be fired if the current graph tool has changed, either by the user
		/// or by the program.
		/// </summary>
		public event EventHandler CurrentGraphToolChanged;


		public GraphToolType CurrentGraphTool
		{
			get
			{
				return _view.GraphTool;
			}
			set
			{
				var oldValue = _view.GraphTool;
				_view.GraphTool = value;
				if (oldValue != value && null != CurrentGraphToolChanged)
					CurrentGraphToolChanged(this, EventArgs.Empty);

			}
		}
		

		#endregion // Properties


		#endregion


		#region IGraphController Members

		private void InternalInitializeGraphDocument(GraphDocument doc)
		{
			if (_doc != null)
				throw new ApplicationException("_doc is already initialized");
			if (doc == null)
				throw new ArgumentNullException("doc");

			_doc = doc;

			_doc.Changed += this.EhGraph_Changed;
			_doc.Layers.LayerCollectionChanged += this.EhGraph_LayerCollectionChanged;
			_doc.BoundsChanged += this.EhGraph_BoundsChanged;
			_doc.NameChanged += this.EhGraphDocumentNameChanged;

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
			get { return _doc; }
		}

		/// <summary>
		/// Returns the currently active layer, or null if there is no active layer.
		/// </summary>
		public XYPlotLayer ActiveLayer
		{
			get
			{
				return this._currentLayerNumber < 0 ? null : _doc.Layers[this._currentLayerNumber];
			}
			set
			{
				CurrentLayerNumber = value.Number;
			}
		}

		/// <summary>
		/// Get / sets the currently active layer by number.
		/// </summary>
		public int CurrentLayerNumber
		{
			get
			{
				EnsureValidityOfCurrentLayerNumber();
				return _currentLayerNumber;
			}
			set
			{
				int oldValue = this._currentLayerNumber;

				// negative values are only accepted if there is no layer
				if (value < 0 && _doc.Layers.Count > 0)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber", value, "Accepted values must be >=0 if there is at least one layer in the graph!");

				if (value >= _doc.Layers.Count)
					throw new ArgumentOutOfRangeException("CurrentLayerNumber", value, "Accepted values must be less than the number of layers in the graph(currently " + _doc.Layers.Count.ToString() + ")!");

				_currentLayerNumber = value < 0 ? -1 : value;

				// if something changed
				if (oldValue != this._currentLayerNumber)
				{
					// reflect the change in layer number in the layer tool bar
					if (_view != null)
						_view.CurrentLayer = this._currentLayerNumber;

					// since the layer changed, also the plots changed, so the menu has
					// to reflect the new plots
					//this.UpdateDataPopup();
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
				if (CurrentLayerNumber >= 0 && 0 != this._doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length && value < 0)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to be greater or equal than zero");

				if (CurrentLayerNumber >= 0 && value >= _doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length)
					throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to  be lesser than actual count: " + _doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length.ToString());

				_currentPlotNumber = value < 0 ? -1 : value;
			}
		}

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		public void EnsureValidityOfCurrentLayerNumber()
		{
			if (_doc.Layers.Count > 0) // if at least one layer is present
			{
				if (_currentLayerNumber < 0)
					CurrentLayerNumber = 0;
				else if (_currentLayerNumber >= _doc.Layers.Count)
					CurrentLayerNumber = _doc.Layers.Count - 1;
			}
			else // no layers present
			{
				if (-1 != _currentLayerNumber)
					CurrentLayerNumber = -1;
			}
		}


		/// <summary>
		/// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
		/// the current plot number is set to -1.
		/// </summary>
		public void EnsureValidityOfCurrentPlotNumber()
		{
			EnsureValidityOfCurrentLayerNumber();

			// if XYPlotLayer don't exist anymore, correct CurrentLayerNumber and ActualPlotAssocitation
			if (null != ActiveLayer) // if the ActiveLayer exists
			{
				// if the XYColumnPlotData don't exist anymore, correct it
				if (ActiveLayer.PlotItems.Flattened.Length > 0) // if at least one plotitem exists
				{
					if (_currentPlotNumber < 0)
						CurrentPlotNumber = 0;
					else if (_currentPlotNumber > ActiveLayer.PlotItems.Flattened.Length)
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
				_view.RefreshGraph();
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.Controller = null;
				}

				_view = value as IGraphView;

				if (null != _view)
				{
					Initialize(false);
					_view.Controller = this;
				}
			}
		}

		#endregion

		#region Event handlers from GraphDocument

		/// <summary>
		/// Called if something in the <see cref="GraphDocument"/> changed.
		/// </summary>
		/// <param name="sender">Not used (always the GraphDocument).</param>
		/// <param name="e">The EventArgs.</param>
		protected void EhGraph_Changed(object sender, System.EventArgs e)
		{
			// if something changed on the graph, make sure that the layer and plot number reflect this changed
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();

			if (null != _view)
				_view.RefreshGraph();
		}


		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraph_BoundsChanged(object sender, System.EventArgs e)
		{
			if (_view != null)
			{
				if (this._isAutoZoomActive)
					this.RefreshAutoZoom();
				_view.RefreshGraph();
			}
		}


		/// <summary>
		/// Handler of the event LayerCollectionChanged of the graph document. Forces to
		/// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
		/// <param name="sender">The sender of the event (the GraphDocument).</param>
		/// <param name="e">The event arguments.</param>
		protected void EhGraph_LayerCollectionChanged(object sender, System.EventArgs e)
		{
			int oldActiveLayer = this._currentLayerNumber;

			// Ensure that the current layer and current plot are valid anymore
			EnsureValidityOfCurrentLayerNumber();

			if (oldActiveLayer != this._currentLayerNumber)
			{
				if (_view != null)
					_view.CurrentLayer = this._currentLayerNumber;
			}

			// even if the active layer number not changed, it can be that the layer itself has changed from
			// one to another, so make sure that the current plot number is valid also
			EnsureValidityOfCurrentPlotNumber();

			// make sure the view knows about when the number of layers changed
			if (_view != null)
				_view.NumberOfLayers = _doc.Layers.Count;
		}

		public void EhGraphDocumentNameChanged(object sender, Main.NameChangedEventArgs e)
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

		#endregion

		#region IGraphViewEventSink

		/// <summary>
		/// The controller should show a data context menu (contains all plots of the currentLayer).
		/// </summary>
		/// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
		/// <param name="parent">The parent control which is the parent of the context menu.</param>
		/// <param name="pt">The location where the context menu should be shown.</param>
		public virtual void EhView_ShowDataContextMenu(int currLayer, object parent, Point pt)
		{
			int oldCurrLayer = this.ActiveLayer.Number;
		
			this.CurrentLayerNumber = currLayer;

			if (null != this.ActiveLayer)
			{
				Current.Gui.ShowContextMenu(pt.X, pt.Y, parent, "/Altaxo/Views/Graph/LayerButton/ContextMenu");
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
			// this.CurrentGraphToolType = currGraphToolType;
		}


		/// <summary>
		/// Handles the selection of the current layer by the <b>user</b>.
		/// </summary>
		/// <param name="currLayer">The current layer number as selected by the user.</param>
		/// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
		public virtual void EhView_CurrentLayerChoosen(int currLayer, bool bAlternative)
		{
			int oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = currLayer;


			// if we have clicked the button already down then open the layer dialog
			if (null != ActiveLayer && currLayer == oldCurrLayer && false == bAlternative)
			{
				LayerController.ShowDialog(ActiveLayer);
				//LayerDialog dlg = new LayerDialog(ActiveLayer,LayerDialog.Tab.Scale,EdgeType.Bottom);
				//dlg.ShowDialog(this.m_View.Window);
			}
		}

		/// <summary>
		/// Handles the event when the size of the graph area is changed.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		public virtual void EhView_GraphPanelSizeChanged()
		{
			if (_isAutoZoomActive)
			{
				RefreshAutoZoom();
			}
			else
			{
				//double pixelh = System.Math.Ceiling(_doc.PageBounds.Width * this._horizontalResolution * this._zoomFactor / (UnitPerInch));
				//double pixelv = System.Math.Ceiling(_doc.PageBounds.Height * this._verticalResolution * this._zoomFactor / (UnitPerInch));
				_view.ShowGraphScrollBars = true;
			}

		}

		public void EhView_Scroll()
		{
			_graphViewOffset = ScrollPositionToGraphViewOffset(_view.GraphScrollPosition);
			_view.RefreshGraph();
		}


		#endregion

		/// <summary>
		/// Recalculates and sets the value of m_Zoom so the whole page is visible
		/// </summary>
		protected void RefreshAutoZoom()
		{
			CalculateAutoZoom();
			_view.ShowGraphScrollBars = false;
			RefreshGraph();
		}


		void CalculateAutoZoom()
		{
			double zoomh = (PtPerInch * _view.ViewportSizeInInch.Width)  / (_doc.Layers.GraphSize.Width * _areaFillingFactor);
			double zoomv = (PtPerInch * _view.ViewportSizeInInch.Height) / (_doc.Layers.GraphSize.Height * _areaFillingFactor);
			_zoomFactor = System.Math.Min(zoomh, zoomv);
			_graphViewOffset = GraphPaddingOffset;
		}


		protected void RefreshManualZoom()
		{
			SizeF virtualSize = VirtualViewportSizeInPt;
			float xratio = virtualSize.Width / _doc.Layers.GraphSize.Width;
			float yratio = virtualSize.Height / _doc.Layers.GraphSize.Height;

			bool showScrollbars = xratio < 1 || yratio < 1;
			if (showScrollbars)
			{
				_view.GraphScrollPosition = GraphViewOffsetToScrollPosition(_graphViewOffset);
				_view.ShowGraphScrollBars = true;
			}
			else
			{
				_view.ShowGraphScrollBars = false;
				// we center the graph in the viewport
				SizeF gz = _doc.Layers.GraphSize;
				SizeF vz = VirtualViewportSizeInPt;
				_graphViewOffset = new PointF((gz.Width - vz.Width) / 2, (gz.Height - vz.Height) / 2);
			}
			if (null != _view)
				_view.RefreshGraph();
		}

		#region Arrange

		public IList<IHitTestObject> SelectedObjects
		{
			get
			{
				return _view == null ? new List<IHitTestObject>() : _view.SelectedObjects;
			}
		}


		public delegate void ArrangeElement(IHitTestObject obj, RectangleF bounds, RectangleF masterbounds);

		/// <summary>
		/// Arranges the objects so they share a common boundary.
		/// </summary>
		/// <param name="arrange">Routine that determines how to arrange the element with respect to the master element.</param>
		public void Arrange(ArrangeElement arrange)
		{
			if (SelectedObjects.Count < 2)
				return;


			RectangleF masterbound = SelectedObjects[SelectedObjects.Count - 1].ObjectPath.GetBounds();

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i >= 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				RectangleF bounds = o.ObjectPath.GetBounds();

				arrange(o, bounds, masterbound);
			}

			_view.RefreshGraph(); // force a refresh
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


			RectangleF firstbound = SelectedObjects[0].SelectionPath.GetBounds();
			RectangleF lastbound = SelectedObjects[SelectedObjects.Count - 1].SelectionPath.GetBounds();
			float step = (lastbound.X + lastbound.Width * 0.5f) - (firstbound.X + firstbound.Width * 0.5f);
			step /= (SelectedObjects.Count - 1);

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i > 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				RectangleF bounds = o.SelectionPath.GetBounds();
				o.ShiftPosition((firstbound.X + firstbound.Width * 0.5f) + i * step - (bounds.X + bounds.Width * 0.5f), 0);
			}

			_view.RefreshGraph(); // force a refresh
		}

		/// <summary>
		/// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
		/// </summary>
		public void ArrangeVerticalTable()
		{
			if (SelectedObjects.Count < 3)
				return;


			RectangleF firstbound = SelectedObjects[0].SelectionPath.GetBounds();
			RectangleF lastbound = SelectedObjects[SelectedObjects.Count - 1].SelectionPath.GetBounds();
			float step = (lastbound.Y + lastbound.Height * 0.5f) - (firstbound.Y + firstbound.Height * 0.5f);
			step /= (SelectedObjects.Count - 1);

			// now move each object to the new position, which is the difference in the position of the bounds.X
			for (int i = SelectedObjects.Count - 2; i > 0; i--)
			{
				IHitTestObject o = SelectedObjects[i];
				RectangleF bounds = o.SelectionPath.GetBounds();
				o.ShiftPosition(0, (firstbound.Y + firstbound.Height * 0.5f) + i * step - (bounds.Y + bounds.Height * 0.5f));
			}

			_view.RefreshGraph(); // force a refresh
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

				_view.RefreshGraph();
			}
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


				var dao = Current.Gui.GetNewClipboardDataObject();

				var objectList = new List<object>();

				foreach (IHitTestObject o in SelectedObjects)
				{
					if (o.HittedObject is System.Runtime.Serialization.ISerializable)
					{
						objectList.Add(o.HittedObject);
					}
				}

				dao.SetData("Altaxo.Graph.GraphObjectList", objectList);

				// Test code to test if the object list can be serialized


				// now copy the data object to the clipboard
				Current.Gui.SetClipboardDataObject(dao, true);
			}
		}

		public void CutSelectedObjectsToClipboard()
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			var objectList = new List<object>();
			var notSerialized = new List<IHitTestObject>();

			foreach (IHitTestObject o in SelectedObjects)
			{
				if (o.HittedObject is System.Runtime.Serialization.ISerializable)
				{
					objectList.Add(o.HittedObject);
				}
				else
				{
					notSerialized.Add(o);
				}
			}

			dao.SetData("Altaxo.Graph.GraphObjectList", objectList);

			// now copy the data object to the clipboard
			Current.Gui.SetClipboardDataObject(dao, true);

			// Remove the not serialized objects from the selection, so they are not removed from the graph..
			foreach (IHitTestObject o in notSerialized)
				SelectedObjects.Remove(o);

			this.RemoveSelectedObjects();
		}

		public void PasteObjectsFromClipboard()
		{
			GraphDocument gd = this.Doc;
			var dao = Current.Gui.OpenClipboardDataObject();

			string[] formats = dao.GetFormats();
			System.Diagnostics.Trace.WriteLine("Available formats:");

			if (dao.GetDataPresent("Altaxo.Graph.GraphObjectList"))
			{
				object obj = dao.GetData("Altaxo.Graph.GraphObjectList");

				// if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
				if (obj is ICollection)
				{
					ICollection list = (ICollection)obj;
					foreach (object item in list)
					{
						if (item is GraphicBase)
							this.ActiveLayer.GraphObjects.Add(item as GraphicBase);
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
							SizeF size = this.ActiveLayer.Size;
							size.Width /= 2;
							size.Height /= 2;
							EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, img);
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
					SizeF size = this.ActiveLayer.Size;
					size.Width /= 2;
					size.Height /= 2;
					EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, ImageProxy.FromImage(img));
					this.ActiveLayer.GraphObjects.Add(item);
					return;
				}
			}
			if (dao.ContainsImage())
			{
				Image img = dao.GetImage();
				if (img != null)
				{
					SizeF size = this.ActiveLayer.Size;
					size.Width /= 2;
					size.Height /= 2;
					EmbeddedImageGraphic item = new EmbeddedImageGraphic(PointF.Empty, size, ImageProxy.FromImage(img));
					this.ActiveLayer.GraphObjects.Add(item);
					return;
				}
			}
		}


		// <summary>
		/// Groups the selected objects to form a ShapeGroup.
		/// </summary>
		public void GroupSelectedObjects()
		{
			System.Collections.Generic.List<IHitTestObject> removedObjects = new System.Collections.Generic.List<IHitTestObject>();

			Altaxo.Graph.Gdi.Shapes.ShapeGroup group = new Altaxo.Graph.Gdi.Shapes.ShapeGroup();
			foreach (IHitTestObject o in SelectedObjects)
			{
				if (o.HittedObject is Altaxo.Graph.Gdi.Shapes.GraphicBase)
				{
					group.Add((Altaxo.Graph.Gdi.Shapes.GraphicBase)o.HittedObject);
					removedObjects.Add(o);
				}
			}

			if (removedObjects.Count > 0)
			{
				foreach (IHitTestObject o in removedObjects)
				{
					o.Remove(o);
					SelectedObjects.Remove(o);
				}
			}

			ActiveLayer.GraphObjects.Add(group);
			_view.RefreshGraph();
		}


		// <summary>
		/// Ungroups the selected objects (if they are ShapeGroup objects).
		/// </summary>
		public void UngroupSelectedObjects()
		{
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



		#endregion

	}
}
