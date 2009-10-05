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

namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// GraphController is our default implementation to control a graph view.
  /// </summary>
  [Altaxo.Gui.UserControllerForObject(typeof(GraphDocument))]
  [Altaxo.Gui.ExpectedTypeOfView(typeof(IGraphView))]
  public class GraphController : IGraphController
  {

    #region Member variables

    // following default unit is point (1/72 inch)
    /// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
    protected const float UnitPerInch = 72;


    /// <summary>Holds the Graph document (the place were the layers, plots, graph elements... are stored).</summary>
    protected GraphDocument _doc;

    /// <summary>Holds the view (the window where the graph is visualized).</summary>
    protected IGraphView _view;
    
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

    /// <summary>Current zoom factor. If AutoZoom is on, this factor is calculated automatically.</summary>
    protected float _zoomFactor;
    
    /// <summary>If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
    protected bool  _isAutoZoomActive; // if true, the sheet is zoomed as big as possible to fit into window

		/// <summary>
		/// Ratio of view port dimension to the dimension of the graph. 
		/// Example: a values of 2 means that the view port size is two times the size of the graph. 
		/// </summary>
		protected float _areaFillingFactor = 1.2f;

		protected PointF _graphViewOffset;
    
    /// <summary>Number of the currently selected layer (or -1 if no layer is present).</summary>
    protected int _currentLayerNumber;

    /// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
    protected int _currentPlotNumber;
    
    /// <summary>A instance of a mouse handler class that currently handles the mouse events..</summary>
    protected MouseStateHandler _mouseState;



    /// <summary>
    /// This holds a frozen image of the graph during the moving time
    /// </summary>
    protected Bitmap _cachedGraphImage;

    protected bool   _isCachedGraphImageDirty;

    #endregion Member variables

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphController),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      Main.DocumentPath _PathToGraph;
      GraphController   _GraphController;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphController s = (GraphController)obj;
        info.AddValue("AutoZoom",s._isAutoZoomActive);
        info.AddValue("Zoom",s._zoomFactor);
        info.AddValue("Graph",Main.DocumentPath.GetAbsolutePath(s._doc));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphController s = null!=o ? (GraphController)o : new GraphController(null,true);
        s._isAutoZoomActive = info.GetBoolean("AutoZoom");
        s._zoomFactor = info.GetSingle("Zoom");
        s._doc = null;
        
        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
        surr._GraphController = s;
        surr._PathToGraph = (Main.DocumentPath)info.GetValue("Graph",s);
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        
        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        object o = Main.DocumentPath.GetObject(_PathToGraph,documentRoot,_GraphController);
        if(o is GraphDocument)
        {
          _GraphController.Doc = o as GraphDocument;
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
        }
      }
    }


    
    #endregion

    #region Constructors


    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    protected virtual void SetMemberVariablesToDefault()
    {
      _nonPageAreaColor = Color.Gray;
    
      _pageGroundBrush = new BrushX(Color.LightGray);

      _graphAreaBrush = new BrushX(Color.Snow);

      _horizontalResolution  = 300;
    
      _verticalResolution = 300;

      _zoomFactor  = 0.4f;
    
      // If true, the view is zoomed so that the page fits exactly into the viewing area.</summary>
      _isAutoZoomActive = true; // if true, the sheet is zoomed as big as possible to fit into window
    
    
      // Number of the currently selected layer (or -1 if no layer is present).</summary>
      _currentLayerNumber = -1;
    
      // Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
      _currentPlotNumber = -1;
    
      // Currently selected GraphTool.</summary>
      // m_CurrentGraphTool = GraphTools.ObjectPointer;
    
      // A instance of a mouse handler class that currently handles the mouse events..</summary>
      _mouseState= new ObjectPointerMouseHandler(this);

      

      // This holds a frozen image of the graph during the moving time
      _cachedGraphImage=null;
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    public GraphController(GraphDocument graphdoc)
      : this(graphdoc,false)
    {
    }

    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    /// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
    public GraphController(GraphDocument graphdoc, bool bDeserializationConstructor)
    {
      SetMemberVariablesToDefault();
    
      if(null!=graphdoc)
        this.Doc = graphdoc;
      else if(null==graphdoc && !bDeserializationConstructor)
        throw new ArgumentNullException("graphdoc","GraphDoc must not be null");

      //this.InitializeMenu();

      if(null!=Doc && 0==Doc.Layers.Count)
        Doc.CreateNewLayerNormalBottomXLeftY();
    }


    static GraphController()
    {
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
      get { return _doc; }
      set
      {
        GraphDocument oldDoc=_doc;
        _doc = value;
        if(!object.ReferenceEquals(_doc,oldDoc))
        {
          if(oldDoc!=null)
          {
            oldDoc.Changed -= new EventHandler(this.EhGraph_Changed);
            oldDoc.Layers.LayerCollectionChanged -= new EventHandler(this.EhGraph_LayerCollectionChanged);
            oldDoc.BoundsChanged -= new EventHandler(this.EhGraph_BoundsChanged);
            oldDoc.NameChanged -= new Main.NameChangedEventHandler(this.EhGraphDocumentNameChanged);
          }
          if(_doc!=null)
          {
            _doc.Changed += new EventHandler(this.EhGraph_Changed);
            _doc.Layers.LayerCollectionChanged += new EventHandler(this.EhGraph_LayerCollectionChanged);
            _doc.BoundsChanged += new EventHandler(this.EhGraph_BoundsChanged);
            _doc.NameChanged += new Main.NameChangedEventHandler(this.EhGraphDocumentNameChanged);

            // Ensure the current layer and plot numbers are valid
            this.EnsureValidityOfCurrentLayerNumber();
            this.EnsureValidityOfCurrentPlotNumber();

            OnTitleNameChanged(EventArgs.Empty);
          }
        }
      }
    }

    /// <summary>
    /// Returns the view that this controller controls.
    /// </summary>
    /// <remarks>Setting the view is only neccessary on deserialization, so the controller
    /// can restrict setting the view only the own view is still null.</remarks>
    public IGraphView View
    {
      get { return _view; }
      set
      {
        IGraphView oldView = _view;
        _view = value;

        if(null!=oldView)
        {
          oldView.GraphMenu = null; // don't let the old view have the menu
          oldView.Controller = null; // no longer the controller of this view
        }

        if(null!=_view)
        {
          _view.Controller = this;
          _view.NumberOfLayers = _doc.Layers.Count;
          _view.CurrentLayer = this.CurrentLayerNumber;
          //m_View.CurrentGraphTool = this.CurrentGraphTool;
        
          // Adjust the zoom level just so, that area fits into control
          Graphics grfx = _view.CreateGraphGraphics();
          this._horizontalResolution = grfx.DpiX;
          this._verticalResolution = grfx.DpiY;
          grfx.Dispose();

          // Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
          this.EhView_GraphPanelSizeChanged(new EventArgs());

          // set the menu of this class
          _view.NumberOfLayers = _doc.Layers.Count; // tell the view how many layers we have
        
        }
      }
    }

    /// <summary>
    /// Creates a default view object.
    /// </summary>
    /// <returns>The default view object, or null if there is no default view object.</returns>
    public virtual object CreateDefaultViewObject()
    {
      this.View = new GraphView();
      return this.View;
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
      if(null!=ActiveLayer && currLayer==oldCurrLayer && false==bAlternative)
      {
        LayerController.ShowDialog(ActiveLayer);
        //LayerDialog dlg = new LayerDialog(ActiveLayer,LayerDialog.Tab.Scale,EdgeType.Bottom);
        //dlg.ShowDialog(this.m_View.Window);
      }
    }

    /// <summary>
    /// The controller should show a data context menu (contains all plots of the currentLayer).
    /// </summary>
    /// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
    /// <param name="parent">The parent control which is the parent of the context menu.</param>
    /// <param name="pt">The location where the context menu should be shown.</param>
    public virtual void EhView_ShowDataContextMenu(int currLayer, System.Windows.Forms.Control parent, Point pt)
    {
      int oldCurrLayer = this.CurrentLayerNumber;
      this.CurrentLayerNumber = currLayer;


      if(null!=this.ActiveLayer)
      {
				var menu = Main.Services.GUIFactoryService.CreateContextMenu(parent, "/Altaxo/Views/Graph/LayerButton/ContextMenu");
				menu.Show(parent, pt);
      }
    }

    /// <summary>
    /// This function is called if the user changed the GraphTool.
    /// </summary>
    /// <param name="currGraphToolType">The type of the new selected GraphTool.</param>
    public virtual void EhView_CurrentGraphToolChoosen(System.Type currGraphToolType)
    {
      this.CurrentGraphToolType = currGraphToolType;
    }

    /// <summary>
    /// Called if a key is pressed in the view.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public bool EhView_ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if(this._mouseState!=null)
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
      if(!Current.ApplicationIsClosing)
      {

        if(false==Current.Gui.YesNoMessageBox("Do you really want to close this graph?","Attention",false))
        {
          return true; // cancel the closing
        }
      }
      return false;
    }

    /// <summary>
    /// Called by the host window after the host window was closed.
    /// </summary>
    public void HostWindowClosed()
    {
      Current.ProjectService.RemoveGraph(this);
    }


    /// <summary>
    /// Handles the event when the size of the graph area is changed.
    /// </summary>
    /// <param name="e">EventArgs.</param>
    public virtual void EhView_GraphPanelSizeChanged(EventArgs e)
    {
      if(_isAutoZoomActive)
      {
        CalculateAutoZoom();
    
        // System.Console.WriteLine("h={0}, v={1} {3} {4} {5}",zoomh,zoomv,UnitPerInch,this.ClientSize.Width,this.m_HorizRes, this.m_PageBounds.Width);
        // System.Console.WriteLine("SizeX = {0}, zoom = {1}, dpix={2},in={3}",this.ClientSize.Width,this.m_Zoom,this.m_HorizRes,this.ClientSize.Width/(this.m_HorizRes*this.m_Zoom));

				_view.ShowGraphScrollBars = false;
      }
      else
      {
        double pixelh = System.Math.Ceiling(_doc.PageBounds.Width*this._horizontalResolution*this._zoomFactor/(UnitPerInch));
        double pixelv = System.Math.Ceiling(_doc.PageBounds.Height*this._verticalResolution*this._zoomFactor/(UnitPerInch));
				_view.ShowGraphScrollBars = true;
      }

    }

		public void EhView_Scroll()
		{
			_graphViewOffset = ScrollPositionToGraphViewOffset(_view.GraphScrollPosition);
			_isCachedGraphImageDirty = true;
			_view.InvalidateGraph();
		}

		/// <summary>
		/// Location of the upper left point of the viewport in graph coordinates.
		/// </summary>
		PointF GraphPaddingOffset
		{
			get
			{
				return new PointF(- _doc.Layers.GraphSize.Width*(_areaFillingFactor-1)/2, -_doc.Layers.GraphSize.Height*(_areaFillingFactor-1)/2);
			}
		}

		/// <summary>
		/// Size of the viewport (i.e. of the visible window in the graph panel) in graph coordinates.
		/// </summary>
		SizeF GraphViewportSize
		{
			get
			{
				return PixelToPageCoordinates(_view.GraphSize);
			}
		}

		PointF ScrollPositionToGraphViewOffset(PointF scrollPos)
		{
			SizeF virtualSize = GraphViewportSize;
			PointF po = GraphPaddingOffset;

			return new PointF(
				scrollPos.X * (_doc.Layers.GraphSize.Width  * _areaFillingFactor - virtualSize.Width) + po.X,
				scrollPos.Y * (_doc.Layers.GraphSize.Height * _areaFillingFactor - virtualSize.Height) + po.Y);
		}

		PointF GraphViewOffsetToScrollPosition(PointF viewOffset)
		{
			SizeF virtualSize = GraphViewportSize;
			PointF po = GraphPaddingOffset;

			float x =	(viewOffset.X - po.X) / (_doc.Layers.GraphSize.Width  * _areaFillingFactor - virtualSize.Width);
			float y = (viewOffset.Y - po.Y) / (_doc.Layers.GraphSize.Height * _areaFillingFactor - virtualSize.Height);

			if (!(x >= 0))
				x = 0;
			if (!(x <= 1))
				x = 1;
			if (!(y >= 0))
				y = 0;
			if (!(y <= 1))
				y = 1;
			return new PointF(x, y);
		}



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
      if(!e.ClipRectangle.IsEmpty)
        this.DoPaint(e.Graphics,false);
    }

    #endregion // IGraphView interface definitions

    #region GraphDocument event handlers

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

      if(oldActiveLayer!=this._currentLayerNumber)
      {
        if(View!=null)
          View.CurrentLayer = this._currentLayerNumber;
      }

      // even if the active layer number not changed, it can be that the layer itself has changed from
      // one to another, so make sure that the current plot number is valid also
      EnsureValidityOfCurrentPlotNumber();

      // make sure the view knows about when the number of layers changed
      if(View!=null)
        View.NumberOfLayers = _doc.Layers.Count;
    }


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
      
      RefreshGraph();
    }

    /// <summary>
    /// Handler of the event LayerCollectionChanged of the graph document. Forces to
    /// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
    /// <param name="sender">The sender of the event (the GraphDocument).</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraph_BoundsChanged(object sender, System.EventArgs e)
    {
      this._isCachedGraphImageDirty = true;

      if(View!=null)
      {
        if(this.AutoZoom)
          this.RefreshAutoZoom();
        View.InvalidateGraph();
      }    
    }

    public void EhGraphDocumentNameChanged(object sender, Main.NameChangedEventArgs e)
    {
      if (View != null)
        View.GraphViewTitle = Doc.Name;

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

    #endregion // GraphDocument event handlers

    #region Other event handlers
    
    
    

    /// <summary>
    /// This is called if the host window is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhParentWindowSelected(object sender, EventArgs e)
    {
      if(View!=null)
        View.OnViewSelection();
    }

    /// <summary>
    /// This is called if the host window is deselected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhParentWindowDeselected(object sender, EventArgs e)
    {
      if(View!=null)
        View.OnViewDeselection();
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

      Current.Gui.ShowDialog(new object[] { pa }, string.Format("#{0}: {1}", pa.Name, pa.ToString()),true);

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


   
    /// <summary>
    /// Saves the graph as an enhanced windows metafile into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <returns>Null if successfull, error description otherwise.</returns>
    public string SaveAsMetafile(System.IO.Stream stream)
    {
      try
      {
				GraphDocumentExportActions.RenderAsMetafile(this.Doc, stream, 300);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      return null;
    }

    /// <summary>
    /// Saves the graph as an tiff file into the stream <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to save the metafile into.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="imageFormat">The format of the destination image.</param>
    /// <returns>Null if successfull, error description otherwise.</returns>
		public string SaveAsBitmap(System.IO.Stream stream, int dpiResolution, System.Drawing.Imaging.ImageFormat imageFormat, GraphExportArea usePageBounds)
    {
			GraphDocumentExportActions.RenderAsBitmap(_doc, stream, imageFormat, usePageBounds, dpiResolution);
      return null;
    }

    private void DoPaint(Graphics g, bool bForPrinting)
    {
      if(bForPrinting)
      {
        DoPaintUnbuffered(g,bForPrinting);
      }
      else
      {

        if(_cachedGraphImage==null || _cachedGraphImage.Width!=_view.GraphSize.Width || _cachedGraphImage.Height!=_view.GraphSize.Height)
        {
          if(_cachedGraphImage!=null)
          {
            _cachedGraphImage.Dispose();
            _cachedGraphImage = null;
          }
        
          // create a frozen bitmap of the graph
          // using(Graphics g = m_View.CreateGraphGraphics())
          
          _cachedGraphImage = new Bitmap(_view.GraphSize.Width,_view.GraphSize.Height,g);
          _isCachedGraphImageDirty = true;
        }

        if(_cachedGraphImage==null)
        {
          DoPaintUnbuffered(g,bForPrinting);
        }
        else if(_isCachedGraphImageDirty)
        {
          using(Graphics gbmp = Graphics.FromImage(_cachedGraphImage))
          {
            DoPaintUnbuffered(gbmp,false);
            _isCachedGraphImageDirty=false;
          }
         
          g.DrawImageUnscaled(_cachedGraphImage,0,0,_view.GraphSize.Width,_view.GraphSize.Height);
          ScaleForPaint(g,bForPrinting);
        }
        else
        {
          g.DrawImageUnscaled(_cachedGraphImage,0,0,_view.GraphSize.Width,_view.GraphSize.Height);
          ScaleForPaint(g,bForPrinting); // to be in the same state as when drawing unbuffered
        }
         
        // special painting depending on current selected tool
				g.TranslateTransform(-_graphViewOffset.X, -_graphViewOffset.Y);
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
        g.PageScale = this._zoomFactor;
        float pointsh = UnitPerInch * _view.GraphScrollPosition.X / (this._horizontalResolution * this._zoomFactor);
        float pointsv = UnitPerInch * _view.GraphScrollPosition.Y / (this._verticalResolution * this._zoomFactor);
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
        ScaleForPaint(g,bForPrinting);

        if(!bForPrinting)
        {
          g.Clear(this._nonPageAreaColor);
          // Fill the page with its own color
          //g.FillRectangle(_pageGroundBrush,_doc.PageBounds);
          //g.FillRectangle(m_PrintableAreaBrush,m_Graph.PrintableBounds);
					g.FillRectangle(_graphAreaBrush, -_graphViewOffset.X, -_graphViewOffset.Y, _doc.Layers.GraphSize.Width, _doc.Layers.GraphSize.Height);
          // DrawMargins(g);
        }

        // Paint the graph now
        //g.TranslateTransform(m_Graph.PrintableBounds.X,m_Graph.PrintableBounds.Y); // translate the painting to the printable area
				g.TranslateTransform(-_graphViewOffset.X, -_graphViewOffset.Y);
        _doc.DoPaint(g,bForPrinting);

       

      
      }
      catch(System.Exception ex)
      {
        g.PageUnit = GraphicsUnit.Point;
        g.PageScale=1;
      
        g.DrawString(ex.ToString(),
          new System.Drawing.Font("Arial",10),
          System.Drawing.Brushes.Black,
          _doc.PrintableBounds);

      
      }

    }


    #endregion // Methods

    #region Properties
    public event EventHandler CurrentGraphToolChanged;


    /// <summary>
    /// Get/sets the currently active GraphTool.
    /// </summary>
    public System.Type CurrentGraphToolType
    {
      get 
      {
        return _mouseState.GetType();
      }
      set
      {
        
        if(_mouseState==null || _mouseState.GetType() != value)
        {
          _mouseState = (MouseStateHandler)System.Activator.CreateInstance(value,new object[]{this});
        
          if(CurrentGraphToolChanged!=null)
            CurrentGraphToolChanged(this,EventArgs.Empty);
        }
      }
    }

   

    /// <summary>
    /// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
    /// </summary>
    public XYPlotLayerCollection Layers
    {
      get { return _doc.Layers; }
    }


    /// <summary>
    /// Returns the currently active layer, or null if there is no active layer.
    /// </summary>
    public XYPlotLayer ActiveLayer
    {
      get
      {
        return this._currentLayerNumber<0 ? null : _doc.Layers[this._currentLayerNumber]; 
      }     
    }

    /// <summary>
    /// check the validity of the CurrentLayerNumber and correct it
    /// </summary>
    public void EnsureValidityOfCurrentLayerNumber()
    {
      if(_doc.Layers.Count>0) // if at least one layer is present
      {
        if(_currentLayerNumber<0)
          CurrentLayerNumber=0;
        else if(_currentLayerNumber>=_doc.Layers.Count)
          CurrentLayerNumber=_doc.Layers.Count-1;
      }
      else // no layers present
      {
        if(-1!=_currentLayerNumber)
          CurrentLayerNumber=-1;
      }
    }

    /// <summary>
    /// Get / sets the currently active layer by number.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        if(value<0 && _doc.Layers.Count>0)
          throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be >=0 if there is at least one layer in the graph!");

        if(value>=_doc.Layers.Count)
          throw new ArgumentOutOfRangeException("CurrentLayerNumber",value,"Accepted values must be less than the number of layers in the graph(currently " + _doc.Layers.Count.ToString() + ")!");

        _currentLayerNumber = value<0 ? -1 : value;

        // if something changed
        if(oldValue!=this._currentLayerNumber)
        {
          // reflect the change in layer number in the layer tool bar
          if(null!=View)
            View.CurrentLayer = this._currentLayerNumber;

          // since the layer changed, also the plots changed, so the menu has
          // to reflect the new plots
          //this.UpdateDataPopup();
        }
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
      if(null!=ActiveLayer) // if the ActiveLayer exists
      {
        // if the XYColumnPlotData don't exist anymore, correct it
        if(ActiveLayer.PlotItems.Flattened.Length>0) // if at least one plotitem exists
        {
          if(_currentPlotNumber<0)
            CurrentPlotNumber=0;
          else if(_currentPlotNumber>ActiveLayer.PlotItems.Flattened.Length)
            CurrentPlotNumber = 0;
        }
        else
        {
          if(-1!=_currentPlotNumber)
            CurrentPlotNumber=-1;
        }
      }
      else // if no layer anymore
      {
        if(-1!=_currentPlotNumber)
          CurrentPlotNumber=-1;
      }
    }

    /// <summary>
    /// Get / sets the currently active plot by number.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentPlotNumber 
    {
      get 
      {
        return _currentPlotNumber;
      }
      set
      {
        if(CurrentLayerNumber>=0 && 0!=this._doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length && value<0)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to be greater or equal than zero");

        if(CurrentLayerNumber>=0 && value>=_doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber",value,"CurrentPlotNumber has to  be lesser than actual count: " + _doc.Layers[CurrentLayerNumber].PlotItems.Flattened.Length.ToString());

        _currentPlotNumber = value<0 ? -1 : value;

        //this.UpdateDataPopup();
      }
    }


    #endregion // Properties

    #region Editing selected objects


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

    /// <summary>
    /// Remove all selected objects of this graph.
    /// </summary>
    public void RemoveSelectedObjects()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).RemoveSelectedObjects();
    }

    /// <summary>
    /// Copy the selected objects of this graph to the clipboard.
    /// </summary>
    public void CopySelectedObjectsToClipboard()
    {
			var mouseStateOPM = _mouseState as ObjectPointerMouseHandler;
			int numberOfSelectedObjects = null == mouseStateOPM ? 0 : mouseStateOPM.NumberOfSelectedObjects;
			if (0 != numberOfSelectedObjects)
			{
				mouseStateOPM.CopySelectedObjectsToClipboard();
			}
			else // we copy the whole graph as xml
			{
				this.Doc.CopyToClipboardAsNative();
			}

    }

		



    /// <summary>
    /// Copy the selected objects of this graph to the clipboard.
    /// </summary>
    public void CutSelectedObjectsToClipboard()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).CutSelectedObjectsToClipboard();
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
        if (obj is ArrayList)
        {
          ArrayList list = (ArrayList)obj;
          foreach (object item in list)
          {
            if(item is GraphicBase)
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

    public void GroupSelectedObjects()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).GroupSelectedObjects();
    }

    public void SetSelectedObjectsProperty(IRoutedSetterProperty property)
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).SetSelectedObjectsProperty(property);
    }

    public void GetSelectedObjectsProperty(IRoutedGetterProperty property)
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).GetSelectedObjectsProperty(property);
    }

    #endregion

    #region Arrangement of selected objects

    /// <summary>
    /// Arranges the objects so that they share the top boundary with the top boundary of the last selected object.
    /// </summary>
    public void ArrangeTopToTop()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeTopToTop();
    }

    /// <summary>
    /// Arranges the objects so that they share the top boundary with the bottom boundary of the last selected object.
    /// </summary>
    public void ArrangeTopToBottom()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeTopToBottom();
    }

    /// <summary>
    /// Arranges the objects so that they share the bottom boundary with the bottom boundary of the last selected object.
    /// </summary>
    public void ArrangeBottomToTop()
    {
        if (_mouseState is ObjectPointerMouseHandler)
          ((ObjectPointerMouseHandler)_mouseState).ArrangeBottomToTop();
    }

    /// <summary>
    /// Arranges the objects so that they share the bottom boundary with the bottom boundary of the last selected object.
    /// </summary>
    public void ArrangeBottomToBottom()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeBottomToBottom();
    }
   

    /// <summary>
    /// Arranges the objects so that they share the left boundary with the left boundary of the last selected object.
    /// </summary>
    public void ArrangeLeftToLeft()
    {
      if (_mouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)_mouseState).ArrangeLeftToLeft();
    }

    /// <summary>
    /// Arranges the objects so that they share the left boundary with the right boundary of the last selected object.
    /// </summary>
    public void ArrangeLeftToRight()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeLeftToRight();
    }

    /// <summary>
    /// Arranges the objects so that they share the right boundary with the left boundary of the last selected object.
    /// </summary>
    public void ArrangeRightToLeft()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeRightToLeft();
    }

    /// <summary>
    /// Arranges the objects so that they share the right boundary with the right boundary of the last selected object.
    /// </summary>
    public void ArrangeRightToRight()
    {
      if (_mouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)_mouseState).ArrangeRightToRight();
    }

    /// <summary>
    /// Arranges the objects so they share the vertical middle line of the last selected object.
    /// </summary>
    public void ArrangeHorizontal()
    {
      if (_mouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)_mouseState).ArrangeHorizontal();
    }


    /// <summary>
    /// Arranges the objects so they share the horizontal middle line of the last selected object.
    /// </summary>
    public void ArrangeVertical()
    {
      if (_mouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)_mouseState).ArrangeVertical();
    }

    /// <summary>
    /// Arranges the objects so they their vertical middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeHorizontalTable()
    {
      if (_mouseState is ObjectPointerMouseHandler)
         ((ObjectPointerMouseHandler)_mouseState).ArrangeHorizontalTable();
    }

    /// <summary>
    /// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeVerticalTable()
    {
      if (_mouseState is ObjectPointerMouseHandler)
        ((ObjectPointerMouseHandler)_mouseState).ArrangeVerticalTable();
    }


    #endregion

    #region Scaling and Positioning

    /// <summary>
    /// Zoom value of the graph view.
    /// </summary>
    public float Zoom
    {
      get
      {
        return _zoomFactor;
      }
      set
      {
				float oldValue = _zoomFactor;

        if( value > 0.05 )
          _zoomFactor = value;
        else
          _zoomFactor = 0.05f;
      } 
    }

    /// <summary>
    /// Enables / disable the autozoom feature.
    /// </summary>
    /// <remarks>If autozoom is enables, the zoom factor is calculated depending on the size of the
    /// graph view so that the graph fits best possible inside the view.</remarks>
    public bool AutoZoom
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
    /// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
    /// </summary>
    public void RefreshGraph()
    {
      this._isCachedGraphImageDirty = true;
      
      if(null!=View) 
        _view.InvalidateGraph();
    }

    /// <summary>
    /// If the graph is cached, this causes an immediate redraw of the client area using the cached bitmap.
    /// If not cached, this simply invalidates the client area.
    /// </summary>
    public void RepaintGraphArea()
    {
      if(View==null)
        return;

      if(this._cachedGraphImage != null && !this._isCachedGraphImageDirty)
      {
        using(Graphics g = this.View.CreateGraphGraphics())
        {
          this.DoPaint(g,false);
        }
      }
      else
      {
        this.View.InvalidateGraph();
      }
        
    }

    /// <summary>
    /// Recalculates and sets the value of m_Zoom so the whole page is visible
    /// </summary>
    protected void RefreshAutoZoom()
    {
      CalculateAutoZoom();
			_view.ShowGraphScrollBars = false;
      RefreshGraph();
    }

		protected void RefreshManualZoom()
		{
			SizeF virtualGraphSize = PixelToPageCoordinates(_view.GraphSize);
			float xratio = virtualGraphSize.Width / _doc.Layers.GraphSize.Width;
			float yratio = virtualGraphSize.Height / _doc.Layers.GraphSize.Height;

			bool showScrollbars = xratio<1 || yratio<1;
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
				SizeF vz = GraphViewportSize;
				_graphViewOffset = new PointF((gz.Width-vz.Width)/2, (gz.Height-vz.Height)/2);
			}
		}


    /// <summary>
    /// Factor for horizontal conversion of page units (points=1/72 inch) to pixel.
    /// The resolution used for this is <see cref="m_HorizRes"/>.
    /// </summary>
    /// <returns>The factor described above.</returns>
    public float HorizFactorPageToPixel()
    {
      return this._horizontalResolution*this._zoomFactor/UnitPerInch;
    }

    /// <summary>
    /// Factor for vertical conversion of page units (points=1/72 inch) to pixel.
    /// The resolution used for this is <see cref="m_VertRes"/>.
    /// </summary>
    /// <returns>The factor described above.</returns>
    public float VertFactorPageToPixel()
    {
      return this._verticalResolution*this._zoomFactor/UnitPerInch;
    }

    /// <summary>
    /// Converts page coordinates (in points=1/72 inch) to pixel units. Uses the resolutions <see cref="m_HorizRes"/>
    /// and <see cref="m_VertRes"/> for calculation-
    /// </summary>
    /// <param name="pagec">The page coordinates to convert.</param>
    /// <returns>The coordinates as pixel coordinates.</returns>
    public PointF PageCoordinatesToPixel(PointF pagec)
    {
      return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
    }

    /// <summary>
    /// Converts pixel coordinates to page coordinates (in points=1/72 inch). Uses the resolutions <see cref="m_HorizRes"/>
    /// and <see cref="m_VertRes"/> for calculation-
    /// </summary>
    /// <param name="pixelc">The pixel coordinates to convert.</param>
    /// <returns>The coordinates as page coordinates (points=1/72 inch).</returns>
    public PointF PixelToPageCoordinates(PointF pixelc)
    {
      return new PointF(pixelc.X/HorizFactorPageToPixel(),pixelc.Y/VertFactorPageToPixel());
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
      return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
    }

    /// <summary>
    /// Converts x,y differences in pixels to the corresponding
    /// differences in page coordinates
    /// </summary>
    /// <param name="pixeldiff">X,Y differences in pixel units</param>
    /// <returns>X,Y differences in page coordinates</returns>
    public PointF PixelToPageDifferences(PointF pixeldiff)
    {
      return new PointF(pixeldiff.X/HorizFactorPageToPixel(),pixeldiff.Y/VertFactorPageToPixel());
    }

    /// <summary>
    /// converts from pixel to printable area coordinates
    /// </summary>
    /// <param name="pixelc">pixel coordinates as returned by MouseEvents</param>
    /// <returns>coordinates of the printable area in 1/72 inch</returns>
    public PointF PixelToPrintableAreaCoordinates(PointF pixelc)
    {
      PointF r = PixelToPageCoordinates(pixelc);
			r.X += _graphViewOffset.X;
			r.Y += _graphViewOffset.Y;
      return r;
    }

    /// <summary>
    /// converts printable area  to pixel coordinates
    /// </summary>
    /// <param name="printc">Printable area coordinates.</param>
    /// <returns>Pixel coordinates as returned by MouseEvents</returns>
    public PointF PrintableAreaToPixelCoordinates(PointF printc)
    {
			printc.X -= _graphViewOffset.X;
			printc.Y -= _graphViewOffset.Y;
      return PageToPixelCoordinates(printc);
    }

    /// <summary>
    /// This calculates the zoom factor using the size of the graph view, so that the page fits
    /// best into the view.
    /// </summary>
    /// <returns>The calculated zoom factor.</returns>
    protected virtual void CalculateAutoZoom()
    {
      float zoomh = (UnitPerInch*_view.GraphSize.Width/this._horizontalResolution)/(_doc.Layers.GraphSize.Width * _areaFillingFactor);
			float zoomv = (UnitPerInch * _view.GraphSize.Height / this._verticalResolution) / (_doc.Layers.GraphSize.Height * _areaFillingFactor);
      _zoomFactor = System.Math.Min(zoomh,zoomv);
			_graphViewOffset = GraphPaddingOffset;
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

      for(int nLayer=0;nLayer<Layers.Count;nLayer++)
      {
        XYPlotLayer layer = Layers[nLayer];
        foundObject = layer.HitTest(mousePT, plotItemsOnly);
        if(null!=foundObject)
        {
          foundInLayerNumber = nLayer;
          return true;
        }
      }
      foundObject=null;
      foundInLayerNumber=0;
      return false;
    }

  

    #region IWorkbenchContentController Members


    public void CloseView()
    {
      this.View = null;
    }

    public void CreateView()
    {
      if(View==null)
      {
        View = new GraphView();
      }
    }


    #endregion

    #region IMVCController
    /// <summary>
    /// Returns the view that shows the model.
    /// </summary>
    public object ViewObject
    {
      get { return View; }
      set { View = value as IGraphView; }
    }
    /// <summary>
    /// Returns the model (document) that this controller controls
    /// </summary>
    public object ModelObject 
    {
      get { return this.Doc; }
    }

    #endregion

	

  }
}
