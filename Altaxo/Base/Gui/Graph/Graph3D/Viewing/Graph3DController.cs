#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  using System.Collections;
  using System.Drawing;
  using Altaxo.Collections;
  using Altaxo.Drawing;
  using Altaxo.Drawing.D3D;
  using Altaxo.Geometry;
  using Altaxo.Graph;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.GraphicsContext;
  using Altaxo.Graph.Graph3D.GuiModels;
  using Altaxo.Graph.Graph3D.Plot;
  using Altaxo.Graph.Graph3D.Shapes;
  using Altaxo.Gui.Workbench;
  //using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
  using Altaxo.Main;
  using Altaxo.Serialization.Clipboard;
  using Graph.Graph3D;

  [UserControllerForObject(typeof(GraphDocument))]
  [UserControllerForObject(typeof(GraphViewOptions))]
  [ExpectedTypeOfView(typeof(IGraph3DView))]
  public class Graph3DController : AbstractViewContent, IDisposable, IMVCANController, IGraphController, IClipboardHandler
  {
    public event EventHandler CurrentGraphToolChanged;

    public IGraph3DView _view;

    protected GraphDocument _doc;

    public GraphDocument Doc { get { return _doc; } }

    /// <summary>Number of the currently selected layer (or null if no layer is present).</summary>
    protected IList<int> _currentLayerNumber = new List<int>();

    /// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
    protected int _currentPlotNumber = -1;

    private NGTreeNode _layerStructure;

    protected Altaxo.Main.TriggerBasedUpdate _triggerBasedUpdate;

    [NonSerialized]
    protected WeakEventHandler[] _weakEventHandlersForDoc;

    private IGraphicsContext3D _drawing;

    /// <summary>
    /// Additional geometry, that is not part of the graph, for instance the selection markers.
    /// </summary>
    private IGraphicsContext3D __markerGeometry;

    /// <summary>
    /// If true, markers are shown in each of the corners of the graph document.
    /// </summary>
    private RootLayerMarkersVisibility? _rootLayerMarkersVisibility;

    /// <summary>
    /// The root layer markers visibility that results from the settings here and the property context of the document
    /// </summary>
    private RootLayerMarkersVisibility _cachedResultingRootLayerMarkersVisibility;

    /// <summary>
    /// The camera as it was when the middle mouse button was pressed. A value != null indicates that the middle button is currently pressed.
    /// This value is used to rotate the camera when the middle mouse button is pressed and the mouse is moved.
    /// </summary>
    protected CameraBase _middleButtonPressed_InitialCamera;

    /// <summary>
    /// The position of the mouse as it was when the middle mouse button was pressed. This value is valid only if the field <see cref="_middleButtonPressed_InitialCamera"/> is not null.
    /// </summary>
    protected PointD3D _middleButtonPressed_InitialPosition;

    protected enum MiddelButtonAction { RotateCamera, MoveCamera, ZoomCamera }

    /// <summary>
    /// The action that is executed if the middle mouse button is pressed and the mouse is moved. This value is only valid if if the field <see cref="_middleButtonPressed_InitialCamera"/> is not null.
    /// </summary>
    protected MiddelButtonAction _middleButtonCurrentAction;

    protected static IList<IHitTestObject> _emptyReadOnlyList;

    #region Constructors

    static Graph3DController()
    {
      _emptyReadOnlyList = new List<IHitTestObject>().AsReadOnly();

      // register here editor methods
      XYPlotLayerController.RegisterEditHandlers();
      XYZPlotLayer.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
      TextGraphic.PlotItemEditorMethod = new DoubleClickHandler(EhEditPlotItem);
      TextGraphic.TextGraphicsEditorMethod = new DoubleClickHandler(EhEditTextGraphics);
    }

    public Graph3DController()
    {
      InitTriggerBasedUpdate();
    }


    /// <summary>
    /// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
    /// </summary>
    /// <param name="graphdoc">The graph which holds the graphical elements.</param>
    public Graph3DController(GraphDocument graphdoc)
    {
      if (null == graphdoc)
        throw new ArgumentNullException("Leaving the graphdoc null in constructor is not supported here");

      InitTriggerBasedUpdate();
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
      else if (args[0] is Altaxo.Graph.Graph3D.GuiModels.GraphViewOptions)
      {
        var o = (Altaxo.Graph.Graph3D.GuiModels.GraphViewOptions)args[0];
        _rootLayerMarkersVisibility = o.RootLayerMarkersVisibility;
        if (_doc == null)
        {
          InternalInitializeGraphDocument(o.GraphDocument);
        }
        else if (!object.ReferenceEquals(_doc, o.GraphDocument))
        {
          throw new InvalidProgramException("The already initialized document and the document in the option class are not identical");
        }
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
        _view.SetCamera(_doc.Camera, _doc.Lighting);

        InitLayerStructure();
        _view.SetLayerStructure(_layerStructure, CurrentLayerNumber.ToArray()); // tell the view how many layers we have

        // Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
        EhView_GraphPanelSizeChanged();

        if (null != _drawing)
        {
          _view.SetDrawing(_drawing);
          _view.TriggerRendering();
        }
        else
        {
          EhUpdateByTimerQueue();
        }
      }
    }

    /// <summary>
    /// Is called when the graph3d is no longer displayed. Used here to free resources
    /// </summary>
    public override void Dispose()
    {
      ViewObject = null;

      base.Dispose();
    }

    private void InitLayerStructure()
    {
      _layerStructure = Altaxo.Collections.TreeNodeExtensions.ProjectTreeToNewTree(
        _doc.RootLayer,
        new List<int>(),
        (sn, indices) => new NGTreeNode { Tag = indices.ToArray(), Text = HostLayer.GetDefaultNameOfLayer(indices) }, (parent, child) => parent.Nodes.Add(child));
    }

    private void InitTriggerBasedUpdate()
    {
      _triggerBasedUpdate = new Altaxo.Main.TriggerBasedUpdate(Current.TimerQueue)
      {
        MinimumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(0.02),
        MinimumWaitingTimeAfterLastTrigger = TimeSpanExtensions.FromSecondsAccurate(0.02),
        MaximumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(0.1)
      };
      _triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
      base.OnPropertyChanged(propertyName);

      if (propertyName == nameof(IsContentVisible))
        OnContentVisibilityChanged();
    }

    protected void OnContentVisibilityChanged()
    {
      _view?.AnnounceContentVisibilityChanged(IsContentVisible);
    }

    #region Properties

    /// <summary>
    /// Returns the layer collection. Is the same as m_GraphDocument.XYPlotLayer.
    /// </summary>
    public HostLayer RootLayer
    {
      get { return _doc.RootLayer; }
    }

    #endregion Properties

    private void InternalInitializeGraphDocument(GraphDocument doc)
    {
      if (_doc != null)
        throw new ApplicationException(nameof(_doc) + " is already initialized");
      if (doc == null)
        throw new ArgumentNullException(nameof(doc));

      _doc = doc;

      {
        // we are using weak events here, to avoid that _doc will maintain strong references to the controller
        // Attention: use local variable doc instead of member _doc for the anonymous methods below!
        var rootLayer = doc.RootLayer; // local variable for rootLayer
        _weakEventHandlersForDoc = new WeakEventHandler[3]; // storage for WeakEventhandlers for later removal
        doc.Changed += (_weakEventHandlersForDoc[0] = new WeakEventHandler(EhGraph_Changed, x => doc.Changed -= x));
        rootLayer.LayerCollectionChanged += (_weakEventHandlersForDoc[1] = new WeakEventHandler(EhGraph_LayerCollectionChanged, x => rootLayer.LayerCollectionChanged -= x));
        doc.SizeChanged += (_weakEventHandlersForDoc[2] = new WeakEventHandler(EhGraph_SizeChanged, x => doc.SizeChanged -= x));
      }
      // if the host layer has at least one child, we set the active layer to the first child of the host layer
      if (_doc.RootLayer.Layers.Count >= 1)
        _currentLayerNumber = new List<int>() { 0 };

      // Ensure the current layer and plot numbers are valid
      EnsureValidityOfCurrentLayerNumber();
      EnsureValidityOfCurrentPlotNumber();

      Title = _doc.Name;
    }

    private void InternalUninitializeGraphDocument()
    {
      // remove the weak event handlers from doc
      var wev = _weakEventHandlersForDoc;
      if (null != wev)
      {
        foreach (var ev in wev)
          ev.Remove();
        _weakEventHandlersForDoc = null;
      }

      _doc = null;
    }

    public override object ModelObject
    {
      get
      {
        return new Altaxo.Graph.Graph3D.GuiModels.GraphViewOptions(_doc, null);
      }
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }

      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as IGraph3DView;

        if (_view != null)
        {
          _view.Controller = this;
          Initialize(false);
        }
      }
    }

    public IGraph3DView View
    {
      get
      {
        return _view;
      }
    }

    /// <summary>
    /// Gets the selected objects. This property must be overriden in derived classes
    /// </summary>
    /// <value>
    /// The selected objects.
    /// </value>
    public virtual IList<IHitTestObject> SelectedObjects
    {
      get { return _view?.SelectedObjects ?? _emptyReadOnlyList; }
    }

    /// <summary>
    /// Returns the currently active layer. There is always an active layer.
    /// </summary>
    public HostLayer ActiveLayer
    {
      get
      {
        return _doc.RootLayer.ElementAt(_currentLayerNumber);
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
            _view.CurrentLayer = _currentLayerNumber.ToArray();
        }
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
        var layer = ActiveLayer as XYZPlotLayer;

        if (null != layer && 0 != layer.PlotItems.Flattened.Length && value < 0)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to be greater or equal than zero");

        if (null != layer && value >= layer.PlotItems.Flattened.Length)
          throw new ArgumentOutOfRangeException("CurrentPlotNumber", value, "CurrentPlotNumber has to  be lesser than actual count: " + layer.PlotItems.Flattened.Length.ToString());

        _currentPlotNumber = value < 0 ? -1 : value;
      }
    }

    /// <summary>
    /// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
    /// the current plot number is set to -1.
    /// </summary>
    public void EnsureValidityOfCurrentPlotNumber()
    {
      var layer = EnsureValidityOfCurrentLayerNumber() as XYZPlotLayer;

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

    /// <summary>
    /// Handles the selection of the current layer by the <b>user</b>.
    /// </summary>
    /// <param name="currLayer">The current layer number as selected by the user.</param>
    /// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
    public virtual void EhView_CurrentLayerChoosen(int[] currLayer, bool bAlternative)
    {
      var oldCurrLayer = CurrentLayerNumber;
      CurrentLayerNumber = new List<int>(currLayer);

      // if we have clicked the button already down then open the layer dialog
      if (null != ActiveLayer && System.Linq.Enumerable.SequenceEqual(_currentLayerNumber, oldCurrLayer) && false == bAlternative)
      {
        var activeLayer = ActiveLayer;
        if (activeLayer is XYZPlotLayer)
          XYPlotLayerController.ShowDialog((XYZPlotLayer)activeLayer);
        else
          HostLayerController.ShowDialog(activeLayer);
      }
    }

    public bool IsCmdCutEnabled()
    {
      return 0 != SelectedObjects.Count;
    }

    public void CutSelectedObjectsToClipboard()
    {
      var objectList = new ArrayList();
      var notSerialized = new List<IHitTestObject>();

      foreach (IHitTestObject o in SelectedObjects)
      {
        objectList.Add(o.HittedObject);
      }

      ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Graph3D.GraphObjectListAsXml", objectList);

      // Remove the not serialized objects from the selection, so they are not removed from the graph..
      foreach (IHitTestObject o in notSerialized)
        SelectedObjects.Remove(o);

      RemoveSelectedObjects();
    }

    public bool IsCmdCopyEnabled()
    {
      return true;
    }

    public void CopySelectedObjectsToClipboard()
    {
      if (0 == SelectedObjects.Count)
      {
        // we copy the whole graph as xml
        ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Graph3D.GraphDocumentAsXml", _doc);
      }
      else
      {
        var objectList = new ArrayList();

        foreach (IHitTestObject o in SelectedObjects)
        {
          objectList.Add(o.HittedObject);
        }
        ClipboardSerialization.PutObjectToClipboard("Altaxo.Graph.Graph3D.GraphObjectListAsXml", objectList);
      }
    }

    public bool IsCmdPasteEnabled()
    {
      return true;
    }

    public void PasteObjectsFromClipboard()
    {
      GraphDocument gd = Doc;
      var dao = Current.Gui.OpenClipboardDataObject();

      if (dao.GetDataPresent("Altaxo.Graph.Graph3D.GraphObjectListAsXml"))
      {
        object obj = ClipboardSerialization.GetObjectFromClipboard("Altaxo.Graph.Graph3D.GraphObjectListAsXml");
        if (obj is ICollection)
        {
          var list = (ICollection)obj;
          foreach (object item in list)
          {
            if (item is GraphicBase)
              ActiveLayer.GraphObjects.Add(item as GraphicBase);
            else if (item is Altaxo.Graph.Graph3D.Plot.IGPlotItem && ActiveLayer is XYZPlotLayer)
              ((XYZPlotLayer)ActiveLayer).PlotItems.Add((Altaxo.Graph.Graph3D.Plot.IGPlotItem)item);
          }
        }
        return;
      }
      if (dao.GetDataPresent("Altaxo.Graph.Graph3D.GraphDocumentAsXml"))
      {
        Doc.PasteFromClipboardAsGraphStyle(true);
        return;
      }
      if (dao.GetDataPresent("Altaxo.Graph.Graph3D.GraphLayerAsXml"))
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
              var size = ActiveLayer.Size / 2;

              PointD2D imgSize = img.GetImage().PhysicalDimension;

              double scale = Math.Min(size.X / imgSize.X, size.Y / imgSize.Y);
              imgSize = imgSize * scale;

              var item = new EmbeddedImageGraphic(PointD3D.Empty, new VectorD3D(imgSize.X, imgSize.Y, 0), img);
              ActiveLayer.GraphObjects.Add(item);
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
        var img = dao.GetData(typeof(System.Drawing.Imaging.Metafile)) as System.Drawing.Imaging.Metafile;
        if (img != null)
        {
          var size = (0.5 * ActiveLayer.Size).WithZ(0);
          var item = new EmbeddedImageGraphic(PointD3D.Empty, size, ImageProxy.FromImage(img));
          ActiveLayer.GraphObjects.Add(item);
          return;
        }
      }
      if (dao.ContainsImage())
      {
        Image img = dao.GetImage();
        if (img != null)
        {
          var size = 0.5 * ActiveLayer.Size.WithZ(0);
          var item = new EmbeddedImageGraphic(PointD3D.Empty, size, ImageProxy.FromImage(img));
          ActiveLayer.GraphObjects.Add(item);
          return;
        }
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
        Current.ProjectService.DeleteDocument(Doc, false);
      }
    }

    public void RemoveSelectedObjects()
    {
      if (null == SelectedObjects || SelectedObjects.Count == 0)
        return;

      using (var token = Doc.SuspendGetToken())
      {
        var removedObjects = new System.Collections.Generic.List<IHitTestObject>();

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
        }
      }
      // Redraw not neccessary since graph should trigger this by itself because some objects were removed
    }

    public bool Apply(bool disposeController)
    {
      return true;
    }

    public bool Revert(bool disposeController)
    {
      return true;
    }

    /// <summary>
    /// Make the views to look at the root layer center. The scale is choosen so that the size of the plot will be maximal.
    /// </summary>
    /// <param name="toEyeVector">The To-Eye vector (vector from the target to the camera position).</param>
    /// <param name="cameraUpVector">The camera up vector.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void ViewToRootLayerCenter(VectorD3D toEyeVector, VectorD3D cameraUpVector)
    {
      double aspectRatio = 1;
      if (null != _view)
      {
        var viewportSize = _view.ViewportSizeInPoints;
        aspectRatio = viewportSize.Y / viewportSize.X;
      }

      if ((VectorD3D.CrossProduct(toEyeVector, cameraUpVector).IsEmpty))
        throw new ArgumentOutOfRangeException(nameof(cameraUpVector) + " is either empty or is parallel to the eyeVector");

      var upVector = Math3D.GetNormalizedVectorOrthogonalToVector(cameraUpVector, toEyeVector);
      var targetPosition = (PointD3D)(0.5 * Doc.RootLayer.Size);
      var cameraDistance = 10 * Doc.RootLayer.Size.Length;
      var eyePosition = cameraDistance * toEyeVector.Normalized + targetPosition;
      var newCamera = Doc.Camera.WithUpEyeTarget(upVector, eyePosition, targetPosition);

      if (newCamera is OrthographicCamera)
      {
        var orthoCamera = (OrthographicCamera)newCamera.WithWidthAtZNear(1);

        var mx = orthoCamera.GetViewProjectionMatrix(aspectRatio);
        // to get the resulting scale, we transform all vertices of the root layer (the destination range would be -1..1, but now is not in range -1..1)
        // then we search for the maximum of the absulute value of x and y. This is our scale.
        double absmax = 0;
        foreach (var p in new RectangleD3D(Doc.RootLayer.Position, Doc.RootLayer.Size).Vertices)
        {
          var ps = mx.Transform(p);
          absmax = Math.Max(absmax, Math.Abs(ps.X));
          absmax = Math.Max(absmax, Math.Abs(ps.Y));
        }
        newCamera = orthoCamera.WithWidthAtZNear(absmax);
      }
      else if (newCamera is PerspectiveCamera)
      {
        var perspCamera = (PerspectiveCamera)newCamera;
        // use worst case
        var diameter = Doc.RootLayer.Size.Length; // diagonal of the root layer = radius of sphere in the worst case

        double minWidthHeight = Math.Min(perspCamera.WidthAtZNear, perspCamera.WidthAtZNear * aspectRatio);

        var distanceWorstCase = diameter * perspCamera.ZNear / minWidthHeight;

        if (distanceWorstCase > perspCamera.Distance)
          perspCamera = (PerspectiveCamera)perspCamera.WithDistanceByChangingEyePosition(distanceWorstCase);

        var rootRect = new RectangleD3D(PointD3D.Empty, Doc.RootLayer.Size);

        for (int i = 0; i < 5; ++i) // 5 iterations to get the right distance
        {
          var viewProj = perspCamera.GetViewProjectionMatrix(aspectRatio);
          var screenRect = RectangleD2D.NewRectangleIncludingAllPoints(rootRect.Vertices.Select(v => viewProj.Transform(v).PointD2DWithoutZ));

          double maxScreen = 0;
          foreach (var v in screenRect.Vertices)
          {
            maxScreen = Math.Max(maxScreen, Math.Abs(v.X));
            maxScreen = Math.Max(maxScreen, Math.Abs(v.Y));
          }

          // now we could decrease the camera distance by maxScreen, but this is only an estimation
          perspCamera = (PerspectiveCamera)perspCamera.WithDistanceByChangingEyePosition(perspCamera.Distance * maxScreen);
        }
        newCamera = perspCamera;
      }
      else
      {
        throw new NotImplementedException("Unknown camera type");
      }

      Doc.Camera = AdjustZNearZFar(newCamera);
    }

    public void ViewFront()
    {
      ViewToRootLayerCenter(new VectorD3D(0, -1, 0), new VectorD3D(0, 0, 1));
    }

    public void ViewRight()
    {
      ViewToRootLayerCenter(new VectorD3D(1, 0, 0), new VectorD3D(0, 0, 1));
    }

    public void ViewBack()
    {
      ViewToRootLayerCenter(new VectorD3D(0, -1, 0), new VectorD3D(0, 0, 1));
    }

    public void ViewLeft()
    {
      ViewToRootLayerCenter(new VectorD3D(-1, 0, 0), new VectorD3D(0, 0, 1));
    }

    public void ViewTop()
    {
      ViewToRootLayerCenter(new VectorD3D(0, 0, 1), new VectorD3D(0, 1, 0));
    }

    public void ViewBottom()
    {
      ViewToRootLayerCenter(new VectorD3D(0, 0, -1), new VectorD3D(0, -1, 0));
    }

    public void ViewIsometricStandard()
    {
      ViewToRootLayerCenter(new VectorD3D(-1, -1, 1), new VectorD3D(0, 0, 1));
    }

    public void ViewIsometricLeftTop()
    {
      ViewToRootLayerCenter(new VectorD3D(-1, -2, 1), new VectorD3D(0, 0, 1));
    }

    public void EhView_GraphPanelMouseWheel(double relX, double relY, double aspectRatio, int delta, bool isSHIFTpressed, bool isCTRLpressed, bool isALTpressed)
    {
      // MouseWheeling only: Zoom in/out
      // MouseWheeling + SHIFT key:	Move camera vertically
      // MouseWheeling + CTRL key: Move camera horizontally
      // MouseWheeling + SHIFT + ALT: Rotate camera around horizontal axis
      // MouseWheeling + SHIFT + CTRL: Rotate camera around vertical axis

      if (!isSHIFTpressed && !isCTRLpressed && !isALTpressed)
      {
        CameraZoomByMouseWheel(relX, relY, aspectRatio, delta);
      }
      else if (isSHIFTpressed && !isCTRLpressed && !isALTpressed) // Move camera vertically
      {
        CameraMoveVerticallyByMouseWheel(relX, relY, aspectRatio, delta);
      }
      else if (!isSHIFTpressed && isCTRLpressed && !isALTpressed) // Move camera horizontally
      {
        CameraMoveHorizontallyByMouseWheel(relX, relY, aspectRatio, delta);
      }
      else if (isSHIFTpressed && !isCTRLpressed && isALTpressed) // rotate camera around horizontal axis
      {
        CameraRotateAroundHorizontalAxisByMouseWheel(relX, relY, aspectRatio, delta);
      }
      else if (isSHIFTpressed && isCTRLpressed && !isALTpressed) // rotate camera around vertical axis
      {
        CameraRotateAroundVerticalAxisByMouseWheel(relX, relY, aspectRatio, delta);
      }
    }

    protected void CameraZoomByMouseWheel(double relX, double relY, double aspectRatio, double delta)
    {
      var camera = CameraZoomByMouseWheel(Doc.Camera, relX, relY, aspectRatio, delta / (4 * 120));
      camera = AdjustZNearZFar(camera);
      Doc.Camera = camera;
    }

    protected static CameraBase CameraZoomByMouseWheel(CameraBase camera, double relX, double relY, double aspectRatio, double delta)
    {
      if (camera is OrthographicCamera)
      {
        var cam = camera as OrthographicCamera;
        var eye = cam.TargetToEyeVectorNormalized;
        var up = cam.UpVectorPerpendicularToEyeVectorNormalized;
        var widthBefore = cam.WidthAtZNear;
        var widthAfter = widthBefore * Math.Pow(2, delta);

        var tam1h = relX - 0.5;
        var tbm1h = relY - 0.5;

        var shift = new VectorD3D(
          -(widthAfter - widthBefore) * (aspectRatio * tbm1h * up.X + eye.Z * tam1h * up.Y - eye.Y * tam1h * up.Z),
          (widthAfter - widthBefore) * (eye.Z * tam1h * up.X - aspectRatio * tbm1h * up.Y - eye.X * tam1h * up.Z),
          -(widthAfter - widthBefore) * (eye.Y * tam1h * up.X - eye.X * tam1h * up.Y + aspectRatio * tbm1h * up.Z)
          );

        var oldCamera = (OrthographicCamera)camera;
        var newCamera = ((OrthographicCamera)camera).WithEyeTargetWidth(oldCamera.EyePosition + shift, oldCamera.TargetPosition + shift, widthAfter);
        return newCamera;
      }
      else if (camera is PerspectiveCamera)
      {
        double rx = 2 * relX - 1;
        double ry = 2 * relY - 1;
        double distanceFactor = Math.Pow(2, delta);
        camera = ((PerspectiveCamera)camera).ZoomByGettingCloserToTarget(distanceFactor, rx, ry, aspectRatio);
      }

      return camera;
    }

    protected void CameraMoveHorizontallyByMouseWheel(double relX, double relY, double aspectRatio, int delta)
    {
      CameraMoveRelative(delta / 4800.0, 0);
    }

    protected void CameraMoveVerticallyByMouseWheel(double relX, double relY, double aspectRatio, int delta)
    {
      CameraMoveRelative(0, delta / 4800.0);
    }

    protected void CameraRotateAroundHorizontalAxisByMouseWheel(double relX, double relY, double aspectRatio, int delta)
    {
      CameraRotateDegrees(delta / 24.0, 0);
    }

    protected void CameraRotateAroundVerticalAxisByMouseWheel(double relX, double relY, double aspectRatio, int delta)
    {
      CameraRotateDegrees(0, delta / 24.0);
    }

    /// <summary>
    /// Rotates the camera.
    /// </summary>
    /// <param name="stepX">The rotation around the vertical axis in degrees.</param>
    /// <param name="stepY">The rotation around the horizontal axis in degrees.</param>
    public void CameraRotateDegrees(double stepX, double stepY)
    {
      Doc.Camera = CameraRotateDegrees(Doc.Camera, stepX, stepY);
    }

    /// <summary>
    /// Rotates the camera.
    /// </summary>
    /// <param name="cam">Initial camera, i.e. camera prior to the rotation</param>
    /// <param name="stepX">The rotation around the vertical axis in degrees.</param>
    /// <param name="stepY">The rotation around the horizontal axis in degrees.</param>
    public static CameraBase CameraRotateDegrees(CameraBase cam, double stepX, double stepY)
    {
      // the axis to turn the camera around is in case of stepY the Cross of UpVector and eyeVector
      // in case of stepX it is the cross of the Upvector and the cross of UpVector and eyeVector

      if (stepX != 0)
      {
        double angleRadian = stepX * Math.PI / 180.0;
        var axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(cam.TargetToEyeVector, VectorD3D.CrossProduct(cam.TargetToEyeVector, cam.UpVector)));
        var matrix = Matrix4x3.NewRotationFromAxisAndAngleRadian(axis, angleRadian, cam.TargetPosition);

        var newEye = matrix.Transform(cam.EyePosition);
        var newUp = matrix.Transform(cam.UpVector);
        cam = cam.WithUpEye(newUp, newEye);
      }

      if (stepY != 0)
      {
        double angleRadian = stepY * Math.PI / 180.0;
        var axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(cam.TargetToEyeVectorNormalized, cam.UpVector));
        var matrix = Matrix4x3.NewRotationFromAxisAndAngleRadian(axis, angleRadian, cam.TargetPosition);

        var newEye = matrix.Transform(cam.EyePosition);
        var newUp = matrix.Transform(cam.UpVector);
        cam = cam.WithUpEye(newUp, newEye);
      }

      return cam;
    }

    public static CameraBase ModelRotateDegrees(CameraBase cam, VectorD3D rootLayerSize, double stepX, double stepY)
    {
      double angleRadianZ = stepX * Math.PI / 180.0;
      double angleRadianX = stepY * Math.PI / 180.0;
      var shift = 0.5 * rootLayerSize;

      var l = cam.LookAtRHMatrix;

      // in the world coordinate space: i) translate to root layer center, ii) rotate around z-axis and iii) translate back
      var m01 = Matrix4x3.NewTranslation(-shift);
      var m02 = Matrix4x3.NewRotationFromAxisAndAngleRadian(new VectorD3D(0, 0, 1), angleRadianZ, PointD3D.Empty);
      var m03 = Matrix4x3.NewTranslation(shift);
      var mstart = m01.WithAppendedTransformation(m02).WithAppendedTransformation(m03);

      // in LookAt coordinate space: i) translate to root layer center, ii) rotate around x-axis and iii) translate back
      var diff11 = (VectorD3D)cam.LookAtRHMatrix.Transform((PointD3D)shift);
      var m11 = Matrix4x3.NewTranslation(-diff11);
      var m12 = Matrix4x3.NewRotationFromAxisAndAngleRadian(new VectorD3D(1, 0, 0), angleRadianX, PointD3D.Empty);
      var m13 = Matrix4x3.NewTranslation(diff11);
      var mend = m11.WithAppendedTransformation(m12).WithAppendedTransformation(m13);

      var t = mstart.WithAppendedTransformation(l).WithAppendedTransformation(mend);
      return cam.WithLookAtRHMatrix(t);
    }

    /// <summary>
    /// Adjusts the zNear and zFar parameter of the camera to make sure that our scene is viewed appropriately, and nothing is cut away.
    /// </summary>
    /// <param name="cam">The cam.</param>
    /// <returns></returns>
    public CameraBase AdjustZNearZFar(CameraBase cam)
    {
      var currentDistanceToRootLayerCenter = ((VectorD3D)(cam.EyePosition - 0.5 * Doc.RootLayer.Size)).Length;
      var rootLayerRadius = 0.5 * Doc.RootLayer.Size.Length;

      double zNear, zFar;
      double rootLayerRadiusTimesFour = 4 * rootLayerRadius;
      if (currentDistanceToRootLayerCenter <= 2 * rootLayerRadius)
      {
        zNear = rootLayerRadius / 100;
        zFar = rootLayerRadiusTimesFour;
      }
      else
      {
        zNear = rootLayerRadius / 2;
        zFar = Math.Max(2 * currentDistanceToRootLayerCenter, rootLayerRadius * 4);
        zFar = rootLayerRadiusTimesFour * Math.Ceiling(zFar / rootLayerRadiusTimesFour);
      }

      return cam.WithZNearZFarWithoutChangingViewAngle(zNear, zFar);
    }

    /// <summary>
    /// Moves the camera horizontally and vertically.
    /// </summary>
    /// <param name="stepX">The movement in horizontal direction. A value of 1 means movement corresponding to the full width of the scene.</param>
    /// <param name="stepY">The movement in horizontal direction. A value of 1 means movement corresponding to the full height of the scene.</param>
    public void CameraMoveRelative(double stepX, double stepY)
    {
      Doc.Camera = CameraMoveRelative(Doc.Camera, stepX, stepY);
    }

    /// <summary>
    /// Moves the camera horizontally and vertically.
    /// </summary>
    /// <param name="camera">The camera prior to the movement.</param>
    /// <param name="stepX">The movement in horizontal direction. A value of 1 means movement corresponding to the full width of the scene.</param>
    /// <param name="stepY">The movement in horizontal direction. A value of 1 means movement corresponding to the full height of the scene.</param>
    /// <returns>The new camera after the movement.</returns>
    public static CameraBase CameraMoveRelative(CameraBase camera, double stepX, double stepY)
    {
      var xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(camera.TargetToEyeVectorNormalized, camera.UpVector));
      var yaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(camera.TargetToEyeVector, VectorD3D.CrossProduct(camera.TargetToEyeVector, camera.UpVector)));

      if (camera is OrthographicCamera)
      {
        var oldCamera = (OrthographicCamera)camera;
        var shift = (xaxis * stepX + yaxis * stepY) * (oldCamera.WidthAtZNear);
        camera = oldCamera.WithEyeTarget(oldCamera.EyePosition + shift, oldCamera.TargetPosition + shift);
      }
      else if (camera is PerspectiveCamera)
      {
        var oldCamera = (PerspectiveCamera)camera;
        var shift = (xaxis * stepX + yaxis * stepY) * (oldCamera.WidthAtZNear * oldCamera.Distance / oldCamera.ZNear);
        camera = oldCamera.WithEyeTarget(oldCamera.EyePosition + shift, oldCamera.TargetPosition + shift);
      }

      return camera;
    }

    /// <summary>
    /// Looks for a graph object at the position given by <paramref name="hitData"/> and returns true if one is found.
    /// </summary>
    /// <param name="hitData">The position of the mouse, expressed as transformation, that when applied, transformes the mouse coordinate to the point x=0, y=0, z=-Infinity....+Infinity.</param>
    /// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
    /// <param name="foundObject">Found object if there is one found, else null</param>
    /// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
    /// <returns>True if a object was found at the position given by <paramref name="hitData"/>, else false.</returns>
    public bool FindGraphObjectAtPixelPosition(HitTestPointData hitData, bool plotItemsOnly, out IHitTestObject foundObject, out int[] foundInLayerNumber)
    {
      foundObject = Doc.RootLayer.HitTest(hitData, plotItemsOnly);

      if (null != foundObject && null != foundObject.ParentLayer)
      {
        foundInLayerNumber = foundObject.ParentLayer.IndexOf().ToArray();
        return true;
      }

      foundObject = null;
      foundInLayerNumber = null;
      return false;
    }

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
        Current.Dispatcher.InvokeIfRequired(EhGraphDocumentNameChanged_Unsynchronized, (GraphDocument)sender, eAsNOC.OldName);
        return;
      }
      else if (e is CameraChangedEventArgs) // Only the camera has changed, there is no need to rebuild the geometry
      {
        _view.SetCamera(_doc.Camera, _doc.Lighting);
        _view?.TriggerRendering();
      }
      else // everything else need to rebuild the geometry
      {
        _triggerBasedUpdate.Trigger();
      }
    }

    /// <summary>
    /// Called when the timer has elapsed.
    /// </summary>
    private void EhUpdateByTimerQueue()
    {
      // if something changed on the graph, make sure that the layer and plot number reflect this change
      EnsureValidityOfCurrentLayerNumber();
      EnsureValidityOfCurrentPlotNumber();

      if (null != _view)
      {
        var newDrawing = _view.GetGraphicContext();
        _doc.Paint(newDrawing);

        var propertyContextOfDocument = Altaxo.PropertyExtensions.GetPropertyHierarchy(_doc);
        var sceneBackColor = propertyContextOfDocument.GetValue(GraphDocument.PropertyKeyDefaultSceneBackColor, NamedColors.White);
        if (!propertyContextOfDocument.TryGetValue(GraphDocument.PropertyKeyRootLayerMarkersVisibility, out var rootLayerMarkersVisibility))
        {
          if (_rootLayerMarkersVisibility.HasValue)
            rootLayerMarkersVisibility = _rootLayerMarkersVisibility.Value;
          else
            rootLayerMarkersVisibility = RootLayerMarkersVisibility.LinesWithArrows;
        }
        _cachedResultingRootLayerMarkersVisibility = rootLayerMarkersVisibility;

        var oldDrawing = _drawing;
        _drawing = newDrawing;
        _view.SetSceneBackColor(sceneBackColor);
        _view.SetDrawing(_drawing);
        _view.SetCamera(_doc.Camera, _doc.Lighting);

        var markerGeometry = _view.GetGraphicContextForMarkers();
        DrawRootLayerMarkers(markerGeometry);
        _view.SetMarkerGeometry(markerGeometry);

        _view.TriggerRendering();

        (oldDrawing as IDisposable)?.Dispose();
      }
    }

    /// <summary>
    /// Handler of the event LayerCollectionChanged of the graph document. Forces to
    /// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
    /// <param name="sender">The sender of the event (the GraphDocument).</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraph_SizeChanged(object sender, System.EventArgs e)
    {
      Current.Dispatcher.InvokeAndForget(EhGraph_BoundsChanged_Unsynchronized);
    }

    protected void EhGraph_BoundsChanged_Unsynchronized()
    {
      var view = _view;
      if (null != view)
      {
        var g = view.GetGraphicContextForMarkers();
        DrawRootLayerMarkers(g);
        view.SetMarkerGeometry(g);
      }
    }

    /// <summary>
    /// Handler of the event LayerCollectionChanged of the graph document. Forces to
    /// check the LayerButtonBar to keep track that the number of buttons match the number of layers.</summary>
    /// <param name="sender">The sender of the event (the GraphDocument).</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraph_LayerCollectionChanged(object sender, System.EventArgs e)
    {
      Current.Dispatcher.InvokeAndForget(EhGraph_LayerCollectionChanged_Unsynchronized);
    }

    protected void EhGraph_LayerCollectionChanged_Unsynchronized()
    {
      var oldActiveLayer = new List<int>(_currentLayerNumber);

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
      Title = Doc.Name;
    }

    /// <summary>
    /// Handles the double click event onto a plot item.
    /// </summary>
    /// <param name="hit">Object containing information about the double clicked object.</param>
    /// <returns>True if the object should be deleted, false otherwise.</returns>
    protected static bool EhEditPlotItem(IHitTestObject hit)
    {
      var actLayer = hit.ParentLayer as XYZPlotLayer;
      var pa = (IGPlotItem)hit.HittedObject;

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
      var layer = hit.ParentLayer;
      var tg = (TextGraphic)hit.HittedObject;

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
            tg.ParentObject.EhChildChanged(tg, EventArgs.Empty);
        }
      }

      return shouldDeleted;
    }

    /// <summary>
    /// Handles the event when the size of the graph area is changed.
    /// </summary>
    public virtual void EhView_GraphPanelSizeChanged()
    {
      /*
			if (_isAutoZoomActive)
			{
				RefreshAutoZoom(false);
			}
			else
			{
				if (null != _view && 0 != _view.ViewportSizeInPoints.X && 0 != _view.ViewportSizeInPoints.Y)
					SetViewsScrollbarParameter();
			}
			*/

      _view?.TriggerRendering();
    }

    /// <summary>
    /// The controller should show a data context menu (contains all plots of the currentLayer).
    /// </summary>
    /// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
    /// <param name="parent">The parent control which is the parent of the context menu.</param>
    /// <param name="pt">The location where the context menu should be shown.</param>
    public virtual void EhView_ShowDataContextMenu(int[] currLayer, object parent, PointD2D pt)
    {
      int oldCurrLayer = ActiveLayer.Number;

      CurrentLayerNumber = new List<int>(currLayer);

      if (null != ActiveLayer)
      {
        Current.Gui.ShowContextMenu(parent, parent, "/Altaxo/Views/Graph3D/LayerButton/ContextMenu", pt.X, pt.Y);
      }
    }

    public virtual void EhView_CurrentGraphToolChanged()
    {
      if (null != CurrentGraphToolChanged)
        CurrentGraphToolChanged(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the mouse down event onto the graph in the controller class.
    /// </summary>
    /// <param name="position">Mouse position. X and Y components are the current relative mouse coordinates, the Z component is the screen's aspect ratio.</param>
    /// <param name="e">MouseEventArgs.</param>
    public virtual void EhView_GraphPanelMouseDown(PointD3D position, AltaxoMouseEventArgs e, AltaxoKeyboardModifierKeys modifierKeys)
    {
      bool isSHIFTpressed = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Shift);
      bool isCTRLpressed = modifierKeys.HasFlag(AltaxoKeyboardModifierKeys.Control);
      if (e.Button == AltaxoMouseButtons.Middle)
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
    public virtual void EhView_GraphPanelMouseUp(PointD3D position, AltaxoMouseEventArgs e)
    {
      if (e.Button == AltaxoMouseButtons.Middle)
      {
        _middleButtonPressed_InitialCamera = null;
      }
    }

    /// <summary>
    /// Handles the mouse move event onto the graph in the controller class.
    /// </summary>
    /// <param name="position">Mouse position.</param>
    /// <param name="e">MouseEventArgs.</param>
    public virtual void EhView_GraphPanelMouseMove(PointD3D position, AltaxoMouseButtons mouseButtonState)
    {
      if (!(mouseButtonState.HasFlag(AltaxoMouseButtons.Middle))) // if middle button is released
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

    public GraphToolType CurrentGraphTool
    {
      get
      {
        return _view?.CurrentGraphTool ?? GraphToolType.None;
      }
      set
      {
        if (null != _view)
        {
          _view.CurrentGraphTool = value;
        }
      }
    }

    #endregion Event handlers from GraphDocument

    #region Drawing markers

    private void DrawMarkerX(IPositionColorIndexedTriangleBuffer buf, PointD3D pos, double markerLenBy2, double markerThicknessBy2)
    {
      float r = 0, g = 0, b = 0;

      // green marker in x-direction
      var voffs = buf.VertexCount;
      r = 1;
      buf.AddTriangleVertex(pos.X + markerLenBy2, pos.Y + 0, pos.Z + 0, r, g, b, 1); // Point
      buf.AddTriangleVertex(pos.X - markerLenBy2, pos.Y + markerThicknessBy2, pos.Z + markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerLenBy2, pos.Y - markerThicknessBy2, pos.Z + markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerLenBy2, pos.Y - markerThicknessBy2, pos.Z - markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerLenBy2, pos.Y + markerThicknessBy2, pos.Z - markerThicknessBy2, r, g, b, 1);
      r = 0;

      buf.AddTriangleIndices(0 + voffs, 1 + voffs, 2 + voffs);
      buf.AddTriangleIndices(0 + voffs, 2 + voffs, 3 + voffs);
      buf.AddTriangleIndices(0 + voffs, 3 + voffs, 4 + voffs);
      buf.AddTriangleIndices(0 + voffs, 4 + voffs, 1 + voffs);
      buf.AddTriangleIndices(4 + voffs, 3 + voffs, 2 + voffs);
      buf.AddTriangleIndices(4 + voffs, 2 + voffs, 1 + voffs);
    }

    private void DrawMarkerY(IPositionColorIndexedTriangleBuffer buf, PointD3D pos, double markerLenBy2, double markerThicknessBy2)
    {
      float r = 0, g = 0, b = 0;

      // green marker in x-direction
      var voffs = buf.VertexCount;
      g = 1;
      buf.AddTriangleVertex(pos.X + 0, pos.Y + markerLenBy2, pos.Z + 0, r, g, b, 1); // Point
      buf.AddTriangleVertex(pos.X + markerThicknessBy2, pos.Y - markerLenBy2, pos.Z + markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X + markerThicknessBy2, pos.Y - markerLenBy2, pos.Z - markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerThicknessBy2, pos.Y - markerLenBy2, pos.Z - markerThicknessBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerThicknessBy2, pos.Y - markerLenBy2, pos.Z + markerThicknessBy2, r, g, b, 1);
      g = 0;

      buf.AddTriangleIndices(0 + voffs, 1 + voffs, 2 + voffs);
      buf.AddTriangleIndices(0 + voffs, 2 + voffs, 3 + voffs);
      buf.AddTriangleIndices(0 + voffs, 3 + voffs, 4 + voffs);
      buf.AddTriangleIndices(0 + voffs, 4 + voffs, 1 + voffs);
      buf.AddTriangleIndices(4 + voffs, 3 + voffs, 2 + voffs);
      buf.AddTriangleIndices(4 + voffs, 2 + voffs, 1 + voffs);
    }

    private void DrawMarkerZ(IPositionColorIndexedTriangleBuffer buf, PointD3D pos, double markerLenBy2, double markerThicknessBy2)
    {
      float r = 0, g = 0, b = 0;

      // green marker in x-direction
      var voffs = buf.VertexCount;
      b = 1;
      buf.AddTriangleVertex(pos.X + 0, pos.Y + 0, pos.Z + markerLenBy2, r, g, b, 1); // Point
      buf.AddTriangleVertex(pos.X + markerThicknessBy2, pos.Y + markerThicknessBy2, pos.Z - markerLenBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerThicknessBy2, pos.Y + markerThicknessBy2, pos.Z - markerLenBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X - markerThicknessBy2, pos.Y - markerThicknessBy2, pos.Z - markerLenBy2, r, g, b, 1);
      buf.AddTriangleVertex(pos.X + markerThicknessBy2, pos.Y - markerThicknessBy2, pos.Z - markerLenBy2, r, g, b, 1);
      b = 0;

      buf.AddTriangleIndices(0 + voffs, 1 + voffs, 2 + voffs);
      buf.AddTriangleIndices(0 + voffs, 2 + voffs, 3 + voffs);
      buf.AddTriangleIndices(0 + voffs, 3 + voffs, 4 + voffs);
      buf.AddTriangleIndices(0 + voffs, 4 + voffs, 1 + voffs);
      buf.AddTriangleIndices(4 + voffs, 3 + voffs, 2 + voffs);
      buf.AddTriangleIndices(4 + voffs, 2 + voffs, 1 + voffs);
    }

    public void DrawRootLayerMarkers(IOverlayContext3D gc)
    {
      var buf = gc.PositionColorIndexedTriangleBuffers;

      VectorD3D size = Doc.RootLayer.Size;

      double markerLen = Math.Max(size.X, Math.Max(size.Y, size.Z)) / 20.0;
      double markerLenBy2 = markerLen / 2;
      double markerThicknessBy2 = markerLenBy2 / 5.0;

      var rect = new RectangleD3D(PointD3D.Empty, size);

      if (_cachedResultingRootLayerMarkersVisibility.HasFlag(RootLayerMarkersVisibility.Arrows))
      {
        foreach (var pos in rect.Vertices)
        {
          var posX = pos.WithXPlus(pos.X == 0 ? markerLenBy2 : -markerLenBy2);
          DrawMarkerX(buf, posX, markerLenBy2, markerThicknessBy2);

          var posY = pos.WithYPlus(pos.Y == 0 ? markerLenBy2 : -markerLenBy2);
          DrawMarkerY(buf, posY, markerLenBy2, markerThicknessBy2);

          var posZ = pos.WithZPlus(pos.Z == 0 ? markerLenBy2 : -markerLenBy2);
          DrawMarkerZ(buf, posZ, markerLenBy2, markerThicknessBy2);
        }
      }

      if (_cachedResultingRootLayerMarkersVisibility.HasFlag(RootLayerMarkersVisibility.Lines))
      {
        var lineBuffer = gc.PositionColorLineListBuffer;
        foreach (var line in rect.Edges)
        {
          lineBuffer.AddLine(line.P0.X, line.P0.Y, line.P0.Z, line.P1.X, line.P1.Y, line.P1.Z, 1, 0, 0, 1);
        }
      }
    }

    #endregion Drawing markers

    #region Object arrangement

    /// <summary>
    /// Determines the parent layer of the selected objects, as far as all selected objects belong to the same layer.
    /// </summary>
    /// <returns>The layer all selected objects belong to. If the selected objects belong to different layers, the return value is null.</returns>
    public HostLayer GetParentLayerOfSelectedObjects()
    {
      HostLayer layer4all = null;
      foreach (IHitTestObject o in SelectedObjects)
      {
        var layer = AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>((IDocumentLeafNode)o.HittedObject);
        if (layer == null)
          continue;

        if (layer4all == null)
          layer4all = layer;
        else if (!object.ReferenceEquals(layer, layer4all))
          return null;
      }

      return layer4all;
    }

    /// <summary>
    /// Gets the principal coordinate system that results of the camera facing a layer. The plane of the layer that best faced the camera is used for the calculations.
    /// The normal of that layer is returned as z-axis, the vector that best matches the up-vector of the camera is becoming the y-axis,
    /// and the x-axis results from the z-axis and the y-axis.
    /// </summary>
    /// <param name="camera">The camera.</param>
    /// <param name="activeLayer">The active layer of the graph document.</param>
    /// <param name="transformation">Matrix that contains the principal axes as described above. The axes coordinates are in the coordinates of the layer provided in the argument <paramref name="activeLayer"/>.</param>
    /// <exception cref="InvalidProgramException">There should always be a plane of a rectangle that can be hit!</exception>
    public static void GetCoordinateSystemBasedOnLayerPlaneFacingTheCamera(CameraBase camera, HostLayer activeLayer, out Matrix3x3 transformation)
    {
      var hitposition = new PointD3D(0.5, 0.5, 1); // this hit position is arbitrary, every other position should work similarly

      var activeLayerTransformation = activeLayer.TransformationFromRootToHere();

      var hitData = new HitTestPointData(camera.GetHitRayMatrix(hitposition));

      hitData = hitData.NewFromAdditionalTransformation(activeLayerTransformation); // now hitdata are in layer cos

      var targetToEye = hitData.WorldTransformation.Transform(camera.TargetToEyeVectorNormalized); // targetToEye in layer coordinates
      var upEye = hitData.WorldTransformation.Transform(camera.UpVectorPerpendicularToEyeVectorNormalized); // camera up vector in layer coordinates

      // get the face which has the best dot product between the eye vector of the camera and the plane's normal
      var layerRect = new RectangleD3D(PointD3D.Empty, activeLayer.Size);
      double maxval = double.MinValue;
      PlaneD3D maxPlane = PlaneD3D.Empty;
      foreach (var plane in layerRect.Planes)
      {
        double val = VectorD3D.DotProduct(plane.Normal, targetToEye);
        if (val > maxval)
        {
          maxval = val;
          maxPlane = plane;
        }
      }

      //	bool isHit = hitData.IsPlaneHitByRay(maxPlane, out hitPointOnPlaneInActiveLayerCoordinates); // hitPointOnPlane is in layer coordinates too

      //	if (!isHit)
      //	throw new InvalidProgramException("There should always be a plane of a rectangle that can be hit!");

      VectorD3D zaxis = maxPlane.Normal;
      VectorD3D yaxis = upEye;

      // Find y axis perpendicular to zaxis
      maxval = double.MinValue;
      foreach (var plane in layerRect.Planes)
      {
        double val = VectorD3D.DotProduct(plane.Normal, upEye);
        if (val > maxval && 0 == VectorD3D.DotProduct(plane.Normal, zaxis))
        {
          maxval = val;
          yaxis = plane.Normal;
        }
      }
      var xaxis = VectorD3D.CrossProduct(yaxis, zaxis);

      // now we have all information about the spatial position and orientation of the text:
      // hitPointOnPlane is the position of the text
      // maxPlane.Normal is the face orientation of the text
      // maxUpVector is the up orientation of the text

      xaxis = xaxis.Normalized;
      yaxis = yaxis.Normalized;
      zaxis = zaxis.Normalized;

      transformation = new Matrix3x3(
        xaxis.X, yaxis.X, zaxis.X,
        xaxis.Y, yaxis.Y, zaxis.Y,
        xaxis.Z, yaxis.Z, zaxis.Z
        );
    }

    /// <summary>
    /// Gets the principal coordinate system that results of the camera facing a layer.
    /// The layer is determined from the base layer of all currently selected objects.
    /// The plane of the layer that best faced the camera is used for the calculations.
    /// The normal of that layer is returned as z-axis, the vector that best matches the up-vector of the camera is becoming the y-axis,
    /// and the x-axis results from the z-axis and the y-axis. These vectors are then transformed to root layer coordinates,
    /// and packed into a matrix with M11, M21, M31 being the x-axis, M12, M22, M32 being the y-axis and M31, M32, M33 being the z-axis.
    /// </summary>
    /// <returns>Matrix with the 3 principal vectors, see above for explanation.</returns>
    public Matrix3x3 GetCoordinateSystemRlcBasedOnLayerPlaneFacingTheCameraForSelectedObjects()
    {
      var layer = GetParentLayerOfSelectedObjects() ?? _doc.RootLayer;
      GetCoordinateSystemBasedOnLayerPlaneFacingTheCamera(_doc.Camera, layer, out var principalMoveVectorsInLayerCoordinates); // transformation is in layer coordinates
      var transformation = layer.TransformationFromHereToRoot();
      var principalVector1_RLC = transformation.Transform(new VectorD3D(principalMoveVectorsInLayerCoordinates.M11, principalMoveVectorsInLayerCoordinates.M21, principalMoveVectorsInLayerCoordinates.M31)).Normalized;
      var principalVector2_RLC = transformation.Transform(new VectorD3D(principalMoveVectorsInLayerCoordinates.M12, principalMoveVectorsInLayerCoordinates.M22, principalMoveVectorsInLayerCoordinates.M32)).Normalized;
      var principalVector3_RLC = transformation.Transform(new VectorD3D(principalMoveVectorsInLayerCoordinates.M13, principalMoveVectorsInLayerCoordinates.M23, principalMoveVectorsInLayerCoordinates.M33)).Normalized;
      return new Matrix3x3(
        principalVector1_RLC.X, principalVector2_RLC.X, principalVector3_RLC.X,
        principalVector1_RLC.Y, principalVector2_RLC.Y, principalVector3_RLC.Y,
        principalVector1_RLC.Z, principalVector2_RLC.Z, principalVector3_RLC.Z);
    }

    /// <summary>
    /// Arranges the objects so they share the top boundary with the top boundary of the master element.
    /// </summary>
    public void ArrangeTopToTop()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(0, tmasterbounds.YPlusSizeY - tbounds.YPlusSizeY, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the bottom boundary with the top boundary of the master element.
    /// </summary>
    public void ArrangeBottomToTop()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(0, tmasterbounds.YPlusSizeY - tbounds.Y, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the top boundary with the bottom boundary of the master element.
    /// </summary>
    public void ArrangeTopToBottom()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(0, tmasterbounds.Y - tbounds.YPlusSizeY, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the bottom boundary with the bottom boundary of the master element.
    /// </summary>
    public void ArrangeBottomToBottom()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(0, tmasterbounds.Y - tbounds.Y, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the left boundary with the left boundary of the master element.
    /// </summary>
    public void ArrangeLeftToLeft()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(tmasterbounds.X - tbounds.X, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the left boundary with the right boundary of the master element.
    /// </summary>
    public void ArrangeLeftToRight()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(tmasterbounds.XPlusSizeX - tbounds.X, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the right boundary with the left boundary of the master element.
    /// </summary>
    public void ArrangeRightToLeft()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(tmasterbounds.X - tbounds.XPlusSizeX, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the right boundary with the right boundary of the master element.
    /// </summary>
    public void ArrangeRightToRight()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(tmasterbounds.XPlusSizeX - tbounds.XPlusSizeX, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they share the horizontal middle line of the last selected object.
    /// </summary>
    public void ArrangeVertical()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(0, tmasterbounds.YCenter - tbounds.YCenter, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
    );
    }

    /// <summary>
    /// Arranges the objects so they share the vertical middle line of the last selected object.
    /// </summary>
    public void ArrangeHorizontal()
    {
      Arrange(delegate (IHitTestObject obj, RectangleD3D tbounds, RectangleD3D tmasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates)
      {
        var vu = new VectorD3D(tmasterbounds.XCenter - tbounds.XCenter, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
      );
    }

    /// <summary>
    /// Arranges the objects so they their vertical middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeHorizontalTable()
    {
      if (SelectedObjects.Count < 3)
        return;

      var principalMoveVectorsInRootLayerCoordinates = GetCoordinateSystemRlcBasedOnLayerPlaneFacingTheCameraForSelectedObjects();

      var firstbound = SelectedObjects[0].ObjectOutlineForArrangements;
      var tfirstbound = firstbound.GetBounds(principalMoveVectorsInRootLayerCoordinates);

      var lastbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements;
      var tlastbound = lastbound.GetBounds(principalMoveVectorsInRootLayerCoordinates);

      var step = (tlastbound.XCenter) - (tfirstbound.XCenter);
      step /= (SelectedObjects.Count - 1);

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = SelectedObjects.Count - 2; i > 0; i--)
      {
        IHitTestObject obj = SelectedObjects[i];
        var bounds = obj.ObjectOutlineForArrangements;
        var tbounds = bounds.GetBounds(principalMoveVectorsInRootLayerCoordinates);

        var vu = new VectorD3D(tfirstbound.XCenter + i * step - tbounds.XCenter, 0, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
    }

    /// <summary>
    /// Arranges the objects so they their horizontal middle line is uniform spaced between the first and the last selected object.
    /// </summary>
    public void ArrangeVerticalTable()
    {
      if (SelectedObjects.Count < 3)
        return;

      var principalMoveVectorsInRootLayerCoordinates = GetCoordinateSystemRlcBasedOnLayerPlaneFacingTheCameraForSelectedObjects();

      var firstbound = SelectedObjects[0].ObjectOutlineForArrangements;
      var tfirstbound = firstbound.GetBounds(principalMoveVectorsInRootLayerCoordinates);

      var lastbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements;
      var tlastbound = lastbound.GetBounds(principalMoveVectorsInRootLayerCoordinates);

      var step = (tlastbound.YCenter) - (tfirstbound.YCenter);
      step /= (SelectedObjects.Count - 1);

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = SelectedObjects.Count - 2; i > 0; i--)
      {
        IHitTestObject obj = SelectedObjects[i];
        var bounds = obj.ObjectOutlineForArrangements;
        var tbounds = bounds.GetBounds(principalMoveVectorsInRootLayerCoordinates);

        var vu = new VectorD3D(0, tfirstbound.YCenter + i * step - tbounds.YCenter, 0);
        var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
        obj.ShiftPosition(u.X, u.Y, u.Z);
      }
    }

    /// <summary>
    /// Arranges the same size base.
    /// </summary>
    /// <param name="ArrangeAction">The arrange action.</param>
    public void ArrangeSameSizeBase(Action<IHitTestObject, RectangleD3D, RectangleD3D, Matrix3x3> ArrangeAction)
    {
      if (SelectedObjects.Count < 2)
        return;

      var principalMoveVectorsInRootLayerCoordinates = GetCoordinateSystemRlcBasedOnLayerPlaneFacingTheCameraForSelectedObjects();

      var masterbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements;
      var tmasterbound = masterbound.GetBounds(principalMoveVectorsInRootLayerCoordinates); // master bounds in principal move vector coordinates

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = SelectedObjects.Count - 2; i >= 0; i--)
      {
        IHitTestObject obj = SelectedObjects[i];
        var bounds = obj.ObjectOutlineForArrangements;
        var tbounds = bounds.GetBounds(principalMoveVectorsInRootLayerCoordinates); // bounds of the object in principal move vector coordinates

        ArrangeAction(obj, tbounds, tmasterbound, principalMoveVectorsInRootLayerCoordinates);
      }
    }

    public void ArrangeSameHorizontalSize()
    {
      ArrangeSameSizeBase(
        (obj, tbounds, tmasterbounds, principalMoveVectorsInRootLayerCoordinates) =>
        {
          var vu = new VectorD3D(tmasterbounds.SizeX - tbounds.SizeX, 0, 0);
          var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
          obj.ChangeSize(u.X, u.Y, u.Z);
        }
        );
    }

    public void ArrangeSameVerticalSize()
    {
      ArrangeSameSizeBase(
        (obj, tbounds, tmasterbounds, principalMoveVectorsInRootLayerCoordinates) =>
        {
          var vu = new VectorD3D(0, tmasterbounds.SizeY - tbounds.SizeY, 0);
          var u = principalMoveVectorsInRootLayerCoordinates.InverseTransform(vu);
          obj.ChangeSize(u.X, u.Y, u.Z);
        }
        );
    }

    private static IEnumerable<PointD3D> AsPoints(IObjectOutline outline)
    {
      foreach (var line in outline.AsLines)
      {
        yield return line.P0;
        yield return line.P1;
      }
    }

    private RectangleD3D GetBounds(IObjectOutline outline, Matrix3x3 transformation)
    {
      return RectangleD3D.NewRectangleIncludingAllPoints(AsPoints(outline).Select(p => transformation.Transform(p)));
    }

    /// <summary>
    /// Delegate used to move a selected object to a now position in the arrange process.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="transformedBounds">The transformed bounds of the object. These are the bounds of the object in root layer coordinates, transformed further with the principal move vectors.</param>
    /// <param name="transformedMasterbounds">The transformed bounds of the master object (the object to arrange with). These are the bounds of the master object in root layer coordinates, transformed further with the principal move vectors.</param>
    /// <param name="principalMoveVectorsInRootLayerCoordinates">The principal move vectors in root layer coordinates. Will be used to back-transform the move vector to root layer coordinates.</param>
    public delegate void ArrangeElement(IHitTestObject obj, RectangleD3D transformedBounds, RectangleD3D transformedMasterbounds, Matrix3x3 principalMoveVectorsInRootLayerCoordinates);

    /// <summary>
    /// Arranges the objects so they share a common boundary.
    /// </summary>
    /// <param name="arrange">Routine that determines how to arrange the element with respect to the master element.</param>
    public void Arrange(ArrangeElement arrange)
    {
      if (SelectedObjects.Count < 2)
        return;

      var principalMoveVectorsInRootLayerCoordinates = GetCoordinateSystemRlcBasedOnLayerPlaneFacingTheCameraForSelectedObjects();

      var masterbound = SelectedObjects[SelectedObjects.Count - 1].ObjectOutlineForArrangements; // is in root layer coordinates
      var tmasterbound = masterbound.GetBounds(principalMoveVectorsInRootLayerCoordinates); // Rectangle with transformation in principal move vectors

      // now move each object to the new position, which is the difference in the position of the bounds.X
      for (int i = SelectedObjects.Count - 2; i >= 0; i--)
      {
        IHitTestObject o = SelectedObjects[i];
        var bounds = o.ObjectOutlineForArrangements; // in Root layer coordinates
        var tbounds = bounds.GetBounds(principalMoveVectorsInRootLayerCoordinates); // Rectangle with transformation in principal coordinates

        arrange(o, tbounds, tmasterbound, principalMoveVectorsInRootLayerCoordinates);
      }
    }

    #endregion Object arrangement

    #region IClipboardHandler Members

    public bool EnableCut
    {
      get { return true; }
    }

    public bool EnableCopy
    {
      get { return true; }
    }

    public bool EnablePaste
    {
      get { return true; }
    }

    public bool EnableDelete
    {
      get { return true; }
    }

    public bool EnableSelectAll
    {
      get { return false; }
    }

    public void Cut()
    {
      CutSelectedObjectsToClipboard();
    }

    public void Copy()
    {
      CopySelectedObjectsToClipboard();
    }

    public void Paste()
    {
      PasteObjectsFromClipboard();
    }

    public void Delete()
    {
      if (SelectedObjects.Count > 0)
      {
        RemoveSelectedObjects();
      }
      else
      {
        throw new NotImplementedException("Please implement according to next line");
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        //Current.ProjectService.DeleteGraphDocument(_controller.Doc, false);
      }
    }

    public void SelectAll()
    {
    }

    #endregion IClipboardHandler Members
  }
}
