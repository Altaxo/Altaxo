using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Collections;
	using Altaxo.Geometry;
	using Altaxo.Graph.Graph3D;
	using Altaxo.Graph.Graph3D.Camera;
	using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
	using Altaxo.Main;

	public class Graph3DController : IDisposable, IMVCANController
	{
		/// <summary>
		/// Is called each time the name for the content has changed.
		/// </summary>
		public event EventHandler TitleNameChanged;

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

		#region Constructors

		protected Graph3DController()
		{
			InitTriggerBasedUpdate();
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		protected Graph3DController(GraphDocument graphdoc)
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
			/*
			else if (args[0] is GraphViewLayout)
			{
				var o = (GraphViewLayout)args[0];
				_isAutoZoomActive = o.IsAutoZoomActive;
				_zoomFactor = o.ZoomFactor;
				_positionOfViewportsUpperLeftCornerInRootLayerCoordinates = o.PositionOfViewportsUpperLeftCornerInRootLayerCoordinates;
				InternalInitializeGraphDocument(o.GraphDocument);
			}
			*/
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

		private void InitTriggerBasedUpdate()
		{
			_triggerBasedUpdate = new Altaxo.Main.TriggerBasedUpdate(Current.TimerQueue);
			_triggerBasedUpdate.MinimumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(0.02);
			_triggerBasedUpdate.MinimumWaitingTimeAfterLastTrigger = TimeSpanExtensions.FromSecondsAccurate(0.02);
			_triggerBasedUpdate.MaximumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(0.1);
			_triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;
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
				doc.Changed += (_weakEventHandlersForDoc[0] = new WeakEventHandler(this.EhGraph_Changed, x => doc.Changed -= x));
				rootLayer.LayerCollectionChanged += (_weakEventHandlersForDoc[1] = new WeakEventHandler(this.EhGraph_LayerCollectionChanged, x => rootLayer.LayerCollectionChanged -= x));
				doc.SizeChanged += (_weakEventHandlersForDoc[2] = new WeakEventHandler(this.EhGraph_SizeChanged, x => doc.SizeChanged -= x));
			}
			// if the host layer has at least one child, we set the active layer to the first child of the host layer
			if (_doc.RootLayer.Layers.Count >= 1)
				_currentLayerNumber = new List<int>() { 0 };

			// Ensure the current layer and plot numbers are valid
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();

			OnTitleNameChanged(EventArgs.Empty);
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

		public object ModelObject
		{
			get
			{
				return Doc;
			}
		}

		public virtual object ViewObject
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

		public IList<object> SelectedObjects { get; internal set; }

		public void Dispose()
		{
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
			var oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = new List<int>(currLayer);

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

		internal void CutSelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void CopySelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void PasteObjectsFromClipboard()
		{
			throw new NotImplementedException();
		}

		internal void RemoveSelectedObjects()
		{
			throw new NotImplementedException();
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

			var upVector = cameraUpVector.Normalized;
			var targetPosition = (PointD3D)(0.5 * Doc.RootLayer.Size);
			var cameraDistance = 10 * Doc.RootLayer.Size.Length;
			var eyePosition = cameraDistance * toEyeVector.Normalized + targetPosition;

			var newCamera = (CameraBase)Doc.Scene.Camera.Clone();
			newCamera.UpVector = upVector;
			newCamera.TargetPosition = targetPosition;
			newCamera.EyePosition = eyePosition;
			newCamera.ZNear = cameraDistance / 8;
			newCamera.ZFar = cameraDistance * 2;

			var orthoCamera = newCamera as OrthographicCamera;

			if (null != orthoCamera)
			{
				orthoCamera.Scale = 1;

				var mx = orthoCamera.GetLookAtRHTimesOrthoRHMatrix(aspectRatio);
				// to get the resulting scale, we transform all vertices of the root layer (the destination range would be -1..1, but now is not in range -1..1)
				// then we search for the maximum of the absulute value of x and y. This is our scale.
				double absmax = 0;
				foreach (var p in new RectangleD3D(Doc.RootLayer.Position, Doc.RootLayer.Size).Vertices)
				{
					var ps = mx.TransformPoint(p);
					absmax = Math.Max(absmax, Math.Abs(ps.X));
					absmax = Math.Max(absmax, Math.Abs(ps.Y));
				}
				orthoCamera.Scale = absmax;
			}
			else
			{
				throw new NotImplementedException();
			}

			Doc.Scene.Camera = newCamera;
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

		internal void ViewBottom()
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

		public void Export3D()
		{
			double dpiX = 300;
			double dpiY = 300;

			var exporter = new Altaxo.Gui.Graph3D.Common.D3D10BitmapExporter();

			var scene = new Altaxo.Gui.Graph3D.Viewing.D3D10Scene();

			var g = new D3D10GraphicContext();

			Doc.Paint(g);

			var matrix = Doc.Scene.Camera.LookAtRHMatrix;

			var rect = new RectangleD3D(PointD3D.Empty, RootLayer.Size);
			var bounds = RectangleD3D.NewRectangleIncludingAllPoints(rect.Vertices.Select(x => matrix.Transform(x)));

			int pixelsX = (int)(dpiX * bounds.SizeX / 72.0);
			int pixelsY = (int)(dpiY * bounds.SizeY / 72.0);

			double aspectRatio = pixelsY / (double)pixelsX;

			var sceneSettings = (SceneSettings)Doc.Scene.Clone();

			var orthoCamera = sceneSettings.Camera as OrthographicCamera;

			if (null != orthoCamera)
			{
				orthoCamera.Scale = bounds.SizeX;

				double offsX = -(1 + 2 * bounds.X / bounds.SizeX);
				double offsY = -(1 + 2 * bounds.Y / bounds.SizeY);
				orthoCamera.ScreenOffset = new PointD2D(offsX, offsY);
			}
			else
			{
				throw new NotImplementedException();
			}

			scene.SetSceneSettings(sceneSettings);
			scene.SetDrawing(g);

			exporter.Export(pixelsX, pixelsY, scene);
		}

		public void EhMouseWheel(double relX, double relY, double aspectRatio, int delta)
		{
			if (Doc.Scene.Camera is OrthographicCamera)
			{
				var cam = Doc.Scene.Camera as OrthographicCamera;
				var eye = cam.NormalizedEyeVector;
				var up = cam.NormalizedUpVectorPerpendicularToEyeVector;
				var scaleBefore = cam.Scale;
				var scaleAfter = delta < 0 ? scaleBefore / 1.25 : scaleBefore * 1.25;

				var tam1h = relX - 0.5;
				var tbm1h = relY - 0.5;

				var shift = new VectorD3D(
					-(scaleAfter - scaleBefore) * (aspectRatio * tbm1h * up.X + eye.Z * tam1h * up.Y - eye.Y * tam1h * up.Z),
					(scaleAfter - scaleBefore) * (eye.Z * tam1h * up.X - aspectRatio * tbm1h * up.Y - eye.X * tam1h * up.Z),
					-(scaleAfter - scaleBefore) * (eye.Y * tam1h * up.X - eye.X * tam1h * up.Y + aspectRatio * tbm1h * up.Z)
					);

				var newCamera = (OrthographicCamera)Doc.Scene.Camera.Clone();
				newCamera.EyePosition += shift;
				newCamera.TargetPosition += shift;
				newCamera.Scale = scaleAfter;
				Doc.Scene.Camera = newCamera;
			}
		}

		public void EhMoveOrRoll(double stepX, double stepY, bool isControlPressed)
		{
			if (isControlPressed)
				EhRoll(stepX, stepY);
			else
				EhMove(stepX, stepY);
		}

		public void EhRoll(double stepX, double stepY)
		{
			var cam = Doc.Scene.Camera;

			// the axis to turn the camera around is in case of stepY the Cross of UpVector and eyeVector
			// in case of stepX it is the cross of the Upvector and the cross of UpVector and eyeVector

			if (stepX != 0)
			{
				double angleRadian = Math.PI * stepX / 18;
				VectorD3D axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, Doc.Scene.Camera.UpVector)));
				var matrix = MatrixD3D.CreateRotationMatrixFromAxisAndAngleRadian(axis, angleRadian, Doc.Scene.Camera.TargetPosition);

				var newEye = matrix.TransformPoint(Doc.Scene.Camera.EyePosition);
				var newUp = matrix.Transform(Doc.Scene.Camera.UpVector);
				var newCamera = (CameraBase)Doc.Scene.Camera.Clone();
				newCamera.EyePosition = newEye;
				newCamera.UpVector = newUp;
				Doc.Scene.Camera = newCamera;
			}

			if (stepY != 0)
			{
				double angleRadian = Math.PI * stepY / 18;
				VectorD3D axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.NormalizedEyeVector, Doc.Scene.Camera.UpVector));
				var matrix = MatrixD3D.CreateRotationMatrixFromAxisAndAngleRadian(axis, angleRadian, Doc.Scene.Camera.TargetPosition);

				var newEye = matrix.TransformPoint(Doc.Scene.Camera.EyePosition);
				var newUp = matrix.Transform(Doc.Scene.Camera.UpVector);
				var newCamera = (CameraBase)Doc.Scene.Camera.Clone();
				newCamera.EyePosition = newEye;
				newCamera.UpVector = newUp;
				Doc.Scene.Camera = newCamera;
			}
		}

		public void EhMove(double stepX, double stepY)
		{
			VectorD3D xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.NormalizedEyeVector, Doc.Scene.Camera.UpVector));
			VectorD3D yaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, Doc.Scene.Camera.UpVector)));

			if (Doc.Scene.Camera is OrthographicCamera)
			{
				var newCamera = Doc.Scene.Camera.Clone() as OrthographicCamera;

				var shift = (xaxis * stepX + yaxis * stepY) * (newCamera.Scale / 20);

				newCamera.EyePosition += shift;
				newCamera.TargetPosition += shift;

				Doc.Scene.Camera = newCamera;
			}
		}

		/// <summary>
		/// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
		/// </summary>
		/// <param name="relativeScreenPosition">The relative screen coordinates. X and Y are relative screen coordinate values, Z is the screen's aspect ratio.</param>
		/// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
		/// <param name="foundObject">Found object if there is one found, else null</param>
		/// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
		/// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
		public bool FindGraphObjectAtPixelPosition(PointD3D relativeScreenPosition, bool plotItemsOnly, out IHitTestObject foundObject, out int[] foundInLayerNumber)
		{
			var camera = Doc.Scene.Camera;

			MatrixD3D hitmatrix = Doc.Scene.Camera.GetHitRayMatrix(relativeScreenPosition);
			var hitdata = new HitTestPointData(hitmatrix);

			foundObject = Doc.RootLayer.HitTest(hitdata, plotItemsOnly);

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
				Current.Gui.Execute(EhGraphDocumentNameChanged_Unsynchronized, (GraphDocument)sender, eAsNOC.OldName);
				return;
			}

			_triggerBasedUpdate.Trigger();
		}

		private void EhUpdateByTimerQueue()
		{
			// if something changed on the graph, make sure that the layer and plot number reflect this change
			this.EnsureValidityOfCurrentLayerNumber();
			this.EnsureValidityOfCurrentPlotNumber();

			if (null != _view)
			{
				_view.FullRepaint(); // this function is non-Gui thread safe
			}
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
			/*
			if (_view != null)
			{
				if (this._isAutoZoomActive)
					this.RefreshAutoZoom(false);
				_view.InvalidateCachedGraphBitmapAndRepaint();
			}
			*/
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
		}

		/// <summary>
		/// The controller should show a data context menu (contains all plots of the currentLayer).
		/// </summary>
		/// <param name="currLayer">The layer number. The controller has to make this number the CurrentLayerNumber.</param>
		/// <param name="parent">The parent control which is the parent of the context menu.</param>
		/// <param name="pt">The location where the context menu should be shown.</param>
		public virtual void EhView_ShowDataContextMenu(int[] currLayer, object parent, PointD2D pt)
		{
			int oldCurrLayer = this.ActiveLayer.Number;

			this.CurrentLayerNumber = new List<int>(currLayer);

			if (null != this.ActiveLayer)
			{
				Current.Gui.ShowContextMenu(parent, parent, "/Altaxo/Views/Graph/LayerButton/ContextMenu", pt.X, pt.Y);
			}
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

		protected virtual void OnTitleNameChanged(System.EventArgs e)
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this, e);
		}

		#endregion Event handlers from GraphDocument
	}
}